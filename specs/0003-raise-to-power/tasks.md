# Tasks: Raise to Power Operation

**Input**: Design documents from `/specs/0003-raise-to-power/`

**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/calculator-power-contract.md, quickstart.md

**Tests**: Included. The constitution (principle III) and FR-007 require MSTest tests in `tests\Calculator.Tests`, written before or with the implementation.

**Organization**: Tasks are grouped by user story. All paths are relative to the repository root. Code MUST stay C# 7.3 on `net48`.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies on incomplete tasks)
- **[Story]**: User story the task belongs to (US1, US2, US3)

## Phase 1: Setup

**Purpose**: Confirm a green baseline before changing anything

- [ ] T001 Build the solution and run the existing MSTest suite to confirm a passing baseline (SQL integration tests may be inconclusive when the database is unreachable)

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Shared types every story needs; the solution must compile after this phase

**CRITICAL**: No user story work can begin until this phase is complete

- [ ] T002 [P] Add `ZeroToNegativePower` and `NotRealNumber` to `CalculationError` in `src/Calculator.Service/CalculationError.cs` (see data-model.md, Type changes)
- [ ] T003 [P] Add `ZeroToNegativePower` and `NotRealNumber` to `ProcedureStatus` in `src/Calculator.Service/Internal/ProcedureStatus.cs` (see data-model.md, Type changes)
- [ ] T004 Add `CalculationResult Power(decimal @base, decimal exponent)` with an XML doc comment to `src/Calculator.Service/ICalculatorFacade.cs` per contracts/calculator-power-contract.md
- [ ] T005 [P] Add a recording `Power` method to `tests/Calculator.Tests/FakeCalculatorFacade.cs`, following the existing `Record("Divide", ...)` pattern (depends on T004)
- [ ] T006 Add temporary `Power` stubs that throw `NotImplementedException` to `src/Calculator.Service/CalculatorFacade.cs` and `src/Calculator.Service/SqlCalculatorFacade.cs` so the solution compiles; each story replaces its stub (depends on T004)

**Checkpoint**: Solution builds; existing tests still pass

---

## Phase 3: User Story 1 - Raise a number to a power (Priority: P1) MVP

**Goal**: A user enters a base and an exponent, presses the power button and sees `base ^ exponent` using the in-process backend.

**Independent Test**: With the in-process backend, enter 2 and 10, press `xʸ`, and see 1024; also 2 ^ -1 = 0.5, 9 ^ 0.5 = 3, any ^ 0 = 1 (quickstart.md manual table, valid rows).

### Tests for User Story 1

> Write these first and confirm they fail before implementing.

- [ ] T007 [P] [US1] Add valid-case tests to `tests/Calculator.Tests/CalculatorFacadeTests.cs`: 2^3 = 8, 2^10 = 1024, x^0 = 1 (including 0^0 and negative base), 2^-1 = 0.5, 9^0.5 = 3 within 12 significant digits, (-2)^3 = -8
- [ ] T008 [P] [US1] Add a data row for `CalculatorOperation.Power` to the operation-routing test in `tests/Calculator.Tests/MainViewModelTests.cs` (expects the fake to record `"Power"` with the two inputs)

### Implementation for User Story 1

- [ ] T009 [US1] Add `Power(decimal @base, decimal exponent)` to `src/Calculator.Service/Internal/Arithmetic.cs`: compute with `Math.Pow` on doubles and convert back to `decimal` per research.md Decision 1 (callers handle invalid inputs; see US2)
- [ ] T010 [US1] Replace the stub with the real `Power` in `src/Calculator.Service/CalculatorFacade.cs` using the existing private `Execute(() => ...)` helper (depends on T009)
- [ ] T011 [P] [US1] Add `Power` to the `CalculatorOperation` enum in `src/Calculator.App/ViewModels/CalculatorOperation.cs`
- [ ] T012 [US1] Add a `case CalculatorOperation.Power` calling `_facade.Power(left, right)` in `src/Calculator.App/ViewModels/MainViewModel.cs` (depends on T011)
- [ ] T013 [US1] Add the `xʸ` button (`OperationButtonStyle`, `ToolTip="Raise to power"`, `CalculatorOperation.Power`) after the divide button in `src/Calculator.App/MainWindow.xaml`; verify all five buttons fit the 360 px window (depends on T011)

**Checkpoint**: US1 works end to end on the in-process backend and is testable on its own

---

## Phase 4: User Story 2 - Clear errors for invalid results (Priority: P2)

**Goal**: Invalid power inputs show a specific, clear error and the calculator keeps working.

**Independent Test**: Enter 0 and -1, press `xʸ`, see "Cannot raise zero to a negative power"; -8 and 0.5 gives "Result is not a real number"; 10 and 1000 gives "Result is too large"; the next calculation still works.

### Tests for User Story 2

- [ ] T014 [P] [US2] Add failure tests to `tests/Calculator.Tests/CalculatorFacadeTests.cs`: 0^-1 gives `ZeroToNegativePower` with "Cannot raise zero to a negative power"; (-8)^0.5 gives `NotRealNumber` with "Result is not a real number"; 10^1000 gives `Overflow` with "Result is too large"; also a decimal-range overflow case such as `decimal.MaxValue ^ 2`
- [ ] T015 [P] [US2] Add a test to `tests/Calculator.Tests/MainViewModelTests.cs` that a `ZeroToNegativePower` failure from the fake shows its message in `ErrorText` and clears the result, mirroring the existing division-by-zero test

### Implementation for User Story 2

- [ ] T016 [US2] In `src/Calculator.Service/CalculatorFacade.cs` `Power`, apply the validation order from data-model.md (zero base with negative exponent, negative base with fractional exponent) before computing; return `CalculationResult.Failure` with the exact messages from FR-005
- [ ] T017 [US2] In `src/Calculator.Service/Internal/Arithmetic.cs` and `CalculatorFacade.cs`, treat NaN, infinity, or a value outside the `decimal` range as `Overflow` with message "Result is too large" (not the existing arithmetic overflow message) (depends on T016)

**Checkpoint**: US1 and US2 both work on the in-process backend

---

## Phase 5: User Story 3 - Same result regardless of calculation backend (Priority: P2)

**Goal**: The SQL-backed facade gives the same results and errors as the in-process facade.

**Independent Test**: Run the same valid and invalid inputs through both facades (integration test) and see matching results to 12 significant digits and identical error kinds and messages.

### Tests for User Story 3

- [ ] T018 [P] [US3] Add tests to `tests/Calculator.Tests/SqlCalculatorFacadeTests.cs` using `FakeProcedureExecutor`: `Power` calls `"dbo.Calc_Power"` with both operands; zero-to-negative and non-real inputs fail without calling the database (like `Divide_ByZero_FailsWithoutCallingDatabase`); executor statuses `ZeroToNegativePower`, `NotRealNumber` and `Overflow` map to the right error and message
- [ ] T019 [P] [US3] Add power cases to the parity test in `tests/Calculator.Tests/SqlCalculatorFacadeIntegrationTests.cs` comparing `_reference.Power` with `_sql.Power` for valid, exponent 0, negative, fractional and invalid inputs (inconclusive when the database is unreachable; honor `CALCULATOR_TEST_CONNECTION`)

### Implementation for User Story 3

- [ ] T020 [US3] Create `database/003_create_power_procedure.sql` with a re-runnable `CREATE OR ALTER PROCEDURE dbo.Calc_Power` (same parameters as the other `Calc_*` procedures, `USE CalculatorDb;`): exponent 0 returns 1, zero base with negative exponent returns code 3, negative base with non-integer exponent returns code 4, otherwise `POWER` on `FLOAT` cast to `DECIMAL(38,18)`; overflow errors (8115 and the float overflow error found in testing) return code 2 (research.md Decisions 1, 2, 4)
- [ ] T021 [P] [US3] Map return codes 3 and 4 to `ProcedureStatus.ZeroToNegativePower` and `ProcedureStatus.NotRealNumber` in `src/Calculator.Service/Internal/SqlProcedureExecutor.cs`
- [ ] T022 [US3] Replace the stub with the real `Power` in `src/Calculator.Service/SqlCalculatorFacade.cs`: pre-check invalid inputs like `Divide` does, call `Execute("dbo.Calc_Power", @base, exponent)`, and map the new statuses to the same errors and messages as the in-process facade; use "Result is too large" for power overflow (depends on T021)
- [ ] T023 [US3] If a local SQL Server is available, apply `database/003_create_power_procedure.sql` and run the integration tests; record the actual SQL error number raised by `POWER` overflow and adjust T020 if needed (depends on T019, T020, T022)

**Checkpoint**: All three stories work; both backends match

---

## Phase 6: Polish & Cross-Cutting Concerns

- [ ] T024 [P] Update `README.md`: add the power operation wherever operations are listed, mention the new `003_create_power_procedure.sql` script, and document the divergences (12-significant-digit comparison, SQL results below 10^20 versus the `decimal` range in-process, existing operand limits) per constitution principle IV
- [ ] T025 Build the solution and run the whole test suite; fix any failures (all tests must pass before commit)
- [ ] T026 Run the manual table in `specs/0003-raise-to-power/quickstart.md` on both backends

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: none
- **Foundational (Phase 2)**: after Setup; blocks all user stories
- **US1 (Phase 3)**: after Foundational; no dependency on other stories
- **US2 (Phase 4)**: after Foundational; edits the same `Power` method as US1, so do it after T010
- **US3 (Phase 5)**: after Foundational; T022 reuses the message wording from US2, so do it after T016 and T017 or keep both in sync by hand
- **Polish (Phase 6)**: after the desired stories are complete

### Within Each Story

- Tests first, confirm they fail, then implement
- `Arithmetic` before `CalculatorFacade`; enum before view model before XAML; executor mapping before SQL facade

### Parallel Opportunities

- T002 and T003; T005 alongside T006 once T004 is done
- T007 and T008; T011 alongside T009
- T014 and T015; T018 and T019; T021 alongside T020
- US3's SQL script (T020) can proceed in parallel with US1 and US2 work; US3's other tasks need those stories' message wording

## Parallel Example: User Story 1

```text
Task: "Add valid-case tests in tests/Calculator.Tests/CalculatorFacadeTests.cs"          (T007)
Task: "Add Power routing data row in tests/Calculator.Tests/MainViewModelTests.cs"       (T008)
Task: "Add Power to src/Calculator.App/ViewModels/CalculatorOperation.cs"                (T011)
```

## Implementation Strategy

### MVP First (User Story 1 only)

1. Phase 1 and Phase 2
2. Phase 3 (US1)
3. Stop and validate with the in-process backend; it shows correct results for valid inputs

### Incremental Delivery

1. Foundation, then US1 (MVP), then US2 (errors), then US3 (SQL parity)
2. Finish with Polish (README, full test run, quickstart)

## Notes

- Commit with Conventional Commits and the scopes in `AGENTS.md` (`service`, `app`, `database`, `tests`, `readme`); one logical change per commit; agents commit only when asked
- The new SQL script is numbered `003_` to continue the existing `001_`/`002_` database script sequence; this is unrelated to the `0003-` feature number
