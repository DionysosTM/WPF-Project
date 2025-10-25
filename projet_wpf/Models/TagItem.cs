using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projet_wpf.Models
{
    public class TagItem : INotifyPropertyChanged
    {
        [Key]
        public int Id { get; set; }

        public int PhotoModelId { get; set; }
        public PhotoModel Photo { get; set; }



        private string _text;
        public string Text
        {
            get => _text;
            set
            {
                if (_text != value)
                {
                    _text = value;
                    NotifyPropertyChanged(nameof(Text));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
