using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.XbimExtensions.SelectTypes;
using System.ComponentModel;
using System.Xml.Serialization;

namespace BimLibrary.MetadataModel
{
    public class MetaProperty : INotifyPropertyChanged
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

        #region Type Type
        private Type _Type;

        public Type Type
        {
            get { return _Type; }
            set {
                if (typeof(IfcValue).IsAssignableFrom(value))
                {
                    _Type = value;
                    OnPropertyChanged("Type");
                }
                else
                    throw new Exception("Only IfcValue subtypes are supported.");
            }
        }
        #endregion
        
        
        public string DefaultValue { get; set; }

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
