using Microsoft.Win32;
using projet_wpf.Helpers;
using projet_wpf.Models;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace projet_wpf.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public ObservableCollection<PhotoModel> Photos { get; set; }
        public ObservableCollection<PhotoModel> VisiblePhotos { get; set; }

        public PhotoModel SelectedPhoto { get; set; }

        public ICommand ImportFilesCommand { get; }
        public ICommand ImportFolderCommand { get; }
        public ICommand StartSlideShowCommand { get; }
        public ICommand RotateImageCommand { get; }
        public ICommand AddTagCommand { get; }
        public ICommand EditTagsCommand { get; }

        public string orderType { get; set; }
        private string _selectedSortOption = "Par date";

        public ObservableCollection<string> FileTypes { get; } = new ObservableCollection<string>();
        private string _selectedFileType = "Tous";
        public ObservableCollection<string> ColorCategories { get; } = new ObservableCollection<string>();
        private string _selectedColorCategory;
        public string SelectedColorCategory
        {
            get => _selectedColorCategory;
            set
            {
                if (_selectedColorCategory == value) return;
                _selectedColorCategory = value;
                OnPropertyChanged(nameof(SelectedColorCategory));
                Filter(_selectedFileType, _selectedColorCategory);
            }
        }
        public string SelectedFileType
        {
            get => _selectedFileType;
            set
            {
                if (_selectedFileType == value) return;
                _selectedFileType = value;
                OnPropertyChanged(nameof(SelectedFileType));
                Filter(_selectedFileType, _selectedColorCategory);
            }
        }

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged(nameof(SearchText));
                    ApplyFilters();
                }
            }
        }


        public MainViewModel()
        {
            Photos = new ObservableCollection<PhotoModel>();
            VisiblePhotos= new ObservableCollection<PhotoModel>();
            ImportFilesCommand = new RelayCommand(ImportFiles);
            ImportFolderCommand = new RelayCommand(ImportFolder);
            orderType  = "Croissant";
            FileTypes.Add("Tous");
            FileTypes.Add(".jpg");
            FileTypes.Add(".jpeg");
            FileTypes.Add(".png");
            SelectedFileType = "Tous";
            StartSlideShowCommand = new RelayCommand(StartSlideShow, CanStartSlideShow); //2e param pour mettre en grisé le button si False
            RotateImageCommand = new RelayCommand(RotateImage);
            AddTagCommand = new RelayCommand(AddTagToPhoto);
            EditTagsCommand = new RelayCommand(EditTagsForPhoto);

            ColorCategories.Add("Tous");
            ColorCategories.Add("Noir");
            ColorCategories.Add("Blanc");
            ColorCategories.Add("Gris");
            ColorCategories.Add("Rouge");
            ColorCategories.Add("Orange");
            ColorCategories.Add("Jaune");
            ColorCategories.Add("Vert");
            ColorCategories.Add("Cyan");
            ColorCategories.Add("Bleu");
            ColorCategories.Add("Violet");
            ColorCategories.Add("Magenta");
            SelectedColorCategory = "Tous";
        }

        #region add photo
        private void AddPhoto(string file)
        {
            PhotoModel photo = new PhotoModel(file);
            Photos.Add(photo);
            if (photo.FileType == _selectedFileType || _selectedFileType == "Tous")
            {
                VisiblePhotos.Add(photo);
            }
            if (!FileTypes.Contains(photo.FileType)) // In case we change supported types later
            {
                FileTypes.Add(photo.FileType);
            }
        }
        #endregion add photo

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
                    {
                        AddPhoto(file);
                    }
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
                    {
                       AddPhoto(file);
                    }
                        
                }
            }
        }

        public void ToggleOrder()
        {
            if (orderType == "Croissant")
            {
                orderType = "Décroissant";
            }
            else
            {
                orderType = "Croissant";
            }

            SortBy(_selectedSortOption);
        }

        public void Filter(string fileType, string colorCategory)
        {
            _selectedFileType = fileType;
            _selectedColorCategory = colorCategory;
            List<PhotoModel> filteredList = Photos.ToList();
            if (_selectedFileType != "Tous" && _selectedFileType != null)
            {
                filteredList = filteredList.Where(photo => photo.FileType == _selectedFileType).ToList();
            }
            if (_selectedColorCategory != "Tous" && _selectedColorCategory != null)
            {
                filteredList = filteredList.Where(photo => ColorsHelper.GetColorCategory(photo.DominantColor) == _selectedColorCategory).ToList();
            }
            VisiblePhotos = new ObservableCollection<PhotoModel>(filteredList);
            OnPropertyChanged(nameof(VisiblePhotos));
            SortBy(_selectedSortOption);
        }

        public void SortBy(string sortType)
        {
            List<PhotoModel> sortedList = VisiblePhotos.ToList();
            switch (sortType)
            {
                case "Par nom":
                    sortedList = orderType == "Croissant" ? sortedList.OrderBy(VisiblePhotos => VisiblePhotos.FileName).ToList() 
                        : sortedList.OrderByDescending(VisiblePhotos => VisiblePhotos.FileName).ToList();
                    break;
                case "Par date":
                    sortedList = orderType == "Croissant" ? sortedList.OrderBy(VisiblePhotos => VisiblePhotos.DateAdded).ToList()
                        : sortedList.OrderByDescending(VisiblePhotos => VisiblePhotos.DateAdded).ToList();
                    break;
                case "Par type":
                    sortedList = orderType == "Croissant" ? sortedList.OrderBy(VisiblePhotos => VisiblePhotos.FileType).ToList() 
                        : sortedList.OrderByDescending(VisiblePhotos => VisiblePhotos.FileType).ToList();
                    break;
                case "Par taille":
                    sortedList = orderType == "Croissant" ? sortedList.OrderBy(VisiblePhotos => VisiblePhotos.FileSize).ToList() 
                        : sortedList.OrderByDescending(VisiblePhotos => VisiblePhotos.FileSize).ToList();
                    break;
                default:
                    break;
            }
            VisiblePhotos = new ObservableCollection<PhotoModel>(sortedList);
            OnPropertyChanged(nameof(VisiblePhotos));
            _selectedSortOption = sortType;
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

        private void ApplyFilters()
        {
            var filtered = Photos.ToList();

            if (_selectedFileType != "Tous" && _selectedFileType != null)
                filtered = filtered.Where(p => p.FileType == _selectedFileType).ToList();

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                string lower = SearchText.ToLower();
                filtered = filtered.Where(p =>
                    p.FileName.ToLower().Contains(lower)
                    || p.Tags.Any(t => t.Text.ToLower().Contains(lower))
                ).ToList();
            }

            VisiblePhotos = new ObservableCollection<PhotoModel>(filtered);
            OnPropertyChanged(nameof(VisiblePhotos));

            SortBy(_selectedSortOption);
        }

        private void AddTagToPhoto(object parameter)
        {
            if (parameter is PhotoModel photo)
            {
                var window = new Views.EditTagsWindow(photo);
                window.ShowDialog();
            }
        }

        private void EditTagsForPhoto(object parameter)
        {
            if (parameter is TagItem tag)
            {
                var photo = Photos.FirstOrDefault(p => p.Tags.Contains(tag));
                if (photo != null)
                {
                    var window = new Views.EditTagsWindow(photo);
                    window.ShowDialog();
                }
            }
        }

    }
}
