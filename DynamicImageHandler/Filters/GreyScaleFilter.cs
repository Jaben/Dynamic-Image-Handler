// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GreyScaleFilter.cs" company="">
//   
// </copyright>
// <summary>
//   The grey scale filter.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DynamicImageHandler.Filters
{
	using System.Drawing;
	using System.Web;

	/// <summary>
	/// The grey scale filter.
	/// </summary>
	internal class GreyScaleFilter : IImageFilter
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
			if (parameters.Parameters.ContainsKey("greyscale"))
			{
				string greyScaleParameter = parameters.Parameters["greyscale"];
				if (string.IsNullOrEmpty(greyScaleParameter))
				{
					return false;
				}

				IImageTool imageTool = Factory.GetImageTool();
				bitmap = imageTool.ToGreyScale(bitmap);
				return true;
			}

			return false;
		}

		#endregion
	}
}