// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IScheduleTask.cs" company="">
//   
// </copyright>
// <summary>
//   From pegasus library: http://pegasus.codeplex.com
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DynamicImageHandler.Utils
{
	using System;

	/// <summary>
	/// 	From pegasus library: http://pegasus.codeplex.com/
	/// </summary>
	public interface IScheduleTask
	{
		#region Public Properties

		/// <summary>
		/// 	Gets a value indicating whether this is a reoccurring task or not.
		/// </summary>
		/// <value>
		/// 	If <c>true</c> the task will be reschedule after is runs.  If <c>false</c> then the
		/// 	task will only be executed once.
		/// </value>
		bool ReoccurringTask { get; }

		#endregion

		#region Public Methods

		/// <summary>
		/// 	Gets the next date and time of the next occurrence of the task.
		/// </summary>
		/// <returns>
		/// The next occurrence of the task.
		/// </returns>
		DateTime GetNextReoccurrence();

		/// <summary>
		/// 	Called to execute the task.
		/// </summary>
		void RunTask();

		#endregion
	}
}