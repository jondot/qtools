namespace qtools.Core.CLI
{
    public abstract class OutputBase : IOutput
    {
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

        public void Info(string subject, string message)
        {
            Out(string.Format("INFO: [{0}] ", subject) + message);
        }

        public abstract void Error(string msg);

        public abstract void Log(string msg);

        public abstract void Warn(string msg);

        public abstract void Out(string s);

    }
}