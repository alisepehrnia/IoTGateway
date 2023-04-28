using MongoDB.Bson.Serialization.Attributes;

namespace IoTGatewayCore.Models.Object
{
	[BsonIgnoreExtraElements]
	public class Transceiver
	{
		public Transceiver()
		{
			TransceiverType = TransceiverType.Internal;
		}

		public int Id { get; set; }
		public string Name { get; set; }
		public string Type { get; set; }

		public string Token { get; set; }

		public string Host { get; set; }
		public int Port { get; set; }

		public TransceiverType TransceiverType { get; set; }
	}
}
