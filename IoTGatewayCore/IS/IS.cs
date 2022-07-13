using IoTGatewayCore.Models.Object;
using IoTGatewayCore.U;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace IoTGatewayCore.IS
{
	/// <summary>
	/// Message Input Service
	/// </summary>
	public class IS
	{
		private readonly LS<IS> _ls;
		private readonly ServerIS _sis;
		private readonly DeviceIS _dis;
		private readonly HubIS _his;

		public IS(LS<IS> ls, ServerIS sis, DeviceIS dis, HubIS his)
		{
			_ls = ls;
			_sis = sis;
			_dis = dis;
			_his = his;
		}

		private static void Runner(Action action) => _ = Task.Run(action);

		public void ProcessMessage<T>(string payload, string token, string deviceName = null)
			=> Runner(() => ProcessByType<T>(payload, token, deviceName));

		private void ProcessByType<T>(string payload, string token, string deviceName = null)
		{
			var t = typeof(T);
			if (t == typeof(Device)) _dis.ProcessDeviceMessage(payload, token);
			else if (t == typeof(Hub)) _his.ProcessHubMessage(payload, token, deviceName);
			else if (t == typeof(Server)) _sis.ProcessServerMessage(payload, token, deviceName);
			else _ls.MessageSrcTypeUnknown(LogLevel.Warning, MethodBase.GetCurrentMethod()?.Name, "Message Source Type", t);
		}

		/// <summary>
		/// For Server Message (Config)
		/// </summary>
		public void ProcessMessage(string payload, string token, int id)
			=> Runner(() => _sis.ProcessServerMessage(payload, token, id));
	}
}
