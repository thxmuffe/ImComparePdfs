namespace ComparePdfs

module PdfHelper =

    open System.IO
    open PdfSharp.Pdf
    open PdfSharp.Pdf.IO

    exception PageCountZero of string

    let PageCount(pdf : FileInfo) =
        PdfReader.Open( pdf.FullName, PdfDocumentOpenMode.InformationOnly ).PageCount

    let maxPage files = 
        match files
                |> Seq.map PageCount
                |> Seq.toList
                |> List.min
            with 
            | 0 -> raise <| PageCountZero "One of given files is empty. No comparison done."
            | nonZeroPageCount -> nonZeroPageCount