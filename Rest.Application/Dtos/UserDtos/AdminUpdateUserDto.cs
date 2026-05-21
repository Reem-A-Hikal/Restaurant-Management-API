using Rest.Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Rest.Application.Dtos.UserDtos
{
    /// <summary>
    /// DTO for Admin to update user - restricted to admin-allowed fields only
    /// </summary>
    public class AdminUpdateUserDto
    {
        /// <summary>
        /// Activate or deactivate the user account
        /// </summary>
        public UserStatus Status { get; set; }
        public string? Specialization { get; set; }
        public string? VehicleNumber { get; set; }
        public bool? IsAvailable { get; set; }
    }
}
