// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppScheduler.cs" company="">
//   
// </copyright>
// <summary>
//   The app scheduler.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DynamicImageHandler.Utils
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using System.Threading;

	/// <summary>
	/// The app scheduler.
	/// </summary>
	public class AppScheduler
	{
		#region Constants and Fields

		/// <summary>
		/// The s_ lock.
		/// </summary>
		private static readonly object s_Lock = new object();

		/// <summary>
		/// The s_ pending tasks.
		/// </summary>
		private static readonly SortedDictionary<DateTime, IScheduleTask> s_PendingTasks =
			new SortedDictionary<DateTime, IScheduleTask>();

		/// <summary>
		/// The s_ timer.
		/// </summary>
		private static readonly Timer s_Timer = new Timer(ProcessTasks, null, -1, -1);

		#endregion

		#region Public Methods

		/// <summary>
		/// 	Determines whether [is scheduled task] [the specified task].
		/// </summary>
		/// <param name="task">
		/// The task.
		/// </param>
		/// <returns>
		/// <c>true</c> if [is scheduled task] [the specified task]; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsScheduledTask(IScheduleTask task)
		{
			return s_PendingTasks.ContainsValue(task);
		}

		/// <summary>
		/// 	Schedules the task.
		/// </summary>
		/// <param name="task">
		/// The task.
		/// </param>
		public static void ScheduleTask(IScheduleTask task)
		{
			lock (s_Lock)
			{
				DateTime occursAt = task.GetNextReoccurrence();
				while (s_PendingTasks.ContainsKey(occursAt))
				{
					occursAt = occursAt.AddMilliseconds(1);
				}

				s_PendingTasks.Add(occursAt, task);
				ProcessTasks(null);
			}
		}

		/// <summary>
		/// 	Unschedules the task.
		/// </summary>
		/// <param name="task">
		/// The task.
		/// </param>
		public static void UnscheduleTask(IScheduleTask task)
		{
			lock (s_Lock)
			{
				KeyValuePair<DateTime, IScheduleTask> entry = s_PendingTasks.Where(p => p.Value == task).FirstOrDefault();
				if (!entry.IsNull())
				{
					s_PendingTasks.Remove(entry.Key);
					ProcessTasks(null);
				}
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// 	Processes the tasks.
		/// </summary>
		/// <param name="state">
		/// The state.
		/// </param>
		private static void ProcessTasks(object state)
		{
			lock (s_Lock)
			{
				DateTime now = DateTime.Now;
				s_PendingTasks.ToArray().Where(p => p.Key < now).ForEach(
					t =>
						{
							s_PendingTasks.Remove(t.Key);
							ThreadPool.QueueUserWorkItem(TaskThreadProc, t.Value);
						});

				if (s_PendingTasks.Any())
				{
					// Set the time to fire for the next task
					KeyValuePair<DateTime, IScheduleTask> first = s_PendingTasks.First();
					DateTime key = first.Key;
					TimeSpan next = key - DateTime.Now;

					// Set the timer to fire just after the task expires
					s_Timer.Change(((int)next.TotalMilliseconds) + 10, -1);
				}
				else
				{
					// Clear the timer we have no more task to run
					s_Timer.Change(-1, -1);
				}
			}
		}

		/// <summary>
		/// 	Tasks the thread proc.
		/// </summary>
		/// <param name="state">
		/// The state.
		/// </param>
		private static void TaskThreadProc(object state)
		{
			var task = (IScheduleTask)state;

			try
			{
				task.RunTask();
			}
			catch (Exception e)
			{
				Trace.TraceError(e.ToString());
			}
			finally
			{
				if (task.ReoccurringTask && !IsScheduledTask(task))
				{
					ScheduleTask(task);
				}
			}
		}

		#endregion
	}
}