using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegexEngineerTester
{
    internal class TestResult
    {
        public bool AllPassed;

        public List<string> Failed;

        public TestResult()
        {
            AllPassed = true;
            Failed = new List<string>();
        }

        public override string ToString()
        {
            string toReturn = "";

            if (AllPassed)
            {
                toReturn = "All tests passed.";
            }
            else
            {
                toReturn = "The following tests failed:\n";

                foreach (var item in Failed)
                {
                    toReturn += item + "\r\n";
                }
            }

            return toReturn;
        }
    }
}
