using NUnit.Framework;
using UnityEngine;

namespace Tests.Editor {
    public class MovementTests {
        [Test]
        public void MoveXTest() {
            Movement m = new Movement(1,0,0);
            Assert.AreEqual(1, m.Calculate(1,0,false,true,1).x);
        }

        [Test]
        public void MoveZTest() {
            Movement m = new Movement(1,0,0);
            Assert.AreEqual(1, m.Calculate(0,1,false,true,1).z);
        }

        [Test]
        public void FallingTest() {
            Movement m = new Movement(0,-10,0);
            for (int i = 0; i < 10; i++) {
                Assert.AreEqual(-0.1f * (i+1), m.Calculate(0,0,false,false,0.1f).y);
            }
        }

        [Test]
        public void JumpingTest() {
            Movement m = new Movement(0,-10,5);
            Assert.IsTrue(Mathf.Approximately(0.9f, m.Calculate(0,0,true,true,0.1f).y));
        }

        [Test]
        public void JumpingNotGroundedTest() {
            Movement m = new Movement(0,-10,5);
            Assert.AreEqual(-0.1f, m.Calculate(0,0,true,false,0.1f).y);
        }
    }
}
