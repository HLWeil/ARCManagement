module ARCManagement.Tests.ExampleModel

open Fable.Pyxpecto
open ARCManagement

let tests = testList "ExampleModel" [
    testCase "construct and mutate" (fun () ->
        let model = ExampleModel("example", 2)
        Expect.equal model.Name "example" "Name should be accessible"
        Expect.equal model.Value 2 "Initial value should be accessible"
        model.Name <- "changed"
        model.Increment 3
        Expect.equal model.Name "changed" "Name should be mutable"
        Expect.equal model.Value 5 "Increment should update the value"
        model.Value <- -1
        Expect.equal model.Value -1 "Value should be mutable")
    testCase "copy is independent" (fun () ->
        let model = ExampleModel("original", 7)
        let copy = model.Copy()
        model.Name <- "changed"
        model.Increment 2
        Expect.equal copy.Name "original" "Copy should retain the original name"
        Expect.equal copy.Value 7 "Copy should retain the original value")
    testCase "runtime-specific emit" (fun () ->
        let expected =
#if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
            "JavaScript"
#endif
#if FABLE_COMPILER_PYTHON
            "Python"
#endif
#if !FABLE_COMPILER
            ".NET"
#endif
        Expect.equal (Runtime.getRuntimeName()) expected "Wrong compiler branch")
]
