using Skyline.DataMiner.Net.Messages;
using System;

namespace Skyline.DataMiner.Library.Common
{
	internal class Tcp : ConnectionSettings, ITcp
	{
		private string remoteHost;
		private int networkInterfaceCard;
		private int localPort;
		private int remotePort;
		private bool isSslTlsEnabled;
		private bool isDedicated;

		//public Tcp(DmsElement dmsElement) : base(dmsElement)
		//{
		//}

		public string RemoteHost
		{
			get { return remoteHost; }
			set { remoteHost = value; }
		}

		public int NetworkInterfaceCard
		{
			get { return networkInterfaceCard; }
			set { networkInterfaceCard = value; }
		}

		public int LocalPort
		{
			get { return localPort; }
			set { localPort = value; }
		}

		public int RemotePort
		{
			get { return remotePort; }
			set { remotePort = value; }
		}

		public bool IsSslTlsEnabled
		{
			get { return isSslTlsEnabled; }
			set { isSslTlsEnabled = value; }
		}

		public bool IsDedicated
		{
			get { return isDedicated; }
		}

		internal override bool IsUpdated
		{
			get
			{
				throw new NotSupportedException("TCP is not supported yet.");
			}
		}

		internal override void ClearUpdates()
		{
			throw new NotSupportedException("TCP is not supported yet.");
		}

		internal override ElementPortInfo CreateElementPortInfo(int portPosition,bool isCompatibilityIssueDetected)
		{
			throw new NotSupportedException("TCP is not supported yet.");
		}

		internal override void UpdateElementPortInfo(ElementPortInfo portInfo, bool isCompatibilityIssueDetected)
		{
			throw new NotSupportedException("TCP is not supported yet.");
		}
	}
}