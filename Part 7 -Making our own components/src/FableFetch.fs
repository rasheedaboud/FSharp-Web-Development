module FableFetch

open Domain
open Feliz
open Feliz.DaisyUI
open Domain

[<ReactComponent>]
let FecthData () =

  let (users, setUsers) = React.useState ([])

  React.useEffect (User.loadUsers setUsers, [||])

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