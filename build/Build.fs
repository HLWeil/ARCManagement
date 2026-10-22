module Build

open System.IO
open Helpers

initializeContext ()

// Register optional targets that are not prerequisites of the default build.
BasicTasks.transpileTS |> ignore
BasicTasks.transpilePy |> ignore
PackageTasks.pack |> ignore
TestTasks.runTests |> ignore
ReleaseNotesTasks.updateReleaseNotes |> ignore
ReleaseNotesTasks.updatePreReleaseNotes |> ignore
ReleaseNotesTasks.githubDraft |> ignore
ReleaseTasks.release |> ignore

// These ordering constraints apply only when both targets are selected (Release).
// Calling one publishing target does not create a tag or publish another package.
open Fake.Core.TargetOperators
"CreateTag" ?=> "PublishNuget" ?=> "PublishNPM" ?=> "PublishPyPi" |> ignore

[<EntryPoint>]
let main arguments =
    Directory.SetCurrentDirectory repositoryRoot
    runOrDefault BasicTasks.build arguments
