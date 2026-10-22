namespace PolyglotTemplate

open Fable.Core

/// AttachMembers keeps instance members accessible to native language callers.
[<AttachMembers>]
type ExampleModel(name: string, value: int) =
    member val Name = name with get, set
    member val Value = value with get, set

    member this.Increment(amount: int) =
        this.Value <- this.Value + amount

    member this.Copy() =
        ExampleModel(this.Name, this.Value)
