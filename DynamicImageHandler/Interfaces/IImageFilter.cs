// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IImageFilter.cs" company="">
//   
// </copyright>
// <summary>
//   Interface to define image filter. An image filter is an action that can be applied to an image
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DynamicImageHandler
{
	using System.Drawing;
	using System.Web;

	/// <summary>
	/// 	Interface to define image filter. An image filter is an action that can be applied to an image
	/// </summary>
	public interface IImageFilter
	{
		#region Public Methods

		/// <summary>
		/// 	Returns true if modified
		/// </summary>
		/// <param name="parameters">
		/// </param>
		/// <param name="context">
		/// </param>
		/// <param name="bitmap">
		/// </param>
		/// <returns>
		/// The process.
		/// </returns>
		bool Process(IImageParameters parameters, HttpContext context, ref Bitmap bitmap);

		#endregion
	}
}