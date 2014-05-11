using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.XbimExtensions.SelectTypes;

namespace BimLibrary.MetadataModel
{
    public class MetaProperty
    {
        public string Name { get; set; }

        private Type _type;
        public Type Type { get { return _type; } set {
            if (typeof(IfcValue).IsAssignableFrom(value))
                _type = value;
            else
                throw new Exception("Only IfcValue subtypes are supported.");
        } }
        public string DefaultValue { get; set; }
    }
}
