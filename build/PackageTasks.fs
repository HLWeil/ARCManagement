module PackageTasks

open System.IO
open BlackFox.Fake
open BasicTasks
open Helpers
open ProjectInfo

let pythonArtifacts () =
    Directory.GetFiles(absolutePath pyPkgDir)
    |> Array.filter (fun path -> path.EndsWith(".whl") || path.EndsWith(".tar.gz"))
    |> Array.sort
    |> Array.toList

let packDotNet = BuildTask.create "PackDotNet" [build] {
    Fake.IO.Directory.ensure (absolutePath netPkgDir)
    for project in libraryProjects do
        run "dotnet" ["pack"; project; "--configuration"; configuration; "--output"; absolutePath netPkgDir; $"-p:Version={versionController.NugetTag}"]
}

let packJS = BuildTask.create "PackJS" [build; transpileTS] {
    Fake.JavaScript.Npm.exec "run build" (fun options -> { options with WorkingDirectory = repositoryRoot })
    Fake.IO.Directory.ensure (absolutePath npmPkgDir)
    run "npm" ["pack"; "--pack-destination"; absolutePath npmPkgDir]
}

let packPy = BuildTask.create "PackPy" [build; transpilePy] {
    run "uv" ["build"; "--out-dir"; pyPkgDir]
    runPython (["-m"; "twine"; "check"] @ pythonArtifacts ())
}

let pack = BuildTask.createEmpty "Pack" [packDotNet; packJS; packPy]
