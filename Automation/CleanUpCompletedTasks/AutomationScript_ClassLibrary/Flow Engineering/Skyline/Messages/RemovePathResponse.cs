namespace Skyline.DataMiner.DeveloperCommunityLibrary.FlowEngineering.Messages
{
	using Skyline.DataMiner.Library.Common.InterAppCalls.CallSingle;

	public class RemovePathResponse : Message
	{
		public bool Success { get; set; }
	}
}