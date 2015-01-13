using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BLData.PropertySets
{
    /// <summary>
    /// The container element of bound value.
    /// </summary>
    public class ValueRangeDef : BLEntity
    {
        private BoundValue _lower;
        /// <summary>
        /// The lower bound value.
        /// </summary>
        [XmlElement("LowerBoundValue")]
        public BoundValue LowerBoundValue {
            get { return _lower; }
            set { var old = _lower; Set("LowerBoundValue", () => _lower = value, () => _lower = old); }
        }

        private BoundValue _upper;
        /// <summary>
        /// The upper bound value.
        /// </summary>
        [XmlElement("UpperBoundValue")]
        public BoundValue UpperBoundValue {
            get { return _upper; }
            set { var old = _upper; Set("UpperBoundValue", () => _upper = value, () => _upper = old); }
        }

        internal override void SetModel(BLModel model)
        {
            _model = model;
            if (LowerBoundValue != null) LowerBoundValue.SetModel(model);
            if (UpperBoundValue != null) UpperBoundValue.SetModel(model);
        }

        public override string Validate()
        {
            var result =  (LowerBoundValue != null && UpperBoundValue != null) ? "" : "Upper and lower bounds should be specified. \n";
            result += LowerBoundValue != null ? LowerBoundValue.Validate() : "";
            result += UpperBoundValue != null ? UpperBoundValue.Validate() : "";
            return result;
        }

        internal override IEnumerable<BLEntity> GetChildren()
        {
            if (LowerBoundValue != null) yield return LowerBoundValue;
            if (UpperBoundValue != null) yield return UpperBoundValue ;
        }
    }

    public class BoundValue : BLEntity
    {
        private string _value;
        [XmlAttribute]
        public string Value {
            get { return _value; }
            set { var old = _value; Set("Value", () => _value = value, () => _value = old); }
        }

        internal override void SetModel(BLModel model)
        {
            _model = model;
        }

        public override string Validate()
        {
            return String.IsNullOrEmpty(_value) ? "Bound value should be specified. \n" : "";
        }

        internal override IEnumerable<BLEntity> GetChildren()
        {
            yield break;
        }
    }
}
