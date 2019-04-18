using System.Collections.Generic;

namespace Itinero.Optimization.Tests
{
    /// <summary>
    /// Contains some extension methods making test code easier to understand.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Puts the elements of the enumerator (back) in a list.
        /// </summary>
        /// <returns></returns>
        public static List<T> ToList<T>(this IEnumerator<T> enumerator)
        {
            var list = new List<T>();
            while (enumerator.MoveNext())
            {
                list.Add(enumerator.Current);
            }

            return list;
        }
    }
}