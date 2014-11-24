using System;
using BLData;
using BLData.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BLTests
{
    [TestClass]
    public class TransactionTests
    {
        [TestMethod]
        public void ListTest()
        {
            var model = new BLModel();
            var list = new BList<string>(model);

            using (var txn = model.BeginTansaction("Test 1"))
            {
                list.Add("A");
                list.Add("B");
                list.Add("C");
                Assert.AreEqual(3, list.Count);

                txn.RollBack();
            }
            Assert.AreEqual(0, list.Count, "List should be empty as transaction has been rolled back");

            using (var txn = model.BeginTansaction("Test 2"))
            {
                list.Add("A");
                list.Add("B");
                list.Add("C");
                Assert.AreEqual(3, list.Count);

                txn.Commit();
            }
            Assert.AreEqual(3, list.Count);

            using (var txn = model.BeginTansaction("Test 1"))
            {
                list[0] = "Abeceda";
                txn.RollBack();
            }
            Assert.AreEqual(list[0], "A");

            using (var txn = model.BeginTansaction("Test 1"))
            {
                list[0] = "Abeceda";
                txn.Commit();
            }
            Assert.AreEqual(list[0], "Abeceda");

            var wasException = false;
            try
            {
                list.Add("Outside transaction");
            }
            catch (NoTransactionException)
            {
                wasException = true;
            }
            Assert.IsTrue(wasException, "Transaction should have been inforced");


            wasException = false;
            using (var txn = model.BeginTansaction("A"))
            {
                try
                {
                    using (var txn2 = model.BeginTansaction("B"))
                    {
                    }
                }
                catch (TransactionNotFinishedException)
                {
                    wasException = true;
                }
            
            }
            Assert.IsTrue(wasException, "Nested transaction sould fail.");

        }
    }
}
