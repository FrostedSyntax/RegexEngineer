# RegexBuilder

## About

RegexBuilder is a C# library used to easily manage writing complicated Regex patterns with code. Each part of the pattern is represented by a RegexFragment object, which can each be modified and grouped according to the needs of the pattern you are writing.

## Example Usage

> string example =  @"[A-Za-z]+(?:['][a-z]+)?";
> 
> RegexBuilder builder = RegexBuilder.Create();
> 
> var frag1 = builder.CreateCharClass("A-Za-z").OneOrMore();
> var frag2 = builder.CreateCharClass('\'').Group(builder.CreateCharClass("a-z").OneOrMore()).Optional();
>
> builder.AddFragments(frag1, frag2);
>
> Console.WriteLine(rb.ToString() == example);
>
> // True

## License

MonoGame.Extended is released under the [The MIT License (MIT)](https://github.com/craftworkgames/MonoGame.Extended/blob/master/LICENSE).
