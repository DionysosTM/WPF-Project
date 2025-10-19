using Microsoft.Win32;
using projet_wpf.Helpers;
using projet_wpf.Models;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using System.Windows.Forms;

namespace projet_wpf.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public ObservableCollection<PhotoModel> Photos { get; set; }
        public PhotoModel SelectedPhoto { get; set; }

        public ICommand ImportFilesCommand { get; }
        public ICommand ImportFolderCommand { get; }

        public MainViewModel()
        {
            Photos = new ObservableCollection<PhotoModel>();
            ImportFilesCommand = new RelayCommand(ImportFiles);
            ImportFolderCommand = new RelayCommand(ImportFolder);
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

    }
}
