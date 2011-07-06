// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileSystemImageProvider.cs" company="">
//   
// </copyright>
// <summary>
//   File System Image Provider
//   Read data from darddrive
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DynamicImageHandler.ImageProviders
{
	using System;
	using System.IO;
	using System.Web.Hosting;

	using DynamicImageHandler.Utils;

	/// <summary>
	/// 	File System Image Provider
	/// 	Read data from darddrive
	/// </summary>
	internal class FileSystemImageProvider : IImageProvider
	{
		#region Constants and Fields

		/// <summary>
		/// The image last updated key.
		/// </summary>
		private const string ImageLastUpdatedKey = "ImageLastUpdated";

		#endregion

		#region Public Methods

		/// <summary>
		/// 	The get image last updated.
		/// </summary>
		/// <param name="parameters">
		/// 	The parameters.
		/// </param>
		public void AddImageLastUpdatedParameter(IImageParameters parameters)
		{
			string fileName = this.GetMappedFileFromParams(parameters);

			if (!string.IsNullOrEmpty(fileName))
			{
				DateTime fileUpdated = File.GetLastWriteTimeUtc(fileName);

				if (parameters.Parameters.ContainsKey(ImageLastUpdatedKey))
				{
					parameters.Parameters.Remove(ImageLastUpdatedKey);
				}

				parameters.Parameters.Add(ImageLastUpdatedKey, fileUpdated.ToString());
			}
		}

		/// <summary>
		/// 	The get image data.
		/// </summary>
		/// <param name="parameters">
		/// 	The parameters.
		/// </param>
		/// <returns>
		/// </returns>
		public byte[] GetImageData(IImageParameters parameters)
		{
			string fileName = this.GetMappedFileFromParams(parameters);

			return string.IsNullOrEmpty(fileName) ? null : File.ReadAllBytes(fileName);
		}

		#endregion

		#region Methods

		/// <summary>
		/// 	The get mapped file from params.
		/// </summary>
		/// <param name="parameters">
		/// 	The parameters.
		/// </param>
		/// <returns>
		/// 	The get mapped file from params.
		/// </returns>
		private string GetMappedFileFromParams(IImageParameters parameters)
		{
			if (string.IsNullOrEmpty(parameters.ImageSrc))
			{
				return null;
			}

			if (FileSystemHelpers.FileExists(parameters.ImageSrc))
			{
				return parameters.ImageSrc;
			}

			string filePath = HostingEnvironment.MapPath(parameters.ImageSrc);

			return FileSystemHelpers.FileExists(filePath) ? filePath : null;
		}

		#endregion
	}
}