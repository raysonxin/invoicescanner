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

namespace QRCodeScanner
{
    public class Manufacturer
    {
        public string Company { get; set; }
        public List<Model> Models { get; set; }
    }

    public class Model
    {
        public string Name { get; set; }
        public string Ram { get; set; }
        public double price { get; set; }
        public string CPU { get; set; }
    }

    /// <summary>
    /// TestWindow.xaml 的交互逻辑
    /// </summary>
    public partial class TestWindow : Window
    {
        public TestWindow()
        {
            InitializeComponent();

            List<Manufacturer> ManufacturerList = new List<Manufacturer>();

            ManufacturerList.Add(new Manufacturer()
            {
                Company = "DEll",
                Models = new List<Model>(){new Model(){CPU = "T7250", Name = "Inspiron1525", price =234434 , Ram= "2048 MB" },
                                       new Model(){CPU = "T5750", Name = "Studio 1535", price =234443 , Ram= "2048 MB" },
                                       new Model(){CPU = "T5780", Name = "Vastro 1510", price =234434 , Ram= "2048 MB" },}
            });

            ManufacturerList.Add(new Manufacturer()
            {
                Company = "Lenovo",
                Models = new List<Model>(){new Model(){CPU = "T1230", Name = "l123", price =23546454 , Ram= "1024 MB" },
                                      new Model(){CPU = "T1230", Name = "l1423", price =2346456 , Ram= "1024 MB" },
                                      new Model(){CPU = "T1230", Name = "ldf123", price =2344646 , Ram= "1024 MB" },}
            });

            ManufacturerListBox.ItemsSource = ManufacturerList;
        }
    }
}
