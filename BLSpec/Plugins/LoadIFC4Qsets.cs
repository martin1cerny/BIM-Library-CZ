using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using BLData;
using BLData.Classification;
using BLData.PropertySets;
using Microsoft.Win32;

namespace BLSpec.Plugins
{
    public class LoadIFC4Qsets 
#if DEBUG
        : IExternalCommand
#endif
    {
        private BLModel _model;
        public void Execute(BLData.BLModel model, UIHelper ui)
        {
            _model = model;
            var origLang = _model.Information.Lang;
            ////create classification strcture based on the inheritance of products
            //var productTypes = typeof(Xbim.Ifc2x3.SharedBldgElements.IfcWall).Assembly.GetTypes().Where(t => typeof(IfcProduct).IsAssignableFrom(t));
            //var tree = new Tree();

            ////build type tree
            //foreach (var type in productTypes)
            //{
            //    tree.AddNode(new Node(type), typeof(IfcProduct));
            //}
            //var roots = tree.Roots;

            using (var txn = model.BeginTansaction("Creation of classification and import of property sets"))
            {
                try
                {
                    //var classification = model.New<BLClassification>(c => { c.Name = "IFC 2x3";});

                    ////create classification structure from type tree
                    //foreach (var root in roots)
                    //{
                    //    var item = AddClassificationItem(root, null);
                    //    classification.RootItemIDs.Add(item.Id);
                    //}

                    _model.Information.Lang = "en-US";

                    //load property sets, add them to the model
                    var mgr = new DefinitionsManager<QtoSetDef>(BLData.PropertySets.Version.IFC4);
                    var dlg = new OpenFileDialog()
                    {
                        AddExtension = true,
                        DefaultExt = ".xml",
                        Filter = "Quantity Set definitions|*.xml",
                        Title = "Otevřít definice sad kvantit...",
                        CheckFileExists = true,
                        CheckPathExists = true,
                        Multiselect = true,
                        ShowReadOnly = false,
                    };

                    if (ui.ShowDialog(dlg) == true)
                    {
                        var msg = "";
                        foreach (var file in dlg.FileNames)
                            mgr.Load(file);

                        if (mgr.DefinitionSets.Any())
                        {
                            mgr.SetModel(model);

                            //assign property sets to classification items
                            foreach (var definitionSet in mgr.DefinitionSets)
                            {
                                //set en-US language aliases
                                definitionSet.LocalizedDefinition = definitionSet.Definition;
                                definitionSet.LocalizedName = GetHumanName(definitionSet.Name);
                                SetPropertyAliases(definitionSet.QuantityDefinitions);

                                //assign to classification items
                                foreach (var appCls in definitionSet.ApplicableClasses)
                                {
                                    var cName = appCls.ClassName;
                                    var cPredefType = appCls.PredefinedType;

                                    var cItems = _model.Get<BLClassificationItem>();
                                    var item = _model.Get<BLClassificationItem>(ci => ci.Name == cName).FirstOrDefault();
                                    if (!String.IsNullOrEmpty(cPredefType) && item != null)
                                        item = item.Children.FirstOrDefault(ci => ci.Name == cPredefType);
                                    if (item != null)
                                    {
                                        item.DefinitionSetIds.Add(definitionSet.Id);
                                    }
                                    else
                                    {
                                        //there is either problem with IFC2x3 and IFC4 type mismatch or it is not product related PSet
                                        msg += String.Format("Class {0} {1} was not found. \n", cName, cPredefType);
                                        //_model.Delete(definitionSet);
                                    }
                                } 
                            }
                        }

                        if (!String.IsNullOrEmpty(msg))
                            MessageBox.Show(msg, "Varovani", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK);

                        _model.Information.Lang = origLang;
                        txn.Commit();
                        return;
                    }
                    else
                    {
                        //Roll back if the dialog is closed or canceled.
                        txn.RollBack();
                        return;
                    }
                }
                catch (Exception)
                {
                    txn.RollBack();   
                    throw;
                }
            }
        }

        private string GetHumanName(string name)
        {
            var result = name;
            //trim starting Pset_
            if (name.IndexOf("Qto_") == 0)
            {
                result = result.Substring(4);
            }

            //split camel case
            result = Regex.Replace(result, "(?<=[a-z])([A-Z])", " $1", RegexOptions.Compiled).Trim();
            return result;
        }

        private void SetPropertyAliases(IEnumerable<QtoDef> set)
        {
            foreach (var quant in set)
            {
                quant.LocalizedDefinition = quant.Definition;
                quant.LocalizedName = GetHumanName(quant.Name);
            }
        }

        public string Name
        {
            get { return "Import IFC4 Quantity Sets"; }
        }


        public Guid ID
        {
            get { return new Guid("3EA15F8F-32B2-49E8-ABF8-F5A094E8FBB3"); }
        }
    }
}
