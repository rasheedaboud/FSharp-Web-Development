namespace App

open Feliz
open Feliz.Router
open Fable.Core
open Fable.React
open Feliz.DaisyUI
open Feliz.DaisyUI.Operators
open ElmishConverter
open ComboBox
open Browser.Dom

[<AutoOpen>]
module JsxHelpers =
  let inline toJsx (el: ReactElement) : JSX.Element = unbox el
  let inline toReact (el: JSX.Element) : ReactElement = unbox el


type Components =
  [<ReactComponent>]
  static member NavBar() =
    Daisy.navbar [
      prop.className "mb-2 shadow-lg bg-neutral text-neutral-content rounded-box"
      prop.children [
        Html.div [
          prop.className "flex-none"
          prop.children [
            Daisy.button.button [
              button.square
              button.ghost
              prop.children [
                Html.i [
                  prop.className "fa-solid fa-gear"
                  ++ color.textSuccess
                ]
              ]
            ]
          ]
        ]
        Html.div [
          prop.className "flex-1 px-2 mx-2"
          prop.children [
            Html.span [
              prop.className "text-lg font-bold"
              prop.text "Feliz Converter"
            ]
          ]
        ]
      ]
    ]

  [<ReactComponent>]
  static member Home() =
    Html.div [
      prop.children [
        Html.h1 [
          prop.className "uppercase font-bold"
          prop.text "Home Page"
        ]
        Html.div [
          prop.className "grid grid-cols-1 md:grid-cols-4 gap-5"
          prop.children [
            Daisy.button.a [
              prop.href "/#converter"
              prop.text "Converter"
            ]
            Daisy.button.a [
              prop.href "/#elmish-converter"
              prop.text "Elmish Converter"
            ]
            Daisy.button.a [
              prop.href "/#fable-fetch"
              prop.text "Fable.Fetch"
            ]
            Daisy.button.a [
              prop.href "/#feliz-deferred"
              prop.text "Feliz.Deferred"
            ]
            ComboBox(
              { label = "Countries";
                selected = fun args -> console.log (args);
                palceholder = "Select a Country";
                data =
                  [| "CANADA"
                     "Cambodia"
                     "Cameroon"
                     "Canada"
                     "Central African Republic"
                     "Chad"
                     "Chile"
                     "China"
                     "Colombia"
                     "Comoros"
                     "Congo, Democratic Republic of the"
                     "Congo, Republic of the"
                     "Costa Rica"
                     "Côte d’Ivoire"
                     "Croatia"
                     "Cuba"
                     "Cyprus"
                     "Czech Republic" |]
              }
            )
          ]
        ]
      ]
    ]


  [<ReactComponent>]
  static member Router() =
    let (currentUrl, updateUrl) = React.useState (Router.currentUrl ())

    Html.div [
      Components.NavBar()
      Html.div [
        prop.className "container mx-auto px-4"
        prop.children [
          React.router [
            router.onUrlChanged updateUrl
            router.children [
              match currentUrl with
              | [] -> Components.Home()
              | [ "elmish-converter" ] -> ElmishConverter.TemperatureConverter()
              | [ "converter" ] -> Converter.Converter()
              | [ "fable-fetch" ] -> FableFetch.FecthData()
              | [ "feliz-deferred" ] -> FelizDeferred.Deferred()
              | otherwise -> Html.h1 "Not found"
            ]
          ]
        ]
      ]
    ]