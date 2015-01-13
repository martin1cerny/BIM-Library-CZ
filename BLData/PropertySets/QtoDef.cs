using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BLData.PropertySets
{
    public class QtoDef : QuantityPropertyDef
    {
        private QtoTypeEnum _quantityType;

        [XmlElement("QtoType")]
        public QtoTypeEnum QuantityType {
            get { return _quantityType; }
            set { var old = _quantityType; Set("QuantityType", () => _quantityType = value, () => _quantityType = old); }
        }
    }

    public enum QtoTypeEnum
    {
        Q_LENGTH,
        Q_AREA,
        Q_VOLUME,
        Q_COUNT,
        Q_TIME,
        Q_WEIGHT
    }
}
