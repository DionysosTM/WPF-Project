using projet_wpf.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace projet_wpf.ViewModels
{
    public class SlideShowViewModel : BaseViewModel
    {
        // Variables
        private readonly ObservableCollection<PhotoModel> _photos;
        private int _currentIndex = 0;
        private readonly DispatcherTimer _timer;

        // Image en cours de diapo
        private BitmapImage _currentImage;
        public BitmapImage CurrentImage
        {
            get => _currentImage;
            set
            {
                _currentImage = value;
                OnPropertyChanged(nameof(CurrentImage));
            }
        }

        // Constructeur
        public SlideShowViewModel(ObservableCollection<PhotoModel> photos)
        {
            // Stock les photos
            _photos = photos;
            if (!_photos.Any())
                return;

            // Init DispatcherTimer pour faire des interval
            _timer = new DispatcherTimer {
                Interval = TimeSpan.FromSeconds(3)
            };
            _timer.Tick += (s, e) => ShowNextImage();

            // Montre first image + déclenche timer
            ShowImage(_photos[_currentIndex]);
            _timer.Start();
        }

        // Montrer l'image suivante dans la liste
        private void ShowNextImage()
        {
            _currentIndex = (_currentIndex + 1) % _photos.Count;
            ShowImage(_photos[_currentIndex]);
        }

        // Met l'image en paramètre en visuel
        private void ShowImage(PhotoModel photo)
        {
            CurrentImage = new BitmapImage(new Uri(photo.FilePath));
        }

        // Stop le timer (si échap ou autre)
        public void Stop()
        {
            _timer.Stop();
        }
    }
}