using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Threading;

namespace MyNeopetPal
{
    class Users
    {
        ModManager modManager;
        public int id;
        public string username;
        public string password;
        public string proxy;
        public bool actionReady = false;
        public int action = 0;
        SQLiteConnection connect;

        public Users(int id, ModManager modManager, string username, string password, string proxy, SQLiteConnection connect)
        {
            this.id = id;
            this.modManager = modManager;
            this.username = username;
            this.password = password;
            this.proxy = proxy;
            this.connect = connect;
        }

        public List<int> actionQueue = new List<int>();

        public ModManager getModManager()
        {
            return modManager;
        }
        public void startThread()
        {
            new Thread(() =>
            {
                getModManager().LoginToNeopets(username, password, proxy); //this should be a bool to indicate if its logged in or not, if it fails it could then retry.
                //i need to also add a bool to say if currently logged in
                Thread.CurrentThread.IsBackground = true;
                System.Threading.TimerCallback cb = new System.Threading.TimerCallback(OnTimedEvent);
                timer = new System.Threading.Timer(cb, null, 1000 * 60 * 1, 0); //tme will be slightly randomised so all users are split up a bit
            }).Start();
        }
        System.Threading.Timer timer;
        void OnTimedEvent(object obj)
        {
            if (SqliteData.UpdateSnowball(connect, this, modManager.form))
            {
                modManager.form.AppendText("Buying stick snowball", username);
                getModManager().buyStickySnowball(this);
                timer.Change(1000 * 60 * 1, 0);  //reset timer
                return;
            }
            else
                modManager.form.AppendText("Snowball purchased in last 30minutes", username);

            if (SqliteData.UpdateTrudy(connect, this, modManager.form))
            {
                modManager.form.AppendText("Starting trudy", username);
                getModManager().startTrudy(this);
                timer.Change(1000 * 60 * 1, 0);  //reset timer
                return;
            }
            else
                modManager.form.AppendText("Trudy completed today", username);
            //check using sql all the events and see what/if anything needs doing for this user.//
            //complete the action and then reset timer to another slightly random (1-2minutes)   


            //Nothing ready to do so just wait a bit longer and go again.
            timer.Change(1000 * 60 * 1, 0);  //reset timer
        }
    }
}

