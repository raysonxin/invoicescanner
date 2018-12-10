using Microsoft.Win32;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace QRCodeScanner
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MainWindow()
        {
            InitializeComponent();
            listener.ScanerEvent += Listener_ScanerEvent;
            this.Loaded += MainWindow_Loaded;
            this.Unloaded += MainWindow_Unloaded;
            this.DataContext = this;
        }

        private void MainWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            listener.Stop();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            listener.Start();
        }

        private ScanerHook listener = new ScanerHook();

        private bool isBusy = false;

        private ObservableCollection<InvoiceModel> invoiceList = new ObservableCollection<InvoiceModel>();
        public ObservableCollection<InvoiceModel> InvoiceList
        {
            get { return invoiceList; }
            set
            {
                invoiceList = value;
                RaisePropertyChanged("InvoiceList");
            }
        }

        //public List<InvoiceModel> invoiceList = new List<InvoiceModel>();

        private void Listener_ScanerEvent(ScanerHook.ScanerCodes codes)
        {
            var items = codes.Result.Split(',', '，');

            if (items.Length < 6)
                return;

            var model = new InvoiceModel
            {
                //   TypeCode = items[1],
                Code = items[2],
                Number = items[3],
                Amount = Convert.ToDecimal(items[4]),
                MakeDate = items[5],
                CheckNumber = items[6],
                ScanDate = DateTime.Now.ToString("yyyy-MM-dd"),
                ScanTime = DateTime.Now.ToString("HH:mm"),
                Remark = "自动",
            };

            InsertOne(model);
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            NewView view = new NewView((obj) =>
            {
                if (obj != null && obj is InvoiceModel)
                {
                    var model = obj as InvoiceModel;

                    model.Remark = "手动";
                    model.ScanDate = DateTime.Now.ToString("yyyy-MM-dd");
                    model.ScanTime = DateTime.Now.ToString("HH:mm");

                    return InsertOne(obj as InvoiceModel);
                }
                return false;
            });
            view.ShowDialog();
        }

        private Boolean InsertOne(InvoiceModel one)
        {
            if (invoiceList.Count > 0)
            {
                var exist = InvoiceList.Where(f => f.Number == one.Number && f.Code == one.Code);
                if (exist.Count() != 0)
                {
                    //listener.Stop();
                    //isBusy = true;
                    if (MessageBox.Show(string.Format("发票号：{0} 已存在，是否添加？", one.Code), "扫描提示", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.No)
                    {
                        //  listener.Start();
                        // isBusy = false;
                        return false;
                    }
                }
            }

            one.RowNumber = invoiceList.Count > 0 ? invoiceList.Max(f => f.RowNumber) + 1 : 1;
            invoiceList.Add(one);
            return true;
        }

        private Boolean UpdateOne(InvoiceModel one)
        {
            if (invoiceList.Count > 0)
            {
                var exist = InvoiceList.SingleOrDefault(f => f.RowNumber == one.RowNumber);
                if (exist != null)
                {
                    var index = InvoiceList.IndexOf(exist);
                    InvoiceList[index] = one;
                }
            }

            return true;
        }

        private void btnModify_Click(object sender, RoutedEventArgs e)
        {
            var model = (sender as Button).DataContext as InvoiceModel;
            NewView view = new NewView((obj) =>
            {
                if (obj != null && obj is InvoiceModel)
                {
                    return UpdateOne(obj as InvoiceModel);
                }
                return false;
            }, model);
            view.ShowDialog();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var model = (sender as Button).DataContext as InvoiceModel;
            var cur = InvoiceList.SingleOrDefault(f => f.RowNumber == model.RowNumber);
            InvoiceList.Remove(cur);
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

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            if (invoiceList.Count == 0)
            {
                MessageBox.Show("未输入任何内容！");
                return;
            }

            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.RestoreDirectory = true;
            fileDialog.Filter = "Excel(2007-2013)|*.xlsx|Excel(97-2003)|*.xls";
            if (fileDialog.ShowDialog() != true)
            {
                return;
            }


            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("sheet1");
            IRow rowHead = sheet.CreateRow(0);

            int index = 0;
            rowHead.CreateCell(index++, CellType.String).SetCellValue("NO");
            rowHead.CreateCell(index++, CellType.String).SetCellValue("批次号");
            rowHead.CreateCell(index++, CellType.String).SetCellValue("发票代码");
            rowHead.CreateCell(index++, CellType.String).SetCellValue("发票号码");
            rowHead.CreateCell(index++, CellType.String).SetCellValue("合计金额");
            rowHead.CreateCell(index++, CellType.String).SetCellValue("开票日期");
            rowHead.CreateCell(index++, CellType.String).SetCellValue("扫描日期");
            rowHead.CreateCell(index++, CellType.String).SetCellValue("扫描时间");
            rowHead.CreateCell(index++, CellType.String).SetCellValue("备注");

            //填写内容
            for (int i = 0; i < invoiceList.Count; i++)
            {
                int columnIndex = 0;
                IRow row = sheet.CreateRow(i + 1);

                row.CreateCell(columnIndex++, CellType.String).SetCellValue(invoiceList[i].RowNumber);
                row.CreateCell(columnIndex++, CellType.String).SetCellValue(invoiceList[i].BatchCode);
                row.CreateCell(columnIndex++, CellType.String).SetCellValue(invoiceList[i].Code);
                row.CreateCell(columnIndex++, CellType.String).SetCellValue(invoiceList[i].Number);
                row.CreateCell(columnIndex++, CellType.String).SetCellValue(invoiceList[i].Amount.ToString());
                row.CreateCell(columnIndex++, CellType.String).SetCellValue(invoiceList[i].MakeDate);
                row.CreateCell(columnIndex++, CellType.String).SetCellValue(invoiceList[i].ScanDate);
                row.CreateCell(columnIndex++, CellType.String).SetCellValue(invoiceList[i].ScanTime);
                row.CreateCell(columnIndex++, CellType.String).SetCellValue(invoiceList[i].Remark);
            }

            for (int i = 0; i < invoiceList.Count; i++)
            {
                sheet.AutoSizeColumn(i);
            }

            sheet.DisplayGridlines = true;

            using (FileStream stream = File.OpenWrite(fileDialog.FileName))
            {
                workbook.Write(stream);
                stream.Close();
            }
            MessageBox.Show("导出数据成功!", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            GC.Collect();
        }
    }
}
