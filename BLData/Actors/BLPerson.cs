using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BLData.Actors
{
    public class BLPerson : BLModelNamedEntity
    {
        private string _surname;
        public string Surname
        {
            get { return _surname; }
            set { var old = _surname; Set("Surname", () => _surname = value, () => _surname = old); }
        }

        private string _email;
        public string Email
        {
            get { return _email; }
            set { var old = _email; Set("Email", () => _email = value, () => _email = old); }
        }

        [XmlIgnore]
        public string FullName
        {
            get { return String.Format("{0}{1}{2}", Name, !String.IsNullOrEmpty(Surname) && !String.IsNullOrEmpty(Name) ? " " : "", Surname); }
        }

        internal override void SetModel(BLModel model)
        {
            _model = model;
        }

        public override string Validate()
        {
            var result = "";

            if (String.IsNullOrEmpty(Name))
                result += Id + ": Name of the person should be defined. \n";

            return result;
        }

        internal override IEnumerable<BLEntity> GetChildren()
        {
            yield break;
        }
    }
}
