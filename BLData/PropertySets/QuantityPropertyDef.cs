using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace BLData.PropertySets
{
    [XmlInclude(typeof(QtoDef))]
    [XmlInclude(typeof(PropertyDef))]
    public class QuantityPropertyDef : BLEntity, INamedEntity
    {
        [XmlIgnore]
        public string PropertyTypeString
        {
            get
            {
                var qto = this as QtoDef;
                if (qto != null)
                    return "Quantity";

                var prop = this as PropertyDef;
                if (prop == null)
                    return "Unknown";

                var propType = prop.PropertyType.PropertyValueType;
                var single = propType as TypePropertySingleValue;
                if (single != null)
                    return "Single Value";

                var enumerated = propType as TypePropertyEnumeratedValue;
                if (enumerated != null)
                    return "Enumeration";

                var bounded = propType as TypePropertyBoundedValue;
                if (bounded != null)
                    return "Bounded Value";

                var table = propType as TypePropertyTableValue;
                if (table != null)
                    return "Table";

                var reference = propType as TypePropertyReferenceValue;
                if (reference != null)
                    return "Reference";

                var list = propType as TypePropertyListValue;
                if (list != null)
                    return "List of values";

                var complex = propType as TypeComplexProperty;
                if (complex != null)
                    return "Complex";

                return "Unknown";
            }
        }

        [XmlIgnore]
        public string ValueTypeString
        {
            get
            {
                var qto = this as QtoDef;
                if (qto != null)
                {
                    var valType = qto.QuantityType.ToString().Substring(2).ToLower(); //strip Q_ at the beginning
                    return valType[0].ToString().ToUpper() + valType.Substring(1);
                }

                var prop = this as PropertyDef;
                if (prop == null)
                    return "Unknown";

                var propType = prop.PropertyType.PropertyValueType;
                var single = propType as TypePropertySingleValue;
                if (single != null)
                {
                    var valType = single?.DataType?.Type?.ToString();
                    return MakeHumanText(valType);
                }

                var enumerated = propType as TypePropertyEnumeratedValue;
                if (enumerated != null)
                {
                    if (enumerated?.EnumList?.Items == null)
                        return "Unknown";
                    return string.Join(", ", enumerated.EnumList.Items);
                }

                var bounded = propType as TypePropertyBoundedValue;
                if (bounded != null)
                {
                    var valType = bounded?.DataType?.Type?.ToString();
                    return MakeHumanText(valType);
                }

                var table = propType as TypePropertyTableValue;
                if (table != null)
                {
                    var defining = table?.DefiningValue?.DataType.Type?.ToString();
                    defining =  MakeHumanText(defining);

                    var defined = table?.DefinedValue?.DataType.Type?.ToString();
                    defined = MakeHumanText(defined);

                    return $"Defining: {defining}, Defined: {defined}";
                }

                var reference = propType as TypePropertyReferenceValue;
                if (reference != null)
                {
                    var valType = reference?.ReferenceType.Value.ToString();
                    return MakeHumanText(valType);
                }

                var list = propType as TypePropertyListValue;
                if (list != null)
                {
                    var valType = list?.ListValue?.DataType?.Type.ToString();
                    return MakeHumanText(valType);
                }

                var complex = propType as TypeComplexProperty;
                if (complex != null)
                    return "Complex";

                return "Unknown";
            }
        }

        private string MakeHumanText(string ifcType)
        {
            if (ifcType == null)
                return "Unknown";

            //strip Ifc at the beginning
            if (ifcType.StartsWith("Ifc"))
                ifcType = ifcType.Substring(3);

            //insert spaces between upper and lower case letters
            ifcType = System.Text.RegularExpressions.Regex.Replace(ifcType, "((?<=[a-z])[A-Z]|[A-Z](?=[a-z]))", " $1").Trim();
            return ifcType;
        }

        private string _name;
        /// <summary>
        /// The name of property.
        /// </summary>
        public string Name {
            get { return _name; }
            set { var old = _name; Set("Name", () => _name =value, () => _name = old); }
        }

        private string _definition;

        /// <summary>
        /// The definition of property.
        /// </summary>
        public string Definition {
            get { return _definition; }
            set { var old = _definition; Set("Definition", () => _definition = value, () => _definition = old); }
        }

        private BList<NameAlias> _nameAliases = new BList<NameAlias>();
        /// <summary>
        /// Name aliases in different languages.
        /// </summary>
        [XmlArrayItem("NameAlias")]
        [XmlArray("NameAliases")]
        public BList<NameAlias> NameAliases {
            get { return _nameAliases; }
            set { var old = _nameAliases; Set("NameAliases", () => _nameAliases = value, () => _nameAliases = old); }
        }

        private BList<NameAlias> _definitionAliases = new BList<NameAlias>();

        /// <summary>
        /// Definition aliases in different languages.
        /// </summary>
        [XmlArrayItem("DefinitionAlias")]
        [XmlArray("DefinitionAliases")]
        public BList<NameAlias> DefinitionAliases {
            get { return _definitionAliases; }
            set { var old = _definitionAliases; Set("DefinitionAliases", () => _definitionAliases = value, () => _definitionAliases = old); }
        }

        internal override void SetModel(BLModel model)
        {
            _model = model;
            if (_definitionAliases != null) _definitionAliases.SetModel(model);
            if (_nameAliases != null) _nameAliases.SetModel(model);
        }

        public override string Validate()
        {
            var msg = "";
            if (_definitionAliases != null) msg += _definitionAliases.Validate();
            if (_nameAliases != null) msg += _nameAliases.Validate();

            if (String.IsNullOrEmpty(_name))
                msg += "Name of the property or quantity should be defined. \n";
            return msg;
        }

        internal override IEnumerable<BLEntity> GetChildren()
        {
            if (_definitionAliases != null) foreach (var item in _definitionAliases) yield return item;
            if (_nameAliases != null) foreach (var item in _nameAliases) yield return item;
        }


        [XmlIgnore]
        public string LocalizedName
        {
            get
            {
                var lang = _model.Information.Lang ?? "en-US";
                if (NameAliases != null)
                {
                    var na = NameAliases.FirstOrDefault(a => a.Lang == lang);
                    if (na != null)
                        return na.Value;
                }
                return Name;
            }
            set
            {
                var lang = _model.Information.Lang ?? "en-US";
                if (NameAliases != null)
                {
                    var na = NameAliases.FirstOrDefault(a => a.Lang == lang);
                    if (na != null)
                    {
                        na.Value = value;
                        return;
                    }
                }
                else
                    NameAliases = new BList<NameAlias>(_model);
                var alias = _model.New<NameAlias>(a => { a.Value = value; a.Lang = lang; });
                NameAliases.Add(alias);
            }
        }

        [XmlIgnore]
        public string LocalizedDefinition
        {
            get
            {
                var lang = _model.Information.Lang ?? "en-US";
                if (DefinitionAliases != null)
                {
                    var na = DefinitionAliases.FirstOrDefault(a => a.Lang == lang);
                    if (na != null)
                        return na.Value;
                }
                return Definition;
            }
            set
            {
                var lang = _model.Information.Lang ?? "en-US";
                if (DefinitionAliases != null)
                {
                    var na = DefinitionAliases.FirstOrDefault(a => a.Lang == lang);
                    if (na != null)
                    {
                        na.Value = value;
                        return;
                    }
                }
                else
                    DefinitionAliases = new BList<NameAlias>(_model);
                var alias = _model.New<NameAlias>(a => { a.Value = value; a.Lang = lang; });
                DefinitionAliases.Add(alias);
            }
        }
    }
}
