using IoTGatewayCore.Models.Data;

namespace IoTGatewayCore.MP
{
	internal class Security : IModule
	{
		private static bool IsAuthenticated(Message message)
			=> message.MessageType == MessageType.Config ? IsAuthenticatedConfigMessage(message) : IsAuthenticatedMessage(message);

		//Is Source Or Destination Has Token?
		private static bool IsAuthenticatedMessage(Message message)
			=> message?.Source?.Token != null || message?.Destination?.Token != null;	

		//Is Config Destination An Old Device OR A New One?
		private static bool IsAuthenticatedConfigMessage(Message message)
			=> message?.Source?.Token != null && (message?.Destination?.Id != null || message?.Destination?.Port < 0);

		public static object Run(Message message)
		{
			//Mark Message Authorization Status
			message.IsAuthorized = IsAuthenticated(message);

			return null;
		}
	}
}
