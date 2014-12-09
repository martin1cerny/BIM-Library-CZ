using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BLData.PropertySets
{
    /// <summary>
    /// The element of property type.
    /// </summary>
    public class PropertyType : BLEntity
    {
        private object __value;

        [XmlElement("TypePropertySingleValue", typeof(TypePropertySingleValue))]
        [XmlElement("TypePropertyEnumeratedValue", typeof(TypePropertyEnumeratedValue))]
        [XmlElement("TypePropertyBoundedValue", typeof(TypePropertyBoundedValue))]
        [XmlElement("TypePropertyTableValue", typeof(TypePropertyTableValue))]
        [XmlElement("TypePropertyReferenceValue", typeof(TypePropertyReferenceValue))]
        [XmlElement("TypePropertyListValue", typeof(TypePropertyListValue))]
        [XmlElement("TypeComplexProperty", typeof(TypeComplexProperty))]
        public object _value {
            get { return __value; }
            //this should only be used by deserializer
            set { if (_model == null) __value = value; else throw new InvalidOperationException(); }
        }

        [XmlIgnore]
        public IPropertyValueType PropertyValueType { 
            get { return _value as IPropertyValueType; }
            set { var old = __value; Set("PropertyValueType", () => __value = value, () => __value = old); }
        }

        internal override void SetModel(BLModel model)
        {
            _model = model;
            if (PropertyValueType != null)
                (PropertyValueType as BLEntity).SetModel(model);
        }

        public override string Validate()
        {
            return "";
        }
    }
}
