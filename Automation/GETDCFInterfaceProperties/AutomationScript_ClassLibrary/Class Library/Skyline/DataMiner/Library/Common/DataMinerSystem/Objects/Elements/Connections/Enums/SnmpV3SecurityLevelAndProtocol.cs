using System.ComponentModel;

namespace Skyline.DataMiner.Library.Common
{
	/// <summary>
	/// Specifies the SNMP v3 security level and protocol.
	/// </summary>
	public enum SnmpV3SecurityLevelAndProtocol
	{
		/// <summary>
		/// Authentication and privacy.
		/// </summary>
		[Description("authPriv")]
		AuthenticationPrivacy = 0,

		/// <summary>
		/// Authentication but no privacy.
		/// </summary>
		[Description("authNoPriv")]
		AuthenticationNoPrivacy = 1,

		/// <summary>
		/// No authentication and no privacy.
		/// </summary>
		[Description("noAuthNoPriv")]
		NoAuthenticationNoPrivacy = 2,

		/// <summary>
		/// Security Level and Protocol is defined in the Credential library.
		/// </summary>
		DefinedInCredentialsLibrary = 3
	}
}
