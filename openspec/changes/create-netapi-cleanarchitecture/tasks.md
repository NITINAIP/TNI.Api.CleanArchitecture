## 1. Solution & Project Scaffolding

- [x] 1.1 Create solution file `tni.api.CleanArchitecture.sln`
- [x] 1.2 Create `src/TNI.Api.CleanArchitecture.Domain` class library project targeting .NET 8
- [x] 1.3 Create `src/TNI.Api.CleanArchitecture.Application` class library project targeting .NET 8
- [x] 1.4 Create `src/TNI.Api.CleanArchitecture.Infrastructure` class library project targeting .NET 8
- [x] 1.5 Create `src/TNI.Api.CleanArchitecture.API` ASP.NET Core Web API project targeting .NET 8
- [x] 1.6 Create `tests/TNI.Api.CleanArchitecture.UnitTests` xUnit test project
- [x] 1.7 Create `tests/TNI.Api.CleanArchitecture.IntegrationTests` xUnit test project
- [x] 1.8 Add all projects to solution and configure project references (Domain ← Application ← Infrastructure ← API)
- [x] 1.9 Create `Directory.Build.props` at solution root with pinned NuGet versions

## 2. Domain Layer

- [x] 2.1 Create `BaseEntity` abstract class with `Id` (Guid), `CreatedAt`, `UpdatedAt` properties
- [x] 2.2 Create `ValueObject` abstract base class with structural equality
- [x] 2.3 Define `IRepository<T>` generic interface with `GetByIdAsync`, `GetAllAsync`, `AddAsync`, `Update`, `Delete`
- [x] 2.4 Define `IUnitOfWork` interface with `CommitAsync()`
- [x] 2.5 Create placeholder `User` entity inheriting `BaseEntity` (for auth flow)
- [x] 2.6 Create `Email` value object with format validation in Domain

## 3. Application Layer

- [x] 3.1 Add NuGet packages: `MediatR`, `AutoMapper`, `FluentValidation.DependencyInjectionExtensions`
- [x] 3.2 Create `DependencyInjection.cs` with `AddApplication(IServiceCollection)` extension method
- [x] 3.3 Register MediatR from Application assembly in `AddApplication`
- [x] 3.4 Register AutoMapper profiles from Application assembly in `AddApplication`
- [x] 3.5 Register FluentValidation validators from Application assembly via `AddValidatorsFromAssembly`
- [x] 3.6 Create `LoggingBehaviour<TRequest, TResponse>` MediatR pipeline behavior
- [x] 3.7 Create `ValidationBehaviour<TRequest, TResponse>` MediatR pipeline behavior
- [x] 3.8 Create `PerformanceBehaviour<TRequest, TResponse>` MediatR pipeline behavior
- [x] 3.9 Register pipeline behaviors in DI in correct order (Logging → Validation → Performance)
- [x] 3.10 Define `ValidationException` carrying `IDictionary<string, string[]>` of failures
- [x] 3.11 Define `NotFoundException` with entity name + key message format
- [x] 3.12 Define `ForbiddenAccessException`
- [x] 3.13 Create `ApplicationAssemblyMarker` class for assembly scanning

## 4. Authentication — Application Commands & Queries

- [x] 4.1 Create `RegisterUserCommand` with `Email`, `Password`, `ConfirmPassword` fields
- [x] 4.2 Create `RegisterUserCommandHandler` that validates uniqueness, hashes password, persists user
- [x] 4.3 Create `RegisterUserCommandValidator` using FluentValidation (email format, password length ≥ 8, passwords match)
- [x] 4.4 Create `LoginCommand` with `Email`, `Password` fields
- [x] 4.5 Create `LoginCommandHandler` that verifies credentials and returns token pair DTO
- [x] 4.6 Create `LoginCommandValidator` using FluentValidation
- [x] 4.7 Create `RefreshTokenCommand` with `RefreshToken` field
- [x] 4.8 Create `RefreshTokenCommandHandler` that validates refresh token and returns new token pair
- [x] 4.9 Define `ITokenService` interface in Application with `GenerateAccessToken`, `GenerateRefreshToken`, `ValidateRefreshToken`

## 5. Infrastructure Layer

- [x] 5.1 Add NuGet packages: `Microsoft.EntityFrameworkCore.SqlServer`, `Microsoft.EntityFrameworkCore.Tools`, `BCrypt.Net-Next`
- [x] 5.2 Create `ApplicationDbContext` inheriting `DbContext` with auto-configuration discovery
- [x] 5.3 Create EF Core entity configuration `UserConfiguration : IEntityTypeConfiguration<User>`
- [x] 5.4 Create `RefreshToken` entity and `RefreshTokenConfiguration`
- [x] 5.5 Implement `Repository<T>` satisfying `IRepository<T>` using EF Core
- [x] 5.6 Implement `UnitOfWork` satisfying `IUnitOfWork` wrapping `ApplicationDbContext.SaveChangesAsync`
- [x] 5.7 Implement `TokenService` satisfying `ITokenService` using `Microsoft.AspNetCore.Authentication.JwtBearer`
- [x] 5.8 Create `DependencyInjection.cs` with `AddInfrastructure(IServiceCollection, IConfiguration)` extension method
- [x] 5.9 Register DbContext, repositories, unit of work, and token service in `AddInfrastructure`
- [x] 5.10 Add initial EF Core migration `InitialCreate`

## 6. API Layer

- [x] 6.1 Add NuGet packages: `Swashbuckle.AspNetCore`, `Serilog.AspNetCore`, `Serilog.Sinks.File`, `Microsoft.AspNetCore.Authentication.JwtBearer`, `AspNetCore.HealthChecks.SqlServer`
- [x] 6.2 Create `BaseApiController` abstract class injecting `IMediator`
- [x] 6.3 Create `ExceptionHandlingMiddleware` mapping exception types to RFC 7807 `ProblemDetails`
- [x] 6.4 Create `AuthController` with `POST /api/v1/auth/register`, `POST /api/v1/auth/login`, `POST /api/v1/auth/refresh`
- [x] 6.5 Configure JWT Bearer authentication middleware in `Program.cs`
- [x] 6.6 Configure Swashbuckle with JWT Bearer security definition in `Program.cs`
- [x] 6.7 Map `/swagger` UI in development environment
- [x] 6.8 Register health checks for SQL Server and map `/health` endpoint
- [x] 6.9 Configure Serilog with console and rolling-file sinks, JSON in production
- [x] 6.10 Enrich Serilog logs with CorrelationId, environment, and machine name
- [x] 6.11 Create `DependencyInjection.cs` with `AddPresentation(IServiceCollection)` extension method
- [x] 6.12 Wire `AddApplication()`, `AddInfrastructure(config)`, `AddPresentation()` in `Program.cs`
- [x] 6.13 Register `ExceptionHandlingMiddleware` in middleware pipeline
- [x] 6.14 Add `appsettings.json` and `appsettings.Development.json` with `ConnectionStrings`, `JwtSettings` sections
- [x] 6.15 Enable XML documentation generation in `API.csproj` for Swagger

## 7. Validation & Error Handling Wiring

- [x] 7.1 Verify `ValidationBehaviour` throws `ValidationException` before handler on invalid request (manual test or unit test)
- [x] 7.2 Verify `ExceptionHandlingMiddleware` maps `ValidationException` → 422 with errors extension
- [x] 7.3 Verify `ExceptionHandlingMiddleware` maps `NotFoundException` → 404
- [x] 7.4 Verify `ExceptionHandlingMiddleware` maps unknown exception → 500 without stack trace in production

## 8. Tests

- [x] 8.1 Add `FluentAssertions`, `Moq`, `Microsoft.EntityFrameworkCore.InMemory` to unit test project
- [x] 8.2 Write unit test: `ValidationBehaviour` throws on invalid request
- [x] 8.3 Write unit test: `TokenService.GenerateAccessToken` returns valid JWT with correct claims
- [x] 8.4 Write unit test: `NotFoundException` message format
- [x] 8.5 Add `Microsoft.AspNetCore.Mvc.Testing`, `Testcontainers.MsSql` to integration test project
- [x] 8.6 Create `WebApplicationFactory` fixture for integration tests
- [x] 8.7 Write integration test: `POST /api/v1/auth/register` → 201 with new user id
- [x] 8.8 Write integration test: `POST /api/v1/auth/login` → 200 with token pair
- [x] 8.9 Write integration test: `POST /api/v1/auth/refresh` → 200 with new token pair
- [x] 8.10 Write integration test: protected endpoint without token → 401

## 9. Final Verification

- [ ] 9.1 Run `dotnet build` — zero errors and warnings
- [ ] 9.2 Run `dotnet test` — all tests pass
- [ ] 9.3 Run API locally and verify Swagger UI renders all endpoints at `/swagger`
- [ ] 9.4 Run `GET /health` and confirm `{"status":"Healthy"}` response
- [ ] 9.5 Verify `dotnet ef database update` applies `InitialCreate` migration without errors
