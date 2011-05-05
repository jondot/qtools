using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using qtools.Core;
using qtools.Core.CLI;

namespace qtouch
{
    class QTouch : ICommand
    {
        private readonly Options _opts;

        public QTouch(Options opts)
        {
            _opts = opts;
        }

        public bool Execute(string subject, IQueueTools tools, IOutput log)
        {
            bool alreadyExists = tools.Exists(subject);

            if (alreadyExists && !_opts.Force)
            {
                log.Error(subject, "Already exists. Issue -f to force creation.");
                return false;
            }

            if (alreadyExists && _opts.Force)
            {
                tools.Delete(subject);
                log.Warn(subject, "Deleted (force).");
            }

            tools.Create(subject, _opts.User, _opts.Permissions, _opts.Transactional,
                        _opts.Limit);

            log.OK(subject, "Created.");
            return true;
        }
    }
}
