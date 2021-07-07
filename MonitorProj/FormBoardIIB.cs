using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using System.Resources;
using System.Threading;
using System.Reflection;

namespace MonitorProj
{
    public partial class FormBoardIIB : Form
    {


        #region 参数定义

        private enum_SerialCOMType mySerialCOMType;
        public enum_SerialCOMType MySerialCOMType
        {
            get
            {
                return mySerialCOMType;
            }
            set
            {
                mySerialCOMType = value;
            }
        }

        //板卡类型
        private enum_BoardType myBoardType;
        public enum_BoardType MyBoardType
        {
            get
            {
                return myBoardType;
            }
            set
            {
                myBoardType = value;
            }
        }

        //板卡子类型，16功能阀箱，分为国外版、国产版
        private enum_BoardTypeClass myBoardTypeClass;
        public enum_BoardTypeClass MyBoardTypeClass
        {
            get
            {
                return myBoardTypeClass;
            }
            set
            {
                myBoardTypeClass = value;
            }
        }

        private string name;
        public string NameIn
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                this.Name = NameIn;
            }
        }

        private string infoRunning;
        public string InfoRunning
        {
            get
            {
                return infoRunning;
            }
            set
            {
                infoRunning = value;
            }
        }

        private IPAddress addressLocal;
        public IPAddress AddressLocal
        {
            get
            {
                return addressLocal;
            }
            set
            {
                addressLocal = value;
            }
        }

        private int portLocal;
        public int PortLocal
        {
            get
            {
                return portLocal;
            }
            set
            {
                portLocal = value;
            }
        }

        private IPAddress addressRemote;
        public IPAddress AddressRemote
        {
            get
            {
                return addressRemote;
            }
            set
            {
                addressRemote = value;
            }
        }

        private int portRemote;
        public int PortRemote
        {
            get
            {
                return portRemote;
            }
            set
            {
                portRemote = value;
            }
        }

        private string[] sQuirys = null;
        public string[] SQuirys
        {
            get
            {
                return sQuirys;
            }
            set
            {
                sQuirys = value;
            }
        }

        private int quiryInterval = 100;
        public int QuiryInterval
        {
            get
            {
                return quiryInterval;
            }
            set
            {
                quiryInterval = value;
            }
        }

        private bool isConnected = false;
        public bool IsConnected
        {
            get
            {
                return isConnected;
            }
        }

        public event EventHandler EventSerialDataSend;
        public event EventHandler EventBtnStatusChanged;

        private byte[, ][] cmdBytesA = new byte[17, 2][];
        private byte[, ][] cmdBytesB = new byte[17, 2][];

        private Dictionary<string, Button> dicBtnByName = new Dictionary<string, Button>();


        #endregion

        public FormBoardIIB()
        {
            InitializeComponent();


            #region 16功能阀箱
            ImageList imageList = new ImageList();
            imageList.Images.Add(Properties.Resources.Range2);
            imageList.Images.Add(Properties.Resources.Range1);

            #region 按钮具体功能提示

            this.btn_A1.ImageList = imageList;
            this.btn_A1.ImageIndex = 0;

            this.btn_A2.ImageList = imageList;
            this.btn_A2.ImageIndex = 0;

            this.btn_A3.ImageList = imageList;
            this.btn_A3.ImageIndex = 0;

            this.btn_A4.ImageList = imageList;
            this.btn_A4.ImageIndex = 0;

            this.btn_A5.ImageList = imageList;
            this.btn_A5.ImageIndex = 0;

            this.btn_A6.ImageList = imageList;
            this.btn_A6.ImageIndex = 0;

            this.btn_A7.ImageList = imageList;
            this.btn_A7.ImageIndex = 0;

            this.btn_A8.ImageList = imageList;
            this.btn_A8.ImageIndex = 0;

            this.btn_A9.ImageList = imageList;
            this.btn_A9.ImageIndex = 0;

            this.btn_A10.ImageList = imageList;
            this.btn_A10.ImageIndex = 0;

            this.btn_A11.ImageList = imageList;
            this.btn_A11.ImageIndex = 0;

            this.btn_A12.ImageList = imageList;
            this.btn_A12.ImageIndex = 0;

            this.btn_A13.ImageList = imageList;
            this.btn_A13.ImageIndex = 0;

            this.btn_A14.ImageList = imageList;
            this.btn_A14.ImageIndex = 0;

            this.btn_A15.ImageList = imageList;
            this.btn_A15.ImageIndex = 0;

            this.btn_A16.ImageList = imageList;
            this.btn_A16.ImageIndex = 0;

            this.btn_B1.ImageList = imageList;
            this.btn_B1.ImageIndex = 0;

            this.btn_B2.ImageList = imageList;
            this.btn_B2.ImageIndex = 0;

            this.btn_B3.ImageList = imageList;
            this.btn_B3.ImageIndex = 0;

            this.btn_B4.ImageList = imageList;
            this.btn_B4.ImageIndex = 0;

            this.btn_B5.ImageList = imageList;
            this.btn_B5.ImageIndex = 0;

            this.btn_B6.ImageList = imageList;
            this.btn_B6.ImageIndex = 0;

            this.btn_B7.ImageList = imageList;
            this.btn_B7.ImageIndex = 0;

            this.btn_B8.ImageList = imageList;
            this.btn_B8.ImageIndex = 0;

            this.btn_B9.ImageList = imageList;
            this.btn_B9.ImageIndex = 0;

            this.btn_B10.ImageList = imageList;
            this.btn_B10.ImageIndex = 0;

            this.btn_B11.ImageList = imageList;
            this.btn_B11.ImageIndex = 0;

            this.btn_B12.ImageList = imageList;
            this.btn_B12.ImageIndex = 0;

            this.btn_B13.ImageList = imageList;
            this.btn_B13.ImageIndex = 0;

            this.btn_B14.ImageList = imageList;
            this.btn_B14.ImageIndex = 0;

            this.btn_B15.ImageList = imageList;
            this.btn_B15.ImageIndex = 0;

            this.btn_B16.ImageList = imageList;
            this.btn_B16.ImageIndex = 0;

            dicBtnByName.Add(this.btn_A1.Name, this.btn_A1);
            dicBtnByName.Add(this.btn_A2.Name, this.btn_A2);
            dicBtnByName.Add(this.btn_A3.Name, this.btn_A3);
            dicBtnByName.Add(this.btn_A4.Name, this.btn_A4);
            dicBtnByName.Add(this.btn_A5.Name, this.btn_A5);
            dicBtnByName.Add(this.btn_A6.Name, this.btn_A6);
            dicBtnByName.Add(this.btn_A7.Name, this.btn_A7);
            dicBtnByName.Add(this.btn_A8.Name, this.btn_A8);
            dicBtnByName.Add(this.btn_A9.Name, this.btn_A9);
            dicBtnByName.Add(this.btn_A10.Name, this.btn_A10);
            dicBtnByName.Add(this.btn_A11.Name, this.btn_A11);
            dicBtnByName.Add(this.btn_A12.Name, this.btn_A12);
            dicBtnByName.Add(this.btn_A13.Name, this.btn_A13);
            dicBtnByName.Add(this.btn_A14.Name, this.btn_A14);
            dicBtnByName.Add(this.btn_A15.Name, this.btn_A15);
            dicBtnByName.Add(this.btn_A16.Name, this.btn_A16);
            dicBtnByName.Add(this.btn_B1.Name, this.btn_B1);
            dicBtnByName.Add(this.btn_B2.Name, this.btn_B2);
            dicBtnByName.Add(this.btn_B3.Name, this.btn_B3);
            dicBtnByName.Add(this.btn_B4.Name, this.btn_B4);
            dicBtnByName.Add(this.btn_B5.Name, this.btn_B5);
            dicBtnByName.Add(this.btn_B6.Name, this.btn_B6);
            dicBtnByName.Add(this.btn_B7.Name, this.btn_B7);
            dicBtnByName.Add(this.btn_B8.Name, this.btn_B8);
            dicBtnByName.Add(this.btn_B9.Name, this.btn_B9);
            dicBtnByName.Add(this.btn_B10.Name, this.btn_B10);
            dicBtnByName.Add(this.btn_B11.Name, this.btn_B11);
            dicBtnByName.Add(this.btn_B12.Name, this.btn_B12);
            dicBtnByName.Add(this.btn_B13.Name, this.btn_B13);
            dicBtnByName.Add(this.btn_B14.Name, this.btn_B14);
            dicBtnByName.Add(this.btn_B15.Name, this.btn_B15);
            dicBtnByName.Add(this.btn_B16.Name, this.btn_B16);

            /*
             * A4-8
                左右推进器A
                动力头反转
                小油缸上升+棘轮A
                钻进
                旋转油缸逆时针动作
             * B1-8
                液压源高压+动力头高速
                钻进油缸中压+水泵
                钻进油缸高压
                左右推进器B
                动力头正转
                小油缸下压+棘轮B
                提钻
                旋转油缸顺时针动作
             * A9-16
                旋转夹紧
                机械手空闲到接杆
                机械手抓取位
                插销
                钻管架顺时针旋转
                机械手夹紧
                下夹紧(下两层)
                动力头浮动
             * B9-16
                旋转松开
                机械手空闲位
                机械手抓取到接杆
                拔销
                钻管架逆时针旋转
                机械手松开
                下夹开(下两层)
                提升油缸浮动
             */
            #endregion

            #region 按钮点击事件

            this.btn_A1.Click += new EventHandler(btn_AB_Click);
            this.btn_A2.Click += new EventHandler(btn_AB_Click);
            this.btn_A3.Click += new EventHandler(btn_AB_Click);
            this.btn_A4.Click += new EventHandler(btn_AB_Click);
            this.btn_A5.Click += new EventHandler(btn_AB_Click);
            this.btn_A6.Click += new EventHandler(btn_AB_Click);
            this.btn_A7.Click += new EventHandler(btn_AB_Click);
            this.btn_A8.Click += new EventHandler(btn_AB_Click);
            this.btn_A9.Click += new EventHandler(btn_AB_Click);
            this.btn_A10.Click += new EventHandler(btn_AB_Click);
            this.btn_A11.Click += new EventHandler(btn_AB_Click);
            this.btn_A12.Click += new EventHandler(btn_AB_Click);
            this.btn_A13.Click += new EventHandler(btn_AB_Click);
            this.btn_A14.Click += new EventHandler(btn_AB_Click);
            this.btn_A15.Click += new EventHandler(btn_AB_Click);
            this.btn_A16.Click += new EventHandler(btn_AB_Click);

            this.btn_B1.Click += new EventHandler(btn_AB_Click);
            this.btn_B2.Click += new EventHandler(btn_AB_Click);
            this.btn_B3.Click += new EventHandler(btn_AB_Click);
            this.btn_B4.Click += new EventHandler(btn_AB_Click);
            this.btn_B5.Click += new EventHandler(btn_AB_Click);
            this.btn_B6.Click += new EventHandler(btn_AB_Click);
            this.btn_B7.Click += new EventHandler(btn_AB_Click);
            this.btn_B8.Click += new EventHandler(btn_AB_Click);
            this.btn_B9.Click += new EventHandler(btn_AB_Click);
            this.btn_B10.Click += new EventHandler(btn_AB_Click);
            this.btn_B11.Click += new EventHandler(btn_AB_Click);
            this.btn_B12.Click += new EventHandler(btn_AB_Click);
            this.btn_B13.Click += new EventHandler(btn_AB_Click);
            this.btn_B14.Click += new EventHandler(btn_AB_Click);
            this.btn_B15.Click += new EventHandler(btn_AB_Click);
            this.btn_B16.Click += new EventHandler(btn_AB_Click);

            #endregion

            #region 阀开关指令数据

            {
                cmdBytesA[1, 0] = new byte[] { 0xC1, 0x05, 0x00, 0x01, 0x00, 0x00, 0x8D, 0x0A };//阀A1关闭	
                cmdBytesA[1, 1] = new byte[] { 0xC1, 0x05, 0x00, 0x01, 0xFF, 0x00, 0xCC, 0xFA };//阀A1打开
                cmdBytesA[2, 0] = new byte[] { 0xC1, 0x05, 0x00, 0x02, 0x00, 0x00, 0x7D, 0x0A };//阀A2关闭	
                cmdBytesA[2, 1] = new byte[] { 0xC1, 0x05, 0x00, 0x02, 0xFF, 0x00, 0x3C, 0xFA };//阀A2打开
                cmdBytesA[3, 0] = new byte[] { 0xC1, 0x05, 0x00, 0x03, 0x00, 0x00, 0x2C, 0xCA };//阀A3关闭	
                cmdBytesA[3, 1] = new byte[] { 0xC1, 0x05, 0x00, 0x03, 0xFF, 0x00, 0x6D, 0x3A };//阀A3打开
                cmdBytesA[4, 0] = new byte[] { 0xC1, 0x05, 0x00, 0x04, 0x00, 0x00, 0x9D, 0x0B };//阀A4关闭	
                cmdBytesA[4, 1] = new byte[] { 0xC1, 0x05, 0x00, 0x04, 0xFF, 0x00, 0xDC, 0xFB };//阀A4打开
                cmdBytesA[5, 0] = new byte[] { 0xC1, 0x05, 0x00, 0x05, 0x00, 0x00, 0xCC, 0xCB };//阀A5关闭	
                cmdBytesA[5, 1] = new byte[] { 0xC1, 0x05, 0x00, 0x05, 0xFF, 0x00, 0x8D, 0x3B };//阀A5打开
                cmdBytesA[6, 0] = new byte[] { 0xC1, 0x05, 0x00, 0x06, 0x00, 0x00, 0x3C, 0xCB };//阀A6关闭	
                cmdBytesA[6, 1] = new byte[] { 0xC1, 0x05, 0x00, 0x06, 0xFF, 0x00, 0x7D, 0x3B };//阀A6打开
                cmdBytesA[7, 0] = new byte[] { 0xC1, 0x05, 0x00, 0x07, 0x00, 0x00, 0x6D, 0x0B };//阀A7关闭	
                cmdBytesA[7, 1] = new byte[] { 0xC1, 0x05, 0x00, 0x07, 0xFF, 0x00, 0x2C, 0xFB };//阀A7打开
                cmdBytesA[8, 0] = new byte[] { 0xC1, 0x05, 0x00, 0x08, 0x00, 0x00, 0x5D, 0x08 };//阀A8关闭	
                cmdBytesA[8, 1] = new byte[] { 0xC1, 0x05, 0x00, 0x08, 0xFF, 0x00, 0x1C, 0xF8 };//阀A8打开
                cmdBytesA[9, 0] = new byte[] { 0xC1, 0x05, 0x00, 0x09, 0x00, 0x00, 0x0C, 0xC8 };//阀A9关闭	
                cmdBytesA[9, 1] = new byte[] { 0xC1, 0x05, 0x00, 0x09, 0xFF, 0x00, 0x4D, 0x38 };//阀A9打开
                cmdBytesA[10, 0] = new byte[] { 0xC1, 0x05, 0x00, 0x0A, 0x00, 0x00, 0xFC, 0xC8 };//阀A10关闭	
                cmdBytesA[10, 1] = new byte[] { 0xC1, 0x05, 0x00, 0x0A, 0xFF, 0x00, 0xBD, 0x38 };//阀A10打开
                cmdBytesA[11, 0] = new byte[] { 0xC1, 0x05, 0x00, 0x0B, 0x00, 0x00, 0xAD, 0x08 };//阀A11关闭	
                cmdBytesA[11, 1] = new byte[] { 0xC1, 0x05, 0x00, 0x0B, 0xFF, 0x00, 0xEC, 0xF8 };//阀A11打开
                cmdBytesA[12, 0] = new byte[] { 0xC1, 0x05, 0x00, 0x0C, 0x00, 0x00, 0x1C, 0xC9 };//阀A12关闭	
                cmdBytesA[12, 1] = new byte[] { 0xC1, 0x05, 0x00, 0x0C, 0xFF, 0x00, 0x5D, 0x39 };//阀A12打开
                cmdBytesA[13, 0] = new byte[] { 0xC1, 0x05, 0x00, 0x0D, 0x00, 0x00, 0x4D, 0x09 };//阀A13关闭	
                cmdBytesA[13, 1] = new byte[] { 0xC1, 0x05, 0x00, 0x0D, 0xFF, 0x00, 0x0C, 0xF9 };//阀A13打开
                cmdBytesA[14, 0] = new byte[] { 0xC1, 0x05, 0x00, 0x0E, 0x00, 0x00, 0xBD, 0x09 };//阀A14关闭	
                cmdBytesA[14, 1] = new byte[] { 0xC1, 0x05, 0x00, 0x0E, 0xFF, 0x00, 0xFC, 0xF9 };//阀A14打开
                cmdBytesA[15, 0] = new byte[] { 0xC1, 0x05, 0x00, 0x0F, 0x00, 0x00, 0xEC, 0xC9 };//阀A15关闭	
                cmdBytesA[15, 1] = new byte[] { 0xC1, 0x05, 0x00, 0x0F, 0xFF, 0x00, 0xAD, 0x39 };//阀A15打开
                cmdBytesA[16, 0] = new byte[] { 0xC1, 0x05, 0x00, 0x10, 0x00, 0x00, 0xDD, 0x0F };//阀A16关闭	
                cmdBytesA[16, 1] = new byte[] { 0xC1, 0x05, 0x00, 0x10, 0xFF, 0x00, 0x9C, 0xFF };//阀A16打开

                cmdBytesB[1, 0] = new byte[] { 0xC1, 0x05, 0x00, 0x11, 0x00, 0x00, 0x8C, 0xCF };//阀B1关闭	
                cmdBytesB[1, 1] = new byte[] { 0xC1, 0x05, 0x00, 0x11, 0xFF, 0x00, 0xCD, 0x3F };//阀B1打开
                cmdBytesB[2, 0] = new byte[] { 0xC1, 0x05, 0x00, 0x12, 0x00, 0x00, 0x7C, 0xCF };//阀B2关闭	
                cmdBytesB[2, 1] = new byte[] { 0xC1, 0x05, 0x00, 0x12, 0xFF, 0x00, 0x3D, 0x3F };//阀B2打开
                cmdBytesB[3, 0] = new byte[] { 0xC1, 0x05, 0x00, 0x13, 0x00, 0x00, 0x2D, 0x0F };//阀B3关闭	
                cmdBytesB[3, 1] = new byte[] { 0xC1, 0x05, 0x00, 0x13, 0xFF, 0x00, 0x6C, 0xFF };//阀B3打开
                cmdBytesB[4, 0] = new byte[] { 0xC1, 0x05, 0x00, 0x14, 0x00, 0x00, 0x9C, 0xCE };//阀B4关闭	
                cmdBytesB[4, 1] = new byte[] { 0xC1, 0x05, 0x00, 0x14, 0xFF, 0x00, 0xDD, 0x3E };//阀B4打开
                cmdBytesB[5, 0] = new byte[] { 0xC1, 0x05, 0x00, 0x15, 0x00, 0x00, 0xCD, 0x0E };//阀B5关闭	
                cmdBytesB[5, 1] = new byte[] { 0xC1, 0x05, 0x00, 0x15, 0xFF, 0x00, 0x8C, 0xFE };//阀B5打开
                cmdBytesB[6, 0] = new byte[] { 0xC1, 0x05, 0x00, 0x16, 0x00, 0x00, 0x3D, 0x0E };//阀B6关闭	
                cmdBytesB[6, 1] = new byte[] { 0xC1, 0x05, 0x00, 0x16, 0xFF, 0x00, 0x7C, 0xFE };//阀B6打开
                cmdBytesB[7, 0] = new byte[] { 0xC1, 0x05, 0x00, 0x17, 0x00, 0x00, 0x6C, 0xCE };//阀B7关闭	
                cmdBytesB[7, 1] = new byte[] { 0xC1, 0x05, 0x00, 0x17, 0xFF, 0x00, 0x2D, 0x3E };//阀B7打开
                cmdBytesB[8, 0] = new byte[] { 0xC1, 0x05, 0x00, 0x18, 0x00, 0x00, 0x5C, 0xCD };//阀B8关闭	
                cmdBytesB[8, 1] = new byte[] { 0xC1, 0x05, 0x00, 0x18, 0xFF, 0x00, 0x1D, 0x3D };//阀B8打开
                cmdBytesB[9, 0] = new byte[] { 0xC1, 0x05, 0x00, 0x19, 0x00, 0x00, 0x0D, 0x0D };//阀B9关闭	
                cmdBytesB[9, 1] = new byte[] { 0xC1, 0x05, 0x00, 0x19, 0xFF, 0x00, 0x4C, 0xFD };//阀B9打开
                cmdBytesB[10, 0] = new byte[] { 0xC1, 0x05, 0x00, 0x1A, 0x00, 0x00, 0xFD, 0x0D };//阀B10关闭	
                cmdBytesB[10, 1] = new byte[] { 0xC1, 0x05, 0x00, 0x1A, 0xFF, 0x00, 0xBC, 0xFD };//阀B10打开
                cmdBytesB[11, 0] = new byte[] { 0xC1, 0x05, 0x00, 0x1B, 0x00, 0x00, 0xAC, 0xCD };//阀B11关闭	
                cmdBytesB[11, 1] = new byte[] { 0xC1, 0x05, 0x00, 0x1B, 0xFF, 0x00, 0xED, 0x3D };//阀B11打开
                cmdBytesB[12, 0] = new byte[] { 0xC1, 0x05, 0x00, 0x1C, 0x00, 0x00, 0x1D, 0x0C };//阀B12关闭	
                cmdBytesB[12, 1] = new byte[] { 0xC1, 0x05, 0x00, 0x1C, 0xFF, 0x00, 0x5C, 0xFC };//阀B12打开
                cmdBytesB[13, 0] = new byte[] { 0xC1, 0x05, 0x00, 0x1D, 0x00, 0x00, 0x4C, 0xCC };//阀B13关闭	
                cmdBytesB[13, 1] = new byte[] { 0xC1, 0x05, 0x00, 0x1D, 0xFF, 0x00, 0x0D, 0x3C };//阀B13打开
                cmdBytesB[14, 0] = new byte[] { 0xC1, 0x05, 0x00, 0x1E, 0x00, 0x00, 0xBC, 0xCC };//阀B14关闭	
                cmdBytesB[14, 1] = new byte[] { 0xC1, 0x05, 0x00, 0x1E, 0xFF, 0x00, 0xFD, 0x3C };//阀B14打开
                cmdBytesB[15, 0] = new byte[] { 0xC1, 0x05, 0x00, 0x1F, 0x00, 0x00, 0xED, 0x0C };//阀B15关闭	
                cmdBytesB[15, 1] = new byte[] { 0xC1, 0x05, 0x00, 0x1F, 0xFF, 0x00, 0xAC, 0xFC };//阀B15打开
                cmdBytesB[16, 0] = new byte[] { 0xC1, 0x05, 0x00, 0x20, 0x00, 0x00, 0xDD, 0x00 };//阀B16关闭	
                cmdBytesB[16, 1] = new byte[] { 0xC1, 0x05, 0x00, 0x20, 0xFF, 0x00, 0x9C, 0xF0 };//阀B16打开

            }

            #endregion
            #endregion


            #region Btn的状态变化事件

            foreach (Control ctl in this.Controls)
            {
                if (ctl is Button)
                {
                    Button btn = ctl as Button;
                    btn.BackColorChanged += new EventHandler(Btn_Status_BackColor_Changed);
                }
                else
                {
                    foreach (Control ctl1 in ctl.Controls)
                    {
                        if (ctl1 is Button)
                        {
                            Button btn = ctl1 as Button;
                            btn.BackColorChanged += new EventHandler(Btn_Status_BackColor_Changed);
                        }
                        else if (ctl1 is GroupBox)
                        {
                            foreach (Control ctl2 in ctl.Controls)
                            {
                                if (ctl2 is Button)
                                {
                                    Button btn = ctl2 as Button;
                                    btn.BackColorChanged += new EventHandler(Btn_Status_BackColor_Changed);
                                }
                                else if (ctl2 is GroupBox)
                                {
                                    foreach (Control ctl3 in ctl2.Controls)
                                    {
                                        if (ctl3 is Button)
                                        {
                                            Button btn = ctl3 as Button;
                                            btn.BackColorChanged += new EventHandler(Btn_Status_BackColor_Changed);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            #endregion

        }




        public void btn_Perform_Click(Button btn, EventArgs e)
        {
            try
            {
                Type t = typeof(Button);
                object[] p = new object[1];
                MethodInfo m = t.GetMethod("OnClick", BindingFlags.NonPublic | BindingFlags.Instance);
                p[0] = EventArgs.Empty;
                m.Invoke(btn, p);
                return;
            }
            catch (Exception ex)
            { }
        }

        //背景色变化会有4次事件触发，仅取第一次
        private int indexBackColorEvenSend = 0;
        private void Btn_Status_BackColor_Changed(object sender, EventArgs e)
        {
            try
            {
                if (indexBackColorEvenSend == 0)
                {

                    Button btn = sender as Button;

                    GEventArgs gEventArgs = new GEventArgs();
                    gEventArgs.dataType = 10;//button状态(BackColor)变化信息
                    gEventArgs.myStruct_Btn_Status_EventSend = new Struct_Btn_Status_EventSend();
                    gEventArgs.myStruct_Btn_Status_EventSend.sName = btn.Name;
                    gEventArgs.myStruct_Btn_Status_EventSend.backColor = btn.BackColor;
                    gEventArgs.myStruct_Btn_Status_EventSend.imageIndex = btn.ImageIndex;
                    if (EventBtnStatusChanged != null)
                    {
                        EventBtnStatusChanged(btn, gEventArgs);
                    }
                    indexBackColorEvenSend++;
                }
                else if (indexBackColorEvenSend > 0 && indexBackColorEvenSend < 4)
                {
                    indexBackColorEvenSend++;
                    if (indexBackColorEvenSend >= 4)
                    {
                        indexBackColorEvenSend = 0;
                    }
                }
            }
            catch (Exception ex)
            { }
        }

        private void Btn_Status_ImageIndex_Changed(object sender, EventArgs e)
        {
            try
            {
                Button btn = sender as Button;

                GEventArgs gEventArgs = new GEventArgs();
                gEventArgs.dataType = 11;//button状态(ImageIndex)变化信息
                gEventArgs.myStruct_Btn_Status_EventSend = new Struct_Btn_Status_EventSend();
                gEventArgs.myStruct_Btn_Status_EventSend.sName = btn.Name;
                gEventArgs.myStruct_Btn_Status_EventSend.backColor = btn.BackColor;
                gEventArgs.myStruct_Btn_Status_EventSend.imageIndex = btn.ImageIndex;
                if (EventBtnStatusChanged != null)
                {
                    EventBtnStatusChanged(btn, gEventArgs);
                }
            }
            catch (Exception ex)
            { }
        }

        private bool Flag_Btn_AB_Click_Finished = true;
        public void btn_AB_Click(object sender, EventArgs e)
        {
            try
            {
                if (Flag_Btn_AB_Click_Finished)
                {
                    threadCmdSercoValvePackSendClick = new Thread(new ParameterizedThreadStart(threadCmdSercoValvePackSendClickFunc));
                    Flag_Btn_AB_Click_Finished = false;
                    threadCmdSercoValvePackSendClick.Start(sender);
                }
                else
                {
                    MessageBox.Show("操作太快！");
                }
            }
            catch (Exception ex)
            { }
        }

        #region 16功能阀箱操作线程
        private Thread threadCmdSercoValvePackSendClick;
        private int CmdInternalDelay = 50;//16功能阀箱互斥开关两条指令间发送时间间隔，ms
        private void threadCmdSercoValvePackSendClickFunc(object sender)
        {
            try
            {
                CmdInternalDelay = Global.delay_CmdSend_ResultComp;
                Button btn = sender as Button;
                int indexImage = btn.ImageIndex;
                string sName = btn.Name;
                string sNameOth;
                int indexBtn = 0;
                string sIndexBtn = "";
                bool flagA = false;
                string sNameA;
                string sNameB;
                string sTextA;
                string sTextB;

                if (sName.Contains("A"))
                {
                    sNameOth = sName.Replace("A", "B");
                    sIndexBtn = sName.Replace("btn_A", "");
                    flagA = true;
                    sNameA = sName;
                    sNameB = sNameOth;
                    sTextA = sNameA.Replace("btn_", "功能阀");
                    sTextB = sNameB.Replace("btn_", "功能阀");
                }
                else if (sName.Contains("B"))
                {
                    sNameOth = sName.Replace("B", "A");
                    sIndexBtn = sName.Replace("btn_B", "");
                    flagA = false;
                    sNameA = sNameOth;
                    sNameB = sName;
                    sTextA = sNameA.Replace("btn_", "功能阀");
                    sTextB = sNameB.Replace("btn_", "功能阀");
                }
                else
                {
                    return;
                }

                string sName_A = dicBtnByName[sNameA].Name;
                Color backColor_A = dicBtnByName[sNameA].BackColor;
                int imageIndex_A = dicBtnByName[sNameA].ImageIndex;
                string sToolTipA = this.ToolTip_Button.GetToolTip(dicBtnByName[sNameA]);
                string sName_B = dicBtnByName[sNameB].Name;
                Color backColor_B = dicBtnByName[sNameB].BackColor;
                int imageIndex_B = dicBtnByName[sNameB].ImageIndex;
                string sToolTipB = this.ToolTip_Button.GetToolTip(dicBtnByName[sNameB]);

                bool bb = Int32.TryParse(sIndexBtn, out indexBtn);
                if (bb == false)
                {
                    return;
                }

                //获得对应互斥的按钮
                Button btn_Oth = dicBtnByName[sNameOth];

                GEventArgs gEventArgsA = new GEventArgs();
                gEventArgsA.dataType = 12;//button按钮状态变化信息，对于16功能阀箱
                gEventArgsA.myStruct_Btn_Status_EventSend = new Struct_Btn_Status_EventSend();

                GEventArgs gEventArgsB = new GEventArgs();
                gEventArgsB.dataType = 12;//button按钮状态变化信息，对于16功能阀箱
                gEventArgsB.myStruct_Btn_Status_EventSend = new Struct_Btn_Status_EventSend();

                GEventArgs gEventArgsBtnOper = new GEventArgs();
                gEventArgsBtnOper.dataType = 13;//button按钮最终执行结果，对于16功能阀箱
                gEventArgsBtnOper.isBtnOperOK = false;

                string sDataLineForTxt = "";

                // (indexImage == 0)//此为点击按钮功能由关-->开
                // (indexImage == 1)//此为点击按钮功能由开-->关

                if ((indexImage == 0 && flagA == true) || (indexImage == 1 && flagA == false))
                {
                    //A由关-->开，(indexImage == 0 && flagA == true)；//B由开-->关，(indexImage == 1 && flagA == false)
                    //先关闭B
                    Global.bFlagCmdDataServoValvePackSendOK = false;
                    Global.bCmdDataServoValvePackSend = cmdBytesB[indexBtn, 0];
                    Global.myBoardServoValvePackIIBClass.BCmdInsert(cmdBytesB[indexBtn, 0]);

                    sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t" +
                        sTextB + "(" + sToolTipB + ")关\t";

                    Thread.Sleep(CmdInternalDelay);
                    if (Global.bFlagCmdDataServoValvePackSendOK)//B已正确关闭
                    {
                        sDataLineForTxt += "返回正确";
                        Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                        Global.myLogStreamWriter.Flush();

                        if (flagA == true)
                        {
                            btn_Oth.ImageIndex = 0;
                        }
                        else if (flagA == false)
                        {
                            btn.ImageIndex = 0;
                        }

                        gEventArgsB.myStruct_Btn_Status_EventSend.sName = sName_B;
                        gEventArgsB.myStruct_Btn_Status_EventSend.backColor = backColor_B;
                        gEventArgsB.myStruct_Btn_Status_EventSend.imageIndex = imageIndex_B;
                        gEventArgsB.myStruct_Btn_Status_EventSend.sInfo = sDataLineForTxt.Replace("\t", ",");
                        if (EventBtnStatusChanged != null)
                        {
                            EventBtnStatusChanged(btn, gEventArgsB);
                        }

                        //仅点击按钮为打开时（0->1），才关闭另外一个，再开此个；当为关闭时（1->0）,关闭此个后，不开另外一个
                        if (indexImage == 0)
                        {
                            //再打开A
                            Global.bFlagCmdDataServoValvePackSendOK = false;
                            Global.bCmdDataServoValvePackSend = cmdBytesA[indexBtn, 1];
                            Global.myBoardServoValvePackIIBClass.BCmdInsert(cmdBytesA[indexBtn, 1]);

                            sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t" +
                                sTextA + "(" + sToolTipA + ")开\t";

                            Thread.Sleep(CmdInternalDelay);
                            if (Global.bFlagCmdDataServoValvePackSendOK)//A已正确打开
                            {
                                sDataLineForTxt += "返回正确";
                                Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                                Global.myLogStreamWriter.Flush();

                                gEventArgsBtnOper.isBtnOperOK = true;

                                gEventArgsA.myStruct_Btn_Status_EventSend.sName = sName_A;
                                gEventArgsA.myStruct_Btn_Status_EventSend.backColor = backColor_A;
                                gEventArgsA.myStruct_Btn_Status_EventSend.imageIndex = imageIndex_A;
                                gEventArgsA.myStruct_Btn_Status_EventSend.sInfo = sDataLineForTxt.Replace("\t", ",");
                                if (EventBtnStatusChanged != null)
                                {
                                    EventBtnStatusChanged(btn, gEventArgsA);
                                }

                                if (flagA == true)
                                {
                                    btn.ImageIndex = 1;
                                    btn_Oth.ImageIndex = 0;
                                }
                                else if (flagA == false)
                                {
                                    btn.ImageIndex = 0;
                                    btn_Oth.ImageIndex = 1;
                                }
                            }
                            else//A没有正确打开
                            {
                                sDataLineForTxt += "返回错误";
                                Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                                Global.myLogStreamWriter.Flush();

                                gEventArgsA.myStruct_Btn_Status_EventSend.sName = sName_A;
                                gEventArgsA.myStruct_Btn_Status_EventSend.backColor = backColor_A;
                                gEventArgsA.myStruct_Btn_Status_EventSend.imageIndex = imageIndex_A;
                                gEventArgsA.myStruct_Btn_Status_EventSend.sInfo = sDataLineForTxt.Replace("\t", ",");
                                if (EventBtnStatusChanged != null)
                                {
                                    EventBtnStatusChanged(btn, gEventArgsA);
                                }

                                //btn.ImageIndex = 0;
                                //btn_Oth.ImageIndex = 0;
                            }
                        }
                        else
                        {
                            gEventArgsBtnOper.isBtnOperOK = true;
                        }
                    }
                    else
                    {
                        sDataLineForTxt += "返回错误";
                        Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                        Global.myLogStreamWriter.Flush();

                        gEventArgsB.myStruct_Btn_Status_EventSend.sName = sName_B;
                        gEventArgsB.myStruct_Btn_Status_EventSend.backColor = backColor_B;
                        gEventArgsB.myStruct_Btn_Status_EventSend.imageIndex = imageIndex_B;
                        gEventArgsB.myStruct_Btn_Status_EventSend.sInfo = sDataLineForTxt.Replace("\t", ",");
                        if (EventBtnStatusChanged != null)
                        {
                            EventBtnStatusChanged(btn, gEventArgsB);
                        }

                        //btn.ImageIndex = 0;
                        //btn_Oth.ImageIndex = 0;
                    }
                }
                else if ((indexImage == 0 && flagA == false) || (indexImage == 1 && flagA == true))
                {
                    //B由关-->开, (indexImage == 0 && flagA == false)；//A由开-->关,(indexImage == 1 && flagA == true)
                    //先关闭A
                    Global.bFlagCmdDataServoValvePackSendOK = false;
                    Global.bCmdDataServoValvePackSend = cmdBytesA[indexBtn, 0];
                    Global.myBoardServoValvePackIIBClass.BCmdInsert(cmdBytesA[indexBtn, 0]);

                    sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t" +
                        sTextA + "(" + sToolTipA + ")关\t";

                    Thread.Sleep(CmdInternalDelay);
                    if (Global.bFlagCmdDataServoValvePackSendOK)//A已正确关闭
                    {
                        sDataLineForTxt += "返回正确";
                        Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                        Global.myLogStreamWriter.Flush();

                        if (flagA == false)
                        {
                            btn_Oth.ImageIndex = 0;
                        }
                        else if (flagA == true)
                        {
                            btn.ImageIndex = 0;
                        }

                        gEventArgsA.myStruct_Btn_Status_EventSend.sName = sName_A;
                        gEventArgsA.myStruct_Btn_Status_EventSend.backColor = backColor_A;
                        gEventArgsA.myStruct_Btn_Status_EventSend.imageIndex = imageIndex_A;
                        gEventArgsA.myStruct_Btn_Status_EventSend.sInfo = sDataLineForTxt.Replace("\t", ",");
                        if (EventBtnStatusChanged != null)
                        {
                            EventBtnStatusChanged(btn, gEventArgsA);
                        }

                        //仅点击按钮为打开时（0->1），才关闭另外一个，再开此个；当为关闭时（1->0）,关闭此个后，不开另外一个
                        if (indexImage == 0)
                        {
                            //再打开B
                            Global.bFlagCmdDataServoValvePackSendOK = false;
                            Global.bCmdDataServoValvePackSend = cmdBytesB[indexBtn, 1];
                            Global.myBoardServoValvePackIIBClass.BCmdInsert(cmdBytesB[indexBtn, 1]);

                            sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t" +
                                sTextB + "(" + sToolTipB + ")开\t";

                            Thread.Sleep(CmdInternalDelay);
                            if (Global.bFlagCmdDataServoValvePackSendOK)//B已正确打开
                            {
                                sDataLineForTxt += "返回正确";
                                Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                                Global.myLogStreamWriter.Flush();

                                gEventArgsBtnOper.isBtnOperOK = true;

                                gEventArgsB.myStruct_Btn_Status_EventSend.sName = sName_B;
                                gEventArgsB.myStruct_Btn_Status_EventSend.backColor = backColor_B;
                                gEventArgsB.myStruct_Btn_Status_EventSend.imageIndex = imageIndex_B;
                                gEventArgsB.myStruct_Btn_Status_EventSend.sInfo = sDataLineForTxt.Replace("\t", ",");
                                if (EventBtnStatusChanged != null)
                                {
                                    EventBtnStatusChanged(btn, gEventArgsB);
                                }

                                if (flagA == false)
                                {
                                    btn.ImageIndex = 1;
                                    btn_Oth.ImageIndex = 0;
                                }
                                else if (flagA == true)
                                {
                                    btn.ImageIndex = 0;
                                    btn_Oth.ImageIndex = 1;
                                }
                            }
                            else//B没有正确打开
                            {
                                sDataLineForTxt += "返回错误";
                                Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                                Global.myLogStreamWriter.Flush();

                                gEventArgsB.myStruct_Btn_Status_EventSend.sName = sName_B;
                                gEventArgsB.myStruct_Btn_Status_EventSend.backColor = backColor_B;
                                gEventArgsB.myStruct_Btn_Status_EventSend.imageIndex = imageIndex_B;
                                gEventArgsB.myStruct_Btn_Status_EventSend.sInfo = sDataLineForTxt.Replace("\t", ",");
                                if (EventBtnStatusChanged != null)
                                {
                                    EventBtnStatusChanged(btn, gEventArgsB);
                                }

                                //btn.ImageIndex = 0;
                                //btn_Oth.ImageIndex = 0;
                            }
                        }
                        else
                        {
                            gEventArgsBtnOper.isBtnOperOK = true;
                        }
                    }
                    else
                    {
                        sDataLineForTxt += "返回错误";
                        Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                        Global.myLogStreamWriter.Flush();

                        gEventArgsA.myStruct_Btn_Status_EventSend.sName = sName_A;
                        gEventArgsA.myStruct_Btn_Status_EventSend.backColor = backColor_A;
                        gEventArgsA.myStruct_Btn_Status_EventSend.imageIndex = imageIndex_A;
                        gEventArgsA.myStruct_Btn_Status_EventSend.sInfo = sDataLineForTxt.Replace("\t", ",");
                        if (EventBtnStatusChanged != null)
                        {
                            EventBtnStatusChanged(btn, gEventArgsA);
                        }

                        //btn.ImageIndex = 0;
                        //btn_Oth.ImageIndex = 0;
                    }
                }

                //最终执行结果，isBtnOperOK
                if (EventBtnStatusChanged != null)
                {
                    EventBtnStatusChanged(btn, gEventArgsBtnOper);
                }

                Global.bFlagCmdDataServoValvePackSendOK = false;
                Flag_Btn_AB_Click_Finished = true;
            }
            catch (Exception ex)
            {
                string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t" + ex.ToString();
                Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                Global.myLogStreamWriter.Flush();
            }
        }
        #endregion


        public bool Initialize()
        {
            try
            {
                Global.myBoardServoValvePackIIBClass = new BoardServoValvePackIIClass()
                {
                    AddressLocal = AddressLocal,
                    PortLocal = PortLocal,
                    AddressRemote = AddressRemote,
                    PortRemote = PortRemote,
                    SQuirys = SQuirys,
                    QuiryInterval = QuiryInterval,
                    Name = NameIn,
                    MyBoardType = MyBoardType,
                    MySerialCOMType = mySerialCOMType,
                    MyBoardTypeClass = myBoardTypeClass
                };
                Global.myBoardServoValvePackIIBClass.EventSerialDataSend += new EventHandler(ReceiveDataFunc);
                Global.myBoardServoValvePackIIBClass.EventSerialDataSend += new EventHandler(Global.myDataRecvToSaveClass.ReceiveDataFromSerial);
                isConnected = Global.myBoardServoValvePackIIBClass.Initialize();

                GEventArgs myGEventArgs = new GEventArgs();
                myGEventArgs.nameSerial = this.Name;
                myGEventArgs.dataType = 1;

                if (isConnected)
                {
                    myGEventArgs.connected = true;
                    if (EventSerialDataSend != null)
                    {
                        EventSerialDataSend(this, myGEventArgs);
                    }
                    return true;
                }
                else
                {
                    myGEventArgs.connected = false;
                    if (EventSerialDataSend != null)
                    {
                        EventSerialDataSend(this, myGEventArgs);
                    }
                    StopSerial();
                    return false;
                }

            }
            catch (Exception ex)
            {
                return false;
            }
        }



        //数据接收
        private delegate void UpdateFormDelegate(object sender, GEventArgs gEventArgs);
        public void ReceiveDataFunc(object sender, EventArgs e)
        {
            try
            {
                GEventArgs gEventArgs = (GEventArgs)e;
                if (EventSerialDataSend != null)
                {
                    EventSerialDataSend(this, gEventArgs);
                }

                UpdateFormDelegate updateFormDelegate = new UpdateFormDelegate(UpdateFormFromModelsState);
                this.BeginInvoke(updateFormDelegate, new object[] { sender, gEventArgs });
            }
            catch (Exception ex)
            { }
        }

        private void UpdateFormFromModelsState(object sender, GEventArgs gEventArgs)
        {
            try
            {
                if (gEventArgs.dataType == 0)
                {
                }
                else if (gEventArgs.dataType == 5)//5-接收到的原始数据+解析后的数据                
                {
                    if (gEventArgs.addressBoard == enum_AddressBoard.ServoValvePacketBoard16Func)
                    {
                        Struct_BoardB_Status myStructBoardIIStatus = (Struct_BoardB_Status)gEventArgs.objParse;

                        textBox_V.Text = myStructBoardIIStatus.V.ToString();
                        textBox_I.Text = myStructBoardIIStatus.I.ToString();
                        textBox_P.Text = myStructBoardIIStatus.P.ToString();
                        if(myStructBoardIIStatus.isLeakage == false)
                        {
                            textBox_Leakage.BackColor = Color.Green;
                        }
                        else
                        {
                            textBox_Leakage.BackColor = Color.Red;
                        }

                    }
                }
                else if (gEventArgs.dataType == 6)//6-接收到的原始执行指令返回数据
                {

                }
            }
            catch (Exception ex)
            { }
        }

        public bool StopSerial()
        {
            try
            {
                Global.myBoardServoValvePackIIBClass.StopSerial();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private void FormBoardIIB_VisibleChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.Visible == true)
                {
                    this.Size = this.Parent.Size;
                    this.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            { }
        }

        private void FormBoardIIB_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                StopSerial();
            }
            catch (Exception ex)
            { }
        }





    }
}
