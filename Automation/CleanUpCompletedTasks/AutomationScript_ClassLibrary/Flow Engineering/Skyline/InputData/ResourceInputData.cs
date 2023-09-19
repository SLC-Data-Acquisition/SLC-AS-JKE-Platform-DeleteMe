namespace Skyline.DataMiner.DeveloperCommunityLibrary.FlowEngineering.InputData
{
	using System;
	using System.Collections.Generic;
	using Skyline.DataMiner.Net.SRM.Capabilities;
	using Skyline.DataMiner.Net.SRM.Capacities;
	
	public class ResourceInputData : IInputData
	{
		public ResourceInputData()
		{
			Capabilities = new List<ResourceCapabilityUsage>();
			Capacities = new List<MultiResourceCapacityUsage>();
		}
		
		/// <inheritdoc />
		public string PathName { get; set; }

		/// <summary>
		/// Resource GUID.
		/// </summary>
		public string Source { get; set; }
		
		/// <summary>
		/// Resource GUID.
		/// </summary>
		public string Destination { get; set; }
		
		public DateTime? StartTime { get; set; }
		
		public DateTime? EndTime { get; set; }
		
		public ICollection<MultiResourceCapacityUsage> Capacities { get; set; }
		
		public ICollection<ResourceCapabilityUsage> Capabilities { get; set; }
	}
}