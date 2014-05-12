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
        private int _ifcEntityLabel;
        public int IfcEntityLabel
        {
            get { return _ifcEntityLabel; }
            set
            {
                _ifcEntityLabel = value;
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
