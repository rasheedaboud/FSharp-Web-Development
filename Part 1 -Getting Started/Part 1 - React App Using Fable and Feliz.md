Hello everyone! As someone who has been using F# for several years now, I have decided to give back to the amazing community that has helped me so much during my journey.

To assist those who are interested in exploring F# for web development, I have created a series of repositories that aim to provide useful resources. Over the past two years, my preferred web technology stack has been Fable-Feliz for the frontend and F# Azure Functions for the backend.

Personally, I have developed a deep admiration for F# and Fable, as well as the various community packages that have flourished around them. F# alone has completely transformed the way I approach and write code, greatly enhancing my overall programming skills. Combining F# with Fable has granted me the luxury of type safety in the browser, using a language and toolset that I truly adore.

Through this series of repositories, I hope to demonstrate how effortless it is to get started with F# and how it can significantly boost your productivity as a developer.

Remember, if you ever find yourself encountering difficulties, please don't hesitate to reach out or open an issue in the repository. I will do my best to assist you as promptly as possible.

In this first part of the series, I will focus on guiding you through the process of setting up and running Feliz, my preferred UI library, for building Fable Apps. I will also quickly show you an alternative way of creating react components using a new Fable feature JSX components. 

## Prerequisites

- .Net 8 | .Net 7
- Node ^18
- npm (I prefer pnpm), yarn work too
- [Visual Studio Code](https://code.visualstudio.com/) with [Ionide](http://ionide.io/)
- [Highlight templates in F#](https://marketplace.visualstudio.com/items?itemName=alfonsogarciacaro.vscode-template-fsharp-highlight)

## Getting Started

First things first we will install the Feliz project template. A guide can be found on the [Feliz Site](https://zaid-ajaj.github.io/Feliz/#/Feliz/ProjectTemplate).

``` csharp
dotnet new -i Feliz.Template
```

If you already have Feliz template installed now is good time to update it

```console
dotnet new -u Feliz.Template
dotnet new -i Feliz.Template
```

next we can create and run the app

```console
dotnet new feliz -n AwesomeApp
cd AwesomeApp
pnpm i
pnpm run start
```

You should be greeted with your shiny new awesome Feliz App. 

Let's quickly step through the app to get familiar with the landscape.

#### Mounting The App

In the ```Main.fs``` we are mounting our react app. This should look very similar to anyone familiar with react. Using Fable we get a reference to 'feliz-app' div from ```index.html``` and we mount our app.
```fsharp
module Main

open Feliz
open App
open Browser.Dom

let root = ReactDOM.createRoot (document.getElementById "feliz-app")
root.render (Components.Counter())
```

#### Type Safe Html

In the ```Components.fs``` we have some example of how to build UI using Feliz. You dont have to use a class with static members, you can define your react components using functions and decorating them ```[<ReactComponent>]```. However using an F# class to group components with similar features or use cases can be a good idea.

You should notice right away there is no Html in our **Counter** component! We are instead using the Feliz DSL to write type safe F# code which is transpiled to Html.

For example the ```Html.div []``` is an F# function which takes a list (denoted by the ```[]```) and renders to a ```div``` once its fed through the Fable compiler.

Were also using some local state to store the value of a count:

```fsharp
let (count, setCount) = React.useState (0)

```

and and button to update this state:

```fsharp
Html.button [ 
  prop.onClick (fun _ -> setCount (count + 1)); 
  prop.text "Increment" ]
```

Thanks to the F# compiler we get immediate feedback if we wrote our Html wrong or supplied and incorrect attribute or property. This compile time safety is a **HUGE** productivity boost for me personally.

I cant tell you how many times I fat finger some Html and don't realize it until someone reported it as a bug.

#### Routing

The **Router** component gives you an example of how we can perform client side routing in Feliz. There are other options available however this is a very elegant solution.

My favorite part is that since this is F# we get  exhaustive pattern matching on our routes, reducing or eliminating chances that user hit a route we did not anticipate.

We also get all the F# type safety goodness in our routes and component functions!

```fsharp
[<ReactComponent>]
static member Router() =
  let (currentUrl, updateUrl) = React.useState (Router.currentUrl ())

  React.router [ 
	router.onUrlChanged updateUrl 
	router.children [ 
		match currentUrl with
		| [] -> Html.h1 "Index"
		| [ "hello" ] -> Components.HelloWorld()
		| [ "counter" ] -> Components.Counter()
		| otherwise -> Html.h1 "Not found" ] ]
```

Basically we take the current url and parse it to return an F# list that we can then pattern match over. In this case we are storing the current url in local state and passing the function ```updateUrl``` to the router component.


## Update Packages add JSX Support

Now that we have the basic app up and running lets add support for JSX components, we will also change the project target to .Net 8. Future you may not need to complete these step once Feliz library get updated to the latest bits.

First lets update the **fable** and **femto** dotnet tools.

```console
dotnet tool update fable
dotnet tool update femto
```

Next we will bump out .Net version target from .Net 7 to .Net 8.

```diff
 ./AwesomeApp.fsproj
<PropertyGroup>
+ <TargetFramework>net8.0</TargetFramework>
- <TargetFramework>net7.0</TargetFramework>
</PropertyGroup>
```

Next we need to update all nuget packages. A very powerful tool for package management a tool called [Fake](https://fake.build/) which I highly recommend you look. For now lets just use the cli.

```console
cd ./src
dotnet add package Feliz
dotnet add package Feliz.Router
dotnet add package Fable.Core
```

Next lets add reference to the Feliz compiler plugin. I'm going to use femto to do this. If you're not familiar with femto tool you can check it out [here](https://github.com/Zaid-Ajaj/Femto).

```console
femto
femto inspall FSharp.Core
femto install Feliz.CompilerPlugins
```

We will have to modify our **package.json** to instruct fable to output .jsx files, also we will need to update our **index.html** to use the Main.jsx instead of Man.fs.js.

In our **package.json**

```diff
{
 "private": true,
 "scripts": {
-   "start": "dotnet tool restore && dotnet fable watch src --runFast vite",
+   "start": "dotnet tool restore && dotnet fable watch src -e .jsx --runFast vite",
 "build": "dotnet tool restore && dotnet fable src --run vite build",
 "clean": "dotnet fable clean src --yes"
 },

 "dependencies": {
 "react": "^18.2.0",
 "react-dom": "^18.2.0"
 },

 "devDependencies": {
 "@vitejs/plugin-react": "^3.1.0",
 "vite": "^4.1.0"
 },
 "engines": {
 "node": ">=18"
 }
}
```

And in our **index.html**;

```diff
<!doctype  html>

<html>
 
 <head>
  <title>Feliz App</title>
  <meta  http-equiv='Content-Type'  content='text/html; charset=utf-8'>
  <meta  name="viewport"  content="width=device-width, initial-scale=1">
  <link  rel="shortcut icon"  type="image/png"  href="./img/favicon-32x32.png"  sizes="32x32"  />
  <link  rel="shortcut icon"  type="image/png"  href="./img/favicon-16x16.png"  sizes="16x16"  />
 </head>
 <body>
  <div  id="feliz-app"></div>
-     <script  type="module"  src="./Main.fs.js"></script>
+     <script  type="module"  src="./Main.jsx"></script>
 </body>

</html>
```

At this point you'll probably want to update your .gitignore to ignore all the generated .jsx files. I also like to add the **fable_modules** to .gitignore.



```git
src/*.jsx
fable_modules
```

## JSX Components

Now that we have all that nonsense out of the way we can add .jsx components and use them from Feliz.

Here is a very contrived example of how to do that.

```fsharp
open  Fable.Core
open  Fable.React

[<AutoOpen>]
module  JsxHelpers  =
  let inline  toJsx  (el:  ReactElement)  :  JSX.Element  =  unbox  el
  let inline  toReact  (el:  JSX.Element)  :  ReactElement  =  unbox  el
 
type  Components  =
  [<ReactComponent>]
  static member  JsxCount(props:{|count:int|})  =
    let  (count,  setCount)  =  React.useState  (0)

    JSX.jsx
      $"""
        <div  style={{{{marginTop:'10px'}}}}  >From JSX: {props.count}</div>
      """ 
	|>  toReact
```

We add a couple helper functions to convert back and forth from **ReactElement**  and  **JSX.Element** types.

Then we can use this new JsxCounter inside the existing Counter component:

```fsharp
[<ReactComponent>]
static member  Counter()  =
  let  (count,  setCount)  =  React.useState  (0)
  Html.div [   
	Html.h1  count
    Html.button  [  
	  prop.onClick  (fun _ ->  setCount  (count  +  1));  
	  prop.text  "Increment"  ]
    //Add our new JSX component.
    Components.JsxCounter({|  count  =  count  |})  ]
```

Lets run the app again and navigate to /counter.

```console
cd ..
pnpm start
```

You should get an output that matches the following. If you run into any problems please open an issue in the repository.

![jsx counter in F#!!!](https://rasheedaboudblogstorage.blob.core.windows.net/blogs/jsxcounter.gif?sp=r&st=2024-01-05T04:17:47Z&se=2050-01-05T12:17:47Z&spr=https&sv=2022-11-02&sr=b&sig=%2FoSyOlsIIgI0Vx04av643TXF3pqzivicMPbx1cjtn%2Bk%3D)

Thanks to the VS code extension we added earlier we get code completions right inside our templated string. You can also pass props from F# to the JSX component and add styles. However as you probably noticed escaping your ```{}``` can get a little crazy and you should probably use **csss** classes to style the components when necessary.

## Wrapping Up

Hopefully this showed you how simple it is to get started with [Fable](https://fable.io/) and [Feliz](https://zaid-ajaj.github.io/Feliz/#/). These tools are my goto anytime I'm making a new react app. I find using F# in the browser to incredibly productive, also the type safety that the F# language provides  me with an added safety net that even Typescript cant compete with.

In this blog post, we covered how to get started with F# and the Feliz library for building web applications. We walked through setting up a Feliz project template, mounting a React app, and using the Feliz DSL to create type-safe HTML. We also explored client-side routing and how to add JSX support to Feliz.

Using F# and Feliz provides many benefits, including type safety, increased productivity, and the ability to leverage the power of React in a functional programming language. With Feliz, we can write type-safe HTML using the Feliz DSL and take advantage of JSX components.

On a side note you may be asking who do we want to jump through all those hoops just to add another way to represent Html in our App (JSX)?  Especially in a manner where we loose all our type safety? Well Alfonso Garcia has an excellent series of blog post about just that!  You can check them out over [here](https://fable.io/blog/2022/2022-10-12-react-jsx.html).

The answer is we didn't  **have** too. However if you want to use any third party JavaScript packages, you will need to wrap them in F# types. Similar to adding Typescript annotations to a JavaScript library. Personally I've found the JSX method to be the most productive way to quickly wrap a component and us it from F#.

If im doing something that is mission critical I would consider making a specific Feliz component for it for some added type safety. Otherwise using JSX method allows you to pull in and use a myriad of npm packages with very little to no friction.

In part 2 we will look at using environment variables on the client and slowly work our way up to a decent sized production app. 
Overall, F# and Feliz are a powerful combination for building web applications, and I hope this blog post has given you a good starting point for exploring these tools further. Happy coding!  
