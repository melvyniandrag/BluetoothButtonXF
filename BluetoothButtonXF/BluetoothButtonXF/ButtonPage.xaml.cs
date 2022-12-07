using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BluetoothButtonXF
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ButtonPage : ContentPage
    {

        private Color _buttonStatusColor = Color.Green;
        public Color ButtonStatusColor
        {
            get => _buttonStatusColor;
            set
            {
                _buttonStatusColor = value;
                OnPropertyChanged(nameof(ButtonStatusColor));
            }

        }
        public ButtonPage()
        {
            InitializeComponent();
            BindingContext = this;
        }
    }
}