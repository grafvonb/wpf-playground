# Research: Database-Backed Calculator Facade

## 1. Data access library
- **Decision**: `System.Data.SqlClient` (in-box with .NET Framework 4.8).
- **Rationale**: no NuGet dependency (constitution III); fully adequate for stored procedure calls with integrated security against SQL Server 2025.
- **Alternatives**: `Microsoft.Data.SqlClient` (extra package, no needed features); EF/Dapper (overkill).

## 2. Numeric parity with .NET `decimal` (FR-004, FR-005)
- **Decision**: pass operands as `SqlDbType.Decimal` parameters built from the exact `decimal` value (precision 28, scale 0-28 as needed) and read the result as an output parameter converted back to `decimal`; a failed conversion is reported as `Overflow`.
- **Known risk**: T-SQL decimal arithmetic adjusts result scale for multiplication and division, so results may differ from .NET in the last digits. The parity suite (a reference table of inputs and expected `CalculatorFacade` outputs, including extreme values) defines the final parameter types. Where SQL cannot reproduce the .NET value, the procedure must signal Overflow instead of returning a different value. Revisit the spec if exact parity proves infeasible.
- **Alternatives**: `FLOAT` (rejected: inexact); strings (does not fix SQL scale rules).

## 3. Procedure contract
- **Decision**: four procedures `dbo.Calc_Add`, `Calc_Subtract`, `Calc_Multiply`, `Calc_Divide`, each with two input decimals and an output decimal, returning a status code: `0` ok, `1` division by zero, `2` overflow. A TRY/CATCH inside maps SQL arithmetic overflow (error 8115) to `2`.
- **Rationale**: expected failures are codes, not exceptions, and are easy to tell apart from real faults.
- **Alternatives**: result sets (extra parsing); raised errors (harder to classify).

## 4. Failure mapping and timeout (FR-006)
- **Decision**: `SqlException`, connection `InvalidOperationException` and timeouts become `CalculationError.Unavailable` with a readable message; `CommandTimeout = 5` and `Connect Timeout=5`. A missing procedure (SQL error 2812) also maps to `Unavailable`. No exception escapes the facade. No fallback to in-process.
- **Alternatives**: retry or silent fallback (rejected by clarification).

## 5. Thread safety (FR-007)
- **Decision**: a new pooled connection per call and no mutable state in the facade.

## 6. Backend selection and configuration (FR-008, FR-010)
- **Decision**: `App.config` `appSettings` key `CalculatorBackend` (`InProcess` default, or `Sql`) and a connection string named `CalculatorDb` (`Server=.;Database=CalculatorDb;Integrated Security=true;Connect Timeout=5`). The choice is made in `App.xaml.cs`. An unrecognized value shows a startup error instead of silently choosing a backend. No credentials are stored (integrated security).
- **Alternatives**: runtime UI toggle (rejected in clarification).

## 7. Database setup (FR-003)
- **Decision**: idempotent scripts run manually with `sqlcmd -S . -E -b -i <file>`. The app never alters the schema.

## 8. Testing without a database (FR-011)
- **Decision**: internal `IProcedureExecutor` seam (tests already have `InternalsVisibleTo`); integration tests mark themselves Inconclusive when the DB is unreachable.

## Parity result (T013)

Add, subtract and multiply match the in-process facade exactly for the tested pairs. Division is exact only to 18 decimal places (T-SQL DECIMAL(38,18) result vs. .NET 28 significant digits); the integration test allows a 1e-18 difference. Operands beyond DECIMAL(18,9) are reported as Overflow.
