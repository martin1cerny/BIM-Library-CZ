using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BLData.Classification
{
    public class BLClassification : BLModelNamedEntity
    {
        private BList<Guid> _rootItemIDs = new BList<Guid>();
        [XmlArray("RootItems")]
        public BList<Guid> RootItemIDs
        {
            get { return _rootItemIDs; }
            set {
                var old = _rootItemIDs;
                Set("RootItemIDs", () => _rootItemIDs = value,() => _rootItemIDs = old); 
                OnPropertyChanged("RootItems");
                if (value != null)
                    value.CollectionChanged += (s, a) => {
                        OnPropertyChanged("RootItems");
                    };
            }
        }

        [XmlIgnore]
        public IEnumerable<BLClassificationItem> RootItems
        {
            get
            {
                if (_rootItemIDs != null)
                foreach (var id in _rootItemIDs)
                    yield return _model.Get<BLClassificationItem>(id);
            }
        }

        internal override void SetModel(BLModel model)
        {
            _model = model;
        }

        public override string Validate()
        {
            var msg = "";
            if (String.IsNullOrEmpty(Name))
                msg += String.Format("Name not defined for classification '{0}' \n", Id);
            return msg;
        }
    }
}
