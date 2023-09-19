namespace Skyline.DataMiner.Library.Automation
{
	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Library.Common;
	using System;

	/// <summary>
	/// Defines extension methods on the <see cref="IEngine"/> interface.
	/// </summary>
	[Skyline.DataMiner.Library.Common.Attributes.DllImport("SLManagedAutomation.dll")]
	[Skyline.DataMiner.Library.Common.Attributes.DllImport("SLNetTypes.dll")]
	public static class IEngineExtensions
	{
		/// <summary>
		/// Retrieves an object implementing the <see cref="IDms"/> interface.
		/// </summary>
		/// <param name="engine">The <see cref="IEngine"/> implementation.</param>
		/// <exception cref="ArgumentNullException"><paramref name="engine"/> is <see langword="null" />.</exception>
		/// <returns>The <see cref="IDms"/> object.</returns>
		public static IDms GetDms(this IEngine engine)
		{
			if (engine == null)
			{
				throw new ArgumentNullException("engine");
			}

			return new Dms(new ConnectionCommunication(Engine.SLNetRaw));
		}
	}
}