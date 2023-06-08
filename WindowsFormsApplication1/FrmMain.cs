using Shell32;
using System;
using System.Collections;
using System.Diagnostics;//获取当前程序相关的信息
using System.Drawing;
using System.Drawing.Drawing2D;//引用Drawing2D 进行图形自定义绘制
using System.Drawing.Text;//安装字体
using System.IO;
using System.Net;
using System.Runtime.InteropServices;//引用系统自带的内存回收机制
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

namespace Music_Player_Test
{
    public partial class FrmMain : Form
    {

        public static double Windows_Width;
        public static double Windows_Heigh;

        //1780,1080
        //1349, 800
        public static double FrmMain_Width;
        public static double FrmMain_Height;

        public static double FrmMain_Size;

        //BitmapRegion bitmapregion;

        Image_Fount images = new Image_Fount();
        public void Button_Image_Change(object sender,EventArgs e)
        {
            images.Show();
        }

        #region 初始化构造

        public FrmMain()
        {
            InitializeComponent();

            //开启双缓冲减少屏幕闪屏
            //  TODO:  在  InitComponent  调用后添加任何初始化 
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            //开启双缓冲
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
        }

        public static FrmMain fm = null;
        public static int Select_DataGridView_Name_nums;//表示当前所选择的歌曲列表 //全局变量// == 1  代表本地音乐 // == 2  代表我的收藏 // == 3  代表默认列表       
        public static FrmMain GetFrmMain()
        {
            if (fm == null)
            {
                fm = new FrmMain();
            }
            return fm;
        }

        /// <summary>
        /// 防止timer无限循环时导致CPU占用高
        /// </summary>
        public void Thread_Sleep()
        {
            if ((int)axWindowsMediaPlayer1.playState != 3)
                Thread.Sleep(50);

            if (Open_Singer_Image == 0)
            {
                Thread.Sleep(50);
            }

        }


        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmMain_Load(object sender, EventArgs e)
        {
            //定义存储缓冲区大小
            StringBuilder s = new StringBuilder(300);
            //获取Window 桌面背景图片地址，使用缓冲区
            SystemParametersInfo(SPI_GETDESKWALLPAPER, 300, s, 0);
            //缓冲区中字符进行转换
            wallpaper_path = s.ToString(); //系统桌面背景图片路径



            FrmMain.CheckForIllegalCrossThreadCalls = false;//取消对线程安全性的监控（不检测使用的线程是否是该控件的线程）


            FrmMain_Size = 1;

            Windows_Width = Convert.ToDouble(System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width) / 1780;
            Windows_Heigh = Convert.ToDouble(System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height) / 1080;



            if (Windows_Width != 1)
            {
                FrmMain_Size = Windows_Width;

                //this.Width = Convert.ToInt32(1350  * 0.9);
                //this.Height = Convert.ToInt32(800 / Windows_Heigh);

                Change_All_Button_Size();
            }

            //this.listBox1.Hide();
            this.comboBox1.Hide();


            ///**调用方法：
            ////初始化调用不规则窗体生成代码* */

            //image_Turntable = new Bitmap(Properties.Resources.player, 296, 486);
            //bitmapregion = new BitmapRegion();//此为生成不规则窗体或者控件的类
            ////BitmapRegion.CreateControlRegion(pictureBox2, image_Turntable, 0, 0);//这里this可替换成其他控件。   , button2.Width, button2.Height

            //double nums = Convert.ToDouble(pictureBox_Panel.Height * 486 / 500) / 486;  //   486:500
            //Bitmap bitmap = new Bitmap(Properties.Resources.player, Convert.ToInt32(296 * nums * Windows_Width), Convert.ToInt32(486 * nums * Windows_Heigh));
            //BitmapRegion BitmapRegion = new BitmapRegion();//此为生成不规则窗体或者控件的类
            //BitmapRegion.CreateControlRegion(pictureBox2, bitmap, 0, 0);//这里this可替换成其他控件。   , button2.Width, button2.Height



            // 阻止系统睡眠，阻止屏幕关闭。
            SystemSleep.PreventForCurrentThread();

            try
            {
                Create_songInfo();//创建文件夹
            }
            catch (Exception erroes)
            {
                MessageBox.Show("" + erroes);
            }
            finally
            {
                backnum = 0;

                this.axWindowsMediaPlayer2.Hide();


                SongIds_New_Update_All();//更新歌曲id

                Load_DataGridView();//读取歌曲文件信息
                Load_Set_Info();//读取歌词配置信息

                try
                {
                    fwsPrevious = this.WindowState;
                    myTopMost = new frmTopMost(this);
                }
                catch
                {

                }

                if (Open_Singer_Image == 0)
                {
                    this.panel_Song_LRC.Location = new System.Drawing.Point(Convert.ToInt32(366 * Windows_Width), Convert.ToInt32(53 * Windows_Heigh));
                    this.panel_Song_LRC.Size = new System.Drawing.Size(Convert.ToInt32(602 * Windows_Width), Convert.ToInt32(622 * Windows_Heigh));
                }
                else
                {
                    this.panel_Song_LRC.Location = new System.Drawing.Point(Convert.ToInt32(520 * Windows_Width), Convert.ToInt32(53 * Windows_Heigh));
                    this.panel_Song_LRC.Size = new System.Drawing.Size(Convert.ToInt32(602 * Windows_Width), Convert.ToInt32(622 * Windows_Heigh));

                    panel_PictureBox_All_Location_Normal();
                }
            }
        }

        #endregion

        #region  修改控件的比例

        /// <summary>
        /// 修改panel_Button的大小比例
        /// </summary>
        public void Change_All_Button_Size()
        {
            //this.ListSong_AllSong.Width = Convert.ToInt32(this.ListSong_AllSong.Width * FrmMain_Size);
            //this.ListSong_love.Width = Convert.ToInt32(this.ListSong_love.Width * FrmMain_Size);
            //this.ListSong_Auto.Width = Convert.ToInt32(this.ListSong_Auto.Width * FrmMain_Size);
            //this.Song_Add_List.Width = Convert.ToInt32(this.Song_Add_List.Width * FrmMain_Size);
            //this.Song_Find_ALL_List.Width = Convert.ToInt32(this.Song_Find_ALL_List.Width * FrmMain_Size);
            //this.Song_Delete_List.Width = Convert.ToInt32(this.Song_Delete_List.Width * FrmMain_Size);
            //this.Set_Song_Lrc_Windows.Width = Convert.ToInt32(this.Set_Song_Lrc_Windows.Width * FrmMain_Size);
            //this.Windows_Song_Lrc.Width = Convert.ToInt32(this.Windows_Song_Lrc.Width * FrmMain_Size);
            //this.Music_MV.Width = Convert.ToInt32(this.Music_MV.Width * FrmMain_Size);

            //this.Song_Add_List.Width = Convert.ToInt32(this.Song_Add_List.Width * FrmMain_Size);
            //this.Song_Find_ALL_List.Width = Convert.ToInt32(this.Song_Find_ALL_List.Width * FrmMain_Size);
            //this.Song_Delete_List.Width = Convert.ToInt32(this.Song_Delete_List.Width * FrmMain_Size);
        }

        #endregion


        #region 解决窗口切换时闪屏效果 会让图片旋转效果消失
        /// <summary>
        /// 解决窗口切换时闪屏效果
        /// </summary>
        //protected override CreateParams CreateParams
        //{
        //    get
        //    {
        //        CreateParams paras = base.CreateParams;
        //        paras.ExStyle |= 0x02000001;
        //        return paras;
        //    }
        //}
        #endregion
        #region 关于内存回收的方法
        /*关于Winform如何降低系统内存占用的资料：
            1、使用性能测试工具dotTrace 3.0，它能够计算出你程序中那些代码占用内存较多
            2、强制垃圾回收
            3、多dispose释放指定的对象的所有占用资源，close同上差不多
            4、用timer，每几秒钟调用：SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, -1, -1);具体见附录。
            5、发布的时候选择Release
            6、注意代码编写时少产生垃圾，比如String + String就会产生大量的垃圾，可以用StringBuffer.Append
            7、this.Dispose();    this.Dispose(True);   this.Close();    GC.Collect();   
            8、注意变量的作用域，具体说某个变量如果只是临时使用就不要定义成成员变量。GC是根据关系网去回收资源的。
            9、检测是否存在内存泄漏的情况，详情可参见：内存泄漏百度百科 ：https://baike.baidu.com/item/%E5%86%85%E5%AD%98%E6%B3%84%E6%BC%8F/6181425?fr=aladdin
         * 
         *  .................................
        */
        #endregion
        //定时器
        #region 内存回收——调用系统工具指定释放无用服务进程的内存，防止一些对象内存分配不足

        //托管资源 ：由CLR管理分配和释放的资源，也就是我们直接new出来的对象；
        //      非托管资源：不受CLR控制的资源，也就是不属于.NET本身的功能，往往是通过调用跨平台程序集(如C++)或者操作系统提供的一些接口，
        //          比如Windows内核对象、文件操作、数据库连接、socket、Win32API、网络等。
        //GC垃圾回收主要是帮我们回收 托管资源
        //      对于非托管资源的回收，需要开发人员自己写代码实现回收。

        //托管堆
        /*.Net CLR把所有的引用对象都分配到托管堆上，那么垃圾回收器是怎么知道一个对象不再使用该回收了呢？
            当一个进程初始化之后，运行时会保留一段连续的空白内存空间，这块内存空间就是托管堆，我们暂且理解为从低地址到高地址的连续内存空间。托管堆会记录一个指针，我们叫它NextObjPtr，这个指针指向下一个对象的分配地址，最初的时候，这个指针指向托管堆的起始位置。
            应用程序使用new操作符创建一个新对象，这个操作符首先要确认托管堆剩余空间能放得下这个对象，如果能放得下，就把NextObjPtr指针指向这个对象，然后调用对象的构造函数，new操作符返回对象的地址。NextObjPtr随之上移。
            当应用程序调用new操作符创建对象时，有可能已经没有内存来存放这个对象了。托管堆可以检测到NextObjPtr指向的空间是否超过了堆的大小，如果超过了就说明托管堆满了，就需要做一次垃圾回收了。
            在现实中，在0代堆满了之后就会触发一次垃圾回收。*/
        
        //代：
        /*“代”：在.net CLR中目前是分为三代，可以通过调用GC.MaxGeneration得知
            0代：内存最大长度通常为256KB，发生垃圾回收后幸存的对象升级为1代
                1代：内存最大长度通常为2 MB，发生垃圾回收后幸存的对象升级为2代
                2代：内存长度比较大，发生垃圾回收后幸存的对象依然为2代
            0代堆满了触发垃圾回收，一般0代回收后内存依然不够分配给新的对象使用，会继续回收1代、0代；还不够则回收 2代、1代、0代；还不够，那么 new操作符就会抛出OutofMemoryException
        */





        //using System.Runtime.InteropServices;//引用系统自带的内存回收机制

        //调用入口设置为SetProcessWorkingSetSize
        //      EntryPoint属性与用法： https://docs.microsoft.com/zh-cn/dotnet/api/system.runtime.interopservices.dllimportattribute.entrypoint?redirectedfrom=MSDN&view=net-5.0 此链接：(Dos/.NET/.NET API文档)
        [DllImport("kernel32.dll", EntryPoint = "SetProcessWorkingSetSize")]//指定系统回收内存的工具：kernel32.dll

        //设置SetProcessWorkingSetSize方法的参数属性值 ,获取用来释放内存的程序，程序所处的序号
        //方法被声明为 static。这是 P/Invoke 方法所要求的，因为在该 Windows API 中没有一致的实例概念。
        //该方法被标记为 extern。这是提示编译器该方法是通过一个从 DLL 导出的函数实现的，因此不需要提供方法体。
        public static extern int SetProcessWorkingSetSize(IntPtr Delete_All_Info, int minSize, int maxSize);
        
        /// <summary>
        /// 释放内存
        /// </summary>
        public static void ClearMemory()
        {
            GC.Collect();//调用系统的垃圾回收器——处理未使用闲置的服务进程
            //强制对所有  代   进行即时垃圾回收

            GC.WaitForPendingFinalizers();//将当前所占用的服务进程排成队列，当指定的服务(GC垃圾算法判定为多余)被清除后关闭
            //挂起当前线程，直到处理终结器队列的线程清空该队列为止

            //判断当前操作系统平台  是否为   Windows NT或更新版本
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)//获取当前的.Net应用程序
            {   
                //指定要Over的应用程序的指定服务进程
                SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, -1, -1);
                //System.Diagnostics.Process.GetCurrentProcess().Handle, -1, -1
                //将指定的服务进程索引设为-1，置顶，再调用垃圾回收器清除指定的进程内存冗余，再为此进程创建新的服务对接指定清理过的服务
                //将指定的应用程序所占用的闲置进程关闭，释放闲置服务所占用的内存

                //此方法将参数值  传入   kernel32.dll(.NET  指定系统回收内存的工具)

                // kernel32.dll(.NET  指定系统回收内存的工具)
                //kernel32.dll内置垃圾回收算法
                    /*每个应用程序都有一组根对象，根是一些存储位置，他们可能指向托管堆上的某个地址，也可能是null
                        .NET中可以当作GC Root的对象有如下几种：
                        1、全局变量
                        2、静态变量
                        3、线程栈上的所有局部变量(JIT)
                        4、线程栈上传入的参数变量
                        5、CPU寄存器中的变量
                        注意，只有引用类型的变量才被认为是根，值类型的变量永远不被认为是根。因为值类型存储在栈中，而引用类型存储在托管堆上。*/
                    /*1.算法第一步：标记
                        当垃圾回收器开始运行，它会假设托管堆上的所有对象都是垃圾。
                        从根对象出发 开始构建一个由所有和根对象之间有引用关系对象构成的图.
                        如果垃圾回收器发现一个对象已经在图中就会换一个路径继续遍历。这样做有两个目的：一是提高性能，二是避免无限循环。

                        另外，实现了Finalize方法的对象，垃圾回收器第一次执行时，会被提升到更老的“代”，这会增加内存压力，使对象和此对象的关联对象不能在成为垃圾的第一时间回收掉。
                        具体执行过程：

                        先搞清楚两个队列
                        1）Finalization 队列：终结队列
                            当应用程序创建一个新对象时，new操作符在堆上分配内存，如果对象实现了Finalize方法，对象的指针会放到终结队列中，暂且称为这种对象为 Finalize对象吧。终结队列是由垃圾回收器控制的内部数据结构。在队列中每一个对象在回收时都需要调用它们的Finalize方法。
                        2）Freachable队列：需要被执行的Finalize对象(指针)队列
                            当Finalize对象需要被回收时，垃圾回收器扫描终结队列找到这些对象的指针，当发现对象指针时，指针会被移动到 Freachable队列。
                            Freachable队列是另一个由垃圾回收器控制的内部数据结构。
                            程序运行时会有一个专门的线程负责调用Freachable队列中对象的Finalize方法。当Freachable队列为空时，这个线程会休 眠，当队列中有对象时，线程被唤醒，移除队列中的对象，并调用它们的Finalize方法。因此，如果一个对象在freachable队列中，那么这个对象就不是垃圾，且在执行Finalize方法时不要企图访问线程的 local storage。
                            再次触发垃圾回收之后，实现Finalize方法的对象才被真正的回收。这些对象的Finalize方法已经执行过了，Freachable队列清空了。

                    2.算法第二步：清除
                        所有的根对象都检查完之后，垃圾回收器的图中就有了应用程序中所有的可达对象。
                        托管堆上所有不在这个图上的对象就是要做回收的垃圾对象了。
                        挂起所有访问托管资源的线程：采用 安全点挂起或劫持的方式 。
                        线性的遍历托管堆，将非垃圾对象向下移动到一起，覆盖所有的内存碎片，同时修改应用程序的根对象使他们指向对象的新内存地址，如果某个对象包含另一个对象的指针，垃圾回收器也要负责修改引用。

                        如你看到的，垃圾回收会有显著的性能损失，这是使用托管堆的一个明显的缺点。 不过，要记着内存回收操作是在托管堆满了之后才会执行。在满之前托管堆的性能比c - runtime堆的性能好要好。并且，运行时垃圾回收器还会做一些性能优化。*/



            }
        }

        private void ClearMemory_Tick(object sender, EventArgs e)//每隔0.777毫秒调用内存回收
        {
            ClearMemory();
            //SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, -1, -1);
            Thread_Sleep();
        }

        #endregion


        #region 歌手图片——引用Drawing2D  指定pictureBox控件转为圆   已弃用
        //using System.Drawing.Drawing2D;//引用Drawing2D 进行图形自定义绘制

        //public void picCircle()//图片框panel_PictureBox_All以圆形展示，需要引用System.Drawing.Drawing2D空间
        //{
        //    GraphicsPath gp = new GraphicsPath();
        //    //GraphicsPath对象：创建表示一系列相互连接的直线和曲线 ——绘制图形的路径

        //    //GraphicsPath对象初始为空路径，需要通过ADD()方法添加数据
        //    gp.AddEllipse(panel_PictureBox_All.ClientRectangle);//向当前路径添加一个椭圆,椭圆的数据来源为panel_PictureBox_All通过.ClientRectangle:获取自身窗口的矩形形状,
        //    //AddEllipse()方法：向当前路径添加一个椭圆
        //    //.ClientRectangle:获取控件工作区的矩形

        //    Region region = new Region(gp);//将得到的路径转化为以椭圆形显示的工作区域
        //    //Region类：指示由矩形和路径构成的图形形状的内部
        //    //new Region(gp);  参数gp为创建Region对象所需路径

        //    panel_PictureBox_All.Region = region;
        //    //.Region = region;——将panel_PictureBox_All的窗口区域改为设置好的椭圆区域

        //    //.Region属性：获取或设置与控件相关联的窗口区域

        //    gp.Dispose();//释放该对象所占用的所有资源
        //    region.Dispose();//释放该对象所占用的所有资源

        #endregion


        #region 开启图片旋转 控制是否显示图片旋转

        public static int Open_Singer_Image = 1;//控制是否显示图片旋转      
        private void Button_Open_Image_Click(object sender, EventArgs e)
        {
            if (Button_Open_Image.Text.Equals("开启图片旋转（性能->Low）"))
            {
                if (image != null)
                {
                    Change_GUI_Pic_Image_Size();//创建专辑图片对象
                }

                panel_PictureBox_All.Show();
                panel_PictureBox_All.Show();

                Open_Singer_Image = 1;
                Button_Open_Image.Text = "关闭图片旋转（性能->UP）";


                //直接播放，专辑自动生成，减少很多麻烦
                axWindowsMediaPlayer1.Ctlcontrols.play();


                Photo_BackGround_Time.Start();


                Open__PictureBox_Image = 1;

                if (this.WindowState == System.Windows.Forms.FormWindowState.Maximized)
                {
                    this.panel_Song_LRC.Location = new System.Drawing.Point(Convert.ToInt32(988 * Windows_Width), Convert.ToInt32(212 * Windows_Heigh));
                    this.panel_Song_LRC.Size = new System.Drawing.Size(Convert.ToInt32(602 * Windows_Width), Convert.ToInt32(622 * Windows_Heigh));

                    panel_PictureBox_All_Location_Max();

                    //double nums = Convert.ToDouble(pictureBox_Panel.Height * 486 / 500 ) / 486;  //   486:500
                    //Bitmap bitmap = new Bitmap(Properties.Resources.player, Convert.ToInt32(296 * nums), Convert.ToInt32(486 * nums));
                    //BitmapRegion BitmapRegion = new BitmapRegion();//此为生成不规则窗体或者控件的类
                    //BitmapRegion.CreateControlRegion(pictureBox2, bitmap, 0, 0);//这里this可替换成其他控件。   , button2.Width, button2.Height
                }
                else if (this.WindowState == System.Windows.Forms.FormWindowState.Normal)
                {
                    this.panel_Song_LRC.Location = new System.Drawing.Point(Convert.ToInt32(620 * Windows_Width), Convert.ToInt32(53 * Windows_Heigh));
                    this.panel_Song_LRC.Size = new System.Drawing.Size(Convert.ToInt32(602 * Windows_Width), Convert.ToInt32(622 * Windows_Heigh));

                    panel_PictureBox_All_Location_Normal();


                    //double nums = Convert.ToDouble(pictureBox_Panel.Height * 486 / 500) / 486;  //   486:500
                    //Bitmap bitmap = new Bitmap(Properties.Resources.player, Convert.ToInt32(296 * nums * Windows_Width), Convert.ToInt32(486 * nums * Windows_Heigh));
                    //BitmapRegion BitmapRegion = new BitmapRegion();//此为生成不规则窗体或者控件的类
                    //BitmapRegion.CreateControlRegion(pictureBox2, bitmap, 0, 0);//这里this可替换成其他控件。   , button2.Width, button2.Height
                }



            }
            else
            {
                panel_PictureBox_All.Hide();
                panel_PictureBox_All.Hide();

                Open_Singer_Image = 0;
                Button_Open_Image.Text = "开启图片旋转（性能->Low）";

                Photo_BackGround_Time.Stop();

                Open__PictureBox_Image = 0;

                if (this.WindowState == System.Windows.Forms.FormWindowState.Maximized)
                {
                    this.panel_Song_LRC.Location = new System.Drawing.Point(Convert.ToInt32(655 * Windows_Width), Convert.ToInt32(212 * Windows_Heigh));
                }
                else if (this.WindowState == System.Windows.Forms.FormWindowState.Normal)
                {
                    this.panel_Song_LRC.Location = new System.Drawing.Point(Convert.ToInt32(366 * Windows_Width), Convert.ToInt32(53 * Windows_Heigh));
                }
            }
        }
        #endregion


        #region 自定义重写控件

        public class CtrlRoundPictureBox : PictureBox
        {
            protected override void OnCreateControl()
            {
                GraphicsPath gp = new GraphicsPath();
                gp.AddEllipse(this.ClientRectangle);
                Region region = new Region(gp);
                this.Region = region;
                gp = null;
                region = null;
                base.OnCreateControl();
            }
        }
        public class Font_Half_White_OtherColor_Label : Label
        {
            protected override void OnPaint(PaintEventArgs e)
            {
                //using (LinearGradientBrush brush = new LinearGradientBrush(e.CellBounds, Color.LightGray,
                //Color.White, LinearGradientMode.Vertical))
                //{
                //    e.Graphics.FillRectangle(brush, e.CellBounds);
                //    Rectangle border = e.CellBounds;
                //    border.Offset(new Point(-1, -1));
                //    e.Graphics.DrawRectangle(Pens.Gray, border);
                //}
                //e.PaintContent(e.CellBounds);
                //e.Handled = true;

                base.OnPaint(e);
            }
        }

        public class Button_Style_1 : Button
        {
            private enum MouseAction
            {
                Leave,
                Over,
                Click
            }
            /// <summary>
            /// 颜色渐变方式
            /// </summary>
            private enum GradualMethod
            {
                UpToDown,
                LeftToRight,
                LeftUpToRightDown,
                RightUpToLeftDown
            }

            Color FirstColor;
            Color SecondColor;
            /// <summary>
            /// 第一渐变颜色
            /// </summary>
            public Color FirstGradualColor
            {
                get
                {
                    return FirstColor;
                }
                set
                {
                    FirstColor = value;
                }
            }
            /// <summary>
            /// 第二渐变颜色
            /// </summary>
            public Color SecondGradualColor
            {
                get
                {
                    return SecondColor;
                }
                set
                {
                    SecondColor = value;
                }
            }

            MouseAction MAction;
            GradualMethod GradualM;

            protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
            {
                int r = 10;
                int BtnOffSet = 0;
                Color FColor = Color.FromArgb(245, 245, 245);
                Color SColor = Color.FromArgb(180, 175, 190);
                Color TempFColor = this.FirstColor;
                Color TempSColor = this.SecondColor;
                int offsetwidth = this.Width / 50;
                switch (MAction)
                {
                    case MouseAction.Click:
                        BtnOffSet = 2;
                        break;
                    case MouseAction.Leave:
                        BtnOffSet = 0;
                        TempFColor = FirstColor;
                        TempSColor = SecondColor;
                        break;
                    case MouseAction.Over:
                        TempFColor = FColor;
                        TempSColor = SColor;
                        break;
                }
                Rectangle rc = new Rectangle(BtnOffSet, BtnOffSet, this.ClientSize.Width - 1, this.ClientSize.Height - 1);
                int x = rc.X, y = rc.Y, w = rc.Width, h = rc.Height;
                GraphicsPath path = new GraphicsPath();
                path.AddArc(x, y, r, r, 180, 90);
                path.AddArc(x + w - r, y, r, r, 270, 90);
                path.AddArc(x + w - r, y + h - r, r, r, 0, 90);
                path.AddArc(x, y + h - r, r, r, 90, 90);
                path.CloseFigure();
                this.Region = new Region(path);
                LinearGradientBrush b = null;
                switch (GradualM)
                {
                    case GradualMethod.UpToDown:
                        b = new LinearGradientBrush(rc, TempFColor, TempSColor, LinearGradientMode.Vertical);
                        break;
                    case GradualMethod.RightUpToLeftDown:
                        b = new LinearGradientBrush(rc, TempFColor, TempSColor, LinearGradientMode.BackwardDiagonal);
                        break;
                    case GradualMethod.LeftUpToRightDown:
                        b = new LinearGradientBrush(rc, TempFColor, TempSColor, LinearGradientMode.ForwardDiagonal);
                        break;
                    case GradualMethod.LeftToRight:
                        b = new LinearGradientBrush(rc, TempFColor, TempSColor, LinearGradientMode.Horizontal);
                        break;
                }
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.FillPath(b, path);
                e.Graphics.DrawPath(new Pen(Color.Gray, 3), path);
                StringFormat drawFormat = new StringFormat();
                drawFormat.FormatFlags = StringFormatFlags.DisplayFormatControl;
                drawFormat.LineAlignment = StringAlignment.Center;
                drawFormat.Alignment = System.Drawing.StringAlignment.Center;
                e.Graphics.DrawString(this.Text, this.Font, new LinearGradientBrush(this.ClientRectangle, Color.Black, Color.Black, LinearGradientMode.Vertical), rc, drawFormat);
                b.Dispose();
            }
            protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs mevent)
            {
                MAction = MouseAction.Click;
                this.Invalidate(false);
                base.OnMouseDown(mevent);
            }
            protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs mevent)
            {
                MAction = MouseAction.Over;
                this.Invalidate(false);
                base.OnMouseUp(mevent);
            }
            protected override void OnMouseEnter(EventArgs e)
            {
                MAction = MouseAction.Over;
                this.Invalidate(false);
                base.OnMouseEnter(e);
            }
            protected override void OnNotifyMessage(System.Windows.Forms.Message m)
            {
                base.OnNotifyMessage(m);
            }
            protected override void OnMouseLeave(EventArgs e)
            {
                MAction = MouseAction.Leave;
                this.Invalidate(false);
                base.OnMouseLeave(e);
            }
            protected override void OnPaintBackground(System.Windows.Forms.PaintEventArgs pevent)
            {
                pevent.Graphics.Clear(Color.Wheat);
            }



        }


        #endregion



        public static int backnum = 0;
        //定时器
        public string singer_Name;
        public string singer_Name_2;
        public string singer_temp;

        //public static string PhotoPath = System.AppDomain.CurrentDomain.BaseDirectory + @"Resources\夜星余晖下的永恒.jpg";//Path.GetFullPath(
        public static string PhotoPath = System.AppDomain.CurrentDomain.BaseDirectory + @"夜星余晖下的永恒.jpg";
        public static string SingerPicPath = PhotoPath;//歌手照片路径

        public int Open_SongPicPath_Nums;
        public static string SongPicPath;//歌曲专辑照片路径
        public StreamReader Song_Pic_StreamReader;//读取mp3文件中最后几行，获取专辑信息

        public double numsDeg = 0;//旋转度数
        public Bitmap image;//矩阵位图
        public Bitmap backgroung;
        Graphics graphics;//绘制矩阵画面
        Rectangle rect;//矩阵的位置和大小
        PointF center;//矩阵的位置
        RectangleF picRect;//新矩阵相对于原矩阵的位置和大小
        PointF Pcenter;//新矩阵相对于原矩阵的位置

        #region 歌手圆形图片旋转——将图片转化为矩阵图，可自由旋转度数，再引用timer不断执行旋转递增的度数,实现动态旋转效果
        //旋转方式 类似视频播放，通过timer控件不断绘制角度细微差别的图片，达到图片动态旋转的视觉效果


        /// <summary>
        /// 检查是否开启专辑旋转，不开启则设置image为默认的唱片3
        /// </summary>
        public void picSelect()//根据歌曲信息匹配歌手名,将匹配的歌手名转化为图片文件的路径
        {
            //专辑图片分辨率最好为666*444左右
            if (Open_SongPicPath_Nums == 0)//找到了专辑照片          
            {
                if (image == null)
                {
                    image = global::Music_Player_Test.Properties.Resources.唱片3;
                }
            }
        }

        /// <summary>
        /// 图片旋转的方法,绘制一层图片
        /// </summary>
        public void picDraw()//图片旋转
        {
            if (Temp_Image != null)
            {
                picDraw_BackGround(Temp_Image);
            }
        }

        /// <summary>
        /// 使用背景图片进行绘制旋转，图片的缩放易控，更美观，但性能不如使用Bitmap，且受其它代码进程影响较多
        /// </summary>
        /// <param name="Temp_Image">背景图片二次生成的Bitmap,二次生成是通过背景图片可控样式生成指定比例大小的Bitmap</param>
        public void picDraw_BackGround(Bitmap Temp_Image)
        {
            //可以不重新绘制，但是Timer的运行速度取决于CPU的调度，如果不重新绘制，图片绘制的速度并不会受到控制，而是越来越快
            //是否重新绘制，目前对性能的影响微乎其微
            graphics = pictureBox1.CreateGraphics();//创建绘画区域            

            rect = new Rectangle(0, 0, pictureBox1.Width, pictureBox1.Height);//创建矩阵区域初始位置及大小  //要显示到Form的矩形区域
            center = new PointF(rect.Width / 2, rect.Height / 2);//制定矩阵区域中心位置

            float offsetX = 0;
            float offsetY = 0;
            offsetX = center.X - pictureBox1.Width / 2;
            offsetY = center.Y - pictureBox1.Height / 2;//计算创建绘图变量

            picRect = new RectangleF(offsetX, offsetY, pictureBox1.Width, pictureBox1.Height);//指定位置与大小来初始化RectangleF类

            Pcenter = new PointF(picRect.X + picRect.Width / 2, picRect.Y + picRect.Height / 2);//指定坐标初始化PointF类

            graphics.TranslateTransform(Pcenter.X, Pcenter.Y);//为绘制的画面指定图片坐标的原点（初始点）

            graphics.RotateTransform(Convert.ToSingle(numsDeg));//将指定旋转度数numsDeg  应用于graphics对象的变换矩阵

            graphics.TranslateTransform(-Pcenter.X, -Pcenter.Y);//为绘制的画面指定图片坐标的原点（结束点）

            //所要旋转的图片信息源
            graphics.DrawImage(Temp_Image, picRect);//在指定的picRect对象的位置绘制指定大小的Bitmap对象image

            numsDeg += 0.3;//只能进行++，不变则无法旋转，每次旋转都是相对于原位置进行旋转，而不是相对于旋转后新的位置
            //所以只能利用timer控件，不断的递增旋转的度数以达到图片旋转的视觉效果
        }

        /// <summary>
        /// 图片旋转所需要的方法合集
        /// </summary>
        public void Image_ALL()
        {
            picSelect();//检查是否开启专辑旋转，不开启则设置image为默认的唱片3

            if (Open_Singer_Image == 1)//如果开启了专辑旋转
            {
                picDraw();//绘制一层图片

                if (backnum == 0)//绑定Buttons,外界赋值一次执行一次界面背景更换
                {
                    SingerPicPath = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"\singerPhoto\" + singer_Name + ".jpg");//获取歌手图片所在路径  

                    if (File.Exists(SingerPicPath))//如果文件存在
                    {
                        backgroung = new Bitmap(SingerPicPath);

                        BackgroundImage = backgroung;//将当前显示在pictuerbox框的image矩阵图转化为背景图片
                    }
                    else
                    {
                        if(File.Exists(PhotoPath))
                            BackgroundImage = new Bitmap(PhotoPath);//提供路径将图片转化为矩阵图  
                    }

                    //切换歌曲时清空上一首残留
                    Clear_Null_KRC("ALL");//清空所有显示的歌词Text

                    backnum = 1;
                }
            }
            else//如果专辑旋转被关闭
            {
                image = global::Music_Player_Test.Properties.Resources.唱片3;//设置为默认图片
                Photo_BackGround_Time.Stop();//关闭旋转定时器
            }
        }

        /// <summary>
        /// 定时器
        /// 不断的绘制角度不同图片，达到图片旋转效果
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Photo_BackGround_Tick(object sender, EventArgs e)//执行方法
        {
            //判断是否暂停播放
            if ((int)axWindowsMediaPlayer1.playState == 3)//正在播放则继续绘制图片
            {
                Image_ALL();
            }
            else//不再播放则停止绘制新的图片
            {
                Photo_BackGround_Time.Stop();//只停止定时器，上一次绘制的图片保留
            }
        }
        #endregion      


        /// <summary>
        /// 已弃用   无法动态使用GDI绘制，因为本身就是GDI绘制而成，GDI中绘制GDI，What The Fuck???
        /// </summary>
        public double Turntable_Deg_Nums = 0;
        public double Turntable_Deg_Sum = 0;
        public Bitmap image_Turntable;
        #region  唱片机指针旋转    未启用

        private void Turntable_player_Nums_Tick(object sender, EventArgs e)
        {

            //Turntable_Deg_Sum += Turntable_Deg_Nums;

            //大于90读停止旋转
            //if (Turntable_Deg_Sum >= 90)
            //{
            //    Turntable_player_Nums.Stop();
            //}
        }

        public void Change_Player_Turntable_Right_90_Nums()
        {
            //image_Turntable = (Bitmap)pictureBox2.BackgroundImage;

            //graphics = pictureBox2.CreateGraphics();//创建绘画区域      

            rect = new Rectangle(0, 0, image_Turntable.Width, image_Turntable.Height);//创建矩阵区域初始位置及大小  //要显示到Form的矩形区域
            center = new PointF(rect.Width / 2, rect.Height / 2);//制定矩阵区域中心位置

            float offsetX = 0;
            float offsetY = 0;
            offsetX = center.X - image_Turntable.Width / 2;
            offsetY = center.Y - image_Turntable.Height / 2;//计算创建绘图变量

            picRect = new RectangleF(offsetX, offsetY, image_Turntable.Width, image_Turntable.Height);//指定位置与大小来初始化RectangleF类

            //Pcenter = new PointF(picRect.X + picRect.Width / 2, picRect.Y + picRect.Height / 2);//指定坐标初始化PointF类
            Pcenter = new PointF(picRect.X, picRect.Y);//指定坐标初始化PointF类

            graphics.TranslateTransform(Pcenter.X, Pcenter.Y);//为绘制的画面指定图片坐标的原点（初始点）

            graphics.RotateTransform(Convert.ToSingle(Turntable_Deg_Nums));//将指定旋转度数numsDeg  应用于graphics对象的变换矩阵

            graphics.TranslateTransform(-Pcenter.X, -Pcenter.Y);//为绘制的画面指定图片坐标的原点（结束点）

            graphics.DrawImage(image_Turntable, picRect);//在指定的picRect对象的位置绘制指定大小的Bitmap对象image


            Turntable_Deg_Nums -= 0.3;
        }

        public void Change_Player_Turntable_Left_90_Nums()
        {


            Turntable_Deg_Nums += 0.3;
        }

        #endregion


        public static int Get_FileMusic_Image;

        public static int Start_Song_Change_Image_Nums_1;//单歌手模式
        public static int Start_Song_Change_Image_Nums_2;//多歌手模式
        public static int Start_Song_Change_Image_Nums_3;//专辑模式
        public static int Start_Song_Change_Complete;
        public static Bitmap Temp_Image;
        #region 歌曲切换时事件  

        /// <summary>
        /// 切换歌曲时所必需的操作
        /// 专辑旋转角度清零
        /// 重新生成存储歌词逐字信息的数组
        /// 清空image图片
        /// 清空pictureBox1背景图片
        /// 代表歌曲播放状态的int型对象清零
        /// 代表歌曲播放状态的int型对象清零
        /// 清空双歌手中第二个歌手的信息
        /// 停止双歌手定时器（生成双图片，及每隔7s切换显示的图片）
        /// </summary>
        public void Song_Change_Clear_And_Create()
        {
            numsDeg = 0;//专辑旋转角度清零

            //重新生成存储歌词逐字信息的数组
            StartTimes = new int[500];
            MiddleTimes = new int[500];
            StartKrcTimes = new int[1000];
            MiddleKrcTimes = new int[1000];
            StartKrcTexts = new string[1000];
            StartKrcTimes_All = new double[1000];

            if (Get_FileMusic_Image != 1)
            {
                //清空image图片
                image = null;
            }

            Temp_Image = null;

            //清空pictureBox1背景图片
            pictureBox1.BackgroundImage = null;

            //代表歌曲播放状态的int型对象清零
            Start_Song_Change_Image_Nums_1 = 0;
            Start_Song_Change_Image_Nums_2 = 0;
            Start_Song_Change_Image_Nums_3 = 0;

            //清空双歌手中第二个歌手的信息
            singer_Name_2 = null;

            //停止双歌手定时器（生成双图片，及每隔7s切换显示的图片）
            Two_Singer_Pic.Stop();
        }


        ShellClass sh = new ShellClass();//调用Shell32.dll  ,   查找mp3文件信息
        Folder Folderdir;
        FolderItem FolderItemitem;
        /// <summary>
        /// 切换歌曲时所必需的操作
        /// 读取歌词路径,创建歌词文件流对象,生成歌词数组
        /// 调用Shell32.dll  ,   查找mp3文件信息
        /// 如果歌曲名合法,显示图片，纪录处于哪种模式（单歌手，.....）
        /// </summary>
        public void Song_Change()
        {
            Turntable_player_Nums.Start();

            Song_Change_Clear_And_Create();// 切换歌曲时所必需的操作

            //读取歌词路径,创建歌词文件流对象
            if (File.Exists(SongLrcPath))
            {
                Song_Lrc_StreamReader = new StreamReader(SongLrcPath, Encoding.UTF8);//完成后继续自动清理缓存
            }

            string str1 = "";
            try
            {
                //开始生成歌词数组
                Show_Lrc_Text.Start();

                //调用Shell32.dll  ,   查找mp3文件信息
                sh = new ShellClass();
                Folderdir = sh.NameSpace(Path.GetDirectoryName(axWindowsMediaPlayer1.URL.ToString()));
                FolderItemitem = Folderdir.ParseName(Path.GetFileName(axWindowsMediaPlayer1.URL.ToString()));
                str1 = Folderdir.GetDetailsOf(FolderItemitem, 14);
            }
            catch
            {
                //防止非法字符
            }

            if (str1.Length == 0)
            {
                string temps = SongNames_Temp;
                int nums = temps.LastIndexOf("-") + 1;
                if (nums > 0)
                {
                    str1 = temps.Substring(temps.LastIndexOf("-") + 2, temps.Length - temps.LastIndexOf("-") - 2);
                }
                else
                {
                    str1 = "";
                }
            }

            if (str1.Length > 0) //专辑模式
            {
                SingerPicPath = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"\singerPhoto\" + singer_Name + ".jpg");//获取歌手图片所在路径  

                try
                {
                    SongPicPath = string.Format(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"\singer_songPhoto\" + str1 + ".jpg"));//获取歌手图片所在路径 
                }
                catch
                {
                    //防止非法字符
                }


                string temps = SongNames_Temp;
                int nums = temps.LastIndexOf("-") + 1;
                if (nums > 0)
                {
                    str1 = temps.Substring(temps.LastIndexOf("-") + 2, temps.Length - temps.LastIndexOf("-") - 2);
                }
                else
                {
                    str1 = "";
                }
                string SongPicPath_2 = string.Format(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"\singer_songPhoto\" + str1 + ".jpg"));//获取歌手图片所在路径 



                //File.Exists
                //如果专辑文件存在
                if (File.Exists(SongPicPath))
                {
                    try
                    {
                        Temp_Image = new Bitmap(SongPicPath);//提供路径将图片转化为矩阵图 
                    }
                    catch
                    {

                    }

                    Start_Song_Change_Image_Nums_3 = 1;

                    image = Temp_Image;
                    Change_GUI_Pic_Image_Size();

                    if (File.Exists(SingerPicPath))
                    {
                        BackgroundImage = new Bitmap(SingerPicPath);//提供路径将图片转化为矩阵图  ;                           
                    }
                    else
                    {
                        //不存在路径则检查是否多歌手
                        Two_Singer_Photo();
                    }

                }
                else if (File.Exists(SongPicPath_2))//如果专辑文件中存在同歌名专辑
                {
                    Temp_Image = new Bitmap(SongPicPath_2);//提供路径将图片转化为矩阵图 

                    Start_Song_Change_Image_Nums_3 = 1;

                    image = Temp_Image;
                    Change_GUI_Pic_Image_Size();

                    if (File.Exists(SingerPicPath))
                    {
                        BackgroundImage = new Bitmap(SingerPicPath);//提供路径将图片转化为矩阵图  ;                           
                    }
                    else
                    {
                        //不存在路径则检查是否多歌手
                        Two_Singer_Photo();
                    }
                }
                else //文件不存在
                {
                    singer_temp = singer_Name;//记录当前歌手名
                    if (singer_temp.IndexOf("、") > 0 && singer_temp.IndexOf("、") == singer_temp.LastIndexOf("、"))
                    { //双歌手

                        Two_Singer_Photo();

                        Start_Song_Change_Image_Nums_2 = 1;

                    }
                    else//单歌手 
                    {
                        SingerPicPath = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"\singerPhoto\" + singer_Name + ".jpg");//获取歌手图片所在路径  
                        if (File.Exists(SingerPicPath))
                        {
                            Temp_Image = new Bitmap(SingerPicPath);//提供路径将图片转化为矩阵图  

                            BackgroundImage = Temp_Image;

                            image = Temp_Image;
                            Change_GUI_Pic_Image_Size();

                            Start_Song_Change_Image_Nums_1 = 1;
                        }
                        else
                        {
                            Temp_Image = global::Music_Player_Test.Properties.Resources.唱片3;
                            if (File.Exists(PhotoPath))
                            {
                                BackgroundImage = new Bitmap(PhotoPath);

                                image = Temp_Image;
                                Change_GUI_Pic_Image_Size();

                                Open_SongPicPath_Nums = 0;
                            }
                        }

                        Two_Singer_Pic.Stop();
                    }
                }

            }
            else
            {
                if (singer_Name != null)
                {
                    singer_temp = singer_Name;//记录当前歌手名
                    if (singer_temp.IndexOf("、") > 0 && singer_temp.IndexOf("、") == singer_temp.LastIndexOf("、"))
                    { //双歌手

                        Two_Singer_Photo();

                        Start_Song_Change_Image_Nums_2 = 1;

                    }
                    else//单歌手 
                    {
                        SingerPicPath = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"\singerPhoto\" + singer_Name + ".jpg");//获取歌手图片所在路径  
                        if (File.Exists(SingerPicPath))
                        {
                            Temp_Image = new Bitmap(SingerPicPath);//提供路径将图片转化为矩阵图  

                            BackgroundImage = Temp_Image;

                            image = Temp_Image;
                            Change_GUI_Pic_Image_Size();


                            Start_Song_Change_Image_Nums_1 = 1;
                        }
                        else
                        {
                            Temp_Image = global::Music_Player_Test.Properties.Resources.唱片3;

                            BackgroundImage = new Bitmap(PhotoPath);

                            image = Temp_Image;
                            Change_GUI_Pic_Image_Size();

                            Open_SongPicPath_Nums = 0;

                        }
                        Two_Singer_Pic.Stop();
                    }
                }
            }

            Start_Song_Change_Complete = 1;
        }




        #endregion


        #region   图片旋转绘制大小

        /// <summary>
        /// 将传入的image对象通过背景图片二次生成转换的具有固定比例大小的Temp_Image
        /// </summary>
        /// <param name="image"></param>
        public void Change_GUI_Pic_Image_Size()
        {
            if (image != null)
            {
                int X_Height = Convert.ToInt32(((Convert.ToDouble(image.Width) - Convert.ToDouble(image.Height)) / 2));//取从0开始到图片中间的宽度值
                int X_Width = Convert.ToInt32(((Convert.ToDouble(image.Height) - Convert.ToDouble(image.Width)) / 2));//取从0开始到图片中间的高度值

                if (image.Width >= image.Height)//横屏图片
                {
                    image = new Bitmap(MakeThumbnailImage(image, 1980, 1980, image.Height, image.Height, X_Height, 0));//X轴 -  取从0开始到图片中间的宽度值
                }
                else//竖屏图片
                {
                    image = new Bitmap(MakeThumbnailImage(image, 1980, 1980, image.Width, image.Width, 0, X_Width));//Y轴 - 取从0开始到图片中间的高度值
                }

                //将图片转换为背景图片，使用背景图片的style样式功能将image转换样式，再将转换过样式的image重新创建
                //不需要再使用背景旋转（背景旋转在切换照片时会显示背景图，旋转角度不同，影响视觉）
                this.pictureBox1.BackgroundImage = new Bitmap(image);

                Temp_Image = new Bitmap(pictureBox1.BackgroundImage);

                pictureBox1.BackgroundImage = null;
            }
            else
            {
                Temp_Image = null;

                pictureBox1.BackgroundImage = null;
            }
        }

        #endregion


        public int Time_1_Middle;	//int 	("," 的位置)
        public int Time_1_End;		//int 	("]" 的位置)
        public int Time_2_Start;	//int 	("<" 的位置)
        public int TIme_2_Middle_1; //int 	("," 的位置)
        public int TIme_2_Middle_2; //int 	("," 的位置)
        public int Time_2_End;      //int 	(">" 的位置)
        public int Start_Time;      //int	开始的时间
        public int Nums_Time;       //int	持续的时间

        public int[] LRC_Nums_Times;  //int数组     存储单个歌词持续的时间
        public string[] LRC_Text_Times;

        public string[] LRC_Time = new string[9999];
        public string[] LRC_Text = new string[9999];

        public StreamReader Song_Lrc_StreamReader;//将当前的歌词文件转化临时文件流
        public String A_String_Read;//传递临时生成的歌词时间

        public static string LRC_Text_Temp;//要返回的歌词Text  

        public static int Selected_KRC;//当前同步显示歌词的选定项
        public double Start_First_Time_1 = 0;
        public double End_Last_Time_1 = 0;
        #region 歌词读取存储——两个数组分别存储  歌词对应时间，与歌词内容（SongLrcPath）

        /// <summary>
        /// 定时器，读取歌词
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Show_Lrc_Text_Tick(object sender, EventArgs e)
        {
            player_lrc_Save_Text();
            Thread_Sleep();
        }


        /// <summary>
        /// 读取歌词文件中的逐字信息处理并保存在数组中
        /// </summary>
        /// <param name="KRC_One_Text">传入文件流读取的一整行KRC</param>
        /// <returns>返回一句完全由字符串拼接的string整个字符串</returns>
        public string Show_Song_KRC_Text(string KRC_One_Text)
        {
            //创建中间值存储
            string Temp_1 = KRC_One_Text;
            string Temp_2 = KRC_One_Text;
            string Temp_3 = KRC_One_Text;
            string Temp_4 = KRC_One_Text;

            LRC_Text_Temp = "";//要返回的歌词Text

            try
            {
                Time_1_Middle = Temp_1.IndexOf(",");//[81751,2131]  下标从0开始
                //MessageBox.Show(KRC_One_Text + "        , 的位置        " + Time_1_Middle);

                Time_1_End = Temp_1.IndexOf("]");
                //MessageBox.Show(KRC_One_Text + "        ] 的位置   " + Time_1_End);

                Start_Time = Convert.ToInt32(Temp_1.Substring(1, Time_1_Middle - 1));
                //MessageBox.Show(KRC_One_Text + "        开始的时间：    " + Start_Time);

                Nums_Time = Convert.ToInt32(Temp_2.Substring(Time_1_Middle + 1, Time_1_End - Time_1_Middle - 1));
                //MessageBox.Show(KRC_One_Text + "        持续的时间：    " + Nums_Time);


                //存储这一行歌词开始与持续的时间
                LRC_Start_And_Middle_Time(Start_Time, Nums_Time);



                Temp_3 = Temp_3.Substring(Time_1_End + 1, Temp_3.Length - Time_1_End - 1);//<0,255,0>胡<255,204,0>歌 <459,253,0>- <712,253,0>六<965,153,0>月<1118,203,0>的<1321,201,0>雨
                Temp_4 = Temp_3;

                LRC_Nums_Times = new int[30];
                LRC_Text_Times = new string[30];


                for (int i = 0; i < LRC_Nums_Times.Length; i++)
                {

                    Temp_3 = Temp_4;

                    if (LRC_Nums_Times[i] == 0 && LRC_Text_Times[i] == null)
                    {
                        try
                        {
                            //LRC_Nums_Times[i]  保存每个歌词的开始时间
                            string tempss = Temp_3;
                            int middle_nums_start_1 = tempss.IndexOf("<") + 1;//从0开始
                            int middle_nums_start_2 = tempss.IndexOf(",");//从0开始
                            int nums_1 = Convert.ToInt32(tempss.Substring(middle_nums_start_1, middle_nums_start_2 - middle_nums_start_1));//从0开始，选3个-> 255,



                            int middle_nums_1 = Temp_3.IndexOf(",");//<0,255,0>胡<255,204,0>歌 <459,253,0>- <712,253,0>六<965,153,0>月<1118,203,0>的<1321,201,0>雨
                            Temp_3 = Temp_3.Substring(middle_nums_1 + 1, Temp_3.Length - middle_nums_1 - 1);//255,0>胡<255,204,0>歌 <459,253,0>- <712,253,0>六<965,153,0>月<1118,203,0>的<1321,201,0>雨



                            //LRC_Text_Times   保存每个歌词的内容
                            string temp = Temp_3;//保存读取的单个歌词
                            Time_2_End = temp.IndexOf(">") + 1;
                            Time_2_Start = temp.IndexOf("<");
                            if (Time_2_Start > 0)
                            {
                                temp = temp.Substring(Time_2_End, Time_2_Start - Time_2_End);
                            }
                            else
                            {
                                temp = temp.Substring(Time_2_End, temp.Length - Time_2_End);
                            }
                            LRC_Text_Times[i] = temp;


                            string temps = Temp_3;//保存下一次substring的变量
                            Time_2_Start = temps.IndexOf("<");
                            if (Time_2_Start > 0)
                                Temp_4 = temps.Substring(Time_2_Start, temps.Length - Time_2_Start);//<255,204,0>歌 <459,253,0>- <712,253,0>六<965,153,0>月<1118,203,0>的<1321,201,0>雨



                            //LRC_Nums_Times[i]  保存每个歌词的持续时间
                            int middle_nums_2 = Temp_3.IndexOf(",");//从0开始
                            Temp_3 = Temp_3.Substring(0, middle_nums_2);//从0开始，选3个-> 255,
                            LRC_Nums_Times[i] = Convert.ToInt32(Temp_3);
                            int nums_2 = Convert.ToInt32(Temp_3);


                            //nums_1 + Start_Time    总时间制
                            KRC_Start_And_Middle_And_Text(nums_1, nums_2, temp, nums_1 + Start_Time);

                            if (Time_2_Start < 0)
                                break;

                        }
                        catch
                        {
                            break;
                        }

                    }
                }
            }
            catch
            {
                //MessageBox.Show("        错误！    ");
            }

            for (int i = 0; i < LRC_Text_Times.Length; i++)//字符串(char)集合
            {
                if (LRC_Text_Times[i] != null)
                {
                    LRC_Text_Temp += LRC_Text_Times[i];
                    if (LRC_Text_Times[i + 1] == null)
                    {
                        break;
                    }
                }
            }

            return LRC_Text_Temp;//整个字符串
        }

        /// <summary>
        /// 一行一行while 的读取歌词文件流中的信息并保存
        /// </summary>
        public void player_lrc_Save_Text()
        {
            //if (axWindowsMediaPlayer1.playState == WMPLib.WMPPlayState.wmppsPlaying)//关闭检查可在暂停音乐时同步歌词
            //{
            try
            {
                ////转换为Utf-8格式
                //string content = File.ReadAllText(SongLrcPath, Encoding.Default);
                //File.WriteAllText(SongLrcPath, content, Encoding.UTF8);
                if(Song_Lrc_StreamReader != null)
                    if (Song_Lrc_StreamReader.EndOfStream == false)//指示当前流位置是否在结尾
                    {
                        while ((A_String_Read = Song_Lrc_StreamReader.ReadLine()) != null)
                        {
                            if (A_String_Read.ToString().Length < 10)//跳过空格
                                continue;
                            if (A_String_Read.ToString().Substring(1, 2).Equals("id"))//跳过offset标签
                                continue;
                            if (A_String_Read.ToString().Substring(1, 2).Equals("ar"))//跳过ar标签
                                continue;
                            if (A_String_Read.ToString().Substring(1, 2).Equals("ti"))//跳过ti标签
                                continue;
                            if (A_String_Read.ToString().Substring(1, 2).Equals("by"))//跳过by标签
                                continue;
                            if (A_String_Read.ToString().Substring(1, 2).Equals("ha"))//跳过by标签
                                continue;
                            if (A_String_Read.ToString().Substring(1, 2).Equals("al"))//跳过by标签
                                continue;
                            if (A_String_Read.ToString().Substring(1, 2).Equals("si"))//跳过by标签
                                continue;
                            if (A_String_Read.ToString().Substring(1, 2).Equals("qq"))//跳过by标签
                                continue;
                            if (A_String_Read.ToString().Substring(1, 2).Equals("to"))//跳过by标签
                                continue;
                            if (A_String_Read.ToString().Substring(1, 2).Equals("of"))//跳过offset标签
                                continue;
                            if (A_String_Read.ToString().Substring(1, 2).Equals("la"))//跳过by标签
                                continue;

                            int nums_temp_1 = A_String_Read.IndexOf("]") + 1;
                            int nums_temp_2 = A_String_Read.IndexOf("[") + 1;

                            if (A_String_Read.Length <= nums_temp_1)//过滤歌词内容为空的行
                            {
                                continue;
                            }

                            //A_String_Read = "[" + Start_Time + "]" + Show_Song_KRC_Text(A_String_Read);
                            A_String_Read = Show_Song_KRC_Text(A_String_Read);

                            for (int i = 5; i < 500; i++)
                            {//获取mrc文件歌词内容                 
                                if (LRC_Text[i] == null)
                                    if (LRC_Text_Temp != null)
                                    {
                                        LRC_Text[i] = A_String_Read;
                                        if (LRC_Time[i] == null)
                                        {
                                            LRC_Time[i] = Start_Time.ToString();
                                            break;
                                        }
                                    }
                            }


                        }
                    }
                    else
                    {
                        //关闭对歌词文件的读取定时
                        Show_Lrc_Text.Stop();

                        //生成ComBox中的歌词文件
                        Clear_And_Create_SongKrc_LRC_Time_To_ComBox();


                        ///生成这首歌词第一句歌词开始的时间和最后一句歌词开始的时间
                        for (int j = 0; j < LRC_Time.Length; j++)//输出数组一开始的两个时间
                            if (LRC_Time[j] != null)
                            {
                                string temp_5 = LRC_Time[j];//753 
                                Start_First_Time_1 = Convert.ToInt64(temp_5) / 1000;
                                break;
                            }
                        for (int j = LRC_Time.Length - 1; j >= 0; j--)//输出数组最后的两个时间
                            if (LRC_Time[j] != null)
                            {
                                string temp_5 = LRC_Time[j];//201538
                                End_Last_Time_1 = Convert.ToInt64(temp_5) / 1000;//毫秒数 / 100
                                break;
                            }

                        Song_Lrc_Times.Start();


                        //MiddleKrcTime_Array = new ArrayList();
                        //string krc_str_temp = "krc_middle";
                        //KRC_Middle temp = null;

                        //for (int i = 5; i < 1000; i++)
                        //{

                        //    if (StartKrcTimes[i] == 0)
                        //    {
                        //        if (temp.nums[i] != 0)
                        //        {
                        //            MiddleKrcTime_Array.Add(temp);
                        //        }
                        //        temp = new KRC_Middle();
                        //    }
                        //    if (MiddleKrcTimes[i] != 0)
                        //    {
                        //        temp.nums[i] = MiddleKrcTimes[i];
                        //    }
                        //}

                        //Console.ReadLine();
                    }
            }
            catch
            {

            }
        }

        /// <summary>
        /// 生成ComBox中的歌词文件
        /// ComBox用来指定歌词的时间，然后通过快捷菜单播放这指定歌词所指向的时间
        /// 生成空的项是为了与歌词数组生成的空值同步显示
        /// </summary>
        public void Clear_And_Create_SongKrc_LRC_Time_To_ComBox()
        {
            this.comboBox1.Items.Clear();
            this.comboBox1.Items.Add("");
            this.comboBox1.Items.Add("");
            this.comboBox1.Items.Add("");
            this.comboBox1.Items.Add("");
            this.comboBox1.Items.Add("");
            for (int i = 5; i < 100; i++)
            {
                if (LRC_Time[i] != null)
                {
                    if (LRC_Text[i] != null)
                    {
                        if (LRC_Time[i].Length > 0)
                        {
                            if (LRC_Text[i].Length > 0)
                            {
                                string temp1 = LRC_Time[i];
                                string temp2 = LRC_Time[i];
                                int temp_min = Convert.ToInt32(temp1) / 1000 / 60;
                                int temp_mis = Convert.ToInt32(temp2) / 1000 % 60;
                                string result_nums = "";

                                if (temp_min < 0)
                                {
                                    if (temp_mis.ToString().Length == 1)
                                    {
                                        result_nums = "00:0" + temp_mis;
                                    }
                                    else
                                    {
                                        result_nums = "00:" + temp_mis;
                                    }
                                }
                                else
                                {
                                    if (temp_min.ToString().Length == 1 && temp_mis.ToString().Length == 1)
                                    {
                                        result_nums = "0" + temp_min + ":0" + temp_mis;
                                    }
                                    else if (temp_min.ToString().Length == 1 && temp_mis.ToString().Length != 1)
                                    {
                                        result_nums = "0" + temp_min + ":" + temp_mis;
                                    }
                                    else if (temp_min.ToString().Length != 1 && temp_mis.ToString().Length == 1)
                                    {
                                        result_nums = temp_min + ":0" + temp_mis;
                                    }
                                    else if (temp_min.ToString().Length != 1 && temp_mis.ToString().Length != 1)
                                    {
                                        result_nums = temp_min + ":" + temp_mis;
                                    }
                                }

                                //listBox1.Items.Add(result_nums + " - " + LRC_Text[i]);
                                comboBox1.Items.Add(result_nums + " - " + LRC_Text[i]);
                            }
                        }
                    }
                }
            }
            this.comboBox1.Items.Add("");
            this.comboBox1.Items.Add("");
            this.comboBox1.Items.Add("");
            this.comboBox1.Items.Add("");
            this.comboBox1.Items.Add("");
        }

        #endregion


        #region 歌词显示

        /// <summary>
        /// 定时器，同步显示歌词
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Song_Lrc_Time(object sender, EventArgs e)
        {
            player_lrc_Time_Read_Text();
            Thread_Sleep();
        }


        /// <summary>
        /// 同步歌词显示
        /// </summary>      
        public void player_lrc_Time_Read_Text()
        {
            //try
            //{
            for (int i = 5; i < 500; i++)
            {

                if (LRC_Time[i] != null)
                {
                    //生成当前歌词行开始的时间
                    string temp_1 = LRC_Time[i];//753
                    double num1 = Convert.ToInt64(temp_1) / 1000;

                    //生成下一行歌词开始的时间
                    double num2 = 0;
                    if (LRC_Time[i + 1] != null)
                    {
                        string temp_3 = LRC_Time[i + 1];//2275
                        num2 = Convert.ToInt64(temp_3) / 1000;
                    }

                    if (axWindowsMediaPlayer1.Ctlcontrols.currentPosition < Start_First_Time_1)
                    {//当前Windows Media Player播放的时间在第一句歌词的时间之前
                        for (int j = 5; j < LRC_Time.Length; j++)
                        {//输出数组最后的两个时间
                            if (LRC_Text[j] != null)
                            {

                                lblS6.Text = LRC_Text[j];
                                lblS7.Text = LRC_Text[j + 1];


                                myTopMost.lblS6_top.Text = LRC_Text[j];
                                myTopMost.lblS7.Text = LRC_Text[j + 1];


                                lblS8.Text = LRC_Text[j + 2];
                                lblS9.Text = LRC_Text[j + 3];
                                lblS10.Text = LRC_Text[j + 4];
                                lblS11.Text = LRC_Text[j + 5];

                                //生成当前歌词进度项
                                Selected_KRC = j;

                                //清空上半部分的歌词Text
                                Clear_Null_KRC("Half_1");


                                lblS6_top.Text = LRC_Text[i];
                                KRC_Time_WMP(i);


                                break;
                            }
                        }
                    }
                    else if (num1 < axWindowsMediaPlayer1.Ctlcontrols.currentPosition && num2 > axWindowsMediaPlayer1.Ctlcontrols.currentPosition)//当前Windows Media Player播放的时间在第一句和最后一句歌词的时间之间
                    {// || 
                        if (LRC_Text[i].Length != 0)
                        {
                            lblS1.Text = LRC_Text[i - 5];
                            lblS2.Text = LRC_Text[i - 4];
                            lblS3.Text = LRC_Text[i - 3];
                            lblS4.Text = LRC_Text[i - 2];
                            lblS5.Text = LRC_Text[i - 1];

                            lblS6.Text = LRC_Text[i];

                            lblS7.Text = LRC_Text[i + 1];

                            myTopMost.lblS6_top.Text = LRC_Text[i];
                            myTopMost.lblS7.Text = LRC_Text[i + 1];

                            lblS8.Text = LRC_Text[i + 2];
                            lblS9.Text = LRC_Text[i + 3];
                            lblS10.Text = LRC_Text[i + 4];
                            lblS11.Text = LRC_Text[i + 5];

                            //生成当前歌词进度项
                            Selected_KRC = i;


                            lblS6_top.Text = LRC_Text[i];
                            KRC_Time_WMP(i);


                        }
                    }
                    else if (axWindowsMediaPlayer1.Ctlcontrols.currentPosition > End_Last_Time_1)//当前Windows Media Player播放的时间在最后一句歌词的时间之后
                    {
                        for (int j = LRC_Time.Length - 1; j >= 5; j--)
                        {//输出数组最后的两个时间
                            if (LRC_Text[j] != null)
                            {
                                lblS1.Text = LRC_Text[j - 5];
                                lblS2.Text = LRC_Text[j - 4];
                                lblS3.Text = LRC_Text[j - 3];
                                lblS4.Text = LRC_Text[j - 2];
                                lblS5.Text = LRC_Text[j - 1];

                                lblS6.Text = LRC_Text[j];


                                myTopMost.lblS6_top.Text = LRC_Text[j];



                                //清空下半部分的歌词Text
                                Clear_Null_KRC("Half_2");

                                //生成当前歌词进度项
                                Selected_KRC = j;


                                lblS6_top.Text = LRC_Text[i];
                                KRC_Time_WMP(i);


                                break;
                            }
                        }
                    }
                }
            }

        }


        /// <summary>
        /// 已弃用     字符串（00:45）转秒数
        /// </summary>
        /// <param name="temp_1"></param>
        /// <param name="temp_2"></param>
        /// <returns>int型秒数</returns>
        public int Convert_LRC_Time(string temp_1, string temp_2)
        {
            try//00:21
            {
                int nums_1 = Convert.ToInt32(temp_1.Substring(0, 2));//string转换double   分
                int nums_2 = Convert.ToInt32(temp_2.Substring(3, 2));//string转换double   秒

                int EndNums = nums_1 * 60 + nums_2;//转化为 秒 数

                return EndNums;
            }
            catch
            {
                Thread_Sleep();
            }

            return 0;
        }

        /// <summary>
        /// 清空歌词数组
        /// </summary>
        public void lrc_Clear()
        {
            for (int i = 0; i < 500; i++)
            {
                if (LRC_Text[i] != null || LRC_Time[i] != null)
                {
                    LRC_Text[i] = null;
                    LRC_Time[i] = null;
                }
            }
        }

        /// <summary>
        /// 清空正在显示的歌词信息，更新背景，更新专辑
        /// </summary>
        public void label_Text_Clear()
        {
            try
            {
                //检测专辑图片是否开启
                picSelect();

                //检测要切换的背景图片是否存在，不存在则指定为系统默认图片
                SingerPicPath = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"\singerPhoto\" + singer_Name + ".jpg");//获取歌手图片所在路径  
                if (File.Exists(SingerPicPath))
                {
                    backgroung = new Bitmap(SingerPicPath);
                    BackgroundImage = backgroung;//将当前显示在pictuerbox框的image矩阵图转化为背景图片
                }
                else
                {
                    BackgroundImage = new Bitmap(PhotoPath);//提供路径将图片转化为矩阵图  
                }

                //清空所有的歌词Text
                Clear_Null_KRC("ALL");
            }
            catch
            {

            }
        }


        public static int Select_Open_ListBox1;//记录ComBox歌词项是否被打开  ,0:关闭  ，1：打开
        /// <summary>
        /// 打开或关闭ComBox，显示歌词进度信息
        /// </summary>
        /// <param name="sender">跳转歌曲进度</param>
        /// <param name="e">跳转歌曲进度</param>
        private void button_WMP_Time_Change_Click(object sender, EventArgs e)
        {
            if (Selected_KRC != 0)
            {
                if (Select_Open_ListBox1 == 0)
                {
                    comboBox1.Show();
                    Select_Open_ListBox1 = 1;

                    try
                    {
                        //ComBox的歌词选定项等于当前歌词进度项
                        this.comboBox1.SelectedIndex = Selected_KRC;
                    }
                    catch
                    {

                    }
                }
                else
                {
                    comboBox1.Hide();
                    Select_Open_ListBox1 = 0;
                }
            }
        }
        /// <summary>
        /// 跳转至指定播放进度,ComBox歌词进度显示项的双击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //当前选定的ComBox歌词进度显示项的值  不为空
            if (this.comboBox1.Items[this.comboBox1.SelectedIndex].ToString() != null)
            {
                //跳转至指定进度
                axWindowsMediaPlayer1.Ctlcontrols.currentPosition = Change_Player_Music_KRC_Time(this.comboBox1.Items[this.comboBox1.SelectedIndex].ToString());

                //this.listBox1.Hide();
                this.comboBox1.Hide();
                Select_Open_ListBox1 = 0;//记录ComBox歌词项是否被打开  ,0:关闭  ，1：打开

                axWindowsMediaPlayer1.Ctlcontrols.play();//跳转至指定进度后就播放
            }
        }


        /// <summary>
        /// 跳转至指定播放进度,ComBox歌词进度显示项的右键快捷菜单事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 跳转此进度ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //当前选定的ComBox歌词进度显示项的值  不为空
            if (this.comboBox1.Items[this.comboBox1.SelectedIndex].ToString() != null)
            {
                //跳转至指定进度
                axWindowsMediaPlayer1.Ctlcontrols.currentPosition = Change_Player_Music_KRC_Time(this.comboBox1.Items[this.comboBox1.SelectedIndex].ToString());

                //this.listBox1.Hide();
                this.comboBox1.Hide();
                Select_Open_ListBox1 = 0;//记录ComBox歌词项是否被打开  ,0:关闭  ，1：打开

                axWindowsMediaPlayer1.Ctlcontrols.play();//跳转至指定进度后就播放
            }
        }
        private void 跳转到高潮ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void 跳转到上一句ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //当前选定的ComBox歌词进度显示项的值  不为空
            if (this.comboBox1.Items[this.comboBox1.SelectedIndex - 1].ToString() != null)
            {
                //跳转至指定进度
                axWindowsMediaPlayer1.Ctlcontrols.currentPosition = Change_Player_Music_KRC_Time(this.comboBox1.Items[this.comboBox1.SelectedIndex - 1].ToString());

                //this.listBox1.Hide();
                this.comboBox1.Hide();
                Select_Open_ListBox1 = 0;//记录ComBox歌词项是否被打开  ,0:关闭  ，1：打开

                axWindowsMediaPlayer1.Ctlcontrols.play();//跳转至指定进度后就播放
            }
        }

        private void 跳转到下一句ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //当前选定的ComBox歌词进度显示项的值  不为空
            if (this.comboBox1.Items[this.comboBox1.SelectedIndex + 1].ToString() != null)
            {
                //跳转至指定进度
                axWindowsMediaPlayer1.Ctlcontrols.currentPosition = Change_Player_Music_KRC_Time(this.comboBox1.Items[this.comboBox1.SelectedIndex + 1].ToString());

                //this.listBox1.Hide();
                this.comboBox1.Hide();
                Select_Open_ListBox1 = 0;//记录ComBox歌词项是否被打开  ,0:关闭  ，1：打开

                axWindowsMediaPlayer1.Ctlcontrols.play();//跳转至指定进度后就播放
            }
        }



        /// <summary>
        /// 获取当前选定的ComBox歌词进度显示项的值，再根据值返回该指定歌词所在的时间
        /// </summary>
        /// <param name="Select_ComBox_Text">选定的ComBox歌词进度项的Text文本</param>
        /// <returns>根据选定的Text文本字符串，处理后返回该歌词进度的int时间秒数</returns>
        public int Change_Player_Music_KRC_Time(string Select_ComBox_Text)
        {
            //string temp_1 = this.listBox1.Items[this.listBox1.SelectedIndex].ToString();
            string temp_1 = Select_ComBox_Text;
            temp_1 = temp_1.Substring(0, temp_1.IndexOf(" - "));
            int Time_nums_temp = Convert_LRC_Time(temp_1, temp_1);

            for (int i = 5; i < 500; i++)
            {
                if (LRC_Time[i] != null)
                {
                    string temp = (Convert.ToInt32(LRC_Time[i]) / 1000).ToString();
                    if (temp.Equals(Time_nums_temp.ToString()))
                    {
                        return Time_nums_temp;
                    }
                }
            }
            return 0;
        }

        /// <summary>
        /// 清空指定部分的正在显示的歌词text
        /// </summary>
        /// <param name="Iinstruct">传入的指令集    ALL:全部   Half_1:上半部分    Half_2:下半部分</param>
        public void Clear_Null_KRC(string Iinstruct)
        {
            if (Iinstruct.Equals("ALL"))
            {
                lblS1.Text = null;//切换歌曲时清空上一首残留
                lblS2.Text = null;
                lblS3.Text = null;
                lblS4.Text = null;
                lblS5.Text = null;
                lblS6.Text = null;
                lblS7.Text = null;
                lblS8.Text = null;
                lblS9.Text = null;
                lblS10.Text = null;
                lblS11.Text = null;

                myTopMost.lblS6_top.Text = null;
                myTopMost.lblS7.Text = null;
            }
            else if (Iinstruct.Equals("Half_1"))
            {
                lblS1.Text = null;//切换歌曲时清空上一首残留
                lblS2.Text = null;
                lblS3.Text = null;
                lblS4.Text = null;
                lblS5.Text = null;
            }
            else if (Iinstruct.Equals("Half_2"))
            {
                lblS7.Text = null;
                lblS8.Text = null;
                lblS9.Text = null;
                lblS10.Text = null;
                lblS11.Text = null;

                myTopMost.lblS7.Text = null;
            }
        }


        #endregion



        public int Select_Open_KRC;//记录歌词逐字同步是否被打开  ,0:关闭  ，1：打开
        public static double Krc_Start_Times;//当前播放时间所同步的单个歌词的持续时间
        public int[] StartKrcTimes;//存储每个歌词开始的时间

        public ArrayList MiddleKrcTime_Array;

        public int[] MiddleKrcTimes;//存储每个歌词持续的时间


        public string[] StartKrcTexts;//存储每个歌词的内容

        public double[] StartKrcTimes_All;//存储每个歌词开始的时间,总时间制

        public int[] StartTimes;//存储每一行歌词开始的时间
        public int[] MiddleTimes;//存储每一行歌词持续的时间
        #region 歌词逐字上色同步    不稳定


        private void Show_Krc_Text_Tick(object sender, EventArgs e)
        {
            if (Select_Open_KRC == 1)
            {
                /*
                for (int k = 5; k < StartKrcTimes_All.Length; k++)
                {
                    if (StartKrcTimes_All[k] >= axWindowsMediaPlayer1.Ctlcontrols.currentPosition && StartKrcTimes_All[k] <= axWindowsMediaPlayer1.Ctlcontrols.currentPosition - 0.2)
                    {
                        Krc_Start_Times = MiddleKrcTimes[k];

                        lblS6_top.Width += Convert.ToInt16(Convert.ToDouble(20F) / Krc_Start_Times * 66);
                        break;
                    }
                }
            */

                for (int k = 5; k < StartKrcTimes_All.Length; k++)
                {
                    if (StartKrcTimes_All[k] >= axWindowsMediaPlayer1.Ctlcontrols.currentPosition)
                    {
                        lblS6_top.Width += Convert.ToInt16(Convert.ToDouble(20F) / MiddleKrcTimes[k] * 66);
                        break;
                    }
                }




            }
        }

        /// <summary>
        /// 当lblS6_Text发生变化时事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lblS6_TextChanged(object sender, EventArgs e)
        {
            if (Select_Open_KRC == 1)
            {
                lblS6_top.Width = 0;
            }
        }

        /// <summary>
        /// 设置是否开启歌词逐字同步
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Button_Open_KRC(object sender, EventArgs e)
        {
            if (button_Open_KRC.Text.Equals("开启歌词逐字同步（不稳定）"))
            {
                Select_Open_KRC = 1;
                button_Open_KRC.Text = "关闭歌词逐字同步（不稳定）";
                lblS6_top.Width = 0;

                Show_Krc_Text.Start();
            }
            else
            {
                Select_Open_KRC = 0;
                button_Open_KRC.Text = "开启歌词逐字同步（不稳定）";
                lblS6_top.Width = 0;
            }
        }
        /// <summary>
        /// 同步逐字歌词，根据传入的下标值增加Label的长度Width
        /// </summary>
        /// <param name="i">当前正在播放的歌词的数组的下标</param>
        public void KRC_Time_WMP(int i)
        {
            if (Select_Open_KRC == 1)
            {
                for (int k = i; k < StartKrcTimes_All.Length; k++)
                {
                    if (StartKrcTimes_All[k] >= axWindowsMediaPlayer1.Ctlcontrols.currentPosition && StartKrcTimes_All[k] <= axWindowsMediaPlayer1.Ctlcontrols.currentPosition - 0.2)
                    {
                        Krc_Start_Times = MiddleKrcTimes[k];
                        break;
                    }
                }

                if (Krc_Start_Times > 0.3)
                {
                    lblS6_top.Width += Convert.ToInt16(Convert.ToDouble(20F) / Krc_Start_Times * 66);
                }
            }
        }


        /// <summary>
        /// 存储单行歌词开始与持续的时间
        /// </summary>
        /// <param name="StartTime">存储单行歌词开始的时间</param>
        /// <param name="MiddleTime">存储单行歌词持续的时间</param>
        public void LRC_Start_And_Middle_Time(int StartTime, int MiddleTime)
        {
            for (int i = 5; i < 500; i++)
            {
                if (StartTimes[i] == 0)
                {
                    if (MiddleTimes[i] == 0)
                    {
                        StartTimes[i] = StartTime;
                        MiddleTimes[i] = MiddleTime;
                        break;
                    }
                }
            }

        }

        /// <summary>
        /// 存储每个歌词的内容
        /// </summary>
        /// <param name="StartTime">存储每个歌词开始的时间</param>
        /// <param name="MiddleTime">存储每个歌词持续的时间</param>
        /// <param name="KRC_Text">存储每个歌词的内容</param>
        public void KRC_Start_And_Middle_And_Text(int StartTime, int MiddleTime, String KRC_Text, int StartTime_ALL)
        {
            for (int i = 5; i < 1000; i++)
            {
                if (StartKrcTimes[i] == 0)
                {
                    if (MiddleKrcTimes[i] == 0)
                    {
                        if (StartKrcTexts[i] == null)
                        {
                            if (StartKrcTimes_All[i] == 0)
                            {
                                StartKrcTimes[i] = StartTime;
                                StartKrcTexts[i] = KRC_Text;
                                StartKrcTimes_All[i] = Convert.ToDouble(StartTime_ALL) / 1000;


                                MiddleKrcTimes[i] = MiddleTime;

                                break;
                            }
                        }
                    }
                }
            }
        }
        #endregion



        public int WMP_Song_Play_Ids;//表示当前正在播放的歌曲(DataGridView)id
        public int WMP_Song_Play_Ids_Love;//表示当前正在播放的歌曲(DataGridView)id
        public int WMP_Song_Play_Ids_Auto;//表示当前正在播放的歌曲(DataGridView)id
        public int WMP_Song_Play_Ids_MV;//表示当前正在播放的歌曲(DataGridView)id
        public int WMP_Song_Play_Ids_Love_MV;//表示当前正在播放的歌曲(DataGridView)id
        public int WMP_Song_Play_Ids_Auto_MV;//表示当前正在播放的歌曲(DataGridView)id

        public int Music_Back_nums = 0;
        public int Music_Next_nums = 0;

        public int num1_0_and_1_OnlyOnce = 0;//使方法中指定的某些代码仅执行一次

        public string SongLrcPath_Temp_MV = "";//存储歌词默认临时位置
        public static string SongLrcPath = "";//存储歌词默认临时位置
        public string SongLrcPath_MV = "";//存储歌词默认临时位置

        public String MV_name;//存储当前播放的歌曲名(用以转换成.MKV视频格式)
        public int Playing_DataGridView_Name_nums;//表示当前正在播放的歌曲列表 //全局变量// == 1  代表本地音乐 // == 2  代表我的收藏 // == 3  代表默认列表

        public string song_src_MV = "";
        public string song_names_MV = "";
        public string SongNames_Temp_MV;//传递临时生成的完好的SongLrcPath

        public string Song_Src_Song = "";
        public string Song_Names_Song = "";
        public string SongNames_Temp;//传递临时生成的完好的SongLrcPath

        public string singer_Name_MV;

        public int Song_Path_Temp_SongName_Player;////传递临时生成的最后一个含有（"-"）歌曲文件名 所处的int位置  SongLrcPath.LastIndexOf("-");
        public int Song_Path_Temp_LastMp3_Player;//传递临时生成的最后一个含有（"mp3"）歌曲文件名 所处的int位置  SongLrcPath.IndexOf("mp3");
        #region MV播放切换（Playing_DataGridView_Name_nums）（MV_name）

        public void Song_MV_Ids_Change()
        {
            if (Playing_DataGridView_Name_nums == 1)
                Song_MV_Chnage(dataGridView_List_ALL);

            else if (Playing_DataGridView_Name_nums == 2)
                Song_MV_Chnage(dataGridView_List_Love);

            else if (Playing_DataGridView_Name_nums == 3)
                Song_MV_Chnage(dataGridView_List_Auto);
        }

        public void Song_MV_Chnage(DataGridView List_Name)
        {
            song_src_MV = "";
            song_names_MV = "";
            SongNames_Temp_MV = "";

            if (List_Name.Equals(dataGridView_List_ALL))
            {
                song_src_MV = this.dataGridView_List_ALL.Rows[WMP_Song_Play_Ids_MV - 1].Cells["song_src_all"].Value.ToString().Trim();

                song_names_MV = this.dataGridView_List_ALL.Rows[WMP_Song_Play_Ids_MV - 1].Cells["song_name_all"].Value.ToString().Trim();

                SongNames_Temp_MV = dataGridView_List_ALL.Rows[WMP_Song_Play_Ids_MV - 1].Cells["song_name_all"].Value.ToString().Trim();//获取歌手姓名   

                WMP_Song_Play_Ids_MV = Convert.ToInt32(dataGridView_List_ALL.Rows[WMP_Song_Play_Ids_MV - 1].Cells["song_ids_all"].Value);
            }
            else if (List_Name.Equals(dataGridView_List_Love))
            {
                song_src_MV = this.dataGridView_List_Love.Rows[WMP_Song_Play_Ids_Love_MV - 1].Cells["song_src_love"].Value.ToString().Trim();

                song_names_MV = this.dataGridView_List_Love.Rows[WMP_Song_Play_Ids_Love_MV - 1].Cells["song_name_love"].Value.ToString().Trim();

                SongNames_Temp_MV = dataGridView_List_Love.Rows[WMP_Song_Play_Ids_Love_MV - 1].Cells["song_name_love"].Value.ToString().Trim();//获取歌手姓名    

                WMP_Song_Play_Ids_Love_MV = Convert.ToInt32(dataGridView_List_Love.Rows[WMP_Song_Play_Ids_Love_MV - 1].Cells["song_ids_love"].Value);
            }
            else if (List_Name.Equals(dataGridView_List_Auto))
            {
                song_src_MV = this.dataGridView_List_Auto.Rows[WMP_Song_Play_Ids_Auto_MV - 1].Cells["song_src_auto"].Value.ToString().Trim();

                song_names_MV = this.dataGridView_List_Auto.Rows[WMP_Song_Play_Ids_Auto_MV - 1].Cells["song_name_auto"].Value.ToString().Trim();

                SongNames_Temp_MV = dataGridView_List_Auto.Rows[WMP_Song_Play_Ids_Auto_MV - 1].Cells["song_name_auto"].Value.ToString().Trim();//获取歌手姓名  

                WMP_Song_Play_Ids_Auto_MV = Convert.ToInt32(dataGridView_List_Auto.Rows[WMP_Song_Play_Ids_Auto_MV - 1].Cells["song_ids_auto"].Value);
            }

            //this.axWindowsMediaPlayer1.URL = song_src_all;

            Song_Path_Temp_SongName_Player = SongNames_Temp_MV.IndexOf("-");

            if (Song_Path_Temp_SongName_Player > 0)
            {

                Song_Path_Temp_LastMp3_Player = SongNames_Temp_MV.LastIndexOf("mp3");
                if (SongNames_Temp_MV.LastIndexOf("mp3") <= 0)
                {
                    Song_Path_Temp_LastMp3_Player = SongNames_Temp_MV.LastIndexOf("flac");
                }

                SongLrcPath_MV = SongNames_Temp_MV;
                SongLrcPath_MV = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"\song_lrc\" + SongLrcPath_MV + ".mrc");


                singer_Name_MV = SongNames_Temp_MV.Substring(0, Song_Path_Temp_SongName_Player).Trim();

                MV_name = SongNames_Temp_MV + "mkv";

                backnum = 0;
            }
            else
            {
                SongLrcPath_MV = "";
                singer_Name_MV = "未知歌手";
                MV_name = "";
                backnum = 0;
            }
        }

        /// <summary>
        /// 播放MV时的排序算法
        /// </summary>
        public void Next_Or_Back_Or_Math_MV()
        {
            if (Song_Play_List.Text.Equals("顺序播放"))
            {
                if (Music_Back_nums == 1)
                {
                    if (Playing_DataGridView_Name_nums == 1)
                    {
                        if (WMP_Song_Play_Ids_MV <= 1)
                            WMP_Song_Play_Ids_MV = this.dataGridView_List_ALL.Rows.Count - 1;
                        else
                            WMP_Song_Play_Ids_MV--;//上一首是--  //下一首是++
                    }
                    else if (Playing_DataGridView_Name_nums == 2)
                    {
                        if (WMP_Song_Play_Ids_Love_MV <= 1)
                            WMP_Song_Play_Ids_Love_MV = this.dataGridView_List_Love.Rows.Count - 1;
                        else
                            WMP_Song_Play_Ids_Love_MV--;//上一首是--  //下一首是++
                    }
                    else if (Playing_DataGridView_Name_nums == 3)
                    {
                        if (WMP_Song_Play_Ids_Auto_MV <= 1)
                            WMP_Song_Play_Ids_Auto_MV = this.dataGridView_List_Auto.Rows.Count - 1;
                        else
                            WMP_Song_Play_Ids_Auto_MV--;//上一首是--  //下一首是++
                    }
                }
                else if (Music_Next_nums == 1)
                {
                    if (Playing_DataGridView_Name_nums == 1)
                    {
                        if (WMP_Song_Play_Ids_MV >= this.dataGridView_List_ALL.Rows.Count - 1)
                            WMP_Song_Play_Ids_MV = 1;
                        else
                            WMP_Song_Play_Ids_MV++;
                    }
                    else if (Playing_DataGridView_Name_nums == 2)
                    {
                        if (WMP_Song_Play_Ids_Love_MV >= this.dataGridView_List_Love.Rows.Count - 1)
                            WMP_Song_Play_Ids_Love_MV = 1;
                        else
                            WMP_Song_Play_Ids_Love_MV++;
                    }
                    else if (Playing_DataGridView_Name_nums == 3)
                    {
                        if (WMP_Song_Play_Ids_Auto_MV >= this.dataGridView_List_Auto.Rows.Count - 1)
                            WMP_Song_Play_Ids_Auto_MV = 1;
                        else
                            WMP_Song_Play_Ids_Auto_MV++;
                    }
                }
                else
                {
                    if (Playing_DataGridView_Name_nums == 1)
                    {
                        if (WMP_Song_Play_Ids_MV >= this.dataGridView_List_ALL.Rows.Count - 1)
                            WMP_Song_Play_Ids_MV = 1;
                        else
                            WMP_Song_Play_Ids_MV++;
                    }
                    else if (Playing_DataGridView_Name_nums == 2)
                    {
                        if (WMP_Song_Play_Ids_Love_MV >= this.dataGridView_List_Love.Rows.Count - 1)
                            WMP_Song_Play_Ids_Love_MV = 1;
                        else
                            WMP_Song_Play_Ids_Love_MV++;
                    }
                    else if (Playing_DataGridView_Name_nums == 3)
                    {
                        if (WMP_Song_Play_Ids_Auto_MV >= this.dataGridView_List_Auto.Rows.Count - 1)
                            WMP_Song_Play_Ids_Auto_MV = 1;
                        else
                            WMP_Song_Play_Ids_Auto_MV++;
                    }
                }
            }
            else if (Song_Play_List.Text.Equals("单曲循环"))
            {
                if (Music_Back_nums == 1)
                {
                    if (Playing_DataGridView_Name_nums == 1)
                    {
                        if (WMP_Song_Play_Ids_MV <= 1)
                            WMP_Song_Play_Ids_MV = this.dataGridView_List_ALL.Rows.Count - 1;
                        else
                            WMP_Song_Play_Ids_MV--;//上一首是--  //下一首是++
                    }
                    else if (Playing_DataGridView_Name_nums == 2)
                    {
                        if (WMP_Song_Play_Ids_Love_MV <= 1)
                            WMP_Song_Play_Ids_Love_MV = this.dataGridView_List_Love.Rows.Count - 1;
                        else
                            WMP_Song_Play_Ids_Love_MV--;//上一首是--  //下一首是++
                    }
                    else if (Playing_DataGridView_Name_nums == 3)
                    {
                        if (WMP_Song_Play_Ids_Auto_MV <= 1)
                            WMP_Song_Play_Ids_Auto_MV = this.dataGridView_List_Auto.Rows.Count - 1;
                        else
                            WMP_Song_Play_Ids_Auto_MV--;//上一首是--  //下一首是++
                    }
                }
                else if (Music_Next_nums == 1)
                {
                    if (Playing_DataGridView_Name_nums == 1)
                    {
                        if (WMP_Song_Play_Ids_MV >= this.dataGridView_List_ALL.Rows.Count - 1)
                            WMP_Song_Play_Ids_MV = 1;
                        else
                            WMP_Song_Play_Ids_MV++;
                    }
                    else if (Playing_DataGridView_Name_nums == 2)
                    {
                        if (WMP_Song_Play_Ids_Love_MV >= this.dataGridView_List_Love.Rows.Count - 1)
                            WMP_Song_Play_Ids_Love_MV = 1;
                        else
                            WMP_Song_Play_Ids_Love_MV++;
                    }
                    else if (Playing_DataGridView_Name_nums == 3)
                    {
                        if (WMP_Song_Play_Ids_Auto_MV >= this.dataGridView_List_Auto.Rows.Count - 1)
                            WMP_Song_Play_Ids_Auto_MV = 1;
                        else
                            WMP_Song_Play_Ids_Auto_MV++;
                    }
                }
                else
                {
                    WMP_Song_Play_Ids_MV += 0;
                    WMP_Song_Play_Ids_Love_MV += 0;
                    WMP_Song_Play_Ids_Auto_MV += 0;
                }
            }
            else
            {
                Random rd = new Random();
                if (Playing_DataGridView_Name_nums == 1)
                    WMP_Song_Play_Ids_MV = rd.Next(1, this.dataGridView_List_ALL.Rows.Count);//(生成1~10之间的随机数，不包括10)
                else if (Playing_DataGridView_Name_nums == 2)
                    WMP_Song_Play_Ids_Love_MV = rd.Next(1, this.dataGridView_List_Love.Rows.Count);//(生成1~10之间的随机数，不包括10)
                else if (Playing_DataGridView_Name_nums == 3)
                    WMP_Song_Play_Ids_Auto_MV = rd.Next(1, this.dataGridView_List_Auto.Rows.Count);//(生成1~10之间的随机数，不包括10)
            }
        }
        public void MV_Change()
        {
            //Song_Path_Temp_LastMp3_Player = SongLrcPath_Temp_MV.LastIndexOf("mp3");
            Song_Path_Temp_SongName_Player = SongLrcPath_Temp_MV.IndexOf("-");

            if (Song_Path_Temp_SongName_Player > 0)
            {
                SongNames_Temp_MV = SongLrcPath_Temp_MV;

                SongLrcPath_Temp_MV = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"\song_lrc\" + SongLrcPath_Temp_MV + ".mrc");

                singer_Name_MV = SongNames_Temp_MV.Substring(0, Song_Path_Temp_SongName_Player).Trim();

                backnum = 0;

                MV_name = SongNames_Temp_MV + ".mkv";
            }
            else
            {
                SongLrcPath_Temp_MV = "";
                singer_Name_MV = "";
                backnum = 0;
                MV_name = "";
            }
        }

        /// <summary>
        /// 下一首MV
        /// </summary>
        public void Next_MV()
        {
            SongLrcPath_Temp_MV = SongLrcPath_MV;

            if (Playing_DataGridView_Name_nums == 1)
            {
                Next_Or_Back_Or_Math_MV();
                for (int i = 0; i < this.dataGridView_List_ALL.Rows.Count - 1; i++)
                {
                    if (this.dataGridView_List_ALL.Rows[i].Cells["song_ids_all"].Value != null)
                    {
                        if (WMP_Song_Play_Ids_MV == Convert.ToInt16(this.dataGridView_List_ALL.Rows[i].Cells["song_ids_all"].Value))
                        {
                            SongLrcPath_Temp_MV = dataGridView_List_ALL.Rows[i].Cells["song_name_all"].Value.ToString().Trim();//获取歌手姓名

                            MV_Change();

                            if (File.Exists(string.Format(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"\song_MV\" + MV_name))))
                            {
                                Music_Next_nums = 0;
                                Music_Back_nums = 0;

                                break;
                            }else
                                Back_MV();
                        }
                        if (i == this.dataGridView_List_ALL.Rows.Count - 2)
                        {
                            i = 0;
                            Next_Or_Back_Or_Math_MV();
                        }
                    }
                }
            }
            else if (Playing_DataGridView_Name_nums == 2)
            {
                Next_Or_Back_Or_Math_MV();
                for (int i = 0; i < this.dataGridView_List_Love.Rows.Count - 1; i++)
                {
                    if (this.dataGridView_List_Love.Rows[i].Cells["song_ids_love"].Value != null)
                    {
                        if ((WMP_Song_Play_Ids_Love_MV.ToString()).Equals(this.dataGridView_List_Love.Rows[i].Cells["song_ids_love"].Value.ToString()))
                        {
                            SongLrcPath_Temp_MV = dataGridView_List_Love.Rows[i].Cells["song_name_love"].Value.ToString().Trim();//获取歌手姓名

                            MV_Change();

                            if (File.Exists(string.Format(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"\song_MV\" + MV_name))))
                            {
                                Music_Next_nums = 0;
                                Music_Back_nums = 0;

                                break;
                            }
                            else
                                Back_MV();

                            break;
                        }
                        else if (i == this.dataGridView_List_Love.Rows.Count - 2)
                        {
                            i = 0;
                            Next_Or_Back_Or_Math_MV();
                        }
                    }
                }
            }
            else if (Playing_DataGridView_Name_nums == 3)
            {
                Next_Or_Back_Or_Math_MV();
                for (int i = 0; i < this.dataGridView_List_Auto.Rows.Count - 1; i++)
                {
                    if (this.dataGridView_List_Auto.Rows[i].Cells["song_ids_auto"].Value != null)
                    {
                        if ((WMP_Song_Play_Ids_Auto_MV.ToString()).Equals(this.dataGridView_List_Auto.Rows[i].Cells["song_ids_auto"].Value.ToString()))
                        {
                            SongLrcPath_Temp_MV = dataGridView_List_Auto.Rows[i].Cells["song_name_auto"].Value.ToString().Trim();//获取歌手姓名

                            MV_Change();

                            if (File.Exists(string.Format(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"\song_MV\" + MV_name))))
                            {
                                Music_Next_nums = 0;
                                Music_Back_nums = 0;

                                break;
                            }
                            else
                                Back_MV();

                            break;
                        }
                        else if (i == this.dataGridView_List_Auto.Rows.Count - 2)
                        {
                            i = 0;
                            Next_Or_Back_Or_Math_MV();
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 上一首MV
        /// </summary>
        public void Back_MV()
        {
            if (Playing_DataGridView_Name_nums == 1)
            {
                Next_Or_Back_Or_Math_MV();
                for (int i = 0; i < this.dataGridView_List_ALL.Rows.Count - 1; i++)
                {
                    if (this.dataGridView_List_ALL.Rows[i].Cells["song_ids_all"].Value != null)
                    {
                        if ((WMP_Song_Play_Ids_MV).ToString().Equals(this.dataGridView_List_ALL.Rows[i].Cells["song_ids_all"].Value.ToString()))
                        {

                            SongLrcPath_Temp_MV = dataGridView_List_ALL.Rows[i].Cells["song_name_all"].Value.ToString().Trim();//获取歌手姓名

                            MV_Change();

                            if (File.Exists(string.Format(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"\song_MV\" + MV_name))))
                            {
                                Music_Next_nums = 0;
                                Music_Back_nums = 0;

                                break;
                            }
                            else
                                Back_MV();

                            break;

                        }
                        if (i == this.dataGridView_List_ALL.Rows.Count - 2)
                        {
                            i = 0;
                            Next_Or_Back_Or_Math_MV();
                        }

                    }
                }

            }
            else if (Playing_DataGridView_Name_nums == 2)
            {
                Next_Or_Back_Or_Math_MV();
                for (int i = 0; i < this.dataGridView_List_Love.Rows.Count - 1; i++)
                {
                    if (this.dataGridView_List_Love.Rows[i].Cells["song_ids_love"].Value != null)
                    {
                        if ((WMP_Song_Play_Ids_Love_MV.ToString()).Equals(this.dataGridView_List_Love.Rows[i].Cells["song_ids_love"].Value.ToString()))
                        {
                            SongLrcPath_Temp_MV = dataGridView_List_Love.Rows[i].Cells["song_name_love"].Value.ToString().Trim();//获取歌手姓名

                            MV_Change();

                            if (File.Exists(string.Format(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"\song_MV\" + MV_name))))
                            {
                                Music_Next_nums = 0;
                                Music_Back_nums = 0;

                                break;
                            }
                            else
                                Back_MV();

                            break;
                        }
                        else if (i == this.dataGridView_List_Love.Rows.Count - 2)
                        {
                            i = 0;
                            Next_Or_Back_Or_Math_MV();
                        }
                    }
                }
            }
            else if (Playing_DataGridView_Name_nums == 3)
            {
                Next_Or_Back_Or_Math_MV();
                for (int i = 0; i < this.dataGridView_List_Auto.Rows.Count - 1; i++)
                {
                    if (this.dataGridView_List_Auto.Rows[i].Cells["song_ids_auto"].Value != null)
                    {
                        if ((WMP_Song_Play_Ids_Auto_MV.ToString()).Equals(this.dataGridView_List_Auto.Rows[i].Cells["song_ids_auto"].Value.ToString()))
                        {
                            SongLrcPath_Temp_MV = dataGridView_List_Auto.Rows[i].Cells["song_name_auto"].Value.ToString().Trim();//获取歌手姓名

                            MV_Change();

                            if (File.Exists(string.Format(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"\song_MV\" + MV_name))))
                            {
                                Music_Next_nums = 0;
                                Music_Back_nums = 0;

                                break;
                            }
                            else
                                Back_MV();

                            break;
                        }
                        else if (i == this.dataGridView_List_Auto.Rows.Count - 2)
                        {
                            i = 0;
                            Next_Or_Back_Or_Math_MV();
                        }
                    }
                }
            }
        }

        #endregion
        #region 歌曲播放切换（Playing_DataGridView_Name_nums）（MV_name）（SongLrcPath）

        public void Next_Or_Back_Or_Math()
        {
            if (Song_Play_List.Text.Equals("顺序播放"))
            {
                if (Music_Back_nums == 1)
                {
                    if (Playing_DataGridView_Name_nums == 1)
                    {
                        if (WMP_Song_Play_Ids <= 1)
                        {
                            WMP_Song_Play_Ids = this.dataGridView_List_ALL.Rows.Count - 1;
                        }
                        else
                        {
                            WMP_Song_Play_Ids--;//上一首是--  //下一首是++
                        }
                    }
                    else if (Playing_DataGridView_Name_nums == 2)
                    {
                        if (WMP_Song_Play_Ids_Love <= 1)
                        {
                            WMP_Song_Play_Ids_Love = this.dataGridView_List_Love.Rows.Count - 1;
                        }
                        else
                        {
                            WMP_Song_Play_Ids_Love--;//上一首是--  //下一首是++
                        }
                    }
                    else if (Playing_DataGridView_Name_nums == 3)
                    {
                        if (WMP_Song_Play_Ids_Auto <= 1)
                        {
                            WMP_Song_Play_Ids_Auto = this.dataGridView_List_Auto.Rows.Count - 1;
                        }
                        else
                        {
                            WMP_Song_Play_Ids_Auto--;//上一首是--  //下一首是++
                        }
                    }
                }
                else if (Music_Next_nums == 1)
                {
                    if (Playing_DataGridView_Name_nums == 1)
                    {
                        if (WMP_Song_Play_Ids >= this.dataGridView_List_ALL.Rows.Count - 1)
                        {
                            WMP_Song_Play_Ids = 1;
                        }
                        else
                        {
                            WMP_Song_Play_Ids++;
                        }
                    }
                    else if (Playing_DataGridView_Name_nums == 2)
                    {
                        if (WMP_Song_Play_Ids_Love >= this.dataGridView_List_Love.Rows.Count - 1)
                        {
                            WMP_Song_Play_Ids_Love = 1;
                        }
                        else
                        {
                            WMP_Song_Play_Ids_Love++;
                        }
                    }
                    else if (Playing_DataGridView_Name_nums == 3)
                    {
                        if (WMP_Song_Play_Ids_Auto >= this.dataGridView_List_Auto.Rows.Count - 1)
                        {
                            WMP_Song_Play_Ids_Auto = 1;
                        }
                        else
                        {
                            WMP_Song_Play_Ids_Auto++;
                        }
                    }
                }
                else
                {
                    if (Playing_DataGridView_Name_nums == 1)
                    {
                        if (WMP_Song_Play_Ids >= this.dataGridView_List_ALL.Rows.Count - 1)
                        {
                            WMP_Song_Play_Ids = 1;
                        }
                        else
                        {
                            WMP_Song_Play_Ids++;
                        }
                    }
                    else if (Playing_DataGridView_Name_nums == 2)
                    {
                        if (WMP_Song_Play_Ids_Love >= this.dataGridView_List_Love.Rows.Count - 1)
                        {
                            WMP_Song_Play_Ids_Love = 1;
                        }
                        else
                        {
                            WMP_Song_Play_Ids_Love++;
                        }
                    }
                    else if (Playing_DataGridView_Name_nums == 3)
                    {
                        if (WMP_Song_Play_Ids_Auto >= this.dataGridView_List_Auto.Rows.Count - 1)
                        {
                            WMP_Song_Play_Ids_Auto = 1;
                        }
                        else
                        {
                            WMP_Song_Play_Ids_Auto++;
                        }
                    }
                }
            }
            else if (Song_Play_List.Text.Equals("单曲循环"))
            {
                if (Music_Back_nums == 1)
                {
                    if (Playing_DataGridView_Name_nums == 1)
                    {
                        if (WMP_Song_Play_Ids <= 1)
                        {
                            WMP_Song_Play_Ids = this.dataGridView_List_ALL.Rows.Count - 1;
                        }
                        else
                        {
                            WMP_Song_Play_Ids--;//上一首是--  //下一首是++
                        }
                    }
                    else if (Playing_DataGridView_Name_nums == 2)
                    {
                        if (WMP_Song_Play_Ids_Love <= 1)
                        {
                            WMP_Song_Play_Ids_Love = this.dataGridView_List_Love.Rows.Count - 1;
                        }
                        else
                        {
                            WMP_Song_Play_Ids_Love--;//上一首是--  //下一首是++
                        }
                    }
                    else if (Playing_DataGridView_Name_nums == 3)
                    {
                        if (WMP_Song_Play_Ids_Auto <= 1)
                        {
                            WMP_Song_Play_Ids_Auto = this.dataGridView_List_Auto.Rows.Count - 1;
                        }
                        else
                        {
                            WMP_Song_Play_Ids_Auto--;//上一首是--  //下一首是++
                        }
                    }
                }
                else if (Music_Next_nums == 1)
                {
                    if (Playing_DataGridView_Name_nums == 1)
                    {
                        if (WMP_Song_Play_Ids >= this.dataGridView_List_ALL.Rows.Count - 1)
                        {
                            WMP_Song_Play_Ids = 1;
                        }
                        else
                        {
                            WMP_Song_Play_Ids++;
                        }
                    }
                    else if (Playing_DataGridView_Name_nums == 2)
                    {
                        if (WMP_Song_Play_Ids_Love >= this.dataGridView_List_Love.Rows.Count - 1)
                        {
                            WMP_Song_Play_Ids_Love = 1;
                        }
                        else
                        {
                            WMP_Song_Play_Ids_Love++;
                        }
                    }
                    else if (Playing_DataGridView_Name_nums == 3)
                    {
                        if (WMP_Song_Play_Ids_Auto >= this.dataGridView_List_Auto.Rows.Count - 1)
                        {
                            WMP_Song_Play_Ids_Auto = 1;
                        }
                        else
                        {
                            WMP_Song_Play_Ids_Auto++;
                        }
                    }
                }
                else
                {
                    WMP_Song_Play_Ids += 0;
                    WMP_Song_Play_Ids_Love += 0;
                    WMP_Song_Play_Ids_Auto += 0;
                }
            }
            else
            {
                Random rd = new Random();
                if (Playing_DataGridView_Name_nums == 1)
                {
                    WMP_Song_Play_Ids = rd.Next(1, this.dataGridView_List_ALL.Rows.Count);//(生成1~10之间的随机数，不包括10)
                }
                else if (Playing_DataGridView_Name_nums == 2)
                {
                    WMP_Song_Play_Ids_Love = rd.Next(1, this.dataGridView_List_Love.Rows.Count);//(生成1~10之间的随机数，不包括10)
                }
                else if (Playing_DataGridView_Name_nums == 3)
                {
                    WMP_Song_Play_Ids_Auto = rd.Next(1, this.dataGridView_List_Auto.Rows.Count);//(生成1~10之间的随机数，不包括10)
                }
            }
        }
        public void Music_Change()
        {
            //Song_Path_Temp_LastMp3_Player = SongLrcPath.LastIndexOf("mp3");
            Song_Path_Temp_SongName_Player = SongLrcPath.IndexOf("-");

            Song_name = SongLrcPath;

            if (Song_Path_Temp_SongName_Player > 0)
            {
                SongNames_Temp = SongLrcPath;

                SongLrcPath = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"\song_lrc\" + SongLrcPath + ".mrc");

                singer_Name = SongNames_Temp.Substring(0, Song_Path_Temp_SongName_Player).Trim();

                backnum = 0;

                MV_name = SongNames_Temp + ".mkv";

                Song_Path_Temp_SongName_Player += 2;
                label_Singer_Name.Text = singer_Name;
                label_Song_Name.Text = Song_name.Substring(Song_Path_Temp_SongName_Player, Song_name.Length - Song_Path_Temp_SongName_Player);
            }
            else
            {

                SongLrcPath = "";

                singer_Name = "未知歌手";

                backnum = 0;

                MV_name = "";

                Song_Path_Temp_SongName_Player += 2;
                label_Singer_Name.Text = singer_Name;
                label_Song_Name.Text = Song_name.Substring(Song_Path_Temp_SongName_Player, Song_name.Length - Song_Path_Temp_SongName_Player);

            }
        }
        public void Next_Song(int nums)
        {

            if (Playing_DataGridView_Name_nums == 1)
            {
                Next_Or_Back_Or_Math();
                for (int i = 0; i < this.dataGridView_List_ALL.Rows.Count - 1; i++)
                {
                    if (this.dataGridView_List_ALL.Rows[i].Cells["song_ids_all"].Value != null)
                    {
                        if (WMP_Song_Play_Ids == Convert.ToInt16(this.dataGridView_List_ALL.Rows[i].Cells["song_ids_all"].Value))
                        {
                            if (nums == 0)
                            {
                                Get_FileMp3_Image_Info(this.dataGridView_List_ALL.Rows[i].Cells["song_src_all"].Value.ToString());

                                axWindowsMediaPlayer1.URL = this.dataGridView_List_ALL.Rows[i].Cells["song_src_all"].Value.ToString();
                            }

                            SongLrcPath = dataGridView_List_ALL.Rows[i].Cells["song_name_all"].Value.ToString().Trim();//获取歌手姓名

                            Music_Change();

                            Music_Next_nums = 0;

                            Music_Back_nums = 0;

                            break;

                        }
                        if (i == this.dataGridView_List_ALL.Rows.Count - 2)
                        {
                            i = 0;
                            Next_Or_Back_Or_Math();
                        }


                    }
                }

            }
            else if (Playing_DataGridView_Name_nums == 2)
            {
                Next_Or_Back_Or_Math();
                for (int i = 0; i < this.dataGridView_List_Love.Rows.Count - 1; i++)
                {
                    if (this.dataGridView_List_Love.Rows[i].Cells["song_ids_love"].Value != null)
                    {
                        if ((WMP_Song_Play_Ids_Love.ToString()).Equals(this.dataGridView_List_Love.Rows[i].Cells["song_ids_love"].Value.ToString()))
                        {
                            if (nums == 0)
                            {
                                Get_FileMp3_Image_Info(this.dataGridView_List_Love.Rows[i].Cells["song_src_love"].Value.ToString());

                                axWindowsMediaPlayer1.URL = this.dataGridView_List_Love.Rows[i].Cells["song_src_love"].Value.ToString();
                            }
                            SongLrcPath = dataGridView_List_Love.Rows[i].Cells["song_name_love"].Value.ToString().Trim();//获取歌手姓名

                            Music_Change();

                            Music_Next_nums = 0;

                            Music_Back_nums = 0;

                            break;

                        }
                        else if (i == this.dataGridView_List_Love.Rows.Count - 2)
                        {
                            i = 0;
                            Next_Or_Back_Or_Math();
                        }
                    }
                }
            }
            else if (Playing_DataGridView_Name_nums == 3)
            {
                Next_Or_Back_Or_Math();
                for (int i = 0; i < this.dataGridView_List_Auto.Rows.Count - 1; i++)
                {
                    if (this.dataGridView_List_Auto.Rows[i].Cells["song_ids_auto"].Value != null)
                    {
                        if ((WMP_Song_Play_Ids_Auto.ToString()).Equals(this.dataGridView_List_Auto.Rows[i].Cells["song_ids_auto"].Value.ToString()))
                        {
                            if (nums == 0)
                            {
                                Get_FileMp3_Image_Info(this.dataGridView_List_Auto.Rows[i].Cells["song_src_auto"].Value.ToString());

                                axWindowsMediaPlayer1.URL = this.dataGridView_List_Auto.Rows[i].Cells["song_src_auto"].Value.ToString();
                            }
                            SongLrcPath = dataGridView_List_Auto.Rows[i].Cells["song_name_auto"].Value.ToString().Trim();//获取歌手姓名

                            Music_Change();

                            Music_Next_nums = 0;

                            Music_Back_nums = 0;

                            break;

                        }
                        else if (i == this.dataGridView_List_Auto.Rows.Count - 2)
                        {
                            i = 0;
                            Next_Or_Back_Or_Math();
                        }
                    }
                }
            }
        }
        public void Back_Song(int nums)
        {

            if (Playing_DataGridView_Name_nums == 1)
            {
                Next_Or_Back_Or_Math();
                for (int i = 0; i < this.dataGridView_List_ALL.Rows.Count - 1; i++)
                {
                    if (this.dataGridView_List_ALL.Rows[i].Cells["song_ids_all"].Value != null)
                    {
                        if ((WMP_Song_Play_Ids).ToString().Equals(this.dataGridView_List_ALL.Rows[i].Cells["song_ids_all"].Value.ToString()))
                        {
                            if (nums == 0)
                            {
                                Get_FileMp3_Image_Info(this.dataGridView_List_ALL.Rows[i].Cells["song_src_all"].Value.ToString());

                                axWindowsMediaPlayer1.URL = this.dataGridView_List_ALL.Rows[i].Cells["song_src_all"].Value.ToString();
                            }
                            SongLrcPath = dataGridView_List_ALL.Rows[i].Cells["song_name_all"].Value.ToString().Trim();//获取歌手姓名

                            Music_Change();

                            Music_Next_nums = 0;

                            Music_Back_nums = 0;

                            break;

                        }
                        if (i == this.dataGridView_List_ALL.Rows.Count - 2)
                        {
                            i = 0;
                            Next_Or_Back_Or_Math();
                        }

                    }
                }

            }
            else if (Playing_DataGridView_Name_nums == 2)
            {
                Next_Or_Back_Or_Math();
                for (int i = 0; i < this.dataGridView_List_Love.Rows.Count - 1; i++)
                {
                    if (this.dataGridView_List_Love.Rows[i].Cells["song_ids_love"].Value != null)
                    {
                        if ((WMP_Song_Play_Ids_Love.ToString()).Equals(this.dataGridView_List_Love.Rows[i].Cells["song_ids_love"].Value.ToString()))
                        {
                            if (nums == 0)
                            {
                                Get_FileMp3_Image_Info(this.dataGridView_List_Love.Rows[i].Cells["song_src_love"].Value.ToString());

                                axWindowsMediaPlayer1.URL = this.dataGridView_List_Love.Rows[i].Cells["song_src_love"].Value.ToString();
                            }
                            SongLrcPath = dataGridView_List_Love.Rows[i].Cells["song_name_love"].Value.ToString().Trim();//获取歌手姓名

                            Music_Change();

                            Music_Next_nums = 0;

                            Music_Back_nums = 0;

                            break;

                        }
                        else if (i == this.dataGridView_List_Love.Rows.Count - 2)
                        {
                            i = 0;
                            Next_Or_Back_Or_Math();
                        }
                    }
                }
            }
            else if (Playing_DataGridView_Name_nums == 3)
            {
                Next_Or_Back_Or_Math();
                for (int i = 0; i < this.dataGridView_List_Auto.Rows.Count - 1; i++)
                {
                    if (this.dataGridView_List_Auto.Rows[i].Cells["song_ids_auto"].Value != null)
                    {
                        if ((WMP_Song_Play_Ids_Auto.ToString()).Equals(this.dataGridView_List_Auto.Rows[i].Cells["song_ids_auto"].Value.ToString()))
                        {
                            if (nums == 0)
                            {
                                Get_FileMp3_Image_Info(this.dataGridView_List_Auto.Rows[i].Cells["song_src_auto"].Value.ToString());

                                axWindowsMediaPlayer1.URL = this.dataGridView_List_Auto.Rows[i].Cells["song_src_auto"].Value.ToString();
                            }
                            SongLrcPath = dataGridView_List_Auto.Rows[i].Cells["song_name_auto"].Value.ToString().Trim();//获取歌手姓名

                            Music_Change();

                            Music_Next_nums = 0;

                            Music_Back_nums = 0;

                            break;

                        }
                        else if (i == this.dataGridView_List_Auto.Rows.Count - 2)
                        {
                            i = 0;
                            Next_Or_Back_Or_Math();
                        }
                    }
                }
            }
        }

        #endregion
        public static int MV_Back_nums = 0;
        public static int MV_Next_nums = 0;
        #region 上一首（num1_0_and_1_OnlyOnce）（MV_name）


        private void pictureBox_Back_Click(object sender, EventArgs e)
        {
            //清空Winodws Media Player控件的数据缓存(控件自带的播放列表，不需要使用该控件自带的播放列表)
            //axWindowsMediaPlayer1.currentPlaylist.clear();
            //axWindowsMediaPlayer2.currentPlaylist.clear();

            //刷新状态  用于MV跳过不存在的MV路径
            MV_Back_nums = 0;
            MV_Next_nums = 0;

            SongIds_New_Update_All();

            Thread.Sleep(50);
            Music_Back_nums = 1;
            if (Song_Play_MV_Nums == 0)
            {
                this.panel_Song_LRC.Invalidate();//重绘背景色

                lrc_Clear();//清理歌词

                Back_Song(0);

                Song_Change();//开启歌词
            }
            else
            {
                MV_Back_nums = 1;

                //停顿2秒钟再重新播放
                Thread.Sleep(50);
                //重新播放

                string temp = MV_name;
                do
                {
                    //刷新同步当前歌曲序号，防止序号不同步，MV播放至下下个
                    Song_MV_Ids_Change();

                    Back_MV();

                    if (temp.Equals(MV_name))
                    {
                        break;
                    }
                } while (!File.Exists(string.Format(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"\song_MV\" + MV_name))));


                this.axWindowsMediaPlayer2.URL = string.Format(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"\song_MV\" + MV_name + ""));

                num1_0_and_1_OnlyOnce = 0;

                axWindowsMediaPlayer2.Ctlcontrols.play();




            }

            label_Text_Clear();

            button_Open_Windows_Picture_All();//切换桌面背景//在  已开启桌面写真的情况下
        }

        #endregion
        #region 下一首（num1_0_and_1_OnlyOnce）（MV_name）

        private void pictureBox_Next_Click(object sender, EventArgs e)
        {
            //清空Winodws Media Player控件的数据缓存(控件自带的播放列表，不需要使用该控件自带的播放列表)
            //axWindowsMediaPlayer1.currentPlaylist.clear();
            //axWindowsMediaPlayer2.currentPlaylist.clear();


            //若不归0 , 则下一首不存在MV时，MV_Back_nums,MV_Next_nums都为1，且timer if(MV_Back_nums == 1)在前优先执行，便会返回上一首MV,即下一首MV为空则无法执行下一首MV操作，一直点击下一首就会在此MV循环播放
            MV_Back_nums = 0;
            MV_Next_nums = 0;

            SongIds_New_Update_All();

            Thread.Sleep(50);
            Music_Next_nums = 1;
            if (Song_Play_MV_Nums == 0)
            {
                this.panel_Song_LRC.Invalidate();//重绘背景色

                lrc_Clear();

                Next_Song(0);

                Song_Change();//开启歌词
            }
            else
            {
                MV_Next_nums = 1;

                //停顿2秒钟再重新播放
                Thread.Sleep(50);
                //重新播放

                string temp = MV_name;
                do
                {
                    //刷新同步当前歌曲序号，防止序号不同步，MV播放至下下个
                    Song_MV_Ids_Change();

                    Next_MV();

                    if (temp.Equals(MV_name))
                    {
                        break;
                    }
                } while (!File.Exists(string.Format(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"\song_MV\" + MV_name))));


                this.axWindowsMediaPlayer2.URL = string.Format(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"\song_MV\" + MV_name + ""));

                num1_0_and_1_OnlyOnce = 0;

                axWindowsMediaPlayer2.Ctlcontrols.play();
            }


            label_Text_Clear();//清理歌词

            button_Open_Windows_Picture_All();//切换桌面背景//在  已开启桌面写真的情况下


        }

        #endregion


        //同时不断更新歌曲序号
        #region 播放方式 判断是否已结束播放  音乐播放（num1_0_and_1_OnlyOnce）（MV_name）

        private void Song_Play_List_Click(object sender, EventArgs e)
        {
            if (Song_Play_List.Text.Equals("顺序播放"))
            {

                Song_Play_List.Text = "单曲循环";
            }
            else if (Song_Play_List.Text.Equals("单曲循环"))
            {

                Song_Play_List.Text = "随机播放";
            }
            else
            {

                Song_Play_List.Text = "顺序播放";
            }
        }

        private void Music_Auto_Open_Tick(object sender, EventArgs e)
        {
            //实时不断的更新歌曲列表
            SongIds_New_Update_All();

            try
            {

                if (Open_Singer_Image == 1)
                {
                    if (Photo_BackGround_Time.Enabled == false)
                    {
                        Photo_BackGround_Time.Start();
                    }
                }

                if (Open_Singer_Image == 0)
                {
                    if (Photo_BackGround_Time.Enabled == true)
                    {
                        Photo_BackGround_Time.Stop();
                    }
                }

                if ((int)axWindowsMediaPlayer1.playState == 3)//正在播放
                {
                    //if (this.pictureBox_Panel.BackgroundImage != global::Music_Player_Test.Properties.Resources.唱片机)
                    //{
                    //    this.pictureBox_Panel.BackgroundImage = global::Music_Player_Test.Properties.Resources.唱片机;
                    //}
                }
                else if ((int)axWindowsMediaPlayer1.playState == 2)//暂停播放
                {
                    //if (this.pictureBox_Panel.BackgroundImage != global::Music_Player_Test.Properties.Resources.唱片3)
                    //{
                    //    this.pictureBox_Panel.BackgroundImage = global::Music_Player_Test.Properties.Resources.唱片3;
                    //}
                }


                //判断是否已结束播放
                if ((int)axWindowsMediaPlayer1.playState == 1)
                {
                    //清空Winodws Media Player控件的数据缓存(控件自带的播放列表，不需要使用该控件自带的播放列表)
                    //axWindowsMediaPlayer1.currentPlaylist.clear();
                    //axWindowsMediaPlayer2.currentPlaylist.clear();


                    //停顿2秒钟再重新播放
                    Thread.Sleep(50);
                    //重新播放

                    Next_Song(0);

                    axWindowsMediaPlayer1.Ctlcontrols.play();

                    this.panel_Song_LRC.Invalidate();//重绘背景色

                    label_Text_Clear();

                    lrc_Clear();

                    button_Open_Windows_Picture_All();//切换桌面背景//在  已开启桌面写真的情况下

                    Song_Change();//开启歌词


                }

                num1_0_and_1_OnlyOnce = 1;

                //判断是否已结束播放
                if ((int)axWindowsMediaPlayer2.playState == 1)
                {
                    //清空Winodws Media Player控件的数据缓存(控件自带的播放列表，不需要使用该控件自带的播放列表)
                    //axWindowsMediaPlayer1.currentPlaylist.clear();
                    //axWindowsMediaPlayer2.currentPlaylist.clear();


                    //停顿2秒钟再重新播放
                    Thread.Sleep(50);
                    //重新播放

                    string temp = MV_name;
                    do
                    {
                        //刷新同步当前歌曲序号，防止序号不同步，MV播放至下下个
                        Song_MV_Ids_Change();

                        Next_MV();

                        if (temp.Equals(MV_name))
                        {
                            break;
                        }
                    } while (!File.Exists(string.Format(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"\song_MV\" + MV_name))));


                    this.axWindowsMediaPlayer2.URL = string.Format(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"\song_MV\" + MV_name + ""));

                    num1_0_and_1_OnlyOnce = 0;

                    axWindowsMediaPlayer2.Ctlcontrols.play();
                }

            }
            catch
            {
                Thread_Sleep();
            }
            Thread_Sleep();
        }
        #endregion


        public int Song_Path_Temp_SongName_Frm;//传递临时生成的最后一个含有（"-"）歌曲文件名 所处的int位置  SongLrcPath.LastIndexOf("-");
        public int Song_Path_Temp_LastMp3_Frm;//传递临时生成的最后一个含有（"-"）歌曲文件名 所处的int位置  SongLrcPath.IndexOf("mp3");
        //BUG：表选中的Select缓存无法被读取（也不应该被读取，表选中的Select缓存应回收）
        #region 播放此音乐,记录点击歌曲列表时的属性变化（Select_DataGridView_Name_nums）（Playing_DataGridView_Name_nums）（MV_name）（SongLrcPath）

        public void Select_DataGridView_All_Click(object sender, DataGridViewCellEventArgs e)
        {
            Select_DataGridView_Name_nums = 1;
        }
        public void Select_DataGridView_Love_Click(object sender, DataGridViewCellEventArgs e)
        {
            Select_DataGridView_Name_nums = 2;
        }
        public void Select_DataGridView_Auto_Click(object sender, DataGridViewCellEventArgs e)
        {
            Select_DataGridView_Name_nums = 3;
        }
        private void 播放此音乐ToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            //清空Winodws Media Player控件的数据缓存(控件自带的播放列表，不需要使用该控件自带的播放列表)
            //axWindowsMediaPlayer1.currentPlaylist.clear();
            //axWindowsMediaPlayer2.currentPlaylist.clear();

            if (this.dataGridView_List_ALL.SelectedRows[0].Cells[0] != null || this.dataGridView_List_Love.SelectedRows[0].Cells[0] != null || this.dataGridView_List_Auto.SelectedRows[0].Cells[0] != null)
            {
                if (Select_DataGridView_Name_nums == 1)
                {
                    lrc_Clear();

                    SongIds_New_Update_All();

                    Thread.Sleep(50);
                    Playing_DataGridView_Name_nums = 1;
                    Start_Song(dataGridView_List_ALL);
                }
                else if (Select_DataGridView_Name_nums == 2)
                {
                    lrc_Clear();

                    SongIds_New_Update_All();

                    Thread.Sleep(50);
                    Playing_DataGridView_Name_nums = 2;
                    Start_Song(dataGridView_List_Love);
                }
                else if (Select_DataGridView_Name_nums == 3)
                {
                    lrc_Clear();

                    SongIds_New_Update_All();

                    Thread.Sleep(50);
                    Playing_DataGridView_Name_nums = 3;
                    Start_Song(dataGridView_List_Auto);
                }

                Clear_ALL_SelectRows_Selected();//清除选中行的选中效果

                //Encoding TxtEncoding = EncodingType.GetType(SongLrcPath);  //get encode from document . 
                //StreamReader sr = new StreamReader(SongLrcPath, TxtEncoding);

                //MessageBox.Show(sr.Read().ToString());



                button_Open_Windows_Picture_All();//切换桌面背景//在  已开启桌面写真的情况下


                Song_Change();//开启歌词


                this.dataGridView_List_ALL.Hide();
                this.dataGridView_List_Love.Hide();
                this.dataGridView_List_Auto.Hide();
            }
        }

        public static string Song_name;

        public void Start_Song(DataGridView List_Name)
        {
            try
            {
                SongNames_Temp = "";

                if (List_Name.Equals(dataGridView_List_Auto))
                {
                    Song_Src_Song = this.dataGridView_List_Auto.SelectedRows[0].Cells["song_src_auto"].Value.ToString().Trim();

                    Song_Names_Song = this.dataGridView_List_Auto.SelectedRows[0].Cells["song_name_auto"].Value.ToString().Trim();

                    SongNames_Temp = dataGridView_List_Auto.SelectedRows[0].Cells["song_name_auto"].Value.ToString().Trim();//获取歌手姓名  

                    WMP_Song_Play_Ids_Auto = Convert.ToInt32(dataGridView_List_Auto.SelectedRows[0].Cells["song_ids_auto"].Value);

                    dataGridView_List_Auto.SelectedRows[0].Selected = false;
                }
                else if (List_Name.Equals(dataGridView_List_Love))
                {
                    Song_Src_Song = this.dataGridView_List_Love.SelectedRows[0].Cells["song_src_love"].Value.ToString().Trim();

                    Song_Names_Song = this.dataGridView_List_Love.SelectedRows[0].Cells["song_name_love"].Value.ToString().Trim();

                    SongNames_Temp = dataGridView_List_Love.SelectedRows[0].Cells["song_name_love"].Value.ToString().Trim();//获取歌手姓名    

                    WMP_Song_Play_Ids_Love = Convert.ToInt32(dataGridView_List_Love.SelectedRows[0].Cells["song_ids_love"].Value);

                    dataGridView_List_Love.SelectedRows[0].Selected = false;
                }
                else if (List_Name.Equals(dataGridView_List_ALL))
                {
                    Song_Src_Song = dataGridView_List_ALL.SelectedRows[0].Cells["song_src_all"].Value.ToString().Trim();

                    Song_Names_Song = dataGridView_List_ALL.SelectedRows[0].Cells["song_name_all"].Value.ToString().Trim();

                    SongNames_Temp = dataGridView_List_ALL.SelectedRows[0].Cells["song_name_all"].Value.ToString().Trim();//获取歌手姓名   

                    WMP_Song_Play_Ids = Convert.ToInt32(dataGridView_List_ALL.SelectedRows[0].Cells["song_ids_all"].Value);

                    dataGridView_List_ALL.SelectedRows[0].Selected = false;
                }


                Get_FileMp3_Image_Info(Song_Src_Song);
                this.axWindowsMediaPlayer1.URL = Song_Src_Song;


                Song_Path_Temp_SongName_Frm = SongNames_Temp.IndexOf("-");
                //Song_Path_Temp_LastMp3_Frm = SongNames_Temp.LastIndexOf("mp3");

                Song_name = SongNames_Temp;

                if (Song_Path_Temp_SongName_Frm > 0)
                {
                    SongLrcPath = SongNames_Temp;
                    SongLrcPath = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"\song_lrc\" + SongLrcPath + ".mrc");


                    singer_Name = SongNames_Temp.Substring(0, Song_Path_Temp_SongName_Frm).Trim();

                    MV_name = SongNames_Temp + ".mkv";
                    //MV_name = MV_name.Substring(0, Song_Path_Temp_LastMp3_Frm).Trim() + "mkv";

                    backnum = 0;

                    Song_Path_Temp_SongName_Frm += 2;
                    label_Singer_Name.Text = singer_Name;
                    label_Song_Name.Text = Song_name.Substring(Song_Path_Temp_SongName_Frm, Song_name.Length - Song_Path_Temp_SongName_Frm);
                }
                else
                {
                    SongLrcPath = "";
                    singer_Name = "未知歌手";
                    MV_name = "";
                    backnum = 0;

                    Song_Path_Temp_SongName_Frm += 2;
                    label_Singer_Name.Text = singer_Name;
                    label_Song_Name.Text = Song_name.Substring(Song_Path_Temp_SongName_Frm, Song_name.Length - Song_Path_Temp_SongName_Frm);
                }



                label_Text_Clear();
            }
            catch
            {
                MessageBox.Show("请选择歌曲播放");
            }

            //Clear_SelectRows_Selected();
        }

        /// <summary>
        /// 清除当前选中行的选中效果  （已弃用）
        /// </summary>
        //public void Clear_SelectRows_Selected()
        //{

        //    dataGridView_List_ALL.SelectedRows[0].Selected = false;
        //    dataGridView_List_Love.SelectedRows[0].Selected = false;
        //    dataGridView_List_Auto.SelectedRows[0].Selected = false;

        //}

        #endregion



        public string Temps_String = "";
        #region 点击播放MV（MV_name）
        public void Song_Show_show()//播放音乐
        {
            //this.axWindowsMediaPlayer2.Dock = System.Windows.Forms.DockStyle.Top;
            this.axWindowsMediaPlayer2.Hide();
            axWindowsMediaPlayer1.Ctlcontrols.next();
            axWindowsMediaPlayer2.Ctlcontrols.pause();

            this.axWindowsMediaPlayer1.Show();
            Show_Song();

        }
        public void MV_Show_show()//播放MV
        {
            axWindowsMediaPlayer1.Ctlcontrols.pause();
            this.axWindowsMediaPlayer2.Dock = System.Windows.Forms.DockStyle.Fill;

            this.axWindowsMediaPlayer2.Show();

            Temps_String = this.axWindowsMediaPlayer1.URL;

            int index_other = Temps_String.LastIndexOf(@"\");



            index_other += 1;

            if (Temps_String.LastIndexOf(".mp3") > 0)
            {
                Temps_String = Temps_String.Substring(index_other, Temps_String.Length - index_other).Replace("mp3", "").Trim();
            }
            else
            {
                Temps_String = Temps_String.Substring(index_other, Temps_String.Length - index_other).Replace("flac", "").Trim();
            }

            Temps_String = Temps_String + "mkv";

            MV_name = Temps_String;

            if (File.Exists(string.Format(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"\song_MV\" + MV_name))))
            {
                this.axWindowsMediaPlayer2.URL = string.Format(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"\song_MV\" + MV_name));

                axWindowsMediaPlayer2.Ctlcontrols.play();

                this.axWindowsMediaPlayer1.Hide();
                Hide_Song();
            }
            else
            {
                MessageBox.Show("未在SongMv文件夹内找到相关资源，请放置同名MV资源至SongMv，视频格式为mkv");
                this.axWindowsMediaPlayer2.Hide();
            }
        }

        #endregion



        /// <summary>
        /// 切换背景图时，会刷新专辑图片（待解决）
        /// </summary>
        [DllImport("user32.dll", EntryPoint = "SystemParametersInfo")]
        public static extern int SystemParametersInfo(
            int uAction,
            int uParam,
            string lpvParam,
            int fuWinIni
            );
        #region   双位歌手以上 Time切换背景


        int pic_change_nums = 0;
        private void Two_Singer_Pic_Tick(object sender, EventArgs e)
        {
            pic_change_nums++;

            if (pic_change_nums == 1)
            {
                SingerPicPath = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"\singerPhoto\" + singer_Name + ".jpg");//获取歌手图片所在路径  

                if (File.Exists(SingerPicPath))
                {
                    BackgroundImage = new Bitmap(SingerPicPath);//提供路径将图片转化为矩阵图
                }
                else
                {
                    BackgroundImage = new Bitmap(PhotoPath);//提供路径将图片转化为矩阵图

                    Two_Singer_Pic.Stop();

                    singer_Name_2 = null;
                }
                if (button_Open_Windows_Picture.Text.Equals("关闭桌面写真"))
                {
                    try
                    {
                        SystemParametersInfo(20, 1, SingerPicPath, 1);//刷新桌面写真
                    }
                    catch
                    {
                        SystemParametersInfo(20, 1, PhotoPath, 1);//刷新桌面写真

                        Two_Singer_Pic.Stop();
                    }
                }

                if (Start_Song_Change_Image_Nums_3 != 1)
                {
                    if (File.Exists(SingerPicPath))
                    {
                        image = new Bitmap(SingerPicPath);//提供路径将图片转化为矩阵图  
                    }
                    else
                    {
                        image = global::Music_Player_Test.Properties.Resources.唱片3;

                        Open_SongPicPath_Nums = 0;

                        Two_Singer_Pic.Stop();
                    }

                    Change_GUI_Pic_Image_Size();
                }
            }
            else if (pic_change_nums == 8)
            {
                SingerPicPath = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"\singerPhoto\" + singer_Name_2 + ".jpg");//获取歌手图片所在路径  

                if (File.Exists(SingerPicPath))
                {
                    BackgroundImage = new Bitmap(SingerPicPath);//提供路径将图片转化为矩阵图                  
                }
                else
                {
                    BackgroundImage = new Bitmap(PhotoPath);//提供路径将图片转化为矩阵图

                    Two_Singer_Pic.Stop();

                    singer_Name_2 = null;
                }

                if (button_Open_Windows_Picture.Text.Equals("关闭桌面写真"))
                {
                    try
                    {
                        SystemParametersInfo(20, 1, SingerPicPath, 1);//刷新桌面写真
                    }
                    catch
                    {
                        SystemParametersInfo(20, 1, PhotoPath, 1);//刷新桌面写真

                        Two_Singer_Pic.Stop();
                    }
                }

                if (Start_Song_Change_Image_Nums_3 != 1)
                {
                    if (File.Exists(SingerPicPath))
                    {
                        image = new Bitmap(SingerPicPath);//提供路径将图片转化为矩阵图  
                    }
                    else
                    {
                        image = global::Music_Player_Test.Properties.Resources.唱片3;

                        Two_Singer_Pic.Stop();

                        Open_SongPicPath_Nums = 0;
                    }
                    Change_GUI_Pic_Image_Size();
                }
            }
            else if (pic_change_nums == 15)
            {
                pic_change_nums = 0;
            }



        }
        public void Two_Singer_Photo()
        {
            singer_temp = singer_Name;//记录当前歌手名
            if (singer_temp.IndexOf("、") > 0 && singer_temp.IndexOf("、") == singer_temp.LastIndexOf("、"))
            {
                //try
                //{
                string temp = singer_Name;
                singer_Name = singer_Name.Substring(0, singer_Name.IndexOf("、"));
                singer_Name_2 = temp.Substring(temp.IndexOf("、") + 1, temp.Length - temp.IndexOf("、") - 1);

                //检测路径是否有效
                SingerPicPath = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"\singerPhoto\" + singer_Name_2 + ".jpg");//获取歌手图片所在路径  
                if (File.Exists(SingerPicPath))
                {
                    SingerPicPath = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"\singerPhoto\" + singer_Name + ".jpg");//获取歌手图片所在路径  
                    if (File.Exists(SingerPicPath))
                    {

                        BackgroundImage = new Bitmap(SingerPicPath);


                        //判断是否是专辑模式
                        if (Start_Song_Change_Image_Nums_3 != 1)
                        {
                            image = new Bitmap(SingerPicPath);//提供路径将图片转化为矩阵图  

                            Change_GUI_Pic_Image_Size();
                        }


                        if (button_Open_Windows_Picture.Text.Equals("关闭桌面写真"))
                        {
                            SystemParametersInfo(20, 1, SingerPicPath, 1);//刷新桌面写真
                        }

                        Two_Singer_Pic.Start();
                    }
                    else
                    {
                        Two_Singer_Pic.Stop();
                    }
                }
                else
                {
                    BackgroundImage = new Bitmap(PhotoPath);

                    if (button_Open_Windows_Picture.Text.Equals("关闭桌面写真"))
                    {
                        SystemParametersInfo(20, 1, PhotoPath, 1);//刷新桌面写真                       
                    }

                    Two_Singer_Pic.Stop();
                    singer_Name_2 = null;
                }

                //}
                //catch
                //{

                //}


            }
            else
            {
                Two_Singer_Pic.Stop();

                singer_Name_2 = null;

                SingerPicPath = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"\singerPhoto\" + singer_Name + ".jpg");//获取歌手图片所在路径  
                if (File.Exists(SingerPicPath))
                {
                    //try
                    //{

                    BackgroundImage = new Bitmap(SingerPicPath);

                    if (button_Open_Windows_Picture.Text.Equals("关闭桌面写真"))
                    {
                        SystemParametersInfo(20, 1, SingerPicPath, 1);//刷新桌面写真
                    }
                    //}
                    //catch
                    //{

                    //}
                }
                else
                {
                    BackgroundImage = new Bitmap(PhotoPath);//提供路径将图片转化为矩阵图  

                    if (button_Open_Windows_Picture.Text.Equals("关闭桌面写真"))
                    {
                        SystemParametersInfo(20, 1, PhotoPath, 1);//刷新桌面写真                       
                    }
                }
            }
        }

        #endregion



        #region 检测MV播放状态  


        private void WMP_MV_State_Tick(object sender, EventArgs e)
        {
            if (axWindowsMediaPlayer2.playState == WMPLib.WMPPlayState.wmppsReady || axWindowsMediaPlayer2.playState == WMPLib.WMPPlayState.wmppsReady || axWindowsMediaPlayer2.playState == WMPLib.WMPPlayState.wmppsReady) //What The Fuck???卧槽???神奇的代码!!!So Crazy!!!
            {                                                                                                               //初步猜测线程异步执行，碰巧满足异步条件
                //此等于非彼等于，可能是多线程异步执行
                //因为调用Windows Media Player的过程中，似乎已经开启了一个多线程
                //两个条件的状态，可能并非同一线程，而是线程异步执行
                if (MV_Back_nums == 1)
                {
                    Music_Back_nums = 1;
                    //停顿2秒钟再重新播放
                    Thread.Sleep(50);
                    //重新播放

                    string temp = MV_name;
                    do
                    {
                        //刷新同步当前歌曲序号，防止序号不同步，MV播放至下下个
                        Song_MV_Ids_Change();

                        Back_MV();

                        if (temp.Equals(MV_name))
                        {
                            break;
                        }
                    } while (!File.Exists(string.Format(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"\song_MV\" + MV_name))));


                    this.axWindowsMediaPlayer2.URL = string.Format(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"\song_MV\" + MV_name));

                    num1_0_and_1_OnlyOnce = 0;

                    axWindowsMediaPlayer2.Ctlcontrols.play();


                }
                else if (MV_Next_nums == 1)
                {
                    Music_Next_nums = 1;
                    //停顿2秒钟再重新播放
                    Thread.Sleep(50);
                    //重新播放

                    string temp = MV_name;
                    do {
                        //刷新同步当前歌曲序号，防止序号不同步，MV播放至下下个
                        Song_MV_Ids_Change();

                        Next_MV();

                        if (temp.Equals(MV_name))
                        {
                            break;
                        }
                    } while (!File.Exists(string.Format(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"\song_MV\" + MV_name))));

                    this.axWindowsMediaPlayer2.URL = string.Format(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"\song_MV\" + MV_name));

                    num1_0_and_1_OnlyOnce = 0;

                    axWindowsMediaPlayer2.Ctlcontrols.play();

                }
            }
        }

        #endregion


        #region 双击播放音乐
        private void dataGridView_List_ALL_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            //清空Winodws Media Player控件的数据缓存(控件自带的播放列表，不需要使用该控件自带的播放列表)
            //axWindowsMediaPlayer1.currentPlaylist.clear();
            //axWindowsMediaPlayer2.currentPlaylist.clear();


            if (this.dataGridView_List_ALL.SelectedRows[0].Cells[0] != null)
            {
                lrc_Clear();
                SongIds_New_Update_All();
                Thread.Sleep(50);

                Playing_DataGridView_Name_nums = 1;
                Start_Song(dataGridView_List_ALL);

                Clear_ALL_SelectRows_Selected();//清除选中行的选中效果

                label_Text_Clear();//清理歌词

                button_Open_Windows_Picture_All();//切换桌面背景//在  已开启桌面写真的情况下

                Song_Change();//开启歌词

                this.dataGridView_List_ALL.Hide();
            }



        }

        private void dataGridView_List_Auto_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            //清空Winodws Media Player控件的数据缓存(控件自带的播放列表，不需要使用该控件自带的播放列表)
            //axWindowsMediaPlayer1.currentPlaylist.clear();
            //axWindowsMediaPlayer2.currentPlaylist.clear();



            if (this.dataGridView_List_Auto.SelectedRows[0].Cells[0] != null)
            {
                lrc_Clear();
                SongIds_New_Update_All();
                Thread.Sleep(50);

                Playing_DataGridView_Name_nums = 3;
                Start_Song(dataGridView_List_Auto);

                Clear_ALL_SelectRows_Selected();//清除选中行的选中效果

                label_Text_Clear();//清理歌词

                button_Open_Windows_Picture_All();//切换桌面背景//在  已开启桌面写真的情况下

                Song_Change();//开启歌词

                this.dataGridView_List_Auto.Hide();
            }


        }

        private void dataGridView_List_Love_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            //清空Winodws Media Player控件的数据缓存(控件自带的播放列表，不需要使用该控件自带的播放列表)
            //axWindowsMediaPlayer1.currentPlaylist.clear();
            //axWindowsMediaPlayer2.currentPlaylist.clear();




            if (this.dataGridView_List_Love.SelectedRows[0].Cells[0] != null)
            {
                lrc_Clear();
                SongIds_New_Update_All();
                Thread.Sleep(50);

                Playing_DataGridView_Name_nums = 2;
                Start_Song(dataGridView_List_Love);

                Clear_ALL_SelectRows_Selected();//清除选中行的选中效果

                label_Text_Clear();//清理歌词

                button_Open_Windows_Picture_All();//切换桌面背景//在  已开启桌面写真的情况下

                Song_Change();//开启歌词

                this.dataGridView_List_Love.Hide();
            }


        }


        #endregion

        public int Song_Ids_Temp = 1;//自动生成歌曲列表的id值
        public int Song_Ids_Temp_Love = 1;//自动生成歌曲列表的id值
        public int Song_Ids_Temp_Auto = 1;//自动生成歌曲列表的id值
        public static DataGridView Test_List_Name = null;//存储DataGridView来执行相关操作
        public string List_Rows_Cells_Name = "";//存储DataGridView的列名来执行相关操作
        public int List_Num = 3;//当前的歌单列表数量
        #region 歌单ids整理重新排列（Song_Ids_Temp  ,  Song_Ids_Temp_MV）（Test_List_Name）

        public void SongIds_New_Update_All()
        {
            for (int i = 0; i < List_Num; i++)
            {
                if (i == 0)
                {
                    Test_List_Name = dataGridView_List_ALL;
                    List_Rows_Cells_Name = "song_ids_all";
                }
                else if (i == 1)
                {
                    Test_List_Name = dataGridView_List_Love;
                    List_Rows_Cells_Name = "song_ids_love";
                }
                else if (i == 2)
                {
                    Test_List_Name = dataGridView_List_Auto;
                    List_Rows_Cells_Name = "song_ids_auto";
                }

                Change_Song_Ids_Num(Test_List_Name, List_Rows_Cells_Name);
            }
        }

        public void Change_Song_Ids_Num(DataGridView Test_List_Name, string List_Rows_Cells_Name)
        {
            Song_Ids_Temp = 1;
            for (int i = 0; i < Test_List_Name.Rows.Count - 1; i++)
            {
                if (Test_List_Name.Rows[i].Cells[List_Rows_Cells_Name].Value != null)
                {
                    Test_List_Name.Rows[i].Cells[List_Rows_Cells_Name].Value = Song_Ids_Temp;
                    Song_Ids_Temp++;
                }
            }
            Song_Ids_Temp = 1;
        }

        #endregion


        #region 歌单清除选中行的选中效果

        public void Clear_ALL_SelectRows_Selected()
        {
            foreach (DataGridViewRow r in dataGridView_List_ALL.SelectedRows)//删除选中的行 
            {
                r.Selected = false;
            }
            foreach (DataGridViewRow r in dataGridView_List_Love.SelectedRows)//删除选中的行 
            {
                r.Selected = false;
            }
            foreach (DataGridViewRow r in dataGridView_List_Auto.SelectedRows)//删除选中的行 
            {
                r.Selected = false;
            }
        }

        #endregion


        public static string[] All_Info_Path;//导入歌曲     临时存储选定的所有文件夹内 文件信息
        public string[] All_Song_Path;//导入歌曲     临时存储选定的所有文件夹内 MP3文件信息
        public int Nums_Song_Name_Index = 0;
        public String Temp_Song_Name;//存储临时的歌曲列表

        public string Song_Path_Temp = "";//存储临时生成的导入的后缀为mp3文件名
        public string Singer_Name_Temp = "";//存储临时提取的歌手名
        public string Song_Src_Paths = "";//存储临时提取的歌词
        public string Aldum_Temp;

        public static int Select_Add_ALL_Song_DataGridView_Num;
        public static int Select_Add_ALL_Song_DataGridView_Num_One;

        public int Song_Path_Temp_SongName;//传递临时生成的最后一个含有（"-"）歌曲文件名 所处的int位置  SongLrcPath.LastIndexOf("-");
        #region 导入歌单（Song_Ids_Temp  ,  Song_Ids_Temp_MV）（Select_DataGridView_Name_nums）

        /// <summary>
        /// 导入歌单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Song_Add_List_Click(object sender, EventArgs e)
        {
            Point p = new Point(0, 0);
            this.Song_List_Set.Show(Song_Add_List, p);
            //MessageBox.Show("请鼠标右击打开该按钮");
        }

        /// <summary>
        /// 选择文件夹
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>      
        private void Select_Song_Src_s(DataGridView List_Name)
        {

            Song_Ids_Temp_Love = dataGridView_List_Love.Rows.Count;
            Song_Ids_Temp_Auto = dataGridView_List_Auto.Rows.Count;

            All_Info_Path = new string[9999];
            All_Song_Path = new string[9999];

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = true;//该值确定是否可以选择多个文件
            dialog.Title = "请选择文件夹";
            dialog.Filter = "所有文件(*.*)|*.*";

            dialog.ShowDialog();

            All_Info_Path = dialog.FileNames;          
        }

        public void Add_Song_Into_List(DataGridView List_Name)
        {
            if (Select_Add_ALL_Song_DataGridView_Num == 1)
            {
                List_Name = dataGridView_List_ALL;
            }
            else if (Select_Add_ALL_Song_DataGridView_Num == 2)
            {
                List_Name = dataGridView_List_Love;
            }
            else if (Select_Add_ALL_Song_DataGridView_Num == 3)
            {
                List_Name = dataGridView_List_Auto;
            }

            foreach (String Song_Name in All_Info_Path)
            {
                if (Song_Name != null)
                {
                    Temp_Song_Name = Song_Name;
                    if (Temp_Song_Name.Substring(Temp_Song_Name.Length - 3, 3).Equals("mp3") || Temp_Song_Name.Substring(Temp_Song_Name.Length - 4, 4).Equals("flac"))//从指定的位置startIndex开始检索长度为length的子字符串
                    {
                        for (int i = 0; i < All_Song_Path.Length; i++)
                        {
                            if (All_Song_Path[i] == null)
                            {
                                All_Song_Path[i] = Song_Name;
                                break;
                            }
                        }
                    }
                }
            }
            string song_name_temp = "";
            for (int i = 0; i < All_Song_Path.Length; i++)
            {
                if (Select_Add_ALL_Song_DataGridView_Num == 1 || Select_Add_ALL_Song_DataGridView_Num_One == 1)
                {
                    if (All_Song_Path[i] != null)
                    {
                        if (All_Song_Path[i].ToString().Length > 0)
                        {
                            Nums_Song_Name_Index = All_Song_Path[i].LastIndexOf(@"\");
                            Nums_Song_Name_Index = Nums_Song_Name_Index + 1;

                            Song_Src_Paths = All_Song_Path[i];
                            Song_Path_Temp = All_Song_Path[i];
                            Song_Path_Temp = Song_Path_Temp.Substring(Nums_Song_Name_Index, Song_Path_Temp.Length - Nums_Song_Name_Index);
                            String temp_song = All_Song_Path[i];

                            Song_Path_Temp_SongName = All_Song_Path[i].LastIndexOf("-");

                            try
                            {
                                if (Song_Path_Temp_SongName > 0)
                                {
                                    Singer_Name_Temp = Song_Path_Temp;
                                    Singer_Name_Temp = Singer_Name_Temp.Substring(0, Singer_Name_Temp.IndexOf("-"));
                                    //Singer_Name_Temp = Singer_Name_Temp.Substring(Nums_Song_Name_Index, Singer_Name_Temp.Length - Nums_Song_Name_Index);

                                    if (Singer_Name_Temp.Length > 0 && Singer_Name_Temp != null)
                                    {
                                        dataGridView_List_ALL.Rows.Add();

                                        Singer_Name_Temp.Trim();

                                        dataGridView_List_ALL.Rows[i].Cells[0].Value = Singer_Name_Temp;//索引设置为-1，索引初始值为0，所以排首列

                                        Song_Path_Temp_SongName = Song_Path_Temp.LastIndexOf(".mp3");
                                        if (Song_Path_Temp.LastIndexOf(".mp3") <= 0)
                                        {
                                            Song_Path_Temp_SongName = Song_Path_Temp.LastIndexOf(".flac");
                                        }

                                        song_name_temp = Song_Path_Temp;
                                        song_name_temp = song_name_temp.Substring(0, Song_Path_Temp_SongName);

                                        //读取
                                        Folderdir = sh.NameSpace(System.IO.Path.GetDirectoryName(Song_Src_Paths));
                                        FolderItemitem = Folderdir.ParseName(System.IO.Path.GetFileName(Song_Src_Paths));
                                        Aldum_Temp = Folderdir.GetDetailsOf(FolderItemitem, 14);

                                        dataGridView_List_ALL.Rows[i].Cells[1].Value = song_name_temp;//设置DisplayMember属性显示为"全部"
                                        dataGridView_List_ALL.Rows[i].Cells[2].Value = Aldum_Temp;
                                        dataGridView_List_ALL.Rows[i].Cells[3].Value = Song_Src_Paths;
                                        dataGridView_List_ALL.Rows[i].Cells[4].Value = Song_Ids_Temp;
                                        Song_Ids_Temp++;
                                    }
                                }
                                else
                                {
                                    dataGridView_List_ALL.Rows.Add();
                                    dataGridView_List_ALL.Rows[i].Cells[0].Value = "未知歌手";//索引设置为-1，索引初始值为0，所以排首列
                                    dataGridView_List_ALL.Rows[i].Cells[1].Value = Song_Path_Temp;//设置DisplayMember属性显示为"全部"
                                    dataGridView_List_ALL.Rows[i].Cells[2].Value = "  ";
                                    dataGridView_List_ALL.Rows[i].Cells[3].Value = Song_Src_Paths;
                                    dataGridView_List_ALL.Rows[i].Cells[4].Value = Song_Ids_Temp;
                                    Song_Ids_Temp++;
                                }
                            }
                            catch
                            {

                            }
                        }
                    }
                }
                else if (Select_Add_ALL_Song_DataGridView_Num == 2 || Select_Add_ALL_Song_DataGridView_Num_One == 2)
                {
                    if (All_Song_Path[i] != null)
                    {
                        if (All_Song_Path[i].ToString().Length > 0)
                        {
                            Nums_Song_Name_Index = All_Song_Path[i].LastIndexOf(@"\");
                            Nums_Song_Name_Index = Nums_Song_Name_Index + 1;


                            Song_Src_Paths = All_Song_Path[i];
                            Song_Path_Temp = All_Song_Path[i];
                            Song_Path_Temp = Song_Path_Temp.Substring(Nums_Song_Name_Index, Song_Path_Temp.Length - Nums_Song_Name_Index);
                            String temp_song = All_Song_Path[i];

                            Song_Path_Temp_SongName = All_Song_Path[i].LastIndexOf("-");

                            try
                            {
                                if (Song_Path_Temp_SongName > 0)
                                {
                                    Singer_Name_Temp = Song_Path_Temp;
                                    Singer_Name_Temp = Singer_Name_Temp.Substring(0, Singer_Name_Temp.LastIndexOf("-"));
                                    //Singer_Name_Temp = Singer_Name_Temp.Substring(Nums_Song_Name_Index, Singer_Name_Temp.Length - Nums_Song_Name_Index);

                                    if (Singer_Name_Temp.Length > 0 && Singer_Name_Temp != null)
                                    {
                                        dataGridView_List_Love.Rows.Add();

                                        Singer_Name_Temp.Trim();
                                        dataGridView_List_Love.Rows[i].Cells[0].Value = Singer_Name_Temp;//索引设置为-1，索引初始值为0，所以排首列

                                        Song_Path_Temp_SongName = Song_Path_Temp.LastIndexOf(".mp3");
                                        if (Song_Path_Temp.LastIndexOf(".mp3") <= 0)
                                        {
                                            Song_Path_Temp_SongName = Song_Path_Temp.LastIndexOf(".flac");
                                        }

                                        song_name_temp = Song_Path_Temp;
                                        song_name_temp = song_name_temp.Substring(0, Song_Path_Temp_SongName);

                                        //读取
                                        Folderdir = sh.NameSpace(System.IO.Path.GetDirectoryName(Song_Src_Paths));
                                        FolderItemitem = Folderdir.ParseName(System.IO.Path.GetFileName(Song_Src_Paths));
                                        Aldum_Temp = Folderdir.GetDetailsOf(FolderItemitem, 14);

                                        dataGridView_List_Love.Rows[i].Cells[1].Value = song_name_temp;//设置DisplayMember属性显示为"全部"
                                        dataGridView_List_Love.Rows[i].Cells[2].Value = Aldum_Temp;
                                        dataGridView_List_Love.Rows[i].Cells[3].Value = Song_Src_Paths;
                                        dataGridView_List_Love.Rows[i].Cells[4].Value = Song_Ids_Temp_Love;
                                        Song_Ids_Temp_Love++;
                                    }
                                }
                                else
                                {
                                    dataGridView_List_Love.Rows.Add();
                                    dataGridView_List_Love.Rows[i].Cells[0].Value = "未知歌手";//索引设置为-1，索引初始值为0，所以排首列
                                    dataGridView_List_Love.Rows[i].Cells[1].Value = Song_Path_Temp;//设置DisplayMember属性显示为"全部"
                                    dataGridView_List_Love.Rows[i].Cells[2].Value = "  ";
                                    dataGridView_List_Love.Rows[i].Cells[3].Value = Song_Src_Paths;
                                    dataGridView_List_Love.Rows[i].Cells[4].Value = Song_Ids_Temp_Love;
                                    Song_Ids_Temp_Love++;
                                }
                            }
                            catch
                            {

                            }
                        }
                    }
                }
                else if (Select_Add_ALL_Song_DataGridView_Num == 3 || Select_Add_ALL_Song_DataGridView_Num_One == 3)
                {
                    if (All_Song_Path[i] != null)
                    {
                        if (All_Song_Path[i].ToString().Length > 0)
                        {
                            Nums_Song_Name_Index = All_Song_Path[i].LastIndexOf(@"\");
                            Nums_Song_Name_Index = Nums_Song_Name_Index + 1;

                            Song_Src_Paths = All_Song_Path[i];
                            Song_Path_Temp = All_Song_Path[i];
                            Song_Path_Temp = Song_Path_Temp.Substring(Nums_Song_Name_Index, Song_Path_Temp.Length - Nums_Song_Name_Index);
                            String temp_song = All_Song_Path[i];

                            Song_Path_Temp_SongName = All_Song_Path[i].LastIndexOf("-");

                            try
                            {
                                if (Song_Path_Temp_SongName > 0)
                                {
                                    Singer_Name_Temp = Song_Path_Temp;
                                    Singer_Name_Temp = Singer_Name_Temp.Substring(0, Singer_Name_Temp.LastIndexOf("-"));
                                    //Singer_Name_Temp = Singer_Name_Temp.Substring(Nums_Song_Name_Index, Singer_Name_Temp.Length - Nums_Song_Name_Index);

                                    if (Singer_Name_Temp.Length > 0 && Singer_Name_Temp != null)
                                    {
                                        dataGridView_List_Auto.Rows.Add();

                                        Singer_Name_Temp.Trim();
                                        dataGridView_List_Auto.Rows[i].Cells[0].Value = Singer_Name_Temp;//索引设置为-1，索引初始值为0，所以排首列

                                        Song_Path_Temp_SongName = Song_Path_Temp.LastIndexOf(".mp3");
                                        if (Song_Path_Temp.LastIndexOf(".mp3") <= 0)
                                        {
                                            Song_Path_Temp_SongName = Song_Path_Temp.LastIndexOf(".flac");
                                        }

                                        song_name_temp = Song_Path_Temp;
                                        song_name_temp = song_name_temp.Substring(0, Song_Path_Temp_SongName);

                                        //读取
                                        Folderdir = sh.NameSpace(System.IO.Path.GetDirectoryName(Song_Src_Paths));
                                        FolderItemitem = Folderdir.ParseName(System.IO.Path.GetFileName(Song_Src_Paths));
                                        Aldum_Temp = Folderdir.GetDetailsOf(FolderItemitem, 14);

                                        dataGridView_List_Auto.Rows[i].Cells[1].Value = song_name_temp;//设置DisplayMember属性显示为"全部"
                                        dataGridView_List_Auto.Rows[i].Cells[2].Value = Aldum_Temp;
                                        dataGridView_List_Auto.Rows[i].Cells[3].Value = Song_Src_Paths;
                                        dataGridView_List_Auto.Rows[i].Cells[4].Value = Song_Ids_Temp_Auto;
                                        Song_Ids_Temp_Auto++;
                                    }
                                }
                                else
                                {
                                    dataGridView_List_Auto.Rows.Add();
                                    dataGridView_List_Auto.Rows[i].Cells[0].Value = "未知歌手";//索引设置为-1，索引初始值为0，所以排首列
                                    dataGridView_List_Auto.Rows[i].Cells[1].Value = Song_Path_Temp;//设置DisplayMember属性显示为"全部"
                                    dataGridView_List_Auto.Rows[i].Cells[2].Value = "  ";
                                    dataGridView_List_Auto.Rows[i].Cells[3].Value = Song_Src_Paths;
                                    dataGridView_List_Auto.Rows[i].Cells[4].Value = Song_Ids_Temp_Auto;
                                    Song_Ids_Temp_Auto++;
                                }
                            }
                            catch
                            {

                            }
                        }
                    }
                }
            }

            SongIds_New_Update_All();

            foreach (DataGridViewRow r in dataGridView_List_Auto.Rows)//删除选中的行 
            {
                r.Selected = false;
                if (r == null)
                    dataGridView_List_Auto.Rows.Remove(r);
            }

            if (Select_Add_ALL_Song_DataGridView_Num == 1 || List_Name.Equals(dataGridView_List_ALL))
                MessageBox.Show("已导入本地音乐", "提示", MessageBoxButtons.OK);
            else if (Select_Add_ALL_Song_DataGridView_Num == 2 || List_Name.Equals(dataGridView_List_Love))
                MessageBox.Show("已导入我的收藏", "提示", MessageBoxButtons.OK);
            else if (Select_Add_ALL_Song_DataGridView_Num == 3 || List_Name.Equals(dataGridView_List_Auto))
                MessageBox.Show("已导入默认列表", "提示", MessageBoxButtons.OK);

            Select_Add_ALL_Song_DataGridView_Num = 0;
        }
        public void Add_Song_to_List_Name(string Singer_Name_Temp, string song_name_temp, string Song_Src_Paths, int Song_Ids_Temp)
        {

        }
        public void Add_Song_to_List_Name_Temp(string Singer_Name_Temp, string song_name_temp, string Song_Src_Paths, int Song_Ids_Temp)
        {

        }

        #endregion
        #region 查找所有音乐并指定歌单导入   （小部分弃用）（Select_DataGridView_Name_nums）

        /// <summary>
        /// 查找所有音乐
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Song_Find_ALL_List_Click(object sender, EventArgs e)
        {
            Point p = new Point(0, 0);
            this.Song_List_Add_ALL.Show(Song_Find_ALL_List, p);
            //MessageBox.Show("请鼠标右击打开该按钮");
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("是否导入？", "提示", MessageBoxButtons.YesNo);

            if (result == DialogResult.Yes)
            {
                Select_DataGridView_Name_nums = 1;
                Test_List_Name = dataGridView_List_ALL;

                Select_Add_ALL_Song_DataGridView_Num = 1;

                Find_All_Song();
            }

        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("是否导入？", "提示", MessageBoxButtons.YesNo);

            if (result == DialogResult.Yes)
            {
                Select_DataGridView_Name_nums = 2;
                Test_List_Name = dataGridView_List_Love;

                Select_Add_ALL_Song_DataGridView_Num = 2;

                Find_All_Song();
            }


        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("是否导入？", "提示", MessageBoxButtons.YesNo);

            if (result == DialogResult.Yes)
            {
                Select_DataGridView_Name_nums = 3;
                Test_List_Name = dataGridView_List_Auto;

                Select_Add_ALL_Song_DataGridView_Num = 3;

                Find_All_Song();
            }


        }

        public static string[] Finds_AllSong = new string[9999];
        public static string[] Finds_AllSong_End = new string[9999];

        public void Find_All_Song()//参数已过时，将弃用
        {


            //var t1 = new Thread(finds);
            //t1.Start();


            finds();
        }
        public void finds()
        {
            Song_Find_ALL_List.Text = "正在查找中";

            //获取本地硬盘驱动器 
            string[] localDrives = Directory.GetLogicalDrives();
            foreach (string eachDrive in localDrives)
            {
                if (!Directory.Exists(eachDrive))
                {
                    MessageBox.Show("文件夹不存在");
                }

                //遍历文件夹
                DirectoryInfo theFolder = new DirectoryInfo(eachDrive);
                FileInfo[] thefileInfo = theFolder.GetFiles("*.mp3", SearchOption.TopDirectoryOnly);
                foreach (FileInfo NextFile in thefileInfo)
                { //遍历文件
                    //listBox1.Items.Add(NextFile.FullName);
                    FindAllFiles(NextFile.FullName);
                }
                //遍历子文件夹

                Find_Like_AllFiles();


                ////遍历文件夹
                //theFolder = new DirectoryInfo(eachDrive);
                //thefileInfo = theFolder.GetFiles("*.flac", SearchOption.TopDirectoryOnly);
                //foreach (FileInfo NextFile in thefileInfo)
                //{ //遍历文件
                //    //listBox1.Items.Add(NextFile.FullName);
                //    FindAllFiles(NextFile.FullName);
                //}
                ////遍历子文件夹

                //Find_Like_AllFiles();


                DirectoryInfo[] dirInfo = theFolder.GetDirectories();

                foreach (DirectoryInfo NextFolder in dirInfo)
                {
                    try
                    {
                        FileInfo[] fileInfo = NextFolder.GetFiles("*.mp3", SearchOption.AllDirectories);

                        foreach (FileInfo NextFile in fileInfo)
                        {//遍历文件
                            //listBox1.Items.Add(NextFile.FullName);
                            FindAllFiles(NextFile.FullName);
                        }
                    }
                    catch
                    {

                    }
                }

                Find_Like_AllFiles();

                //dirInfo = theFolder.GetDirectories();
                //foreach (DirectoryInfo NextFolder in dirInfo)
                //{
                //    try
                //    {
                //        FileInfo[] fileInfo = NextFolder.GetFiles("*.flac", SearchOption.AllDirectories);
                //        foreach (FileInfo NextFile in fileInfo)
                //        {//遍历文件
                //            //listBox1.Items.Add(NextFile.FullName);
                //            FindAllFiles(NextFile.FullName);
                //        }
                //    }
                //    catch{ }
                //}
                //Find_Like_AllFiles();
            }



            All_Info_Path = new string[9999];
            All_Song_Path = new string[9999];

            foreach (string song_url in Finds_AllSong)
            {
                if (song_url != null)
                {
                    for (int i = 0; i < Finds_AllSong_End.Length; i++)
                    {
                        if (Finds_AllSong_End[i] == null)
                        {
                            Finds_AllSong_End[i] = song_url;
                            break;
                        }
                    }
                }
            }

            All_Info_Path = Finds_AllSong_End;

            Add_Song_Into_List(Test_List_Name);//方法已过时，将弃用

            Song_Find_ALL_List.Text = "查找本机所有歌曲";

            SongIds_New_Update_All();
        }

        public void FindAllFiles(string fileDicPath)
        {
            for (int i = 0; i < Finds_AllSong.Length; i++)
            {
                if (Finds_AllSong[i] == null)
                {
                    if (fileDicPath != null)
                    {
                        Finds_AllSong[i] = fileDicPath;
                        break;
                    }
                }
            }
        }

        string Song_Path_1;
        string Song_Path_2;


        public void Find_Like_AllFiles()//删除重复的歌曲
        {
            for (int i = 0; i < Finds_AllSong.Length; i++)
            {
                if (Finds_AllSong[i] != null)
                {

                    Nums_Song_Name_Index = Finds_AllSong[i].LastIndexOf(@"\");
                    Nums_Song_Name_Index = Nums_Song_Name_Index + 1;

                    Song_Path_1 = Finds_AllSong[i];
                    Song_Path_1 = Song_Path_1.Substring(Nums_Song_Name_Index, Song_Path_1.Length - Nums_Song_Name_Index);

                    for (int j = 0; j < Finds_AllSong.Length; j++)
                    {
                        if (Finds_AllSong[j] != null)
                        {
                            if (i != j)
                            {


                                Nums_Song_Name_Index = Finds_AllSong[j].LastIndexOf(@"\");
                                Nums_Song_Name_Index = Nums_Song_Name_Index + 1;

                                Song_Path_2 = Finds_AllSong[j];
                                Song_Path_2 = Song_Path_2.Substring(Nums_Song_Name_Index, Song_Path_2.Length - Nums_Song_Name_Index);

                                if (Song_Path_1.Equals(Song_Path_2))
                                {
                                    Finds_AllSong[j] = null;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        #endregion



        #region 快捷菜单删除DataGrifView选中的音乐   （部分弃用）（Test_List_Name）（Select_DataGridView_Name_nums）

        private void 删除此音乐ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("确认删除？", "提示", MessageBoxButtons.YesNo);

            if (DialogResult.Yes == result)
            {

                Test_List_Name = null;

                if (Select_DataGridView_Name_nums == 1)
                    Test_List_Name = dataGridView_List_ALL;
                else if (Select_DataGridView_Name_nums == 2)
                    Test_List_Name = dataGridView_List_Love;
                else if (Select_DataGridView_Name_nums == 3)
                    Test_List_Name = dataGridView_List_Auto;

                foreach (DataGridViewRow r in Test_List_Name.SelectedRows)//删除选中的行 
                {
                    r.Selected = false;
                    if (!r.IsNewRow)
                        Test_List_Name.Rows.Remove(r);
                }

                SongIds_New_Update_All();

                Test_List_Name = null;


                Clear_ALL_SelectRows_Selected();

            }
        }

        #endregion
        #region 快捷菜单添加DataGrifView音乐   （部分弃用）（使用空表格仓库）（Select_DataGridView_Name_nums）

        public int Is_Not_Have_Song_Src_dataGridView_List_ALL(string rows)
        {
            for (int i = 0; i < this.dataGridView_List_ALL.Rows.Count - 1; i++)
            {
                if (this.dataGridView_List_ALL.Rows[i].Cells["song_name_all"].Value.Equals(rows))
                {
                    MessageBox.Show(this.dataGridView_List_ALL.Rows[i].Cells["song_name_all"].Value + "已存在", "提示", MessageBoxButtons.OK);
                    return 1;
                }
            }
            return 0;
        }
        public int Is_Not_Have_Song_Src_dataGridView_List_Love(string rows)
        {
            for (int i = 0; i < this.dataGridView_List_Love.Rows.Count - 1; i++)
            {
                if (this.dataGridView_List_Love.Rows[i].Cells["song_name_love"].Value.Equals(rows))
                {
                    MessageBox.Show(this.dataGridView_List_Love.Rows[i].Cells["song_name_love"].Value + "已存在", "提示", MessageBoxButtons.OK);
                    return 1;
                }
            }
            return 0;
        }
        public int Is_Not_Have_Song_Src_dataGridView_List_Auto(string rows)
        {
            for (int i = 0; i < this.dataGridView_List_Auto.Rows.Count - 1; i++)
            {
                if (this.dataGridView_List_Auto.Rows[i].Cells["song_name_auto"].Value.Equals(rows))
                {
                    MessageBox.Show(this.dataGridView_List_Auto.Rows[i].Cells["song_name_auto"].Value + "已存在", "提示", MessageBoxButtons.OK);
                    return 1;
                }
            }
            return 0;
        }

        private void 本地音乐ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("确认添加到本地音乐？", "提示", MessageBoxButtons.YesNo);

            if (DialogResult.Yes == result)
            {
                Song_Ids_Temp = dataGridView_List_ALL.Rows.Count;
                Song_Ids_Temp_Love = dataGridView_List_Love.Rows.Count;
                Song_Ids_Temp_Auto = dataGridView_List_Auto.Rows.Count;

                if (Select_DataGridView_Name_nums == 2)
                {
                    foreach (DataGridViewRow r in dataGridView_List_Love.SelectedRows)
                    {
                        if (!r.IsNewRow)
                        {
                            int nums = Is_Not_Have_Song_Src_dataGridView_List_ALL(r.Cells[1].Value.ToString());

                            if (nums == 0)
                            {
                                dataGridView_List_ALL.Rows.Add();
                                dataGridView_List_ALL.Rows[Song_Ids_Temp - 1].Cells[0].Value = r.Cells[0].Value;
                                dataGridView_List_ALL.Rows[Song_Ids_Temp - 1].Cells[1].Value = r.Cells[1].Value;
                                dataGridView_List_ALL.Rows[Song_Ids_Temp - 1].Cells[2].Value = r.Cells[2].Value;
                                dataGridView_List_ALL.Rows[Song_Ids_Temp - 1].Cells[3].Value = r.Cells[3].Value;
                                dataGridView_List_ALL.Rows[Song_Ids_Temp - 1].Cells[4].Value = Song_Ids_Temp;

                                Song_Ids_Temp++;
                            }
                        }
                        r.Selected = false;//清除选中效果
                    }
                }
                else if (Select_DataGridView_Name_nums == 3)
                {
                    foreach (DataGridViewRow r in dataGridView_List_Auto.SelectedRows)
                    {
                        if (!r.IsNewRow)
                        {
                            int nums = Is_Not_Have_Song_Src_dataGridView_List_ALL(r.Cells[1].Value.ToString());

                            if (nums == 0)
                            {
                                dataGridView_List_ALL.Rows.Add();
                                dataGridView_List_ALL.Rows[Song_Ids_Temp - 1].Cells[0].Value = r.Cells[0].Value;
                                dataGridView_List_ALL.Rows[Song_Ids_Temp - 1].Cells[1].Value = r.Cells[1].Value;
                                dataGridView_List_ALL.Rows[Song_Ids_Temp - 1].Cells[2].Value = r.Cells[2].Value;
                                dataGridView_List_ALL.Rows[Song_Ids_Temp - 1].Cells[3].Value = r.Cells[3].Value;
                                dataGridView_List_ALL.Rows[Song_Ids_Temp - 1].Cells[4].Value = Song_Ids_Temp;

                                Song_Ids_Temp++;
                            }
                        }
                        r.Selected = false;
                    }
                }
                else
                {
                    MessageBox.Show("歌曲已存在", "提示");
                }

                SongIds_New_Update_All();

                Clear_ALL_SelectRows_Selected();//清除选中行的选中效果

            }
        }

        private void 我的收藏ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("确认添加到我的收藏？", "提示", MessageBoxButtons.YesNo);

            if (DialogResult.Yes == result)
            {

                Song_Ids_Temp = dataGridView_List_ALL.Rows.Count;
                Song_Ids_Temp_Love = dataGridView_List_Love.Rows.Count;
                Song_Ids_Temp_Auto = dataGridView_List_Auto.Rows.Count;

                if (Select_DataGridView_Name_nums == 1)
                {
                    foreach (DataGridViewRow r in dataGridView_List_ALL.SelectedRows)
                    {
                        if (!r.IsNewRow)
                        {
                            int nums = Is_Not_Have_Song_Src_dataGridView_List_Love(r.Cells[1].Value.ToString());

                            if (nums == 0)
                            {
                                dataGridView_List_Love.Rows.Add();
                                dataGridView_List_Love.Rows[Song_Ids_Temp_Love - 1].Cells[0].Value = r.Cells[0].Value;
                                dataGridView_List_Love.Rows[Song_Ids_Temp_Love - 1].Cells[1].Value = r.Cells[1].Value;
                                dataGridView_List_Love.Rows[Song_Ids_Temp_Love - 1].Cells[2].Value = r.Cells[2].Value;
                                dataGridView_List_Love.Rows[Song_Ids_Temp_Love - 1].Cells[3].Value = r.Cells[3].Value;
                                dataGridView_List_Love.Rows[Song_Ids_Temp_Love - 1].Cells[4].Value = Song_Ids_Temp_Love;

                                Song_Ids_Temp_Love++;
                            }
                        }
                        r.Selected = false;
                    }
                }

                else if (Select_DataGridView_Name_nums == 3)
                {
                    foreach (DataGridViewRow r in dataGridView_List_Auto.SelectedRows)
                    {
                        if (!r.IsNewRow)
                        {
                            int nums = Is_Not_Have_Song_Src_dataGridView_List_Love(r.Cells[1].Value.ToString());

                            if (nums == 0)
                            {
                                dataGridView_List_Love.Rows.Add();
                                dataGridView_List_Love.Rows[Song_Ids_Temp_Love - 1].Cells[0].Value = r.Cells[0].Value;
                                dataGridView_List_Love.Rows[Song_Ids_Temp_Love - 1].Cells[1].Value = r.Cells[1].Value;
                                dataGridView_List_Love.Rows[Song_Ids_Temp_Love - 1].Cells[2].Value = r.Cells[2].Value;
                                dataGridView_List_Love.Rows[Song_Ids_Temp_Love - 1].Cells[3].Value = r.Cells[3].Value;
                                dataGridView_List_Love.Rows[Song_Ids_Temp_Love - 1].Cells[4].Value = Song_Ids_Temp_Love;

                                Song_Ids_Temp_Love++;
                            }
                        }
                        r.Selected = false;
                    }
                }
                else
                {
                    MessageBox.Show("歌曲已存在", "提示");
                }

                SongIds_New_Update_All();

                Clear_ALL_SelectRows_Selected();//清除选中行的选中效果

            }
        }

        private void 默认列表ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("确认添加到默认列表？", "提示", MessageBoxButtons.YesNo);

            if (DialogResult.Yes == result)
            {

                Song_Ids_Temp = dataGridView_List_ALL.Rows.Count;
                Song_Ids_Temp_Love = dataGridView_List_Love.Rows.Count;
                Song_Ids_Temp_Auto = dataGridView_List_Auto.Rows.Count;

                if (Select_DataGridView_Name_nums == 1)
                {
                    foreach (DataGridViewRow r in dataGridView_List_ALL.SelectedRows)
                    {
                        if (!r.IsNewRow)
                        {
                            int nums = Is_Not_Have_Song_Src_dataGridView_List_Auto(r.Cells[1].Value.ToString());

                            if (nums == 0)
                            {
                                dataGridView_List_Auto.Rows.Add();
                                dataGridView_List_Auto.Rows[Song_Ids_Temp_Auto - 1].Cells[0].Value = r.Cells[0].Value;
                                dataGridView_List_Auto.Rows[Song_Ids_Temp_Auto - 1].Cells[1].Value = r.Cells[1].Value;
                                dataGridView_List_Auto.Rows[Song_Ids_Temp_Auto - 1].Cells[2].Value = r.Cells[2].Value;
                                dataGridView_List_Auto.Rows[Song_Ids_Temp_Auto - 1].Cells[3].Value = r.Cells[3].Value;
                                dataGridView_List_Auto.Rows[Song_Ids_Temp_Auto - 1].Cells[4].Value = Song_Ids_Temp_Auto;

                                Song_Ids_Temp_Auto++;
                            }
                        }
                        r.Selected = false;
                    }
                }
                else if (Select_DataGridView_Name_nums == 2)
                {
                    foreach (DataGridViewRow r in dataGridView_List_Love.SelectedRows)
                    {
                        if (!r.IsNewRow)
                        {
                            int nums = Is_Not_Have_Song_Src_dataGridView_List_Auto(r.Cells[1].Value.ToString());

                            if (nums == 0)
                            {
                                dataGridView_List_Auto.Rows.Add();
                                dataGridView_List_Auto.Rows[Song_Ids_Temp_Auto - 1].Cells[0].Value = r.Cells[0].Value;
                                dataGridView_List_Auto.Rows[Song_Ids_Temp_Auto - 1].Cells[1].Value = r.Cells[1].Value;
                                dataGridView_List_Auto.Rows[Song_Ids_Temp_Auto - 1].Cells[2].Value = r.Cells[2].Value;
                                dataGridView_List_Auto.Rows[Song_Ids_Temp_Auto - 1].Cells[3].Value = r.Cells[3].Value;
                                dataGridView_List_Auto.Rows[Song_Ids_Temp_Auto - 1].Cells[4].Value = Song_Ids_Temp_Auto;

                                Song_Ids_Temp_Auto++;

                            }
                        }
                        r.Selected = false;
                    }
                }
                else
                {
                    MessageBox.Show("歌曲已存在", "提示");
                }


                SongIds_New_Update_All();

                Clear_ALL_SelectRows_Selected();//清除选中行的选中效果

            }
        }



        #endregion


        #region 快捷菜单导入音乐至歌曲列表 （Select_DataGridView_Name_nums）
        /// <summary>
        /// 开启多线程会造成歌手名信息随机缺失
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>


        /// <summary>
        /// 本地音乐
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Add_List_All_Click(object sender, EventArgs e)
        {
            Select_Add_ALL_Song_DataGridView_Num_One = 1;

            Select_DataGridView_Name_nums = 1;

            Select_Song_Src_s(dataGridView_List_ALL);

            //var t1 = new Thread(Thread_Add_List_All_Click);
            //t1.Start();

            Add_Song_Into_List(dataGridView_List_ALL);
            Select_Add_ALL_Song_DataGridView_Num_One = 1;
            SongIds_New_Update_All();
            Select_Add_ALL_Song_DataGridView_Num_One = 0;
        }
        public void Thread_Add_List_All_Click()
        {
            Add_Song_Into_List(dataGridView_List_ALL);

            Select_Add_ALL_Song_DataGridView_Num_One = 1;

            SongIds_New_Update_All();

            Select_Add_ALL_Song_DataGridView_Num_One = 0;
        }

        /// <summary>
        /// 我的收藏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Add_List_Love_Click(object sender, EventArgs e)
        {
            Select_Add_ALL_Song_DataGridView_Num_One = 2;

            Select_DataGridView_Name_nums = 2;

            Select_Song_Src_s(dataGridView_List_Love);

            //var t1 = new Thread(Thread_Add_List_Love_Click);
            //t1.Start();

            Add_Song_Into_List(dataGridView_List_Love);
            Select_Add_ALL_Song_DataGridView_Num_One = 2;
            SongIds_New_Update_All();
            Select_Add_ALL_Song_DataGridView_Num_One = 0;
        }
        public void Thread_Add_List_Love_Click()
        {
            Add_Song_Into_List(dataGridView_List_Love);

            Select_Add_ALL_Song_DataGridView_Num_One = 2;

            SongIds_New_Update_All();

            Select_Add_ALL_Song_DataGridView_Num_One = 0;
        }

        /// <summary>
        /// 默认列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Add_List_Auto_Click(object sender, EventArgs e)
        {
            Select_Add_ALL_Song_DataGridView_Num_One = 3;

            Select_DataGridView_Name_nums = 3;

            Select_Song_Src_s(dataGridView_List_Auto);

            //var t1 = new Thread(Thread_Add_List_Auto_Click);
            //t1.Start();

            Add_Song_Into_List(dataGridView_List_Auto);
            Select_Add_ALL_Song_DataGridView_Num_One = 3;
            SongIds_New_Update_All();
            Select_Add_ALL_Song_DataGridView_Num_One = 0;
        }
        public void Thread_Add_List_Auto_Click()
        {
            Add_Song_Into_List(dataGridView_List_Auto);

            Select_Add_ALL_Song_DataGridView_Num_One = 3;

            SongIds_New_Update_All();

            Select_Add_ALL_Song_DataGridView_Num_One = 0;
        }

        #endregion 


        #region 清空歌曲列表内所有音乐

        /// <summary>
        /// 右键快捷菜单删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Delete_List_All_Click(object sender, EventArgs e)
        {

            dataGridView_List_ALL.Rows.Clear();
            MessageBox.Show("已清空本地音乐", "提示", MessageBoxButtons.OK);
        }

        private void Delete_List_Love_Click(object sender, EventArgs e)
        {
            dataGridView_List_Love.Rows.Clear();
            MessageBox.Show("已清空我的收藏", "提示", MessageBoxButtons.OK);
        }

        private void Delete_List_Auto_Click(object sender, EventArgs e)
        {
            dataGridView_List_Auto.Rows.Clear();
            MessageBox.Show("已清空默认列表", "提示", MessageBoxButtons.OK);
        }


        /// <summary>
        /// 点击显示删除歌单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Song_Delete_List_Click(object sender, EventArgs e)
        {
            Point p = new Point(0, 0);
            this.Song_List_Delete.Show(Song_Delete_List, p);
            //MessageBox.Show("请鼠标右击打开该按钮");
        }

        #endregion


        public DataGridView Save_Load_List_Name = null;
        #region 歌单的保存

        //实例化一个文件流--->与写入文件相关联
        //静态读取资源文件会一直占用，导致只能写入不能导出，出现文件内容清空
        private FileStream FS_List_Save = null;
        private StreamWriter SW_List = null;//写入 
        private StreamReader SR_List = null;//读取

        public void Save_DataGridView()
        {
            Save_Data_ALL_List();

            FS_List_Save = null;

            SW_List = null;

        }
        public void Save_Data_ALL_List()
        {

            string temp = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"\本地音乐.ini");
            Clear_File_Info(temp);

            FS_List_Save = new FileStream(temp, FileMode.Create);
            Save_Load_List_Name = dataGridView_List_ALL;
            SW_List = new StreamWriter(FS_List_Save);//无法静态
            Write_Song_Info(Save_Load_List_Name, FS_List_Save);

            FS_List_Save = null;
            Save_Load_List_Name = null;
            SW_List = null;


            temp = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"\我的收藏.ini");
            Clear_File_Info(temp);

            FS_List_Save = new FileStream(temp, FileMode.Create);
            Save_Load_List_Name = dataGridView_List_Love;
            SW_List = new StreamWriter(FS_List_Save);//无法静态
            Write_Song_Info(Save_Load_List_Name, FS_List_Save);

            FS_List_Save = null;
            Save_Load_List_Name = null;
            SW_List = null;


            temp = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"\默认列表.ini");
            Clear_File_Info(temp);

            FS_List_Save = new FileStream(temp, FileMode.Create);
            Save_Load_List_Name = dataGridView_List_Auto;
            SW_List = new StreamWriter(FS_List_Save);//无法静态
            Write_Song_Info(Save_Load_List_Name, FS_List_Save);

            FS_List_Save = null;
            Save_Load_List_Name = null;
            SW_List = null;
        }
        public void Clear_File_Info(string FullName)
        {
            //先清空文件信息
            FileStream fs = new FileStream(FullName, FileMode.Create);//清空此文件的数据
            fs.Flush();
            fs.Close();
        }

        public void Write_Song_Info(DataGridView Save_Load_List_Name, FileStream FS_List)
        {

            //开始写入
            if (Save_Load_List_Name.Rows.Count > 0) //if有新的行可以插入
            {
                //因为DataGridView最后一行为空，所以减一
                SW_List.WriteLine(Save_Load_List_Name.Rows.Count - 1);
                for (int i = 0; i < Save_Load_List_Name.Rows.Count - 1; i++)
                {
                    //如果某一列数据为空，就写入""，因为空对象不能调用tostring()；
                    if (Save_Load_List_Name.Rows[i].Cells[0].Value != null)
                        SW_List.WriteLine(Save_Load_List_Name.Rows[i].Cells[0].Value.ToString());

                    if (Save_Load_List_Name.Rows[i].Cells[1].Value != null)
                        SW_List.WriteLine(Save_Load_List_Name.Rows[i].Cells[1].Value.ToString());

                    if (Save_Load_List_Name.Rows[i].Cells[2].Value != null)
                        SW_List.WriteLine(Save_Load_List_Name.Rows[i].Cells[2].Value.ToString());

                    if (Save_Load_List_Name.Rows[i].Cells[3].Value != null)
                        SW_List.WriteLine(Save_Load_List_Name.Rows[i].Cells[3].Value.ToString());

                    if (Save_Load_List_Name.Rows[i].Cells[4].Value != null)
                        SW_List.WriteLine(Save_Load_List_Name.Rows[i].Cells[4].Value.ToString());
                }
                //清空缓冲区
                SW_List.Flush();
                //关闭流
                SW_List.Close();
                FS_List.Close();
            }




        }

        #endregion
        #region 歌单的读取

        /// <summary>
        /// 读取歌单
        /// </summary>
        public void Load_DataGridView()
        {
            try
            {
                Load_Data_ALL_D_Grid_View();

            }
            catch
            {

            }
        }
        public void Load_Data_ALL_D_Grid_View()
        {
            string temp = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"\本地音乐.ini");

            FS_List_Save = new FileStream(temp, FileMode.Open);
            SR_List = new StreamReader(FS_List_Save);
            Load_Data_ALL(dataGridView_List_ALL, FS_List_Save, SR_List);

            FS_List_Save = null;
            SR_List = null;


            temp = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"\我的收藏.ini");

            FS_List_Save = new FileStream(temp, FileMode.Open);
            SR_List = new StreamReader(FS_List_Save);
            Load_Data_ALL(dataGridView_List_Love, FS_List_Save, SR_List);

            FS_List_Save = null;
            SR_List = null;


            temp = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"\默认列表.ini");

            FS_List_Save = new FileStream(temp, FileMode.Open);
            SR_List = new StreamReader(FS_List_Save);
            Load_Data_ALL(dataGridView_List_Auto, FS_List_Save, SR_List);

            FS_List_Save = null;
            SR_List = null;
        }

        public void Load_Data_ALL(DataGridView Save_Load_List_Name, FileStream FS_List_Save, StreamReader SR_List)
        {
            try
            {
                //开始读取
                if (Save_Load_List_Name.Rows.Count > 0)//if有新的行可以插入
                {
                    int RowCount = int.Parse(SR_List.ReadLine());
                    //判断如果大于读取的行数小于等于当前dataGridView的行数，就不add行，否则add行
                    if (RowCount < Save_Load_List_Name.Rows.Count || RowCount == Save_Load_List_Name.Rows.Count)
                    {
                        for (int i = 0; i <= RowCount - 1; i++)
                        {
                            Save_Load_List_Name.Rows[i].Cells[0].Value = SR_List.ReadLine();
                            Save_Load_List_Name.Rows[i].Cells[1].Value = SR_List.ReadLine();
                            Save_Load_List_Name.Rows[i].Cells[2].Value = SR_List.ReadLine();
                            Save_Load_List_Name.Rows[i].Cells[3].Value = SR_List.ReadLine();
                            Save_Load_List_Name.Rows[i].Cells[4].Value = SR_List.ReadLine();
                        }
                    }
                    else
                    {
                        for (int i = 0; i <= RowCount - 1; i++)
                        {
                            Save_Load_List_Name.Rows.Add();
                            Save_Load_List_Name.Rows[i].Cells[0].Value = SR_List.ReadLine();
                            Save_Load_List_Name.Rows[i].Cells[1].Value = SR_List.ReadLine();
                            Save_Load_List_Name.Rows[i].Cells[2].Value = SR_List.ReadLine();
                            Save_Load_List_Name.Rows[i].Cells[3].Value = SR_List.ReadLine();
                            Save_Load_List_Name.Rows[i].Cells[4].Value = SR_List.ReadLine();

                        }
                    }
                    //关闭流
                    SR_List.Close();
                    FS_List_Save.Close();
                }

            }
            catch
            {

            }

        }

        #endregion



        #region 自动生成配置文件夹

        public void Create_songInfo()
        {

            //System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"");
            System.IO.DirectoryInfo di1 = new System.IO.DirectoryInfo(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"\singerPhoto"));
            System.IO.DirectoryInfo di2 = new System.IO.DirectoryInfo(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"\song_lrc"));
            System.IO.DirectoryInfo di3 = new System.IO.DirectoryInfo(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"\song_MV"));
            System.IO.DirectoryInfo di4 = new System.IO.DirectoryInfo(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"\songnew"));
            System.IO.DirectoryInfo di5 = new System.IO.DirectoryInfo(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"\singer_songPhoto"));
            //di.Create();
            di1.Create();
            di2.Create();
            di3.Create();
            di4.Create();
            di5.Create();

            string Path_List_1 = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"\本地音乐.ini");
            //判断文件是否存在，没有则创建。
            if (!System.IO.File.Exists(Path_List_1))
            {
                FileStream stream = System.IO.File.Create(Path_List_1);
                stream.Close();
                stream.Dispose();
            }

            string Path_List_2 = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"\我的收藏.ini");
            //判断文件是否存在，没有则创建。
            if (!System.IO.File.Exists(Path_List_2))
            {
                FileStream stream = System.IO.File.Create(Path_List_2);
                stream.Close();
                stream.Dispose();
            }

            string Path_List_3 = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"\默认列表.ini");
            //判断文件是否存在，没有则创建。
            if (!System.IO.File.Exists(Path_List_3))
            {
                FileStream stream = System.IO.File.Create(Path_List_3);
                stream.Close();
                stream.Dispose();
            }

            string Path_List_4 = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"\歌词字体设置.ini");
            //判断文件是否存在，没有则创建。
            if (!System.IO.File.Exists(Path_List_4))
            {
                FileStream stream = System.IO.File.Create(Path_List_4);
                stream.Close();
                stream.Dispose();
            }

            string Path_List_5 = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"\歌词颜色设置.ini");
            //判断文件是否存在，没有则创建。
            if (!System.IO.File.Exists(Path_List_5))
            {
                FileStream stream = System.IO.File.Create(Path_List_5);
                stream.Close();
                stream.Dispose();
            }

            string Path_List_6 = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"\XML设置.ini");
            //判断文件是否存在，没有则创建。
            if (!System.IO.File.Exists(Path_List_6))
            {
                FileStream stream = System.IO.File.Create(Path_List_6);
                stream.Close();
                stream.Dispose();
            }
        }

        #endregion


        public Process Delete_All_Info = new Process();//获取程序路径
        #region 卸载软件
        /// <summary>
        /// 卸载软件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //卸载软件
        private void Delete_All_Click(object sender, EventArgs e)
        {

            //获取基目录，它由程序集冲突解决程序用来探测程序集。
            Delete_All_Info.StartInfo.FileName = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + "unins000.exe");//软件安装路径

            DialogResult result = MessageBox.Show("是否卸载墨智音乐?", "卸载", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                try
                {
                    Delete_All_Info.Start(); //启动选择的exe文件
                    this.Close();
                }
                catch
                {
                    MessageBox.Show("卸载失败");
                }
            }
        }


        #endregion




        public double widths;
        public double heights;
        #region 最大化，最小化，退出按钮  以及事件

        public void LRC_Time_Stop()
        {
            Song_Lrc_Times.Stop();
            Show_Lrc_Text.Stop();

            Photo_BackGround_Time.Stop();
        }

        public void LRC_Time_Start()
        {
            Song_Lrc_Times.Start();
            Show_Lrc_Text.Start();

            Photo_BackGround_Time.Start();
        }

        public void LRC_Hide_Max_Minimized()
        {
            this.panel_Song_LRC.Hide();
            this.panel_Button.Hide();

            if (Open_Singer_Image == 1)
            {
                panel_PictureBox_All.Hide();
            }

            panel_Windows.Hide();

            panel_Button.Hide();

            pictureBox_Next.Hide();

            pictureBox_Back.Hide();

            //button_Font_User.Hide();

            button_WMP_Time_Change.Hide();
        }

        public void LRC_Show_Max_Minimized()
        {
            this.panel_Song_LRC.Show();
            this.panel_Button.Show();

            if (Open_Singer_Image == 1)
            {
                panel_PictureBox_All.Show();
            }

            panel_Windows.Show();

            panel_Button.Show();

            pictureBox_Next.Show();

            pictureBox_Back.Show();

            //button_Font_User.Show();

            button_WMP_Time_Change.Show();
        }


        private void Exit_Button_Click_1(object sender, EventArgs e)
        {
            Save_DataGridView();
            Save_Set_Info();//保存配置文件

            SystemParametersInfo(20, 1, wallpaper_path, 1);//改回原来的的桌面背景

            // 恢复此线程曾经阻止的系统休眠和屏幕关闭。
            SystemSleep.RestoreForCurrentThread();

            this.Close();
        }
        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            SystemParametersInfo(20, 1, wallpaper_path, 1);//改回原来的的桌面背景

            Save_DataGridView();
            Save_Set_Info();//保存配置文件

            //SystemParametersInfo(20, 1, wallpaper_path, 1);//改回原来的的桌面背景

            //this.Close();
        }

        public void panel_PictureBox_All_Location_Max()
        {
            double nums_1 = 0;
            if (Windows_Heigh != 1)
            {
                nums_1 = (500 * Windows_Heigh - 500 * Windows_Heigh * FrmMain_Size) / 2 - axWindowsMediaPlayer1.Height / 2;
            }
            else
            {
                nums_1 = (500 * Windows_Heigh - 500 * Windows_Heigh * FrmMain_Size) / 2;
            }

            this.panel_PictureBox_All.Size = new System.Drawing.Size(Convert.ToInt32(666 * Windows_Heigh), Convert.ToInt32(666 * Windows_Heigh));
            this.panel_PictureBox_All.Location = new System.Drawing.Point(Convert.ToInt32(144 * Windows_Width), Convert.ToInt32(188 * Windows_Heigh + nums_1 + 55));

            this.pictureBox_Empty_Red.Size = new System.Drawing.Size(Convert.ToInt32(122 * Windows_Heigh), Convert.ToInt32(122 * Windows_Heigh));
            this.pictureBox_Empty_Red.Location = new System.Drawing.Point(Convert.ToInt32(274 * Windows_Heigh), Convert.ToInt32(274 * Windows_Heigh));

            this.pictureBox1.Size = new System.Drawing.Size(Convert.ToInt32(566 * Windows_Heigh), Convert.ToInt32(566 * Windows_Heigh));
            this.pictureBox1.Location = new System.Drawing.Point(Convert.ToInt32(48 * Windows_Heigh), Convert.ToInt32(48 * Windows_Heigh));

            //重新绘制控件
            this.panel_PictureBox_All.Invalidate();
            this.pictureBox_Empty_Red.Invalidate();
            this.pictureBox1.Invalidate();

            if (Open_Singer_Image == 1)
            {

                GraphicsPath gp = new GraphicsPath();
                gp.AddEllipse(pictureBox1.ClientRectangle);
                Region region = new Region(gp);
                pictureBox1.Region = region;
                gp = null;
                region = null;
                base.OnCreateControl();

                gp = new GraphicsPath();
                gp.AddEllipse(pictureBox_Empty_Red.ClientRectangle);
                region = new Region(gp);
                pictureBox_Empty_Red.Region = region;
                gp = null;
                region = null;
                base.OnCreateControl();

                gp = new GraphicsPath();
                gp.AddEllipse(pictureBox_Panel.ClientRectangle);
                region = new Region(gp);
                pictureBox_Panel.Region = region;
                gp = null;
                region = null;
                base.OnCreateControl();

            }
        }
        public void panel_PictureBox_All_Location_Normal()
        {
            double nums_1 = 0;
            if (Windows_Heigh != 1)
            {
                nums_1 = (500 * Windows_Heigh - 500 * Windows_Heigh * FrmMain_Size) / 2 - axWindowsMediaPlayer1.Height / 2;
            }
            else
            {
                nums_1 = (500 * Windows_Heigh - 500 * Windows_Heigh * FrmMain_Size) / 2;
            }

            this.panel_PictureBox_All.Size = new System.Drawing.Size(Convert.ToInt32(500 * Windows_Heigh), Convert.ToInt32(500 * Windows_Heigh));
            this.panel_PictureBox_All.Location = new System.Drawing.Point(Convert.ToInt32(36 * Windows_Width), Convert.ToInt32(124 * Windows_Heigh + nums_1));

            this.pictureBox_Empty_Red.Size = new System.Drawing.Size(Convert.ToInt32(99 * Windows_Heigh), Convert.ToInt32(99 * Windows_Heigh));
            this.pictureBox_Empty_Red.Location = new System.Drawing.Point(Convert.ToInt32(201 * Windows_Heigh), Convert.ToInt32(201 * Windows_Heigh));

            this.pictureBox1.Size = new System.Drawing.Size(Convert.ToInt32(422 * Windows_Heigh), Convert.ToInt32(422 * Windows_Heigh));
            this.pictureBox1.Location = new System.Drawing.Point(Convert.ToInt32(38 * Windows_Heigh), Convert.ToInt32(38 * Windows_Heigh));

            if (Open_Singer_Image == 1)
            {

                //重新绘制控件
                this.panel_PictureBox_All.Invalidate();
                this.pictureBox_Empty_Red.Invalidate();
                this.pictureBox1.Invalidate();

                GraphicsPath gp = new GraphicsPath();
                gp.AddEllipse(pictureBox1.ClientRectangle);
                Region region = new Region(gp);
                pictureBox1.Region = region;
                gp = null;
                region = null;
                base.OnCreateControl();

                gp = new GraphicsPath();
                gp.AddEllipse(pictureBox_Empty_Red.ClientRectangle);
                region = new Region(gp);
                pictureBox_Empty_Red.Region = region;
                gp = null;
                region = null;
                base.OnCreateControl();

                gp = new GraphicsPath();
                gp.AddEllipse(pictureBox_Panel.ClientRectangle);
                region = new Region(gp);
                pictureBox_Panel.Region = region;
                gp = null;
                region = null;
                base.OnCreateControl();

            }
        }

        private void Loaction_Max_Button_Click(object sender, EventArgs e)
        {


            if (Song_Play_MV_Nums == 0)
            {

                LRC_Time_Stop();

                LRC_Hide_Max_Minimized();

                if (this.WindowState == System.Windows.Forms.FormWindowState.Normal)
                {
                    this.WindowState = System.Windows.Forms.FormWindowState.Maximized;


                    this.Loaction_Max_Button.BackgroundImage = global::Music_Player_Test.Properties.Resources.MaxNormal;


                    if (Open_Singer_Image == 0)
                    {
                        this.panel_Song_LRC.Location = new System.Drawing.Point(Convert.ToInt32(333 * Windows_Width), Convert.ToInt32(212 * Windows_Heigh));
                        this.panel_Song_LRC.Size = new System.Drawing.Size(Convert.ToInt32(1222 * Windows_Width), Convert.ToInt32(622 * Windows_Heigh));
                    }
                    else
                    {
                        this.panel_Song_LRC.Location = new System.Drawing.Point(Convert.ToInt32(988 * Windows_Width), Convert.ToInt32(212 * Windows_Heigh));
                        this.panel_Song_LRC.Size = new System.Drawing.Size(Convert.ToInt32(602 * Windows_Width), Convert.ToInt32(622 * Windows_Heigh));

                        panel_PictureBox_All_Location_Max();
                    }

                    ///**调用方法：
                    ////初始化调用不规则窗体生成代码* */
                    //double nums = Convert.ToDouble(pictureBox_Panel.Height * 486 / 500 ) / 486;  //   486:500
                    //Bitmap bitmap = new Bitmap(Properties.Resources.player, Convert.ToInt32(296 * nums * Windows_Width), Convert.ToInt32(486 * nums * Windows_Heigh));
                    //BitmapRegion BitmapRegion = new BitmapRegion();//此为生成不规则窗体或者控件的类
                    //BitmapRegion.CreateControlRegion(pictureBox2, bitmap, 0, 0);//这里this可替换成其他控件。   , button2.Width, button2.Height

                }
                else
                {
                    this.WindowState = System.Windows.Forms.FormWindowState.Normal;


                    this.Loaction_Max_Button.BackgroundImage = global::Music_Player_Test.Properties.Resources.Max;


                    if (Open_Singer_Image == 0)
                    {
                        this.panel_Song_LRC.Location = new System.Drawing.Point(Convert.ToInt32(366 * Windows_Width), Convert.ToInt32(53 * Windows_Heigh));
                        this.panel_Song_LRC.Size = new System.Drawing.Size(Convert.ToInt32(602 * Windows_Width), Convert.ToInt32(622 * Windows_Heigh));
                    }
                    else
                    {
                        this.panel_Song_LRC.Location = new System.Drawing.Point(Convert.ToInt32(520 * Windows_Width), Convert.ToInt32(53 * Windows_Heigh));
                        this.panel_Song_LRC.Size = new System.Drawing.Size(Convert.ToInt32(602 * Windows_Width), Convert.ToInt32(622 * Windows_Heigh));

                        panel_PictureBox_All_Location_Normal();
                    }

                    ///**调用方法：
                    ////初始化调用不规则窗体生成代码* */
                    //double nums = Convert.ToDouble(pictureBox_Panel.Height * 486 / 500) / 486;  //   486:500
                    //Bitmap bitmap = new Bitmap(Properties.Resources.player, Convert.ToInt32(296 * nums * Windows_Width), Convert.ToInt32(486 * nums * Windows_Heigh));
                    //BitmapRegion BitmapRegion = new BitmapRegion();//此为生成不规则窗体或者控件的类
                    //BitmapRegion.CreateControlRegion(pictureBox2, bitmap, 0, 0);//这里this可替换成其他控件。   , button2.Width, button2.Height


                }
                if (Song_Play_MV_Nums == 0)
                {
                    //Thread.Sleep(1000);

                    LRC_Time_Start();
                }

                LRC_Show_Max_Minimized();

                int panel_LRC_Width = this.panel_Song_LRC.Width;

                this.lblS1.Width = panel_LRC_Width;
                this.lblS2.Width = panel_LRC_Width;
                this.lblS3.Width = panel_LRC_Width;
                this.lblS4.Width = panel_LRC_Width;
                this.lblS5.Width = panel_LRC_Width;
                this.lblS6.Width = panel_LRC_Width;
                this.lblS7.Width = panel_LRC_Width;
                this.lblS8.Width = panel_LRC_Width;
                this.lblS9.Width = panel_LRC_Width;
                this.lblS10.Width = panel_LRC_Width;
                this.lblS11.Width = panel_LRC_Width;

                Thread_Sleep();
            }
            else
            {
                if (this.WindowState == System.Windows.Forms.FormWindowState.Normal)
                {
                    this.WindowState = System.Windows.Forms.FormWindowState.Maximized;

                    this.Loaction_Max_Button.BackgroundImage = global::Music_Player_Test.Properties.Resources.MaxNormal;

                    if (Open_Singer_Image == 0)
                    {
                        this.panel_Song_LRC.Location = new System.Drawing.Point(Convert.ToInt32(655 * Windows_Width), Convert.ToInt32(212 * Windows_Heigh));
                    }
                    else
                    {
                        this.panel_Song_LRC.Location = new System.Drawing.Point(Convert.ToInt32(988 * Windows_Width), Convert.ToInt32(212 * Windows_Heigh));

                        panel_PictureBox_All_Location_Max();
                    }


                }
                else
                {
                    this.WindowState = System.Windows.Forms.FormWindowState.Normal;

                    this.Loaction_Max_Button.BackgroundImage = global::Music_Player_Test.Properties.Resources.Max;

                    if (Open_Singer_Image == 0)
                    {
                        this.panel_Song_LRC.Location = new System.Drawing.Point(Convert.ToInt32(366 * Windows_Width), Convert.ToInt32(53 * Windows_Heigh));
                    }
                    else
                    {
                        this.panel_Song_LRC.Location = new System.Drawing.Point(Convert.ToInt32(520 * Windows_Width), Convert.ToInt32(53 * Windows_Heigh));

                        panel_PictureBox_All_Location_Normal();
                    }


                }
            }
        }

        private void Loaction_Min_Button_Click(object sender, EventArgs e)
        {
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;

            Thread_Sleep();
        }

        #endregion

        private void Exit_Button_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #region 点击控件显示or隐藏

        public static int Song_Play_MV_Nums = 0;
        private void button3_Click(object sender, EventArgs e)
        {
            WMP_Song_Play_Ids_MV = WMP_Song_Play_Ids;//表示当前正在播放的歌曲(DataGridView)id
            WMP_Song_Play_Ids_Love_MV = WMP_Song_Play_Ids_Love;//表示当前正在播放的歌曲(DataGridView)id
            WMP_Song_Play_Ids_Auto_MV = WMP_Song_Play_Ids_Auto;

            Song_Play_MV_Nums = 1;
            MV_Show_show();
            Music_MV.Hide();

            Set_Song_Lrc_Windows.Hide();
            Windows_Song_Lrc.Hide();

            label_Singer_Name.Hide();
            label_Song_Name.Hide();

            panel_Button.Hide();

            Delete_All.Hide();
            Button_Open_Image.Hide();
            Close_Song_LRC.Hide();
            Font_Set_Font.Hide();
            Font_Set_Color.Hide();

            //button_Font_User.Hide();
            button_WMP_Time_Change.Hide();


            //关闭图片旋转，以提高性能
            panel_PictureBox_All.Hide();

            Open_Singer_Image = 0;
            Button_Open_Image.Text = "开启图片旋转（性能->Low）";

            Photo_BackGround_Time.Stop();

            Open__PictureBox_Image = 0;

            if (this.WindowState == System.Windows.Forms.FormWindowState.Maximized)
            {
                this.panel_Song_LRC.Location = new System.Drawing.Point(Convert.ToInt32(655 * Windows_Width), Convert.ToInt32(212 * Windows_Heigh));

            }
            else if (this.WindowState == System.Windows.Forms.FormWindowState.Normal)
            {
                this.panel_Song_LRC.Location = new System.Drawing.Point(Convert.ToInt32(366 * Windows_Width), Convert.ToInt32(53 * Windows_Heigh));
            }

            WMP_MV_State.Start();//开启检测WMP_MV控件的状态

            MV_Next_nums = 1;
            Music_Next_nums = 1;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Song_Play_MV_Nums = 0;
            Song_Show_show();
            Music_MV.Show();

            panel_Button.Show();

            //axWindowsMediaPlayer1.URL = Song_Path_Temp;

            Set_Song_Lrc_Windows.Show();
            Windows_Song_Lrc.Show();

            label_Singer_Name.Show();
            label_Song_Name.Show();

            Delete_All.Show();
            Button_Open_Image.Show();
            Close_Song_LRC.Show();
            Font_Set_Font.Show();
            Font_Set_Color.Show();

            //button_Font_User.Show();
            button_WMP_Time_Change.Show();

            panel_Windows.Show();

            WMP_MV_State.Stop();//开启检测WMP_MV控件的状态
        }

        public void Show_Song()
        {

            this.panel_Song_LRC.Show();

            this.ListSong_love.Show();
            this.ListSong_AllSong.Show();

            this.ListSong_Auto.Show();
            this.Song_Add_List.Show();
            this.Song_Delete_List.Show();
            this.Song_Find_ALL_List.Show();
        }
        public void Hide_Song()
        {
            this.dataGridView_List_ALL.Hide();
            this.dataGridView_List_Love.Hide();
            this.dataGridView_List_Auto.Hide();

            this.panel_Song_LRC.Hide();

            this.ListSong_love.Hide();
            this.ListSong_AllSong.Hide();

            this.ListSong_Auto.Hide();
            this.Song_Add_List.Hide();
            this.Song_Delete_List.Hide();
            this.Song_Find_ALL_List.Hide();
        }

        /// <summary>
        /// 歌词显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Close_Song_LRC_Click(object sender, EventArgs e)
        {
            if (Close_Song_LRC.Text.Equals("开启歌词显示"))
            {
                panel_Song_LRC.Show();


                Close_Song_LRC.Text = "关闭歌词显示";

                Open__Panel_LRC = 1;

                Song_Lrc_Times.Start();
            }
            else
            {
                panel_Song_LRC.Hide();


                Close_Song_LRC.Text = "开启歌词显示";

                Open__Panel_LRC = 0;

                Song_Lrc_Times.Stop();
            }
        }

        #endregion



        public Boolean flag;//ListSong_AllSong所处状态//true为列表已展开
        #region 点击歌曲列表，其它列表隐藏

        /// <summary>
        /// 点击歌曲列表控件其它则隐藏
        /// 本地音乐
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListSong_AllSong_Click(object sender, EventArgs e)
        {
            Thread.Sleep(50);
            if (flag == true)
            {
                this.dataGridView_List_ALL.Hide();
                this.dataGridView_List_Love.Hide();
                this.dataGridView_List_Auto.Hide();

                flag = false;
            }
            else
            {
                this.dataGridView_List_ALL.Width = 366;
                this.dataGridView_List_ALL.Height = 577;
                this.dataGridView_List_ALL.Show();
                flag = true;
            }

            Clear_ALL_SelectRows_Selected();//清除选中行的选中效果
        }

        /// <summary>
        /// 我的收藏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListSong_love_Click(object sender, EventArgs e)
        {
            Thread.Sleep(50);
            if (flag == true)
            {
                this.dataGridView_List_Love.Hide();
                this.dataGridView_List_ALL.Hide();
                this.dataGridView_List_Auto.Hide();

                flag = false;
            }
            else
            {
                this.dataGridView_List_Love.Width = 366;
                this.dataGridView_List_Love.Height = 577;
                this.dataGridView_List_Love.Show();
                flag = true;
            }

            Clear_ALL_SelectRows_Selected();//清除选中行的选中效果
        }

        /// <summary>
        /// 默认列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListSong_Auto_Click(object sender, EventArgs e)
        {
            Thread.Sleep(50);
            if (flag == true)
            {
                this.dataGridView_List_Auto.Hide();
                this.dataGridView_List_ALL.Hide();
                this.dataGridView_List_Love.Hide();

                flag = false;
            }
            else
            {
                this.dataGridView_List_Auto.Width = 366;
                this.dataGridView_List_Auto.Height = 577;
                this.dataGridView_List_Auto.Show();
                flag = true;
            }

            Clear_ALL_SelectRows_Selected();//清除选中行的选中效果
        }

        #endregion


        private static frmTopMost myTopMost = new frmTopMost();
        #region 桌面歌词 悬浮框          （部分弃用）
        /// <summary>
        /// 窗体初始状态
        /// </summary>
        private FormWindowState fwsPrevious;
        /// <summary>
        /// 悬浮窗体
        /// </summary>



        public void Show_Song_Lrc_Windows(object sender, EventArgs e)
        {
            if (Windows_Song_Lrc.Text.Equals("打开桌面歌词") || Windows_Song_Lrc_2.Text.Equals("打开桌面歌词"))
            {
                myTopMost.Show();

                Windows_Song_Lrc.Text = "关闭桌面歌词";
                Windows_Song_Lrc_2.Text = "关闭桌面歌词";
            }
            else
            {
                myTopMost.Hide();

                Windows_Song_Lrc.Text = "打开桌面歌词";
                Windows_Song_Lrc_2.Text = "打开桌面歌词";
            }


        }

        /// <summary>
        /// 还原窗口方法，即供悬浮窗口进行调用的。
        /// </summary>
        public void RestoreWindow()
        {
            this.WindowState = fwsPrevious;
            //this.ShowInTaskbar = true;
        }

        public int Change_Nums = 0;
        private void Set_Windows_Click(object sender, EventArgs e)
        {
            if (Windows_Song_Lrc.Text.Equals("关闭桌面歌词"))
            {
                if (Change_Nums == 0)
                {
                    myTopMost.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;

                    Change_Nums = 1;
                }
                else
                {
                    myTopMost.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;

                    Change_Nums = 0;
                }
            }
            else
            {
                MessageBox.Show("打开桌面歌词，即可通过标题栏拖动桌面歌词");
            }
        }
        #endregion





        #region 设置歌词字体的颜色和大小

        //设置主歌词颜色按钮
        private void Font_Color_LblS6_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                this.lblS6.ForeColor = colorDialog1.Color;
            }
        }

        //设置副歌词颜色按钮
        private void Font_Color_ALL_OtherS6_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                this.lblS1.ForeColor = colorDialog1.Color;
                this.lblS2.ForeColor = colorDialog1.Color;
                this.lblS3.ForeColor = colorDialog1.Color;
                this.lblS4.ForeColor = colorDialog1.Color;
                this.lblS5.ForeColor = colorDialog1.Color;
                this.lblS7.ForeColor = colorDialog1.Color;
                this.lblS8.ForeColor = colorDialog1.Color;
                this.lblS9.ForeColor = colorDialog1.Color;
                this.lblS10.ForeColor = colorDialog1.Color;
                this.lblS11.ForeColor = colorDialog1.Color;
            }
        }

        //设置桌面主歌词颜色按钮
        private void Font_Color_myTopMost_LblS6_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                myTopMost.lblS6_top.ForeColor = colorDialog1.Color;
            }
        }

        //设置桌面副歌词颜色按钮
        private void Font_Color_myTopMost_ALL_OtherS6_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                myTopMost.lblS7.ForeColor = colorDialog1.Color;

            }
        }


        //设置主歌词字体按钮
        private void Font_Text_Click(object sender, EventArgs e)
        {
            if (fontDialog1.ShowDialog() == DialogResult.OK)
            {
                this.lblS6.Font = fontDialog1.Font;
            }
        }

        //设置副歌词字体按钮
        private void Font_Text_ALL_OtherS6_Click(object sender, EventArgs e)
        {
            if (fontDialog1.ShowDialog() == DialogResult.OK)
            {
                this.lblS1.Font = fontDialog1.Font;
                this.lblS2.Font = fontDialog1.Font;
                this.lblS3.Font = fontDialog1.Font;
                this.lblS4.Font = fontDialog1.Font;
                this.lblS5.Font = fontDialog1.Font;
                this.lblS7.Font = fontDialog1.Font;
                this.lblS8.Font = fontDialog1.Font;
                this.lblS9.Font = fontDialog1.Font;
                this.lblS10.Font = fontDialog1.Font;
                this.lblS11.Font = fontDialog1.Font;
            }
        }

        //设置主歌词字体按钮
        private void Font_myTopMost_Text_Click(object sender, EventArgs e)
        {
            if (fontDialog1.ShowDialog() == DialogResult.OK)
            {
                myTopMost.lblS6_top.Font = fontDialog1.Font;
            }
        }

        //设置副歌词字体按钮
        private void Font_myTopMost_Text_ALL_OtherS6_Click(object sender, EventArgs e)
        {
            if (fontDialog1.ShowDialog() == DialogResult.OK)
            {
                myTopMost.lblS7.Font = fontDialog1.Font;
            }
        }


        private void Font_Set_Font_Click(object sender, EventArgs e)
        {
            Point p = new Point(0, 0);
            this.Set_Font_Font.Show(Font_Set_Font, p);
            //MessageBox.Show("请鼠标右击打开该按钮");
        }

        private void Font_Set_Color_Click(object sender, EventArgs e)
        {
            Point p = new Point(0, 0);
            this.Set_Font_Color.Show(Font_Set_Color, p);
            //MessageBox.Show("请鼠标右击打开该按钮");
        }
        #endregion

        #region 读取配置文件
        /// <summary>
        /// 读取配置文件
        /// </summary>
        public void Load_Set_Info()
        {
            Load_LRC_XML();//读取歌词的配置文件
            Load_ALL_XML();
        }
        /// <summary>
        /// 保存配置文件
        /// </summary>
        public void Save_Set_Info()
        {
            Save_LRC_XML();//保存歌词的配置文件
            Save_ALL_XML();
        }
        #endregion

        private FileStream FS_List_Save_XML = null;
        private StreamWriter SW_List_XML = null;//写入 
        private StreamReader SR_List_XML = null;//读取
        private FontConverter fc = new FontConverter();
        private ColorConverter cc = new ColorConverter();

        public Color LblS1;
        public Color LblS6;
        public static Font lbl_1;
        public static Font lbl_2;
        public static Color lbl_1_Color;
        public static Color lbl_2_Color;
        #region 歌词设置配置

        /// <summary>
        /// 读取歌词的配置文件
        /// </summary>
        public void Load_LRC_XML()
        {
            try
            {
                FontConverter fontConverter1 = new FontConverter();

                string temp = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"\歌词字体设置.ini");

                FS_List_Save_XML = new FileStream(temp, FileMode.Open);
                SR_List_XML = new StreamReader(FS_List_Save_XML);



                this.lblS1.Font = (Font)fc.ConvertFromString(SR_List_XML.ReadLine());
                this.lblS2.Font = (Font)fc.ConvertFromString(SR_List_XML.ReadLine());
                this.lblS3.Font = (Font)fc.ConvertFromString(SR_List_XML.ReadLine());
                this.lblS4.Font = (Font)fc.ConvertFromString(SR_List_XML.ReadLine());
                this.lblS5.Font = (Font)fc.ConvertFromString(SR_List_XML.ReadLine());
                this.lblS6.Font = (Font)fc.ConvertFromString(SR_List_XML.ReadLine());
                this.lblS7.Font = (Font)fc.ConvertFromString(SR_List_XML.ReadLine());
                this.lblS8.Font = (Font)fc.ConvertFromString(SR_List_XML.ReadLine());
                this.lblS9.Font = (Font)fc.ConvertFromString(SR_List_XML.ReadLine());
                this.lblS10.Font = (Font)fc.ConvertFromString(SR_List_XML.ReadLine());
                this.lblS11.Font = (Font)fc.ConvertFromString(SR_List_XML.ReadLine());

                //myTopMost.lblS6_top.Font = (Font)fc.ConvertFromString(SR_List_XML.ReadLine());
                //myTopMost.lblS7.Font = (Font)fc.ConvertFromString(SR_List_XML.ReadLine());

                lbl_1 = (Font)fc.ConvertFromString(SR_List_XML.ReadLine());
                lbl_2 = (Font)fc.ConvertFromString(SR_List_XML.ReadLine());

                //清空缓冲区
                SR_List_XML.Close();
                //关闭流
                FS_List_Save_XML.Close();



                temp = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"\歌词颜色设置.ini");

                FS_List_Save_XML = new FileStream(temp, FileMode.Open);
                SR_List_XML = new StreamReader(FS_List_Save_XML);

                this.lblS1.ForeColor = (Color)cc.ConvertFromString(SR_List_XML.ReadLine());
                this.lblS2.ForeColor = (Color)cc.ConvertFromString(SR_List_XML.ReadLine());
                this.lblS3.ForeColor = (Color)cc.ConvertFromString(SR_List_XML.ReadLine());
                this.lblS4.ForeColor = (Color)cc.ConvertFromString(SR_List_XML.ReadLine());
                this.lblS5.ForeColor = (Color)cc.ConvertFromString(SR_List_XML.ReadLine());
                this.lblS6.ForeColor = (Color)cc.ConvertFromString(SR_List_XML.ReadLine());
                this.lblS7.ForeColor = (Color)cc.ConvertFromString(SR_List_XML.ReadLine());
                this.lblS8.ForeColor = (Color)cc.ConvertFromString(SR_List_XML.ReadLine());
                this.lblS9.ForeColor = (Color)cc.ConvertFromString(SR_List_XML.ReadLine());
                this.lblS10.ForeColor = (Color)cc.ConvertFromString(SR_List_XML.ReadLine());
                this.lblS11.ForeColor = (Color)cc.ConvertFromString(SR_List_XML.ReadLine());

                //myTopMost.lblS6_top.ForeColor = (Color)cc.ConvertFromString(SR_List_XML.ReadLine());
                //myTopMost.lblS7.ForeColor = (Color)cc.ConvertFromString(SR_List_XML.ReadLine());

                lbl_1_Color = (Color)cc.ConvertFromString(SR_List_XML.ReadLine());
                lbl_2_Color = (Color)cc.ConvertFromString(SR_List_XML.ReadLine());


                LblS1 = this.lblS1.ForeColor;
                LblS6 = this.lblS6.ForeColor;


                //清空缓冲区
                SR_List_XML.Close();
                //关闭流
                FS_List_Save_XML.Close();
            }
            catch
            {

            }

        }

        /// <summary>
        /// 保存歌词的配置文件
        /// </summary>
        public void Save_LRC_XML()
        {
            string temp = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"\歌词字体设置.ini");

            FS_List_Save_XML = new FileStream(temp, FileMode.Create);
            SW_List_XML = new StreamWriter(FS_List_Save_XML);//无法静态



            SW_List_XML.WriteLine(fc.ConvertToInvariantString(lblS1.Font));
            SW_List_XML.WriteLine(fc.ConvertToInvariantString(lblS2.Font));
            SW_List_XML.WriteLine(fc.ConvertToInvariantString(lblS3.Font));
            SW_List_XML.WriteLine(fc.ConvertToInvariantString(lblS4.Font));
            SW_List_XML.WriteLine(fc.ConvertToInvariantString(lblS5.Font));
            SW_List_XML.WriteLine(fc.ConvertToInvariantString(lblS6.Font));
            SW_List_XML.WriteLine(fc.ConvertToInvariantString(lblS7.Font));
            SW_List_XML.WriteLine(fc.ConvertToInvariantString(lblS8.Font));
            SW_List_XML.WriteLine(fc.ConvertToInvariantString(lblS9.Font));
            SW_List_XML.WriteLine(fc.ConvertToInvariantString(lblS10.Font));
            SW_List_XML.WriteLine(fc.ConvertToInvariantString(lblS11.Font));

            SW_List_XML.WriteLine(fc.ConvertToInvariantString(myTopMost.lblS6_top.Font));
            SW_List_XML.WriteLine(fc.ConvertToInvariantString(myTopMost.lblS7.Font));
            //清空缓冲区
            SW_List_XML.Flush();
            //关闭流
            FS_List_Save_XML.Close();


            temp = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"\歌词颜色设置.ini");

            FS_List_Save_XML = new FileStream(temp, FileMode.Create);
            SW_List_XML = new StreamWriter(FS_List_Save_XML);//无法静态

            SW_List_XML.WriteLine(cc.ConvertToInvariantString(lblS1.ForeColor));
            SW_List_XML.WriteLine(cc.ConvertToInvariantString(lblS2.ForeColor));
            SW_List_XML.WriteLine(cc.ConvertToInvariantString(lblS3.ForeColor));
            SW_List_XML.WriteLine(cc.ConvertToInvariantString(lblS4.ForeColor));
            SW_List_XML.WriteLine(cc.ConvertToInvariantString(lblS5.ForeColor));
            SW_List_XML.WriteLine(cc.ConvertToInvariantString(lblS6.ForeColor));
            SW_List_XML.WriteLine(cc.ConvertToInvariantString(lblS7.ForeColor));
            SW_List_XML.WriteLine(cc.ConvertToInvariantString(lblS8.ForeColor));
            SW_List_XML.WriteLine(cc.ConvertToInvariantString(lblS9.ForeColor));
            SW_List_XML.WriteLine(cc.ConvertToInvariantString(lblS10.ForeColor));
            SW_List_XML.WriteLine(cc.ConvertToInvariantString(lblS11.ForeColor));

            SW_List_XML.WriteLine(cc.ConvertToInvariantString(myTopMost.lblS6_top.ForeColor));
            SW_List_XML.WriteLine(cc.ConvertToInvariantString(myTopMost.lblS7.ForeColor));
            //清空缓冲区
            SW_List_XML.Flush();
            //关闭流
            FS_List_Save_XML.Close();

        }

        #endregion

        #region XML配置

        public static int Open__PictureBox_Image;
        public static int Open__Panel_LRC;

        public void Load_ALL_XML()
        {
            string temp = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"\XML设置.ini");

            FS_List_Save_XML = new FileStream(temp, FileMode.Open);
            SR_List_XML = new StreamReader(FS_List_Save_XML);

            try
            {
                Open__PictureBox_Image = Convert.ToInt16(SR_List_XML.ReadLine());
            }
            catch
            {
                Open__PictureBox_Image = 1;
            }

            //Open_Singer_Image = Convert.ToInt16(SR_List_XML.ReadLine());

            if (Open__PictureBox_Image == 1)
            {
                panel_PictureBox_All.Show();

                Open_Singer_Image = 1;
                Button_Open_Image.Text = "关闭图片旋转（性能->UP）";

                //Photo_BackGround_Time.Start();

                Open__PictureBox_Image = 1;



            }
            else
            {
                panel_PictureBox_All.Hide();

                Open_Singer_Image = 0;
                Button_Open_Image.Text = "开启图片旋转（性能->Low）";

                //Photo_BackGround_Time.Stop();

                Open__PictureBox_Image = 0;

            }

            try
            {
                Open__Panel_LRC = Convert.ToInt16(SR_List_XML.ReadLine());
            }
            catch
            {
                Open__Panel_LRC = 1;
            }

            if (Open__Panel_LRC == 1)
            {
                panel_Song_LRC.Show();


                Close_Song_LRC.Text = "关闭歌词显示";
            }
            else
            {
                panel_Song_LRC.Hide();


                Close_Song_LRC.Text = "开启歌词显示";
            }

            //清空缓冲区
            SR_List_XML.Close();
            //关闭流
            FS_List_Save_XML.Close();
        }

        public void Save_ALL_XML()
        {
            string temp = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"\XML设置.ini");

            FS_List_Save_XML = new FileStream(temp, FileMode.Create);
            SW_List_XML = new StreamWriter(FS_List_Save_XML);//无法静态


            SW_List_XML.WriteLine(Open__PictureBox_Image);

            SW_List_XML.WriteLine(Open__Panel_LRC);
            //清空缓冲区
            SW_List_XML.Flush();
            //关闭流
            FS_List_Save_XML.Close();
        }

        #endregion





        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
        public static extern int SystemParametersInfo(int uAction, int uParam, StringBuilder lpvParam, int fuWinIni);
        private const int SPI_GETDESKWALLPAPER = 0x0073;
        #region 桌面写真模式

        //调用
        public static string wallpaper_path;

        private void button_Open_Windows_Picture_Click(object sender, EventArgs e)
        {
            SingerPicPath = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"\singerPhoto\" + singer_Name + ".jpg");//获取歌手图片所在路径  

            if (button_Open_Windows_Picture.Text.Equals("开启桌面写真"))
            {
                //myTopMost.Show();
                try
                {
                    backgroung = new Bitmap(SingerPicPath);//测试是否存在该文件
                    SystemParametersInfo(20, 1, SingerPicPath, 1);
                }
                catch
                {
                    SystemParametersInfo(20, 1, PhotoPath, 1);
                }

                button_Open_Windows_Picture.Text = "关闭桌面写真";


                myTopMost.Show();
                Windows_Song_Lrc.Text = "关闭桌面歌词";
                Windows_Song_Lrc_2.Text = "关闭桌面歌词";
            }
            else
            {
                //myTopMost.Hide();

                SystemParametersInfo(20, 1, wallpaper_path, 1);

                button_Open_Windows_Picture.Text = "开启桌面写真";

                myTopMost.Hide();
                Windows_Song_Lrc.Text = "打开桌面歌词";
                Windows_Song_Lrc_2.Text = "打开桌面歌词";
            }

        }

        public void button_Open_Windows_Picture_All()
        {
            if (button_Open_Windows_Picture.Text.Equals("关闭桌面写真"))
            {
                //myTopMost.Show();

                SystemParametersInfo(20, 1, SingerPicPath, 1);

            }

        }

        #endregion       


        #region 爬虫

        public static string web_src;
        /// <summary>
        /// 下载网页
        /// </summary>
        private void webBrowser1_Show()
        {
            web_src = "https://blog.csdn.net/u011614610/article/details/103718563";
            //web_src = @"https://www.kugou.com/yy/html/search.html#searchType=song&searchKeyWord=" + Song_Names_Song.Substring(Song_Names_Song.LastIndexOf("-") + 2, Song_Names_Song.Length - Song_Names_Song.IndexOf("- ") - 2);
            try
            {
                WebClient MyWebClient = new WebClient();
                MyWebClient.Credentials = CredentialCache.DefaultCredentials;//获取或设置用于向Internet资源的请求进行身份验证的网络凭据
                Byte[] pageData = MyWebClient.DownloadData(web_src); //从指定网站下载数据
                //string pageHtml = Encoding.Default.GetString(pageData);  //如果获取网站页面采用的是GB2312，则使用这句    
                string pageHtml = Encoding.UTF8.GetString(pageData); //如果获取网站页面采用的是UTF-8，则使用这句        

                Console.WriteLine(pageHtml);//在控制台输入获取的内容

                FileStream fs = new FileStream(@"C:\Users\17741\Desktop\全民K歌\相关资料\NAudio-master -（音频处理类库）\C#播放音频的正确姿势（一）——NAudio的简介与基础播放.html", FileMode.Create);//清空此文件的数据
                fs.Flush();
                fs.Close();


                using (StreamWriter sw = new StreamWriter(@"C:\Users\17741\Desktop\全民K歌\相关资料\NAudio-master -（音频处理类库）\C#播放音频的正确姿势（一）——NAudio的简介与基础播放.html"))//将获取的内容写入文本
                {
                    sw.Write(pageHtml);
                }
                //Console.ReadLine(); //让控制台暂停,否则一闪而过了    
            }
            catch (WebException webEx)
            {
                Console.WriteLine(webEx.Message.ToString());
            }


            //MessageBox.Show("正在获取页面代码，请稍后...");
            //strCode = GetPageSource(web_src);
            //MessageBox.Show("正在提取超链接，请稍侯...");
            //alLinks = GetHyperLinks(strCode);
            //MessageBox.Show("正在写入文件，请稍侯...");
            //WriteToXml(web_src, alLinks);

            //GetPageSource(web_src);
            ////GetPageSource("file:///D:/%E5%A2%A8%E6%99%BA_%E6%AF%92%E8%9B%87%E8%AE%AF%E6%81%AF%EF%BC%88%E7%AE%80%E5%8D%95%E9%9F%B3%E4%B9%90%E6%92%AD%E6%94%BE%E5%99%A8%EF%BC%891.0.1/Find_Song_Info.html");
        }



        //string strCode;
        //ArrayList alLinks;
        // 获取指定网页的HTML代码
        public static string GetPageSource(string URL)
        {
            Uri uri = new Uri(URL);

            HttpWebRequest hwReq = (HttpWebRequest)WebRequest.Create(uri);
            HttpWebResponse hwRes = (HttpWebResponse)hwReq.GetResponse();

            hwReq.Method = "Get";

            hwReq.KeepAlive = false;

            StreamReader reader = new StreamReader(hwRes.GetResponseStream(), System.Text.Encoding.GetEncoding("utf-8"));

            return reader.ReadToEnd();
        }
        // 提取HTML代码中的网址
        public static ArrayList GetHyperLinks(string htmlCode)
        {
            ArrayList al = new ArrayList();

            string strRegex = @"http://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?";

            Regex r = new Regex(strRegex, RegexOptions.IgnoreCase);
            MatchCollection m = r.Matches(htmlCode);

            for (int i = 0; i <= m.Count - 1; i++)
            {
                bool rep = false;
                string strNew = m[i].ToString();

                // 过滤重复的URL
                foreach (string str in al)
                {
                    if (strNew == str)
                    {
                        rep = true;
                        break;
                    }
                }

                if (!rep) al.Add(strNew);
            }

            al.Sort();

            return al;
        }
        // 把网址写入xml文件
        static void WriteToXml(string strURL, ArrayList alHyperLinks)
        {
            string xml_src = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"\ALL_Links.xml");

            FileStream fs = new FileStream(xml_src, FileMode.Create);//清空此文件的数据
            fs.Flush();
            fs.Close();

            XmlTextWriter writer = new XmlTextWriter(xml_src, Encoding.UTF8);

            writer.Formatting = Formatting.Indented;
            writer.WriteStartDocument(false);
            writer.WriteDocType("HyperLinks", null, "urls.dtd", null);
            writer.WriteComment("提取自" + strURL + "的超链接");
            writer.WriteStartElement("HyperLinks");
            writer.WriteStartElement("HyperLinks", null);
            writer.WriteAttributeString("DateTime", DateTime.Now.ToString());


            foreach (string str in alHyperLinks)
            {
                string title = GetDomain(str);
                string body = str;
                writer.WriteElementString(title, null, body);
            }

            writer.WriteEndElement();
            writer.WriteEndElement();

            writer.Flush();
            writer.Close();
        }

        // 获取网址的域名后缀
        static string GetDomain(string strURL)
        {
            string retVal;

            string strRegex = @"(\.com/|\.net/|\.cn/|\.org/|\.gov/|\.html/)";

            Regex r = new Regex(strRegex, RegexOptions.IgnoreCase);
            Match m = r.Match(strURL);
            retVal = m.ToString();

            strRegex = @"\.|/$";
            retVal = Regex.Replace(retVal, strRegex, "").ToString();

            if (retVal == "")
                retVal = "other";

            return retVal;
        }

        #endregion


        #region 生成统一尺寸的专辑，歌手图片   后台管理系统专用

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
        public static Bitmap MakeThumbnailImage(Image originalImage, int maxWidth, int maxHeight, int cropWidth, int cropHeight, int X, int Y)
        {
            Bitmap b = new Bitmap(cropWidth, cropHeight);
            try
            {
                using (Graphics g = Graphics.FromImage(b))
                {
                    //清空画布并以透明背景色填充
                    g.Clear(Color.Transparent);
                    //在指定位置并且按指定大小绘制原图片的指定部分
                    g.DrawImage(originalImage, new Rectangle(0, 0, cropWidth, cropHeight), X, Y, cropWidth, cropHeight, GraphicsUnit.Pixel);
                    Image displayImage = new Bitmap(b, maxWidth, maxHeight);

                    string temp = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"\singer_songPhoto\缩略图.jpg");

                    displayImage.Save(temp, System.Drawing.Imaging.ImageFormat.Jpeg);
                    Bitmap bit = new Bitmap(b, maxWidth, maxHeight);
                    return bit;
                }
            }
            catch (System.Exception e)
            {
                throw e;
            }
            finally
            {
                //originalImage.Dispose();
                //b.Dispose();
            }
        }

        #endregion

        #region 获取MP3文件中的专辑图片

        /// <summary>
        /// 获取MP3文件中的专辑图片
        /// </summary>
        /// <param name="song_url">歌曲的绝对路径</param>
        /// <returns>Bitmap类对象</returns>
        public bool Get_FileMp3_Image_Info(string song_url)
        {

            //FileStream fs = new FileStream(song_url, FileMode.Open);
            //byte[] header = new byte[10];       //标签头
            //fs.Read(header, 0, 10);
            //string head = Encoding.Default.GetString(header, 0, 3);
            //if (head.Equals("ID3"))
            //{
            //    int sizeAll = header[6] * 0x200000  //获取该标签的尺寸，不包括标签头
            //        + header[7] * 0x4000
            //        + header[8] * 0x80
            //        + header[9];
            //    int size = 0;
            //    byte[] body = new byte[10];     //数据帧头,这里认为数据帧头不包括编码方式
            //    fs.Read(body, 0, 10);
            //    head = Encoding.Default.GetString(body, 0, 4);
            //    while (!("APIC".Equals(head)))          //当数据帧不是图片的时候继续查找
            //    {
            //        size = body[size + 4] * 0x1000000    //(不包括帧头)
            //        + body[size + 5] * 0x10000
            //        + body[size + 6] * 0x100
            //        + body[size + 7];
            //        body = new byte[size + 10];
            //        fs.Read(body, 0, size + 10);
            //        head = Encoding.Default.GetString(body, size, 4);
            //    }
            //    size = body[size + 4] * 0x1000000
            //        + body[size + 5] * 0x10000
            //        + body[size + 6] * 0x100
            //        + body[size + 7];
            //    byte[] temp = new byte[4];


            //    fs.Read(temp, 0, 4);
            //    string tmp = Encoding.Default.GetString(temp);
            //    while (!("JFIF".Equals(tmp)))            //我发现一个规律就是所有在ID3v2中的图片都会有
            //    {                                        //JFIF的标志，而图片开始的位置就是这个标志尾部
            //        fs.Seek(-3, SeekOrigin.Current);     //所在的字节位置-10，所以当获取到图片数据帧的
            //        fs.Read(temp, 0, 4);                 //时候，要先查找这个位置，来获取图片
            //        tmp = Encoding.Default.GetString(temp);
            //    }
            //    fs.Seek(-10, SeekOrigin.Current);
            //    byte[] image = new byte[size];
            //    fs.Read(image, 0, size);
            //    MemoryStream ms = new MemoryStream(image);

            //    //BitmapImage newBitmapImage = new BitmapImage();
            //    //newBitmapImage.BeginInit();
            //    //newBitmapImage.StreamSource = ms;
            //    //newBitmapImage.EndInit();
            //    //image1.Source = newBitmapImage;

            //    Bitmap images_temp = new Bitmap(ms);

            //    this.image = images_temp;

            //    Get_FileMusic_Image = 1;

            //    fs.Flush();
            //    fs.Close();

            //    return true;
            //}
            //else { }

            //Get_FileMusic_Image = 0;

            //fs.Flush();
            //fs.Close();

            return false;

        }

        #endregion


        #region 安装字体   已弃用

        public void Setfont()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = false;//该值确定是否可以选择多个文件            

            dialog.ShowDialog();

            string AppPath = dialog.FileName;

            if (AppPath.LastIndexOf(".ttf") < 0)
            {
                MessageBox.Show("请选择.ttf文件(仅支持ttf文件)", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {

                //string AppPath = Application.StartupPath;  
                try
                {
                    PrivateFontCollection font = new PrivateFontCollection();
                    font.AddFontFile(AppPath);//字体的路径及名字
                    Font myFont = new Font(font.Families[0].Name, 18F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(134)));
                    //设置窗体控件字体，哪些控件要更改都写到下面
                    lblS1.Font = myFont;
                    lblS2.Font = myFont;
                    lblS3.Font = myFont;
                    lblS4.Font = myFont;
                    lblS5.Font = myFont;
                    lblS6.Font = myFont;
                    lblS7.Font = myFont;
                    lblS8.Font = myFont;
                    lblS9.Font = myFont;
                    lblS10.Font = myFont;
                    lblS11.Font = myFont;
                    //lblS1.Font = myFont;


                    this.panel_Song_LRC.Hide();
                    this.panel_Song_LRC.Show();

                    DialogResult result = MessageBox.Show("安装字体是否成功？", "提示", MessageBoxButtons.YesNo);

                    if (DialogResult.Yes == result)
                    {
                        this.Close();

                        Thread.Sleep(1000);
                    }


                }
                catch
                {
                    MessageBox.Show("字体不存在或加载失败/n程序将以默认字体显示", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }


        }

        private void button_Font_User_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("是否安装字体？安装后该应用需要重新手动启动", "提示", MessageBoxButtons.YesNo);

            if (DialogResult.Yes == result)
            {

                InstallFont();

            }
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern int WriteProfileString(string lpszSection, string lpszKeyName, string lpszString);

        [DllImport("user32.dll")]
        public static extern int SendMessage(int hWnd, // handle to destination window 
        uint Msg, // message 
        int wParam, // first message parameter 
        int lParam // second message parameter 
        );

        [DllImport("gdi32")]
        public static extern int AddFontResource(string lpFileName);

        private void InstallFont()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = false;//该值确定是否可以选择多个文件            

            dialog.ShowDialog();

            string AppPath = dialog.FileName;

            if (AppPath.LastIndexOf(".ttf") < 0)
            {
                MessageBox.Show("请选择.ttf文件(仅支持ttf文件)", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                string fontFileName = Path.GetFileName(AppPath);
                string fontNameWithoutExtenstion = Path.GetFileNameWithoutExtension(AppPath);
                string WinFontDir = Environment.GetEnvironmentVariable("WINDIR") + "\\fonts";
                string FontPath = WinFontDir + "\\" + fontFileName;
                if (!File.Exists(FontPath))
                {
                    File.Copy(AppPath, FontPath);
                    AddFontResource(FontPath);
                    WriteProfileString("fonts", fontNameWithoutExtenstion + "(TrueType)", fontFileName);
                }
            }
        }

        #endregion


        #region 让屏幕不休眠  （Windows API）

        /// <summary>
        /// Enables an application to inform the system that it is in use, thereby preventing the system from entering sleep or turning off the display while the application is running.
        /// </summary>
        [DllImport("kernel32")]
        private static extern ExecutionState SetThreadExecutionState(ExecutionState esFlags);


        /// <summary>
        /// 微软的官方 API 文档
        /// https://docs.microsoft.com/en-us/windows/win32/api/winbase/nf-winbase-setthreadexecutionstate
        /// </summary>
        [Flags]
        private enum ExecutionState : uint
        {
            /// <summary>
            /// Forces the system to be in the working state by resetting the system idle timer.
            /// </summary>
            SystemRequired = 0x01,

            /// <summary>
            /// Forces the display to be on by resetting the display idle timer.
            /// </summary>
            DisplayRequired = 0x02,

            /// <summary>
            /// This value is not supported. If <see cref="UserPresent"/> is combined with other esFlags values, the call will fail and none of the specified states will be set.
            /// </summary>
            [Obsolete("This value is not supported.")]
            UserPresent = 0x04,

            /// <summary>
            /// Enables away mode. This value must be specified with <see cref="Continuous"/>.
            /// <para />
            /// Away mode should be used only by media-recording and media-distribution applications that must perform critical background processing on desktop computers while the computer appears to be sleeping.
            /// </summary>
            AwaymodeRequired = 0x40,

            /// <summary>
            /// Informs the system that the state being set should remain in effect until the next call that uses <see cref="Continuous"/> and one of the other state flags is cleared.
            /// </summary>
            Continuous = 0x80000000,
        }


        /// <summary>
        /// 包含控制屏幕关闭以及系统休眠相关的方法。
        /// </summary>
        public static class SystemSleep
        {
            /// <summary>
            /// 设置此线程此时开始一直将处于运行状态，此时计算机不应该进入睡眠状态。
            /// 此线程退出后，设置将失效。
            /// 如果需要恢复，请调用 <see cref="RestoreForCurrentThread"/> 方法。
            /// </summary>
            /// <param name="keepDisplayOn">
            /// 表示是否应该同时保持屏幕不关闭。
            /// 对于游戏、视频和演示相关的任务需要保持屏幕不关闭；而对于后台服务、下载和监控等任务则不需要。
            /// </param>
            public static void PreventForCurrentThread(bool keepDisplayOn = true)
            {
                SetThreadExecutionState(keepDisplayOn
                    ? ExecutionState.Continuous | ExecutionState.SystemRequired | ExecutionState.DisplayRequired
                    : ExecutionState.Continuous | ExecutionState.SystemRequired);
            }

            /// <summary>
            /// 恢复此线程的运行状态，操作系统现在可以正常进入睡眠状态和关闭屏幕。
            /// </summary>
            public static void RestoreForCurrentThread()
            {
                SetThreadExecutionState(ExecutionState.Continuous);
            }

            /// <summary>
            /// 重置系统睡眠或者关闭屏幕的计时器，这样系统睡眠或者屏幕能够继续持续工作设定的超时时间。
            /// </summary>
            /// <param name="keepDisplayOn">
            /// 表示是否应该同时保持屏幕不关闭。
            /// 对于游戏、视频和演示相关的任务需要保持屏幕不关闭；而对于后台服务、下载和监控等任务则不需要。
            /// </param>
            public static void ResetIdle(bool keepDisplayOn = true)
            {
                SetThreadExecutionState(keepDisplayOn
                    ? ExecutionState.SystemRequired | ExecutionState.DisplayRequired
                    : ExecutionState.SystemRequired);
            }
        }

        public void test()
        {
            // 阻止系统睡眠，阻止屏幕关闭。
            SystemSleep.PreventForCurrentThread();

            // 恢复此线程曾经阻止的系统休眠和屏幕关闭。
            SystemSleep.RestoreForCurrentThread();

            // 重置系统计时器，临时性阻止系统睡眠和屏幕关闭。
            // 此效果类似于手动使用鼠标或键盘控制了一下电脑。
            SystemSleep.ResetIdle();
        }


        #endregion


        public static int Text_Nums = 300;
        #region 单元测试

        public void Test_Test_Click(object sender, EventArgs e)
        {
            //Text_Nums = Convert.ToInt32(this.textBox1.Text.ToString().Trim());


        }

        #endregion


        #region 开启唱片机模式

        private void Turntable_CD_Player_Click(object sender, EventArgs e)
        {

            //Turntable_CD_Player player = new Turntable_CD_Player();
            //player.Show();



            if (button_Open_KRC.Text.Equals("开启唱片机模式"))
            {
                this.pictureBox_Panel.BackgroundImage = global::Music_Player_Test.Properties.Resources.唱片机;

                button_Open_KRC.Text = "关闭唱片机模式";


                //this.pictureBox2.Show();


                ///**调用方法：
                //    //初始化调用不规则窗体生成代码**/
                //if (this.WindowState == System.Windows.Forms.FormWindowState.Maximized)
                //{
                //    double nums = Convert.ToDouble(pictureBox_Panel.Height * 486 / 500 ) / 486;  //   486:500
                //    Bitmap bitmap = new Bitmap(Properties.Resources.player, Convert.ToInt32(296 * nums), Convert.ToInt32(486 * nums));
                //    BitmapRegion BitmapRegion = new BitmapRegion();//此为生成不规则窗体或者控件的类
                //    BitmapRegion.CreateControlRegion(pictureBox2, bitmap, 0, 0);//这里this可替换成其他控件。   , button2.Width, button2.Height
                //}
                //else
                //{

                //    double nums = Convert.ToDouble(pictureBox_Panel.Height * 486 / 500) / 486;  //   486:500
                //    Bitmap bitmap = new Bitmap(Properties.Resources.player, Convert.ToInt32(296 * nums * Windows_Width), Convert.ToInt32(486 * nums * Windows_Heigh));
                //    BitmapRegion BitmapRegion = new BitmapRegion();//此为生成不规则窗体或者控件的类
                //    BitmapRegion.CreateControlRegion(pictureBox2, bitmap, 0, 0);//这里this可替换成其他控件。   , button2.Width, button2.Height
                //}
            }
            else
            {
                this.pictureBox_Panel.BackgroundImage = global::Music_Player_Test.Properties.Resources.唱片3;

                button_Open_KRC.Text = "开启唱片机模式";


                //this.pictureBox2.BackgroundImage = null;
                //this.pictureBox2.Hide();
            }
        }

        #endregion


        #region 自定义控件形状

        ///
        /// Create and apply the region on the supplied control
        /// 创建支持位图区域的控件（目前有button和form）
        ///
        /// The Control object to apply the region to控件
        /// The Bitmap object to create the region from位图
        public static void CreateControlRegion(Control control, Bitmap bitmap)
        {
            // Return if control and bitmap are null
            //判断是否存在控件和位图
            if (control == null || bitmap == null)
                return;
            // Set our control''s size to be the same as the bitmap
            //设置控件大小为位图大小
            control.Width = bitmap.Width;
            control.Height = bitmap.Height;
            // Check if we are dealing with Form here
            //当控件是form时
            if (control is System.Windows.Forms.Form)
            {
                // Cast to a Form object
                //强制转换为FORM
                Form form = (Form)control;
                // Set our form''s size to be a little larger that the bitmap just
                // in case the form''s border style is not set to none in the first place
                //当FORM的边界FormBorderStyle不为NONE时，应将FORM的大小设置成比位图大小稍大一点
                form.Width = control.Width;
                form.Height = control.Height;
                // No border
                //没有边界
                form.FormBorderStyle = FormBorderStyle.None;
                // Set bitmap as the background image
                //将位图设置成窗体背景图片
                form.BackgroundImage = bitmap;
                // Calculate the graphics path based on the bitmap supplied
                //计算位图中不透明部分的边界
                GraphicsPath graphicsPath = CalculateControlGraphicsPath(bitmap);
                // Apply new region
                //应用新的区域
                form.Region = new Region(graphicsPath);
            }
            // Check if we are dealing with Button here
            //当控件是button时
            else if (control is System.Windows.Forms.Button)
            {
                // Cast to a button object
                //强制转换为 button
                Button button = (Button)control;
                // Do not show button text
                //不显示button text
                button.Text = "";
                // Change cursor to hand when over button
                //改变 cursor的style
                button.Cursor = Cursors.Hand;
                // Set background image of button
                //设置button的背景图片
                button.BackgroundImage = bitmap;
                // Calculate the graphics path based on the bitmap supplied
                //计算位图中不透明部分的边界
                GraphicsPath graphicsPath = CalculateControlGraphicsPath(bitmap);
                // Apply new region
                //应用新的区域
                button.Region = new Region(graphicsPath);
            }
        }
        ///
        /// Calculate the graphics path that representing the figure in the bitmap
        /// excluding the transparent color which is the top left pixel.
        /// //计算位图中不透明部分的边界
        ///
        /// The Bitmap object to calculate our graphics path from
        /// Calculated graphics path
        private static GraphicsPath CalculateControlGraphicsPath(Bitmap bitmap)
        {
            // Create GraphicsPath for our bitmap calculation
            //创建 GraphicsPath
            GraphicsPath graphicsPath = new GraphicsPath();
            // Use the top left pixel as our transparent color
            //使用左上角的一点的颜色作为我们透明色
            Color colorTransparent = bitmap.GetPixel(0, 0);
            // This is to store the column value where an opaque pixel is first found.
            // This value will determine where we start scanning for trailing opaque pixels.
            //第一个找到点的X
            int colOpaquePixel = 0;
            // Go through all rows (Y axis)
            // 偏历所有行（Y方向）
            for (int row = 0; row < bitmap.Height; row++)
            {
                // Reset value
                //重设
                colOpaquePixel = 0;
                // Go through all columns (X axis)
                //偏历所有列（X方向）
                for (int col = 0; col < bitmap.Width; col++)
                {
                    // If this is an opaque pixel, mark it and search for anymore trailing behind
                    //如果是不需要透明处理的点则标记，然后继续偏历
                    if (bitmap.GetPixel(col, row) != colorTransparent)
                    {
                        // Opaque pixel found, mark current position
                        //记录当前
                        colOpaquePixel = col;
                        // Create another variable to set the current pixel position
                        //建立新变量来记录当前点
                        int colNext = col;
                        // Starting from current found opaque pixel, search for anymore opaque pixels
                        // trailing behind, until a transparent   pixel is found or minimum width is reached
                        ///从找到的不透明点开始，继续寻找不透明点,一直到找到或则达到图片宽度
                        for (colNext = colOpaquePixel; colNext < bitmap.Width; colNext++)
                            if (bitmap.GetPixel(colNext, row) == colorTransparent)
                                break;
                        // Form a rectangle for line of opaque   pixels found and add it to our graphics path
                        //将不透明点加到graphics path
                        graphicsPath.AddRectangle(new Rectangle(colOpaquePixel, row, colNext - colOpaquePixel, 1));
                        // No need to scan the line of opaque pixels just found
                        col = colNext;
                    }
                }
            }
            // Return calculated graphics path
            return graphicsPath;
        }

        #endregion


        #region 无边框可拖动窗体   已弃用

        [DllImport("user32.dll")]

        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]

        public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int IParam);

        public const int WM_SYSCOMMAND = 0x0112;

        public const int SC_MOVE = 0xF010;

        public const int HTCAPTION = 0x0002;

        private void FrmMain_MouseDown(object sender, MouseEventArgs e)
        {
            //拖动窗体

            this.Cursor = System.Windows.Forms.Cursors.Hand;//改变鼠标样式

            ReleaseCapture();

            SendMessage(this.Handle, WM_SYSCOMMAND, SC_MOVE + HTCAPTION, 0);

            this.Cursor = System.Windows.Forms.Cursors.Default;

        }

        #endregion






    }
}

