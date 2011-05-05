using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using CommandLine;
using CommandLine.Text;
using qtools.Core;
using qtools.Core.CLI;
using qtools.Core.Extensions;

namespace qgrep
{
    internal sealed class Options
    {
        private static readonly HeadingInfo _headingInfo = new HeadingInfo("qgrep", ((AssemblyFileVersionAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyFileVersionAttribute), false)[0]).Version);

        [Option("e", "Expression", Required = true, HelpText = "Expression (regular expression)")]
        public string Expression = String.Empty;

        [Option("n", "name",
                Required = false,
                HelpText = "Queue name")]
        public string Name = String.Empty;

        [Option("i", "Insensitive", Required = false, HelpText = "Expression is case insensitive")]
        public bool CaseInsensitive;

        [HelpOption(HelpText = "display this help screen.")]
        public string GetUsage()
        {
            var help = new HelpText(_headingInfo);

            help.AdditionalNewLineAfterOption = true;
            help.AddPreOptionsLine(Assembly.GetExecutingAssembly().GetResourceText("qgrep.usage.txt"));
            help.AddOptions(this);
            return help;
        }
    }

    class Program : BaseProgram
    {
        static void Main(string[] args)
        {
            var options = new Options();

            ICommandLineParser parser = new CommandLineParser(new CommandLineParserSettings(Console.Error));
            if (!parser.ParseArguments(args, options)
                || !Invoke(options.Name,
                            new QGrep(options),
                            new QueueTools(),
                            new ConsoleOutput(),
                            new ConsoleInput()))
            {
                Environment.Exit(1);
            }
        }
    }
}
