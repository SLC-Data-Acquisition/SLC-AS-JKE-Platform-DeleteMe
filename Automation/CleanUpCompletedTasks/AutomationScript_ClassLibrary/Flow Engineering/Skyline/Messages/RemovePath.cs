namespace Skyline.DataMiner.DeveloperCommunityLibrary.FlowEngineering.Messages
{
	using System;

	using Skyline.DataMiner.Library.Common.InterAppCalls.CallSingle;

	public class RemovePath : Message
	{
		public RemovePath()
		{
			PathName = String.Empty;
		}

		public string ElementKey { get; set; }

		public int? DcfInterfaceId { get; set; }

		/// <summary>
		/// Is optional. If there is no name defined, it will remove all paths linked to the specified element (and DCF interface).
		/// </summary>
		public string PathName { get; set; }
	}
}