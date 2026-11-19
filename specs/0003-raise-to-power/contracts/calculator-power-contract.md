# Contract: Power operation

## `ICalculatorFacade`

```text
CalculationResult Power(decimal @base, decimal exponent)
```

Applies to both `CalculatorFacade` and `SqlCalculatorFacade`. Never throws for invalid input; reports
failure through `CalculationResult`. A SQL backend that is unreachable reports
`CalculationError.Unavailable`, as for the other operations.

| Input (base, exponent) | Result |
|------------------------|--------|
| 2, 3 | Success 8 |
| 2, 0 / 0, 0 / -5, 0 | Success 1 |
| 2, -1 | Success 0.5 |
| 9, 0.5 | Success 3 (to 12 significant digits) |
| -2, 3 | Success -8 |
| 0, -1 | Failure `ZeroToNegativePower`, "Cannot raise zero to a negative power" |
| -8, 0.5 | Failure `NotRealNumber`, "Result is not a real number" |
| 10, 1000 (result too large) | Failure `Overflow`, "Result is too large" |

## `dbo.Calc_Power`

```text
@Left   DECIMAL(18, 9)
@Right  DECIMAL(18, 9)
@Result DECIMAL(38, 18) OUTPUT
RETURN  0 ok | 2 overflow | 3 zero to negative power | 4 not a real number
```

Same parameter shape as the other `Calc_*` procedures so `SqlProcedureExecutor` can call it unchanged.
Script is re-runnable (`CREATE OR ALTER`).

## UI contract

One operation button labeled `xʸ` (tooltip "Raise to power") in the operation row. It runs
`CalculateCommand` with `CalculatorOperation.Power`. First number is the base, second number is the
exponent. Success shows the result; failure shows the message from `CalculationResult` in the error
area, as for the other operations.
