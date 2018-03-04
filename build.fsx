// --------------------------------------------------------------------------------------
// FAKE build script 
// --------------------------------------------------------------------------------------
#I "packages/FAKE/tools"
#r "FakeLib.dll"

open Fake 
open Fake.AssemblyInfoFile
open Fake.ReleaseNotesHelper
open Fake.Testing

open System

// Information about each project is used
//  - for version and project name in generated AssemblyInfo file
//  - by the generated NuGet package 
//  - to run tests and to publish documentation on GitHub gh-pages
//  - for documentation, you also need to edit info in "docs/tools/generate.fsx"

type ProjectInfo =
  { /// The name of the project 
    /// (used by attributes in AssemblyInfo, name of a NuGet package and directory in 'src')
    Name : string
    /// Short summary of the project
    /// (used as description in AssemblyInfo and as a short summary for NuGet package)
    Summary : string
  }

//File that contains the release notes.
let releaseNotes = "System.Functional Release Notes.md"

// Read additional information from the release notes document
Environment.CurrentDirectory <- __SOURCE_DIRECTORY__
let release = LoadReleaseNotes releaseNotes

let buildDate = DateTime.UtcNow
let buildVersion = 
    let isVersionTag tag = Version.TryParse tag |> fst
    release.NugetVersion

let packages =
  [
    { Name = "System.Functional"
      Summary = "System.Functional is a minimum extension library to write more readable functional code in C#."
    }
  ]


// Generate assembly info files with the right version & up-to-date information
Target "AssemblyInfo" (fun _ ->
    packages |> Seq.iter (fun package ->
    let fileName = "src/" + package.Name + "/AssemblyInfo.fs"
    CreateCSharpAssemblyInfo fileName
        ([Attribute.Title package.Name
          Attribute.Product package.Name
          Attribute.Description package.Summary
          Attribute.Version release.AssemblyVersion
          Attribute.FileVersion release.AssemblyVersion
        ] @ ([]))
    )
)

let assertExitCodeZero x = if x = 0 then () else failwithf "Command failed with exit code %i" x
let isDotnetSDKInstalled = try Shell.Exec("dotnet", "--version") = 0 with _ -> false
let shellExec cmd args dir =
    printfn "%s %s" cmd args
    Shell.Exec(cmd, args, dir) |> assertExitCodeZero

Target "Build" <| fun _ ->
    shellExec "dotnet" (sprintf "restore /p:Version=%s System.Functional.sln" buildVersion) "."

Target "RunTests" <| fun _ ->
    shellExec "dotnet" "xunit" "tests/System.Functional.Tests"

Target "Nuget" <| fun _ ->
    shellExec "dotnet" (sprintf "pack /p:Version=%s --configuration Release" buildVersion) "src/System.Functional"

"Build"
==> "RunTests"
==> "Nuget"

RunTargetOrDefault "RunTests"