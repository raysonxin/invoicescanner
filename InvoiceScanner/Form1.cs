using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InvoiceScanner
{
    public partial class Form1 : Form
    {
        private ScanerHook listener = new ScanerHook();
        public Form1()
        {
            InitializeComponent();
            listener.ScanerEvent += Listener_ScanerEvent;
        }

        public List<InvoiceModel> invoiceList = new List<InvoiceModel>();

        private void Listener_ScanerEvent(ScanerHook.ScanerCodes codes)
        {
            var items = codes.Result.Split(',', '，');

            if (invoiceList.Count > 0)
            {
                var exist = invoiceList.Exists(f => f.Code == items[2]);
                if (exist)
                {
                    if (MessageBox.Show("发票号：{0}已存在，是否添加？", "扫描提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.No)
                    {
                        return;
                    }
                }
            }

            var model = new InvoiceModel
            {
                TypeCode = items[1],
                Code = items[2],
                Number = items[3],
                Amount = Convert.ToDecimal(items[4]),
                MakeDate = items[5],
                CheckNumber = items[6],
                ScanDate = DateTime.Now.ToString("yyyy-MM-dd"),
                ScanTime = DateTime.Now.ToString("HH:mm"),
                Remark = "自动",
            };

            invoiceList.Add(model);
            var index = 1;
            invoiceList.ForEach(f => f.RowNumber = index++);

            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = invoiceList;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            listener.Start();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            listener.Stop();
        }

        private void tsmiExport_Click(object sender, EventArgs e)
        {
            if (invoiceList.Count == 0)
            {
                MessageBox.Show("未输入任何内容！");
                return;
            }

            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.RestoreDirectory = true;
            fileDialog.Filter = "Excel(97-2003)|*.xls|Excel(2007-2013)|*.xlsx";
            if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
            {
                return;
            }


            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("sheet1");
            IRow rowHead = sheet.CreateRow(0);

            rowHead.CreateCell(0, CellType.String).SetCellValue("NO");
            rowHead.CreateCell(1, CellType.String).SetCellValue("发票代码");
            rowHead.CreateCell(2, CellType.String).SetCellValue("发票号码");
            rowHead.CreateCell(3, CellType.String).SetCellValue("合计金额");
            rowHead.CreateCell(4, CellType.String).SetCellValue("开票日期");


            //填写内容
            for (int i = 0; i < invoiceList.Count; i++)
            {
                int columnIndex = 0;
                IRow row = sheet.CreateRow(i + 1);

                row.CreateCell(columnIndex++, CellType.String).SetCellValue(invoiceList[i].RowNumber);
                row.CreateCell(columnIndex++, CellType.String).SetCellValue(invoiceList[i].Code);
                row.CreateCell(columnIndex++, CellType.String).SetCellValue(invoiceList[i].Number);
                row.CreateCell(columnIndex++, CellType.String).SetCellValue(invoiceList[i].Amount.ToString());
                row.CreateCell(columnIndex++, CellType.String).SetCellValue(invoiceList[i].MakeDate);
            }

            for (int i = 0; i < invoiceList.Count; i++)
            {
                sheet.AutoSizeColumn(i);
            }

            using (FileStream stream = File.OpenWrite(fileDialog.FileName))
            {
                workbook.Write(stream);
                stream.Close();
            }
            MessageBox.Show("导出数据成功!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            GC.Collect();
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > 0)
            {
                string strcolumn = dataGridView1.Columns[e.ColumnIndex].HeaderText;//获取列标题
                string strrow = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();//获取焦点触发行的第一个值
                string value = dataGridView1.CurrentCell.Value.ToString();//获取当前点击的活动单元格的值

                var cur = invoiceList.SingleOrDefault(f => f.RowNumber.ToString() == strrow);

            }
        }
    }
}
