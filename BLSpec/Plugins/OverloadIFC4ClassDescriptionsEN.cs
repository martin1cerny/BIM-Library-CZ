using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLSpec.Plugins
{
    internal class OverloadIFC4ClassDescriptionsEN: LoadIFC4ClassDescriptionsCZ
    {
        public OverloadIFC4ClassDescriptionsEN()
        {
            Lang = "en-US";
        }

        public override string Name
        {
            get
            {
                return "Overload IFC descriptions EN";
            }
        }
        private Guid _id = new Guid("903D318A-3962-4BBC-8740-AC02B3F203A1");

        public override Guid ID
        {
            get { return _id; }
        }
    }
}
