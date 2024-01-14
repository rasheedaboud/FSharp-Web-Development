module Main

open Feliz
open App
open Browser.Dom
open Fable.Core.JsInterop
open Validation.Yup

importSideEffects "./main.css"

validate ()

let root = ReactDOM.createRoot (document.getElementById "feliz-app")
root.render (Components.Router())