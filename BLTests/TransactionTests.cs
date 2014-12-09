using System;
using System.Linq;
using BLData;
using BLData.Classification;
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

        [TestMethod]
        public void ObjectsTest()
        {
            var model = new BLModel();

            var test = false;
            try
            {
                model.New<BLClassificationItem>(ci => {
                    ci.Name = "A";
                });
            }
            catch (BLData.Exceptions.NoTransactionException e)
            {
                test = true;
            }
            Assert.IsTrue(test, "Exception should have been trown out outside transaction.");
            
            using (var txn = model.BeginTansaction("Classification creation"))
            {
                model.New<BLClassificationItem>(ci =>
                {
                    ci.Name = "A";
                });
                txn.Commit();
            }

            var ci1 = model.Get<BLClassificationItem>(c => c.Name == "A").FirstOrDefault();
            Assert.IsNotNull(ci1, "There should be classification item existing in the model");

            using (var txn = model.BeginTansaction("Classification creation rolled back"))
            {
                ci1.Name = "B";
                txn.RollBack();
            }

            var ci2 = model.Get<BLClassificationItem>(c => c.Name == "B").FirstOrDefault();
            Assert.IsNull(ci2, "There shouldn't be classification item 'B' existing in the model");

        }

        [TestMethod]
        public void RollingBackTest()
        {
            var model = new BLModel();
            using (var txn = model.BeginTansaction("1"))
            {
                model.New<BLClassification>(c => c.Name = "A");
                txn.Commit();
            }
            var cls = model.Get<BLClassification>().FirstOrDefault();
            Assert.IsNotNull(cls);

            using (var txn = model.BeginTansaction("2"))
            {
                cls.Name = "B";
                txn.Commit();
            }

            using (var txn = model.BeginTansaction("3"))
            {
                cls.Name = "C";
                txn.Commit();
            }

            using (var txn = model.BeginTansaction("4"))
            {
                cls.Name = "D";
                txn.Commit();
            }

            using (var txn = model.BeginTansaction("5"))
            {
                cls.Name = "E";
                txn.Commit();
            }

            Assert.AreEqual("E", cls.Name);
            Assert.IsTrue(model.Session.HasUndo);

            model.Session.Undo();
            Assert.AreEqual("D", cls.Name);

            model.Session.Undo();
            Assert.AreEqual("C", cls.Name);
            Assert.AreEqual(3, model.Session.UndoTransactions.Count());

            model.Session.DiscardHistory();
            Assert.IsFalse(model.Session.CanUndo);
        }
    }
}
