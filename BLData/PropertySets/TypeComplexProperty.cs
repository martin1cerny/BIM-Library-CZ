using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BLData.PropertySets
{
    /// <summary>
    /// The element of IfcComplexProperty.
    /// </summary>
    public class TypeComplexProperty : BLEntity, IPropertyValueType
    {
        private string _name;
        /// <summary>
        /// The name of complex property, i.e., "CP_*".
        /// </summary>
        [XmlAttribute("name")]
        public string Name {
            get { return _name; }
            set { var old = _name; Set("Name", () => _name = value, () => _name = old); }
        }

        private BList<PropertyDef> _properties = new BList<PropertyDef>();
        [XmlElement("PropertyDef")]
        public BList<PropertyDef> Properties
        {
            get { return _properties; }
            set { var old = _properties; Set("Properties", () => _properties = value, () => _properties = old); }
        }

        internal override void SetModel(BLModel model)
        {
            _model = model;
            if (Properties != null) Properties.SetModel(model);
        }

        public override string Validate()
        {
            var result = "";
            if (Properties != null) result += Properties.Validate();
            return result;
        }

        internal override IEnumerable<BLEntity> GetChildren()
        {
            if (Properties != null) foreach (var item in Properties) yield return item;
        }
    }
}
