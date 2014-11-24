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
            set { Set("_name", _name, value); }
        }

        private string _description;

        public string Description
        {
            get { return _description; }
            set { Set("_description", _description, value); }
        }
        
        
    }
}
