using IoTGatewayCore.MC;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace IoTGatewayCore.Connection
{
	public class HttpProvider
	{
		private readonly ILogger<HttpProvider> _l;
		public readonly HttpClient Client;

		public HttpProvider(ILogger<HttpProvider> l)
		{
			_l = l;
			Client = new HttpClient();
		}

		private async Task<HttpResponseMessage> PostAsync(HttpRequestMessage request)
			=> await Client.PostAsync(request.RequestUri, request.Content);

		public async Task<HttpResponseMessage> SafePostAsync(HttpRequestMessage request)
		{
			HttpResponseMessage response;
			try
			{
				response = await PostAsync(request);
			}
			catch (Exception ex)
			{
				_l.LogError(ex, "Http Post Exception");
				response = Helper.BuildExceptionHttpResponse(ex, request);
			}
			return response;
		}
	}
}
