using IoTGatewayCore.Models.Data;

namespace IoTGatewayCore.MP
{
	internal interface IModule
	{
		public static object Run(Message message) => null;
	}
}
