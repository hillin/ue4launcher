using System;
using IOPath = System.IO.Path;

namespace UE4Launcher.Places
{
    class FileSearchResultViewModel : SearchResultViewModelBase
    {
        private static double CalculateFilenameRelevancyRating(string filename, string keyword)
        {
            if (string.IsNullOrEmpty(filename))
                return 0;

            if (filename.Equals(keyword))
                return 1;

            if (filename.Equals(keyword, StringComparison.CurrentCultureIgnoreCase))
                return 0.95;

            if (filename.StartsWith(keyword))
                return 0.95;

            var upperKeyword = keyword.ToUpper();
            var upperFilename = filename.ToUpper();

            if (upperFilename.StartsWith(upperKeyword))
                return 0.9;

            if (filename.Contains(keyword))
                return 0.85;

            if (upperFilename.Contains(upperKeyword))
                return 0.8;

            return 0;
        }

        public static double CalculateRelevancyRating(string path, string keyword)
        {

            if (string.IsNullOrEmpty(keyword))
                return 0;

            var fileName = IOPath.GetFileName(path);
            if (string.IsNullOrEmpty(fileName))
                return 0;

            var fileNameWithoutExtension = IOPath.GetFileNameWithoutExtension(path);
            
            var rating =
                Math.Max(FileSearchResultViewModel.CalculateFilenameRelevancyRating(fileName,  keyword),
                         FileSearchResultViewModel.CalculateFilenameRelevancyRating(fileNameWithoutExtension, keyword) * 0.95);

            rating *= (double)keyword.Length / fileName.Length;

            if (rating <= 0)
                return 0;

            if (path.Contains(@"\Intermediate\")
                || path.Contains(@"\Saved\")
                || path.Contains(@"\DerivedDataCache\"))
                rating *= 0.2;

            if (path.Contains(@"\Binaries\"))
                rating *= 1.2;
            else if (path.Contains(@"\Content\")
                || path.Contains(@"\Source\"))
                rating *= 1.5;

            if (rating > 1)
                rating = 1;
            else if (rating < 0)
                rating = 0;

            return rating;
        }

        public override string DisplayName
        {
            get { return IOPath.GetFileName(this.Path); }
            set { base.DisplayName = value; }
        }

        protected override string StringComparablePart => this.Path;

        public FileSearchResultViewModel(string path, double relevancyRating)
        {
            this.Path = path;
            this.RelevancyRating = relevancyRating;
        }

    }
}
