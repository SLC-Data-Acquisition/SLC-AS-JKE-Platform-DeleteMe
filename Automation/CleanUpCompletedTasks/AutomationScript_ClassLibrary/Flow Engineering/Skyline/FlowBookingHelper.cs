namespace Skyline.DataMiner.DeveloperCommunityLibrary.FlowEngineering
{
	using System;
	using System.Collections.Generic;
	using Booking;
	using Newtonsoft.Json;
	using Path;
	using Skyline.DataMiner.Automation;

	public static class FlowBookingHelper
	{
		public static string CreateContributingBooking(
			IEngine engine,
			FlowPath path,
			string bookingName,
			Guid parentSystemFunction,
			FlowRecurrence flowRecurrence,
			string bookingAppName)
		{
			return CreateContributingBooking(
				engine, 
				path, 
				bookingName,
				new FlowContributingConfig(parentSystemFunction),
				flowRecurrence,
				bookingAppName);
		}

		public static string CreateContributingBooking(
			IEngine engine,
			FlowPath path,
			string bookingName,
			FlowContributingConfig contributingConfig,
			FlowRecurrence flowRecurrence,
			string bookingAppName)
		{
			FlowInputData inputData = new FlowInputData(
				bookingName,
				bookingAppName,
				contributingConfig,
				path,
				flowRecurrence);
			
			return CallScriptAndGetResult(engine, inputData);
		}

		private static string CallScriptAndGetResult(IEngine engine, FlowInputData inputData)
		{
			try
			{
				SubScriptOptions scriptOptions = engine.PrepareSubScript("FLE_CreateBooking");
				scriptOptions.PerformChecks = false;
				scriptOptions.Synchronous = true;
				scriptOptions.SelectScriptParam("InputData", JsonConvert.SerializeObject(inputData));
				scriptOptions.StartScript();

				Dictionary<string, string> result = scriptOptions.GetScriptResult();
				return result.TryGetValue("ContributingResourceId", out string id) ? id : Guid.Empty.ToString();
			}
			catch (Exception)
			{
				return Guid.Empty.ToString();
			}
		}
	}
}
