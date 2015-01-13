using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLData.PropertySets
{
    /// <summary>
    /// The element of IfcPropertyListValue.
    /// </summary>
    public class TypePropertyListValue :BLEntity, IPropertyValueType
    {
        private Value _val;
        /// <summary>
        /// The container element of list value.
        /// </summary>
        public Value ListValue 
        {
            get { return _val; }
            set { var old = _val; Set("ListValue", () => _val = value, () => _val = old); }
        }

        internal override void SetModel(BLModel model)
        {
            _model = model;
            if (ListValue != null) ListValue.SetModel(model);
        }

        public override string Validate()
        {
            var result = "";
            if (ListValue != null) result += ListValue.Validate();
            return result;
        }

        internal override IEnumerable<BLEntity> GetChildren()
        {
            if (ListValue != null) yield return ListValue;
        }
    }
}
