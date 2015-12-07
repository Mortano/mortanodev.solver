using Microsoft.VisualStudio.TestTools.UnitTesting;

using mortanodev.solver.backend;
using mortanodev.solver.backend.Util;
using System;
using System.Collections.Generic;
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
            Assert.AreEqual(startState, fsm.StartingState.Id);
            Assert.AreEqual(acceptingStates.Length, fsm.AcceptedStates.Length);

            for(int i = 0; i < acceptingStates.Length; i++)
            {
                var expected = acceptingStates[i];
                var actual = fsm.AcceptedStates[i];

                Assert.AreEqual(expected, actual.Id);
            }

            Assert.AreEqual(transitions.Length, fsm.Transitions.Length);
            
            for(int i = 0; i < transitions.Length; i++)
            {
                var expected = transitions[i];
                var actual = fsm.Transitions[i];

                Assert.AreEqual(fsm.States.First(s => s.Id == expected.Item1), actual.Start);
                Assert.AreEqual(fsm.States.First(s => s.Id == expected.Item2), actual.End);
                Assert.AreEqual(expected.Item3, actual.Symbol);
            }

            Assert.AreEqual(FsmType.Deterministic, fsm.Type);

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
            Assert.AreEqual(FsmType.NonDeterministic, fsm.Type);
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

            Assert.AreEqual(FsmType.NonDeterministic, fsm.Type);
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

            Assert.AreEqual(FsmType.NonDeterministic, fsm.Type);
        }

        [TestMethod]        
        public void TestMinimize_SimpleCase()
        {
            var alphabet = Alphabet.Create(new[] { 'a', 'b' });
            var numStates = 4;
            var startState = 0;
            var acceptingStates = new[] { 2, 3 };
            var transitions = new Tuple<int, int, char>[]
            {
                new Tuple<int, int, char>(0,1,'b'),
                new Tuple<int, int, char>(0,2,'a'),
                new Tuple<int, int, char>(1,1,'b'),
                new Tuple<int, int, char>(1,2,'a'),
                new Tuple<int, int, char>(2,3,'a'),
                new Tuple<int, int, char>(2,3,'b'),
                new Tuple<int, int, char>(3,2,'a'),
                new Tuple<int, int, char>(3,2,'b'),
            };

            var fsm = FSM.Create(alphabet, numStates, acceptingStates, startState, transitions);

            fsm.Minimize();            

            Assert.AreEqual(2, fsm.States.Length);
            Assert.AreEqual(1, fsm.AcceptedStates.Length);
            Assert.AreEqual(1, fsm.AcceptedStates[0].Id);

            Assert.AreEqual(4, fsm.Transitions.Length);

            var expectedTransitions = new FSM.Transition[]
            {
                new FSM.Transition(fsm.States[0], fsm.States[0], 'b'),
                new FSM.Transition(fsm.States[0], fsm.States[1], 'a'),
                new FSM.Transition(fsm.States[1], fsm.States[1], 'a'),
                new FSM.Transition(fsm.States[1], fsm.States[1], 'b'),
            };

            Assert.IsTrue(expectedTransitions.PermutationEquals(fsm.Transitions));
        }

        [TestMethod]
        public void TestMinimize_ComplexCase()
        {
            var alphabet = Alphabet.Create(new[] { 'a', 'b' });
            var numStates = 8;
            var startState = 0;
            var acceptingStates = new[] { 2, 7 };
            //Lots of transitions
            var transitions = new Tuple<int, int, char>[]
            {
                new Tuple<int, int, char>(0, 1, 'a'),
                new Tuple<int, int, char>(0, 4, 'b'),

                new Tuple<int, int, char>(1, 5, 'a'),
                new Tuple<int, int, char>(1, 2, 'b'),

                new Tuple<int, int, char>(2, 3, 'a'),
                new Tuple<int, int, char>(2, 6, 'b'),

                new Tuple<int, int, char>(3, 3, 'a'),
                new Tuple<int, int, char>(3, 3, 'b'),

                new Tuple<int, int, char>(4, 1, 'a'),
                new Tuple<int, int, char>(4, 4, 'b'),

                new Tuple<int, int, char>(5, 1, 'a'),
                new Tuple<int, int, char>(5, 4, 'b'),

                new Tuple<int, int, char>(6, 3, 'a'),
                new Tuple<int, int, char>(6, 7, 'b'),

                new Tuple<int, int, char>(7, 3, 'a'),
                new Tuple<int, int, char>(7, 6, 'b'),
            };

            var fsm = FSM.Create(alphabet, numStates, acceptingStates, startState, transitions);

            fsm.Minimize();
            
            Assert.AreEqual(5, fsm.States.Length);
            Assert.AreEqual(0, fsm.StartingState.Id);
            Assert.AreEqual(1, fsm.AcceptedStates.Length);
            Assert.AreEqual(4, fsm.AcceptedStates.First().Id);
            
            var expectedTransitions = new FSM.Transition[]
            {
                new FSM.Transition(fsm.States[0], fsm.States[2], 'a'),
                new FSM.Transition(fsm.States[0], fsm.States[0], 'b'),

                new FSM.Transition(fsm.States[1], fsm.States[1], 'a'),
                new FSM.Transition(fsm.States[1], fsm.States[1], 'b'),

                new FSM.Transition(fsm.States[2], fsm.States[0], 'a'),
                new FSM.Transition(fsm.States[2], fsm.States[4], 'b'),

                new FSM.Transition(fsm.States[3], fsm.States[1], 'a'),
                new FSM.Transition(fsm.States[3], fsm.States[4], 'b'),

                new FSM.Transition(fsm.States[4], fsm.States[1], 'a'),
                new FSM.Transition(fsm.States[4], fsm.States[3], 'b'),
            };

            Func<IEnumerable<FSM.Transition>, string> printTransitions = r =>
            {
                string ret = "{\n";
                foreach(var transition in r)
                {
                    ret += "(" + transition.Start.Id + ";" + transition.End.Id + ";'" + transition.Symbol + "')\n";
                }
                ret += "}";
                return ret;
            };

            Assert.IsTrue(expectedTransitions.PermutationEquals(fsm.Transitions), "Expected " + printTransitions(expectedTransitions) + " but got " + printTransitions(fsm.Transitions));
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void TestMinimize_ND()
        {
            var alphabet = Alphabet.Create(new[] { 'a', 'b' });
            var numStates = 2;
            var startState = 0;
            var acceptingStates = new[] { 1 };
            var transitions = new Tuple<int, int, char>[] 
            {
                new Tuple<int, int, char>(0, 1, 'a')
            };

            var fsm = FSM.Create(alphabet, numStates, acceptingStates, startState, transitions);

            fsm.Minimize();
        }

        [TestMethod]
        public void TestMinimize_AlreadyMinimal()
        {
            var alphabet = Alphabet.Create(new[] { 'a', 'b' });
            var numStates = 4;
            var startState = 0;
            var acceptingStates = new[] { 2, 3 };
            var transitions = new Tuple<int, int, char>[]
            {
                new Tuple<int, int, char>(0,1,'b'),
                new Tuple<int, int, char>(0,2,'a'),
                new Tuple<int, int, char>(1,1,'b'),
                new Tuple<int, int, char>(1,2,'a'),
                new Tuple<int, int, char>(2,3,'a'),
                new Tuple<int, int, char>(2,3,'b'),
                new Tuple<int, int, char>(3,2,'a'),
                new Tuple<int, int, char>(3,2,'b'),
            };

            var fsm = FSM.Create(alphabet, numStates, acceptingStates, startState, transitions);

            fsm.Minimize();

            var statesAfterMinimizingOnce = fsm.States.Length;

            fsm.Minimize();

            Assert.AreEqual(statesAfterMinimizingOnce, fsm.States.Length);
        }

    }
}
