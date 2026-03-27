## ADDED Requirements

### Requirement: User registration creates a new account
The system SHALL allow a new user to register by providing an email address and password. The password SHALL be hashed using BCrypt before storage. Duplicate email addresses SHALL be rejected.

#### Scenario: Successful registration
- **WHEN** `POST /api/v1/auth/register` is called with a unique email and a valid password
- **THEN** the system SHALL create the user, return `201 Created`, and include the user's `id` and `email` in the response

#### Scenario: Duplicate email is rejected
- **WHEN** `POST /api/v1/auth/register` is called with an email that already exists
- **THEN** the system SHALL return `409 Conflict` with a problem details body

#### Scenario: Weak password is rejected
- **WHEN** `POST /api/v1/auth/register` is called with a password shorter than 8 characters
- **THEN** the system SHALL return `422 Unprocessable Entity` with validation error details

---

### Requirement: User login issues JWT access and refresh tokens
The system SHALL authenticate a user by email and password. On success, it SHALL return a short-lived JWT access token (60 minutes) and a long-lived refresh token (7 days) stored in the `RefreshTokens` table.

#### Scenario: Successful login
- **WHEN** `POST /api/v1/auth/login` is called with valid credentials
- **THEN** the response SHALL be `200 OK` containing `accessToken` (JWT string) and `refreshToken` (opaque string)

#### Scenario: Invalid credentials return 401
- **WHEN** `POST /api/v1/auth/login` is called with an incorrect password
- **THEN** the response SHALL be `401 Unauthorized` with a problem details body

---

### Requirement: Access token can be refreshed
The system SHALL allow a client to exchange a valid, non-expired refresh token for a new access token and a new refresh token (token rotation). The old refresh token SHALL be invalidated upon use.

#### Scenario: Valid refresh token returns new token pair
- **WHEN** `POST /api/v1/auth/refresh` is called with a valid unexpired refresh token
- **THEN** the response SHALL return a new `accessToken` and `refreshToken`, and the old refresh token SHALL be marked as used

#### Scenario: Expired or used refresh token returns 401
- **WHEN** `POST /api/v1/auth/refresh` is called with an expired or already-used token
- **THEN** the response SHALL return `401 Unauthorized`

---

### Requirement: Protected endpoints require a valid JWT
All endpoints decorated with `[Authorize]` SHALL require the caller to provide a valid, unexpired JWT Bearer token in the `Authorization` header.

#### Scenario: Missing token returns 401
- **WHEN** a protected endpoint is called without an `Authorization` header
- **THEN** the response SHALL be `401 Unauthorized`

#### Scenario: Valid token grants access
- **WHEN** a protected endpoint is called with a valid JWT Bearer token
- **THEN** the request SHALL proceed and return the expected response
