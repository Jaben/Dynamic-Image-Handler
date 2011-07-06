// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileSystemImageStore.cs" company="">
//   
// </copyright>
// <summary>
//   The file system image store.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DynamicImageHandler.ImageStores
{
	#region Using

	using System;
	using System.Diagnostics;
	using System.IO;
	using System.Threading;

	using DynamicImageHandler.Properties;
	using DynamicImageHandler.Utils;

	#endregion

	/// <summary>
	/// 	The file system image store.
	/// </summary>
	public class FileSystemImageStore : ImageStoreScavanger, IImageStore
	{
		#region Constants and Fields

		/// <summary>
		/// 	The cache file extension.
		/// </summary>
		private const string CacheFileExtension = ".config";

		/// <summary>
		/// 	The m_ file timeout.
		/// </summary>
		private readonly TimeSpan m_FileTimeout;

		/// <summary>
		/// 	The m_ root.
		/// </summary>
		private readonly string m_Root;

		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// 	Initializes a new instance of the <see cref="FileSystemImageStore"/> class.
		/// </summary>
		/// <param name="root">
		/// 	The root.
		/// </param>
		/// <param name="fileTimeout">
		/// 	The file timeout.
		/// </param>
		/// <param name="scavangeInterval">
		/// 	The scavange interval.
		/// </param>
		/// <exception cref="ArgumentException">
		/// </exception>
		public FileSystemImageStore(string root, TimeSpan fileTimeout, TimeSpan scavangeInterval)
			: base(scavangeInterval, true)
		{
			if (!Directory.Exists(root))
			{
				throw new ArgumentException("Directory " + root + " does not exist", root);
			}

			this.m_Root = root;
			this.m_FileTimeout = fileTimeout;

			// AppScheduler.ScheduleTask(this);
		}

		/// <summary>
		/// 	Initializes a new instance of the <see cref = "FileSystemImageStore" /> class.
		/// </summary>
		public FileSystemImageStore()
			: this(GetFileSystemImageStorePath(), Settings.Default.ImageCacheTimeout, Settings.Default.ImageCleanRunInterval)
		{
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// 	The get image data.
		/// </summary>
		/// <param name="key">
		/// 	The key.
		/// </param>
		/// <returns>
		/// </returns>
		public byte[] GetImageData(string key)
		{
			// Don't check file age here, to slow, let the scavanger handle that
			string fileName = this.GetFileName(key);
			return !FileSystemHelpers.FileExists(fileName) ? null : File.ReadAllBytes(fileName);
		}

		/// <summary>
		/// 	The put image data.
		/// </summary>
		/// <param name="key">
		/// 	The key.
		/// </param>
		/// <param name="imageData">
		/// 	The image data.
		/// </param>
		public void PutImageData(string key, byte[] imageData)
		{
			// Do writing in thread pool, so request can return as quickly as possible
			ThreadPool.QueueUserWorkItem(
				o =>
					{
						try
						{
							string fileName = this.GetFileName(key);
							File.WriteAllBytes(fileName, imageData);
						}
						catch (Exception e)
						{
							// Something unexpected happend
							Trace.Write(e);
						}
					});
		}

		#endregion

		#region Methods

		/// <summary>
		/// 	The clean old images.
		/// </summary>
		protected override void CleanOldImages()
		{
			DateTime timeoutUtc = DateTime.UtcNow.Subtract(this.m_FileTimeout);
			string[] files = Directory.GetFiles(this.m_Root, "*" + CacheFileExtension);
			foreach (string fileName in files)
			{
				try
				{
					var fi = new FileInfo(fileName);
					if (fi.LastWriteTimeUtc < timeoutUtc)
					{
						fi.Attributes = FileAttributes.Normal;
						fi.Delete();
					}
				}
				catch (Exception e)
				{
					// Something unexpected happend
					Trace.Write(e);
				}
			}
		}

		/// <summary>
		/// 	The get file system image store path.
		/// </summary>
		/// <returns>
		/// 	The get file system image store path.
		/// </returns>
		private static string GetFileSystemImageStorePath()
		{
			object tmp = AppDomain.CurrentDomain.GetData("DataDirectory");
			string appDataDirectory = tmp != null ? tmp.ToString() : string.Empty;
			return Settings.Default.FileSystemImageStorePath.Replace("|AppData|", appDataDirectory);
		}

		/// <summary>
		/// 	The get file name.
		/// </summary>
		/// <param name="key">
		/// 	The key.
		/// </param>
		/// <returns>
		/// 	The get file name.
		/// </returns>
		private string GetFileName(string key)
		{
			string validFileName = FileSystemHelpers.ToValidFileName(key) + CacheFileExtension;
			string completefileName = Path.Combine(this.m_Root, validFileName);
			return completefileName;
		}

		#endregion
	}
}