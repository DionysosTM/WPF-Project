using Microsoft.Win32;
using projet_wpf.Helpers;
using projet_wpf.Models;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace projet_wpf.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public ObservableCollection<PhotoModel> Photos { get; set; }
        public PhotoModel SelectedPhoto { get; set; }

        public ICommand ImportFilesCommand { get; }
        public ICommand ImportFolderCommand { get; }
        public ICommand StartSlideShowCommand { get; }
        public ICommand RotateImageCommand { get; }

        public MainViewModel()
        {
            Photos = new ObservableCollection<PhotoModel>();
            ImportFilesCommand = new RelayCommand(ImportFiles);
            ImportFolderCommand = new RelayCommand(ImportFolder);
            StartSlideShowCommand = new RelayCommand(StartSlideShow, CanStartSlideShow); //2e param pour mettre en grisé le button si False
            RotateImageCommand = new RelayCommand(RotateImage);
        }

        private void ImportFiles(object parameter)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Multiselect = true,
                Filter = "Images (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png"
            };

            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                foreach (var file in dialog.FileNames)
                {
                    if (!Photos.Any(p => p.FilePath == file))
                        Photos.Add(new PhotoModel(file));
                }
            }
        }

        private void ImportFolder(object parameter)
        {
            var dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                var files = Directory.GetFiles(dialog.SelectedPath, "*.*")
                    .Where(f => f.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)
                             || f.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase)
                             || f.EndsWith(".png", StringComparison.OrdinalIgnoreCase));

                foreach (var file in files)
                {
                    if (!Photos.Any(p => p.FilePath == file))
                        Photos.Add(new PhotoModel(file));
                }
            }
        }

        // Retourne False si aucune photo n'est dans la Grid
        private bool CanStartSlideShow(object parameter) => Photos.Count > 0;

        // Démarre la Window du diapo
        private void StartSlideShow(object parameter)
        {
            var window = new Views.SlideShowWindow(Photos);
            window.ShowDialog();
        }

        // Rotate 90 degrés
        private void RotateImage(object parameter)
        {
            if (parameter is PhotoModel photo)
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(photo.FilePath);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();

                var rotated = new TransformedBitmap(bitmap, new System.Windows.Media.RotateTransform(90));

                // Met à jour la propriété Thumbnail -> notifie la vue
                photo.RotateBy(90);
            }
        }
    }
}
