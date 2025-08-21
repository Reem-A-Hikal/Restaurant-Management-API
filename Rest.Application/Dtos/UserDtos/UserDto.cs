using Rest.Application.Dtos.AddressDtos;

namespace Rest.Application.Dtos.UserDtos
{
    public class UserDto
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string? ProfileImageUrl { get; set; }
        public DateTime JoinDate { get; set; }
        public bool IsActive { get; set; }
        public IList<string> Roles { get; set; } = [];

        public List<AddressDto> Addresses { get; set; } = [];

    }
}
