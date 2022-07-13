using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Options;
using MQTTnet.Client.Publishing;
using MQTTnet.Client.Receiving;
using MQTTnet.Client.Subscribing;
using MQTTnet.Client.Unsubscribing;
using MQTTnet.Protocol;

namespace IoTGatewayCore.Connection.Mqtt
{
	public sealed class MqttClientService : IMqttApplicationMessageReceivedHandler, IMqttClientConnectedHandler
	{
		private IMqttClient MqttClient { get; set; } = new MqttFactory().CreateMqttClient();
		private IMqttClientOptions MqttClientOptions { get; set; }
		public bool IsConnected => MqttClient.IsConnected;

		public void ConfigureMqttClientOptions(IMqttClientOptions options)
		{
			MqttClientOptions = options;
		}

		public void ConfigureMqttClient(IMqttClient mqtt)
		{
			MqttClient = mqtt;
			MqttClient.ApplicationMessageReceivedHandler = this;
			MqttClient.ConnectedHandler = this;
		}

		public async Task<MqttClientConnectResult> Connect()
		{
			if (MqttClient.IsConnected)
			{
				await Disconnect();
			}

			try
			{
				var result = await MqttClient.ConnectAsync(MqttClientOptions);
				return result;
			}
			catch (Exception e)
			{
				return default;
			}
		}
		public async Task Disconnect()
		{
			try
			{
				await MqttClient.DisconnectAsync();
			}
			catch (Exception e)
			{

			}
		}

		public async Task<MqttClientPublishResult> Publish(string topic, string payload)
		{
			topic ??= string.Empty;
			payload ??= string.Empty;

			try
			{
				var applicationMessage = new MqttApplicationMessageBuilder()
					.WithTopic(topic)
					.WithPayload(Encoding.UTF8.GetBytes(payload))
					.Build();

				var result = await MqttClient.PublishAsync(applicationMessage);

				return result;
			}
			catch (Exception e)
			{
				return default;
			}
		}

		public async Task<MqttClientSubscribeResult> Subscribe(string topic)
		{
			topic ??= string.Empty;

			try
			{
				//Logger.Verbose("Building filter...");
				var filter = new MqttTopicFilterBuilder()
									.WithTopic(topic)
									.WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtMostOnce)
									.Build();

				//Logger.Verbose("Building options...");
				var subscribeOptions = new MqttClientSubscribeOptionsBuilder()
											.WithTopicFilter(filter)
											.Build();

				//Logger.Verbose("Subscribing to Topic: {topic}", options.Topic);
				var result = await MqttClient.SubscribeAsync(subscribeOptions)
											.ConfigureAwait(false);
				//Logger.Debug("Subscribtion result: {result}", result);

				return result;
			}
			catch (Exception e)
			{
				//Logger.Error(e,"");
				return default;
			}
		}
		public async Task<MqttClientUnsubscribeResult> Unsubscribe(params string[] topic)
		{
			if (topic == null)
			{
				//Logger.Warning("Unsubscribing from null Topic!");
				return default;
			}

			try
			{
				return await MqttClient.UnsubscribeAsync(topic);
			}
			catch (Exception e)
			{
				//Logger.Error(e,"");
				return default;
			}
		}

		public static Dictionary<string, string> FilesToDic(params string[] pathes)
		{
			Dictionary<string, string> dic = new();
			foreach (var path in pathes)
			{
				var bytes = File.ReadAllBytes(path);
				dic.Add(Path.GetFileName(path), Convert.ToBase64String(bytes));
			}
			return dic;
		}
		public async Task<MqttClientPublishResult> SendData(string topic, string[] addImages, string[] removeImages, Dictionary<string, string> files)
		{
			dynamic json = new ExpandoObject();
			dynamic Images = new ExpandoObject();
			Images.Insert = addImages;
			Images.Remove = removeImages;
			Images.File = files;
			json.Images = Images;
			var message = JsonSerializer.Serialize(json);

			return await Publish(message, topic);
		}

		public async Task HandleApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs e)
		{

		}

		public Task HandleConnectedAsync(MqttClientConnectedEventArgs eventArgs)
		{
			return Task.CompletedTask;
		}
	}
}
