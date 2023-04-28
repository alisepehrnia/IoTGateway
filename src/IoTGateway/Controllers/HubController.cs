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
	public class HubController : ControllerBase
	{
		private readonly LS<HubController> _ls;
		private readonly IS _is;

		public HubController(LS<HubController> ls, IS @is)
		{
			_ls = ls;
			_is = @is;
		}

		[HttpPost("Telemetry/{deviceName}")]
		public IActionResult Telemetry([FromBody] object payload, [FromHeader] string token, [FromRoute] string deviceName)
		{
			var methodName = MethodBase.GetCurrentMethod()?.Name;
			_ls.TokenCheck(LogLevel.Warning, methodName, token);
			_ls.NullCheck(LogLevel.Warning, methodName, deviceName, nameof(deviceName));

			_is.ProcessMessage<Hub>(payload?.ToString(), token, deviceName);

			return Accepted();
		}
	}
}
