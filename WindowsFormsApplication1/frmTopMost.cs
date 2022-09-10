using System;
using System.Windows.Forms;

namespace Music_Player_Test
{
    public partial class frmTopMost : Form
    {
        public frmTopMost()
        {
            InitializeComponent();
        }

        private frmTopMost fm = null;

        public frmTopMost GetFrmMain()
        {
            if (fm == null)
            {
                fm = new frmTopMost();
            }
            return fm;
        }
        private void frmTopMost_FormClosing(object sender, FormClosingEventArgs e)
        {
            //关闭窗体时,释放自己
            fm = null;
        }


        /// <summary>
        /// 悬浮窗口的构造函数
        /// </summary>
        /// <param name="main"></param>
        public frmTopMost(FrmMain main)
        {
            InitializeComponent();
            pParent = main;
        }
        private FrmMain pParent;


        /// <summary>
        /// 悬浮窗口的Load事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmTopMost_Load(object sender, EventArgs e)
        {
            double Windows_Width;
            double Windows_Heigh;

            Windows_Width = Convert.ToDouble(System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width) / 1780;
            Windows_Heigh = Convert.ToDouble(System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height) / 1080;

            int width = Convert.ToInt16(Windows_Width * 200);
            int height = Convert.ToInt16(Windows_Heigh * 850);

            this.Show();
            this.Location = new System.Drawing.Point(width, height);

            this.Width = 1600;
            this.Height = 200;

            this.BackColor = System.Drawing.Color.Green;//将窗体背景设置成红色
            this.TransparencyKey = System.Drawing.Color.Green; //将红色设置成透明色
            //将from的FormBorderStyle设置为None

            lblS6_top.Font = FrmMain.lbl_1;
            lblS7.Font = FrmMain.lbl_2;

            lblS6_top.ForeColor = FrmMain.lbl_1_Color;
            lblS7.ForeColor = FrmMain.lbl_2_Color;
        }
        private void frmTopMost_MouseLeave(object sender, MouseEventArgs e)
        {
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
        }
        private void frmTopMost_MouseUp(object sender, MouseEventArgs e)
        {

        }


        private void lblS6_top_Click(object sender, EventArgs e)
        {

        }

        private void lblS7_Click(object sender, EventArgs e)
        {

        }


    }
}
