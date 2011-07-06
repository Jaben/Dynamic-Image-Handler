// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IImageParameters.cs" company="">
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
//   The i image parameters.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DynamicImageHandler
{
	using System.Collections.Generic;
	using System.Web;

	/// <summary>
	/// The i image parameters.
	/// </summary>
	public interface IImageParameters
	{
		#region Public Properties

		/// <summary>
		/// Gets ImageSrc.
		/// </summary>
		string ImageSrc { get; }

		/// <summary>
		/// Gets Key.
		/// </summary>
		string Key { get; }

		/// <summary>
		/// Gets Parameters.
		/// </summary>
		IDictionary<string, string> Parameters { get; }

		#endregion

		#region Public Indexers

		/// <summary>
		/// The this.
		/// </summary>
		/// <param name="parameter">
		/// The parameter.
		/// </param>
		string this[string parameter] { get; }

		#endregion

		#region Public Methods

		/// <summary>
		/// The add collection.
		/// </summary>
		/// <param name="context">
		/// The context.
		/// </param>
		void AddCollection(HttpContext context);

		#endregion
	}
}