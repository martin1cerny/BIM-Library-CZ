using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLSpec;
using BLData.Classification;
using BLData;
using BLData.PropertySets;
using Microsoft.Win32;
using System.Windows;
using System.Text.RegularExpressions;

namespace Xbim.ExpressParser
{
    public class CreateClassificationFromSchema : IExternalCommand
    {
        private BLModel _model;

        public void Execute(BLData.BLModel model, UIHelper ui)
        {
            _model = model;
            using (var file = new MemoryStream(Schemas.SchemasDefinitions.IFC4))
            {
                using (var txn = model.BeginTansaction("Creation of the IFC structure"))
                {
                    try
                    {
                        var scanner = new Scanner(file);
                        var parser = new Parser(scanner, IfcVersionEnum.IFC4, model);
                        var result = parser.Parse();

                        if (parser.Output != null)
                            parser.Output.Close();
                        file.Close();


                        if (scanner.Errors.Any())
                        {
                            if (!result)
                            {
                                Debug.WriteLine("Errors occured during the processing. Output might be incomplete or eroneous.");
                                foreach (var err in scanner.Errors)
                                {
                                    Debug.WriteLine(err);
                                }
                            }
                            else
                                Debug.WriteLine("Errors occured during the processing but all of them had been catched.");
                        }
                        else
                            Debug.WriteLine("Completed with no errors.");

                        if (!result)
                        {
                            ui.ShowMessage("Creation failed", "Creation of the structure failed. Operation canceled.");
                            var err = String.Concat(scanner.Errors);
                            txn.RollBack();
                            return;
                        }
                        
                         var classification = model.New<BLClassification>(c => { c.Name = "IFC 4";});

                    //create classification structure from type tree
                         var rootTypes = new[] { "IfcProduct", "IfcMaterial", "IfcSystem"};
                    foreach (var root in parser.Tree.Where(n => rootTypes.Contains(n.Name)))
                    {
                        var item = AddClassificationItem(root, null);
                        classification.RootItemIDs.Add(item.Id);
                    }

                    //load property sets, add them to the model
                    var mgr = new DefinitionsManager<PropertySetDef>(BLData.PropertySets.Version.IFC4);
                    var dlg = new OpenFileDialog()
                    {
                        AddExtension = true,
                        DefaultExt = ".xml",
                        Filter = "PSet definitions|*.xml",
                        Title = "Otevřít definice sad paametrů...",
                        CheckFileExists = true,
                        CheckPathExists = true,
                        Multiselect = true,
                        ShowReadOnly = false,
                    };

                    if (ui.ShowDialog(dlg) == true)
                    {
                        var msg = "";
                        foreach (var fileName in dlg.FileNames)
                            mgr.Load(fileName);

                        if (mgr.DefinitionSets.Any())
                        {
                            mgr.SetModel(model);

                            //assign property sets to classification items
                            foreach (var definitionSet in mgr.DefinitionSets)
                            {
                                foreach (var appCls in definitionSet.ApplicableClasses)
                                {
                                    var cName = appCls.ClassName;
                                    var cPredefType = appCls.PredefinedType;

                                    var cItems = _model.Get<BLClassificationItem>();
                                    var item = _model.Get<BLClassificationItem>(ci => ci.Name == cName).FirstOrDefault();
                                    //if predefined type is not UPPERCASE it is ObjectType, not PredefinedType
                                    if (!String.IsNullOrEmpty(cPredefType) && item != null)
                                    {
                                        var pItem = item.Children.FirstOrDefault(ci => ci.Name == cPredefType);
                                        if (pItem != null) item = pItem;
                                        if (pItem == null && cPredefType.ToUpper() == cPredefType)
                                            msg += String.Format("Class {0} {1} was not found. \n", cName, cPredefType);
                                    }
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
                            File.WriteAllText("Mismatch.txt", msg);

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
        }

        private BLClassificationItem AddClassificationItem(Node node, BLClassificationItem parent)
        {
            var item = _model.New<BLClassificationItem>(ci => { ci.Name = node.Name; if (parent != null) ci.ParentID = parent.Id; });
            item.NameAliases.Add(_model.New<NameAlias>(na =>
            {
                na.Lang = "en-US";

                var alias = node.Name;
                if (alias.StartsWith("Ifc"))
                {
                    alias = alias.Substring(3); //strip off 'Ifc'
                    alias = Regex.Replace(alias, @"([a-z])([A-Z])", "$1 $2").Trim();
                }
                na.Value = alias;
            }));
            
            //add all children
            if (node.Children != null)
                foreach (var child in node.Children)
                {
                    AddClassificationItem(child, item);
                }

            //add all predefined type leafs
            if (node.PredefinedTypes != null)
                foreach (var pt in node.PredefinedTypes)
                    _model.New<BLClassificationItem>(ci => { ci.Name = pt; ci.ParentID = item.Id; });

            return item;
        }

        public string Name
        {
            get { return "Create IFC4 structure"; }
        }

        public Guid ID
        {
            get { return new Guid("7464EFB2-ADAD-4D98-AB00-6C341AD1DA91"); }
        }
    }
}
