// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileSystemHelpers.cs" company="">
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
//   The file system helpers.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DynamicImageHandler.Utils
{
	using System.IO;
	using System.Linq;

	/// <summary>
	/// The file system helpers.
	/// </summary>
	internal class FileSystemHelpers
	{
		#region Methods

		/// <summary>
		/// The file exists.
		/// </summary>
		/// <param name="fileName">
		/// The file name.
		/// </param>
		/// <returns>
		/// The file exists.
		/// </returns>
		internal static bool FileExists(string fileName)
		{
			return File.Exists(fileName);
		}

		/// <summary>
		/// The to valid file name.
		/// </summary>
		/// <param name="key">
		/// The key.
		/// </param>
		/// <returns>
		/// The to valid file name.
		/// </returns>
		internal static string ToValidFileName(string key)
		{
			return Path.GetInvalidFileNameChars().Aggregate(key, (current, c) => current.Replace(c, '_'));
		}

		#endregion
	}
}