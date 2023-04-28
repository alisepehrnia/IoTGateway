using System;
using System.Text.Json;
using System.Collections.Generic;

namespace IoTGatewayCore.MC
{
	internal class Decoder
	{
		public static string DecodeToString(byte[] payload)
		{
			return Convert.ToString(payload);
		}

		public static IDictionary<string, object> DecodeToJson(string payload)
		{
			return JsonSerializer.Deserialize<IDictionary<string, object>>(payload);
		}

		public static IDictionary<string, object> DecodeToJson(byte[] payload)
		{
			return DecodeToJson(Convert.ToString(payload));
		}
	}
}
