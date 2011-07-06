// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HiddenImageParameters.cs" company="">
// Copyright (c) 2009-2010 Esben Carlsen
// Forked by Jaben Cargman
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

namespace DynamicImageHandler.ImageParameters
{
	#region Using

	using System;
	using System.Diagnostics;
	using System.Web;

	using DynamicImageHandler.Properties;

	#endregion

	/// <summary>
	/// 	The hidden image parameters.
	/// </summary>
	public class HiddenImageParameters : SimpleImageParameters
	{
		#region Constants and Fields

		/// <summary>
		/// 	The _current key.
		/// </summary>
		private static ISymCryptKey _currentKey;

		#endregion

		#region Properties

		/// <summary>
		/// 	Gets CurrentCryptKey.
		/// </summary>
		protected static ISymCryptKey CurrentCryptKey
		{
			get
			{
				return _currentKey ?? (_currentKey = new SymCryptKey(Settings.Default.ImageParameterKey));
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// 	The get image class url.
		/// </summary>
		/// <param name="parameters">
		/// 	The parameters.
		/// </param>
		/// <returns>
		/// 	The get image class url.
		/// </returns>
		public static string GetImageClassUrl(string parameters)
		{
			var paramClass = new HiddenImageParameters();

			paramClass.LoadParametersFromString(parameters);

			// get the encoded parameters
			string encodedParams = paramClass.CreateEncodedParameterString();

			// url encode and return...
			return string.Format("id={0}", HttpUtility.UrlEncode(encodedParams));
		}

		/// <summary>
		/// 	The add collection.
		/// </summary>
		/// <param name="context">
		/// 	The context.
		/// </param>
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
				Debug.WriteLine("Exception: " + x.Message);
			}
		}

		/// <summary>
		/// 	The create encoded parameter string.
		/// </summary>
		/// <returns>
		/// 	The create encoded parameter string.
		/// </returns>
		public virtual string CreateEncodedParameterString()
		{
			return SymCrypt.Encrypt(this.ParametersAsString(), CurrentCryptKey);
		}

		/// <summary>
		/// 	The load encoded parameter string.
		/// </summary>
		/// <param name="paramString">
		/// 	The param string.
		/// </param>
		public virtual void LoadEncodedParameterString(string paramString)
		{
			string decrypted = SymCrypt.Decrypt(paramString, CurrentCryptKey);
			this.LoadParametersFromString(decrypted);
		}

		/// <summary>
		/// 	The load parameters from string.
		/// </summary>
		/// <param name="parameters">
		/// 	The parameters.
		/// </param>
		public void LoadParametersFromString(string parameters)
		{
			string[] split = parameters.Split('&');

			foreach (string item in split)
			{
				string[] vars = item.Split('=');

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

		#endregion
	}
}