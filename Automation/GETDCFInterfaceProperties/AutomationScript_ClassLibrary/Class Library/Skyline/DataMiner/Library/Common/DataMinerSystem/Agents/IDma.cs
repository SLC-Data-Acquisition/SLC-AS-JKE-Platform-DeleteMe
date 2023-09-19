namespace Skyline.DataMiner.Library.Common
{
	using System;
	using System.Collections.Generic;
	using System.Net;

	/// <summary>
	/// DataMiner Agent interface.
	/// </summary>
	public interface IDma
	{
		/// <summary>
		/// Gets the DataMiner System this Agent is part of.
		/// </summary>
		/// <value>The DataMiner system this Agent is part of.</value>
		IDms Dms { get; }

		/// <summary>
		/// Gets the ID of this DataMiner Agent.
		/// </summary>
		/// <value>The ID of this DataMiner Agent.</value>
		int Id { get; }

		/// <summary>
		/// Gets the state of this DataMiner Agent.
		/// </summary>
		/// <value>The state of this DataMiner Agent.</value>
		/// <exception cref="AgentNotFoundException">The Agent was not found in the DataMiner System.</exception>
		AgentState State { get; }

		/// <summary>
		/// Gets the name of this DataMiner Agent.
		/// </summary>
		/// <value>The name of this DataMiner Agent.</value>
		/// <exception cref="AgentNotFoundException">The Agent was not found in the DataMiner System.</exception>
		string Name { get; }

		/// <summary>
		/// Gets the version info of this DataMiner Agent.
		/// </summary>
		string VersionInfo { get; }


		/// <summary>
		/// Gets the name of the host of this DataMiner Agent.
		/// </summary>
		/// <value>The name of the host of this DataMiner Agent.</value>
		/// <exception cref="AgentNotFoundException">The Agent was not found in the DataMiner System.</exception>
		string HostName { get; }

		/// <summary>
		/// Creates a new element with the specified configuration.
		/// </summary>
		/// <param name="configuration">The configuration of the element to be created.</param>
		/// <exception cref="ArgumentNullException"><paramref name="configuration"/> is <see langword="null"/>.</exception>
		/// <exception cref="IncorrectDataException">The provided configuration is invalid.</exception>
		/// <returns>The ID of the created element.</returns>
		/// <remarks>Currently, this method only supports creating virtual elements.</remarks>
		DmsElementId CreateElement(ElementConfiguration configuration);

		/// <summary>
		/// Determines whether an element with the specified DataMiner Agent ID/element ID exists on this DataMiner Agent.
		/// </summary>
		/// <param name="dmsElementId">The DataMiner Agent ID/element ID of the element.</param>
		/// <exception cref="ArgumentException"><paramref name="dmsElementId"/> is invalid.</exception>
		/// <returns><c>true</c> if the element exists on this DataMiner Agent; otherwise, <c>false</c>.</returns>
		bool ElementExists(DmsElementId dmsElementId);

		/// <summary>
		/// Determines whether an element with the specified name exists on this DataMiner Agent.
		/// </summary>
		/// <param name="elementName">The name of the element.</param>
		/// <returns><c>true</c> if the element exists on this DataMiner Agent; otherwise, <c>false</c>.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="elementName"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException"><paramref name="elementName"/> is the empty string ("") or white space.</exception>
		bool ElementExists(string elementName);

		/// <summary>
		/// Determines whether this DataMiner Agent exists in the DataMiner System.
		/// </summary>
		/// <returns><c>true</c> if the DataMiner Agent exists in the DataMiner System; otherwise, <c>false</c>.</returns>
		bool Exists();

		/// <summary>
		/// Retrieves the element with the specified the DataMiner Agent ID and element ID from this DataMiner Agent.
		/// </summary>
		/// <param name="dmsElementId">The Agent ID/element ID of the element.</param>
		/// <exception cref="ArgumentException"><paramref name="dmsElementId"/> is invalid.</exception>
		/// <exception cref="ElementNotFoundException">The element was not found on this DataMiner Agent.</exception>
		/// <returns>The element with the specified DataMiner Agent ID and element ID.</returns>
		IDmsElement GetElement(DmsElementId dmsElementId);

		/// <summary>
		/// Retrieves the element with the specified name from this DataMiner Agent.
		/// </summary>
		/// <param name="elementName">The name of the element.</param>
		/// <exception cref="ArgumentNullException"><paramref name="elementName"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException"><paramref name="elementName"/> is the empty string ("") or white space.</exception>
		/// <exception cref="ElementNotFoundException">The element with the specified name was not found on this DataMiner Agent.</exception>
		/// <returns>The element with the specified element name.</returns>
		IDmsElement GetElement(string elementName);

		/// <summary>
		/// Retrieves all elements present on this DataMiner Agent.
		/// </summary>
		/// <returns>The elements present on this DataMiner Agent.</returns>
		ICollection<IDmsElement> GetElements();

		/// <summary>
		/// Verifies if the provided version number is higher then the DataMiner Agent version.
		/// </summary>
		/// <param name="versionNumber">The version number to compare against the version of the DMA.</param>
		/// <returns></returns>
		bool IsVersionHigher(string versionNumber);
	}
}