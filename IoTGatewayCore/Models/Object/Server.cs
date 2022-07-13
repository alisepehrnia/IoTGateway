using MongoDB.Bson.Serialization.Attributes;

namespace IoTGatewayCore.Models.Object
{
	[BsonIgnoreExtraElements]
	public class Server : Transceiver
	{
		public Server()
		{
			TransceiverType = TransceiverType.Server;
		}
	}
}
