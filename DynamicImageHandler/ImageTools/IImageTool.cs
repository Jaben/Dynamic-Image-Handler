﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IImageTool.cs" company="">
// Copyright (c) 2009-2010 Esben Carlsen
// Forked by Jaben Cargman and CaptiveAire Systems
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
// </copyright>
// <summary>
//   Interface defining basic image operations
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Drawing;
using System.Drawing.Imaging;

namespace DynamicImageHandler.ImageTools
{
    /// <summary>
    ///     Interface defining basic image operations
    /// </summary>
    public interface IImageTool
    {
        byte[] Encode(Bitmap source, ImageFormat imageFormat);

        Bitmap Resize(Bitmap source, int? width, int? height, bool preservePerspective, bool keepSquare, Color bgColor);

        Bitmap Rotate(Bitmap source, float angle);

        Bitmap ToGreyScale(Bitmap source);

        Bitmap Watermark(Bitmap source, string watermarkText, float fontSize, int alpha, Color color);

        Bitmap Zoom(Bitmap source, float zoomFactor);
    }
}