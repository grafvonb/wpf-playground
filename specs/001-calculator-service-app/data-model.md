# Data Model: Calculator Service and Desktop App

## CalculationResult (service, public, immutable)

| Field | Type | Notes |
|-------|------|-------|
| `IsSuccess` | bool | True when a value was calculated |
| `Value` | decimal | Valid only when `IsSuccess` is true; otherwise 0 |
| `Error` | `CalculationError` | `None` when successful |
| `ErrorMessage` | string | User-readable text; empty when successful |

Created through static factories `Success(decimal)` and `Failure(CalculationError, string)`.

## CalculationError (service, public enum)

| Value | Meaning |
|-------|---------|
| `None` | No error |
| `DivisionByZero` | Divisor was zero (including 0 ÷ 0) |
| `Overflow` | Result is outside the representable range |

## CalculatorOperation (app, enum)

`Add`, `Subtract`, `Multiply`, `Divide`. Maps one-to-one to facade methods (FR-008).

## MainViewModel state (app)

| Property | Type | Rules |
|----------|------|-------|
| `FirstInput` | string | Raw text; parsed on calculate |
| `SecondInput` | string | Raw text; parsed on calculate |
| `ResultText` | string | Formatted result per FR-011; empty when an error is shown |
| `ErrorText` | string | Empty when the last calculation succeeded |

## Behavior and validation rules

- Empty or non-numeric input: set `ErrorText` ("Enter a valid number"), clear `ResultText`, do not
  call the facade (FR-009).
- Facade failure: set `ErrorText` to the result's `ErrorMessage`, clear `ResultText`.
- Success: set `ResultText` via `ResultFormatter`, clear `ErrorText`.
- Every calculate action first clears the previous result and error (FR-010).
- Negative and decimal operands are accepted (FR-007).

## State transitions

`Idle` -> (calculate) -> `ShowingResult` or `ShowingError`. Any later calculate action replaces the
current state; editing inputs does not change the displayed state until the next calculate.
