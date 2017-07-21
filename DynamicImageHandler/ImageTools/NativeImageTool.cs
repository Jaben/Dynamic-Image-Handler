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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

using DynamicImageHandler.Utils;

namespace DynamicImageHandler.ImageTools
{
    /// <summary>
    ///     The native image tool.
    /// </summary>
    public class NativeImageTool : IImageTool
    {
        /// <summary>
        ///     The encode.
        /// </summary>
        /// <param name="source">
        ///     The source.
        /// </param>
        /// <param name="imageFormat">
        ///     The image format.
        /// </param>
        /// <returns>
        /// </returns>
        public virtual byte[] Encode(Bitmap source, ImageFormat imageFormat)
        {
            using (var ms = new MemoryStream())
            {
                source.Save(ms, imageFormat);
                return ms.ToArray();
            }
        }

        /// <summary>
        ///     The resize.
        /// </summary>
        /// <param name="source">
        ///     The source.
        /// </param>
        /// <param name="width">
        ///     The width.
        /// </param>
        /// <param name="height">
        ///     The height.
        /// </param>
        /// <param name="preservePerspective">
        ///     The preserve perspective.
        /// </param>
        /// <param name="keepSquare"></param>
        /// <returns>
        /// </returns>
        public virtual Bitmap Resize(Bitmap source, int? width, int? height, bool preservePerspective, bool keepSquare, Color bgColor)
        {
            int destX = 0;
            int destY = 0;
            int destWidth = width ?? 1;
            int destHeight = height ?? 1;

            int newBitmapWidth = destWidth;
            int newBitmapHeight = destHeight;

            if (preservePerspective)
            {
                double maxWidth = width.HasValue ? Convert.ToDouble(width.Value) : 0;
                double maxHeight = height.HasValue ? Convert.ToDouble(height.Value) : 0;

                if (maxWidth > 0 && maxHeight <= 0)
                {
                    maxHeight = (maxWidth / source.Width) * source.Height;
                }
                else if (maxHeight > 0 && maxWidth <= 0)
                {
                    maxWidth = (maxHeight / source.Height) * source.Width;
                }

                double aspectRatio = source.Width / (double)source.Height;
                double boxRatio = maxWidth / maxHeight;
                double scaleFactor;

                if (boxRatio > aspectRatio)
                {
                    // Use height, since that is the most restrictive dimension of box.
                    scaleFactor = maxHeight / source.Height;
                }
                else
                {
                    scaleFactor = maxWidth / source.Width;
                }

                double doubleDestWidth = source.Width * scaleFactor;
                double doubleDestHeight = source.Height * scaleFactor;

                if (keepSquare && doubleDestWidth < maxWidth)
                {
                    destX = Convert.ToInt32(Math.Floor(((maxWidth - doubleDestWidth) * .5)));
                }

                if (keepSquare && doubleDestHeight < maxHeight)
                {
                    destY = Convert.ToInt32(Math.Floor(((maxHeight - doubleDestHeight) * .5)));
                }

                newBitmapWidth = destWidth = Math.Max(Convert.ToInt32(Math.Floor(doubleDestWidth)), 1);
                newBitmapHeight = destHeight = Math.Max(Convert.ToInt32(Math.Floor(doubleDestHeight)), 1);

                if (keepSquare)
                {
                    newBitmapWidth = Math.Max(Convert.ToInt32(Math.Floor(maxWidth)), 1);
                    newBitmapHeight = Math.Max(Convert.ToInt32(Math.Floor(maxHeight)), 1);
                }
            }
            else
            {
                newBitmapWidth = Math.Max(destWidth, 1);
                newBitmapHeight = Math.Max(destHeight, 1);
            }

            var bitmap = new Bitmap(newBitmapWidth, newBitmapHeight, PixelFormat.Format32bppArgb);

            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.Clear(Color.Transparent);

                if (bgColor != Color.Transparent)
                {
                    using (var backgroundBrush = new SolidBrush(bgColor))
                    {
                        graphics.FillRectangle(backgroundBrush, new Rectangle(0, 0, newBitmapWidth, newBitmapHeight));
                    }
                }

                graphics.CompositingMode = CompositingMode.SourceOver;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

                using (var imageAttr = new ImageAttributes())
                {
                    imageAttr.SetWrapMode(WrapMode.TileFlipXY);

                    graphics.DrawImage(
                        source,
                        new Rectangle(destX, destY, destWidth, destHeight),
                        0,
                        0,
                        source.Width,
                        source.Height,
                        GraphicsUnit.Pixel,
                        imageAttr);

                    return bitmap;
                }
            }
        }

        /// <summary>
        ///     The rotate.
        /// </summary>
        /// <param name="source">
        ///     The source.
        /// </param>
        /// <param name="angle">
        ///     The angle.
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        public virtual Bitmap Rotate(Bitmap source, float angle)
        {
            if (source.IsDefault())
            {
                throw new ArgumentNullException("source");
            }

            const double pi2 = Math.PI / 2.0D;
            double oldWidth = source.Width;
            double oldHeight = source.Height;

            // Convert degrees to radians
            double theta = angle * Math.PI / 180.0D;
            double lockedTheta = theta;

            // Ensure theta is now [0, 2pi)
            while (lockedTheta < 0.0D)
            {
                lockedTheta += 2.0D * Math.PI;
            }

            /*
             * The trig involved in calculating the new width and height
             * is fairly simple; the hard part was remembering that when 
             * PI/2 <= theta <= PI and 3PI/2 <= theta < 2PI the width and 
             * height are switched.
             * 
             * When you rotate a rectangle, r, the bounding box surrounding r
             * contains for right-triangles of empty space.  Each of the 
             * triangles hypotenuse's are a known length, either the width or
             * the height of r.  Because we know the length of the hypotenuse
             * and we have a known angle of rotation, we can use the trig
             * function identities to find the length of the other two sides.
             * 
             * sine = opposite/hypotenuse
             * cosine = adjacent/hypotenuse
             * 
             * solving for the unknown we get
             * 
             * opposite = sine * hypotenuse
             * adjacent = cosine * hypotenuse
             * 
             * Another interesting point about these triangles is that there
             * are only two different triangles. The proof for which is easy
             * to see, but its been too long since I've written a proof that
             * I can't explain it well enough to want to publish it.  
             * 
             * Just trust me when I say the triangles formed by the lengths 
             * width are always the same (for a given theta) and the same 
             * goes for the height of r.
             * 
             * Rather than associate the opposite/adjacent sides with the
             * width and height of the original bitmap, I'll associate them
             * based on their position.
             * 
             * adjacent/oppositeTop will refer to the triangles making up the 
             * upper right and lower left corners
             * 
             * adjacent/oppositeBottom will refer to the triangles making up 
             * the upper left and lower right corners
             * 
             * The names are based on the right side corners, because thats 
             * where I did my work on paper (the right side).
             * 
             * Now if you draw this out, you will see that the width of the 
             * bounding box is calculated by adding together adjacentTop and 
             * oppositeBottom while the height is calculate by adding 
             * together adjacentBottom and oppositeTop.
             */
            double adjacentTop, oppositeTop;
            double adjacentBottom, oppositeBottom;

            // We need to calculate the sides of the triangles based
            // on how much rotation is being done to the bitmap.
            // Refer to the first paragraph in the explaination above for 
            // reasons why.
            if ((lockedTheta >= 0.0D && lockedTheta < pi2) || (lockedTheta >= Math.PI && lockedTheta < (Math.PI + pi2)))
            {
                adjacentTop = Math.Abs(Math.Cos(lockedTheta)) * oldWidth;
                oppositeTop = Math.Abs(Math.Sin(lockedTheta)) * oldWidth;

                adjacentBottom = Math.Abs(Math.Cos(lockedTheta)) * oldHeight;
                oppositeBottom = Math.Abs(Math.Sin(lockedTheta)) * oldHeight;
            }
            else
            {
                adjacentTop = Math.Abs(Math.Sin(lockedTheta)) * oldHeight;
                oppositeTop = Math.Abs(Math.Cos(lockedTheta)) * oldHeight;

                adjacentBottom = Math.Abs(Math.Sin(lockedTheta)) * oldWidth;
                oppositeBottom = Math.Abs(Math.Cos(lockedTheta)) * oldWidth;
            }

            double newWidth = adjacentTop + oppositeBottom;
            double newHeight = adjacentBottom + oppositeTop;

            int nWidth = (int)newWidth;
            int nHeight = (int)newHeight;

            var rotatedBmp = new Bitmap(nWidth, nHeight);

            using (Graphics g = Graphics.FromImage(rotatedBmp))
            {
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                // This array will be used to pass in the three points that 
                // make up the rotated source
                PointF[] points;

                /*
                 * The values of opposite/adjacentTop/Bottom are referring to 
                 * fixed locations instead of in relation to the
                 * rotating source so I need to change which values are used
                 * based on the how much the source is rotating.
                 * 
                 * For each point, one of the coordinates will always be 0, 
                 * nWidth, or nHeight.  This because the Bitmap we are drawing on
                 * is the bounding box for the rotated bitmap.  If both of the 
                 * coordinates for any of the given points wasn't in the set above
                 * then the bitmap we are drawing on WOULDN'T be the bounding box
                 * as required.
                 */
                if (lockedTheta >= 0.0D && lockedTheta < pi2)
                {
                    points = new[]
                        {
                            new PointF((float)oppositeBottom, 0.0F), new PointF((float)newWidth, (float)oppositeTop),
                            new PointF(0.0F, (float)adjacentBottom)
                        };
                }
                else if (lockedTheta >= pi2 && lockedTheta < Math.PI)
                {
                    points = new[]
                        {
                            new PointF((float)newWidth, (float)oppositeTop), new PointF((float)adjacentTop, (float)newHeight),
                            new PointF((float)oppositeBottom, 0.0F)
                        };
                }
                else if (lockedTheta >= Math.PI && lockedTheta < (Math.PI + pi2))
                {
                    points = new[]
                        {
                            new PointF((float)adjacentTop, (float)newHeight), new PointF(0.0F, (float)adjacentBottom),
                            new PointF((float)newWidth, (float)oppositeTop)
                        };
                }
                else
                {
                    points = new[]
                        {
                            new PointF(0.0F, (float)adjacentBottom), new PointF((float)oppositeBottom, 0.0F),
                            new PointF((float)adjacentTop, (float)newHeight)
                        };
                }

                g.DrawImage(source, points);
            }

            return rotatedBmp;
        }

        /// <summary>
        ///     The to grey scale.
        /// </summary>
        /// <param name="source">
        ///     The source.
        /// </param>
        /// <returns>
        /// </returns>
        public virtual Bitmap ToGreyScale(Bitmap source)
        {
            var tempBitmap = new Bitmap(source, source.Width, source.Height);

            using (Graphics newGraphics = Graphics.FromImage(tempBitmap))
            {
                float[][] floatColorMatrix =
                    {
                        new[] { .3f, .3f, .3f, 0, 0 }, new[] { .59f, .59f, .59f, 0, 0 },
                        new[] { .11f, .11f, .11f, 0, 0 }, new float[] { 0, 0, 0, 1, 0 }, new float[] { 0, 0, 0, 0, 1 }
                    };

                var newColorMatrix = new ColorMatrix(floatColorMatrix);
                using (var attributes = new ImageAttributes())
                {
                    attributes.SetColorMatrix(newColorMatrix);
                    newGraphics.DrawImage(
                        tempBitmap,
                        new Rectangle(0, 0, tempBitmap.Width, tempBitmap.Height),
                        0,
                        0,
                        tempBitmap.Width,
                        tempBitmap.Height,
                        GraphicsUnit.Pixel,
                        attributes);
                }
            }

            return tempBitmap;
        }

        /// <summary>
        ///     The watermark.
        /// </summary>
        /// <param name="source">
        ///     The source.
        /// </param>
        /// <param name="watermarkText">
        ///     The watermark text.
        /// </param>
        /// <param name="fontSize">
        ///     The font size.
        /// </param>
        /// <param name="alpha">
        ///     The alpha.
        /// </param>
        /// <param name="color">
        ///     The color.
        /// </param>
        /// <returns>
        /// </returns>
        public virtual Bitmap Watermark(Bitmap source, string watermarkText, float fontSize, int alpha, Color color)
        {
            var tempBitmap = new Bitmap(source, source.Width, source.Height);

            using (Graphics newGraphics = Graphics.FromImage(tempBitmap))
            {
                newGraphics.SmoothingMode = SmoothingMode.HighQuality;

                using (var myFont = new Font("Arial", fontSize))
                using (var brush = new SolidBrush(Color.FromArgb(alpha, color)))
                {
                    /* This gets the size of the graphic so we can determine 
                         * the loop counts and placement of the watermarked text. */
                    SizeF textSize = newGraphics.MeasureString(watermarkText, myFont);

                    // Write the text across the image. 
                    for (int y = 0; y < tempBitmap.Height; y++)
                    {
                        for (int x = 0; x < tempBitmap.Width; x++)
                        {
                            var pointF = new PointF(x, y);
                            newGraphics.DrawString(watermarkText, myFont, brush, pointF);
                            x += Convert.ToInt32(textSize.Width);
                        }

                        y += Convert.ToInt32(textSize.Height);
                    }
                }
            }

            return tempBitmap;
        }

        /// <summary>
        ///     The zoom.
        /// </summary>
        /// <param name="source">
        ///     The source.
        /// </param>
        /// <param name="zoomFactor">
        ///     The zoom factor.
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref="OverflowException"><paramref name="value" /> is greater than <see cref="F:System.Int32.MaxValue" /> or less than <see cref="F:System.Int32.MinValue" />. </exception>
        public virtual Bitmap Zoom(Bitmap source, float zoomFactor)
        {
            int width = Convert.ToInt32(source.Width * zoomFactor);
            int height = Convert.ToInt32(source.Height * zoomFactor);

            return this.Resize(source, width, height, true, false, Color.White);
        }
    }
}