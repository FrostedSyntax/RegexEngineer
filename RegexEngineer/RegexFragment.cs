using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;

namespace RegexEngineerLib
{
    /// <summary>
    /// An object representing a part of a regular expression pattern.
    /// </summary>
    public sealed class RegexFragment
    {
        internal Guid _id;
        internal RegexFragmentOptions _options;
        internal string _basePattern = null;

        private RegexFragment _parent;
        private readonly List<RegexFragment> _contents;
        private readonly List<RegexFragment> _modifiers;

        /// <summary>
        /// The unique identifier for this fragment.
        /// </summary>
        public Guid Id => _id;

        /// <summary>
        /// The containing fragment, if any.
        /// </summary>
        public RegexFragment Parent => _parent;

        /// <summary>
        /// The contained fragments, if any.
        /// </summary>
        public IEnumerable<RegexFragment> Contents => _contents;

        /// <summary>
        /// The modifiers for this fragment, if any.
        /// </summary>
        public IEnumerable<RegexFragment> Modifiers => _modifiers;

        private RegexFragment()
        {
            _id = Guid.NewGuid();
            _contents = new List<RegexFragment>();
            _modifiers = new List<RegexFragment>();
            _options = new RegexFragmentOptions();
        }

        private RegexFragment(params RegexFragment[] innerFragments) : this() => _contents.AddRange(innerFragments);

        internal RegexFragment(string literal) : this()
        {
            _options.FragmentKind = RegexFragmentKind.Literal;
            if (string.IsNullOrEmpty(literal) == false)
            {
                _basePattern = literal;
            }
        }

        #region Group Methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public RegexFragment Group(params RegexFragment[] fragments)
        {
            List<RegexFragment> contents = fragments.Prepend(this).ToList();

            var toReturn = new RegexFragment(contents.ToArray())
            {
                _options = new RegexFragmentOptions
                {
                    FragmentKind = RegexFragmentKind.Group
                }
            };

            contents.ForEach(f => f._parent = toReturn);

            return toReturn;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public RegexFragment CaptureGroup(params RegexFragment[] fragments)
        {
            List<RegexFragment> contents = fragments.Prepend(this).ToList();

            var toReturn = new RegexFragment(contents.ToArray())
            {
                _options = new RegexFragmentOptions
                {
                    FragmentKind = RegexFragmentKind.CaptureGroup
                }
            };

            contents.ForEach(f => f._parent = toReturn);

            return toReturn;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public RegexFragment NamedCaptureGroup(string name)
        {
            var toReturn = new RegexFragment(this)
            {
                _options = new RegexFragmentOptions
                {
                    FragmentKind = RegexFragmentKind.NamedCaptureGroup,
                    GroupName = name
                }
            };

            _parent = toReturn;

            return toReturn;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public RegexFragment LookaroundGroup(LookaroundType type)
        {
            var toReturn = new RegexFragment(this)
            {
                _options = new RegexFragmentOptions
                {
                    FragmentKind = RegexFragmentKind.LookaroundGroup,
                    LookaroundType = type
                }
            };

            _parent = toReturn;
            return toReturn;
        }

        #endregion

        #region Modifier Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <returns>This fragment with the added modifier.</returns>
        public RegexFragment Repeat(int count)
        {
            _modifiers.Add(new RegexFragment(count.ToString())
            {
                _options =
                {
                    FragmentKind = RegexFragmentKind.ExactRepeatModifier,
                    RepeatCount = count
                }
            });

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="minimum"></param>
        /// <param name="maximum">The most times to match the pattern. 0 for unlimited times.</param>
        /// <returns>This fragment with the added modifier.</returns>
        public RegexFragment Repeat(int minimum, int maximum)
        {
            _modifiers.Add(new RegexFragment(minimum.ToString() + ", " + (maximum > 0 ? maximum.ToString() : ""))
            {
                _options =
                {
                    FragmentKind = RegexFragmentKind.RepeatRangeModifier,
                    RepetitionMinimum = minimum,
                    RepetitionMaximum = maximum
                }
            });

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>This fragment with the added modifier.</returns>
        public RegexFragment Optional()
        {
            _modifiers.Add(new RegexFragment("?")
            {
                _options = {
                    FragmentKind = RegexFragmentKind.Operator,
                    Position = AnchorPosition.Trailing
                }
            });

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public RegexFragment OneOrMore()
        {
            _modifiers.Add(new RegexFragment("+")
            {
                _options = {
                    FragmentKind = RegexFragmentKind.Operator,
                    Position = AnchorPosition.Trailing
                }
            });

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public RegexFragment ZeroOrMore()
        {
            _modifiers.Add(new RegexFragment("*")
            {
                _options = {
                    FragmentKind = RegexFragmentKind.Operator,
                    Position = AnchorPosition.Trailing
                }
            });

            return this;
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fragments"></param>
        /// <returns></returns>
        public RegexFragment Combine(params RegexFragment[] fragments)
        {
            List<RegexFragment> contents = fragments.Prepend(this).ToList();

            var toReturn = new RegexFragment(contents.ToArray())
            {
                _options =
                {
                    FragmentKind = RegexFragmentKind.Combined
                }
            };

            contents.ForEach(f => f._parent = toReturn);

            return toReturn;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fragments"></param>
        /// <returns></returns>
        public RegexFragment CombineAlternates(params RegexFragment[] fragments)
        {
            List<RegexFragment> contents = fragments.Prepend(this).ToList();

            var toReturn = new RegexFragment(contents.ToArray())
            {
                _options =
                {
                    FragmentKind = RegexFragmentKind.AlternationList
                }
            };

            contents.ForEach(f => f._parent = toReturn);

            return toReturn;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal string Compile()
        {
            switch (_options.FragmentKind)
            {
                case RegexFragmentKind.Group:
                    return Wrap(CompileContents(), "(?:", ")") + CompileModifiers();
                case RegexFragmentKind.CaptureGroup:
                    return Wrap(CompileContents(), "(", ")") + CompileModifiers();
                case RegexFragmentKind.NamedCaptureGroup:
                    return Wrap(CompileContents(), "(?<" + _options.GroupName + ">", ")") + CompileModifiers();
                case RegexFragmentKind.CharacterSet:
                    return Wrap(_basePattern, "[", "]") + CompileModifiers();
                case RegexFragmentKind.NegatedCharacterSet:
                    return Wrap(_basePattern, "[^", "]") + CompileModifiers();
                case RegexFragmentKind.ExactRepeatModifier:
                    return "{" + _options.RepeatCount + "}";
                case RegexFragmentKind.RepeatRangeModifier:
                    return "{" + _options.RepetitionMinimum + ", " + (_options.RepetitionMaximum > 0 ? _options.RepetitionMaximum.ToString() : "") + "}";
                case RegexFragmentKind.Anchor:
                    switch (_options.Position)
                    {
                        case AnchorPosition.Leading:
                            return _basePattern + CompileContents() + CompileModifiers();
                        case AnchorPosition.Trailing:
                            return CompileContents() + CompileModifiers() + _basePattern;
                    }
                    break;
                case RegexFragmentKind.LookaroundGroup:
                    switch (_options.LookaroundType)
                    {
                        case LookaroundType.PositiveLookahead:
                            return CompileContents() + CompileModifiers() + Wrap(_basePattern, "(?=", ")");
                        case LookaroundType.NegativeLookahead:
                            return CompileContents() + CompileModifiers() + Wrap(_basePattern, "(?!", ")");
                        case LookaroundType.PositiveLookbehind:
                            return CompileContents() + CompileModifiers() + Wrap(_basePattern, "(?<=", ")");
                        case LookaroundType.NegativeLookbehind:
                            return CompileContents() + CompileModifiers() + Wrap(_basePattern, "(?<!", ")");
                        default:
                            break;
                    }
                    break;
                case RegexFragmentKind.AlternationList:
                    return Wrap(_contents.Flatten(f => f.Compile(), "|"), "(?:", ")") + CompileModifiers();
                case RegexFragmentKind.Combined:
                    return _contents.Flatten(f => f.Compile());
                case RegexFragmentKind.Operator:
                case RegexFragmentKind.Literal:
                case RegexFragmentKind.CharacterEscape:
                default:
                    return _basePattern + CompileModifiers();
            }

            return _basePattern;
        }

        private string CompileContents() => _contents.Flatten(f => f.Compile());

        private string CompileModifiers() => _modifiers.Flatten(f => f.Compile());

        private string Wrap(string toWrap, string left, string right) => left + toWrap + right;

        /// <inheritdoc/>
        public override string ToString() => Compile();
    }
}
