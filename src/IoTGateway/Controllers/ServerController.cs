using IoTGatewayCore.IS;
using IoTGatewayCore.Models.Object;
using IoTGatewayCore.U;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace IoTGateway.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class ServerController : ControllerBase
	{
		private readonly LS<ServerController> _ls;
		private readonly IS _is;

		public ServerController(LS<ServerController> ls, IS @is)
		{
			_ls = ls;
			_is = @is;
		}

		//Command Message (Toward Device)
		[HttpPost("Command/{deviceName}")]
		public IActionResult Command([FromBody] object payload, [FromHeader] string token, [FromRoute] string deviceName)
		{
			var methodName = MethodBase.GetCurrentMethod()?.Name;
			_ls.TokenCheck(LogLevel.Warning, methodName, token);
			_ls.NullCheck(LogLevel.Warning, methodName, deviceName, nameof(deviceName));

			_is.ProcessMessage<Server>(payload?.ToString(), token, deviceName);

			return Accepted();
		}

		[HttpPost("Config")]
		public IActionResult Config([FromBody] object payload, [FromHeader] string serverToken, [FromHeader] int id)
		{
			_ls.NullCheck(LogLevel.Warning, MethodBase.GetCurrentMethod()?.Name, serverToken, nameof(serverToken));

			_is.ProcessMessage(payload.ToString(), serverToken, id);

			return Accepted();
		}
	}
}
