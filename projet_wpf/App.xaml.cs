using projet_wpf.DataAccess;
using projet_wpf.Models;
using projet_wpf.ViewModels;
using System.Configuration;
using System.Data;
using System.Windows;

namespace projet_wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            using (var db = new AppDbContext())
            {
                db.Database.EnsureCreated();

                var photos = db.Photos
                               .Where(p => !p.IsDeleted)
                               .ToList();

                foreach (var photo in photos)
                {
                    var tags = db.Tags.Where(t => t.PhotoModelId == photo.Id).ToList();
                    photo.Tags = new System.Collections.ObjectModel.ObservableCollection<TagItem>(tags);

                    photo.ReloadImages();
                }

                var viewModel = new MainViewModel(photos);

                var mainWindow = new Views.MainWindow
                {
                    DataContext = viewModel
                };

                mainWindow.Show();
            }
        }
    }

}
