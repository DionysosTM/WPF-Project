using Microsoft.VisualBasic;
using projet_wpf.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Logique d'interaction pour EditTagsWindow.xaml
    /// </summary>
    public partial class EditTagsWindow : Window
    {
        public ObservableCollection<TagItem> Tags { get; set; }

        public EditTagsWindow(PhotoModel photo)
        {
            InitializeComponent();
            Tags = photo.Tags;
            DataContext = this;
        }

        private void AddTag_Click(object sender, RoutedEventArgs e)
        {
            var input = Microsoft.VisualBasic.Interaction.InputBox(
                "Entrez un nouveau tag :", "Ajouter un tag", "");
            if (!string.IsNullOrWhiteSpace(input))
            {
                Tags.Add(new TagItem { Text = input.Trim().ToLower() });
            }
        }

        private void EditTag_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBlock tb && tb.DataContext is TagItem tag)
            {
                var input = Interaction.InputBox("Modifiez le tag :", "Modifier un tag", tag.Text);
                if (!string.IsNullOrWhiteSpace(input))
                {
                    tag.Text = input.Trim().ToLower();
                    tag.NotifyPropertyChanged(nameof(tag.Text));
                }
            }
        }

        private void DeleteTag_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is TagItem tag)
            {
                Tags.Remove(tag);
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
