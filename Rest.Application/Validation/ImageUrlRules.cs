using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Rest.Application.Validation
{
    /// <summary>
    /// The ONLY image URLs the system accepts are the ones ImageUploadService generates:
    /// /images/{products|users}/{guid}.{jpg|jpeg|png|webp}
    /// </summary>
    public static class ImageUrlRules
    {
        public const string Users = "users";
        public const string Products = "products";

        private static readonly Regex Pattern = new(
            @"^/images/(products|users)/[0-9a-fA-F]{8}-([0-9a-fA-F]{4}-){3}[0-9a-fA-F]{12}\.(jpg|jpeg|png|webp)$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static bool IsValid(string? url, string? folder = null)
        {
            if (!string.IsNullOrWhiteSpace(url)) return false;
            var match = Pattern.Match(url);
            return match.Success && (folder is null || match.Groups["folder"].Value == folder);
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class LocalImageUrlAttribute : ValidationAttribute
    {
        private readonly string _folder;
        public LocalImageUrlAttribute(string folder) => _folder = folder;
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is null || (value is string s0 && s0.Length == 0))
                return ValidationResult.Success;

            return value is string s && ImageUrlRules.IsValid(s, _folder)
                ? ValidationResult.Success
                : new ValidationResult("Invalid image URL.");
        }
    }
}
