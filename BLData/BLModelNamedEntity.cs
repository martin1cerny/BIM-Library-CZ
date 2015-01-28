using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLData
{
    public abstract class BLModelNamedEntity : BLModelEntity
    {
        private string _name;

        public string Name
        {
            get { return _name; }
            set { var old = _name; Set("Name", ()=> _name = value, () => _name = old); }
        }

        private string _definition;

        public string Definition
        {
            get { return _definition; }
            set { var old = _definition; Set("Definition", () => _definition = value, () => _definition = old); }
        }
        
        
    }
}
