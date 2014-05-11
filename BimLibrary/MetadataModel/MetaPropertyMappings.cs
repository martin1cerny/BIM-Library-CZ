using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;

namespace BimLibrary.MetadataModel
{
    public class MetaPropertyMappings : List<MetaPropertyMapping>
    {
        public static MetaPropertyMappings Open(Stream stream)
        {
            var serializer = new XmlSerializer(typeof(MetaPropertyMappings));
            var mappings = serializer.Deserialize(stream) as MetaPropertyMappings;
            return mappings;
        }

        public void Save(Stream stream)
        {
            var serializer = new XmlSerializer(typeof(MetaPropertyMappings));
            serializer.Serialize(stream, this);
        }
    }
}
