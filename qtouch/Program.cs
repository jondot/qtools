using System;
using System.Reflection;
using CommandLine;
using CommandLine.Text;
using qtools.Core;
using qtools.Core.CLI;
using qtools.Core.Extensions;

namespace qtouch
{
    internal sealed class Options
    {
        private static readonly HeadingInfo _headingInfo = new HeadingInfo("qtouch", ((AssemblyFileVersionAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyFileVersionAttribute), false)[0]).Version);

        [Option("n", "name",
                Required = false,
                HelpText = "Queue name")]
        public string Name = String.Empty;

        [Option("u", "user", Required = false, HelpText = "User (e.g. Everyone)")]
        public string User = String.Empty;

        [Option("p", "permissions", Required = false, HelpText = "Permissions (e.g. FullControl)")]
        public string Permissions = String.Empty;

        [Option("t", "transactional", Required = false, HelpText = "Transactional queue")]
        public bool Transactional = false;

        [Option("f", "force creation", Required = false, HelpText = "Force creation (delete existing and recreate)")]
        public bool Force = false;

        [Option("l", "limit (KB)", Required = false, HelpText = "Set queue limit in KB")]
        public int Limit = 0;

        [HelpOption(HelpText = "display this help screen.")]
        public string GetUsage()
        {
            var help = new HelpText(_headingInfo);

            help.AdditionalNewLineAfterOption = true;
            help.AddPreOptionsLine(Assembly.GetExecutingAssembly().GetResourceText("qtouch.usage.txt"));
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
            if (!parser.ParseArguments(args, options))
                Environment.Exit(1);

            if (!Invoke(options.Name, new QTouch(options), new QueueTools(), new ConsoleOutput(), new ConsoleInput()))
                Environment.Exit(1);
        }
    }
}
