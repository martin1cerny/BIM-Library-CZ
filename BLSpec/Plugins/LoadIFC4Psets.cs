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
            //create classification strcture based on the inheritance of products
            var productTypes = typeof(Xbim.Ifc2x3.SharedBldgElements.IfcWall).Assembly.GetTypes().Where(t => typeof(IfcProduct).IsAssignableFrom(t));
            var tree = new Tree();

            //build type tree
            foreach (var type in productTypes)
            {
                tree.AddNode(new Node() { type = type}, typeof(IfcProduct));
            }
            var roots = tree.Roots;

            using (var txn = model.BeginTansaction("Creation of classification and import of property sets"))
            {
                try
                {
                    var classification = model.New<BLClassification>(c => { c.Name = "IFC 2x3";});

                    //create classification structure from type tree
                    foreach (var root in roots)
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
                        Title = "Uložit BIM specifikaci...",
                        CheckFileExists = true,
                        CheckPathExists = true,
                        Multiselect = true,
                        ShowReadOnly = false,
                    };

                    if (ui.ShowDialog(dlg) == true)
                    {
                        foreach (var file in dlg.FileNames)
                            mgr.Load(file);

                        if (mgr.DefinitionSets.Any())
                        {
                            mgr.SetModel(model);
                        }

                        //assign property sets to classification items

                        txn.Commit();
                        return;
                    }
                    else
                    {
                        txn.Commit();
                        //txn.RollBack();
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

        private BLClassificationItem AddClassificationItem(Node node, BLClassificationItem parent)
        {
            var name = node.type.Name.Substring(3); //strip off 'Ifc'
            name = Regex.Replace(name, @"([a-z])([A-Z])", "$1 $2").Trim();

            var item = _model.New<BLClassificationItem>(ci => { ci.Name = name; if (parent != null) ci.ParentID = parent.Id; });
            foreach (var child in node.children)
            {
                AddClassificationItem(child, item);
            }
            return item;
        }

        public string Name
        {
            get { return "..Import.IFC4 Property Sets"; }
        }


        public Guid ID
        {
            get { return new Guid("3EA15F8F-32B2-49E8-ABF8-F5F094E8FBB3"); }
        }

        private class Node {
            public Type type;
            public Node parent;
            public List<Node> children = new List<Node>();
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
                    parent = new Node() { type = node.type.BaseType };
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
