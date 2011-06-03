using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace qtools.Core.CLI
{
    class JsonReporter : IReporter
    {
        private readonly IEnumerable<QueueDescriptor> _qs;

        public JsonReporter(IEnumerable<QueueDescriptor> qs)
        {
            _qs = qs;
        }

        public void Report(IOutput output)
        {
            JavaScriptSerializer s = new JavaScriptSerializer();
            
            string serialize = s.Serialize(_qs);
            output.Out(serialize);
        }
    }
}
