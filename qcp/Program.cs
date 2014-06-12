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

namespace qcp
{
    internal sealed class Options
    {
        private static readonly HeadingInfo _headingInfo = new HeadingInfo("qcp", ((AssemblyFileVersionAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyFileVersionAttribute), false)[0]).Version);

        [Option("n", "source-name",
                Required = true,
                HelpText = "Queue source name")]
        public string Source = String.Empty;

        [Option("d", "destination-name",
          Required = true,
          HelpText = "Queue destination name")]
        public string Destination = String.Empty;

        [HelpOption(HelpText = "display this help screen.")]
        public string GetUsage()
        {
            var help = new HelpText(_headingInfo);

            help.AdditionalNewLineAfterOption = true;
            help.AddPreOptionsLine(Assembly.GetExecutingAssembly().GetResourceText("qcp.usage.txt"));
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
                || !Invoke(options.Source,
                            new QCp(options),
                            new QueueTools(),
                            new ConsoleOutput(),
                            new ConsoleInput()))
            {
                Environment.Exit(1);
            }
        }
    }
}
