namespace Skyline.DataMiner.DeveloperCommunityLibrary.FlowEngineering.Path
{
	using System;
	using System.Collections.Generic;
	using Net.SRM.Capacities;
	using Skyline.DataMiner.DeveloperCommunityLibrary.FlowEngineering.Enums;

	public class FlowNode
	{
		public FlowNode(
			int agentId,
			int elementId,
			int interfaceId,
			InterfaceTypes interfaceType,
			string resourceName,
			Guid resourceId,
			List<MultiResourceCapacityUsage> capacities)
		{
			AgentId = agentId;
			ElementId = elementId;
			InterfaceId = interfaceId;
			InterfaceType = interfaceType;
			ResourceName = resourceName;
			Capacities = capacities;
			ResourceId = resourceId;
		}

		public int AgentId { get; private set; }

		public int ElementId { get; private set; }

		public int InterfaceId { get; private set; }

		public InterfaceTypes InterfaceType { get; private set; }

		public string ResourceName { get; private set; }

		public Guid ResourceId { get; private set; }

		public List<MultiResourceCapacityUsage> Capacities { get; set; }

		public override string ToString()
		{
			return String.IsNullOrWhiteSpace(ResourceName)
					   ? String.Format("[{0}/{1}] {2}/{3}", AgentId, ElementId, InterfaceId, InterfaceType.ToString())
					   : String.Format("[{0}/{1}] {2}/{3} {4}", AgentId, ElementId, InterfaceId, InterfaceType.ToString(), ResourceName);
		}
	}
}