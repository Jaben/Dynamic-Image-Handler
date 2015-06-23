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

namespace DynamicImageHandler.ImageTool.Wpf
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Windows;
    using System.Windows.Interop;
    using System.Windows.Media.Imaging;

    using DynamicImageHandler.ImageTools;

    /// <summary>
    /// 	The wpf image tool.
    /// </summary>
    public class WpfImageTool : NativeImageTool
    {
        /// <summary>
        /// 	The encode.
        /// </summary>
        /// <param name="source">
        /// 	The source.
        /// </param>
        /// <param name="imageFormat">
        /// 	The image format.
        /// </param>
        /// <returns>
        /// </returns>
        public override byte[] Encode(Bitmap source, ImageFormat imageFormat)
        {
            if (imageFormat == ImageFormat.Png)
            {
                return EncodeBitmap(new PngBitmapEncoder(), source);
            }

            if (imageFormat == ImageFormat.Gif)
            {
                return EncodeBitmap(new GifBitmapEncoder(), source);
            }

            if (imageFormat == ImageFormat.Jpeg)
            {
                return EncodeBitmap(new JpegBitmapEncoder() { QualityLevel = 90 }, source);
            }

            if (imageFormat == ImageFormat.Tiff)
            {
                return EncodeBitmap(new TiffBitmapEncoder(), source);
            }

            return base.Encode(source, imageFormat);
        }

        /// <summary>
        /// 	The encode bitmap.
        /// </summary>
        /// <param name="bitmapEncoder">
        /// 	The bitmap encoder.
        /// </param>
        /// <param name="bitmap">
        /// 	The bitmap.
        /// </param>
        /// <returns>
        /// </returns>
        private static byte[] EncodeBitmap(BitmapEncoder bitmapEncoder, Bitmap bitmap)
        {
            BitmapSource bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(
                bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

            using (var ms = new MemoryStream())
            {
                bitmapEncoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                bitmapEncoder.Save(ms);
                return ms.ToArray();
            }
        }
    }
}