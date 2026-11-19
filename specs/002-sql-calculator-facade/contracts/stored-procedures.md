# Contract: Stored Procedures

Database `CalculatorDb`, schema `dbo`. Scripts in `database/`; all are re-runnable.

| Procedure | Inputs | Output |
|-----------|--------|--------|
| `Calc_Add` | `@Left`, `@Right` decimal | `@Result` decimal OUTPUT |
| `Calc_Subtract` | `@Left`, `@Right` decimal | `@Result` decimal OUTPUT |
| `Calc_Multiply` | `@Left`, `@Right` decimal | `@Result` decimal OUTPUT |
| `Calc_Divide` | `@Dividend`, `@Divisor` decimal | `@Result` decimal OUTPUT |

**Return code**: `0` success, `1` division by zero (Calc_Divide only), `2` overflow.

**Rules**
- Never raise an error for expected failures; return the code and leave `@Result` NULL.
- Results must equal `CalculatorFacade` for the same inputs, or return `2` (see research.md, item 2).
- The calling account needs only EXECUTE permission on the four procedures.
- Inputs are always bound as typed parameters; never concatenated.
