using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xbim.ExpressParser
{
    class Inheritance : Section
    {
        public bool IsAbstract { get; set; }
        public IEnumerable<string> SubtypeOf { get; set; }
    }
}
