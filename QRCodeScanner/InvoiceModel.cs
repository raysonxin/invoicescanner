using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QRCodeScanner
{

    public class BaseModel : INotifyPropertyChanged
    {
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
    }

    public class InvoiceModel : BaseModel
    {
        /// <summary>
        /// 快递单号
        /// </summary>
        public string PkgNumber { get; set; }

        /// <summary>
        /// 发票在快递包裹内编号
        /// </summary>
        public int PkgIndex { get; set; }

        private int rowNumber;
        /// <summary>
        /// 总序号
        /// </summary>
        public int RowNumber
        {
            get { return rowNumber; }
            set
            {
                rowNumber = value;
                RaisePropertyChanged("RowNumber");
            }
        }

        public string Code { get; set; }

        public string Number { get; set; }

        public decimal Amount { get; set; }

        public string MakeDate { get; set; }

        public string CheckNumber { get; set; }

        public string ScanDate { get; set; }

        public string ScanTime { get; set; }

        public string Remark { get; set; }
    }

    public class PackageModel
    {
        /// <summary>
        /// 快递单号
        /// </summary>
        public string PkgNumber { get; set; }

        /// <summary>
        /// 发票列表
        /// </summary>
        public List<InvoiceModel> InvoiceList { get; set; }
    }
}
