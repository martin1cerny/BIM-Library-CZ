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

        private ObservableCollection<MetaPropertySet> _pSets;
        public ObservableCollection<MetaPropertySet> PropertySets
        {
            get 
            {
                if (_pSets == null)
                {
                    _pSets = new ObservableCollection<MetaPropertySet>();
                    InitPsetCollection();
                }
                return _pSets; 
            }
            set 
            { 
                _pSets = value;
                InitPsetCollection();
                OnPropertyChanged("PropertySets");  
            }
        }

        private void InitPsetCollection()
        {
            _pSets.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(_pSets_CollectionChanged);
        }


        //TODO: Implement synchonization with the elements already classified as this
        void _pSets_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    break;
            }
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
