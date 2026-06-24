using System.ComponentModel.DataAnnotations;

namespace Rest.Application.Dtos.OrderDtos
{
    public class CancelOrderDto
    {
        [Required, StringLength(1000)]
        public string CancellationReason { get; set; }
    }
}
