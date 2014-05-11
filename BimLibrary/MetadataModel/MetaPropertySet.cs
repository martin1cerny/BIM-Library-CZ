using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BimLibrary.MetadataModel
{
    public class MetaPropertySet
    {
        public string Name { get; set; }
        private List<MetaProperty> _properties = new List<MetaProperty>();
        public List<MetaProperty> Properties
        {
            get { return _properties = new List<MetaProperty>(); }
            set { _properties = value; }
        }
        
    }
}
