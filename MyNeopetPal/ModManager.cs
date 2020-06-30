using HtmlAgilityPack;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyNeopetPal
{
    class ModManager
    {
        public Form1 form;
        CookieContainer cookies = new CookieContainer();

        public ModManager(Form1 form)
        {
            this.form = form;
        }

        #region-Tools
        private HttpWebRequest generateRequest(string Method, string Uri, string proxy, string Referer = "empty")
        {
            HttpWebRequest request = null;
            if (Referer == "empty")
                Referer = Uri;

            request = (HttpWebRequest)WebRequest.Create(new Uri(Uri));
            if (proxy != "")
            {
                request.Proxy = new WebProxy(proxy);
            }

            request.Host = "www.neopets.com";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Method = Method;
            request.CookieContainer = cookies;
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.116 Safari/537.36";
            request.Referer = Referer;
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9";
            request.AllowAutoRedirect = true;
            request.KeepAlive = true;

            return request;
        }
        private HttpWebResponse writeData(HttpWebRequest request, string[] keys, string[] values)
        {
            HttpWebResponse response = null;
            StringBuilder postData = new StringBuilder();
            for (int i = 0; i < keys.Length; i++)
            {
                postData.Append(keys[i] + "=" + values[i]);
            }

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
                    form.AppendText("Failed to post and write data due to timeout or connection failed", "system");
                }
            }
            return response;
        }

        #endregion


        public void LoginToNeopets(string username, string password, string proxy)
        {

            //Do a GET request, this also sets the Cookies in our cookiecontainer.
            HttpWebRequest loginRequest = generateRequest("GET", "http://www.neopets.com/", proxy);
            using (HttpWebResponse responsecookie = (HttpWebResponse)loginRequest.GetResponse()) ;
            //Post the login details, also uses the Cookies in our cookiecontainer.
            loginRequest = generateRequest("POST", "http://www.neopets.com/login.phtml", proxy);
            String[] keys = { "pageResponse", "&destination", "&username", "&password" };
            String[] values = { "", "", username, password };
            string returnData = string.Empty;
            using (HttpWebResponse response = writeData(loginRequest, keys, values))
                try
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                        returnData = reader.ReadToEnd();
                }
                catch (Exception)
                {
                    form.AppendText("Failed", username);
                    throw;
                }

            if (returnData.Contains("Welcome,"))
            {
                form.AppendText("Logged in successful as " + username, username);
            }
            else
                form.AppendText("Not logged in", username);
            //Read the response and confirm you are logged in
            //Log it
        }
        public void buyStickySnowball(Users user)
        {
                string returnData = string.Empty;
                HttpWebRequest request = generateRequest("POST", "http://www.neopets.com/faerieland/springs.phtml", user.proxy);

                String[] keys = { "type" };
                String[] values = { "purchase" };
                using (HttpWebResponse responsecookie = writeData(request, keys, values))
                    request = generateRequest("GET", "http://www.neopets.com/faerieland/process_springs.phtml?obj_info_id=8429", user.proxy);
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    returnData = reader.ReadToEnd();

                if (returnData.Contains("you are only allowed to purchase one item every 30 minutes"))
                {
                    form.AppendText("Sql wasnt up to date and already collected", user.username);
                }
                else
                {
                    form.AppendText("Purchased snowball", user.username);
                }
        }
        public void startKitchenQuest(Users user, SQLiteConnection connect)
        {
            string returnData = string.Empty;
            HttpWebRequest request = generateRequest("GET", "http://www.neopets.com/island/kitchen.phtml", user.proxy, "http://www.neopets.com/island/index.phtml");
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                try
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                        returnData = reader.ReadToEnd();
                }
                catch (Exception)
                {
                    form.AppendText("Failed", user.username);
                    throw;
                }
                string searchFor = "Do you want to help me or not?";
                int Pos1 = returnData.IndexOf(searchFor);   //This gets the index of the start of the search
                if (Pos1 < 0)
                {
                    string itemOne = "";
                    string itemTwo = "";
                    string itemThree = "";
                    string itemFour = "";

                    searchFor = "1><BR>";
                    Pos1 = returnData.IndexOf(searchFor) + 9;
                    string adjust = returnData.Substring(Pos1);
                    int Pos1end = adjust.IndexOf("</B>");
                    if (Pos1end < 0)
                    {
                        form.AppendText("10 quest limit reached", user.username);
                        return;
                    }
                    itemOne = adjust.Substring(0, Pos1end);
                    adjust = adjust.Substring(Pos1end);
                    int itemTwoPos = adjust.IndexOf("<BR>") + 7;
                    if (itemTwoPos > 0)
                    {

                        adjust = adjust.Substring(itemTwoPos);
                        int itemTwoPosEnd = adjust.IndexOf("</B>");
                        if (itemTwoPosEnd > 0)
                        {
                            itemTwo = adjust.Substring(0, itemTwoPosEnd);
                            adjust = adjust.Substring(itemTwoPosEnd);
                        }
                    }
                    int itemThreeoPos = adjust.IndexOf("1><BR>");
                    if (itemThreeoPos > 0)
                    {

                        adjust = adjust.Substring(itemThreeoPos);
                        int itemThreeoPosEnd = adjust.IndexOf("</B>");
                        if (itemThreeoPosEnd > 0)
                        {
                            itemThree = adjust.Substring(0, itemThreeoPosEnd);
                            adjust = adjust.Substring(itemThreeoPosEnd);
                        }

                    }
                    int itemFourPos = adjust.IndexOf("1><BR>");
                    if (itemFourPos > 0)
                    {

                        adjust = adjust.Substring(itemFourPos);
                        int itemFourPosEnd = adjust.IndexOf("</B>");
                        if (itemFourPosEnd > 0)
                        {
                            itemFour = adjust.Substring(0, itemFourPosEnd);
                            adjust = adjust.Substring(itemFourPosEnd);
                        }

                        form.AppendText("can't start kitchen quest, Is it already started?", user.username);
                    }

                    if (itemOne != "")
                    {
                        if (buyFromShopWizard(user, itemOne) == 1)
                        {
                            System.Threading.Thread.Sleep(250);
                            if (buyFromShopWizard(user, itemOne) == 1)
                            {
                                System.Threading.Thread.Sleep(250);
                                if (buyFromShopWizard(user, itemOne) == 1)
                                {
                                    form.AppendText("Can;t find " + itemOne + " in the shop wizzard", user.username);
                                    System.Threading.Thread.Sleep(250);
                                }
                            }
                        }
                        System.Threading.Thread.Sleep(250);
                    }

                    if (itemTwo != "")
                    {
                        if (buyFromShopWizard(user, itemTwo) == 1)
                        {
                            System.Threading.Thread.Sleep(250);
                            if (buyFromShopWizard(user, itemTwo) == 1)
                            {
                                System.Threading.Thread.Sleep(250);
                                if (buyFromShopWizard(user, itemTwo) == 1)
                                {
                                    System.Threading.Thread.Sleep(250);
                                    form.AppendText("Can;t find " + itemTwo + " in the shop wizzard", user.username);
                                }
                            }
                        }
                        System.Threading.Thread.Sleep(250);

                    }
                    if (itemThree != "")
                    {
                        if (buyFromShopWizard(user, itemThree) == 1)
                        {
                            System.Threading.Thread.Sleep(250);
                            if (buyFromShopWizard(user, itemThree) == 1)
                            {
                                System.Threading.Thread.Sleep(250);
                                if (buyFromShopWizard(user, itemThree) == 1)
                                {
                                    System.Threading.Thread.Sleep(250);
                                    form.AppendText("Can;t find " + itemThree + " in the shop wizzard", user.username);
                                }
                            }
                        }
                        System.Threading.Thread.Sleep(250);



                    }
                    if (itemFour != "")
                    {
                        if (buyFromShopWizard(user, itemFour) == 1)
                        {
                            System.Threading.Thread.Sleep(250);
                            if (buyFromShopWizard(user, itemFour) == 1)
                            {
                                System.Threading.Thread.Sleep(250);
                                if (buyFromShopWizard(user, itemFour) == 1)
                                {
                                    System.Threading.Thread.Sleep(250);
                                    form.AppendText("Can;t find " + itemFour + " in the shop wizzard", user.username);
                                }
                            }
                        }
                    }
                }
                else
                {
                    string returnstring = returnData.Substring(Pos1);    //This removes all the stuff before the start search
                    int Pos2 = returnstring.IndexOf("b>") + 2; //This uses the modified string so we get the 'Next' bit of syntax.
                    int Pos3 = returnstring.IndexOf("</b");
                    string FinalString = returnstring.Substring(Pos2, Pos3 - Pos2); //This uses an offset to get past the syntax. Maybe i can implement something to filter it a better way such as a regex match.
                    string finalStringPlusified = FinalString.Replace(" ", "+");
                    //now that i have the name of the dish, we can do a post request for this dish.
                    HttpWebRequest requestQuest = generateRequest("POST", "http://www.neopets.com/island/kitchen2.phtml", user.proxy, "http://www.neopets.com/island/kitchen.phtml");
                    //Quest kitchen accept has an 'origin' which i'll need to add. //http://www.neopets.com
                    String[] keys = { "food_desc" };
                    String[] values = { finalStringPlusified };
                    //read the string to confirm you have started the quest

                    using (HttpWebResponse responsecookie = writeData(requestQuest, keys, values))
                    {
                        try
                        {
                            using (StreamReader reader = new StreamReader(responsecookie.GetResponseStream()))
                                returnData = reader.ReadToEnd();
                        }
                        catch (Exception)
                        {
                            form.AppendText("Failed", user.username);
                            throw;
                        }

                        try
                        {
                            string searchfor = "80>";
                            string adjusted = "";

                            int PosItemOneStart = returnData.IndexOf(searchfor) + 10;
                            adjusted = returnData.Substring(PosItemOneStart);
                            int PosItemOneEnd = adjusted.IndexOf("<");
                            string itemOne = adjusted.Substring(0, PosItemOneEnd);
                            adjusted = adjusted.Substring(PosItemOneEnd);

                            int PosItemTwoStart = adjusted.IndexOf(searchfor) + 10;
                            adjusted = adjusted.Substring(PosItemTwoStart);
                            int PosItemTwoEnd = adjusted.IndexOf("<");
                            string itemTwo = adjusted.Substring(0, PosItemTwoEnd);
                            adjusted = adjusted.Substring(PosItemTwoEnd);

                            int PosItemThreeStart = adjusted.IndexOf(searchfor) + 10;
                            adjusted = adjusted.Substring(PosItemThreeStart);
                            int PosItemThreeEnd = adjusted.IndexOf("<");
                            string itemThree = adjusted.Substring(0, PosItemThreeEnd);
                            adjusted = adjusted.Substring(PosItemThreeEnd);

                            int PosItemFourStart = adjusted.IndexOf(searchfor) + 10;
                            adjusted = adjusted.Substring(PosItemFourStart);
                            int PosItemFourEnd = adjusted.IndexOf("<");
                            string itemFour = adjusted.Substring(0, PosItemFourEnd);
                            adjusted = adjusted.Substring(PosItemFourEnd);

                            int PosItemFiveStart = adjusted.IndexOf(searchfor) + 10;
                            adjusted = adjusted.Substring(PosItemFiveStart);
                            int PosItemFiveEnd = adjusted.IndexOf("<");
                            string itemFive = adjusted.Substring(0, PosItemFiveEnd);
                            adjusted = adjusted.Substring(PosItemFiveEnd);
                            //Proof of conept, obviously this won't make it into production.

                            if (itemOne != "")
                            {
                                if (buyFromShopWizard(user, itemOne) == 1)
                                    if (buyFromShopWizard(user, itemOne) == 1)
                                        if (buyFromShopWizard(user, itemOne) == 1)
                                            form.AppendText("Can;t find " + itemOne + " in the shop wizzard", user.username);
                                System.Threading.Thread.Sleep(250);
                            }

                            if (itemTwo != "")
                            {
                                if (buyFromShopWizard(user, itemTwo) == 1)
                                    if (buyFromShopWizard(user, itemTwo) == 1)
                                        if (buyFromShopWizard(user, itemTwo) == 1)
                                            form.AppendText("Can;t find " + itemTwo + " in the shop wizzard", user.username);
                            }
                            if (itemThree != "")
                            {
                                if (buyFromShopWizard(user, itemThree) == 1)
                                    if (buyFromShopWizard(user, itemThree) == 1)
                                        if (buyFromShopWizard(user, itemThree) == 1)
                                            form.AppendText("Can;t find " + itemThree + " in the shop wizzard", user.username);
                            }
                            if (itemFour != "")
                            {
                                if (buyFromShopWizard(user, itemFour) == 1)
                                    if (buyFromShopWizard(user, itemFour) == 1)
                                        if (buyFromShopWizard(user, itemFour) == 1)
                                            form.AppendText("Can;t find " + itemFour + " in the shop wizzard", user.username);
                            }
                            if (itemFive != "")
                            {
                                if (buyFromShopWizard(user, itemFive) == 1)
                                    if (buyFromShopWizard(user, itemFive) == 1)
                                        if (buyFromShopWizard(user, itemFive) == 1)
                                            form.AppendText("Can;t find " + itemFive + " in the shop wizzard", user.username);
                            }
                        }

                        catch (Exception)
                        {

                        }


                    }
                }
            }
        }
        
        public int buyFromShopWizard(Users user, string item)
        {
            string returnData = string.Empty;
            HttpWebRequest requestQuest = generateRequest("POST", "http://www.neopets.com/market.phtml", user.proxy, "http://www.neopets.com/market.phtml?type=wizard");
            String[] keys = { "type", "&feedset", "&shopwizard", "&table", "&criteria", "&min_price", "&max_price" };
            String[] values = { "process_wizard", "0", item.Replace(" ", "+"), "shop", "exact", "0", "25000" };
            //read the string to confirm you have started the quest

            using (HttpWebResponse responsecookie = writeData(requestQuest, keys, values))
            {
                try
                {
                    if (responsecookie == null)
                    {
                        form.AppendText("Responsecookie was null", user.username);
                        return 1;
                    }
                    using (StreamReader reader = new StreamReader(responsecookie.GetResponseStream()))
                        returnData = reader.ReadToEnd();
                }
                catch (Exception)
                {
                    form.AppendText("Failed", user.username);
                    throw;
                }
                //we have now search for the item we must parse the list of shops
                if (returnData.Contains("I did not find anything."))
                {
                    form.AppendText("Can't find item will try again " + item, user.username);
                    return 1;
                }
                int ownerStartPos = returnData.IndexOf("owner=") + 6;
                if (ownerStartPos < 0)
                {
                    return 1; //1 means try again
                }
                int ownerEndPos = returnData.IndexOf("&buy_obj_info_id");
                string shopuserName = returnData.Substring(ownerStartPos, ownerEndPos - ownerStartPos);

                int objectEndPos = returnData.IndexOf("&buy_cost_neopoints");
                string objectId = returnData.Substring(ownerEndPos + 17, objectEndPos - ownerEndPos - 17);

                string adjustedPattern = returnData.Substring(objectEndPos);
                int neopointCostEnd = adjustedPattern.IndexOf("\">");
                string neopointcost = adjustedPattern.Substring(0 + 20, neopointCostEnd - 20);
                form.AppendText("Buying " + item + " for " + neopointcost, user.username);

                //now we have shop and ID we can make a get request to shop
                HttpWebRequest request = generateRequest("GET", "http://www.neopets.com/browseshop.phtml?owner=" + shopuserName + "&buy_obj_info_id=" + objectId + "&buy_cost_neopoints=" + neopointcost + "&lower=0", user.proxy, "http://www.neopets.com/browseshop.phtml?owner=" + shopuserName + "&buy_obj_info_id=" + objectId + "&buy_cost_neopoints=" + neopointcost);
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    returnData = reader.ReadToEnd();
                //now we have made a get request to shop we can extract the BUY link

                int urlStart = returnData.IndexOf("A href=\"buy_item") + 8;
                string adjusted = returnData.Substring(urlStart);
                int urlEnd = adjusted.IndexOf("onClick") - 2;
                adjusted = adjusted.Substring(0, urlEnd);
                //now we have the buy link we can make a GET request to it
                //now we should have the item
                HttpWebRequest requestBuy = generateRequest("GET", "http://www.neopets.com/" + adjusted, user.proxy, "http://www.neopets.com/browseshop.phtml?owner=" + shopuserName + "&buy_obj_info_id=" + objectId + "&buy_cost_neopoints=" + neopointcost);
                using (HttpWebResponse response = (HttpWebResponse)requestBuy.GetResponse()) ;
                //
                return 0;
            }
        }
        public void startTrudy(Users user)
        {
            string returnData = string.Empty;
            HttpWebRequest trudyRequset = generateRequest("GET", "http://www.neopets.com/trudys_surprise.phtml", user.proxy);
            using (HttpWebResponse response = (HttpWebResponse)trudyRequset.GetResponse())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    returnData = reader.ReadToEnd();
            }
                
            HttpWebRequest requestQuest = generateRequest("POST", "http://www.neopets.com/trudydaily/ajax/claimprize.php ", user.proxy, "http://www.neopets.com/trudys_surprise.phtml?delevent=yes");
            String[] keys = { "action" };
            String[] values = { "beginroll" };
            //read the string to confirm you have started the quest

            using (HttpWebResponse responsecookie = writeData(requestQuest, keys, values))
            {
                System.Threading.Thread.Sleep(500);
                HttpWebRequest request = generateRequest("POST", "http://www.neopets.com/trudydaily/ajax/claimprize.php ", user.proxy, "http://www.neopets.com/trudys_surprise.phtml?delevent=yes");
                String[] keys2 = { "action" };
                String[] values2 = { "prizeclaimed" };
                //read the string to confirm you have started the quest

                using (HttpWebResponse response = writeData(request, keys2, values2))
                {
                    try
                    {
                        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                            returnData = reader.ReadToEnd();
                    }
                    catch (Exception)
                    {
                        form.AppendText("Failed", user.username);
                        throw;
                    }

                    if (returnData.Contains("Something went wrong!"))
                    {
                        form.AppendText("Trudy return diff, maybe it was already complete?", user.username);
                    }
                    else
                    {
                        form.AppendText("Trudy complete", user.username);
                        //log to sqlite the time/date
                    }

                }
            }
        }
    
   
    }
}
