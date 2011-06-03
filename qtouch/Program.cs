using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Web.Script.Serialization;
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

        [Option("d", "Dump", Required = false, HelpText = "Dump queue descriptions into configuration format")]
        public string DumpFile;


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

            var consoleOutput = new ConsoleOutput();
            var queueTools = new QueueTools();


            if(!string.IsNullOrEmpty(options.DumpFile))
            {
                if(File.Exists(options.DumpFile))
                {
                    ImportFromDumpFile(options, queueTools, consoleOutput);
                    consoleOutput.Info(options.DumpFile, "Imported.");
                }
                else
                {
                    consoleOutput.Error(options.DumpFile, "Does not exist.");
                    Environment.Exit(1);
                }
            }
            else if (!Invoke(options.Name, new QTouch(options), queueTools, consoleOutput, new ConsoleInput()))
                Environment.Exit(1);
        }

        private static void ImportFromDumpFile(Options options, QueueTools queueTools, ConsoleOutput consoleOutput)
        {
            var s = new JavaScriptSerializer();
            var deserialize = s.Deserialize<IEnumerable<QueueDescriptor>>(File.ReadAllText(options.DumpFile));
            foreach (var queueDescriptor in deserialize)
            {
                options.Limit = (int)queueDescriptor.Limit;
                options.Transactional = queueDescriptor.Transactional;

                Invoke(queueDescriptor.Name, new QTouch(options), queueTools, consoleOutput, new ConsoleInput());
            }
        }
    }
}
