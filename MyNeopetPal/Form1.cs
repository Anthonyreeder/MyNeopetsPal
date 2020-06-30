using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Timers;
using Timer = System.Timers.Timer;
using System.Threading;

namespace MyNeopetPal
{
    public partial class Form1 : Form
    {
        List<Users> allUsers = new List<Users>();
        SQLiteConnection connect;

        public void AppendText(string what, string user)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(
                    new MethodInvoker(
                    delegate () { AppendText(what, user); }));
            }
            else
            {
                DateTime timestamp = DateTime.Now;
                logbox.AppendText(timestamp.ToShortTimeString() + " : " + user + " : " + what + Environment.NewLine);
                logbox.ScrollToCaret();
            }
        }
        public void setPage(string page)
        {
            //wbview.DocumentText = page;
            //wbview.AllowNavigation = false;
        }
        public Form1()
        {
            InitializeComponent();
            connect = SqliteData.CreateConnection();
            loadusers();
        }
        System.Threading.Timer timer;

        void OnTimedEvent(object obj)
        {
            foreach (var user in allUsers)
            {
                if (user.actionReady)
                {
                    //grab action from user
                    user.getModManager().buyStickySnowball(user);
                }  
            }
            timer.Change(1000 * 60 * 15, 0);
        }

        private void loadusers()
        {
            allUsers = SqliteData.ReadData(connect, this);
        }
        


        private void startBot()
        {
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                /* run your code here */
                foreach (var user in allUsers)
                {
                    user.startThread();
                   // LoginToNeopets(user.username, user.password, user.proxy);
                    System.Threading.Thread.Sleep(150);
                }
            }).Start();
        }


        private void btn_login_Click(object sender, EventArgs e)
        {

            startBot();
            //have a form control to select which user
            //using their ID/Name/Username whatever... find the correct one in list (for testing ill just use 0 as theres only 1 user)            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //have a form control to select which user
            //using their ID/Name/Username whatever... find the correct one in list (for testing ill just use 0 as theres only 1 user)
            foreach (var user in allUsers)
            {
                user.getModManager().buyStickySnowball(user);
            }

            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                /* run your code here */
                foreach (var user in allUsers)
                {
                    user.getModManager().buyStickySnowball(user);
                }
                System.Threading.TimerCallback cb = new System.Threading.TimerCallback(OnTimedEvent);
                timer = new System.Threading.Timer(cb, null, 1000 * 60 * 15, 0);
            }).Start();
        }

        private void btnLogin1_Click(object sender, EventArgs e)
        {
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                /* run your code here */
                allUsers[5].getModManager().LoginToNeopets(allUsers[5].username, allUsers[5].password, "");
            System.Threading.Thread.Sleep(1000);
            }).Start();
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                allUsers[5].getModManager().startTrudy(allUsers[5]);
            }).Start();
    }
    }
}
