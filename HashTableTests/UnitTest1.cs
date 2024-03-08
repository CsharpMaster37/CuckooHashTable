using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using HashTablesLib;
using System.Collections;

namespace HashTableTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestCountInc()
        {
            CuckooHashTable<int, int> table = new CuckooHashTable<int, int>();
            table.Add(1, 1);
            Assert.AreEqual(1, table.Count);
            table.Add(2, 1);
            Assert.AreEqual(2, table.Count);
        }
        [TestMethod]
        public void TestCountDec()
        {
            CuckooHashTable<int, int> table = new CuckooHashTable<int, int>();
            table.Add(1, 1);
            table.Add(2, 1);
            table.Remove(1);
            Assert.AreEqual(1, table.Count);
        }

        [TestMethod]
        public void TestAddItem()
        {
            CuckooHashTable<int, int> table = new CuckooHashTable<int, int>();
            table.Add(1, 1);
            table.Add(3, 1);
            table.Add(7, 1);
            Assert.AreEqual(true, table.ContainsKey(1));
            Assert.AreEqual(true, table.ContainsKey(3));
            Assert.AreEqual(true, table.ContainsKey(7));
            Assert.AreEqual(false, table.ContainsKey(10));
        }
        [TestMethod]
        public void TestRemove()
        {
            CuckooHashTable<int, int> table = new CuckooHashTable<int, int>();
            table.Add(1, 1);
            table.Add(3, 1);
            table.Add(7, 1);
            table.Remove(1);
            Assert.AreEqual(false, table.ContainsKey(1));
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ContainsNullKey()
        {
            CuckooHashTable<string, int> table = new CuckooHashTable<string, int>();
            table.ContainsKey(null);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddNullKey()
        {
            CuckooHashTable<string, int> table = new CuckooHashTable<string, int>();
            table.Add(null,1);
        }
    }
}
