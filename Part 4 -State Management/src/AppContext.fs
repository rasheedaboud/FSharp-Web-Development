[<RequireQualifiedAccess>]
module AppContext

open Feliz

type AppState = { message: string }

let appState = { message = "Hello" }

let appContext = React.createContext ()

[<ReactComponent>]
let MessageContext (child: ReactElement) =
  let (state, setState) = React.useState (appState)
  React.contextProvider (appContext, (state, setState), React.fragment [ child ])