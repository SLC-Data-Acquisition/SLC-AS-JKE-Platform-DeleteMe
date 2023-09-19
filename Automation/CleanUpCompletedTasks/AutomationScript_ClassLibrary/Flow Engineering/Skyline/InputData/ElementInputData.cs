namespace Skyline.DataMiner.DeveloperCommunityLibrary.FlowEngineering.InputData
{
	public class ElementInputData : IInputData
	{
		/// <inheritdoc />
		public string PathName { get; set; }

		/// <summary>
		/// Element Key (Agent Id/Element Id).
		/// </summary>
		public string Source { get; set; }

		/// <summary>
		/// ID of the DCF Interface.
		/// </summary>
		public int? SourceDcfInterface { get; set; }

		/// <summary>
		/// Element Key (Agent Id/Element Id).
		/// </summary>
		public string Destination { get; set; }

		/// <summary>
		/// ID of the DCF Interface.
		/// </summary>
		public int? DestinationDcfInterface { get; set; }
	}
}