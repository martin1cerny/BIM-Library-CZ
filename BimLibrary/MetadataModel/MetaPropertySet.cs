using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace BimLibrary.MetadataModel
{
    public class MetaPropertySet : INotifyPropertyChanged
    {
        #region string Name
        private string _Name;

        public string Name
        {
            get { return _Name; }
            set { _Name = value; OnPropertyChanged("Name"); }
        }
        #endregion

        #region string Description
        private string _Description;

        public string Description
        {
            get { return _Description; }
            set { _Description = value; OnPropertyChanged("Description"); }
        }
        #endregion

        #region ObservableCollection<MetaProperty> Properties
        private ObservableCollection<MetaProperty> _properties = new ObservableCollection<MetaProperty>();
        public ObservableCollection<MetaProperty> Properties
        {
            get { return _properties; }
            set { _properties = value; OnPropertyChanged("Properties"); }
        }
        #endregion

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
