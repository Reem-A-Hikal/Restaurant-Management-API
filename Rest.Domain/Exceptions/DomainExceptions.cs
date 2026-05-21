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
        protected DomainException(string message) : base(message) { }
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
        public BusinessException(string message) : base(message) { }
    }

    /// <summary>
    /// Thrown when input validation fails
    /// Example: Email already exists
    /// </summary>
    public class ValidationException : DomainException
    {
        public IEnumerable<string> Errors { get; set; }

        public ValidationException(string message) : base(message)
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
        public ForbiddenException(string message = "You are not authorized to perform this action.")
            : base(message) { }
    }
}
