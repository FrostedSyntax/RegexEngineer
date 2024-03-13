namespace RegexEngineerLib
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class RegexFragmentOptions
    {
        public RegexFragmentKind FragmentKind;

        public bool Container => FragmentKind == RegexFragmentKind.Group ||
                                 FragmentKind == RegexFragmentKind.CaptureGroup ||
                                 FragmentKind == RegexFragmentKind.NamedCaptureGroup ||
                                 FragmentKind == RegexFragmentKind.LookaroundGroup;

        public int RepeatCount = -1;

        public int RepetitionMinimum = 0;

        public int RepetitionMaximum = -1;

        public AnchorPosition Position;

        public LookaroundType LookaroundType;

        public string GroupName;
    }

    /// <summary>
    /// 
    /// </summary>
    public enum AnchorPosition
    {
        Leading,
        Trailing
    }

    /// <summary>
    /// 
    /// </summary>
    public enum LookaroundType
    {
        PositiveLookahead,
        NegativeLookahead,
        PositiveLookbehind,
        NegativeLookbehind
    }
}
