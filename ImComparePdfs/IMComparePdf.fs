namespace ComparePdfs

module Exceptions =
    exception PageCountZero = PdfHelper.PageCountZero
    exception FileCount = Differences.FileCount

// Public interface
module Comparison =
    let FileExtension = Differences.FileExtension
    let DiffFilePath = Differences.diffFilePath
    let FirstDifference = Differences.findFirst
    let AnyDifference = Differences.findAny
    let AllDifferences = Differences.findAll
    let DifferenceBetween f1 f2 = Differences.findAny [f1; f2]
    let AllDifferencesBetween f1 f2 = Differences.findAll [f1; f2]