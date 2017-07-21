// --------------------------------------------------------------------------------------------------------------------
// Copyright (c) 2009-2010 Esben Carlsen
// Forked Copyright (c) 2011-2017 Jaben Cargman and CaptiveAire Systems
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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace DynamicImageHandler.Utils
{
    public static class CollectionExtensions
    {
        public static IEnumerable<KeyValuePair<string, string>> ToKeyValuePairs(this NameValueCollection nameValueCollection)
        {
            if (nameValueCollection == null)
            {
                throw new ArgumentNullException("nameValueCollection");
            }

            foreach (string key in nameValueCollection.Keys)
            {
                string value = nameValueCollection[key];
                if (!string.IsNullOrWhiteSpace(value))
                {
                    yield return new KeyValuePair<string, string>(key, value);
                }
            }
        }

        public static string ToQueryString(this IDictionary<string, string> parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException("parameters");
            }

            return string.Join(
                "&",
                parameters.Where(kv => !string.IsNullOrEmpty(kv.Value)).Select(kv => $"{kv.Key.ToLower()}={kv.Value}"));
        }

        public static IEnumerable<KeyValuePair<string, string>> ParseParametersAsKeyValuePairs(this string parameters)
        {
            if (string.IsNullOrWhiteSpace(parameters))
                yield break;

            foreach (string item in parameters.Split('&'))
            {
                var vars = item.Split('=');

                if (vars.Length != 2)
                {
                    continue;
                }

                yield return new KeyValuePair<string, string>(vars[0].Trim().ToLower(), vars[1]);
            }
        }

        public static void AddRange<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IEnumerable<KeyValuePair<TKey, TValue>> values)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException("dictionary");
            }
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }

            foreach (var kv in values)
            {
                if (dictionary.ContainsKey(kv.Key))
                {
                    dictionary[kv.Key] = kv.Value;
                }
                else
                {
                    dictionary.Add(kv);
                }
            }
        }
    }
}