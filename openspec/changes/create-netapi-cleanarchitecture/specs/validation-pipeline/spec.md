## ADDED Requirements

### Requirement: All MediatR requests are validated before handler execution
The system SHALL include a `ValidationBehaviour<TRequest, TResponse>` MediatR pipeline behavior that automatically runs all registered FluentValidation `IValidator<TRequest>` implementations before the handler is called. If validation fails, it SHALL throw a `ValidationException` containing all failures — the handler SHALL NOT be invoked.

#### Scenario: Invalid request throws ValidationException before handler
- **WHEN** a MediatR request is dispatched that fails one or more FluentValidation rules
- **THEN** the handler's `Handle` method SHALL NOT be called, and a `ValidationException` SHALL be thrown with all validation errors

#### Scenario: Valid request passes through to handler
- **WHEN** a MediatR request is dispatched that satisfies all registered validators
- **THEN** the pipeline SHALL call the handler and return its result normally

#### Scenario: Request with no registered validator passes through
- **WHEN** a MediatR request is dispatched and no `IValidator<TRequest>` is registered
- **THEN** the pipeline SHALL proceed directly to the handler without error

---

### Requirement: Validators are co-located with their request in Application layer
Each FluentValidation `AbstractValidator<TCommand>` or `AbstractValidator<TQuery>` class SHALL be defined in the same folder/namespace as the request it validates within the `Application` layer.

#### Scenario: Validator file is in same namespace as request
- **WHEN** a command `CreateUserCommand.cs` exists at `Application/Users/Commands/CreateUser/`
- **THEN** its validator `CreateUserCommandValidator.cs` SHALL also exist at `Application/Users/Commands/CreateUser/`

---

### Requirement: Validators are auto-registered with the DI container
The system SHALL register all `IValidator<T>` implementations from the `Application` assembly automatically using `services.AddValidatorsFromAssembly(typeof(ApplicationAssemblyMarker).Assembly)` — no manual per-validator registration is required.

#### Scenario: New validator is discovered without code change in DI setup
- **WHEN** a new `AbstractValidator<T>` class is added to the `Application` project
- **THEN** it SHALL be resolved by DI and invoked by `ValidationBehaviour` without modifying `DependencyInjection.cs`
