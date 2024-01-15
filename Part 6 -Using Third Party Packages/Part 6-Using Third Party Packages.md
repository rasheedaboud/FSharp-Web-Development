
## TL;DR 

In this post we learned how to wrap a third-party JavaScript library, Yup, in an F# application using Fable. This enables us to utilize Yup's schema builder for runtime value parsing and validation within our F# code. We also installed the Formik library, used JSX for importing Formik components, and styled them with Tailwind CSS. We then integrated a reactive form in our `Components.fs` that leverages Yup for validation and Formik for form handling, allowing us to validate user input conveniently in our F# web application.

## Background

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

If you look at the shape of the [yup](https://www.npmjs.com/package/yup) object you can see how we will want to be able to call static and instance members to create a schema object.

Here is what that will look lke in F#:

```fsharp
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
```

This more or less replicates the interface we need to use the example listed in their docs. We can now use this to build a schema object and validate objects.

We use the `[<ImportAll("yup")>]` to replicate `import * as Yup from 'yup` in javascript. The calls to `jsNative` you see, mean take whatever you give me add pass it through to native javascript. I would strongly encourage you to read the [fable docs](https://fable.io/docs/javascript/features.html#jsnative) on JS interop as well as [this](https://medium.com/@zaid.naom/f-interop-with-javascript-in-fable-the-complete-guide-ccc5b896a59f) blog post by [Zaid](https://github.com/Zaid-Ajaj), both are invaluable learning resources.

I also found it very useful to dig through some of the open source packages to see how the authors use some of Fables features to wrap and call javascript.

Now that we have defined and imported the necessary javascript we can use this interface to define our schema:

```fsharp
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
```

If you log the results of the `validate()` call in the console you will see that we get an error:

```console
Uncaught (in promise) ValidationError: Invalid email.
    at createError (index.esm.js:304:21)
    at handleResult (index.esm.js:321:104)
    at validate (index.esm.js:344:5)
    at StringSchema.runTests (index.esm.js:709:7)
    at index.esm.js:664:12
    at nextOnce (index.esm.js:695:7)
    at finishTestRun (index.esm.js:714:11)
    at handleResult (index.esm.js:321:124)
    at validate (index.esm.js:344:5)
    at StringSchema.runTests (index.esm.js:709:7)
```

Success! We now have the capability to use this anywhere on our F# code to validate objects.

At this point I want to point out there are much more powerfull validation options available in F#, one for example is [FsToolkit.ErrorHandling](https://demystifyfp.gitbook.io/fstoolkit-errorhandling). Composition IT wrote an excellent blog post about how to use it [here](https://www.compositional-it.com/news-blog/validation-with-f-5-and-fstoolkit/).

All I was trying to illustrate is how easy it is to pull in a react library from the community and use it.

The next component I would like to bring in if a form validator called [Formik](https://formik.org/). I've used this for years in react as a declarative form validation library. This time however we will just use a JSX component for the import along with a bit of tailwind to spruce up the generic formik fields.

First add npm package.

```console
pnpm i formik
```

Next lets create a new module in the `Validation.fs` file.

```fsharp
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
```

Ok that was alot of code! Let's break it down.

First thing I did was create `type SignUpFormProps<'a> = { onSubmit: 'a -> unit }`, this will allow us to pass in a function from outside the form to do something with values after validation.

Next we create a `SignUpForm` component and import the required items from the `formik` library. In this case `Formik,Form` and `Field`. Using an example copied right out the Formik docs,  we paste the JSX directly into our string template. Although it does not come through well in this markdown we do get code suggestions thanks to [Highlight templates in F#](https://marketplace.visualstudio.com/items?itemName=alfonsogarciacaro.vscode-template-fsharp-highlight). You will have to do some formatting to get the JSX to look ok, but thats a very small price to pay.

Next we create our `initialValues` and bring in our `schema` we defined in our Yup module, and we pass these along with our `onSubmit` handler to Formik.

I also added some tailwind styles to the html, I do not consider myself a UI/UX export but I think it looks ok ish.

With all this wired up we now have a working form we can use to validate a users information.

Let's head back to our good old `Components.fs` and update the `Home` component:

```fsharp
  [<ReactComponent>]
  static member Home() =

    let onSubmit (values) =
      let data = Fable.Core.JS.JSON.stringify (values)
      Browser.Dom.window.alert (data)

    Html.div
      [ prop.className "container mx-auto"
        prop.children
          [ Html.h1 "Home Page"
            Daisy.button.a [ prop.href "/#converter"; prop.text "Converter" ]
            Daisy.button.a [ prop.href "/#elmish-converter"; prop.text "Elmish Converter" ]
            Daisy.button.a [ prop.href "/#fable-fetch"; prop.text "Fable.Fetch" ]
            Daisy.button.a [ prop.href "/#feliz-deferred"; prop.text "Feliz.Deferred" ]
            Formik.SignUpForm { onSubmit = onSubmit } |> toReact ] ]
```

Run the app with `npm start`, and voila! We have a react form using Yup and Formik!

![Formik Example](https://rasheedaboudblogstorage.blob.core.windows.net/blogs/formikclip.gif?sp=r&st=2024-01-14T23:48:40Z&se=2050-01-15T07:48:40Z&spr=https&sv=2022-11-02&sr=b&sig=5uCKKxo6ottlwTJOzSJsDReLqoNzcoTEoFzARNZhcjM%3D)

## Summary

In this exploration, we've ventured into the realm of integrating third-party JavaScript libraries into an F# web application powered by Feliz and Fable. We specifically focused on wrapping a JavaScript validation library called Yup and a form management library named Formik, both popular within the React ecosystem.

To begin, we installed Yup through npm and created an F# interface to define the methods and properties we required from the Yup library. We used Fable's JS interop features to map the Yup validation schema to F# types, ensuring a fluent and type-safe experience when defining validation rules.

We then installed Formik and directly imported its JSX components (Formik, Form, and Field) into our F# codebase. We crafted a `SignUpForm` component in F# that employed Formik for form handling, used Yup for validation, and added Tailwind CSS for styling. We also defined `SignUpFormProps<'a>` in F# to allow the passing of an external `onSubmit` function to handle form submissions.

Finally, we integrated the `SignUpForm` into our existing `Components.fs` file, providing an `onSubmit` function that utilizes browser alerts to display the result of form submissions.

Through this journey, we've demonstrated the power and flexibility of F#, Feliz, and Fable in bridging the gap between the robust F# functional programming paradigm and the rich JavaScript ecosystem, enabling developers to leverage the best of both worlds in their web applications.
