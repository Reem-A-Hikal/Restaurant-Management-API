using System.ComponentModel.DataAnnotations;

namespace Rest.Application.Dtos.OrderDtos
{
    public class ConfirmOrderDto
    {
        [StringLength(1000)]
        public string? StaffNotes { get; set; }
        public DateTime? RequiredTime { get; set; }
    }
}
