using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.IO;

namespace Watch
{
    public enum Status
    {
        CUR,//当前时间
        UP,//正计时
        DOWN,//倒计时
    }

    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        //private float m_smoothTime;
        //private float m_targetWidth;

        private DispatcherTimer m_ShowTimer;//显示时间
        private DispatcherTimer m_FlowTimer;//流动时间


        private Status status;//状态
        private float timeNow = 0;
        private float delta;//计时器变化
        //=======状态变量=======\\
        private bool m_record_up_open = false;//false 此页未打开
        private bool m_RecordUpLoaded = false;//记录是否被载入过 只载入一次
        //=======本地读取=======\\
        private float scoreSum;//累计分数
        private List<string> m_record_up;//正计时记录

        //=======跳板变量=======\\

        
        private string t_record_up;//跳板变量 记录时间

        //=======设置界面=======\\
        //从这里获取
        private float timeForDown;//倒计时开始时间


        public MainWindow()
        {
            InitializeComponent();

            this.WindowState = System.Windows.WindowState.Maximized;
            this.WindowStyle = System.Windows.WindowStyle.None;


            m_ShowTimer = new DispatcherTimer();
            m_ShowTimer.Tick += new EventHandler(ShowCurTimer);//开个Timer一直获取当前时间
            m_ShowTimer.Interval = new TimeSpan(0, 0, 0, 1, 0);
            m_ShowTimer.Start();

            m_FlowTimer = new DispatcherTimer();
            m_FlowTimer.Tick += new EventHandler(Flow_Timer);
            m_FlowTimer.Interval = new TimeSpan(0, 0, 0, 0, 1000);

            timeForDown = float.Parse(this.textBox.Text);

            string[] record_up =
            {
                "2017/7/4 3.5",
            };
            m_record_up = new List<string>();
            foreach (string s in record_up)
            {
                m_record_up.Add(s);
            }
        }

        public void Flow_Timer(object sender, EventArgs e)//计时
        {
            timeNow += delta;

            this.textBlock.Text = string.Format("{0:0.0}", timeNow);//TODO ？？？ 这里的1000毫秒不是1秒
        }

        public void ShowCurTimer(object sender, EventArgs e)//显示时间
        {
            #region

            //"星期"+DateTime.Now.DayOfWeek.ToString(("d"))

            //获得星期几
            //this.Tt.Text = DateTime.Now.ToString("dddd", new System.Globalization.CultureInfo("zh-cn"));
            //this.Tt.Text += " ";
            //获得年月日
            //this.Tt.Text += DateTime.Now.ToString("yyyy年MM月dd日");   //yyyy年MM月dd日
            //this.Tt.Text += " ";
            //获得时分秒
            //System.Diagnostics.Debug.Print("this.ShowCurrentTime {0}", this.ShowCurrentTime);
            //System.Diagnostics.Debug.Print("OK");
            #endregion
            this.Tt.Text = DateTime.Now.ToString("HH:mm:ss");
        }

        private void textBlock2_MouseDown(object sender, MouseButtonEventArgs e)//todo 在down中记录按键 在up中执行 否则在upd中无法检测到按键
        {

            int state0 = e.LeftButton == MouseButtonState.Pressed ? 100 : 0;//左键
            int state1 = e.MiddleButton == MouseButtonState.Pressed ? 10 : 0;//中键
            int state2 = e.RightButton == MouseButtonState.Pressed ? 1 : 0;//右键
            int state = state0 + state1 + state2;
            switch (state)
            {
                case 100: App.Current.Shutdown(); break;//按下左键停止
                case 010: this.WindowState = (this.WindowState == WindowState.Maximized ? WindowState.Minimized : WindowState.Maximized); break; //按下右键切换大小
                case 001: showSettingUI(); break;
            }

        }

        private void showSettingUI()//设置界面
        {
            if (Panel1.IsVisible == false)
            {
                Panel1.Visibility = Visibility.Visible;
            }
            else
            {
                Panel1.Visibility = Visibility.Collapsed;
            }

        }

        private string readRecord(string path)
        {
            return File.ReadAllText(path);
        }
        private void writeRecord(string path, string str)
        {
            File.AppendAllText(path, str);
        }

        private void loadRecord()//只进行一次
        {
            P2_record_StackPanel.Children.Clear();
            TextBlock tb = new TextBlock();
            tb = new TextBlock();
            tb.Text = readRecord("record_up.txt");
            tb.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0x57, 0xF0, 0xFF));
            tb.Height = 30;
            tb.VerticalAlignment = VerticalAlignment.Center;
            tb.FontSize = 15;
            tb.HorizontalAlignment = HorizontalAlignment.Center;
            tb.Height = double.NaN;//自动调整高度
            P2_record_StackPanel.Children.Add(tb);
        }

        //========================事件==========================\\
        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (status == Status.CUR)
            {
                if (e.Key == Key.Add)//按下+号
                {
                    t_record_up = DateTime.Now.ToShortDateString();//设置初始时间
                    timeNow = 0;//重设时间
                    delta = 0.1f;//时间步进 正计时为正 倒计时为负
                    status = Status.UP;//设置状态
                    this.textBlock.Text = string.Format("{0:0.0}", timeNow);//设置初始内容
                    m_FlowTimer.Start();//开始
                }
                else if (e.Key == Key.Subtract)//按下-号
                {
                    timeNow = timeForDown;//设置时间
                    delta = -1f;//设置步进 正计时为正 倒计时为负
                    status = Status.DOWN;//设置状态
                    this.textBlock.Text = string.Format("{0:0.0}", timeNow);//设置初始内容
                    m_FlowTimer.Start();//开始
                }

            }
            else
            {
                if (e.Key == Key.Escape)//Esc结束
                {
                    t_record_up = "";//跳板置空
                    m_FlowTimer.Stop();
                    this.textBlock.Text = "";
                    status = Status.CUR;
                }
                else if (e.Key == Key.Return)//回车结束
                {
                    if (status == Status.UP)
                    {
                        float score = float.Parse(this.textBlock.Text);
                        scoreSum += score;//累加总分
                        t_record_up += " " + score;//按下-号时的时间 通过这个变量跳到了按下回车时
                        m_record_up.Add(t_record_up);
                        writeRecord("record_up.txt", "\r\n"+t_record_up);
                        m_RecordUpLoaded = false;//重置载入
                    }

                    m_FlowTimer.Stop();
                    this.textBlock.Text = "";
                    status = Status.CUR;
                }
            }

        }

        private void label_MouseDown(object sender, MouseButtonEventArgs e)//设置  转换
        {
            Panel1.Visibility = Visibility.Hidden;
            Panel2.Visibility = Visibility.Visible;
        }

        private void Panel2_MouseDown(object sender, MouseButtonEventArgs e)//统计 转换
        {
            Panel1.Visibility = Visibility.Visible;
            Panel2.Visibility = Visibility.Hidden;
        }
        /// <summary>
        /// 打开倒计时记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Label_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            if (m_record_up_open == true)//如果已经打开就合上
            {
                P2_record_StackPanel.Visibility = Visibility.Collapsed;
                m_record_up_open = false;
            }
            else//如果未打开就打开
            {
                P2_record_StackPanel.Visibility = Visibility.Visible;
                if (!m_RecordUpLoaded)
                {
                    loadRecord();
                    m_RecordUpLoaded = true;
                }
                m_record_up_open = true;
            }

        }

        /// <summary>
        /// 接收倒计时数字
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Return)
            {
                float result;
                if (float.TryParse(this.textBox.Text, out result))
                {
                    timeForDown = result;
                }
                else
                {
                    this.textBox.Text = "格式错误";
                }
            }
        }
    }
    
}




                 
                 
                 