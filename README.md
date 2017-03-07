# ImComparePdfs
ImageMagick Compare PDFs is designed to compare PDF's as images. All code in F# and anyone is free to suggest changes.

PDF's are compared page by page using Magick.Net ( https://magick.codeplex.com/ )
One can compare any number of PDF's simultaneously or just two.

The functions available are:

  Takes a list of PDF files, list<FileInfo>
      FirstDifference   // Sequential search. Always returns first difference.
      AnyDifference     // Parallel search. Might not return first difference.
      AllDifferences    // Parallel search. Looks for all differences

  Takes a 2 FileInfo objects (Compares two files)
      DifferenceBetween     // Two files. Find first difference
      AllDifferencesBetween // Two files. Find all differences
      
 All functions will return a list<FileInfo> list of image files, which describe the difference on particular page. 
