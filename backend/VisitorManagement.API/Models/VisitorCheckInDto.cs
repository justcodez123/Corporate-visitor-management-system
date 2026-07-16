using System.ComponentModel.DataAnnotations;

namespace VisitorManagement.API.Models
{
    /// <summary>
    /// Data Transfer Object for checking in a new visitor.
    /// Contains validation attributes to enforce data integrity before persisting.
    /// 
    /// Why a DTO instead of using the entity directly?
    /// - Prevents clients from setting fields they shouldn't (Id, CheckInTime, CheckOutTime).
    /// - Validation rules live here, keeping the entity model clean.
    /// - This is a common pattern in enterprise .NET applications.
    /// </summary>
    public class VisitorCheckInDto
    {
        /// <summary>
        /// Full name of the visitor. Cannot be empty.
        /// </summary>
        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Full name must be between 2 and 100 characters.")]
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Email address. Must be a valid email format.
        /// </summary>
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Please provide a valid email address.")]
        [StringLength(150)]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Phone number in Indian format.
        /// Accepts: +91XXXXXXXXXX, 91XXXXXXXXXX, or plain 10-digit XXXXXXXXXX.
        /// </summary>
        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^(\+91[\-\s]?)?[0]?(91)?[6-9]\d{9}$",
            ErrorMessage = "Please provide a valid Indian phone number (e.g., +919876543210 or 9876543210).")]
        public string PhoneNumber { get; set; } = string.Empty;

        /// <summary>
        /// The company the visitor represents. Optional.
        /// </summary>
        [StringLength(100)]
        public string? Company { get; set; }

        /// <summary>
        /// Name of the host (employee) the visitor is here to meet.
        /// </summary>
        [Required(ErrorMessage = "Host name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Host name must be between 2 and 100 characters.")]
        public string HostName { get; set; } = string.Empty;

        /// <summary>
        /// Reason for the visit.
        /// </summary>
        [Required(ErrorMessage = "Purpose of visit is required.")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Purpose must be between 3 and 200 characters.")]
        public string Purpose { get; set; } = string.Empty;

        /// <summary>
        /// Optional badge number assigned at check-in.
        /// </summary>
        [StringLength(20)]
        public string? BadgeNumber { get; set; }
    }
}
