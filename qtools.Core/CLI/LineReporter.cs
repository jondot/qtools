using System.Collections.Generic;

namespace qtools.Core.CLI
{
    public class LineReporter : IReporter
    {
        private readonly IEnumerable<QueueDescriptor> _qs;

        public LineReporter(IEnumerable<QueueDescriptor> qs)
        {
            _qs = qs;
        }

        public void Report(IOutput output)
        {
            foreach (var q in _qs)
            {
                output.Out(q.ToString());
            }
        }
    }
}