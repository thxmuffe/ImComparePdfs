namespace ComparePdfs

module private Differences =

    open System.IO
    open FSharp.Collections.ParallelSeq
    open ImageMagick

    exception FileCount of string

    let FileType = "png"
    let FileExtension = "." + FileType
    let DiffFilePattern = "_DIFF_ON_PAGE_"

    let pageWithLeadingZeroes( page : int ) =
        let numberOfDigits = List.max [6; page.ToString().Length + 1] // Try to keep at least one leading zero
        page.ToString("D" + numberOfDigits.ToString())

    let diffFilePath(files : list<FileInfo>, page) =
        new FileInfo(files.Head.FullName + DiffFilePattern + pageWithLeadingZeroes(page) + FileExtension)

    let PageDiff(files : list<FileInfo> , page : int ) =
        if files.Length < 2 then
            raise <| FileCount "Cannot compare less than 2 files."

        files
            |> PSeq.map ( fun file -> ReadImage( file, page ) )
                |> Seq.toList
            |> List.pairwise
                |> List.map compare
            |> List.filter (fun (_, _, delta) -> delta > 0.0 ) // Differences only
            |> List.truncate 1 // Take max. 1 difference
            |> List.map (fun (image1, image2, _) -> 
                WriteComparison(image1, image2, diffFilePath(files, page)))

    let findFirst(files : list<FileInfo>) =
        // Looping recursively until finding a match
        let rec firstDifference = function
            | page :: restOfPages -> 
                match PageDiff(files, page) with
                | [] -> firstDifference restOfPages
                | diffFiles -> diffFiles
            | _ -> [] // List consumed. No differences found.
        firstDifference [1 .. PdfHelper.maxPage files]

    let findAny(files : list<FileInfo>) =
        // Split the investigation to multiple threads. Does not guarantee sequential search. (Is close though)
        let rec FindInChuncks pages = 
            let ReturnOrContinue(differences, restOfPages) : list<FileInfo> = 
                match differences with
                | [] -> FindInChuncks restOfPages // No differences continue searching  
                | diffFiles -> diffFiles               

            let PMapPageDiff(pages) : list<FileInfo> =
                pages 
                    |> PSeq.map (fun page -> PageDiff(files, page))
                        |> Seq.toList
                        |> List.concat
                    |> List.truncate 1 // Intrested in only one

            match pages with
            // Image magick works pretty ok with a few threads. more threads than 2 does not increase performance much
            | p1 :: p2 :: restOfPages ->
                ReturnOrContinue( PMapPageDiff [p1; p2], restOfPages)
            | page :: restOfPages ->
                ReturnOrContinue( PageDiff( files, page), restOfPages)
            | _ -> [] // List consumed. No differences found.

        let max = PdfHelper.maxPage files

        // Check first and last page first
        List.concat [[1; max ]; [2 .. (max - 1)]]
            |> FindInChuncks

    let findAll files =
        [1 .. PdfHelper.maxPage files] 
            |> List.chunkBySize 4 // Split for multicore processing
                |> List.map (fun chunck ->
                    chunck
                        |> PSeq.map (fun page -> PageDiff(files, page) )
                        |> Seq.toList )
                |> List.concat
            |> List.concat

            
