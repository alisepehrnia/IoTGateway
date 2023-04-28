using System;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client.Receiving;
using MQTTnet.Server;
using MQTTnet.Protocol;
using MQTTnet.AspNetCore;
using System.Collections.Generic;
using MQTTnet.Server.Status;
using System.Text;
using MQTTnet.Client.Publishing;
using Microsoft.Extensions.Logging;

namespace IoTGatewayCore.Connection
{
	public class MqttProvider : IMqttServerStartedHandler, IMqttServerStoppedHandler, IMqttServerConnectionValidator,
		IMqttServerClientConnectedHandler, IMqttServerClientDisconnectedHandler, IMqttServerClientSubscribedTopicHandler,
		IMqttServerClientUnsubscribedTopicHandler, IMqttApplicationMessageReceivedHandler, IMqttServerApplicationMessageInterceptor,
		IMqttServerSubscriptionInterceptor
	{
		private readonly ILogger<MqttProvider> _l;

		public MqttProvider(ILogger<MqttProvider> logger)
		{
			_l = logger;
		}

		private IMqttServer MqttServer { get; set; } = new MqttFactory().CreateMqttServer();

		public void ConfigureMqttServerOptions(AspNetMqttServerOptionsBuilder options)
		{
			options.WithConnectionValidator(this);
			options.WithSubscriptionInterceptor(this);
		}

		public void ConfigureMqttServer(IMqttServer mqtt)
		{
			MqttServer = mqtt;
			MqttServer.StartedHandler = this;
			MqttServer.StoppedHandler = this;
			MqttServer.ClientConnectedHandler = this;
			MqttServer.ClientDisconnectedHandler = this;
			MqttServer.ClientSubscribedTopicHandler = this;
			MqttServer.ClientUnsubscribedTopicHandler = this;
			MqttServer.ApplicationMessageReceivedHandler = this;
		}

		public async Task<IList<IMqttClientStatus>> GetConnectedDevices() => await MqttServer.GetClientStatusAsync();

		public async Task<MqttClientPublishResult> SendJson(string topic, string payload)
			=> await MqttServer.PublishAsync(new MqttApplicationMessage()
			{
				Topic = topic,
				Payload = Encoding.UTF8.GetBytes(payload),
				ContentType = "application/json"
			});

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

		public async Task HandleApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs e)
		{
			_l.LogInformation($"Message Received: Topic ${e.ApplicationMessage?.Topic ?? null} ClientId ${e.ClientId ?? null}, PayloadSize ${e.ApplicationMessage?.Payload?.Length ?? 0}B");

			var payload = e?.ApplicationMessage?.Payload ?? null;
			var json = payload == null ? null : Encoding.UTF8.GetString(payload);
			_l.LogDebug($"Message From ClientId ${e.ClientId ?? null}:\n${json}");
		}

		public async Task HandleServerStartedAsync(EventArgs e) => _l.LogInformation("Server Started");
		public async Task HandleServerStoppedAsync(EventArgs e) => _l.LogCritical("Server Stopped");

		public async Task ValidateConnectionAsync(MqttConnectionValidatorContext c)
		{
			if (string.IsNullOrEmpty(c.ClientId))
			{
				c.ReasonCode = MqttConnectReasonCode.ClientIdentifierNotValid;
				return;
			}
			if (string.IsNullOrEmpty(c.Username) || string.IsNullOrEmpty(c.Password))
			{
				c.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
				return;
			}

			dynamic deviceConfig = null;// _appConfig.Devices.Find(d => d.ClientId == c.ClientId);
			if (deviceConfig == null)
			{
				c.ReasonCode = MqttConnectReasonCode.ClientIdentifierNotValid;
				_l.LogWarning($"Unknown Device: Username ${c.Username}, Endpoint ${c.Endpoint}");
				return;
			}
			if (c.Username != deviceConfig.Username || c.Password != deviceConfig.Password)
			{
				c.ReasonCode = MqttConnectReasonCode.NotAuthorized;
				_l.LogWarning($"Unauthorized Connection: Username ${c.Username}, Endpoint ${c.Endpoint}");
				return;
			}

			c.ReasonCode = MqttConnectReasonCode.Success;
		}

		public async Task HandleClientSubscribedTopicAsync(MqttServerClientSubscribedTopicEventArgs e)
			=> _l.LogInformation($"Client Subscribed: ${e?.ClientId ?? null} To ${e?.TopicFilter ?? null}");

		public async Task HandleClientUnsubscribedTopicAsync(MqttServerClientUnsubscribedTopicEventArgs e)
			=> _l.LogWarning($"Client Unsubscribed: ${e?.ClientId ?? null} From ${e?.TopicFilter ?? null}");

		public async Task HandleClientConnectedAsync(MqttServerClientConnectedEventArgs e)
			=> _l.LogInformation($"Client Connected: ${e?.ClientId ?? null} To ${e?.Endpoint ?? null} Wth Username ${e?.UserName ?? null}");

		public async Task HandleClientDisconnectedAsync(MqttServerClientDisconnectedEventArgs e)
			=> _l.LogWarning($"Client Disconnected: ${e?.ClientId ?? null} From ${e?.Endpoint ?? null} (DisconnecyType=${e?.DisconnectType ?? null})");

		public async Task InterceptSubscriptionAsync(MqttSubscriptionInterceptorContext c)
		{
			if (c.TopicFilter.Topic.Contains(c?.ClientId)) return;

			c.AcceptSubscription = false;
		}

		public async Task InterceptApplicationMessagePublishAsync(MqttApplicationMessageInterceptorContext c)
		{
			var payload = c?.ApplicationMessage?.Payload ?? null;
			var json = payload == null ? null : Encoding.UTF8.GetString(payload);
			_l.LogDebug($"Sending Message to client ${c?.ClientId ?? null} on topic ${c?.ApplicationMessage?.Topic ?? null}:\n${json}");
		}
	}
}