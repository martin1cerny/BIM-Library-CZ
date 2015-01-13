using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLData
{
    public interface IBLModelEntity : INotifyPropertyChanged
    {
        Guid Id { get; set; }
        BLModel Model { get; }
        void OnPropertyChanged(string name);
        string Validate();
    }
}
