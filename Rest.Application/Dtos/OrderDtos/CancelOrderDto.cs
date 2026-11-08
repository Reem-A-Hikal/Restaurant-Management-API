using System.ComponentModel.DataAnnotations;

namespace Rest.Application.Dtos.OrderDtos
{
    public class CancelOrderDto
    {
        [Required, StringLength(250, MinimumLength = 3)]
        public string CancellationReason { get; set; } = string.Empty;
    }
}
