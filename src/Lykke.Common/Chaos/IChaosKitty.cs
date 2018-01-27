using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Lykke.Common.Chaos
{
    /// <summary>
    /// <p>
    /// Chaos kitty abstraction. It's inteded to verify your application
    /// reliability and methods idempotency on the dev and test environments, 
    /// by periodicaly generating random exceptions. 
    /// </p>
    /// <p>
    /// Most suitable cases for the chaos kitty are message handlers, that executes
    /// with retries. Chaos kitty is good tool to test idempotency of these handlers.
    /// But it's also can be used in the HTTP API actions or even business services 
    /// and repositories. Call <see cref="Meow"/> after every state changing method 
    /// call to emulate infrastructure failure. Consistency of overall state of your 
    /// application should be eventually achivable even after such exceptions. If your 
    /// application recovers after the chaos kitty meowing, then it will be able to 
    /// recover after any transient infrastructure failures in production.
    /// </p>
    /// </summary>
    [PublicAPI]
    public interface IChaosKitty
    {
        /// <summary>
        /// Let the kitty think to do meow or not this time.
        /// </summary>
        /// <param name="tag">
        ///     An object that will be converted to the string by the <see cref="object.ToString"/> call and will be 
        ///     passed to the message of the generated exception, if kitty is meowed. Passing ID of the executed
        ///     process, or another context, will be good idea, to reconstruct the state of the application by the logs,
        ///     when the "meow" is lead to the inconsistent state of the app and you searching how to fix this issue.
        /// </param>
        /// <param name="lineNumber">
        /// Line number, where <see cref="Meow"/> was called. Will be substituted automatically
        /// </param>
        /// <exception cref="ChaosException">
        /// If the kitty is meowed, then this exception will be thrown
        /// </exception>
        void Meow(object tag, [CallerLineNumber] int lineNumber = 0);
    }
}
