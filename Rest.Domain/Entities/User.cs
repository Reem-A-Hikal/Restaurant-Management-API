using Microsoft.AspNetCore.Identity;
using Rest.Domain.Entities.Enums;
using Rest.Domain.Exceptions;

namespace Rest.Domain.Entities
{
    public class User : IdentityUser
    {
        public string FullName { get; protected set; } = null!;
        public string? ProfileImageUrl { get; protected set; }
        public UserStatus Status { get; protected set; } = UserStatus.Active;
        public DateTime JoinDate { get; protected set; } = DateTime.UtcNow;
        public DateTime? LastLoginDate { get; protected set; }

        public bool IsActive => Status == UserStatus.Active;
        public bool IsDeleted => Status == UserStatus.Deleted;

        // Navigation properties
        public virtual ICollection<Order> CustomerOrders { get; set; } = [];
        public virtual ICollection<Address> Addresses { get; set; } = [];
        public virtual ICollection<Review> Reviews { get; set; } = [];


        #region Factory Method
        public static User Create(string email, string userName, string fullName, string? phoneNumber, string? profileImageUrl)
            => InitializeBase(new User(), email, userName, fullName, phoneNumber, profileImageUrl);

        /// <summary>
        /// Shared initialization used by User and its subclasses' factory methods
        /// (Chef.Create, DeliveryPerson.Create). Keeps validation and base field
        /// assignment in one place instead of duplicating it per role.
        /// </summary>
        protected static TUser InitializeBase<TUser>(
            TUser user, string email, string userName, string fullName,
            string? phoneNumber, string? profileImageUrl) where TUser : User
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ValidationException("Email is required.");

            ValidateFullName(fullName);

            user.Email = email;
            user.UserName = string.IsNullOrWhiteSpace(userName) ? email : userName;
            user.FullName = fullName;
            user.PhoneNumber = phoneNumber;
            user.ProfileImageUrl = profileImageUrl;
            user.Status = UserStatus.Active;
            user.JoinDate = DateTime.UtcNow;

            return user;
        }

        protected static void ValidateFullName(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName) || fullName.Length < 3 || fullName.Length > 100)
                throw new ValidationException("Full name must be between 3 and 100 characters.");
        }
        #endregion

        #region Domain Methods
        public void UpdateProfile(string? fullName, string? phoneNumber, string? profileImageUrl)
        {
            if (!string.IsNullOrWhiteSpace(fullName))
            {
                ValidateFullName(fullName);
                FullName = fullName;
            }

            if (!string.IsNullOrWhiteSpace(phoneNumber))
                PhoneNumber = phoneNumber;

            if (!string.IsNullOrWhiteSpace(profileImageUrl))
                ProfileImageUrl = profileImageUrl;
        }

        public void ChangeStatus(UserStatus status)
        {
            if (status == UserStatus.Deleted)
                throw new BusinessException("Use MarkAsDeleted() to delete a user, not ChangeStatus().");

            Status = status;
        }

        public void MarkAsDeleted()
        {
            Status = UserStatus.Deleted;
            Email = $"deleted_{Id}@deleted.com";
            UserName = $"deleted_{Id}";
            FullName = $"deleted_{Id}";
            PhoneNumber = null;
            ProfileImageUrl = null;
        }

        public void RecordLogin() => LastLoginDate = DateTime.UtcNow;
        #endregion
    }
}
