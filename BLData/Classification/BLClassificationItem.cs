using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BLData.Classification
{
    public class BLClassificationItem : BLModelNamedEntity
    {
        private string _code;

        public string Code
        {
            get { return _code; }
            set { Set("_code", _code, value); }
        }

        private string _selector;

        public string Selector
        {
            get { return _selector; }
            set { Set("_selector", _selector, value); }
        }

        private Guid? _parentID;

        public Guid? ParentID
        {
            get { return _parentID; }
            set 
            { 
                Set("_parentID", _parentID, value); 
                OnPropertyChanged("Parent"); 
                if (value != null && Parent != null)
                    Parent.OnPropertyChanged("Children"); 
            }
        }

        [XmlIgnore]
        public BLClassificationItem Parent
        {
            get
            {
                return _parentID != null && _model != null ? _model.Get<BLClassificationItem>(_parentID ?? Guid.Empty) : null;
            }
        }

        [XmlIgnore]
        IEnumerable<BLClassificationItem> Children
        {
            get
            {
                return _model.Get<BLClassificationItem>(i => i.ParentID == this.Id);
            }
        }


        internal override void SetModel(BLModel model)
        {
            _model = model;
        }

        public override string Validate()
        {
            return "";
        }
    }
}
