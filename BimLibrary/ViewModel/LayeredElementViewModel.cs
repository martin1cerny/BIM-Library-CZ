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

                    var rel = Model.Instances.Where<IfcRelAssociatesMaterial>(r => r.RelatedObjects.Contains(_type)).FirstOrDefault();
                    if (rel != null)
                    {
                        var matLaySet = rel.RelatingMaterial as IfcMaterialLayerSet;
                        if (matLaySet != null)
                        {
                            foreach (var layer in matLaySet.MaterialLayers)
                            {
                                _layers.Add(new LayerViewModel(layer));
                            }
                        }
                    }

                    _layers.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(_layers_CollectionChanged);
                }
                return _layers; 
            }
        }

        void _layers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //keep underlying objects up to date
            throw new NotImplementedException();
        }
    }
}
