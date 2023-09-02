using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using TwinCAT.Ads;

namespace TcUnit.TestAdapter.Services
{
	public class SystemService
	{
		private string _target;
		private readonly AdsClient adsClient;

		public SystemService(string target)
		{
			adsClient = new AdsClient();
		}

		public void SetToRunMode()
		{
			StateInfo setState = new StateInfo(AdsState.Reset, 0);

			if (!adsClient.IsConnected)
				adsClient.Connect(_target, (int)AmsPort.SystemService);

			adsClient.WriteControl(setState);
		}

		public async Task SetToRunMode(CancellationToken cancel)
		{
			if (!adsClient.IsConnected)
				adsClient.Connect(_target, (int)AmsPort.SystemService);

			await adsClient.WriteControlAsync(AdsState.Reset, 0, cancel);

		}

		public static void SetToRunMode(string target)
		{
			var systemService = new SystemService(target);

			systemService.SetToRunMode();
		}

		public static async Task SetToRunMode(string target, CancellationToken cancel)
		{
			var systemService = new SystemService(target);

			await systemService.SetToRunMode(cancel);
		}
		public void SetToConfigMode()
		{
			StateInfo setState = new StateInfo(AdsState.Reconfig, 0);

			if (!adsClient.IsConnected)
				adsClient.Connect(_target, (int)AmsPort.SystemService);

			adsClient.WriteControl(setState);
		}

		public async Task SetToConfigMode(CancellationToken cancel)
		{
			if (!adsClient.IsConnected)
				adsClient.Connect(_target, (int)AmsPort.SystemService);

			await adsClient.WriteControlAsync(AdsState.Reconfig, 0, cancel);

		}

		public static void SetToConfigMode(string target)
		{
			var systemService = new SystemService(target);

			systemService.SetToConfigMode();
		}

		public static async Task SetToConfigMode(string target, CancellationToken cancel)
		{
			var systemService = new SystemService(target);

			await systemService.SetToConfigMode(cancel);
		}
		public void StopRuntime()
		{
			StateInfo setState = new StateInfo(AdsState.Stop, 0);

			if (!adsClient.IsConnected)
				adsClient.Connect(_target, (int)AmsPort.SystemService);

			adsClient.WriteControl(setState);
		}

		public async Task StopRuntime(CancellationToken cancel)
		{
			if (!adsClient.IsConnected)
				adsClient.Connect(_target, (int)AmsPort.SystemService);

			await adsClient.WriteControlAsync(AdsState.Stop, 0, cancel);

		}

		public static void StopRuntime(string target)
		{
			var systemService = new SystemService(target);

			systemService.StopRuntime();
		}

		public static async Task StopRuntime(string target, CancellationToken cancel)
		{
			var systemService = new SystemService(target);

			await systemService.StopRuntime(cancel);
		}
		public Version GetVersionInfo()
		{
			UInt16[] adsBuffer = new UInt16[4];

			if (!adsClient.IsConnected)
				adsClient.Connect(_target, (int)AmsPort.SystemService);

			adsBuffer = (UInt16[])adsClient.ReadAny(160, 0, typeof(UInt16[]), new int[] { 4 });

			return new Version((ushort)adsBuffer[1], (ushort)adsBuffer[0], (ushort)adsBuffer[3], (ushort)adsBuffer[2]);
		}

		public async Task<Version> GetVersionInfo(CancellationToken cancel)
		{
			UInt16[] adsBuffer = new UInt16[4];

			if (!adsClient.IsConnected)
				adsClient.Connect(_target, (int)AmsPort.SystemService);

			var result = await adsClient.ReadAnyAsync(160, 0, typeof(UInt16[]), new int[] { 4 }, cancel);
			adsBuffer = (UInt16[])result.Value;

			return new Version((ushort)adsBuffer[1], (ushort)adsBuffer[0], (ushort)adsBuffer[3], (ushort)adsBuffer[2]);
		}

		public static Version GetVersionInfo(string target)
		{
			var systemService = new SystemService(target);

			return systemService.GetVersionInfo();
		}

		public static async Task<Version> GetVersionInfo(string target, CancellationToken cancel)
		{
			var systemService = new SystemService(target);

			return await systemService.GetVersionInfo(cancel);
		}
		public string GetHostname()
		{
			if (!adsClient.IsConnected)
				adsClient.Connect(_target, (int)AmsPort.SystemService);

			var readData = new byte[255];
			var len = adsClient.Read(AmsAdsConstants.SYSTEMSERVICE_IPHOSTNAME, AmsAdsConstants.IPHELPERAPI_HostnameIdxOff, readData);
			if (len > 0)
				len -= 1;      

			return System.Text.Encoding.ASCII.GetString(readData, 0, len);
		}

		public async Task<string> GetHostname(CancellationToken cancel)
		{
			if (!adsClient.IsConnected)
				adsClient.Connect(_target, (int)AmsPort.SystemService);

			var readData = new byte[255];
			var result = await adsClient.ReadAsync(AmsAdsConstants.SYSTEMSERVICE_IPHOSTNAME, AmsAdsConstants.IPHELPERAPI_HostnameIdxOff, readData, cancel);
			var len = result.ReadBytes;
			if (len > 0)
				len -= 1;      

			return System.Text.Encoding.ASCII.GetString(readData, 0, len);
		}

		public static string GetHostname(string target)
		{
			var systemService = new SystemService(target);

			return systemService.GetHostname();
		}

		public static async Task<string> GetHostname(string target, CancellationToken cancel)
		{
			var systemService = new SystemService(target);

			return await systemService.GetHostname(cancel);
		}

		public ST_DeviceIdentificationEx ReadDeviceIdent()
		{
			ST_DeviceIdentificationEx devIdent = new ST_DeviceIdentificationEx();

			if (!adsClient.IsConnected)
				adsClient.Connect(_target, (int)AmsPort.SystemService);

			byte[] readBuffer = new byte[AmsAdsConstants.SizeDevIdent];

			var len = adsClient.Read(AmsAdsConstants.DevIdentIdxGrp, AmsAdsConstants.DevIdentIdxOff, readBuffer);
			if (len <= 0)
				return devIdent;

			string ReadBuff = System.Text.Encoding.ASCII.GetString(readBuffer, 0, len - 1);       

			if (ReadBuff.Length <= 0)
				return devIdent;

			devIdent = ParseIdentification(ReadBuff);

			adsClient.Disconnect();

			return devIdent;
		}

		public async Task<ST_DeviceIdentificationEx> ReadDeviceIdent(CancellationToken cancel)
		{
			ST_DeviceIdentificationEx devIdent = new ST_DeviceIdentificationEx();

			if (!adsClient.IsConnected)
				adsClient.Connect(_target, (int)AmsPort.SystemService);

			byte[] readBuffer = new byte[AmsAdsConstants.SizeDevIdent];

			var result = await adsClient.ReadAsync(AmsAdsConstants.DevIdentIdxGrp, AmsAdsConstants.DevIdentIdxOff, readBuffer, cancel);
			if (result.ReadBytes <= 0)
				return devIdent;

			string ReadBuff = System.Text.Encoding.ASCII.GetString(readBuffer, 0, result.ReadBytes - 1);       

			if (ReadBuff.Length <= 0)
				return devIdent;

			devIdent = ParseIdentification(ReadBuff);

			adsClient.Disconnect();

			return devIdent;
		}

		public static ST_DeviceIdentificationEx ReadDeviceIdent(string target)
		{
			var systemService = new SystemService(target);

			return systemService.ReadDeviceIdent();
		}

		public static async Task<ST_DeviceIdentificationEx> ReadDeviceIdent(string target, CancellationToken cancel)
		{
			var systemService = new SystemService(target);

			return await systemService.ReadDeviceIdent(cancel);
		}
		private static ST_DeviceIdentificationEx ParseIdentification(string buffer)
		{
			var ret = new ST_DeviceIdentificationEx();

			XmlDocument doc = new XmlDocument();
			doc.LoadXml(buffer);

			XmlNode TargetType = doc.SelectSingleNode("/TcTargetDesc/TargetType");
			XmlNode TargetVersion = doc.SelectSingleNode("/TcTargetDesc/TargetVersion");
			XmlNode TargetFeatures = doc.SelectSingleNode("/TcTargetDesc/TargetFeatures");
			XmlNode TargetHardware = doc.SelectSingleNode("/TcTargetDesc/Hardware");
			XmlNode TargetOsImage = doc.SelectSingleNode("/TcTargetDesc/OsImage");

			ret.strTargetType = TargetType.InnerText;

			ret.strHardwareModel = TargetHardware.SelectSingleNode("Model").InnerText;
			ret.strHardwareSerialNo = TargetHardware.SelectSingleNode("SerialNo").InnerText;
			ret.strHardwareVersion = TargetHardware.SelectSingleNode("CPUVersion").InnerText;
			ret.strHardwareDate = TargetHardware.SelectSingleNode("Date").InnerText;
			ret.strHardwareCPU = TargetHardware.SelectSingleNode("CPUArchitecture").InnerText;

			ret.strImageDevice = TargetOsImage.SelectSingleNode("ImageDevice").InnerText;
			if (TargetOsImage.SelectSingleNode("ImageVersion") != null)
				ret.strImageVersion = TargetOsImage.SelectSingleNode("ImageVersion").InnerText;
			else
				ret.strImageVersion = string.Empty;
			if (TargetOsImage.SelectSingleNode("ImageLevel") != null)
				ret.strImageLevel = TargetOsImage.SelectSingleNode("ImageLevel").InnerText;
			else
				ret.strImageLevel = string.Empty;
			ret.strImageOsName = TargetOsImage.SelectSingleNode("OsName").InnerText;
			ret.strImageOsVersion = TargetOsImage.SelectSingleNode("OsVersion").InnerText;

			ret.strTwinCATVersion = TargetVersion.SelectSingleNode("Version").InnerText;
			ret.strTwinCATRevision = TargetVersion.SelectSingleNode("Revision").InnerText;
			ret.strTwinCATBuild = TargetVersion.SelectSingleNode("Build").InnerText;

			if (TargetFeatures.SelectSingleNode("Level") != null)
				ret.strTwinCATLevel = TargetFeatures.SelectSingleNode("Level").InnerText;
			else
				ret.strTwinCATLevel = "";
			ret.strtarget = TargetFeatures.SelectSingleNode("NetId").InnerText;

			return ret;
		}

		private static T_targetArr GetT_targetArrFromString(string netIdStr)
		{
			T_targetArr netId = new T_targetArr();
			netId.netId = new byte[6];
			string[] IDChars = netIdStr.Split('.');
			for (int i = 0; i < IDChars.Length; i++)
			{
				netId.netId[i] = Convert.ToByte(IDChars[i]);
			}
			return netId;
		}
		private byte[] GetByteNetIdFromString(string netIdStr)
		{
			var netIDType = GetT_targetArrFromString(netIdStr);
			return netIDType.netId;
		}
	}

	public struct ST_DeviceIdentificationEx
	{
		public string strTargetType;
		public string strHardwareModel;
		public string strHardwareSerialNo;
		public string strHardwareVersion;
		public string strHardwareDate;
		public string strHardwareCPU;
		public string strImageDevice;
		public string strImageVersion;
		public string strImageLevel;
		public string strImageOsName;
		public string strImageOsVersion;
		public string strTwinCATVersion;
		public string strTwinCATRevision;
		public string strTwinCATBuild;
		public string strTwinCATLevel;
		public string strtarget;
	}

}
