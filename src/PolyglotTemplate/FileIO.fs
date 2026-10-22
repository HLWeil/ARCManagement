module PolyglotTemplate.FileIO

open PolyglotTemplate.CrossAsync

#if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
open Fable.Core
open Fable.Core.JsInterop

// Importing the model must also work in a browser. Load Node only inside an operation.
let private invoke (methodName: string) (arguments: obj array) : JS.Promise<'T> =
    emitJsExpr (methodName, arguments) """(async () => {
        if (typeof process === 'undefined' || !process.versions?.node) {
            throw new Error('File I/O requires Node.js and is unavailable in the browser.');
        }
        const fs = await import('node:fs/promises');
        return fs[$0](...$1);
    })()"""
#endif

/// Create the directory and any missing parent directories.
let ensureDirectoryAsync (path: string) : CrossAsync<unit> =
#if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
    crossAsync {
        let! _ = invoke "mkdir" [| box path; box {| recursive = true |} |]
        return ()
    }
#endif
#if FABLE_COMPILER_PYTHON
    async {
        Fable.Core.PyInterop.emitPyStatement path "__import__('os').makedirs($0, exist_ok=True)"
    }
#endif
#if !FABLE_COMPILER
    async {
        System.IO.Directory.CreateDirectory path |> ignore
    }
#endif

/// Read UTF-8 text. A missing file raises the platform's filesystem error.
let readTextAsync (path: string) : CrossAsync<string> =
#if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
    invoke "readFile" [| box path; box "utf8" |]
#endif
#if FABLE_COMPILER_PYTHON
    async {
        return Fable.Core.PyInterop.emitPyExpr path "__import__('pathlib').Path($0).read_text(encoding='utf-8')"
    }
#endif
#if !FABLE_COMPILER
    async {
        return System.IO.File.ReadAllText(path, System.Text.Encoding.UTF8)
    }
#endif

/// Create or overwrite a UTF-8 text file. The parent directory must already exist.
let writeTextAsync (path: string) (text: string) : CrossAsync<unit> =
#if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
    invoke "writeFile" [| box path; box text; box "utf8" |]
#endif
#if FABLE_COMPILER_PYTHON
    async {
        Fable.Core.PyInterop.emitPyStatement (path, text) "__import__('pathlib').Path($0).write_text($1, encoding='utf-8', newline='')"
    }
#endif
#if !FABLE_COMPILER
    async {
        System.IO.File.WriteAllText(path, text, System.Text.UTF8Encoding(false))
    }
#endif
