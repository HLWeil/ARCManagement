module PolyglotTemplate.CrossAsync

open Fable.Core

/// A Promise in JavaScript and an F# async workflow in .NET and Python.
type CrossAsync<'T> =
#if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
    JS.Promise<'T>
#else
    Async<'T>
#endif

let inline crossAsync<'T> =
#if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
    promise
#else
    async
#endif

let asAsync (operation: CrossAsync<'T>) : Async<'T> =
#if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
    Async.AwaitPromise operation
#else
    operation
#endif

let map transform operation =
    crossAsync {
        let! value = operation
        return transform value
    }

let catchAsChoice (operation: CrossAsync<'T>) : CrossAsync<Choice<'T, exn>> =
    crossAsync {
        try
            let! value = operation
            return Choice1Of2 value
        with error ->
            return Choice2Of2 error
    }
