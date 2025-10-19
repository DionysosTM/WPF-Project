using projet_wpf.ViewModels;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace projet_wpf.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }

        private void Sort(object sender, RoutedEventArgs e)
        {
            var radio = sender as RadioButton;
            if (radio == null || radio.Content == null) return;

            if (DataContext is MainViewModel vm)
            {
                vm.SortBy(radio.Content?.ToString());
            }
        }

        private void callToggleOrder(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.ToggleOrder();
            }
        }

        private void FilterByType(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                var comboBox = sender as ComboBox;
                if (comboBox != null && comboBox.SelectedItem != null)
                {
                    vm.FilterByFileType(comboBox.SelectedItem.ToString());
                }
            }
        }
    }
}