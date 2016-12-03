namespace YC.BioGraph


open WebSharper
open WebSharper.JavaScript
open WebSharper.Html.Client

module wsfc = WebSharper.Formlets.Controls
module wsfe = WebSharper.Formlets.Enhance
module wsfd = WebSharper.Formlets.Data
module wsff = WebSharper.Formlets.Formlet
module wsfl = WebSharper.Formlets.Layout



[<JavaScript>]
module Client =

    let ChooseDefaultControl = //opens the list of default grammars/graphs
       wsff.OfElement (fun () ->
            Input [Attr.Type "button"; Attr.Value "Choose default"; Attr.Style "color: #000000"]
        )
   
    let FileControl = 
        wsff.OfElement (fun () ->
            Input [Attr.Type "file"]
        )
    
    let InputControl lbl =
        wsff.Do {
            let! textInput =
                wsfc.TextArea ""                    
                |> wsfe.WithTextLabel lbl
                |> wsfe.WithLabelAbove
                |> wsff.MapElement (fun e ->
                    JQuery.JQuery.Of(e.Dom.QuerySelector("textarea"))
                        .Css("height", "200px") 
                        .Css("width", "500px")
                        .Ignore
                    e)
            let! fileInput = FileControl
            let! chooseButton = ChooseDefaultControl
            return (textInput, fileInput, chooseButton)             
        }
        |> wsff.Vertical
        |> wsfe.WithFormContainer

    let RangeControl =
        wsff.Do {                
            let! min = wsfc.Input "1" |> wsfe.WithTextLabel "from" |> wsfd.Validator.IsInt "Enter numericr value" 
            let! max  = wsfc.Input "5" |> wsfe.WithTextLabel "to" |> wsfd.Validator.IsInt "Enter numericr value" 
            return (int min, int max)
        }
        |> wsff.Horizontal 
        |> wsfe.WithTextLabel "String range"
        |> wsfe.WithLabelAbove 
        |> wsfe.WithFormContainer 

    let OutputControl = 
        wsff.Do {
            let! output =
                wsfc.ReadOnlyTextArea""                    
                |> wsfe.WithTextLabel "Output"
                |> wsfe.WithLabelAbove 
                |> wsff.MapElement (fun e ->
                    JQuery.JQuery.Of(e.Dom.QuerySelector("textarea"))
                            .Css("height", "300px")
                            .Css("width", "500px")
                            .Ignore
                    e)
            return output
        }
        |> wsfe.WithFormContainer
    
    let ShowImageControl =
       wsff.OfElement (fun () ->
            Img[Attr.Style "height: 300px; width: 500px"]
        )
       |> wsfe.WithTextLabel "Graph visualisation"
       |> wsfe.WithLabelAbove 
       |> wsfe.WithFormContainer
        

    let frm =        
     
        let InputForm  =
            wsff.Do {                
                let! grammar = InputControl "Grammar"
                let! graph = InputControl "Graph"
                let! range = RangeControl
                let! checkbox = wsfc.Checkbox false |> wsfe.WithTextLabel "DRAW GRAPH" |> wsfe.WithLabelLeft
                return (grammar, graph, range, checkbox)
            }
            |> wsff.Vertical

        let OutputForm =
            wsff.Do {
                let! picture = ShowImageControl |> wsfe.WithFormContainer
                let! output = OutputControl 
                return (picture, output)
            }
            |> wsff.Vertical 
        
        wsff.Do {
            let! x = InputForm
            let! y = OutputForm
            return (x, y)
        }
        |> wsff.Horizontal
        |> wsfe.WithCustomSubmitButton ({ wsfe.FormButtonConfiguration.Default with 
                                                                                   Label = Some "GO" 
                                                                                   Style=Some "background-color: #FF1493; font-size: 40px"})

    let Main () =
        let MainForm =
            frm.Run(fun _ -> ())
        
        Div [      
           MainForm
        ] 


 