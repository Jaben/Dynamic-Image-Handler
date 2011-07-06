// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectExtensions.cs" company="">
//   
// </copyright>
// <summary>
//   The object extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DynamicImageHandler.Utils
{
	/// <summary>
	/// The object extensions.
	/// </summary>
	public static class ObjectExtensions
	{
		#region Public Methods

		/// <summary>
		/// The is null.
		/// </summary>
		/// <param name="object">
		/// The object.
		/// </param>
		/// <typeparam name="T">
		/// </typeparam>
		/// <returns>
		/// The is null.
		/// </returns>
		public static bool IsNull<T>(this T @object)
		{
			return Equals(@object, null);
		}

		#endregion
	}
}