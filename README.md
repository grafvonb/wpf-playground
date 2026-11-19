# wpf-playground

WPF calculator (.NET Framework 4.8) with a swappable calculation backend.

## SQL Server backend

By default the app calculates in-process. To use the stored procedures on the local SQL Server instead:

1. Create the database and procedures (run once, and again after script changes):

   ```powershell
   sqlcmd -S . -E -b -i database\001_create_database.sql
   sqlcmd -S . -E -b -i database\002_create_procedures.sql
   ```

2. In `src\Calculator.App\App.config` set `CalculatorBackend` to `Sql`. The `CalculatorDb` connection string
   defaults to `Server=.;Database=CalculatorDb;Integrated Security=true;Connect Timeout=5`.

If the database is unreachable the app shows "The calculation service is unavailable" and does not fall back to
in-process. Integration tests are reported as inconclusive when the database is unavailable; set
`CALCULATOR_TEST_CONNECTION` to override the connection string.

Known limitation: the database keeps 18 decimal places, so results such as `2/3` can differ from the in-process
result beyond the 18th decimal place. Operands must have at most 9 decimal places and be below 1,000,000,000.
