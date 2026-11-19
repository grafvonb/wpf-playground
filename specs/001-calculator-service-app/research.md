# Research: Calculator Service and Desktop App

All Technical Context items were resolved from the user input and the constitution; no
NEEDS CLARIFICATION items remained.

## Project file style

- **Decision**: SDK-style `.csproj` files with `<TargetFramework>net48</TargetFramework>` and
  `<LangVersion>7.3</LangVersion>`; WPF app uses `<UseWPF>true</UseWPF>`.
- **Rationale**: Builds with Visual Studio 2026 and MSBuild, supports `PackageReference` for MSTest,
  and keeps project files short.
- **Alternatives considered**: Classic verbose `.csproj` (more boilerplate, `packages.config`);
  modern .NET WPF (violates the constitution's .NET Framework 4.8 requirement).

## Numeric type

- **Decision**: `decimal` for operands and results.
- **Rationale**: Exact base-10 results for typical calculator input (0.1 + 0.2 = 0.3), 28-29 digit
  precision, and a built-in `Round`, which supports FR-011.
- **Alternatives considered**: `double` (binary rounding artifacts shown to users); `BigInteger` or
  rational types (over-engineering).

## Error reporting

- **Decision**: The facade returns a `CalculationResult` carrying either a value or a
  `CalculationError` code with a message. Division by zero and arithmetic overflow are returned as
  errors, not thrown.
- **Rationale**: Expected failures are part of the contract (FR-004), easy to test, and the ViewModel
  needs no `try/catch` for normal flows.
- **Alternatives considered**: Throwing `DivideByZeroException` (relies on callers catching it);
  returning nullable values (loses the reason for failure).

## Facade shape

- **Decision**: Public `ICalculatorFacade` with `Add`, `Subtract`, `Multiply`, `Divide` and a public
  `CalculatorFacade` implementation. The arithmetic helper is `internal`. The app creates the facade
  in `App.xaml.cs` and injects it into `MainViewModel` by constructor.
- **Rationale**: One entry point (FR-003), replaceable in tests, and no DI container needed.
- **Alternatives considered**: Static facade class (hard to substitute in tests); DI container
  (third-party dependency, violates the simplicity principle).

## Input parsing and display

- **Decision**: The ViewModel exposes two text inputs and parses them with the current culture's
  decimal rules. Failed parsing yields a validation message and no call to the facade. A
  `ResultFormatter` rounds with `Math.Round(value, 10, MidpointRounding.AwayFromZero)` and formats
  without trailing zeros.
- **Rationale**: Implements FR-007, FR-009 and FR-011 outside the view so it is unit-testable.
- **Alternatives considered**: WPF `ValidationRule` (logic would live in the view layer); formatting
  inside the service (display is a UI concern).

## Overflow

- **Decision**: Multiplication and addition overflow are caught in the service and returned as an
  `Overflow` error (resolves the low-impact item deferred during clarification).
- **Rationale**: The app never crashes on extreme values (SC-003).
- **Alternatives considered**: Letting the exception propagate.

## Testing

- **Decision**: MSTest v2 with a hand-written `FakeCalculatorFacade`; no mocking library.
- **Rationale**: Matches the constitution (MSTest, no third-party frameworks beyond need).
- **Alternatives considered**: NUnit or xUnit (the constitution names MSTest); Moq (extra dependency
  not justified for one interface).
