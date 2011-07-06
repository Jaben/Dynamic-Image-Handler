// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileSystemHelpers.cs" company="">
//   
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