namespace Skyline.DataMiner.DeveloperCommunityLibrary.FlowEngineering.Path
{
	using System;

	public class FlowEdge
	{
		public FlowEdge(FlowNode source, FlowNode target, double cost)
		{
			Source = source;
			Target = target;
			Cost = cost;
		}

		public FlowNode Source { get; private set; }

		public FlowNode Target { get; private set; }

		public double Cost { get; private set; }

		public override string ToString()
		{
			return String.Format("{0} - {1}", Source, Target);
		}
	}
}