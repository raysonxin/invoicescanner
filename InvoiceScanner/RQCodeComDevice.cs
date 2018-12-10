using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InvoiceScanner
{
    public class RQCodeComDevice
    {
        public delegate void EventHandleQRCode(string text);
        public event EventHandleQRCode DataReceivedQRCode;

        public SerialPort serialPort;
        Thread thread;
        volatile bool _keepReading;

        public RQCodeComDevice()
        {
            serialPort = new SerialPort();
            thread = null;
            _keepReading = false;
        }

        public bool IsOpen
        {
            get
            {
                return serialPort.IsOpen;
            }
        }

        private void StartReading()
        {
            if (!_keepReading)
            {
                _keepReading = true;
                thread = new Thread(new ThreadStart(ReadPort));
                thread.Start();
            }
        }

        private void StopReading()
        {
            if (_keepReading)
            {
                _keepReading = false;
                thread.Join();
                thread = null;
            }
        }

        private void ReadPort()
        {
            while (_keepReading)
            {
                try
                {
                    if (serialPort.IsOpen)
                    {
                        string gpsinfo = "";
                    READAGAIN:
                        int n = serialPort.BytesToRead;
                        byte[] buf = new byte[n];


                        while (serialPort.BytesToRead > 0)
                        {
                            if (serialPort.Read(buf, 0, n) != 0)
                            {
                                String readstr = Encoding.UTF8.GetString(buf);
                                //Encoding encoding = System.Text.Encoding.GetEncoding("GB2312");
                                //String readstr = encoding.GetString(buf);
                                gpsinfo += readstr;
                            }
                        }

                        Thread.Sleep(50);

                        if (serialPort.BytesToRead > 0) goto READAGAIN;//goto标记，数据未接收完继续等待接收完成

                        string strInfo = gpsinfo;

                        if (!string.IsNullOrEmpty(strInfo))
                        {
                            DataReceivedQRCode(strInfo);
                        }

                    }
                }
                catch (Exception)
                {
                    //throw new ArgumentException("【ComDevice.ReadPort】" + ex.Message);
                }
            }
        }

        /// <summary>
        /// 打开串口
        /// </summary>
        /// <returns></returns>
        public bool Open()
        {
            try
            {
                Close();
                serialPort.Open();
                if (serialPort.IsOpen)
                {
                    StartReading();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                //new AggregateException(ex);
            }

            return false;
        }

        /// <summary>
        /// 关闭串口
        /// </summary>
        public void Close()
        {
            StopReading();
            serialPort.Close();
        }

        /// <summary>
        /// 写数据到卡
        /// </summary>
        /// <param name="send"></param>
        /// <param name="offSet"></param>
        /// <param name="count"></param>
        public void WritePort(byte[] send, int offSet, int count)
        {
            if (IsOpen)
            {
                serialPort.Write(send, offSet, count);
            }
        }
    }

}
