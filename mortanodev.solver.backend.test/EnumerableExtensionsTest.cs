using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using mortanodev.solver.backend.Util;
using System.Linq;

namespace mortanodev.solver.backend.test
{
    [TestClass]
    public class EnumerableExtensionsTest
    {
        [TestMethod]
        public void TestFirstIndexOf_Empty()
        {
            var range = new List<int>();
            Assert.AreEqual(-1, range.FirstIndexOf(42));
        }

        [TestMethod]
        public void TestFirstIndexOf_OneElement_Found()
        {
            var range = new[] { 42 };
            Assert.AreEqual(0, range.FirstIndexOf(42));
        }

        [TestMethod]
        public void TestFirstIndexOf_OneElement_NotFound()
        {
            var range = new[] { 23 };
            Assert.AreEqual(-1, range.FirstIndexOf(42));
        }

        [TestMethod]
        public void TestFirstIndexWhere_Empty()
        {
            var range = new List<int>();
            Assert.AreEqual(-1, range.FirstIndexWhere(e => true));
        }

        [TestMethod]
        public void TestFirstIndexWhere_OneElement_Found()
        {
            var range = new[] { 42 };
            Assert.AreEqual(0, range.FirstIndexWhere(e => e == 42));
        }

        [TestMethod]
        public void TestFirstIndexWhere_OneElement_NotFound()
        {
            var range = new[] { 3 };
            Assert.AreEqual(-1, range.FirstIndexWhere(e => (e % 2) == 0));
        }

        [TestMethod]
        public void TestGroupAndSplit_Empty()
        {
            var range = new List<int>();
            var grouped = range.GroupAndSplit(e => (e % 2) == 0);
            Assert.IsTrue(grouped.IsEmpty());
        }

        [TestMethod]
        public void TestGroupAndSplit_BoolPartition()
        {
            var range = new[] { 1, 2, 3, 4, 5, 6 };
            var grouped = range.GroupAndSplit(e => (e % 2) == 0).ToArray();
            Assert.AreEqual(2, grouped.Length);

            var evens = range.Where(e => (e % 2) == 0);
            var odds = range.Where(e => (e % 2) == 1);

            Assert.IsTrue(grouped[0].SequenceEqual(odds), "Wrong sequence: Expected " + odds.ToString() + " but got " + grouped[0].ToString());
            Assert.IsTrue(grouped[1].SequenceEqual(evens), "Wrong sequence: Expected " + evens.ToString() + " but got " + grouped[1].ToString());
        }

        [TestMethod]
        public void TestGroupAndSplit_IntPartition()
        {
            var range = new[] { 1, 2, 3, 4 };
            var grouped = range.GroupAndSplit(e => e).ToArray();
            Assert.AreEqual(range.Length, grouped.Length);

            for(int i = 0; i < range.Length; i++)
            {
                Assert.AreEqual(1, grouped[i].Count());
                Assert.AreEqual(range[i], grouped[i].First());
            }
        }

        [TestMethod]
        public void TestIsEmpty_Empty()
        {
            var range = new List<int>();
            Assert.IsTrue(range.IsEmpty());
        }

        [TestMethod]
        public void TestIsEmpty_NotEmpty()
        {
            var range = new[] { 1 };
            Assert.IsFalse(range.IsEmpty());
        }

        [TestMethod]
        public void TestPermutationEquals_Empty()
        {
            var p1 = new List<int>();
            var p2 = new List<int>();
            Assert.IsTrue(p1.PermutationEquals(p2));
            Assert.IsTrue(p2.PermutationEquals(p1));
        }

        [TestMethod]
        public void TestPermutationEquals_OneElement()
        {
            var p1 = new[] { 2 };
            var p2 = new[] { 2 };
            Assert.IsTrue(p1.PermutationEquals(p2));
            Assert.IsTrue(p2.PermutationEquals(p1));
        }

        [TestMethod]
        public void TestPermutationEquals_Many_Equals()
        {
            var p1 = new[] { 3, 5, 7, 9 };
            var p2 = new[] { 7, 3, 5, 9 };
            Assert.IsTrue(p1.PermutationEquals(p2));
            Assert.IsTrue(p2.PermutationEquals(p1));
        }

        [TestMethod]
        public void TestPermutationEquals_Many_NotEqual()
        {
            var p1 = new[] { 3, 5, 7 };
            var p2 = new[] { 1, 4, 8 };
            Assert.IsFalse(p1.PermutationEquals(p2));
            Assert.IsFalse(p2.PermutationEquals(p1));
        }

        [TestMethod]
        public void TestPermutationEquals_Many_Subset()
        {
            var p1 = new[] { 3, 5, 7 };
            var p2 = new[] { 7, 9, 5, 3 };
            Assert.IsFalse(p1.PermutationEquals(p2));
            Assert.IsFalse(p2.PermutationEquals(p1));
        }

    }
}
