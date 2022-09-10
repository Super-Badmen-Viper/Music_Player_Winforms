using System;
using System.Windows.Forms;

namespace Music_Player_Test
{
    public partial class Finds_Song_Info : Form
    {
        public Finds_Song_Info()
        {
            InitializeComponent();
        }

        private void Finds_Song_Info_Load(object sender, EventArgs e)
        {
            Uri uri = new Uri(FrmMain.web_src);

            webBrowser1.Url = uri;

            webBrowser1.Navigate(uri);

            //HtmlElement htmlElement = GetElement_Name(webBrowser1,"");




        }
        public HtmlElement GetElement_Name(WebBrowser wb, string Name)
        {
            HtmlElement e = wb.Document.All[Name];
            return e;
        }

        public void Connect_Photo(string web_src)
        {

            using (System.Net.WebClient wc = new System.Net.WebClient())
            {
                wc.Headers.Add("User-Agent", "Chrome");
                wc.DownloadFile(web_src, @"D:\墨智_毒蛇讯息（简单音乐播放器）1.0.1\singer_songPhoto\mobile.gif");//保存到本地的文件名和路径，请自行更改
            }
        }

        /**
         * 10、屏蔽脚本错误：

            将WebBrowser控件ScriptErrorsSuppressed设置为True即可
        */
    }
}
