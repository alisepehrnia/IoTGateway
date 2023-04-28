using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace IoTGatewayCore.MC
{
	public static class Helper
	{
		public static string SafeExceptionToJson(Exception ex)
		{
			string json = null;
			if (ex != null)
			{
				try
				{
					json = JsonSerializer.Serialize(ex);
				}
				catch { }
			}
			return json;
		}

		public static HttpResponseMessage BuildJsonHttpResponse(string json, string reason = null, HttpRequestMessage request = null) => new()
		{
			StatusCode = System.Net.HttpStatusCode.InternalServerError,
			ReasonPhrase = reason,
			Content = json == null ? null : new StringContent(json, Encoding.UTF8, "application/json"),
			RequestMessage = request
		};

		public static HttpResponseMessage BuildExceptionHttpResponse(Exception ex, HttpRequestMessage reqMessage = null)
		{
			var response = BuildJsonHttpResponse(SafeExceptionToJson(ex), ex?.Message?.ToString(), reqMessage);
			response.SafeParseHttpExceptionStatusCode(ex);

			return response;
		}

		private static HttpResponseMessage SafeParseHttpExceptionStatusCode(this HttpResponseMessage response, Exception ex)
		{
			if (ex != null && ex.GetType() == typeof(HttpRequestException))
			{
				var code = ((HttpRequestException)ex)?.StatusCode;
				if (code != null) response.StatusCode = code.Value;
			}

			return response;
		}
	}
}
