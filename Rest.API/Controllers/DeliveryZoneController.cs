using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rest.Application.Dtos.DeliveryZoneDtos;
using Rest.Application.Interfaces.IServices;

namespace Rest.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeliveryZoneController : BaseController
    {
        private readonly IDeliveryZone _zone;

        public DeliveryZoneController(IDeliveryZone zone) => _zone = zone;

        /// <summary>
        /// Where the kitchen delivers. Public: the menu and address forms need it before sign-in.
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Get() =>
            SuccessResponse(new DeliveryZoneDto
            {
                Governorate = _zone.Governorate,
                Areas = _zone.Areas,
            });
    }
}
