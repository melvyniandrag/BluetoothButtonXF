using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;

namespace BluetoothButtonXF.Util
{
    public class BTDevice : INotifyPropertyChanged
    {
        public Color TextColor { get; set; }

        public Color BackgroundColor { get; set; }

        public String DeviceName { get; set; }
        
        public String DeviceMAC { get; set; }

        public Boolean IsSelected { get; set; } = false;

        // event handler for updating the list views
        public event PropertyChangedEventHandler PropertyChanged;
        
        public void OnPropertyChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(""));
        }
    }
}
