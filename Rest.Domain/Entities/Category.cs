using Rest.Domain.Entities.Enums;
using System.ComponentModel.DataAnnotations;

namespace Rest.Domain.Entities
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public CategoryStatus Status { get; set; } = CategoryStatus.Active;

        public int DisplayOrder { get; set; }

        // Navigation property
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
