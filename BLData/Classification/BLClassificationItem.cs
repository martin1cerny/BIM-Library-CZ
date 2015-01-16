﻿using System;
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
        public IEnumerable<BLClassificationItem> Children
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

        private BList<NameAlias> _nameAliases = new BList<NameAlias>();
        public BList<NameAlias> NameAliases 
        {
            get { return _nameAliases; }
            set { var old = _nameAliases; Set("NameAliases", () => _nameAliases = value, () => _nameAliases = old); }
        }

        private BList<NameAlias> _definitionAliases = new BList<NameAlias>();
        [XmlArrayItem("DefinitionAlias")]
        public BList<NameAlias> DefinitionAliases
        {
            get { return _definitionAliases; }
            set { var old = _definitionAliases; Set("DefinitionAliases", () => _definitionAliases = value, () => _definitionAliases = old); }
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

        [XmlIgnore]
        public IEnumerable<QuantityPropertySetDef> DefinitionSetsUp
        {
            get
            {
                if (_definitionSetIds != null)
                    foreach (var id in _definitionSetIds)
                    {
                        yield return _model.Get<QuantityPropertySetDef>(id);
                    }
                if (ParentID != null)
                    foreach (var item in Parent.DefinitionSetsUp)
                    {
                        yield return item;
                    }
            }
        }

        internal override void SetModel(BLModel model)
        {
            _model = model;
            if (NameAliases != null) NameAliases.SetModel(model);
            if (DefinitionAliases != null) DefinitionAliases.SetModel(model);
        }

        public override string Validate()
        {
            var result = "";
            if (NameAliases != null) result += NameAliases.Validate();
            if (DefinitionAliases != null) result += DefinitionAliases.Validate();
            return result;
        }

        internal override IEnumerable<BLEntity> GetChildren()
        {
            if (NameAliases != null)
                foreach (var item in NameAliases)
                {
                    yield return item;
                }
            if (DefinitionAliases != null)
                foreach (var item in DefinitionAliases)
                {
                    yield return item;
                }
        }
    }
}
