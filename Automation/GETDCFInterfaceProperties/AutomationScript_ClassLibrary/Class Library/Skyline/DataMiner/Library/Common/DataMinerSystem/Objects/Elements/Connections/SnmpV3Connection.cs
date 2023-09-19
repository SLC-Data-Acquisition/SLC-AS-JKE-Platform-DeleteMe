using System;
using System.Security.Cryptography;
using System.Text;
using Skyline.DataMiner.Net.Messages;

namespace Skyline.DataMiner.Library.Common
{
	/// <summary>
	/// Class representing a SNMPv3 class.
	/// </summary>
	public class SnmpV3Connection : ConnectionSettings, ISnmpV3Connection
	{
		private TimeSpan? elementTimeout;
		private TimeSpan timeout;
		private int retries;
		private string deviceAddress;
		private IUdp udpIpConfiguration;
		private ISnmpV3SecurityConfig securityConfig;
		private readonly int id;
		private readonly Guid libraryCredentials;


		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		internal SnmpV3Connection(ElementPortInfo info)
		{
			this.deviceAddress = info.BusAddress;
			this.retries = info.Retries;
			this.timeout = new TimeSpan(0, 0, 0, 0, info.TimeoutTime);
			this.elementTimeout = new TimeSpan(0, 0, info.ElementTimeoutTime / 1000);

			if (this.libraryCredentials == Guid.Empty)
			{

				SnmpV3SecurityLevelAndProtocol securityLevelAndProtocol = HelperClass.GetEnumFromDescription<SnmpV3SecurityLevelAndProtocol>(info.StopBits);
				SnmpV3EncryptionAlgorithm encryptionAlgorithm = HelperClass.GetEnumFromDescription<SnmpV3EncryptionAlgorithm>(info.FlowControl);
				SnmpV3AuthenticationAlgorithm authenticationProtocol = HelperClass.GetEnumFromDescription<SnmpV3AuthenticationAlgorithm>(info.Parity);

				string authenticationKey = info.GetCommunity;
				string encryptionKey = info.SetCommunity;
				string username = info.DataBits;

				this.securityConfig = new SnmpV3SecurityConfig(securityLevelAndProtocol, username, authenticationKey, encryptionKey, authenticationProtocol, encryptionAlgorithm);
			}
			else
			{
				this.SecurityConfig = new SnmpV3SecurityConfig(
					SnmpV3SecurityLevelAndProtocol.DefinedInCredentialsLibrary,
					"",
					"",
					"",
					SnmpV3AuthenticationAlgorithm.DefinedInCredentialsLibrary,
					SnmpV3EncryptionAlgorithm.DefinedInCredentialsLibrary);
			}

			this.id = info.PortID;
			this.elementTimeout = new TimeSpan(0, 0, 0, 0, info.ElementTimeoutTime);
			this.udpIpConfiguration = new Udp(info);
		}

		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		/// <param name="udpConfiguration">The udp configuration settings.</param>
		///<param name="securityConfig">The SNMPv3 security configuration.</param>
		public SnmpV3Connection(IUdp udpConfiguration, SnmpV3SecurityConfig securityConfig)
		{
			if (udpConfiguration == null)
			{
				throw new ArgumentNullException("udpConfiguration");
			}

			if (securityConfig == null)
			{
				throw new ArgumentNullException("securityConfig");
			}

			this.libraryCredentials = Guid.Empty;
			this.id = -1;
			this.udpIpConfiguration = udpConfiguration;
			this.deviceAddress = String.Empty;
			this.securityConfig = securityConfig;
			this.timeout = new TimeSpan(0, 0, 0, 0, 1500);
			this.retries = 3;
			this.elementTimeout = new TimeSpan(0, 0, 0, 30);
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
		public int Id
		{
			get { return id; }
			//set
			//{
			//	ChangedPropertyList.Add("Id");
			//	id = value;
			//}
		}

		/// <summary>
		/// Gets or Sets the device address.
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
		/// Get or Set the UDP Connection settings
		/// </summary>
		public IUdp UdpConfiguration
		{
			get { return udpIpConfiguration; }
			set
			{
				ChangedPropertyList.Add(ConnectionSetting.PortConnection);
				udpIpConfiguration = value;
			}
		}

		/// <summary>
		/// Get the libraryCredentials.
		/// </summary>
		public Guid LibraryCredentials
		{
			get
			{
				return libraryCredentials;
			}
		}

		/// <summary>
		/// Gets or sets the SNMPv3 security configuration.
		/// </summary>
		public ISnmpV3SecurityConfig SecurityConfig
		{
			get { return securityConfig; }
			set
			{
				ChangedPropertyList.Add(ConnectionSetting.SecurityConfig);
				securityConfig = value;
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
				LibraryCredential = Guid.Empty,
				StopBits = HelperClass.GetDescriptionFromEnum(this.SecurityConfig.SecurityLevelAndProtocol),
				FlowControl = HelperClass.GetDescriptionFromEnum(this.SecurityConfig.EncryptionAlgorithm),
				Parity = HelperClass.GetDescriptionFromEnum(this.SecurityConfig.AuthenticationAlgorithm),
				GetCommunity = RSA.Encrypt(this.SecurityConfig.AuthenticationKey),
				SetCommunity = RSA.Encrypt(this.SecurityConfig.EncryptionKey),
				DataBits = this.SecurityConfig.Username,

				PollingIPPort = Convert.ToString(this.udpIpConfiguration.RemotePort),
				IsSslTlsEnabled = this.udpIpConfiguration.IsSslTlsEnabled,
				PollingIPAddress = this.udpIpConfiguration.RemoteHost,
				PortID = portPosition,
				ProtocolType = ProtocolType.SnmpV3,

				Baudrate = String.Empty,
				LocalIPPort = this.udpIpConfiguration.LocalPort.ToString(),
				Number = this.udpIpConfiguration.NetworkInterfaceCard.ToString(),
				Type = "ip"
			};

			return portInfo;
		}

		/// <summary>
		/// Updates the provided ElementPortInfo object with any performed changes on the object.
		/// </summary>
		/// <param name="portInfo"></param>
		/// <param name="isCompatibilityIssueDetected"></param>
		internal override void UpdateElementPortInfo(ElementPortInfo portInfo, bool isCompatibilityIssueDetected)
		{
			ConnectionSettings mySecurityConfig = (ConnectionSettings)securityConfig;
			ConnectionSettings udpSettings = (ConnectionSettings)udpIpConfiguration;

			foreach (ConnectionSetting property in ChangedPropertyList)
			{
				switch (property)
				{

					case ConnectionSetting.DeviceAddress:
						portInfo.BusAddress = this.deviceAddress;
						break;
					case ConnectionSetting.Timeout:
						portInfo.TimeoutTime = Convert.ToInt32(timeout.TotalMilliseconds);
						break;
					case ConnectionSetting.ElementTimeout:
						portInfo.ElementTimeoutTime = Convert.ToInt32(elementTimeout.Value);
						break;
					case ConnectionSetting.Retries:
						portInfo.Retries = this.retries;
						break;
					case ConnectionSetting.PortConnection:
						udpSettings.UpdateElementPortInfo(portInfo, isCompatibilityIssueDetected);
						break;
					//case "Id":
					//	portInfo.PortID = this.id;
					//	break;
					case ConnectionSetting.SecurityConfig:
						mySecurityConfig.UpdateElementPortInfo(portInfo, isCompatibilityIssueDetected);
						break;
					default:
						continue;

				}
			}
			portInfo.ProtocolType = ProtocolType.SnmpV3;

			if (mySecurityConfig.IsUpdated)
			{
				mySecurityConfig.UpdateElementPortInfo(portInfo, isCompatibilityIssueDetected);
			}
			if (udpSettings.IsUpdated)
			{
				udpSettings.UpdateElementPortInfo(portInfo, isCompatibilityIssueDetected);
			}

		}

		/// <summary>
		/// Indicates if updates have been performed on the properties of the object.
		/// </summary>
		internal override bool IsUpdated
		{
			get
			{
				ConnectionSettings udpSettings = (ConnectionSettings)this.udpIpConfiguration;
				ConnectionSettings mySecurityConfig = (ConnectionSettings)securityConfig;
				return (ChangedPropertyList.Count > 0 || udpSettings.IsUpdated || mySecurityConfig.IsUpdated);
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
			ConnectionSettings mySecurityConfig = (ConnectionSettings)securityConfig;
			mySecurityConfig.ClearUpdates();
		}

	}
}