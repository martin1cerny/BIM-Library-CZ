using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BLData
{
    public abstract class BLModelNamedEntity : BLModelEntity, INamedEntity
    {
        protected string _name;

        public virtual string Name
        {
            get { return _name; }
            set { var old = _name; Set("Name", ()=> _name = value, () => _name = old); }
        }

        private string _definition;

        public string Definition
        {
            get { return _definition; }
            set { var old = _definition; Set("Definition", () => _definition = value, () => _definition = old); }
        }

        private BList<NameAlias> _nameAliases = new BList<NameAlias>();
        public virtual BList<NameAlias> NameAliases
        {
            get { return _nameAliases; }
            set { var old = _nameAliases; Set("NameAliases", () => _nameAliases = value, () => _nameAliases = old); }
        }

        private BList<NameAlias> _definitionAliases = new BList<NameAlias>();
        [XmlArrayItem("DefinitionAlias")]
        public virtual BList<NameAlias> DefinitionAliases
        {
            get { return _definitionAliases; }
            set { var old = _definitionAliases; Set("DefinitionAliases", () => _definitionAliases = value, () => _definitionAliases = old); }
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
                    na = NameAliases.FirstOrDefault(a => a.Lang == "en-US");
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
                    na = DefinitionAliases.FirstOrDefault(a => a.Lang == "en-US");
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
