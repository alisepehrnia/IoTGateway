using IoTGatewayCore.Models.Data;
using IoTGatewayCore.Models.Object;

namespace IoTGatewayCore.MC
{
	public class DMC
	{
		public static Message D2I(string payload, Device device, Server server)
			=> new(MessageType.Telemetry, payload, device, server);
	}
}
