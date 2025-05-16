using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegexEngineerLib
{
    internal static class PatternUtilities
    {
        private static readonly char[] Operators = new char[] { '+', '*', '?', '|', '(', ')', '[', ']', '{', '}', '\\', '^', '$' };

        public static string EscapeLiteral(string literal)
        {
            string toReturn = string.Empty;

            foreach (char c in literal)
            {
                if (Operators.Contains(c))
                {
                    toReturn += "\\";
                }
                toReturn += c;
            }

            return toReturn;
        }
    }
}
