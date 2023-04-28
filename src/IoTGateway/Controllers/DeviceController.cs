using Microsoft.AspNetCore.Mvc;
using IoTGatewayCore.Models.Object;
using Microsoft.Extensions.Logging;
using System.Reflection;
using IoTGatewayCore.U;
using IoTGatewayCore.IS;

namespace IoTGateway.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class DeviceController : ControllerBase
	{
		private readonly LS<DeviceController> _ls;
		private readonly IS _is;

		public DeviceController(LS<DeviceController> ls, IS @is)
		{
			_ls = ls;
			_is = @is;
		}

		[HttpPost("Telemetry")]
		public IActionResult Telemetry([FromBody] object payload, [FromHeader] string token)
		{
			_ls.TokenCheck(LogLevel.Warning, MethodBase.GetCurrentMethod()?.Name, token);

			_is.ProcessMessage<Device>(payload?.ToString(), token);

			return Accepted();
		}
	}
}
