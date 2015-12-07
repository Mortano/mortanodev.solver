using System.Collections.Generic;

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
            //Using a list here because we might want to enumerate multiple times
            var ret = new List<T>();
            while (enumerator.MoveNext()) ret.Add(enumerator.Current);
            return ret;
        }

    }
}
