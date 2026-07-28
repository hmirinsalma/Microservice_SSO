# ONEE.SSO - Authentication & Authorization Microservice

![.NET](https://img.shields.io/badge/.NET-9.0-purple)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-9.0-blue)
![EF Core](https://img.shields.io/badge/EF_Core-9.0-green)
![SQL Server](https://img.shields.io/badge/SQL_Server-2022-red)
![Architecture](https://img.shields.io/badge/Architecture-Clean-success)
![Status](https://img.shields.io/badge/Status-In_Development-orange)

---

## 📖 Project Overview

ONEE.SSO is an authentication and authorization microservice developed with **ASP.NET Core 9** following **Clean Architecture** principles.

The goal of this project is to provide a centralized identity management solution for multiple client applications by implementing secure authentication, authorization, session management, role-based access control (RBAC), refresh tokens and modern security best practices.

This project is developed as part of an engineering internship and is intended to evolve into a production-ready authentication service.

---

# 📑 Table of Contents

- Project Overview
- Architecture
- Technologies
- Features
- Project Structure
- Database
- Getting Started
- Development Progress
- Roadmap
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
- **Application** → Contains business use cases, interfaces and services.
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

✔ Clean Architecture

✔ SQL Server integration

✔ Entity Framework Core configuration

✔ Database migrations

✔ Generic Repository Pattern

✔ Specialized Repositories

✔ Dependency Injection

✔ Entity relationships

✔ Fluent API configurations

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

Database Provider

- SQL Server

ORM

- Entity Framework Core 9

Current Tables

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

---

# 📈 Development Progress

| Phase | Status |
|--------|--------|
| Foundation | ✅ Completed |
| Database | ✅ Completed |
| Persistence | ✅ Completed |
| Business Services | ⏳ In Progress |
| Authentication | ⏳ Pending |
| Authorization | ⏳ Pending |
| JWT | ⏳ Pending |
| OpenIddict | ⏳ Pending |
| API Endpoints | ⏳ Pending |
| Unit Tests | ⏳ Pending |
| Deployment | ⏳ Pending |

---

# 🛣️ Roadmap

- [x] Clean Architecture
- [x] SQL Server integration
- [x] Entity Framework Core
- [x] Database Migrations
- [x] Repository Pattern
- [ ] Business Services
- [ ] Authentication
- [ ] JWT
- [ ] Refresh Tokens
- [ ] Role-Based Access Control (RBAC)
- [ ] OpenIddict
- [ ] REST API
- [ ] Unit Testing
- [ ] Docker
- [ ] CI/CD Pipeline

---

# 📸 Screenshots

This section will be updated during the project.

Future screenshots:

- Swagger UI
- SQL Server Database
- Authentication Flow
- Login Process
- OpenIddict Configuration

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