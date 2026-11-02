using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rest.Domain.Exceptions
{
    /// <summary>
    /// Base exception for all domain exceptions
    /// </summary>
    public class DomainException : Exception
    {
        /// <summary>
        /// Machine-readable code clients translate. Null when an error has none yet.
        /// </summary>
        public string? Code { get; }

        /// <summary>
        /// Values the translated message needs (e.g. opening hours).
        /// </summary>
        public IReadOnlyDictionary<string, object?>? Params { get; }
        protected DomainException(
            string message,
            string? code = null,
            IReadOnlyDictionary<string, object?>? parameters = null) : base(message)
        {
            Code = code;
            Params = parameters;
        }
    }


    /// <summary>
    /// Thrown when a requested resource is not found
    /// Example: User with ID 5 not found
    /// </summary>
    public class NotFoundException : DomainException
    {
        public NotFoundException(string resourceName, object key)
            : base($"{resourceName} with ID '{key}' was not found.") { }

        public NotFoundException(string message)
            : base(message) { }
    }

    /// <summary>
    /// Thrown when a business rule is violated
    /// Example: Cannot cancel a delivered order
    /// </summary>
    public class BusinessException : DomainException
    {
        public BusinessException(
            string message,
            string? code = null,
            IReadOnlyDictionary<string, object?>? parameters = null) 
            : base(message, code, parameters) { }
    }

    /// <summary>
    /// Thrown when input validation fails
    /// Example: Email already exists
    /// </summary>
    public class ValidationException : DomainException
    {
        public IEnumerable<string> Errors { get; set; }

        public ValidationException(string message, string? code = null) 
            : base(message, code)
        {
            Errors = new[] { message };
        }

        public ValidationException(IEnumerable<string> errors)
            : base("One or more validation errors occurred.")
        {
            Errors = errors;
        }
    }

    /// <summary>
    /// Thrown when user is not authorized to perform an action
    /// Example: Non-admin trying to delete a user
    /// </summary>
    public class ForbiddenException : DomainException
    {
        public ForbiddenException(
            string message = "You are not authorized to perform this action.",
            string? code = null)
            : base(message, code) { }
    }
}
