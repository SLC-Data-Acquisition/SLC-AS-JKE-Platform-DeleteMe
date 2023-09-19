namespace Skyline.DataMiner.DeveloperCommunityLibrary.FlowEngineering.Executors
{
	using Skyline.DataMiner.DeveloperCommunityLibrary.FlowEngineering.Messages;
	using Skyline.DataMiner.Library.Common.InterAppCalls.CallSingle;
	using Skyline.DataMiner.Library.Common.InterAppCalls.MessageExecution;
	
	public class RemovePathResponseExecutor : MessageExecutor<RemovePathResponse>
	{
		public RemovePathResponseExecutor(RemovePathResponse message) : base(message)
		{
		}

		/// <summary>
		/// 1) Reads data from SLProtocol, Engine or other data sources.
		/// </summary>
		/// <param name="dataSource">SLProtocol, Engine, or other data sources.</param>
		public override void DataGets(object dataSource)
		{
		}

		/// <summary>
		/// 2) Parses the data retrieved from a data source in DataGets.
		/// </summary>
		public override void Parse()
		{
		}

		/// <summary>
		/// 3) Validates received data for validity.
		/// </summary>
		/// <returns>A boolean indicating if the received data is valid.</returns>
		public override bool Validate()
		{
			return Message.Success;
		}

		/// <summary>
		/// 4) Modifies retrieved data and Message data into a correct format for setting.
		/// </summary>
		public override void Modify()
		{
		}

		/// <summary>
		/// 5) Writes data to SLProtocol, Engine, or another data destination.
		/// </summary>
		/// <param name="dataDestination">SLProtocol, Engine, or another data destination.</param>
		public override void DataSets(object dataDestination)
		{
		}

		/// <summary>
		/// 6) Creates a return Message.
		/// </summary>
		/// <returns>A message representing the response for the processed message.</returns>
		public override Message CreateReturnMessage()
		{
			return null;
		}
	}
}