using IoTGatewayCore.MC;
using IoTGatewayCore.Models.Object;
using IoTGatewayCore.MP;
using IoTGatewayCore.U;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace IoTGatewayCore.IS
{
	public class DeviceIS
	{
		private readonly LS<dis> _ls;
		private readonly CS _cp;
		private readonly Terminal _t;

		public DeviceIS(LS<dis> ls, CS cp, Terminal t)
		{
			_ls = ls;
			_cp = cp;
			_t = t;
		}

		public void ProcessDeviceMessage(string payload, string token)
		{
			GetConfigs(token, out Server server, out Device device);
			CheckAndLog(token, server, device, MethodBase.GetCurrentMethod()?.Name);
			var message = DMC.D2I(payload, device, server);

			_t.ProcessAndRoute(message);
		}

		private void CheckAndLog(string token, Server server, Device device, string name)
		{
			_ls.NullCheck(LogLevel.Warning, name, server);
			_ls.NullCheck(LogLevel.Warning, name, device, token);
		}

		private void GetConfigs(string token, out Server server, out Device device)
		{
			server = _cp.FindServer();
			device = _cp.FindDevice(token);
		}
	}
}
