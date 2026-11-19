# Implementation Plan: Raise to Power Operation

**Branch**: `0003-raise-to-power` | **Date**: 2026-11-19 | **Spec**: [spec.md](spec.md)

**Input**: Feature specification from `/specs/0003-raise-to-power/spec.md`

## Summary

Add a fifth calculator operation, `base ^ exponent`, to `ICalculatorFacade` and implement it in both
backends (`CalculatorFacade` in-process, `SqlCalculatorFacade` via a new `dbo.Calc_Power` stored
procedure). Both backends compute fractional exponents in floating point and agree to 12 significant
digits. Three failure modes get dedicated errors and messages (zero to a negative power, not a real
number, too large). The WPF UI gets one more operation button wired through the existing
`CalculateCommand`. No new projects or dependencies.

## Technical Context

**Language/Version**: C# 7.3 on .NET Framework 4.8 (`net48`)

**Primary Dependencies**: WPF, `System.Data.SqlClient`; no new dependencies

**Storage**: SQL Server `CalculatorDb`; one new procedure `dbo.Calc_Power` in a new numbered script

**Testing**: MSTest on `net48`; fakes (`FakeProcedureExecutor`, `FakeCalculatorFacade`); SQL integration
tests report inconclusive when the database is unreachable and honor `CALCULATOR_TEST_CONNECTION`

**Target Platform**: Windows desktop

**Project Type**: desktop-app (`Calculator.App`, `Calculator.Service`, `Calculator.Tests`)

**Performance Goals**: Result appears instantly, like the existing operations (database timeout stays 5 s)

**Constraints**: 12-significant-digit parity between backends; operands for the SQL backend keep the
existing limits (below 1,000,000,000, at most 9 decimal places); results in SQL are `DECIMAL(38,18)`

**Scale/Scope**: One operation, about 6 source files, 1 SQL script, README update

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Principle | Status | Notes |
|-----------|--------|-------|
| I. MVVM separation | Pass | The view model calls `ICalculatorFacade.Power`; math stays in `Calculator.Service`; XAML only adds a button |
| II. Facade with interchangeable backends | Pass | `Power` is added to the interface and both backends; no silent fallback |
| III. Test-first with MSTest | Pass | Tests in `tests\Calculator.Tests` cover both backends via fakes plus the integration parity test |
| IV. Database scripts as source of truth | Pass | New numbered re-runnable script `database\003_create_power_procedure.sql`; divergences documented in `README.md` |
| V. Simplicity / C# 7.3 | Pass | No new project, abstraction or dependency; no C# 8+ syntax |
| Platform: `net48` | Pass | No target change |

No violations. Complexity Tracking is not needed. Re-checked after Phase 1 design: still passing.

## Project Structure

### Documentation (this feature)

```text
specs/0003-raise-to-power/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── calculator-power-contract.md
├── checklists/
│   └── requirements.md
└── tasks.md             # Created later by /speckit-tasks
```

### Source Code (repository root)

```text
src/
├── Calculator.App/
│   ├── MainWindow.xaml                      # add the power button
│   └── ViewModels/
│       ├── CalculatorOperation.cs           # add Power
│       └── MainViewModel.cs                 # route Power to the facade
└── Calculator.Service/
    ├── ICalculatorFacade.cs                 # add Power
    ├── CalculationError.cs                  # add ZeroToNegativePower, NotRealNumber
    ├── CalculatorFacade.cs                  # in-process Power
    ├── SqlCalculatorFacade.cs               # SQL Power and status mapping
    └── Internal/
        ├── Arithmetic.cs                    # Power math
        ├── ProcedureStatus.cs               # add ZeroToNegativePower, NotRealNumber
        └── SqlProcedureExecutor.cs          # map new return codes
database/
└── 003_create_power_procedure.sql           # dbo.Calc_Power
tests/Calculator.Tests/
├── CalculatorFacadeTests.cs                 # in-process cases
├── SqlCalculatorFacadeTests.cs              # SQL facade with fake executor
├── SqlCalculatorFacadeIntegrationTests.cs   # parity cases
├── MainViewModelTests.cs                    # Power command and error display
└── FakeCalculatorFacade.cs                  # add Power
README.md                                    # operation list and divergences
```

**Structure Decision**: Extend the existing three-project solution in place; no new projects.

## Complexity Tracking

No constitution violations; nothing to justify.
