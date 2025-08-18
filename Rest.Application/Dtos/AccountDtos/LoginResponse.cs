using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rest.Application.Dtos.AccountDtos
{
    public class LoginResponse
    {
        public string Token { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public IList<string> Roles { get; set; }

    }
}
