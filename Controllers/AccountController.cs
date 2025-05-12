using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Rest.API.Dtos.AccountDtos;
using Rest.API.Models;
using Rest.API.Services.Interfaces;

namespace Rest.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AccountController( IAuthService authService)
        {
            _authService = authService;
        }

        //[HttpPost("register")]
        //public async Task<IActionResult> Register(RegisterDto registerDto)
        //{

        //}

        //[HttpPost("login")]
        //public async Task<IActionResult> Login(LoginDto loginDto)
        //{
        //}
    }
}
