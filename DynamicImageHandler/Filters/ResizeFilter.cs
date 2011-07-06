// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResizeFilter.cs" company="">
//   
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