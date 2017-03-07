open System.IO

[<EntryPoint>]
let main argv = 

    printfn "Files to compare: %A" argv

    let files = match argv.Length with
                // | 1 -> DirectoryInfo(argv.[0]).GetFiles() |> List.concat
                | _ -> argv |> Seq.toList |> List.map FileInfo

    match files |> ComparePdfs.Comparison.AllDifferences with
    | [] -> printfn "%s" "Completed!"
    | differences -> printfn "%i Differences found" differences.Length
    
    System.Console.ReadLine() |> ignore

    0 // return an integer exit code
