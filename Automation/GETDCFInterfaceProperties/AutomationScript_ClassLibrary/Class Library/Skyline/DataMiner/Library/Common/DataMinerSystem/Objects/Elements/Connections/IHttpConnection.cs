using System.Collections.Generic;

namespace Skyline.DataMiner.Library.Common
{
	interface IHttpConnection : IRealConnection
	{
		ITcp Connection { get; set; }

		// TODO: "BypassProxy" and possible other uses? Where to model this?
		string BusAddress { get; set; }

		// TODO: Has dedicated field in ElementInfoResponseMessage. Only seems applicable in case of HTTP (WebSocket) connections.
		bool BypassProxy { get; set; }

		// For HTTP, this will change from http:// to https://
		// TODO: Define interface ISecurable { boll IsSecure {get;set;}} ?
		bool IsSecure { get; set; } // TODO: Could also be provided in the TCP and UDP underlying connection. Not applicable for SNMP. For HTTP and WebSocket,  from ws:// to wss://. For serial (and smart-serial), this will set the IsSslTlsEnabled.
	}
}
