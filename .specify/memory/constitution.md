<!--
Sync Impact Report
Version change: 1.0.0 → 1.1.0 (MUST target .NET Framework 4.8 added to platform constraints)
Modified principles: none
Added sections: Core Principles I-V, Technology & Platform Constraints, Development Workflow & Quality Gates, Governance
Removed sections: none
Deferred items: none
-->
# WPF Playground Constitution

## Core Principles

### I. MVVM Separation (NON-NEGOTIABLE)
`Calculator.App` MUST follow MVVM: views (XAML) contain no business logic, view models hold
presentation state and commands, and calculation logic lives only in `Calculator.Service`.
View models MUST depend on `ICalculatorFacade`, never on a concrete implementation.
Rationale: keeps the UI testable without a window and the backend swappable.

### II. Service Facade with Interchangeable Backends
All calculation behavior MUST be exposed through `ICalculatorFacade`. Each backend
(`CalculatorFacade` in-process, `SqlCalculatorFacade` over SQL Server stored procedures) MUST
satisfy the same contract. The backend is selected at startup from `App.config`; if the selected
backend is unavailable the app MUST report an error and MUST NOT silently fall back to another.
Rationale: predictable, observable behavior and a single contract to test.

### III. Test-First with MSTest
New behavior MUST have MSTest tests in `tests\Calculator.Tests`, written before or with the
implementation. Backends MUST be testable via fakes (e.g. `FakeCalculatorFacade`,
`FakeProcedureExecutor`). Database integration tests MUST report inconclusive, not fail, when the
database is unreachable, and MUST honor `CALCULATOR_TEST_CONNECTION`.
Rationale: the SQL backend is not always available, yet regressions must be caught.

### IV. Database Scripts as Source of Truth
Schema and stored procedures MUST be defined only in numbered, re-runnable scripts under
`database\` (`Calc_*` procedure naming). Changes MUST be made by editing or adding scripts, never
by ad-hoc changes to a live database. Known divergences between backends (e.g. 18-decimal
precision, operand limits) MUST be documented in `README.md`.
Rationale: reproducible environments and honest documentation of parity limits.

### V. Simplicity
Prefer the simplest solution that meets the spec (YAGNI). New projects, abstractions, or
dependencies MUST be justified in the plan. Code MUST remain compatible with C# 7.3.
Rationale: this is a learning playground; clarity outranks cleverness.

## Technology & Platform Constraints

- All projects (app, service, tests) MUST target .NET Framework 4.8 (`net48`). Retargeting to
  another framework (.NET Core, .NET 5+, or an older .NET Framework) MUST NOT occur without a
  constitution amendment.
- Language C# 7.3, WPF for UI.
- Dependencies MUST be compatible with `net48`.
- Test stack: MSTest (TestFramework and TestAdapter) on `net48`.
- Database: SQL Server (local instance, integrated security by default); connection strings MUST
  live in configuration, never hard-coded in code paths.
- Secrets MUST NOT be committed to the repository.

## Development Workflow & Quality Gates

- Features follow Spec Kit: specification, plan, tasks, then implementation under `specs\`.
- Before a change is merged, the solution MUST build and all tests MUST pass (SQL integration
  tests may be inconclusive only when the database is unavailable).
- User-visible or setup changes MUST update `README.md` in the same change.
- Reviews MUST verify compliance with the principles above; deviations MUST be justified in the
  plan's complexity notes.

## Governance

This constitution supersedes other practices. Amendments require a documented change to this
file, a version bump, and review of dependent specs/plans for conflicts. Versioning follows
semantic versioning: MAJOR for removed or redefined principles, MINOR for added principles or
materially expanded guidance, PATCH for clarifications. Compliance MUST be checked during plan
creation and code review. Runtime guidance lives in `README.md`.

**Version**: 1.1.0 | **Ratified**: 2026-11-19 | **Last Amended**: 2026-11-19
