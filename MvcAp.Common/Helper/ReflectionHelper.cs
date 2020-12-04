using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace System
{
    public static class ReflectionHelper
    {
        public static object ConvertValue(Type targetType, object value)
        {
            string strValue = value as string;
            if (strValue != null)
            {
                if (string.IsNullOrWhiteSpace(strValue))
                {
                    value = null;
                }
            }

            if (value == null)
            {
                if (targetType.IsValueType)
                {
                    return Activator.CreateInstance(targetType);
                }
                else
                {
                    return null;
                }
            }

            if (value != null && !targetType.IsInstanceOfType(value))
            {
                value = Convert.ChangeType(value, targetType);
            }

            return value;
        }

        public static T ConvertValue<T>(this object value)
        {
            Type targetType = typeof(T);

            return (T)ConvertValue(targetType, value);
        }
    }
}