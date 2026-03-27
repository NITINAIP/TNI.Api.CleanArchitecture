## ADDED Requirements

### Requirement: Unhandled exceptions return RFC 7807 Problem Details
The system SHALL include an `ExceptionHandlingMiddleware` in the API pipeline that catches all unhandled exceptions and writes a JSON response conforming to RFC 7807 `ProblemDetails` format. No raw exception messages or stack traces SHALL be returned to clients in non-development environments.

#### Scenario: ValidationException returns 422 Unprocessable Entity
- **WHEN** a `ValidationException` propagates out of the MediatR pipeline
- **THEN** the middleware SHALL return `422 Unprocessable Entity` with a `ProblemDetails` body containing an `errors` extension field listing field-level validation failures

#### Scenario: NotFoundException returns 404 Not Found
- **WHEN** a `NotFoundException` is thrown (e.g., entity not found in repository)
- **THEN** the middleware SHALL return `404 Not Found` with a `ProblemDetails` body

#### Scenario: Unrecognized exception returns 500 Internal Server Error
- **WHEN** any other unhandled exception is thrown
- **THEN** the middleware SHALL return `500 Internal Server Error` with a generic `ProblemDetails` body and SHALL NOT expose the exception message or stack trace in non-development environments

#### Scenario: Error is logged regardless of type
- **WHEN** `ExceptionHandlingMiddleware` catches any exception
- **THEN** it SHALL log the exception details (type, message, stack trace) via `ILogger` before writing the response

---

### Requirement: Exception base types are defined in Application layer
The system SHALL define custom exception base classes in the `Application` layer: `ValidationException`, `NotFoundException`, and `ForbiddenAccessException`. These SHALL be the only exception types thrown deliberately by Application handlers.

#### Scenario: NotFoundException carries entity name and key
- **WHEN** `NotFoundException` is constructed with entity name and key value
- **THEN** its `Message` property SHALL read: `"Entity '{EntityName}' ({Key}) was not found."`
