using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace projet_wpf.Models
{
    public class PhotoModel
    {
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public DateTime DateAdded { get; set; }

        public BitmapImage Thumbnail { get; set; }

        public PhotoModel(string filePath)
        {
            FilePath = filePath;
            FileName = System.IO.Path.GetFileName(filePath);
            DateAdded = DateTime.Now;

            Thumbnail = new BitmapImage();
            Thumbnail.BeginInit();
            Thumbnail.UriSource = new Uri(filePath);
            Thumbnail.DecodePixelWidth = 150;
            Thumbnail.EndInit();
        }
    }
}
