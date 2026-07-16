# VisiTrack — Corporate Visitor Management System

VisiTrack is a secure, modern web application designed to track and manage guests entering a corporate building. It replicates a practical, real-world enterprise tool with a streamlined codebase, making it an excellent showcase for developers to demonstrate clean programming patterns in ASP.NET Core and modern Angular.

---

## 🚀 Key Features

*   **Real-Time Active Dashboard:** Displays metrics and listings for guests currently inside the building. Provides instant one-click guest check-out.
*   **Digital Check-In Form:** Features comprehensive client-side reactive validation (including Indian phone format matching and email structures) before staging requests.
*   **Searchable Registry Logs:** Debounced name lookup for past and present visitor logs, showing exact visit durations, check-in/out timestamps, and option to remove history logs.

---

## 🛠️ Technology Stack

### Backend
*   **Framework:** ASP.NET Core 8.0 Web API (Controller-Based Architecture)
*   **Database ORM:** Entity Framework Core 8.0
*   **Database:** SQLite (Volatile file-based database; stores logs locally in `visitors.db`)
*   **API Documentation:** Swagger / OpenAPI UI

### Frontend
*   **Framework:** Angular 22 (Modern Standalone Components & Signals architecture)
*   **Styling:** Tailwind CSS v4 (Using `@tailwindcss/postcss` for lightning-fast styling compilation)
*   **Change Detection:** Angular Signals (Modern reactive state management)
*   **Reactive Streams:** RxJS Observables (Used to debounce typing inputs)

---

## 📁 Repository Structure

```
Corporate Visitor Management System/
├── backend/                              # ASP.NET Core Web API Project
│   └── VisitorManagement.API/
│       ├── Controllers/                  # API Controllers (CRUD endpoints)
│       ├── Data/                         # EF Core Database Context & Config
│       ├── Models/                       # Entity Models & Input DTOs
│       ├── Program.cs                    # Application Services & CORS Config
│       └── appsettings.json              # Connection Strings & SQLite path
├── frontend/                             # Angular Web Application
│   ├── src/
│   │   ├── app/
│   │   │   ├── models/                   # TypeScript Type Interfaces
│   │   │   ├── services/                 # HttpClient HTTP request services
│   │   │   ├── pages/                    # Lazy-loaded page components
│   │   │   │   ├── dashboard/            # Active visitor metrics & listing
│   │   │   │   ├── check-in/             # Form validation & check-in triggers
│   │   │   │   └── search/               # Debounced past logs list
│   │   │   └── app.html & app.ts         # Root Navigation shell Layout
│   └── package.json                      # Scripts & dependencies
└── .gitignore                            # Excludes build files & DB databases
```

---

## 🧑‍💻 Architecture Details 

This codebase incorporates several core design patterns :

1.  **Data Transfer Objects (DTOs):** Separating API contract inputs from direct EF Core models. [`VisitorCheckInDto.cs`](backend/VisitorManagement.API/Models/VisitorCheckInDto.cs) handles incoming field validations using validation attributes, preventing over-posting and keeping database schema configurations clean.
2.  **Angular Signals:** The frontend implements native Signals (`activeVisitors()`, `isLoading()`) for local component state. This replaces manual RxJS behavior subjects for simplified, high-performance UI state synchronization.
3.  **RxJS Search Debouncing:** The Search Page features debounced search streams to prevent database querying on every single keystroke:
    ```typescript
    this.searchControl.valueChanges.pipe(
      debounceTime(300),          // wait for 300ms of pause
      distinctUntilChanged(),     // trigger only if query has changed
      switchMap(query => this.visitorService.getVisitors(query)) // cancel previous call
    )
    ```
4.  **CORS Integration:** Configures secure Cross-Origin Resource Sharing on the Web API to whitelist the Angular local dev host (`http://localhost:4200`).
5.  **SQLite Indexes:** Custom DB indexes are applied on `FullName` (optimizing searches) and `CheckOutTime` (optimizing current in-building queries) in `OnModelCreating`.

---

## ⚙️ Running Locally

### 1. Prerequisites
Ensure you have Node.js (v18+) and .NET SDK 8 installed.

### 2. Start the Backend API
```bash
# Navigate to API directory
cd "backend/VisitorManagement.API"

# Run the project
dotnet run
```
*   The API server will run at: `http://localhost:5147`
*   Open Swagger UI in your browser to test endpoints: `http://localhost:5147/swagger`
*   SQLite will auto-generate the `visitors.db` file in the folder on first run.

### 3. Start the Angular Frontend
```bash
# Navigate to the frontend directory
cd "frontend"

# Install dependencies (if not already done)
npm install

# Start the dev server
npm start
```
*   The development portal will open at: `http://localhost:4200`
