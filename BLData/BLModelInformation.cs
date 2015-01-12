using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLData
{
    public class BLModelInformation : BLModelNamedEntity
    {

        internal override void SetModel(BLModel model)
        {
            _model = model;
        }

        public override string Validate()
        {
            var msg = "";
            if (_model.Get<BLModelInformation>().Count() > 1)
                msg += String.Format("There should only be one model information object defined. \n");
            return msg;
        }



        internal override IEnumerable<BLEntity> GetChildren()
        {
            yield break;
        }
    }
}
