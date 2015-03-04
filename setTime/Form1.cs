using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace setTime
{
    public partial class Form1 : Form
    {
        string[] args = null;
        public Form1(string[] args)
        {
            InitializeComponent();
            this.args = args;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            UAC.ByPass();

            Object val;
            val = Application.UserAppDataRegistry.GetValue("time");
            if (val != null)
            {
                try
                {
                    dateTimePicker1.Value = DateTime.ParseExact(val.ToString(), dateTimePicker1.Format.ToString(), null);
                }
                catch(Exception)
                {
                    dateTimePicker1.Value = System.DateTime.Now;
                }
            }
            else
            {
                dateTimePicker1.Value = System.DateTime.Now;
            }
            
            val = null;
            val = Application.UserAppDataRegistry.GetValue("path");
            if (val != null)
            {
                path.Text = val.ToString();
            }

            val = null;
            val = Application.UserAppDataRegistry.GetValue("lag");
            if (val != null)
            {
                lag.Text = val.ToString();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();//新建打开文件对话框
            ofd.InitialDirectory = Environment.CurrentDirectory;//设置初始文件目录
            ofd.Filter = "试用程序(*.exe)|*.exe|所有文件(*.*)|*.*";//设置打开文件类型
            if (ofd.ShowDialog(this) == DialogResult.OK)
            {
                path.Text = ofd.FileName;//FileName就是要打开的文件路径  
            }
        }

        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public class SystemTime
        {
            public ushort wYear;
            public ushort wMonth;
            public ushort wDayOfWeek;
            public ushort wDay;
            public ushort Whour;
            public ushort wMinute;
            public ushort wSecond;
            public ushort wMilliseconds;

        }
        //调用Kernel32.DLL
        [System.Runtime.InteropServices.DllImport("Kernel32.dll")]
        public static extern void GetLocalTime(SystemTime st);
        [System.Runtime.InteropServices.DllImport("Kernel32.dll")]
        public static extern void SetLocalTime(SystemTime st);

        DateTime last_time;
        private void button2_Click(object sender, EventArgs e)
        {
            Application.UserAppDataRegistry.SetValue("time", dateTimePicker1.Format.ToString());
            Application.UserAppDataRegistry.SetValue("path",path.Text);
            Application.UserAppDataRegistry.SetValue("lag", lag.Text);

            SystemTime last = new SystemTime();
            GetLocalTime(last);
            last_time = new DateTime(last.wYear, last.wMonth, last.wDay, last.Whour, last.wMinute, last.wSecond);
            
            SystemTime set_time = new SystemTime();
            set_time.wYear = (ushort)this.dateTimePicker1.Value.Year;
            set_time.wMonth = (ushort)this.dateTimePicker1.Value.Month;
            set_time.wDay = (ushort)this.dateTimePicker1.Value.Day;
            set_time.Whour = (ushort)this.dateTimePicker1.Value.Hour;
            set_time.wMinute = (ushort)this.dateTimePicker1.Value.Minute;
            set_time.wSecond = (ushort)this.dateTimePicker1.Value.Second;
            SetLocalTime(set_time);

            System.Diagnostics.Process.Start(path.Text, "");
            new System.Threading.Thread(lag_exit).Start(); 
        }

        void lag_exit()
        {
            System.Threading.Thread.Sleep(Convert.ToInt32(lag.Text)*1000);

            DateTime nowtime = last_time.AddMilliseconds(Convert.ToInt32(lag.Text) * 1000);
            SystemTime set_time = new SystemTime();
            set_time.wYear = (ushort)nowtime.Year;
            set_time.wMonth = (ushort)nowtime.Month;
            set_time.wDay = (ushort)nowtime.Day;
            set_time.Whour = (ushort)nowtime.Hour;
            set_time.wMinute = (ushort)nowtime.Minute;
            set_time.wSecond = (ushort)nowtime.Second;
            SetLocalTime(set_time);

            Application.Exit();
        }

    }
}
