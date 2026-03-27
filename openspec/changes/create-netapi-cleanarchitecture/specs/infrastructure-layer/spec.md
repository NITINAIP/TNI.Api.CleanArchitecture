## ADDED Requirements

### Requirement: EF Core DbContext is configured in Infrastructure
The system SHALL provide an `ApplicationDbContext` in the `Infrastructure` project that inherits `DbContext`. All entity type configurations SHALL use the Fluent API via `IEntityTypeConfiguration<T>` classes, not Data Annotations on the domain entity.

#### Scenario: Entity configuration is in Infrastructure, not Domain
- **WHEN** all `.cs` files in the `Domain` project are inspected
- **THEN** no `[Column]`, `[Table]`, or `[Key]` Data Annotation attributes SHALL be present on entity classes

#### Scenario: DbContext discovers configurations automatically
- **WHEN** `ApplicationDbContext.OnModelCreating` executes
- **THEN** it SHALL call `modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly)` to auto-register all `IEntityTypeConfiguration<T>` classes

---

### Requirement: Repository pattern wraps EF Core access
The system SHALL implement `Repository<T>` in Infrastructure that satisfies the `IRepository<T>` interface defined in Domain. The implementation SHALL support async CRUD operations: `GetByIdAsync`, `GetAllAsync`, `AddAsync`, `Update`, `Delete`, and `SaveChangesAsync`.

#### Scenario: Repository returns null for non-existent entity
- **WHEN** `GetByIdAsync` is called with an ID that does not exist in the database
- **THEN** it SHALL return `null` (not throw an exception)

#### Scenario: Unit of work commits all pending changes
- **WHEN** `IUnitOfWork.CommitAsync()` is called
- **THEN** all pending EF Core tracked changes SHALL be persisted in a single database transaction

---

### Requirement: Infrastructure registers its dependencies in an extension method
The Infrastructure layer SHALL expose a single `AddInfrastructure(IServiceCollection, IConfiguration)` extension method that registers `ApplicationDbContext`, repositories, `IUnitOfWork`, `ITokenService`, and all other infrastructure services with appropriate lifetimes.

#### Scenario: API startup only calls AddInfrastructure
- **WHEN** `Program.cs` configures services
- **THEN** a single call to `services.AddInfrastructure(configuration)` SHALL register all infrastructure dependencies without additional manual wiring in the API layer
