using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegexEngineerLib
{
    /// <summary>
    /// Parses regular expression patterns into a sequence of <see cref="PatternComponent"/> objects
    /// representing syntactic elements.
    /// </summary>
    public static class RegexParser
    {
        /// <summary>
        /// Parses the specified regular expression <paramref name="pattern"/> and returns the
        /// list of components discovered in the pattern in sequential order.
        /// </summary>
        /// <param name="pattern">The regular expression pattern to parse.</param>
        /// <returns>
        /// A list of <see cref="PatternComponent"/> instances describing each element of the pattern.
        /// </returns>
        public static List<PatternComponent> Parse(string pattern)
        {
            var components = new List<PatternComponent>();
            int pos = 0;

            while (pos < pattern.Length)
            {
                char c = pattern[pos];

                switch (c)
                {
                    case '^':
                        components.Add(new PatternComponent
                        {
                            Type = "Anchor",
                            Value = "^",
                            Description = "Start of string/line",
                            Position = pos
                        });
                        pos++;
                        break;

                    case '$':
                        components.Add(new PatternComponent
                        {
                            Type = "Anchor",
                            Value = "$",
                            Description = "End of string/line",
                            Position = pos
                        });
                        pos++;
                        break;

                    case '.':
                        components.Add(new PatternComponent
                        {
                            Type = "Wildcard",
                            Value = ".",
                            Description = "Any character except newline",
                            Position = pos
                        });
                        pos++;
                        break;

                    case '\\':
                        var escape = ParseEscapeSequence(pattern, pos);
                        components.Add(escape);
                        pos += escape.Value.Length;
                        break;

                    case '[':
                        var charClass = ParseCharacterClass(pattern, pos);
                        components.Add(charClass);
                        pos += charClass.Value.Length;
                        break;

                    case '(':
                        var group = ParseGroup(pattern, pos);
                        components.Add(group);
                        pos += group.Value.Length;
                        break;

                    case '*':
                    case '+':
                    case '?':
                        components.Add(new PatternComponent
                        {
                            Type = "Quantifier",
                            Value = c.ToString(),
                            Description = GetQuantifierDescription(c),
                            Position = pos
                        });
                        pos++;
                        break;

                    case '{':
                        var quantifier = ParseBracedQuantifier(pattern, pos);
                        components.Add(quantifier);
                        pos += quantifier.Value.Length;
                        break;

                    case '|':
                        components.Add(new PatternComponent
                        {
                            Type = "Alternation",
                            Value = "|",
                            Description = "OR operator",
                            Position = pos
                        });
                        pos++;
                        break;

                    default:
                        components.Add(new PatternComponent
                        {
                            Type = "Literal",
                            Value = c.ToString(),
                            Description = $"Literal character '{c}'",
                            Position = pos
                        });
                        pos++;
                        break;
                }
            }

            return components;
        }

        private static PatternComponent ParseEscapeSequence(string pattern, int start)
        {
            if (start + 1 >= pattern.Length)
                return new PatternComponent { Type = "Literal", Value = "\\", Description = "Backslash", Position = start };

            char next = pattern[start + 1];
            string value = "\\" + next;
            string desc = next switch
            {
                'd' => "Any digit (0-9)",
                'D' => "Any non-digit",
                'w' => "Any word character (a-z, A-Z, 0-9, _)",
                'W' => "Any non-word character",
                's' => "Any whitespace character",
                'S' => "Any non-whitespace character",
                'b' => "Word boundary",
                'B' => "Non-word boundary",
                'n' => "Newline",
                't' => "Tab",
                'r' => "Carriage return",
                _ => $"Escaped character '{next}'"
            };

            return new PatternComponent
            {
                Type = "Escape",
                Value = value,
                Description = desc,
                Position = start
            };
        }

        private static PatternComponent ParseCharacterClass(string pattern, int start)
        {
            int end = pattern.IndexOf(']', start + 1);
            if (end == -1)
                end = pattern.Length;

            string value = pattern.Substring(start, end - start + 1);
            bool negated = value.StartsWith("[^");
            string desc = negated ? "Negated character class: NOT " : "Character class: ";
            desc += value.Substring(negated ? 2 : 1, value.Length - (negated ? 3 : 2));

            return new PatternComponent
            {
                Type = "CharacterClass",
                Value = value,
                Description = desc,
                Position = start
            };
        }

        private static PatternComponent ParseGroup(string pattern, int start)
        {
            int depth = 1;
            int pos = start + 1;

            while (pos < pattern.Length && depth > 0)
            {
                if (pattern[pos] == '\\')
                {
                    pos += 2;
                    continue;
                }
                if (pattern[pos] == '(')
                    depth++;
                if (pattern[pos] == ')')
                    depth--;
                pos++;
            }

            string value = pattern.Substring(start, pos - start);
            string type = "Group";
            string desc = "Capturing group";

            if (value.StartsWith("(?="))
            {
                type = "Lookahead";
                desc = "Positive lookahead";
            }
            else if (value.StartsWith("(?!"))
            {
                type = "Lookahead";
                desc = "Negative lookahead";
            }
            else if (value.StartsWith("(?:"))
            {
                type = "Group";
                desc = "Non-capturing group";
            }

            return new PatternComponent
            {
                Type = type,
                Value = value,
                Description = desc,
                Position = start
            };
        }

        private static PatternComponent ParseBracedQuantifier(string pattern, int start)
        {
            int end = pattern.IndexOf('}', start);
            if (end == -1)
            {
                end = pattern.Length - 1;
            }

            string value = pattern.Substring(start, end - start + 1);
            string desc = $"Quantifier: {value.Substring(1, value.Length - 2)} occurrences";

            return new PatternComponent
            {
                Type = "Quantifier",
                Value = value,
                Description = desc,
                Position = start
            };
        }

        private static string GetQuantifierDescription(char q)
        {
            return q switch
            {
                '*' => "Zero or more times",
                '+' => "One or more times",
                '?' => "Zero or one time (optional)",
                _ => "Unknown quantifier"
            };
        }

        /// <summary>
        /// Writes a formatted table of the given <paramref name="components"/> to the console.
        /// </summary>
        /// <param name="components">The list of <see cref="PatternComponent"/> instances to print.</param>
        public static void PrintComponents(List<PatternComponent> components)
        {
            Console.WriteLine($"{"Position",-10}{"Type",-20}{"Value",-20}Description");
            Console.WriteLine(new string('-', 80));

            foreach (var comp in components)
            {
                Console.WriteLine($"{comp.Position,-10}{comp.Type,-20}{comp.Value,-20}{comp.Description}");
            }
        }
    }
}
