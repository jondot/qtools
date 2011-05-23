using System;
using System.Reflection;
using CommandLine;
using CommandLine.Text;
using qtools.Core;
using qtools.Core.CLI;
using qtools.Core.Extensions;

namespace qls
{
    internal sealed class Options
    {
        private static readonly HeadingInfo _headingInfo = new HeadingInfo("qtouch", ((AssemblyFileVersionAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyFileVersionAttribute), false)[0]).Version);

        [Option("p", "public",
            Required = false,
            HelpText = "List public queues")] 
        public bool Public;

        [Option("f", "Filter", Required = false, HelpText = "Path filter expression (regular expression)")]
        public string Filter = string.Empty;

        [Option("m", "Machine", Required = false, HelpText = "Machine, default is local (.)")]
        public string Machine = string.Empty;

        [Option("t", "Transactional", Required = false, HelpText = "Take only transactional queues")]
        public bool Transactional;

        [Option("n", "Non-transactional", Required = false, HelpText = "Take only non transactional queues")]
        public bool NonTransactional;



        [HelpOption(HelpText = "display this help screen.")]
        public string GetUsage()
        {
            var help = new HelpText(_headingInfo);

            help.AdditionalNewLineAfterOption = true;
            help.AddPreOptionsLine(Assembly.GetExecutingAssembly().GetResourceText("qls.usage.txt"));
            help.AddOptions(this);
            return help;
        }
    }
    class Program : BaseProgram
    {
        static void Main(string[] args)
        {
            var options = new Options();
            if(string.IsNullOrEmpty(options.Machine))
            {
                options.Machine = ".";
            }

            ICommandLineParser parser = new CommandLineParser(new CommandLineParserSettings(Console.Error));
            if (!parser.ParseArguments(args, options))
                Environment.Exit(1);

            if (!Invoke("ls", new QLs(options), new QueueTools(), new ConsoleOutput(), new ConsoleInput()))
                Environment.Exit(1);
        }
    }
}
