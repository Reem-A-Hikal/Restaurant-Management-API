namespace Rest.Application.Dtos.DeliveryDtos
{
    public class AssignDeliveryDto
    {
        /// <summary>
        /// null = auto-assign, not-null = manual override (Admin only)
        /// </summary>
        public string? DeliveryPersonId { get; set; }
    }
}
