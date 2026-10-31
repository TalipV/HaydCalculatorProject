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
- **No `DateTime` / `DateTimeOffset` anywhere.** `BannedSymbols.txt` bans them, together with every
  direct read of the system clock (`DateTime.Now`, `SystemClock.Instance`, ...), via the
  BannedApiAnalyzers (RS0030 = build error). The current time comes from an injected NodaTime
  `IClock` (registered once in `AddCore()`; tests use `FakeClock` from `NodaTime.Testing`).
  The only BCL dates left are at XCalendar's API in `MainPageViewModel.CreateCalendar` /
  `SetCalendarColors`, converted with `ToDateTimeUnspecified()` / `LocalDate.FromDateTime(...)`.
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

All text in the repository is English: code, comments, test descriptions, docs, CI output. No German.

`.editorconfig` follows Microsoft's published C# coding and naming conventions (same file as
in PrayerTimeEngine), and `EnforceCodeStyleInBuild` turns every violation into a build
**error**. Key naming rules: PascalCase for types, members, constants (also local constants)
and local functions; `_camelCase` for private/internal instance fields; `s_camelCase` for
private/internal static fields; `I`/`T` prefixes for interfaces/type parameters; camelCase for
locals and parameters. `var` only when the type is apparent from the right-hand side.
File-scoped namespaces.

Line endings are CRLF in the working tree (`.gitattributes`: `* text=auto eol=crlf`, `.editorconfig`:
`end_of_line = crlf`); the repository itself stores LF. Tools that write LF (scripts, generated
files) leave mixed line endings behind, which Visual Studio then complains about. After such
edits, check with `git ls-files --eol` that every file shows `w/crlf`.

XML doc comments: `<summary>` and `</summary>` always stand on their own lines, also for a
one-line text (no `/// <summary>text</summary>`).

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
  > 🤖 **This pull request was created by Claude Code.**
  ```

- **End of the PR body** keeps the generated-with footer and the session link.

Same for the PR title: describe the change normally, no marker there — the banner,
the label and the branch prefix carry that.

## Fiqh reference

`docs/fiqh/` is the reference for the fiqh behind the calculator: rules with sources and Arabic
quotations, Shamela page offsets, every khilaf, open questions, and which tests cover what.
**Before any fiqh statement, test or calculator change, read `docs/fiqh/README.md` and the file
for the topic**, instead of relying on the session context. Add every new finding there (new
source, user decision, khilaf, resolved or new open question).

## Fiqh tests

Tests that check calculator results against the Shafi'i mu'tamad follow these rules:

- **Allowed sources:** the user's global rules for Shafi'i fiqh sources apply (user-level CLAUDE.md and
  section 4 of the skill `post-validation-shafii-madhhab`): only the later Shafi'is (Tuhfat
  al-Muhtaj, Nihayat al-Muhtaj, Mughni al-Muhtaj, their hawashi, the other hawashi of the later
  Shafi'is such as al-Bajuri, al-Bujayrimi, ash-Sharqawi, al-Kurdi, at-Tarmasi, I'anat
  at-Talibin, Ibn Hajar's al-Fatawa al-Kubra) plus the user's own sources (the lecture series
  "Rulings of Haydh and Nifas in the Shafi'i school", al-Ibana wa-l-Ifada); never a matn such as
  al-Minhaj as evidence; no early or comparative works; at least 2, ideally 4 sources from different
  authors per point.
- Every expected result has a source comment above the test:
  `// [Author, Work, vol. X, p. Y; https://shamela.ws/book/<book>/<page>] short English summary`.
  Prefer sources that state the case with explicit numbers; a source that states the rule
  clearly but without the numbers of the test is marked `[IMPLICIT]`.
- **Every known khilaf is stated** in a `// KHILAF: ...` line: the other view(s), who holds
  them, where, and which view the test follows. This includes differences between Ibn Hajar
  (Tuhfat al-Muhtaj) and ar-Ramli / al-Khatib (Nihayat al-Muhtaj, Mughni al-Muhtaj) and weaker
  views inside the madhhab (asahh vs. other, azhar vs. other). Where the mu'tamad works agree,
  say so ("no khilaf here").
- Points of khilaf are selectable through `HaydCalculatorSettings` (Core, `Calculator/Models`): one
  bool or enum per point, defaults = asahh / azhar and ar-Ramli / al-Khatib where the mu'tamad
  authorities differ, presets `IbnHajar`, `Ramli`, `Shirbini`. Only mu'tamad authorities and views
  that are at least strong inside the school go in there. Tests for another view pass the settings
  (`HaydCalculatorSettings.Default with { ... }` or a preset) and live in the region
  "Other views (khilaf)". A new khilaf point gets a setting, a KHILAF line and a test per view.
- Every calculator test carries the attribute `[FiqhTodo]` (`tests/.../Util/FiqhTodoAttribute.cs`)
  until the user has checked its expected result. It sets the xunit trait `Fiqh = ToDo`, so the
  Visual Studio Test Explorer can group and filter by it (Group by: Traits, or search
  `Trait:"ToDo"`), and `dotnet test --filter "Fiqh=ToDo"` runs only these tests. Add it to every
  new test; only the user removes it.
- A wrong result stays a red test (TDD); never change the calculator or the test to make it
  pass unless asked. Cases the sources do not settle go into
  `HaydCalculatorServiceTests_OpenQuestions` and throw `OpenFiqhQuestionException`.
