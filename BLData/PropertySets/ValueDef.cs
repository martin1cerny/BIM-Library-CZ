using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLData.PropertySets
{
    public class ValueDef : BLEntity
    {
        private string _min;
        public string MinValue {
            get { return _min; }
            set { var old = _min; Set("MinValue", () => _min = value, () => _min = old); }
        }

        private string _max;
        public string MaxValue {
            get { return _max; }
            set { var old = _max; Set("MaxValue", () => _max = value, () => _max = old); }
        }

        private string _def;
        public string DefaultValue {
            get { return _def; }
            set { var old = _def; Set("DefaultValue", () => _def = value, () => _def = old); }
        }

        internal override void SetModel(BLModel model)
        {
            _model = model;
        }

        public override string Validate()
        {
            return "";
        }

        internal override IEnumerable<BLEntity> GetChildren()
        {
            yield break;
        }
    }
}
