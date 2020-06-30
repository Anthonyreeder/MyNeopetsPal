using Microsoft.Data.Sqlite;

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyNeopetPal
{
    class SqliteData
    {

        public static void start(Form1 form)
        {
            SQLiteConnection sqlite_conn;
            sqlite_conn = CreateConnection();
        }

        public static SQLiteConnection CreateConnection()
        {

            SQLiteConnection sqlite_conn;
            // Create a new database connection:
            sqlite_conn = new SQLiteConnection("Data Source=database.db; Version = 3; New = True; Compress = True; ");
                // Open the connection:
         try
            {
                sqlite_conn.Open();
            }
            catch (Exception ex)
            {

            }
            return sqlite_conn;
        }

        static void CreateTable(SQLiteConnection conn)
        {

            SQLiteCommand sqlite_cmd;
            string Createsql = "CREATE TABLE SampleTable(Col1 VARCHAR(20), Col2 INT)";
           string Createsql1 = "CREATE TABLE SampleTable1(Col1 VARCHAR(20), Col2 INT)";
           sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = Createsql;
            sqlite_cmd.ExecuteNonQuery();
            sqlite_cmd.CommandText = Createsql1;
            sqlite_cmd.ExecuteNonQuery();

        }

        public static bool UpdateSnowball(SQLiteConnection conn, Users user, Form1 form)
        {
            using (SQLiteCommand cmd = new SQLiteCommand(conn))
            {
                SQLiteDataReader sqlite_datareader;
                cmd.CommandText = "select Snowball from Daily where Id = @id";
                cmd.Parameters.AddWithValue("@Id", user.id);
                cmd.Prepare();
                sqlite_datareader = cmd.ExecuteReader();
                while (sqlite_datareader.Read())
                { 
                    DateTime time = sqlite_datareader.GetDateTime(0);
                    DateTime MinsLater = time.AddMinutes(30);
                    if (DateTime.Now > MinsLater)
                    {
                        using (SQLiteCommand newcmd = new SQLiteCommand(conn))
                        {
                            //Time now is later than that time +30mins so first update sql snowball time to new one
                            newcmd.CommandText = "UPDATE Daily SET Snowball = datetime('now', 'localtime') where Id = @id";
                            newcmd.Parameters.AddWithValue("@Id", user.id);
                            newcmd.Prepare();
                            newcmd.ExecuteNonQuery();
                            form.AppendText("Snowball time updated", user.username, user.txtbox);
                        }

                        //now return true to say we can collect
                        return true;
                    }
                    else
                        return false;
                }
                return false;
            }
        }
        public static bool UpdateTrudy(SQLiteConnection conn, Users user, Form1 form)
        {
            using (SQLiteCommand cmd = new SQLiteCommand(conn))
            {
                SQLiteDataReader sqlite_datareader;
                cmd.CommandText = "select Trudy from Daily where Id = @id";
                cmd.Parameters.AddWithValue("@Id", user.id);
                cmd.Prepare();
                sqlite_datareader = cmd.ExecuteReader();
                while (sqlite_datareader.Read())
                {
                    DateTime time = sqlite_datareader.GetDateTime(0);
                    DateTime MinsLater = time.AddMinutes(30);
                    if (DateTime.Now.Date > time.Date && DateTime.Now.TimeOfDay > new TimeSpan(10, 0, 0))
                    {
                        using (SQLiteCommand newcmd = new SQLiteCommand(conn))
                        {
                            //Date now is greater than recorded time so we will update and grab
                            newcmd.CommandText = "UPDATE Daily SET Trudy = datetime('now', 'localtime') where Id = @id";
                            newcmd.Parameters.AddWithValue("@Id", user.id);
                            newcmd.Prepare();
                            newcmd.ExecuteNonQuery();
                            form.AppendText("Trudy time updated", user.username, user.txtbox);
                        }
                        return true;
                    }
                    else
                        //Date is not greater so its probably the same day so return false
                        return false;
                }
                //ASomething happened, i dunno what but just do false cus it aint done shit.
                return false;
            }
        }


        public static List<Users> ReadData(SQLiteConnection conn, Form1 form, List<RichTextBox> txtbox)
        {
            SQLiteDataReader sqlite_datareader;
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = "SELECT * FROM Users where Active = 1";
            sqlite_datareader = sqlite_cmd.ExecuteReader();
            List<Users> users = new List<Users>();
            while (sqlite_datareader.Read())
            {
                ModManager modManager = new ModManager(form);

                Users newUser = new Users(sqlite_datareader.GetInt32(0), modManager, sqlite_datareader.GetString(1), sqlite_datareader.GetString(2), sqlite_datareader.GetString(3), conn, txtbox[0]);
                users.Add(newUser);
                form.AppendText("Username: " + newUser.username, "DATA", txtbox[0]);
                txtbox.RemoveAt(0);
            }
            return users;
        }
    } 
}
