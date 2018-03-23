using System.Collections.Generic;
using Itinero.Optimization.Abstract.Tours.Sequences;
using NUnit.Framework;

namespace Itinero.Optimization.Test.Abstract.Tours.Sequences
{
    /// <summary>
    /// Tests related to the sequence structure.
    /// </summary>
    [TestFixture]
    public class SequenceTests
    {
        /// <summary>
        /// Tests the to string method.
        /// </summary>
        [Test]
        public void TestToString()
        {
            var s = new Sequence(new int[] { 0 }, 0, 1);
            Assert.AreEqual("0", s.ToString());
            s = new Sequence(new int[] { 0, 1 }, 0, 2);
            Assert.AreEqual("0->1", s.ToString());
            s = new Sequence(new int[] { 0, 1, 2, 3 }, 0, 4);
            Assert.AreEqual("0->1->2->3", s.ToString());
            s = new Sequence(new int[] { 0, 1, 2, 3 }, 2, 2);
            Assert.AreEqual("2->3", s.ToString());
            s = new Sequence(new int[] { 0, 1, 2, 3 }, 2, 4);
            Assert.AreEqual("2->3->0->1", s.ToString());
        }

        /// <summary>
        /// Tests enumerating sequences of size 1.
        /// </summary>
        [Test]
        public void TestSize1()
        {
            var s = new Sequence(new int[] { 0, 1, 2, 3, 4, 5 }, 0, 6);
            var enumeration = new List<Sequence>(s.SubSequences(1, 1));
            Assert.AreEqual(6, enumeration.Count);
            Assert.AreEqual("0", enumeration[0].ToString());
            Assert.AreEqual("1", enumeration[1].ToString());
            Assert.AreEqual("2", enumeration[2].ToString());
            Assert.AreEqual("3", enumeration[3].ToString());
            Assert.AreEqual("4", enumeration[4].ToString());
            Assert.AreEqual("5", enumeration[5].ToString());
        }

        /// <summary>
        /// Tests enumerating sequences of size 2.
        /// </summary>
        [Test]
        public void TestSize2()
        {
            var s = new Sequence(new int[] { 0, 1, 2, 3, 4, 5 }, 0, 6);
            var enumeration = new List<Sequence>(s.SubSequences(2, 2));
            Assert.AreEqual(5, enumeration.Count);
            Assert.AreEqual("0->1", enumeration[0].ToString());
            Assert.AreEqual("1->2", enumeration[1].ToString());
            Assert.AreEqual("2->3", enumeration[2].ToString());
            Assert.AreEqual("3->4", enumeration[3].ToString());
            Assert.AreEqual("4->5", enumeration[4].ToString());
        }
    }
}