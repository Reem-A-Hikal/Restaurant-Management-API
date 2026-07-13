using Rest.Domain.Exceptions;

namespace Rest.Domain.Entities
{
    public class Chef :User
    {
        public string Specialization { get; set; }

        public static Chef Create(
            string email, string userName, string fullName,
            string? phoneNumber, string? profileImageUrl, string specialization)
        {
            var chef = InitializeBase(new Chef(), email, userName, fullName, phoneNumber, profileImageUrl);
            chef.SetSpecialization(specialization);
            return chef;
        }

        public void SetSpecialization(string specialization)
        {
            if (string.IsNullOrWhiteSpace(specialization))
                throw new ValidationException("Specialization is required for a Chef.");

            Specialization = specialization;
        }
    }
}
