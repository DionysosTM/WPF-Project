using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace projet_wpf.Models
{
    public class PhotoModel : INotifyPropertyChanged
    {
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public long FileSize { get; set; }
        public DateTime DateAdded { get; set; }
        public double CurrentAngle { get; private set; } = 0;
        
        // Image originale
        private BitmapImage _originalBitmap;
        // Miniature originale
        private BitmapSource _originalThumbnail;

        // Image diaporama
        private ImageSource _fullImage;
        public ImageSource FullImage
        {
            get => _fullImage;
            set {
                if (_fullImage != value) {
                    _fullImage = value;
                    OnPropertyChanged(nameof(FullImage));
                }
            }
        }

        // Image miniature
        private ImageSource _thumbnail;
        public ImageSource Thumbnail
        {
            get => _thumbnail;
            set {
                if (_thumbnail != value) {
                    _thumbnail = value;
                    OnPropertyChanged(nameof(Thumbnail));
                }
            }
        }

        public PhotoModel(string filePath)
        {
            FilePath = filePath;
            FileName = Path.GetFileName(filePath);
            DateAdded = DateTime.Now;
            FileType = Path.GetExtension(filePath).ToLower();          
            FileSize = new FileInfo(filePath).Length;

            // Image originale
            _originalBitmap = new BitmapImage();
            _originalBitmap.BeginInit();
            _originalBitmap.UriSource = new Uri(filePath);
            _originalBitmap.CacheOption = BitmapCacheOption.OnLoad;
            _originalBitmap.EndInit();
            _originalBitmap.Freeze();

            FullImage = _originalBitmap;

            // Image miniature
            var thumb = new BitmapImage();
            thumb.BeginInit();
            thumb.UriSource = new Uri(filePath);
            thumb.DecodePixelWidth = 150;
            thumb.CacheOption = BitmapCacheOption.OnLoad;
            thumb.EndInit();
            thumb.Freeze();

            // Stocker la miniature originale
            _originalThumbnail = thumb;
            Thumbnail = thumb;
        }

        // Méthode pour faire rotate l'image
        public void RotateBy(double degrees)
        {
            CurrentAngle = (CurrentAngle + degrees) % 360;

            // Rotater la diaporama
            var rotatedFull = new TransformedBitmap(_originalBitmap, new RotateTransform(CurrentAngle));
            rotatedFull.Freeze();
            FullImage = rotatedFull;

            // Rotater la miniature
            var rotatedThumb = new TransformedBitmap(_originalThumbnail, new RotateTransform(CurrentAngle));
            rotatedThumb.Freeze();
            Thumbnail = rotatedThumb;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
