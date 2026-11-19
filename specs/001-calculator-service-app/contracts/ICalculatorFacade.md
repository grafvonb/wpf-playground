# Contract: ICalculatorFacade

Assembly: `Calculator.Service`. The only public entry point of the service (FR-003).

## Interface

```csharp
public interface ICalculatorFacade
{
    CalculationResult Add(decimal left, decimal right);
    CalculationResult Subtract(decimal left, decimal right);
    CalculationResult Multiply(decimal left, decimal right);
    CalculationResult Divide(decimal dividend, decimal divisor);
}
```

Implementation: `CalculatorFacade` (public, parameterless constructor). Result and error types are
defined in [data-model.md](../data-model.md).

## Behavior

| Call | Result |
|------|--------|
| `Add(6, 3)` | Success, 9 |
| `Subtract(6, 3)` | Success, 3 |
| `Multiply(6, 3)` | Success, 18 |
| `Divide(6, 3)` | Success, 2 |
| `Divide(1, 0)` and `Divide(0, 0)` | Failure, `DivisionByZero` |
| Any operation exceeding the `decimal` range | Failure, `Overflow` |

## Guarantees

- Methods never throw for the expected failures listed above.
- Methods are stateless and thread-safe.
- Results are exact `decimal` values; rounding for display is the caller's concern (FR-011 is
  handled by the app's `ResultFormatter`).
- The service has no dependency on WPF or on the app.
