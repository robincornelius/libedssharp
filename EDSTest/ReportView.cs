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

        private void toolStripButton_print_Click(object sender, EventArgs e)
        {
            webBrowser1.ShowPrintDialog();
        }

        private void toolStripButton_preview_Click(object sender, EventArgs e)
        {
            webBrowser1.ShowPrintPreviewDialog();
        }

        private void toolStripButton_save_Click(object sender, EventArgs e)
        {
            webBrowser1.ShowSaveAsDialog();
        }
    }
}
