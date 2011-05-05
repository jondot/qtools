using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace qtools.Core.Extensions
{
    public static class RegexExtensions
    {
        public static string HighlightMatch(this Regex r , string text, string wrapper)
        {
            return r.Replace(text, x => x.Captures.Count == 1 ? string.Format(wrapper, x.Captures[0].Value) : string.Empty);
        }
    }
}
