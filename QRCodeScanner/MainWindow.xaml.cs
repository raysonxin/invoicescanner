﻿using Microsoft.Win32;
using Newtonsoft.Json;
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
using System.Windows.Controls.Primitives;
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
            invoiceList.CollectionChanged += InvoiceList_CollectionChanged;
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

        /// <summary>
        /// 发票序列号起始编号
        /// </summary>
        private int baseNumber = 1;

        private ScanerHook listener = new ScanerHook();

        private ObservableCollection<PackageModel> packageList = new ObservableCollection<PackageModel>();
        public ObservableCollection<PackageModel> PackageList
        {
            get { return packageList; }
            set
            {
                packageList = value;
                RaisePropertyChanged("PackageList");
            }
        }

        private bool isNeedSave = false;
        /// <summary>
        /// 是否需要保存
        /// </summary>
        public bool IsNeedSave
        {
            get { return isNeedSave; }
            set
            {
                isNeedSave = value;
                RaisePropertyChanged("IsNeedSave");
            }
        }


        /// <summary>
        /// 所有发票列表
        /// </summary>
        public ObservableCollection<InvoiceModel> invoiceList = new ObservableCollection<InvoiceModel>();

        /// <summary>
        /// 当前快递单号
        /// </summary>
        private string pkgNumber = string.Empty;

        /// <summary>
        /// 当前包裹中发票序号
        /// </summary>
        private int pkgIndex = 1;

        private void Listener_ScanerEvent(ScanerHook.ScanerCodes codes)
        {
            try
            {
                string content = codes.Result;

                //快递单号
                if (!content.Contains(",") && !content.Contains("，"))
                {
                    pkgNumber = content;
                    InsertOne(pkgNumber);
                    return;
                }

                //分析发票号码
                var items = content.Split(',', '，');
                if (items.Length < 6)
                    return;

                var model = new InvoiceModel
                {
                    PkgNumber = pkgNumber,
                    PkgIndex = pkgIndex++,
                    Code = items[2],
                    Number = items[3],
                    Amount = items[4],
                    MakeDate = items[5],
                    CheckNumber = items[6],
                    ScanDate = DateTime.Now.ToString("yyyy-MM-dd"),
                    ScanTime = DateTime.Now.ToString("HH:mm"),
                    Remark = "自动",
                };

                InsertOne("", model);
            }
            catch (Exception ex)
            {

            }
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
                    //model.PkgIndex = pkgIndex;

                    return InsertOne("", obj as InvoiceModel);
                }
                return false;
            }, null, this.pkgNumber);
            view.ShowDialog();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //invoiceList
                var json = JsonConvert.SerializeObject(invoiceList);
                var draftFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Yonghui\\InvoiceScanner\\";
                if (!Directory.Exists(draftFolder))
                {
                    Directory.CreateDirectory(draftFolder);
                }
                string name = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + "-手动保存.draft";
                using (var fs = new FileStream(draftFolder + name, FileMode.OpenOrCreate))
                {
                    var buffer = Encoding.UTF8.GetBytes(json);
                    fs.Write(buffer, 0, buffer.Length);
                }
                IsNeedSave = false;
            }
            catch (Exception ex)
            {

            }
        }

        private void btnOpenDraft_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var view = new SelectDraftView((o, args) =>
                {

                    if (o is DraftModel)
                    {
                        var model = o as DraftModel;
                        using (var fs = new FileStream(model.FullPath, FileMode.Open))
                        {
                            var buffer = new byte[fs.Length];

                            fs.Read(buffer, 0, buffer.Length);
                            var json = Encoding.UTF8.GetString(buffer);
                            var list = JsonConvert.DeserializeObject<List<InvoiceModel>>(json);
                            if (list != null)
                            {
                                invoiceList.Clear();

                                list.ForEach(f =>
                                {
                                    invoiceList.Add(f);
                                });
                            }
                        }
                    }

                });
                view.ShowDialog();
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// 添加发票信息
        /// </summary>
        /// <param name="one">发票信息实体</param>
        /// <returns></returns>
        private bool InsertOne(string pkgNo, InvoiceModel one = null)
        {
            if (!String.IsNullOrEmpty(pkgNo))
            {
                var pkg = PackageList.SingleOrDefault(f => f.PkgNumber == pkgNo);
                if (pkg == null)
                {
                    pkgIndex = 1;
                    PackageList.Add(new PackageModel { PkgNumber = pkgNo, InvoiceList = new List<InvoiceModel>() });
                    return true;
                }
                else
                {
                    pkgIndex = pkg.InvoiceList.Count + 1;
                }

                return true;
            }

            if (one == null)
                return true;

            if (invoiceList.Count > 0)
            {
                var exist = invoiceList.Where(f => f.Number == one.Number);
                if (exist.Count() != 0)
                {
                    if (MessageBox.Show(string.Format("发票号码：{0} 已存在，是否重复添加？", one.Number), "扫描提示", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.No)
                    {
                        return false;
                    }
                }
            }

            if (isAllowInsert)
            {
                //invoiceList.Where(f => f.PkgNumber == one.PkgNumber && f.PkgIndex >= one.PkgIndex).ToList();
                invoiceList.Insert(insertIndex, one);

                IsAllowInsert = false;
            }
            else
            {
                invoiceList.Add(one);
            }
            IsNeedSave = true;
            return true;
        }

        private bool UpdateOne(InvoiceModel one)
        {
            if (invoiceList.Count > 0)
            {
                var exist = invoiceList.SingleOrDefault(f => f.RowNumber == one.RowNumber);
                if (exist != null)
                {
                    var index = invoiceList.IndexOf(exist);
                    invoiceList[index] = one;
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
            var model = ((Button)sender).DataContext as InvoiceModel;
            if (MessageBox.Show("确定删除发票号码为：" + model.Code + " 的信息吗？", "操作提示", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                var cur = invoiceList.SingleOrDefault(f => f.RowNumber == model.RowNumber);
                invoiceList.Remove(cur);
            }
        }

        private void InvoiceList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            try
            {
                if (invoiceList.Count <= 0)
                    return;

                var temp = invoiceList.GroupBy(f => f.PkgNumber).Select(g => new PackageModel
                {
                    PkgNumber = g.Key,
                    InvoiceList = g.ToList()
                }).ToList();

                PackageList = new ObservableCollection<PackageModel>(temp);
                UpdateRowNumber();

                var scroll = GetChildObject<ScrollViewer>(ManufacturerListBox, "ScrollViewer");
                scroll.ScrollToBottom();
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// 查找子元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public T GetChildObject<T>(DependencyObject obj, string name) where T : FrameworkElement
        {
            DependencyObject child = null;
            T grandChild = null;


            for (int i = 0; i <= VisualTreeHelper.GetChildrenCount(obj) - 1; i++)
            {
                child = VisualTreeHelper.GetChild(obj, i);


                if (child is T && (((T)child).Name == name | string.IsNullOrEmpty(name)))
                {
                    return (T)child;
                }
                else
                {
                    grandChild = GetChildObject<T>(child, name);
                    if (grandChild != null)
                        return grandChild;
                }
            }
            return null;
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

        /// <summary>
        /// 导出excel文件
        /// </summary
        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                var list = PackageList.Select(f => f.InvoiceList).ToList();
                if (list == null || list.Count == 0)
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
                rowHead.CreateCell(index++, CellType.String).SetCellValue("快递单号");
                rowHead.CreateCell(index++, CellType.String).SetCellValue("张数");
                rowHead.CreateCell(index++, CellType.String).SetCellValue("发票序号");
                rowHead.CreateCell(index++, CellType.String).SetCellValue("发票代码");
                rowHead.CreateCell(index++, CellType.String).SetCellValue("发票号码");
                rowHead.CreateCell(index++, CellType.String).SetCellValue("合计金额");
                rowHead.CreateCell(index++, CellType.String).SetCellValue("开票日期");
                rowHead.CreateCell(index++, CellType.String).SetCellValue("扫描日期");
                rowHead.CreateCell(index++, CellType.String).SetCellValue("扫描时间");
                rowHead.CreateCell(index++, CellType.String).SetCellValue("备注");
                rowHead.CreateCell(index++, CellType.String).SetCellValue("张数/份");
                rowHead.CreateCell(index++, CellType.String).SetCellValue("公司");
                rowHead.CreateCell(index++, CellType.String).SetCellValue("Flow");
                rowHead.CreateCell(index++, CellType.String).SetCellValue("专票");
                rowHead.CreateCell(index++, CellType.String).SetCellValue("备注1");

                rowHead.CreateCell(index++, CellType.String).SetCellValue("备注2");

                int startRow = 1;
                int columnIndex = 0;
                //填写内容
                for (int i = 0; i < PackageList.Count; i++)
                {
                    if (PackageList[i].InvoiceList.Count == 0)
                        continue;

                    for (int j = 0; j < PackageList[i].InvoiceList.Count; j++)
                    {
                        var item = PackageList[i].InvoiceList[j];
                        IRow row = sheet.CreateRow(startRow + j);

                        columnIndex = 0;

                        if (j == 0)
                        {
                            row.CreateCell(columnIndex++, CellType.String).SetCellValue(item.PkgNumber);
                        }
                        else
                        {
                            columnIndex++;
                        }

                        row.CreateCell(columnIndex++, CellType.String).SetCellValue(item.PkgIndex);
                        row.CreateCell(columnIndex++, CellType.String).SetCellValue(item.RowNumber);
                        row.CreateCell(columnIndex++, CellType.String).SetCellValue(item.Code);
                        row.CreateCell(columnIndex++, CellType.String).SetCellValue(item.Number);
                        row.CreateCell(columnIndex++, CellType.String).SetCellValue(item.Amount.ToString());
                        row.CreateCell(columnIndex++, CellType.String).SetCellValue(item.MakeDate);
                        row.CreateCell(columnIndex++, CellType.String).SetCellValue(item.ScanDate);
                        row.CreateCell(columnIndex++, CellType.String).SetCellValue(item.ScanTime);
                        row.CreateCell(columnIndex++, CellType.String).SetCellValue(item.Remark);
                        row.CreateCell(columnIndex++, CellType.String).SetCellValue(item.PageCount);
                        row.CreateCell(columnIndex++, CellType.String).SetCellValue(item.Company);
                        row.CreateCell(columnIndex++, CellType.String).SetCellValue(item.Flow);
                        row.CreateCell(columnIndex++, CellType.String).SetCellValue(item.SpecialTicket);
                        row.CreateCell(columnIndex++, CellType.String).SetCellValue(item.Remark1);
                        row.CreateCell(columnIndex++, CellType.String).SetCellValue(item.Remark2);
                    }

                    sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(startRow, startRow + PackageList[i].InvoiceList.Count - 1, 0, 0));
                    startRow += PackageList[i].InvoiceList.Count;
                }

                for (int i = 0; i < columnIndex; i++)
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
            catch (Exception ex)
            {
                MessageBox.Show("导出失败，请检查文件是否被占用!", "操作提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        /// <summary>
        /// 更新发票总体序列号和张数
        /// </summary>
        private void UpdateRowNumber()
        {
            try
            {
                if (PackageList.Count == 0)
                    return;

                var currentIndex = baseNumber;
                for (int i = 0; i < PackageList.Count; i++)
                {
                    if (PackageList[i].InvoiceList.Count == 0)
                        continue;

                    int innerIndex = 1;

                    for (int j = 0; j < PackageList[i].InvoiceList.Count; j++)
                    {
                        PackageList[i].InvoiceList[j].RowNumber = currentIndex++;
                        PackageList[i].InvoiceList[j].PkgIndex = innerIndex++;
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void btnSetRowNumber_Click(object sender, RoutedEventArgs e)
        {
            SetNumberView view = new SetNumberView((obj, args) =>
            {
                baseNumber = (int)obj;
                UpdateRowNumber();
            }, baseNumber);
            view.ShowDialog();
        }

        #region 测试部分代码
        private void Test(string codes)
        {
            try
            {
                string content = codes;

                //快递单号
                if (!content.Contains(",") && !content.Contains("，"))
                {
                    pkgNumber = content;
                    InsertOne(pkgNumber);
                    return;
                }

                //分析发票号码
                var items = content.Split(',', '，');
                if (items.Length < 6)
                    return;

                var model = new InvoiceModel
                {
                    PkgNumber = pkgNumber,
                    PkgIndex = pkgIndex++,
                    Code = items[2],
                    Number = items[3],
                    Amount = items[4],
                    MakeDate = items[5],
                    CheckNumber = items[6],
                    ScanDate = DateTime.Now.ToString("yyyy-MM-dd"),
                    ScanTime = DateTime.Now.ToString("HH:mm"),
                    Remark = "自动",
                };

                InsertOne("", model);
            }
            catch (Exception ex)
            {

            }
        }

        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            // System.Windows.Forms.SendKeys
            Test(DateTime.Now.ToString("MMddHHmmss"));
        }

        private void btnTest1_Click(object sender, RoutedEventArgs e)
        {
            //01,10,011001800211,35367871,100.00,20181029,06339552573889022162,CE35,
            Random rand = new Random();
            Test(string.Format("01,10,{0},{1},100.00,20181029,06339552573889022162,CE35", rand.Next(1000000, 9000000), rand.Next(10000, 90000)));
        }
        #endregion

        #region 行间插入数据

        /// <summary>
        /// 是否允许插入数据
        /// </summary>
        private bool isAllowInsert = false;
        public bool IsAllowInsert
        {
            get { return isAllowInsert; }
            set
            {
                isAllowInsert = value;
                RaisePropertyChanged("IsAllowInsert");
                InsertBefore = false;
                InsertAfter = false;
            }
        }

        /// <summary>
        /// 当前选中的行
        /// </summary>
        private InvoiceModel currentInvoice;

        private int insertIndex = 0;

        /// <summary>
        /// 插入位置
        /// </summary>
        private InsertPosition insertPosition = InsertPosition.After;

        enum InsertPosition
        {
            Before = 0,
            After
        }

        private bool insertBefore = false;
        public bool InsertBefore
        {
            get { return insertBefore; }
            set
            {
                insertBefore = value;
                RaisePropertyChanged("InsertBefore");
            }
        }

        private bool insertAfter = false;
        public bool InsertAfter
        {
            get { return insertAfter; }
            set
            {
                insertAfter = value;
                RaisePropertyChanged("InsertAfter");
            }
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                currentInvoice = ((ListBox)sender).SelectedItem as InvoiceModel;
                if (currentInvoice != null)
                {
                    pkgNumber = currentInvoice.PkgNumber;
                    insertIndex = invoiceList.IndexOf(currentInvoice) + 1;
                    IsAllowInsert = true;

                    InsertAfter = true;
                    InsertBefore = false;
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void btnInsertRow_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                isAllowInsert = true;
                var menuItem = sender as MenuItem;

                var tag = menuItem.Tag.ToString();
                if (tag == "Before")
                {
                    insertPosition = InsertPosition.Before;
                    insertIndex--;
                    InsertAfter = false;
                    InsertBefore = true;
                }
                else
                {
                    insertPosition = InsertPosition.After;
                    InsertAfter = true;
                    InsertBefore = false;
                }
            }
            catch (Exception)
            {

            }
        }

        private void btnCancelInsert_Click(object sender, RoutedEventArgs e)
        {
            IsAllowInsert = false;
           
        }
        #endregion
    }
}
