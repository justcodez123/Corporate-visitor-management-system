using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VisitorManagement.API.Data;
using VisitorManagement.API.Models;

namespace VisitorManagement.API.Controllers
{
    /// <summary>
    /// API Controller for managing visitor check-ins and check-outs.
    /// 
    /// Key concepts demonstrated:
    /// - [ApiController] attribute: Enables automatic model validation, binding, and 400 responses.
    /// - Dependency Injection: VisitorContext is injected via the constructor.
    /// - Async/Await: All database operations are async for better scalability.
    /// - DTOs: Input is validated through VisitorCheckInDto, not the raw entity.
    /// - RESTful routing: Follows standard REST conventions.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class VisitorsController : ControllerBase
    {
        private readonly VisitorContext _context;

        /// <summary>
        /// Constructor — EF Core context is injected by the DI container.
        /// You never create this yourself; ASP.NET Core handles it.
        /// </summary>
        public VisitorsController(VisitorContext context)
        {
            _context = context;
        }

        // ==========================================
        // GET: api/visitors
        // Retrieves all visitors. Supports ?search=name query.
        // ==========================================
        /// <summary>
        /// Get all visitors, optionally filtered by name search.
        /// </summary>
        /// <param name="search">Optional search term to filter by visitor name (case-insensitive).</param>
        /// <returns>List of visitors matching the criteria.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Visitor>>> GetVisitors([FromQuery] string? search)
        {
            // Start with all visitors, ordered by most recent check-in
            IQueryable<Visitor> query = _context.Visitors
                .OrderByDescending(v => v.CheckInTime);

            // If a search term is provided, filter by FullName (case-insensitive)
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(v => v.FullName.ToLower().Contains(search.ToLower()));
            }

            return await query.ToListAsync();
        }

        // ==========================================
        // GET: api/visitors/active
        // Retrieves only visitors currently in the building.
        // ==========================================
        /// <summary>
        /// Get all visitors who are currently checked in (no check-out time recorded).
        /// This powers the "Active Visitors" dashboard on the frontend.
        /// </summary>
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<Visitor>>> GetActiveVisitors()
        {
            var activeVisitors = await _context.Visitors
                .Where(v => v.CheckOutTime == null)           // Still in the building
                .OrderByDescending(v => v.CheckInTime)        // Most recent first
                .ToListAsync();

            return activeVisitors;
        }

        // ==========================================
        // GET: api/visitors/{id}
        // Retrieves a specific visitor by ID.
        // ==========================================
        /// <summary>
        /// Get a specific visitor by their ID.
        /// Returns 404 if not found.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Visitor>> GetVisitor(int id)
        {
            var visitor = await _context.Visitors.FindAsync(id);

            if (visitor == null)
            {
                return NotFound(new { message = $"Visitor with ID {id} not found." });
            }

            return visitor;
        }

        // ==========================================
        // POST: api/visitors
        // Check-in a new visitor. CheckInTime is auto-set.
        // ==========================================
        /// <summary>
        /// Check in a new visitor. The CheckInTime is automatically set to the current UTC time.
        /// 
        /// The [ApiController] attribute ensures that if the DTO validation fails,
        /// ASP.NET Core automatically returns a 400 Bad Request with validation errors.
        /// You don't need to manually check ModelState.IsValid.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Visitor>> CheckIn([FromBody] VisitorCheckInDto dto)
        {
            // Map the DTO to the entity
            var visitor = new Visitor
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Company = dto.Company,
                HostName = dto.HostName,
                Purpose = dto.Purpose,
                BadgeNumber = dto.BadgeNumber,
                CheckInTime = DateTime.UtcNow,   // Auto-set — client cannot override this
                CheckOutTime = null               // Not checked out yet
            };

            _context.Visitors.Add(visitor);
            await _context.SaveChangesAsync();

            // Return 201 Created with the location of the new resource
            return CreatedAtAction(
                nameof(GetVisitor),
                new { id = visitor.Id },
                visitor
            );
        }

        // ==========================================
        // PUT: api/visitors/{id}/checkout
        // Records the check-out time for a visitor.
        // ==========================================
        /// <summary>
        /// Check out a visitor by recording their CheckOutTime.
        /// Returns 400 if the visitor has already checked out.
        /// Returns 404 if the visitor doesn't exist.
        /// </summary>
        [HttpPut("{id}/checkout")]
        public async Task<IActionResult> CheckOut(int id)
        {
            var visitor = await _context.Visitors.FindAsync(id);

            if (visitor == null)
            {
                return NotFound(new { message = $"Visitor with ID {id} not found." });
            }

            if (visitor.CheckOutTime != null)
            {
                return BadRequest(new { message = "This visitor has already checked out." });
            }

            visitor.CheckOutTime = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(visitor);
        }

        // ==========================================
        // DELETE: api/visitors/{id}
        // Deletes a visitor record.
        // ==========================================
        /// <summary>
        /// Delete a visitor record permanently.
        /// Returns 404 if the visitor doesn't exist.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVisitor(int id)
        {
            var visitor = await _context.Visitors.FindAsync(id);

            if (visitor == null)
            {
                return NotFound(new { message = $"Visitor with ID {id} not found." });
            }

            _context.Visitors.Remove(visitor);
            await _context.SaveChangesAsync();

            return NoContent(); // 204 — standard REST response for successful delete
        }
    }
}
