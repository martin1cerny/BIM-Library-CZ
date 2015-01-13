using System;
using System.IO;
using System.Linq;
using BLData;
using BLData.Classification;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BLTests
{
    [TestClass]
    public class SerializationTests
    {
        [TestMethod]
        public void SerializeModel()
        {
            var model = new BLModel();
            using (var txn = model.BeginTansaction("Creation of classification items"))
            {
                var c1 = model.New<BLClassificationItem>(c => c.Name = "A");
                var c2 = model.New<BLClassificationItem>(c => c.Name = "B");
                var c3 = model.New<BLClassificationItem>(c => c.Name = "C");

                c3.ParentID = c2.Id;
                c2.ParentID = c1.Id;

                txn.Commit();
            }

            var path = "model.xml";

            using (var file = File.Create(path))
            {
                model.Save(file);
                file.Close();
            }
            

            Assert.IsTrue(File.Exists(path));

            using (var file = File.Open(path, FileMode.Open))
            {
                model = BLModel.Open(file);
                file.Close();
            }

            var clss = model.Get<BLClassificationItem>();
            Assert.AreEqual(3, clss.Count());

            var cc1 = model.Get<BLClassificationItem>(c => c.Name == "A").FirstOrDefault();
            Assert.IsNotNull(cc1);
        }
    }
}
