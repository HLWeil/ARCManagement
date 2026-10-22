module PolyglotTemplate.Tests.FileIO

open Fable.Pyxpecto
open PolyglotTemplate
open PolyglotTemplate.CrossAsync
open TestingUtils

let tests = testList "FileIO" [
    testCaseCrossAsync "create parents, roundtrip Unicode and overwrite" (fun () ->
        withTemporaryDirectory (fun directory -> crossAsync {
            let nested = directory + "/nested/child"
            do! FileIO.ensureDirectoryAsync nested
            let path = nested + "/example.txt"
            let text = "Hello, 世界!\nGrüße 👋\n"
            do! FileIO.writeTextAsync path text
            let! actual = FileIO.readTextAsync path
            Expect.equal actual text "UTF-8 text should roundtrip"
            do! FileIO.writeTextAsync path "replacement"
            let! replaced = FileIO.readTextAsync path
            Expect.equal replaced "replacement" "Writing should overwrite"
            do! FileIO.writeTextAsync path ""
            let! empty = FileIO.readTextAsync path
            Expect.equal empty "" "Empty files should roundtrip"
        }))
    testCaseCrossAsync "missing file raises an error" (fun () ->
        withTemporaryDirectory (fun directory -> crossAsync {
            let! outcome = FileIO.readTextAsync (directory + "/missing.txt") |> catchAsChoice
            match outcome with
            | Choice2Of2 _ -> ()
            | Choice1Of2 _ -> failwith "Reading a missing file must fail"
        }))
    testCaseCrossAsync "writing does not create parent directories" (fun () ->
        withTemporaryDirectory (fun directory -> crossAsync {
            let! outcome = FileIO.writeTextAsync (directory + "/missing/example.txt") "text" |> catchAsChoice
            match outcome with
            | Choice2Of2 _ -> ()
            | Choice1Of2 _ -> failwith "Writing beneath a missing parent must fail"
        }))
]
