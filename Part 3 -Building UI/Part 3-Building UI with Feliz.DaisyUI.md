## TL;DR

In this part of the series, we look into integrating Feliz.DaisyUI into an F# web app to leverage its variety of beautiful UI components based on Tailwind CSS and DaisyUI. The process includes installing the Feliz.DaisyUI library, configuring it, and updating the app components to use Feliz.DaisyUI's functional programming approach for creating a visually appealing user interface. We also walk through the setup of Tailwind CSS and the DaisyUI plugin, importing CSS styles, and making some adjustments to the app's components for a better UI structure.

## Background

In this third part of the series, we will explore how to integrate Feliz.DaisyUI into our web app, [Feliz.DaisyUI](https://dzoukr.github.io/Feliz.DaisyUI/#/) is a library that provides a set of UI components and utilities based on the DaisyUI framework, which is built on top of Tailwind CSS.

By integrating Feliz.DaisyUI, we can easily create beautiful and responsive UI components in our F# web app. We'll be able to leverage the power of Feliz's functional programming approach and DaisyUI's utility-focused CSS classes to create stunning UIs with minimal effort.

In this post, we'll walk through the steps to install and configure Feliz.DaisyUI in our project. We'll also explore how to use different components provided by Feliz.DaisyUI to build a responsive and visually appealing user interface.

Let's go!

## Prerequisites

Grab the code form [Part 2](https://github.com/rasheedaboud/FSharp-Web-Development/tree/main/Part%202%20-Environment%20Variables). It should contain all the required bits to get going.

Here is a short list of the requirements to run the app:

- .Net 8
- Node ^18
- npm | pnpm | yarn
- [Visual Studio Code](https://code.visualstudio.com/) with [Ionide](http://ionide.io/)
- [Highlight templates in F#](https://marketplace.visualstudio.com/items?itemName=alfonsogarciacaro.vscode-template-fsharp-highlight)

## Daisy UI

In the ```./src``` folder run:

```console
femto install  Feliz.DaisyUI
```

Then head over to ```Components.fs``` and change it tp the following:

```fsharp
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
  static member Home() =
    Daisy.navbar
      [ prop.className "mb-2 shadow-lg bg-neutral text-neutral-content rounded-box"
        prop.children
          [ Html.div
              [ prop.className "flex-none"
                prop.children
                  [ Daisy.button.button
                      [ button.square
                        button.ghost
                        prop.children
                          [ Html.i [ prop.className "fa-solid fa-heart" ++ color.textSuccess ] ] ] ] ]
            Html.div
              [ prop.className "flex-1 px-2 mx-2"
                prop.children [ Html.span [ 
                  prop.className "text-lg font-bold"; 
                  prop.text "With one icon" ] ] ] ] ]

  [<ReactComponent>]
  static member Router() =
      let (currentUrl, updateUrl) = React.useState (Router.currentUrl ())

      React.router
        [ router.onUrlChanged updateUrl
          router.children
            [ match currentUrl with
              | [] -> Components.Home()
              | otherwise -> Html.h1 "Not found" ] ]

```

Make sure you change ```Main.fs``` to account for the change in our components;

```fsharp
module Main

open Feliz
open App
open Browser.Dom

let root = ReactDOM.createRoot (document.getElementById "feliz-app")
root.render (Components.Home())
```

The next few steps have alot of moving parts. If you ever get frustrated rember you can always just import the styles you from CDN. Just be aware you app size will be larger.

1. [Install Tailwind](https://tailwindcss.com/docs/guides/vite). This should be done in root of the app not in the src folder by running the following:

```console
pnpm install -D tailwindcss postcss autoprefixer
npx tailwindcss init -p
```
Then we need to configure our template paths luckily the defaults will work just fine:

```js
export default {
  content: ["./index.html", "./src/**/*.{js,ts,jsx,tsx}"],
  theme: {
    extend: {},
  },
  plugins: [],
};
```

2. [Install Daisy UI Plugin](https://daisyui.com/docs/install/) and add it to the ```tailwind.config.js``` like so:

```console
npm i -D daisyui@latest
```

head over to your ```tailwind.config.js``` file and:

```js
module.exports = {
  //...
  plugins: [require("daisyui")],
}
```

3. Add a ```main.css``` in src folder and add all tailwind styles:

```css
@tailwind base;
@tailwind components;
@tailwind utilities;
```

Make sure to import the styles by adding the following to your ```Main.fs```:

```fsharp
open Fable.Core.JsInterop
importAll "./main.css"
```

Last but not least add the following to the head of your ```index.html```:

```html
   <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.1/css/all.min.css" integrity="sha512-DTOQO9RWCH3ppGqcWaEA1BIZOC6xxalwEsw9c2QQeAIftl+Vegovlnee1c9QX4TctnWMn13TZye+giMm8e2LwA==" crossorigin="anonymous" referrerpolicy="no-referrer" />
```
Re-start the app with ```pnpm start```, and hopefully at this point you see the following:

![daisy ui app bar](https://rasheedaboudblogstorage.blob.core.windows.net/blogs/appbar.png?sp=r&st=2024-01-05T17:20:35Z&se=2050-01-06T01:20:35Z&spr=https&sv=2022-11-02&sr=b&sig=tSBZX%2BAVNZ6qAKPOzPY5IgGYWpUFRJsGu42eFhk2zio%3D)

If you get stuck please create an issue in the repo and I will try and help you out.

Lets update the UI a little bit so that we're ready for the next part in this series.

Back in ```Components.fs```

```fsharp
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

```
Then change which component get loaded initially in ```Main.fs```:

```fsharp
open Feliz
open App
open Browser.Dom
open Fable.Core.JsInterop

importAll "./main.css"
let root = ReactDOM.createRoot (document.getElementById "feliz-app")
root.render (Components.Router())
```

There now our app shell is perfect! We have a navbar that will persist between page changes and a nice home page to usto start building our app.

![daisy ui app bar](https://rasheedaboudblogstorage.blob.core.windows.net/blogs/appbar2.png?sp=r&st=2024-01-05T17:21:18Z&se=2050-01-06T01:21:18Z&spr=https&sv=2022-11-02&sr=b&sig=U%2B8VKyVy%2FNrh5IzeFwoZ8%2BdzFUO92MhpI7Fptnc43OI%3D)


## Summary

In this post, we integrated Feliz.DaisyUI into our F# web app. We installed and configured Feliz.DaisyUI, a library that provides a set of UI components and utilities based on the DaisyUI framework. We also explored how to use different components provided by Feliz.DaisyUI to build a responsive and visually appealing user interface. By leveraging the power of Feliz's functional programming approach and DaisyUI's utility-focused CSS classes, we were able to create stunning UIs with minimal effort.

In the next part of this series, we will be taking our UI building skills a step further by creating a UI for performing a series of unit conversions. We'll explore how to use the components provided by Feliz.DaisyUI and integrate them with our application logic to create an interactive and dynamic UI for performing unit conversions. Stay tuned for the next part where we dive deeper into creating a functional and user-friendly UI for our app!
