using System;
using qtools.Core;
using qtools.Core.CLI;

namespace qcp
{
    class QCp : ICommand
    {
        private readonly Options _opts;

        public QCp(Options opts)
        {
            _opts = opts;
        }

        public bool Execute(string subject, IQueueTools tools, IOutput log)
        {
            try
            {
                int copied = tools.Transfer(subject, _opts.Destination, null, false, false);
                log.Out(subject + " copied to " + _opts.Destination + " ("+ copied +" messages)");
                return true;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                return false;
            }
        }
    }
}
