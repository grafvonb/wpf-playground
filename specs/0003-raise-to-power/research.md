# Research: Raise to Power Operation

## Decision 1: Compute in floating point, convert back to decimal

- **Decision**: In-process, compute `Math.Pow((double)base, (double)exponent)`, then convert to `decimal`.
  In SQL, use `POWER(CAST(@Left AS FLOAT), CAST(@Right AS FLOAT))` and cast to `DECIMAL(38,18)`.
- **Rationale**: .NET has no decimal power for fractional exponents, and T-SQL `POWER` only works on
  float for them. Both engines use IEEE double math, so results agree to well over the required 12
  significant digits (clarification: equal when they agree to 12 significant digits).
- **Alternatives considered**: Exact decimal exponentiation by repeated multiplication for integer
  exponents (rejected: does not cover fractional or negative exponents, and two code paths would
  diverge); a decimal `Exp/Ln` implementation (rejected: complexity, violates Simplicity).

## Decision 2: Detect invalid inputs before computing

- **Decision**: Check inputs first and report: base 0 with exponent < 0 gives `ZeroToNegativePower`;
  base < 0 with non-integer exponent gives `NotRealNumber`. After computing, `NaN`, infinity, or a
  value outside `decimal` range (in-process) or `DECIMAL(38,18)` (SQL) gives `Overflow`.
- **Rationale**: Explicit checks give the exact messages required by FR-005 and do not depend on
  engine-specific exceptions. Matches the existing `Divide` pattern (facade pre-check, procedure also
  guards).
- **Alternatives considered**: Rely on `double.NaN` checks only (rejected: cannot tell zero-to-negative
  from non-real); rely on SQL error 3623 domain errors (rejected: engine-specific).

## Decision 3: Error model and messages

- **Decision**: Add `CalculationError.ZeroToNegativePower` and `CalculationError.NotRealNumber`; reuse
  `CalculationError.Overflow`. Messages for power: "Cannot raise zero to a negative power",
  "Result is not a real number", "Result is too large". The existing arithmetic overflow message
  ("The result is too large to calculate") is unchanged.
- **Rationale**: Spec FR-005 requires specific messages; a distinct power overflow message avoids
  changing existing behavior for other operations.
- **Alternatives considered**: One generic error (rejected by clarification).

## Decision 4: SQL return codes

- **Decision**: `Calc_Power` returns 0 success, 2 overflow (existing code), 3 zero to negative power,
  4 not a real number. `SqlProcedureExecutor` maps 3 and 4 to new `ProcedureStatus` values. Overflow
  is caught from the float/decimal conversion errors (8115 arithmetic overflow, 232 float overflow)
  and returned as 2.
- **Rationale**: Extends the documented return-code convention without breaking codes 0 to 2.
- **Open item (verify in implementation)**: confirm which SQL error numbers `POWER` raises for
  overflow on the target SQL Server; the integration test for a huge result covers this.

## Decision 5: Known backend divergences (documented in README)

- **Decision**: Document that results are compared at 12 significant digits, that SQL results are
  limited to `DECIMAL(38,18)` (magnitude below 10^20) while in-process results are limited to the
  `decimal` range (about 7.9 x 10^28), and that SQL operands keep the existing limits. Tests avoid
  inputs whose outcome differs because of these limits, or assert each backend's documented outcome.
- **Rationale**: Constitution principle IV requires documenting parity limits honestly.
- **Alternatives considered**: Make the in-process backend also fail above 10^20 (rejected: changes
  in-process behavior to mimic a storage limit).

## Decision 6: UI placement

- **Decision**: Add a fifth `OperationButtonStyle` button labeled `xʸ` (tooltip "Raise to power") in the
  existing operation row, bound to `CalculateCommand` with `CalculatorOperation.Power`. Inputs keep
  their labels ("First number" = base, "Second number" = exponent).
- **Rationale**: Consistent with the other operations; no layout or flow change. Verify the fifth
  button fits the 360 px window.
- **Alternatives considered**: A separate exponent input or an operation selector (rejected: breaks
  consistency with the existing buttons).
