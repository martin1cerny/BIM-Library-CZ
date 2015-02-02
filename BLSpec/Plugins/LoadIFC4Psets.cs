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
using Xbim.Ifc2x3.Kernel;

namespace BLSpec.Plugins
{
    public class LoadIFC4Psets : IExternalCommand
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
                    var mgr = new DefinitionsManager<PropertySetDef>(BLData.PropertySets.Version.IFC4);
                    var dlg = new OpenFileDialog()
                    {
                        AddExtension = true,
                        DefaultExt = ".xml",
                        Filter = "Property Set definitions|*.xml",
                        Title = "Otevřít definice sad parametrů...",
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
                                SetPropertyAliases(definitionSet.PropertyDefinitions);

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
            if (name.IndexOf("Pset_") == 0)
            {
                result = result.Substring(5);
            }

            //split camel case
            result = Regex.Replace(result, "(?<=[a-z])([A-Z])", " $1", RegexOptions.Compiled).Trim();
            return result;
        }

        private void SetPropertyAliases(IEnumerable< PropertyDef> set)
        {
            foreach (var prop in set)
            {
                prop.LocalizedDefinition = prop.Definition;
                prop.LocalizedName = GetHumanName(prop.Name);

                //set complex properties
                var complex = prop.PropertyType.PropertyValueType as TypeComplexProperty;
                if (complex != null)
                    SetPropertyAliases(complex.Properties);
            }
        }

        private BLClassificationItem AddClassificationItem(Node node, BLClassificationItem parent)
        {
            var name = node.name != null ? node.name : node.type.Name;
            

            var item = _model.New<BLClassificationItem>(ci => { ci.Name = name; if (parent != null) ci.ParentID = parent.Id; });
            item.NameAliases.Add(_model.New<NameAlias>(na => {
                na.Lang = "en-US";

                var alias = name;
                if (name.StartsWith("Ifc"))
                {
                    alias = alias.Substring(3); //strip off 'Ifc'
                    alias = Regex.Replace(alias, @"([a-z])([A-Z])", "$1 $2").Trim();
                }
                na.Value = alias;
            }));
            foreach (var child in node.children)
            {
                AddClassificationItem(child, item);
            }
            return item;
        }

        public string Name
        {
            get { return "Import IFC4 Property Sets"; }
        }


        public Guid ID
        {
            get { return new Guid("3EA15F8F-32B2-49E8-ABF8-F5F094E8FBB3"); }
        }

        private class Node {
            public string name;
            public Type type;
            public Node parent;
            public List<Node> children = new List<Node>();
            private static IEnumerable<Type> elementTypes = typeof(Xbim.Ifc2x3.SharedBldgElements.IfcWall).Assembly.GetTypes().Where(t => typeof(IfcTypeProduct).IsAssignableFrom(t));


            public Node(Type type)
            {
                this.type = type;

                //use reflection to add predefined type children
                var info = type.GetProperty("PredefinedType");
                if (info == null)
                {
                    var elementType = elementTypes.FirstOrDefault(t => t.Name == type.Name + "Type");
                    if (elementType != null)
                    {
                        info = elementType.GetProperty("PredefinedType");
                    }
                }
                if (info != null)
                {
                    var enumType = info.PropertyType;
                    if (enumType.IsEnum)
                    {
                        var names = Enum.GetNames(enumType);
                        foreach (var name in names)
                            children.Add(new Node(enumType) { name = name });
                    }
                    //nullable type
                    else if (enumType.IsGenericType && enumType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        enumType = Nullable.GetUnderlyingType(enumType);
                        var names = Enum.GetNames(enumType);
                        foreach (var name in names)
                            children.Add(new Node(enumType) { name = name });
                    }
                    //this shouldn't happen
                    else
                        throw new Exception("Unexpected predefined type object type.");
                    
                }
            }
        }

        private class Tree : List<Node>{
            public void AddNode(Node node, Type rootType) {
                if (this.Contains(node)) return;
                if (this.Any(n => n.type == node.type)) return;
                if (node.type == rootType.BaseType) return;
                
                this.Add(node);

                var parent = this.FirstOrDefault(n => n.type == node.type.BaseType);
                if (parent == null && node.type.BaseType != null && node.type.BaseType != rootType.BaseType)
                {
                    parent = new Node(node.type.BaseType) { };
                    this.AddNode(parent, rootType);
                }
                if (parent != null)
                {
                    parent.children.Add(node);
                    node.parent = parent;
                }

            }

            public IEnumerable<Node> Roots
            {
                get { return this.Where(n => n.parent == null); }
            }
        }
    }
}
