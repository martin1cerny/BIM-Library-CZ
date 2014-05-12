using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.ExternalReferenceResource;
using System.Collections.ObjectModel;
using Xbim.XbimExtensions.Interfaces;
using System.ComponentModel;

namespace BimLibrary.ViewModel
{
    public class ClassificationViewModel : INotifyPropertyChanged
    {
        private IfcClassification _classification;
        public IfcClassification IfcClassification { get { return _classification; } }
        private IModel _model { get { return _classification.ModelOf; } }


        public ClassificationViewModel(IfcClassification classification)
        {
            _classification = classification;
        }

        public string Name
        {
            get 
            {
                return _classification.Name;
            }
            set
            {
                _classification.Name = value;
                OnPropertyChanged("Name");
            }
        }

        private ObservableCollection<ClassificationItemViewModel> _rootClassificationItems;
        public ObservableCollection<ClassificationItemViewModel> RootClassificationItems
        {
            get
            {
                if (_rootClassificationItems == null)
                    LoadRootItems();
                return _rootClassificationItems;
            }
        }

        public void LoadRootItems()
        {
            _rootClassificationItems = new ObservableCollection<ClassificationItemViewModel>();
            foreach (var item in _classification.Contains.Where(c => !c.IsClassifiedItemIn.Any()))
            {
                _rootClassificationItems.Add(new ClassificationItemViewModel(item));
            }
            OnPropertyChanged("RootClassificationItems");
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
