Welcome back to the second part of our blog series on F# web development with Vite and Feliz. In the previous post, we set up our development environment and built a basic web application. Now, we will dive into the topic of environment variables and how they can change how our app's behave in different environments.

Environment variables can provide a way to store sensitive information, such as API keys or database connection strings, outside of our source code. This allows us to keep our secrets hidden and easily configure our app for different environments, such as development, testing, and production.

We will start by learning how to read environment variables in our F# code using the Vite build tool. Then, we will see how to store variables in .env files and access them in a type-safe manner. Additionally, we will explore the concept of F# conditional compilation and its role in controlling the behavior of our app based on different environments. 

By the end of this post, you will have a solid understanding of how to leverage environment variables and conditional compilation in your F# web applications, bringing greater flexibility, security, and configurability to your projects.

Let's dive in!

## Prerequisites

Grab the code form [Part 1](https://github.com/rasheedaboud/FSharp-Web-Development/tree/main/Part%201%20-Getting%20Started). It should contain all the required bits to get going.

Here is a short list of the requirements to run the app:

- .Net 8
- Node ^18
- npm | pnpm | yarn
- [Visual Studio Code](https://code.visualstudio.com/) with [Ionide](http://ionide.io/)
- [Highlight templates in F#](https://marketplace.visualstudio.com/items?itemName=alfonsogarciacaro.vscode-template-fsharp-highlight)

## Getting Started

First thing were going to do is introduce environment variables. Were going to need them eventually. You can read about how vite deals with .env files and variables [here](https://vitejs.dev/guide/env-and-mode.html). The short and sweet of it is you can add a ```.env``` file to your repo, and add any 'config' values that you may want to use during testing or development.

By placing any the following files in the **src** folder;

- ```.env```
- ```.env.local```
- ```.env.production```

You can control what get checked into source control and change how you app behaves in different environments. Although when you publish your app there are better / more secure ways of storing real secrets!

First things first we need a way to read the values we store in the ```.env``` files.

In JavaScript we could just do the following:

```js
import.meta.env.VITE_SOME_KEY
```

In F# just like in typescript if we want to access these environment variables in a type safe manner there is a bit of setup. I guarantee there is a much nicer / more elegant way of doing this but i find it works well for me.

In  ```/src``` directory create a new F# file, I call mine ```Env.fs``` and add the following code:

```fsharp
[<RequireQualifiedAccess>]
module Env

    open Fable.Core

    [<Emit("import.meta.env.VITE_KEY")>]
    let vite_key: string = jsNative
```

Next add the following to a file called ```.env``` located in the ```./src``` folder:

```env
VITE_KEY="SOME VALUE"
```

Similar to setting up a typescript definition file for your vite variables this will now give you access to the value of a variable called 'VITE_KEY'.

You can test this by printing the value to the console.

```Main.fs```

```fsharp
module Main

open Feliz
open App
open Browser.Dom

console.log(Env.vite_key)

let root = ReactDOM.createRoot (document.getElementById "feliz-app")
root.render (Components.Counter())
```
Which should output ```SOME VALUE``` to the dev console in your browser.

If you want some extra type safety you could do the following back in the ```Env.fs``` file.

```fsharp
[<RequireQualifiedAccess>]
module Env

open Fable.Core

[<RequireQualifiedAccess>]
module ViteKey =

    [<Literal>]
    let private key = "import.meta.env.VITE_KEY"

    [<Emit(key)>]
    let private vite_key_maybe: string option = jsNative

    let value =
        match vite_key_maybe with
        | Some envValue -> envValue
        | None -> failwith $"no env variable with name:{key}  found."
```

```fsharp
module Main

open Feliz
open App
open Browser.Dom
open Fable.Core.JsInterop

console.log(Env.ViteKey.value)

let root = ReactDOM.createRoot (document.getElementById "feliz-app")
root.render (Components.Home())
```
Instead of throwing an error you could also return whatever you want.

If you're curious about the weird ```[<Emit("import.meta.env.VITE_KEY")>]``` syntax. This a way to call arbitrary javascript methods from F#. Zaid made an excellent post about it back in 2017 that you can [here](https://medium.com/@zaid.naom/f-interop-with-javascript-in-fable-the-complete-guide-ccc5b896a59f). Dont worry too much we'll dig a little deeper into Javascript interop later on.

## F# Conditional Compilation

Another way of conditionally changing how you app behaves is using F# conditional compilation ```#if #else``` blocks in your source code. You can learn all about [compiler directives](https://learn.microsoft.com/en-us/dotnet/fsharp/language-reference/compiler-directives) from the microsoft docs.

Alfonso Garcia wrote an excellent blog post about how this topic called '[When Fable and F# bring you better tooling than JS](https://fable.io/blog/2022/2022-10-26-hot-reload.html)'. He gives some great examples of how using F# and .Net can actually give you a better dev experience than the native tooling available in the javascript ecosystem.

Here is a contrived example of how this would look:
```fsharp
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

```

## Summary

In this blog post, we explored how to use environment variables in our F# web application built with Vite and Feliz. We learned how to access environment variables defined in the .env files and store them in F# modules for type-safe access. We also looked at using F# conditional compilation to conditionally change the behavior of our app based on different environments.

By using environment variables, we can easily configure our app's behavior in different environments without hardcoding values. This allows for greater flexibility and security, as we can store sensitive information in private .env files that are not checked into source control.

Overall, leveraging environment variables in our F# web application helps us build more configurable and secure apps with a smoother development experience.

Stay tuned for Part 3 of this blog series, where we will explore how to integrate [Daisy UI](https://daisyui.com/docs/install/) into our F# web application. Daisy UI is a powerful UI library that provides ready-to-use components and styles for building beautiful user interfaces. We will see how to install and use Daisy UI within our Vite and Feliz setup, allowing us to easily create visually appealing and responsive web interfaces. Don't miss it!
