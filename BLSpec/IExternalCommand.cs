using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLSpec
{
    interface IExternalCommand
    {
        void Execute(BLData.BLModel model, System.Windows.Window mainWindow);
        string Name { get; }
        Guid ID { get; }
    }
}
