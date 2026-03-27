## ADDED Requirements

### Requirement: API project wires all layers via Dependency Injection
The `API` project (`TNI.Api.CleanArchitecture.API`) SHALL configure the DI container in `Program.cs` by calling `AddApplication()`, `AddInfrastructure(config)`, and `AddPresentation()` extension methods. No service registration SHALL occur inline in `Program.cs`.

#### Scenario: Program.cs uses only layer-level DI extension methods
- **WHEN** `Program.cs` is inspected
- **THEN** it SHALL contain calls to exactly `AddApplication()`, `AddInfrastructure(configuration)`, and `AddPresentation()` for service registration — no other `services.Add*()` calls for business logic

---

### Requirement: Controllers are thin and delegate to MediatR
All API controllers SHALL inherit from `BaseApiController` which injects `IMediator`. Controller action methods SHALL only construct a request object, call `_mediator.Send(request)`, and return an HTTP result — zero business logic in controllers.

#### Scenario: Controller action has no business logic
- **WHEN** a controller action method body is inspected
- **THEN** it SHALL contain at most: input model construction, `_mediator.Send()`, and returning an `ActionResult`

#### Scenario: Controller returns 200 OK with DTO on success
- **WHEN** a GET endpoint is called with a valid request
- **THEN** it SHALL return `200 OK` with the DTO serialized as JSON

---

### Requirement: Swagger/OpenAPI documentation is auto-generated
The system SHALL configure Swashbuckle so that Swagger UI is available at `/swagger` in development. All endpoints SHALL be documented with `[ProducesResponseType]` attributes and XML doc comments.

#### Scenario: Swagger UI is accessible in development
- **WHEN** the API runs with `ASPNETCORE_ENVIRONMENT=Development` and a browser navigates to `/swagger`
- **THEN** the Swagger UI SHALL render with all API endpoints visible

#### Scenario: Swagger shows response types
- **WHEN** a controller action has `[ProducesResponseType(typeof(ResponseDto), 200)]`
- **THEN** the Swagger schema SHALL display the response DTO shape

---

### Requirement: Health check endpoint is available
The system SHALL expose a health check endpoint at `/health` that returns `200 OK` with `{"status":"Healthy"}` when all dependencies (database) are reachable.

#### Scenario: Health endpoint returns healthy when DB is reachable
- **WHEN** `GET /health` is called and the database is available
- **THEN** the response SHALL be `200 OK` with status `Healthy`

#### Scenario: Health endpoint returns unhealthy when DB is unreachable
- **WHEN** `GET /health` is called and the database connection fails
- **THEN** the response SHALL be `503 Service Unavailable` with status `Unhealthy`
