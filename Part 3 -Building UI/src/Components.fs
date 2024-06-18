namespace App

open Feliz
open Feliz.Router
open Fable.Core
open Fable.React
open Feliz.DaisyUI
open Feliz.DaisyUI.Operators

[<AutoOpen>]
module JsxHelpers =
  let inline toJsx (el: ReactElement) : JSX.Element = unbox el
  let inline toReact (el: JSX.Element) : ReactElement = unbox el


type Components =
  [<ReactComponent>]
  static member NavBar() =
    Daisy.navbar
      [ prop.className "mb-2 shadow-lg bg-neutral text-neutral-content rounded-box"
        prop.children
          [ Html.div
              [ prop.className "flex-none"
                prop.children
                  [ Daisy.button.button
                      [ button.square
                        button.ghost
                        prop.children [ Html.i [ prop.className "fa-solid fa-gear" ++ color.textSuccess ] ] ] ] ]
            Html.div
              [ prop.className "flex-1 px-2 mx-2"
                prop.children [ Html.span [ prop.className "text-lg font-bold"; prop.text "Feliz Converter" ] ] ] ] ]

  [<ReactComponent>]
  static member Home() =
    Html.div[prop.className "container mx-auto"
             prop.children [ Html.h1 "Home Page" ]]

  [<ReactComponent>]
  static member Router() =
    let (currentUrl, updateUrl) = React.useState (Router.currentUrl ())

    Html.div
      [ Components.NavBar()
        React.router
          [ router.onUrlChanged updateUrl
            router.children
              [ match currentUrl with
                | [] -> Components.Home()
                | otherwise -> Html.h1 "Not found" ] ] ]