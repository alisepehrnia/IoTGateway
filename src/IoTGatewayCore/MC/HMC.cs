using IoTGatewayCore.Models.Data;
using IoTGatewayCore.Models.Object;

namespace IoTGatewayCore.MC
{
	public class HMC
	{
		public static Message H2I(string payload, Device device, Hub hub, Server server) 
			=> new(MessageType.Telemetry, payload, device, server, hub);
	}
}
