using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeyboardTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnExpress_Click(object sender, EventArgs e)
        {
            SendKeys.SendWait("(" + DateTime.Now.ToString("MMddHHmmss") + ")");
        }

        private void btnInvoice_Click(object sender, EventArgs e)
        {
            Random rand = new Random();
            SendKeys.SendWait(string.Format("(01,10,{0},{1},100.00,20181029,06339552573889022162,CE35)", rand.Next(1000000, 9000000), rand.Next(10000, 90000)));
        }
    }
}
