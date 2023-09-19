namespace Skyline.DataMiner.DeveloperCommunityLibrary.FlowEngineering
{
	using System.ComponentModel;

	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface ILogger
	{
		void Log(string message);
	}
}