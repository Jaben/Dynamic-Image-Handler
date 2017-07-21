// // --------------------------------------------------------------------------------------------------------------------
// // Copyright (c) 2009-2010 Esben Carlsen
// // Forked Copyright (c) 2011-2017 Jaben Cargman and CaptiveAire Systems
// // 
// // This library is free software; you can redistribute it and/or
// // modify it under the terms of the GNU Lesser General Public
// // License as published by the Free Software Foundation; either
// // version 2.1 of the License, or (at your option) any later version.
// 
// // This library is distributed in the hope that it will be useful,
// // but WITHOUT ANY WARRANTY; without even the implied warranty of
// // MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// // Lesser General Public License for more details.
// 
// // You should have received a copy of the GNU Lesser General Public
// // License along with this library; if not, write to the Free Software
// // Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA 
// // --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web;

namespace DynamicImageHandler.ImageParameters
{
    public static class ImageParameterExtensions
    {
        public static IDictionary<string, ImageFormat> ImageFormatLookup = new Dictionary<string, ImageFormat>(StringComparer.InvariantCultureIgnoreCase)
            {
                { "png", ImageFormat.Png },
                { "gif", ImageFormat.Gif },
                { "jpg", ImageFormat.Jpeg },
                { "tif", ImageFormat.Tiff }
            };

        public static ImageFormat GetImageFormat(this IImageParameters parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException("parameters");
            }

            string format = parameters.GetValueOrEmpty("format");

            if (!string.IsNullOrWhiteSpace(format))
            {
                ImageFormat imageFormat;
                if (ImageFormatLookup.TryGetValue(format, out imageFormat))
                {
                    return imageFormat;
                }
            }

            // default is JPEG
            return ImageFormat.Jpeg;
        }

        public static string GetValueOrEmpty(this IImageParameters imageParameters, string name)
        {
            if (imageParameters == null)
            {
                throw new ArgumentNullException("imageParameters");
            }

            string value;
            imageParameters.Parameters.TryGetValue(name, out value);

            if (string.IsNullOrWhiteSpace(value))
            {
                value = string.Empty;
            }

            return value;
        }

        public static string GetImageSrc(this IImageParameters imageParameters)
        {
            if (imageParameters == null)
            {
                throw new ArgumentNullException("imageParameters");
            }

            return imageParameters.GetValueOrEmpty("src");
        }

        static ConcurrentDictionary<Type, KeyValuePair<string, Action<object, string>>[]> _mappedParameterLookup =
            new ConcurrentDictionary<Type, KeyValuePair<string, Action<object, string>>[]>();

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

            var mappings = _mappedParameterLookup.GetOrAdd(typeof(T), paramObjectType => GetParameterMappings(paramObjectType).ToArray());

            var mappedParamObj = new T();

            foreach (var map in mappings)
            {
                string stringValue;
                if (parameters.Parameters.TryGetValue(map.Key, out stringValue) && !string.IsNullOrWhiteSpace(stringValue))
                {
                    map.Value(mappedParamObj, stringValue);
                }
            }

            return mappedParamObj;
        }

        static IEnumerable<KeyValuePair<string, Action<object, string>>> GetParameterMappings(Type paramObjectType)
        {
            var properties = paramObjectType
                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.SetProperty);

            foreach (var p in properties)
            {
                var paramNames = p.GetCustomAttributes(typeof(ParameterNamesAttribute), true).OfType<ParameterNamesAttribute>()
                    .SelectMany(_ => _.Names).Concat(new[] { p.Name.ToLower() }).Distinct().ToList();

                foreach (var name in paramNames)
                {
                    yield return new KeyValuePair<string, Action<object, string>>(
                        name,
                        (o, s) =>
                            {
                                try
                                {
                                    object value = ConvertValue(p.PropertyType, s.Trim());
                                    p.SetValue(o, value, null);
                                }
                                catch (InvalidCastException)
                                {
                                    // do nothing
                                }
                            });
                }
            }
        }

        static object ConvertValue(Type conversionType, string stringValue)
        {
            if (conversionType.IsGenericType && conversionType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                conversionType = (new NullableConverter(conversionType)).UnderlyingType;
            }

            if (conversionType == typeof(bool))
            {
                // special handling for bool
                bool bv;
                if (bool.TryParse(stringValue, out bv))
                {
                    return bv;
                }

                if (stringValue == "1")
                {
                    return true;
                }

                if (stringValue == "0")
                {
                    return false;
                }
            }
            else if (conversionType == typeof(float))
            {
                float f;
                if (float.TryParse(stringValue, NumberStyles.Float, CultureInfo.InvariantCulture, out f))
                {
                    return f;
                }
            }

            return Convert.ChangeType(stringValue, conversionType);
        }
    }
}