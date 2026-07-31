# ONEE.SSO - Authentication & Authorization Microservice

![.NET](https://img.shields.io/badge/.NET-9.0-purple)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-9.0-blue)
![EF Core](https://img.shields.io/badge/EF_Core-9.0-green)
![SQL Server](https://img.shields.io/badge/SQL_Server-2022-red)
![Architecture](https://img.shields.io/badge/Architecture-Clean-success)
![Progress](https://img.shields.io/badge/Progress-5%2F12_Phases_Completed-brightgreen)

---

# 📖 Project Overview

ONEE.SSO is an Authentication and Authorization microservice built with **ASP.NET Core 9** following **Clean Architecture** principles.

Its objective is to provide a centralized identity management system for multiple client applications by implementing secure authentication, authorization, role-based access control (RBAC), session management, refresh tokens and modern security best practices.

The project is developed as part of an engineering internship and is designed to evolve into a production-ready authentication service.

---

# 📑 Table of Contents

- Project Overview
- Architecture
- Technologies
- Current Features
- Implemented API
- Project Structure
- Database
- Getting Started
- Tested Features
- Development Progress
- Roadmap
- Screenshots
- Contributing
- License
- Author

---

# 🏗️ Architecture

The project follows the **Clean Architecture** pattern.

```
                +--------------------+
                |       API          |
                +--------------------+
                          │
                          ▼
                +--------------------+
                |    Application     |
                +--------------------+
                          │
                          ▼
                +--------------------+
                |      Domain        |
                +--------------------+
                          ▲
                          │
                +--------------------+
                |   Infrastructure   |
                +--------------------+
                          │
                          ▼
                +--------------------+
                |    SQL Server      |
                +--------------------+
```

Each layer has a single responsibility.

- **API** → Exposes REST endpoints.
- **Application** → Contains business use cases, services and interfaces.
- **Domain** → Contains business entities and business rules.
- **Infrastructure** → Implements repositories, Entity Framework Core and external services.
- **SQL Server** → Stores application data.

---

# 💻 Technologies

- ASP.NET Core 9
- C#
- Entity Framework Core 9
- SQL Server
- Swagger / OpenAPI
- Dependency Injection
- Repository Pattern
- Clean Architecture
- Domain-Driven Design (DDD)
- Serilog

---

# 🚀 Current Features

- ✔ Clean Architecture
- ✔ SQL Server integration
- ✔ Entity Framework Core configuration
- ✔ Database migrations
- ✔ Generic Repository Pattern
- ✔ Specialized Repositories
- ✔ Dependency Injection
- ✔ Entity relationships
- ✔ Fluent API configurations
- ✔ Business Services
- ✔ User Management Service
- ✔ Complete User CRUD
- ✔ User Search
- ✔ User Pagination
- ✔ User Filtering
- ✔ User Activation / Deactivation
- ✔ Swagger REST API
- ✔ SQL Server persistence

---

# 🌐 Implemented API

## User Management

| Method | Endpoint |
|---------|----------|
| GET | `/api/users` |
| GET | `/api/users/{id}` |
| POST | `/api/users` |
| PUT | `/api/users/{id}` |
| DELETE | `/api/users/{id}` |
| GET | `/api/users/search` |
| GET | `/api/users/paged` |
| GET | `/api/users/filter` |
| PUT | `/api/users/{id}/activate` |
| PUT | `/api/users/{id}/deactivate` |

---

# 📂 Project Structure

```
ONEE.SSO
│
├── src
│   ├── ONEE.SSO.API
│   ├── ONEE.SSO.Application
│   ├── ONEE.SSO.Domain
│   ├── ONEE.SSO.Infrastructure
│   └── ONEE.SSO.Shared
│
├── README.md
├── .gitignore
└── ONEE.SSO.sln
```

---

# 🗄️ Database

### Database Provider

- SQL Server

### ORM

- Entity Framework Core 9

### Current Tables

- Users
- Roles
- Permissions
- UserRoles
- RolePermissions
- RefreshTokens
- UserSessions
- ClientApplications
- AuditLogs

---

# ⚙️ Getting Started

Clone the repository

```bash
git clone https://github.com/YOUR_USERNAME/Microservice_SSO.git
```

Restore packages

```bash
dotnet restore
```

Build the solution

```bash
dotnet build
```

Apply database migrations

```bash
dotnet ef database update
```

Run the API

```bash
dotnet run --project src/ONEE.SSO.API
```

Swagger

```
http://localhost:5205/swagger
```

---

# ✅ Tested Features

The following functionalities have been manually tested using **Swagger** and **SQL Server**.

- ✔ Create User
- ✔ Update User
- ✔ Delete User
- ✔ Get User by Id
- ✔ Get All Users
- ✔ Search Users
- ✔ Pagination
- ✔ Filtering
- ✔ Activate User
- ✔ Deactivate User
- ✔ SQL Server persistence
- ✔ Entity Framework Core integration

---

# 📈 Development Progress

| Phase | Status |
|--------|--------|
| Phase 1 – Foundation | ✅ Completed |
| Phase 2 – Database | ✅ Completed |
| Phase 3 – Persistence | ✅ Completed |
| Phase 4 – Business Services | ✅ Completed |
| Phase 5 – User Management | ✅ Completed |
| Phase 6 – RBAC | ⏳ Pending |
| Phase 7 – Client Applications (OIDC) | ⏳ Pending |
| Phase 8 – Authentication | ⏳ Pending |
| Phase 9 – Audit | ⏳ Pending |
| Phase 10 – Advanced Security | ⏳ Pending |
| Phase 11 – Testing | ⏳ Pending |
| Phase 12 – Optimization | ⏳ Pending |

---

# 🛣️ Roadmap

- [x] Clean Architecture
- [x] SQL Server integration
- [x] Entity Framework Core
- [x] Database Migrations
- [x] Repository Pattern
- [x] Business Services
- [x] User Management (CRUD)
- [x] User Search
- [x] User Pagination
- [x] User Filtering
- [x] User Activation / Deactivation
- [ ] Role-Based Access Control (RBAC)
- [ ] Client Applications (OIDC)
- [ ] Authentication
- [ ] JWT
- [ ] Refresh Tokens
- [ ] Audit Logging
- [ ] Advanced Security
- [ ] Unit Testing
- [ ] Docker
- [ ] CI/CD Pipeline

---

# 📸 Screenshots

## Swagger API

![Swagger](docs/images/swagger-users.png)

---

## SQL Server

![SQL Server](docs/images/sql-users.png)

Future screenshots:

- Login
- JWT Authentication
- OpenIddict Configuration
- Role Management
- Client Applications

---

# 🤝 Contributing

This repository is currently developed as part of an engineering internship.

Contributions are not open at this stage.

---

# 📄 License

This project is currently distributed without an open-source license.

All rights reserved.

---

# 👨‍💻 Author

**Salma**

Engineering Student

EMSI — Digital Development & Information Systems

2026