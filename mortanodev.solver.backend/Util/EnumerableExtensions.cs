using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mortanodev.solver.backend.Util
{
    /// <summary>
    /// Extension methods for arrays
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Returns the first index of the given element in the given range. Returns -1 if no matching
        /// element can be found
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="range">Range</param>
        /// <param name="element">Element to find</param>
        /// <returns>First index of the element or -1 if no matching element exists</returns>
        public static int FirstIndexOf<T>(this IEnumerable<T> range, T element)
        {
            int idx = 0;
            foreach(var elem in range)
            {
                if ( elem.Equals(element) ) return idx;
                idx++;
            }
            return -1;
        }

        /// <summary>
        /// Returns the index of the first element that matches the given predicate. Returns -1 if no matching
        /// element can be found
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="range">Range</param>
        /// <param name="predicate">Predicate</param>
        /// <returns>Index of first element matching the predicate, or -1 if no matching element exists</returns>
        public static int FirstIndexWhere<T>(this IEnumerable<T> range, Func<T, bool> predicate)
        {
            int idx = 0;
            foreach(var elem in range)
            {
                if (predicate(elem)) return idx;
                idx++;
            }
            return -1;
        }

        /// <summary>
        /// Groups the given range by using the given key selector and then splits this range into a range of 
        /// ranges where each element of the outer range is one of the groups of the grouping, but without
        /// the IGrouping interface
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="range">Range to group</param>
        /// <param name="keySelector">Key selector to group by</param>
        /// <param name="equalityComparer">Optional equality comparison object for the keys</param>
        /// <returns>Range of ranges after grouping</returns>
        public static IEnumerable<IEnumerable<T>> GroupAndSplit<T, Key>(this IEnumerable<T> range, Func<T, Key> keySelector, IEqualityComparer<Key> equalityComparer = null)
        {
            var grouping = range.GroupBy(keySelector, equalityComparer);
            return grouping.Select(g => g.GetEnumerator().Enumerate());
        }

        /// <summary>
        /// Returns true if the two sequences are permutations of each other, i.e. they contain the same elements but
        /// not neccesarily in the same order. 
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="thisRange">First range</param>
        /// <param name="otherRange">Second range</param>
        /// <returns>True if the two ranges contain the same elements regardless of order</returns>
        public static bool PermutationEquals<T>(this IEnumerable<T> thisRange, IEnumerable<T> otherRange)
        {
            var p1 = thisRange.ToArray();
            var p2 = otherRange.ToArray();
            if (p1.Length != p2.Length) return false;
            foreach(var outer in p1)
            {
                bool match = false;
                foreach(var inner in p2)
                {
                    if(inner.Equals(outer))
                    {
                        match = true;
                        break;
                    }
                }
                if (!match) return false;
            }
            return true;
        }

        /// <summary>
        /// Returns true if the given enumerable is empty, otherwise false
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="thisRange">Enumerable</param>
        /// <returns>True if the enumerable is empty, otherwise false</returns>
        public static bool IsEmpty<T>(this IEnumerable<T> thisRange)
        {
            foreach (var elem in thisRange) return false;
            return true;
        }

        /// <summary>
        /// Converts a range into a HashSet
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="thisRange">Range</param>
        /// <returns>HashSet containing unique elements of the range</returns>
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> thisRange)
        {
            var ret = new HashSet<T>();
            foreach (var elem in thisRange)
            {
                ret.Add(elem);
            }
            return ret;
        }

    }
}
