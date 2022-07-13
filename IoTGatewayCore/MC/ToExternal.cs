using IoTGatewayCore.Models.Data;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Text;

namespace IoTGatewayCore.MC
{
	public class ToExternal
	{
		private readonly ILogger<ToExternal> _l;

		public ToExternal(ILogger<ToExternal> l)
		{
			_l = l;
		}

		public HttpRequestMessage ToHttp(Message message)
		{
			if(message.Destination == null)
			{
				_l.LogCritical("(ToHttp) Message Destination Is Null");
				return null;
			}

			var destination = message.Destination.Host != null ? message.Destination : message.Hub;
			var url = JoinHostPortString(destination.Host, destination.Port);

			if(url == null)
			{
				_l.LogWarning("HttpRequestMessage Url Is Null");
			}

			return BuildHttpRequestMessage(HttpMethod.Post, url, message.Payload);
		}

		public HttpRequestMessage BuildHttpRequestMessage(HttpMethod method, string url, string? payload)
		{
			if (url == null)
			{
				_l.LogWarning("HttpRequestMessage Url Is Null");
			}

			var hMessage = new HttpRequestMessage(method, url)
			{
				Content = payload == null ? null : new StringContent(payload, Encoding.UTF8, "application/json")
			};
			return hMessage;
		}

		public static string JoinHostPortString(string host, int port)
		{
			if (host == null || port <= 0) return null;

			return $"http://{host.TrimEnd('/')}:{port}";
		}
	}
}
