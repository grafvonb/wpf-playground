# Quickstart: Calculator Service and Desktop App

## Prerequisites

- Windows with Visual Studio 2026 (or MSBuild) and the .NET Framework 4.8 targeting pack.
- Solution `WpfPlayground.sln` containing `Calculator.Service`, `Calculator.App`, `Calculator.Tests`.

## Build

```powershell
msbuild WpfPlayground.sln /restore /p:Configuration=Debug
```

Expected: build succeeds with no errors; all projects report C# 7.3.

## Run the automated tests

```powershell
vstest.console.exe tests\Calculator.Tests\bin\Debug\net48\Calculator.Tests.dll
```

Expected: all tests pass. They cover the facade contract
([ICalculatorFacade](contracts/ICalculatorFacade.md)), the ViewModel behavior and the result
formatter ([data-model.md](data-model.md)) without any UI.

## Manual validation scenarios

Start `src\Calculator.App\bin\Debug\net48\Calculator.App.exe`.

| Scenario | Steps | Expected |
|----------|-------|----------|
| Basic arithmetic (US1) | Enter 6 and 3, choose each operation | 9, 3, 18, 2 |
| Decimals and negatives | Enter -1.5 and 2, choose multiply | -3 |
| Rounding (FR-011) | Enter 1 and 3, choose divide | 0.3333333333 |
| Division by zero (US2) | Enter 5 and 0, choose divide | Error message, no result, app still usable |
| Invalid input (US2) | Enter `abc` and 2, choose add | "Enter a valid number", no result |
| Recovery | After an error, enter valid numbers and calculate | Error clears, correct result shown |
| Service independence (US3) | Run the facade tests only | Pass with the app not running |
