using System.Collections.Generic;

namespace mortanodev.solver.backend.Util
{
    /// <summary>
    /// Comparer for HashSets that compares the set equality 
    /// </summary>
    /// <typeparam name="T">Element type</typeparam>
    public class HashSetComparer<T> : IEqualityComparer<HashSet<T>>
    {
        public bool Equals(HashSet<T> x, HashSet<T> y)
        {
            if (x == null && y == null) return true;
            if (x == null || y == null) return false;
            return x.SetEquals(y);
        }

        public int GetHashCode(HashSet<T> obj)
        {
            return obj.GetHashCode();
        }
    }
}
