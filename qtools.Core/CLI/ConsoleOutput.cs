using System;

namespace qtools.Core.CLI
{
    public class ConsoleOutput : IOutput
    {
        public void Error(string msg)
        {
            Console.Error.WriteLine(msg);
        }

        public void Log(string msg)
        {
            Console.Out.WriteLine(msg);
        }

        public void Warn(string msg)
        {
            Console.Error.WriteLine(msg);
        }

        public void Warn(string name, string message)
        {
            Warn(string.Format("Warning: [{0}] ", name) + message);
        }

        public void Error(string name, string message)
        {
            Error(string.Format("Error: [{0}] ", name) + message);
        }

        public void OK(string name, string message)
        {
            Log(string.Format("OK: [{0}] ", name) + message);
        }

        public void Out(string s)
        {
            Console.WriteLine(s);
        }

        public void Info(string subject, string message)
        {
            Out(string.Format("INFO: [{0}] ", subject) + message);
        }
    }

    public interface IOutput
    {
        void Error(string msg);
        void Log(string msg);
        void Warn(string msg);
        void Warn(string name, string message);
        void Error(string name, string message);
        void OK(string name, string message);
        void Out(string s);
        void Info(string subject, string msg);
    }
}
