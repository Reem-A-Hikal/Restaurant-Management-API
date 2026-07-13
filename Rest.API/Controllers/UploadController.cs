using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rest.Application.Interfaces.IServices;
using Swashbuckle.AspNetCore.Annotations;

namespace Rest.API.Controllers
{
    /// <summary>
    /// Generic image upload controller — used by Product images,
    /// User profile images, etc.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UploadController : BaseController
    {
        private readonly IImageUploadService _imageUploadService;

        public UploadController(IImageUploadService imageUploadService)
        {
            _imageUploadService = imageUploadService;
        }

        [HttpPost("image")]
        [Authorize(Roles = "Admin, Customer")]
        [SwaggerOperation(Summary = "Upload an image", Description = "Saves the image under wwwroot/images/{folder} and returns its URL.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Returns { imageUrl }")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid file")]
        public async Task<IActionResult> UploadImage(IFormFile file, [FromQuery] string folder = "products")
        {
            var allowedFolders = new[] { "products", "users" };
            if (!allowedFolders.Contains(folder))
                return ValidationErrorResponse(new[] { $"Invalid folder. Allowed: {string.Join(", ", allowedFolders)}" });

            await using var stream = file.OpenReadStream();
            var url = await _imageUploadService.UploadAsync(stream, file.FileName, file.Length, folder);

            return SuccessResponse(new { imageUrl = url }, "Image uploaded successfully");
        }
    }
}
