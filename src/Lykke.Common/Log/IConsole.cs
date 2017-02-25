using System;

namespace Common.Log
{
    public interface IConsole
    {
        void WriteLine(string line);
    }

    public class ConsoleLWriter : IConsole
    {
        private readonly Action<string> _writeMethod;

        public ConsoleLWriter(Action<string> writeMethod)
        {
            _writeMethod = writeMethod;
        }

        public void WriteLine(string line)
        {
            _writeMethod(line);
        }
    }
}
