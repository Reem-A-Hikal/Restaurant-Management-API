using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Rest.Application.Dtos.AccountDtos;
using Rest.Application.Interfaces.IServices;
using Rest.Domain.Entities;
using Rest.Domain.Exceptions;

namespace Rest.Application.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<User> _userManager;
        private readonly IAuthService _authService;
        private readonly UserCreationHelper _creationHelper;
        private readonly IMapper _mapper;

        public AccountService(UserManager<User> userManager, IAuthService authService, UserCreationHelper creationHelper, IMapper mapper)
        {
            _userManager = userManager;
            _authService = authService;
            _creationHelper = creationHelper;
            _mapper = mapper;
        }
        public async Task RegisterAsync(RegisterDto registerDto)
        {
            var user = _mapper.Map<User>(registerDto);
            await _creationHelper.CreateAndAssignRoleAsync(user, registerDto.Password, "Customer");
        }
        public async Task<LoginResponse> LoginAsync(LoginDto loginDto)
        {
            
            var user = await _authService.ValidateUserCredentialsAsync(loginDto.Email, loginDto.Password) 
                ?? throw new ValidationException("Invalid email or password");

            var token = await _authService.GenerateJwtTokenAsync(user);
            var userRoles = await _userManager.GetRolesAsync(user);
            user.LastLoginDate = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            return new LoginResponse()
            {
                UserId = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Token = token,
                Role = userRoles[0],

            };
            
        }
    }
}
