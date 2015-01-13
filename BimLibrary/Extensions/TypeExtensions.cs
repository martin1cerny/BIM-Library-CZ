using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BimLibrary.Extensions
{
    public static class TypeExtensions
    {
        public static bool IsNullableType(this Type type)
        {
            return type.IsGenericType
            && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>));
        }
    }
}
