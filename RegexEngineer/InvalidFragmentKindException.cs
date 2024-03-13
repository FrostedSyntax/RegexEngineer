using System;
using System.Runtime.Serialization;

namespace RegexEngineerLib
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    internal class InvalidFragmentKindException : Exception
    {
        public InvalidFragmentKindException() { }

        public InvalidFragmentKindException(string message) : base(message) { }
    }
}