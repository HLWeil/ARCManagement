# Contributing to ARCManagement



## Contribution workflow

- Open an issue to discuss your idea before starting to work on it.
- Fork the repository and create a new branch for your feature or bug fix.
- Make your changes and [write tests](#test-driven-development) for them.
- `build.cmd runtests` (Windows) or `bash build.sh runtests` (Linux/macOS) to run the tests and ensure everything is working correctly.
- Commit your changes with a clear message and push them to your fork.
- Open a pull request to the main repository and describe your changes.

## Project structure

- `src/` - contains the source code for the project.
- `tests/` - contains the test code for the project.
- `build/` - contains build scripts and configuration files.

## Local Installation

#### Requirements

- [Node.js 24 and npm](https://nodejs.org/en/download)
    - Verify with `node --version` (24.x, matching CI).
    - Verify with `npm --version`; use the npm version bundled with Node.js 24.
- [.NET SDK 10.0.402](https://dotnet.microsoft.com/en-us/download)
    - Verify with `dotnet --version` from the repository root; the SDK is selected by `global.json`.
    - Fable 5.23.0 is pinned in `.config/dotnet-tools.json` and installed by `dotnet tool restore`.
- [Python 3.12 or newer](https://www.python.org/downloads/)
    - Verify with `python --version` or `py --version` on Windows. CI uses Python 3.12.
- [uv 0.13.5](https://docs.astral.sh/uv/)
    - Verify with `uv --version`; ensure uv is available on `PATH`.

##### Windows

1. Setup dotnet tools

   `dotnet tool restore`


2. Install NPM dependencies

    `npm install`

3. Setup python environment

    `py -m venv .venv`

4. Install pip dependencies

   `uv pip install -r pyproject.toml --group dev`

Verify correct setup with `./build.cmd runtests` ✨

##### Linux / macOS

1. Setup dotnet tools

   `dotnet tool restore`


2. Install NPM dependencies

    `npm install`

3. Setup python environment

    `python -m venv .venv`

4. Install pip dependencies

   `uv pip install -r pyproject.toml --group dev`

Verify correct setup with `bash build.sh runtests` ✨

## Fable compatibility

This project is compatible with Fable, a tool that compiles F# to JavaScript and Python. This allows you to use the same codebase across different platforms and languages, but it also means that you need to be careful when writing code that is compatible with Fable. Please refer to the [Fable documentation](https://fable.io/docs/) for more information on how to write Fable-compatible code.

Core design decisions here: 
- All functionality must be written in the F# source code.
- Avoid using .NET APIs that are not available in Fable. This includes APIs that are specific to the .NET runtime, such as `System.IO` and `System.Net`. We offer some available helpers.
- Avoid referencing external libraries.
- Do not use `async` but our own `CrossAsync` implementation, which is compatible with Fable.
- Use the `[AttachMembers]` attribute on classes.
- If you want to expose a class in JavaScript or Python, reference them in [src/ARCManagement/index.ts](src/ARCManagement/index.ts) and [src/ARCManagement/__init__.py](src/ARCManagement/__init__.py) respectively.

## Test-driven Development

This project contains a comprehensive test suite that ensures the correctness and reliability of the codebase. We encourage contributors to follow a test-driven development (TDD) approach when adding new features or fixing bugs.

So, when fixing bugs, please first write a test that reproduces the bug and check that it fails. Only then, implement the fix and verify that the test passes. This ensures that the bug is properly fixed and prevents regressions in the future.

For new features, please write tests that cover the expected behavior of the feature. This helps to ensure that the feature works as intended and prevents future changes from breaking it.

If big test objects are needed, please check whether appropriate test objects are already available in the [tests/TestingUtils](tests/TestingUtils) folder. If not, please create them and add them to the folder, before referencing them in your tests.

You can run the tests using the following commands:
- `build.cmd runtests` (Windows) or `bash build.sh runtests` (Linux/macOS). This will run all the tests across all languages.
- `build.cmd runtestspy` (Windows) or `bash build.sh runtestspy` (Linux/macOS). This will transpile the F# code to Python and run the Python tests.
- `build.cmd runtestsjs` (Windows) or `bash build.sh runtestsjs` (Linux/macOS).  This will transpile the F# code to JavaScript and run the JavaScript tests.
- `build.cmd runtestsdotnet` (Windows) or `bash build.sh runtestsdotnet` (Linux/macOS). This will run the F# tests using the .NET runtime.

You can set tests to focused mode by renaming the testfunction from `testCase` to `ftestCase`. This will only run the focused tests.
Please remember to rename the testfunction back to `testCase` before committing your changes, so that all tests are run in the CI/CD pipeline.
