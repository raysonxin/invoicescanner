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
using System.Windows.Shapes;

namespace QRCodeScanner
{
    /// <summary>
    /// SelectDraftView.xaml 的交互逻辑
    /// </summary>
    public partial class SelectDraftView : Window, INotifyPropertyChanged
    {
        public SelectDraftView(EventHandler onSelect)
        {
            InitializeComponent();
            if (onSelect != null)
            {
                OnSelectDraft = onSelect;
            }
            this.DataContext = this;
            this.Loaded += SelectDraftView_Loaded;
        }

        private ObservableCollection<DraftModel> draftList;
        public ObservableCollection<DraftModel> DraftList
        {
            get { return draftList; }
            set
            {
                draftList = value;
                RaisePropertyChanged("DraftList");
            }
        }

        private EventHandler OnSelectDraft;

        private void SelectDraftView_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var draftFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Yonghui\\InvoiceScanner\\";
                if (Directory.Exists(draftFolder))
                {
                    DirectoryInfo dirInfo = new DirectoryInfo(draftFolder);
                    var files = dirInfo.GetFiles();
                    if (files.Length == 0)
                        return;

                    var list = new List<DraftModel>();
                    files = files.OrderByDescending(f => f.CreationTime).ToArray();
                    for (int i = 0; i < files.Length; i++)
                    {
                        list.Add(new DraftModel
                        {
                            RowNumber = i + 1,
                            FileName = files[i].Name,
                            FullPath = files[i].FullName
                        });
                    }
                    DraftList = new ObservableCollection<DraftModel>(list);
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
            try
            {
                var dataContext = ((Button)sender).DataContext as DraftModel;
                if (OnSelectDraft != null)
                {
                    OnSelectDraft.Invoke(dataContext, null);
                }
                this.Close();
            }
            catch (Exception ex)
            {

            }
        }
    }
}
