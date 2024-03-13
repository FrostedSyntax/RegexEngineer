# RegexBuilder

## Example Usage

> string example =  @"[A-Za-z]+(?:['][a-z]+)?";
> 
> RegexBuilder rb = RegexBuilder.Create();
> var frag1 = rb.CreateCharClass("A-Za-z").OneOrMore();
> var frag2 = rb.CreateCharClass('\'').Group(rb.CreateCharClass("a-z").OneOrMore()).Optional();
>
> rb.AddFragments(frag1, frag2);
>
> Console.WriteLine(rb.ToString());
>
> // [A-Za-z]+(?:['][a-z]+)?

## License

MonoGame.Extended is released under the [The MIT License (MIT)](https://github.com/craftworkgames/MonoGame.Extended/blob/master/LICENSE).
