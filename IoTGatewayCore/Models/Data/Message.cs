using IoTGatewayCore.Models.Object;
using System;
using System.Text.Json;

namespace IoTGatewayCore.Models.Data
{
	public class Message
	{
		public Message()
		{
			Id = new Random((int)System.DateTime.Now.Ticks).Next();
			Source = new Transceiver() { Id = 0 };
			Destination = new Transceiver() { Id = 0 };
			DateTime = System.DateTime.Now;
			IsLoopback = false;
		}

		public Message(MessageType messageType, string payload) : this()
		{
			MessageType = messageType;
			Payload = payload;
		}

		public Message(MessageType messageType, string payload, Transceiver source = null, Transceiver destination = null, Hub hub = null)
			: this(messageType, payload)
		{
			Source = source;
			Destination = destination;
			Hub = hub;
		}

		public string GetPayload() => JsonSerializer.Serialize(Payload);

		public int Id { get; set; }
		public MessageType MessageType { get; set; }
		public Transceiver Source { get; set; }
		public Hub Hub { get; set; }
		public Transceiver Destination { get; set; }
		public string Payload { get; set; }
		public bool? IsAuthorized { get; set; }
		public DateTime? DateTime { get; set; }
		public bool IsLoopback { get; set; }
	}
}
