module Main

open Feliz
open App
open Browser.Dom
open Fable.Core.JsInterop

importSideEffects "./main.css"

let root = ReactDOM.createRoot (document.getElementById "feliz-app")
root.render (Components.Router())