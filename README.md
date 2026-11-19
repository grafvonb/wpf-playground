# WPF Calculator with a SQL Server Stored Procedures Service

Example app showing a WPF application with an MVVM calculator UI and a service layer that can be implemented
either in-process or with SQL Server stored procedures.

## Projects

- `src\Calculator.App` – WPF UI (MVVM). Selects the calculation backend at startup from `App.config`.
- `src\Calculator.Service` – `ICalculatorFacade` with two implementations: `CalculatorFacade` (in-process) and
  `SqlCalculatorFacade` (SQL Server stored procedures).
- `database` – SQL scripts that create `CalculatorDb` and the `Calc_*` stored procedures.
- `tests\Calculator.Tests` – MSTest unit and integration tests.
- `specs` – Spec Kit feature specifications, plans and tasks.

The calculator supports add, subtract, multiply, divide and raise to power (`xʸ`).

Built with .NET Framework 4.8 and C# 7.3.

## SQL Server backend

By default the app calculates in-process. To use the stored procedures on the local SQL Server instead:

1. Create the database and procedures (run once, and again after script changes):

   ```powershell
   sqlcmd -S . -E -b -i database\001_create_database.sql
   sqlcmd -S . -E -b -i database\002_create_procedures.sql
   sqlcmd -S . -E -b -i database\003_create_power_procedure.sql
   ```

2. In `src\Calculator.App\App.config` set `CalculatorBackend` to `Sql`. The `CalculatorDb` connection string
   defaults to `Server=.;Database=CalculatorDb;Integrated Security=true;Connect Timeout=5`.

If the database is unreachable the app shows "The calculation service is unavailable" and does not fall back to
in-process. Integration tests are reported as inconclusive when the database is unavailable; set
`CALCULATOR_TEST_CONNECTION` to override the connection string.

Known limitation: the database keeps 18 decimal places, so results such as `2/3` can differ from the in-process
result beyond the 18th decimal place. Operands must have at most 9 decimal places and be below 1,000,000,000.

Raise to power: both backends return the same results and error messages. Results are considered equal when they
agree to 12 significant digits (the database computes with floating point). Results are limited to below 10^20 in
the database and to the `decimal` range (about 7.9 x 10^28) in-process; beyond its limit each backend reports
"Result is too large". Operands keep the limits above on the SQL backend.
