using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Xbim.Ifc2x3.MaterialResource;

namespace BimLibrary.ViewModel
{
    public class LayerViewModel : INotifyPropertyChanged
    {
        IfcMaterialLayer _layer;

        public LayerViewModel(IfcMaterialLayer layer)
        {
            _layer = layer;

            var notifier = layer as INotifyPropertyChanged;
            notifier.PropertyChanged += new PropertyChangedEventHandler(notifier_PropertyChanged);
        }

        void notifier_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Material")
                OnPropertyChanged("Material");
        }

        #region Material
        private MaterialViewModel _Material;

        public MaterialViewModel Material
        {
            get 
            {
                if (_Material == null)
                {
                    var material = App.Library.Materials.Where(m => m.Name == _layer.Material.Name).FirstOrDefault();
                    if (material == null)
                        throw new Exception("Material should exist in the library list");
                    _Material = material;
                }
                return _Material; 
            }
            set 
            {
                _Material = value;
                if (_Material != null)
                    _layer.Material = _Material.IfcMaterial;
                else
                    _layer.Material = null;
                OnPropertyChanged("Material"); 
            }
        }
        #endregion

        #region Thickness
        private float _Thickness;

        public double Thickness
        {
            get { return _layer.LayerThickness; }
            set { _layer.LayerThickness = value; OnPropertyChanged("Thickness"); }
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
