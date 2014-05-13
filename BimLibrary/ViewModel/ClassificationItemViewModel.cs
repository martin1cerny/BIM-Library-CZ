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

        public string Notation { 
            get 
            {
                if (_item.Notation == null)
                    return null;
                return _item.Notation.NotationValue; 
            }
            set
            {
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
    }
}
