module BasicTasks

open System
open System.IO
open BlackFox.Fake
open Fake.Core
open Helpers
open ProjectInfo

let private executable name =
    let command = if OperatingSystem.IsWindows() && (name = "npm" || name = "npx") then name + ".cmd" else name
    ProcessUtils.tryFindFileOnPath command
    |> Option.defaultWith (fun () -> failwith $"{command} is missing from PATH. See README.md prerequisites.")

let runIn directory name arguments =
    CreateProcess.fromRawCommand (executable name) arguments
    |> CreateProcess.withWorkingDirectory (absolutePath directory)
    |> CreateProcess.ensureExitCode
    |> Proc.run
    |> ignore

let run name arguments = runIn "." name arguments
let runPython arguments = run "uv" (["run"; "--no-sync"; "python"] @ arguments)

let hasArgument argument (arguments: string list) =
    arguments |> List.exists (fun value -> value.Equals(argument, StringComparison.OrdinalIgnoreCase))

let clean = BuildTask.create "Clean" [] {
    let directories = [
        "src/ARCManagement/bin"; "src/ARCManagement/obj"
        "src/ARCManagement/ts"; "src/ARCManagement/py"
        "tests/TestingUtils/bin"; "tests/TestingUtils/obj"
        "tests/ARCManagement/bin"; "tests/ARCManagement/obj"
        "tests/ARCManagement/ts"; "tests/ARCManagement/py"
        "dist"
    ]
    for directory in directories do
        let path = absolutePath directory
        if not (path.StartsWith(repositoryRoot + string Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase)) then
            failwith $"Refusing to clean outside the repository: {path}"
        if Directory.Exists path then Directory.Delete(path, true)
}

let build = BuildTask.create "Build" [clean] {
    Fake.DotNet.DotNet.build (fun options ->
        { options with
            Configuration = Fake.DotNet.DotNet.BuildConfiguration.Release
            MSBuildParams = { options.MSBuildParams with DisableInternalBinLog = true }
        }) solutionFile
}

let transpileTS = BuildTask.create "TranspileTS" [clean] {
    run "dotnet" ["fable"; libraryProjects[1]; "--lang"; "ts"; "--fableLib"; "@fable-org/fable-library-ts"; "--noCache"; "-o"; $"{sourceDirectory}/ts"]
}

let transpilePy = BuildTask.create "TranspilePy" [clean] {
    run "dotnet" ["fable"; libraryProjects[2]; "--lang"; "python"; "--noCache"; "-o"; $"{sourceDirectory}/py"]
}
