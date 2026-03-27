# TNI.Api.CleanArchitecture

Template สำหรับสร้าง RESTful API ด้วย **.NET 8** ตามแนวทาง **Clean Architecture** พร้อมระบบ Authentication ด้วย JWT + Refresh Token ครบชุด

---

## Tech Stack

| Layer | เทคโนโลยี |
|---|---|
| Framework | .NET 8, ASP.NET Core |
| ORM | Entity Framework Core 8 (SQL Server) |
| Authentication | JWT Bearer + Refresh Token (BCrypt) |
| CQRS / Mediator | MediatR 14 |
| Validation | FluentValidation 12 |
| Mapping | AutoMapper 16 |
| Logging | Serilog (Console + File) |
| Documentation | Swagger / OpenAPI (Swashbuckle) |
| Health Check | AspNetCore.HealthChecks.SqlServer |
| Testing | xUnit, Moq, FluentAssertions, EF InMemory |

---

## Solution Structure

```
tni.api.CleanArchitecture.sln
├── src/
│   ├── TNI.Api.CleanArchitecture.API             # Presentation Layer
│   ├── TNI.Api.CleanArchitecture.Application     # Application Layer (CQRS)
│   ├── TNI.Api.CleanArchitecture.Domain          # Domain Layer
│   └── TNI.Api.CleanArchitecture.Infrastructure  # Infrastructure Layer
└── tests/
    ├── TNI.Api.CleanArchitecture.IntegrationTests
    └── TNI.Api.CleanArchitecture.UnitTests
```

---

## Architecture Overview (Clean Architecture)

```
┌─────────────────────────────────────────────────────┐
│                    API Layer                         │
│  Controllers · Middleware · Serilog · Swagger        │
│  JWT Auth · HealthCheck                              │
└────────────────────┬────────────────────────────────┘
                     │ MediatR (ISender)
┌────────────────────▼────────────────────────────────┐
│               Application Layer                      │
│  Commands/Handlers · Pipeline Behaviours             │
│  Interfaces (IUserRepository, ITokenService, ...)    │
└────────────────────┬────────────────────────────────┘
         defines     │         implements
┌────────▼──────┐   │   ┌─────▼──────────────────────┐
│ Domain Layer  │   │   │   Infrastructure Layer       │
│ Entities      │   │   │   EF Core · SQL Server       │
│ Value Objects │   │   │   TokenService (JWT)         │
│ Repo Ifaces   │◄──┘   │   PasswordHasher (BCrypt)   │
└───────────────┘       └─────────────────────────────┘
```

> Domain Layer ไม่มี external dependency ใดๆ ทั้งสิ้น

---

## Site Map

### API Endpoints

```
/
├── /health                    GET   Health check (SQL Server connectivity)
│
├── /swagger                   GET   Swagger UI (Development เท่านั้น)
│
└── /api/v1/
    └── /auth/
        ├── POST /register     สมัครสมาชิกใหม่
        ├── POST /login        เข้าสู่ระบบ → รับ Access Token + Refresh Token
        └── POST /refresh      ต่ออายุ Token ด้วย Refresh Token
```

### Request / Response ตัวอย่าง

#### `POST /api/v1/auth/register`
```json
// Request
{ "email": "user@example.com", "password": "P@ssword1", "confirmPassword": "P@ssword1" }

// Response 201
{ "id": "guid", "email": "user@example.com" }
```

#### `POST /api/v1/auth/login`
```json
// Request
{ "email": "user@example.com", "password": "P@ssword1" }

// Response 200
{ "accessToken": "eyJ...", "refreshToken": "base64string" }
```

#### `POST /api/v1/auth/refresh`
```json
// Request
{ "refreshToken": "base64string" }

// Response 200
{ "accessToken": "eyJ...", "refreshToken": "newBase64string" }
```

---

## Layer Details

### 1. API Layer (`TNI.Api.CleanArchitecture.API`)

| ไฟล์ | หน้าที่ |
|---|---|
| `Program.cs` | Bootstrap application, configure pipeline |
| `DependencyInjection.cs` | ลงทะเบียน JWT, Swagger, HealthCheck |
| `Controllers/BaseApiController.cs` | Abstract base — inject MediatR `ISender` |
| `Controllers/AuthController.cs` | Auth endpoints ทั้ง 3 |
| `Middleware/ExceptionHandlingMiddleware.cs` | Global error handling → RFC 7807 ProblemDetails |

**Request Pipeline:**
```
ExceptionHandlingMiddleware
  → SerilogRequestLogging
  → Swagger (Dev only)
  → HTTPS Redirect
  → Authentication / Authorization
  → Controllers
```

### 2. Application Layer (`TNI.Api.CleanArchitecture.Application`)

```
Application/
├── Auth/
│   ├── Commands/
│   │   ├── RegisterUserCommand  (+Handler, +Validator)
│   │   ├── LoginCommand         (+Handler, +Validator)
│   │   └── RefreshTokenCommand  (+Handler)
│   └── DTOs/
│       ├── RegisteredUserDto
│       └── TokenPairDto
└── Common/
    ├── Behaviours/
    │   ├── LoggingBehaviour       (log ก่อน/หลัง handle)
    │   ├── ValidationBehaviour    (FluentValidation, throw 422 ถ้า fail)
    │   └── PerformanceBehaviour   (warn ถ้า > 500ms)
    └── Interfaces/
        ├── IUserRepository
        ├── ITokenService
        └── IPasswordHasher
```

**MediatR Pipeline Order:** `Logging → Validation → Performance → Handler`

### 3. Domain Layer (`TNI.Api.CleanArchitecture.Domain`)

```
Domain/
├── Common/
│   ├── BaseEntity     (Id: Guid, CreatedAt, UpdatedAt)
│   └── ValueObject    (equality by components)
├── Entities/
│   ├── User           (Email, PasswordHash, FirstName, LastName)
│   └── RefreshToken   (Token, UserId, ExpiresAt, IsRevoked, IsUsed)
├── ValueObjects/
│   └── Email          (validated, normalized to lowercase)
├── Repositories/
│   ├── IRepository<T>
│   └── IUnitOfWork
└── Exceptions/
    └── DomainException
```

### 4. Infrastructure Layer (`TNI.Api.CleanArchitecture.Infrastructure`)

```
Infrastructure/
├── Persistence/
│   ├── ApplicationDbContext
│   └── Configurations/
│       ├── UserConfiguration         (unique email index, cascade delete)
│       └── RefreshTokenConfiguration (unique token index)
├── Repositories/
│   ├── Repository<T>      (generic EF Core)
│   ├── UserRepository     (GetByEmailAsync)
│   └── UnitOfWork
└── Services/
    ├── TokenService       (JWT access token + random refresh token)
    └── PasswordHasher     (BCrypt)
```

---

## Exception → HTTP Status Mapping

| Exception | HTTP | Title |
|---|---|---|
| `ValidationException` | 422 | Validation Failure |
| `NotFoundException` | 404 | Not Found |
| `ConflictException` | 409 | Conflict |
| `UnauthorizedException` | 401 | Unauthorized |
| `ForbiddenAccessException` | 403 | Forbidden |
| *(อื่นๆ)* | 500 | Internal Server Error |

---

## Configuration

**`appsettings.json`** — ค่าที่ต้องกำหนด:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=TNIDb;Trusted_Connection=True;"
  },
  "JwtSettings": {
    "Secret": "your-256-bit-secret-key-here",
    "Issuer": "TNI.Api",
    "Audience": "TNI.Client",
    "AccessTokenExpirationMinutes": 60
  },
  "Serilog": {
    "WriteTo": [
      { "Name": "Console" },
      { "Name": "File", "Args": { "path": "logs/log-.txt", "rollingInterval": "Day" } }
    ]
  }
}
```

> Refresh Token มีอายุ **7 วัน** และใช้ได้ **ครั้งเดียว** (revoke-on-use)

---

## Getting Started

### Prerequisites
- .NET 8 SDK
- SQL Server (local หรือ Docker)

### Run

```bash
# 1. Update database
dotnet ef database update --project src/TNI.Api.CleanArchitecture.Infrastructure --startup-project src/TNI.Api.CleanArchitecture.API

# 2. Run API
dotnet run --project src/TNI.Api.CleanArchitecture.API
```

- Swagger UI: `https://localhost:{port}/swagger`
- Health Check: `https://localhost:{port}/health`

### Run Tests

```bash
# Unit Tests
dotnet test tests/TNI.Api.CleanArchitecture.UnitTests

# Integration Tests
dotnet test tests/TNI.Api.CleanArchitecture.IntegrationTests
```

---

## Tests Overview

### Integration Tests
ใช้ `WebApplicationFactory` + EF InMemory แทน SQL Server จริง

| Test | Description |
|---|---|
| Register valid request | Returns 201 |
| Register duplicate email | Returns 409 |
| Register weak password | Returns 422 |
| Login valid credentials | Returns 200 + TokenPair |
| Login invalid credentials | Returns 401 |
| Refresh with valid token | Returns 200 + new TokenPair |

### Unit Tests
| ไฟล์ | สิ่งที่ทดสอบ |
|---|---|
| `ValidationBehaviourTests` | Pipeline behaviour + FluentValidation |
| `NotFoundExceptionTests` | Domain exception properties |
| `TokenServiceTests` | JWT generation + refresh token logic |

---

## Project Conventions

- **CQRS** — แยก Command (write) ออกจาก Query (read) อย่างชัดเจน
- **Rich Domain Model** — Entity มี static factory method, private setters
- **No direct DbContext in handler** — ผ่าน Repository + UnitOfWork เท่านั้น
- **ProblemDetails** — ทุก error response เป็น RFC 7807 format
- **Serilog structured logging** — log ทุก request ผ่าน middleware
