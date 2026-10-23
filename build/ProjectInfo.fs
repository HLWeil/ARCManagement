module ProjectInfo

open System.Text.RegularExpressions
open Fake.Core
open Helpers

let project = "ARCManagement"
let gitOwner = "HLWeil"
let projectRepo = $"https://github.com/{gitOwner}/{project}"
let solutionFile = $"{project}.sln"
let configuration = "Release"
let sourceDirectory = "src/ARCManagement"
let allTestsProject = "tests/ARCManagement/ARCManagement.Tests.fsproj"
let libraryProjects = [
    $"{sourceDirectory}/ARCManagement.fsproj"
    $"{sourceDirectory}/ARCManagement.Javascript.fsproj"
    $"{sourceDirectory}/ARCManagement.Python.fsproj"
]
let netPkgDir = "dist/net"
let jsOutputDir = "dist/ts"
let npmPkgDir = "dist/npm"
let pyPkgDir = "dist/py"

type VersionController() =
    let mutable release = ReleaseNotes.load (absolutePath "RELEASE_NOTES.md")

    member _.NugetTag = release.NugetVersion.Split('+')[0]
    member this.NPMTag = this.NugetTag
    member _.IsPrerelease = release.SemVer.PreRelease.IsSome
    member _.Notes = release.Notes
    member this.PyTag =
        let version = this.NugetTag
        let prerelease = Regex.Match(version, @"^(\d+\.\d+\.\d+)-(alpha|beta|rc)\.(\d+)$")
        if prerelease.Success then
            let suffix = match prerelease.Groups[2].Value with "alpha" -> "a" | "beta" -> "b" | _ -> "rc"
            prerelease.Groups[1].Value + suffix + prerelease.Groups[3].Value
        elif Regex.IsMatch(version, @"^\d+\.\d+\.\d+$") then version
        else failwith $"Unsupported release version: {version}. Use major.minor.patch[-alpha.N|-beta.N|-rc.N]."
    member _.Refresh() = release <- ReleaseNotes.load (absolutePath "RELEASE_NOTES.md")

let versionController = VersionController()
