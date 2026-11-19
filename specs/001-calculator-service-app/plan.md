# Implementation Plan: Calculator Service and Desktop App

**Branch**: `001-calculator-service-app` | **Date**: 2026-11-19 | **Spec**: [spec.md](spec.md)

**Input**: Feature specification from `/specs/001-calculator-service-app/spec.md`

## Summary

Deliver a solution with two separate C# projects plus a test project: a calculation service library
exposed only through a facade (add, subtract, multiply, divide) and a WPF desktop calculator that
performs every calculation through that facade. Division by zero and invalid input are reported as
explicit error outcomes (no exceptions for expected failures). Results are displayed rounded to at
most 10 decimal places with trailing zeros removed (FR-011). The app follows MVVM with a
hand-written `ViewModelBase` and `RelayCommand`.

## Technical Context

**Language/Version**: C# 7.3 (`LangVersion` 7.3), .NET Framework 4.8

**Primary Dependencies**: None at runtime (WPF is part of the framework). Test-only: MSTest v2
(`MSTest.TestFramework`, `MSTest.TestAdapter`).

**Storage**: N/A (no persistence between sessions)

**Testing**: MSTest; service and ViewModel tests run without any UI

**Target Platform**: Windows desktop, built with Visual Studio 2026 or MSBuild

**Project Type**: desktop-app plus class library (solution with three projects)

**Performance Goals**: Result shown within 1 second of choosing an operation (SC-005); trivially met
by in-process calls

**Constraints**: No C# 8+ features; no business logic in code-behind; no third-party frameworks;
`decimal` arithmetic for operands and results

**Scale/Scope**: One window, four operations, three projects, roughly 10 source files

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Principle | Status | How the plan complies |
|-----------|--------|-----------------------|
| I. .NET Framework 4.8 and C# 7.3 only | PASS | All projects target `net48` with `LangVersion` 7.3 |
| II. MVVM separation | PASS | Window binds to `MainViewModel`; code-behind is empty apart from `InitializeComponent` |
| III. Simplicity first | PASS | No third-party runtime dependencies; hand-written `ViewModelBase` and `RelayCommand` |
| IV. Testability | PASS | `MainViewModel` depends on `ICalculatorFacade`; tests substitute it with a hand-written fake; MSTest only |
| V. Code and XAML style | PASS | One class per file; shared styles in `Styles.xaml` resource dictionary |
| Build with VS 2026 / MSBuild | PASS | SDK-style projects with `net48` build through MSBuild and Visual Studio |

Post-design re-check (after Phase 1): PASS, no violations. Complexity Tracking is not required.

## Project Structure

### Documentation (this feature)

```text
specs/001-calculator-service-app/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── ICalculatorFacade.md
└── tasks.md             # Created later by /speckit-tasks
```

### Source Code (repository root)

```text
WpfPlayground.sln
src/
├── Calculator.Service/
│   ├── Calculator.Service.csproj
│   ├── ICalculatorFacade.cs
│   ├── CalculatorFacade.cs
│   ├── CalculationResult.cs
│   ├── CalculationError.cs
│   └── Internal/
│       └── Arithmetic.cs
└── Calculator.App/
    ├── Calculator.App.csproj
    ├── App.xaml
    ├── App.xaml.cs
    ├── MainWindow.xaml
    ├── MainWindow.xaml.cs
    ├── Resources/
    │   └── Styles.xaml
    ├── ViewModels/
    │   ├── ViewModelBase.cs
    │   ├── RelayCommand.cs
    │   ├── CalculatorOperation.cs
    │   └── MainViewModel.cs
    └── Formatting/
        └── ResultFormatter.cs
tests/
└── Calculator.Tests/
    ├── Calculator.Tests.csproj
    ├── CalculatorFacadeTests.cs
    ├── MainViewModelTests.cs
    ├── ResultFormatterTests.cs
    └── FakeCalculatorFacade.cs
```

**Structure Decision**: Three projects under `src/` and `tests/`. `Calculator.Service` has no
reference to WPF or the app. `Calculator.App` references only the service's public facade and result
types (the `Internal` namespace is `internal`). `Calculator.Tests` references both. This satisfies
FR-001, FR-003, FR-005 and FR-006.

## Complexity Tracking

No constitution violations; nothing to justify.
