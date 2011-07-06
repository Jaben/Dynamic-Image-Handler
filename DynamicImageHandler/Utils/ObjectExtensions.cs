// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectExtensions.cs" company="">
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
//   The object extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DynamicImageHandler.Utils
{
	/// <summary>
	/// The object extensions.
	/// </summary>
	public static class ObjectExtensions
	{
		#region Public Methods

		/// <summary>
		/// The is null.
		/// </summary>
		/// <param name="object">
		/// The object.
		/// </param>
		/// <typeparam name="T">
		/// </typeparam>
		/// <returns>
		/// The is null.
		/// </returns>
		public static bool IsNull<T>(this T @object)
		{
			return Equals(@object, null);
		}

		#endregion
	}
}