# RegexBuilder

## About

RegexBuilder is a C# library used to easily manage writing complicated Regex patterns with code. Each part of the pattern is represented by a RegexFragment object, which can each be modified and grouped according to the needs of the pattern you are writing.

The methods are meant to be chainable to keep the code as short as possible but I wrote it to be flexible so there are several ways to use the library.

> :wrench: This is a work in progress and I am interested in getting input from anyone who has ideas.

## Example Usage

```
string example =  @"[A-Za-z]+(?:['][a-z]+)?";

RegexBuilder builder = RegexBuilder.Create();

var frag1 = builder.CreateCharClass("A-Za-z").OneOrMore();

var frag2 = builder.CreateCharClass('\'').Group(builder.CreateCharClass("a-z").OneOrMore()).Optional();

builder.AddFragments(frag1, frag2);

Console.WriteLine(builder.ToString() == example);
```
> // True

## License

MonoGame.Extended is released under the [The MIT License (MIT)](https://github.com/craftworkgames/MonoGame.Extended/blob/master/LICENSE).
