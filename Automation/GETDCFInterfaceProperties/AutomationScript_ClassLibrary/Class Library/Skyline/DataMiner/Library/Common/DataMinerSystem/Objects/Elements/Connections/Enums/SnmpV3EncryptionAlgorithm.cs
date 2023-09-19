using System.ComponentModel;

namespace Skyline.DataMiner.Library.Common
{
	/// <summary>
	/// Specifies the SNMPv3 encryption algorithm.
	/// </summary>
	public enum SnmpV3EncryptionAlgorithm
	{
		/// <summary>
		/// Data Encryption Standard (DES).
		/// </summary>
		[Description("DES")]
		Des = 0,

		/// <summary>
		/// Advanced Encryption Standard (AES) 128 bit.
		/// </summary>
		[Description("AES128")]
		Aes128 = 1,

		/// <summary>
		/// Advanced Encryption Standard (AES) 192 bit.
		/// </summary>
		[Description("AES192")]
		Aes192 = 2,

		/// <summary>
		/// Advanced Encryption Standard (AES) 256 bit.
		/// </summary>
		[Description("AES256")]
		Aes256 = 3,

		/// <summary>
		/// Advanced Encryption Standard is defined in the Credential Library.
		/// </summary>
		DefinedInCredentialsLibrary = 4,

		/// <summary>
		/// No algorithm selected.
		/// </summary>
		[Description("None")]
		None = 5,

	}
}