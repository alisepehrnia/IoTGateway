using MongoDB.Bson.Serialization.Attributes;

namespace IoTGatewayCore.Models.Object
{
	[BsonIgnoreExtraElements]
	public class Device : Transceiver
	{
		public Device()
		{
			TransceiverType = TransceiverType.Device;
		}

		[BsonIgnoreIfNull]
		public int? Hub_Id { get; set; }
	}
}
