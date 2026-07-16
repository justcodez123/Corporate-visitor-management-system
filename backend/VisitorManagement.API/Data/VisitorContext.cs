using Microsoft.EntityFrameworkCore;
using VisitorManagement.API.Models;

namespace VisitorManagement.API.Data
{
    /// <summary>
    /// Entity Framework Core database context for the Visitor Management System.
    /// 
    /// This class is the bridge between your C# code and the SQLite database.
    /// EF Core uses this to:
    ///   1. Map the Visitor class to a database table.
    ///   2. Track changes to entities.
    ///   3. Generate SQL queries from LINQ expressions.
    ///   
    /// Why SQLite?
    /// - Zero-config: No database server to install or manage.
    /// - File-based: Data persists in a single .db file in your project.
    /// - Portable: The database file can be moved, backed up, or shared easily.
    /// - Production-ready for small-to-medium apps like visitor management.
    /// - Easy to swap: Change one line in Program.cs to switch to SQL Server/PostgreSQL.
    /// </summary>
    public class VisitorContext : DbContext
    {
        public VisitorContext(DbContextOptions<VisitorContext> options) : base(options)
        {
        }

        /// <summary>
        /// Represents the "Visitors" table in the database.
        /// Each Visitor object maps to a row in this table.
        /// </summary>
        public DbSet<Visitor> Visitors { get; set; }

        /// <summary>
        /// Configure the model — add indexes, seed data, etc.
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Add an index on FullName for faster search queries
            modelBuilder.Entity<Visitor>()
                .HasIndex(v => v.FullName);

            // Add an index on CheckOutTime to quickly filter active visitors
            modelBuilder.Entity<Visitor>()
                .HasIndex(v => v.CheckOutTime);
        }
    }
}
