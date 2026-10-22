# PolyglotTemplate

An example library written in F#, compiled for .NET, and transpiled with Fable to
TypeScript and Python. Adapt the shared sources once and publish NuGet, npm, and
PyPI packages from the same repository. The build and async patterns are adapted
from [ARCtrl](https://github.com/nfdi4plants/ARCtrl).

## Create your repository

Select **Use this template → Create a new repository** on GitHub, then clone your
new repository. To offer a customized version as a template, enable **Template
repository** in its GitHub repository settings. This repository contains the
project directly at its root.

All package names, descriptions, authors, and links below are examples. Customize
them before publishing. NuGet packages start at `0.0.1`:

| Registry | Package |
| --- | --- |
| NuGet | `PolyglotTemplate`, `PolyglotTemplate.Javascript`, `PolyglotTemplate.Python` |
| npm | `polyglottemplate` (ES modules) |
| PyPI | `polyglot_template`|

PyPI normalizes underscores and hyphens in distribution names. Python imports
still use underscores: `import polyglot_template`.

## Prerequisites and setup

- .NET SDK **10.0.402**, selected by `global.json`.
- Node.js **24**, including npm.
- Python **3.12 or newer**; CI uses 3.12.
- uv **0.13.5** on `PATH`.
- Git for release-note generation, tags, and releases.

Run these commands at the repository root:

```text
dotnet tool restore
npm install
uv pip install -r pyproject.toml --group dev
py -m venv .venv
```

On Windows use `build.cmd`; on Linux/macOS use `bash ./build.sh`. The launchers
also work when invoked from another directory and restore the local Fable tool.

```text
build.cmd RunTests --fail-on-focused-tests
bash ./build.sh RunTests --fail-on-focused-tests
```

GitHub Actions runs the same
test target on Windows and Ubuntu for pushes and pull requests to `main`.

## Project layout

```text
PolyglotTemplate/
├── PolyglotTemplate.sln                        # Solution containing library, test, and build projects
├── build.cmd                                  # Windows build launcher; restores local .NET tools
├── build.sh                                   # Bash build launcher; restores local .NET tools
├── global.json                                # Selects the .NET SDK version
├── Directory.Build.props                      # Shared F# settings and FSharp.Core reference
├── Directory.Packages.props                   # Central NuGet versions for library and test dependencies
├── LICENSE                                    # MIT license and attribution for copied code
├── .gitignore                                 # Excludes generated code, environments, caches, and artifacts
├── .zenodo.json                               # Zenodo citation metadata, creators, license, and keywords
├── package.json                               # npm metadata, public exports, scripts, and dependencies
├── package-lock.json                          # Resolved npm dependency versions
├── pyproject.toml                             # PyPI metadata, package mapping, dependencies, and backend
├── uv.lock                                    # Resolved Python dependency versions
├── README.md                                  # Repository setup and customization instructions
├── RELEASE_NOTES.md                           # Release history and source version for packaging
├── tsconfig.json                              # TypeScript checking and declaration settings
├── vite.config.mts                            # ESM packaging, declaration generation, and Vitest settings
├── .config/                                   # Repository-local .NET tool configuration
│   └── dotnet-tools.json                      # Pins the Fable transpiler version
├── .github/                                   # GitHub repository automation
│   └── workflows/                             # GitHub Actions workflows
│       └── build-test.yml                     # Build and test on Windows and Ubuntu
├── build/                                     # FAKE build and release automation
│   ├── Build.fsproj                           # Executable F# project for the build runner
│   ├── Build.Packages.props                   # Build dependencies and patched transitive versions
│   ├── Helpers.fs                             # FAKE setup, paths, environment, and version helpers
│   ├── MessagePrompts.fs                      # Confirmation and prerelease input prompts
│   ├── ProjectInfo.fs                         # Repository identity, project paths, and version mapping
│   ├── BasicTasks.fs                          # Cleanup, .NET build, and Fable transpilation targets
│   ├── PackageTasks.fs                        # Create NuGet, npm, and Python package artifacts
│   ├── TestTasks.fs                           # Run shared/native tests and browser checks
│   ├── ReleaseNotesTasks.fs                   # Release notes, version synchronization, and GitHub drafts
│   ├── ReleaseTasks.fs                        # Create Git tags and publish package artifacts
│   └── Build.fs                               # Register targets and select the default build
├── src/                                       # Library source and shared package metadata
│   ├── Package.Metadata.props                 # Shared NuGet authors, description, URLs, license, and version
│   └── PolyglotTemplate/                      # One shared library with three compilation variants
│       ├── Directory.Build.props              # Separate restore/build output paths for each project
│       ├── PolyglotTemplate.Common.props      # Ordered F# source list shared by all three variants
│       ├── PolyglotTemplate.fsproj            # .NET library and NuGet package
│       ├── PolyglotTemplate.Javascript.fsproj # JavaScript/TypeScript variant and NuGet package
│       ├── PolyglotTemplate.Python.fsproj     # Python transpilation variant and NuGet package
│       ├── CrossAsync.fs                      # Unify JavaScript Promises with .NET/Python Async
│       ├── ExampleModel.fs                    # Mutable class with AttachMembers and an independent Copy()
│       ├── Runtime.fs                         # Select native emits with Fable compiler directives
│       ├── FileIO.fs                          # Create directories and read/write UTF-8 text asynchronously
│       ├── index.ts                           # Public npm exports for generated TypeScript
│       └── __init__.py                        # Public Python exports and awaitable async wrappers
└── tests/                                     # Shared F# tests and native package-consumer tests
    ├── TestingUtils/                          # Helpers reused by the shared test suite
    │   ├── TestingUtils.fsproj                # Test dependencies and target-specific library references
    │   └── TestingUtils.fs                    # Async tests, temporary-directory cleanup, and Vitest bindings
    ├── PolyglotTemplate/                      # Shared F# tests executed on all three targets
    │   ├── PolyglotTemplate.Tests.fsproj      # Executable shared test project
    │   ├── ExampleModel.Tests.fs              # Model construction, mutation, copies, and compiler branches
    │   ├── FileIO.Tests.fs                    # UTF-8 roundtrips, overwrites, empty files, and errors
    │   └── Main.fs                            # Collect shared tests and select the platform runner
    ├── JavaScript/                            # Native Vitest tests of the public npm API
    │   ├── ExampleModel.test.ts               # Attached members, copy behavior, and runtime identity
    │   └── FileIO.test.ts                     # Async file helpers and browser-runtime behavior
    ├── Python/                                # Native pytest tests of the installed wheel
    │   ├── test_example_model.py              # Public model members, copies, and runtime identity
    │   └── test_file_io.py                    # Awaitable file helpers exercised through asyncio
    └── WebBundling/                           # Browser bundling smoke test
        ├── index.html                         # Browser page loading the smoke-test entrypoint
        ├── main.ts                            # Import and use the built npm model in a browser
        └── vite.config.mts                    # Bundle the consumer into dist/browser-smoke
```

Generated source goes to `src/PolyglotTemplate/ts` and `src/PolyglotTemplate/py`.
Shared test transpilation goes to `tests/PolyglotTemplate/ts` and `py`.
These directories are ignored and recreated by build targets.

| Artifact | Location |
| --- | --- |
| NuGet packages | `dist/net` |
| ESM library and declarations | `dist/ts` |
| npm archive | `dist/npm` |
| Python wheel and source distribution | `dist/py` |
| Browser smoke-test output | `dist/browser-smoke` |

## Build commands

Use either launcher with a target from this table. Running a launcher without a
target builds the solution. Targets perform their prerequisites automatically;
`Clean` runs once at the beginning of a task graph and preserves the build runner.

| Target | Action |
| --- | --- |
| `Clean` | Remove generated source, source/test build output, and `dist` |
| `Build` | Build all .NET projects in Release configuration |
| `TranspileTS`, `TranspilePy` | Generate the corresponding library sources |
| `RunTestsDotnet` | Run shared F# tests on .NET |
| `RunTestsJS` | Transpile shared tests to TypeScript and run through Vitest |
| `RunTestsPy` | Transpile shared tests to Python and run through Pyxpecto |
| `RunTestsJSNative` | Build npm output and test its public root export |
| `RunTestsPyNative` | Build/install the wheel and run pytest against the installed package |
| `TestWebBundling` | Bundle a browser consumer of the built data model |
| `RunTests` | Run all five suites and the browser bundling check |
| `PackDotNet`, `PackJS`, `PackPy` | Create the corresponding package artifacts |
| `Pack` | Create all three NuGet packages, the npm archive, and Python distributions |

## Customization checklist

Choose a name for your project and update every applicable entry below. Rename files/folders and
references together; update the solution through your IDE or `dotnet sln`.

| Area | Places to update |
| --- | --- |
| Repository | GitHub repository name and template setting; solution filename and entries; source/test directory and project filenames |
| F# identity | Namespaces/modules in all F# source and tests; package IDs/tags in the three source projects; project references and shared props filename/imports |
| Shared NuGet metadata | `src/Package.Metadata.props`: authors, description, URLs, license, README location, version |
| Build identity | `build/ProjectInfo.fs`: project, owner, repository URL, solution, source/test paths, library project names, output paths if changed |
| Build paths | `build/BasicTasks.fs`, `TestTasks.fs`, `ReleaseNotesTasks.fs`: cleanup, transpilation/test paths, metadata path; launchers if the build location changes |
| npm metadata | `package.json`: name, description, author/email, repository, homepage, issues URL, keywords, version; main/module/types/exports/files when paths change |
| Python metadata | `pyproject.toml`: name, description, authors/email, URLs, keywords, version, package mapping, generated-file include paths |
| Facades | `src/PolyglotTemplate/index.ts` and `__init__.py`: exports/imports, `__all__`, async adapters when source modules or APIs change |
| Compiler/bundler configuration | `tsconfig.json` source paths; `vite.config.mts` entry, library name, declaration paths, test paths; source-local `Directory.Build.props` root import if moving folders |
| Tests | Shared namespaces/references; native npm/Python package imports; generated-test paths in build/config; browser fixture imports and title |
| Git ignore rules | `.gitignore`: renamed source-generated directories; any additional generated outputs |
| CI | `.github/workflows/build-test.yml`: default branch, runtime versions, launcher paths if changed |
| Documentation | This README: title, links/badges if added, installation/import examples, commands, paths, package names |
| Release metadata | `RELEASE_NOTES.md`: your release history; shared version plus npm/Python versions must agree |
| Attribution | `LICENSE`: copyright for new code; preserve required DataPLANT/copied-code attribution |
| Zenodo | `.zenodo.json`: title, description, creators/affiliations, license, keywords, repository identifier; add only your own valid ORCID/DOI identifiers |
| Registry/release setup | Reserve package names, configure registry authentication, set GitHub draft-release identity/token, check release branch/remotes |
| Lockfiles | Regenerate `package-lock.json` and `uv.lock` after renaming or changing dependencies; never copy another project's lockfiles |

After customization, run `npm install --package-lock-only`, `uv lock`, and the
full test and packaging targets. Search tracked files for missed placeholders:

```text
git grep -n -E 'PolyglotTemplate|polyglottemplate|polyglot_template|example-owner|Example Author|author@example.com|Example Organization'
```

Inspect each match; your checklist/documentation and preserved attribution may
intentionally mention the template. Keep the new project's metadata consistent
across all registries.

## Versions and releases

Commit your customized repository before using Git-based release-note tasks.
`RELEASE_NOTES.md` is the release-version source for packaging. The release-note
targets synchronize its version to `src/Package.Metadata.props`, `package.json`,
`package-lock.json`, `pyproject.toml`, and `uv.lock`.

| Target | Action |
| --- | --- |
| `ReleaseNotes` | Generate release notes from Git history and synchronize versions |
| `PreReleaseNotes` | Prompt for alpha/beta/rc and a number, generate notes, synchronize versions |
| `GithubDraft` | Create a GitHub draft release using the release extension |
| `CreateTag` | Test, pack, then prompt before creating/pushing the version tag |
| `PublishNuget` | Test/pack, prompt, and upload the three NuGet packages |
| `PublishNPM` | Test/pack, prompt, and upload the npm archive |
| `PublishPyPi` | Test/pack, prompt, and upload the Python distributions |
| `Release` | Test/pack, create/push the tag, and publish to all three registries |

Follow the release extension's `semver:patch`, `semver:minor`, or `semver:major`
argument convention, for example `build.cmd ReleaseNotes semver:minor`.
Prereleases map `0.1.0-alpha.1`, `0.1.0-beta.2`, and `0.1.0-rc.3` to Python
`0.1.0a1`, `0.1.0b2`, and `0.1.0rc3`. npm prereleases use the `next` dist-tag;
stable packages use `latest`.

e.g.

```shell
./build.sh ReleaseNotes semver:minor
```
would increment the version from `0.0.1` to `0.1.0`, update the release notes, and synchronize the version across all package metadata files. After that, you can run `./build.sh Release` to create a tag and publish the packages to NuGet, npm, and PyPI.

Before a release, update/review release notes, commit version changes, run
`RunTests --fail-on-focused-tests`, run `Pack`, and inspect the artifacts. Use
`CreateTag` and the publishing targets only once package names, ownership,
repository identity, and authentication are configured:

- NuGet: set `NUGET_KEY` to your API key.
- npm: use `npm login` or your normal registry authentication configuration.
  An OTP is separate from an authentication token.
- PyPI: set `PYPI_KEY` to your API token; Twine receives it through its environment.
- GitHub drafts: set the `github_token` environment variable as described by
  [Fake.Extensions.Release](https://github.com/Freymaurer/Fake.Extensions.Release).
- Tags: ensure the configured repository URL and Git credentials are correct.

Keep credentials out of source control. The build does not print registry keys in
command arguments. Publishing and tagging require an explicit target and
confirmation; the default build and CI do not publish.

## Updating dependencies

Update the local Fable tool in `.config/dotnet-tools.json`, library/test packages
in `Directory.Packages.props`, build packages in `build/Build.Packages.props`, npm
dependencies in `package.json`, and Python dependencies/backend in `pyproject.toml`.
Update the documented uv version and CI together. Compiler, Fable.Core, npm
runtime, and Python runtime versions have independent release sequences; verify
compatibility rather than assigning them all the same version.

`build/Build.Packages.props` also centrally pins four transitive FAKE dependencies
to patched versions (`System.Drawing.Common`, `NuGet.Protocol`, and
`NuGet.Packaging`, and `System.Security.Cryptography.Pkcs`). These overrides apply only to the build project; preserve or
refresh them until upstream FAKE dependencies no longer need the overrides.

Regenerate lockfiles, restore Fable, and run the full test/package checks on both
CI operating systems. Validate package installation from a separate consumer
directory so local sources cannot mask missing package files. Check that the
Python source distribution can build a wheel using only Python tooling.
