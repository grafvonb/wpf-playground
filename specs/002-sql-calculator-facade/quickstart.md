# Quickstart: Validating the Database-Backed Facade

## Prerequisites
- Local SQL Server default instance running (verified: service `MSSQLSERVER`, SQL Server 2025).
- Your Windows account can create databases on it.

## 1. Prepare the database (once)
```powershell
sqlcmd -S . -E -b -i database\001_create_database.sql
sqlcmd -S . -E -b -i database\002_create_procedures.sql
```
Expected: no errors. Re-running is safe.

## 2. Spot-check a procedure
```powershell
sqlcmd -S . -E -d CalculatorDb -Q "DECLARE @r decimal(38,18), @c int; EXEC @c = dbo.Calc_Add 2.5, 3.5, @r OUTPUT; SELECT @c AS code, @r AS result"
```
Expected: code `0`, result `6`. Divide by zero returns code `1`.

## 3. Run tests
```powershell
dotnet test tests\Calculator.Tests\Calculator.Tests.csproj
```
Expected: all pass; the integration tests run against the database (Inconclusive if it is unreachable).

## 4. Run the app on the database backend
Set `CalculatorBackend` to `Sql` in `src\Calculator.App\App.config`, then:
```powershell
dotnet run --project src\Calculator.App
```
Expected: calculations work as before. Stop the SQL Server service and calculate: a clear "unavailable" message appears within about 5 seconds and the app stays running.

## 5. Switch back
Set `CalculatorBackend` to `InProcess`; no database connection is attempted.

See [contracts/stored-procedures.md](contracts/stored-procedures.md) and [contracts/configuration.md](contracts/configuration.md).
