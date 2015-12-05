using Microsoft.VisualStudio.TestTools.UnitTesting;

using mortanodev.solver.backend;
using System;
using System.Linq;

namespace mortanodev.solver.backend.test
{
    [TestClass]
    public class FSMTest
    {
        [TestMethod]
        public void TestConstruction_Valid()
        {

            var alphabet = Alphabet.Create(new[] { '0', '1' });
            var numStates = 2;
            var startState = 0;
            var acceptingStates = new[] { 1 };
            var transitions = new Tuple<int, int, char>[]
            {
                new Tuple<int, int, char>(0,1,'0'),
                new Tuple<int, int, char>(0,1,'1'),
                new Tuple<int, int, char>(1,1,'0'),
                new Tuple<int, int, char>(1,1,'1'),
            };

            var fsm = FSM.Create(alphabet, numStates, acceptingStates, startState, transitions);

            Assert.IsNotNull(fsm);
            Assert.AreSame(alphabet, fsm.Alphabet);
            Assert.AreEqual(numStates, fsm.States.Length);
            Assert.AreEqual(startState, fsm.StartingState.ID);
            Assert.AreEqual(acceptingStates.Length, fsm.AcceptedStates.Length);

            for(int i = 0; i < acceptingStates.Length; i++)
            {
                var expected = acceptingStates[i];
                var actual = fsm.AcceptedStates[i];

                Assert.AreEqual(expected, actual.ID);
            }

            Assert.AreEqual(transitions.Length, fsm.Transitions.Length);
            
            for(int i = 0; i < transitions.Length; i++)
            {
                var expected = transitions[i];
                var actual = fsm.Transitions[i];

                Assert.AreEqual(fsm.States.First(s => s.ID == expected.Item1), actual.Start);
                Assert.AreEqual(fsm.States.First(s => s.ID == expected.Item2), actual.End);
                Assert.AreEqual(expected.Item3, actual.Symbol);
            }

            Assert.AreEqual(FSMType.Deterministic, fsm.Type);

        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestConstruction_NegativeStateCount()
        {
            var alphabet = Alphabet.Create(new[] { '0', '1' });
            var numStates = -1;
            var startState = 0;
            var acceptingStates = new[] { 1 };
            var transitions = new Tuple<int, int, char>[]
            {
                new Tuple<int, int, char>(0,1,'0'),
                new Tuple<int, int, char>(0,1,'1'),
                new Tuple<int, int, char>(1,1,'0'),
                new Tuple<int, int, char>(1,1,'1'),
            };            

            var fsm = FSM.Create(alphabet, numStates, acceptingStates, startState, transitions);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestConstruction_NegativeStartState()
        {
            var alphabet = Alphabet.Create(new[] { '0', '1' });
            var numStates = 2;
            var startState = -1;
            var acceptingStates = new[] { 1 };
            var transitions = new Tuple<int, int, char>[]
            {
                new Tuple<int, int, char>(0,1,'0'),
                new Tuple<int, int, char>(0,1,'1'),
                new Tuple<int, int, char>(1,1,'0'),
                new Tuple<int, int, char>(1,1,'1'),
            };

            var fsm = FSM.Create(alphabet, numStates, acceptingStates, startState, transitions);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestConstruction_OutOfRangeStartState()
        {
            var alphabet = Alphabet.Create(new[] { '0', '1' });
            var numStates = 2;
            var startState = 2;
            var acceptingStates = new[] { 1 };
            var transitions = new Tuple<int, int, char>[]
            {
                new Tuple<int, int, char>(0,1,'0'),
                new Tuple<int, int, char>(0,1,'1'),
                new Tuple<int, int, char>(1,1,'0'),
                new Tuple<int, int, char>(1,1,'1'),
            };

            var fsm = FSM.Create(alphabet, numStates, acceptingStates, startState, transitions);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestConstruction_NegativeAcceptingState()
        {
            var alphabet = Alphabet.Create(new[] { '0', '1' });
            var numStates = 2;
            var startState = 0;
            var acceptingStates = new[] { -1 };
            var transitions = new Tuple<int, int, char>[]
            {
                new Tuple<int, int, char>(0,1,'0'),
                new Tuple<int, int, char>(0,1,'1'),
                new Tuple<int, int, char>(1,1,'0'),
                new Tuple<int, int, char>(1,1,'1'),
            };

            var fsm = FSM.Create(alphabet, numStates, acceptingStates, startState, transitions);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestConstruction_OutOfRangeAcceptingState()
        {
            var alphabet = Alphabet.Create(new[] { '0', '1' });
            var numStates = 2;
            var startState = 0;
            var acceptingStates = new[] { 2 };
            var transitions = new Tuple<int, int, char>[]
            {
                new Tuple<int, int, char>(0,1,'0'),
                new Tuple<int, int, char>(0,1,'1'),
                new Tuple<int, int, char>(1,1,'0'),
                new Tuple<int, int, char>(1,1,'1'),
            };

            var fsm = FSM.Create(alphabet, numStates, acceptingStates, startState, transitions);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestConstruction_TooManyAcceptingStates()
        {
            var alphabet = Alphabet.Create(new[] { '0', '1' });
            var numStates = 2;
            var startState = 0;
            var acceptingStates = new[] { 0,0,0 };
            var transitions = new Tuple<int, int, char>[]
            {
                new Tuple<int, int, char>(0,1,'0'),
                new Tuple<int, int, char>(0,1,'1'),
                new Tuple<int, int, char>(1,1,'0'),
                new Tuple<int, int, char>(1,1,'1'),
            };

            var fsm = FSM.Create(alphabet, numStates, acceptingStates, startState, transitions);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestConstruction_NegativeStartStateInTransition()
        {
            var alphabet = Alphabet.Create(new[] { '0', '1' });
            var numStates = 2;
            var startState = 0;
            var acceptingStates = new[] { 0 };
            var transitions = new Tuple<int, int, char>[]
            {
                new Tuple<int, int, char>(-1,1,'0'),
                new Tuple<int, int, char>(0,1,'1'),
                new Tuple<int, int, char>(1,1,'0'),
                new Tuple<int, int, char>(1,1,'1'),
            };

            var fsm = FSM.Create(alphabet, numStates, acceptingStates, startState, transitions);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestConstruction_OutOfRangeStartStateInTransition()
        {
            var alphabet = Alphabet.Create(new[] { '0', '1' });
            var numStates = 2;
            var startState = 0;
            var acceptingStates = new[] { 0 };
            var transitions = new Tuple<int, int, char>[]
            {
                new Tuple<int, int, char>(2,1,'0'),
                new Tuple<int, int, char>(0,1,'1'),
                new Tuple<int, int, char>(1,1,'0'),
                new Tuple<int, int, char>(1,1,'1'),
            };

            var fsm = FSM.Create(alphabet, numStates, acceptingStates, startState, transitions);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestConstruction_NegativeEndStateInTransition()
        {
            var alphabet = Alphabet.Create(new[] { '0', '1' });
            var numStates = 2;
            var startState = 0;
            var acceptingStates = new[] { 0 };
            var transitions = new Tuple<int, int, char>[]
            {
                new Tuple<int, int, char>(0,-1,'0'),
                new Tuple<int, int, char>(0,1,'1'),
                new Tuple<int, int, char>(1,1,'0'),
                new Tuple<int, int, char>(1,1,'1'),
            };

            var fsm = FSM.Create(alphabet, numStates, acceptingStates, startState, transitions);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestConstruction_OutOfRangeEndStateInTransition()
        {
            var alphabet = Alphabet.Create(new[] { '0', '1' });
            var numStates = 2;
            var startState = 0;
            var acceptingStates = new[] { 0 };
            var transitions = new Tuple<int, int, char>[]
            {
                new Tuple<int, int, char>(0,2,'0'),
                new Tuple<int, int, char>(0,1,'1'),
                new Tuple<int, int, char>(1,1,'0'),
                new Tuple<int, int, char>(1,1,'1'),
            };

            var fsm = FSM.Create(alphabet, numStates, acceptingStates, startState, transitions);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestConstruction_WrongSymbolInTransition()
        {
            var alphabet = Alphabet.Create(new[] { '0', '1' });
            var numStates = 2;
            var startState = 0;
            var acceptingStates = new[] { 0 };
            var transitions = new Tuple<int, int, char>[]
            {
                new Tuple<int, int, char>(0,1,'A'),
                new Tuple<int, int, char>(0,1,'1'),
                new Tuple<int, int, char>(1,1,'0'),
                new Tuple<int, int, char>(1,1,'1'),
            };

            var fsm = FSM.Create(alphabet, numStates, acceptingStates, startState, transitions);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestConstruction_NullAlphabet()
        {            
            var numStates = 2;
            var startState = 0;
            var acceptingStates = new[] { 0 };
            var transitions = new Tuple<int, int, char>[]
            {
                new Tuple<int, int, char>(-1,1,'0'),
                new Tuple<int, int, char>(0,1,'1'),
                new Tuple<int, int, char>(1,1,'0'),
                new Tuple<int, int, char>(1,1,'1'),
            };

            var fsm = FSM.Create(null, numStates, acceptingStates, startState, transitions);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestConstruction_NoStates()
        {
            var alphabet = Alphabet.Create(new[] { '0', '1' });
            var numStates = 0;

            var fsm = FSM.Create(alphabet, numStates, null, 0, null);

            Assert.AreEqual(alphabet, fsm.Alphabet);
            Assert.AreEqual(0, fsm.States.Length);
            Assert.AreEqual(0, fsm.AcceptedStates.Length);
            Assert.AreEqual(null, fsm.StartingState);
            Assert.AreEqual(0, fsm.Transitions.Length);
            Assert.AreEqual(FSMType.NonDeterministic, fsm.Type);
        }

        [TestMethod]
        public void TestIsND_NotEnoughTransitions()
        {
            var alphabet = Alphabet.Create(new[] { '0', '1' });
            var numStates = 2;
            var startState = 0;
            var acceptingStates = new[] { 0 };
            var transitions = new Tuple<int, int, char>[]
            {
                new Tuple<int, int, char>(0,1,'0'),
                new Tuple<int, int, char>(0,1,'1'),
                new Tuple<int, int, char>(1,1,'1'), //Missing transition from 1 with '0' -> ND
            };

            var fsm = FSM.Create(alphabet, numStates, acceptingStates, startState, transitions);

            Assert.AreEqual(FSMType.NonDeterministic, fsm.Type);
        }

        [TestMethod]
        public void TestIsND_TooManyTransitions()
        {
            var alphabet = Alphabet.Create(new[] { '0', '1' });
            var numStates = 2;
            var startState = 0;
            var acceptingStates = new[] { 0 };
            var transitions = new Tuple<int, int, char>[]
            {
                new Tuple<int, int, char>(0,1,'0'),
                new Tuple<int, int, char>(0,1,'1'),
                new Tuple<int, int, char>(1,1,'0'),
                new Tuple<int, int, char>(1,1,'1'),
                new Tuple<int, int, char>(1,0,'1') //Double transition from 1 with '1' -> ND
            };

            var fsm = FSM.Create(alphabet, numStates, acceptingStates, startState, transitions);

            Assert.AreEqual(FSMType.NonDeterministic, fsm.Type);
        }



    }
}
