using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
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

    public delegate bool AddCallbackDelegate(InvoiceModel model);

    /// <summary>
    /// NewView.xaml 的交互逻辑
    /// </summary>
    public partial class NewView : Window, INotifyPropertyChanged
    {
        public NewView(AddCallbackDelegate action, InvoiceModel one = null)
        {
            InitializeComponent();
            this.DataContext = this;
            if (action != null)
            {
                callbackAction = action;
            }
            if (one != null)
            {
                this.Model = one;

                //DateTimeFormatInfo dtFormat = new DateTimeFormatInfo();
                //dtFormat.ShortDatePattern = "yyyyMMdd";

                dpDate.SelectedDate = DateTime.ParseExact(model.MakeDate, "yyyyMMdd", null);//Convert.ToDateTime(model.MakeDate,dtFormat);
            }
        }

        private AddCallbackDelegate callbackAction { get; set; }

        private InvoiceModel model = new InvoiceModel();
        public InvoiceModel Model
        {
            get { return model; }
            set
            {
                model = value;
                RaisePropertyChanged("Model");
            }
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Code))
                {
                    MessageBox.Show("发票代码不能为空！");
                    return;
                }

                if (string.IsNullOrEmpty(model.Number))
                {
                    MessageBox.Show("发票号码不能为空！");
                    return;
                }

                if (string.IsNullOrEmpty(model.Amount.ToString()))
                {
                    MessageBox.Show("发票金额不能为空！");
                    return;
                }

                //if (string.IsNullOrEmpty(model.MakeDate))
                //{
                //    MessageBox.Show("开票日期不能为空！");
                //    return;
                //}

                model.MakeDate = ((DateTime)dpDate.SelectedDate).ToString("yyyyMMdd");
                if (callbackAction == null)
                {
                    this.Close();
                    return;
                }

                if (callbackAction.Invoke(model))
                {
                    this.Close();
                }
            }
            catch (Exception ex)
            {

            }
        }

        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 属性改变事件
        /// </summary>
        /// <param name="name"></param>
        private void RaisePropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
        #endregion
    }
}
