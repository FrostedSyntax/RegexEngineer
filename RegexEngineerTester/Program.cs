using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using RegexEngineerLib;

namespace RegexEngineerTester
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var re = RegexEngineer.Create();

            re.AddFragments(re.CreateLiteral("!?"));

            Console.WriteLine(re.ToString());

            //RunAllTests();

            Console.ReadLine();
        }

        private static void RunAllTests()
        {
            Console.WriteLine("Test 1: " + RunTest(Test1, "Hello World", "hello world", "Hello world!", "Hello World!!"));
            Console.WriteLine("Test 2: " + RunTest(Test2, "100", "100 101 102", "123 32123"));
        }

        private static TestResult RunTest(Func<string, bool> test, params string[] testStrings)
        {
            var result = new TestResult();

            for (int i = 0 ; i < testStrings.Length ; i++)
            {
                if (test(testStrings[i]) == false)
                {
                    result.AllPassed = false;
                    result.Failed.Add(testStrings[i]);
                }
            }

            return result;
        }

        // [Hh]ello [Ww]orld
        [Test]
        private static bool Test1(string testString)
        {
            var regexEngineer = RegexEngineer.Create();

            var fragments = new List<RegexFragment>
            {
                regexEngineer.CreateCharClass(false, "Hh").Combine(regexEngineer.CreateLiteral("ello")),
                regexEngineer.CreateCharClass(false, " "),
                regexEngineer.CreateCharClass(false, "Ww").Combine(regexEngineer.CreateLiteral("orld"))
            };

            regexEngineer.AddFragments(fragments.ToArray());

            return regexEngineer.Test(testString);
        }

        // [0-9]+[ ]?
        [Test]
        private static bool Test2(string testString)
        {
            var regexEngineer = RegexEngineer.Create();

            var fragments = new List<RegexFragment>
            {
                regexEngineer.CreateEscapedChar(EscapedCharacterKind.Digit).OneOrMore().Combine(regexEngineer.CreateCharClass(false, " ").Optional()),
            };

            regexEngineer.AddFragments(fragments.ToArray());

            return regexEngineer.GetMatches(testString).Count > 0;
        }
    }

    internal class TestAttribute : Attribute { }
}
