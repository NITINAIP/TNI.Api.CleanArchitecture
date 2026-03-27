## ADDED Requirements

### Requirement: Domain entities are persistence-ignorant
The system SHALL define domain entities in the `TNI.Api.CleanArchitecture.Domain` project with no references to EF Core, ASP.NET Core, or any infrastructure concern. Entities SHALL inherit from a `BaseEntity` abstract class that includes `Id`, `CreatedAt`, and `UpdatedAt` audit fields.

#### Scenario: Domain project has no EF Core dependency
- **WHEN** the `Domain.csproj` project is inspected
- **THEN** it SHALL contain no reference to `Microsoft.EntityFrameworkCore` or any infrastructure NuGet package

#### Scenario: BaseEntity provides audit fields
- **WHEN** a new entity class is created that inherits `BaseEntity`
- **THEN** it SHALL automatically carry `Id` (Guid), `CreatedAt` (DateTimeOffset), and `UpdatedAt` (DateTimeOffset) properties

---

### Requirement: Domain interfaces define persistence contracts
The system SHALL define repository and unit-of-work interfaces in the `Domain` layer under a `Repositories` namespace so that the Application layer can depend on abstractions without knowing about EF Core.

#### Scenario: IRepository interface is in Domain project
- **WHEN** the `Domain` project's namespace tree is inspected
- **THEN** `IRepository<T>` and `IUnitOfWork` interfaces SHALL exist under `TNI.Api.CleanArchitecture.Domain.Repositories`

#### Scenario: Application can reference IRepository without Infrastructure
- **WHEN** the `Application.csproj` dependency graph is resolved
- **THEN** it SHALL compile successfully with only a reference to `Domain`, never to `Infrastructure`

---

### Requirement: Value objects encapsulate domain invariants
The system SHALL support value object types (immutable, equality by value) to encapsulate domain rules such as email address format and money amounts.

#### Scenario: Value object equality is by value
- **WHEN** two value object instances are created with identical field values
- **THEN** they SHALL be considered equal (`==` and `.Equals()` return `true`)

#### Scenario: Value object rejects invalid data at construction
- **WHEN** a value object is constructed with invalid data (e.g., malformed email)
- **THEN** it SHALL throw a domain exception rather than create an invalid object
