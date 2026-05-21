# ElectroPiTask API

A RESTful Web API built with **ASP.NET Core 9** for managing projects and tasks, featuring JWT-based authentication, role-based authorization, and a clean layered architecture.

---

## Table of Contents

- [Overview](#overview)
- [Architecture](#architecture)
- [Tech Stack](#tech-stack)
- [Getting Started](#getting-started)
- [Configuration](#configuration)
- [Database Setup](#database-setup)
- [Default Admin Account](#default-admin-account)
- [API Endpoints](#api-endpoints)
- [Project Structure](#project-structure)

---

## Overview

ElectroPiTask is a project and task management API that allows admins to create and manage projects and assign tasks with priorities and statuses. Viewers can browse projects and tasks without authentication. The API uses JWT tokens for secure access and BCrypt for password hashing.

---

## Architecture

The solution follows a **Clean Architecture** pattern and is split into four layers:

- **ElectroPiTask.API** — Controllers, middleware, Swagger configuration, and the application entry point.
- **ElectroPiTask.Application** — Business logic services, DTOs, and repository interfaces.
- **ElectroPiTask.Domain** — Core entities, enums, and the base entity class. No external dependencies.
- **ElectroPiTask.Infrastructure** — EF Core DbContext, entity configurations, migrations, the generic repository implementation, and database seeding.

---

## Tech Stack

| Concern | Technology |
|---|---|
| Framework | ASP.NET Core 9 |
| ORM | Entity Framework Core 9 |
| Database | SQL Server |
| Authentication | JWT Bearer (Microsoft.AspNetCore.Authentication.JwtBearer) |
| Password Hashing | BCrypt.Net-Next |
| Logging | Serilog (Console + rolling File sink) |
| API Docs | Swagger / Swashbuckle |
| Validation | FluentValidation |

---

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- SQL Server (local or remote)
- Visual Studio 2022+ or any editor with C# support

### Clone & Run

```bash
git clone https://github.com/your-username/ElectroPiTask.git
cd ElectroPiTask
```

Restore dependencies and run the API:

```bash
dotnet restore
dotnet run --project ElectroPiTask.API
```

The API starts on:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`

Swagger UI is available at `https://localhost:5001/swagger` in the Development environment.

---

## Configuration

Copy and update `appsettings.json` (or use `appsettings.Development.json` for local development):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=ElectroPiTaskDB;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "JWT": {
    "Secret": "YourSuperSecretKeyThatIsAtLeast32CharactersLong!",
    "Issuer": "ElectroPiTask",
    "Audience": "ElectroPiTaskUsers"
  }
}
```

> **Important:** Replace `JWT:Secret` with a strong, randomly generated key before deploying to production. Never commit real secrets to version control.

---

## Database Setup

Migrations are applied automatically on startup. If you prefer to run them manually:

```bash
dotnet ef database update --project ElectroPiTask.Infrastructure --startup-project ElectroPiTask.API
```

---

## Default Admin Account

On first run, the database is seeded with a default admin user:

| Field | Value |
|---|---|
| Email | `admin@elctropi.com` |
| Password | `Admin@123` |
| Role | Admin |

Change this password immediately after your first login in any non-development environment.

---

## API Endpoints

All responses are wrapped in a standard `ApiResponse<T>` envelope:

```json
{
  "success": true,
  "message": "...",
  "data": { }
}
```

### Auth — `/api/auth`

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| POST | `/api/auth/register` | Public | Register a new user |
| POST | `/api/auth/login` | Public | Login and receive a JWT token |

**Register body:**
```json
{
  "firstName": "John",
  "lastName": "Doe",
  "email": "john@example.com",
  "password": "P@ssw0rd",
  "role": 0
}
```
Role values: `0` = Admin, `1` = Viewer

**Login body:**
```json
{
  "email": "john@example.com",
  "password": "P@ssw0rd"
}
```

---

### Projects — `/api/project`

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| GET | `/api/project` | Public | Get all projects |
| GET | `/api/project/{id}` | Public | Get a project by ID |
| POST | `/api/project` | Admin only | Create a new project |
| PUT | `/api/project` | Admin only | Update a project |
| DELETE | `/api/project/{projectId}` | Admin only | Delete a project |

---

### Project Tasks — `/api/projecttask`

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| GET | `/api/projecttask/{projectId}` | Public | Get all tasks for a project |
| POST | `/api/projecttask` | Admin only | Create a new task |
| PUT | `/api/projecttask` | Admin only | Update a task's status |
| DELETE | `/api/projecttask/{taskId}` | Admin only | Delete a task |

**Create task body:**
```json
{
  "title": "Design login screen",
  "description": "Create wireframes and final design",
  "status": 0,
  "dueDate": "2026-06-01T00:00:00Z",
  "priority": 2,
  "projectId": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx"
}
```

**Task Status values:** `0` = Todo, `1` = InProgress, `2` = Done, `3` = Cancelled

**Task Priority values:** `0` = Low, `1` = Medium, `2` = High, `3` = Critical

---

### Authentication

Protected endpoints require a Bearer token in the `Authorization` header:

```
Authorization: Bearer <your_jwt_token>
```

Tokens are valid for **7 days**.

---

## Project Structure

```
ElectroPiTask/
├── ElectroPiTask.API/
│   ├── Controllers/          # AuthController, ProjectController, ProjectTaskController
│   ├── Extensions/           # SwaggerExtensions
│   ├── Middleware/
│   ├── Properties/
│   ├── appsettings.json
│   └── Program.cs
│
├── ElectroPiTask.Application/
│   ├── Common/
│   │   ├── Interfaces/       # IRepository<T>
│   │   └── Models/           # ApiResponse<T>
│   ├── DTOs/
│   │   ├── Auth/             # LoginDto, RegisterDto, AuthResponseDto
│   │   ├── Project/          # CreateProjectDto, ProjectDto, UpdateProjectDto
│   │   └── ProjectTask/      # CreateProjectTaskDto, ProjectTaskDto, UpdateProjectTaskStatusDto
│   └── Services/             # AuthService, TokenService, ProjectService, ProjectTaskService
│
├── ElectroPiTask.Domain/
│   ├── Common/               # BaseEntity
│   ├── Entities/             # User, Project, ProjectTask
│   └── Enums/                # UserRole, TaskStatus, TaskPriority
│
└── ElectroPiTask.Infrastructure/
    ├── Migrations/
    ├── Persistence/
    │   ├── Configurations/   # EF entity type configurations
    │   ├── ApplicationDbContext.cs
    │   └── ApplicationDbContextSeed.cs
    └── Repositories/         # GenericRepository<T>
```

---

## Logging

Serilog writes structured logs to the console and to daily rolling files under `ElectroPiTask.API/Logs/`. Log files are retained for 90 days.

---

## License

This project is provided for demonstration/task purposes. Add your preferred license here.