# Crafting a Custom ComboBox in F# with DaisyUI and TailwindCSS

Welcome back, intrepid developers and F# enthusiasts! After taking some time off to focus on a new job. I thought it was time to continue our journey. 

Today, we’ll venture into the exciting realm of user interface components—specifically, crafting a custom ComboBox. Leveraging the elegance of DaisyUI and the flexibility of TailwindCSS, we’ll create a ComboBox that is both stylish and functional.

A ComboBox (or dropdown) is a fundamental UI element that allows users to select an item from a list of options or input their choice directly. In this tutorial, we’ll guide you through setting up a ComboBox component step-by-step using F# within our static type-safe environment.

So, polish your keyboards and get ready to dive in!

## Prerequisites

Ensure you have the following setup ready before we start:

- .Net 8
- Node ^18
- npm | pnpm | yarn
- [Visual Studio Code](https://code.visualstudio.com/) with [Ionide](http://ionide.io/)
- [Highlight templates in F#](https://marketplace.visualstudio.com/items?itemName=alfonsogarciacaro.vscode-template-fsharp-highlight)

## Setting Up

Begin by creating a new F# file called `ComboBox.fs` where we'll be developing our ComboBox component. Make sure you have TailwindCSS and DaisyUI installed and correctly configured in your project, as these libraries will provide us with the necessary styling and components.

If you haven't already, install the required packages:

```bash
pnpm i tailwindcss daisyui
```

Configure `postcss.config.js` to include TailwindCSS and DaisyUI:

```js
module.exports = {
  plugins: {
    tailwindcss: {},
    autoprefixer: {},
    daisyui: {},
  },
}
```

And set up your `tailwind.config.js`:

```js
module.exports = {
  content: ["./src/**/*.{fs,fsx}", "./public/index.html"],
  theme: {
    extend: {},
  },
  plugins: [require("daisyui")],
}
```

Set up Tailwind directives in your CSS file (`styles.css`):

```css
@tailwind base;
@tailwind components;
@tailwind utilities;
```

## Creating the ComboBox Component

Let's outline our new `ComboBox.fs`:

```fsharp
module ComboBox
open Feliz

type ComboBoxProps<'a> =
  { data: 'a []
    label: string
    palceholder: string
    selected: string -> unit }

[<ReactComponent>]
let ComboBox (props: ComboBoxProps<'a>) =
  let input, setInput = React.useState ("")
  let selectedValue, setSelectedValue = React.useState ("")
  let initial, setInitial = React.useState (props.data)
  let data, setData = React.useState (props.data)
  let hasFocus, setHasfocus = React.useState (false)
  let activeSuggestionIndex, setActiveSuggestionIndex = React.useState (0)

  React.useEffect ((fun () -> props.selected (selectedValue)), [| box selectedValue |])

  React.useEffect (
    (fun () ->
      if System.String.IsNullOrEmpty(input) then
        setData (initial)
      else
        let data =
          initial
          |> Array.filter (fun (x: string) -> x.ToLower().Contains(input.ToLower()))

        if data.Length <= 0 then
          setData (initial)
          setInput ("")
        else
          setData (data)),
    [| box input |]
  )

  let handleKeyDown (evt: Browser.Types.KeyboardEvent) =
    setHasfocus (true)

    if evt.key = "ArrowUp" then
      if activeSuggestionIndex = 0 then
        ()
      else
        setActiveSuggestionIndex (activeSuggestionIndex - 1)

    elif evt.key = "ArrowDown" then

      if activeSuggestionIndex = data.Length - 1 then
        ()
      else
        setActiveSuggestionIndex (activeSuggestionIndex + 1)
    elif evt.key = "Enter" then
      setInput (data[activeSuggestionIndex])
      setSelectedValue (data[activeSuggestionIndex])
      setActiveSuggestionIndex (0)
      setHasfocus (false)

  Html.div [
    prop.className "form-control"
    prop.children [
      Html.div [
        prop.className "dropdown"
        prop.children [
          Html.div [
            prop.className "label"
            prop.children [
              Html.span [
                prop.className "label-text font-bold"
                prop.text props.label
              ]
            ]
          ]
          Html.input [
            prop.classes [
              "input input-sm"
              "input-bordered"
              "w-full"
            ]
            prop.type' "search"
            prop.placeholder props.palceholder
            prop.value input
            prop.onChange (fun x -> setInput (x))
            prop.onFocus (fun _ -> setHasfocus (true))
            prop.onKeyDown handleKeyDown
          ]
          if hasFocus then
            Html.ul [
              prop.classes [
                "dropdown-content"
                "z-[1]"
                "menu"
                "p-2"
                "shadow"
                "bg-base-100"
                "rounded-box"
                "w-full"
                "max-h-80"
                "flex-nowrap"
                "overflow-auto"
              ]
              prop.tabIndex 0
              prop.children [

                for i = 0 to data.Length do
                  Html.li [
                    prop.id data[i]
                    prop.key i
                    prop.children [
                      Html.a [
                        prop.className (
                          if activeSuggestionIndex = i then
                            "active"
                          else
                            ""
                        )
                        prop.onClick (fun _ ->
                          setInput (data[i])
                          setSelectedValue (data[i])
                          setHasfocus (false))
                        prop.text data[i]
                      ]
                    ]
                  ]
              ]
            ]
        ]
      ]
    ]
  ]
```

### Explanation

1. **State Management**: We use `React.useState` to manage the selected option, initial data, filtered data etc.
2. **Event Handling**: we use the `onClick` of the individual `<li>` elements to update the selected option and current input values if user uses thier mouse or touch to select a value. Whereas we use the `onChange` and `onFocus` and `onKeyDown` events to filter and update the dropdown when user is using a keyboard. Together they allow a user to easily filter and select a value. 

By setting `prop.value input` we tell the browser to show a clear icon, that allows the user to quickly clear value in the text input.

![alt text](https://rasheedaboudblogstorage.blob.core.windows.net/blogs/dropdown_clear.png?sp=r&st=2024-06-17T23:49:50Z&se=2099-06-18T07:49:50Z&spr=https&sv=2022-11-02&sr=b&sig=ThSMizywiZTkS5jHZSNzQXiGIu%2BGJjiQfDqEo1uoeQc%3D)

3. **DaisyUI Dropdown Styling**:  As we loop through each item in the dropdown list we compare the index to the selected value index and conditionally apply `active` class to highlight the selected `<li>` item.
4. **Dynamic Option Rendering**: We loop through the provided options and render each as a clickable item within the dropdown.

### Integrating ComboBox in Your App

Now that we've created our `ComboBox` component, let’s integrate it into our main application. We'll add a new page that utilizes this ComboBox.

Update your `Components.fs` with a new `ComboBox` component:

```fsharp
module Components

open ComboBox

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
                // we can bond to this function to whatever we need to with the output
                selected = fun args -> console.log (args);
                palceholder = "Select a Country";
                //We'll add a list of countries to use with our example
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
```

### Run the App

With all the pieces in place, run your application and navigate to the ComboBox page. You should see a beautifully rendered ComboBox using DaisyUI and TailwindCSS styles, allowing users to select from the provided options or input their value.

```bash
pnpm start
```

Open your browser and navigate to the ComboBox page. You should see the ComboBox working seamlessly, allowing you to select from the list and displaying the selected option below.

![combo box in action](https://rasheedaboudblogstorage.blob.core.windows.net/blogs/combobox.gif?sp=r&st=2024-06-17T23:58:26Z&se=2099-06-18T07:58:26Z&spr=https&sv=2022-11-02&sr=b&sig=MuLkUGEFCUQzlXqbZB6IPohhE9W8%2BgStogzer19iqx8%3D)

## Summary

In this post, we delved into the world of custom UI components by crafting a ComboBox using F#, DaisyUI, and TailwindCSS. We defined our ComboBox component with state management and event handling, styled it using DaisyUI components, and seamlessly integrated it into our existing application.

I also thought I would pass along a few tools I find very useful when making components in feliz.

- [Typescript to Fable](https://fable.io/ts2fable/0) this is an amazing tool that can take typescript definitions and generate valid F#. There is a new tool that looks amazing [glutinum](https://github.com/glutinum-org/Glutinum), I have not had the opportunity to play with it yet but I hope to very soon.

- [Html2Feliz](https://thisfunctionaltom.github.io/Html2Feliz/) is another very useful tool that can convert any valid html into Feliz syntax. This make creating new components effortless.


This journey through creating a ComboBox showcases the power of combining functional programming with modern web development tools, providing a clean, type-safe, and stylish user experience. Stay tuned for more explorations and enhancements as we continue our adventure in building robust web applications with F# and its vibrant ecosystem!

Until next time, happy coding!