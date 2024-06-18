## TL;DR

In this post we expanded our F# web application's functionality with Feliz and Fable by exploring asynchronous HTTP requests to pull in live data from an external service. We walked through two methods: using Fable.Fetch along with promises and Fable.Promise for async calls, and using React.useDeferred for deferred data fetching with better loading state management. We created examples to fetch random user data from an API upon loading the page, showcasing loading states and data rendering in our app's UI. Both methods were straightforward, enhancing our app with dynamic, real-time content. Next, we'll explore how to integrate third-party components into our F# web app.

## Background

Welcome back to our adventurous journey through the world of web development with F# using Feliz and Fable! In our previous chapters, we've established a sturdy foundation of our F# web application, crafting interactive UI components with state management techniques. But our application isn't an isolated island; it needs to communicate with the vast and dynamic world of the internet. It's time to broaden our horizons and venture into the realm of asynchronicity and data fetching.

In this post, we'll elevate our application's capabilities by learning how to reach out to external services using promises and the Fetch API. Our focus will be on how to make asynchronous HTTP requests that breathe life into our app with fresh, live data from various sources. We'll examine the practical uses of promises in F#, how to handle responses and potential errors gracefully, and how to integrate these asynchronous patterns within our functional program design.

As we embark on this path, we’ll get a closer look at employing typed HttpClient instances and integrate these with our Elmish components, ensuring type safety and functional clarity across our application. By the end of this guide, you will have the tools to make your F# web applications not only interactive but also connected and data-rich.

Prepare to unlock the full potential of your web applications with F# and Fable!

## The Plan

For this demonstration, we will load some data from a third party using browser fetch API to get some random data from [random-data-api](https://random-data-api.com/api/v2/users). I will show you two ways of completing the task

1. [Fable.Fetch](https://github.com/fable-compiler/fable-fetch)
2. [React.useDeferred](https://zaid-ajaj.github.io/Feliz/#/Hooks/UseDeferred)

Both are dead simple to use, each having it's own pro and cons.

## Prerequisites

Grab the code form [Part 4](https://github.com/rasheedaboud/FSharp-Web-Development/tree/main/Part%204%20-State%20Management). It should contain all the required bits to get going.

Here is a short list of the requirements to run the app:

- .Net 8
- Node ^18
- npm | pnpm | yarn
- [Visual Studio Code](https://code.visualstudio.com/) with [Ionide](http://ionide.io/)
- [Highlight templates in F#](https://marketplace.visualstudio.com/items?itemName=alfonsogarciacaro.vscode-template-fsharp-highlight)

## Fable.Fetch

Lets dive right in and install the required nuget packages.

```console
femto install Fable.Fetch ./src
femto install Fable.Promise ./src
```

Next lets create a new F# file called, you got it ``FableFetch.fs`` to hold our example using the fetch api and ``Domain.fs`` where we will create a subtype of the field return by the API we will be using.

``Domain.fs``

```fsharp
module Domain

type User =
  { id: int
    first_name: string
    last_name: string
    email: string }
```

``FableFetch.fs``

```fsharp
module FableFetch

open Domain
open Feliz
open Feliz.DaisyUI
open Fetch

[<ReactComponent>]
let FetchData () =

  let (users, setUsers) = React.useState ([])

  let loadUsers () =
    promise {
      let! response = fetch "https://random-data-api.com/api/v2/users?size=10" []
      if response.Ok then
        let! users = response.json<User list> ()
        setUsers (users)
    }
    //we need this to make useEffect happy...
    |> Promise.start


  React.useEffect (loadUsers, [||])

  Html.div [
    if users = [] then
      Html.div [
        prop.className "absolute right-1/2 bottom-1/2  transform translate-x-1/2 translate-y-1/2"
        prop.children [
          Daisy.loading [
            loading.spinner
            loading.lg
          ]
        ]
      ]

    else
      for user in users do
      Daisy.card [
        card.bordered
        prop.children [
            Daisy.cardBody [
                Daisy.cardTitle (sprintf "%s %s" user.first_name user.last_name)
                Html.p user.email
            ]
        ]
      ]
  ]
```

Pretty straight forward stuff, if you squint a little bit its barely noticeable that this is not a standard javascript react component.

We create a function ``loadUsers`` which returns a promise that contains a list of users. We then load the users in the state variable ``users``. We wrap this function in a ``useEffect`` hook with an empty array so that it is only ever called once.

Make the following changes to the `Components.fs` file:

```fsharp
  [<ReactComponent>]
  static member Home() =
    Html.div [
      prop.className "container mx-auto"
      prop.children [
        Html.h1 "Home Page"
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
      ]
    ]

  [<ReactComponent>]
  static member Router() =
    let (currentUrl, updateUrl) = React.useState (Router.currentUrl ())

    Html.div [
      Components.NavBar()
      React.router [
        router.onUrlChanged updateUrl
        router.children [
          match currentUrl with
          | [] -> Components.Home()
          | [ "elmish-converter" ] -> ElmishConverter.TemperatureConverter()
          | [ "converter" ] -> Converter.Converter()
          | [ "fable-fetch" ] -> FableFetch.FetchData()
          | otherwise -> Html.h1 "Not found"
        ]
      ]
    ]
```

Run the app and click on button `Fable.Fetch`. You should see a loading spinner for a second followed by a list of users.

![Fable Fetch](https://rasheedaboudblogstorage.blob.core.windows.net/blogs/fablefetchclip.gif?sp=r&st=2024-01-06T04:17:17Z&se=2050-01-06T12:17:17Z&spr=https&sv=2022-11-02&sr=b&sig=8KFjMliIEfjo7xqeE9DrL39MIJGk4PS2qATkdKaYB%2BQ%3D)

[Fable.Fetch](https://github.com/fable-compiler/fable-fetch) is incredibly easy to use. Just remember that this example has no error handling.

In the repo I give another example using an `async` computation block instead of a `promise`  however the end result is the same. If you prefer not to pull in Fable.Promise package then using `async` work fine.

## Feliz.UseDeferred

Let's create a file called `FelizDeferred.fs` and install the required nuget package.

```console
femto install Feliz.UseDeferred ./src
```

We can re-use our domain model from previous however we will quickly pull out the logic to load users into its own module, this is 100% not required its just something i like to do.

```fsharp
module Domain
open Fable.Core
open Fetch

type User =
  { id: int
    first_name: string
    last_name: string
    email: string }

[<RequireQualifiedAccess>]
module User =
  //Required for useDeferred
  let loadUsersAsync =
    async {
      let! response =
        fetch "https://random-data-api.com/api/v2/users?size=10" []
        |> Async.AwaitPromise

      if response.Ok then
        let! users = response.json<User list> () |> Async.AwaitPromise
        return users
      else
        return []
    }
  //Required for out Fetch example
  let loadUsers setUsers () =
    promise {
      let! response = fetch "https://random-data-api.com/api/v2/users?size=10" []

      if response.Ok then
        let! users = response.json<User list> ()
        setUsers (users)
    }
    |> Promise.start
```

Update our `FetchData.fs` to use our new `loadUsers` function:

```fsharp
open Domain

[<ReactComponent>]
let FetchData () =
  let (users, setUsers) = React.useState ([])
  React.useEffect (User.loadUsers setUsers, [||])

  ... 
```

Now we can go back to our `FelizDeferred` component and finish it off:

```fsharp
module FelizDeferred

open Feliz
open Feliz.UseDeferred
open Feliz.DaisyUI
open Domain

let Spinner () =
  Html.div [
    prop.className "absolute right-1/2 bottom-1/2  transform translate-x-1/2 translate-y-1/2"
    prop.children [
      Daisy.loading [
        loading.spinner
        loading.lg
      ]
    ]
  ]

let Error (message: string) =
  Daisy.card [
    card.bordered
    prop.children [
      Daisy.cardBody [
        Daisy.cardTitle "An error occurred!"
        Html.p message
      ]
    ]
  ]

let UserCard (props: {| users: User list |}) =
  Html.div [
    for user in users do
      Daisy.card [
        card.bordered
        prop.children [
          Daisy.cardBody [
            Daisy.cardTitle (sprintf "%s %s" user.first_name user.last_name)
            Html.p user.email
          ]
        ]
      ]
  ]

[<ReactComponent>]
let Deferred () =
  let data = React.useDeferred (User.loadUsersAsync, [||])

  match data with
  | Deferred.HasNotStartedYet -> Html.none
  | Deferred.InProgress -> Spinner()
  | Deferred.Failed error -> Error error.Message
  | Deferred.Resolved content -> UserCard {| users = content |}
```

Add button to navigate in our `Home` component

```fsharp
Daisy.button.a [
    prop.href "/#feliz-deferred"
    prop.text "Feliz.Deferred"
]
```

And update our router with our new page:

```fsharp
  [<ReactComponent>]
  static member Router() =
    let (currentUrl, updateUrl) = React.useState (Router.currentUrl ())

    Html.div [
      Components.NavBar()
      React.router [
        router.onUrlChanged updateUrl
        router.children [
          match currentUrl with
          | [] -> Components.Home()
          | [ "elmish-converter" ] -> ElmishConverter.TemperatureConverter()
          | [ "converter" ] -> Converter.Converter()
          | [ "fable-fetch" ] -> FableFetch.FetchData()
          | [ "feliz-deferred" ] -> FelizDeferred.Deferred()
          | otherwise -> Html.h1 "Not found"
        ]
      ]
    ]
```

Run the app and check out the new page! It should render out exactly the same as out previous component.

One of my favorite parts of using this method for data fetching is it forces me to consider and handle all the loading states and tends to help give a better user experience. I typically break out most of the states into re-usable components, that way I ony need to implement the case where data has loaded successfully.

## Summary

Hey there! In the latest chapter of our web development journey with F# using Feliz and Fable, we took a thrilling dive into the world of asynchronicity and data fetching. We explored how to make our web application come alive by reaching out to external services for fresh, dynamic data. I showed you two awesome methods for doing this: Fable.Fetch and React.useDeferred.

We started by setting sail with Fable.Fetch, installed the necessary packages, and created a nifty example to fetch random user data from an API. I walked you through creating a `loadUsers` async function wrapped in a `useEffect` hook to load the users once and populate our app's UI with the data received.

Then, I introduced the magical world of React.useDeferred. We created a new component, separated our fetching logic, and managed to render similar output to our Fable.Fetch example, with beautiful loading state management right out of the box!

Both methods were super easy and fun to implement, adding that connectedness and richness to our F# web application. In our next post, we'll raise the stakes even higher as we learn how to wrap third-party components into our app. It's going to be fantastic, and I can't wait to show you how!

Stay tuned, and keep it upbeat, as our adventure only gets more exciting from here!
