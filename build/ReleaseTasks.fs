module ReleaseTasks

open System
open System.Diagnostics
open System.IO
open BlackFox.Fake
open BasicTasks
open Helpers
open MessagePrompts
open PackageTasks
open ProjectInfo
open TestTasks

let private requiredVariable name =
    match Environment.GetEnvironmentVariable name with
    | null | "" -> failwith $"Set {name} before publishing."
    | value -> value

// NuGet requires the API key as a process argument. Do not trace that argument,
// and redact the key from process output before displaying it.
let private pushNuget artifact key =
    let start = ProcessStartInfo("dotnet")
    start.WorkingDirectory <- repositoryRoot
    start.UseShellExecute <- false
    start.RedirectStandardOutput <- true
    start.RedirectStandardError <- true
    for argument in ["nuget"; "push"; artifact; "--source"; "https://api.nuget.org/v3/index.json"; "--api-key"; key; "--skip-duplicate"] do
        start.ArgumentList.Add argument
    use child = new Process(StartInfo = start)
    let print (line: string) = if not (isNull line) then printfn "%s" (line.Replace(key, "[REDACTED]"))
    child.OutputDataReceived.Add(fun data -> print data.Data)
    child.ErrorDataReceived.Add(fun data -> print data.Data)
    if not (child.Start()) then failwith "Failed to start NuGet publishing."
    child.BeginOutputReadLine()
    child.BeginErrorReadLine()
    child.WaitForExit()
    if child.ExitCode <> 0 then failwith $"NuGet publishing failed with exit code {child.ExitCode}."

let createTag = BuildTask.create "CreateTag" [runTests; pack] {
    let tag = versionController.NugetTag
    confirm $"Create and push tag {tag} to {projectRepo}?"
    Fake.Tools.Git.Branches.tag repositoryRoot tag
    Fake.Tools.Git.Branches.pushTag repositoryRoot projectRepo tag
}

let publishNuget = BuildTask.create "PublishNuget" [runTests; packDotNet] {
    confirm $"Publish NuGet packages {versionController.NugetTag}?"
    let key = requiredVariable "NUGET_KEY"
    for artifact in Directory.GetFiles(absolutePath netPkgDir, "*.nupkg") |> Array.sort do
        pushNuget artifact key
}

let publishNPM = BuildTask.create "PublishNPM" [runTests; packJS] {
    confirm $"Publish npm package {versionController.NPMTag}?"
    let archive = Directory.GetFiles(absolutePath npmPkgDir, "*.tgz") |> Array.exactlyOne
    let tag = if versionController.IsPrerelease then "next" else "latest"
    // npm reads authentication from npm login / the user's registry configuration.
    run "npm" ["publish"; archive; "--access"; "public"; "--tag"; tag]
}

let publishPyPi = BuildTask.create "PublishPyPi" [runTests; packPy] {
    confirm $"Publish Python package {versionController.PyTag}?"
    let token = requiredVariable "PYPI_KEY"
    withEnvironment ["TWINE_USERNAME", "__token__"; "TWINE_PASSWORD", token; "TWINE_NON_INTERACTIVE", "1"] (fun () ->
        runPython (["-m"; "twine"; "upload"] @ pythonArtifacts ()))
}

let release = BuildTask.createEmpty "Release" [createTag; publishNuget; publishNPM; publishPyPi]
