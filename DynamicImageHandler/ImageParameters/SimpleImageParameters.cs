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
using System.Linq;
using System.Security.Cryptography;
using System.Text;

using DynamicImageHandler.Utils;

namespace DynamicImageHandler.ImageParameters
{
    public class SimpleImageParameters : IImageParameters
    {
        private readonly Dictionary<string, string> _parameters = new Dictionary<string, string>();

        public virtual string Key => this.MD5HashString(this.ParametersAsString(), 64);

        public virtual IDictionary<string, string> Parameters => this._parameters;

        public virtual void AppendRawParameters(IEnumerable<KeyValuePair<string,string>> values)
        {
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }

            this.Parameters.AddRange(values);
        }

        protected string MD5HashString(string value, int maxLength)
        {
            using (var cryptoServiceProvider = new MD5CryptoServiceProvider())
            {
                var data = Encoding.ASCII.GetBytes(value);
                data = cryptoServiceProvider.ComputeHash(data);

                string str = SymCrypt.BytesToHexString(data);

                if (str.Length > maxLength)
                {
                    str = str.Substring(0, maxLength);
                }

                return str;
            }
        }

        protected virtual string ParametersAsString()
        {
            var builder = new StringBuilder();

            bool isFirst = true;

            // create a key for this item...
            foreach (var kv in this.Parameters.Where(kv => !string.IsNullOrEmpty(kv.Value)))
            {
                if (!isFirst)
                {
                    builder.Append("&");
                }

                builder.AppendFormat("{0}={1}", kv.Key.ToLower(), kv.Value);
                isFirst = false;
            }

            return builder.ToString();
        }
    }
}