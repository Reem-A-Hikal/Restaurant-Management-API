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
    /// Admin can ONLY update:
    /// 1. Status (Active, Inactive, Suspended)
    /// 2. Role-specific data (Specialization, VehicleNumber, IsAvailable)
    /// </summary>
    public class AdminUpdateUserDto
    {
        /// <summary>
        /// Change user status - Admin can set Active, Inactive, or Suspended
        /// Cannot set Deleted (use Delete endpoint instead)
        /// </summary>
        public UserStatus? Status { get; set; }
        /// <summary>
        /// Chef specialization (Chef role only - ignored for other roles)
        /// </summary>
        [StringLength(100, ErrorMessage = "Specialization cannot exceed 100 characters")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Specialization { get; set; }
        /// <summary>
        /// Vehicle number (DeliveryPerson role only - ignored for other roles)
        /// </summary>
        [StringLength(20, ErrorMessage = "Vehicle number cannot exceed 20 characters")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? VehicleNumber { get; set; }

        /// <summary>
        /// Availability (DeliveryPerson role only - ignored for other roles)
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? IsAvailable { get; set; }
    }
}
