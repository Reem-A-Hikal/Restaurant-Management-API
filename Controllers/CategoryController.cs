using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Rest.API.Services.Interfaces;

namespace Rest.API.Controllers
{
    /// <summary>
    /// Controller for managing categories.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService productService;
        private readonly ILogger<CategoryController> logger;

        /// <summary>
        /// Constructor for CategoryController.
        /// </summary>
        /// <param name="categoryService"></param>
        /// <param name="logger"></param>
        public CategoryController(ICategoryService categoryService, ILogger<CategoryController> logger)
        {
            this.productService = categoryService;
            this.logger = logger;
        }

    }
}
