using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// <summary>
    /// SetNumberView.xaml 的交互逻辑
    /// </summary>
    public partial class SetNumberView : Window, INotifyPropertyChanged
    {
        public SetNumberView(EventHandler action, int number)
        {
            InitializeComponent();
            this.DataContext = this;
            if (action != null)
            {
                callbackAction = action;
            }
            CurrentNumber = number;
            NewNumber = number;
        }

        private EventHandler callbackAction;

        private int currentNumber;
        public int CurrentNumber
        {
            get { return currentNumber; }
            set
            {
                currentNumber = value;
                RaisePropertyChanged("CurrentNumber");
            }
        }


        private int newNumber;
        public int NewNumber
        {
            get { return newNumber; }
            set
            {
                newNumber = value;
                RaisePropertyChanged("NewNumber");
            }
        }

        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 属性改变事件
        /// </summary>
        /// <param name="name"></param>
        public void RaisePropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (NewNumber > 0)
            {
                callbackAction?.Invoke(NewNumber, null);
                this.Close();
            }
            else
            {
                MessageBox.Show("输入的数字不合法！");
            }
        }
    }
}
