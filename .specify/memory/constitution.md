<!--
Sync Impact Report
Version change: (unratified template) -> 1.0.0
Modified principles: none (all principles newly defined from template placeholders)
Added sections: Core Principles I-V, Technology & Build Constraints, Development Workflow, Governance
Removed sections: none
Deferred TODOs: none
-->
# WPF Playground Constitution

## Core Principles

### I. Target Platform: .NET Framework 4.8 and C# 7.3 Only
All projects MUST target .NET Framework 4.8 and set `LangVersion` to `7.3`. C# 8.0+ features
MUST NOT be used, including records, nullable reference types, switch expressions, and default
interface members. Rationale: the app is a deliberately constrained WPF learning project, and
the language level must be predictable and compile identically everywhere.

### II. MVVM Separation (NON-NEGOTIABLE)
Views MUST bind to ViewModels through data binding, `INotifyPropertyChanged`, and `ICommand`.
Code-behind MUST NOT contain business logic; it is limited to view-only concerns. ViewModels
MUST NOT reference View types or WPF controls. Rationale: separation keeps logic testable and
the UI replaceable.

### III. Simplicity First
Third-party frameworks and libraries MUST NOT be added unless the justification is recorded in
the feature plan. A small hand-written `ViewModelBase` and `RelayCommand` MUST be preferred over
MVVM frameworks. Abstractions MUST be introduced only when a concrete need exists (YAGNI).
Rationale: fewer dependencies keep the project easy to learn and maintain.

### IV. Testability
ViewModels and services MUST be unit-testable without launching any UI, and MUST be covered by
MSTest tests. Dependencies that touch the file system, dialogs, or time MUST be accessed through
interfaces so they can be substituted in tests. Rationale: logic outside the UI thread and
visual tree can be verified quickly and reliably.

### V. Code and XAML Style
Each class MUST live in its own file, named after the class. Names MUST be meaningful and
consistent. XAML MUST stay tidy: shared styles, brushes, and templates MUST be defined in
resource dictionaries rather than duplicated inline. Rationale: consistent structure reduces
navigation cost and review noise.

## Technology & Build Constraints

- The application MUST build with Visual Studio 2026 or MSBuild on Windows.
- The build MUST succeed with no errors from a clean checkout using the documented command.
- The UI framework is WPF; no other UI technology may be mixed in without a recorded
  justification in the plan.

## Development Workflow

- Specs, plans, and tasks follow the Spec Kit flow (specify, plan, tasks, implement).
- Every plan MUST include a Constitution Check against the principles above.
- Changes MUST keep the build green and all MSTest tests passing before being considered done.
- Reviews MUST verify the C# 7.3 constraint and the absence of business logic in code-behind.

## Governance

This constitution supersedes other practices. Amendments MUST be documented in this file with a
version bump, a rationale, and an update of dependent plans where affected. Versioning follows
semantic versioning: MAJOR for removed or redefined principles, MINOR for added or materially
expanded guidance, PATCH for clarifications. Compliance MUST be checked in every plan and
review; any violation MUST be explicitly justified in the plan's complexity tracking.

**Version**: 1.0.0 | **Ratified**: 2026-11-19 | **Last Amended**: 2026-11-19
