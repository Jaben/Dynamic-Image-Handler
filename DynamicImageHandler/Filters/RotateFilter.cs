// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RotateFilter.cs" company="">
//   
// </copyright>
// <summary>
//   The rotate filter.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DynamicImageHandler.Filters
{
	#region Using

	using System.Drawing;
	using System.Globalization;
	using System.Web;

	#endregion

	/// <summary>
	/// 	The rotate filter.
	/// </summary>
	internal class RotateFilter : IImageFilter
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
			if (parameters.Parameters.ContainsKey("rotate"))
			{
				string rotateParameter = parameters.Parameters["rotate"];

				float angle;
				if (float.TryParse(rotateParameter, NumberStyles.Float, CultureInfo.InvariantCulture, out angle))
				{
					IImageTool imageTool = Factory.GetImageTool();
					bitmap = imageTool.Rotate(bitmap, angle);
					return true;
				}
			}

			return false;
		}

		#endregion
	}
}