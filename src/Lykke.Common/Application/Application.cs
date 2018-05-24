using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Loader;
using System.Threading;
using System.Threading.Tasks;

using Autofac;
using Common.Abstractions;

namespace Common.Application
{
    /// <summary>
    /// Manages application state.
    /// </summary>
    public sealed class Application
    {
        private readonly ContainerBuilder _builder = new ContainerBuilder();
        private IContainer _container;
        private ILifetimeScope _scope;
        private static readonly ManualResetEventSlim _done = new ManualResetEventSlim(false);
        private static readonly object _sync = new object();
        private AppState _state = AppState.Undefined;

        /// <summary>
        /// Autofac container builder.
        /// </summary>
        public ContainerBuilder ContainerBuilder
        {
            get { return _builder; }
        }

        /// <summary>
        /// Build dependency container and call all IStartable.
        /// </summary>
        /// <returns>New scope.</returns>
        public ILifetimeScope Start()
        {
            if (_state >= AppState.Started)
            {
                throw new InvalidOperationException("Application is already started.");
            }

            this._container = _builder.Build();
            this._scope = _container.BeginLifetimeScope();
            _state = AppState.Started;
            return this._scope;
        }

        /// <summary>
        /// Blocks thread until termincation thread is received.
        /// </summary>
        public void Run()
        {
            if (_state >= AppState.Running)
            {
                throw new InvalidOperationException("Application is already running.");
            }

            // Subscribe to events and wait
            //
            AssemblyLoadContext.Default.Unloading += context => this.OnShutdown();
            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                OnShutdown();
                // Don't terminate the process immediately, wait for the Main thread to exit gracefully.
                eventArgs.Cancel = true;
            };
            // Wait until all components stopped and released.
            _done.Wait();
        }

        /// <summary>
        /// Termination event handler
        /// </summary>
        private void OnShutdown()
        {
            // Check state
            lock (_sync)
            {
                if (_state >= AppState.Shutdown) { return; }
                else { _state = AppState.Shutdown; }
            }

            // Call all registered interfaces
            //
            this.Stop();
            this.Persist();
            this.DisposeAll();

            // Release scope
            _scope.Dispose();

            // Allow to exit application
            _done.Set();
        }

        /// <summary>
        /// Call all IStoppable.
        /// </summary>
        private void Stop()
        {
            var startables = _scope.Resolve<IEnumerable<IStopable>>();
            foreach (var startable in startables)
            {
                startable.Stop();
            }
        }

        /// <summary>
        /// Call all IPersitent.
        /// </summary>
        private void Persist()
        {
            var startables = _scope.Resolve<IEnumerable<IPersistent>>();
            List<Task> tasks = new List<Task>();
            foreach (var startable in startables)
            {
                tasks.Add(startable.Save());
            }
            Task.WaitAll(tasks.ToArray());
        }

        /// <summary>
        /// Call all IDisposable.
        /// </summary>
        private void DisposeAll()
        {
            var startables = _scope.Resolve<IEnumerable<IDisposable>>();
            foreach (var startable in startables)
            {
                startable.Dispose();
            }
        }

        #region "Singleton implementation"
        private static volatile Application _instance;

        private Application() { }

        public static Application Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_sync)
                    {
                        if (_instance == null)
                            _instance = new Application();
                    }
                }

                return _instance;
            }
        }
        #endregion
    }
}
