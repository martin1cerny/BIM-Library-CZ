using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BLData.PropertySets
{
    /// <summary>
    /// The element of enumeration constaint definition.
    /// </summary>
    public class ConstantDef : BLEntity
    {
        private string _name;

        /// <summary>
        /// The name of the enumeration constant.
        /// </summary>
        public string Name 
        {
            get { return _name; }
            set { var old = _name; Set("Name", () => _name = value, () => _name = old); }
        }

        private string _definition;

        /// <summary>
        /// The definition of the enumeration constant.
        /// </summary>
        public string Definition 
        {
            get { return _definition; }
            set { var old = _definition; Set("Definition", () => _definition = value, () => _definition = old); }
        }


        private BList<NameAlias> _nameAliases = new BList<NameAlias>();
        /// <summary>
        /// The container element of name alias.
        /// </summary>
        [XmlArray("NameAliases")]
        [XmlArrayItem("NameAlias")]
        public BList<NameAlias> NameAliases 
        {
            get { return _nameAliases; }
            set { var old = _nameAliases; Set("NameAliases", () => _nameAliases = value, () => _nameAliases = old); }
        }

        private BList<NameAlias> _definitionAliases;

        [XmlArray("DefinitionAliases")]
        [XmlArrayItem("DefinitionAlias")]
        public BList<NameAlias> DefinitionAliases 
        {
            get { return _definitionAliases; }
            set { var old = _definitionAliases; Set("DefinitionAliases", () => _definitionAliases = value, () => _definitionAliases = old); }
        
        }

        internal override void SetModel(BLModel model)
        {
            _model = model;
            if (_nameAliases != null) _nameAliases.SetModel(model);
            if (_definitionAliases != null) _definitionAliases.SetModel(model);
        }

        public override string Validate()
        {
            var result = "";
            if (_definitionAliases != null) result += _definitionAliases.Validate();
            if (_nameAliases != null) result += _nameAliases.Validate();
            return result;
        }

        internal override IEnumerable<BLEntity> GetChildren()
        {
            if (_nameAliases != null) foreach (var item in _nameAliases) yield return item;
            if (_definitionAliases != null) foreach (var item in _definitionAliases) yield return item;
        }
    }
}
