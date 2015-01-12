using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLData.PropertySets
{
    /// <summary>
    /// The element of IfcPropertySingleValue.
    /// </summary>
    public class TypePropertySingleValue : BLEntity, IPropertyValueType
    {
        private DataType _type;
        /// <summary>
        /// The element of property data type.
        /// </summary>
        public DataType DataType {
            get { return _type; }
            set { var old = _type; Set("DataType", () => _type = value, () => _type = old); }
        }

        private UnitType _unit;
        /// <summary>
        /// The element of property data unit.
        /// </summary>
        public UnitType UnitType {
            get { return _unit; }
            set { var old = _unit; Set("UnitType", () => _unit = value, () => _unit = old); }
        }

        internal override void SetModel(BLModel model)
        {
            _model = model;
            if (DataType != null) DataType.SetModel(model);
            if (UnitType != null) UnitType.SetModel(model);
        }

        public override string Validate()
        {
            var result = "";
            if (UnitType != null) result += UnitType.Validate();
            if (DataType != null) result += DataType.Validate();
            return result;
        }

        internal override IEnumerable<BLEntity> GetChildren()
        {
            if (UnitType != null) yield return UnitType;
            if (DataType != null) yield return DataType;
        }
    }
}
