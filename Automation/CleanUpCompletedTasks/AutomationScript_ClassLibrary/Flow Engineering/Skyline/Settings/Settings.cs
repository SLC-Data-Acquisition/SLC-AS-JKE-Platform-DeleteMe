namespace Skyline.DataMiner.DeveloperCommunityLibrary.FlowEngineering.Settings
{
	using Skyline.DataMiner.DeveloperCommunityLibrary.FlowEngineering.Enums;

	public class Settings : ISettings
	{
		/// <inheritdoc />
		public Algorithms Algorithm { get; set; }

		/////// <inheritdoc />
		////public uint NumberOfPaths { get; set; }

		/// <inheritdoc />
		public bool TakeMetrics { get; set; }

		/// <inheritdoc />
		public bool DocumentPath { get; set; }
	}
}