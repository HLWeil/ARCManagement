# AGENTS.md

Guidance for AI coding agents working in this repository. Adapted from the
applicable parts of [DataHubClient's AGENTS.md](https://github.com/nfdi4plants/DataHubClient/blob/main/AGENTS.md).

## What this repo is

ARCManagement is a library for managing ARCs (Annotated Research Contexts),
written once in F# and compiled for .NET or transpiled to TypeScript and Python
with Fable. Packages are distributed through NuGet (`ARCManagement`), npm
(`arcmanagement`), and PyPI (`arc-management`, imported as `arc_management`).

Read [README.md](README.md) for the project layout, setup, and build targets.
Read [CONTRIBUTING.md](CONTRIBUTING.md) for the contribution workflow. Keep
dependencies small.

## Architecture

- One source tree, `src/ARCManagement`, contains the shared library. Its three
  project files select the .NET, JavaScript/TypeScript, and Python variants.
- All three variants import `ARCManagement.Common.props`. Add shared `.fs`
  files to its compile list in dependency order; F# compilation is order-sensitive.
- Keep platform-specific implementation details behind the appropriate
  `FABLE_COMPILER_*` directives. Shared code must remain usable on all targets.
- Public npm exports live in `src/ARCManagement/index.ts`; public Python
  exports and async adapters live in `src/ARCManagement/__init__.py`.
  Update these facades when adding public APIs.
- Shared F# tests live in `tests/ARCManagement`, with reusable helpers in
  `tests/TestingUtils`. Native package tests live in `tests/JavaScript` and
  `tests/Python`; `tests/WebBundling` checks browser bundling.
- The executable FAKE project under `build/` orchestrates compilation,
  transpilation, testing, packaging, and releases.

## F# code conventions

- Use classes for public models intended for native JavaScript and Python
  consumers. F# record update syntax is unavailable to those callers.
- Put `[<AttachMembers>]` on public classes so consumers can call instance
  methods directly.
- Use `ResizeArray<'T>` for public collections intended to behave as native
  arrays. Keep F# lists and sequences internal and convert at the public boundary.
- Avoid exposing F#-specific types such as options, results, and discriminated
  unions where a simple value or class provides a usable native API.
- Use the existing `CrossAsync` abstraction for shared asynchronous operations
  and keep the `Async` suffix on those functions and methods. Preserve the
  Promise-based npm API and awaitable Python facade; avoid public `Task<'T>` APIs.
- Do not call runtime-specific .NET APIs from shared code without a Fable
  implementation. Use the existing file helpers and compiler-specific branches
  as examples.
- Add XML documentation comments to new public types, members, functions, and
  constructor parameters. Describe behavior and parameters, not just signatures.
- After a substantial API addition, transpile and inspect the generated code and
  declarations. Check attached members, property names, collection types, and
  exported names. Fix the F# source or handwritten facades rather than generated
  TypeScript, Python, or declaration files.

## Build and test

Use the FAKE pipeline through `build.cmd` on Windows or `bash ./build.sh` on
Linux/macOS. Target definitions live in the corresponding task modules under
`build/` and are registered by `build/Build.fs`.

```text
bash ./build.sh                                 # Default: build the solution
bash ./build.sh RunTests --fail-on-focused-tests # All shared/native suites and browser checks
bash ./build.sh RunTestsDotnet                   # Shared F# tests on .NET
bash ./build.sh RunTestsJS                       # Shared F# tests transpiled to TypeScript
bash ./build.sh RunTestsPy                       # Shared F# tests transpiled to Python
bash ./build.sh RunTestsJSNative                 # Native Vitest tests of built npm output
bash ./build.sh RunTestsPyNative                 # Native pytest tests of the installed wheel
bash ./build.sh TranspileTS                      # Generate library TypeScript
bash ./build.sh TranspilePy                      # Generate library Python
bash ./build.sh Pack                             # Build all registry artifacts
```

- On Windows, replace `bash ./build.sh` with `build.cmd`, keeping the arguments.
- For quick .NET iteration, use `dotnet build ARCManagement.sln` or
  `dotnet run --project tests/ARCManagement/ARCManagement.Tests.fsproj`.
- Pipeline targets run cleanup as a prerequisite. Inspect generated output
  before starting another invocation that removes it.
- Use the repository's tool versions and restore its local Fable tool with
  `dotnet tool restore`. Follow the CI workflow for reproducible environment setup.
- Python commands require the repository's `.venv`. Virtual environments are
  OS-specific; recreate the environment for the current OS when switching
  platforms rather than reusing another OS's environment.

## Testing conventions

- Use `Fable.Pyxpecto` for shared F# suites, with a single
  `open Fable.Pyxpecto`. Do not add Expecto, Fable.Mocha, or VSTest adapters for
  these suites.
- Run shared .NET tests through `dotnet run`, not `dotnet test`: Pyxpecto uses an
  executable entry point rather than a test SDK adapter.
- Follow the existing `Main.fs` entrypoint: .NET/Python use Pyxpecto directly;
  TypeScript uses the Vitest adapter, awaits completion, and checks the exit code.
- Keep Vitest for native JavaScript/TypeScript tests and pytest for native Python
  tests. Exercise public exports so tests catch missing facade members.
- Add regression tests for bug fixes and behavior tests for new functionality.
  Put reusable test helpers in `tests/TestingUtils`.
- `ftestCase` focuses a Pyxpecto test; `ptestCase` marks it pending. Do not leave
  focused tests in completed changes. Use `--fail-on-focused-tests` for validation.
- For code changes, run the affected targets and the full shared/native suite
  when changing behavior across targets. Report what ran and any checks that
  could not run. Documentation-only changes do not require the full test pipeline.

## Useful pointers

- [README and customization checklist](README.md)
- [Contribution guide](CONTRIBUTING.md)
- [Build target registration](build/Build.fs)
- [Test orchestration](build/TestTasks.fs)
- [Fable documentation](https://fable.io/docs/)
- [Fable.Pyxpecto](https://github.com/Freymaurer/Fable.Pyxpecto)
