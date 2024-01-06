namespace App

open Feliz
open Feliz.Router
open Fable.Core
open Fable.React

[<AutoOpen>]
module JsxHelpers =
    let inline toJsx (el: ReactElement) : JSX.Element = unbox el
    let inline toReact (el: JSX.Element) : ReactElement = unbox el


type Components =
    [<ReactComponent>]
    static member JsxCounter(props: {| count: int |}) =
        JSX.jsx
            $"""
            <div style={{{{marginTop:'10px'}}}} >From JSX: {props.count}</div>
        """
        |> toReact

    /// <summary>
    /// The simplest possible React component.
    /// Shows a header with the text Hello World
    /// </summary>
    [<ReactComponent>]
    static member HelloWorld() = Html.h1 "Hello World"

    /// <summary>
    /// A stateful React component that maintains a counter
    /// </summary>
    [<ReactComponent>]
    static member Counter() =
        let (count, setCount) = React.useState (0)

        Html.div
            [ Html.h1 count
              Html.button [ prop.onClick (fun _ -> setCount (count + 1)); prop.text "Increment" ]
              Components.JsxCounter({| count = count |}) ]

    /// <summary>
    /// A React component that uses Feliz.Router
    /// to determine what to show based on the current URL
    /// </summary>
    [<ReactComponent>]
    static member Router() =
        let (currentUrl, updateUrl) = React.useState (Router.currentUrl ())

        React.router
            [ router.onUrlChanged updateUrl
              router.children
                  [ match currentUrl with
                    | [] -> Html.h1 "Index"
                    | [ "hello" ] -> Components.HelloWorld()
                    | [ "counter" ] -> Components.Counter()
                    | otherwise -> Html.h1 "Not found" ] ]
