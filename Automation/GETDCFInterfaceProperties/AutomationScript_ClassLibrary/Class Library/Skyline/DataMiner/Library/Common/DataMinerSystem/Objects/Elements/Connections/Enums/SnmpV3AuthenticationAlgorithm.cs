using System.ComponentModel;

namespace Skyline.DataMiner.Library.Common
{
	/// <summary>
	/// Specifies the SNMPv3 authentication protocol.
	/// </summary>
	public enum SnmpV3AuthenticationAlgorithm
	{
		/// <summary>
		/// Message Digest 5 (MD5).
		/// </summary>
		[Description("MD5")]
		Md5 = 0,

		/// <summary>
		/// Secure Hash Algorithm (SHA).
		/// </summary>
		[Description("SHA")]
		Sha1 = 1,

		/// <summary>
		/// Secure Hash Algorithm (SHA) 224.
		/// </summary>
		[Description("SHA224")]
		Sha224 = 2,

		/// <summary>
		/// Secure Hash Algorithm (SHA) 256.
		/// </summary>
		[Description("SHA256")]
		Sha256 = 3,

		/// <summary>
		/// Secure Hash Algorithm (SHA) 384.
		/// </summary>
		[Description("SHA384")]
		Sha384 = 4,

		/// <summary>
		/// Secure Hash Algorithm (SHA) 512.
		/// </summary>
		[Description("SHA512")]
		Sha512 = 5,

		/// <summary>
		/// Algorithm is defined in the Credential Library.
		/// </summary>
		DefinedInCredentialsLibrary = 6,

		/// <summary>
		/// No algorithm selected.
		/// </summary>
		[Description("None")]
		None = 7,

	}
}
