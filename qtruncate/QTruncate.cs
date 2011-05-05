using qtools.Core;
using qtools.Core.CLI;

namespace qtruncate
{
    class QTruncate : ICommand
    {
        private readonly Options _opts;

        public QTruncate(Options opts)
        {
            _opts = opts;
        }

        public bool Execute(string subject, IQueueTools tools, IOutput log)
        {
            bool alreadyExists = tools.Exists(subject);

            if (!alreadyExists)
            {
                log.Warn(subject, "Doesn't exist, nothing to truncate.");
                return false;
            }

            tools.DeleteAllMessages(subject);
            log.OK(subject, "Truncated.");

            return true;
        }
    }
}
