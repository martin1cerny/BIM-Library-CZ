using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLData.PropertySets
{
    public class ApplicableClass : BLEntity
    {
        private string _name;
        public string ClassName 
        {
            get { return _name; }
            set { var old = _name; Set("ClassName", () => _name = value, () => _name = old); } 
        }

        private string _type;
        public string PredefinedType 
        {
            get { return _type; }
            set { var old = _type; Set("PredefinedType", () => _type = value, () => _type = old); }
        }

        public override string ToString()
        {
            if (String.IsNullOrEmpty(PredefinedType))
                return ClassName;
            else
                return String.Format("{0}/{1}", ClassName, PredefinedType);
        }

        internal override void SetModel(BLModel model)
        {
            _model = model;
        }


        public override string Validate()
        {
            var msg = "";
            if (String.IsNullOrEmpty(_name))
                msg += String.Format("Applicable class doesn't have a name defined. \n");
            return msg;
        }

        internal override IEnumerable<BLEntity> GetChildren()
        {
            yield break;
        }
    }
}
