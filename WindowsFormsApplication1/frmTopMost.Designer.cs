
namespace Music_Player_Test
{
    partial class frmTopMost
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblS6_top = new System.Windows.Forms.Label();
            this.lblS7 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblS6_top
            // 
            this.lblS6_top.BackColor = System.Drawing.Color.Transparent;
            this.lblS6_top.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblS6_top.Font = new System.Drawing.Font("宋体", 26.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblS6_top.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.lblS6_top.Location = new System.Drawing.Point(267, 45);
            this.lblS6_top.Name = "lblS6_top";
            this.lblS6_top.Size = new System.Drawing.Size(1333, 39);
            this.lblS6_top.TabIndex = 27;
            this.lblS6_top.Text = "墨智音乐";
            this.lblS6_top.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblS6_top.Click += new System.EventHandler(this.lblS6_top_Click);
            // 
            // lblS7
            // 
            this.lblS7.BackColor = System.Drawing.Color.Transparent;
            this.lblS7.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblS7.Font = new System.Drawing.Font("宋体", 26.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblS7.ForeColor = System.Drawing.Color.Gray;
            this.lblS7.Location = new System.Drawing.Point(846, 101);
            this.lblS7.Name = "lblS7";
            this.lblS7.Size = new System.Drawing.Size(900, 39);
            this.lblS7.TabIndex = 26;
            this.lblS7.Text = "毒蛇云生态，致力于生活更美好";
            this.lblS7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblS7.Click += new System.EventHandler(this.lblS7_Click);
            // 
            // frmTopMost
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.BackColor = System.Drawing.Color.Gainsboro;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1492, 160);
            this.ControlBox = false;
            this.Controls.Add(this.lblS6_top);
            this.Controls.Add(this.lblS7);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Location = new System.Drawing.Point(0, 600);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmTopMost";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmTopMost";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.frmTopMost_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.frmTopMost_MouseUp);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Label lblS6_top;
        public System.Windows.Forms.Label lblS7;

    }
}