// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEnumerableExtensions.cs" company="">
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
//   The i enumerable extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DynamicImageHandler.Utils
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	/// The i enumerable extensions.
	/// </summary>
	public static class IEnumerableExtensions
	{
		#region Public Methods

		/// <summary>
		/// 	Iterates through all sequence and performs specified action on each
		/// 	element
		/// </summary>
		/// <typeparam name="T">
		/// Sequence element type
		/// </typeparam>
		/// <param name="enumerable">
		/// Target enumeration
		/// </param>
		/// <param name="action">
		/// Action
		/// </param>
		/// <exception cref="System.ArgumentNullException">
		/// One of the input agruments is null
		/// </exception>
		public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
		{
			foreach (T elem in enumerable)
			{
				action(elem);
			}
		}

		#endregion
	}
}