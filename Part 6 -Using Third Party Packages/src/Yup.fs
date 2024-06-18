module Validation

open Fable.Core
open Browser
open Feliz

module Yup =
  /// We add a static and abstract member to support the method chaining we expect
  /// when using the yup object in javascript.
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
    abstract member validate: 'a -> JS.Promise<obj>

  /// Define our schema
  let schema =
    Yup.object (
      {| firstName =
          Yup
            .string()
            .min(2, "Too Short!")
            .max(50, "Too Long!")
            .required ("Value is required.")
         lastName =
          Yup
            .string()
            .min(2, "Too Short!")
            .max(50, "Too Long!")
            .required ("Value is required.")
         email = Yup.string().email("Invalid email.").required ("Value is required.") |}
    )

  ///Define object to validate
  let data =
    {| firstName = "Rasheed"
       lastName = "Aboud"
       email = "this is not an email" |}

  ///Validate and output result
  let validate () =
    promise {
      let! result = schema.validate (data)
      console.log (result)
    }
    |> Promise.start



module Formik =


  type SignUpFormProps<'a> = { onSubmit: 'a -> unit }

  [<ReactComponent>]
  let SignUpForm (props: SignUpFormProps<'a>) =
    JsInterop.import "Formik" "formik"
    JsInterop.import "Form" "formik"
    JsInterop.import "Field" "formik"

    let initialValues =
      {| firstName = ""
         lastName = ""
         email = "" |}

    let schema = Yup.schema

    JSX.jsx
      """
       <div className='m-10 p-5 w-1/2 rounded overflow-hidden shadow-lg'>
        <h1 className='m-5 uppercase font-black'>Signup</h1>
        <Formik
          initialValues={initialValues}
          validationSchema={schema}
          onSubmit={props.onSubmit}
        >
          {({ errors, touched }) => (
            <Form>

              <label htmlFor='firstName' className='text-sm'>First Name</label>
              <Field name="firstName" className='block w-full rounded-md border-0 py-1.5 pl-7 pr-20 text-gray-900 ring-1 ring-inset ring-gray-300 placeholder:text-gray-400 focus:ring-2 focus:ring-inset focus:ring-gray-500 sm:text-sm sm:leading-6'/>
              {errors.firstName && touched.firstName ? (
                <div className='text-red-500 text-xs mb-5'>**{errors.firstName}</div>
              ) : null}

              <label htmlFor='lastName' className='text-sm'>Last Name</label>
              <Field name="lastName" className='block w-full mt-5 rounded-md border-0 py-1.5 pl-7 pr-20 text-gray-900 ring-1 ring-inset ring-gray-300 placeholder:text-gray-400 focus:ring-2 focus:ring-inset focus:ring-gray-500 sm:text-sm sm:leading-6'/>
              {errors.lastName && touched.lastName ? (
                <div className='text-red-500 text-xs mb-5'>**{errors.lastName}</div>
              ) : null}

              <label htmlFor='email' className='text-sm'>Email</label>
              <Field name="email" type="email" className='peer block w-full mt-5 rounded-md border-0 py-1.5 pl-7 pr-20 text-gray-900 ring-1 ring-inset ring-gray-300 placeholder:text-gray-400 focus:ring-2 focus:ring-inset focus:ring-gray-500 sm:text-sm sm:leading-6' />
              {errors.email && touched.email ?
                <div className='peer-invalid text-red-500 text-xs'>**{errors.email}</div>
                : null}

              <button type="submit" className="btn btn-sm btn-primary mt-10">Submit</button>
            </Form>
          )}
        </Formik>
      </div>
    """