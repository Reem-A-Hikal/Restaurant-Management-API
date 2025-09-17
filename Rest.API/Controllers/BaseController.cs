using Microsoft.AspNetCore.Mvc;
using Rest.API.Responses;

namespace Rest.API.Controllers
{
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected IActionResult SuccessResponse<T>(T data, string message = "Success")
        {
            return Ok(ApiResponse<T>.SuccessResponse(data, message));
        }

        protected IActionResult CreatedResponse<T>(string actionName, object routeValues, T data, string message = "Created successfully")
        {
            return CreatedAtAction(actionName, routeValues, ApiResponse<T>.SuccessResponse(data, message));
        }

        protected IActionResult ValidationErrorResponse(IEnumerable<string> errors, string message = "Validation Error")
        {
            return BadRequest(ApiResponse<string>.FailResponse([.. errors], message));
        }

        protected IActionResult ErrorResponse(IEnumerable<string> errors, string message = "Error")
        {
            return BadRequest(ApiResponse<string>.FailResponse([.. errors], message));
        }

        protected IActionResult InternalErrorResponse(Exception ex, string message = "Internal server error")
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<string>.FailResponse(new List<string> { ex.Message }, message));
        }
    }
}
