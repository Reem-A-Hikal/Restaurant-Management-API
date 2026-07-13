using Rest.Domain.Entities.Enums;
using Rest.Domain.Exceptions;

namespace Rest.Domain.Entities
{
    public class Category
    {
        public int CategoryId { get; private set; }

        public string Name { get; private set; }

        public CategoryStatus Status { get; private set; } = CategoryStatus.Active;

        public int DisplayOrder { get; private set; }

        // Navigation property
        public virtual ICollection<Product> Products { get; private set; } = new List<Product>();

        public static Category Create(string name, int displayOrder, CategoryStatus status = CategoryStatus.Active)
        {
            ValidateName(name);

            return new Category
            {
                Name = name,
                DisplayOrder = displayOrder,
                Status = status
            };
        }

        #region Domain Methods
        public void UpdateDetails(string? name, int? displayOrder)
        {
            if (name != null)
            {
                ValidateName(name);
                Name = name;
            }

            if (displayOrder.HasValue)
            {
                if (displayOrder < 0)
                    throw new ValidationException("Display order cannot be negative.");
                DisplayOrder = displayOrder.Value;
            }
        }

        public void Activate() => Status = CategoryStatus.Active;

        public void Deactivate() => Status = CategoryStatus.Inactive;

        /// <summary>
        /// Archives the category and cascades unavailability to all its products.
        /// </summary>
        public void Archive()
        {
            if (Status == CategoryStatus.Archived)
                throw new BusinessException("Category is already archived");

            Status = CategoryStatus.Archived;

            foreach (var product in Products)
                product.Deactivate();
        }
        #endregion

        private static void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ValidationException("Category name is required.");

            if (name.Length > 50)
                throw new ValidationException("Category name cannot exceed 50 characters.");
        }
    }
}
