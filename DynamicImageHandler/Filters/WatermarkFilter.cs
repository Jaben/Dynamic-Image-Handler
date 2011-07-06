// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WatermarkFilter.cs" company="">
//   
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