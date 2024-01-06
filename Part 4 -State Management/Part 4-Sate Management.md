Welcome back to our series on building web applications using F# with Feliz and Fable! In our previous post, we delved into constructing a UI using the Feliz.DaisyUI library. Today, we will be moving a step further into the realm of interactivity by introducing state management to our application.

There are a few ways this can be accomplished in a Javascript react application, to name a few we have the inbuilt

- [useState](https://www.w3schools.com/react/react_usestate.asp)
- [useContext](https://www.w3schools.com/react/react_usecontext.asp)

We have third party libraries like:

- [Redux](https://redux.js.org/)
- [Recoil](https://recoiljs.org/)
- [MobX](https://mobx.js.org/README.html)

Today we will go over a couple options for state management in Feliz. We will start with ``React.useState`` hook and also another very popular approached called **[elmish](https://elmish.github.io/elmish/)** using ```React.useElmish``` hook. I peronally really like the elmish approach, however they are not mutually exclusive. They can be used alone or together as you see fit.

## The Plan

For this demonstration, we will build a simple temperature converter form. You will be able to input a temperature in Celsius and get the equivalent in Fahrenheit, and vice versa. Let's start by setting up the model, which will represent the state for our form. First we will do it using ```useElmish``` and then we will re-implement it with ```useState```. Hopefully this will illustrate how to use them effectively.

## Elmish Approach

### Setting Up the Model

First let's create a new F# file called ``ElmishConverter.fs``. 

Our state needs to store both the current temperature and the unit it represents. Here's what our state will look like:
```fsharp
module ElmishConverter

type TemperatureScale =
    | Celsius
    | Fahrenheit

    member this.Value =
        match this with
        | Celsius -> nameof Celsius
        | Fahrenheit -> nameof Fahrenheit

type State =
    { Temperature: decimal
      Scale: TemperatureScale }

let init () =
    { Temperature = -20.0; Scale = Celsius }
```

Next, we define messages that represent all possible updates to our state:
```fsharp
type Msg =
    | UpdateTemperature of string
    | ChangeScale
```

### Implementing the Update Function
The update function will handle the transformation of our state in response to messages:

```fsharp
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
```

### Crafting the View

The view function will take our state and render it. We will perform the temperature conversion within the view for simplicity:

```fsharp
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
```
### Employing `useElmish`
With the state, messages, update, and view defined, we now hook them up by using the `useElmish` hook in our component:

```fsharp
[<ReactComponent>]
let TemperatureConverter () =
  let (state, dispatch) = React.useElmish (init, update)
  view state dispatch
```

Now add a reference to the package ``Feliz.UseElmish``.

```console
femto install Feliz.UseElmish ./src
```
Back in our ``Components/fs`` update the router to show our new page when we navigate to ``/#elmish-converter``:

```fsharp
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
                | [ "elmish-converter" ] -> ElmishConverter.TemperatureConverter()
                | otherwise -> Html.h1 "Not found" ] ] ]
```


And that's it! You now have a simple but interactive temperature converter built with F# and Feliz using the Elmish pattern. Although this is a small example, `useElmish` can be scaled to manage state for more complex components and applications while keeping your codebase organized and robust.

## State with ```useState```

Our state will be identical to the previous example provided:

```fsharp
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

let init =
  {
    Temperature = -20.0m
    Scale = Celsius
  }
```

We wont need the update function anymore, instead we will be dealing with all UI changes inside our component.

```fsharp
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
```
Add our new component to the router:

```fsharp
      React.router [
        router.onUrlChanged updateUrl
        router.children [
          match currentUrl with
          | [] -> Components.Home()
          | [ "elmish-converter" ] -> ElmishConverter.TemperatureConverter()
          | [ "converter" ] -> Converter.Converter()
          | otherwise -> Html.h1 "Not found"
        ]
      ]
```

![Converters](https://rasheedaboudblogstorage.blob.core.windows.net/blogs/converters.gif?sp=r&st=2024-01-05T19:36:21Z&se=2050-01-06T03:36:21Z&spr=https&sv=2022-11-02&sr=b&sig=sn18ulpkLtaS9fd5RYdv%2B8jC0gN3KuwN1QMLDzaAPck%3D)

Run the app with ```pnpm start``` and navigate to ```/#elmish-converter``` and ```/#converter``` respectively. You should see two identical pages.

In the final solution I will save both these approaches in separate files so you will have examples of both side by side.

### Conclusion

Throughout this series, we started with getting set up with Feliz and Fable, added environment variables, and constructed UI elements using Feliz.DaisyUI. With the addition of state management using `useElmish` and `useState`, we've laid a solid foundation for building powerful and complex web applications with F#. Remember that the magic of F# isn't just its succinctness; it's the robust functional programming paradigm it supports that enables us to build reliable applications.

As you can see both methods are , however I find anytime I'm building stateful components I prefer the elmish style. I find it easier to read and reason about. If however im just rendering out a bit of state I will use the `useState` hook. I also like that you can easily see all the possible state transitions in your update function, and that the F# compiler will help you if you add a new message type and forget to handle it.

If you want to see any specific topics covered in the future please open a github issue and up-vote it. Otherwise I'll just keep on building up our app to show off all the capabilities that F# has to offer.

Happy coding, and stay tuned for the next part of our series, where we'll dive deeper into Feliz/Fable by exploring how make api calls!
