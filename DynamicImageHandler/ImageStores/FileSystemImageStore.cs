// --------------------------------------------------------------------------------------------------------------------
// Copyright (c) 2009-2010 Esben Carlsen
// Forked Copyright (c) 2011-2017 Jaben Cargman and CaptiveAire Systems
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
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

using DynamicImageHandler.Properties;
using DynamicImageHandler.Utils;

namespace DynamicImageHandler.ImageStores
{
    /// <summary>
    ///     The file system image store.
    /// </summary>
    public class FileSystemImageStore : ImageStoreScavenger, IImageStore
    {
        /// <summary>
        ///     The cache file extension.
        /// </summary>
        private const string CacheFileExtension = ".config";

        /// <summary>
        ///     The m_ file timeout.
        /// </summary>
        private readonly TimeSpan _fileTimeout;

        /// <summary>
        ///     The m_ root.
        /// </summary>
        private readonly string _root;

        /// <summary>
        ///     Initializes a new instance of the <see cref="FileSystemImageStore" /> class.
        /// </summary>
        /// <param name="root">
        ///     The root.
        /// </param>
        /// <param name="fileTimeout">
        ///     The file timeout.
        /// </param>
        /// <param name="scavengeInterval">
        ///     The scavange interval.
        /// </param>
        /// <exception cref="ArgumentException">
        /// </exception>
        public FileSystemImageStore(string root, TimeSpan fileTimeout, TimeSpan scavengeInterval)
            : base(scavengeInterval, true)
        {
            if (!Directory.Exists(root))
            {
                throw new ArgumentException("Directory " + root + " does not exist", root);
            }

            this._root = root;
            this._fileTimeout = fileTimeout;

            // AppScheduler.ScheduleTask(this);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="FileSystemImageStore" /> class.
        /// </summary>
        public FileSystemImageStore()
            : this(GetFileSystemImageStorePath(), Settings.Default.ImageCacheTimeout, Settings.Default.ImageCleanRunInterval)
        {
        }

        /// <summary>
        ///     The get image data.
        /// </summary>
        /// <param name="key">
        ///     The key.
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
        ///     The put image data.
        /// </summary>
        /// <param name="key">
        ///     The key.
        /// </param>
        /// <param name="imageData">
        ///     The image data.
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

        /// <summary>
        ///     The clean old images.
        /// </summary>
        protected override void CleanOldImages()
        {
            DateTime timeoutUtc = DateTime.UtcNow.Subtract(this._fileTimeout);
            var files = Directory.GetFiles(this._root, "*" + CacheFileExtension);
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
        ///     The get file system image store path.
        /// </summary>
        /// <returns>
        ///     The get file system image store path.
        /// </returns>
        private static string GetFileSystemImageStorePath()
        {
            object tmp = AppDomain.CurrentDomain.GetData("DataDirectory");
            string appDataDirectory = tmp != null ? tmp.ToString() : string.Empty;
            return Settings.Default.FileSystemImageStorePath.Replace("|AppData|", appDataDirectory);
        }

        /// <summary>
        ///     The get file name.
        /// </summary>
        /// <param name="key">
        ///     The key.
        /// </param>
        /// <returns>
        ///     The get file name.
        /// </returns>
        private string GetFileName(string key)
        {
            string validFileName = FileSystemHelpers.ToValidFileName(key) + CacheFileExtension;
            string completefileName = Path.Combine(this._root, validFileName);
            return completefileName;
        }
    }
}