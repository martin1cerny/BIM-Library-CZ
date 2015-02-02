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

        private string _lang = "en-US";
        /// <summary>
        /// Language for localized names taken from name and definition aliases
        /// </summary>
        public string Lang {
            get { return _lang; }
            set { var old = _lang; Set("Lang", () => _lang = value, () => _lang = old); } 
        }


        internal override IEnumerable<BLEntity> GetChildren()
        {
            yield break;
        }
    }
}
