using BLData;
using BLData.Classification;
using BLData.PropertySets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLSpec
{
    public delegate void BLEntityActiveHandler(object sender, BLEntityActiveEventArgs args);

    public class BLEntityActiveEventArgs
    {
        private INamedEntity _entity;
        public INamedEntity Entity { get { return _entity; } }

        public BList<NameAlias> NameAliases
        {
            get
            {
                if (_entity == null)
                    return null;
                else
                    return _entity.NameAliases;
            }
        }

        public BList<NameAlias> DefinitionAliases
        {
            get
            {
                if (_entity == null)
                    return null;
                else
                    return _entity.DefinitionAliases;
            }
        }

        public BLEntityActiveEventArgs(INamedEntity entity)
        {
            _entity = entity;
        }
    }
}
