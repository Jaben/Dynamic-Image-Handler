// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImageStoreScavanger.cs" company="">
//   
// </copyright>
// <summary>
//   The image store scavanger.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DynamicImageHandler.ImageStores
{
	using System;

	using DynamicImageHandler.Utils;

	/// <summary>
	/// The image store scavanger.
	/// </summary>
	public abstract class ImageStoreScavanger : IScheduleTask
	{
		#region Constants and Fields

		/// <summary>
		/// The m_ scavange interval.
		/// </summary>
		private readonly TimeSpan m_ScavangeInterval;

		/// <summary>
		/// The m_ first run.
		/// </summary>
		private bool m_FirstRun = true;

		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="ImageStoreScavanger"/> class.
		/// </summary>
		/// <param name="scavangeInterval">
		/// The scavange interval.
		/// </param>
		/// <param name="startInitialScavange">
		/// The start initial scavange.
		/// </param>
		protected ImageStoreScavanger(TimeSpan scavangeInterval, bool startInitialScavange)
		{
			this.m_ScavangeInterval = scavangeInterval;
			this.m_FirstRun = startInitialScavange;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets a value indicating whether ReoccurringTask.
		/// </summary>
		public bool ReoccurringTask
		{
			get
			{
				return true;
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// The get next reoccurrence.
		/// </summary>
		/// <returns>
		/// </returns>
		public DateTime GetNextReoccurrence()
		{
			if (this.m_FirstRun)
			{
				this.m_FirstRun = false;
				return DateTime.Now.AddSeconds(10);
			}

			return DateTime.Now.Add(this.m_ScavangeInterval);
		}

		/// <summary>
		/// The run task.
		/// </summary>
		public void RunTask()
		{
			this.CleanOldImages();
		}

		#endregion

		#region Methods

		/// <summary>
		/// The clean old images.
		/// </summary>
		protected abstract void CleanOldImages();

		#endregion
	}
}