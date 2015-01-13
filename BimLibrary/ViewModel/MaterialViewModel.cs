using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.MaterialResource;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace BimLibrary.ViewModel
{
    public class MaterialViewModel : INotifyPropertyChanged
    {
        private IfcMaterial _material;
        public IfcMaterial IfcMaterial { get { return _material; } }

        public MaterialViewModel(IfcMaterial material)
        {
            _material = material;
        }

        public string Name
        {
            get { return _material.Name; }
            set { _material.Name = value; OnPropertyChanged("Name"); }
        }

        #region PropertySets
        private ObservableCollection<PropertySetViewModel> _PropertySets;

        public ObservableCollection<PropertySetViewModel> PropertySets
        {
            get
            {
                if (_PropertySets == null)
                {
                    _PropertySets = new ObservableCollection<PropertySetViewModel>();
                    //TODO: Fill in existing values from model

                    _PropertySets.CollectionChanged += new NotifyCollectionChangedEventHandler(_PropertySets_CollectionChanged);
                }
                return _PropertySets;
            }
            set { _PropertySets = value; OnPropertyChanged("PropertySets"); }
        }

        void _PropertySets_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }
        #endregion
    }
}
