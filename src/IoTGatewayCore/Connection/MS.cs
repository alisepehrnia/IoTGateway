using IoTGatewayCore.MC;
using IoTGatewayCore.Models.Data;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace IoTGatewayCore.Connection
{
	public class MS
	{
		private readonly ILogger<MS> _l;
		private readonly HttpProvider _httpProvider;
		private readonly ToExternal _toExternal;

		public MS(ILogger<MS> l, HttpProvider httpProvider, ToExternal toExternal)
		{
			_l = l;
			_httpProvider = httpProvider;
			_toExternal = toExternal;
		}

		//General
		public async Task<HttpResponseMessage> Send(Message message)
		{
			var src = message.Source == null ? null : JsonSerializer.Serialize(message.Source);
			UnknownDestinationLog(message, src);
			//Just Send to logger
			if (message.IsLoopback)
			{
				LoopbackLog(message, src);
				return null;
			}

			return await SendHttp(message);
		}

		private void UnknownDestinationLog(Message message, string src)
		{
			if (message.Destination == null
							|| string.IsNullOrEmpty(message.Destination?.Host)
							|| message.Destination?.Port == null)
			{
				_l.LogCritical($"(Message Destination Unknown/ Bad Host or Port)\n\t Id={message.Id}\n\t Type={message.MessageType}\n\t From={src}\n\t Payload={message.Payload}");
			}
		}

		private void LoopbackLog(Message message, string src)
		{
			var hub = message.Hub == null ? null : JsonSerializer.Serialize(message.Hub);
			var dest = message.Destination == null ? null : JsonSerializer.Serialize(message.Destination);
			_l.LogDebug($"[Message Echo]\n\t Id={message.Id}\n\t Type={message.MessageType}\n\t From={src}\n\t Hub={hub}\n\t Destination={dest}\n\t Payload={message.Payload}");
		}

		//Http
		private async Task<HttpResponseMessage> SendHttp(Message message)
		{
			var request = _toExternal.ToHttp(message);
			var json = request == null ? null : JsonSerializer.Serialize(request);
			_l.LogDebug($"Http Message To Send:\n{json}");
			var response = await _httpProvider.SafePostAsync(request);
			json = response == null ? null : JsonSerializer.Serialize(response);
			_l.LogDebug($"Http Message Response:\n{json}");

			return response;
		}
	}
}
