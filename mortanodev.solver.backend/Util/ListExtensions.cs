using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mortanodev.solver.backend.Util
{
    public static class ListExtensions
    {

        /// <summary>
        /// Returns the index of the first element in the given list that matches the predicate, or
        /// -1 if no matching element exists
        /// </summary>
        /// <typeparam name="T">Type of the list elements</typeparam>
        /// <param name="list">List</param>
        /// <param name="predicate">Predicate</param>
        /// <returns>Index of first element in the list that matches the predicate, or -1</returns>
        public static int FirstIndexOf<T>(this List<T> list, Func<T, bool> predicate)
        {
            for(int i = 0; i < list.Count; i++)
            {
                if (predicate(list[i])) return i;
            }
            return -1;
        }

    }
}
