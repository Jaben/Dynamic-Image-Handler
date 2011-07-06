// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IImageTool.cs" company="">
// Copyright (c) 2009-2010 Esben Carlsen
// Forked by Jaben Cargman
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

namespace DynamicImageHandler
{
	using System.Drawing;
	using System.Drawing.Imaging;

	/// <summary>
	/// 	Interface defining basic image operations
	/// </summary>
	public interface IImageTool
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
		byte[] Encode(Bitmap source, ImageFormat imageFormat);

		/// <summary>
		/// The resize.
		/// </summary>
		/// <param name="source">
		/// The source.
		/// </param>
		/// <param name="width">
		/// The width.
		/// </param>
		/// <param name="height">
		/// The height.
		/// </param>
		/// <param name="preservePerspective">
		/// The preserve perspective.
		/// </param>
		/// <param name="keepSquare">
		/// Keep the image 1:1.
		/// </param>
		/// <returns>
		/// </returns>
		Bitmap Resize(Bitmap source, int? width, int? height, bool preservePerspective, bool keepSquare, Color bgColor);

		/// <summary>
		/// The rotate.
		/// </summary>
		/// <param name="source">
		/// The source.
		/// </param>
		/// <param name="angle">
		/// The angle.
		/// </param>
		/// <returns>
		/// </returns>
		Bitmap Rotate(Bitmap source, float angle);

		/// <summary>
		/// The to grey scale.
		/// </summary>
		/// <param name="source">
		/// The source.
		/// </param>
		/// <returns>
		/// </returns>
		Bitmap ToGreyScale(Bitmap source);

		/// <summary>
		/// The watermark.
		/// </summary>
		/// <param name="source">
		/// The source.
		/// </param>
		/// <param name="watermarkText">
		/// The watermark text.
		/// </param>
		/// <param name="fontSize">
		/// The font size.
		/// </param>
		/// <param name="alpha">
		/// The alpha.
		/// </param>
		/// <param name="color">
		/// The color.
		/// </param>
		/// <returns>
		/// </returns>
		Bitmap Watermark(Bitmap source, string watermarkText, float fontSize, int alpha, Color color);

		/// <summary>
		/// The zoom.
		/// </summary>
		/// <param name="source">
		/// The source.
		/// </param>
		/// <param name="zoomFactor">
		/// The zoom factor.
		/// </param>
		/// <returns>
		/// </returns>
		Bitmap Zoom(Bitmap source, float zoomFactor);

		#endregion
	}
}