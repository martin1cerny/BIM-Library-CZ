using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.ExternalReferenceResource;
using Xbim.IO;
using Xbim.XbimExtensions.Interfaces;
using System.ComponentModel;
using BimLibrary.MetadataModel;
using Xbim.Ifc2x3.MaterialResource;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.Ifc2x3.Kernel;

namespace BimLibrary.ViewModel
{
    public class ClassificationItemViewModel : INotifyPropertyChanged
    {
        private IfcClassificationItem _item;
        public IfcClassificationItem IfcClassificationItem { get { return _item; } }

        private IModel _model { get { return _item.ModelOf; } }

        private MetaPropertyMapping _pMap;
        public MetaPropertyMapping PropertyMapping { get { return _pMap; } }

        public ClassificationItemViewModel(IfcClassificationItem item)
        {
            _item = item;

            var lib = App.Library;
            _pMap = lib.PropertyMappings.Where(pm => pm.IfcEntityLabel == item.EntityLabel).FirstOrDefault();
            if (_pMap == null)
            {
                _pMap = new MetadataModel.MetaPropertyMapping() { IfcEntityLabel = item.EntityLabel };
                lib.PropertyMappings.Add(_pMap);
            }
        }

        public ClassificationItemViewModel(XbimModel model)
        {
            _item = model.Instances.New<IfcClassificationItem>(ci => {
                ci.Notation = model.Instances.New<IfcClassificationNotationFacet>();
            });
        }

        public string Notation { 
            get 
            {
                if (_item.Notation == null)
                    return null;
                return _item.Notation.NotationValue; 
            }
            set
            {
                var model = _item.ModelOf as XbimModel;
                if (model != null && !model.IsTransacting)
                    throw new Exception("Model has to be transacting.");

                if (_item.Notation == null)
                    _item.Notation = _item.ModelOf.Instances.New<IfcClassificationNotationFacet>();
                _item.Notation.NotationValue = value;
                OnPropertyChanged("Notation");
            }
        }

        public string Title
        {
            get
            {
                return _item.Title;
            }
            set
            {
                _item.Title = value;
                OnPropertyChanged("Title");
            }
        }

        private bool _isSelected;

        public bool IsSelected
        {
            get { return _isSelected; }
            set { _isSelected = value; OnPropertyChanged("IsSelected"); }
        }

        private bool _isExpanded;

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set { 
                _isExpanded = value;
                if (value && !_children.Any())
                    LoadChildren();
                OnPropertyChanged("IsExpanded"); 
            }
        }



        private ObservableCollection<ClassificationItemViewModel> _children = new ObservableCollection<ClassificationItemViewModel>();
        public ObservableCollection<ClassificationItemViewModel> Children
        {
            get { return _children;  }
        }

        private void LoadChildren()
        {
            if (_item.IsClassifyingItemIn.Any())
            {
                foreach (var rel in _item.IsClassifyingItemIn)
                {
                    foreach (var item in rel.RelatedItems)
                    {
                        _children.Add(new ClassificationItemViewModel(item));
                    }
                }
                OnPropertyChanged("Children");
            }
        }

        #region PropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }
        #endregion

        public IEnumerable<MaterialViewModel> GetMaterials()
        {
            var model = _item.ModelOf;

            foreach (var material in App.Library.Materials)
            {
                var ifcMat = material.IfcMaterial;
                var clasRel = model.Instances.Where<IfcMaterialClassificationRelationship>(r => r.ClassifiedMaterial == ifcMat && ItemIsInSelection( r.MaterialClassifications));
                foreach (var rel in clasRel)
                {
                    yield return material;
                }
            }
        }

        public IEnumerable<ElementTypeViewModel> GetElementTypes()
        {
            var model = _item.ModelOf;

            foreach (var type in App.Library.ElementTypes)
            {
                var rels = model.Instances.Where<IfcRelAssociatesClassification>(r => r.RelatedObjects.Contains(type.ElementType) && ItemIsInSelection(r.RelatingClassification));
                if (rels.Any())
                    yield return type;
            }
        }

        private bool ItemIsInSelection(IfcClassificationNotationSelect notationSelect)
        {
            var notation = notationSelect as IfcClassificationNotation;
            if (notation == null)
                return false;
            var facets = notation.NotationFacets;
            if (facets.Contains(_item.Notation))
                return true;
            else
                return false;
        }

        private bool ItemIsInSelection(IEnumerable< IfcClassificationNotationSelect> notationSelects)
        {
            foreach (var select in notationSelects)
            {
                if (ItemIsInSelection(select))
                    return true;
            }
            return false;
        }
    }
}
