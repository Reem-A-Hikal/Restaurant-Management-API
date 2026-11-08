using Rest.Application.Validation;
using System.ComponentModel.DataAnnotations;

namespace Rest.Application.Dtos.UserDtos
{
    /// <summary>
    /// User profile update data transfer object
    /// </summary>
    public class UpdateProfileDto
    {
        /// <summary>
        /// User's full name
        /// </summary>
        /// <example>John Doe</example>
        [StringLength(100, MinimumLength = 3)]
        public string? FullName { get; set; }

        /// <summary>
        /// User's phone number
        /// </summary>
        /// <example>01234567890</example>
        [RegularExpression(@"^01[0125]\d{8}$", ErrorMessage = "Invalid phone number format")]
        public string? PhoneNumber { get; set; }

        /// <summary>
        ///  profile image
        /// </summary>
        /// <example>user.jpg</example>
        [LocalImageUrl(ImageUrlRules.Users)]
        [StringLength(255)]
        public string? ProfileImageUrl { get; set; }
    }
}
