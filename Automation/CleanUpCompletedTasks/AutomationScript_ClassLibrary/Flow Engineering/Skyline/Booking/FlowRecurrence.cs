namespace Skyline.DataMiner.DeveloperCommunityLibrary.FlowEngineering.Booking
{
	using System;

	/// <summary>
	/// Copy from DataMiner.DeveloperCommunityLibrary.FlowEngineering.Booking.Recurrence
	/// </summary>
	public class FlowRecurrence
	{
		/// <summary>
		/// Gets or sets a value indicating whether the booking takes all day.
		/// If true, then <see cref="StartDate" /> and <see cref="EndDate" /> will not be taken into account.
		/// </summary>
		public bool AllDayEvent { get; set; }

		/// <summary>Gets or sets the duration of the booking.</summary>
		public TimeSpan Duration { get; set; }

		/// <summary>
		/// Gets or sets the EndDate value. This Date time must be set with DateTime.Kind of type Local. The system will handle all conversions to UTC.
		/// </summary>
		public DateTime EndDate { get; set; }

		/// <summary>
		/// Gets or sets a value indicating if a permanent service should be created or not.
		/// If specified <see cref="EndDate" /> will not be taken into account.
		/// </summary>
		public bool PermanentService { get; set; }

		/// <summary>Gets or sets the post roll duration.</summary>
		public TimeSpan PostRoll { get; set; }

		/// <summary>Gets or sets the pre roll duration.</summary>
		public TimeSpan PreRoll { get; set; }

		/// <summary>
		/// Gets or sets The StartDate value. This Date time  must be set with DateTime.Kind of type Local. The system will handle all conversions to UTC.
		/// </summary>
		public DateTime StartDate { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this is a single event or not.
		/// </summary>
		public bool SingleEvent { get; set; }
	}
}
