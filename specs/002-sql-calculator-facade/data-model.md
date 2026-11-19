# Data Model: Database-Backed Calculator Facade

No tables are stored; the database only hosts procedures.

## Calculation request (internal)
- **Operation**: Add | Subtract | Multiply | Divide
- **Left / Right**: decimal operands (divide: dividend / divisor)

## ProcedureOutcome (internal, `Internal/ProcedureOutcome.cs`)
- **Status**: Ok | DivisionByZero | Overflow
- **Value**: decimal, meaningful only when Status is Ok
- Infrastructure problems are not outcomes; the executor throws and the facade converts them.

## CalculationResult (existing, unchanged)
- Success(value) or Failure(error, message).

## CalculationError (existing, extended)
- `None`, `DivisionByZero`, `Overflow`, **`Unavailable`** (new: database unreachable, timeout, procedure missing).

## Mapping

| Procedure status | Result |
|------------------|--------|
| 0 | Success(value) |
| 1 | Failure(DivisionByZero, "Cannot divide by zero") |
| 2, or value not convertible to `decimal` | Failure(Overflow, "The result is too large to calculate") |
| SQL/connection error or timeout | Failure(Unavailable, "The calculation service is unavailable") |

Messages for DivisionByZero and Overflow match `CalculatorFacade`.
