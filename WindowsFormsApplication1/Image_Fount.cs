using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Music_Player_Test
{
    public partial class Image_Fount : Form
    {
        public Image_Fount()
        {
            InitializeComponent();
        }

        public static bool Select_Image_Complete;
        /// <summary>
        /// 选择图形文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Select_Image_Click(object sender, EventArgs e)
        {
            All_Info_Path = new string[9999];
            Image_Info_Path = new string[9999];

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = true;//该值确定是否可以选择多个文件
            dialog.Title = "请选择文件夹";
            dialog.Filter = "标签|*.jpg;*.png;*.gif";

            dialog.ShowDialog();

            All_Info_Path = dialog.FileNames;

            Clear_Null_All_Info_Path();

            Select_Image_Complete = true;
        }
        /// <summary>
        /// 开始转换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_start_Click(object sender, EventArgs e)
        {
            if (Select_Image_Complete == true)
            {
                Create_image();

                MessageBox.Show("转换完成");

                listBox_image_url.Items.Clear();
                pictureBox1.BackgroundImage = null;

            }
            else
            {
                MessageBox.Show("请先选择图形文件");
            }
        }


        public static Image image;
        public static string[] All_Info_Path;
        public static string[] Image_Info_Path;

        /// <summary>
        /// 清理存储数组NULL
        /// </summary>
        public void Clear_Null_All_Info_Path()
        {
            listBox_image_url.Items.Clear();

            for (int i = 0; i < All_Info_Path.Length; i++)
            {
                if (All_Info_Path[i] != null)
                {
                    if (Image_Info_Path[i] == null)
                    {
                        Image_Info_Path[i] = All_Info_Path[i];

                        listBox_image_url.Items.Add(Image_Info_Path[i]);
                    }
                }
            }
        }
        /// <summary>
        /// 创建image对象
        /// </summary>
        public void Create_image()
        {
            for (int i = 0; i < Image_Info_Path.Length; i++)
            {
                if (Image_Info_Path[i] != null)
                {
                    string image_url = Image_Info_Path[i];
                    image_name = image_url.Substring(image_url.LastIndexOf(@"\"));
                    image_name = image_name.Replace(@"\", "");

                    try
                    {
                        image = new Bitmap(Image_Info_Path[i]);
                        Change_GUI_Pic_Image_Size();
                    }
                    catch
                    {
                        MessageBox.Show("转换失败" + image_name);
                    }
                }
            }
        }

        /// <summary>
        /// 将传入的image对象通过背景图片二次生成转换的具有固定比例大小的Temp_Image
        /// </summary>
        /// <param name="image"></param>
        public void Change_GUI_Pic_Image_Size()
        {
            try
            {
                if (image != null)
                {
                    int X_Height = Convert.ToInt32(((Convert.ToDouble(image.Width) - Convert.ToDouble(image.Height)) / 2));//取从0开始到图片中间的宽度值
                    int X_Width = Convert.ToInt32(((Convert.ToDouble(image.Height) - Convert.ToDouble(image.Width)) / 2));//取从0开始到图片中间的高度值

                    if (image.Width >= image.Height)//横屏图片
                    {
                        image = new Bitmap(MakeThumbnailImage(1080, 1080, image.Height, image.Height, X_Height, 0));//X轴 -  取从0开始到图片中间的宽度值
                    }
                    else//竖屏图片
                    {
                        image = new Bitmap(MakeThumbnailImage(1080, 1080, image.Width, image.Width, 0, X_Width));//Y轴 - 取从0开始到图片中间的高度值
                    }

                    //将图片转换为背景图片，使用背景图片的style样式功能将image转换样式，再将转换过样式的image重新创建
                    //不需要再使用背景旋转（背景旋转在切换照片时会显示背景图，旋转角度不同，影响视觉）
                    this.pictureBox1.BackgroundImage = new Bitmap(image);


                }
                else
                {
                    pictureBox1.BackgroundImage = null;
                }
            }
            catch
            {
                MessageBox.Show("转换失败" + image_name);
            }
        }


        public static string image_name;
        //MakeThumbnailImage(image, 1980, 1080, image.Width, image.Height, 0, 0);         
        /// <summary>
        /// 裁剪图片并保存
        /// </summary>
        /// <param name="Image">图片信息</param>
        /// <param name="maxWidth">缩略图宽度</param>
        /// <param name="maxHeight">缩略图高度</param>
        /// <param name="cropWidth">裁剪宽度</param>
        /// <param name="cropHeight">裁剪高度</param>
        /// <param name="X">X轴</param>
        /// <param name="Y">Y轴</param>
        public static Bitmap MakeThumbnailImage(int maxWidth, int maxHeight, int cropWidth, int cropHeight, int X, int Y)
        {
            Bitmap b = new Bitmap(cropWidth, cropHeight);
            Image displayImage = null;
            try
            {
                using (Graphics g = Graphics.FromImage(b))
                {
                    //清空画布并以透明背景色填充
                    g.Clear(Color.Transparent);
                    //在指定位置并且按指定大小绘制原图片的指定部分
                    g.DrawImage(image, new Rectangle(0, 0, cropWidth, cropHeight), X, Y, cropWidth, cropHeight, GraphicsUnit.Pixel);
                    displayImage = new Bitmap(b, maxWidth, maxHeight);

                    string temp = @"D:\墨智音乐 - KRC\图片资源\已处理裁切图\专辑\" + image_name;

                    displayImage.Save(temp, System.Drawing.Imaging.ImageFormat.Jpeg);

                    return new Bitmap(b, maxWidth, maxHeight);
                }
            }
            catch (System.Exception e)
            {
                throw e;
            }
            finally
            {
                image.Dispose();
                b.Dispose();
                displayImage.Dispose();

                ClearMemory();
                SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, -1, -1);
            }
        }


        [DllImport("kernel32.dll", EntryPoint = "SetProcessWorkingSetSize")]//指定系统回收内存的工具：kernel32.dll
        public static extern int SetProcessWorkingSetSize(IntPtr Delete_All_Info, int minSize, int maxSize);
        /// <summary>
        /// 释放内存
        /// </summary>
        public static void ClearMemory()
        {
            GC.Collect();//调用系统的垃圾回收器——处理未使用闲置的服务进程
            GC.WaitForPendingFinalizers();//将当前所占用的服务进程排成队列，当指定的服务被清除后关闭

            if (Environment.OSVersion.Platform == PlatformID.Win32NT)//获取当前的.Net应用程序
            {
                SetProcessWorkingSetSize(System.Diagnostics.Process.GetCurrentProcess().Handle, -1, -1);
            }
        }



    }
}
