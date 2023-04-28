using IoTGatewayCore.Connection;
using IoTGatewayCore.Models.Data;
using IoTGatewayCore.Models.Object;
using IoTGatewayCore.U;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace IoTGatewayCore.MP
{
	public class Terminal
	{
		private readonly ILogger<Terminal> _l;
		private readonly MS _ms;
		private readonly CS _cs;

		public Terminal(ILogger<Terminal> logger, MS ms, CS cs)
		{
			_l = logger;
			_ms = ms;
			_cs = cs;
		}

		public async void ProcessAndRoute(Message message)
		{
			TerminalRoutine(message);
			_ = await MessageRouterAsync(message);
		}

		private static void TerminalRoutine(Message message)
		{
			Security.Run(message);
			Profiler.Run(message);
			Processor.Run(message);
			ModuleA.Run(message);
			LoopbackModule.Run(message);
		}
		
		public async Task<HttpResponseMessage> MessageRouterAsync(Message message)
		{
			if (message.MessageType == MessageType.Config)
			{
				ProcessConfigMessage(message);
				return null;
			}
			else
				return await _ms.Send(message);
		}

		private void ProcessConfigMessage(Message message)
		{
			var cm = ConfigMessage.FromMessage(message);

			var success = cm.TryGetDevice(out Device d, out Exception e);
			if (!success)
			{
				_l.LogWarning($"Config Message Payload Unknown Format: Payload={cm.Payload ?? null}");
				_l.LogWarning(e, "Config Message Payload Exception");

				return;
			}
			if (!cm.Confirm())
			{
				_l.LogWarning($"Config Message NOT Confirmed");

				return;
			}
			if (d.Port <= 0)
			{
				_l.LogWarning($"Device Port Not Positive Number!");

				return;
			}

			var dId = cm.Destination?.Id;
			//Device Exists
			if (dId.HasValue && dId.Value > 0)
			{
				if (cm.Destination.Port == -1) _cs.DeleteDevice("Id", cm.Destination.Id);
				else _cs.UpdateDevice(cm.Destination as Device, "Id", cm.Destination.Id);
			}
			//New Device
			else _cs.AddDevice(cm.Destination as Device);
		}
	}
}
