## Projects

- **`HaydCalculator.Core`** — the calculation logic (`HaydCalculatorService`, flow data / hayd
  cycle models). Plain `net10.0`; only NodaTime plus the logging and DI abstractions are allowed.
  `AddCore()` (`DependencyInjection/CoreServiceCollectionExtensions.cs`) is its composition root.
- **`HaydCalculator.MAUI`** — the app and the real composition root (`MauiProgram`, which calls
  `AddCore()`). Pages live in `Presentation/Pages/<Name>/` (`<Name>Page.cs` + `<Name>PageViewModel.cs`),
  reusable views in `Presentation/Views/`. UI is written in C# with
  `CommunityToolkit.Maui.Markup`, no XAML pages. Logging goes through `ILogger` (MetroLog file
  logger in the cache folder + debug output); `[Time]` (MethodTimer.Fody) logs method durations.
- **`tests/HaydCalculator.Core.Tests.Unit`** — xunit + AwesomeAssertions + NSubstitute tests for
  Core. Shared test settings (`IsTestProject`, test SDK, xunit runner) live in
  `tests/Directory.Build.props`. `Architecture/ArchitectureTests.cs` (NetArchTest) fails when
  Core picks up a dependency on MAUI or another UI library.

## Calculation model

- Time is NodaTime: flows are `Instant` ranges (`From`/`To`), lengths are `Duration`. Hayd rules
  count real elapsed time (15 days = 360 hours), which `Instant`/`Duration` give exactly, without
  floating-point day counts or daylight saving shifts. The app maps calendar days to midnight UTC.
- `HaydCalculatorService` is stateless (singleton); each `Calculate` call creates its own
  `HaydCalculation`, which holds all intermediate state of that one run.
- `InfoException` = expected outcome shown to the user (invalid input, case not supported yet).
  `HaydCalculationStateException` = the calculation reached a state its own rules forbid, i.e. a
  bug. Never throw a bare `Exception`.

## Building and testing

`global.json` pins the SDK feature band (any `10.0.4xx` patch) and the exact MAUI workload set
version. Install an SDK from that band, then run `dotnet workload update` once in the repo root.
Without it every project fails with `MSB4242 ... Workload version ... was not found`.

Core and tests need no MAUI workload:

```
dotnet build tests/HaydCalculator.Core.Tests.Unit/HaydCalculator.Core.Tests.Unit.csproj
dotnet test tests/HaydCalculator.Core.Tests.Unit/HaydCalculator.Core.Tests.Unit.csproj
```

The MAUI project needs the `maui-android` workload (and the Android SDK platform for the
targeted API level). In environments without it (e.g. Claude Code cloud sessions),
`dotnet build HaydCalculatorProject.slnx` fails with `NETSDK1147`; that is expected and says
nothing about the code. Do not try to install the workload there. To still get the analyzers
over the MAUI code on a Windows machine, build a single TFM:

```
dotnet build HaydCalculator.MAUI/HaydCalculator.MAUI.csproj -f net10.0-windows10.0.26100.0
```

GitHub Actions (`.github/workflows/ci.yml`) runs the tests on every push/PR and builds the
Android APK; pushes to `main` refresh the rolling `latest` prerelease.

Three Tamyiz tests are skipped (`TamyizNotFinished`) because that part of the calculation is
not finished; remove the `Skip` once it is.

## Code style

`.editorconfig` follows Microsoft's published C# coding and naming conventions (same file as
in PrayerTimeEngine), and `EnforceCodeStyleInBuild` turns every violation into a build
**error**. Key naming rules: PascalCase for types, members, constants (also local constants)
and local functions; `_camelCase` for private/internal instance fields; `s_camelCase` for
private/internal static fields; `I`/`T` prefixes for interfaces/type parameters; camelCase for
locals and parameters. `var` only when the type is apparent from the right-hand side.
File-scoped namespaces.

Most violations are auto-fixable: `dotnet format <project>.csproj --severity error`.
Run it per project. Do not run it on the multi-targeted MAUI project as a whole (formatting
several TFMs at once can write merge-conflict markers into shared files). Naming (IDE1006) is
not auto-fixed; rename by hand. Watch out: `dotnet format` also *deletes* unused private
members (IDE0051); check the diff before committing.

`global.json` is the single source of truth for toolchain versions: the .NET SDK
(`sdk.version` + `rollForward: latestPatch`) and the MAUI workload set (`sdk.workloadVersion`).
`latestPatch` pins the SDK *feature band*: any newer patch of `10.0.4xx` is accepted, so a
Visual Studio update that replaces one patch with the next does not break the build. New
analyzer rules arrive with a new feature band, which requires a deliberate change here.
GitHub Actions reads both values, so local and CI builds run the same analyzers and workloads.

**Keep `sdk.workloadVersion` in step with Visual Studio** (and with PrayerTimeEngine, which
uses the same pin). After a VS update, local CLI builds of the MAUI project can fail with
`NETSDK1147` or `MSB4242`. Then find the workload set that matches VS's packs
(`dotnet workload search version 10.0.40x`; compare with `C:\Program Files\dotnet\packs\Microsoft.Maui.Sdk`
and `Microsoft.Android.Sdk.Windows`), raise `sdk.workloadVersion` to it and run
`dotnet workload update` in an admin shell. Remind the user of this whenever a VS update is
mentioned or one of these errors shows up.

## Git

The working copy is owned by a different Windows account than the one Claude runs under, so
plain `git` fails with `detected dubious ownership`. Pass the exception per command instead of
changing the global config: `git -c safe.directory=C:/Users/talip/source/repos/HaydCalculatorProject ...`.

## Pull requests created by Claude

PRs opened by a Claude session must be recognizable as such at a glance:

- **Branch name** starts with `claude/`.
- **Label** `claude` is applied to the PR.
- **First line of the PR body** is this banner, before any heading:

  ```
  > 🤖 **Dieser Pull Request wurde von Claude Code erstellt.**
  ```

- **End of the PR body** keeps the generated-with footer and the session link.

Same for the PR title: describe the change normally, no marker there — the banner,
the label and the branch prefix carry that.
