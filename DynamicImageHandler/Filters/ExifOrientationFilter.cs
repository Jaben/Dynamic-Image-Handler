// // --------------------------------------------------------------------------------------------------------------------
// // Copyright (c) 2009-2010 Esben Carlsen
// // Forked Copyright (c) 2011-2019 Jaben Cargman and CaptiveAire Systems
// // 
// // This library is free software; you can redistribute it and/or
// // modify it under the terms of the GNU Lesser General Public
// // License as published by the Free Software Foundation; either
// // version 2.1 of the License, or (at your option) any later version.
// 
// // This library is distributed in the hope that it will be useful,
// // but WITHOUT ANY WARRANTY; without even the implied warranty of
// // MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// // Lesser General Public License for more details.
// 
// // You should have received a copy of the GNU Lesser General Public
// // License along with this library; if not, write to the Free Software
// // Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA 
// // --------------------------------------------------------------------------------------------------------------------

using System;
using System.Drawing;
using System.Linq;

using DynamicImageHandler.ImageParameters;

namespace DynamicImageHandler.Filters
{
    internal class ExifOrientationFilter : IImagePrefilter
    {
        public enum ExifOrientations
        {
            Unknown = 0,
            TopLeft = 1,
            TopRight = 2,
            BottomRight = 3,
            BottomLeft = 4,
            LeftTop = 5,
            RightTop = 6,
            RightBottom = 7,
            LeftBottom = 8
        }

        const int OrientationId = 0x112; //274

        public void Process(Bitmap bitmap)
        {
            var orientation = GetExifOrientation(bitmap);

            if (orientation == ExifOrientations.TopLeft || orientation == ExifOrientations.Unknown)
            {
                // nothing to do...
                return;
            }

            OrientImage(bitmap, orientation);

            // Set the image's orientation to TopLeft.
            SetImageOrientation(bitmap, ExifOrientations.TopLeft);
        }

        // Set the image's orientation.
        public static void SetImageOrientation(Image img, ExifOrientations orientation)
        {
            // Get the index of the orientation property.
            int orientationIndex = Array.IndexOf(img.PropertyIdList, OrientationId);

            // If there is no such property, do nothing.
            if (orientationIndex < 0) return;

            // Set the orientation value.
            var item = img.GetPropertyItem(OrientationId);
            item.Value[0] = (byte)orientation;
            img.SetPropertyItem(item);
        }

        static void OrientImage(Image img, ExifOrientations orientation)
        {
            // Orient the image.
            switch (orientation)
            {
                case ExifOrientations.Unknown:
                case ExifOrientations.TopLeft:
                    break;
                case ExifOrientations.TopRight:
                    img.RotateFlip(RotateFlipType.RotateNoneFlipX);
                    break;
                case ExifOrientations.BottomRight:
                    img.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    break;
                case ExifOrientations.BottomLeft:
                    img.RotateFlip(RotateFlipType.RotateNoneFlipY);
                    break;
                case ExifOrientations.LeftTop:
                    img.RotateFlip(RotateFlipType.Rotate90FlipX);
                    break;
                case ExifOrientations.RightTop:
                    img.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    break;
                case ExifOrientations.RightBottom:
                    img.RotateFlip(RotateFlipType.Rotate90FlipY);
                    break;
                case ExifOrientations.LeftBottom:
                    img.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    break;
            }
        }

        static ExifOrientations GetExifOrientation(Image img)
        {
            if (img.PropertyIdList.All(s => s != OrientationId))
                return ExifOrientations.Unknown;

            var prop = img.GetPropertyItem(OrientationId);

            return (ExifOrientations)BitConverter.ToUInt16(prop.Value, 0);
        }
    }
}