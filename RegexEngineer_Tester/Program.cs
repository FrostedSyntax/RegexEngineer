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
            RunAllTests();

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
                regexEngineer.CreateCharClass("Hh").Combine(regexEngineer.CreateLiteral("ello")),
                regexEngineer.CreateCharClass(" "),
                regexEngineer.CreateCharClass("Ww").Combine(regexEngineer.CreateLiteral("orld"))
            };

            regexEngineer.AddFragments(fragments.ToArray());

            return new Regex(regexEngineer.ToString()).IsMatch(testString);
        }

        // [0-9]+[ ]?
        [Test]
        private static bool Test2(string testString)
        {
            var regexEngineer = RegexEngineer.Create();

            var fragments = new List<RegexFragment>
            {
                regexEngineer.CreateCharClass("0-9").OneOrMore().Combine(regexEngineer.CreateCharClass(" ").Optional()),
            };

            regexEngineer.AddFragments(fragments.ToArray());

            return new Regex(regexEngineer.ToString()).Matches(testString).Count > 0;
        }
    }

    internal class TestAttribute : Attribute { }
}
