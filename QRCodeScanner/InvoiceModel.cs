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

        public string Amount { get; set; }

        public string MakeDate { get; set; }

        public string CheckNumber { get; set; }

        public string ScanDate { get; set; }

        public string ScanTime { get; set; }

        public string Remark { get; set; }

        /// <summary>
        /// 张数/份
        /// </summary>
        public string PageCount { get; set; }

        /// <summary>
        /// 公司
        /// </summary>
        public string Company { get; set; }

        /// <summary>
        /// Flow
        /// </summary>
        public string Flow { get; set; }

        /// <summary>
        /// 专票
        /// </summary>
        public string SpecialTicket { get; set; }

        /// <summary>
        /// 备注1
        /// </summary>
        public string Remark1 { get; set; }

        /// <summary>
        /// 备注2
        /// </summary>
        public string Remark2 { get; set; }
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

    public class DraftModel
    {
        /// <summary>
        /// 行号
        /// </summary>
        public int RowNumber { get; set; }

        /// <summary>
        /// 文件名称路径
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 完整路径
        /// </summary>
        public string FullPath { get; set; }
    }
}
