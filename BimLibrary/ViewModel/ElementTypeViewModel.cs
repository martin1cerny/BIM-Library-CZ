using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Xbim.Ifc2x3.Kernel;
using Xbim.XbimExtensions.Interfaces;
using Xbim.IO;

namespace BimLibrary.ViewModel
{
    public class ElementTypeViewModel : INotifyPropertyChanged
    {
        protected XbimModel Model { get { return _type.ModelOf as XbimModel; } }

        protected IfcTypeProduct _type;

        public ElementTypeViewModel(IfcTypeProduct elementType)
        {
            _type = elementType;
        }

        #region Name
        private string _Name;

        public string Name
        {
            get { return _type.Name; }
            set { _type.Name = value; OnPropertyChanged("Name"); }
        }
        #endregion

        #region Description
        private string _Description;

        public string Description
        {
            get { return _type.Description; }
            set { _type.Description = value; OnPropertyChanged("Description"); }
        }
        #endregion
        


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
