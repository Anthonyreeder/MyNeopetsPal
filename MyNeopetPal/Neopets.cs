using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace MyNeopetPal
{
    class Neopets
    {
        Form1 form;
        CookieContainer cookies = new CookieContainer();

        public Neopets(Form1 form)
        {
            this.form = form;
          //  form.AppendText("test");
        }
        private HttpWebRequest FormatRequest(string Type, string Uri)
        {   
           
            HttpWebRequest request = null;

            request = (HttpWebRequest)WebRequest.Create(new Uri(Uri));
            request.Host = "www.neopets.com";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Method = Type;
            request.CookieContainer = cookies;
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.116 Safari/537.36";
            request.Referer = "http://www.neopets.com/login/";
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9";
            request.AllowAutoRedirect = true;
            request.KeepAlive = true;
            
            return request;
        }
        private HttpWebResponse GetCookies(HttpWebRequest request)
        {
            HttpWebResponse response = null;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.Timeout || e.Status == WebExceptionStatus.ConnectFailure)
                {
                  //  form.AppendText("Failed to connect");
                  //  Logbox.AppendText("");
                    //connection failed? WTF?! Logi t.
                    // NeopetsLogin(username, password);
                }else
                {
                   // form.AppendText("Unknown error");
                }
            }
            return response;
        }
        private HttpWebResponse PostAndWrite(string username, string password, HttpWebRequest request)
        {
            HttpWebResponse response = null;
            StringBuilder postData = new StringBuilder();
            postData.Append("pageResponse=");
            postData.Append("destination=");
            postData.Append("&username=" + username);
            postData.Append("&password=" + password);
           
            using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
                writer.Write(postData.ToString());
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.Timeout || e.Status == WebExceptionStatus.ConnectFailure)
                {
                 //   form.AppendText("Failed to post and write data due to timeout or connection failed");
                }
            }
            return response;
        }
        private int ParseResponse(HttpWebResponse response)
        {
            string returnData = string.Empty;
            try
            {
                if (response != null)
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                        returnData = reader.ReadToEnd();
                else
                {
                  //  form.AppendText("Is null");
                }

            }
            catch (Exception)
            {
            //    form.AppendText("Failed");
                throw;
            }


            if (returnData.Contains("Simply login or register"))
            {
                return 1;
            }

            if (returnData.Contains("Sorry, we did not find an account with that username"))
            {
                return 2;
            }

            if (returnData.Contains("Invalid Password. Please enter the correct password to continue."))
            {
                return 3;
            }

            if (returnData.Contains("Welcome,"))
            {
                form.setPage(returnData);
                return 0;
            }
            
            System.IO.File.WriteAllText("C:\\Users\\Ant\\Desktop\\Neo\\here.txt", returnData);
            return 999;        
        }
        public void LoginToNeopets(string username, string password)
        {
            //create request using wrapper
            HttpWebRequest LoginRequest = FormatRequest("GET", "http://www.neopets.com/");
            //Get the cookies from neopets using wrapper
            HttpWebResponse response = GetCookies(LoginRequest); //if we want to check the response to this.
            //Setup post request for login using wrapper
            LoginRequest = FormatRequest("POST", "http://www.neopets.com/login.phtml");
            //Format the POST data and write to the stream
            response = PostAndWrite(username, password, LoginRequest);
            //Read response and see if we logged in!!
            switch (ParseResponse(response))
            {
                case 0:
                    form.AppendText("Success loggin as "+username, username);
                    break;
                case 1:
                    form.AppendText("Cookie malfunction - Incorrect username", username);
                    break;
                case 2:
                    form.AppendText("Username does not exist", username);
                    break;
                case 3:
                    form.AppendText("Password is invalid", username);
                    break;
                default:
                    form.AppendText("Unknown error", username);
                    break;
            }


        }
    
    
        public void checkMoney()
        {
            HttpWebRequest request = null;

            request = (HttpWebRequest)WebRequest.Create(new Uri("http://www.neopets.com/"));
            request.Host = "www.neopets.com";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Method = "GET";
            request.CookieContainer = cookies;
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.116 Safari/537.36";
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9";
            request.AllowAutoRedirect = true;
            request.KeepAlive = true;

                       
            HttpWebResponse response = GetCookies(request);

            string returnData = string.Empty;
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                returnData = reader.ReadToEnd();
            //href="/inventory.phtml">449,507</a>
            int Pos1 = returnData.IndexOf("inventory.phtml\">");
            int Pos2 = returnData.IndexOf("</a> <span style=");
            string FinalString = returnData.Substring(Pos1+17, (Pos1) - (Pos2+70));
            form.setPage(returnData);
            form.AppendText(FinalString, "System");
        }
    }
}
