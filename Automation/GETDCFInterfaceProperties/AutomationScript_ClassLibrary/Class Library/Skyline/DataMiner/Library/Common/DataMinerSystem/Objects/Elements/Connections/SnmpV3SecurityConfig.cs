using Skyline.DataMiner.Net.Messages;
using System;

namespace Skyline.DataMiner.Library.Common
{
	/// <summary>
	/// Represents the Security settings linked to SNMPv3.
	/// </summary>
	public class SnmpV3SecurityConfig : ConnectionSettings, ISnmpV3SecurityConfig
	{
		private SnmpV3SecurityLevelAndProtocol securityLevelAndProtocol;
		private string username;
		private string authenticationKey;
		private string encryptionKey;
		private SnmpV3AuthenticationAlgorithm authenticationAlgorithm;
		private SnmpV3EncryptionAlgorithm encryptionAlgorithm;

		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		/// <param name="securityLevelAndProtocol">The security Level and Protocol.</param>
		/// <param name="username">The username.</param>
		/// <param name="authenticationKey">The authenticationKey</param>
		/// <param name="encryptionKey">The encryptionKey</param>
		/// <param name="authenticationAlgorithm">The authentication Algorithm.</param>
		/// <param name="encryptionAlgorithm">The encryption Algorithm.</param>
		/// <exception cref="System.ArgumentNullException">When username, authenticationKey or encryptionKey is null.</exception>
		internal SnmpV3SecurityConfig(SnmpV3SecurityLevelAndProtocol securityLevelAndProtocol, string username,
			string authenticationKey, string encryptionKey, SnmpV3AuthenticationAlgorithm authenticationAlgorithm,
			SnmpV3EncryptionAlgorithm encryptionAlgorithm)
		{
			if (username == null)
			{
				throw new System.ArgumentNullException("username");
			}

			if (authenticationKey == null)
			{
				throw new System.ArgumentNullException("authenticationKey");
			}

			if (encryptionKey == null)
			{
				throw new System.ArgumentNullException("encryptionKey");
			}

			this.securityLevelAndProtocol = securityLevelAndProtocol;
			this.username = username;
			this.authenticationKey = authenticationKey;
			this.encryptionKey = encryptionKey;
			this.authenticationAlgorithm = authenticationAlgorithm;
			this.encryptionAlgorithm = encryptionAlgorithm;
		}

		/// <summary>
		/// Initializes a new instance using No Authentication and No Privacy.
		/// </summary>
		/// <param name="username">The username.</param>
		/// <exception cref="System.ArgumentNullException">When the username is null.</exception>
		public SnmpV3SecurityConfig(string username)
		{
			if (username == null)
			{
				throw new System.ArgumentNullException("username");
			}
			this.securityLevelAndProtocol = SnmpV3SecurityLevelAndProtocol.NoAuthenticationNoPrivacy;
			this.username = username;
			this.authenticationKey = "";
			this.encryptionKey = "";
			this.authenticationAlgorithm = SnmpV3AuthenticationAlgorithm.None;
			this.encryptionAlgorithm = SnmpV3EncryptionAlgorithm.None;
		}
		/// <summary>
		/// Initializes a new instance using Authentication No Privacy.
		/// </summary>
		/// <param name="username">The username.</param>
		/// <param name="authenticationKey">The Authentication key.</param>
		/// <param name="authenticationAlgorithm">The Authentication Algorithm.</param>
		/// <exception cref="System.ArgumentNullException">When username, authenticationKey is null.</exception>
		/// <exception cref="IncorrectDataException">When None or DefinedInCredentialsLibrary is selected as authentication algorithm.</exception>
		public SnmpV3SecurityConfig(string username,string authenticationKey, SnmpV3AuthenticationAlgorithm authenticationAlgorithm)
		{
			if (username == null)
			{
				throw new System.ArgumentNullException("username");
			}

			if (authenticationKey == null)
			{
				throw new System.ArgumentNullException("authenticationKey");
			}

			if((authenticationAlgorithm==SnmpV3AuthenticationAlgorithm.None) || (authenticationAlgorithm == SnmpV3AuthenticationAlgorithm.DefinedInCredentialsLibrary))
			{
				throw new IncorrectDataException("Authentication Algorithm 'None' and 'DefinedInCredentialsLibrary' is Invalid when choosing 'Authentication No Privacy' as Security Level and Protocol.");
			}

			this.securityLevelAndProtocol = SnmpV3SecurityLevelAndProtocol.AuthenticationNoPrivacy;
			this.username = username;
			this.authenticationKey = authenticationKey;
			this.encryptionKey = "";
			this.authenticationAlgorithm = authenticationAlgorithm;
			this.encryptionAlgorithm = SnmpV3EncryptionAlgorithm.None;
		}
		/// <summary>
		/// Initializes a new instance using Authentication and Privacy.
		/// </summary>
		/// <param name="username">The username.</param>
		/// <param name="authenticationKey">The authentication key.</param>
		/// <param name="encryptionKey">The encryptionKey.</param>
		/// <param name="authenticationProtocol">The authentication algorithm.</param>
		/// <param name="encryptionAlgorithm">The encryption algorithm.</param>
		/// <exception cref="System.ArgumentNullException">When username, authenticationKey or encryptionKey is null.</exception>
		/// <exception cref="IncorrectDataException">When None or DefinedInCredentialsLibrary is selected as authentication algorithm or encryption algorithm.</exception>
		public SnmpV3SecurityConfig(string username,string authenticationKey,SnmpV3AuthenticationAlgorithm authenticationProtocol, string encryptionKey, SnmpV3EncryptionAlgorithm encryptionAlgorithm)
		{
			if (username == null)
			{
				throw new System.ArgumentNullException("username");
			}

			if (authenticationKey == null)
			{
				throw new System.ArgumentNullException("authenticationKey");
			}

			if (encryptionKey == null)
			{
				throw new System.ArgumentNullException("encryptionKey");
			}

			if ((authenticationProtocol == SnmpV3AuthenticationAlgorithm.None) || (authenticationProtocol == SnmpV3AuthenticationAlgorithm.DefinedInCredentialsLibrary))
			{
				throw new IncorrectDataException("Authentication Algorithm 'None' and 'DefinedInCredentialsLibrary' is Invalid when choosing 'Authentication No Privacy' as Security Level and Protocol.");
			}

			if ((encryptionAlgorithm == SnmpV3EncryptionAlgorithm.None) || (encryptionAlgorithm == SnmpV3EncryptionAlgorithm.DefinedInCredentialsLibrary))
			{
				throw new IncorrectDataException("Encryption Algorithm 'None' and 'DefinedInCredentialsLibrary' is Invalid when choosing 'Authentication and Privacy' as Security Level and Protocol.");
			}

			this.securityLevelAndProtocol = SnmpV3SecurityLevelAndProtocol.AuthenticationPrivacy;
			this.username = username;
			this.authenticationKey = authenticationKey;
			this.encryptionKey = encryptionKey;
			this.authenticationAlgorithm = authenticationProtocol;
			this.encryptionAlgorithm = encryptionAlgorithm;
		}



		/// <summary>
		/// Gets or sets the EncryptionAlgorithm.
		/// </summary>
		public SnmpV3EncryptionAlgorithm EncryptionAlgorithm
		{
			get { return encryptionAlgorithm; }
			set
			{
				if (encryptionAlgorithm != value)
				{
					ChangedPropertyList.Add(ConnectionSetting.EncryptionAlgorithm);
					encryptionAlgorithm = value;
				}
				
			}
		}

		/// <summary>
		/// Gets or sets the AuthenticationProtocol.
		/// </summary>
		public SnmpV3AuthenticationAlgorithm AuthenticationAlgorithm
		{
			get { return authenticationAlgorithm; }
			set
			{
				if (authenticationAlgorithm != value)
				{
					ChangedPropertyList.Add(ConnectionSetting.AuthenticationProtocol);
					authenticationAlgorithm = value;
				}
				
			}
		}

		/// <summary>
		/// Gets or sets the EncryptionKey.
		/// </summary>
		public string EncryptionKey
		{
			get { return encryptionKey; }
			set
			{
				if (encryptionKey != value)
				{
					ChangedPropertyList.Add(ConnectionSetting.EncryptionKey);
					encryptionKey = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the AuthenticationKey.
		/// </summary>
		public string AuthenticationKey
		{
			get { return authenticationKey; }
			set
			{
				if (AuthenticationKey != value)
				{
					ChangedPropertyList.Add(ConnectionSetting.AuthenticationKey);
					authenticationKey = value;
				}
				
			}
		}

		/// <summary>
		/// Gets or sets the username.
		/// </summary>
		public string Username
		{
			get { return username; }
			set
			{
				if (username != value)
				{
					ChangedPropertyList.Add(ConnectionSetting.Username);
					username = value;
				}
				
			}
		}

		/// <summary>
		/// Gets or sets the SecurityLevelAndProtocol.
		/// </summary>
		public SnmpV3SecurityLevelAndProtocol SecurityLevelAndProtocol
		{
			get { return securityLevelAndProtocol; }
			set
			{
				if (securityLevelAndProtocol != value)
				{
					ChangedPropertyList.Add(ConnectionSetting.SecurityLevelAndProtocol);
					securityLevelAndProtocol = value;
				}
				
			}
		}

		/// <summary>
		/// Indicates if a property has been updated or not.
		/// </summary>
		internal override bool IsUpdated
		{
			get
			{
				return (ChangedPropertyList.Count > 0);
			}
		}

		/// <summary>
		/// Creates an ElementPortInfo object based on the property values.
		/// </summary>
		/// <returns></returns>
		internal override ElementPortInfo CreateElementPortInfo(int portPosition,bool isCompatibilityIssueDetected)
		{
			throw new NotSupportedException("Method is not supported. ElementPortInfo content is directly created in corresponding connection.");
		}

		/// <summary>
		/// Update the ElementPortInfo object with the changed properties.
		/// </summary>
		/// <param name="portInfo"></param>
		/// <param name="isCompatibilityIssueDetected"></param>
		internal override void UpdateElementPortInfo(ElementPortInfo portInfo, bool isCompatibilityIssueDetected)
		{
			foreach (ConnectionSetting property in ChangedPropertyList)
			{
				switch (property)
				{
					case ConnectionSetting.SecurityLevelAndProtocol:
						portInfo.StopBits = HelperClass.GetDescriptionFromEnum(this.securityLevelAndProtocol);
						break;
					case ConnectionSetting.EncryptionAlgorithm:
						portInfo.FlowControl = HelperClass.GetDescriptionFromEnum(this.encryptionAlgorithm);
						break;
					case ConnectionSetting.AuthenticationProtocol:
						portInfo.Parity = HelperClass.GetDescriptionFromEnum(this.authenticationAlgorithm);
						break;
					case ConnectionSetting.GetCommunityString:
						portInfo.GetCommunity = RSA.Encrypt(this.authenticationKey);
						break;
					case ConnectionSetting.EncryptionKey:
						portInfo.SetCommunity = RSA.Encrypt(this.encryptionKey);
						break;
					case ConnectionSetting.Username:
						portInfo.DataBits = this.username;
						break;
				}
			}
		}

		/// <summary>
		/// Clear the performed update flags of the properties of the object.
		/// </summary>
		internal override void ClearUpdates()
		{
			this.ChangedPropertyList.Clear();
		}

	}
}