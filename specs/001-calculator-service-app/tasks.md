---

description: "Task list for Calculator Service and Desktop App"
---

# Tasks: Calculator Service and Desktop App

**Input**: Design documents from `/specs/001-calculator-service-app/`

**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/ICalculatorFacade.md

**Tests**: Included. The constitution (Principle IV) requires MSTest coverage for ViewModels and services.

**Organization**: Tasks are grouped by user story. All code MUST target .NET Framework 4.8 with C# 7.3 (no C# 8+ features), one class per file, no business logic in code-behind.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (US1, US2, US3)

## Path Conventions

Paths follow plan.md: `src/Calculator.Service/`, `src/Calculator.App/`, `tests/Calculator.Tests/`, solution `WpfPlayground.sln` at the repository root.

---

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Solution and project scaffolding

- [ ] T001 Create `WpfPlayground.sln` at the repository root and the folders `src/` and `tests/`
- [ ] T002 [P] Create `src/Calculator.Service/Calculator.Service.csproj` (SDK-style, `net48` class library, `LangVersion` 7.3, no WPF references)
- [ ] T003 [P] Create `src/Calculator.App/Calculator.App.csproj` (SDK-style, `net48`, `UseWPF` true, `OutputType` WinExe, `LangVersion` 7.3, project reference to Calculator.Service)
- [ ] T004 [P] Create `tests/Calculator.Tests/Calculator.Tests.csproj` (SDK-style, `net48`, `LangVersion` 7.3, MSTest.TestFramework and MSTest.TestAdapter package references, project references to Calculator.Service and Calculator.App)
- [ ] T005 Add all three projects to `WpfPlayground.sln` and verify `msbuild WpfPlayground.sln /restore` succeeds

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Shared types and MVVM infrastructure required by every user story

**CRITICAL**: No user story work can begin until this phase is complete

- [ ] T006 [P] Create `CalculationError` enum (`None`, `DivisionByZero`, `Overflow`) in `src/Calculator.Service/CalculationError.cs` per data-model.md `CalculationError`
- [ ] T007 Create immutable `CalculationResult` with `Success(decimal)` and `Failure(CalculationError, string)` factories in `src/Calculator.Service/CalculationResult.cs`, citing data-model.md `CalculationResult.IsSuccess`, `CalculationResult.Value`, `CalculationResult.Error`, `CalculationResult.ErrorMessage` (depends on T006)
- [ ] T008 Create public `ICalculatorFacade` (Add, Subtract, Multiply, Divide) in `src/Calculator.Service/ICalculatorFacade.cs` exactly as in contracts/ICalculatorFacade.md (depends on T007)
- [ ] T009 [P] Create `ViewModelBase` implementing `INotifyPropertyChanged` with a `SetProperty` helper in `src/Calculator.App/ViewModels/ViewModelBase.cs`
- [ ] T010 [P] Create `RelayCommand` (`ICommand` with `Action<object>` execute and optional `Func<object, bool>` canExecute) in `src/Calculator.App/ViewModels/RelayCommand.cs`
- [ ] T011 [P] Create `CalculatorOperation` enum (`Add`, `Subtract`, `Multiply`, `Divide`) in `src/Calculator.App/ViewModels/CalculatorOperation.cs`
- [ ] T012 [P] Create the shared styles resource dictionary in `src/Calculator.App/Resources/Styles.xaml`
- [ ] T013 Create `App.xaml` (merging `Resources/Styles.xaml`, `StartupUri` MainWindow.xaml) and `App.xaml.cs` in `src/Calculator.App/`
- [ ] T014 [P] Create `FakeCalculatorFacade` (configurable result, records calls) in `tests/Calculator.Tests/FakeCalculatorFacade.cs`

**Checkpoint**: Foundation ready; user story work can begin

---

## Phase 3: User Story 1 - Perform basic arithmetic in the calculator (Priority: P1) MVP

**Goal**: A user enters two numbers, picks an operation and sees the correct result computed through the service facade.

**Independent Test**: Launch the app, enter 6 and 3, choose each operation, and confirm 9, 3, 18, 2; run the US1 tests.

### Tests for User Story 1

> Write these tests first and confirm they fail before implementation.

- [ ] T015 [P] [US1] Facade tests for happy-path add, subtract, multiply, divide (positive, negative, decimal operands) in `tests/Calculator.Tests/CalculatorFacadeTests.cs`
- [ ] T016 [P] [US1] `ResultFormatter` tests: 6 ÷ 3 gives "2", 1 ÷ 3 gives "0.3333333333", trailing zeros dropped, at most 10 decimals (FR-011) in `tests/Calculator.Tests/ResultFormatterTests.cs`
- [ ] T017 [P] [US1] `MainViewModel` tests using `FakeCalculatorFacade`: each operation calls the matching facade method and sets `ResultText` (data-model.md `MainViewModel.ResultText`, `MainViewModel.ErrorText`); a second calculation replaces the first result in `tests/Calculator.Tests/MainViewModelTests.cs`

### Implementation for User Story 1

- [ ] T018 [P] [US1] Create `internal static class Arithmetic` with add, subtract, multiply, divide on `decimal` in `src/Calculator.Service/Internal/Arithmetic.cs`
- [ ] T019 [US1] Create public `CalculatorFacade : ICalculatorFacade` returning `CalculationResult.Success` for the four operations in `src/Calculator.Service/CalculatorFacade.cs` (depends on T008, T018)
- [ ] T020 [P] [US1] Create `ResultFormatter` using `Math.Round(value, 10, MidpointRounding.AwayFromZero)` and trailing-zero removal in `src/Calculator.App/Formatting/ResultFormatter.cs` (FR-011)
- [ ] T021 [US1] Create `MainViewModel` with `FirstInput`, `SecondInput`, `ResultText`, `ErrorText`, a `CalculateCommand` taking a `CalculatorOperation`, a constructor taking `ICalculatorFacade`, and parsing of decimal input, in `src/Calculator.App/ViewModels/MainViewModel.cs` (depends on T009, T010, T011, T020)
- [ ] T022 [US1] Create `MainWindow.xaml` with two input boxes, four operation buttons bound to `CalculateCommand`, and a result display, using styles from `Resources/Styles.xaml`, in `src/Calculator.App/MainWindow.xaml` (depends on T012, T021)
- [ ] T023 [US1] Create `MainWindow.xaml.cs` with only `InitializeComponent`, and set `DataContext` to `new MainViewModel(new CalculatorFacade())` in `src/Calculator.App/App.xaml.cs` startup (depends on T019, T021, T022)

**Checkpoint**: User Story 1 is fully functional and testable on its own

---

## Phase 4: User Story 2 - Clear feedback for invalid input and division by zero (Priority: P2)

**Goal**: Invalid input, division by zero and overflow show a clear message, and the app stays usable.

**Independent Test**: Divide 5 by 0 and enter `abc`; a clear message shows, no result appears, and a following valid calculation works.

### Tests for User Story 2

- [ ] T024 [P] [US2] Facade tests: `Divide(1, 0)` and `Divide(0, 0)` return `DivisionByZero`; `Multiply(decimal.MaxValue, 2)` and `Add(decimal.MaxValue, 1)` return `Overflow`; no exceptions thrown, in `tests/Calculator.Tests/CalculatorFacadeTests.cs`
- [ ] T025 [P] [US2] `MainViewModel` tests: empty or non-numeric input sets `ErrorText` and does not call the fake facade; facade failure sets `ErrorText` and clears `ResultText`; a later valid calculation clears the error, in `tests/Calculator.Tests/MainViewModelTests.cs`

### Implementation for User Story 2

- [ ] T026 [US2] Update `Divide` in `src/Calculator.Service/CalculatorFacade.cs` to return `CalculationResult.Failure(CalculationError.DivisionByZero, ...)` when the divisor is zero, and wrap add, subtract, multiply, divide in `OverflowException` handling returning `CalculationError.Overflow`
- [ ] T027 [US2] Update `MainViewModel.CalculateCommand` in `src/Calculator.App/ViewModels/MainViewModel.cs` to clear previous result and error first, show "Enter a valid number" on parse failure without calling the facade, and map failed results to `ErrorText` (data-model.md `MainViewModel.ErrorText`, `MainViewModel.ResultText`)
- [ ] T028 [US2] Add an error message display bound to `ErrorText` (styled via `Resources/Styles.xaml`) in `src/Calculator.App/MainWindow.xaml`

**Checkpoint**: User Stories 1 and 2 both work independently

---

## Phase 5: User Story 3 - Calculation logic reusable independently of the app (Priority: P3)

**Goal**: The service is usable and verifiable with no desktop app, and the app performs all calculations through the facade only.

**Independent Test**: Run the facade tests with the app not running; verify the service project has no WPF or app references.

### Tests for User Story 3

- [ ] T029 [P] [US3] Add a test asserting `Calculator.Service` references no WPF assemblies (`PresentationFramework`, `PresentationCore`, `WindowsBase`) and no reference to `Calculator.App`, in `tests/Calculator.Tests/CalculatorFacadeTests.cs`
- [ ] T030 [US3] Add a test asserting `Arithmetic` is not public and `ICalculatorFacade` and `CalculatorFacade` are the only public calculation types in `Calculator.Service`, in `tests/Calculator.Tests/CalculatorFacadeTests.cs`

### Implementation for User Story 3

- [ ] T031 [US3] Verify `src/Calculator.Service/Calculator.Service.csproj` has no WPF or `Calculator.App` references, and that `src/Calculator.App/ViewModels/MainViewModel.cs` contains no arithmetic operators on operands (calls only `ICalculatorFacade`); fix any violation
- [ ] T032 [US3] Make every facade method in `src/Calculator.Service/CalculatorFacade.cs` stateless and thread-safe as specified in contracts/ICalculatorFacade.md, and add XML documentation comments to `src/Calculator.Service/ICalculatorFacade.cs`

**Checkpoint**: All user stories are independently functional

---

## Phase 6: Polish & Cross-Cutting Concerns

- [ ] T033 [P] Confirm all projects declare `LangVersion` 7.3 and contain no C# 8+ syntax across `src/` and `tests/`
- [ ] T034 [P] Confirm one class per file and tidy XAML (no duplicated inline styles) in `src/Calculator.App/`
- [ ] T035 Run `msbuild WpfPlayground.sln /restore` and all MSTest tests; all must pass
- [ ] T036 Run the manual scenarios in `specs/001-calculator-service-app/quickstart.md`

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies
- **Foundational (Phase 2)**: Depends on Setup and blocks all user stories
- **User Story 1 (Phase 3)**: Depends on Foundational
- **User Story 2 (Phase 4)**: Depends on Foundational; edits files created in US1 (`CalculatorFacade.cs`, `MainViewModel.cs`, `MainWindow.xaml`), so run after US1
- **User Story 3 (Phase 5)**: Depends on US1 (needs the facade implementation); US2 recommended first
- **Polish (Phase 6)**: Depends on all stories

### Within Each User Story

- Tests are written first and must fail before implementation
- Service types before ViewModel, ViewModel before XAML wiring

### Parallel Opportunities

- T002, T003, T004 together; T006, T009, T010, T011, T012, T014 together
- US1 tests T015, T016, T017 together; T018 and T020 together
- US2 tests T024, T025 together

### Parallel Example: User Story 1

```text
Task: "Facade tests in tests/Calculator.Tests/CalculatorFacadeTests.cs"
Task: "ResultFormatter tests in tests/Calculator.Tests/ResultFormatterTests.cs"
Task: "MainViewModel tests in tests/Calculator.Tests/MainViewModelTests.cs"
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1 (Setup) and Phase 2 (Foundational)
2. Complete Phase 3 (User Story 1)
3. Stop and validate with the US1 tests and the manual arithmetic scenario

### Incremental Delivery

1. Setup and Foundational give a building solution
2. Add US1 for a working calculator (MVP)
3. Add US2 for robust error handling
4. Add US3 to lock in service independence
5. Finish with Polish

---

## Notes

- [P] tasks touch different files and have no unfinished dependencies
- Commit after each task or logical group
- Tasks touching the same file (`CalculatorFacadeTests.cs`, `MainViewModelTests.cs`, `CalculatorFacade.cs`, `MainViewModel.cs`, `MainWindow.xaml`) must not run in parallel with each other
