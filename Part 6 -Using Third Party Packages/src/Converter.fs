module Converter

open System
open Feliz
open Feliz.DaisyUI
open Browser.Types

type TemperatureScale =
  | Celsius
  | Fahrenheit

  member this.Value =
    match this with
    | Celsius -> nameof Celsius
    | Fahrenheit -> nameof Fahrenheit

type State = {
  Temperature: decimal
  Scale: TemperatureScale
}

let init = {
  Temperature = -20.0m
  Scale = Celsius
}


type Msg =
  | UpdateTemperature of string
  | ChangeScale

let validTemp temp =
  if String.IsNullOrEmpty(temp) then 0.0m else decimal temp

let view (state: State) dispatch =
  Html.div [
    prop.className "grid grid-flow-row auto-rows-max"
    prop.children [
      Daisy.label [ prop.htmlFor "temperature-input"; prop.text "Enter Temperature:" ]

      Daisy.input [
        prop.id "temperature-input"
        prop.type' "text"
        prop.placeholder "Temperature"
        prop.value (string state.Temperature)
        prop.onChange (fun e -> dispatch (UpdateTemperature e))
      ]

      Daisy.button.button [
        prop.classes [ "btn"; "btn-primary"; "my-2" ]
        prop.onClick (fun _ -> dispatch ChangeScale)
        if state.Scale = Celsius then
          prop.text "Convert to Fahrenheit"
        else
          prop.text "Convert to Celsius"
      ]

      Html.div [
        prop.classes [ "text-lg"; "font-bold"; "pt-4" ]
        prop.text (sprintf "Temperature: %.2f° %s" state.Temperature state.Scale.Value)
      ]
    ]
  ]

[<ReactComponent>]
let Converter () =

  let state, setState = React.useState (init)

  let changeScale (evt: MouseEvent) =
    match state.Scale with
    | Celsius ->
      let convert = (state.Temperature * 9.0m / 5.0m) + 32.0m

      setState (
        {
          state with
              Scale = Fahrenheit
              Temperature = convert
        }
      )
    | Fahrenheit ->
      let convert = (state.Temperature - 32.0m) * 5.0m / 9.0m

      setState (
        {
          state with
              Scale = Celsius
              Temperature = convert
        }
      )

  let updateTemperature (temp: string) =
    setState (
      {
        state with
            Temperature = validTemp temp
      }
    )

  Html.div [
    prop.className "grid grid-flow-row auto-rows-max"
    prop.children [
      Daisy.label [ prop.htmlFor "temperature-input"; prop.text "Enter Temperature:" ]

      Daisy.input [
        prop.id "temperature-input"
        prop.type' "text"
        prop.placeholder "Temperature"
        prop.value (string state.Temperature)
        prop.onChange updateTemperature
      ]

      Daisy.button.button [
        prop.classes [ "btn"; "btn-primary"; "my-2" ]
        prop.onClick changeScale
        if state.Scale = Celsius then
          prop.text "Convert to Fahrenheit"
        else
          prop.text "Convert to Celsius"
      ]

      Html.div [
        prop.classes [ "text-lg"; "font-bold"; "pt-4" ]
        prop.text (sprintf "Temperature: %.2f° %s" state.Temperature state.Scale.Value)
      ]
    ]
  ]