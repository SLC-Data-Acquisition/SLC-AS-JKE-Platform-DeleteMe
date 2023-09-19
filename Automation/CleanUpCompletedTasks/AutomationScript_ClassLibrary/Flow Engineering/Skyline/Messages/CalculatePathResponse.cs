namespace Skyline.DataMiner.DeveloperCommunityLibrary.FlowEngineering.Messages
{
	using System.Collections.Generic;

	using Skyline.DataMiner.DeveloperCommunityLibrary.FlowEngineering.Path;
	using Skyline.DataMiner.Library.Common.InterAppCalls.CallSingle;
	
	public class CalculatePathResponse : Message
	{
		public CalculatePathResponse()
		{
			Paths = new List<FlowPath>();
		}
		
		public bool Success { get; set; }
		
		public ICollection<FlowPath> Paths { get; set; }
	}
}