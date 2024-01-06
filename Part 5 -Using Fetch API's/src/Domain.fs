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
  /// An alternative to using promise is to just use an async block and pipe through to |> Async.StartImmediate to
  /// make it compatible with React.useEffect
  ///
  // let loadUsers setUsers () =
  //   async {
  //     let! response = fetch "https://random-data-api.com/api/v2/users?size=10" [] |> Async.AwaitPromise

  //     if response.Ok then
  //       let! users = response.json<User list> () |> Async.AwaitPromise
  //       setUsers (users)
  //   }
  //   |> Async.StartImmediate

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

  let loadUsers setUsers () =
    promise {
      let! response = fetch "https://random-data-api.com/api/v2/users?size=10" []

      if response.Ok then
        let! users = response.json<User list> ()
        setUsers (users)
    }
    |> Promise.start