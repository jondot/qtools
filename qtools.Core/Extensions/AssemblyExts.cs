using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace qtools.Core.Extensions
{
    public static class AssemblyExts
    {
        public static string GetResourceText(this System.Reflection.Assembly assy, string resourceName)
        {
            using (var s = new StreamReader(assy.GetManifestResourceStream(resourceName)))
            {
                return s.ReadToEnd();
            }
        }
    }
}
