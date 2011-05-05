using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace qtools.Core.CLI
{
    public interface IInput
    {
        string ReadLine();
    }

    public class ConsoleInput : IInput
    {
        public string ReadLine()
        {
            return Console.ReadLine();
        }
    }
}
