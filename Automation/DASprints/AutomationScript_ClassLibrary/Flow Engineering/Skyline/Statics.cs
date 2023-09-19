namespace Skyline.DataMiner.DeveloperCommunityLibrary.FlowEngineering
{
	using System;
	using System.Collections.Generic;

	using Skyline.DataMiner.DeveloperCommunityLibrary.FlowEngineering.Enums;
	using Skyline.DataMiner.DeveloperCommunityLibrary.FlowEngineering.InputData;
	using Skyline.DataMiner.DeveloperCommunityLibrary.FlowEngineering.Messages;
	using Skyline.DataMiner.DeveloperCommunityLibrary.FlowEngineering.Path;
	using Skyline.DataMiner.DeveloperCommunityLibrary.FlowEngineering.Settings;
	using Skyline.DataMiner.Library.Common.InterAppCalls.CallSingle;
	using Skyline.DataMiner.Library.Common.Serializing;
	using Skyline.DataMiner.Net.SRM.Capabilities;
	using Skyline.DataMiner.Net.SRM.Capacities;

	public static class Statics
	{
		public static readonly IReadOnlyList<Type> KnownTypes = new List<Type>
		{
			// Messages
			typeof(CalculatePath),
			typeof(CalculatePathResponse),
			typeof(RemovePath),
			typeof(RemovePathResponse),

			// Settings
			typeof(ISettings),
			typeof(Settings.Settings),
			
			// InputData
			typeof(IInputData),
			typeof(ElementInputData),
			typeof(ResourceInputData),

			// InputData - SRM
			typeof(MultiResourceCapacityUsage),
			typeof(ResourceCapabilityUsage),

			// Path
			typeof(FlowPath),
			typeof(FlowEdge),
			typeof(FlowNode),

			// Enums
			typeof(InterfaceTypes),
			typeof(Algorithms)
		};
		
		public static readonly ISerializer CustomSerializer = SerializerFactory.CreateInterAppSerializer(typeof(Message), KnownTypes);
	}
}