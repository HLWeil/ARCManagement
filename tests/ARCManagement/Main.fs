module ARCManagement.Tests.Main

open Fable.Core
open Fable.Pyxpecto

let all = testList "ARCManagement" [ ExampleModel.tests; FileIO.tests ]

#if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
open TestingUtils.TSUtils

describe ("Shared F# tests", fun () ->
    itAsync ("all shared examples pass", fun () ->
        let config = [|
            ConfigArg.DoNotExitWithCode
            if failOnFocused () then ConfigArg.FailOnFocused
        |]
        Pyxpecto.runTestsAsync config all
        |> Async.StartAsPromise
        |> Promise.map (fun exitCode -> Expect.equal exitCode 0 "Shared tests failed")))
#else
[<EntryPoint>]
let main _argv = Pyxpecto.runTests [||] all
#endif
