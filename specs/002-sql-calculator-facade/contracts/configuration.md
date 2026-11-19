# Contract: Configuration and Public Surface

## `Calculator.App/App.config`

| Entry | Values | Default |
|-------|--------|---------|
| `appSettings` / `CalculatorBackend` | `InProcess` or `Sql` | `InProcess` (when missing) |
| `connectionStrings` / `CalculatorDb` | SQL Server connection string using integrated security | `Server=.;Database=CalculatorDb;Integrated Security=true;Connect Timeout=5` |

An unrecognized `CalculatorBackend` value stops startup with a clear error message. The connection string is read only when the backend is `Sql`; with `InProcess` no connection is attempted.

## Public API (`Calculator.Service`)

- `ICalculatorFacade`: unchanged.
- `CalculationError.Unavailable`: new enum member, appended after `Overflow`.
- `SqlCalculatorFacade : ICalculatorFacade`: public constructor taking the connection string. It is stateless, thread-safe and never throws for expected failures.
