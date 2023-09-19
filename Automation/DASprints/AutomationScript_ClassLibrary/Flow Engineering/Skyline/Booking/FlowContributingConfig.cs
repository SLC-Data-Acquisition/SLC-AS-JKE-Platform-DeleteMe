namespace Skyline.DataMiner.DeveloperCommunityLibrary.FlowEngineering.Booking
{
	using System;

	/// <summary>
	/// Copy from Skyline.DataMiner.Library.Solutions.SRM.Contributing.ContributingConfig
	/// </summary>
	public class FlowContributingConfig
	{
		public FlowContributingConfig(Guid parentSystemFunction)
		{
			if (parentSystemFunction == Guid.Empty)
			{
				throw new ArgumentException("Parent system function is invalid");
			}

			ParentSystemFunction = parentSystemFunction;
		}

		/// <summary>
		/// Gets or sets the concurrency that should be set on the created contributing resource.
		/// </summary>
		public int Concurrency { get; set; } = 1;

		/// <summary>
		/// Gets or sets a value indicating the parent system function to be used.
		/// </summary>
		public Guid ParentSystemFunction { get; set; }

		/// <summary>
		/// Gets or sets a value indicating the resource pool name to which the contributing resource should be posed.
		/// </summary>
		public string ResourcePool { get; set; } = "FLE_Contributing";

		/// <summary>
		/// Gets or sets the name of the custom script to be triggered after converting the Reservation to Contributing was successful.
		/// Example: "Script:SRM_DummyScript||DummyParameterName=DummyValue" similar to <see cref="F:Skyline.DataMiner.Library.Solutions.SRM.KnownProperties.Node.CreatedBookingAction" /> property.
		/// </summary>
		public string Script { get; set; }

		/// <summary>
		/// Gets or sets the name of the visio drawing that will be assigned to the generated service.
		/// </summary>
		public string VisioFileName { get; set; }
	}
}
