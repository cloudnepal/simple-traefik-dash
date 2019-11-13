namespace STD.Views

open DomainAgnostic
open Giraffe.GiraffeViewEngine
open STD.Parsers.Traefik
open STD.Consts


module TraefikServices =

    let Page tit bdy =
        html []
            [ head []
                  [ meta [ _charset "utf-8" ]
                    meta
                        [ _name "viewport"
                          _content "width=device-width, initial-scale=1" ]
                    link
                        [ _rel "stylesheet"
                          _href "site.css" ]
                    title [] [ str tit ] ]
              body [] bdy ]

    let Layout contents =
        div []
            [ main [] contents
              footer []
                  [ a [ _href PROJECTURI ] [ str "Find me on Github" ]
                    a [ _href "status" ] [ str "status api" ] ] ]
        |> List.singleton

    let prettifyName (name: string) =
        (name.Split('-', '_') |> String.concat " ").Split(' ')
        |> Seq.map (fun s ->
            match s |> List.ofSeq with
            | [] -> ""
            | h :: t ->
                t
                |> List.map ToString
                |> List.Prepending(h.ToString().ToUpper())
                |> String.concat "")
        |> String.concat " "

    let Route uri name =
        let pretty = prettifyName name

        let big =
            pretty
            |> Seq.tryHead
            |> Option.map ToString
            |> Option.defaultValue ""

        a [ _href uri ]
            [ figure []
                  [ h1 [] [ str big ]
                    figcaption [] [ str pretty ] ] ]

    let disperse { name = n; uris = us } =
        match us |> List.ofSeq with
        | [ u ] -> Route u n |> List.singleton
        | u -> u |> List.mapi (fun i u -> sprintf "%s-%d" n i |> Route u)


    let Print tit routes =
        routes
        |> List.ofSeq
        |> List.Bind disperse
        |> Layout
        |> Page tit
        |> renderHtmlDocument
