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
    for user in props.users do
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