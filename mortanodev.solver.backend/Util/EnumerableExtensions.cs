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
        /// <returns>Range of ranges after grouping</returns>
        public static IEnumerable<IEnumerable<T>> GroupAndSplit<T>(this IEnumerable<T> range, Func<T, bool> keySelector)
        {
            var grouping = range.GroupBy(keySelector);
            foreach(var group in grouping)
            {
                yield return group.GetEnumerator().Enumerate();
            }
        }

    }
}
