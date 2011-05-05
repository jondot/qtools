namespace qtools.Core.CLI
{
    public interface ICommand
    {
        bool Execute(string subject, IQueueTools tools, IOutput log);
    }
}