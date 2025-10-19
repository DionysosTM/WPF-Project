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

            var mainWindow = new Views.MainWindow();
            var viewModel = new ViewModels.MainViewModel();

            mainWindow.DataContext = viewModel;
            mainWindow.Show();
        }
    }

}
