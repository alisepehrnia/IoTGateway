using IoTGatewayCore.Models.Data;
using IoTGatewayCore.Models.Object;

namespace IoTGatewayCore.MC
{
	public class SMC
	{
		/// <summary>
		/// SMC (Config)
		/// </summary>
		public static Message S2I(string payload, Server server, Device device) => new(MessageType.Config, payload)
		{
			Source = server,
			Destination = device ?? new Device() { Port = -1 }
		};

		/// <summary>
		/// SMC (Command)
		/// </summary>
		public static Message S2I(string payload, Server server, Device device, Hub hub) 
			=> new(MessageType.Command, payload, server, (Transceiver)device ?? hub, hub);
	}
}
