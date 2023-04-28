using IoTGatewayCore.DB;
using Microsoft.AspNetCore.Mvc;

namespace IoTGateway.Controllers
{
	[ApiController]
	[Route("Test")]
	public class TestController : Controller
	{
		private readonly DeviceDB _db;

		public TestController(DeviceDB db)
		{
			_db = db;
		}

		[HttpGet]
		public IActionResult Index()
		{
			return Accepted();
		}
	}
}
