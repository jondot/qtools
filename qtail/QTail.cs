using qtools.Core;
using qtools.Core.CLI;

namespace qtail
{
    class QTail : ICommand
    {
        public bool Execute(string subject, IQueueTools tools, IOutput log)
        {
            foreach(var m in tools.Tail(subject))
            {
                log.Out(m.ToString());
            }
            return true;
        }
    }
}
