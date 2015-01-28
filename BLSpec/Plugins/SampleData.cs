using BLData;
using BLData.Classification;
using BLData.PropertySets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLSpec.Plugins
{
    class SampleData: IExternalCommand
    {
        public void Execute(BLData.BLModel model, UIHelper ui)
        {
            using (var txn = model.BeginTansaction("Sample data creation"))
            {
                try
                {
                    var cls = model.New<BLClassification>(c => { c.Name = "Classification A"; c.Definition = "Sample classification"; });
                    var item = model.New<BLClassificationItem>(i => {
                        i.Name = "Item A";
                        i.Definition = "Item A description";
                        i.NameAliases.Add(model.New<NameAlias>(a => {
                            a.Lang = "cs-cz";
                            a.Value = "Položka A";
                        }));
                        i.DefinitionAliases.Add(model.New<NameAlias>(a =>
                        {
                            a.Lang = "cs-cz";
                            a.Value = "Popis položky A";
                        }));
                    });
                    cls.RootItemIDs.Add(item.Id);

                    var pset = model.New<PropertySetDef>(p => {
                        p.Name = "Sample set";
                        p.DefinitionAliases.Add(model.New<NameAlias>(a => {
                            a.Lang = "cs-cz";
                            a.Value = "Popis ukazkove sady parametru";
                        }));
                        p.PropertyDefinitions.Add(model.New<PropertyDef>(d => {
                            d.Name = "Sample property";
                            d.Definition = "Definition of sample property";
                            d.NameAliases.Add(model.New<NameAlias>(a =>
                            {
                                a.Lang = "cs-cz";
                                a.Value = "Ukázkový parametr";
                            }));
                            d.DefinitionAliases.Add(model.New<NameAlias>(a =>
                            {
                                a.Lang = "cs-cz";
                                a.Value = "Popis ukázkového parametru";
                            }));
                        }));
                    });
                    item.DefinitionSetIds.Add(pset.Id);

                    txn.Commit();
                }
                catch (Exception)
                {
                    txn.RollBack();
                    throw;
                }
            }
        }

        public string Name
        {
            get { return "Create data"; }
        }

        public Guid ID
        {
            get { return new Guid("913310A7-D500-4523-97B3-814709AB9465"); }
        }
    }
}
