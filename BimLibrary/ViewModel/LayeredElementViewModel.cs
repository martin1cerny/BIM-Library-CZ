using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Xbim.Ifc2x3.ProductExtension;
using Xbim.Ifc2x3.MaterialResource;
using System.Collections.Specialized;

namespace BimLibrary.ViewModel
{
    public class LayeredElementViewModel: ElementTypeViewModel
    {

        public LayeredElementViewModel(IfcElementType element) : base(element)
        {

        }

        private ObservableCollection<LayerViewModel> _layers;
        public ObservableCollection<LayerViewModel> Layers
        {
            get 
            {
                if (_layers == null)
                {
                    _layers = new ObservableCollection<LayerViewModel>();
                    _layers.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(_layers_CollectionChanged);
                }
                return _layers; 
            }
            set 
            {
                if (!Model.IsTransacting)
                    throw new Exception("Model has to be in transaction.");
                if (_layers != null)
                {
                    //set property set for underlying IfcElementType
                    var rel = Model.Instances.Where<IfcRelAssociatesMaterial>(r => r.RelatedObjects.Contains(_type)).FirstOrDefault();
                    if (rel != null)
                    {
                        
                        rel.RelatingMaterial = Model.Instances.New<IfcMaterialLayerSet>(ls =>
                        {
                            foreach (var item in value)
                            {
                                ls.MaterialLayers.Add_Reversible(Model.Instances.New<IfcMaterialLayer>(ml =>
                                {
                                    ml.Material = item.Material.IfcMaterial;
                                    ml.LayerThickness = item.Thickness;
                                }));
                            }

                        });
                    }
                    else
                    {
                        var mls = rel.RelatingMaterial as IfcMaterialLayerSet;
                        mls.MaterialLayers.Clear_Reversible();
                        foreach (var item in value)
                        {
                            mls.MaterialLayers.Add_Reversible(Model.Instances.New<IfcMaterialLayer>(ml =>
                            {
                                ml.Material = item.Material.IfcMaterial;
                                ml.LayerThickness = item.Thickness;
                            }));
                        }
                    }
                }
                _layers = value;

                OnPropertyChanged("Layers");
            }
        }

        #region ClassifiedAs
        private ObservableCollection<ClassificationItemViewModel> _ClassifiedAs;

        public ObservableCollection<ClassificationItemViewModel> ClassifiedAs
        {
            get {
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
        

        void _layers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //keep underlying objects up to date
            throw new NotImplementedException();
        }
    }
}
