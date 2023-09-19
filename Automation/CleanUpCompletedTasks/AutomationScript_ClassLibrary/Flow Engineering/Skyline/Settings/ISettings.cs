namespace Skyline.DataMiner.DeveloperCommunityLibrary.FlowEngineering.Settings
{
	using Skyline.DataMiner.DeveloperCommunityLibrary.FlowEngineering.Enums;

	public interface ISettings
	{
		/// <summary>
		/// Type of algorithm to run the path calculation.
		/// </summary>
		Algorithms Algorithm { get; }

		/////// <summary>
		/////// Maximum number of paths that should be calculated.
		/////// </summary>
		////uint NumberOfPaths { get; }

		/// <summary>
		/// Take metrics that can be found on the Data page of the element.
		/// </summary>
		bool TakeMetrics { get; }

		/// <summary>
		/// Document the path on the DCF interfaces with properties.
		/// </summary>
		bool DocumentPath { get; }
	}
}