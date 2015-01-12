using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLData.PropertySets
{
    /// <summary>
    /// The element of IfcPropertyTableValue.
    /// </summary>
    public class TypePropertyTableValue : BLEntity, IPropertyValueType
    {
        private string _exp;
        /// <summary>
        /// The information about the expression for the derivation of defined values from the defining values.
        /// </summary>
        public string Expression {
            get { return _exp; }
            set { var old = _exp; Set("Expression", () => _exp = value, () => _exp = old); }
        }

        private Value _defining;
        /// <summary>
        /// The list of defining values, which determine the defined values. This list shall have unique values only.
        /// </summary>
        public Value DefiningValue {
            get { return _defining; }
            set { var old = _defining; Set("DefiningValue", () => _defining = value, () => _defining = old); }
        }

        private Value _defined;
        /// <summary>
        /// The defined values which are applicable for the scope as defined by the defining values.
        /// </summary>
        public Value DefinedValue {
            get { return _defined; }
            set { var old = _defined; Set("DefinedValue", () => _defined = value, () => _defined = old); }
        }

        internal override void SetModel(BLModel model)
        {
            _model = model;
            if (DefinedValue != null) DefinedValue.SetModel(model);
            if (DefiningValue != null) DefiningValue.SetModel(model);
        }

        public override string Validate()
        {
            var result = "";
            if (DefinedValue != null) result += DefinedValue.Validate();
            if (DefiningValue != null) result += DefiningValue.Validate();
            return result;
        }

        internal override IEnumerable<BLEntity> GetChildren()
        {
            if (DefinedValue != null) 
                yield return DefinedValue;
            if (DefiningValue != null)
                yield return DefiningValue;
        }
    }
}
