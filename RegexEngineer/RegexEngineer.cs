using System.Collections.Generic;
using System.Linq;

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
        public RegexFragment CreateCharEscape(char escapeChar)
        {
            return new RegexFragment($"\\{escapeChar}")
            {
                _options = {
                    FragmentKind = RegexFragmentKind.CharacterEscape
                }
            };
        }

        /// <summary>
        /// Creates a new instance of <see cref="RegexFragment"/> representing the specified negated character class pattern.
        /// </summary>
        /// <param name="chars">A string representing the contents of the character class.</param>
        /// <returns>The newly created <see cref="RegexFragment"/>.</returns>
        public RegexFragment CreateCharClass(string chars)
        {
            return new RegexFragment(chars)
            {
                _options =
                {
                    FragmentKind = RegexFragmentKind.CharacterSet
                }
            };
        }

        /// <summary>
        /// Creates a new instance of <see cref="RegexFragment"/> representing the specified negated character class pattern.
        /// </summary>
        /// <param name="chars">A parameterized list of characters.</param>
        /// <returns>The newly created <see cref="RegexFragment"/>.</returns>
        public RegexFragment CreateNegatedCharClass(params char[] chars)
        {
            List<char> charsList = chars.Distinct().ToList();

            // Makes sure hyphen is at the end of the list.
            if (charsList.Contains('-'))
            {
                charsList.Remove('-');
                charsList.Add('-');
            }

            return CreateNegatedCharClass(charsList.Flatten(c => c.ToString()));
        }

        /// <summary>
        /// Creates a new instance of <see cref="RegexFragment"/> representing the specified negated character class pattern.
        /// </summary>
        /// <param name="chars">A string representing the contents of the negated character class.</param>
        /// <returns>The newly created <see cref="RegexFragment"/>.</returns>
        public RegexFragment CreateNegatedCharClass(string chars)
        {
            return new RegexFragment("^" + chars)
            {
                _options =
                {
                    FragmentKind = RegexFragmentKind.CharacterSet
                }
            };
        }

        /// <summary>
        /// Returns the full pattern as a string.
        /// </summary>
        /// <returns>A string representing the full Regex pattern.</returns>
        public override string ToString() => Compile();

        // Compiles all fragments into a single string.
        private string Compile() => _fragments.Flatten(f => f.Compile());
    }
}
