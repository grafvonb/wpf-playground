---

description: "Task list for the database-backed calculator facade"
---

# Tasks: Database-Backed Calculator Facade

**Input**: Design documents from `/specs/002-sql-calculator-facade/`

**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/, quickstart.md

**Tests**: Included, because FR-011 requires automated tests (unit tests without a database, plus integration tests against the local SQL Server).

**Constraints (constitution)**: .NET Framework 4.8, C# 7.3 only, one class per file, MSTest, no new NuGet packages.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: can run in parallel (different files, no dependencies)
- **[Story]**: US1, US2, US3 from spec.md

## Phase 1: Setup

**Purpose**: Folders and project references

- [X] T001 Create the `database/` folder at the repository root for the SQL scripts
- [X] T002 Add `<Reference Include="System.Data" />` and `<Reference Include="System.Configuration" />` as needed to `src/Calculator.Service/Calculator.Service.csproj` and `src/Calculator.App/Calculator.App.csproj`, with no NuGet packages (research.md item 1)

---

## Phase 2: Foundational (blocking prerequisites)

**Purpose**: Shared types and the test seam that every story needs

**CRITICAL**: complete before any user story

- [X] T003 [P] Add `Unavailable` to the enum in `src/Calculator.Service/CalculationError.cs`, appended after `Overflow` (data-model.md `CalculationError.Unavailable`)
- [X] T004 [P] Create `src/Calculator.Service/Internal/ProcedureOutcome.cs` with Status (Ok, DivisionByZero, Overflow) and Value per data-model.md `ProcedureOutcome.Status` and `ProcedureOutcome.Value`; the Status enum goes in its own file `src/Calculator.Service/Internal/ProcedureStatus.cs`
- [X] T005 [P] Create `src/Calculator.Service/Internal/IProcedureExecutor.cs` with `ProcedureOutcome Execute(string procedureName, decimal left, decimal right)`; it may throw for infrastructure failures (research.md item 8)
- [X] T006 [P] Create `database/001_create_database.sql`, an idempotent script that creates `CalculatorDb` only if it does not exist (contracts/stored-procedures.md)

**Checkpoint**: shared types compile; `dotnet build WpfPlayground.sln` is green

---

## Phase 3: User Story 1 - Calculate via database procedures (Priority: P1) MVP

**Goal**: Add, Subtract, Multiply and Divide run through stored procedures with results identical to `CalculatorFacade`.

**Independent Test**: Run the integration tests against the local database and compare with `CalculatorFacade` for a reference set; unit tests verify status mapping without a database.

### Tests for User Story 1

- [X] T007 [P] [US1] Create `tests/Calculator.Tests/FakeProcedureExecutor.cs`, a configurable fake `IProcedureExecutor` (returns a given outcome or throws a given exception, records calls)
- [X] T008 [P] [US1] Create `tests/Calculator.Tests/SqlCalculatorFacadeTests.cs` with unit tests: status Ok gives Success(value); DivisionByZero status gives DivisionByZero with the same message as `CalculatorFacade`; Overflow status gives Overflow with the same message; a zero divisor never reaches the executor; each operation calls its own procedure (`Calc_Add`, `Calc_Subtract`, `Calc_Multiply`, `Calc_Divide`)
- [X] T009 [P] [US1] Create `tests/Calculator.Tests/SqlCalculatorFacadeIntegrationTests.cs` with a parity test over a reference set (positive, negative, zero, many decimals, max/min decimal values, overflow cases) comparing `SqlCalculatorFacade` with `CalculatorFacade`; each test calls `Assert.Inconclusive` when the database is unreachable; the connection string comes from the `CALCULATOR_TEST_CONNECTION` environment variable, defaulting to `Server=.;Database=CalculatorDb;Integrated Security=true;Connect Timeout=5`

### Implementation for User Story 1

- [X] T010 [US1] Create `database/002_create_procedures.sql` with `CREATE OR ALTER PROCEDURE` for `dbo.Calc_Add`, `dbo.Calc_Subtract`, `dbo.Calc_Multiply`, `dbo.Calc_Divide`, following contracts/stored-procedures.md (return codes 0, 1, 2; TRY/CATCH mapping error 8115 to 2; `@Result` NULL on failure)
- [X] T011 [US1] Create `src/Calculator.Service/Internal/SqlProcedureExecutor.cs`: opens a new connection per call, sets `CommandType.StoredProcedure`, `CommandTimeout = 5`, binds typed `SqlDbType.Decimal` parameters (never string concatenation), reads the return code and output, converts out-of-range values to the Overflow status (research.md items 2, 3, 5)
- [X] T012 [US1] Create `src/Calculator.Service/SqlCalculatorFacade.cs` implementing `ICalculatorFacade` with a public constructor taking the connection string; a zero divisor returns DivisionByZero without calling the database; map statuses per the data-model.md mapping table; keep it stateless (depends on T003-T005, T011)
- [X] T013 [US1] Run the scripts with `sqlcmd -S . -E -b -i database\001_create_database.sql` and `database\002_create_procedures.sql`, run T009, and adjust the parameter types in T010/T011 until the parity set passes (research.md item 2); if exact parity is impossible, return Overflow and record the finding in research.md

**Checkpoint**: US1 works end to end; unit and integration tests pass

---

## Phase 4: User Story 2 - Graceful handling of database problems (Priority: P2)

**Goal**: Database outages, missing procedures and timeouts return failures, never exceptions.

**Independent Test**: Use the fake executor to throw, and an unreachable connection string for the real executor; every operation returns a failure result.

### Tests for User Story 2

- [X] T014 [P] [US2] Add tests to `tests/Calculator.Tests/SqlCalculatorFacadeTests.cs`: the executor throwing `SqlException`-like, `InvalidOperationException` and `TimeoutException` yields Failure(Unavailable) with a readable message for all four operations, and nothing is thrown
- [X] T015 [P] [US2] Add tests to `tests/Calculator.Tests/SqlCalculatorFacadeIntegrationTests.cs`: a connection string to a non-existent server or database returns Unavailable within about 6 seconds; a missing procedure (call through the executor with an unknown name) returns Unavailable while the other operations still work; the facade is called from several threads at once and all results are correct (FR-007)

### Implementation for User Story 2

- [X] T016 [US2] In `src/Calculator.Service/SqlCalculatorFacade.cs`, catch `SqlException`, `InvalidOperationException` and `TimeoutException` around the executor call and return `CalculationResult.Failure(CalculationError.Unavailable, "The calculation service is unavailable")`; make sure `Connect Timeout` and the 5-second command timeout are enforced in `src/Calculator.Service/Internal/SqlProcedureExecutor.cs` (research.md item 4)

**Checkpoint**: US1 and US2 pass together

---

## Phase 5: User Story 3 - Choose the calculation backend (Priority: P3)

**Goal**: A setting in `App.config` chooses the in-process or database backend at startup without UI changes.

**Independent Test**: Start the app with each setting and calculate; unit-test the selection logic.

### Tests for User Story 3

- [X] T017 [P] [US3] Create `tests/Calculator.Tests/CalculatorFacadeFactoryTests.cs`: missing setting gives `CalculatorFacade`; `InProcess` gives `CalculatorFacade`; `Sql` with a connection string gives `SqlCalculatorFacade`; an unknown value throws a descriptive `ConfigurationErrorsException`; `InProcess` never reads the connection string

### Implementation for User Story 3

- [X] T018 [P] [US3] Create `src/Calculator.App/App.config` with `CalculatorBackend` = `InProcess` and the `CalculatorDb` connection string (`Server=.;Database=CalculatorDb;Integrated Security=true;Connect Timeout=5`), per contracts/configuration.md; no credentials
- [X] T019 [US3] Create `src/Calculator.App/CalculatorFacadeFactory.cs` with a method that takes backend name and a connection-string provider and returns the facade (testable without a config file)
- [X] T020 [US3] Update `src/Calculator.App/App.xaml.cs` to read the settings through `ConfigurationManager` and build the facade with `CalculatorFacadeFactory`; on a configuration error show a clear message and shut down; keep all logic out of code-behind beyond composition (constitution II) (depends on T018, T019)

**Checkpoint**: all three stories work

---

## Phase 6: Polish & Cross-Cutting

- [X] T021 [P] Add a short "Database backend" section to the repository README (or create `README.md` if absent) pointing to `specs/002-sql-calculator-facade/quickstart.md`
- [X] T022 Run `dotnet build WpfPlayground.sln` and `dotnet test tests\Calculator.Tests\Calculator.Tests.csproj`, and confirm no C# 8 features and no new packages
- [X] T023 Run the steps in `specs/002-sql-calculator-facade/quickstart.md` end to end, including stopping the SQL Server service and checking the "unavailable" message within about 5 seconds

---

## Dependencies & Execution Order

- Setup (T001-T002) has no dependencies.
- Foundational (T003-T006) depends on Setup and blocks all stories; T003-T006 are parallel.
- US1 depends on Foundational. Order: T007-T009 (tests, parallel), T010, T011, T012, T013.
- US2 depends on US1 (it extends T011 and T012). US3 depends only on Foundational and on `SqlCalculatorFacade` existing (T012) for its Sql case.
- Polish depends on all stories.

```text
Setup -> Foundational -> US1 -> US2
                           \-> US3
                                 \-> Polish
```

## Parallel Examples

```text
Foundational: T003, T004, T005, T006 together
US1 tests:    T007, T008, T009 together
US3:          T017 and T018 together
```

## Implementation Strategy

- **MVP**: Phases 1-3 (User Story 1). Stop and validate with the integration parity tests.
- **Incremental**: add US2 (resilience), then US3 (config switch), then Polish.
- Commit after each task or logical group, using Conventional Commits with a scope (for example `feat(service): add sql calculator facade`).
