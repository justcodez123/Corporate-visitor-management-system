using System.ComponentModel.DataAnnotations;

namespace VisitorManagement.API.Models
{
    /// <summary>
    /// Represents a visitor who checks into the corporate building.
    /// This is the main entity stored in the database.
    /// </summary>
    public class Visitor
    {
        /// <summary>
        /// Primary key — auto-incremented by SQLite.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Full name of the visitor. Required field.
        /// </summary>
        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Email address of the visitor. Must be a valid email format.
        /// </summary>
        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Phone number in Indian format: +91XXXXXXXXXX or 10 digits.
        /// Validated via regex on the DTO layer.
        /// </summary>
        [Required]
        [StringLength(15)]
        public string PhoneNumber { get; set; } = string.Empty;

        /// <summary>
        /// The company the visitor belongs to (optional for walk-in guests).
        /// </summary>
        [StringLength(100)]
        public string? Company { get; set; }

        /// <summary>
        /// Name of the employee the visitor is here to meet.
        /// </summary>
        [Required]
        [StringLength(100)]
        public string HostName { get; set; } = string.Empty;

        /// <summary>
        /// Purpose of the visit (e.g., Meeting, Interview, Delivery).
        /// </summary>
        [Required]
        [StringLength(200)]
        public string Purpose { get; set; } = string.Empty;

        /// <summary>
        /// Automatically set when the visitor checks in (POST).
        /// </summary>
        public DateTime CheckInTime { get; set; }

        /// <summary>
        /// Set when the visitor checks out (PUT). Null while still in the building.
        /// </summary>
        public DateTime? CheckOutTime { get; set; }

        /// <summary>
        /// Badge number assigned to the visitor at check-in.
        /// </summary>
        [StringLength(20)]
        public string? BadgeNumber { get; set; }
    }
}
