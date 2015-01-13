using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BLData.PropertySets
{
    /// <summary>
    /// The name alias in local language.
    /// </summary>
    public class NameAlias : BLEntity
    {
        private string _lang;
        /// <summary>
        /// The language code based on ISO 639-1 and ISO 3166-1 alpha-2 codes, i.e., "de-DE", "ja-JP", "fr-FR", "no-NO".
        /// </summary>
        [XmlAttribute("lang")]
        public string Lang {
            get { return _lang; }
            set { var old = _lang; Set("Lang", () => _lang = value, () => _lang = old); }
        }

        private string _value;

        [XmlText]
        public string Value {
            get { return _value; }
            set { var old = _value; Set("Value", () => _value = value, () => _value = old); }
        }

        internal override void SetModel(BLModel model)
        {
            _model = model;
        }

        public override string Validate()
        {
            var msg = "";
            if (String.IsNullOrEmpty(_lang))
                msg += "Language code should be defined for named alias \n";
            return msg;
        }

        internal override IEnumerable<BLEntity> GetChildren()
        {
            yield break;
        }
    }
}
