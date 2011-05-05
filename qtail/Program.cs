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

namespace qtail
{
    internal sealed class Options
    {
        private static readonly HeadingInfo _headingInfo = new HeadingInfo("qtail", ((AssemblyFileVersionAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyFileVersionAttribute), false)[0]).Version);

        [Option("n", "name",
                Required = true,
                HelpText = "Queue name")]
        public string Name = String.Empty;

        [HelpOption(HelpText = "display this help screen.")]
        public string GetUsage()
        {
            var help = new HelpText(_headingInfo);

            help.AdditionalNewLineAfterOption = true;
            help.AddPreOptionsLine(Assembly.GetExecutingAssembly().GetResourceText("qtail.usage.txt"));
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
                            new QTail(),
                            new QueueTools(),
                            new ConsoleOutput(),
                            new ConsoleInput()))
            {
                Environment.Exit(1);
            }
        }
    }
}
