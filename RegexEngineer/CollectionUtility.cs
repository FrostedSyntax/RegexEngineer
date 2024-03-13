using System.Collections.Generic;
using System.Linq;

namespace RegexEngineer
{
    internal static class CollectionUtility
    {
        /// <summary>
        /// Represents a method that converts an object into a string.
        /// </summary>
        /// <typeparam name="T">The type of the object to resolve.</typeparam>
        /// <param name="toResolve">The object to get as a string.</param>
        /// <returns>The string value of the specified object.</returns>
        public delegate string StringConverter<T>(T toResolve);

        /// <summary>
        /// Represents a method that converts an object into a string.
        /// </summary>
        /// <param name="toResolve">The object to make a string.</param>
        /// <returns>The string value of the specified object.</returns>
        public delegate string StringConverter(object toResolve);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string Flatten<T>(this IEnumerable<T> list, string separator = "")
        {
            if (list == null)
            { return null; }

            return list.ToList().ConvertAll(item => item.ToString()).Flatten(separator);
        }

        /// <summary>
        /// Applies a resolver delegate to the string value of each item in the 
        /// specified IEnumerable and returns the combined value with a string delimiter.
        /// </summary>
        /// <param name="list">The ist of strings to flatten..</param>
        /// <param name="resolver">A delegate representing a method to apply to each string before flattening.</param>
        /// <param name="separator">An additional separator added between each string after being manipulated.</param>
        /// <returns>The combined string.</returns>
        public static string Flatten(this IEnumerable<string> list, StringConverter resolver, string separator = "")
        {
            if (list == null)
            { return null; }

            return list.ToList().ConvertAll(item => resolver(item)).Flatten(separator);
        }

        /// <summary>
        /// Applies a resolver delegate to the string value of each item in the 
        /// specified IEnumerable and returns the combined value with a string delimiter.
        /// </summary>
        /// <typeparam name="T">The Type of objects to flatten.</typeparam>
        /// <param name="list">The ist of strings to flatten..</param>
        /// <param name="resolver">A delegate representing a method to apply to each string before flattening.</param>
        /// <returns>The combined string.</returns>
        public static string Flatten<T>(this IEnumerable<T> list, StringConverter<T> resolver)
        {
            if (list == null)
            { return null; }

            string toReturn = string.Empty;
            list.ToList().ForEach(item => toReturn += resolver(item));

            return toReturn;
        }

        /// <summary>
        /// Applies a resolver delegate to the string value of each item in the 
        /// specified IEnumerable and returns the combined value with a string delimiter.
        /// </summary>
        /// <typeparam name="T">The Type of objects to flatten</typeparam>
        /// <param name="list">The ist of strings to flatten..</param>
        /// <param name="resolver">A delegate representing a method to apply to each string before flattening.</param>
        /// <param name="separator">A string delimiter to place between the strings to flatten.</param>
        /// <returns>The combined string.</returns>
        public static string Flatten<T>(this IEnumerable<T> list, StringConverter<T> resolver, string separator)
        {
            if (list == null)
            { return null; }

            string toReturn = string.Empty;
            list.ToList().ForEach(item => toReturn += resolver(item) + separator);

            if (string.IsNullOrEmpty(toReturn) == false && string.IsNullOrEmpty(separator) == false)
            {
                return toReturn.Remove(toReturn.Length - separator.Length);
            }
            else if (string.IsNullOrEmpty(toReturn) == false)
            {
                return toReturn;
            }

            return string.Empty;
        }
    }
}
