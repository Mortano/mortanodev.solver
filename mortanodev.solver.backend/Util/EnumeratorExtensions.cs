using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mortanodev.solver.backend.Util
{
    public static class EnumeratorExtensions
    {
        /// <summary>
        /// Enumerates the given enumerator and yields an IEnumerable
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="enumerator">Enumerator</param>
        /// <returns>IEnumerable over the enumerator</returns>
        public static IEnumerable<T> Enumerate<T>(this IEnumerator<T> enumerator)
        {
            while (enumerator.MoveNext()) yield return enumerator.Current;
        }

    }
}
