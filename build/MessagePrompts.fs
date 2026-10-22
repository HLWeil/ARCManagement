module MessagePrompts

let confirm message =
    printf "%s [y/N]: " message
    match System.Console.ReadLine() with
    | "y" | "Y" -> ()
    | _ -> failwith "Aborted."

let readPrerelease () =
    printf "Prerelease kind (alpha/beta/rc): "
    let kind = System.Console.ReadLine() |> Helpers.PreReleaseFlag.fromInput
    printf "Prerelease number: "
    let number = System.Console.ReadLine() |> int
    Helpers.PreReleaseFlag.toNugetSuffix kind number
