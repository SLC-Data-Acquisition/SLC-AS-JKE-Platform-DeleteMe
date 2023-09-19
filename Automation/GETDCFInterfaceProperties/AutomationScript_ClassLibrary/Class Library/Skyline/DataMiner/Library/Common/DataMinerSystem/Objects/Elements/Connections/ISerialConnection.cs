namespace Skyline.DataMiner.Library.Common
{
	/// <summary>
	/// Represents a serial connection.
	/// </summary>
	interface ISerialConnection : IRealConnection
	{
		// TODO: Model serial single.
		// bool IsDedicatedConnection { get; } or make subclass?

		/// <summary>
		/// Gets or sets the port connection.
		/// </summary>
		/// <value>The port connection.</value>
		IPortConnection PortConnection { get; set; }

		// TODO: TBD: where to model this?
		string BusAddress { get; set; }

		bool IsSecure { get; set; }
	}
}
