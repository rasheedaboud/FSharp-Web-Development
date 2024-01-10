module Validation

open Fable.Core
open Fable.Core.JS

[<ImportAll("yup")>]
[<Interface>]
type Yup =
  static member object(schema: 'a) : Yup = jsNative
  static member string() : Yup = jsNative
  abstract member string: unit -> Yup

  static member min(?min: int, ?message: string) : Yup = jsNative
  abstract member min: ?min: int * ?message: string -> Yup

  static member max(?max: int, ?message: string) : Yup = jsNative
  abstract member max: ?max: int * ?message: string -> Yup

  static member required(?message: string) : Yup = jsNative
  abstract member required: ?message: string -> Yup

  static member email(?message: string) : Yup = jsNative
  abstract member email: ?message: string -> Yup

  abstract member validate: 'a -> Promise<obj>


/// We add a static and abstract member to support the method chaining we expect
/// when using the yup object in javascript.
/// Define our schema
let schema =
  Yup.object (
    {| firstName =
        Yup
          .string()
          .min(2, "Too Short!")
          .max(50, "Too Long!")
          .required ("Required.")
       lastName =
        Yup
          .string()
          .min(2, "Too Short!")
          .max(50, "Too Long!")
          .required ()
       email =
        Yup
          .string()
          .email("Invalid email.")
          .required ("Required.") |}
  )

///Define object to validate
let data =
  {| firstName = "Rasheed"
     lastName = "Aboud"
     email = "this is not an email" |}

///Validate and output result
let validate () =
  async {
    let! result = schema.validate (data) |> Async.AwaitPromise
    console.log (result)
  }
  |> Async.StartImmediate