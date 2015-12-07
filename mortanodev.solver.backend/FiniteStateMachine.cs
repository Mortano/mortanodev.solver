using log4net;
using mortanodev.solver.backend.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace mortanodev.solver.backend
{
    /// <summary>
    /// Different types of FSM
    /// </summary>
    public enum FsmType
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
            /// The unique Id of this state
            /// </summary>
            public int Id
            {
                get;
            }

            /// <summary>
            /// Whether this is an accepting state or not
            /// </summary>
            public bool IsAccepting
            {
                get;
            }

            public State( int id, bool accepts )
            {
                Id = id;
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
            }

            /// <summary>
            /// The end state
            /// </summary>
            public State End
            {
                get;
            }

            /// <summary>
            /// The symbol for this transition
            /// </summary>
            public char Symbol
            {
                get;
            }

            public Transition(State start, State end, char symbol)
            {
                Start = start;
                End = end;
                Symbol = symbol;
            }

            public override bool Equals(object obj)
            {
                var asTransition = obj as Transition;
                if (asTransition == null) return false;
                if (Start != asTransition.Start) return false;
                if (End != asTransition.End) return false;
                return Symbol == asTransition.Symbol;
            }

            public override int GetHashCode()
            {
                var hash = Start.GetHashCode();
                hash ^= End.GetHashCode() * 17;
                hash ^= Symbol * 17;
                return hash;
            }
        }

        /// <summary>
        /// A partition of states used for minimizing the FSM. This is essentially an integer vector,
        /// but Math.NET does not support those, so we have to roll our own...
        /// </summary>
        private class StatePartition
        {

            #region Properties

            /// <summary>
            /// The elements of this state partition that indicate the partitions that a state leads into
            /// when traversing from that state using symbols from the alphabet. 
            /// </summary>
            private int[] Elements
            {
                get;
            }

            /// <summary>
            /// Index operator
            /// </summary>
            /// <param name="idx">Index</param>
            /// <returns>Element at idx</returns>
            public int this[int idx]
            {
                get
                {
                    return Elements[idx];
                }
                set
                {
                    Elements[idx] = value;
                }
            }

            #endregion

            #region Constructors

            /// <summary>
            /// Creates a new StatePartition over the given alphabet. The state partition will contain
            /// one element for each symbol in the given alphabet
            /// </summary>
            /// <param name="alphabet">Alphabet to use for the partition</param>
            public StatePartition(Alphabet alphabet)
            {
                Elements = new int[alphabet.Cardinality];
            }

            #endregion

            #region PublicMethods

            public override bool Equals(object obj)
            {
                var asStatePartition = obj as StatePartition;
                return asStatePartition != null && asStatePartition.Elements.SequenceEqual(Elements);
            }

            public override int GetHashCode()
            {
                var hash = 0;
                foreach (var element in Elements) hash ^= element * 17;
                return hash;
            }

            #endregion

        }

        /// <summary>
        /// Equality comparer for IEnumerable
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        private class EnumerableComparer<T> : IEqualityComparer<IEnumerable<T>>
        {
            public bool Equals(IEnumerable<T> x, IEnumerable<T> y)
            {
                if (x == null && y == null) return true;
                if (x == null || y == null) return false;
                return x.SequenceEqual(y);
            }

            public int GetHashCode(IEnumerable<T> obj)
            {
                var hash = 0;
                foreach(var elem in obj)
                {
                    if (elem == null) continue;
                    hash ^= elem.GetHashCode() * 17;
                }
                return hash;
            }
        }

        #region Properties

        /// <summary>
        /// The alphabet that this FSM operates on
        /// </summary>
        public Alphabet Alphabet
        {
            get;
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
        public FsmType Type
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

            Type = CalculateIsNonDeterministic() ? FsmType.NonDeterministic : FsmType.Deterministic;
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
            var acceptedStatesArray = acceptedStates as int[] ?? acceptedStates.ToArray();
            if (acceptedStatesArray.Count() >= numberOfStates) throw new ArgumentException(nameof(acceptedStates) + " contains more states than the state count!");

            //Check that all accepted states are in range
            if( acceptedStatesArray.Any(s => s < 0 || s >= numberOfStates) )
            {
                throw new ArgumentException(nameof(acceptedStates) + " contains out of range states!");
            }

            //Check that all transitions are valid
            Func<Tuple<int, int, char>, bool> transitionInvalid = t =>
            {
                if (t.Item1 < 0 || t.Item1 >= numberOfStates) return true;
                if (t.Item2 < 0 || t.Item2 >= numberOfStates) return true;
                return !alphabet.Contains(t.Item3);
            };
            var transitionsArray = transitions as Tuple<int, int, char>[] ?? transitions.ToArray();
            var invalidTransitions = transitionsArray.Where(transitionInvalid).ToList();
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

            var states = Enumerable.Range(0, numberOfStates).Select(id => new State(id, acceptedStatesArray.Any(ac => ac == id))).ToArray();
            var accepted = acceptedStatesArray.Select(id => states.First(s => s.Id == id)).ToArray();
            var start = states.First(s => s.Id == startState);
            
            var trans = transitionsArray.Select(t =>
            {
                var sState = states.First(s => s.Id == t.Item1);
                var eState = states.First(s => s.Id == t.Item2);
                return new Transition(sState, eState, t.Item3);
            }).ToArray();

            return new FSM(alphabet, states, accepted, start, trans);
        }

        /// <summary>
        /// Turns this FSM into a deterministic state machine, if it isn't already deterministic
        /// </summary>
        public void MakeDeterministic()
        {
            if (Type == FsmType.Deterministic) return;



            Type = FsmType.Deterministic;
        }

        /// <summary>
        /// Minimizes this FSM if it is not already minimal. Only works if this FSM is deterministic!
        /// </summary>
        public void Minimize()
        {
            if (Type != FsmType.Deterministic) throw new Exception("Minimization only works on deterministic state machines!");
            if (_hasBeenMinimized) return;

            var statePartitionsOld = States.GroupAndSplit(s => s.IsAccepting).ToList();

            var statePartitionMatrix = PartitionStates(statePartitionsOld);
            
            var statePartitionsNew = SplitStates(statePartitionsOld, statePartitionMatrix).ToList();

            //As long as there are differences between the last state partition and the current one, we continue
            //running the minimizing algorithm
            while(!statePartitionsNew.SequenceEqual(statePartitionsOld, StateCollectionComparer))
            {
                statePartitionsOld = statePartitionsNew;

                statePartitionMatrix = PartitionStates(statePartitionsOld);

                statePartitionsNew = SplitStates(statePartitionsOld, statePartitionMatrix);
            }

            //Now we have reached equilibrium and the current state partition is the minimal one. We now have to 
            //create new states and new transitions from this partition!

            //1) Each state partition that contains a state that accepts becomes a new accepting state
            var acceptingMask = statePartitionsNew.Select(p => p.FirstIndexWhere(s => s.IsAccepting) != -1).ToArray();
            var minimalStates = Enumerable.Range(0, acceptingMask.Length).Select(id => new State(id, acceptingMask[id])).ToArray();

            //2) The starting state is the state partition that contained the initial starting state
            var startingStateIdx = statePartitionsNew.FirstIndexWhere(p => p.FirstIndexWhere(s => s == StartingState) != -1);

            //3) The state partition matrix contains all the transitions between the states
            var newTransitions = new List<Transition>();
            var partitionsArray = statePartitionsNew.ToArray();
            for(int i = 0; i < minimalStates.Length; i++)
            {
                //We have to find one of the old states that have been combined into this new minimal state
                //For that state, we can then query the transitions from the statePartitionMatrix
                var oldStateInThisState = partitionsArray[i].First();
                var statePartitionRow = statePartitionMatrix[oldStateInThisState.Id];
                newTransitions.AddRange(
                    Alphabet.Symbols.Select((t, j) => new Transition(minimalStates[i], minimalStates[statePartitionRow[j]], t))
                );
            }

            States = minimalStates;
            StartingState = minimalStates[startingStateIdx];
            AcceptedStates = States.Where(s => s.IsAccepting).ToArray();
            Transitions = newTransitions.ToArray();
        }

        #endregion

        #region Private
        
        /// <summary>
        /// Calculates whether this FSM is ND or not
        /// </summary>
        /// <returns>True if this FSM is ND</returns>
        private bool CalculateIsNonDeterministic()
        {
            //If any of the states does not have one transition for each of the symbols in the alphabet, this FSM
            //is non-deterministic
            
            var transitions = new List<Transition>(Transitions);

            foreach(var state in States)
            {
                foreach(var symbol in Alphabet.Symbols)
                {
                    var matchingTransitionIdx = transitions.FirstIndexWhere(t => t.Start == state && t.Symbol == symbol);
                    if (matchingTransitionIdx == -1) return true;
                    transitions.RemoveAt(matchingTransitionIdx);
                }
            }

            //If any transitions are leftovers, there are more transitions than there should be for a deterministic FSM!
            return transitions.Count != 0;
        }

        /// <summary>
        /// Performs a state partition by applying all transitions to the states based on the given state
        /// partition. The values are stored inside a matrix where each entry shows the index of the resulting
        /// state group that this state leads into after applying a transition with a specific symbol of the
        /// alphabet.
        /// </summary>
        /// <param name="oldStatePartition">The previous state partition</param>
        /// <returns>Partition matrix</returns>
        private List<StatePartition> PartitionStates(IReadOnlyCollection<IEnumerable<State>> oldStatePartition)
        {
            var matrix = States.Select(state => new StatePartition(Alphabet)).ToList();
            foreach (var transition in Transitions)
            {
                var targetStateId = oldStatePartition.FirstIndexWhere(p => p.Contains(transition.End));
                var rowIdx = transition.Start.Id;
                var columnIdx = Alphabet.Symbols.FirstIndexOf(transition.Symbol);

                matrix[rowIdx][columnIdx] = targetStateId;
            }
            return matrix;
        }

        /// <summary>
        /// Splits the given set of states into possibly smaller sets using the list of state partitions. Each set of states
        /// in stateSets will be split separately, making sure that no elements can move from multiple sets into one new set
        /// </summary>
        /// <param name="stateSets">Set of grouped states</param>
        /// <param name="partition">State partition matrix</param>
        /// <returns>New set of grouped states</returns>
        private List<IEnumerable<State>> SplitStates(IEnumerable<IEnumerable<State>> stateSets, List<StatePartition> partition)
        {
            var ret = new List<IEnumerable<State>>();
            foreach(var stateGroup in stateSets)
            {
                var split = stateGroup.GroupAndSplit(s => partition[s.Id]);
                ret.AddRange(split);
            }
            return ret;
        }

        #endregion

        #endregion

        #region Members

        private static readonly ILog Log = LogManager.GetLogger(typeof(FSM));
        private static readonly IEqualityComparer<IEnumerable<State>> StateCollectionComparer = new EnumerableComparer<State>();

        private bool _hasBeenMinimized = false;

        #endregion

    }
}
