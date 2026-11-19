# Feature Specification: Raise to Power Operation

**Feature Branch**: `0003-raise-to-power`

**Created**: 2026-11-19

**Status**: Draft

**Input**: User description: "feat: add 'raise to power' operation to the calculator (GitHub issue #3)"

## Clarifications

### Session 2026-11-19

- Q: How precise must a fractional-exponent result be, and must both backends match exactly? → A: Results are equal when they agree to 12 significant digits; integer exponents give exact results where representable.
- Q: What message should the user see when the power operation has no valid result? → A: Specific messages per failure: "Cannot raise zero to a negative power", "Result is not a real number", "Result is too large".

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Raise a number to a power (Priority: P1)

A calculator user enters a base and an exponent, selects the "raise to power" operation and gets the result of base ^ exponent, in the same way as with the existing operations.

**Why this priority**: This is the core value of the feature.

**Independent Test**: Enter base 2 and exponent 10, select the power operation, and verify the result shows 1024.

**Acceptance Scenarios**:

1. **Given** base 2 and exponent 3, **When** the user runs the power operation, **Then** the result is 8.
2. **Given** any base and exponent 0, **When** the user runs the power operation, **Then** the result is 1.
3. **Given** base 2 and exponent -1, **When** the user runs the power operation, **Then** the result is 0.5.
4. **Given** base 9 and exponent 0.5, **When** the user runs the power operation, **Then** the result is 3.

---

### User Story 2 - Clear errors for invalid results (Priority: P2)

When the power operation has no valid numeric result, the user sees a clear error message and the calculator keeps working.

**Why this priority**: Prevents crashes and confusing output on invalid input.

**Independent Test**: Enter base 0 and exponent -1, run the operation, and verify an error message is shown and the app remains usable.

**Acceptance Scenarios**:

1. **Given** base 0 and exponent -1, **When** the user runs the power operation, **Then** a clear error is shown instead of a result.
2. **Given** values whose result is too large to represent, **When** the user runs the operation, **Then** a clear overflow error is shown.
3. **Given** a combination with no real result (e.g. negative base with fractional exponent), **When** the user runs the operation, **Then** a clear error is shown.

---

### User Story 3 - Same result regardless of calculation backend (Priority: P2)

The power operation gives the same results and the same errors whether the calculator uses its in-process calculation or its database-backed calculation.

**Why this priority**: The calculator supports both backends and users must not see differing behavior.

**Independent Test**: Run the same set of power inputs through both backends and compare results and errors.

**Acceptance Scenarios**:

1. **Given** valid inputs, **When** run on either backend, **Then** both return the same result.
2. **Given** invalid inputs, **When** run on either backend, **Then** both report an error.

---

### Edge Cases

- Exponent 0 with base 0 (0 ^ 0): treated as 1.
- Negative base with integer exponent (e.g. -2 ^ 3 = -8) is valid.
- Negative base with fractional exponent has no real result and is an error.
- Zero base with negative exponent is an error (division by zero).
- Results too large (infinite) or not a number are errors.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: Users MUST be able to enter a base and an exponent and obtain base ^ exponent.
- **FR-002**: The operation MUST support integer, negative and fractional exponents. Results for integer exponents MUST be exact where representable; results for fractional exponents MUST be accurate to at least 12 significant digits.
- **FR-003**: Any base raised to exponent 0 MUST yield 1.
- **FR-004**: The operation MUST be available on both the in-process and database-backed calculation backends with identical outcomes; numeric results are considered identical when they agree to 12 significant digits.
- **FR-005**: Invalid results MUST display a clear error message without crashing the application, using these exact messages: "Cannot raise zero to a negative power" (zero base, negative exponent), "Result is not a real number" (e.g. negative base with fractional exponent), "Result is too large" (overflow). Both backends MUST show the same message for the same input.
- **FR-006**: The user interface MUST expose the operation consistently with the other operations.
- **FR-007**: Automated tests MUST cover normal cases, exponent 0, negative exponents and the invalid cases.
- **FR-008**: The README operation list MUST include the new operation if it enumerates operations.

### Key Entities

- **Power operation**: A calculation with two inputs (base, exponent) producing one numeric result or an error.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: A user can obtain a power result in the same number of steps as for any existing two-operand operation.
- **SC-002**: 100% of the defined valid test cases (integer, negative, fractional, zero exponent) return the expected result (to 12 significant digits) on both backends.
- **SC-003**: 100% of the defined invalid cases show a clear error message with no application crash.
- **SC-004**: Documentation lists the operation wherever existing operations are listed.

## Assumptions

- Existing operations are two-operand and the new operation follows the same input flow.
- 0 ^ 0 is defined as 1, following common convention.
- Numeric precision and display formatting follow the existing operations.
- Scope covers the calculator app, calculation service, database scripts and tests.
