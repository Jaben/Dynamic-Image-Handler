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

using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

using DynamicImageHandler.ImageTools;

using FreeImageAPI;

namespace DynamicImageHandler.ImageTool.FreeImage
{
    /// <summary>
    /// The free image image tool.
    /// </summary>
    public class FreeImageImageTool : NativeImageTool
    {
        #region Public Methods

        /// <summary>
        /// The encode.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="imageFormat">
        /// The image format.
        /// </param>
        /// <returns>
        /// </returns>
        public override byte[] Encode(Bitmap source, ImageFormat imageFormat)
        {
            FIBITMAP dib = FreeImageAPI.FreeImage.CreateFromBitmap(source);

            if (dib.IsNull)
            {
                return base.Encode(source, imageFormat);
            }

            using (MemoryStream ms = new MemoryStream())
            {
                if (!FreeImageAPI.FreeImage.SaveToStream(
                        ref dib,
                        ms,
                        ConvertToFreeImageFormat(imageFormat),
                        ConvertToFreeImageSaveFlags(imageFormat),
                        FREE_IMAGE_COLOR_DEPTH.FICD_AUTO,
                        true))
                {
                    return base.Encode(source, imageFormat);
                }

                return ms.ToArray();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The convert to free image format.
        /// </summary>
        /// <param name="imageFormat">
        /// The image format.
        /// </param>
        /// <returns>
        /// </returns>
        private static FREE_IMAGE_FORMAT ConvertToFreeImageFormat(ImageFormat imageFormat)
        {
            if (imageFormat == ImageFormat.Png)
            {
                return FREE_IMAGE_FORMAT.FIF_PNG;
            }

            if (imageFormat == ImageFormat.Gif)
            {
                return FREE_IMAGE_FORMAT.FIF_GIF;
            }

            if (imageFormat == ImageFormat.Jpeg)
            {
                return FREE_IMAGE_FORMAT.FIF_JPEG;
            }

            if (imageFormat == ImageFormat.Tiff)
            {
                return FREE_IMAGE_FORMAT.FIF_TIFF;
            }

            return FREE_IMAGE_FORMAT.FIF_PNG;
        }

        /// <summary>
        /// The convert to free image save flags.
        /// </summary>
        /// <param name="imageFormat">
        /// The image format.
        /// </param>
        /// <returns>
        /// </returns>
        private static FREE_IMAGE_SAVE_FLAGS ConvertToFreeImageSaveFlags(ImageFormat imageFormat)
        {
            if (imageFormat == ImageFormat.Png)
            {
                return FREE_IMAGE_SAVE_FLAGS.PNG_Z_BEST_COMPRESSION;
            }

            if (imageFormat == ImageFormat.Gif)
            {
                return FREE_IMAGE_SAVE_FLAGS.DEFAULT;
            }

            if (imageFormat == ImageFormat.Jpeg)
            {
                return FREE_IMAGE_SAVE_FLAGS.JPEG_QUALITYGOOD;
            }

            if (imageFormat == ImageFormat.Tiff)
            {
                return FREE_IMAGE_SAVE_FLAGS.TIFF_LZW;
            }

            return FREE_IMAGE_SAVE_FLAGS.PNG_Z_BEST_COMPRESSION;
        }

        #endregion
    }
}