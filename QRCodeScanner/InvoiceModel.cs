using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QRCodeScanner
{
    public class InvoiceModel
    {
        private Dictionary<string, string> typeDic = new Dictionary<string, string>() {
            { "10","增值税电子普通发票"},
            { "04","增值税普通发票"},
            {"01","增值税专用发票" }
        };

        public int RowNumber { get; set; }

        public string Code { get; set; }

        public string Number { get; set; }

        //public string TypeCode { get; set; }

        //public string TypeName
        //{
        //    get
        //    {
        //        return typeDic[this.TypeCode];
        //    }
        //}

        public decimal Amount { get; set; }

        public string MakeDate { get; set; }

        public string CheckNumber { get; set; }

        public string ScanDate { get; set; }

        public string ScanTime { get; set; }

        public string BatchCode { get; set; }

        public string Remark { get; set; }
    }
}
