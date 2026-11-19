# Feature Specification: Database-Backed Calculator Facade

**Feature Branch**: `002-sql-calculator-facade`

**Created**: 2026-11-19

**Status**: Draft

**Input**: User description: "add implementation of ICalculatorFacade that uses stored procedures from the local installed sql server database"

## Clarifications

### Session 2026-11-19

- Q: How should results that cannot be represented exactly be handled? → A: Match the existing in-process calculator exactly (same rounding, Overflow for anything it would reject).
- Q: How is the backend chosen? → A: A setting in the app's configuration file, read at startup; default is in-process.
- Q: What happens when the database backend is selected but unreachable? → A: Show a clear error for that calculation; no fallback to in-process.
- Q: What is the maximum wait for the database before a calculation fails? → A: 5 seconds.
- Q: Who creates the database and procedures? → A: The developer runs the provided scripts manually; the app never changes the schema.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Calculate via database procedures (Priority: P1)

A user of the calculator performs add, subtract, multiply and divide operations, and each calculation is carried out by the locally installed database's stored procedures instead of in-process arithmetic. Results appear exactly as they do with the existing calculator.

**Why this priority**: This is the core of the feature; without it there is no database-backed calculation.

**Independent Test**: Call each of the four operations on the database-backed facade with sample inputs and compare the results with the expected values and the existing in-process facade.

**Acceptance Scenarios**:

1. **Given** the local database is reachable and the procedures are installed, **When** the user adds 2.5 and 3.5, **Then** the result is a success with value 6.0.
2. **Given** the same conditions, **When** the user subtracts, multiplies or divides valid numbers, **Then** each returns a success with the mathematically correct value.
3. **Given** the same conditions, **When** the user divides by zero, **Then** the result is a failure with the DivisionByZero error and a readable message, and no exception reaches the caller.

---

### User Story 2 - Graceful handling of database problems (Priority: P2)

When the database is stopped, unreachable, or lacks the expected procedures, the user gets a clear failure message instead of a crash, and the application keeps running.

**Why this priority**: The facade contract promises no exceptions for expected failures; a database dependency introduces new expected failures.

**Independent Test**: Point the facade at an unavailable database and invoke any operation; verify a failure result with a clear message is returned.

**Acceptance Scenarios**:

1. **Given** the database service is not running, **When** any operation is invoked, **Then** a failure result with a clear "calculation service unavailable" message is returned and no exception is thrown.
2. **Given** a required procedure is missing, **When** its operation is invoked, **Then** a failure result is returned and other operations remain unaffected.

---

### User Story 3 - Choose the calculation backend (Priority: P3)

The application can be configured to use either the existing in-process calculator or the database-backed one without changing how the UI uses the facade.

**Why this priority**: Keeps the app usable without a database and allows side-by-side comparison, but is not required to deliver the core value.

**Independent Test**: Switch the setting and confirm the UI behaves identically with both backends for valid inputs.

**Acceptance Scenarios**:

1. **Given** the database backend is selected, **When** the user calculates, **Then** the database performs the calculation.
2. **Given** the in-process backend is selected, **When** the user calculates, **Then** no database connection is attempted.

---

### Edge Cases

- Values at the limits of the supported number range and results that exceed it (must yield the Overflow failure, not a crash).
- Numbers with many decimal places: precision must match the in-process calculator.
- Negative numbers and zero operands.
- Database becomes unavailable between two calls; the next call succeeds once it is back.
- Many calculations invoked concurrently from several threads.
- Database call takes longer than 5 seconds: the user receives a failure instead of waiting indefinitely.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: A database-backed implementation of the calculator facade MUST exist and provide Add, Subtract, Multiply and Divide.
- **FR-002**: Each operation MUST be performed by a dedicated stored procedure in the local SQL Server database.
- **FR-003**: The feature MUST provide the stored procedures (installable scripts) so a fresh local database can be prepared by a developer running them manually; the application MUST NOT create or alter the database schema.
- **FR-004**: Results MUST be identical in value and precision to the existing in-process calculator for the same inputs.
- **FR-005**: Division by zero MUST return the DivisionByZero failure; any result the existing in-process calculator would reject as out of range MUST return the Overflow failure, and any result it would round MUST be rounded identically.
- **FR-006**: Database unavailability, missing procedures, or timeouts (a call exceeding 5 seconds) MUST return a failure result with a readable message and MUST NOT throw to the caller. There MUST be no silent fallback to the in-process calculator.
- **FR-007**: The facade MUST be stateless and safe for concurrent use.
- **FR-008**: The database connection settings MUST be configurable and MUST NOT be hard-coded in source; credentials MUST NOT be stored in the repository.
- **FR-009**: Calls MUST be parameterized so user-entered values can never be interpreted as commands.
- **FR-010**: The application MUST select the in-process or database-backed implementation from a setting in its configuration file, read at startup, defaulting to in-process, without UI changes.
- **FR-011**: The database-backed behavior MUST be covered by automated tests, including failure paths, runnable without a real database where possible.

### Key Entities

- **Calculation request**: Operation type plus two decimal operands.
- **Calculation result**: Success with a value, or failure with an error category and message (existing concept, extended with an "unavailable" category for database problems).
- **Stored procedures**: One per arithmetic operation, accepting two operands and returning the result or an error indication.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: 100% of a reference set of valid calculations return the same results as the existing calculator.
- **SC-002**: 100% of error cases (divide by zero, overflow, database unavailable) produce a readable failure message with zero crashes.
- **SC-003**: A typical calculation completes in under 1 second from the user's perspective on a local database.
- **SC-004**: A new developer can prepare the database and run the app on the database backend in under 10 minutes using the documented steps.

## Assumptions

- SQL Server is already installed locally and reachable using the current Windows user's integrated login.
- The database and procedures are created by scripts delivered with this feature; the existing facade interface stays unchanged.
- The in-process calculator remains the default backend when none is configured.
- The new "unavailable" failure category is added to the existing error list; existing categories keep their meaning.
- Only the four existing operations are in scope; no history or persistence of calculations.
- The project's constitution constraints (.NET Framework 4.8, C# 7.3, MSTest) apply.
