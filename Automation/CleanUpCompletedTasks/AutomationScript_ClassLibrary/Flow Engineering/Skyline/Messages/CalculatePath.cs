namespace Skyline.DataMiner.DeveloperCommunityLibrary.FlowEngineering.Messages
{
	using Skyline.DataMiner.DeveloperCommunityLibrary.FlowEngineering.InputData;
	using Skyline.DataMiner.DeveloperCommunityLibrary.FlowEngineering.Settings;
	using Skyline.DataMiner.Library.Common.InterAppCalls.CallSingle;

	public class CalculatePath : Message
	{
		public ISettings Settings { get; set; }

		public IInputData InputData { get; set; }
	}
}