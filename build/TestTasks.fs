module TestTasks

open System.IO
open BlackFox.Fake
open BasicTasks
open Helpers
open PackageTasks
open ProjectInfo

let private focused arguments action =
    if hasArgument "--fail-on-focused-tests" arguments then
        withEnvironment ["FAIL_ON_FOCUSED_TESTS", "1"] action
    else action ()

let private flags arguments =
    if hasArgument "--fail-on-focused-tests" arguments then ["--fail-on-focused-tests"] else []

let runTestsDotnet = BuildTask.createFn "RunTestsDotnet" [build] (fun config ->
    run "dotnet" (["run"; "--project"; allTestsProject; "--configuration"; configuration; "--no-build"; "--"] @ flags config.Context.Arguments))

let runTestsJS = BuildTask.createFn "RunTestsJS" [build] (fun config ->
    run "dotnet" ["fable"; allTestsProject; "--lang"; "ts"; "--fableLib"; "@fable-org/fable-library-ts"; "--noCache"; "-e"; "fs.ts"; "-o"; "tests/PolyglotTemplate/ts"]
    focused config.Context.Arguments (fun () -> run "npx" ["vitest"; "run"; "tests/PolyglotTemplate/ts/Main.fs.ts"]))

let runTestsPy = BuildTask.createFn "RunTestsPy" [build] (fun config ->
    run "dotnet" ["fable"; allTestsProject; "--lang"; "python"; "--noCache"; "-o"; "tests/PolyglotTemplate/py"]
    runPython (["tests/PolyglotTemplate/py/main.py"] @ flags config.Context.Arguments))

let runTestsJSNative = BuildTask.createFn "RunTestsJSNative" [packJS] (fun config ->
    focused config.Context.Arguments (fun () -> run "npx" ["vitest"; "run"; "tests/JavaScript"]))

let runTestsPyNative = BuildTask.create "RunTestsPyNative" [packPy] {
    let wheel = Directory.GetFiles(absolutePath pyPkgDir, "*.whl") |> Array.exactlyOne
    run "uv" ["pip"; "install"; "--python"; absolutePath (if System.OperatingSystem.IsWindows() then ".venv/Scripts/python.exe" else ".venv/bin/python"); "--no-deps"; "--reinstall"; wheel]
    Fake.IO.Directory.ensure (absolutePath "tmp/pytest")
    let temporaryDirectory = absolutePath ("tmp/pytest/" + System.Guid.NewGuid().ToString("N"))
    runPython ["-m"; "pytest"; "tests/Python"; "--basetemp"; temporaryDirectory; "-p"; "no:cacheprovider"]
}

let testWebBundling = BuildTask.create "TestWebBundling" [packJS] {
    runIn "tests/WebBundling" "npx" ["vite"; "build"]
}

let runTests = BuildTask.createEmpty "RunTests" [runTestsDotnet; runTestsJS; runTestsPy; runTestsJSNative; runTestsPyNative; testWebBundling]
