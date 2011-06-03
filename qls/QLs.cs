using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using qtools.Core;
using qtools.Core.CLI;

namespace qls
{
    class QLs : ICommand
    {
        private readonly Options _options;

        public QLs(Options options)
        {
            _options = options;
        }

        public bool Execute(string subject, IQueueTools tools, IOutput log)
        {
            IEnumerable<QueueDescriptor> queueDescriptors;
            if(_options.Public)
            {
                queueDescriptors = tools.GetPublicQueuesByMachine(_options.Machine, TransactionFromFlags(_options.Transactional, _options.NonTransactional));
            }
            else
            {
                queueDescriptors = tools.GetPrivateQueues(_options.Machine, TransactionFromFlags(_options.Transactional, _options.NonTransactional));
            }

            if(!string.IsNullOrEmpty(_options.Filter))
            {
                queueDescriptors = Filter(_options.Filter, queueDescriptors);
            }

            IReporter r;

            if(!string.IsNullOrEmpty(_options.DumpFile))
            {
                r = new JsonFileReporter(queueDescriptors, _options.DumpFile);
            }
            else
            {
                r = new LineReporter(queueDescriptors);
            }
            
            r.Report(log);
            return true;
        }


        private static IEnumerable<QueueDescriptor> Filter(string filter, IEnumerable<QueueDescriptor> queueDescriptors)
        {
            var r = new Regex(filter, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            return queueDescriptors.Where(x => r.IsMatch(x.Name));
        }

        private static QueueTransaction TransactionFromFlags(bool xactional, bool nonxactional)
        {
            return xactional ? QueueTransaction.Transactional : (nonxactional ? QueueTransaction.NonTransactional : QueueTransaction.Ignore);
        }
    }
}
