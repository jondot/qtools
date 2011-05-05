using qtools.Core;
using qtools.Core.CLI;

namespace qrm
{
    class QRm : ICommand
    {
        private readonly Options _opts;

        public QRm(Options opts)
        {
            _opts = opts;
        }

        public bool Execute(string subject, IQueueTools tools, IOutput log)
        {
            bool alreadyExists = tools.Exists(subject);

            if (!alreadyExists)
            {
                log.Warn(subject, "Does not exist.");
                return true;
            }

            tools.Delete(subject);
            log.OK(subject, "Deleted.");

            return true;
        }
    }
}
