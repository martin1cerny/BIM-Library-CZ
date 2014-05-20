using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.MaterialResource;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace BimLibrary.MetadataModel
{
    public class MetaPropertyMapping : INotifyPropertyChanged
    {
        #region int IfcEntityLabel
        private int _ifcEntityLabel;
        public int IfcEntityLabel
        {
            get { return _ifcEntityLabel; }
            set
            {
                _ifcEntityLabel = value;
                OnPropertyChanged("IfcEntityLabel");
            }
        }
        #endregion

        private ObservableCollection<MetaPropertySet> _pSets = new ObservableCollection<MetaPropertySet>();
        public ObservableCollection<MetaPropertySet> PropertySets
        {
            get { return _pSets; }
            set { _pSets = value; OnPropertyChanged("PropertySets"); }
        }


        #region INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }
        #endregion
    }
}
