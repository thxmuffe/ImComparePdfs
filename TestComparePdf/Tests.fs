namespace ComparePdfs

open Xunit

module Tests = 

    // All these should raise an error
    [<Fact>]
    let Test_BadInput_ErrorRaised() =
        // Bad FilePath given
        AssertErrorRaised.PdfComparison( ["2-page-doxxxxxxt.pdf"], [])

        // Not any files given
        AssertErrorRaised.PdfComparison( [], [])

        // Cannot comapre only 1 document
        AssertErrorRaised.PdfComparison( ["2-page-document.pdf"], [])
        []

    [<Fact>]
    let Assert_TwoSmallEqualDocuments() =
        Assert.PdfComparison( 
            ["2-page-document.pdf";
            "2-page-document.pdf" ], 
            [])

    [<Fact>]
    let Assert_TwoUnequalLengthDocuments() =
        Assert.PdfComparison( 
            ["2-page-document.pdf";
            "11-page-document_with_text_on_page11.pdf" ], 
            [2])

    [<Fact>]
    let Assert_TwoUnequalSmallDocuments() =
        Assert.PdfComparison( 
            ["2-page-document.pdf";
            "2-page-document_without_content_on_2nd_page.pdf" ], 
            [2])

    [<Fact>]
    let Assert_MultipleUnequalSmallDocuments() =
        Assert.PdfComparison( 
            ["2-page-document.pdf";
            "2-page-document_without_content_on_2nd_page.pdf";
            "11-page-document_without_text_on_page11.pdf";
             "2-page-document.pdf"], 
            [2])

    [<Fact>]
    let Assert_TwoUnequalMediumSizeDocuments() =
        Assert.PdfComparison( 
            ["11-page-document_with_text_on_page11.pdf";
            "11-page-document_without_text_on_page11.pdf" ], 
            [11])

    [<Fact>]
    let Assert_TwoUnequalLargeDocuments() =
        Assert.PdfComparison( 
            ["60-page-document_with_text_on_page40.pdf";
            "60-page-document_without_text_on_page40.pdf" ], 
            [40])

    [<Fact>]
    let Assert_MultipleUnequalLargeDocuments() =
        Assert.PdfComparison( 
            ["60-page-document_with_text_on_page40.pdf";
            "60-page-document_without_text_on_page40.pdf";
            "60-page-document_with_text_on_page40.pdf";
            "60-page-document_without_text_on_page40.pdf"  ], 
            [])

    let Assert_MultipleEqualLargeDocuments() =
        Assert.PdfComparison( 
            ["60-page-document_with_text_on_page40.pdf";
            "60-page-document_with_text_on_page40.pdf";
            "60-page-document_with_text_on_page40.pdf";
            "60-page-document_with_text_on_page40.pdf";
            "60-page-document_with_text_on_page40.pdf";
            "60-page-document_with_text_on_page40.pdf"  ], 
            [])

    let printResult result = 
        let origColor = System.Console.ForegroundColor
        try
            result
                |> List.map (fun res ->
                match res  with
                | Assert.DocumentsEqual ->
                    System.Console.ForegroundColor <- System.ConsoleColor.Green
                    printfn "%s" "Documents are EQUAL"
                | Assert.DifferencesFound ->
                    System.Console.ForegroundColor <- System.ConsoleColor.Yellow
                    printfn "%s" "Documents are DIFFERING" )
        finally
             System.Console.ForegroundColor <- origColor
        |> ignore

    let allTests = [
        Test_BadInput_ErrorRaised;
        Assert_TwoSmallEqualDocuments;
        Assert_TwoUnequalLengthDocuments;
        Assert_TwoUnequalSmallDocuments;
        Assert_MultipleUnequalSmallDocuments;
        Assert_TwoUnequalMediumSizeDocuments;
        Assert_TwoUnequalLargeDocuments;
        Assert_MultipleUnequalLargeDocuments;
        Assert_MultipleEqualLargeDocuments]

    let RunAll() =
        
        printfn "%s" "Starting tests"

        allTests
            |> List.iter (fun test -> test() |> printResult )