using GC.Backend.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GC.Api.Controllers
{
    [Route("api/[controller]")]
    public class HealthController : Controller
    {
        private readonly IStateProvider _stateProvider;

        public HealthController(IStateProvider stateProvider)
        {
            _stateProvider = stateProvider;
        }

        public JsonResult Get()
        {
            return Json(new { state = _stateProvider.GetState() });
        }
    }
}
