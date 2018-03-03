// --------------------------------------------------------------------------------------
// FAKE build script 
// --------------------------------------------------------------------------------------
#I "packages/FAKE/tools"
#r "FakeLib.dll"

open Fake 
open Fake.Git
open Fake.AssemblyInfoFile
open Fake.ReleaseNotesHelper
open Fake.Testing

open System
open System.IO
open Fake.AppVeyor

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

/// Solution or project files to be built during the building process
let solution = "System.Functional.sln"

/// Pattern specifying assemblies to be tested
let testAssemblies = "tests/**/bin/Release/*.Tests.dll"

// Git configuration (used for publishing documentation in gh-pages branch)
// The profile where the project is posted
let gitOwner = "stijnmoreels"
let gitHome = sprintf "ssh://github.com/%s" gitOwner

// The name of the project on GitHub
let gitName = "System.Functional"

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
    CreateFSharpAssemblyInfo fileName
        ([Attribute.Title package.Name
          Attribute.Product package.Name
          Attribute.Description package.Summary
          Attribute.Version release.AssemblyVersion
          Attribute.FileVersion release.AssemblyVersion
        ] @ ([]))
    )
)

// --------------------------------------------------------------------------------------
// Clean build results


Target "Clean" (fun _ ->
    CleanDirs ["bin"; "temp"]
)

Target "CleanDocs" (fun _ ->
    CleanDirs ["docs/output"]
)

// --------------------------------------------------------------------------------------
// Build library & test project

Target "Build" (fun _ ->
    !! solution
    |> MSBuildRelease "" "Rebuild"
    |> ignore
)

// --------------------------------------------------------------------------------------
// Run the unit tests using test runner

Target "RunTests" (fun _ ->
    !! testAssemblies
    |> xUnit2 (fun p ->
            { p with
                //ToolPath = "packages/build/xunit.runner.console/tools/xunit.console.exe"
                //The NoAppDomain setting requires care.
                //On mono, it needs to be true otherwise xunit won't work due to a Mono bug.
                //On .NET, it needs to be false otherwise Unquote won't work because it won't be able to load the FsCheck assembly.
                NoAppDomain = isMono
                ShadowCopy = false })
)

// --------------------------------------------------------------------------------------
// Build a NuGet package

Target "PaketPack" (fun _ ->
    Paket.Pack (fun p ->
      { p with
          OutputPath = "bin"
          Version = buildVersion
          ReleaseNotes = toLines release.Notes
      })
)

Target "PaketPush" (fun _ ->
    Paket.Push (fun p ->
        { p with 
            WorkingDir = "bin"
        })
)

// --------------------------------------------------------------------------------------
// .NET Core SDK and .NET Core

let assertExitCodeZero x = if x = 0 then () else failwithf "Command failed with exit code %i" x
let isDotnetSDKInstalled = try Shell.Exec("dotnet", "--version") = 0 with _ -> false
let shellExec cmd args dir =
    printfn "%s %s" cmd args
    Shell.Exec(cmd, args, dir) |> assertExitCodeZero

Target "Build.NetCore" (fun _ ->
    shellExec "dotnet" (sprintf "restore /p:Version=%s System.Functional.sln" buildVersion) "."
    shellExec "dotnet" (sprintf "pack /p:Version=%s --configuration Release" buildVersion) "src/System.Functional"
)

Target "RunTests.NetCore" (fun _ ->
    shellExec "dotnet" "xunit" "tests/System.Functional.Tests"
)

Target "Nuget.AddNetCore" (fun _ ->

    for name in [ "System.Functional"; ] do
        let nupkg = sprintf "../../bin/%s.%s.nupkg" name buildVersion
        let netcoreNupkg = sprintf "bin/Release/%s.%s.nupkg" name buildVersion

        shellExec "dotnet" (sprintf """mergenupkg --source "%s" --other "%s" --framework netstandard1.6 """ nupkg netcoreNupkg) (sprintf "src/%s.netcore/" name)

)

// --------------------------------------------------------------------------------------
// Run all targets by default. Invoke 'build <Target>' to override

Target "CI" DoNothing
Target "Tests" DoNothing

"Clean"
  =?> ("BuildVersion", isAppVeyorBuild)
  ==> "AssemblyInfo"
  ==> "Build"
  ==> "RunTests"
  =?> ("Build.NetCore", isDotnetSDKInstalled)
  =?> ("RunTests.NetCore", isDotnetSDKInstalled)
  ==> "Tests"

"Tests"
  ==> "PaketPack"
  =?> ("Nuget.AddNetCore", isDotnetSDKInstalled)
  ==> "PaketPush"
  ==> "Release"

"GenerateDocs"
  ==> "CI"

"Tests"
  ==> "PaketPack"
  
RunTargetOrDefault "RunTests"
