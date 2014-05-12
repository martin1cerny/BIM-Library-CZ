using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.MaterialResource;
using System.ComponentModel;

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
