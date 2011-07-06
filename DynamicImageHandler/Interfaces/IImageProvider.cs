// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IImageProvider.cs" company="">
//   
// </copyright>
// <summary>
//   Interface for an image provider. A class for retrieving image data. For example file file system, CMS or database
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DynamicImageHandler
{
	/// <summary>
	/// 	Interface for an image provider. A class for retrieving image data. For example file file system, CMS or database
	/// </summary>
	public interface IImageProvider
	{
		#region Public Methods

		/// <summary>
		/// The add image last updated parameter.
		/// </summary>
		/// <param name="parameters">
		/// The parameters.
		/// </param>
		void AddImageLastUpdatedParameter(IImageParameters parameters);

		/// <summary>
		/// The get image data.
		/// </summary>
		/// <param name="parameters">
		/// The parameters.
		/// </param>
		/// <returns>
		/// </returns>
		byte[] GetImageData(IImageParameters parameters);

		#endregion
	}
}