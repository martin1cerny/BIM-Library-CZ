using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.ExternalReferenceResource;
using Xbim.IO;

namespace BimLibrary.ViewModel
{
    public class ClassificationViewModel
    {
        private IfcClassificationItem _item;
        public IfcClassificationItem IfcClassificationItem { get { return _item; } }

        public ClassificationViewModel(IfcClassificationItem item)
        {
            _item = item;
        }

        public ClassificationViewModel(XbimModel model)
        {
            _item = model.Instances.New<IfcClassificationItem>(ci => {
                ci.Notation = model.Instances.New<IfcClassificationNotationFacet>();
            });
        }

        public string Name { 
            get 
            { 
                return _item.Notation.NotationValue; 
            }
            set
            {
                if (_item.Notation == null)
                    _item.Notation = _item.ModelOf.Instances.New<IfcClassificationNotationFacet>();
                _item.Notation.NotationValue = value;
            }
        }

        private ObservableCollection<ClassificationViewModel> _children;
        public ObservableCollection<ClassificationViewModel> Children
        {
            get { return _children; }
        }
    }
}
