using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BLData.PropertySets
{
    /// <summary>
    /// The top node element of Property Set Definition (PSD).
    /// </summary>
    [XmlRoot("PropertySetDef")]
    public class PropertySetDef: QuantityPropertySetDef
    {
        internal override void SetModel(BLModel model)
        {
            if (_propertyDefinitions != null) _propertyDefinitions.SetModel(model);
            base.SetModel(model);
        }

        public override string Validate()
        {
            var result = "";
            result += base.Validate();
            if (_propertyDefinitions != null) result += _propertyDefinitions.Validate();
            return result;
        }

        private string _applicability;
        /// <summary>
        /// The description of applicability and usecases, i.e., "IfcDoor entity", "Common Pset of Ifc...".
        /// </summary>
        public string Applicability {
            get { return _applicability; }
            set { var old = _applicability; Set("Applicability", () => _applicability = value, () => _applicability = old); }
        }


        private string _applicableTypeValue;
        /// <summary>
        /// The format of applicable type value is ENTITY_TYPE/PREDEFINED_TYPE. Multiple value is accepted, and format is: "TYPE_1 | TYPE_2 ...".
        /// </summary>
        public string ApplicableTypeValue {
            get { return _applicableTypeValue; }
            set { var old = _applicableTypeValue; Set("ApplicableTypeValue", () => _applicableTypeValue = value, () => _applicableTypeValue = old); }
        }

        private BList<PropertyDef> _propertyDefinitions = new BList<PropertyDef>();
        /// <summary>
        /// The container element of property definition.
        /// </summary>
        [XmlArray("PropertyDefs")]
        [XmlArrayItem("PropertyDef")]
        public BList<PropertyDef> PropertyDefinitions {
            get { return _propertyDefinitions; }
            set { 
                var old = _propertyDefinitions;
                Set(new[] { "PropertyDefinitions", "Definitions", "AllPropertyDefinitions" }, () => _propertyDefinitions = value, () => _propertyDefinitions = old);
            }
        }

        /// <summary>
        /// The container element of property set definition alias.
        /// </summary>
        [XmlArrayItem("PsetDefinitionAlias")]
        [XmlArray("PsetDefinitionAliases")]
        public BList<NameAlias> _PsetDefinitionAliases { 
            get { return DefinitionAliases; } 
            set { DefinitionAliases = value; OnPropertyChanged("_PsetDefinitionAliases"); } 
        }


        private string _ifdGuid;
        /// <summary>
        /// The Globally Unique Identifier (GIUD) for the property set definition. The ID is referencing the IFD GUID.
        /// </summary>
        [XmlAttribute("ifdguid")]
        public string IfdGuid {
            get { return _ifdGuid; }
            set { var old = _ifdGuid; Set("IfdGuid", () => _ifdGuid = value, () => _ifdGuid = old); }
        }

        private templatetype _templateType;
        /// <summary>
        /// The property set template type.
        /// </summary>
        [XmlAttribute("templatetype")]
        public templatetype TemplateType {
            get { return _templateType; }
            set { var old = _templateType; Set("TemplateType", () => _templateType = value, () => _templateType = old); }
        }


        /// <summary>
        /// This includes nested property definitions of complex types
        /// </summary>
        [XmlIgnore]
        public IEnumerable<PropertyDef> AllPropertyDefinitions
        {
            get
            {
                foreach (var property in PropertyDefinitions)
                {
                    yield return property;

                    var complex = property.PropertyType.PropertyValueType as TypeComplexProperty;
                    if (complex != null)
                    {
                        foreach (var p in complex.Properties)
                            yield return p;
                    }
                }
            }
        }

        [XmlIgnore]
        public override IEnumerable<QuantityPropertyDef> Definitions
        {
            get
            {
                if (PropertyDefinitions != null)
                    foreach (var item in PropertyDefinitions)
                    {
                        yield return item;
                    }
            }
        }

        internal override IEnumerable<BLEntity> GetChildren()
        {
            if (_propertyDefinitions != null) foreach (var item in _propertyDefinitions) yield return item;
            foreach (var item in base.GetChildren()) yield return item;
        }
    }

    public enum templatetype
    {
        NOTDEFINED,
        PSET_TYPEDRIVENONLY,
        PSET_TYPEDRIVENOVERRIDE,
        PSET_OCCURRENCEDRIVEN,
        PSET_PERFORMANCEDRIVEN,
        QTO_TYPEDRIVENONLY,
        QTO_TYPEDRIVENOVERRIDE,
        QTO_OCCURRENCEDRIVEN,
    }
}
