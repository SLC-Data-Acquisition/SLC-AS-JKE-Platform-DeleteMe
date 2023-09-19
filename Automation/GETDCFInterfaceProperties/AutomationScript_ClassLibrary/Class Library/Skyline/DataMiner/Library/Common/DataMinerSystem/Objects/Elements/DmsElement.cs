namespace Skyline.DataMiner.Library.Common
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Globalization;
	using System.Linq;
	using System.Text;

	using Net.Exceptions;
	using Net.Messages;
	using Net.Messages.Advanced;

	using Properties;

	using Templates;

	/// <summary>
	/// Represents a DataMiner element.
	/// </summary>
	internal class DmsElement : DmsObject, IDmsElement
	{
		// Keep this message in case we need to parse the element properties when the user wants to use these.
		private ElementInfoEventMessage elementInfo;

		/// <summary>
		/// Specifies whether the properties of the elementInfo object have been parsed into dedicated objects.
		/// </summary>
		private bool propertiesLoaded;

		/// <summary>
		/// Contains the properties for the element.
		/// </summary>
		private readonly IDictionary<string, DmsElementProperty> properties = new Dictionary<string, DmsElementProperty>();

		/// <summary>
		/// A set of all updated properties.
		/// </summary>
		private readonly HashSet<string> updatedProperties = new HashSet<string>();

		/// <summary>
		/// Array of views where the element is contained in.
		/// </summary>
		private readonly ISet<IDmsView> views = new DmsViewSet();

		/// <summary>
		/// This list will be used to keep track of which views were assigned / removed during the life time of the element.
		/// </summary>
		private readonly List<int> registeredViewIds = new List<int>();

		/// <summary>
		/// The advanced settings.
		/// </summary>
		private AdvancedSettings advancedSettings;

		/// <summary>
		/// The device settings.
		/// </summary>
		private DeviceSettings deviceSettings;

		/// <summary>
		/// The DVE settings.
		/// </summary>
		private DveSettings dveSettings;

		/// <summary>
		/// The failover settings.
		/// </summary>
		private FailoverSettings failoverSettings;

		/// <summary>
		/// The general settings.
		/// </summary>
		private GeneralSettings generalSettings;

		/// <summary>
		/// The redundancy settings.
		/// </summary>
		private RedundancySettings redundancySettings;

		/// <summary>
		/// The replication settings.
		/// </summary>
		private ReplicationSettings replicationSettings;

		/// <summary>
		/// The element components.
		/// </summary>
		private IList<ElementSettings> settings;

		/// <summary>
		/// Specifies whether the views have been loaded.
		/// </summary>
		private bool viewsLoaded;

		/// <summary>
		/// Collection of connections available on the element.
		/// </summary>
		private ElementConnectionCollection elementCommunicationConnections;

		/// <summary>
		/// Initializes a new instance of the <see cref="DmsElement"/> class.
		/// </summary>
		/// <param name="dms">Object implementing <see cref="IDms"/> interface.</param>
		/// <param name="dmsElementId">The system-wide element ID.</param>
		/// <exception cref="ArgumentNullException"><paramref name="dms"/> is <see langword="null"/>.</exception>
		internal DmsElement(IDms dms, DmsElementId dmsElementId) :
			base(dms)
		{
			Initialize();
			generalSettings.DmsElementId = dmsElementId;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DmsElement"/> class.
		/// </summary>
		/// <param name="dms">Object implementing the <see cref="IDms"/> interface.</param>
		/// <param name="elementInfo">The element information.</param>
		/// <exception cref="ArgumentNullException"><paramref name="dms"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="elementInfo"/> is <see langword="null"/>.</exception>
		internal DmsElement(IDms dms, ElementInfoEventMessage elementInfo)
			: base(dms)
		{
			if (elementInfo == null)
			{
				throw new ArgumentNullException("elementInfo");
			}

			Initialize(elementInfo);
			Parse(elementInfo);
		}

		/// <summary>
		/// Gets the advanced settings of this element.
		/// </summary>
		/// <value>The advanced settings of this element.</value>
		public IAdvancedSettings AdvancedSettings
		{
			get
			{
				return advancedSettings;
			}
		}

		/// <summary>
		/// Gets the DataMiner Agent ID.
		/// </summary>
		/// <value>The DataMiner Agent ID.</value>
		public int AgentId
		{
			get
			{
				return generalSettings.DmsElementId.AgentId;
			}
		}

		/// <summary>
		/// Gets or sets the alarm template assigned to this element.
		/// </summary>
		/// <value>The alarm template assigned to this element.</value>
		/// <exception cref="ArgumentException">The specified alarm template is not compatible with the protocol this element executes.</exception>
		public IDmsAlarmTemplate AlarmTemplate
		{
			get
			{
				return generalSettings.AlarmTemplate;
			}

			set
			{
				if (!InputValidator.IsCompatibleTemplate(value, this.Protocol))
				{
					throw new ArgumentException("The specified alarm template is not compatible with the protocol this element executes.", "value");
				}
				else
				{
					generalSettings.AlarmTemplate = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the element description.
		/// </summary>
		/// <value>The element description.</value>
		public string Description
		{
			get
			{
				return GeneralSettings.Description;
			}

			set
			{
				GeneralSettings.Description = value;
			}
		}

		/// <summary>
		/// Gets the system-wide element ID of the element.
		/// </summary>
		/// <value>The system-wide element ID of the element.</value>
		public DmsElementId DmsElementId
		{
			get
			{
				return generalSettings.DmsElementId;
			}
		}

		/// <summary>
		/// Gets the DVE settings of this element.
		/// </summary>
		/// <value>The DVE settings of this element.</value>
		public IDveSettings DveSettings
		{
			get
			{
				return dveSettings;
			}
		}

		/// <summary>
		/// Gets the failover settings of this element.
		/// </summary>
		/// <value>The failover settings of this element.</value>
		public IFailoverSettings FailoverSettings
		{
			get
			{
				return failoverSettings;
			}
		}

		/// <summary>
		/// Gets the DataMiner Agent that hosts this element.
		/// </summary>
		/// <value>The DataMiner Agent that hosts this element.</value>
		public IDma Host
		{
			get
			{
				return generalSettings.Host;
			}
		}

		/// <summary>
		/// Gets the element ID.
		/// </summary>
		/// <value>The element ID.</value>
		public int Id
		{
			get
			{
				return generalSettings.DmsElementId.ElementId;
			}
		}

		/// <summary>
		/// Gets or sets the element name.
		/// </summary>
		/// <value>The element name.</value>
		/// <exception cref="ArgumentNullException">The value of a set operation is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException">The value of a set operation is empty or white space.</exception>
		/// <exception cref="ArgumentException">The value of a set operation exceeds 200 characters.</exception>
		/// <exception cref="ArgumentException">The value of a set operation contains a forbidden character.</exception>
		/// <exception cref="ArgumentException">The value of a set operation contains more than one '%' character.</exception>
		/// <exception cref="NotSupportedException">A set operation is not supported on a DVE child or a derived element.</exception>
		/// <remarks>
		/// <para>The following restrictions apply to element names:</para>
		/// <list type="bullet">
		///		<item><para>Names may not start or end with the following characters: '.' (dot), ' ' (space).</para></item>
		///		<item><para>Names may not contain the following characters: '\', '/', ':', '*', '?', '"', '&lt;', '&gt;', '|', '°', ';'.</para></item>
		///		<item><para>The following characters may not occur more than once within a name: '%' (percentage).</para></item>
		/// </list>
		/// </remarks>
		public string Name
		{
			get
			{
				return generalSettings.Name;
			}

			set
			{
				generalSettings.Name = InputValidator.ValidateName(value, "value");
			}
		}

		/// <summary>
		/// Gets the properties of this element.
		/// </summary>
		/// <value>The element properties.</value>
		public IPropertyCollection<IDmsElementProperty, IDmsElementPropertyDefinition> Properties
		{
			get
			{
				LoadOnDemand();

				// Parse properties using definitions from Dms.
				if(!propertiesLoaded)
				{
					ParseElementProperties();
				}

				IDictionary<string, IDmsElementProperty> copy = new Dictionary<string, IDmsElementProperty>(properties.Count);

				foreach (KeyValuePair<string, DmsElementProperty> kvp in properties)
				{
					copy.Add(kvp.Key, kvp.Value);
				}

				return new PropertyCollection<IDmsElementProperty, IDmsElementPropertyDefinition>(copy);
			}
		}

		/// <summary>
		/// Gets the protocol executed by this element.
		/// </summary>
		/// <value>The protocol executed by this element.</value>
		public IDmsProtocol Protocol
		{
			get
			{
				return generalSettings.Protocol;
			}
		}

		/// <summary>
		/// Gets the redundancy settings.
		/// </summary>
		/// <value>The redundancy settings.</value>
		public IRedundancySettings RedundancySettings
		{
			get
			{
				return redundancySettings;
			}
		}

		/// <summary>
		/// Gets the replication settings.
		/// </summary>
		/// <value>The replication settings.</value>
		public IReplicationSettings ReplicationSettings
		{
			get
			{
				return replicationSettings;
			}
		}

		/// <summary>
		/// Gets the element state.
		/// </summary>
		/// <value>The element state.</value>
		public ElementState State
		{
			get
			{
				return GeneralSettings.State;
			}

			internal set
			{
				GeneralSettings.State = value;
			}
		}

		/// <summary>
		/// Gets or sets the trend template that is assigned to this element.
		/// </summary>
		/// <value>The trend template that is assigned to this element.</value>
		/// <exception cref="ArgumentException">The specified trend template is not compatible with the protocol this element executes.</exception>
		public IDmsTrendTemplate TrendTemplate
		{
			get
			{
				return generalSettings.TrendTemplate;
			}

			set
			{
				if (!InputValidator.IsCompatibleTemplate(value, this.Protocol))
				{
					throw new ArgumentException("The specified trend template is not compatible with the protocol this element executes.", "value");
				}
				else
				{
					generalSettings.TrendTemplate = value;
				}
			}
		}

		/// <summary>
		/// Gets the type of the element.
		/// </summary>
		/// <value>The element type.</value>
		public string Type
		{
			get
			{
				return deviceSettings.Type;
			}
		}

		/// <summary>
		/// Gets the views the element is part of.
		/// </summary>
		/// <value>The views the element is part of.</value>
		/// <exception cref="ArgumentNullException">The value of a set operation is <see langword = "null" />.</exception>
		/// <exception cref="ArgumentException">The value of a set operation is an empty collection.</exception>
		public ISet<IDmsView> Views
		{
			get
			{
				if (!viewsLoaded)
				{
					LoadViews();
				}

				return views;
			}
		}

		/// <summary>
		/// Gets the general settings of the element.
		/// </summary>
		internal GeneralSettings GeneralSettings
		{
			get
			{
				return generalSettings;
			}
		}

		/// <summary>
		/// Gets or sets the available connections on the element.
		/// </summary>
		public ElementConnectionCollection Connections
		{
			get { return elementCommunicationConnections; }
			set { elementCommunicationConnections = value; }
		}

		/// <summary>
		/// Deletes the element.
		/// </summary>
		/// <exception cref="ElementNotFoundException">The element was not found in the DataMiner System.</exception>
		/// <exception cref="NotSupportedException">The element is a DVE child or a derived element.</exception>
		public void Delete()
		{
			if (DveSettings.IsChild)
			{
				throw new NotSupportedException("Deleting a DVE child is not supported.");
			}

			if (RedundancySettings.IsDerived)
			{
				throw new NotSupportedException("Deleting a derived element is not supported.");
			}

			ChangeElementState(ElementState.Deleted);
		}

		/// <summary>
		/// Duplicates the element.
		/// </summary>
		/// <param name="newElementName">The name to assign to the duplicated element.</param>
		/// <param name="agent">The target DataMiner Agent where the duplicated element should be created.</param>
		/// <exception cref = "ArgumentNullException"><paramref name="newElementName"/> is <see langword = "null" />.</exception>
		/// <exception cref="ArgumentException"><paramref name="newElementName"/> is invalid.</exception>
		/// <exception cref="NotSupportedException">The element is a DVE child or a derived element.</exception>
		/// <exception cref="AgentNotFoundException">The specified DataMiner Agent was not found in the DataMiner System.</exception>
		/// <exception cref="ElementNotFoundException">The element was not found in the DataMiner System.</exception>
		/// <exception cref="IncorrectDataException">Invalid data.</exception>
		/// <returns>The duplicated element.</returns>
		public IDmsElement Duplicate(string newElementName, IDma agent = null)
		{
			string trimmedName = InputValidator.ValidateName(newElementName, "newElementName");

			if (String.Equals(newElementName, Name, StringComparison.OrdinalIgnoreCase))
			{
				throw new ArgumentException("The new element name cannot be equal to the name of the element being duplicated.", "newElementName");
			}

			if (DveSettings.IsChild)
			{
				throw new NotSupportedException("Duplicating a DVE child is not supported.");
			}

			if (RedundancySettings.IsDerived)
			{
				throw new NotSupportedException("Duplicating a derived element is not supported.");
			}

			try
			{
				bool isCompatibilityIssueDetected = false;
				if (agent == null)
				{
					IDma targetDma = new Dma(Dms, this.DmsElementId.AgentId);
					isCompatibilityIssueDetected = targetDma.IsVersionHigher(Dma.snmpV3AuthenticationChangeDMAVersion);
				}
				else
				{
					isCompatibilityIssueDetected = agent.IsVersionHigher(Dma.snmpV3AuthenticationChangeDMAVersion);
				}

				AddElementMessage configuration = HelperClass.CreateAddElementMessage(Dms, this,isCompatibilityIssueDetected);
				configuration.DataMinerID = agent == null ? configuration.DataMinerID : agent.Id;
				configuration.ElementName = trimmedName;
				configuration.ElementID = -1;

				AddElementResponseMessage response = (AddElementResponseMessage)Dms.Communication.SendSingleResponseMessage(configuration);
				int elementId = response.NewID;

				return new DmsElement(dms, new DmsElementId(configuration.DataMinerID, elementId));
			}
			catch (DataMinerCommunicationException e)
			{
				if (e.ErrorCode == -2147220787)
				{
					// 0x800402CD, SL_NO_CONNECTION.
					int agentId = agent == null ? DmsElementId.AgentId : agent.Id;
					throw new AgentNotFoundException(String.Format(CultureInfo.InvariantCulture, "No DataMiner agent with ID '{0}' was found in the DataMiner system.", agentId), e);
				}
				else
				{
					throw;
				}
			}
			catch (DataMinerException e)
			{
				if (e.ErrorCode == -2147220959)
				{
					// 0x80040221, SL_INVALID_DATA, "Invalid data".
					if (agent == null)
					{
						string message = String.Format(CultureInfo.InvariantCulture, "Invalid data - element: '{0}', new element name: {1}", DmsElementId.Value, newElementName);
						throw new IncorrectDataException(message);
					}
					else
					{
						string message = String.Format(CultureInfo.InvariantCulture, "Invalid data - element: '{0}', new element name: {1}, agent: {2}", DmsElementId.Value, newElementName, agent.Id);
						throw new IncorrectDataException(message);
					}
				}
				else
				{
					throw;
				}
			}
		}

		/// <summary>
		/// Determines whether this DataMiner element exists in the DataMiner System.
		/// </summary>
		/// <returns><c>true</c> if the DataMiner element exists in the DataMiner System; otherwise, <c>false</c>.</returns>
		public override bool Exists()
		{
			return Dms.ElementExists(DmsElementId);
		}

		/// <summary>
		/// Gets the number of active alarms.
		/// </summary>
		/// <exception cref="ElementStoppedException">The element is stopped.</exception>
		/// <exception cref="ElementNotFoundException">The element was not found in the DataMiner System.</exception>
		/// <returns>The number of active alarms.</returns>
		public int GetActiveAlarmCount()
		{
			DmsStandaloneParameter<int> activeAlarmCountParameter = new DmsStandaloneParameter<int>(this, 65003);

			return activeAlarmCountParameter.GetValue();
		}

		/// <summary>
		/// Gets the alarm level of the element.
		/// </summary>
		/// <returns>The element alarm level.</returns>
		/// <exception cref="ElementStoppedException">The element is stopped.</exception>
		/// <exception cref="ElementNotFoundException">The element was not found in the DataMiner System.</exception>
		public AlarmLevel GetAlarmLevel()
		{
			// Alarm state is found at 650008.
			DmsStandaloneParameter<int> alarmStateParameter = new DmsStandaloneParameter<int>(this, 65008);
			int alarmState = alarmStateParameter.GetValue();

			return (AlarmLevel)alarmState;
		}

		/// <summary>
		/// Gets the number of critical alarms.
		/// </summary>
		/// <exception cref="ElementStoppedException">The element is stopped.</exception>
		/// <exception cref="ElementNotFoundException">The element was not found in the DataMiner System.</exception>
		/// <returns>The number of critical alarms.</returns>
		public int GetActiveCriticalAlarmCount()
		{
			DmsStandaloneParameter<int> criticalAlarmCountParameter = new DmsStandaloneParameter<int>(this, 65004);

			return criticalAlarmCountParameter.GetValue();
		}

		/// <summary>
		/// Gets the number of major alarms.
		/// </summary>
		/// <exception cref="ElementStoppedException">The element is stopped.</exception>
		/// <exception cref="ElementNotFoundException">The element was not found in the DataMiner System.</exception>
		/// <returns>The number of major alarms.</returns>
		public int GetActiveMajorAlarmCount()
		{
			DmsStandaloneParameter<int> majorAlarmCountParameter = new DmsStandaloneParameter<int>(this, 65005);

			return majorAlarmCountParameter.GetValue();
		}

		/// <summary>
		/// Gets the number of minor alarms.
		/// </summary>
		/// <exception cref="ElementStoppedException">The element is stopped.</exception>
		/// <exception cref="ElementNotFoundException">The element was not found in the DataMiner System.</exception>
		/// <returns>The number of minor alarms.</returns>
		public int GetActiveMinorAlarmCount()
		{
			DmsStandaloneParameter<int> minorAlarmCountParameter = new DmsStandaloneParameter<int>(this, 65006);

			return minorAlarmCountParameter.GetValue();
		}

		/// <summary>
		/// Gets the specified standalone parameter.
		/// </summary>
		/// <typeparam name="T">The type of the parameter. Currently supported types: int?, double?, DateTime? and string.</typeparam>
		/// <param name="parameterId">The parameter ID.</param>
		/// <exception cref="ArgumentException"><paramref name="parameterId"/> is invalid.</exception>
		/// <exception cref="ElementNotFoundException">The element was not found in the DataMiner System.</exception>
		/// <exception cref="ElementStoppedException">The element is stopped.</exception>
		/// <exception cref="NotSupportedException">A type other than string, int?, double? or DateTime? was provided.</exception>
		/// <returns>The standalone parameter that corresponds with the specified ID.</returns>
		public IDmsStandaloneParameter<T> GetStandaloneParameter<T>(int parameterId)
		{
			if (parameterId < 1)
			{
				throw new ArgumentException("Invalid parameter ID.", "parameterId");
			}

			Type type = typeof(T);

			if (type != typeof(string) && type != typeof(int?) && type != typeof(double?) && type != typeof(DateTime?))
			{
				throw new NotSupportedException("Only one of the following types is supported: string, int?, double? or DateTime?.");
			}

			HelperClass.CheckElementState(this);

			return new DmsStandaloneParameter<T>(this, parameterId);
		}

		/// <summary>
		/// Gets the specified table.
		/// </summary>
		/// <param name="tableId">The table parameter ID.</param>
		/// <exception cref="ArgumentException"><paramref name="tableId"/> is invalid.</exception>
		/// <exception cref="ElementNotFoundException">The element was not found in the DataMiner System.</exception>
		/// <exception cref="ElementStoppedException">The element is stopped.</exception>
		/// <returns>The table that corresponds with the specified ID.</returns>
		public IDmsTable GetTable(int tableId)
		{
			HelperClass.CheckElementState(this);

			if (tableId < 1)
			{
				throw new ArgumentException("Invalid table ID.", "tableId");
			}

			return new DmsTable(this, tableId);
		}

		/// <summary>
		/// Gets the number of warnings.
		/// </summary>
		/// <exception cref="ElementStoppedException">The element is stopped.</exception>
		/// <exception cref="ElementNotFoundException">The element was not found in the DataMiner System.</exception>
		/// <returns>The number of warnings.</returns>
		public int GetActiveWarningAlarmCount()
		{
			DmsStandaloneParameter<int> warningCountParameter = new DmsStandaloneParameter<int>(this, 65007);
			return warningCountParameter.GetValue();
		}

		/// <summary>
		/// Determines whether the element has been started up completely.
		/// </summary>
		/// <returns><c>true</c> if the element is started up; otherwise, <c>false</c>.</returns>
		/// <exception cref="ElementNotFoundException">The element was not found.</exception>
		/// <remarks>By default, the time-out value is set to 10s.
		/// This call should only be executed on elements that are in state Active.
		/// </remarks>
		public bool IsStartupComplete()
		{
			return NotifyElementStartupComplete();
		}

		/// <summary>
		/// Pauses the element.
		/// </summary>
		/// <exception cref="NotSupportedException">The element is a DVE child or derived element.</exception>
		/// <exception cref="ElementNotFoundException">The element was not found in the DataMiner System.</exception>
		public void Pause()
		{
			if (DveSettings.IsChild)
			{
				throw new NotSupportedException("Pausing a DVE child is not supported.");
			}

			if (RedundancySettings.IsDerived)
			{
				throw new NotSupportedException("Pausing a derived element is not supported.");
			}

			ChangeElementState(ElementState.Paused);
		}

		/// <summary>
		/// Restarts the element.
		/// </summary>
		/// <exception cref="NotSupportedException">The element is a DVE child or derived element.</exception>
		/// <exception cref="ElementNotFoundException">The element was not found in the DataMiner System.</exception>
		public void Restart()
		{
			if (DveSettings.IsChild)
			{
				throw new NotSupportedException("Pausing a DVE child is not supported.");
			}

			if (RedundancySettings.IsDerived)
			{
				throw new NotSupportedException("Pausing a derived element is not supported.");
			}

			ChangeElementState(ElementState.Restart);
		}

		/// <summary>
		/// Starts the element.
		/// </summary>
		/// <exception cref="NotSupportedException">The element is a DVE child or derived element.</exception>
		/// <exception cref="ElementNotFoundException">The element was not found in the DataMiner System.</exception>
		public void Start()
		{
			if (DveSettings.IsChild)
			{
				throw new NotSupportedException("Starting a DVE child is not supported.");
			}

			if (RedundancySettings.IsDerived)
			{
				throw new NotSupportedException("Starting a derived element is not supported.");
			}

			ChangeElementState(ElementState.Active);
		}

		/// <summary>
		/// Stops the element.
		/// </summary>
		/// <exception cref="NotSupportedException">The element is a DVE child or derived element.</exception>
		/// <exception cref="ElementNotFoundException">The element was not found in the DataMiner System.</exception>
		public void Stop()
		{
			if (DveSettings.IsChild)
			{
				throw new NotSupportedException("Stopping a DVE child is not supported.");
			}

			if (RedundancySettings.IsDerived)
			{
				throw new NotSupportedException("Stopping a derived element is not supported.");
			}

			ChangeElementState(ElementState.Stopped);
		}

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns>A string that represents the current object.</returns>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat(CultureInfo.InvariantCulture, "Name: {0}{1}", Name, Environment.NewLine);
			sb.AppendFormat(CultureInfo.InvariantCulture, "agent ID/element ID: {0}{1}", DmsElementId.Value, Environment.NewLine);
			sb.AppendFormat(CultureInfo.InvariantCulture, "Description: {0}{1}", Description, Environment.NewLine);
			sb.AppendFormat(CultureInfo.InvariantCulture, "Protocol name: {0}{1}", Protocol.Name, Environment.NewLine);
			sb.AppendFormat(CultureInfo.InvariantCulture, "Protocol version: {0}{1}", Protocol.Version, Environment.NewLine);
			sb.AppendFormat(CultureInfo.InvariantCulture, "Hosting agent ID: {0}{1}", Host.Id, Environment.NewLine);

			return sb.ToString();
		}

		/// <summary>
		/// Updates the element.
		/// </summary>
		/// <exception cref="IncorrectDataException">Invalid data was set.</exception>
		/// <exception cref="ElementNotFoundException">The element was not found in the DataMiner system.</exception>
		public void Update()
		{
			if (generalSettings.State == ElementState.Deleted)
			{
				throw new ElementNotFoundException(String.Format(CultureInfo.InvariantCulture, "The element with name {0} was not found on this DataMiner agent.", Name));
			}

			try
			{
				// Use this flag to see if we actually have to perform an update on the element.
				if (UpdateRequired())
				{
					if (ViewsRequireUpdate() && views.Count == 0)
					{
						throw new IncorrectDataException("Views must not be empty; an element must belong to at least one view.");
					}

					IDma targetDma = new Dma(Dms, this.DmsElementId.AgentId);
					bool isCompatibilityIssueDetected = targetDma.IsVersionHigher(Dma.snmpV3AuthenticationChangeDMAVersion);

					AddElementMessage message = CreateUpdateMessage(isCompatibilityIssueDetected);

					Communication.SendSingleResponseMessage(message);
					ClearChangeList();

					// Performed the update, so tell the element to refresh.
					IsLoaded = false;
					viewsLoaded = false;
					propertiesLoaded = false;
				}
			}
			catch (DataMinerException e)
			{
				IsLoaded = false;
				viewsLoaded = false;
				propertiesLoaded = false;

				if (!Exists())
				{
					generalSettings.State = ElementState.Deleted;
					throw new ElementNotFoundException(DmsElementId, e);
				}
				else
				{
					throw;
				}
			}
		}

		/// <summary>
		/// Loads all the data and properties found related to the element.
		/// </summary>
		/// <exception cref="ElementNotFoundException">The element was not found in the DataMiner system.</exception>
		internal override void Load()
		{
			try
			{
				IsLoaded = true;

				GetElementByIDMessage message = new GetElementByIDMessage(generalSettings.DmsElementId.AgentId, generalSettings.DmsElementId.ElementId);
				ElementInfoEventMessage response = (ElementInfoEventMessage)Communication.SendSingleResponseMessage(message);

				elementCommunicationConnections = new ElementConnectionCollection(response);
				Parse(response);
			}
			catch (DataMinerException e)
			{
				if (e.ErrorCode == -2146233088)
				{
					// 0x80131500, Element "[element ID]" is unavailable.
					throw new ElementNotFoundException(DmsElementId, e);
				}
				else
				{
					throw;
				}
			}
		}

		/// <summary>
		/// Loads all the views where this element is included.
		/// </summary>
		internal void LoadViews()
		{
			GetViewsForElementMessage message = new GetViewsForElementMessage
			{
				DataMinerID = generalSettings.DmsElementId.AgentId,
				ElementID = generalSettings.DmsElementId.ElementId
			};

			GetViewsForElementResponse response = (GetViewsForElementResponse)Communication.SendSingleResponseMessage(message);

			views.Clear();
			registeredViewIds.Clear();
			foreach (DataMinerObjectInfo info in response.Views)
			{
				DmsView view = new DmsView(dms, info.ID, info.Name);
				registeredViewIds.Add(info.ID);
				views.Add(view);
			}

			viewsLoaded = true;
		}

		/// <summary>
		/// Update the updataProperties HashSet with a change event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		internal void PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			updatedProperties.Add(e.PropertyName);
		}

		/// <summary>
		/// Parses all of the element info.
		/// </summary>
		/// <param name="elementInfo">The element info message.</param>
		internal void Parse(ElementInfoEventMessage elementInfo)
		{
			IsLoaded = true;

			try
			{
				ParseElementInfo(elementInfo);
			}
			catch
			{
				IsLoaded = false;
				throw;
			}
		}

		/// <summary>
		/// Clears all of the queued updated properties.
		/// </summary>
		private void ClearChangeList()
		{
			ChangedPropertyList.Clear();
			foreach (ElementSettings setting in settings)
			{
				setting.ClearUpdates();
			}

			updatedProperties.Clear();
			elementCommunicationConnections.Clear();

			// If the update passes, update the list of registered views for the element.
			registeredViewIds.Clear();
			registeredViewIds.AddRange(views.Select(v => v.Id));
		}

		/// <summary>
		/// Creates the AddElementMessage based on the current state of the object.
		/// </summary>
		/// <returns>The AddElementMessage.</returns>
		private AddElementMessage CreateUpdateMessage(bool isCompatibilityIssueDetected)
		{
			AddElementMessage message = new AddElementMessage
			{
				ElementID = DmsElementId.ElementId,
				DataMinerID = DmsElementId.AgentId
			};

			if (!dveSettings.IsChild)
			{
				message.ProtocolName = Protocol.Name;
				message.ProtocolVersion = Protocol.Version;
			}

			foreach (ElementSettings setting in settings)
			{
				if (setting.Updated)
				{
					setting.FillUpdate(message);
				}
			}

			ElementPortInfo[] elementPortInfos = HelperClass.ObtainElementPortInfos(elementInfo);
			this.elementCommunicationConnections.UpdatePortInfo(elementPortInfos, isCompatibilityIssueDetected);
			message.Ports = elementPortInfos;

			if (ViewsRequireUpdate())
			{
				message.ViewIDs = views.Select(v => v.Id).ToArray();
			}

			List<PropertyInfo> newPropertyValues = new List<PropertyInfo>();
			foreach (string propertyName in updatedProperties)
			{
				DmsElementProperty property = properties[propertyName];
				newPropertyValues.Add(new PropertyInfo
				{
					DataType = "Element",
					Value = property.Value,
					Name = property.Definition.Name,
					AccessType = PropertyAccessType.ReadWrite
				});
			}

			if (newPropertyValues.Count > 0)
			{
				message.Properties = newPropertyValues.ToArray();
			}

			return message;
		}

		/// <summary>
		/// Initializes the element.
		/// </summary>
		private void Initialize(ElementInfoEventMessage elementInfo)
		{
			this.elementInfo = elementInfo;

			generalSettings = new GeneralSettings(this);
			deviceSettings = new DeviceSettings(this);
			replicationSettings = new ReplicationSettings(this);
			advancedSettings = new AdvancedSettings(this);
			failoverSettings = new FailoverSettings(this);
			redundancySettings = new RedundancySettings(this);
			dveSettings = new DveSettings(this);
			elementCommunicationConnections = new ElementConnectionCollection(this.elementInfo);

			settings = new List<ElementSettings> { generalSettings, deviceSettings, replicationSettings, advancedSettings, failoverSettings, redundancySettings, dveSettings };
		}

		/// <summary>
		/// Initializes the element.
		/// </summary>
		private void Initialize()
		{
			generalSettings = new GeneralSettings(this);
			deviceSettings = new DeviceSettings(this);
			replicationSettings = new ReplicationSettings(this);
			advancedSettings = new AdvancedSettings(this);
			failoverSettings = new FailoverSettings(this);
			redundancySettings = new RedundancySettings(this);
			dveSettings = new DveSettings(this);

			settings = new List<ElementSettings> { generalSettings, deviceSettings, replicationSettings, advancedSettings, failoverSettings, redundancySettings, dveSettings };
		}

		/// <summary>
		/// Performs a Notify type 377 call.
		/// </summary>
		/// <exception cref="ElementNotFoundException">The element was not found.</exception>
		/// <returns><c>true</c> if startup is complete; otherwise, <c>false</c>.</returns>
		private bool NotifyElementStartupComplete()
		{
			bool startupComplete = false;

			try
			{
				SetDataMinerInfoMessage message = new SetDataMinerInfoMessage
				{
					Uia1 = new UIA(new[] { (uint)AgentId, (uint)Id }),
					Uia2 = null,
					What = 377
				};

				SetDataMinerInfoResponseMessage response = (SetDataMinerInfoResponseMessage)Communication.SendSingleResponseMessage(message);

				if (response != null)
				{
					object result = response.RawData;

					if (result != null)
					{
						startupComplete = Convert.ToBoolean(result, CultureInfo.InvariantCulture);
					}
				}
			}
			catch (DataMinerException e)
			{
				if (e.ErrorCode == -2147220718)
				{   // 0x80040312, Unknown destination DataMiner specified.
					throw new ElementNotFoundException(DmsElementId, e);
				}
				else
				{
					// -2147220916, 0x8004024C, SL_NO_SUCH_ELEMENT, "The element is unknown." 
					// Note: When element is stopped it will throw 0x8004024 SL_NO_SUCH_ELEMENT (-2147220916).
					return false;
				}
			}

			return startupComplete;
		}

		/// <summary>
		/// Parses the element info.
		/// </summary>
		/// <param name="elementInfo">The element info.</param>
		private void ParseElementInfo(ElementInfoEventMessage elementInfo)
		{
			// Keep this object in case properties are accessed.
			this.elementInfo = elementInfo;

			foreach (ElementSettings component in settings)
			{
				component.Load(elementInfo);
			}

			ParseConnections(elementInfo);
		}

		/// <summary>
		/// Parse an ElementInfoEventMessage object.
		/// </summary>
		/// <param name="elementInfo"></param>
		private void ParseConnections(ElementInfoEventMessage elementInfo)
		{
			// Keep this object in case properties are accessed.
			this.elementInfo = elementInfo;

			ParseConnection(elementInfo.MainPort);

			if (elementInfo.ExtraPorts != null)
			{
				foreach (ElementPortInfo info in elementInfo.ExtraPorts)
				{
					ParseConnection(info);
				}
			}


		}

		/// <summary>
		/// Parse an ElementPortInfo object in order to add IElementConnection objects to the ElementConnectionCollection.
		/// </summary>
		/// <param name="info">The ElementPortInfo object.</param>
		private void ParseConnection(ElementPortInfo info)
		{
			switch (info.ProtocolType)
			{
				case ProtocolType.Virtual:
					VirtualConnection myVirtualConnection = new VirtualConnection(info);
					elementCommunicationConnections[info.PortID]=myVirtualConnection;
					break;
				case ProtocolType.SnmpV1:
					SnmpV1Connection mySnmpV1Connection = new SnmpV1Connection(info);
					elementCommunicationConnections[info.PortID] = mySnmpV1Connection;
					break;
				case ProtocolType.SnmpV2:
					SnmpV2Connection mySnmpv2Connection = new SnmpV2Connection(info);
					elementCommunicationConnections[info.PortID] = mySnmpv2Connection;
					break;
				case ProtocolType.SnmpV3:
					SnmpV3Connection mySnmpV3Connection = new SnmpV3Connection(info);
					elementCommunicationConnections[info.PortID] = mySnmpV3Connection;
					break;
				default:
					RealConnection myConnection = new RealConnection(info);
					elementCommunicationConnections[info.PortID] = myConnection;
					break;
			}
		}

		/// <summary>
		/// Parses the element properties.
		/// </summary>
		private void ParseElementProperties()
		{
			properties.Clear();
			foreach (IDmsElementPropertyDefinition definition in Dms.ElementPropertyDefinitions)
			{
				PropertyInfo info = null;
				if (elementInfo.Properties != null)
				{
					info = elementInfo.Properties.FirstOrDefault(p => p.Name.Equals(definition.Name, StringComparison.OrdinalIgnoreCase));

					List<String> duplicates = elementInfo.Properties.GroupBy(p => p.Name)
							.Where(g => g.Count() > 1)
							.Select(g => g.Key)
							.ToList();

					if (duplicates.Count > 0)
					{
						string message = "Duplicate element properties detected. Element \"" + elementInfo.Name + "\" (" + elementInfo.DataMinerID + "/" + elementInfo.ElementID + "), duplicate properties: " + String.Join(", ", duplicates) + ".";
						Logger.Log(message);
					}
				}

				string propertyValue = info != null ? info.Value : String.Empty;

				if (definition.IsReadOnly)
				{
					properties.Add(definition.Name, new DmsElementProperty(this, definition, propertyValue));
				}
				else
				{
					var property = new DmsWritableElementProperty(this, definition, propertyValue);
					properties.Add(definition.Name, property);

					property.PropertyChanged += this.PropertyChanged;
				}
			}

			propertiesLoaded = true;
		}

		/// <summary>
		/// Specifies if the element requires an update or not.
		/// </summary>
		/// <returns><c>true</c> if an update is required; otherwise, <c>false</c>.</returns>
		private bool UpdateRequired()
		{
			bool settingsChanged = settings.Any(s => s.Updated) || updatedProperties.Count != 0 || ChangedPropertyList.Count != 0 || ViewsRequireUpdate();
			bool connectionInfoChanged = elementCommunicationConnections.IsUpdateRequired();

			return settingsChanged || connectionInfoChanged;
		}

		/// <summary>
		/// Specifies if the views of the element have been updated.
		/// </summary>
		/// <returns><c>true</c> if the views have been updated; otherwise, <c>false</c>.</returns>
		private bool ViewsRequireUpdate()
		{
			if (views.Count != registeredViewIds.Count)
			{
				return true;
			}
			else
			{
				List<int> ids = views.Select(t => t.Id).ToList();

				IEnumerable<int> distinctOne = ids.Except(registeredViewIds);
				IEnumerable<int> distinctTwo = registeredViewIds.Except(ids);

				return distinctOne.Any() || distinctTwo.Any();
			}
		}

		/// <summary>
		/// Changes the state of an element.
		/// </summary>
		/// <param name="newState">Specifies the state that should be assigned to the element.</param>
		/// <exception cref="ElementNotFoundException">The element was not found in the DataMiner system.</exception>
		private void ChangeElementState(ElementState newState)
		{
			if (generalSettings.State == ElementState.Deleted)
			{
				throw new ElementNotFoundException(DmsElementId);
			}

			try
			{
				SetElementStateMessage message = new SetElementStateMessage
				{
					BState = false,
					DataMinerID = generalSettings.DmsElementId.AgentId,
					ElementId = generalSettings.DmsElementId.ElementId,
					State = (Net.Messages.ElementState)newState
				};

				Communication.SendMessage(message);

				// Set the value in the element.
				generalSettings.State = newState == ElementState.Restart ? ElementState.Active : newState;
			}
			catch (DataMinerException e)
			{
				if (!Exists())
				{
					generalSettings.State = ElementState.Deleted;
					throw new ElementNotFoundException(DmsElementId, e);
				}
				else
				{
					throw;
				}
			}
		}
	}
}