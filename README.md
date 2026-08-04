# ONEE.SSO

> Enterprise Authentication & Authorization Microservice built with ASP.NET Core 9 and Clean Architecture.

![.NET](https://img.shields.io/badge/.NET-9.0-purple)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-9.0-blue)
![EF Core](https://img.shields.io/badge/EF_Core-9.0-green)
![SQL Server](https://img.shields.io/badge/SQL_Server-2022-red)
![Architecture](https://img.shields.io/badge/Architecture-Clean-success)
![Status](https://img.shields.io/badge/Status-In_Development-orange)
![Progress](https://img.shields.io/badge/Progress-4%2F12_Phases_Completed-brightgreen)

---

## Enterprise Single Sign-On (SSO) Microservice

ONEE.SSO is an enterprise-grade Authentication and Authorization microservice designed using **ASP.NET Core 9**, **Entity Framework Core** and **Clean Architecture**.

The project centralizes authentication, authorization and identity management for multiple client applications while following modern security practices such as OAuth2/OpenID Connect, JWT authentication, Refresh Tokens, Role-Based Access Control (RBAC), session management and audit logging.

The objective is to build a scalable and production-ready Identity Provider that can securely authenticate users across several enterprise applications.

---
# 📑 Table of Contents

- [Project Overview](#-project-overview)
- [Key Features](#-key-features)
- [Architecture](#-architecture)
- [Technology Stack](#-technology-stack)
- [Project Structure](#-project-structure)
- [Database Design](#-database-design)
- [REST API](#-rest-api)
- [Getting Started](#-getting-started)
- [Testing](#-testing)
- [Development Progress](#-development-progress)
- [Roadmap](#-roadmap)
- [Future Improvements](#-future-improvements)
- [Screenshots](#-screenshots)
- [Author](#-author)
- [License](#-license)

---
# 📖 Project Overview

ONEE.SSO is a centralized Identity and Access Management (IAM) microservice developed using **ASP.NET Core 9** and **Clean Architecture**.

The system is designed to authenticate users, authorize access to enterprise applications and provide a secure identity provider for multiple client systems.

The architecture follows SOLID principles and separates responsibilities into independent layers, making the application maintainable, scalable and easy to test.

Current implementation includes:

- User Management
- Role Management
- Permission Management
- Client Application Management
- Refresh Token Management
- User Session Management
- Audit Log Management
- Repository Pattern
- Business Services
- Entity Framework Core
- SQL Server persistence
- Swagger REST API

Future versions will include:

- JWT Authentication
- OpenIddict
- OAuth2
- OpenID Connect
- Claims-based Authorization
- Email Verification
- Password Recovery
- Advanced Security Features
- Unit Tests
- Docker Deployment

---
# ✨ Key Features

### Architecture

- ✔ Clean Architecture
- ✔ Layered Design
- ✔ Dependency Injection
- ✔ Repository Pattern
- ✔ Service Layer
- ✔ SOLID Principles

### Database

- ✔ SQL Server
- ✔ Entity Framework Core
- ✔ Fluent API
- ✔ Migrations
- ✔ Relationships
- ✔ Constraints
- ✔ Indexes

### Business Modules

- ✔ User Management
- ✔ Role Management
- ✔ Permission Management
- ✔ Client Application Management
- ✔ Refresh Token Management
- ✔ User Session Management
- ✔ Audit Log Management

### API

- ✔ RESTful API
- ✔ Swagger / OpenAPI
- ✔ CRUD Operations
- ✔ Search
- ✔ Pagination
- ✔ Activation / Deactivation

---
# 🏗️ Architecture

The solution follows the **Clean Architecture** pattern.

```
                +----------------------+
                |         API          |
                +----------------------+
                           │
                           ▼
                +----------------------+
                |     Application      |
                +----------------------+
                           │
                           ▼
                +----------------------+
                |       Domain         |
                +----------------------+
                           ▲
                           │
                +----------------------+
                |    Infrastructure    |
                +----------------------+
                           │
                           ▼
                +----------------------+
                |     SQL Server       |
                +----------------------+
```

Each layer has a dedicated responsibility.

| Layer | Responsibility |
|--------|----------------|
| API | Exposes REST endpoints |
| Application | Business use cases, DTOs, interfaces and services |
| Domain | Business entities, rules and abstractions |
| Infrastructure | Entity Framework Core, repositories and external services |
| SQL Server | Persistent data storage |

The dependencies always point toward the Domain layer, ensuring a clean separation of concerns.

---
# 💻 Technology Stack

| Category | Technologies |
|----------|--------------|
| Language | C# |
| Framework | ASP.NET Core 9 |
| ORM | Entity Framework Core 9 |
| Database | SQL Server |
| Architecture | Clean Architecture |
| Design Pattern | Repository Pattern |
| Documentation | Swagger / OpenAPI |
| Logging | Serilog |
| Dependency Injection | Built-in ASP.NET Core DI |
| Version Control | Git & GitHub |

---
# 📂 Project Structure

```text
ONEE.SSO
│
├── src
│   ├── ONEE.SSO.API
│   │   ├── Controllers
│   │   ├── Middlewares
│   │   ├── Extensions
│   │   └── Program.cs
│   │
│   ├── ONEE.SSO.Application
│   │   ├── DTOs
│   │   ├── Interfaces
│   │   ├── Repositories
│   │   └── Services
│   │
│   ├── ONEE.SSO.Domain
│   │   ├── Common
│   │   ├── Entities
│   │   └── Enums
│   │
│   ├── ONEE.SSO.Infrastructure
│   │   ├── Persistence
│   │   ├── Configurations
│   │   ├── Repositories
│   │   ├── Services
│   │   └── Migrations
│   │
│   └── ONEE.SSO.Shared
│
├── README.md
├── .gitignore
└── ONEE.SSO.sln
```

---

## 📦 Solution Layers

### API

Responsible for exposing the REST API.

Contains:

- Controllers
- Middleware
- Dependency Injection
- Swagger configuration
- Application startup

---

### Application

Contains the application use cases.

Includes:

- DTOs
- Service Interfaces
- Repository Interfaces
- Application Contracts

The Application layer never depends on Infrastructure.

---

### Domain

Contains the business model.

Includes:

- Business Entities
- Base Entities
- Domain Rules

This layer has **no dependency** on any external framework.

---

### Infrastructure

Implements all technical concerns.

Includes:

- Entity Framework Core
- SQL Server
- Repository implementations
- Business service implementations
- Fluent API configurations
- Database migrations

Infrastructure depends on the Application and Domain layers.
# 🗄️ Database Design

The application uses **Microsoft SQL Server** as the relational database management system and **Entity Framework Core 9** as the Object-Relational Mapper (ORM).

The database schema has been designed following normalization principles while ensuring scalability and maintainability.

---

## Database Provider

- Microsoft SQL Server 2022

## ORM

- Entity Framework Core 9

## Database First Approach

- Code First
- Fluent API Configuration
- Entity Relationships
- Database Migrations

---

# 📋 Current Database Schema

| Entity | Description |
|----------|-------------|
| Users | Stores user accounts and authentication information |
| Roles | Defines application roles |
| Permissions | Defines granular permissions |
| UserRoles | Junction table between Users and Roles |
| RolePermissions | Junction table between Roles and Permissions |
| ClientApplications | Registered client applications (OAuth/OpenID Connect clients) |
| RefreshTokens | Stores refresh tokens for authenticated users |
| UserSessions | Tracks authenticated user sessions |
| AuditLogs | Stores security and business audit events |

---

# 🔗 Entity Relationships

The current domain model includes the following relationships.

## Users

- One User → Many Refresh Tokens
- One User → Many User Sessions
- One User → Many Audit Logs
- Many Users ↔ Many Roles

---

## Roles

- One Client Application → Many Roles
- Many Roles ↔ Many Users
- Many Roles ↔ Many Permissions

---

## Permissions

- One Client Application → Many Permissions
- Many Permissions ↔ Many Roles

---

## Client Applications

Each Client Application can own:

- Multiple Roles
- Multiple Permissions

This design allows each application to define its own authorization model independently from other client applications.

---

# ⚙️ Entity Framework Features

The project currently uses:

- Fluent API
- Primary Keys
- Foreign Keys
- Required Constraints
- Unique Constraints
- Indexes
- Delete Behaviors
- Navigation Properties
- Database Migrations

---

# 🔒 Data Integrity

Several constraints are enforced at the database level.

Examples include:

- Unique usernames
- Unique email addresses
- Unique role code per client application
- Unique permission code per client application
- Foreign key constraints
- Required relationships
- Restrictive delete behaviors where appropriate

---

# 📈 Database Evolution

The schema is managed through **Entity Framework Core Migrations**, allowing the database structure to evolve while preserving existing data.

Each structural modification is versioned and can be applied using:

```bash
dotnet ef database update
```

---
# ⚙️ Getting Started

Follow the steps below to run the project locally.

---

## 1. Clone the repository

```bash
git clone https://github.com/YOUR_USERNAME/ONEE.SSO.git
```

Navigate to the project folder.

```bash
cd ONEE.SSO
```

---

## 2. Restore NuGet Packages

Restore all project dependencies.

```bash
dotnet restore
```

---

## 3. Build the Solution

Compile the solution.

```bash
dotnet build
```

The build should complete without errors.

---

## 4. Configure SQL Server

Open the **appsettings.json** file located in:

```
src/ONEE.SSO.API
```

Update the connection string.

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=ONEE.SSO;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

---

## 5. Apply Database Migrations

Create the database schema.

```bash
dotnet ef database update --project src/ONEE.SSO.Infrastructure --startup-project src/ONEE.SSO.API
```

Entity Framework Core will automatically create all required tables.

---

## 6. Run the API

```bash
dotnet run --project src/ONEE.SSO.API
```

You should see something similar to:

```
Environment : Development

Now listening on:

http://localhost:5205
```

---

## 7. Open Swagger

Navigate to:

```
http://localhost:5205/swagger
```

Swagger provides an interactive interface for testing all available REST endpoints.

---

## Current API Modules

The following modules are currently available:

- Users
- Roles
- Permissions
- Client Applications
- Refresh Tokens
- User Sessions
- Audit Logs

---
# 🧪 Testing

The current implementation has been manually tested using **Swagger UI** and **Microsoft SQL Server**.

---

## User Management

✔ Create User

✔ Update User

✔ Delete User

✔ Get User by Id

✔ Get All Users

✔ Search Users

✔ User Pagination

✔ User Filtering

✔ Activate User

✔ Deactivate User

---

## Role Management

✔ Create Role

✔ Update Role

✔ Delete Role

✔ Get Role by Id

✔ Get All Roles

---

## Permission Management

✔ Create Permission

✔ Update Permission

✔ Delete Permission

✔ Get Permission by Id

✔ Get All Permissions

---

## Client Application Management

✔ Create Client Application

✔ Update Client Application

✔ Delete Client Application

✔ Get Client Application by Id

✔ Get All Client Applications

✔ Search Client Applications

✔ Pagination

✔ Activate Client Application

✔ Deactivate Client Application

---

## Refresh Tokens

✔ Get All Refresh Tokens

✔ Get Refresh Token by Id

✔ Revoke Refresh Token

---

## User Sessions

✔ Get All User Sessions

✔ Get User Session by Id

✔ Revoke User Session

---

## Audit Logs

✔ Get All Audit Logs

✔ Get Audit Log by Id

---

## Database

✔ SQL Server Persistence

✔ Entity Framework Core

✔ Fluent API Configuration

✔ Foreign Key Constraints

✔ Database Migrations

---

## Manual Testing

All implemented endpoints have been successfully tested using:

- Swagger UI
- SQL Server
- Entity Framework Core

No critical issues were found during manual testing.

---
# 📈 Development Progress

| Phase | Description | Status |
|--------|-------------|--------|
| Phase 1 | Foundation (Solution, Clean Architecture, EF Core, SQL Server, Swagger, DI) | ✅ Completed |
| Phase 2 | Database Design (Entities, Relationships, Fluent API, Migrations) | ✅ Completed |
| Phase 3 | Repository Layer | ✅ Completed |
| Phase 4 | Business Services | ✅ Completed |
| Phase 5 | User Management | 🚧 In Progress |
| Phase 6 | RBAC (UserRoles & RolePermissions Management) | ⏳ Pending |
| Phase 7 | OpenID Connect Client Applications | ⏳ Pending |
| Phase 8 | Authentication (JWT + Refresh Tokens) | ⏳ Pending |
| Phase 9 | Audit Logging Automation | ⏳ Pending |
| Phase 10 | Advanced Security | ⏳ Pending |
| Phase 11 | Automated Testing | ⏳ Pending |
| Phase 12 | Optimization & Documentation | ⏳ Pending |

---

### Current Progress

- **4 phases completed**
- **1 phase currently in progress**
- **7 phases remaining**

---