using log4net;
using mortanodev.solver.backend.Util;
using System;
using System.Collections.Generic;
using System.Linq;

using MathNet.Numerics.LinearAlgebra;

namespace mortanodev.solver.backend
{
    /// <summary>
    /// Different types of FSM
    /// </summary>
    public enum FSMType
    {
        /// <summary>
        /// A deterministic state machine, meaning that there is exactly one transition per state
        /// for each symbol in the alphabet
        /// </summary>
        Deterministic,
        /// <summary>
        /// A non-deterministic state machine, meaning that there are more or fewer than one transitions
        /// per state for each symbol in the alphabet
        /// </summary>
        NonDeterministic
    }

    /// <summary>
    /// A finite state machine, either deterministic or non-determinstic. These intrinsic properties are
    /// given by the states and connections of the state machine, so I chose to use only a single class for it
    /// </summary>
    public class FSM
    {

        /// <summary>
        /// State of a FSM. All states are handled only through IDs, no actual names
        /// </summary>
        public class State
        {
            /// <summary>
            /// The unique ID of this state
            /// </summary>
            public int ID
            {
                get;
                private set;
            }

            /// <summary>
            /// Whether this is an accepting state or not
            /// </summary>
            public bool IsAccepting
            {
                get;
                private set;
            }

            public State( int id, bool accepts )
            {
                ID = id;
                IsAccepting = accepts;
            }
        }

        /// <summary>
        /// Transition between two states of a FSM
        /// </summary>
        public class Transition
        {
            /// <summary>
            /// The start state
            /// </summary>
            public State Start
            {
                get;
                private set;
            }

            /// <summary>
            /// The end state
            /// </summary>
            public State End
            {
                get;
                private set;
            }

            /// <summary>
            /// The symbol for this transition
            /// </summary>
            public char Symbol
            {
                get;
                private set;
            }

            public Transition(State start, State end, char symbol)
            {
                Start = start;
                End = end;
                Symbol = symbol;
            }
        }

        #region Properties

        /// <summary>
        /// The alphabet that this FSM operates on
        /// </summary>
        public Alphabet Alphabet
        {
            get;
            private set;
        }

        /// <summary>
        /// All transitions between states that this FSM has. Depending on these transitions,
        /// this FSM will be either deterministic or non-deterministic:
        /// 
        /// If there is exactly one transition for each symbol in the alphabet for each of the
        /// states, then this FSM is deterministic, otherwise it is non-deterministic.
        /// </summary>
        public Transition[] Transitions
        {
            get;
            private set;
        }

        /// <summary>
        /// All states of this FSM
        /// </summary>
        public State[] States
        {
            get;
            private set;
        }

        /// <summary>
        /// All states in this FSM that accept
        /// </summary>
        public State[] AcceptedStates
        {
            get;
            private set;
        }

        /// <summary>
        /// The starting state of this FSM
        /// </summary>
        public State StartingState
        {
            get;
            private set;
        }        

        /// <summary>
        /// The type of this FSM
        /// </summary>
        public FSMType Type
        {
            get;
            private set;
        }

        #endregion

        #region Constructor

        private FSM(Alphabet alhpabet, 
                    State[] states, 
                    State[] acceptedStates, 
                    State startState, 
                    Transition[] transitions)
        {
            Alphabet = alhpabet;
            States = states;
            AcceptedStates = acceptedStates;
            StartingState = startState;
            Transitions = transitions;

            Type = CalculateIsND() ? FSMType.NonDeterministic : FSMType.Deterministic;
        }

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// Creates a new finite state machine from the given set of rules. This method does extensive checking on
        /// the arguments to see if they are valid an match. If any of the arguments don't match, an exception will
        /// be thrown. 
        /// </summary>
        /// <param name="alphabet">The alphabet that this FSM shall use</param>
        /// <param name="numberOfStates">The number of states of the FSM. This has to be zero or greater!</param>
        /// <param name="acceptedStates">A collection of zero or more accepted states. The size of this collection
        /// must not exceed the number of states that this FSM has</param>
        /// <param name="startState">The index of the starting state amongst all the states. This number identifies 
        /// any state in the range [0;numberOfStates)</param>
        /// <param name="transitions">A collection of zero or more transitions, packed as 3-tuples of integers. The
        /// first value denotes the starting state of that transition, the second value denotes the ending state and
        /// the third value denotes the symbol in the alphabet associated with this transition. The first two values
        /// must point to valid states (i.e. they must be in the range [0;numberOfStates) ) while the third value 
        /// must be contained in the given alphabet.</param>
        /// <returns>A new FSM instance initialized with the given values</returns>
        public static FSM Create(Alphabet alphabet, 
                                 int numberOfStates, 
                                 IEnumerable<int> acceptedStates, 
                                 int startState, 
                                 IEnumerable<Tuple<int, int, char>> transitions)
        {
            if (alphabet == null) throw new ArgumentNullException(nameof(alphabet));
            if (acceptedStates == null) acceptedStates = Enumerable.Empty<int>();
            if (transitions == null) transitions = Enumerable.Empty<Tuple<int, int, char>>();
            if (numberOfStates < 0) throw new ArgumentOutOfRangeException(nameof(numberOfStates));
            if (numberOfStates > 0 && 
                (startState < 0 || startState >= numberOfStates) )
            {
                throw new ArgumentOutOfRangeException(nameof(startState));
            }

            //Check that accepted states are not too many
            if (acceptedStates.Count() >= numberOfStates) throw new ArgumentException(nameof(acceptedStates) + " contains more states than the state count!");

            //Check that all accepted states are in range
            if( acceptedStates.Any(s => s < 0 || s >= numberOfStates) )
            {
                throw new ArgumentException(nameof(acceptedStates) + " contains out of range states!");
            }

            //Check that all transitions are valid
            Func<Tuple<int, int, char>, bool> transitionInvalid = (t) =>
            {
                if (t.Item1 < 0 || t.Item1 >= numberOfStates) return true;
                if (t.Item2 < 0 || t.Item2 >= numberOfStates) return true;
                if (!alphabet.Contains(t.Item3)) return true;
                return false;
            };
            var invalidTransitions = transitions.Where(transitionInvalid).ToList();
            if(invalidTransitions.Count > 0)
            {
                Log.Error("Invalid transitions:");
                foreach(var transition in invalidTransitions)
                {
                    Log.Error("[" + transition.Item1 + ";" + transition.Item2 + ";" + transition.Item3 + "]");
                }
                throw new ArgumentException("Invalid transitions!");
            }

            //Everything is valid, we can safely create the object

            var states = Enumerable.Range(0, numberOfStates).Select(id => new State(id, acceptedStates.Any(ac => ac == id))).ToArray();
            var accepted = acceptedStates.Select(id => states.First(s => s.ID == id)).ToArray();
            var start = states.First(s => s.ID == startState);
            
            var trans = transitions.Select(t =>
            {
                var sState = states.First(s => s.ID == t.Item1);
                var eState = states.First(s => s.ID == t.Item2);
                return new Transition(sState, eState, t.Item3);
            }).ToArray();

            return new FSM(alphabet, states, accepted, start, trans);
        }

        /// <summary>
        /// Minimizes this FSM if it is not already minimal. Only works if this FSM is deterministic!
        /// </summary>
        public void Minimize()
        {
            if (Type != FSMType.Deterministic) throw new Exception("Minimization only works on deterministic state machines!");
            if (_hasBeenMinimized) return;

            var statePartitionsOld = States.GroupBy(s => s.IsAccepting);

            var stateMatrixCur = Matrix<int>.Build.Dense(States.Length, Alphabet.Symbols.Count);

            foreach(var transition in Transitions)
            { 
                var targetStateId = statePartitionsOld.FirstIndexWhere(p => p.Contains(transition.End));
                var rowIdx = transition.Start.ID;
                var columnIdx = Alphabet.Symbols.FirstIndexOf(transition.Symbol);

                stateMatrixCur[rowIdx, columnIdx] = targetStateId;
            }

            var newPartitionGroups = stateMatrixCur.EnumerateRows().Distinct().ToArray();
            var statePartitionsNew = States.GroupBy(s => newPartitionGroups[s.ID]);

            while(!statePartitionsNew.SequenceEqual(statePartitionsOld))
            {

            }
        }

        #endregion

        #region Private
        
        /// <summary>
        /// Calculates whether this FSM is ND or not
        /// </summary>
        /// <returns>True if this FSM is ND</returns>
        private bool CalculateIsND()
        {
            //If any of the states does not have one transition for each of the symbols in the alphabet, this FSM
            //is non-deterministic

            //TODO Make this algorithm less stupid
            var transitions = new List<Transition>(Transitions);

            foreach(var state in States)
            {
                foreach(var symbol in Alphabet.Symbols)
                {
                    var matchingTransitionIdx = transitions.FirstIndexOf(t => t.Start == state && t.Symbol == symbol);
                    if (matchingTransitionIdx == -1) return true;
                    transitions.RemoveAt(matchingTransitionIdx);
                }
            }

            //If any transitions are leftovers, there are more transitions than there should be for a deterministic FSM!
            return transitions.Count != 0;
        }

        #endregion

        #endregion

        #region Members

        private static readonly ILog Log = LogManager.GetLogger(typeof(FSM));

        private bool _hasBeenMinimized = false;

        #endregion

    }
}
