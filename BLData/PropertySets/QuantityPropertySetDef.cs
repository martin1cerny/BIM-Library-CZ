using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BLData.PropertySets
{
    [XmlInclude(typeof(PropertySetDef))]
    [XmlInclude(typeof(QtoSetDef))]
    public abstract class QuantityPropertySetDef : BLModelNamedEntity
    {
        private IfcVersion _version;
        /// <summary>
        /// Version information of IFC release and sub schema.
        /// </summary>
        public IfcVersion IfcVersion {
            get { return _version; }
            set { var old = _version; Set("IfcVersion", () => _version = value, () => _version = old); }
        }

        private BList<NameAlias> _definitionAliases = new BList<NameAlias>();

        /// <summary>
        /// This property needs to be overwritten in inhereted classes 
        /// with corret attributes sut up for serialization
        /// </summary>
        [XmlIgnore]
        public BList<NameAlias> DefinitionAliases {
            get { return _definitionAliases; }
            set { var old = _definitionAliases; Set("DefinitionAliases", () => _definitionAliases = value, () => _definitionAliases = old); }
        }

        /// <summary>
        /// Applicable entity types.
        /// </summary>
        [XmlIgnore]
        public IEnumerable<ApplicableClass> ApplicableClasses
        {
            get
            {
                var res = new ApplicableClass();
                if (_model != null) res.SetModel(_model);
                foreach (var ac in applicableClasses)
                {
                    if (String.IsNullOrEmpty(ac))
                        continue;

                    var fields = ac.Trim().Split('/');
                    if (fields.Length == 1)
                    {
                        res.ClassName = fields[0].Trim();
                        yield return res;
                        continue;
                    }

                    //IFC4 variant
                    if (fields[0].Contains("Ifc"))
                    {
                        res.ClassName = fields[0].Trim();
                        res.PredefinedType = fields[1].Trim();
                        yield return res;
                        continue;
                    }
                    //IFC2x3 variant where fields[0] is the name of the schema domain
                    else
                    {
                        res.ClassName = fields[1].Trim();
                        yield return res;
                        continue;
                    }
                }
            }
        }

        public void AddApplicableClass(ApplicableClass cls)
        {
            applicableClasses.Add(cls.ToString());
            OnPropertyChanged("ApplicableClasses");
        }


        private BList<string> _applicableClasses = new BList<string>();

        /// <summary>
        /// The container element of applicable entity types. This is only to be used by
        /// serializer. Use "ApplicableClasses" instead.
        /// </summary>
        [XmlArray("ApplicableClasses")]
        [XmlArrayItem("ClassName")]
        public BList<string> applicableClasses {
            get { return _applicableClasses; }
            set { var old = _applicableClasses; Set("applicableClasses", () => _applicableClasses = value, () => _applicableClasses = old); }
        }

        [XmlIgnore]
        public QuantityPropertyDef this[string name]
        {
            get
            {
                return Definitions.First(d => d.Name == name);
            }
        }

        [XmlIgnore]
        public abstract IEnumerable<QuantityPropertyDef> Definitions { get; }

        internal override void SetModel(BLModel model)
        {
            _model = model;
            _version.SetModel(model);
            _definitionAliases.SetModel(model);
            _applicableClasses.SetModel(model);
        }

        public override string Validate()
        {
            var msg = "";
            if (String.IsNullOrEmpty(Name))
                msg += "Property or quantity set should have a name. \n";
            return msg;
        }
    }
}
