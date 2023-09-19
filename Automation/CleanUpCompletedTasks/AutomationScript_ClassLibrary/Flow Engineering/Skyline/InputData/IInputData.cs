namespace Skyline.DataMiner.DeveloperCommunityLibrary.FlowEngineering.InputData
{
	public interface IInputData
	{
		/// <summary>
		/// Path name. In case this is empty, a default will be used.
		/// </summary>
		string PathName { get; set; }

		/// <summary>
		/// Left side of the path.
		/// </summary>
		string Source { get; set; }

		/// <summary>
		/// Right side of the path.
		/// </summary>
		string Destination { get; set; }
	}
}