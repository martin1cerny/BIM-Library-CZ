using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLData.PropertySets
{
    /// <summary>
    /// The element of IfcPropertyReferenceValue.
    /// </summary>
    public class TypePropertyReferenceValue : ReferenceSelect, IPropertyValueType
    {
        private DataType _type;
        public DataType DataType {
            get { return _type; }
            set { var old = _type; Set("DataType", () => _type = value, () => _type = old); }
        }

        internal override void SetModel(BLModel model)
        {
            base.SetModel(model);
            if (DataType != null) DataType.SetModel(model);
        }

        public override string Validate()
        {
            var result = "";
            result += base.Validate();
            if (DataType != null) result += DataType.Validate();
            return result;
        }
    }
}
