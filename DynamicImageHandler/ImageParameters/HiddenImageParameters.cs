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
using System.Security.Cryptography;
using System.Web;

using DynamicImageHandler.Properties;
using DynamicImageHandler.Utils;

namespace DynamicImageHandler.ImageParameters
{
    public class HiddenImageParameters : SimpleImageParameters
    {
        private static readonly Lazy<ISymCryptKey> _currentKey =
            new Lazy<ISymCryptKey>(() => new SymCryptKey(Settings.Default.ImageParameterKey));

        /// <exception cref="MemberAccessException" accessor="get">The <see cref="T:System.Lazy`1" /> instance is initialized to use the default constructor of the type that is being lazily initialized, and permissions to access the constructor are missing.</exception>
        protected static ISymCryptKey CurrentCryptKey
        {
            get
            {
                return _currentKey.Value;
            }
        }

        /// <exception cref="CryptographicException">The value of the <see cref="P:System.Security.Cryptography.SymmetricAlgorithm.Mode" /> parameter is not <see cref="F:System.Security.Cryptography.CipherMode.ECB" />, <see cref="F:System.Security.Cryptography.CipherMode.CBC" />, or <see cref="F:System.Security.Cryptography.CipherMode.CFB" />.</exception>
        public static string GetImageClassUrl(string parameters)
        {
            var paramClass = new HiddenImageParameters();

            paramClass.LoadParametersFromString(parameters);

            // get the encoded parameters
            string encodedParams = paramClass.CreateEncodedParameterString();

            // url encode and return...
            return string.Format("id={0}", HttpUtility.UrlEncode(encodedParams));
        }

        /// <exception cref="CryptographicException">The value of the <see cref="P:System.Security.Cryptography.SymmetricAlgorithm.Mode" /> parameter is not <see cref="F:System.Security.Cryptography.CipherMode.ECB" />, <see cref="F:System.Security.Cryptography.CipherMode.CBC" />, or <see cref="F:System.Security.Cryptography.CipherMode.CFB" />.</exception>
        public override void AddCollection(HttpContext context)
        {
            // process the hidden class into a string...
            string classData = context.Request.QueryString["id"];

            if (string.IsNullOrEmpty(classData))
            {
                return;
            }

            this.LoadEncodedParameterString(classData);
        }

        /// <exception cref="CryptographicException">The value of the <see cref="P:System.Security.Cryptography.SymmetricAlgorithm.Mode" /> parameter is not <see cref="F:System.Security.Cryptography.CipherMode.ECB" />, <see cref="F:System.Security.Cryptography.CipherMode.CBC" />, or <see cref="F:System.Security.Cryptography.CipherMode.CFB" />.</exception>
        public virtual string CreateEncodedParameterString()
        {
            return SymCrypt.Encrypt(this.ParametersAsString(), CurrentCryptKey);
        }

        /// <exception cref="CryptographicException">The value of the <see cref="P:System.Security.Cryptography.SymmetricAlgorithm.Mode" /> parameter is not <see cref="F:System.Security.Cryptography.CipherMode.ECB" />, <see cref="F:System.Security.Cryptography.CipherMode.CBC" />, or <see cref="F:System.Security.Cryptography.CipherMode.CFB" />.</exception>
        public virtual void LoadEncodedParameterString(string paramString)
        {
            string decrypted = SymCrypt.Decrypt(paramString, CurrentCryptKey);
            this.LoadParametersFromString(decrypted);
        }

        public void LoadParametersFromString(string parameters)
        {
            var split = parameters.Split('&');

            foreach (string item in split)
            {
                var vars = item.Split('=');

                if (vars.Length == 2)
                {
                    // add or overwrite the variable...
                    if (this.Parameters.ContainsKey(vars[0].ToLower()))
                    {
                        this.Parameters[vars[0].ToLower()] = vars[1];
                    }
                    else
                    {
                        this.Parameters.Add(vars[0].ToLower(), vars[1]);
                    }
                }
            }
        }
    }
}