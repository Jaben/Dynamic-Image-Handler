// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResizeFilter.cs" company="">
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
//   The resize filter.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DynamicImageHandler.Filters
{
	#region Using

	using System;
	using System.Drawing;
	using System.Web;

	#endregion

	/// <summary>
	/// 	The resize filter.
	/// </summary>
	internal class ResizeFilter : IImageFilter
	{
		#region Public Methods

		/// <summary>
		/// 	The process.
		/// </summary>
		/// <param name="parameters">
		/// 	The parameters.
		/// </param>
		/// <param name="context">
		/// 	The context.
		/// </param>
		/// <param name="bitmap">
		/// 	The bitmap.
		/// </param>
		/// <returns>
		/// 	The process.
		/// </returns>
		public bool Process(IImageParameters parameters, HttpContext context, ref Bitmap bitmap)
		{
			if (parameters.Parameters.ContainsKey("width") || parameters.Parameters.ContainsKey("height"))
			{
				string widthParameter = parameters.Parameters.ContainsKey("width") ? parameters.Parameters["width"] : string.Empty;
				string heightParameter = parameters.Parameters.ContainsKey("height")
				                         	? parameters.Parameters["height"]
				                         	: string.Empty;

				int imageResizeWidth = 0;
				int imageResizeHeight = 0;

				// Preserve aspect only if either width or height is specified, not both.
				if (!string.IsNullOrEmpty(widthParameter))
				{
					int width;
					if (int.TryParse(widthParameter, out width))
					{
						imageResizeWidth = width;
					}
				}

				if (!string.IsNullOrEmpty(heightParameter))
				{
					int height;
					if (int.TryParse(heightParameter, out height))
					{
						imageResizeHeight = height;
					}
				}

				if (imageResizeWidth == 0 && imageResizeHeight == 0)
				{
					imageResizeWidth = bitmap.Width;
					imageResizeHeight = bitmap.Height;
				}

				bool constrainImageSize = true;
				if (!string.IsNullOrEmpty(parameters["resize_constrain"]))
				{
					bool.TryParse(parameters["resize_constrain"], out constrainImageSize);
				}

				bool keepSquare = false;
				if (!string.IsNullOrEmpty(parameters["resize_square"]))
				{
					bool.TryParse(parameters["resize_square"], out keepSquare);
				}

				Color bgColor = Color.White;
				if (!string.IsNullOrEmpty(parameters["resize_bgcolor"]))
				{
					bgColor = Color.FromName(parameters["resize_bgcolor"]);
				}

				IImageTool imageTool = Factory.GetImageTool();
				bitmap = imageTool.Resize(bitmap, imageResizeWidth, imageResizeHeight, constrainImageSize, keepSquare, bgColor);
				return true;
			}

			return false;
		}

		#endregion
	}
}