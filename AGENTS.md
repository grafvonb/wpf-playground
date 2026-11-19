# Agent Instructions

## Commit messages

Always use [Conventional Commits](https://www.conventionalcommits.org/) with a proper scope:

```text
<type>(<scope>): <short imperative summary>
```

- **Types**: `feat`, `fix`, `docs`, `style`, `refactor`, `perf`, `test`, `build`, `ci`, `chore`.
- **Scope**: the area changed, for example `calculator`, `service`, `app`, `tests`, `specs`, `build`.
  Never omit the scope.
- Keep the summary lowercase, imperative, without a trailing period.
- Use `!` or a `BREAKING CHANGE:` footer for breaking changes.
