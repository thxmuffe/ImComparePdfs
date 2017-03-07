namespace ComparePdfs

    open ImageMagick
    open System.IO

    module ImageMagick =

        let CompareOptions = ErrorMetric.Absolute

        let WriteComparison(image1 : MagickImage, image2, diffFilePath : FileInfo) =
            let diffFile = new MagickImage()
            image1.Compare(image2, CompareOptions, diffFile) |> ignore
            diffFile.Write diffFilePath.FullName
            diffFilePath

        let ReadImage(doc : FileInfo, page) : MagickImage =
            let settings = 
                let s : MagickReadSettings = new MagickReadSettings()
                try 
                    s.FrameIndex <- page - 1 |> System.Nullable // Change to zero-based indexing   
                    s.FrameCount <- 1 |> System.Nullable  ; s 
                with
                    | :? _ -> ( printfn "Could not read image on page"; s)

            let images = new MagickImageCollection()
            images.Read(doc, settings)
            images.[0]

        let compare(image1 : MagickImage, image2) =
            // (img, img, delta)
            (image1, image2, image1.Compare( image2, CompareOptions) )
