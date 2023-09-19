namespace Skyline.DataMiner.DeveloperCommunityLibrary.FlowEngineering.Path
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class FlowPath
	{
		public FlowPath(IReadOnlyCollection<FlowEdge> edges, IReadOnlyCollection<FlowNode> nodes)
		{
			if (edges == null)
			{
				throw new ArgumentNullException("edges");
			}

			if (nodes == null)
			{
				throw new ArgumentNullException("nodes");
			}

			Edges = edges.ToArray();
			Nodes = nodes.ToArray();
			TotalCost = edges.Sum(edge => edge.Cost);
		}

		public FlowEdge[] Edges { get; private set; }

		public FlowNode[] Nodes { get; private set; }

		public double TotalCost { get; private set; }

		public override string ToString()
		{
			// {SourceId/Type ResourceName - DestinationId/Type ResourceName} -> ...
			return String.Format("{{{0}}}", String.Join("} -> {", Edges.Select(e => e.ToString())));
		}
	}
}