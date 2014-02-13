using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlAgilityPack;

namespace DataSources.Connect
{
    public class ConnectDataProvider : IDataPull<ConnectSubmission>
    {
        // https://msconnect.microsoft.com/ViewHistory/GetConnectHistory/%7B0%7D
        public IEnumerable<ConnectSubmission> PullData(DateTime startUtcTime, DateTime endUtcTime)
        {
            // because cloud property requires Federated auth -- no time to figure that out
            var connectHistoryForm = new ConnectHistoryForm();
            connectHistoryForm.ShowDialog();

            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(connectHistoryForm.Html);
            
            return doc.DocumentNode
                      .SelectNodes("//a")
                      .Select(p => p.InnerText)
                      .Zip(doc.DocumentNode.SelectNodes("//b").Select(p => p.InnerText), (titleText, dateText) => 
                      {
                          return new ConnectSubmission
                          {
                              Title = titleText,
                              SubmittedUtcDate = DateTime.Parse(dateText).ToUniversalTime()
                          };
                      })
                      .Where(p => p.SubmittedUtcDate >= startUtcTime && p.SubmittedUtcDate <= endUtcTime)
                      .ToList();
        }
        
        // Adapted from Codebox/Yammer.NET
        private class ConnectHistoryForm : Form
        {
            private static readonly string s_connectHistoryUrlString = "https://msconnect.microsoft.com/ViewHistory/GetConnectHistory/%7B0%7D";

            private WebBrowser webBrowser;

            public ConnectHistoryForm()
            {
                Html = String.Empty;
                this.InitializeComponent();
            }

            public string Html { get; set; }

            private void LoginForm_Load(object sender, EventArgs e)
            {
                this.webBrowser.Navigate(s_connectHistoryUrlString);
            }
            
            private void webBrowser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
            {
                string str1 = e.Url.ToString();
                Uri uri = new Uri(s_connectHistoryUrlString);
                if (!(e.Url.Scheme == uri.Scheme) || !(e.Url.Host == uri.Host) || (e.Url.Port != uri.Port || !(e.Url.LocalPath == uri.LocalPath)))
                    return;
                
                Html = webBrowser.Document.Body.InnerHtml;
                this.Close();
            }

            private void InitializeComponent()
            {
                ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ConnectHistoryForm));
                this.webBrowser = new WebBrowser();
                this.SuspendLayout();
                this.webBrowser.Dock = DockStyle.Fill;
                this.webBrowser.Location = new Point(0, 0);
                this.webBrowser.MinimumSize = new Size(20, 20);
                this.webBrowser.Name = "webBrowser";
                this.webBrowser.Size = new Size(829, 566);
                this.webBrowser.TabIndex = 0;
                this.webBrowser.Navigated += new WebBrowserNavigatedEventHandler(this.webBrowser_Navigated);
                this.AutoScaleDimensions = new SizeF(6f, 13f);
                this.AutoScaleMode = AutoScaleMode.Font;
                this.ClientSize = new Size(829, 566);
                this.Controls.Add((Control)this.webBrowser);
                this.Name = "LoginForm";
                this.Text = "Connect to MS Connect";
                this.Load += new EventHandler(this.LoginForm_Load);
                this.ResumeLayout(false);
            }
        }
    }
}
