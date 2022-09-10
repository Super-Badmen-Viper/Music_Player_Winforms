namespace Music_Player_Test
{
    partial class Image_Fount
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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.button_start = new System.Windows.Forms.Button();
            this.button_Select_Image = new System.Windows.Forms.Button();
            this.listBox_image_url = new System.Windows.Forms.ListBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.pictureBox1.Location = new System.Drawing.Point(0, 2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(413, 413);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // button_start
            // 
            this.button_start.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_start.Location = new System.Drawing.Point(774, 470);
            this.button_start.Name = "button_start";
            this.button_start.Size = new System.Drawing.Size(93, 40);
            this.button_start.TabIndex = 1;
            this.button_start.Text = "开始转换";
            this.button_start.UseVisualStyleBackColor = true;
            this.button_start.Click += new System.EventHandler(this.button_start_Click);
            // 
            // button_Select_Image
            // 
            this.button_Select_Image.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_Select_Image.Location = new System.Drawing.Point(662, 470);
            this.button_Select_Image.Name = "button_Select_Image";
            this.button_Select_Image.Size = new System.Drawing.Size(93, 40);
            this.button_Select_Image.TabIndex = 2;
            this.button_Select_Image.Text = "选择图形文件";
            this.button_Select_Image.UseVisualStyleBackColor = true;
            this.button_Select_Image.Click += new System.EventHandler(this.button_Select_Image_Click);
            // 
            // listBox_image_url
            // 
            this.listBox_image_url.FormattingEnabled = true;
            this.listBox_image_url.ItemHeight = 12;
            this.listBox_image_url.Location = new System.Drawing.Point(443, 2);
            this.listBox_image_url.Name = "listBox_image_url";
            this.listBox_image_url.Size = new System.Drawing.Size(413, 412);
            this.listBox_image_url.TabIndex = 3;
            // 
            // Image_Fount
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(879, 522);
            this.Controls.Add(this.listBox_image_url);
            this.Controls.Add(this.button_Select_Image);
            this.Controls.Add(this.button_start);
            this.Controls.Add(this.pictureBox1);
            this.Name = "Image_Fount";
            this.Text = "Image_Fount";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button button_start;
        private System.Windows.Forms.Button button_Select_Image;
        private System.Windows.Forms.ListBox listBox_image_url;
    }
}