// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ZoomFilter.cs" company="">
//   
// </copyright>
// <summary>
//   The zoom filter.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DynamicImageHandler.Filters
{
	using System.Drawing;
	using System.Globalization;
	using System.Web;

	/// <summary>
	/// The zoom filter.
	/// </summary>
	internal class ZoomFilter : IImageFilter
	{
		#region Public Methods

		/// <summary>
		/// The process.
		/// </summary>
		/// <param name="parameters">
		/// The parameters.
		/// </param>
		/// <param name="context">
		/// The context.
		/// </param>
		/// <param name="bitmap">
		/// The bitmap.
		/// </param>
		/// <returns>
		/// The process.
		/// </returns>
		public bool Process(IImageParameters parameters, HttpContext context, ref Bitmap bitmap)
		{
			if (parameters.Parameters.ContainsKey("zoom"))
			{
				string zoomParameter = parameters.Parameters["zoom"];

				float zoomFactor;
				if (float.TryParse(zoomParameter, NumberStyles.Float, CultureInfo.InvariantCulture, out zoomFactor))
				{
					IImageTool imageTool = Factory.GetImageTool();
					bitmap = imageTool.Zoom(bitmap, zoomFactor);
					return true;
				}
			}

			return false;
		}

		#endregion
	}
}