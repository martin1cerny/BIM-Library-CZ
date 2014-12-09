﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLData.PropertySets
{
    /// <summary>
    /// The element of IfcPropertyReferenceValue.
    /// </summary>
    public class TypePropertyReferenceValue : ReferenceSelect, IPropertyValueType
    {
        public DataType DataType { get; set; }
    }
}
