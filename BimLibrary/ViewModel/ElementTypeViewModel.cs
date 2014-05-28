using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Xbim.Ifc2x3.Kernel;
using Xbim.XbimExtensions.Interfaces;
using Xbim.IO;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace BimLibrary.ViewModel
{
    public class ElementTypeViewModel : INotifyPropertyChanged
    {
        protected XbimModel Model { get { return _type.ModelOf as XbimModel; } }

        protected IfcTypeProduct _type;
        public IfcTypeProduct ElementType { get { return _type; } }

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

        #region PropertySets
        private ObservableCollection<PropertySetViewModel> _PropertySets;

        public ObservableCollection<PropertySetViewModel> PropertySets
        {
            get {
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

        #region ClassifiedAs
        private ObservableCollection<ClassificationItemViewModel> _ClassifiedAs;

        public ObservableCollection<ClassificationItemViewModel> ClassifiedAs
        {
            get
            {
                if (_ClassifiedAs == null)
                {
                    _ClassifiedAs = new ObservableCollection<ClassificationItemViewModel>();

                    //TODO: Fill in with initial data

                    _ClassifiedAs.CollectionChanged += new NotifyCollectionChangedEventHandler(_ClassifiedAs_CollectionChanged);
                }
                return _ClassifiedAs;
            }
            set { _ClassifiedAs = value; OnPropertyChanged("ClassifiedAs"); }
        }

        void _ClassifiedAs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //keep underlying objects up to date
            throw new NotImplementedException();
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
