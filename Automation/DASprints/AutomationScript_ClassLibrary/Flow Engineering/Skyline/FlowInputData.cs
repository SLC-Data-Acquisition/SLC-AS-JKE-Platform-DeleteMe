namespace Skyline.DataMiner.DeveloperCommunityLibrary.FlowEngineering
{
	using System;
	using Booking;
	using Path;

	public class FlowInputData
	{
		public FlowInputData()
		{
		}

		public FlowInputData(string bookingName, string bookingAppName, FlowContributingConfig contributingConfig, FlowPath path, FlowRecurrence flowRecurrence)
		{
			if (String.IsNullOrWhiteSpace(bookingName))
			{
				throw new ArgumentException("Value cannot be null or whitespace.", nameof(bookingName));
			}

			if (String.IsNullOrWhiteSpace(bookingAppName))
			{
				throw new ArgumentException("Value cannot be null or whitespace.", nameof(bookingAppName));
			}

			BookingName = bookingName;
			BookingAppName = bookingAppName;
			ContributingConfig = contributingConfig ?? throw new ArgumentNullException(nameof(contributingConfig));
			PathConfig = path ?? throw new ArgumentNullException(nameof(path));
			FlowRecurrence = flowRecurrence ?? throw new ArgumentNullException(nameof(flowRecurrence));
		}

		/// <summary>
		/// Gets or sets the booking name.
		/// </summary>
		public string BookingName { get; set; }

		/// <summary>
		/// Gets or sets the ContributingConfig settings.
		/// </summary>
		public FlowContributingConfig ContributingConfig { get; set; }

		/// <summary>
		/// Gets or sets the path configuration. 
		/// </summary>
		public FlowPath PathConfig { get; set; }

		/// <summary>
		/// Gets or sets the booking recurrence.
		/// </summary>
		public FlowRecurrence FlowRecurrence { get; set; }

		/// <summary>
		/// Gets or sets the booking app name.
		/// </summary>
		public string BookingAppName { get; set; }
	}
}
