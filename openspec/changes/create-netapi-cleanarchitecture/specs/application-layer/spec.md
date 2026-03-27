## ADDED Requirements

### Requirement: CQRS commands and queries are handled by MediatR
The system SHALL implement all business operations as MediatR `IRequest<T>` commands or queries handled by corresponding `IRequestHandler<TRequest, TResponse>` classes in the `Application` layer. No handler SHALL directly depend on `DbContext` or any infrastructure type.

#### Scenario: Command handler resolves via MediatR
- **WHEN** a command request is sent via `IMediator.Send(command)`
- **THEN** the corresponding `IRequestHandler` SHALL execute and return a result, without the caller knowing the handler's type

#### Scenario: Query handler returns a DTO
- **WHEN** a query request is dispatched
- **THEN** the handler SHALL return a DTO (not a domain entity) mapped via AutoMapper

---

### Requirement: Application layer depends only on Domain
The `Application.csproj` SHALL reference only `Domain` and have no direct reference to `Infrastructure`, ASP.NET Core web hosting, or EF Core packages.

#### Scenario: Application project has no Infrastructure reference
- **WHEN** the `Application.csproj` project references are examined
- **THEN** no reference to `Infrastructure` project or EF Core shall exist

---

### Requirement: AutoMapper profiles map entities to DTOs
The system SHALL provide AutoMapper `Profile` classes in the `Application` layer for every entity-to-DTO mapping. Mapping profiles SHALL be auto-discovered via `AddAutoMapper(Assembly)`.

#### Scenario: Entity maps to response DTO
- **WHEN** AutoMapper maps a domain entity to its response DTO
- **THEN** all annotated fields SHALL be correctly projected without manual mapping code in the handler

#### Scenario: Unmapped member causes test failure
- **WHEN** `mapper.ConfigurationProvider.AssertConfigurationIsValid()` is called in tests
- **THEN** it SHALL throw if any destination member is unmapped and not explicitly ignored

---

### Requirement: MediatR pipeline behaviors are applied in order
The Application layer SHALL register pipeline behaviors: (1) `LoggingBehaviour`, (2) `ValidationBehaviour`, (3) `PerformanceBehaviour`. They SHALL execute in registration order before the handler.

#### Scenario: Validation runs before handler on invalid request
- **WHEN** a request with validation errors is dispatched
- **THEN** `ValidationBehaviour` SHALL throw a `ValidationException` before the handler is invoked

#### Scenario: Logging captures request and response
- **WHEN** any MediatR request completes
- **THEN** `LoggingBehaviour` SHALL log the request name, input (sanitized), execution time, and response type
