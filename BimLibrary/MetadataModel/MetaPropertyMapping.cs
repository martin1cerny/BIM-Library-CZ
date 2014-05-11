using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.MaterialResource;

namespace BimLibrary.MetadataModel
{
    public class MetaPropertyMapping
    {
        private Type _type;
        public Type RelatingType
        {
            get { return _type; }
            set
            {
                if (typeof(IfcProduct).IsAssignableFrom(value) || typeof(IfcMaterial).IsAssignableFrom(value))
                    _type = value;
                else
                    throw new Exception("Only IfcProducts and IfcMaterials are supported.");
            }
        }

        private List<MetaPropertySet> _pSets = new List<MetaPropertySet>();
        public List<MetaPropertySet> PropertySets
        {
            get { return _pSets = new List<MetaPropertySet>(); }
            set { _pSets = value; }
        }
        
    }
}
