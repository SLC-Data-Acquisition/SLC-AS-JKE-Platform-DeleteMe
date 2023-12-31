﻿namespace Skyline.DataMiner.Library.Common
{
	using Net.Exceptions;
	using Net.Messages;

	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Security.Cryptography;

	/// <summary>
	/// Represents a DataMiner Agent.
	/// </summary>
	internal class Dma : DmsObject, IDma
	{
		/// <summary>
		/// The object used for DMS communication.
		/// </summary>
		private new readonly IDms dms;

		/// <summary>
		/// The DataMiner Agent ID.
		/// </summary>
		private readonly int id;
		private string versionInfo;
		private string hostName;
		private string name;
		private AgentState state;
		private bool stateRetrieved;
		//not applicable in current build (kept to not differ a lot from 9.6.3 build and can be used whenever a new compatibility issue needs resolving)
		internal const string snmpV3AuthenticationChangeDMAVersion = "10.0.3.0";

		/// <summary>
		/// Initializes a new instance of the <see cref="Dma"/> class.
		/// </summary>
		/// <param name="dms">The DataMiner System.</param>
		/// <param name="id">The ID of the DataMiner Agent.</param>
		/// <exception cref="ArgumentNullException">The <see cref="IDms"/> reference is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException">The DataMiner Agent ID is negative.</exception>
		internal Dma(IDms dms, int id)
			: base(dms)
		{
			if (id < 1)
			{
				throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, "Invalid DataMiner agent ID: {0}", id), "id");
			}

			this.dms = dms;
			this.id = id;
		}

		internal Dma(IDms dms, GetDataMinerInfoResponseMessage infoMessage)
			: base(dms)
		{
			if (infoMessage == null)
			{
				throw new ArgumentNullException("infoMessage");
			}

			Parse(infoMessage);
		}

		/// <summary>
		/// Gets the name of the host that is hosting this DataMiner Agent.
		/// </summary>
		/// <value>The name of the host.</value>
		/// <exception cref="AgentNotFoundException">The Agent was not found in the DataMiner System.</exception>
		public string HostName
		{
			get
			{
				LoadOnDemand();
				return hostName;
			}
		}

		/// <summary>
		/// Gets the ID of this DataMiner Agent.
		/// </summary>
		/// <value>The ID of this DataMiner Agent.</value>
		public int Id
		{
			get
			{
				return id;
			}
		}

		/// <summary>
		/// Gets the name of this DataMiner Agent.
		/// </summary>
		/// <value>The name of this DataMiner Agent.</value>
		/// <exception cref="AgentNotFoundException">The Agent was not found in the DataMiner System.</exception>
		public string Name
		{
			get
			{
				LoadOnDemand();
				return name;
			}
		}

		/// <summary>
		/// Gets the version information of the DataMiner Agent.
		/// </summary>
		public string VersionInfo
		{
			get
			{
				LoadOnDemand();
				return versionInfo;
			}
		}

		/// <summary>
		/// Gets the state of this DataMiner Agent.
		/// </summary>
		/// <value>The state of this DataMiner Agent.</value>
		/// <exception cref="AgentNotFoundException">The Agent was not found in the DataMiner System.</exception>
		public AgentState State
		{
			get
			{
				if (!stateRetrieved)
				{
					try
					{
						GetDataMinerStateMessage message = new GetDataMinerStateMessage(id);
						var response = dms.Communication.SendSingleResponseMessage(message) as GetDataMinerStateResponseMessage;

						if (response != null)
						{
							stateRetrieved = true;
							state = (AgentState)response.State;
						}
					}
					catch (Skyline.DataMiner.Net.Exceptions.DataMinerCommunicationException e)
					{
						if (e.ErrorCode == -2147220787)
						{
							// 0x800402CD No connection.
							throw new AgentNotFoundException(id);
						}
					}
				}

				return state;
			}
		}

		/// <summary>
		/// Creates a new element with the specified configuration.
		/// </summary>
		/// <param name="configuration">The configuration of the element to be created.</param>
		/// <exception cref="ArgumentNullException"><paramref name="configuration"/> is <see langword="null"/>.</exception>
		/// <exception cref="IncorrectDataException">The provided configuration is invalid.</exception>
		/// <returns>The ID of the created element.</returns>
		public DmsElementId CreateElement(ElementConfiguration configuration)
		{
			if (configuration == null)
			{
				throw new ArgumentNullException("configuration");
			}

			bool valid = configuration.Connections.ValidateConnectionTypes();
			if (valid)
			{
				try
				{
					bool isCompatibilityIssueDetected = IsVersionHigher(snmpV3AuthenticationChangeDMAVersion);
					AddElementMessage createMessage = HelperClass.CreateAddElementMessage(configuration, isCompatibilityIssueDetected);
					createMessage.DataMinerID = id;

					AddElementResponseMessage createResponse = (AddElementResponseMessage)dms.Communication.SendSingleResponseMessage(createMessage);
					int elementId = createResponse.NewID;

					return new DmsElementId(id, elementId);
				}
				catch (DataMinerException e)
				{
					if (e.ErrorCode == -2147220959)
					{
						// 0x80040221, SL_INVALID_DATA, "Invalid data".
						string message = String.Format(CultureInfo.InvariantCulture, "Invalid data: '{0}'", configuration);
						throw new IncorrectDataException(message);
					}
					else
					{
						throw;
					}
				}
			}
			else
			{
				throw new IncorrectDataException("Invalid Element Connections provided in ElementConfiguration object.");
			}

			
		}

		/// <summary>
		/// Determines whether an element with the specified DataMiner Agent ID/element ID exists on this DataMiner Agent.
		/// </summary>
		/// <param name="dmsElementId">The DataMiner Agent ID/element ID of the element.</param>
		/// <returns><c>true</c> if the element exists on this DataMiner Agent; otherwise, <c>false</c>.</returns>
		/// <exception cref="ArgumentException"><paramref name="dmsElementId"/> is invalid.</exception>
		public bool ElementExists(DmsElementId dmsElementId)
		{
			bool exists = false;

			try
			{
				ElementInfoEventMessage response = GetElementSLNet(dmsElementId);

				if (response != null && response.HostingAgentID == Id)
				{
					exists = true;
				}
			}
			catch (ElementNotFoundException)
			{
				exists = false;
			}

			return exists;
		}

		/// <summary>
		/// Determines whether an element with the specified name exists on this DataMiner Agent.
		/// </summary>
		/// <param name="elementName">The name of the element.</param>
		/// <returns><c>true</c> if the element exists on this DataMiner Agent; otherwise, <c>false</c>.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="elementName"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException"><paramref name="elementName"/> is the empty string ("") or white space.</exception>
		public bool ElementExists(string elementName)
		{
			bool exists = false;

			try
			{
				var response = GetElementSLNet(elementName);

				if (response != null && response.HostingAgentID == Id)
				{
					exists = true;
				}
			}
			catch (ElementNotFoundException)
			{
				exists = false;
			}

			return exists;
		}

		/// <summary>
		/// Determines whether this DataMiner Agent exists in the DataMiner System.
		/// </summary>
		/// <returns><c>true</c> if the DataMiner Agent exists in the DataMiner System; otherwise, <c>false</c>.</returns>
		public override bool Exists()
		{
			return dms.AgentExists(id);
		}

		/// <summary>
		/// Retrieves the element with the specified the DataMiner Agent ID and element ID from this DataMiner Agent.
		/// </summary>
		/// <param name="dmsElementId">The DataMiner Agent ID/element ID of the element.</param>
		/// <exception cref="ArgumentException"><paramref name="dmsElementId"/> is invalid.</exception>
		/// <exception cref="ElementNotFoundException">The element was not found on this DataMiner Agent.</exception>
		/// <returns>The element with the specified DataMiner Agent ID and element ID.</returns>
		public IDmsElement GetElement(DmsElementId dmsElementId)
		{
			IDmsElement element = dms.GetElement(dmsElementId);

			if (element.Host.Id != id)
			{
				throw new ElementNotFoundException(dmsElementId);
			}

			return element;
		}

		/// <summary>
		/// Retrieves the element with the specified name from this DataMiner Agent.
		/// </summary>
		/// <param name="elementName">The name of the element.</param>
		/// <exception cref="ArgumentNullException"><paramref name="elementName"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException"><paramref name="elementName"/> is the empty string ("") or white space.</exception>
		/// <exception cref="ElementNotFoundException">The element with the specified name was not found on this DataMiner Agent.</exception>
		/// <returns>The element with the specified element name.</returns>
		public IDmsElement GetElement(string elementName)
		{
			IDmsElement element = dms.GetElement(elementName);

			if (element.Host.Id != id)
			{
				throw new ElementNotFoundException(String.Format(CultureInfo.InvariantCulture, "The element with name {0} was not found on this DataMiner agent.", elementName));
			}

			return element;
		}

		/// <summary>
		/// Retrieves all elements present on this DataMiner Agent.
		/// </summary>
		/// <returns>The elements present on this DataMiner Agent.</returns>
		public ICollection<IDmsElement> GetElements()
		{
			List<IDmsElement> elements = new List<IDmsElement>();

			GetInfoMessage message = new GetInfoMessage
			{
				DataMinerID = id,
				Type = InfoType.ElementInfo
			};

			DMSMessage[] responses = dms.Communication.SendMessage(message);

			foreach (DMSMessage response in responses)
			{
				ElementInfoEventMessage elementInfo = (ElementInfoEventMessage)response;

				try
				{
					DmsElement element = new DmsElement(dms, elementInfo);

					if (element.DmsElementId.AgentId != -1 && element.Id != -1 && element.Host.Id == id)
					{
						elements.Add(element);
					}
				}
				catch (Exception ex)
				{
					string logMessage = "Failed parsing element info for element " + Convert.ToString(elementInfo.Name) + " (" + Convert.ToString(elementInfo.DataMinerID) + "/" + Convert.ToString(elementInfo.ElementID) + ")." + Environment.NewLine + ex;
					Logger.Log(logMessage);
				}
				
			}

			return elements;
		}

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns>A string that represents the current object.</returns>
		public override string ToString()
		{
			return String.Format(CultureInfo.InvariantCulture, "DataMiner agent ID: {0}", id);
		}

		internal override void Load()
		{
			try
			{
				GetDataMinerByIDMessage message = new GetDataMinerByIDMessage(id);
				GetDataMinerInfoResponseMessage infoResponseMessage = Dms.Communication.SendSingleResponseMessage(message) as GetDataMinerInfoResponseMessage;

				if (infoResponseMessage != null)
				{
					Parse(infoResponseMessage);
				}
				else
				{
					throw new AgentNotFoundException(id);
				}

				GetAgentBuildInfo buildInfoMessage = new GetAgentBuildInfo(id);
				BuildInfoResponse buildInfoResponse = (BuildInfoResponse)Dms.Communication.SendSingleResponseMessage(buildInfoMessage);

				if(buildInfoResponse != null)
				{
					Parse(buildInfoResponse);
				}

				RSAPublicKeyRequest rsapkr;
				rsapkr = new RSAPublicKeyRequest(id)
				{
					HostingDataMinerID = id
				};

				RSAPublicKeyResponse resp = Dms.Communication.SendSingleResponseMessage(rsapkr) as RSAPublicKeyResponse;
				RSA.PublicKey = new RSAParameters
				{
					Modulus = resp.Modulus,
					Exponent = resp.Exponent
				};

				IsLoaded = true;
			}
			catch (DataMinerException e)
			{
				if (e.ErrorCode == -2146233088)
				{
					// 0x80131500, No agent available with ID.
					throw new AgentNotFoundException(id);
				}
				else
				{
					throw;
				}
			}
		}

		/// <summary>
		/// Gets the element via SLNet.
		/// </summary>
		/// <param name="dmsElementId">The DataMiner agent ID/element ID of the element.</param>
		/// <exception cref="ArgumentException"><paramref name="dmsElementId"/> is invalid.</exception>
		/// <exception cref="ElementNotFoundException">The element was not found on this DataMiner agent.</exception>
		/// <returns>The ElementInfoEventMessage response.</returns>
		private ElementInfoEventMessage GetElementSLNet(DmsElementId dmsElementId)
		{
			int dmaId = dmsElementId.AgentId;
			int elementId = dmsElementId.ElementId;

			if (dmaId < 1)
			{
				throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, "Invalid DataMiner agent ID: {0}", dmaId), "dmsElementId");
			}

			if (elementId < 1)
			{
				throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, "Invalid DataMiner element ID: {0}", elementId), "dmsElementId");
			}

			try
			{
				GetElementByIDMessage message = new GetElementByIDMessage(dmaId, elementId);
				ElementInfoEventMessage response = (ElementInfoEventMessage)Dms.Communication.SendSingleResponseMessage(message);

				return response;
			}
			catch (DataMinerException e)
			{
				// 0x80131500
				if (e.ErrorCode == -2146233088)
				{
					throw new ElementNotFoundException(dmsElementId);
				}
				else
				{
					throw;
				}
			}
		}

		/// <summary>
		/// Retrieves the element with the specified element name.
		/// </summary>
		/// <param name="elementName">The name of the element.</param>
		/// <exception cref="ArgumentNullException"><paramref name="elementName"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException"><paramref name="elementName"/> is the empty string ("") or white space.</exception>
		/// <exception cref="ElementNotFoundException">No element with the specified name was found on this DataMiner agent.</exception>
		/// <returns>The element with the specified name.</returns>
		private ElementInfoEventMessage GetElementSLNet(string elementName)
		{
			if (elementName == null)
			{
				throw new ArgumentNullException("elementName");
			}

			if (String.IsNullOrWhiteSpace(elementName))
			{
				throw new ArgumentException("The element name is the empty string (\"\") or white space.", "elementName");
			}

			try
			{
				GetElementByNameMessage message = new GetElementByNameMessage(elementName);
				ElementInfoEventMessage response = (ElementInfoEventMessage)Dms.Communication.SendSingleResponseMessage(message);

				return response;
			}
			catch (DataMinerException e)
			{
				if (e.ErrorCode == -2146233088)
				{
					// 0x80131500, Element "[element name]" is unavailable.
					throw new ElementNotFoundException(elementName);
				}
				else
				{
					throw;
				}
			}
		}

		private void Parse(GetDataMinerInfoResponseMessage infoMessage)
		{
			name = infoMessage.AgentName;
			hostName = infoMessage.ComputerName;
		}

		/// <summary>
		/// Parses the version information of the DataMiner Agent.
		/// </summary>
		/// <param name="response"></param>
		private void Parse(BuildInfoResponse response)
		{
			if (response == null || response.Agents == null || response.Agents.Length == 0)
			{
				throw new ArgumentException("Agent build information cannot be null or empty");
			}

			string rawVersion = response.Agents[0].RawVersion;
			this.versionInfo = rawVersion;
		}

		/// <summary>
		/// Verifies if the provided version number is higher then the DataMiner Agent version.
		/// </summary>
		/// <param name="versionNumber">The version number to compare against the version of the DMA.</param>
		/// <returns></returns>
		public bool IsVersionHigher(string versionNumber)
		{
			bool isHigher = false;

			LoadOnDemand();

			Int32[] dmaVersionParts = ParseVersionNumbers(versionInfo);
			Int32[] versionParts = ParseVersionNumbers(versionNumber);

			for(int i=0;i<4;i++)
			{
				int dmaNumber = dmaVersionParts[i];
				int inputNumber = versionParts[i];
				if (inputNumber > dmaNumber)
				{
					isHigher = true;
					return isHigher;
				}
			}

			return isHigher;
		}

		/// <summary>
		/// parses the version number string into a string array.
		/// </summary>
		/// <param name="versionNumber">the version number.</param>
		/// <returns>string array containing the parsed version number.</returns>
		/// <exception cref="ArgumentException">When the version number is not in the expected format of a.b.c.d where a,b,c and d are integers.</exception>
		private static Int32[] ParseVersionNumbers(string versionNumber)
		{
			string[] splitDot = new[] { "." };
			string[] versionParts = versionNumber.Split(splitDot, StringSplitOptions.None);
			if (versionParts.Length != 4)
			{
				throw new ArgumentException("versionNumber is not in expected format.");
			}

			int versionPartMajor = 0;
			int versionPartMinor = 0;
			int versionPartMonth = 0;
			int versionPartWeek = 0;

			if (!Int32.TryParse(versionParts[0], out versionPartMajor) ||
				!Int32.TryParse(versionParts[2], out versionPartMonth) ||
				!Int32.TryParse(versionParts[1], out versionPartMinor) ||
				!Int32.TryParse(versionParts[3], out versionPartWeek))
			{
				throw new ArgumentException("versionNumber is not in expected format.");
			}

			Int32[] versionPartNumbers = new Int32[4];
			versionPartNumbers[0] = versionPartMajor;
			versionPartNumbers[1] = versionPartMinor;
			versionPartNumbers[2] = versionPartMonth;
			versionPartNumbers[3] = versionPartWeek;

			return versionPartNumbers;
		}
	}
}