namespace ComparePdfs

module Assert = 

    open Xunit
    open System.IO
    open ComparePdfs

    type Results =
        |DocumentsEqual
        |DifferencesFound

    let expectedEqual(docs) =
        // If document path is different they are expected to have differing content as well
        (docs |> List.distinct).Length = 1

    module DiffFiles =
        let expected(docs, expectedPages) =
            expectedPages
                |> List.map (fun page -> Comparison.DiffFilePath(docs |> List.map FileInfo , page))
    
        let Assert_FileExists(file : FileInfo) =
            Assert.True file.Exists

        let Assert_Exists(docs : list<string>, expectedPages) = 
            expected(docs, expectedPages)
                |> List.map Assert_FileExists

        let DeleteExisting(docs : list<string>, expectedPages) = 
            expected(docs, expectedPages)
                |> List.iter (fun f -> if File.Exists(f.FullName) then File.Delete(f.FullName) )

    let AssertDifferencesCount( docs , differences : list<FileInfo>, expectedPages : list<int> ) =
        DiffFiles.Assert_Exists(docs, expectedPages) |> ignore
            
        if docs |> expectedEqual then
            Assert.True( differences.Length = 0 )
            DocumentsEqual
        else
            Assert.True( differences.Length > 0 )
            DifferencesFound

    let PdfComparisonWithBothMethods ( docs : list<string>, expectedPages ) =
        DiffFiles.DeleteExisting(docs, expectedPages)
        // Test finding first
        let t1 = System.DateTime.Now
        let resultFirst = AssertDifferencesCount( 
            docs,
            docs
                |> List.map FileInfo
                |> Comparison.FirstDifference, 
            expectedPages)
        let t1_end = System.DateTime.Now

        DiffFiles.DeleteExisting(docs, expectedPages)
        // Test parallel implementation of findAny
        let t2 = System.DateTime.Now
        let resultsAny = AssertDifferencesCount( 
            docs,
            docs
                |> List.map FileInfo
                |> Comparison.AnyDifference,
            expectedPages)
        let t2_end = System.DateTime.Now

        // If there is only one expected difference both method should return the same result
        if expectedPages.Length = 1 then
            Assert.Equal(resultFirst, resultsAny)

        printfn "%s" ("Test duration: { FirstDifference=" + t1_end.Subtract(t1).ToString() + " AnyDifference=" +  t2_end.Subtract(t2).ToString() + "}")
        [resultFirst; resultsAny]

    let PdfComparison ( docs, expectedPages )   =
        printfn "%s" "Comparing files:"
        docs |> List.iter (fun f -> printfn "   %s" f )
        PdfComparisonWithBothMethods( docs, expectedPages)

module AssertErrorRaised =

    open Xunit
    open System.IO

    let PdfComparison(docs : list<string>, expectedDiffFile : list<int>) =
        Assert.True (
            try
                Assert.PdfComparison(docs, expectedDiffFile) |> ignore
                false
            with
                // TODO: Match to proper exception, not all
                | :? _ as ex -> true  )
