namespace Skyline.DataMiner.DeveloperCommunityLibrary.FlowEngineering.Enums
{
	using System;

	[Flags]
	public enum InterfaceTypes
	{
		In = 1,
		Out = 2,
		InOut = In | Out // 3
	}
}