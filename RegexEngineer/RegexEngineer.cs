using System.Collections.Generic;
using System.Linq;

namespace RegexEngineer
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class RegexEngineer
    {
        private readonly List<RegexFragment> _fragments;

        /// <summary>
        /// 
        /// </summary>
        public List<RegexFragment> Fragments => _fragments;

        private RegexEngineer() => _fragments = new List<RegexFragment>();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static RegexEngineer Create() => new RegexEngineer();

        /// <summary>
        /// 
        /// </summary>
        public string Compile() => _fragments.Flatten(f => f.Compile());

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fragments"></param>
        public void AddFragments(params RegexFragment[] fragments) => _fragments.AddRange(fragments);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="literal"></param>
        /// <returns></returns>
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
        /// 
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public RegexFragment CreateCharEscape(char c)
        {
            return new RegexFragment($"\\{c}")
            {
                _options = {
                    FragmentKind = RegexFragmentKind.CharacterEscape
                }
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chars"></param>
        /// <returns></returns>
        public RegexFragment CreateCharClass(params char[] chars)
        {
            List<char> charsList = chars.Distinct().ToList();

            // Makes sure hyphen is at the end of the list.
            if (charsList.Contains('-'))
            {
                charsList.Remove('-');
                charsList.Add('-');
            }

            return CreateCharClass(charsList.Flatten());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chars"></param>
        /// <returns></returns>
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
        /// 
        /// </summary>
        /// <param name="chars"></param>
        /// <returns></returns>
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
        /// 
        /// </summary>
        /// <param name="chars"></param>
        /// <returns></returns>
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
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString() => Compile();
    }
}
