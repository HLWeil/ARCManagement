module TestingUtils

open Fable.Core
open Fable.Pyxpecto
open ARCManagement.CrossAsync

let testCaseCrossAsync name (operation: unit -> CrossAsync<unit>) =
    testCaseAsync name (async { do! operation () |> asAsync })

let private createTemporaryDirectory () : CrossAsync<string> =
#if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
    Fable.Core.JsInterop.emitJsExpr () """(async () => {
        const fs = await import('node:fs/promises');
        const os = await import('node:os');
        const path = await import('node:path');
        return fs.mkdtemp(path.join(os.tmpdir(), 'arc-management-'));
    })()"""
#endif
#if FABLE_COMPILER_PYTHON
    async { return Fable.Core.PyInterop.emitPyExpr () "__import__('tempfile').mkdtemp(prefix='arc-management-')" }
#endif
#if !FABLE_COMPILER
    async {
        let path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "arc-management-" + System.Guid.NewGuid().ToString("N"))
        System.IO.Directory.CreateDirectory path |> ignore
        return path
    }
#endif

let private removeTemporaryDirectory (path: string) : CrossAsync<unit> =
#if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
    Fable.Core.JsInterop.emitJsExpr path "import('node:fs/promises').then(fs => fs.rm($0, { recursive: true, force: true }))"
#endif
#if FABLE_COMPILER_PYTHON
    async { Fable.Core.PyInterop.emitPyStatement path "__import__('shutil').rmtree($0)" }
#endif
#if !FABLE_COMPILER
    async { System.IO.Directory.Delete(path, true) }
#endif

/// Await cleanup on both paths, including when a test assertion fails.
let withTemporaryDirectory (operation: string -> CrossAsync<'T>) : CrossAsync<'T> =
    crossAsync {
        let! path = createTemporaryDirectory ()
        let! outcome = operation path |> catchAsChoice
        do! removeTemporaryDirectory path
        match outcome with
        | Choice1Of2 value -> return value
        | Choice2Of2 error -> return raise error
    }

#if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
module TSUtils =
    open Fable.Core.JsInterop

    [<Import("describe", "vitest")>]
    let describe (_name: string, _suite: unit -> unit) : unit = jsNative

    [<Import("it", "vitest")>]
    let itAsync (_name: string, _test: unit -> JS.Promise<unit>) : unit = jsNative

    let failOnFocused () : bool =
        emitJsExpr () "process.env.FAIL_ON_FOCUSED_TESTS === '1'"
#endif
