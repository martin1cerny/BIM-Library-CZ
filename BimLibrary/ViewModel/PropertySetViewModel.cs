using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Xbim.Ifc2x3.Kernel;
using System.Collections.ObjectModel;

namespace BimLibrary.ViewModel
{
    public class PropertySetViewModel : INotifyPropertyChanged
    {
        private IfcPropertySet _pSet;

        public PropertySetViewModel(IfcPropertySet pSet)
        {
            _pSet = pSet;

            //TODO: fill in existing properties
        }

        public string Description {
            get
            {
                return _pSet.Description;
            }
            set
            {
                _pSet.Description = value;
                OnPropertyChanged("Description");
            }
        }

        public string Name
        {
            get { return _pSet.Name; }
            set { _pSet.Name = value; OnPropertyChanged("Name"); }
        }

        public ObservableCollection<PropertyViewModel> _properties = new ObservableCollection<PropertyViewModel>();
        public ObservableCollection<PropertyViewModel> Properties
        {
            get { return _properties; }
        }

        #region INotifyPropertyChanged implementation
        protected void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}
