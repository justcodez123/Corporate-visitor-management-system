# Walkthrough — Phase 1: Backend Complete

## What Was Built

A fully functional **ASP.NET Core 8 Web API** for the Corporate Visitor Management System with:

### Files Created

| File | Purpose |
|------|---------|
| [Visitor.cs](file:///home/ash/Desktop/Corporate%20Visitor%20Management%20System/backend/VisitorManagement.API/Models/Visitor.cs) | Entity model — maps to the `Visitors` table in SQLite |
| [VisitorCheckInDto.cs](file:///home/ash/Desktop/Corporate%20Visitor%20Management%20System/backend/VisitorManagement.API/Models/VisitorCheckInDto.cs) | Input DTO with `[Required]`, `[EmailAddress]`, `[RegularExpression]` validation |
| [VisitorContext.cs](file:///home/ash/Desktop/Corporate%20Visitor%20Management%20System/backend/VisitorManagement.API/Data/VisitorContext.cs) | EF Core DbContext with indexes on FullName and CheckOutTime |
| [VisitorsController.cs](file:///home/ash/Desktop/Corporate%20Visitor%20Management%20System/backend/VisitorManagement.API/Controllers/VisitorsController.cs) | Full CRUD controller with 6 endpoints |
| [Program.cs](file:///home/ash/Desktop/Corporate%20Visitor%20Management%20System/backend/VisitorManagement.API/Program.cs) | Service registration — SQLite, CORS, Swagger, auto DB creation |
| [appsettings.json](file:///home/ash/Desktop/Corporate%20Visitor%20Management%20System/backend/VisitorManagement.API/appsettings.json) | Connection string for SQLite (`visitors.db`) |

### API Endpoints — All Tested ✅

| Method | Endpoint | Status | Notes |
|--------|----------|--------|-------|
| `POST` | `/api/visitors` | ✅ 201 | Auto-sets `CheckInTime`, returns created visitor |
| `GET` | `/api/visitors` | ✅ 200 | Returns all visitors, supports `?search=name` |
| `GET` | `/api/visitors/active` | ✅ 200 | Only visitors with `CheckOutTime == null` |
| `GET` | `/api/visitors/{id}` | ✅ 200/404 | Returns specific visitor or 404 |
| `PUT` | `/api/visitors/{id}/checkout` | ✅ 200/400 | Sets `CheckOutTime`, blocks double-checkout |
| `DELETE` | `/api/visitors/{id}` | ✅ 204 | Removes record permanently |

### Validation — Working ✅

Invalid input returns **400 Bad Request** with clear error messages:
```json
{
  "errors": {
    "Email": ["Please provide a valid email address."],
    "PhoneNumber": ["Please provide a valid Indian phone number (e.g., +919876543210 or 9876543210)."]
  }
}
```

### Key Design Decisions

1. **SQLite over InMemory** — Data persists between restarts. Zero-config, file-based database (`visitors.db`). Perfect for a portfolio project. One line change to swap to SQL Server/PostgreSQL later.

2. **DTO Pattern** — `VisitorCheckInDto` prevents clients from setting `Id`, `CheckInTime`, or `CheckOutTime`. Validation lives on the DTO, keeping the entity clean.

3. **CORS** — Configured to allow `http://localhost:4200` (Angular dev server) with all headers and methods.

4. **Database Indexes** — Added on `FullName` (for search) and `CheckOutTime` (for active visitor queries).

---

## How to Run the Backend

```bash
# Set up PATH (already in .bashrc, or run manually)
export DOTNET_ROOT=$HOME/.dotnet
export PATH=$PATH:$HOME/.dotnet

# Navigate to the project
cd "/home/ash/Desktop/Corporate Visitor Management System/backend/VisitorManagement.API"

# Run the server
dotnet run

# API runs at: http://localhost:5147
# Swagger UI:  http://localhost:5147/swagger
```

---

## Next Steps: Angular Frontend (Phase 2)

When you're ready, here's what to do:

```bash
cd "/home/ash/Desktop/Corporate Visitor Management System"

# Create Angular project
ng new frontend --style css --ssr false

# Install Tailwind CSS v4
cd frontend
npm install tailwindcss @tailwindcss/postcss postcss
```

Then we'll build:
1. **Check-in form** — Reactive Forms with validators (phone, email)
2. **Dashboard** — Real-time active visitors list
3. **Search** — Look up past visitors by name
4. **Visitor service** — HttpClient calls to the backend API
