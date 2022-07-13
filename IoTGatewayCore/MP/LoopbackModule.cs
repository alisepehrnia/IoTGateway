using IoTGatewayCore.Models.Data;

namespace IoTGatewayCore.MP
{
	internal class LoopbackModule : IModule
	{
		public static object Run(Message message)
		{
			message.IsLoopback = true;

			return null;
		}
	}
}
