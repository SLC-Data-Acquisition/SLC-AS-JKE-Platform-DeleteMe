namespace Skyline.DataMiner.Library.Common
{
	/// <summary>
	/// Represents a TCP/IP connection.
	/// </summary>
	internal interface ITcp : IIpBased
	{
		/// <summary>
		/// Gets or sets the local port number.
		/// </summary>
		/// <value>The local port number.</value>
		/// <remarks>Configuring the local port is only supported for <see cref="ISerialConnection"/> connections.</remarks>
		int LocalPort { get; set; }

		/// <summary>
		/// Gets or set the remote port number.
		/// </summary>
		/// <value>The remote port number.</value>
		int RemotePort { get; set; }

		bool IsSslTlsEnabled { get; set; } // TODO: NF:	Whether this connection has SSL/TLS enabled. Can only be set to true on connection for protocol type Serial and port type IP.

		/// <summary>
		/// 
		/// </summary>
		bool IsDedicated { get; } // TODO: This is the "single" of serial and smart-serial. Cannot be configured.
	}
}
