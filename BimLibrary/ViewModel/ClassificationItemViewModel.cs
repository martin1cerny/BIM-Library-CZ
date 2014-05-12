using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.ExternalReferenceResource;
using Xbim.IO;
using Xbim.XbimExtensions.Interfaces;
using System.ComponentModel;

namespace BimLibrary.ViewModel
{
    public class ClassificationItemViewModel : INotifyPropertyChanged
    {
        private IfcClassificationItem _item;
        public IfcClassificationItem IfcClassificationItem { get { return _item; } }

        private IModel _model { get { return _item.ModelOf; } }

        public ClassificationItemViewModel(IfcClassificationItem item)
        {
            _item = item;
        }

        public ClassificationItemViewModel(XbimModel model)
        {
            _item = model.Instances.New<IfcClassificationItem>(ci => {
                ci.Notation = model.Instances.New<IfcClassificationNotationFacet>();
            });
        }

        public string Name { 
            get 
            { 
                return _item.Notation.NotationValue; 
            }
            set
            {
                if (_item.Notation == null)
                    _item.Notation = _item.ModelOf.Instances.New<IfcClassificationNotationFacet>();
                _item.Notation.NotationValue = value;
                OnPropertyChanged("Name");
            }
        }

        private ObservableCollection<ClassificationItemViewModel> _children;
        public ObservableCollection<ClassificationItemViewModel> Children
        {
            get {
                if (_children == null)
                {
                    LoadChildren();
                }
                return _children; 
            }
        }

        public void LoadChildren()
        {
            if (_item.IsClassifyingItemIn.Any())
            {
                _children = new ObservableCollection<ClassificationItemViewModel>();
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
    }
}
