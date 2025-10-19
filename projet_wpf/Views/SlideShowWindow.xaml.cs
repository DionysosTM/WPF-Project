using projet_wpf.Models;
using projet_wpf.ViewModels;
using System.Collections.ObjectModel;
using System.Windows;

namespace projet_wpf.Views
{
    public partial class SlideShowWindow : Window
    {
        private readonly SlideShowViewModel _viewModel;

        // Constructeur du diapo
        public SlideShowWindow(ObservableCollection<PhotoModel> photos)
        {
            InitializeComponent();
            _viewModel = new SlideShowViewModel(photos);
            DataContext = _viewModel;
        }

        // Ferme la fenêtre si ECHAP (seule interaction possible)
        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
            {
                _viewModel.Stop();
                Close();
            }
        }
    }
}