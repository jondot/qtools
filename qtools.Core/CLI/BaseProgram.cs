using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace qtools.Core.CLI
{
    public class BaseProgram
    {
        // in case action takes a subject (queue)
        protected static bool ExecuteSafely(string subject, IQueueTools tools, IOutput output, Func<string, IQueueTools, IOutput, bool> action)
        {
            try
            {
                return action.Invoke(subject, tools, output);
            }
            catch (Exception e)
            {
                output.Error(subject, e.Message);
                return false;
            }
        }

        
        protected static bool ExecuteSafelyForEachInput(IInput input, IQueueTools tools, IOutput output, Func<string, IQueueTools, IOutput, bool> f)
        {
            string line = string.Empty;
            bool res = false;

            while(!string.IsNullOrEmpty(line = input.ReadLine()))
            {
                var exres = ExecuteSafely(line, tools, output, f);
                res = res || exres;
            }

            return res ;
        }

        protected static bool Invoke(string initialSubject, ICommand q, IQueueTools tools, IOutput output, IInput input)
        {
            if (string.IsNullOrEmpty(initialSubject))
            {
                return ExecuteSafelyForEachInput(input, tools, output, q.Execute);
            }

            return ExecuteSafely(initialSubject, tools, output, (Func<string, IQueueTools, IOutput, bool>) q.Execute);

        }
    }
}
