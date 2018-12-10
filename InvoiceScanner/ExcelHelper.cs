
namespace InvoiceScanner
{
    using NPOI.HSSF.Util;
    using NPOI.SS.UserModel;
    using NPOI.SS.Util;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;


    public class ExcelHelp
    {
        public static NPOI.HSSF.UserModel.HSSFWorkbook obook;
        public static NPOI.SS.UserModel.ISheet osheet;

        /// <summary>
        /// 构造函数，初始化表单对象
        /// </summary>
        /// <param name="path"></param>
        /// <param name="sheetname"></param>
        public ExcelHelp()
        {
            obook = new NPOI.HSSF.UserModel.HSSFWorkbook();
            osheet = obook.CreateSheet("sheet1");
        }

        /// <summary>
        /// 合并单元格
        /// </summary>
        /// <param name="r1">左上角单元格行标(从0开始，下同)</param>
        /// <param name="c1">左上角单元格列标</param>
        /// <param name="r2">右下角单元格行标</param>
        /// <param name="c2">右下角单元格列标</param>
        public void Merge(int r1, int c1, int r2, int c2)
        {
            osheet.AddMergedRegion(new CellRangeAddress(r1, c1, r2, c2));
        }
        /// <summary>
        /// 设置单元格内容
        /// </summary>
        /// <param name="row">单元格行标(从0开始，下同)</param>
        /// <param name="col">单元格列标</param>
        /// <param name="o">写入内容</param>
        public void SetValue(int r, int c, object o)
        {
            if (o != null)
            {
                if (r <= osheet.LastRowNum)
                {
                    IRow row = osheet.GetRow(r);

                    if (row == null)
                    {
                        row = osheet.CreateRow(r);
                        row.HeightInPoints = 14;
                    }
                    if (c <= row.LastCellNum)
                    {
                        ICell cell = row.GetCell(c);
                        if (cell == null)
                        {
                            row.CreateCell(c, CellType.String).SetCellValue(o.ToString());
                        }
                        else
                        {
                            cell.SetCellValue(o.ToString());
                        }
                    }
                    else
                    {
                        for (int j = row.LastCellNum; j < c; j++)
                        {
                            row.CreateCell(j + 1);
                            ICell cell22 = row.GetCell(j + 1);
                            ICellStyle style = obook.CreateCellStyle();
                            cell22.CellStyle = style;
                        }
                        ICell cell = row.GetCell(c);
                        cell.SetCellValue(o.ToString());
                    }
                }
                else
                {
                    for (int i = osheet.LastRowNum; i < r; i++)
                    {
                        IRow row22 = osheet.CreateRow(i + 1);
                        row22.HeightInPoints = 14;
                    }
                    IRow row = osheet.GetRow(r);
                    for (int j = row.LastCellNum; j < c; j++)
                    {
                        row.CreateCell(j + 1); ;
                        ICell cell22 = row.GetCell(j + 1);
                        ICellStyle style = obook.CreateCellStyle();
                        cell22.CellStyle = style;
                    }
                    ICell cell = row.GetCell(c);
                    cell.SetCellValue(o.ToString());

                }

            }
        }
        /// <summary>
        /// 设置行高
        /// </summary>
        /// <param name="r">行数</param>
        /// <param name="height">高度</param>
        public void SetRowHeight(int r, int height)
        {
            if (r <= osheet.LastRowNum)
            {
                IRow row = osheet.GetRow(r);
                if (row != null)
                {
                    row.HeightInPoints = height;
                }
            }
        }
        /// <summary>
        /// 设置字体宽度
        /// </summary>
        /// <param name="c">列标</param>
        /// <param name="width">宽度值（例如设置为1，表示一个英文字符的宽度）</param>
        public void SetCollumWdith(int c, int width)
        {
            osheet.SetColumnWidth(c, 256 * width);
        }
        /// <summary>
        /// 设置单元格对齐方式
        /// </summary>
        /// <param name="r">行标</param>
        /// <param name="c">列标</param>
        /// <param name="align">对齐方式（'L',左对齐；'C'居中；'R'右对齐）</param>
        public void SetCellAlignment(int r, int c, char align)
        {
            if (r <= osheet.LastRowNum)
            {
                IRow row = osheet.GetRow(r);
                if (row != null)
                {
                    if (c <= row.LastCellNum)
                    {
                        ICell cell = row.GetCell(c);
                        ICellStyle style = cell.CellStyle;
                        //设置单元格的样式：水平对齐居中
                        if (align == 'C')
                            style.Alignment = HorizontalAlignment.Center;
                        else if (align == 'L')
                            style.Alignment = HorizontalAlignment.Left;
                        else if (align == 'R')
                            style.Alignment = HorizontalAlignment.Right;
                        cell.CellStyle = style;
                    }
                }
            }
        }

        /// <summary>
        /// 设置字体样式
        /// </summary>
        /// <param name="r">行标</param>
        /// <param name="c">列标</param>
        /// <param name="size">字体大小，0为默认</param>
        /// <param name="f">字体样式（‘B’加粗，‘I’斜体）</param>
        /// <param name="color">字体颜色('R'红,'B'蓝,'G'绿,'Y'黄,'P'粉,'O'橙,'W'白)</param>
        public void SetCellFont(int r, int c, int size, char f, char color)
        {
            if (r <= osheet.LastRowNum)
            {
                IRow row = osheet.GetRow(r);
                if (row != null)
                {
                    if (c <= row.LastCellNum)
                    {
                        ICell cell = row.GetCell(c);
                        ICellStyle style = cell.CellStyle;
                        //新建一个字体样式对象
                        IFont font = obook.CreateFont();
                        //设置字体大小
                        if (size > 0)
                            font.FontHeightInPoints = Convert.ToInt16(size);
                        switch (f)
                        {
                            case 'B':
                                {
                                    //设置字体加粗样式
                                    font.IsBold = true;
                                }
                                break;
                            case 'I':
                                {
                                    //设置字体加粗样式
                                    font.IsItalic = true;
                                }
                                break;
                        }
                        switch (color)
                        {
                            case 'R': { font.Color = HSSFColor.Red.Index; } break;
                            case 'B': { font.Color = HSSFColor.Blue.Index; } break;
                            case 'G': { font.Color = HSSFColor.Green.Index; } break;
                            case 'Y': { font.Color = HSSFColor.Yellow.Index; } break;
                            case 'P': { font.Color = HSSFColor.Pink.Index; } break;
                            case 'O': { font.Color = HSSFColor.Orange.Index; } break;
                            case 'W': { font.Color = HSSFColor.White.Index; } break;
                        }
                        //使用SetFont方法将字体样式添加到单元格样式中 
                        style.SetFont(font);
                        //将新的样式赋给单元格
                        cell.CellStyle = style;
                    }
                }
            }
        }

        /// <summary>
        /// 写入到Excel文档
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool ExportToExcel(string path)
        {
            try
            {
                // 写入到客户端  
                using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    obook.Write(fs);
                }
                return true;
            }
            catch { return false; }
        }

        public void Dispose()
        {
            obook.Close();
            GC.Collect();
        }
    }
}


