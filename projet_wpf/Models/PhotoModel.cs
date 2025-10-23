using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
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

        private Color _dominantColor = Colors.Transparent;
        public Color DominantColor
        {
            get => _dominantColor;
            set
            {
                if (_dominantColor == value) return;
                _dominantColor = value;
                OnPropertyChanged(nameof(DominantColor));
                OnPropertyChanged(nameof(DominantColorHex));
            }
        }
        public string DominantColorHex => $"#{DominantColor.R:X2}{DominantColor.G:X2}{DominantColor.B:X2}";

        private ObservableCollection<TagItem> _tags = new ObservableCollection<TagItem>();
        public string TagsString => string.Join(", ", Tags.Select(t => t.Text));
        public ObservableCollection<TagItem> Tags
        {
            get => _tags;
            set
            {
                _tags = value;
                OnPropertyChanged(nameof(Tags));
                OnPropertyChanged(nameof(TagsString));
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



            Task.Run(() =>
            {
                var color = GetDominantColorFromBitmapSource(_originalThumbnail, quantizeBitsPerChannel: 5);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    DominantColor = color;
                });
            });
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

        public void AddTag(string tag)
        {
            tag = tag.Trim().ToLower();
            if (!string.IsNullOrWhiteSpace(tag) && !Tags.Any(t => t.Text == tag))
            {
                Tags.Add(new TagItem { Text = tag });
                OnPropertyChanged(nameof(TagsString));
            }
        }

        private static Color GetDominantColorFromBitmapSource(BitmapSource bitmap, int quantizeBitsPerChannel = 5)
        {
            if (bitmap == null) return Colors.Transparent;

            var converted = new FormatConvertedBitmap(bitmap, PixelFormats.Bgra32, null, 0);
            converted.Freeze();

            int w = converted.PixelWidth;
            int h = converted.PixelHeight;
            int stride = w * 4;
            byte[] pixels = new byte[h * stride];
            converted.CopyPixels(pixels, stride, 0);

            var counts = new Dictionary<int, int>();

            int shift = 8 - quantizeBitsPerChannel;
            for (int y = 0; y < h; y++)
            {
                int offset = y * stride;
                for (int x = 0; x < w; x++)
                {
                    byte b = pixels[offset + 0];
                    byte g = pixels[offset + 1];
                    byte r = pixels[offset + 2];
                    int rq = r >> shift;
                    int gq = g >> shift;
                    int bq = b >> shift;
                    int key = (rq << (2 * quantizeBitsPerChannel)) | (gq << quantizeBitsPerChannel) | bq;
                    if (counts.ContainsKey(key)) counts[key]++; else counts[key] = 1;
                    offset += 4;
                }
            }

            if (counts.Count == 0) return Colors.Transparent;

            var best = counts.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;

            int mask = (1 << quantizeBitsPerChannel) - 1;
            int br = (best >> (2 * quantizeBitsPerChannel)) & mask;
            int bg = (best >> quantizeBitsPerChannel) & mask;
            int bb = best & mask;

            byte r8 = (byte)((br << shift) | (br >> (quantizeBitsPerChannel - shift)));
            byte g8 = (byte)((bg << shift) | (bg >> (quantizeBitsPerChannel - shift)));
            byte b8 = (byte)((bb << shift) | (bb >> (quantizeBitsPerChannel - shift)));

            return Color.FromRgb(r8, g8, b8);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
