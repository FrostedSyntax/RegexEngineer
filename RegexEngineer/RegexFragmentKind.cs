namespace RegexEngineerLib
{
    public enum RegexFragmentKind
    {
        Literal,
        CharacterEscape,
        Group,
        AlternationList,
        CaptureGroup,
        NamedCaptureGroup,
        LookaroundGroup,
        CharacterSet,
        NegatedCharacterSet,
        Operator,
        ExactRepeatModifier,
        RepeatRangeModifier,
        Anchor,
        Combined
    }
}