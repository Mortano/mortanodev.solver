using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mortanodev.solver.backend
{
    /// <summary>
    /// An alphabet that consists of zero or more unique symbols
    /// </summary>
    public class Alphabet
    {

        #region Properties

        /// <summary>
        /// Collection of unique symbols that make up the alphabet
        /// </summary>
        public IReadOnlyList<char> Symbols
        {
            get;
            private set;
        }

        /// <summary>
        /// Returns the cardinality (i.e. the number of symbols) of this alphabet
        /// </summary>
        public int Cardinality
        {
            get
            {
                return Symbols.Count;
            }
        }

        #endregion

        #region Constructors

        private Alphabet()
        {
            Symbols = new char[0];
        }

        private Alphabet(char[] symbols)
        {
            Symbols = symbols;
        }

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// Creates a language from the given range of symbols. Duplicate symbols will be removed from that
        /// range. If the symbols are empty, the empty language will be returned.
        /// </summary>
        /// <param name="symbols">A collection of zero or more symbols for this alphabet</param>
        /// <returns></returns>
        public static Alphabet Create(IEnumerable<char> symbols)
        {
            if (symbols == null) return Alphabet.Empty;
            var distinctSymbols = symbols.Distinct().ToArray();
            if (distinctSymbols.Length == 0) return Alphabet.Empty;
            return new Alphabet(distinctSymbols);
        }

        /// <summary>
        /// Returns true if the given symbol is part of this alphabet
        /// </summary>
        /// <param name="symbol">Symbol</param>
        /// <returns>True if the symbol is in this alphabet</returns>
        public bool Contains(char symbol)
        {
            return Symbols.Contains(symbol);
        }

        #endregion

        #endregion

        #region Members

        public static readonly Alphabet Empty = new Alphabet();        

        #endregion

    }
}
