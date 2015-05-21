using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLData.Classification;

namespace BLSpec.Plugins
{
    public class FilterOutMostOfTheData
#if DEBUG
 : IExternalCommand
#endif
    {
        public void Execute(BLData.BLModel model, UIHelper ui)
        {
            var deleted = new List<Guid>();
            using (var txn = model.BeginTansaction("Deletion"))
            {
                foreach (var item in model.Get<BLData.Classification.BLClassificationItem>().ToList())
                {
                    if (filter.Contains(item.Name) || 
                        item.ChildrenDeep.Any(c => filter.Contains(c.Name)) ||
                        item.ParentDeep.Any(c => filter.Contains(c.Name))) 
                        continue;

                    model.Delete(item);
                    deleted.Add(item.Id);
                }

                //purge classification root items
                foreach (var cls in model.Get<BLClassification>())
                {
                    foreach (var d in deleted)
                    {
                        cls.RootItemIDs.Remove(d);
                    }
                }

                txn.Commit();
            }
        }

        public string Name
        {
            get { return "Filter out most of the classes"; }
        }

        public Guid ID
        {
            get { return new Guid("C28D5329-A637-4F95-9D84-62D354E8D4A8"); }
        }

        private string[] filter = new string [] { "IfcWall", "IfcMaterial", "IfcSlab", "IfcWindow", "IfcDoor" }; 
    }
}
