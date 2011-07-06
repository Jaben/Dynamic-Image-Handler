// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEnumerableExtensions.cs" company="">
//   
// </copyright>
// <summary>
//   The i enumerable extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DynamicImageHandler.Utils
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	/// The i enumerable extensions.
	/// </summary>
	public static class IEnumerableExtensions
	{
		#region Public Methods

		/// <summary>
		/// 	Iterates through all sequence and performs specified action on each
		/// 	element
		/// </summary>
		/// <typeparam name="T">
		/// Sequence element type
		/// </typeparam>
		/// <param name="enumerable">
		/// Target enumeration
		/// </param>
		/// <param name="action">
		/// Action
		/// </param>
		/// <exception cref="System.ArgumentNullException">
		/// One of the input agruments is null
		/// </exception>
		public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
		{
			foreach (T elem in enumerable)
			{
				action(elem);
			}
		}

		#endregion
	}
}