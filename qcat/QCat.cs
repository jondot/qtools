using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using qtools.Core;
using qtools.Core.CLI;


namespace qcat
{
    class QCat : ICommand
    {
        private readonly Options _opts;

        public QCat(Options opts)
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
            foreach(var res in tools.Cat(subject, _opts.WithExtension))
            {
                log.Out(res.ToString());
            }

            return true;
        }
    }
}
