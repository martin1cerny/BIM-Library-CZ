using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLData.Classification
{
    public class BLClassificationItem : BLModelNamedEntity
    {
        private string _code;

        public string Code
        {
            get { return _code; }
            set { Set("_code", _code, value); }
        }

        private string _selector;

        public string Selector
        {
            get { return _selector; }
            set { Set("_selector", _selector, value); }
        }
        

    }
}
