module ARCManagement.Runtime

/// Demonstrates selecting native emits at compile time.
let getRuntimeName () : string =
#if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
    Fable.Core.JsInterop.emitJsExpr () "'JavaScript'"
#endif
#if FABLE_COMPILER_PYTHON
    Fable.Core.PyInterop.emitPyExpr () "'Python'"
#endif
#if !FABLE_COMPILER
    ".NET"
#endif
