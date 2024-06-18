module Main

open Feliz
open App
open Browser.Dom
open Fable.Core.JsInterop
open Validation

importSideEffects "./main.css"

let result = validate ()


let root = ReactDOM.createRoot (document.getElementById "feliz-app")
root.render (Components.Router())