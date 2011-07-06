// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IImageStore.cs" company="">
//   
// </copyright>
// <summary>
//   Interface defining classes to store and retrieve image data from a cache store
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DynamicImageHandler
{
	/// <summary>
	/// 	Interface defining classes to store and retrieve image data from a cache store
	/// </summary>
	public interface IImageStore
	{
		#region Public Methods

		/// <summary>
		/// The get image data.
		/// </summary>
		/// <param name="key">
		/// The key.
		/// </param>
		/// <returns>
		/// </returns>
		byte[] GetImageData(string key);

		/// <summary>
		/// The put image data.
		/// </summary>
		/// <param name="key">
		/// The key.
		/// </param>
		/// <param name="imageData">
		/// The image data.
		/// </param>
		void PutImageData(string key, byte[] imageData);

		#endregion
	}
}