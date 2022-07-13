using MQTTnet;
using MQTTnet.Client.Publishing;
using MQTTnet.Client.Receiving;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace IoTGatewayCore.Connection.Mqtt
{
	internal class MqttArchive : IMqttApplicationMessageReceivedHandler
	{
		public enum RequestType { GetAll, GetActive };

		public Task<MqttClientPublishResult> SendRequest(string topic, RequestType requestType)
		{
			dynamic json = new ExpandoObject();
			dynamic Images = new ExpandoObject();
			(Images as IDictionary<string, object>).Add(requestType.ToString(), "");
			json.Images = Images;
			var message = JsonSerializer.Serialize(json);
			var mqttMessage = new MqttApplicationMessage()
			{
				Topic = topic,
				Payload = Encoding.UTF8.GetBytes(message),
				ContentType = "application/json"
			};
			//return MqttServer.PublishAsync(mqttMessage);
			return null;
		}

		public class RequestDataResult
		{
			public bool Success { get; set; }
			public MqttApplicationMessageReceivedEventArgs MessageArgs { get; set; }
			public MqttClientPublishResult PublishResult { get; set; }

		}
		private bool Waiting { get; set; } = false;
		private string WaitingDeviceId { get; set; } = string.Empty;
		private RequestDataResult RequestResult { get; set; } = new RequestDataResult();
		public double RequestResponseTimeout { get; private set; }

		public async Task<RequestDataResult> RequestData(string topic, string deviceId, RequestType requestType)
		{
			var publishResult = SendRequest(topic, requestType).Result;

			if (publishResult == null)
			{
				return new RequestDataResult() { Success = false };
			}

			if (publishResult.ReasonCode != MqttClientPublishReasonCode.Success)
			{
				RequestResult.Success = false;
				RequestResult.PublishResult = publishResult;
				return RequestResult;
			}
			else
			{
				Waiting = true;
				WaitingDeviceId = deviceId;
				RequestResult = new RequestDataResult();

				var timer = Task.Delay(TimeSpan.FromSeconds(RequestResponseTimeout));

				while (!timer.IsCompleted & Waiting)
				{
					await Task.Delay(25);
				}

				return RequestResult;
			}
		}

		/// <summary>
		/// Handles the received application message event.
		/// </summary>
		/// <param name="e">The event args.</param>
		public Task HandleApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs e)
		{
			if (Waiting && e.ClientId == WaitingDeviceId)
			{
				RequestResult.MessageArgs = e;
				RequestResult.Success = true;
				Waiting = false;
			}

			Console.WriteLine($"[{e.ClientId}]\t" + e.ApplicationMessage.ConvertPayloadToString());

			return Task.CompletedTask;
		}
	}
}
