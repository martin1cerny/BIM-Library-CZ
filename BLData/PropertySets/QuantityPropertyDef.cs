using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BLData.PropertySets
{
    [XmlInclude(typeof(QtoDef))]
    [XmlInclude(typeof(PropertyDef))]
    public class QuantityPropertyDef : BLEntity, INamedEntity
    {
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
