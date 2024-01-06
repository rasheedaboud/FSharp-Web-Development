module ElmishConverter

open System
open Feliz
open Feliz.UseElmish
open Feliz.DaisyUI
open Elmish

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

let init () =
  {
    Temperature = -20.0m
    Scale = Celsius
  },
  Cmd.none


type Msg =
  | UpdateTemperature of string
  | ChangeScale

let validTemp temp =
  if String.IsNullOrEmpty(temp) then 0.0m else decimal temp

let update msg state =
  match msg with
  | UpdateTemperature temp ->

    {
      state with
          Temperature = validTemp temp
    },
    Cmd.none
  | ChangeScale ->
    match state.Scale with
    | Celsius ->
      let convert = (state.Temperature * 9.0m / 5.0m) + 32.0m

      {
        state with
            Scale = Fahrenheit
            Temperature = convert
      },
      Cmd.none

    | Fahrenheit ->
      let convert = (state.Temperature - 32.0m) * 5.0m / 9.0m

      {
        state with
            Scale = Celsius
            Temperature = convert
      },
      Cmd.none

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
let TemperatureConverter () =
  let (state, dispatch) = React.useElmish (init, update)
  view state dispatch