using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
// using System.Web.Helpers;
// using System.Web.UI.WebControls;
using System.Collections.Specialized;
using System.Net;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json;
using System.Xml;
using Microsoft.VisualBasic;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            label11.Text = trackBar1.Value.ToString() + " X";
           
        }
        private void panel1_Paint(object sender, PaintEventArgs e)
        {
        
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.listView1.SelectedItems.Count == 0)
                return;
            textBox1.Text = listView1.SelectedItems[0].SubItems[8].Text;
           // label23.Text = "Entry: " + listView5.SelectedItems[0].SubItems[5].Text;
           // label22.Text = "ID: " + listView5.SelectedItems[0].Text;

            if (listView1.SelectedItems[0].SubItems[10].Text == "True")
            {
                button10.Enabled = true;

            }
            else
            {
                button10.Enabled = false;
            }
            if (listView1.SelectedItems[0].SubItems[11].Text == "false")
            {
                button3.Enabled = false;

            }
            else
            {
                button3.Enabled = true;
            }
            try
            {
                string ding = listView1.SelectedItems[0].SubItems[9].Text;
            }
            catch (ArgumentOutOfRangeException)
            {
                textBox2.Text = "";
                return;
                // Console.WriteLine("Error: {0}", outOfRange.Message);
            }

            textBox2.Text = listView1.SelectedItems[0].SubItems[9].Text;
            string trailstop = comboBox1.Text;
            if (trailstop == "") {
                return;
            }
            float trailstopf = (float)Convert.ToDouble(trailstop);
            float num = (float)Convert.ToDouble(this.listView1.SelectedItems[0].SubItems[6].Text);
            label20.Text = ((trailstopf / 100) * num).ToString();
        }
        private Dictionary<string, float> dictionary = new Dictionary<string, float>();
        private int datx;
        private void button1_Click(object sender, EventArgs e)
        {

            listView1.Items.Clear();

      
            string sURL;
            string apitoken = textBox7.Text;
            sURL = "https://1broker.com/api/v2/position/open.php?token=" + apitoken;
            //  System.Net.ServicePointManager.CertificatePolicy = new TrustAllCertificatePolicy();
            WebRequest wrGETURL;
            wrGETURL = WebRequest.Create(sURL);

            try
            {
                WebResponse responsetest = wrGETURL.GetResponse();
            }
            catch (WebException ex)
            {
                using (WebResponse response = ex.Response)
                {
                    if (datx == 1)
                    {
                        datx = 2;
                        ListViewItem lvi2 = new ListViewItem(DateTime.Now.ToString("HH:mm:ss tt"));
                        lvi2.SubItems.Add("Can't connect to 1Broker's API. Check your internet connection or complain to 1Broker.");
                        listView3.Items.Add(lvi2);
                        this.timer1.Stop();
                        return;
                    }
                    else
                    {
                        datx = 1;
                        this.PositionTimer();
                        return;
                    }
                }


            }
            Stream objStream;
            objStream = wrGETURL.GetResponse().GetResponseStream();

            StreamReader reader = new StreamReader(objStream);
            string responsestring = reader.ReadToEnd();

            dynamic positionlist = JsonConvert.DeserializeObject(responsestring);


            if (positionlist.error == "true")
            {
                ListViewItem lvi3 = new ListViewItem(DateTime.Now.ToString("HH:mm:ss tt"));
                lvi3.SubItems.Add("Error:" + positionlist.error_message);
                listView3.Items.Add(lvi3);

                return;
            }
            else
            {
                float postotal = 0;
                float pnltotal = 0;
                int datcount = positionlist["response"].Count;
                string datcount2 = datcount.ToString();
                // button2.PerformClick();
                //  MessageBox.Show("Item #" + i + ":" + posid);
                for (int i = 0; i < datcount; i++)
                {
                    string str12;
                    string positionid = positionlist["response"][i]["position_id"];
                    string possymbol = positionlist["response"][i]["symbol"];
                    string posmargin = positionlist["response"][i]["margin"];
                    string posleverage = positionlist["response"][i]["leverage"];
                    string posdirection = positionlist["response"][i]["direction"];
                    string posentry = positionlist["response"][i]["entry_price"];
                    // string posbid = positionlist["response"][i]["current_bid"];
                    string posmktclose = positionlist["response"][i]["market_close"];
                    string pospnl = positionlist["response"][i]["profit_loss"];
                    float pospnl2 = positionlist["response"][i]["profit_loss"];
                    float pospnlpct = positionlist["response"][i]["profit_loss_percent"];
                    float entryfloat = positionlist["response"][i]["entry_price"];
                    float levfloat = positionlist["response"][i]["leverage"];
                    float posbidfloat;
                    string posbid = posentry;
                    if (posdirection == "short")
                    {
                        posbidfloat = (entryfloat * (1-((pospnlpct / 100) / levfloat)));
                        posbid = posbidfloat.ToString();
                    }
                    if (posdirection == "long")
                    {
                        posbidfloat = (entryfloat * (1 + ((pospnlpct / 100) / levfloat)));
                        posbid = posbidfloat.ToString();
                    }
 
                    string possl = positionlist["response"][i]["stop_loss"];
                    string postp = positionlist["response"][i]["take_profit"];
                    if (postp == null)
                    {
                        postp = "";
                    }
                    if (dictionary.ContainsKey(string.Concat(positionid, "ts")))
                    {
                        str12 = dictionary[string.Concat(positionid, "ts")].ToString();
                    }
                    else
                    {
                        str12 = "false";
                    }
                    string[] row1 = { possymbol, posmargin, posleverage, posdirection, posentry, posbid, pospnl, possl, postp, posmktclose, str12 };
                    listView1.Items.Add(positionid).SubItems.AddRange(row1);
                    postotal = postotal + (float)Convert.ToDouble(posmargin);
                    pnltotal = pnltotal + (float)Convert.ToDouble(pospnl2);

                    if (pospnl2 < 0)
                    {

                        listView1.Items[i].ForeColor = Color.Red;
                    }
                    if (posmktclose == "True")
                    {

                        listView1.Items[i].BackColor = Color.LightGray;
                    }
                }

                //

             label18.Text = "Gross Size: " + postotal.ToString() + "BTC";
                label19.Text = "Net PNL: " + Math.Round(pnltotal,4).ToString() + "BTC";

            }
            ListViewItem lvi = new ListViewItem(DateTime.Now.ToString("HH:mm:ss tt"));
            lvi.SubItems.Add("Positions updated.");
            listView3.Items.Add(lvi);
            // string sURL3 = " https://1broker.com/api/v1/account/info.php?token=" + apitoken;
            string sURL3 = " https://1broker.com/api/v2/user/details.php?token=" + apitoken;


            WebRequest wrGETURL3;
            wrGETURL3 = WebRequest.Create(sURL3);
            try
            {
                WebResponse responsetest = wrGETURL3.GetResponse();
            }
            catch (WebException ex3)
            {
                using (WebResponse response3 = ex3.Response)
                {
                    ListViewItem lvi2 = new ListViewItem(DateTime.Now.ToString("HH:mm:ss tt"));
                    lvi2.SubItems.Add("Problem getting HTTPS, retry again or check internet connection.");
                    listView3.Items.Add(lvi2);
                    return;
                }


            }
            Stream objStream3;
            objStream3 = wrGETURL3.GetResponse().GetResponseStream();

            StreamReader reader3 = new StreamReader(objStream3);
            string responsestring3 = reader3.ReadToEnd();
            dynamic forexquotes3 = JsonConvert.DeserializeObject(responsestring3);
            if (forexquotes3.error == "true")
            {
                ListViewItem lvi3 = new ListViewItem(DateTime.Now.ToString("HH:mm:ss tt"));
                lvi3.SubItems.Add("Error:" + forexquotes3.error_message);
                listView3.Items.Add(lvi3);
                return;
            }
            else
            {
                string username = forexquotes3["response"]["username"];
                string balance = forexquotes3["response"]["balance"];
                label17.Text = "Balance: " + balance + " BTC";
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            listView2.Items.Clear();

            string sURL;
            string apitoken = textBox7.Text;

            sURL = "https://1broker.com/api/v2/order/open.php?token=" + apitoken;

            WebRequest wrGETURL;
            wrGETURL = WebRequest.Create(sURL);
            try
            {
                WebResponse responsetest = wrGETURL.GetResponse();
            }
            catch (WebException ex)
            {
                using (WebResponse response = ex.Response)
                {
                    ListViewItem lvi2 = new ListViewItem(DateTime.Now.ToString("HH:mm:ss tt"));
                    lvi2.SubItems.Add("Problem getting HTTPS, retry again or check internet connection.");
                    listView3.Items.Add(lvi2);
                    return;
                }


            }
            Stream objStream;
            objStream = wrGETURL.GetResponse().GetResponseStream();

            StreamReader reader = new StreamReader(objStream);
            string responsestring = reader.ReadToEnd();


            dynamic positionlist = JsonConvert.DeserializeObject(responsestring);

            if (positionlist.error == "true")
            {
                ListViewItem lvi2 = new ListViewItem(DateTime.Now.ToString("HH:mm:ss tt"));
                lvi2.SubItems.Add("Error:" + positionlist.error_message);
                listView3.Items.Add(lvi2);

                return;
            }
            else
            {

                float ordtotal = 0;
                int datcount = positionlist["response"].Count;
                string datcount2 = datcount.ToString();
                //   string rowcount2 = rowcount.ToString();
                // Loop through positions 
                for (int i = 0; i < datcount; i++)
                {
                    string orderid = positionlist["response"][i]["order_id"];
                    string ordsymbol = positionlist["response"][i]["symbol"];
                    string ordmargin = positionlist["response"][i]["margin"];
                    string ordleverage = positionlist["response"][i]["leverage"];
                    string orddirection = positionlist["response"][i]["direction"];
                    string ordtype = positionlist["response"][i]["order_type"];
                    string typeparm = positionlist["response"][i]["order_type_parameter"];
                    string[] row1 = { ordsymbol, ordmargin, ordleverage, orddirection, ordtype, typeparm };
                    listView2.Items.Add(orderid).SubItems.AddRange(row1);
                    ordtotal = ordtotal + (float)Convert.ToDouble(ordmargin);
                }

               // label28.Text = "Total: " + ordtotal.ToString() + " BTC";

            }
            ListViewItem lvi = new ListViewItem(DateTime.Now.ToString("HH:mm:ss tt"));
            lvi.SubItems.Add("Active orders updated.");
            listView3.Items.Add(lvi);
            button1.PerformClick();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            string marketselection;
            string sURL2;
            string apitoken = textBox7.Text;
            marketselection = comboBox2.Text;

            sURL2 = " https://1broker.com/api/v2/market/list.php?&token=" + apitoken + "&category=" + marketselection;
            comboBox3.Items.Clear();



            WebRequest wrGETURL2;
            wrGETURL2 = WebRequest.Create(sURL2);
            try
            {
                WebResponse responsetest = wrGETURL2.GetResponse();
            }
            catch (WebException ex2)
            {
                using (WebResponse response2 = ex2.Response)
                {
                    ListViewItem lvi = new ListViewItem(DateTime.Now.ToString("HH:mm:ss tt"));
                    lvi.SubItems.Add("Problem getting HTTPS, retry again or check internet connection.");
                    listView3.Items.Add(lvi);
                    return;
                }


            }
            Stream objStream2;
            objStream2 = wrGETURL2.GetResponse().GetResponseStream();

            StreamReader reader2 = new StreamReader(objStream2);
            string responsestring2 = reader2.ReadToEnd();
            dynamic forexquotes2 = JsonConvert.DeserializeObject(responsestring2);
            if (forexquotes2.error == "true")
            {
                ListViewItem lvi = new ListViewItem(DateTime.Now.ToString("HH:mm:ss tt"));
                lvi.SubItems.Add("Error:" + forexquotes2.error_message);
                listView3.Items.Add(lvi);
                return;
            }
            else
            {



               // marketselection = comboBox2.Text;
                int datcount1 = forexquotes2["response"].Count;
                string datcount12 = datcount1.ToString();
                int fxcount = 0;
                for (int i = 0; i < datcount1; i++)
                {

                    string fxcategory = forexquotes2["response"][i]["category"];
                    string fxsymbol = forexquotes2["response"][i]["symbol"];
                    if (fxcategory == marketselection)
                    {
                        fxcount++;
                        // MessageBox.Show(fxsymbol);
                        comboBox3.Items.Add(fxsymbol);

                    }

                }

            }



        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
            {
                ListViewItem lvi = new ListViewItem(DateTime.Now.ToString("HH:mm:ss tt"));
                lvi.SubItems.Add("Select an item.");
                listView3.Items.Add(lvi);
                return;
            }
            else
            {

                string apitoken = textBox7.Text;
                string sURL;

                // String posid = listView5.SelectedItems[0].Text;

                String poseditsl = textBox1.Text;
                String posedittp = textBox2.Text;

                int count = this.listView1.SelectedItems.Count;
                int num = 0;
                while (num < count)
                {
                   
                    String posid = listView1.SelectedItems[num].Text;
                    if (posedittp == "")
                    {
                        sURL = "https://1broker.com/api/v2/position/edit.php?position_id=" + posid + "&stop_loss=" + poseditsl + "&take_profit=null" + "&token=" + apitoken;

                    }
                    else
                    {
                        sURL = "https://1broker.com/api/v2/position/edit.php?position_id=" + posid + "&stop_loss=" + poseditsl + "&take_profit=" + posedittp + "&token=" + apitoken;
                    }
                   
                    WebRequest wrGETURL;
                    wrGETURL = WebRequest.Create(sURL);
                    try
                    {
                        WebResponse responsetest = wrGETURL.GetResponse();
                    }
                    catch (WebException ex)
                    {
                        using (WebResponse response = ex.Response)
                        {
                            ListViewItem lvi = new ListViewItem(DateTime.Now.ToString("HH:mm:ss tt"));
                            lvi.SubItems.Add("Problem with 1Broker servers, retry again or check internet connection.");
                            listView3.Items.Add(lvi);
                            return;
                        }


                    }
                    Stream objStream;
                    objStream = wrGETURL.GetResponse().GetResponseStream();

                    StreamReader reader = new StreamReader(objStream);
                    string responsestring = reader.ReadToEnd();

                    dynamic poseditresp = JsonConvert.DeserializeObject(responsestring);



                    if (poseditresp.error == "true")
                    {
                        ListViewItem lvi = new ListViewItem(DateTime.Now.ToString("HH:mm:ss tt"));
                        lvi.SubItems.Add("Error:" + poseditresp.error_message);
                        listView3.Items.Add(lvi);
                        button1.PerformClick();
                        
                        return;
                    }
                    else
                    {
                        // button2.PerformClick();
                        //  MessageBox.Show("Item #" + i + ":" + posid);
                        num++;
                        
                       
                    }
                 

                }
                ListViewItem lvi1 = new ListViewItem(DateTime.Now.ToString("HH:mm:ss tt"));
                lvi1.SubItems.Add("Position(s) edited.");
                listView3.Items.Add(lvi1);
                button1.PerformClick();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

            string trailstop = comboBox1.Text;
            if (trailstop == "")
            {
                return;
            }
            if (listView1.SelectedItems.Count == 0)
            {
                return;
            }
            float trailstopf = (float)Convert.ToDouble(trailstop);
            float num = (float)Convert.ToDouble(this.listView1.SelectedItems[0].SubItems[6].Text);
            label20.Text = ((trailstopf / 100)*num).ToString();
     
        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            label11.Text = trackBar1.Value.ToString() + " X";
            textBox8.Text = trackBar1.Value.ToString();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            string sURL;
            string apitoken = textBox7.Text;
            string dirord;

            if (radioButton1.Checked == true)
            {
                dirord = "long";
            }
            else
            {
                dirord = "short";
            }
            String levord = trackBar1.Value.ToString();
            String marginord = textBox3.Text;
            String ordparam = textBox6.Text;
            String symbolord = comboBox3.Text;
            String typord = comboBox4.Text;
            String stoploss = textBox5.Text;
            String takeprofit = textBox4.Text;
            string stoplossc;
            string takeprofitc;
            if (stoploss == "")
            {
                stoplossc = "";
            }
            else
            {
                stoplossc = "&stop_loss=" + stoploss;
            }
            if (takeprofit == "")
            {
                takeprofitc = "";
            }
            else
            {
                takeprofitc = "&take_profit=" + takeprofit;
            }
            // sURL = " https://1broker.com/api/v1/order/create.php?symbol=" + symbolord + "&margin=" + marginord + "&direction=" + dirord + "&leverage=" + levord + "&order_type=" + typord + "&order_type_parameter=" + ordparam + stoplossc + takeprofitc + "&token=" + apitoken + "&referral_id=4333";
            sURL = " https://1broker.com/api/v2/order/create.php?symbol=" + symbolord + "&margin=" + marginord + "&direction=" + dirord + "&leverage=" + levord + "&order_type=" + typord + "&order_type_parameter=" + ordparam + stoplossc + takeprofitc + "&token=" + apitoken + "&referral_id=4333";

            if (symbolord == "" || dirord == "" || levord == "" || marginord == "")
            {
                ListViewItem lvi = new ListViewItem(DateTime.Now.ToString("HH:mm:ss tt"));
                lvi.SubItems.Add("Fill out appropriate fields.");
                listView3.Items.Add(lvi);
                return;
            }
            else
            {


                if (typord == "limit")
                {
                if (ordparam == "")
                    {
                        ListViewItem lvi = new ListViewItem(DateTime.Now.ToString("HH:mm:ss tt"));
                        lvi.SubItems.Add("Limit requires entering limit price.");
                        listView3.Items.Add(lvi);
                        return;
                    }
                    //// CODE FOR CHECKING IF PROPER LIMIT
                    string sURL4 = " https://1broker.com/api/v2/market/quotes.php?symbols=" + symbolord + "&token=" + apitoken;




                    WebRequest wrGETURL4;
                    wrGETURL4 = WebRequest.Create(sURL4);
                    try
                    {
                        WebResponse responsetest3 = wrGETURL4.GetResponse();
                    }
                    catch (WebException ex3)
                    {
                        using (WebResponse response3 = ex3.Response)
                        {
                            ListViewItem lvi = new ListViewItem(DateTime.Now.ToString("HH:mm:ss tt"));
                            lvi.SubItems.Add("Problem getting HTTPS, retry again or check internet connection.");
                            listView3.Items.Add(lvi); 
                            return;
                        }


                    }
                    Stream objStream6;
                    objStream6 = wrGETURL4.GetResponse().GetResponseStream();

                    StreamReader reader6 = new StreamReader(objStream6);
                    string responsestring6 = reader6.ReadToEnd();
                    dynamic accountinfo = JsonConvert.DeserializeObject(responsestring6);
                    if (accountinfo.error == "true")
                    {
                        ListViewItem lvi = new ListViewItem(DateTime.Now.ToString("HH:mm:ss tt"));
                        lvi.SubItems.Add("Error:" + accountinfo.error_message);
                        listView3.Items.Add(lvi); 
                        return;
                    }
                    else
                    {


                        /// CHECK WHAT PRICE CURRENTLY IS, SEE IF GIVEN INITIAL PRICE IS ABOVE IF SHORT AND BELOW IF LONG
                        /// 
                        string stairquote = accountinfo["response"][0]["bid"];
                        string stairquotea = accountinfo["response"][0]["ask"];
                        // MessageBox.Show("current quote: " + stairquote);
                        float stairquoteint = (float)Convert.ToDouble(stairquote);
                        float stairquoteinta = (float)Convert.ToDouble(stairquotea);
                        float limitcheckparam = (float)Convert.ToDouble(ordparam);

                        if (dirord == "long" && stairquoteinta < limitcheckparam)
                        {
                            ListViewItem lvi = new ListViewItem(DateTime.Now.ToString("HH:mm:ss tt"));
                            lvi.SubItems.Add("For limit long position, entry price for " + symbolord + " must be below current spot quote: " + stairquotea);
                            listView3.Items.Add(lvi);
                            return;
                        }
                        else if (dirord == "short" && stairquoteint > limitcheckparam)
                        {
                            ListViewItem lvi = new ListViewItem(DateTime.Now.ToString("HH:mm:ss tt"));
                            lvi.SubItems.Add("For limit short position, entry price for " + symbolord + " must be above current spot quote: " + stairquote);
                            listView3.Items.Add(lvi);

                            return;
                        }

                    }
                }

                else if (typord == "stop_entry")
                {
                    if (ordparam == "")
                    {
                        ListViewItem lvi = new ListViewItem(DateTime.Now.ToString("HH:mm:ss tt"));
                        lvi.SubItems.Add("StopEntry requires entering stop price.");
                        listView3.Items.Add(lvi);
                        return;
                    }
                    //// CODE FOR CHECKING IF PROPER STOPENTRY
                    string sURL4 = " https://1broker.com/api/v2/market/quotes.php?symbols=" + symbolord + "&token=" + apitoken;




                    WebRequest wrGETURL4;
                    wrGETURL4 = WebRequest.Create(sURL4);
                    try
                    {
                        WebResponse responsetest3 = wrGETURL4.GetResponse();
                    }
                    catch (WebException ex3)
                    {
                        using (WebResponse response3 = ex3.Response)
                        {
                            ListViewItem lvi = new ListViewItem(DateTime.Now.ToString("HH:mm:ss tt"));
                            lvi.SubItems.Add("Problem getting HTTPS, retry again or check internet connection.");
                            listView3.Items.Add(lvi);
                            return;
                        }


                    }
                    Stream objStream5;
                    objStream5 = wrGETURL4.GetResponse().GetResponseStream();

                    StreamReader reader5 = new StreamReader(objStream5);
                    string responsestring5 = reader5.ReadToEnd();
                    dynamic forexquotes6 = JsonConvert.DeserializeObject(responsestring5);
                    if (forexquotes6.error == "true")
                    {
                        ListViewItem lvi = new ListViewItem(DateTime.Now.ToString("HH:mm:ss tt"));
                        lvi.SubItems.Add("Error:" + forexquotes6.error_message);
                        listView3.Items.Add(lvi);
                        return;
                    }
                    else
                    {


                        /// CHECK WHAT PRICE CURRENTLY IS, SEE IF GIVEN INITIAL PRICE IS ABOVE IF SHORT AND BELOW IF LONG
                        /// 
                        string stairquote = forexquotes6["response"][0]["bid"];
                        string stairquotea = forexquotes6["response"][0]["ask"];
                        // MessageBox.Show("current quote: " + stairquote);
                        float stairquoteint = (float)Convert.ToDouble(stairquote);
                        float stairquoteinta = (float)Convert.ToDouble(stairquotea);
                        float limitcheckparam = (float)Convert.ToDouble(ordparam);

                        if (dirord == "long" && stairquoteinta > limitcheckparam)
                        {
                            ListViewItem lvi = new ListViewItem(DateTime.Now.ToString("HH:mm:ss tt"));
                            lvi.SubItems.Add("For stopentry long position, entry price for " + symbolord + " must be above current spot quote: " + stairquote);
                            listView3.Items.Add(lvi);
                            return;
                        }
                        else if (dirord == "short" && stairquoteint < limitcheckparam)
                        {
                            ListViewItem lvi = new ListViewItem(DateTime.Now.ToString("HH:mm:ss tt"));
                            lvi.SubItems.Add("For stopentry short position, entry price for " + symbolord + " must be below current spot quote: " + stairquote);
                            listView3.Items.Add(lvi);
                            return;
                        }

                    }
                }


                WebRequest wrGETURL;
                wrGETURL = WebRequest.Create(sURL);
                try
                {
                    WebResponse responsetest = wrGETURL.GetResponse();
                }
                catch (WebException ex)
                {
                    using (WebResponse response = ex.Response)
                    {
                        ListViewItem lvi = new ListViewItem(DateTime.Now.ToString("HH:mm:ss tt"));
                        lvi.SubItems.Add("Problem getting HTTPS, retry again or check internet connection.");
                        listView3.Items.Add(lvi);
                        return;
                    }


                }
                Stream objStream;
                objStream = wrGETURL.GetResponse().GetResponseStream();

                StreamReader reader = new StreamReader(objStream);
                string responsestring = reader.ReadToEnd();
                dynamic ordercancelresp = JsonConvert.DeserializeObject(responsestring);
                if (ordercancelresp.error == "true")
                {
                    ListViewItem lvi = new ListViewItem(DateTime.Now.ToString("HH:mm:ss tt"));
                    lvi.SubItems.Add("Error:" + ordercancelresp.error_message);
                    listView3.Items.Add(lvi);
                    return;
                }
                else
                {
                    button5.PerformClick();
                    ListViewItem lvi = new ListViewItem(DateTime.Now.ToString("HH:mm:ss tt"));
                    lvi.SubItems.Add("Order placed");
                    listView3.Items.Add(lvi);
                }
            
                // string sURL3 = " https://1broker.com/api/v1/account/info.php?token=" + apitoken;
                string sURL3 = " https://1broker.com/api/v2/user/details.php?token=" + apitoken;


                WebRequest wrGETURL3;
                wrGETURL3 = WebRequest.Create(sURL3);
                try
                {
                    WebResponse responsetest = wrGETURL3.GetResponse();
                }
                catch (WebException ex3)
                {
                    using (WebResponse response3 = ex3.Response)
                    {
                        ListViewItem lvi = new ListViewItem(DateTime.Now.ToString("HH:mm:ss tt"));
                        lvi.SubItems.Add("Problem getting HTTPS, retry again or check internet connection.");
                        listView3.Items.Add(lvi);
                        return;
                    }


                }
                Stream objStream3;
                objStream3 = wrGETURL3.GetResponse().GetResponseStream();

                StreamReader reader3 = new StreamReader(objStream3);
                string responsestring3 = reader3.ReadToEnd();
                dynamic forexquotes3 = JsonConvert.DeserializeObject(responsestring3);
                if (forexquotes3.error == "true")
                {
                    ListViewItem lvi = new ListViewItem(DateTime.Now.ToString("HH:mm:ss tt"));
                    lvi.SubItems.Add("Error:" + forexquotes3.error_message);
                    listView3.Items.Add(lvi);
                    return;
                }
                else
                {
                    string username = forexquotes3["response"]["username"];
                    string balance = forexquotes3["response"]["balance"];
                    label17.Text = "Balance: " + balance + " BTC";
                }
            }
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox4.Text == "limit" || comboBox4.Text == "stop_entry")
            {
                textBox6.Enabled = true;
            }
            else
            {
                textBox6.Enabled = false;
            }
        }

        private void label13_Click(object sender, EventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (this.listView2.SelectedItems.Count != 0)
            {
                int count = this.listView2.SelectedItems.Count;
                count.ToString();
                int num = 0;
                while (num < count)
                {
                    string text = textBox7.Text;
                    string str = this.listView2.SelectedItems[num].Text;
                    string str1 = string.Concat(" https://1broker.com/api/v2/order/cancel.php?order_id=", str, "&token=", text);
                    WebRequest webRequest = WebRequest.Create(str1);
                    try
                    {
                        webRequest.GetResponse();
                    }
                    catch (WebException webException)
                    {
                        WebResponse response = webException.Response;
                        try
                        {
                            listView3.Items.Add("Problem getting HTTPS, retry or see if your internet connection/1Broker servers are down.");
                            return;
                        }
                        finally
                        {
                            if (response != null)
                            {
                                ((IDisposable)response).Dispose();
                            }
                        }
                    }
                    Stream responseStream = webRequest.GetResponse().GetResponseStream();
                    dynamic obj = JsonConvert.DeserializeObject((new StreamReader(responseStream)).ReadToEnd());
                    if (!(obj.error == "true"))
                    {
                        num++;
                    }
                    else
                    {
                        ListViewItem lvi2 = new ListViewItem(DateTime.Now.ToString("HH:mm:ss tt"));
                        lvi2.SubItems.Add("Error: " + obj.error_message);
                        listView3.Items.Add(lvi2);

                        return;
                    }
                }
                button5.PerformClick();
                ListViewItem lvi = new ListViewItem(DateTime.Now.ToString("HH:mm:ss tt"));
                lvi.SubItems.Add("Order(s) canceled");
                listView3.Items.Add(lvi);

            }
            else
            {
                ListViewItem lvi = new ListViewItem(DateTime.Now.ToString("HH:mm:ss tt"));
                lvi.SubItems.Add("Select an item");
                listView3.Items.Add(lvi);

            }
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            string symbolselection;
            string sURL2;
            string sURL3;
            string apitoken = textBox7.Text;
            symbolselection = comboBox3.Text;

            sURL2 = " https://1broker.com/api/v2/market/details.php?&symbol=" + symbolselection + "&token=" + apitoken;


            WebRequest wrGETURL2;
            wrGETURL2 = WebRequest.Create(sURL2);
            try
            {
                WebResponse responsetest = wrGETURL2.GetResponse();
            }
            catch (WebException ex2)
            {
                using (WebResponse response2 = ex2.Response)
                {
                    ListViewItem lvi = new ListViewItem(DateTime.Now.ToString("HH:mm:ss tt"));
                    lvi.SubItems.Add("Problem getting HTTPS, retry again or check internet connection.");
                    listView3.Items.Add(lvi);
                    return;
                }


            }
            Stream objStream2;
            objStream2 = wrGETURL2.GetResponse().GetResponseStream();

            StreamReader reader2 = new StreamReader(objStream2);
            string responsestring2 = reader2.ReadToEnd();
            dynamic forexquotes2 = JsonConvert.DeserializeObject(responsestring2);
            if (forexquotes2.error == "true")
            {
                ListViewItem lvi = new ListViewItem(DateTime.Now.ToString("HH:mm:ss tt"));
                lvi.SubItems.Add("Error:" + forexquotes2.error_message);
                listView3.Items.Add(lvi);
                return;
            }
            else
            {
               trackBar1.Maximum = forexquotes2["response"]["maximum_leverage"];
                trackBar1.Value = forexquotes2["response"]["maximum_leverage"];
                label11.Text = trackBar1.Value + " X";
            }


            sURL3 = " https://1broker.com/api/v2/market/quotes.php?symbols=" + symbolselection + "&token=" + apitoken;


            WebRequest wrGETURL3;
            wrGETURL3 = WebRequest.Create(sURL3);
            try
            {
                WebResponse responsetest = wrGETURL3.GetResponse();
            }
            catch (WebException ex3)
            {
                using (WebResponse response3 = ex3.Response)
                {
                    ListViewItem lvi = new ListViewItem(DateTime.Now.ToString("HH:mm:ss tt"));
                    lvi.SubItems.Add("Problem getting HTTPS, retry again or check internet connection.");
                    listView3.Items.Add(lvi);
                    return;
                }


            }
            Stream objStream3;
            objStream3 = wrGETURL3.GetResponse().GetResponseStream();

            StreamReader reader3 = new StreamReader(objStream3);
            string responsestring3 = reader3.ReadToEnd();
            dynamic forexquotes3 = JsonConvert.DeserializeObject(responsestring3);
            if (forexquotes3.error == "true")
            {
                ListViewItem lvi = new ListViewItem(DateTime.Now.ToString("HH:mm:ss tt"));
                lvi.SubItems.Add("Error:" + forexquotes3.error_message);
                listView3.Items.Add(lvi);
                return;
            }
            else
            {
                string bid = forexquotes3["response"][0]["bid"];
                string ask = forexquotes3["response"][0]["ask"];
                label16.Text = "bid: " + bid + ", ask: " + ask;
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            string str;
            if ((this.listView1.SelectedItems.Count == 0 ? false : !(this.comboBox1.Text == "")))
            {
                int count = this.listView1.SelectedItems.Count;
                string text = textBox7.Text;
                for (int i = 0; i < count; i++)
                {
                    string text1 = this.listView1.SelectedItems[i].Text;
                    float num = (float)Convert.ToDouble(this.listView1.SelectedItems[i].SubItems[6].Text);
                    float single = (float)Convert.ToDouble(this.listView1.SelectedItems[i].SubItems[5].Text);
                    float num1 = (float)Convert.ToDouble(this.listView1.SelectedItems[i].SubItems[7].Text);
                    string str1 = this.listView1.SelectedItems[i].SubItems[4].Text;
                    string text2 = this.comboBox1.Text;
                    float single1 = (float)Convert.ToDouble(text2) / 100f;
                    float single2 = num * single1;
                    float single3 = num + single2;
                    float single4 = num - single2;
                    if (dictionary.ContainsKey(string.Concat(text1, "ts")))
                    {
                        dictionary.Remove(string.Concat(text1, "ts"));
                    }
                    if (!(str1 == "long"))
                    {
                        if (num > single3)
                        {
                            listView3.Items.Add("Your trail stop must be above the current price for short position.");
                        }
                        str = single3.ToString();
                    }
                    else
                    {
                        if (num < single4)
                        {
                            listView3.Items.Add("Your trail stop must be below the current price for long position.");
                        }
                        str = single4.ToString();
                    }
                    string[] strArrays = new string[] { "https://1broker.com/api/v2/position/edit.php?position_id=", text1, "&stop_loss=", str, "&token=", text };
                    WebRequest webRequest = WebRequest.Create(string.Concat(strArrays));
                    try
                    {
                        webRequest.GetResponse();
                    }
                    catch (WebException webException)
                    {
                        WebResponse response = webException.Response;
                        try
                        {
                            ListViewItem lvi = new ListViewItem(DateTime.Now.ToString("HH:mm:ss tt"));
                            lvi.SubItems.Add("Problem getting HTTPS, retry again or check internet connection.");
                            listView3.Items.Add(lvi);
                            return;
                        }
                        finally
                        {
                            if (response != null)
                            {
                                ((IDisposable)response).Dispose();
                            }
                        }
                    }
                    Stream responseStream = webRequest.GetResponse().GetResponseStream();
                    dynamic obj = JsonConvert.DeserializeObject((new StreamReader(responseStream)).ReadToEnd());
                    if (!(obj.error == "true"))
                    {
                        if (this.dictionary.ContainsKey(string.Concat(text1, "ts")))
                        {
                            this.dictionary.Remove(string.Concat(text1, "ts"));
                        }
                        this.dictionary.Add(string.Concat(text1, "ts"), (float)Convert.ToDouble(text2));
                    }
                    else
                    {
                        ListViewItem lvi = new ListViewItem(DateTime.Now.ToString("HH:mm:ss tt"));
                        lvi.SubItems.Add("Error:" + obj.error_message);
                        listView3.Items.Add(lvi);
                        button1.PerformClick();
                        return;
                    }
                }
                PositionTimer();
                button1.PerformClick();
            }
            else
            {
                // this.textBox20.Text = "";
                // this.textBox21.Text = "";
                // this.label23.Text = "";
                // this.label22.Text = "";
                ListViewItem lvi = new ListViewItem(DateTime.Now.ToString("HH:mm:ss tt"));
                lvi.SubItems.Add("Select an item or TS value.");
                listView3.Items.Add(lvi);
            }
            return;
        }
        private void PositionTimer()
        {
            button1.PerformClick();
            timer1.Interval = 20000;
            timer1.Start();
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            float single;
            float single1;
            int count = this.listView1.Items.Count;
            string text = textBox7.Text;
            string str = "";
            int num = 0;
            int num1 = 0;
            while (true)
            {
                if (num1 < count)
                {
                    // MessageBox.Show("firstif");
                    if (!(this.listView1.Items[num1].SubItems[11].Text == "false"))
                    {
                        num++;
                        // MessageBox.Show("othernum");
                        string text1 = this.listView1.Items[num1].Text;
                        float num2 = (float)Convert.ToDouble(this.listView1.Items[num1].SubItems[6].Text);
                        float single2 = (float)Convert.ToDouble(this.listView1.Items[num1].SubItems[5].Text);
                        float num3 = (float)Convert.ToDouble(this.listView1.Items[num1].SubItems[7].Text);
                        float single3 = (float)Convert.ToDouble(this.listView1.Items[num1].SubItems[8].Text);
                        string str1 = this.listView1.Items[num1].SubItems[4].Text;
                        string text2 = this.listView1.Items[num1].SubItems[11].Text;
                        float num4 = (float)Convert.ToDouble(text2) / 100f;
                        float single4 = num2 * num4;
                        single = num2 + single4;
                        single1 = num2 - single4;
                        if (!(str1 == "long"))
                        {
                            if ((single >= single3 ? false : single > num2))
                            {
                                str = single.ToString();
                                string[] strArrays = new string[] { "https://1broker.com/api/v2/position/edit.php?position_id=", text1, "&stop_loss=", str, "&token=", text };
                                WebRequest webRequest = WebRequest.Create(string.Concat(strArrays));
                                try
                                {
                                    webRequest.GetResponse();
                                }
                                catch (WebException webException)
                                {
                                    WebResponse response = webException.Response;
                                    try
                                    {
                                        break;
                                    }
                                    finally
                                    {
                                        if (response != null)
                                        {
                                            ((IDisposable)response).Dispose();
                                        }
                                    }
                                }
                                Stream responseStream = webRequest.GetResponse().GetResponseStream();
                                dynamic obj = JsonConvert.DeserializeObject((new StreamReader(responseStream)).ReadToEnd());
                                if (obj.error == "true")
                                {
                                    ListViewItem lvi = new ListViewItem(DateTime.Now.ToString("HH:mm:ss tt"));
                                    lvi.SubItems.Add("Error:" + obj.error_message);
                                    listView3.Items.Add(lvi);
                                    button1.PerformClick();
                                    button1.PerformClick();
                                    break;
                                }
                            }
                            num1++;
                            //MessageBox.Show(num1.ToString());
                        }
                        else
                        {
                            if ((single1 <= single3 ? false : single1 < num2))
                            {
                                str = single1.ToString();
                                string[] strArrays = new string[] { "https://1broker.com/api/v2/position/edit.php?position_id=", text1, "&stop_loss=", str, "&token=", text };
                                WebRequest webRequest = WebRequest.Create(string.Concat(strArrays));
                                try
                                {
                                    webRequest.GetResponse();
                                }
                                catch (WebException webException)
                                {
                                    WebResponse response = webException.Response;
                                    try
                                    {
                                        break;
                                    }
                                    finally
                                    {
                                        if (response != null)
                                        {
                                            ((IDisposable)response).Dispose();
                                        }
                                    }
                                }
                                Stream responseStream = webRequest.GetResponse().GetResponseStream();
                                dynamic obj = JsonConvert.DeserializeObject((new StreamReader(responseStream)).ReadToEnd());
                                if (obj.error == "true")
                                {
                                    ListViewItem lvi = new ListViewItem(DateTime.Now.ToString("HH:mm:ss tt"));
                                    lvi.SubItems.Add("Error:" + obj.error_message);
                                    listView3.Items.Add(lvi);
                                    button1.PerformClick();
                                    break;
                                }
                            }
                            num1++;
                            //MessageBox.Show(num1.ToString());
                        }
           
                    }   
                   num1++;
                }
                else if (num != 0)
                {
                    // MessageBox.Show("secondelseif");
                    button1.PerformClick();
                    break;
                }
                else
                {
                   // MessageBox.Show("theelse");
                    this.timer1.Stop();
                    break;
                }
            }
            return;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to close ALL open positions at market price?", "Confirmation", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.No)
            {
                return;
            }
            else
            {


                string apitoken = textBox7.Text;
                if (listView1.Items.Count == 0)
                {
                    return;
                }
                else
                {
                    string sURL;



                    int poscount = listView1.Items.Count;
                    string poscounts = poscount.ToString();
                    for (int i = 0; i < poscount; i++)
                    {

                        String posid = listView1.Items[i].Text;
                        // sURL = "https://1broker.com/api/v1/position/edit.php?position_id=" + posid + "&market_close=true&token=" + apitoken;
                        sURL = "https://1broker.com/api/v2/position/close.php?position_id=" + posid + "&token=" + apitoken;

                        WebRequest wrGETURL;
                        wrGETURL = WebRequest.Create(sURL);
                        try
                        {
                            WebResponse responsetest = wrGETURL.GetResponse();
                        }
                        catch (WebException ex)
                        {
                            using (WebResponse response = ex.Response)
                            {
                                ListViewItem lvi2 = new ListViewItem(DateTime.Now.ToString("HH:mm:ss tt"));
                                lvi2.SubItems.Add("Problem getting HTTPS, retry again or check internet connection.");
                                listView3.Items.Add(lvi2);
                                return;
                            }


                        }
                        Stream objStream;
                        objStream = wrGETURL.GetResponse().GetResponseStream();

                        StreamReader reader = new StreamReader(objStream);
                        string responsestring = reader.ReadToEnd();

                        dynamic poscancelresp = JsonConvert.DeserializeObject(responsestring);



                        if (poscancelresp.error == "true")
                        {
                            ListViewItem lvi3 = new ListViewItem(DateTime.Now.ToString("HH:mm:ss tt"));
                            lvi3.SubItems.Add("Error:" + poscancelresp.error_message);
                            listView3.Items.Add(lvi3);
                            button2.PerformClick();
                            return;
                        }
                        else
                        {
                            // button2.PerformClick();
                            //  MessageBox.Show("Item #" + i + ":" + posid);
                            continue;
                        }

                    }
                    button1.PerformClick();
                    ListViewItem lvi = new ListViewItem(DateTime.Now.ToString("HH:mm:ss tt"));
                    lvi.SubItems.Add("Position(s) closed.");
                    listView3.Items.Add(lvi);
                    button1.PerformClick();

                }



            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            string apitoken = textBox7.Text;
            if (listView1.SelectedItems.Count == 0)
            {
                return;
            }
            else
            {
                string sURL;



                int poscount = listView1.SelectedItems.Count;
                string poscounts = poscount.ToString();
                for (int i = 0; i < poscount; i++)
                {

                    String posid = listView1.SelectedItems[i].Text;

                    // sURL = "https://1broker.com/api/v1/position/edit.php?position_id=" + posid + "&market_close=true&token=" + apitoken;
                    sURL = "https://1broker.com/api/v2/position/close.php?position_id=" + posid + "&token=" + apitoken;

                    WebRequest wrGETURL;
                    wrGETURL = WebRequest.Create(sURL);
                    try
                    {
                        WebResponse responsetest = wrGETURL.GetResponse();
                    }
                    catch (WebException ex)
                    {
                        using (WebResponse response = ex.Response)
                        {
                            ListViewItem lvi3 = new ListViewItem(DateTime.Now.ToString("HH:mm:ss tt"));
                            lvi3.SubItems.Add("Problem getting HTTPS, retry again or check internet connection.");
                            listView3.Items.Add(lvi3);
                            return;
                        }


                    }
                    Stream objStream;
                    objStream = wrGETURL.GetResponse().GetResponseStream();

                    StreamReader reader = new StreamReader(objStream);
                    string responsestring = reader.ReadToEnd();

                    dynamic poscancelresp = JsonConvert.DeserializeObject(responsestring);



                    if (poscancelresp.error == "true")
                    {
                        listView3.Items.Add("Error:" + poscancelresp.error_message);
                        button2.PerformClick();
                        return;
                    }
                    else
                    {
                        // button2.PerformClick();
                        //  MessageBox.Show("Item #" + i + ":" + posid);
                        continue;
                    }

                }
                button1.PerformClick();
                ListViewItem lvi = new ListViewItem(DateTime.Now.ToString("HH:mm:ss tt"));
                lvi.SubItems.Add("Position(s) closed.");
                listView3.Items.Add(lvi);
                button1.PerformClick();

            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int count = this.listView1.SelectedItems.Count;
            if (count != 0)
            {
            
                for (int i = 0; i < count; i++)
                {
                    string text = this.listView1.SelectedItems[i].Text;
                    if (this.dictionary.ContainsKey(string.Concat(text, "ts")))
                    {
                        this.dictionary.Remove(string.Concat(text, "ts"));
                    }
                }
                button1.PerformClick();
                this.button3.Enabled = false;
            }
            else
            {
                listView3.Items.Add("Select item");
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {

            string theapitoken = Microsoft.VisualBasic.Interaction.InputBox("Enter API Token",
                       "Simple 1Broker C# Trader",
                       "Default",
                       0,
                       0);
            if (theapitoken == "")
            {
                return;
            }
            else
            {

            textBox7.Text = theapitoken;

            }
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            try
            {
                XmlWriter writer1 = XmlWriter.Create("settings.xml", settings);
                writer1.Close();
            }
            catch
            {
                MessageBox.Show("Error writing API token to file");
                return;
            }
            XmlWriter writer = XmlWriter.Create("settings.xml", settings);
            writer.WriteStartDocument();
            writer.WriteComment("1Broker API Connector");
            writer.WriteStartElement("Token");
            writer.WriteElementString("Value", textBox7.Text);
            writer.WriteEndElement();
            writer.WriteEndDocument();

            writer.Flush();
            writer.Close();
            string apitoken = textBox7.Text;
            string sURL3 = " https://1broker.com/api/v2/user/details.php?token=" + apitoken;


            WebRequest wrGETURL3;
            wrGETURL3 = WebRequest.Create(sURL3);
            try
            {
                WebResponse responsetest = wrGETURL3.GetResponse();
            }
            catch (WebException ex3)
            {
                using (WebResponse response3 = ex3.Response)
                {
                    ListViewItem lvi = new ListViewItem(DateTime.Now.ToString("HH:mm:ss tt"));
                    lvi.SubItems.Add("Problem getting HTTPS, retry again or check internet connection.");
                    listView3.Items.Add(lvi);
                    return;
                }


            }
            Stream objStream3;
            objStream3 = wrGETURL3.GetResponse().GetResponseStream();

            StreamReader reader3 = new StreamReader(objStream3);
            string responsestring3 = reader3.ReadToEnd();
            dynamic forexquotes3 = JsonConvert.DeserializeObject(responsestring3);
            if (forexquotes3.error == "true")
            {
                ListViewItem lvi = new ListViewItem(DateTime.Now.ToString("HH:mm:ss tt"));
                lvi.SubItems.Add("Error:" + forexquotes3.error_message);
                listView3.Items.Add(lvi);
                return;
            }
            else
            {
                string username = forexquotes3["response"]["username"];
                string balance = forexquotes3["response"]["balance"];
                label17.Text = "Balance: " + balance + " BTC";
                ListViewItem lvi = new ListViewItem(DateTime.Now.ToString("HH:mm:ss tt"));
                lvi.SubItems.Add(" API token for user '" + username + "'" + " added.");
                listView3.Items.Add(lvi);
            }
        }

        private void listView3_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                timer2.Interval = 5000;
                timer2.Start();
            }
         else
            {
                timer2.Stop();
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            string symbolselection = comboBox3.Text;

            string apitoken = textBox7.Text;
            string sURL3 = " https://1broker.com/api/v2/market/quotes.php?symbols=" + symbolselection + "&token=" + apitoken;

            WebRequest wrGETURL3;
            wrGETURL3 = WebRequest.Create(sURL3);
            try
            {
                WebResponse responsetest = wrGETURL3.GetResponse();
            }
            catch (WebException ex3)
            {
                using (WebResponse response3 = ex3.Response)
                {
                    ListViewItem lvi = new ListViewItem(DateTime.Now.ToString("HH:mm:ss tt"));
                    lvi.SubItems.Add("Problem getting HTTPS, retry again or check internet connection.");
                    listView3.Items.Add(lvi);
                    return;
                }


            }
            Stream objStream3;
            objStream3 = wrGETURL3.GetResponse().GetResponseStream();

            StreamReader reader3 = new StreamReader(objStream3);
            string responsestring3 = reader3.ReadToEnd();
            dynamic forexquotes3 = JsonConvert.DeserializeObject(responsestring3);
            if (forexquotes3.error == "true")
            {
                ListViewItem lvi = new ListViewItem(DateTime.Now.ToString("HH:mm:ss tt"));
                lvi.SubItems.Add("Error:" + forexquotes3.error_message);
                listView3.Items.Add(lvi);

                return;
            }
            else
            {
                string bid = forexquotes3["response"][0]["bid"];
                string ask = forexquotes3["response"][0]["ask"];
                label16.Text = "bid: " + bid + ", ask: " + ask;
            }

            // Add code to update balance when auto is checked

            // string apitoken = textBox7.Text;
            string sURL4 = " https://1broker.com/api/v2/user/details.php?token=" + apitoken;


            WebRequest wrGETURL4;
            wrGETURL4 = WebRequest.Create(sURL4);
            try
            {
                WebResponse responsetest = wrGETURL4.GetResponse();
            }
            catch (WebException ex4)
            {
                using (WebResponse response4 = ex4.Response)
                {
                    ListViewItem lvi = new ListViewItem(DateTime.Now.ToString("HH:mm:ss tt"));
                    lvi.SubItems.Add("Problem getting HTTPS, retry again or check internet connection.");
                    listView3.Items.Add(lvi);
                    return;
                }


            }
            Stream objStream4;
            objStream4 = wrGETURL4.GetResponse().GetResponseStream();

            StreamReader reader4 = new StreamReader(objStream4);
            string responsestring4 = reader4.ReadToEnd();
            dynamic forexquotes4 = JsonConvert.DeserializeObject(responsestring4);
            if (forexquotes4.error == "true")
            {
                ListViewItem lvi = new ListViewItem(DateTime.Now.ToString("HH:mm:ss tt"));
                lvi.SubItems.Add("Error:" + forexquotes4.error_message);
                listView3.Items.Add(lvi);
                return;
            }
            else
            {
                // string username = forexquotes3["response"]["username"];
                string balance = forexquotes4["response"]["balance"];
                label17.Text = "Balance: " + balance + " BTC";
                // ListViewItem lvi = new ListViewItem(DateTime.Now.ToString("HH:mm:ss tt"));
                // lvi.SubItems.Add(" API token for user '" + username + "'" + " added.");
                // listView3.Items.Add(lvi);
            }

            //End new code for balance update

        }

        private void button10_Click(object sender, EventArgs e)
        {
            int count = this.listView1.SelectedItems.Count;
            if (count != 0)
            {

                for (int i = 0; i < count; i++)
                {
                    string text = listView1.Items[i].SubItems[10].Text;
                    string apitoken = textBox7.Text;
                    // MessageBox.Show(text);
                    if (text == "True")
                    {
                        String posid = listView1.SelectedItems[i].Text;
                        // I think this is the Close Cancel action, api endpoint changed, so use new
                        // String sURL = "https://1broker.com/api/v1/position/edit.php?position_id=" + posid + "&market_close=false&token=" + apitoken;
                        String sURL = "https://1broker.com/api/v2/position/close_cancel.php?position_id=" + posid + "&token=" + apitoken;

                        WebRequest wrGETURL;
                        wrGETURL = WebRequest.Create(sURL);
                        try
                        {
                            WebResponse responsetest = wrGETURL.GetResponse();
                        }
                        catch (WebException ex)
                        {
                            using (WebResponse response = ex.Response)
                            {
                                ListViewItem lvi4 = new ListViewItem(DateTime.Now.ToString("HH:mm:ss tt"));
                                lvi4.SubItems.Add("Problem getting HTTPS, retry again or check internet connection.");
                                listView3.Items.Add(lvi4);
                                return;
                            }


                        }
                        Stream objStream;
                        objStream = wrGETURL.GetResponse().GetResponseStream();

                        StreamReader reader = new StreamReader(objStream);
                        string responsestring = reader.ReadToEnd();

                        dynamic poscancelresp = JsonConvert.DeserializeObject(responsestring);



                        if (poscancelresp.error == "true")
                        {
                            ListViewItem lvi5 = new ListViewItem(DateTime.Now.ToString("HH:mm:ss tt"));
                            lvi5.SubItems.Add("Error:" + poscancelresp.error_message);
                            listView3.Items.Add(lvi5);
                            button1.PerformClick();
                            return;
                        }
                        else
                        {
                            // button2.PerformClick();
                            //  MessageBox.Show("Item #" + i + ":" + posid);
                            continue;
                        }
                    }
                }
                button1.PerformClick();
                this.button10.Enabled = false;
                ListViewItem lvi = new ListViewItem(DateTime.Now.ToString("HH:mm:ss tt"));
                lvi.SubItems.Add(" Selected position(s) un-closed.");
                listView3.Items.Add(lvi);
            }
            else
            {
                ListViewItem lvi = new ListViewItem(DateTime.Now.ToString("HH:mm:ss tt"));
                lvi.SubItems.Add("No item selected.");
                listView3.Items.Add(lvi);

            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            XmlReader reader;
            try
            {
                reader = XmlReader.Create("settings.xml");
                reader.Close();
            }
            catch
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;

                XmlWriter writer = XmlWriter.Create("settings.xml", settings);
                writer.WriteStartDocument();
                writer.WriteComment("1Broker API Connector");
                writer.WriteStartElement("Token");
                writer.WriteElementString("Value", "Go to Account Settings in 1Broker To Find");
                writer.WriteEndElement();
                writer.WriteEndDocument();

                writer.Flush();
                writer.Close();
               
                return;
            }
           
            reader = XmlReader.Create("settings.xml");
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "Token")
                {
                    while (reader.NodeType != XmlNodeType.EndElement)
                    {
                        reader.Read();
                        if (reader.Name == "Value")
                        {
                            while (reader.NodeType != XmlNodeType.EndElement)
                            {
                                reader.Read();
                                if (reader.NodeType == XmlNodeType.Text)
                                {
                                    textBox7.Text = reader.Value;
                                    string apitoken = textBox7.Text;
                                    // sURL3 = " https://1broker.com/api/v1/account/info.php?token=" + apitoken;
                                    string sURL3 = " https://1broker.com/api/v2/user/details.php?token=" + apitoken;


                                    WebRequest wrGETURL3;
                                    wrGETURL3 = WebRequest.Create(sURL3);
                                    try
                                    {
                                        WebResponse responsetest = wrGETURL3.GetResponse();
                                    }
                                    catch (WebException ex3)
                                    {
                                        using (WebResponse response3 = ex3.Response)
                                        {
                                            ListViewItem lvi = new ListViewItem(DateTime.Now.ToString("HH:mm:ss tt"));
                                            lvi.SubItems.Add("Problem getting HTTPS, retry again or check internet connection.");
                                            listView3.Items.Add(lvi);
                                            return;
                                        }


                                    }
                                    Stream objStream3;
                                    objStream3 = wrGETURL3.GetResponse().GetResponseStream();

                                    StreamReader reader3 = new StreamReader(objStream3);
                                    string responsestring3 = reader3.ReadToEnd();
                                    dynamic forexquotes3 = JsonConvert.DeserializeObject(responsestring3);
                                    if (forexquotes3.error == "true")
                                    {
                                        ListViewItem lvi = new ListViewItem(DateTime.Now.ToString("HH:mm:ss tt"));
                                        lvi.SubItems.Add("Error:" + forexquotes3.error_message);
                                        listView3.Items.Add(lvi);
                                        return;
                                    }
                                    else
                                    {
                                        string username = forexquotes3["response"]["username"];
                                        string balance = forexquotes3["response"]["balance"];
                                        label17.Text = "Balance: " + balance + " BTC";
                                        ListViewItem lvi = new ListViewItem(DateTime.Now.ToString("HH:mm:ss tt"));
                                        lvi.SubItems.Add(" API token loaded for user '" + username + "'" + " from file.");
                                        listView3.Items.Add(lvi);
                                    }
                                }
                            }
                        }
                    }
                }
            }
   
            reader.Close();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string apitoken = textBox7.Text;
            // string sURL3 = " https://1broker.com/api/v1/account/info.php?token=" + apitoken;
            string sURL3 = " https://1broker.com/api/v2/user/details.php?token=" + apitoken;

            WebRequest wrGETURL3;
            wrGETURL3 = WebRequest.Create(sURL3);
            try
            {
                WebResponse responsetest = wrGETURL3.GetResponse();
            }
            catch (WebException ex3)
            {
                using (WebResponse response3 = ex3.Response)
                {
                    ListViewItem lvi = new ListViewItem(DateTime.Now.ToString("HH:mm:ss tt"));
                    lvi.SubItems.Add("Problem getting HTTPS, retry again or check internet connection.");
                    listView3.Items.Add(lvi);
                    return;
                }


            }
            Stream objStream3;
            objStream3 = wrGETURL3.GetResponse().GetResponseStream();

            StreamReader reader3 = new StreamReader(objStream3);
            string responsestring3 = reader3.ReadToEnd();
            dynamic forexquotes3 = JsonConvert.DeserializeObject(responsestring3);
            if (forexquotes3.error == "true")
            {
                ListViewItem lvi = new ListViewItem(DateTime.Now.ToString("HH:mm:ss tt"));
                lvi.SubItems.Add("Error:" + forexquotes3.error_message);
                listView3.Items.Add(lvi);
                return;
            }
            else
            {
                string username = forexquotes3["response"]["username"];
                string balance = forexquotes3["response"]["balance"];
                string unconfirmed = forexquotes3["response"]["deposit_unconfirmed"];
                string email = forexquotes3["response"]["email"];
                string regdate = forexquotes3["response"]["date_created"];
                label17.Text = "Balance: " + balance + " BTC";
                ListViewItem lvi = new ListViewItem(DateTime.Now.ToString("HH:mm:ss tt"));
                lvi.SubItems.Add(" Username: " + username);
                listView3.Items.Add(lvi);
                ListViewItem lvi6 = new ListViewItem(DateTime.Now.ToString("HH:mm:ss tt"));
                lvi6.SubItems.Add(" Email: " + email);
                listView3.Items.Add(lvi6);
                ListViewItem lvi1 = new ListViewItem(DateTime.Now.ToString("HH:mm:ss tt"));
                lvi1.SubItems.Add(" Balance: " + balance);
                listView3.Items.Add(lvi1);
                ListViewItem lvi2 = new ListViewItem(DateTime.Now.ToString("HH:mm:ss tt"));
                lvi2.SubItems.Add(" Pending deposits: " + unconfirmed);
                listView3.Items.Add(lvi2);
                ListViewItem lvi3 = new ListViewItem(DateTime.Now.ToString("HH:mm:ss tt"));
                lvi3.SubItems.Add(" Registered: " + regdate);
                listView3.Items.Add(lvi3);

            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            listView3.Items.Clear();
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("The app is in early development, tooltips and other will come soon. Any issues contact swapman, see ReadMe.txt");
        }

        private void button12_Click(object sender, EventArgs e)
        {
            if (listView3.Items.Count < 10)
            {
                return;
            }
            listView3.Items[listView3.Items.Count - 1].EnsureVisible();
        }

        private void label17_Click(object sender, EventArgs e)
        {

        }

        private void button13_Click(object sender, EventArgs e)
        {
            textBox4.Text = "";
            textBox5.Text = "";
            textBox6.Text = "";
        }

        private void label21_Click(object sender, EventArgs e)
        {
            textBox3.Text = label21.Text;
        }

        private void label22_Click(object sender, EventArgs e)
        {
            textBox3.Text = label22.Text;
        }

        private void label23_Click(object sender, EventArgs e)
        {
            textBox3.Text = label23.Text;
        }

        private void label24_Click(object sender, EventArgs e)
        {
            textBox3.Text = label24.Text;
        }

        private void label25_Click(object sender, EventArgs e)
        {
            textBox3.Text = label25.Text;
        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {


            try
            {
                trackBar1.Value = Int32.Parse(textBox8.Text);
            }
            catch
            {
                return;

            }

            trackBar1.Value = Int32.Parse(textBox8.Text);
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            label11.Text = trackBar1.Value.ToString() + " X";
            textBox8.Text = trackBar1.Value.ToString();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
    }
