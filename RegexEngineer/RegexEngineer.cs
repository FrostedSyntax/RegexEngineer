using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace RegexEngineerLib
{
    /// <summary>
    /// An object used to build a regular expression pattern from a collection of <see cref="RegexFragment"/> objects.
    /// </summary>
    public sealed class RegexEngineer
    {
        private readonly List<RegexFragment> _fragments;

        /// <summary>
        /// All <see cref="RegexFragment"/> objects that make up the pattern."/>
        /// </summary>
        public List<RegexFragment> Fragments => _fragments;

        private RegexEngineer() => _fragments = new List<RegexFragment>();

        /// <summary>
        /// Creates a new instance of <see cref="RegexEngineer"/>.
        /// </summary>
        /// <returns></returns>
        public static RegexEngineer Create() => new RegexEngineer();

        /// <summary>
        /// Adds all specified fragments to the list of fragments.
        /// </summary>
        /// <param name="fragments">A parameterized array of <see cref="RegexFragment"/> objects.</param>
        public void AddFragments(params RegexFragment[] fragments) => _fragments.AddRange(fragments);

        /// <summary>
        /// Creates a new instance of <see cref="RegexFragment"/> representing the specified literal string.
        /// </summary>
        /// <param name="literal">A string to search for literally.</param>
        /// <returns>The newly created <see cref="RegexFragment"/>.</returns>
        public RegexFragment CreateLiteral(string literal)
        {
            return new RegexFragment(literal)
            {
                _options = {
                    FragmentKind = RegexFragmentKind.Literal
                }
            };
        }

        /// <summary>
        /// Creates a new instance of <see cref="RegexFragment"/> representing the specified character escape pattern.
        /// </summary>
        /// <param name="escapeChar">The character portion of the escape pattern.</param>
        /// <returns>The newly created <see cref="RegexFragment"/>.</returns>
        public RegexFragment CreateEscapedChar(EscapedCharacterKind charKind)
        {
            return new RegexFragment($"{GetEscaped(charKind)}")
            {
                _options = {
                    FragmentKind = RegexFragmentKind.CharacterEscape
                }
            };
        }

        /// <summary>
        /// Creates a new instance of <see cref="RegexFragment"/> representing the specified character class pattern.
        /// </summary>
        /// <param name="chars">A string representing the contents of the character class.</param>
        /// <returns>The newly created <see cref="RegexFragment"/>.</returns>
        public RegexFragment CreateCharClass(bool negated, params string[] chars)
        {
            List<string> charsList = chars.Distinct().ToList();

            // Makes sure hyphen is at the end of the list.
            if (charsList.Contains("-"))
            {
                charsList.Remove("-");
                charsList.Add("-");
            }

            return new RegexFragment(chars, negated)
            {
                _options =
                {
                    FragmentKind = RegexFragmentKind.CharacterSet
                }
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public MatchCollection GetMatches(string input) => new Regex(Compile()).Matches(input);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public bool Test(string input) => new Regex(Compile()).IsMatch(input);

        /// <summary>
        /// Returns the full pattern as a string.
        /// </summary>
        /// <returns>A string representing the full Regex pattern.</returns>
        public override string ToString() => Compile();

        // Compiles all fragments into a single string.
        private string Compile() => _fragments.Flatten(f => f.Compile());

        // Returns the escaped character pattern for the specified character kind.
        internal static string GetEscaped(EscapedCharacterKind charKind)
        {
            switch (charKind)
            {
                case EscapedCharacterKind.WordBoundary:
                    return @"\b";
                case EscapedCharacterKind.Tab:
                    return @"\t";
                case EscapedCharacterKind.CarriageReturn:
                    return @"\r";
                case EscapedCharacterKind.NewLine:
                    return @"\n";
                case EscapedCharacterKind.FormFeed:
                    return @"\f";
                case EscapedCharacterKind.VerticalTab:
                    return @"\v";
                case EscapedCharacterKind.Null:
                    return @"\0";
                case EscapedCharacterKind.Digit:
                    return @"\d";
                case EscapedCharacterKind.NonDigit:
                    return @"\D";
                case EscapedCharacterKind.Whitespace:
                    return @"\s";
                case EscapedCharacterKind.NonWhitespace:
                    return @"\S";
                case EscapedCharacterKind.Word:
                    return @"\w";
                case EscapedCharacterKind.NonWord:
                    return @"\W";
                case EscapedCharacterKind.Bell:
                    return @"\a";
                case EscapedCharacterKind.Escape:
                    return @"\e";
                case EscapedCharacterKind.Backslash:
                    return @"\\";
            }

            return string.Empty;
        }
    }
}
