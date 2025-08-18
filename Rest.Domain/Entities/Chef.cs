using System.ComponentModel.DataAnnotations;

namespace Rest.Domain.Entities
{
    public class Chef :User
    {
        [Required]
        public string Specialization { get; set; }
    }
}
