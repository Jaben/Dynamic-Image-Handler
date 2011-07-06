// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SymCryptKey.cs" company="">
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
//   The i sym crypt key.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DynamicImageHandler.ImageParameters
{
	#region Using

	using System.Security.Cryptography;
	using System.Text;

	#endregion

	/// <summary>
	/// 	The i sym crypt key.
	/// </summary>
	public interface ISymCryptKey
	{
		#region Public Properties

		/// <summary>
		/// 	Gets Key.
		/// </summary>
		byte[] Key { get; }

		/// <summary>
		/// 	Gets Salt.
		/// </summary>
		byte[] Salt { get; }

		#endregion
	}

	/// <summary>
	/// 	The sym crypt key.
	/// </summary>
	public class SymCryptKey : ISymCryptKey
	{
		#region Constants and Fields

		/// <summary>
		/// 	The _key.
		/// </summary>
		private readonly byte[] _key;

		/// <summary>
		/// 	The _salt.
		/// </summary>
		private readonly byte[] _salt;

		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// 	Initializes a new instance of the <see cref="SymCryptKey"/> class.
		/// </summary>
		/// <param name="password">
		/// 	The password.
		/// </param>
		public SymCryptKey(string password)
		{
			byte[] salt = Encoding.ASCII.GetBytes(password.Substring(0, 10));

			var secretKey = new Rfc2898DeriveBytes(password, salt);

			this._key = secretKey.GetBytes(32);
			this._salt = secretKey.GetBytes(16);
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// 	Gets Key.
		/// </summary>
		public byte[] Key
		{
			get
			{
				return this._key;
			}
		}

		/// <summary>
		/// 	Gets Salt.
		/// </summary>
		public byte[] Salt
		{
			get
			{
				return this._salt;
			}
		}

		#endregion
	}
}