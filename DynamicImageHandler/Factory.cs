// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Factory.cs" company="">
//   
// </copyright>
// <summary>
//   Generic factory class for creating IImageStore, IImageTool and IImageProvider
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DynamicImageHandler
{
	using System;
	using System.Configuration;

	using DynamicImageHandler.Properties;
	using DynamicImageHandler.Utils;

	/// <summary>
	/// 	Generic factory class for creating IImageStore, IImageTool and IImageProvider
	/// </summary>
	public class Factory
	{
		#region Constants and Fields

		/// <summary>
		/// The s_ sync lock.
		/// </summary>
		private static readonly object s_SyncLock = new object();

		/// <summary>
		/// The s_ image provider.
		/// </summary>
		private static IImageProvider s_ImageProvider;

		/// <summary>
		/// The s_ image store.
		/// </summary>
		private static IImageStore s_ImageStore;

		/// <summary>
		/// The s_ image tool.
		/// </summary>
		private static IImageTool s_ImageTool;

		#endregion

		#region Public Methods

		/// <summary>
		/// 	Creates an unique instance of the image parameter class...
		/// </summary>
		/// <returns>
		/// </returns>
		public static IImageParameters GetImageParameters()
		{
			Type imageParamters = Type.GetType(Settings.Default.ImageParametersType);
			if (imageParamters == null)
			{
				throw new ConfigurationErrorsException(
					string.Format("Unable to resolve image tool type: {0}", Settings.Default.ImageParametersType));
			}

			return Activator.CreateInstance(imageParamters) as IImageParameters;
		}

		/// <summary>
		/// The get image provider.
		/// </summary>
		/// <returns>
		/// </returns>
		/// <exception cref="ConfigurationErrorsException">
		/// </exception>
		public static IImageProvider GetImageProvider()
		{
			if (s_ImageProvider.IsNull())
			{
				lock (s_SyncLock)
				{
					if (s_ImageProvider.IsNull())
					{
						Type imageProviderType = Type.GetType(Settings.Default.ImageProviderType);
						if (imageProviderType.IsNull())
						{
							throw new ConfigurationErrorsException(
								string.Format("Unable to resolve image provider type: {0}", Settings.Default.ImageProviderType));
						}

						s_ImageProvider = Activator.CreateInstance(imageProviderType) as IImageProvider;
					}
				}
			}

			return s_ImageProvider;
		}

		/// <summary>
		/// The get image store.
		/// </summary>
		/// <returns>
		/// </returns>
		/// <exception cref="ConfigurationErrorsException">
		/// </exception>
		public static IImageStore GetImageStore()
		{
			if (s_ImageStore.IsNull())
			{
				lock (s_SyncLock)
				{
					if (s_ImageStore.IsNull())
					{
						Type imageStoreType = Type.GetType(Settings.Default.ImageStoreType);
						if (imageStoreType.IsNull())
						{
							throw new ConfigurationErrorsException(
								string.Format("Unable to resolve image store type: {0}", Settings.Default.ImageStoreType));
						}

						s_ImageStore = Activator.CreateInstance(imageStoreType) as IImageStore;
					}
				}
			}

			return s_ImageStore;
		}

		/// <summary>
		/// The get image tool.
		/// </summary>
		/// <returns>
		/// </returns>
		/// <exception cref="ConfigurationErrorsException">
		/// </exception>
		public static IImageTool GetImageTool()
		{
			if (s_ImageTool.IsNull())
			{
				lock (s_SyncLock)
				{
					if (s_ImageTool.IsNull())
					{
						Type imageToolType = Type.GetType(Settings.Default.ImageToolType);
						if (imageToolType.IsNull())
						{
							throw new ConfigurationErrorsException(
								string.Format("Unable to resolve image tool type: {0}", Settings.Default.ImageToolType));
						}

						s_ImageTool = Activator.CreateInstance(imageToolType) as IImageTool;
					}
				}
			}

			return s_ImageTool;
		}

		#endregion
	}
}