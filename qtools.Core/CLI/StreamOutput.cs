using System.IO;

namespace qtools.Core.CLI
{
    public class StreamOutput : OutputBase
    {
        private readonly StreamWriter _s;

        public StreamOutput(Stream s)
        {
            _s = new StreamWriter(s);
            _s.AutoFlush = true;
        }

        public override void Error(string msg)
        {
            _s.Write(msg);
        }

        public override void Log(string msg)
        {
            _s.Write(msg);
        }

        public override void Warn(string msg)
        {
            _s.Write(msg);
        }

        public override void Out(string s)
        {
            _s.Write(s);
        }
    }
}