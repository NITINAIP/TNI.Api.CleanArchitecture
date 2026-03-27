## Why

A reusable .NET 8 Web API template following Clean Architecture principles is needed to standardize project scaffolding across teams — reducing setup time, enforcing separation of concerns, and embedding best practices (CQRS, Repository Pattern, JWT auth, global error handling) from day one.

## What Changes

- Create a new .NET 8 Web API solution structured around Clean Architecture layers
- Introduce Domain, Application, Infrastructure, and API (Presentation) project layers
- Add CQRS pattern using MediatR for command/query separation
- Add Entity Framework Core with SQL Server for persistence
- Add JWT Bearer authentication and authorization
- Add global exception handling middleware
- Add FluentValidation for request validation
- Add AutoMapper for object mapping
- Add Swagger/OpenAPI documentation
- Add health check endpoints
- Add structured logging with Serilog
- Add unit and integration test project stubs

## Capabilities

### New Capabilities

- `domain-layer`: Core domain entities, value objects, domain events, and interfaces with no external dependencies
- `application-layer`: CQRS commands/queries with MediatR, service interfaces, DTOs, validation pipeline, and AutoMapper profiles
- `infrastructure-layer`: EF Core DbContext, repository implementations, JWT token service, and external service integrations
- `api-layer`: ASP.NET Core controller endpoints, middleware pipeline, Swagger setup, health checks, and dependency injection wiring
- `authentication`: JWT Bearer token issuance and validation supporting user login/register flows
- `error-handling`: Global exception handling middleware returning RFC 7807 Problem Details responses
- `validation-pipeline`: MediatR pipeline behavior using FluentValidation for automatic request validation

### Modified Capabilities

<!-- No existing capabilities — this is a net-new project creation -->

## Impact

- **New solution**: `tni.api.CleanArchitecture.sln` with four projects under `src/` and test projects under `tests/`
- **Dependencies added**: MediatR, EF Core, FluentValidation, AutoMapper, Serilog, Swashbuckle, Microsoft.AspNetCore.Authentication.JwtBearer
- **APIs**: RESTful endpoints established under `/api/v1/`
- **Database**: EF Core migrations pattern established for SQL Server
- **No breaking changes** — this is a greenfield template project
