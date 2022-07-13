using IoTGatewayCore.Models.Object;
using System;
using System.Text.Json;

namespace IoTGatewayCore.Models.Data
{
	public partial class ConfigMessage : Message
	{
		public ConfigMessageType Type { get; set; }

		public ConfigMessage()
		{
			MessageType = MessageType.Config;
			Destination = new Transceiver()
			{
				TransceiverType = TransceiverType.Internal
			};
		}

		public ConfigMessage(ConfigMessageType type) : this()
		{
			Type = type;
		}

		public static ConfigMessage FromMessage(Message message)
			=> JsonSerializer.Deserialize<ConfigMessage>(JsonSerializer.Serialize(message));

		public bool TryGetDevice(out Device device, out Exception ex) 
		{
			device = null;
			ex = null;

			try
			{
				device = JsonSerializer.Deserialize<Device>(Payload);
				return true;
			}
			catch (Exception e)
			{
				ex = e;
				return false;
			}
		}

		public bool Confirm() => IsAuthorized.HasValue && IsAuthorized.Value == true && Source != null && Destination != null;
		
	}
}
