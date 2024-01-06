module Main

open Feliz
open App
open Browser.Dom
open Fable.Core.JsInterop

importAll "./main.css"
let root = ReactDOM.createRoot (document.getElementById "feliz-app")
root.render (Components.Router())
