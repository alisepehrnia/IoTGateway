using MongoDB.Bson.Serialization.Attributes;

namespace IoTGatewayCore.Models.Object
{
	[BsonIgnoreExtraElements]
	public class Hub : Transceiver
	{
		public Hub()
		{
			TransceiverType = TransceiverType.Hub;
		}
	}
}
