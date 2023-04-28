using IoTGatewayCore.MC;
using IoTGatewayCore.Models.Object;
using IoTGatewayCore.MP;
using IoTGatewayCore.U;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace IoTGatewayCore.IS
{
	public class ServerIS
	{
		private readonly LS<sis> _ls;
		private readonly CS _cp;
		private readonly Terminal _t;

		public ServerIS(LS<sis> ls, CS cp, Terminal t)
		{
			_ls = ls;
			_cp = cp;
			_t = t;
		}

		/// <summary>
		/// Server (Config)
		/// </summary>
		public void ProcessServerMessage(string payload, string token, int id)
		{
			GetConfigs(token, id, out Server server, out Device device);

			var name = MethodBase.GetCurrentMethod()?.Name;
			CheckAndLog(token, id, server, device, MethodBase.GetCurrentMethod()?.Name);

			var message = SMC.S2I(payload, server, device);
			_ls.Log(LogLevel.Debug, $"({name}) S2I Output:\nPayload={message.GetPayload() ?? null}");

			_t.ProcessAndRoute(message);
		}

		private void CheckAndLog(string token, int id, Server server, Device device, string name)
		{
			_ls.NullCheck(LogLevel.Warning, name, server, token);
			_ls.NullCheck(LogLevel.Warning, name, device, id);
		}

		private void GetConfigs(string token, int id, out Server server, out Device device)
		{
			server = _cp.FindServer(token);
			//Null when adding new device one
			device = _cp.FindDevice(id);
		}

		/// <summary>
		/// Server (Command)
		/// </summary>
		public void ProcessServerMessage(string payload, string token, string deviceName)
		{
			GetConfigs(token, deviceName, out Server server, out Device device, out Hub hub);
			CheckAndLog(token, deviceName, server, device, hub, MethodBase.GetCurrentMethod()?.Name);
			var message = SMC.S2I(payload, server, device, hub);

			_t.ProcessAndRoute(message);
		}

		private void CheckAndLog(string token, string deviceName, Server server, Device device, Hub hub, string name)
		{
			_ls.NullCheck(LogLevel.Warning, name, server, token);
			_ls.NullCheck(LogLevel.Warning, name, device, deviceName);
			_ls.NullCheck(LogLevel.Warning, name, hub, deviceName, device?.Hub_Id);
		}

		private void GetConfigs(string token, string deviceName, out Server server, out Device device, out Hub hub)
		{
			server = _cp.FindServer(token);
			device = _cp.FindDeviceByName(deviceName);
			hub = _cp.FindHub(device?.Hub_Id ?? 0);
		}
	}
}
