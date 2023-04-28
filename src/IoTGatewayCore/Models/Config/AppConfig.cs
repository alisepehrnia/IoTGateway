using IoTGatewayCore.Models.Object;
using System.Collections.Generic;

namespace IoTGatewayCore.Models.Config
{
	public class AppConfig
	{
		public Database Database { get; set; } = new();

		public List<Device> Devices { get; set; } = new();
		public List<Hub> Hubs { get; set; } = new();
		public List<Server> Servers { get; set; } = new();
	}
}
