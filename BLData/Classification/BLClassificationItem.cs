using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using BLData.PropertySets;

namespace BLData.Classification
{
    public class BLClassificationItem : BLModelNamedEntity
    {
        private string _code;

        public string Code
        {
            get { return _code; }
            set { var old = _code; Set("Code", () => _code = value, () => _code = old); }
        }

        private string _selector;

        public string Selector
        {
            get { return _selector; }
            set { var old = _selector; Set("Selector", () => _selector= value, () => _selector = old); }
        }

        private Guid? _parentID;

        public Guid? ParentID
        {
            get { return _parentID; }
            set 
            {
                var old = _parentID;
                Set("ParentID", () => _parentID = value, () => _parentID = old); 
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

        private BList<Guid> _definitionSetIds = new BList<Guid>();

        public BList<Guid> DefinitionSetIds
        {
            get { return _definitionSetIds; }
            set
            {
                var old = _definitionSetIds; Set(new[] { "DefinitionSetIds", "DefinitionSets" }, () => _definitionSetIds = value, () => _definitionSetIds = old);
                if (_definitionSetIds != null)
                {
                    _definitionSetIds.CollectionChanged += (s, a) => OnPropertyChanged("DefinitionSets");
                }
            }

        }

        [XmlIgnore]
        public IEnumerable<QuantityPropertySetDef> DefinitionSets
        {
            get
            {
                if (_definitionSetIds != null)
                    foreach (var id in _definitionSetIds)
                    {
                        yield return _model.Get<QuantityPropertySetDef>(id);
                    }
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

        internal override IEnumerable<BLEntity> GetChildren()
        {
            yield break;
        }
    }
}
