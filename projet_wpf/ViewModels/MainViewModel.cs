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

namespace projet_wpf.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public ObservableCollection<PhotoModel> Photos { get; set; }
        public ObservableCollection<PhotoModel> VisiblePhotos { get; set; }

        public PhotoModel SelectedPhoto { get; set; }

        public ICommand ImportFilesCommand { get; }
        public ICommand ImportFolderCommand { get; }

        public string orderType { get; set; }
        private string _selectedSortOption = "Par date";

        public ObservableCollection<string> FileTypes { get; } = new ObservableCollection<string>();
        private string _selectedFileType = "Tous";
        public string SelectedFileType
        {
            get => _selectedFileType;
            set
            {
                if (_selectedFileType == value) return;
                _selectedFileType = value;
                OnPropertyChanged(nameof(SelectedFileType));
                FilterByFileType(_selectedFileType);
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

        public void FilterByFileType(string fileType)
        {
            _selectedFileType = fileType;
            List<PhotoModel> filteredList = Photos.ToList();
           
            if(_selectedFileType != "Tous" && _selectedFileType != null)
            {
                filteredList = filteredList.Where(photo => photo.FileType == _selectedFileType).ToList();
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

    }
}
