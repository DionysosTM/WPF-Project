using projet_wpf.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace projet_wpf.Views
{
    /// <summary>
    /// Logique d'interaction pour PhotoViewWindow.xaml
    /// </summary>
    public partial class PhotoViewWindow : Window
    {
        public PhotoViewWindow(PhotoModel photo)
        {
            InitializeComponent();
            DataContext = photo;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                Close();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }
    }
}
