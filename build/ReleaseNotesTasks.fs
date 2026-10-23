module ReleaseNotesTasks

open System.IO
open System.Xml.Linq
open BlackFox.Fake
open Fake.Extensions.Release
open BasicTasks
open Helpers
open MessagePrompts
open ProjectInfo

let private synchronizeVersions () =
    versionController.Refresh()
    // Validate Python's prerelease mapping before changing any manifests.
    let pythonVersion = versionController.PyTag
    run "npm" ["version"; versionController.NPMTag; "--no-git-tag-version"; "--allow-same-version"; "--ignore-scripts"]
    run "uv" ["version"; pythonVersion; "--no-sync"]
    let metadataPath = absolutePath "src/Package.Metadata.props"
    let metadata = XDocument.Load(metadataPath, LoadOptions.PreserveWhitespace)
    metadata.Descendants(XName.Get "Version") |> Seq.exactlyOne |> fun version -> version.Value <- versionController.NugetTag
    metadata.Save(metadataPath, SaveOptions.DisableFormatting)
    printfn "Versions synchronized: NuGet/npm %s, Python %s" versionController.NugetTag pythonVersion

let updateReleaseNotes = BuildTask.createFn "ReleaseNotes" [] (fun config ->
    if config.Context.Arguments |> List.exists (fun arg -> arg.StartsWith("pre:")) then
        failwith "Use PreReleaseNotes for alpha, beta, and release-candidate versions."
    ReleaseNotes.update(gitOwner, project, config)
    synchronizeVersions ())

let updatePreReleaseNotes = BuildTask.createFn "PreReleaseNotes" [] (fun config ->
    if config.Context.Arguments |> List.exists (fun arg -> arg.StartsWith("pre:")) then
        failwith "Enter the prerelease kind and number when prompted."
    let suffix = readPrerelease ()
    let arguments = config.Context.Arguments @ ["pre:" + suffix]
    let config = { config with Context = { config.Context with Arguments = arguments } }
    ReleaseNotes.update(gitOwner, project, config)
    synchronizeVersions ())

let githubDraft = BuildTask.createFn "GithubDraft" [] (fun config ->
    Github.draft(gitOwner, project, Some "Release of ARCManagement.", None, config))
