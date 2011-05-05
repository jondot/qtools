using qtools.Core;
using qtools.Core.CLI;


namespace qcount
{
    class QCount : ICommand
    {
        private readonly Options _opts;

        public QCount(Options opts)
        {
            _opts = opts;
        }

        public bool Execute(string subject, IQueueTools tools, IOutput log)
        {
            bool alreadyExists = tools.Exists(subject);

            if (!alreadyExists)
            {
                log.Error(subject, "Doesn't exist.");
                return false;
            }

            log.OK(subject, string.Format("{0} message(s).", tools.Count(subject)));
            return true;
        }
    }
}
