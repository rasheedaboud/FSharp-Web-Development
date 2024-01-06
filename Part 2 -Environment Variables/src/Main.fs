module Main

open Feliz
open App
open Browser.Dom

#if DEBUG
console.log ("This will only print if environment is set to debug")
#endif

console.log (Env.ViteKey.value)

let root = ReactDOM.createRoot (document.getElementById "feliz-app")
root.render (Components.Counter())
