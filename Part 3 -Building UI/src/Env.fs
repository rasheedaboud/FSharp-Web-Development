[<RequireQualifiedAccess>]
module Env

open Fable.Core

[<RequireQualifiedAccess>]
module ViteKey =

    [<Literal>]
    let private key = "import.meta.env.VITE_KEY"

    [<Emit(key)>]
    let private vite_key_maybe: string option = jsNative

    let value =
        match vite_key_maybe with
        | Some envValue -> envValue
        | None -> failwith $"no env variable with name:{key}  found."
