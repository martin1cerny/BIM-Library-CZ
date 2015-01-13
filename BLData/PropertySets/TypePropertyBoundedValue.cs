using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLData.PropertySets
{
    /// <summary>
    /// The element of IfcPropertyBoundedValue.
    /// </summary>
    public class TypePropertyBoundedValue : BLEntity, IPropertyValueType
    {
        private ValueRangeDef _valRangeDef;
        /// <summary>
        /// The container element of bound value.
        /// </summary>
        public ValueRangeDef ValueRangeDef {
            get { return _valRangeDef; }
            set { var old = _valRangeDef; Set("ValueRangeDef", () => _valRangeDef = value, () => _valRangeDef = old); }
        }

        private DataType _dataType;
        /// <summary>
        /// The property data type.
        /// </summary>
        public DataType DataType {
            get { return _dataType; }
            set { var old = _dataType; Set("DataType", () => _dataType = value, () => _dataType = old); }
        }

        private UnitType _unitType;
        /// <summary>
        /// The property data unit.
        /// </summary>
        public UnitType UnitType {
            get { return _unitType; }
            set { var old = _unitType; Set("UnitType", () => _unitType = value, () => _unitType = old); }
        }

        internal override void SetModel(BLModel model)
        {
            _model = model;
            if (ValueRangeDef != null) ValueRangeDef.SetModel(model);
            if (DataType != null) DataType.SetModel(model);
            if (UnitType != null) UnitType.SetModel(model);
        }

        public override string Validate()
        {
            var result = "";
            result += DataType != null ? DataType.Validate() : "";
            result += UnitType != null ? UnitType.Validate() : "";
            result += ValueRangeDef != null ? ValueRangeDef.Validate() : "";

            return result;
        }

        internal override IEnumerable<BLEntity> GetChildren()
        {
            if (ValueRangeDef != null) yield return ValueRangeDef;
            if (DataType != null) yield return DataType;
            if (UnitType != null) yield return  UnitType;
        }
    }
}
