using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BLData.PropertySets
{
    /// <summary>
    /// The defined values which are applicable for the scope as defined by the defining values.
    /// </summary>
    public class Value : BLEntity
    {
        private DataType _dataType;
        /// <summary>
        /// The defining value data type.
        /// </summary>
        public DataType DataType 
        {
            get { return _dataType; }
            set { var old = _dataType; Set("DataType", () => _dataType = value, () => _dataType = old); }
        }

        private UnitType _unit;
        /// <summary>
        /// The defining value unit.
        /// </summary>
        public UnitType UnitType {
            get { return _unit; }
            set { var old = _unit; Set("UnitType", () => _unit = value, () => _unit = old); }
        }

        private BList<string> _values = new BList<string>();
        /// <summary>
        /// The container element of values.
        /// </summary>
        [XmlArrayItem("ValueItem")]
        [XmlArray("Values")]
        public BList<string> Values 
        {
            get { return _values; }
            set { var old = _values; Set("Values", () => _values = value, () => _values = old); }
        }

        internal override void SetModel(BLModel model)
        {
            _model = model;
            if (DataType != null) DataType.SetModel(model);
            if (UnitType != null) UnitType.SetModel(model);
            if (Values != null) Values.SetModel(model);
        }

        public override string Validate()
        {
            var result = "";
            if (DataType != null) result += DataType.Validate();
            if (UnitType != null) result += UnitType.Validate();
            if (Values != null) result += Values.Validate();
            return result;
        }

        internal override IEnumerable<BLEntity> GetChildren()
        {
            if (DataType != null) yield return DataType;
            if (UnitType != null) yield return UnitType ;
        }
    }
}
