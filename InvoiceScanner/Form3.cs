using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InvoiceScanner
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
            this.Load += Form3_Load;
            this.FormClosing += Form3_FormClosing;
        }

        private void Form3_FormClosing(object sender, FormClosingEventArgs e)
        {
            //       throw new NotImplementedException();
            comm1.Close();
        }

        private void Form3_Load(object sender, EventArgs e)
        {

            //二维码驱动方法
            comm1.DataReceivedQRCode += new RQCodeComDevice.EventHandleQRCode(getQRCodeValue);
            comm1.Open();
            
        }

        public RQCodeComDevice comm1 = new RQCodeComDevice();


        /// <summary>
        /// 读取二维码的数据
        /// </summary>
        /// <param name="text"></param>
        public void getQRCodeValue(string text)
        {
            try
            {
                if (!string.IsNullOrEmpty(text))
                {
                    text = text.Trim();
                    //这里就是读取到的内容，可以进行各种业务处理
                    System.Diagnostics.Debug.WriteLine(text);
                }
            }
            catch (Exception)
            {
            }
        }

    }
}
