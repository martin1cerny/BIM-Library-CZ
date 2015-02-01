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
        private BLEntity _entity;
        public BLEntity Entity { get { return _entity; } }

        public BList<NameAlias> NameAliases
        {
            get
            {
                if (_entity == null)
                    return null;
                var clsi = _entity as BLClassificationItem;
                if (clsi != null)
                    return clsi.NameAliases;
                var pset = _entity as QuantityPropertySetDef;
                if (pset != null)
                    return pset.NameAliases;
                var prop = _entity as QuantityPropertyDef;
                if (prop != null)
                    return prop.NameAliases;
                return null;
            }
        }

        public BList<NameAlias> DefinitionAliases
        {
            get
            {
                if (_entity == null)
                    return null;
                var clsi = _entity as BLClassificationItem;
                if (clsi != null)
                    return clsi.DefinitionAliases;
                var pset = _entity as QuantityPropertySetDef;
                if (pset != null)
                    return pset.DefinitionAliases;
                var prop = _entity as QuantityPropertyDef;
                if (prop != null)
                    return prop.DefinitionAliases;
                return null;
            }
        }

        public BLEntityActiveEventArgs(BLEntity entity)
        {
            _entity = entity;
        }
    }
}
