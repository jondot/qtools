using System;

namespace qtools.Core.CLI
{
    public class ConsoleOutput : OutputBase
    {
        public override void Error(string msg)
        {
            Console.Error.WriteLine(msg);
        }

        public override void Log(string msg)
        {
            Console.Out.WriteLine(msg);
        }

        public override void Warn(string msg)
        {
            Console.Error.WriteLine(msg);
        }

        public override void Out(string s)
        {
            Console.WriteLine(s);
        }
    }
}
