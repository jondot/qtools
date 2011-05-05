using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using qtools.Core;
using qtools.Core.CLI;


namespace qgrep
{
    class QGrep : ICommand
    {
        private readonly Options _opts;

        public QGrep(Options opts)
        {
            _opts = opts;
        }

        public bool Execute(string subject, IQueueTools tools, IOutput log)
        {
            bool alreadyExists = tools.Exists(subject);

            if (!alreadyExists)
            {
                log.Warn(subject, "Doesn't exist. Skipping.");
                return false;
            }

            log.Info(subject, "Listing results.");
            foreach(var res in tools.Grep(subject, _opts.Expression, _opts.CaseInsensitive))
            {
                log.Out(res.ToString());
            }

            return true;
        }
    }
}
