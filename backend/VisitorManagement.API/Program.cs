using Microsoft.EntityFrameworkCore;
using VisitorManagement.API.Data;

var builder = WebApplication.CreateBuilder(args);

// ============================================
// SERVICES CONFIGURATION
// ============================================

// Register controllers (our VisitorsController)
builder.Services.AddControllers();

// Register Swagger for API documentation and testing
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register the database context with SQLite
// Why SQLite over InMemory?
// 1. Data persists between app restarts — you don't lose visitor records.
// 2. No database server to install/manage — it's just a file.
// 3. Perfect for small-to-medium scale apps like visitor management.
// 4. Easy to swap later: change this one line to UseSqlServer() or UseNpgsql().
builder.Services.AddDbContext<VisitorContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure CORS to allow the Angular frontend (localhost:4200) to call our API
// Without this, the browser blocks cross-origin requests.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200")  // Angular dev server
              .AllowAnyHeader()                       // Accept any HTTP header
              .AllowAnyMethod();                      // Accept GET, POST, PUT, DELETE, etc.
    });
});

var app = builder.Build();

// ============================================
// MIDDLEWARE PIPELINE
// The order of middleware matters!
// ============================================

// Enable Swagger UI in development mode
// Visit: http://localhost:5062/swagger to test your endpoints
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Apply the CORS policy — must be before UseAuthorization
app.UseCors("AllowAngularApp");

app.UseAuthorization();

app.MapControllers();

// Ensure the database is created on startup
// This creates the SQLite database file and tables if they don't exist.
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<VisitorContext>();
    context.Database.EnsureCreated();
}

app.Run();
