using IoTGatewayCore.MC;
using IoTGatewayCore.Models.Object;
using IoTGatewayCore.MP;
using IoTGatewayCore.U;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace IoTGatewayCore.IS
{
	public class HubIS
	{
		private readonly LS<his> _ls;
		private readonly CS _cp;
		private readonly Terminal _t;

		public HubIS(LS<his> ls, CS cp, Terminal t)
		{
			_ls = ls;
			_cp = cp;
			_t = t;
		}

		public void ProcessHubMessage(string payload, string token, string deviceName)
		{
			GetConfigs(token, deviceName, out Server server, out Hub hub, out Device device);

			var name = MethodBase.GetCurrentMethod()?.Name;
			CheckAndLog(token, deviceName, server, hub, device, name);

			var message = HMC.H2I(payload, device, hub, server);

			_t.ProcessAndRoute(message);
		}

		private void CheckAndLog(string token, string deviceName, Server server, Hub hub, Device device, string name)
		{
			_ls.NullCheck(LogLevel.Warning, name, server);
			_ls.NullCheck(LogLevel.Warning, name, hub, token);
			_ls.NullCheck(LogLevel.Warning, name, device, deviceName);
		}

		private void GetConfigs(string token, string deviceName, out Server server, out Hub hub, out Device device)
		{
			server = _cp.FindServer();
			hub = _cp.FindHub(token);
			device = _cp.FindDevice(deviceName, hub?.Id ?? 0);
		}

		public class his
		{
		}
	}
}