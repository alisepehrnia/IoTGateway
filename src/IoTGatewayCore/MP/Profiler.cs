using IoTGatewayCore.Models.Data;
using System;

namespace IoTGatewayCore.MP
{
	internal class Profiler : IModule
	{
		private static void AddTimestamp(Message message)
		{
			message.DateTime = DateTime.Now;
		}

		public static object Run(Message message)
		{
			//AddTimestamp(message);

			return null;
		}
	}
}
