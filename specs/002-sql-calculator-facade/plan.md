# Implementation Plan: Database-Backed Calculator Facade

**Branch**: `002-sql-calculator-facade` | **Date**: 2026-11-19 | **Spec**: [spec.md](spec.md)

**Input**: Feature specification from `/specs/002-sql-calculator-facade/spec.md`

## Summary

Add `SqlCalculatorFacade`, a second `ICalculatorFacade` implementation in `Calculator.Service` that runs each operation through its own stored procedure on the local SQL Server (default instance, Windows login) using the in-box `System.Data.SqlClient`. Ship idempotent T-SQL scripts (`database/`) that a developer runs manually. The app picks the backend at startup from `App.config` (default: in-process). Database problems map to a new `CalculationError.Unavailable` failure; results are converted so they match `CalculatorFacade` exactly.

## Technical Context

**Language/Version**: C# 7.3 on .NET Framework 4.8

**Primary Dependencies**: `System.Data.SqlClient` and `System.Configuration` (framework assemblies, no NuGet packages)

**Storage**: Local SQL Server 2025 (default instance, integrated security); database `CalculatorDb` with 4 stored procedures; no tables

**Testing**: MSTest; unit tests through an internal `IProcedureExecutor` seam (no DB), plus integration tests that report Inconclusive when the DB is unreachable

**Target Platform**: Windows desktop (WPF)

**Project Type**: desktop app + class library

**Performance Goals**: typical call < 1 s; hard 5 s command timeout

**Constraints**: stateless and thread-safe (new pooled connection per call); parameterized calls only; no credentials in the repo; the app never changes the schema; no silent fallback

**Scale/Scope**: 4 operations, one new facade, one SQL script set, two config entries

## Constitution Check

| Principle | Status |
|-----------|--------|
| I. .NET 4.8 / C# 7.3 | Pass: same targets, no C# 8 features |
| II. MVVM | Pass: no ViewModel or View changes; backend chosen in the `App.xaml.cs` composition root |
| III. Simplicity | Pass: no third-party packages; one small seam interface justified by testability |
| IV. Testability | Pass: DB access behind `IProcedureExecutor`; unit tests need no database |
| V. Style | Pass: one class per file |

Post-design re-check: still passes. No complexity violations.

## Project Structure

### Documentation (this feature)

```text
specs/002-sql-calculator-facade/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   ├── stored-procedures.md
│   └── configuration.md
└── tasks.md             # created by /speckit-tasks
```

### Source Code (repository root)

```text
database/
├── 001_create_database.sql        # creates CalculatorDb if missing
└── 002_create_procedures.sql      # CREATE OR ALTER the 4 procedures

src/Calculator.Service/
├── CalculationError.cs            # + Unavailable
├── SqlCalculatorFacade.cs
└── Internal/
    ├── IProcedureExecutor.cs
    ├── ProcedureOutcome.cs
    └── SqlProcedureExecutor.cs

src/Calculator.App/
├── App.config                     # backend + connection string
└── App.xaml.cs                    # chooses the facade at startup

tests/Calculator.Tests/
├── SqlCalculatorFacadeTests.cs            # fake executor
└── SqlCalculatorFacadeIntegrationTests.cs # real local DB
```

**Structure Decision**: extend the existing two-project layout; SQL scripts live in a new top-level `database/` folder.

## Complexity Tracking

No violations.
