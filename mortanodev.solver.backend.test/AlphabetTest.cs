using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace mortanodev.solver.backend.test
{
    [TestClass]
    public class AlphabetTest
    {
        [TestMethod]
        public void TestConstruction_Empty()
        {
            var alphabet = Alphabet.Create(Enumerable.Empty<char>());

            Assert.IsNotNull(alphabet);
            Assert.AreEqual(0, alphabet.Symbols.Count);

            Assert.AreSame(Alphabet.Empty, alphabet);
        }

        [TestMethod]
        public void TestConstruction_FromNull()
        {
            var alphabet = Alphabet.Create(null);

            Assert.IsNotNull(alphabet);
            Assert.AreEqual(0, alphabet.Symbols.Count);

            Assert.AreSame(Alphabet.Empty, alphabet);
        }

        [TestMethod]
        public void TestConstruction_GeneralCase()
        {
            var symbols = new char[] { '0', '1' };
            var alphabet = Alphabet.Create(symbols);

            Assert.IsTrue(symbols.SequenceEqual(alphabet.Symbols));
        }

        [TestMethod]
        public void TestConstruction_DuplicatedSymbols()
        {
            var symbols = new char[] { '0', '0', '1' };
            var symbolsNoDuplicates = symbols.Distinct();

            var alphabet = Alphabet.Create(symbols);

            Assert.IsTrue(
                symbolsNoDuplicates.SequenceEqual(alphabet.Symbols), 
                "Expected " + symbolsNoDuplicates.ToString() + " but got " + alphabet.Symbols.ToString()
            );
        }

        [TestMethod]
        public void TestContains_True()
        {
            var symbols = new char[] { '0', '1' };
            var alphabet = Alphabet.Create(symbols);

            foreach(var symbol in symbols)
            {
                Assert.IsTrue(alphabet.Contains(symbol));
            }
        }

        [TestMethod]
        public void TestContains_False()
        {
            var symbols = new char[] { '0', '1' };
            var alphabet = Alphabet.Create(symbols);

            Assert.IsFalse(alphabet.Contains('2'));
        }
    }
}
