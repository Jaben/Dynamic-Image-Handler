// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WatermarkFilter.cs" company="">
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
//   The watermark filter.
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
	/// 	The watermark filter.
	/// </summary>
	internal class WatermarkFilter : IImageFilter
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
			if (parameters.Parameters.ContainsKey("watermark"))
			{
				string watermarkParameter = parameters["watermark"];
				if (string.IsNullOrEmpty(watermarkParameter))
				{
					return false;
				}

				float fontSize = 15;
				if (!string.IsNullOrEmpty(parameters["watermark_fontsize"]))
				{
					fontSize = (float)Convert.ToDouble(parameters["watermark_fontsize"]);
				}

				int alpha = 20;
				if (!string.IsNullOrEmpty(parameters["watermark_opacity"]))
				{
					alpha = Convert.ToInt32(parameters["watermark_opacity"]);
				}

				Color color = Color.White;
				if (!string.IsNullOrEmpty(parameters["watermark_color"]))
				{
					color = Color.FromName(parameters["watermark_color"]);
				}

				IImageTool imageTool = Factory.GetImageTool();
				bitmap = imageTool.Watermark(bitmap, watermarkParameter, fontSize, alpha, color);
				return true;
			}

			return false;
		}

		#endregion
	}
}