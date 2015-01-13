using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BLData.PropertySets
{
    [XmlRoot("QtoSetDef")]
    public class QtoSetDef : QuantityPropertySetDef
    {
        internal override void SetModel(BLModel model)
        {
            if (QuantityDefinitions != null) QuantityDefinitions.SetModel(model);
            base.SetModel(model);
        }

        public override string Validate()
        {
            var result = base.Validate();
            if (QuantityDefinitions != null) result += QuantityDefinitions.Validate();
            return result;
        }

        private string _methodOfMeasurement;
        public string MethodOfMeasurement {
            get { return _methodOfMeasurement; }
            set { var old = _methodOfMeasurement; Set("MethodOfMeasurement", () => _methodOfMeasurement = value, () => _methodOfMeasurement = old); }
        }

        private string _applicableTypeValue;
        public string ApplicableTypeValue {
            get { return _applicableTypeValue; }
            set { var old = _applicableTypeValue; Set("ApplicableTypeValue", () => _applicableTypeValue = value, () => _applicableTypeValue = old); }
        }

        private BList<QtoDef> _quantityDefinitions = new BList<QtoDef>();
        [XmlArray("QtoDefs")]
        [XmlArrayItem("QtoDef")]
        public BList<QtoDef> QuantityDefinitions {
            get { return _quantityDefinitions; }
            set { var old = _quantityDefinitions; Set("QuantityDefinitions", () => _quantityDefinitions = value, () => _quantityDefinitions = old);}
        }

        [XmlArrayItem("QtoDefinitionAlias")]
        [XmlArray("QtoDefinitionAliases")]
        public BList<NameAlias> _QtoDefinitionAliases { get { return DefinitionAliases; } set { DefinitionAliases = value; } }

        [XmlIgnore]
        public override IEnumerable<QuantityPropertyDef> Definitions
        {
            get
            {
                if (QuantityDefinitions != null)
                    foreach (var item in QuantityDefinitions)
                    {
                        yield return item;
                    }
            }
        }

        internal override IEnumerable<BLEntity> GetChildren()
        {
            if (_quantityDefinitions != null && _quantityDefinitions.Any())
                foreach (var item in _quantityDefinitions)
                    yield return item;
        }
    }
}
