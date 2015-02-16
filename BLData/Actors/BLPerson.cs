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

        private string _titleBefore;

        public string TitleBefore
        {
            get { return _titleBefore; }
            set { var old = _titleBefore; Set(new string[] { "TitleBefore", "FullName" }, () => _titleBefore = value, () => _titleBefore = old); }
        }

        private string _titleAfter;

        public string TitleAfter
        {
            get { return _titleAfter; }
            set { var old = _titleAfter; Set(new string[] { "TitleAfter", "FullName" }, () => _titleAfter = value, () => _titleAfter = old); }
        }

        public override string Name
        {
            get { return _name; }
            set { var old = _name; Set(new string[] { "Name" , "FullName"}, () => _name = value, () => _name = old); }
        }

        private string _surname;
        public string Surname
        {
            get { return _surname; }
            set { var old = _surname; Set(new[] { "Surname" , "FullName"}, () => _surname = value, () => _surname = old); }
        }

        private string _email;
        public string Email
        {
            get { return _email; }
            set { var old = _email; Set("Email", () => _email = value, () => _email = old); }
        }

        private string _companyName;
        public string CompanyName
        {
            get { return _companyName; }
            set { var old = _companyName; Set("CompanyName", () => _companyName = value, () => _companyName = old); }
        }


        [XmlIgnore]
        public string FullName
        {
            get { return String.Format("{0}{1}{2}{3}{4}{5}{6}", 
                TitleBefore,
                !String.IsNullOrEmpty(TitleBefore) && !String.IsNullOrEmpty(Name) ? " " : "", 
                Name, 
                !String.IsNullOrEmpty(Surname) && !String.IsNullOrEmpty(Name) ? " " : "", 
                Surname,
                !String.IsNullOrEmpty(Surname) && !String.IsNullOrEmpty(TitleAfter) ? ", " : "", 
                TitleAfter
                ); }
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
