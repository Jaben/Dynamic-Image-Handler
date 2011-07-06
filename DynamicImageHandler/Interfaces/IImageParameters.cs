// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IImageParameters.cs" company="">
//   
// </copyright>
// <summary>
//   The i image parameters.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DynamicImageHandler
{
	using System.Collections.Generic;
	using System.Web;

	/// <summary>
	/// The i image parameters.
	/// </summary>
	public interface IImageParameters
	{
		#region Public Properties

		/// <summary>
		/// Gets ImageSrc.
		/// </summary>
		string ImageSrc { get; }

		/// <summary>
		/// Gets Key.
		/// </summary>
		string Key { get; }

		/// <summary>
		/// Gets Parameters.
		/// </summary>
		IDictionary<string, string> Parameters { get; }

		#endregion

		#region Public Indexers

		/// <summary>
		/// The this.
		/// </summary>
		/// <param name="parameter">
		/// The parameter.
		/// </param>
		string this[string parameter] { get; }

		#endregion

		#region Public Methods

		/// <summary>
		/// The add collection.
		/// </summary>
		/// <param name="context">
		/// The context.
		/// </param>
		void AddCollection(HttpContext context);

		#endregion
	}
}