// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HiddenImageParameters.cs" company="">
// Copyright (c) 2009-2010 Esben Carlsen
// Forked by Jaben Cargman and CaptiveAire Systems
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
// </copyright>
// <summary>
//   The hidden image parameters.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Web;

using DynamicImageHandler.Properties;
using DynamicImageHandler.Utils;

namespace DynamicImageHandler.ImageParameters
{
    public class HiddenImageParameters : SimpleImageParameters
    {
        private static ISymCryptKey _currentKey;

        protected static ISymCryptKey CurrentCryptKey
        {
            get
            {
                return _currentKey ?? (_currentKey = new SymCryptKey(Settings.Default.ImageParameterKey));
            }
        }

        public static string GetImageClassUrl(string parameters)
        {
            var paramClass = new HiddenImageParameters();

            paramClass.LoadParametersFromString(parameters);

            // get the encoded parameters
            string encodedParams = paramClass.CreateEncodedParameterString();

            // url encode and return...
            return string.Format("id={0}", HttpUtility.UrlEncode(encodedParams));
        }

        public override void AddCollection(HttpContext context)
        {
            try
            {
                // process the hidden class into a string...
                if (!string.IsNullOrEmpty(context.Request.QueryString["id"]))
                {
                    string classData = context.Request.QueryString["id"];
                    this.LoadEncodedParameterString(classData);
                }
            }
            catch (Exception x)
            {
                Trace.WriteLine("Exception: " + x.Message);
            }
        }

        public virtual string CreateEncodedParameterString()
        {
            return SymCrypt.Encrypt(this.ParametersAsString(), CurrentCryptKey);
        }

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