using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Rest.API.Services.Interfaces;

namespace Rest.API.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IMapper mapper;

        public AuthService(IMapper mapper)
        {
            this.mapper = mapper;
        }
    }
}
