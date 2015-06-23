// --------------------------------------------------------------------------------------------------------------------
// Copyright (c) 2009-2010 Esben Carlsen
// Forked Copyright (c) 2011-2015 Jaben Cargman and CaptiveAire Systems
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
using System.IO;
using System.Web.Hosting;

using DynamicImageHandler.ImageParameters;
using DynamicImageHandler.Utils;

namespace DynamicImageHandler.ImageProviders
{
    internal class FileSystemImageProvider : IImageProvider
    {
        private const string ImageLastUpdatedKey = "ImageLastUpdated";

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

        public byte[] GetImageData(IImageParameters parameters)
        {
            string fileName = this.GetMappedFileFromParams(parameters);

            return string.IsNullOrEmpty(fileName) ? null : File.ReadAllBytes(fileName);
        }
    }
}