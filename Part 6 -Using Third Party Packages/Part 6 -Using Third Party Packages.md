Welcome back, intrepid developers and F# enthusiasts! In our web development odyssey with F#, Feliz, and Fable, we've been getting our hands dirty crafting interactive UIs, managing state elegantly, and integrating asynchronous data fetching to create responsive and dynamic applications. Our coding sails have caught a hearty wind, and we're cruising at remarkable speeds through the exciting seas of functional programming.

However, every application at some point requires a unique feature or a complex component that would take extensive time and resources to build from scratch. This is where the treasure trove of third-party libraries comes to our rescue. In the vast ecosystem of open-source contributions and pre-packaged bundles of code lies the potential to extend our application's capabilities without reinventing the wheel. But how do we bring these external riches into our type-safe F# shores? Fear not, for this is the quest we embark upon today!

In this post, I will guide you through the adventurous process of wrapping a third-party library using F#. Whether it's a charting library for data analysis, a slick UI component for a touch of finesse, or a complex algorithmic toolset, we'll learn together how to harness these external powers within our F# and Fable applications. We'll examine the path to creating type-safe bindings, maintaining the functional integrity of our code, and seamlessly incorporating JavaScript interop capabilities. By the end of this journey, you'll be well-versed in the art of library wrapping, ready to elevate your web apps to unparalleled heights of functionality and flair.

So, adjust your captain's hat, set the coordinates, and prepare to discover how to amplify your web development prowess with the integration of third-party libraries in F#. Let's get wrapping!

## The Plan

For this demonstration, we will wrap a validation library called [Yup](https://www.npmjs.com/package/yup#typescript-integration). For those of you not familiar,Yup is a schema builder for runtime value parsing and validation. Define a schema, transform a value to match, assert the shape of an existing value, or both. Yup schema are extremely expressive and allow modeling complex, interdependent validations, or value transformation.

Although there are [better](https://demystifyfp.gitbook.io/fstoolkit-errorhandling/)  options available to use as F# developers. A lot of people coming from react are familiar with Yup or Zod. So I thought this might be a good intro.

Once we have Yup ready to go we will wrap a couple [formik](https://formik.org/docs/overview) components and build a simple contact form.

Lets go!

## Prerequisites

Grab the code form [Part 5](). It should contain all the required bits to get going.

Here is a short list of the requirements to run the app:

- .Net 8
- Node ^18
- npm | pnpm | yarn
- [Visual Studio Code](https://code.visualstudio.com/) with [Ionide](http://ionide.io/)
- [Highlight templates in F#](https://marketplace.visualstudio.com/items?itemName=alfonsogarciacaro.vscode-template-fsharp-highlight)


## Yup

Lets dive right in and install the required packages.

```console
pnpm i yup
```

Next lets create a new F# file called, you got it ``Validation.fs`` to hold our example using yup. 

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
let FecthData () =

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

We create an async function ``loadUsers`` which returns a promise that contains a list of users. We then load the users in the state variable ``users``. We wrap this function in a ``useEffect`` hook with an empty array so that it is only ever called once.

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
          | [ "fable-fetch" ] -> FableFetch.FecthData()
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

Let'c create a file called `FelizDeferred.fs` and install the required nuget package.

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
let FecthData () =

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
          | [ "fable-fetch" ] -> FableFetch.FecthData()
          | [ "feliz-deferred" ] -> FelizDeferred.Deferred()
          | otherwise -> Html.h1 "Not found"
        ]
      ]
    ]
```

Run the app and check out the new page! It should render out exactly the same as out previous component.

## Summary

Hey there! In the latest chapter of our web development journey with F# using Feliz and Fable, we took a thrilling dive into the world of asynchronicity and data fetching. We explored how to make our web application come alive by reaching out to external services for fresh, dynamic data. I showed you two awesome methods for doing this: Fable.Fetch and React.useDeferred.

We started by setting sail with Fable.Fetch, installed the necessary packages, and created a nifty example to fetch random user data from an API. I walked you through creating a `loadUsers` async function wrapped in a `useEffect` hook to load the users once and populate our app's UI with the data received.

Then, I introduced the magical world of React.useDeferred. We created a new component, separated our fetching logic, and managed to render similar output to our Fable.Fetch example, with beautiful loading state management right out of the box!

Both methods were super easy and fun to implement, adding that connectedness and richness to our F# web application. In our next post, we'll raise the stakes even higher as we learn how to wrap third-party components into our app. It's going to be fantastic, and I can't wait to show you how!

Stay tuned, and keep it upbeat, as our adventure only gets more exciting from here!