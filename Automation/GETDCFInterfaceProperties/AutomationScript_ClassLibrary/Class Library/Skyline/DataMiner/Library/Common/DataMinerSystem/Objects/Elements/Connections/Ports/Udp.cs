namespace Skyline.DataMiner.Library.Common
{
	using System;
	using Skyline.DataMiner.Net.Messages;
	/// <summary>
	/// Class representing an UDP connection.
	/// </summary>
	public class Udp : ConnectionSettings, IUdp
	{
		private int? localPort;
		private int remotePort;
		private bool isSslTlsEnabled;
		private readonly bool isDedicated;
		private string remoteHost;
		private int networkInterfaceCard;

		/// <summary>
		/// Initializes a new instance, using default values for localPort (null=Auto) SslTlsEnabled (false), IsDedicated (false) and NetworkInterfaceCard (0=Auto)
		/// </summary>
		/// <param name="remoteHost">The IP or name of the remote host.</param>
		/// <param name="remotePort">The port number of the remote host.</param>
		public Udp(string remoteHost, int remotePort)
		{
			this.localPort = null;
			this.remotePort = remotePort;
			this.isSslTlsEnabled = false;
			this.isDedicated = false;
			this.remoteHost = remoteHost;
			this.networkInterfaceCard = 0;
		}

		/// <summary>
		/// Initializes a new instance using a <see cref="ElementPortInfo"/> object.
		/// </summary>
		/// <param name="info"></param>
		internal Udp(ElementPortInfo info)
		{
			if (!info.LocalIPPort.Equals("")) localPort = Convert.ToInt32(info.LocalIPPort);
			remoteHost = info.PollingIPAddress;
			remotePort = Convert.ToInt32(info.PollingIPPort);
			isSslTlsEnabled = info.IsSslTlsEnabled;
			isDedicated = IsDedicatedConnection(info);

			int networkInterfaceId = String.IsNullOrWhiteSpace(info.Number) ? 0 : Convert.ToInt32(info.Number);
			networkInterfaceCard = networkInterfaceId;
		}

		/// <summary>
		/// Gets or sets the local port.
		/// </summary>
		public int? LocalPort
		{
			get { return localPort; }

			set
			{
				if (localPort != value)
				{
					ChangedPropertyList.Add(ConnectionSetting.LocalPort);
					localPort = value;
				}
				
			}
		}

		/// <summary>
		/// Gets or sets the remote port.
		/// </summary>
		public int RemotePort
		{
			get { return remotePort; }
			set
			{
				if (remotePort != value)
				{
					ChangedPropertyList.Add(ConnectionSetting.RemotePort);
					remotePort = value;
				}
				
			}
		}

		/// <summary>
		/// Gets or sets if TLS is enabled on the connection.
		/// </summary>
		public bool IsSslTlsEnabled
		{
			get { return isSslTlsEnabled; }
			set
			{
				if (isSslTlsEnabled != value)
				{
					ChangedPropertyList.Add(ConnectionSetting.IsSslTlsEnabled);
					isSslTlsEnabled = value;
				}
				
			}
		}

		/// <summary>
		/// Gets or sets if a dedicated connection is used.
		/// </summary>
		public bool IsDedicated
		{
			get { return isDedicated; }
		}

		/// <summary>
		/// Gets or sets the IP Address or name of the remote host.
		/// </summary>
		public string RemoteHost
		{
			get { return remoteHost; }
			set
			{
				if (remoteHost != value)
				{
					ChangedPropertyList.Add(ConnectionSetting.RemoteHost);
					remoteHost = value;
				}
				
			}
		}

		/// <summary>
		/// Gets or sets the network interface card number.
		/// </summary>
		public int NetworkInterfaceCard
		{
			get { return networkInterfaceCard; }
			set
			{
				if (networkInterfaceCard != value)
				{
					ChangedPropertyList.Add(ConnectionSetting.NetworkInterfaceCard);
					networkInterfaceCard = value;
				}
				
			}
		}

		/// <summary>
		/// Indicates whether changes have been applied to the properties.
		/// </summary>
		internal override bool IsUpdated
		{
			get
			{
				return (ChangedPropertyList.Count > 0);
			}
		}

		/// <summary>
		/// Updates the provided ElementPortInfo object with any performed changes on the object.
		/// </summary>
		/// <param name="portInfo"></param>
		/// <param name="isCompatibilityIssueDetected"></param>
		internal override void UpdateElementPortInfo(ElementPortInfo portInfo, bool isCompatibilityIssueDetected)
		{
			foreach (ConnectionSetting property in ChangedPropertyList)
			{
				switch (property)
				{

					case ConnectionSetting.LocalPort:
						portInfo.LocalIPPort = Convert.ToString(this.localPort);
						break;
					case ConnectionSetting.RemotePort:
						portInfo.PollingIPPort = Convert.ToString(this.remotePort);
						break;
					case ConnectionSetting.IsSslTlsEnabled:
						portInfo.IsSslTlsEnabled = this.isSslTlsEnabled;
						break;
					case ConnectionSetting.RemoteHost:
						portInfo.PollingIPAddress = this.remoteHost;
						break;
					case ConnectionSetting.NetworkInterfaceCard:
						portInfo.Number = Convert.ToString(this.networkInterfaceCard);
						break;
					default:
						continue;

				}
			}
		}

		/// <summary>
		/// Creates a new <see cref="ElementPortInfo"/> object.
		/// </summary>
		/// <returns></returns>
		internal override ElementPortInfo CreateElementPortInfo(int portPosition,bool isCompatibilityIssueDetected)
		{
			throw new NotSupportedException("Method is not supported. ElementPortInfo content is directly created in corresponding connection.");
		}

		/// <summary>
		/// Clear the list keeping track of all the changes performed on properties.
		/// </summary>
		internal override void ClearUpdates()
		{
			this.ChangedPropertyList.Clear();
		}

		/// <summary>
		/// Determines if a connection is using a dedicated connection or not (e.g serial single, smart serial single).
		/// </summary>
		/// <param name="info">ElementPortInfo</param>
		/// <returns>Whether a connection is marked as single or not.</returns>
		private static bool IsDedicatedConnection(ElementPortInfo info)
		{
			bool isDedicatedConnection = false;

			switch (info.ProtocolType)
			{
				case ProtocolType.SerialSingle:
				case ProtocolType.SmartSerialRawSingle:
				case ProtocolType.SmartSerialSingle:
					isDedicatedConnection = true;
					break;
				default:
					isDedicatedConnection = false;
					break;
			}

			return isDedicatedConnection;
		}
	}
}