using System;
using System.Diagnostics;
using Skyline.DataMiner.Net.Messages;

namespace Skyline.DataMiner.Library.Common
{
	/// <summary>
	/// Class representing an SNMPv1 connection.
	/// </summary>
	public class SnmpV1Connection : ConnectionSettings, ISnmpV1Connection
	{
		private string getCommunityString;
		private string setCommunityString;
		private string deviceAddress;
		private TimeSpan timeout;
		private int retries;
		private IUdp udpIpConfiguration;
		private readonly int id;
		private readonly Guid libraryCredentials;
		private TimeSpan? elementTimeout;

		/// <summary>
		/// Initiates an new instance.
		/// </summary>
		internal SnmpV1Connection(ElementPortInfo info)
		{
			this.deviceAddress = info.BusAddress;
			this.retries = info.Retries;
			this.timeout = new TimeSpan(0, 0, 0, 0, info.TimeoutTime);
			this.libraryCredentials = info.LibraryCredential;

			//this.elementTimeout = new TimeSpan(0, 0, info.ElementTimeoutTime / 1000);
			if (this.libraryCredentials == Guid.Empty)
			{
				this.getCommunityString = info.GetCommunity;
				this.setCommunityString = info.SetCommunity;
			}
			else
			{
				this.getCommunityString = String.Empty;
				this.setCommunityString = String.Empty;
			}

			this.id = info.PortID;
			this.elementTimeout = new TimeSpan(0, 0, 0, 0, info.ElementTimeoutTime);
			this.udpIpConfiguration = new Udp(info);
		}

		/// <summary>
		/// /// Initiates a new instance with default settings for Get Community String (public), Set Community String (private), Device Address (empty),
		/// Command Timeout (1500ms), Retries (3) and Element Timeout (30s).
		/// </summary>
		/// <param name="udpConfiguration">The UDP configuration parameters.</param>
		public SnmpV1Connection(IUdp udpConfiguration)
		{
			if (udpConfiguration == null)
			{
				throw new ArgumentNullException("udpConfiguration");
			}

			this.id = -1;
			this.udpIpConfiguration = udpConfiguration;
			this.getCommunityString = "public";
			this.setCommunityString = "private";
			this.deviceAddress = String.Empty;
			this.timeout = new TimeSpan(0, 0, 0, 0, 1500);
			this.retries = 3;
			this.elementTimeout = new TimeSpan(0, 0, 0, 30);
		}

		/// <summary>
		/// Get or sets the Get community string.
		/// </summary>
		public string GetCommunityString
		{
			get { return getCommunityString; }

			set
			{
				if (getCommunityString != value)
				{
					ChangedPropertyList.Add(ConnectionSetting.GetCommunityString);
					getCommunityString = value;
				}
			}
		}

		/// <summary>
		/// Get or set the Set Community String.
		/// </summary>
		public string SetCommunityString
		{
			get { return setCommunityString; }
			set
			{
				if (setCommunityString != value)
				{
					ChangedPropertyList.Add(ConnectionSetting.SetCommunityString);
					setCommunityString = value;
				}
			}
		}

		/// <summary>
		/// Get the libraryCredentials
		/// </summary>
		public Guid LibraryCredentials
		{
			get { return libraryCredentials; }
		}

		/// <summary>
		/// Get or Set the device address.
		/// </summary>
		public string DeviceAddress
		{
			get { return deviceAddress; }
			set
			{
				if (deviceAddress != value)
				{
					ChangedPropertyList.Add(ConnectionSetting.DeviceAddress);
					deviceAddress = value;
				}
			}
		}

		/// <summary>
		/// Get or Set the timeout value.
		/// </summary>
		public TimeSpan Timeout
		{
			get { return timeout; }
			set
			{
				if (timeout != value)
				{
					if (value.TotalMilliseconds >= 10 && value.TotalMilliseconds <= 120000)
					{
						ChangedPropertyList.Add(ConnectionSetting.Timeout);
						timeout = value;
					}
					else
					{
						throw new IncorrectDataException("Timeout value should be between 10 and 120 000 ms.");
					}
				}
				
			}
		}

		/// <summary>
		/// Get or Set the amount of retries.
		/// </summary>
		public int Retries
		{
			get { return retries; }
			set
			{
				if (retries != value)
				{
					if (value >= 0 && value <= 10)
					{
						ChangedPropertyList.Add(ConnectionSetting.Retries);
						retries = value;
					}
					else
					{
						throw new IncorrectDataException("Retries value should be between 0 and 10.");
					}
				}
			}
		}

		/// <summary>
		/// Get or Set the timespan before the element will go into timeout after not responding.
		/// </summary>
		/// <value>When null, the connection will not be taken into account for the element to go into timeout.</value>
		public TimeSpan? ElementTimeout
		{
			get { return elementTimeout; }
			set
			{
				if (elementTimeout != value)
				{
					if (value == null || (value.Value.TotalSeconds >= 1 && value.Value.TotalSeconds <= 120))
					{
						ChangedPropertyList.Add(ConnectionSetting.ElementTimeout);
						elementTimeout = value;
					}
					else
					{
						throw new IncorrectDataException("ElementTimeout value should be between 1 and 120 sec.");
					}
				}
				
			}
		}

		/// <summary>
		/// Get or Set the UDP Connection settings
		/// </summary>
		public IUdp UdpConfiguration
		{
			get
			{
				return udpIpConfiguration;
			}

			set
			{
				ChangedPropertyList.Add(ConnectionSetting.PortConnection);
				udpIpConfiguration = value;
			}
		}

		/// <summary>
		/// Get or Set the connection id.
		/// </summary>
		public int Id
		{
			get
			{
				return id;
			}
		}

		/// <summary>
		/// Creates an ElementPortPortInfo object based on the field contents.
		/// </summary>
		/// <returns>ElementPortInfo object.</returns>
		internal override ElementPortInfo CreateElementPortInfo(int portPosition,bool isCompatibilityIssueDetected)
		{
			ElementPortInfo portInfo = new ElementPortInfo
			{
				BusAddress = this.deviceAddress,
				Retries = this.retries,
				TimeoutTime = Convert.ToInt32(timeout.TotalMilliseconds),
				LibraryCredential = this.libraryCredentials
			};
			if (this.libraryCredentials == Guid.Empty)
			{
				portInfo.GetCommunity = this.getCommunityString;
				portInfo.SetCommunity = this.setCommunityString;
			}
			else
			{
				portInfo.GetCommunity = String.Empty;
				portInfo.SetCommunity = String.Empty;
			}

			portInfo.PollingIPPort = Convert.ToString(this.udpIpConfiguration.RemotePort);
			portInfo.IsSslTlsEnabled = this.udpIpConfiguration.IsSslTlsEnabled;
			portInfo.PollingIPAddress = this.udpIpConfiguration.RemoteHost;
			portInfo.LocalIPPort = this.udpIpConfiguration.LocalPort.ToString();
			portInfo.Number = this.udpIpConfiguration.NetworkInterfaceCard.ToString();

			portInfo.PortID = portPosition;
			portInfo.ProtocolType = ProtocolType.SnmpV1;
			portInfo.Type = "ip";

			portInfo.Baudrate = String.Empty;
			portInfo.DataBits = String.Empty;
			portInfo.FlowControl = String.Empty;
			portInfo.Parity = String.Empty;

			return portInfo;
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
					case ConnectionSetting.GetCommunityString:
						portInfo.GetCommunity = this.getCommunityString;
						break;
					case ConnectionSetting.SetCommunityString:
						portInfo.SetCommunity = this.setCommunityString;
						break;
					case ConnectionSetting.DeviceAddress:
						portInfo.BusAddress = this.deviceAddress;
						break;
					case ConnectionSetting.Timeout:
						portInfo.TimeoutTime = Convert.ToInt32(timeout.TotalMilliseconds);
						break;
					case ConnectionSetting.Retries:
						portInfo.Retries = this.retries;
						break;
					case ConnectionSetting.PortConnection:
						portInfo.PollingIPPort = Convert.ToString(this.udpIpConfiguration.RemotePort);
						portInfo.IsSslTlsEnabled = this.udpIpConfiguration.IsSslTlsEnabled;
						portInfo.PollingIPAddress = this.udpIpConfiguration.RemoteHost;
						break;
					case ConnectionSetting.ElementTimeout:
						portInfo.ElementTimeoutTime = Convert.ToInt32(elementTimeout.Value);
						break;
					//case "Id":
					//	portInfo.PortID = this.id;
					//	break;
					default:
						continue;

				}
			}

			ConnectionSettings udpSettings = (ConnectionSettings)this.udpIpConfiguration;
			udpSettings.UpdateElementPortInfo(portInfo, isCompatibilityIssueDetected);
			portInfo.ProtocolType = ProtocolType.SnmpV1;
		}

		/// <summary>
		/// Indicates if updates have been performed on the properties of the object.
		/// </summary>
		internal override bool IsUpdated
		{
			get
			{
				ConnectionSettings udpSettings = (ConnectionSettings)this.udpIpConfiguration;
				return (ChangedPropertyList.Count > 0 || udpSettings.IsUpdated);
			}
		}

		/// <summary>
		/// Clear the performed update flags of the properties of the object.
		/// </summary>
		internal override void ClearUpdates()
		{
			this.ChangedPropertyList.Clear();
			ConnectionSettings udpSettings = (ConnectionSettings)this.udpIpConfiguration;
			udpSettings.ClearUpdates();
		}
	}
}