// --------------------------------------------------------------------------------------------------------------------
// Copyright (c) 2009-2010 Esben Carlsen
// Forked Copyright (c) 2011-2015 Jaben Cargman and CaptiveAire Systems
//	
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.

// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA 
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace DynamicImageHandler.ImageParameters
{
    public static class ImageParameterExtensions
    {
        /// <exception cref="TargetInvocationException">An error occurred while setting the property value. For example, an index value specified for an indexed property is out of range. The <see cref="P:System.Exception.InnerException" /> property indicates the reason for the error.</exception>
        /// <exception cref="MethodAccessException">There was an illegal attempt to access a private or protected method inside a class. </exception>
        /// <exception cref="TargetParameterCountException">The number of parameters in <paramref name="index" /> does not match the number of parameters the indexed property takes. </exception>
        /// <exception cref="TargetException">The object does not match the target type, or a property is an instance property but <paramref name="obj" /> is null. </exception>
        /// <exception cref="InvalidCastException">This conversion is not supported.  -or-<paramref name="value" /> is null and <paramref name="conversionType" /> is a value type.-or-<paramref name="value" /> does not implement the <see cref="T:System.IConvertible" /> interface.</exception>
        /// <exception cref="OverflowException"><paramref name="value" /> represents a number that is out of the range of <paramref name="conversionType" />.</exception>
        /// <exception cref="TypeLoadException">A custom attribute type cannot be loaded. </exception>
        /// <exception cref="ArgumentNullException"><paramref name="parameters"/> is <see langword="null" />.</exception>
        public static T MapParameter<T>(this IImageParameters parameters) where T : IImageParameterMapping, new()
        {
            if (parameters == null)
            {
                throw new ArgumentNullException("parameters");
            }

            var mappedParamObj = new T();
            var properties =
                mappedParamObj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.SetProperty);

            foreach (var p in properties)
            {
                var paramNames =
                    p.GetCustomAttributes(typeof(ParameterNamesAttribute), true)
                        .OfType<ParameterNamesAttribute>()
                        .SelectMany(_ => _.Names)
                        .Concat(new[] { p.Name.ToLower() })
                        .Distinct()
                        .ToList();

                foreach (var name in paramNames)
                {
                    string strValue;
                    if (parameters.Parameters.TryGetValue(name, out strValue) && !string.IsNullOrWhiteSpace(strValue))
                    {
                        // map value...
                        try
                        {
                            object value = ConvertValue(p.PropertyType, strValue.Trim());
                            p.SetValue(mappedParamObj, value, null);
                        }
                        catch (InvalidCastException)
                        {
                            // do nothing
                        }
                    }
                }
            }

            return mappedParamObj;
        }

        private static object ConvertValue(Type conversionType, string strValue)
        {
            if (conversionType.IsGenericType && conversionType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                conversionType = (new NullableConverter(conversionType)).UnderlyingType;
            }

            if (conversionType == typeof(bool))
            {
                // special handling for bool
                bool bv;
                if (bool.TryParse(strValue, out bv))
                {
                    return bv;
                }

                if (strValue == "1")
                {
                    return true;
                }

                if (strValue == "0")
                {
                    return false;
                }
            }
            else if (conversionType == typeof(float))
            {
                float f;
                if (float.TryParse(strValue, NumberStyles.Float, CultureInfo.InvariantCulture, out f))
                {
                    return f;
                }
            }

            return Convert.ChangeType(strValue, conversionType);
        }
    }
}