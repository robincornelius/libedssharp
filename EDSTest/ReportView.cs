using System;
using System.Windows.Forms;
using System.IO;
using mshtml;

namespace ODEditor
{
    public partial class ReportView : Form
    {
        public ReportView(string pathtohtml)
        {
            InitializeComponent();

           

            webBrowser1.Url = new Uri(pathtohtml);

            webBrowser1.Navigated += WebBrowser1_Navigated;

          
        }

        private void WebBrowser1_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            string text = System.IO.File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "style.css"));
            mshtml.HTMLDocument CurrentDocument = (HTMLDocument)webBrowser1.Document.DomDocument;
            mshtml.IHTMLStyleSheet styleSheet = CurrentDocument.createStyleSheet("", 0);
            styleSheet.cssText = text;
        }
    }
}
