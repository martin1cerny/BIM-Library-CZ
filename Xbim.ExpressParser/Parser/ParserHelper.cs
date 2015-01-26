using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QUT.Xbim.Gppg;
using System.Linq.Expressions;
using System.Reflection;
using System.Globalization;
using System.IO;
using System.CodeDom;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using BLData;

namespace Xbim.ExpressParser
{
    internal partial class Parser
    {
        Scanner _scanner;
        public TextWriter Output { get; set; }

        private Schemas.Schema _schema;
        private IfcVersionEnum _version;
        private BLModel _model;
        private Tree _tree = new Tree();

        internal Parser(Scanner lex, IfcVersionEnum version, BLModel model): base(lex)
        {
            _model = model;
            _scanner = lex;
            _version = version;

            switch (version)
            {
                case IfcVersionEnum.IFC2x3:
                    _schema = Schemas.Schema.LoadIFC2x3();
                    break;
                case IfcVersionEnum.IFC4:
                    _schema = Schemas.Schema.LoadIFC4();
                    break;
                default:
                    break;
            }
        }


        private Dictionary<string, IEnumerable<string>> _enumerations = new Dictionary<string,IEnumerable<string>>();
        private void CreateEnumeration(string name, IEnumerable<string> members)
        {
            _enumerations.Add(name, members);
        }

        private void CreateEntity(string name, IEnumerable<Section> sections)
        {
            var inheritance = sections.OfType<Inheritance>().FirstOrDefault();
            string parent = null;
            if (inheritance != null)
                parent = inheritance.SubtypeOf != null ? inheritance.SubtypeOf.FirstOrDefault() : null;

            var props = sections.OfType<PropertySection>().FirstOrDefault();
            IEnumerable<string> predefinedTypes = null;
            if (props != null)
            {
                var prop = props.FirstOrDefault(p => p.Name == "PredefinedType");
                if (prop != null)
                    predefinedTypes = _enumerations[prop.Type];
            }

            var node = _tree.GetOrCreate(name);
            if (parent != null)
            {
                var parentNode = _tree.GetOrCreate(parent);
                node.Parent = parentNode;
            }
            if (predefinedTypes != null)
                node.PredefinedTypes = predefinedTypes;
        }

        private void Finish()
        {
            _tree.SetAllChildren();

            var root = _tree.FirstOrDefault(n => n.Name == "IfcProduct");

            //create classification nodes from the tree
        }

        private class Node
        {
            public Node Parent { get; set; }
            public string Name { get; set; }
            public IEnumerable<string> PredefinedTypes { get; set; }
            public IEnumerable<Node> Children { get; set; }
        }

        private class Tree : List<Node>
        {
            public void SetAllChildren()
            {
                foreach (var node in this)
                {
                    node.Children = this.Where(n => n.Parent == node);
                }
            }

            public Node GetOrCreate(string name)
            {
                var node = this.FirstOrDefault(n => n.Name == name);
                if (node != null)
                    return node;
                node = new Node() { Name = name };
                this.Add(node);
                return node;
            }

            public IEnumerable<Node> Roots 
            {
                get
                {
                    return this.Where(n => n.Parent == null);
                }
            }
        }


    }

    public enum IfcVersionEnum
    {
        IFC2x3,
        IFC4
    }
}
