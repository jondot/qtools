using System.Collections.Generic;
using System.IO;

namespace qtools.Core.CLI
{
    public class JsonFileReporter : IReporter
    {
        private readonly string _filename;
        private readonly JsonReporter _jsonReporter;

        public JsonFileReporter(IEnumerable<QueueDescriptor> qs, string filename)
        {
            _filename = filename;
            _jsonReporter = new JsonReporter(qs);
        }

        public void Report(IOutput output)
        {
            using(var fs = File.Create(_filename))
            {
                _jsonReporter.Report(new StreamOutput(fs));
            }
            output.Log(string.Format("Wrote {0}.", _filename));
        }
    }
}