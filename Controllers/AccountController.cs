using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Rest.API.Dtos.AccountDtos;
using Rest.API.Models;
using System.Security.Claims;

namespace Rest.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> userManager;
        private readonly IMapper mapper;
        private readonly SignInManager<User> signInManager;

        public AccountController(
            UserManager<User> userManager,
            IMapper mapper,
            SignInManager<User> signInManager )
        {
            this.userManager = userManager;
            this.mapper = mapper;
            this.signInManager = signInManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var existingUser = await userManager.FindByEmailAsync(registerDto.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "Email already in use");
                    return BadRequest(ModelState);
                }

                var user = mapper.Map<User>(registerDto);

                var result = await userManager.CreateAsync(user, registerDto.Password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Customer");
                    return Ok(new 
                    {
                        message = "User registered successfully"
                    });
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during registration", error = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var user = await userManager.FindByEmailAsync(loginDto.Email);
                if (user == null)
                {
                    return BadRequest(new { message = "Invalid email or password" });
                }
                var result = await signInManager.PasswordSignInAsync(
                    loginDto.Email,
                    loginDto.Password,
                    isPersistent: false,
                    lockoutOnFailure: false
                );

                if (result.Succeeded)
                {
                    return Ok(new { message = "Login successful", userId = user.Id });
                }
                return BadRequest(new { message = "Invalid email or password" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during login", error = ex.Message });
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await signInManager.SignOutAsync();
                return Ok(new { message = "Logout successful" });
            }
            catch(Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during logout", error = ex.Message });
            }
        }

        [HttpGet("getUser/{userId}")]
        [Authorize]
        public async Task<IActionResult> GetUser(string userId)
        {
            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (currentUserId != userId && !User.IsInRole("Admin"))
                {
                    //return Unauthorized();
                    return StatusCode(403, new
                    {
                        message = "You can only access your own profile"
                    });
                }
                //var user = await userManager.GetUserAsync(User);
                var user = await userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }
                return Ok(new
                {
                    id = user.Id,
                    email = user.Email,
                    fullName = user.FullName,
                    phoneNumber = user.PhoneNumber,
                    profileImageUrl = user.ProfileImageUrl,
                    joinDate = user.JoinDate,
                    isActive = user.IsActive
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving user", error = ex.Message });
            }
        }
    }
}
