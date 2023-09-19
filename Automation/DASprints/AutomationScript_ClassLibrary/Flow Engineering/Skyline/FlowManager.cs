namespace Skyline.DataMiner.DeveloperCommunityLibrary.FlowEngineering
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Linq;
	using Skyline.DataMiner.DeveloperCommunityLibrary.FlowEngineering.Enums;
	using Skyline.DataMiner.DeveloperCommunityLibrary.FlowEngineering.Executors;
	using Skyline.DataMiner.DeveloperCommunityLibrary.FlowEngineering.InputData;
	using Skyline.DataMiner.DeveloperCommunityLibrary.FlowEngineering.Messages;
	using Skyline.DataMiner.DeveloperCommunityLibrary.FlowEngineering.Path;
	using Skyline.DataMiner.DeveloperCommunityLibrary.FlowEngineering.Settings;
	using Skyline.DataMiner.Library.Common;
	using Skyline.DataMiner.Library.Common.InterAppCalls.CallBulk;
	using Skyline.DataMiner.Library.Common.InterAppCalls.CallSingle;
	using Skyline.DataMiner.Library.Common.InterAppCalls.Shared;
	using Skyline.DataMiner.Net;

	public class FlowManager
	{
		private static readonly Dictionary<Type, Type> MsgToExecutor = new Dictionary<Type, Type>
		{
			{ typeof(CalculatePathResponse), typeof(CalculatePathResponseExecutor) },
			{ typeof(RemovePathResponse), typeof(RemovePathResponseExecutor) }
		};

		private readonly IConnection _connection;
		private readonly ILogger _logger;

		private readonly IDmsElement _flowEngineering;

		private FlowManager(IConnection connection)
		{
			_connection = connection;
		}

		public FlowManager(IConnection connection, IDms dms) : this(connection)
		{
			var elements = dms.GetElements();

			IDmsElement[] flowEngineeringElements = elements.Where(IsFlowEngineeringElement).ToArray();

			if (flowEngineeringElements.Length == 0)
			{
				throw new InvalidOperationException("No Flow Engineering element found.");
			}

			if (flowEngineeringElements.Length > 1)
			{
				throw new InvalidOperationException(
					"Multiple Flow Engineering elements found. Use the other constructor to specify the correct element.");
			}

			_flowEngineering = flowEngineeringElements[0];
		}

		public FlowManager(IConnection connection, IDmsElement flowEngineeringElement) : this(connection)
		{
			if (!IsFlowEngineeringElement(flowEngineeringElement))
			{
				throw new ArgumentException("Invalid flowEngineering.", nameof(flowEngineeringElement));
			}

			_flowEngineering = flowEngineeringElement;
		}
		
		[EditorBrowsable(EditorBrowsableState.Never)]
		public FlowManager(IConnection connection, IDms dms, ILogger logger) : this(connection, dms)
		{
			_logger = logger;
		}
		
		[EditorBrowsable(EditorBrowsableState.Never)]
		public FlowManager(IConnection connection, IDmsElement flowEngineeringElement, ILogger logger) : this(connection, flowEngineeringElement)
		{
			_logger = logger;
		}

		/// <summary>
		/// Get the default settings. This includes the Dijkstra Algorithm & to stop at the destination.
		/// </summary>
		public static ISettings DefaultSettings
		{
			get
			{
				return new Settings.Settings
				{
					Algorithm = Algorithms.Dijkstra
				};
			}
		}

		private static bool IsFlowEngineeringElement(IDmsElement element)
		{
			if (element?.Protocol == null)
			{
				return false;
			}

			return String.Equals(element.Protocol.Name, "Skyline Route Engineering");
		}

		/// <summary>
		/// Try to calculate (a) path(s) with the provided input data. In case no settings are provided, the default settings will be used.
		/// </summary>
		/// <param name="inputData">Input data.</param>
		/// <param name="paths">Found paths.</param>
		/// <param name="settings">More granular specification about the path calculation.</param>
		/// <returns>True when at least one path is found.</returns>
		public bool TryCalculatePath(IInputData inputData, out IReadOnlyList<FlowPath> paths, ISettings settings = null)
		{
			_logger?.Log("TryCalculatePath: START");
			_logger?.Log("Settings is null: " + (settings == null));
			paths = null;

			try
			{
				CalculatePath request = new CalculatePath
				{
					InputData = inputData,
					Settings = settings ?? DefaultSettings
				};

				var responses = SendCommand(request);
				_logger?.Log("#Responses: " + responses.Count);
				if (responses.Count != 1)
				{
					return false;
				}

				Message response = responses[0];

				List<FlowPath> tempPaths = new List<FlowPath>();
				Message returnMessage;
				bool successExecute = response.TryExecute(_logger, tempPaths, MsgToExecutor, out returnMessage);

				_logger?.Log("Execution: " + successExecute);
				if (!successExecute)
				{
					return false;
				}

				paths = tempPaths;
				return true;
			}
			catch (Exception ex)
			{
				_logger?.Log("TryCalculatePath: Exception: " + ex);
				return false;
			}
			finally
			{
				_logger?.Log("TryCalculatePath: END");
			}
		}

		/// <summary>
		/// Try to remove a path.
		/// </summary>
		/// <param name="request">Request to remove the path.</param>
		/// <returns>True when the path is removed.</returns>
		public bool TryRemovePath(RemovePath request)
		{
			try
			{
				var responses = SendCommand(request).ToArray();

				Message returnMessage;
				return responses.Length == 1 && responses[0].TryExecute(null, null, MsgToExecutor, out returnMessage);
			}
			catch (Exception)
			{
				return false;
			}
		}

		private IList<Message> SendCommand(params Message[] requests)
		{
			IInterAppCall command = InterAppCallFactory.CreateNew();
			command.Messages.AddMessage(requests);
			command.Source = new Source("Flow Engineering API");
			command.ReturnAddress = new ReturnAddress(_flowEngineering.AgentId, _flowEngineering.Id, 9000001);
			return command.Send(_connection, _flowEngineering.AgentId, _flowEngineering.Id, 9000000, TimeSpan.FromSeconds(20), Statics.CustomSerializer).ToList();
		}
	}
}