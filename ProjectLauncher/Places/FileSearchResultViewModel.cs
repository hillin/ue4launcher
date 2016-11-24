using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IOPath = System.IO.Path;

namespace UE4Launcher.Places
{
    class FileSearchResultViewModel : SearchResultViewModelBase
    {
        public static double CalculateRelevancyRating(string path, string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
                return 0;

            var filename = IOPath.GetFileName(path);
            if (string.IsNullOrEmpty(filename))
                return 0;

            if (filename.Equals(keyword))
                return 1;

            if (filename.Equals(keyword, StringComparison.CurrentCultureIgnoreCase))
                return 0.9;

            filename = IOPath.GetFileNameWithoutExtension(path);
            if (string.IsNullOrEmpty(filename))
                return 0;

            if (filename.Equals(keyword))
                return 0.9;

            if (filename.Equals(keyword, StringComparison.CurrentCultureIgnoreCase))
                return 0.8;

            if(filename.StartsWith(keyword))
                return 0.8 * keyword.Length / filename.Length;

            var upperFilename = filename.ToUpper();
            var upperKeyword = keyword.ToUpper();

            if(upperFilename.StartsWith(upperKeyword))
                return 0.7 * keyword.Length / filename.Length;

            if (filename.Contains(keyword))
                return 0.7 * keyword.Length / filename.Length;

            if (upperFilename.Contains(upperKeyword))
                return 0.6 * keyword.Length / filename.Length;

            return 0;
        }

        public override string DisplayName
        {
            get { return IOPath.GetFileName(this.Path); }
            set { base.DisplayName = value; }
        }
        public FileSearchResultViewModel(string path, double relevancyRating)
        {
            this.Path = path;
            this.RelevancyRating = relevancyRating;
        }

    }
}
