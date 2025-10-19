using projet_wpf.Models;
using projet_wpf.ViewModels;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media.Animation;

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

            AttachFadeAnimation();
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

        // Créer l'animation
        private void AttachFadeAnimation()
        {
            _viewModel.PropertyChanged += (s, args) =>
            {
                if (args.PropertyName == nameof(_viewModel.CurrentPhoto))
                {
                    // Redémarre l'animation de fondu
                    var fade = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(1));
                    DiapoImage.BeginAnimation(UIElement.OpacityProperty, fade);
                }
            };
        }

    }
}