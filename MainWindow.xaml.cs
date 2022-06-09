using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using HslCommunication;
using HslCommunication.ModBus;
using HslCommunication.LogNet;
using System.Threading;
using System.Collections.ObjectModel;

namespace Hsl
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        public ModbusTcpServer busTcpServer; //HslCommunication.ModBus 當Master 
        public ModbusTcpNet busTcpClient = null;
        public System.Windows.Forms.Timer timer;
        public string Message,Status,ip; 
        public int a = 0; //關閉的Flag 預設為0  0：開啟 1：關閉
        public DateTime Old_Time, Now_Time, dt1, dt2;

        //定義Slaver的IP
        class Slaver_IP
        {
            public string IP { get; set; }
        }
        ObservableCollection<Slaver_IP> IP_List;

        public MainWindow()
        {
            InitializeComponent();
            dt1 = DateTime.Now;
            timer = new System.Windows.Forms.Timer
            {
                Interval = 1000
            };
            timer.Tick += new EventHandler(Timer_Tick);
            IP_List = new ObservableCollection<Slaver_IP>();
        }

        //每秒更新
        void Timer_Tick(object sender, EventArgs e)
        {
            try
            {
                timer.Stop();
                Now_Time = DateTime.Now;
                double s = new TimeSpan(Now_Time.Ticks - Old_Time.Ticks).TotalSeconds;
                //大於2秒未回應則斷線
                if (s >= 2)
                {
                    TB_Message.Background = Brushes.Red;
                }
                else
                {
                    TB_Message.Background = Brushes.Green;
                }

                //讀取
                busTcpClient?.ConnectClose();
                busTcpClient = new ModbusTcpNet(ip, 502);
                busTcpClient.IsStringReverse = true;
                busTcpClient.DataFormat = HslCommunication.Core.DataFormat.BADC;
                OperateResult<string> read = busTcpClient.ReadString("0", 30, Encoding.UTF8); //讀輸入
                if (read.IsSuccess)
                {
                    string ClientID = read.Content;
                    ClientID = ClientID.Replace("\0", "");
                    ClientID = ClientID.Trim();
                    TB_Input.Text = ClientID;
                }
                busTcpClient?.ConnectClose();

                //不同天
                dt2 = DateTime.Now;
                bool oneDay = DateTime.Equals(dt1.Date, dt2.Date);
                if (oneDay != true)
                {
                    dt1 = DateTime.Now;
                    busTcpServer.LogNet = new LogNetSingle(AppDomain.CurrentDomain.BaseDirectory + @"\Logs\" + dt1.ToString("yyyyMMdd") + @"\log.txt");
                }
            }
            catch
            {
            }
            finally 
            {
                timer.Start();
            }
        }

        //開始
        private void BT_S_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                a = 0;
                busTcpServer = new ModbusTcpServer();
                busTcpServer.LogNet = new LogNetSingle(AppDomain.CurrentDomain.BaseDirectory + @"\Logs\" + dt1.ToString("yyyyMMdd") + @"\log.txt"); 
                busTcpServer.OnDataReceived += TcpServer_OnDataReceived;
                busTcpServer.LogNet.BeforeSaveToFile += TcpServer_BeforeSaveToFile;
                busTcpServer.ServerStart(502);
                timer?.Start();
            }
            catch
            {

            }

        }

        //結束
        private void BT_E_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                a = 1;
            }
            catch
            {

            }

        }

        //接收訊息
        public void TcpServer_OnDataReceived(object sender, byte[] data)
        {
            if (a == 1)
            {
                busTcpServer.ServerClose();
                return;
            }
            Old_Time = DateTime.Now;
        }

        //接收日誌
        public void TcpServer_BeforeSaveToFile(object sender, HslEventArgs e)
        {
            try
            {
                //讀取上線的IP
                if (e.HslMessage.ToString().Contains("上线") == true)
                {
                    string a = e.HslMessage.Text.Split(':')[0];
                    string b = a.Split('[')[1];
                    string c = b.Trim();
                    Message = c;
                    ip = Message;
                }

                //多執行緒處理接收日誌值
                this.Dispatcher.Invoke((Action)(() =>
                {
                    TB_Message.Text = Message;
                    int count = 0;
                    foreach (var List in IP_List)
                    {
                        if(List.IP == Message)
                        {
                            count += 1;
                        }
                    }
                    if (count == 0)
                    {
                        Slaver_IP item = new Slaver_IP
                        {
                            IP = Message
                        };
                        if (Message != "" && Message != null)
                        {
                            IP_List.Add(item);
                        }
                    }
                    Dag_IP.ItemsSource = IP_List;
                }));
            }
            catch
            {

            }
            finally
            {
            }
        }

        //傳送訊息到Slaver
        private void BT_Send_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (TB_Message.Background == Brushes.Green)
                {
                    if (TB_Retun.Text != "" && ip != null)
                    {
                        //寫入
                        busTcpClient?.ConnectClose();
                        busTcpClient = new ModbusTcpNet(ip, 502);
                        busTcpClient.IsStringReverse = true;
                        busTcpClient.DataFormat = HslCommunication.Core.DataFormat.BADC;
                        busTcpClient.Write("30", TB_Retun.Text, Encoding.UTF8);
                        busTcpClient?.ConnectClose();
                    }
                }
            }
            catch
            {

            }

        }

        //關閉
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            a = 1;
            timer.Stop();
            base.OnClosed(e);
            Application.Current.Shutdown();
        }
    }
}
