module Helpers

open System
open System.IO
open BlackFox.Fake
open Fake.Core

let repositoryRoot = Path.GetFullPath(Path.Combine(__SOURCE_DIRECTORY__, ".."))
let absolutePath path = Path.GetFullPath(Path.Combine(repositoryRoot, path))

let initializeContext () =
    let context = Context.FakeExecutionContext.Create false "build/Build.fsproj" []
    Context.setExecutionContext (Context.RuntimeContext.Fake context)

let runOrDefault defaultTarget (args: string array) =
    try
        match args with
        | [||] -> BuildTask.runOrDefault defaultTarget
        | [| target |] -> Target.runOrDefault target
        | _ -> Target.run 0 args[0] (args |> Array.skip 1 |> Array.toList)
        0
    with error ->
        eprintfn "%s" (error.ToString())
        1

let withEnvironment variables action =
    let previous = variables |> List.map (fun (key, _) -> key, Environment.GetEnvironmentVariable key)
    try
        variables |> List.iter (fun (key, value) -> Environment.SetEnvironmentVariable(key, value))
        action ()
    finally
        previous |> List.iter (fun (key, value) -> Environment.SetEnvironmentVariable(key, value))

type PreReleaseFlag =
    | Alpha
    | Beta
    | ReleaseCandidate

    static member fromInput input =
        match input with
        | "a" | "alpha" -> Alpha
        | "b" | "beta" -> Beta
        | "rc" -> ReleaseCandidate
        | _ -> invalidArg "input" "Use alpha, beta, or rc."

    static member toNugetSuffix flag number =
        if number < 1 then invalidArg "number" "Prerelease numbers start at 1."
        let name = match flag with Alpha -> "alpha" | Beta -> "beta" | ReleaseCandidate -> "rc"
        $"{name}.{number}"
