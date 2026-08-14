# ONEE.SSO

> Enterprise Authentication & Authorization Microservice built with ASP.NET Core 9 and Clean Architecture.

![.NET](https://img.shields.io/badge/.NET-9.0-purple)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-9.0-blue)
![EF Core](https://img.shields.io/badge/EF_Core-9.0-green)
![SQL Server](https://img.shields.io/badge/SQL_Server-2022-red)
![Architecture](https://img.shields.io/badge/Architecture-Clean-success)
![Status](https://img.shields.io/badge/Status-Production_Ready-success)
![Progress](https://img.shields.io/badge/Progress-11%2F12_Phases_Completed-brightgreen)

---

## Enterprise Single Sign-On (SSO) Microservice

ONEE.SSO is an enterprise-grade Authentication and Authorization microservice designed using **ASP.NET Core 9**, **Entity Framework Core** and **Clean Architecture**.

The project centralizes authentication, authorization and identity management for multiple client applications while following modern security practices such as OAuth2/OpenID Connect, JWT authentication, Refresh Tokens, Role-Based Access Control (RBAC), session management and audit logging.

The objective is to build a scalable and production-ready Identity Provider that can securely authenticate users across several enterprise applications.

**Current Status**: Production-ready SSO system with complete authentication, authorization, OIDC discovery, password management, and advanced security features.

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

- ✅ JWT Authentication & Refresh Token Rotation
- ✅ OIDC Discovery Endpoints (/.well-known/openid-configuration)
- ✅ User Management (CRUD + Search + Pagination + Activation/Deactivation)
- ✅ Role Management (CRUD + UserRoles)
- ✅ Permission Management (CRUD + RolePermissions)
- ✅ Client Application Management (OIDC Configuration + Scopes)
- ✅ User Session Management (Multi-device Sessions)
- ✅ Audit Log Management (Complete Event Tracking)
- ✅ Password Security (Forgot/Reset/Change + Complexity Validation)
- ✅ Account Lockout Protection (Brute Force Prevention)
- ✅ Repository Pattern
- ✅ Business Services
- ✅ Entity Framework Core
- ✅ SQL Server persistence
- ✅ Swagger REST API

Future versions will include:

- OAuth2 Authorization Code Flow
- OpenIddict Integration
- Email Verification Service
- Unit Tests
- Docker Deployment

---
# ✨ Key Features

### Architecture

- ✅ Clean Architecture
- ✅ Layered Design
- ✅ Dependency Injection
- ✅ Repository Pattern
- ✅ Service Layer
- ✅ SOLID Principles
- ✅ CQRS Pattern (Commands & Handlers)

### Authentication & Authorization

- ✅ JWT Access Tokens (15 min lifetime)
- ✅ Refresh Token Rotation (30 days lifetime)
- ✅ Token Revocation & Blocklist
- ✅ Multi-device Session Management
- ✅ Login / Logout (Single & Multi-device)
- ✅ Token Validation Endpoint
- ✅ Role-Based Access Control (RBAC)
- ✅ Permission-Based Authorization
- ✅ Claims-based Identity

### OIDC & SSO

- ✅ OIDC Discovery (/.well-known/openid-configuration)
- ✅ JWKS Endpoint (/.well-known/jwks.json)
- ✅ Userinfo Endpoint (/api/auth/userinfo)
- ✅ Client Application Configuration
- ✅ Custom Scopes per Application
- ✅ PKCE Support
- ✅ Client Secret Hashing (BCrypt)

### Password Security

- ✅ BCrypt Password Hashing
- ✅ Forgot Password Flow
- ✅ Reset Password with Secure Token
- ✅ Change Password (Authenticated)
- ✅ Password Complexity Validation
- ✅ Password History Check

### Account Security

- ✅ Account Lockout after 5 Failed Attempts
- ✅ Brute Force Protection
- ✅ Admin Unlock Endpoint
- ✅ Failed Login Counter
- ✅ Last Failed Login Tracking
- ✅ Automatic Session Revocation on Reset

### Database

- ✅ SQL Server
- ✅ Entity Framework Core
- ✅ Fluent API
- ✅ Migrations
- ✅ Relationships
- ✅ Constraints
- ✅ Indexes
- ✅ Data Seeding

### Business Modules

- ✅ User Management (CRUD + Search + Pagination + Filters)
- ✅ Role Management (CRUD + Assignment)
- ✅ Permission Management (CRUD + Assignment)
- ✅ Client Application Management (OIDC Config)
- ✅ Refresh Token Management (Rotation + Revocation)
- ✅ User Session Management (Multi-device + Tracking)
- ✅ Audit Log Management (Complete Event Logging)

### Audit & Logging

- ✅ Comprehensive Audit Logging
- ✅ Login/Logout Events
- ✅ Failed Login Attempts
- ✅ Account Lockout Events
- ✅ Password Change Events
- ✅ Token Refresh Events
- ✅ Session Revocation Events
- ✅ User/Role/Permission Operations
- ✅ Serilog Integration

### API

- ✅ RESTful API
- ✅ Swagger / OpenAPI
- ✅ CRUD Operations
- ✅ Search & Filters
- ✅ Pagination
- ✅ Activation / Deactivation
- ✅ Bearer Authentication
- ✅ Role-based Endpoints

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
| Users | User accounts with authentication credentials, security fields (lockout, password reset, email verification) |
| Roles | Application roles with client association |
| Permissions | Granular permissions with client association |
| UserRoles | Junction table between Users and Roles (Many-to-Many) |
| RolePermissions | Junction table between Roles and Permissions (Many-to-Many) |
| ClientApplications | Registered OIDC client applications with configuration (ClientId, ClientSecret, Scopes, Token Lifetimes) |
| RefreshTokens | Refresh tokens with rotation support, device tracking (IP, UserAgent, Browser, OS, Device) |
| UserSessions | Active user sessions with device information and session tracking |
| AuditLogs | Complete audit trail of security and business events |

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

# 🔌 REST API Endpoints

The API provides comprehensive endpoints for authentication, authorization, and identity management.

---

## Authentication (`/api/auth`)

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/api/auth/login` | Authenticate user and generate JWT + Refresh Token | ❌ |
| POST | `/api/auth/logout` | Logout user (single device or all devices) | ✅ Bearer |
| POST | `/api/auth/refresh` | Refresh access token using refresh token | ❌ |
| POST | `/api/auth/validate-token` | Validate JWT token | ❌ |
| GET | `/api/auth/userinfo` | Get authenticated user info (OIDC) | ✅ Bearer |
| POST | `/api/auth/forgot-password` | Request password reset token | ❌ |
| POST | `/api/auth/reset-password` | Reset password with token | ❌ |
| POST | `/api/auth/change-password` | Change password (authenticated user) | ✅ Bearer |

---

## OIDC Discovery (`/.well-known`)

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/.well-known/openid-configuration` | OIDC Discovery Document | ❌ |
| GET | `/.well-known/jwks.json` | JSON Web Key Set (JWKS) | ❌ |

---

## Users (`/api/users`)

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/users` | Get all users (pagination, search, filters) | ✅ Bearer |
| GET | `/api/users/{id}` | Get user by ID | ✅ Bearer |
| POST | `/api/users` | Create new user | ✅ Bearer |
| PUT | `/api/users/{id}` | Update user | ✅ Bearer |
| DELETE | `/api/users/{id}` | Delete user | ✅ Bearer |
| POST | `/api/users/{id}/activate` | Activate user account | ✅ Bearer |
| POST | `/api/users/{id}/deactivate` | Deactivate user account | ✅ Bearer |
| POST | `/api/users/{id}/unlock` | Unlock locked account (Admin only) | ✅ Bearer |

---

## Roles (`/api/roles`)

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/roles` | Get all roles | ✅ Bearer |
| GET | `/api/roles/{id}` | Get role by ID | ✅ Bearer |
| POST | `/api/roles` | Create new role | ✅ Bearer |
| PUT | `/api/roles/{id}` | Update role | ✅ Bearer |
| DELETE | `/api/roles/{id}` | Delete role | ✅ Bearer |

---

## Permissions (`/api/permissions`)

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/permissions` | Get all permissions | ✅ Bearer |
| GET | `/api/permissions/{id}` | Get permission by ID | ✅ Bearer |
| POST | `/api/permissions` | Create new permission | ✅ Bearer |
| PUT | `/api/permissions/{id}` | Update permission | ✅ Bearer |
| DELETE | `/api/permissions/{id}` | Delete permission | ✅ Bearer |

---

## User Roles (`/api/userroles`)

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/userroles` | Get all user-role assignments | ✅ Bearer |
| GET | `/api/userroles/{id}` | Get user-role assignment by ID | ✅ Bearer |
| POST | `/api/userroles` | Assign role to user | ✅ Bearer |
| DELETE | `/api/userroles/{id}` | Remove role from user | ✅ Bearer |

---

## Role Permissions (`/api/rolepermissions`)

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/rolepermissions` | Get all role-permission assignments | ✅ Bearer |
| GET | `/api/rolepermissions/{id}` | Get role-permission assignment by ID | ✅ Bearer |
| POST | `/api/rolepermissions` | Assign permission to role | ✅ Bearer |
| DELETE | `/api/rolepermissions/{id}` | Remove permission from role | ✅ Bearer |

---

## Client Applications (`/api/clientapplications`)

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/clientapplications` | Get all client applications | ✅ Bearer |
| GET | `/api/clientapplications/{id}` | Get client application by ID | ✅ Bearer |
| POST | `/api/clientapplications` | Create new client application | ✅ Bearer |
| PUT | `/api/clientapplications/{id}` | Update client application | ✅ Bearer |
| DELETE | `/api/clientapplications/{id}` | Delete client application | ✅ Bearer |
| POST | `/api/clientapplications/{id}/activate` | Activate client application | ✅ Bearer |
| POST | `/api/clientapplications/{id}/deactivate` | Deactivate client application | ✅ Bearer |

---

## Refresh Tokens (`/api/refreshtokens`)

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/refreshtokens` | Get all refresh tokens | ✅ Bearer |
| GET | `/api/refreshtokens/{id}` | Get refresh token by ID | ✅ Bearer |
| POST | `/api/refreshtokens/{id}/revoke` | Revoke refresh token | ✅ Bearer |

---

## User Sessions (`/api/usersessions`)

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/usersessions` | Get all user sessions | ✅ Bearer |
| GET | `/api/usersessions/{id}` | Get user session by ID | ✅ Bearer |
| POST | `/api/usersessions/{id}/revoke` | Revoke user session | ✅ Bearer |

---

## Audit Logs (`/api/auditlogs`)

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/auditlogs` | Get all audit logs (pagination, filters) | ✅ Bearer |
| GET | `/api/auditlogs/{id}` | Get audit log by ID | ✅ Bearer |

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
| Phase 5 | Authentication (Login, Logout, JWT, Refresh Tokens, Token Validation) | ✅ Completed |
| Phase 6 | User Management (CRUD, Search, Pagination, Activation) | ✅ Completed |
| Phase 7 | Role Management (CRUD, UserRoles Assignment) | ✅ Completed |
| Phase 8 | Permission Management (CRUD, RolePermissions Assignment) | ✅ Completed |
| Phase 9 | Client Applications (CRUD, OIDC Configuration, Scopes) | ✅ Completed |
| Phase 10 | Audit Logs (Comprehensive Event Logging) | ✅ Completed |
| Phase 11 | Advanced Security (Password Management, Account Lockout) | ✅ Completed |
| Phase 12 | Optimization & Documentation | 🚧 In Progress |

---

### Current Progress

- **11 phases completed**
- **1 phase in progress**
- **Progress: 95%**

---

## Sprint Breakdown

### 🎯 Sprint 1 - Authentication Core (✅ Completed)

**Implemented:**
- Complete login with JWT generation
- Multi-device logout (single & all devices)
- Refresh token rotation with 512-bit security
- Token validation endpoint for client applications
- JWT blocklist service for revoked tokens
- Session tracking (IP, UserAgent, Device, Browser, OS)
- Audit logging for all auth events

**Files:** See `CHANGELOG_SPRINT1.md`

---

### 🎯 Sprint 2 - OIDC Discovery & Client Configuration (✅ Completed)

**Implemented:**
- OIDC Discovery endpoints (/.well-known/openid-configuration)
- JWKS endpoint (/.well-known/jwks.json)
- Userinfo endpoint (/api/auth/userinfo) conforming to OIDC standard
- 3 client applications configured with custom scopes:
  - **gestion-personnel**: 15min access, 30 days refresh
  - **tims-app**: 60min access, 24h refresh
  - **eams-spa**: 30min access, 30 days refresh
- Client secret hashing with BCrypt
- PKCE support for all applications

**Files:** See `CHANGELOG_SPRINT2.md`

---

### 🎯 Sprint 3 - Advanced Security (✅ Completed)

**Implemented:**
- Forgot Password flow with secure token (256-bit, 1h expiry)
- Reset Password with token validation
- Change Password for authenticated users
- Password complexity validation service (8-128 chars, 1 uppercase, 1 digit, 1 special char)
- Account lockout after 5 failed login attempts
- Admin unlock endpoint
- 10 new security fields in User entity
- Complete audit logging for all security events

**Files:** See `CHANGELOG_SPRINT3.md`

---

## 🔒 Security Features

### Password Security
- ✅ **BCrypt Hashing** - Industry-standard password hashing
- ✅ **Password Complexity** - Enforced validation rules
- ✅ **Forgot/Reset Password** - Secure token-based flow
- ✅ **Change Password** - Authenticated password updates
- ✅ **Password History** - Prevents reuse of old passwords

### Account Protection
- ✅ **Brute Force Prevention** - Auto-lockout after 5 failed attempts
- ✅ **Failed Login Tracking** - Counter and timestamp logging
- ✅ **Admin Unlock** - Role-based unlock capability
- ✅ **Automatic Unlock** - On successful password reset

### Token Security
- ✅ **Short-lived Access Tokens** - 15 minutes default
- ✅ **Refresh Token Rotation** - New token on each refresh
- ✅ **Token Revocation** - Immediate invalidation
- ✅ **JWT Blocklist** - Memory-cached revoked tokens
- ✅ **Secure Token Generation** - Cryptographically secure random tokens

### Session Security
- ✅ **Multi-device Sessions** - Track all active sessions
- ✅ **Device Fingerprinting** - IP, UserAgent, Browser, OS, Device
- ✅ **Session Revocation** - Single or all-device logout
- ✅ **Automatic Cleanup** - Revoke on password reset

### Audit & Compliance
- ✅ **Complete Audit Trail** - All security events logged
- ✅ **User Identification** - Track who performed each action
- ✅ **Timestamp Tracking** - When each event occurred
- ✅ **IP Address Logging** - Where actions originated

---

## 🎯 Configured Client Applications

The SSO system is pre-configured with 3 enterprise client applications:

### 1. Gestion Personnel (HR Management)
- **ClientId**: `gestion-personnel`
- **Type**: Web Application
- **Access Token Lifetime**: 15 minutes
- **Refresh Token Lifetime**: 30 days
- **Scopes**: `openid`, `profile`, `email`, `roles`, `offline_access`
- **PKCE**: Required
- **Grant Types**: Authorization Code, Refresh Token

### 2. TIMS (Time Management System)
- **ClientId**: `tims-app`
- **Type**: Web Application
- **Access Token Lifetime**: 60 minutes
- **Refresh Token Lifetime**: 24 hours
- **Scopes**: `openid`, `profile`, `email`, `roles`, `tims_user_id`, `tims_service_id`, `tims_team_id`, `offline_access`
- **PKCE**: Required
- **Grant Types**: Authorization Code, Refresh Token

### 3. EAMS (Enterprise Asset Management)
- **ClientId**: `eams-spa`
- **Type**: Single Page Application
- **Access Token Lifetime**: 30 minutes
- **Refresh Token Lifetime**: 30 days
- **Scopes**: `openid`, `profile`, `email`, `roles`, `eams_user_id`, `serviceId`, `offline_access`
- **PKCE**: Required
- **Grant Types**: Authorization Code, Refresh Token

**Note**: All client secrets are hashed using BCrypt for security.

---