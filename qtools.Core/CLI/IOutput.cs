namespace qtools.Core.CLI
{
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