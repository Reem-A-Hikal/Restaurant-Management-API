using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
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
        private const string Admin = "Admin";
        private readonly IImageUploadService _imageUploadService;

        public UploadController(IImageUploadService imageUploadService)
        {
            _imageUploadService = imageUploadService;
        }

        /// Everyone can upload a profile picture ("users");
        /// only Admin can upload product images.
        [HttpPost("image")]
        [Authorize(Roles = "Admin,Customer,Chef,DeliveryPerson")]
        [EnableRateLimiting("UploadPolicy")]
        [RequestSizeLimit(6 * 1024 * 1024)]
        [SwaggerOperation(Summary = "Upload an image", Description = "Saves the image under wwwroot/images/{folder} and returns its URL.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Returns { imageUrl }")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid file")]
        public async Task<IActionResult> UploadImage(IFormFile file, [FromQuery] string folder = "products")
        {
            if (file is null || file.Length == 0)
                return ValidationErrorResponse(new[] { "No file was uploaded." });

            var allowed = User.IsInRole(Admin) ? new[] { "products", "users" } : new[] { "users" };

            if (!allowed.Contains(folder))
                return ForbiddenResponse($"You can only upload to: {string.Join(", ", allowed)}");

            await using var stream = file.OpenReadStream();
            var url = await _imageUploadService.UploadAsync(stream, file.FileName, file.Length, folder);

            return SuccessResponse(new { imageUrl = url }, "Image uploaded successfully");
        }

        [HttpDelete("image")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Delete an uploaded image")]
        public IActionResult DeleteImage([FromQuery] string url)
        {
            _imageUploadService.Delete(url);
            return SuccessResponse<string>(null!, "Image deleted successfully");
        }
    }
}
