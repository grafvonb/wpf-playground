# Quickstart: Validate Raise to Power

## Prerequisites

- Windows with the .NET Framework 4.8 developer tools and MSBuild or Visual Studio
- Optional: local SQL Server with `database\001_create_database.sql`, `002_create_procedures.sql`
  and `003_create_power_procedure.sql` applied (set `CALCULATOR_TEST_CONNECTION` to use another server)

## Automated validation

1. Build the solution and run the MSTest suite, for example `dotnet test` or Visual Studio Test Explorer.
2. Expect all power tests to pass. SQL integration tests are inconclusive, not failing, when the
   database is unreachable.

## Manual validation (in-process backend, then SQL backend via `App.config`)

| First number | Second number | Press | Expected |
|--------------|---------------|-------|----------|
| 2 | 10 | `xʸ` | Result 1024 |
| 2 | -1 | `xʸ` | Result 0.5 |
| 9 | 0.5 | `xʸ` | Result 3 |
| 5 | 0 | `xʸ` | Result 1 |
| 0 | -1 | `xʸ` | Error "Cannot raise zero to a negative power" |
| -8 | 0.5 | `xʸ` | Error "Result is not a real number" |
| 10 | 1000 | `xʸ` | Error "Result is too large" |

After each error, the next calculation must work normally. Results and errors must be the same on both
backends (numbers equal to 12 significant digits).

See [contracts/calculator-power-contract.md](contracts/calculator-power-contract.md) and
[data-model.md](data-model.md) for details.
