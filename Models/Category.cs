using System.ComponentModel.DataAnnotations;

namespace Rest.API.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [StringLength(255)]
        public string Image { get; set; }

        public bool IsActive { get; set; }

        public int DisplayOrder { get; set; }

        // Navigation property
        public virtual ICollection<Product> Products { get; set; }
    }
}
