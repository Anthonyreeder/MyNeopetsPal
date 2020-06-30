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
        public List<RichTextBox> logBoxes = new List<RichTextBox>();

        public void AppendText(string what, string user, RichTextBox txtbox = null)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(
                    new MethodInvoker(
                    delegate () { AppendText(what, user, txtbox); }));
            }
            else
            {
                DateTime timestamp = DateTime.Now;
                if (txtbox == null)
                {
                    
                    logboxSystem.AppendText(timestamp.ToShortTimeString() + " : " + user + " : " + what + Environment.NewLine);
                    logboxSystem.ScrollToCaret();
                }
                else
                {
                    txtbox.AppendText(timestamp.ToShortTimeString() + ":" + what + Environment.NewLine);
                    txtbox.ScrollToCaret();
                }
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
            logBoxes.Add(logbox);
            logBoxes.Add(logbox1);
            logBoxes.Add(logbox2);
            logBoxes.Add(logbox3);
            logBoxes.Add(logbox4);
            logBoxes.Add(logbox5);
            logBoxes.Add(logbox6);
            logBoxes.Add(logbox7);
            logBoxes.Add(logbox8);
            logBoxes.Add(logbox9);
            logBoxes.Add(logboxSystem);
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
            allUsers = SqliteData.ReadData(connect, this, logBoxes);
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
    }
}
