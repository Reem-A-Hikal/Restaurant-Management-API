using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Rest.Application.Interfaces.IServices;

namespace Rest.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuController : BaseController
    {
        private readonly IMenuService _menuService;
        public MenuController(IMenuService menuService) => _menuService = menuService;

        /// <summary>
        /// The public menu: active categories with available dishes only.
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get() =>
            SuccessResponse(await _menuService.GetMenuAsync(), "Menu retrieved successfully");
    }
}
