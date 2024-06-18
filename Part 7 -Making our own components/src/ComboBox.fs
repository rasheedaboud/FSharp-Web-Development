module ComboBox

open Feliz

type ComboBoxProps<'a> =
  { data: 'a []
    label: string
    palceholder: string
    selected: string -> unit }

[<ReactComponent>]
let ComboBox (props: ComboBoxProps<'a>) =
  let input, setInput = React.useState ("")
  let selectedValue, setSelectedValue = React.useState ("")
  let initial, setInitial = React.useState (props.data)
  let data, setData = React.useState (props.data)
  let hasFocus, setHasfocus = React.useState (false)
  let activeSuggestionIndex, setActiveSuggestionIndex = React.useState (0)

  React.useEffect ((fun () -> props.selected (selectedValue)), [| box selectedValue |])

  React.useEffect (
    (fun () ->
      if System.String.IsNullOrEmpty(input) then
        setData (initial)
      else
        let data =
          initial
          |> Array.filter (fun (x: string) -> x.ToLower().Contains(input.ToLower()))

        if data.Length <= 0 then
          setData (initial)
          setInput ("")
        else
          setData (data)),
    [| box input |]
  )

  let handleKeyDown (evt: Browser.Types.KeyboardEvent) =
    setHasfocus (true)

    if evt.key = "ArrowUp" then
      if activeSuggestionIndex = 0 then
        ()
      else
        setActiveSuggestionIndex (activeSuggestionIndex - 1)

    elif evt.key = "ArrowDown" then

      if activeSuggestionIndex = data.Length - 1 then
        ()
      else
        setActiveSuggestionIndex (activeSuggestionIndex + 1)
    elif evt.key = "Enter" then
      setInput (data[activeSuggestionIndex])
      setSelectedValue (data[activeSuggestionIndex])
      setActiveSuggestionIndex (0)
      setHasfocus (false)

  Html.div [
    prop.className "form-control"
    prop.children [
      Html.div [
        prop.className "dropdown"
        prop.children [
          Html.div [
            prop.className "label"
            prop.children [
              Html.span [
                prop.className "label-text font-bold"
                prop.text props.label
              ]
            ]
          ]
          Html.input [
            prop.classes [
              "input input-sm"
              "input-bordered"
              "w-full"
            ]
            prop.type' "search"
            prop.placeholder props.palceholder
            prop.value input
            prop.onChange (fun x -> setInput (x))
            prop.onFocus (fun _ -> setHasfocus (true))
            prop.onKeyDown handleKeyDown
          ]
          if hasFocus then
            Html.ul [
              prop.classes [
                "dropdown-content"
                "z-[1]"
                "menu"
                "p-2"
                "shadow"
                "bg-base-100"
                "rounded-box"
                "w-full"
                "max-h-80"
                "flex-nowrap"
                "overflow-auto"
              ]
              prop.tabIndex 0
              prop.children [

                for i = 0 to data.Length do
                  Html.li [
                    prop.id data[i]
                    prop.key i
                    prop.children [
                      Html.a [
                        prop.className (
                          if activeSuggestionIndex = i then
                            "active"
                          else
                            ""
                        )
                        prop.onClick (fun _ ->
                          setInput (data[i])
                          setSelectedValue (data[i])
                          setHasfocus (false))
                        prop.text data[i]
                      ]
                    ]
                  ]
              ]
            ]
        ]
      ]
    ]
  ]