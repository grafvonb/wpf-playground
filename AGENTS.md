# AGENTS.md

Rules for AI agents and contributors working in this repository. The project constitution at
`.specify\memory\constitution.md` takes precedence over this file.

## Commit Messages

Commits MUST follow [Conventional Commits](https://www.conventionalcommits.org/):

```
<type>(<scope>): <description>

[optional body]

[optional footer(s)]
```

### Types

`feat`, `fix`, `docs`, `style`, `refactor`, `perf`, `test`, `build`, `ci`, `chore`, `revert`.

### Scopes

A scope is REQUIRED and MUST name the area changed:

| Scope          | Area                                                      |
|----------------|-----------------------------------------------------------|
| `app`          | `src\Calculator.App` (WPF UI, view models)                |
| `service`      | `src\Calculator.Service` (facades, calculation logic)     |
| `database`     | `database\` SQL scripts and stored procedures             |
| `tests`        | `tests\Calculator.Tests`                                  |
| `specs`        | `specs\` Spec Kit specs, plans and tasks                  |
| `constitution` | `.specify\memory\constitution.md`                         |
| `agents`       | `AGENTS.md` and other agent guidance                      |
| `readme`       | `README.md`                                               |
| `repo`         | Repository-wide config (`.gitignore`, solution, tooling)  |

- Use the single most relevant scope; if a change spans several areas, prefer the primary one
  or split it into separate commits.
- Add a new scope only when no listed scope fits, and add it to this table.

### Rules

- Description: imperative mood, lowercase start, no trailing period, 72 characters or fewer.
- Body (optional): explain what and why, wrapped at about 72 characters.
- Breaking changes MUST use `!` after the scope (e.g. `feat(service)!: ...`) and a
  `BREAKING CHANGE:` footer.
- One logical change per commit; do not mix unrelated changes.
- The solution MUST build and tests MUST pass before committing code changes.
- Do not commit secrets, build output (`bin\`, `obj\`) or `TestResults\`.
- Agents MUST NOT commit, push or rewrite history unless the user explicitly asks.
- When an agent creates a commit, it MUST end the message with this trailer:

  ```
  Co-authored-by: Copilot App <223556219+Copilot@users.noreply.github.com>
  ```

### Examples

```
feat(service): add sql calculator facade using stored procedures
fix(app): show error when calculation service is unavailable
docs(readme): document CALCULATOR_TEST_CONNECTION override
test(service): cover division by zero in in-process facade
chore(repo): add gitignore for Visual Studio and build output
```
