using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Xbim.Ifc2x3.PropertyResource;
using Xbim.XbimExtensions.SelectTypes;
using BimLibrary.Extensions;

namespace BimLibrary.ViewModel
{
    public class PropertyViewModel : INotifyPropertyChanged
    {
        private IfcPropertySingleValue _property;

        public PropertyViewModel(IfcPropertySingleValue property)
        {
            _property = property;
        }

        #region Name
        public string Name
        {
            get { return _property.Name; }
            set { _property.Name = value; OnPropertyChanged("Name"); }
        }
        #endregion

        #region Description
        public string Description
        {
            get { return _property.Description; }
            set { _property.Description = value; OnPropertyChanged("Description"); }
        }
        #endregion

        public Type PropertyType { get { return _property.NominalValue.GetType(); } }

        public string Value
        {
            get 
            {
                return _property.NominalValue.ToString();
            }
            set
            {
                try
                {

                    var targetType = PropertyType.IsNullableType()
                        ? Nullable.GetUnderlyingType(PropertyType)
                        : PropertyType;

                    var newVal = Activator.CreateInstance(targetType, value);
                    _property.NominalValue = newVal as IfcValue;
                    OnPropertyChanged("Value");
                }
                catch (Exception)
                {
                    
                    throw;
                }
            }
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
