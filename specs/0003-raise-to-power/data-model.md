# Data Model: Raise to Power Operation

No persistent data is added. The feature extends existing in-memory types and one stored procedure.

## Power operation

| Field | Type | Notes |
|-------|------|-------|
| Base | decimal | First input. SQL backend: below 1,000,000,000 in magnitude, at most 9 decimal places |
| Exponent | decimal | Second input. Same limits on the SQL backend |
| Result | `CalculationResult` | Success with a decimal value, or failure with error kind and message |

## Validation rules (evaluated in this order)

1. Base = 0 and exponent < 0: failure `ZeroToNegativePower`, "Cannot raise zero to a negative power".
2. Base < 0 and exponent is not a whole number: failure `NotRealNumber`, "Result is not a real number".
3. Computed value is NaN, infinite, or outside the representable range: failure `Overflow`,
   "Result is too large".
4. Otherwise success. Exponent 0 gives 1 for any base, including 0 (0 ^ 0 = 1).

## Type changes

- `CalculationError`: add `ZeroToNegativePower`, `NotRealNumber`.
- `ProcedureStatus` (internal): add `ZeroToNegativePower`, `NotRealNumber`.
- `CalculatorOperation` (app): add `Power`.

## Stored procedure return codes

| Code | Meaning |
|------|---------|
| 0 | Success |
| 1 | Division by zero (Calc_Divide only) |
| 2 | Overflow |
| 3 | Zero to a negative power (Calc_Power only) |
| 4 | Not a real number (Calc_Power only) |
