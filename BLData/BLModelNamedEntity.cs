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

        private string _description;

        public string Description
        {
            get { return _description; }
            set { var old = _description; Set("Description", () => _description = value, () => _description = old); }
        }
        
        
    }
}
