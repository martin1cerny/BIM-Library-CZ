using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLData
{
    public interface INamedEntity
    {
        Guid Id { get; }
        BLModel Model { get; }
        string Name { get; set; }
        string Definition { get; set; }
        BList<NameAlias> NameAliases { get; set; }
        BList<NameAlias> DefinitionAliases { get; set; }
        string LocalizedName { get; set; }
        string LocalizedDefinition { get; set; }
    }
}
