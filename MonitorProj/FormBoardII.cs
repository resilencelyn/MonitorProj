using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using DevComponents.DotNetBar.Controls;
using System.Reflection;

namespace MonitorProj
{
    public partial class FormBoardII : Form
    {

        public event EventHandler EventBtnStatusChanged;

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

        #endregion


        public FormBoardII()
        {
            InitializeComponent();


            #region 16功能阀箱
            ImageList imageList = new ImageList();
            imageList.Images.Add(Properties.Resources.Range2);
            imageList.Images.Add(Properties.Resources.Range1);

            #region 按钮具体功能提示

            this.btn_PWM1.ImageList = imageList;
            this.btn_PWM1.ImageIndex = 0;

            this.btn_PWM2.ImageList = imageList;
            this.btn_PWM2.ImageIndex = 0;

            this.btn_PWM3.ImageList = imageList;
            this.btn_PWM3.ImageIndex = 0;

            this.btn_PWM4.ImageList = imageList;
            this.btn_PWM4.ImageIndex = 0;

            this.btn_PWM5.ImageList = imageList;
            this.btn_PWM5.ImageIndex = 0;

            this.btn_PWM6.ImageList = imageList;
            this.btn_PWM6.ImageIndex = 0;

            this.btn_PWM7.ImageList = imageList;
            this.btn_PWM7.ImageIndex = 0;

            this.btn_PWM8.ImageList = imageList;
            this.btn_PWM8.ImageIndex = 0;

            this.btn_PWM9.ImageList = imageList;
            this.btn_PWM9.ImageIndex = 0;

            this.btn_PWM10.ImageList = imageList;
            this.btn_PWM10.ImageIndex = 0;

            this.btn_PWM11.ImageList = imageList;
            this.btn_PWM11.ImageIndex = 0;

            this.btn_PWM12.ImageList = imageList;
            this.btn_PWM12.ImageIndex = 0;

            this.btn_PWM13.ImageList = imageList;
            this.btn_PWM13.ImageIndex = 0;

            this.btn_PWM14.ImageList = imageList;
            this.btn_PWM14.ImageIndex = 0;

            this.btn_PWM15.ImageList = imageList;
            this.btn_PWM15.ImageIndex = 0;

            this.btn_PWM16.ImageList = imageList;
            this.btn_PWM16.ImageIndex = 0;


            this.btn_DOUT1.ImageList = imageList;
            this.btn_DOUT1.ImageIndex = 0;

            this.btn_DOUT2.ImageList = imageList;
            this.btn_DOUT2.ImageIndex = 0;

            this.btn_DOUT3.ImageList = imageList;
            this.btn_DOUT3.ImageIndex = 0;

            this.btn_DOUT4.ImageList = imageList;
            this.btn_DOUT4.ImageIndex = 0;

            this.btn_DOUT5.ImageList = imageList;
            this.btn_DOUT5.ImageIndex = 0;

            this.btn_DOUT6.ImageList = imageList;
            this.btn_DOUT6.ImageIndex = 0;

            this.btn_DOUT7.ImageList = imageList;
            this.btn_DOUT7.ImageIndex = 0;

            this.btn_DOUT8.ImageList = imageList;
            this.btn_DOUT8.ImageIndex = 0;

            this.btn_DOUT9.ImageList = imageList;
            this.btn_DOUT9.ImageIndex = 0;

            this.btn_DOUT10.ImageList = imageList;
            this.btn_DOUT10.ImageIndex = 0;

            this.btn_DOUT11.ImageList = imageList;
            this.btn_DOUT11.ImageIndex = 0;

            this.btn_DOUT12.ImageList = imageList;
            this.btn_DOUT12.ImageIndex = 0;

            this.btn_DOUT13.ImageList = imageList;
            this.btn_DOUT13.ImageIndex = 0;

            this.btn_DOUT14.ImageList = imageList;
            this.btn_DOUT14.ImageIndex = 0;

            this.btn_DOUT15.ImageList = imageList;
            this.btn_DOUT15.ImageIndex = 0;

            this.btn_DOUT16.ImageList = imageList;
            this.btn_DOUT16.ImageIndex = 0;


            this.btn_DOUT16.ImageList = imageList;
            this.btn_DOUT16.ImageIndex = 0;

            this.btn_SSUP1.ImageList = imageList;
            this.btn_SSUP1.ImageIndex = 0;

            this.btn_SSUP2.ImageList = imageList;
            this.btn_SSUP2.ImageIndex = 0;

            this.btn_SSUP3.ImageList = imageList;
            this.btn_SSUP3.ImageIndex = 0;

            this.btn_SSUP4.ImageList = imageList;
            this.btn_SSUP4.ImageIndex = 0;

            #endregion

            #region 按钮点击事件

            this.btn_PWM1.Click += new EventHandler(btn_Operate_Click);
            this.btn_PWM2.Click += new EventHandler(btn_Operate_Click);
            this.btn_PWM3.Click += new EventHandler(btn_Operate_Click);
            this.btn_PWM4.Click += new EventHandler(btn_Operate_Click);
            this.btn_PWM5.Click += new EventHandler(btn_Operate_Click);
            this.btn_PWM6.Click += new EventHandler(btn_Operate_Click);
            this.btn_PWM7.Click += new EventHandler(btn_Operate_Click);
            this.btn_PWM8.Click += new EventHandler(btn_Operate_Click);
            this.btn_PWM9.Click += new EventHandler(btn_Operate_Click);
            this.btn_PWM10.Click += new EventHandler(btn_Operate_Click);
            this.btn_PWM11.Click += new EventHandler(btn_Operate_Click);
            this.btn_PWM12.Click += new EventHandler(btn_Operate_Click);
            this.btn_PWM13.Click += new EventHandler(btn_Operate_Click);
            this.btn_PWM14.Click += new EventHandler(btn_Operate_Click);
            this.btn_PWM15.Click += new EventHandler(btn_Operate_Click);
            this.btn_PWM16.Click += new EventHandler(btn_Operate_Click);

            this.btn_DOUT1.Click += new EventHandler(btn_Operate_Click);
            this.btn_DOUT2.Click += new EventHandler(btn_Operate_Click);
            this.btn_DOUT3.Click += new EventHandler(btn_Operate_Click);
            this.btn_DOUT4.Click += new EventHandler(btn_Operate_Click);
            this.btn_DOUT5.Click += new EventHandler(btn_Operate_Click);
            this.btn_DOUT6.Click += new EventHandler(btn_Operate_Click);
            this.btn_DOUT7.Click += new EventHandler(btn_Operate_Click);
            this.btn_DOUT8.Click += new EventHandler(btn_Operate_Click);
            this.btn_DOUT9.Click += new EventHandler(btn_Operate_Click);
            this.btn_DOUT10.Click += new EventHandler(btn_Operate_Click);
            this.btn_DOUT11.Click += new EventHandler(btn_Operate_Click);
            this.btn_DOUT12.Click += new EventHandler(btn_Operate_Click);
            this.btn_DOUT13.Click += new EventHandler(btn_Operate_Click);
            this.btn_DOUT14.Click += new EventHandler(btn_Operate_Click);
            this.btn_DOUT15.Click += new EventHandler(btn_Operate_Click);
            this.btn_DOUT16.Click += new EventHandler(btn_Operate_Click);

            this.btn_SSUP1.Click += new EventHandler(btn_Operate_Click);
            this.btn_SSUP2.Click += new EventHandler(btn_Operate_Click);
            this.btn_SSUP3.Click += new EventHandler(btn_Operate_Click);
            this.btn_SSUP4.Click += new EventHandler(btn_Operate_Click);

            #endregion


            #region 开环/闭环切换事件

            this.checkBox_PWMOpenLoop_1_2.CheckedChanged += new EventHandler(checkBox_PWMOpenLoop_CheckedChanged);
            this.checkBox_PWMOpenLoop_3_4.CheckedChanged += new EventHandler(checkBox_PWMOpenLoop_CheckedChanged);
            this.checkBox_PWMOpenLoop_5_6.CheckedChanged += new EventHandler(checkBox_PWMOpenLoop_CheckedChanged);
            this.checkBox_PWMOpenLoop_7_8.CheckedChanged += new EventHandler(checkBox_PWMOpenLoop_CheckedChanged);
            this.checkBox_PWMOpenLoop_9_10.CheckedChanged += new EventHandler(checkBox_PWMOpenLoop_CheckedChanged);
            this.checkBox_PWMOpenLoop_11_12.CheckedChanged += new EventHandler(checkBox_PWMOpenLoop_CheckedChanged);
            this.checkBox_PWMOpenLoop_13_14.CheckedChanged += new EventHandler(checkBox_PWMOpenLoop_CheckedChanged);
            this.checkBox_PWMOpenLoop_15_16.CheckedChanged += new EventHandler(checkBox_PWMOpenLoop_CheckedChanged);

            #endregion

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



        private void checkBox_PWMOpenLoop_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                CheckBox cb = sender as CheckBox;
                string name = cb.Name;
                switch (name)
                {

                    #region PWM1_16_Open_Loop_OnOff
                    case "checkBox_PWMOpenLoop_1_2":
                        if (!cb.Checked)
                        {
                            byte_PWM1_16_Open_Loop_OnOff |= 0x01;
                            label_PWMCurrentSetPoint_1_2.Text = "%";
                            numericUpDown_PWMCurrentSetPoint_1_2.Minimum = 0;
                            numericUpDown_PWMCurrentSetPoint_1_2.Maximum = 100;
                        }
                        else
                        {
                            byte_PWM1_16_Open_Loop_OnOff &= BitConverter.GetBytes(~(0x01))[0];
                            label_PWMCurrentSetPoint_1_2.Text = "mA";
                            numericUpDown_PWMCurrentSetPoint_1_2.Minimum = 0;
                            numericUpDown_PWMCurrentSetPoint_1_2.Maximum = 750;
                        }
                        break;

                    case "checkBox_PWMOpenLoop_3_4":
                        if (!cb.Checked)
                        {
                            byte_PWM1_16_Open_Loop_OnOff |= (0x01) << 1;
                            label_PWMCurrentSetPoint_3_4.Text = "%";
                            numericUpDown_PWMCurrentSetPoint_3_4.Minimum = 0;
                            numericUpDown_PWMCurrentSetPoint_3_4.Maximum = 100;
                        }
                        else
                        {
                            byte_PWM1_16_Open_Loop_OnOff &= BitConverter.GetBytes(~((0x01) << 1))[0];
                            label_PWMCurrentSetPoint_3_4.Text = "mA";
                            numericUpDown_PWMCurrentSetPoint_3_4.Minimum = 0;
                            numericUpDown_PWMCurrentSetPoint_3_4.Maximum = 750;
                        }
                        break;

                    case "checkBox_PWMOpenLoop_5_6":
                        if (!cb.Checked)
                        {
                            byte_PWM1_16_Open_Loop_OnOff |= (0x01) << 2;
                            label_PWMCurrentSetPoint_5_6.Text = "%";
                            numericUpDown_PWMCurrentSetPoint_5_6.Minimum = 0;
                            numericUpDown_PWMCurrentSetPoint_5_6.Maximum = 100;
                        }
                        else
                        {
                            byte_PWM1_16_Open_Loop_OnOff &= BitConverter.GetBytes(~((0x01) << 2))[0];
                            label_PWMCurrentSetPoint_5_6.Text = "mA";
                            numericUpDown_PWMCurrentSetPoint_5_6.Minimum = 0;
                            numericUpDown_PWMCurrentSetPoint_5_6.Maximum = 750;
                        }
                        break;

                    case "checkBox_PWMOpenLoop_7_8":
                        if (!cb.Checked)
                        {
                            byte_PWM1_16_Open_Loop_OnOff |= (0x01) << 3;
                            label_PWMCurrentSetPoint_7_8.Text = "%";
                            numericUpDown_PWMCurrentSetPoint_7_8.Minimum = 0;
                            numericUpDown_PWMCurrentSetPoint_7_8.Maximum = 100;
                        }
                        else
                        {
                            byte_PWM1_16_Open_Loop_OnOff &= BitConverter.GetBytes(~((0x01) << 3))[0];
                            label_PWMCurrentSetPoint_7_8.Text = "mA";
                            numericUpDown_PWMCurrentSetPoint_7_8.Minimum = 0;
                            numericUpDown_PWMCurrentSetPoint_7_8.Maximum = 750;
                        }
                        break;

                    case "checkBox_PWMOpenLoop_9_10":
                        if (!cb.Checked)
                        {
                            byte_PWM1_16_Open_Loop_OnOff |= (0x01) << 4;
                            label_PWMCurrentSetPoint_9_10.Text = "%";
                            numericUpDown_PWMCurrentSetPoint_9_10.Minimum = 0;
                            numericUpDown_PWMCurrentSetPoint_9_10.Maximum = 100;
                        }
                        else
                        {
                            byte_PWM1_16_Open_Loop_OnOff &= BitConverter.GetBytes(~((0x01) << 4))[0];
                            label_PWMCurrentSetPoint_9_10.Text = "mA";
                            numericUpDown_PWMCurrentSetPoint_9_10.Minimum = 0;
                            numericUpDown_PWMCurrentSetPoint_9_10.Maximum = 750;
                        }
                        break;

                    case "checkBox_PWMOpenLoop_11_12":
                        if (!cb.Checked)
                        {
                            byte_PWM1_16_Open_Loop_OnOff |= (0x01) << 5;
                            label_PWMCurrentSetPoint_11_12.Text = "%";
                            numericUpDown_PWMCurrentSetPoint_11_12.Minimum = 0;
                            numericUpDown_PWMCurrentSetPoint_11_12.Maximum = 100;
                        }
                        else
                        {
                            byte_PWM1_16_Open_Loop_OnOff &= BitConverter.GetBytes(~((0x01) << 5))[0];
                            label_PWMCurrentSetPoint_11_12.Text = "mA";
                            numericUpDown_PWMCurrentSetPoint_11_12.Minimum = 0;
                            numericUpDown_PWMCurrentSetPoint_11_12.Maximum = 750;
                        }
                        break;

                    case "checkBox_PWMOpenLoop_13_14":
                        if (!cb.Checked)
                        {
                            byte_PWM1_16_Open_Loop_OnOff |= (0x01) << 6;
                            label_PWMCurrentSetPoint_13_14.Text = "%";
                            numericUpDown_PWMCurrentSetPoint_13_14.Minimum = 0;
                            numericUpDown_PWMCurrentSetPoint_13_14.Maximum = 100;
                        }
                        else
                        {
                            byte_PWM1_16_Open_Loop_OnOff &= BitConverter.GetBytes(~((0x01) << 6))[0];
                            label_PWMCurrentSetPoint_13_14.Text = "mA";
                            numericUpDown_PWMCurrentSetPoint_13_14.Minimum = 0;
                            numericUpDown_PWMCurrentSetPoint_13_14.Maximum = 750;
                        }
                        break;

                    case "checkBox_PWMOpenLoop_15_16":
                        if (!cb.Checked)
                        {
                            byte_PWM1_16_Open_Loop_OnOff |= (0x01) << 7;
                            label_PWMCurrentSetPoint_15_16.Text = "%";
                            numericUpDown_PWMCurrentSetPoint_15_16.Minimum = 0;
                            numericUpDown_PWMCurrentSetPoint_15_16.Maximum = 100;
                        }
                        else
                        {
                            byte_PWM1_16_Open_Loop_OnOff &= BitConverter.GetBytes(~((0x01) << 7))[0];
                            label_PWMCurrentSetPoint_15_16.Text = "mA";
                            numericUpDown_PWMCurrentSetPoint_15_16.Minimum = 0;
                            numericUpDown_PWMCurrentSetPoint_15_16.Maximum = 750;
                        }
                        break;

                    #endregion

                    default:
                        break;
                }
            }
            catch (Exception ex)
            { }
        }


        private byte byte_PWM1_8_OnOff = 0x00;
        private byte byte_PWM9_16_OnOff = 0x00;
        private byte byte_DOUT1_8_OnOff = 0x00;
        private byte byte_DOUT9_16_OnOff = 0x00;
        private byte byte_SSUP1_4_OnOff = 0x00;
        private byte byte_PWM1_16_Open_Loop_OnOff = 0x00;
        public void btn_Operate_Click(object sender, EventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                string name = btn.Name;

                if (btn.ImageIndex == 0)
                {
                    btn.ImageIndex = 1;
                }
                else
                {
                    btn.ImageIndex = 0;
                }

                switch (name)
                {
                    #region PWM1_8_OnOff
                    case "btn_PWM1":
                        if (btn.ImageIndex == 1)
                        {
                            byte_PWM1_8_OnOff |= 0x01;
                        }
                        else
                        {
                            byte_PWM1_8_OnOff &= BitConverter.GetBytes(~(0x01))[0];
                        }
                        break;

                    case "btn_PWM2":
                        if (btn.ImageIndex == 1)
                        {
                            byte_PWM1_8_OnOff |= (0x01) << 1;
                        }
                        else
                        {
                            byte_PWM1_8_OnOff &= BitConverter.GetBytes(~((0x01) << 1))[0];
                        }
                        break;

                    case "btn_PWM3":
                        if (btn.ImageIndex == 1)
                        {
                            byte_PWM1_8_OnOff |= (0x01) << 2;
                        }
                        else
                        {
                            byte_PWM1_8_OnOff &= BitConverter.GetBytes(~((0x01) << 2))[0];
                        }
                        break;

                    case "btn_PWM4":
                        if (btn.ImageIndex == 1)
                        {
                            byte_PWM1_8_OnOff |= (0x01) << 3;
                        }
                        else
                        {
                            byte_PWM1_8_OnOff &= BitConverter.GetBytes(~((0x01) << 3))[0];
                        }
                        break;

                    case "btn_PWM5":
                        if (btn.ImageIndex == 1)
                        {
                            byte_PWM1_8_OnOff |= (0x01) << 4;
                        }
                        else
                        {
                            byte_PWM1_8_OnOff &= BitConverter.GetBytes(~((0x01) << 4))[0];
                        }
                        break;

                    case "btn_PWM6":
                        if (btn.ImageIndex == 1)
                        {
                            byte_PWM1_8_OnOff |= (0x01) << 5;
                        }
                        else
                        {
                            byte_PWM1_8_OnOff &= BitConverter.GetBytes(~((0x01) << 5))[0];
                        }
                        break;

                    case "btn_PWM7":
                        if (btn.ImageIndex == 1)
                        {
                            byte_PWM1_8_OnOff |= (0x01) << 6;
                        }
                        else
                        {
                            byte_PWM1_8_OnOff &= BitConverter.GetBytes(~((0x01) << 6))[0];
                        }
                        break;

                    case "btn_PWM8":
                        if (btn.ImageIndex == 1)
                        {
                            byte_PWM1_8_OnOff |= (0x01) << 7;
                        }
                        else
                        {
                            byte_PWM1_8_OnOff &= BitConverter.GetBytes(~((0x01) << 7))[0];
                        }
                        break;

                    #endregion

                    #region PWM9_16_OnOff
                    case "btn_PWM9":
                        if (btn.ImageIndex == 1)
                        {
                            byte_PWM9_16_OnOff |= 0x01;
                        }
                        else
                        {
                            byte_PWM9_16_OnOff &= BitConverter.GetBytes(~(0x01))[0];
                        }
                        break;

                    case "btn_PWM10":
                        if (btn.ImageIndex == 1)
                        {
                            byte_PWM9_16_OnOff |= (0x01) << 1;
                        }
                        else
                        {
                            byte_PWM9_16_OnOff &= BitConverter.GetBytes(~((0x01) << 1))[0];
                        }
                        break;

                    case "btn_PWM11":
                        if (btn.ImageIndex == 1)
                        {
                            byte_PWM9_16_OnOff |= (0x01) << 2;
                        }
                        else
                        {
                            byte_PWM9_16_OnOff &= BitConverter.GetBytes(~((0x01) << 2))[0];
                        }
                        break;

                    case "btn_PWM12":
                        if (btn.ImageIndex == 1)
                        {
                            byte_PWM9_16_OnOff |= (0x01) << 3;
                        }
                        else
                        {
                            byte_PWM9_16_OnOff &= BitConverter.GetBytes(~((0x01) << 3))[0];
                        }
                        break;

                    case "btn_PWM13":
                        if (btn.ImageIndex == 1)
                        {
                            byte_PWM9_16_OnOff |= (0x01) << 4;
                        }
                        else
                        {
                            byte_PWM9_16_OnOff &= BitConverter.GetBytes(~((0x01) << 4))[0];
                        }
                        break;

                    case "btn_PWM14":
                        if (btn.ImageIndex == 1)
                        {
                            byte_PWM9_16_OnOff |= (0x01) << 5;
                        }
                        else
                        {
                            byte_PWM9_16_OnOff &= BitConverter.GetBytes(~((0x01) << 5))[0];
                        }
                        break;

                    case "btn_PWM15":
                        if (btn.ImageIndex == 1)
                        {
                            byte_PWM9_16_OnOff |= (0x01) << 6;
                        }
                        else
                        {
                            byte_PWM9_16_OnOff &= BitConverter.GetBytes(~((0x01) << 6))[0];
                        }
                        break;

                    case "btn_PWM16":
                        if (btn.ImageIndex == 1)
                        {
                            byte_PWM9_16_OnOff |= (0x01) << 7;
                        }
                        else
                        {
                            byte_PWM9_16_OnOff &= BitConverter.GetBytes(~((0x01) << 7))[0];
                        }
                        break;

                    #endregion

                    #region DOUT1_8_OnOff
                    case "btn_DOUT1":
                        if (btn.ImageIndex == 1)
                        {
                            byte_DOUT1_8_OnOff |= 0x01;
                        }
                        else
                        {
                            byte_DOUT1_8_OnOff &= BitConverter.GetBytes(~(0x01))[0];
                        }
                        break;

                    case "btn_DOUT2":
                        if (btn.ImageIndex == 1)
                        {
                            byte_DOUT1_8_OnOff |= (0x01) << 1;
                        }
                        else
                        {
                            byte_DOUT1_8_OnOff &= BitConverter.GetBytes(~((0x01) << 1))[0];
                        }
                        break;

                    case "btn_DOUT3":
                        if (btn.ImageIndex == 1)
                        {
                            byte_DOUT1_8_OnOff |= (0x01) << 2;
                        }
                        else
                        {
                            byte_DOUT1_8_OnOff &= BitConverter.GetBytes(~((0x01) << 2))[0];
                        }
                        break;

                    case "btn_DOUT4":
                        if (btn.ImageIndex == 1)
                        {
                            byte_DOUT1_8_OnOff |= (0x01) << 3;
                        }
                        else
                        {
                            byte_DOUT1_8_OnOff &= BitConverter.GetBytes(~((0x01) << 3))[0];
                        }
                        break;

                    case "btn_DOUT5":
                        if (btn.ImageIndex == 1)
                        {
                            byte_DOUT1_8_OnOff |= (0x01) << 4;
                        }
                        else
                        {
                            byte_DOUT1_8_OnOff &= BitConverter.GetBytes(~((0x01) << 4))[0];
                        }
                        break;

                    case "btn_DOUT6":
                        if (btn.ImageIndex == 1)
                        {
                            byte_DOUT1_8_OnOff |= (0x01) << 5;
                        }
                        else
                        {
                            byte_DOUT1_8_OnOff &= BitConverter.GetBytes(~((0x01) << 5))[0];
                        }
                        break;

                    case "btn_DOUT7":
                        if (btn.ImageIndex == 1)
                        {
                            byte_DOUT1_8_OnOff |= (0x01) << 6;
                        }
                        else
                        {
                            byte_DOUT1_8_OnOff &= BitConverter.GetBytes(~((0x01) << 6))[0];
                        }
                        break;

                    case "btn_DOUT8":
                        if (btn.ImageIndex == 1)
                        {
                            byte_DOUT1_8_OnOff |= (0x01) << 7;
                        }
                        else
                        {
                            byte_DOUT1_8_OnOff &= BitConverter.GetBytes(~((0x01) << 7))[0];
                        }
                        break;

                    #endregion

                    #region DOUT9_16_OnOff
                    case "btn_DOUT9":
                        if (btn.ImageIndex == 1)
                        {
                            byte_DOUT9_16_OnOff |= 0x01;
                        }
                        else
                        {
                            byte_DOUT9_16_OnOff &= BitConverter.GetBytes(~(0x01))[0];
                        }
                        break;

                    case "btn_DOUT10":
                        if (btn.ImageIndex == 1)
                        {
                            byte_DOUT9_16_OnOff |= (0x01) << 1;
                        }
                        else
                        {
                            byte_DOUT9_16_OnOff &= BitConverter.GetBytes(~((0x01) << 1))[0];
                        }
                        break;

                    case "btn_DOUT11":
                        if (btn.ImageIndex == 1)
                        {
                            byte_DOUT9_16_OnOff |= (0x01) << 2;
                        }
                        else
                        {
                            byte_DOUT9_16_OnOff &= BitConverter.GetBytes(~((0x01) << 2))[0];
                        }
                        break;

                    case "btn_DOUT12":
                        if (btn.ImageIndex == 1)
                        {
                            byte_DOUT9_16_OnOff |= (0x01) << 3;
                        }
                        else
                        {
                            byte_DOUT9_16_OnOff &= BitConverter.GetBytes(~((0x01) << 3))[0];
                        }
                        break;

                    case "btn_DOUT13":
                        if (btn.ImageIndex == 1)
                        {
                            byte_DOUT9_16_OnOff |= (0x01) << 4;
                        }
                        else
                        {
                            byte_DOUT9_16_OnOff &= BitConverter.GetBytes(~((0x01) << 4))[0];
                        }
                        break;

                    case "btn_DOUT14":
                        if (btn.ImageIndex == 1)
                        {
                            byte_DOUT9_16_OnOff |= (0x01) << 5;
                        }
                        else
                        {
                            byte_DOUT9_16_OnOff &= BitConverter.GetBytes(~((0x01) << 5))[0];
                        }
                        break;

                    case "btn_DOUT15":
                        if (btn.ImageIndex == 1)
                        {
                            byte_DOUT9_16_OnOff |= (0x01) << 6;
                        }
                        else
                        {
                            byte_DOUT9_16_OnOff &= BitConverter.GetBytes(~((0x01) << 6))[0];
                        }
                        break;

                    case "btn_DOUT16":
                        if (btn.ImageIndex == 1)
                        {
                            byte_DOUT9_16_OnOff |= (0x01) << 7;
                        }
                        else
                        {
                            byte_DOUT9_16_OnOff &= BitConverter.GetBytes(~((0x01) << 7))[0];
                        }
                        break;

                    #endregion

                    #region SSUP1_4_OnOff
                    case "btn_SSUP1":
                        if (btn.ImageIndex == 1)
                        {
                            byte_SSUP1_4_OnOff |= 0x01;
                        }
                        else
                        {
                            byte_SSUP1_4_OnOff &= BitConverter.GetBytes(~(0x01))[0];
                        }
                        break;

                    case "btn_SSUP2":
                        if (btn.ImageIndex == 1)
                        {
                            byte_SSUP1_4_OnOff |= (0x01) << 1;
                        }
                        else
                        {
                            byte_SSUP1_4_OnOff &= BitConverter.GetBytes(~((0x01) << 1))[0];
                        }
                        break;

                    case "btn_SSUP3":
                        if (btn.ImageIndex == 1)
                        {
                            byte_SSUP1_4_OnOff |= (0x01) << 2;
                        }
                        else
                        {
                            byte_SSUP1_4_OnOff &= BitConverter.GetBytes(~((0x01) << 2))[0];
                        }
                        break;

                    case "btn_SSUP4":
                        if (btn.ImageIndex == 1)
                        {
                            byte_SSUP1_4_OnOff |= (0x01) << 3;
                        }
                        else
                        {
                            byte_SSUP1_4_OnOff &= BitConverter.GetBytes(~((0x01) << 3))[0];
                        }
                        break;

                    #endregion

                    default:
                        break;
                }
            }
            catch (Exception ex)
            { }
        }

        //远端操作
        public void btn_Operate_Click_Remote(object sender, EventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                string name = btn.Name;

                switch (name)
                {
                    #region PWM1_8_OnOff
                    case "btn_PWM1":
                        if (btn.ImageIndex == 1)
                        {
                            byte_PWM1_8_OnOff |= 0x01;
                        }
                        else
                        {
                            byte_PWM1_8_OnOff &= BitConverter.GetBytes(~(0x01))[0];
                        }
                        break;

                    case "btn_PWM2":
                        if (btn.ImageIndex == 1)
                        {
                            byte_PWM1_8_OnOff |= (0x01) << 1;
                        }
                        else
                        {
                            byte_PWM1_8_OnOff &= BitConverter.GetBytes(~((0x01) << 1))[0];
                        }
                        break;

                    case "btn_PWM3":
                        if (btn.ImageIndex == 1)
                        {
                            byte_PWM1_8_OnOff |= (0x01) << 2;
                        }
                        else
                        {
                            byte_PWM1_8_OnOff &= BitConverter.GetBytes(~((0x01) << 2))[0];
                        }
                        break;

                    case "btn_PWM4":
                        if (btn.ImageIndex == 1)
                        {
                            byte_PWM1_8_OnOff |= (0x01) << 3;
                        }
                        else
                        {
                            byte_PWM1_8_OnOff &= BitConverter.GetBytes(~((0x01) << 3))[0];
                        }
                        break;

                    case "btn_PWM5":
                        if (btn.ImageIndex == 1)
                        {
                            byte_PWM1_8_OnOff |= (0x01) << 4;
                        }
                        else
                        {
                            byte_PWM1_8_OnOff &= BitConverter.GetBytes(~((0x01) << 4))[0];
                        }
                        break;

                    case "btn_PWM6":
                        if (btn.ImageIndex == 1)
                        {
                            byte_PWM1_8_OnOff |= (0x01) << 5;
                        }
                        else
                        {
                            byte_PWM1_8_OnOff &= BitConverter.GetBytes(~((0x01) << 5))[0];
                        }
                        break;

                    case "btn_PWM7":
                        if (btn.ImageIndex == 1)
                        {
                            byte_PWM1_8_OnOff |= (0x01) << 6;
                        }
                        else
                        {
                            byte_PWM1_8_OnOff &= BitConverter.GetBytes(~((0x01) << 6))[0];
                        }
                        break;

                    case "btn_PWM8":
                        if (btn.ImageIndex == 1)
                        {
                            byte_PWM1_8_OnOff |= (0x01) << 7;
                        }
                        else
                        {
                            byte_PWM1_8_OnOff &= BitConverter.GetBytes(~((0x01) << 7))[0];
                        }
                        break;

                    #endregion

                    #region PWM9_16_OnOff
                    case "btn_PWM9":
                        if (btn.ImageIndex == 1)
                        {
                            byte_PWM9_16_OnOff |= 0x01;
                        }
                        else
                        {
                            byte_PWM9_16_OnOff &= BitConverter.GetBytes(~(0x01))[0];
                        }
                        break;

                    case "btn_PWM10":
                        if (btn.ImageIndex == 1)
                        {
                            byte_PWM9_16_OnOff |= (0x01) << 1;
                        }
                        else
                        {
                            byte_PWM9_16_OnOff &= BitConverter.GetBytes(~((0x01) << 1))[0];
                        }
                        break;

                    case "btn_PWM11":
                        if (btn.ImageIndex == 1)
                        {
                            byte_PWM9_16_OnOff |= (0x01) << 2;
                        }
                        else
                        {
                            byte_PWM9_16_OnOff &= BitConverter.GetBytes(~((0x01) << 2))[0];
                        }
                        break;

                    case "btn_PWM12":
                        if (btn.ImageIndex == 1)
                        {
                            byte_PWM9_16_OnOff |= (0x01) << 3;
                        }
                        else
                        {
                            byte_PWM9_16_OnOff &= BitConverter.GetBytes(~((0x01) << 3))[0];
                        }
                        break;

                    case "btn_PWM13":
                        if (btn.ImageIndex == 1)
                        {
                            byte_PWM9_16_OnOff |= (0x01) << 4;
                        }
                        else
                        {
                            byte_PWM9_16_OnOff &= BitConverter.GetBytes(~((0x01) << 4))[0];
                        }
                        break;

                    case "btn_PWM14":
                        if (btn.ImageIndex == 1)
                        {
                            byte_PWM9_16_OnOff |= (0x01) << 5;
                        }
                        else
                        {
                            byte_PWM9_16_OnOff &= BitConverter.GetBytes(~((0x01) << 5))[0];
                        }
                        break;

                    case "btn_PWM15":
                        if (btn.ImageIndex == 1)
                        {
                            byte_PWM9_16_OnOff |= (0x01) << 6;
                        }
                        else
                        {
                            byte_PWM9_16_OnOff &= BitConverter.GetBytes(~((0x01) << 6))[0];
                        }
                        break;

                    case "btn_PWM16":
                        if (btn.ImageIndex == 1)
                        {
                            byte_PWM9_16_OnOff |= (0x01) << 7;
                        }
                        else
                        {
                            byte_PWM9_16_OnOff &= BitConverter.GetBytes(~((0x01) << 7))[0];
                        }
                        break;

                    #endregion

                    #region DOUT1_8_OnOff
                    case "btn_DOUT1":
                        if (btn.ImageIndex == 1)
                        {
                            byte_DOUT1_8_OnOff |= 0x01;
                        }
                        else
                        {
                            byte_DOUT1_8_OnOff &= BitConverter.GetBytes(~(0x01))[0];
                        }
                        break;

                    case "btn_DOUT2":
                        if (btn.ImageIndex == 1)
                        {
                            byte_DOUT1_8_OnOff |= (0x01) << 1;
                        }
                        else
                        {
                            byte_DOUT1_8_OnOff &= BitConverter.GetBytes(~((0x01) << 1))[0];
                        }
                        break;

                    case "btn_DOUT3":
                        if (btn.ImageIndex == 1)
                        {
                            byte_DOUT1_8_OnOff |= (0x01) << 2;
                        }
                        else
                        {
                            byte_DOUT1_8_OnOff &= BitConverter.GetBytes(~((0x01) << 2))[0];
                        }
                        break;

                    case "btn_DOUT4":
                        if (btn.ImageIndex == 1)
                        {
                            byte_DOUT1_8_OnOff |= (0x01) << 3;
                        }
                        else
                        {
                            byte_DOUT1_8_OnOff &= BitConverter.GetBytes(~((0x01) << 3))[0];
                        }
                        break;

                    case "btn_DOUT5":
                        if (btn.ImageIndex == 1)
                        {
                            byte_DOUT1_8_OnOff |= (0x01) << 4;
                        }
                        else
                        {
                            byte_DOUT1_8_OnOff &= BitConverter.GetBytes(~((0x01) << 4))[0];
                        }
                        break;

                    case "btn_DOUT6":
                        if (btn.ImageIndex == 1)
                        {
                            byte_DOUT1_8_OnOff |= (0x01) << 5;
                        }
                        else
                        {
                            byte_DOUT1_8_OnOff &= BitConverter.GetBytes(~((0x01) << 5))[0];
                        }
                        break;

                    case "btn_DOUT7":
                        if (btn.ImageIndex == 1)
                        {
                            byte_DOUT1_8_OnOff |= (0x01) << 6;
                        }
                        else
                        {
                            byte_DOUT1_8_OnOff &= BitConverter.GetBytes(~((0x01) << 6))[0];
                        }
                        break;

                    case "btn_DOUT8":
                        if (btn.ImageIndex == 1)
                        {
                            byte_DOUT1_8_OnOff |= (0x01) << 7;
                        }
                        else
                        {
                            byte_DOUT1_8_OnOff &= BitConverter.GetBytes(~((0x01) << 7))[0];
                        }
                        break;

                    #endregion

                    #region DOUT9_16_OnOff
                    case "btn_DOUT9":
                        if (btn.ImageIndex == 1)
                        {
                            byte_DOUT9_16_OnOff |= 0x01;
                        }
                        else
                        {
                            byte_DOUT9_16_OnOff &= BitConverter.GetBytes(~(0x01))[0];
                        }
                        break;

                    case "btn_DOUT10":
                        if (btn.ImageIndex == 1)
                        {
                            byte_DOUT9_16_OnOff |= (0x01) << 1;
                        }
                        else
                        {
                            byte_DOUT9_16_OnOff &= BitConverter.GetBytes(~((0x01) << 1))[0];
                        }
                        break;

                    case "btn_DOUT11":
                        if (btn.ImageIndex == 1)
                        {
                            byte_DOUT9_16_OnOff |= (0x01) << 2;
                        }
                        else
                        {
                            byte_DOUT9_16_OnOff &= BitConverter.GetBytes(~((0x01) << 2))[0];
                        }
                        break;

                    case "btn_DOUT12":
                        if (btn.ImageIndex == 1)
                        {
                            byte_DOUT9_16_OnOff |= (0x01) << 3;
                        }
                        else
                        {
                            byte_DOUT9_16_OnOff &= BitConverter.GetBytes(~((0x01) << 3))[0];
                        }
                        break;

                    case "btn_DOUT13":
                        if (btn.ImageIndex == 1)
                        {
                            byte_DOUT9_16_OnOff |= (0x01) << 4;
                        }
                        else
                        {
                            byte_DOUT9_16_OnOff &= BitConverter.GetBytes(~((0x01) << 4))[0];
                        }
                        break;

                    case "btn_DOUT14":
                        if (btn.ImageIndex == 1)
                        {
                            byte_DOUT9_16_OnOff |= (0x01) << 5;
                        }
                        else
                        {
                            byte_DOUT9_16_OnOff &= BitConverter.GetBytes(~((0x01) << 5))[0];
                        }
                        break;

                    case "btn_DOUT15":
                        if (btn.ImageIndex == 1)
                        {
                            byte_DOUT9_16_OnOff |= (0x01) << 6;
                        }
                        else
                        {
                            byte_DOUT9_16_OnOff &= BitConverter.GetBytes(~((0x01) << 6))[0];
                        }
                        break;

                    case "btn_DOUT16":
                        if (btn.ImageIndex == 1)
                        {
                            byte_DOUT9_16_OnOff |= (0x01) << 7;
                        }
                        else
                        {
                            byte_DOUT9_16_OnOff &= BitConverter.GetBytes(~((0x01) << 7))[0];
                        }
                        break;

                    #endregion

                    #region SSUP1_4_OnOff
                    case "btn_SSUP1":
                        if (btn.ImageIndex == 1)
                        {
                            byte_SSUP1_4_OnOff |= 0x01;
                        }
                        else
                        {
                            byte_SSUP1_4_OnOff &= BitConverter.GetBytes(~(0x01))[0];
                        }
                        break;

                    case "btn_SSUP2":
                        if (btn.ImageIndex == 1)
                        {
                            byte_SSUP1_4_OnOff |= (0x01) << 1;
                        }
                        else
                        {
                            byte_SSUP1_4_OnOff &= BitConverter.GetBytes(~((0x01) << 1))[0];
                        }
                        break;

                    case "btn_SSUP3":
                        if (btn.ImageIndex == 1)
                        {
                            byte_SSUP1_4_OnOff |= (0x01) << 2;
                        }
                        else
                        {
                            byte_SSUP1_4_OnOff &= BitConverter.GetBytes(~((0x01) << 2))[0];
                        }
                        break;

                    case "btn_SSUP4":
                        if (btn.ImageIndex == 1)
                        {
                            byte_SSUP1_4_OnOff |= (0x01) << 3;
                        }
                        else
                        {
                            byte_SSUP1_4_OnOff &= BitConverter.GetBytes(~((0x01) << 3))[0];
                        }
                        break;

                    #endregion

                    default:
                        break;
                }
            }
            catch (Exception ex)
            { }
        }


        private enum enumPWMCurrentSetPointMode
        {
            closed_loop = 0,
            open_loop = 1
        };

        private bool PWMCurrentSetPoint(decimal val, enumPWMCurrentSetPointMode mode, out byte[] dataBytes)
        {
            #region
            //In closed loop mode the range is 0x00A5 (approx. 50mA) to 0x0898 (approx 750mA).
            //0x0000 is off.
            //Always set the On/Off bit to On before attempting to control the valve via the current set point.
            //In open loop mode the range is 0x0000 to 0x3FFF, 0% to 100% PWM.
            #endregion

            dataBytes = new byte[2] { 0x00, 0x00 };
            try
            {
                if (mode == enumPWMCurrentSetPointMode.closed_loop)
                {
                    if (val < 50)
                    {
                        dataBytes[0] = 0x00;
                        dataBytes[1] = 0x00;
                    }
                    else if (val > 750)
                    {
                        dataBytes[0] = 0x08;
                        dataBytes[1] = 0x98;
                    }
                    else
                    {
                        double d = Convert.ToDouble(val - 50) / (750 - 50) * (0x0898 - 0x00A5) + 0x00A5;
                        int ii = Convert.ToInt32(Math.Floor(d));
                        byte[] b = BitConverter.GetBytes(ii);
                        dataBytes[0] = b[1];
                        dataBytes[1] = b[0];
                    }
                }
                else if (mode == enumPWMCurrentSetPointMode.open_loop)
                {
                    if (val < 0)
                    {
                        dataBytes[0] = 0x00;
                        dataBytes[1] = 0x00;
                    }
                    else if (val > 100)
                    {
                        dataBytes[0] = 0x3F;
                        dataBytes[1] = 0xFF;
                    }
                    else
                    {
                        double d = Convert.ToDouble(val) / (100 - 0) * (0x3FFF - 0x0000);
                        int ii = Convert.ToInt32(Math.Floor(d));
                        byte[] b = BitConverter.GetBytes(ii);
                        dataBytes[0] = b[1];
                        dataBytes[1] = b[0];
                    }
                }
                else
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Initialize()
        {
            try
            {
                Global.myBoardServoValvePackIIClass = new BoardServoValvePackIIClass()
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
                Global.myBoardServoValvePackIIClass.EventSerialDataSend += new EventHandler(ReceiveDataFunc);
                Global.myBoardServoValvePackIIClass.EventSerialDataSend += new EventHandler(Global.myDataRecvToSaveClass.ReceiveDataFromSerial);
                isConnected = Global.myBoardServoValvePackIIClass.Initialize();

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

                        richTextBox_RecvInfo.Clear();
                        richTextBox_RecvInfo.SelectionColor = Color.Black;
                        richTextBox_RecvInfo.Text = myStructBoardIIStatus.sData;

                        for (int i = 0; i < myStructBoardIIStatus.indexSubstitution.Count; i++)
                        {
                            richTextBox_RecvInfo.Select(myStructBoardIIStatus.indexSubstitution[i] * 2, 2);
                            richTextBox_RecvInfo.SelectionColor = Color.Red;
                            richTextBox_RecvInfo.SelectedText = "";
                        }
                        richTextBox_RecvInfo.SelectionStart = myStructBoardIIStatus.sData.Length - 6;
                        richTextBox_RecvInfo.SelectionLength = 4;
                        richTextBox_RecvInfo.SelectionColor = Color.Blue;
                        richTextBox_RecvInfo.SelectionLength = 0;
                        richTextBox_RecvInfo.AppendText(myStructBoardIIStatus.sCRCResult);



                        //各解析量显示
                        richTextBox_RecvInfo.Clear();
                        richTextBox_RecvInfo.SelectionColor = Color.Black;
                        richTextBox_RecvInfo.Text = myStructBoardIIStatus.sData;

                        for (int i = 0; i < myStructBoardIIStatus.indexSubstitution.Count; i++)
                        {
                            richTextBox_RecvInfo.Select(myStructBoardIIStatus.indexSubstitution[i] * 2, 2);
                            richTextBox_RecvInfo.SelectionColor = Color.Red;
                            richTextBox_RecvInfo.SelectedText = "";
                        }
                        richTextBox_RecvInfo.SelectionStart = myStructBoardIIStatus.sData.Length - 6;
                        richTextBox_RecvInfo.SelectionLength = 4;
                        richTextBox_RecvInfo.SelectionColor = Color.Blue;
                        richTextBox_RecvInfo.SelectionLength = 0;
                        richTextBox_RecvInfo.AppendText(myStructBoardIIStatus.sCRCResult);

                        if (myStructBoardIIStatus.bDIN8 == 1)
                        {
                            textBox_DIN8.BackColor = Color.Green;
                        }
                        else
                        {
                            textBox_DIN8.BackColor = Color.LightGray;
                        }
                        if (myStructBoardIIStatus.bDIN7 == 1)
                        {
                            textBox_DIN7.BackColor = Color.Green;
                        }
                        else
                        {
                            textBox_DIN7.BackColor = Color.LightGray;
                        }
                        if (myStructBoardIIStatus.bDIN6 == 1)
                        {
                            textBox_DIN6.BackColor = Color.Green;
                        }
                        else
                        {
                            textBox_DIN6.BackColor = Color.LightGray;
                        }
                        if (myStructBoardIIStatus.bDIN5 == 1)
                        {
                            textBox_DIN5.BackColor = Color.Green;
                        }
                        else
                        {
                            textBox_DIN5.BackColor = Color.LightGray;
                        }
                        if (myStructBoardIIStatus.bDIN4 == 1)
                        {
                            textBox_DIN4.BackColor = Color.Green;
                        }
                        else
                        {
                            textBox_DIN4.BackColor = Color.LightGray;
                        }
                        if (myStructBoardIIStatus.bDIN3 == 1)
                        {
                            textBox_DIN3.BackColor = Color.Green;
                        }
                        else
                        {
                            textBox_DIN3.BackColor = Color.LightGray;
                        }
                        if (myStructBoardIIStatus.bDIN2 == 1)
                        {
                            textBox_DIN2.BackColor = Color.Green;
                        }
                        else
                        {
                            textBox_DIN2.BackColor = Color.LightGray;
                        }
                        if (myStructBoardIIStatus.bDIN1 == 1)
                        {
                            textBox_DIN1.BackColor = Color.Green;
                        }
                        else
                        {
                            textBox_DIN1.BackColor = Color.LightGray;
                        }

                        textBox_PWM_1_2.Text = myStructBoardIIStatus.Current_Feedback_PWM_1_2.ToString();
                        textBox_PWM_3_4.Text = myStructBoardIIStatus.Current_Feedback_PWM_3_4.ToString();
                        textBox_PWM_5_6.Text = myStructBoardIIStatus.Current_Feedback_PWM_5_6.ToString();
                        textBox_PWM_7_8.Text = myStructBoardIIStatus.Current_Feedback_PWM_7_8.ToString();
                        textBox_PWM_9_10.Text = myStructBoardIIStatus.Current_Feedback_PWM_9_10.ToString();
                        textBox_PWM_11_12.Text = myStructBoardIIStatus.Current_Feedback_PWM_11_12.ToString();
                        textBox_PWM_13_14.Text = myStructBoardIIStatus.Current_Feedback_PWM_13_14.ToString();
                        textBox_PWM_15_16.Text = myStructBoardIIStatus.Current_Feedback_PWM_15_16.ToString();

                        textBox_VCCD.Text = myStructBoardIIStatus.VCCD.ToString();
                        textBox_VCCA.Text = myStructBoardIIStatus.VCCA.ToString();
                        textBox_24VDC.Text = myStructBoardIIStatus.Main_Supply_Voltage_24VDC.ToString();
                        textBox_15VDC.Text = myStructBoardIIStatus.Main_Supply_Voltage_15VDC.ToString();

                        textBox_External_Analog_In_1.Text = myStructBoardIIStatus.External_Analog_In_1.ToString();
                        textBox_External_Analog_In_2.Text = myStructBoardIIStatus.External_Analog_In_2.ToString();
                        textBox_External_Analog_In_3.Text = myStructBoardIIStatus.External_Analog_In_3.ToString();
                        textBox_External_Analog_In_4.Text = myStructBoardIIStatus.External_Analog_In_4.ToString();

                        textBox_DOUT1_8_SSUP1_2.Text = myStructBoardIIStatus.Current_Feedback_DOUT1_8_SSUP_1_2.ToString();
                        textBox_DOUT9_16_SSUP3_4.Text = myStructBoardIIStatus.Current_Feedback_DOUT9_16_SSUP_3_4.ToString();
                        textBox_Temperature.Text = myStructBoardIIStatus.Temperature.ToString();

                        textBox_BadCRCS.Text = myStructBoardIIStatus.Received_Bad_CRCs.ToString();
                        textBox_RS232Ch2_R.Text = myStructBoardIIStatus.RS232_2_Received_Data.ToString();
                        textBox_CAN1_R.Text = myStructBoardIIStatus.CAN_1_Received_Data.ToString();
                        textBox_CAN2_R.Text = myStructBoardIIStatus.CAN_2_Received_Data.ToString();
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
                Global.myBoardServoValvePackIIClass.StopSerial();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool SetDataIntoPCB()
        {
            try
            {
                byte[] dataSend = new byte[32];

                dataSend[1] = 0x01;//Message I.D.Unique message identifier currently set to 0x01.

                #region dataSend[2],Control Serial and Message Address
                //(Upper Nibble) Adjust baud rate of slave serial port, not currently implemented, set to 0x0_.
                //(Lower Nibble) Message address – intended recipient board address to match jumper links (for RS485 multidrop applications.)
                #endregion
                dataSend[2] = 0x00;

                dataSend[3] = 0x00;//Adjust range of external analogue inputs, not currently implemented, set to 0x00.

                dataSend[4] = Global.FaXiang16_PWM1_8_OnOff;
                dataSend[5] = Global.FaXiang16_PWM9_16_OnOff;
                dataSend[6] = Global.FaXiang16_DOUT1_8_OnOff;
                dataSend[7] = Global.FaXiang16_DOUT9_16_OnOff;
                dataSend[8] = 0x00;
                dataSend[9] = 0x00;

                dataSend[10] = (Global.FaXiang16_PWM1_8_OnOff & 0x01) != 0 || (Global.FaXiang16_PWM1_8_OnOff & 0x02) != 0 ? (byte)0x3f : (byte)0x00;    // PWMOpenLoop_1_2
                dataSend[11] = (Global.FaXiang16_PWM1_8_OnOff & 0x01) != 0 || (Global.FaXiang16_PWM1_8_OnOff & 0x02) != 0 ? (byte)0xff : (byte)0x00;

                dataSend[12] = (Global.FaXiang16_PWM1_8_OnOff & 0x04) != 0 || (Global.FaXiang16_PWM1_8_OnOff & 0x08) != 0 ? (byte)0x3f : (byte)0x00;    // PWMOpenLoop_3_4
                dataSend[13] = (Global.FaXiang16_PWM1_8_OnOff & 0x04) != 0 || (Global.FaXiang16_PWM1_8_OnOff & 0x08) != 0 ? (byte)0xff : (byte)0x00;

                dataSend[14] = (Global.FaXiang16_PWM1_8_OnOff & 0x10) != 0 || (Global.FaXiang16_PWM1_8_OnOff & 0x20) != 0 ? (byte)0x3f : (byte)0x00;    // PWMOpenLoop_5_6
                dataSend[15] = (Global.FaXiang16_PWM1_8_OnOff & 0x10) != 0 || (Global.FaXiang16_PWM1_8_OnOff & 0x20) != 0 ? (byte)0xff : (byte)0x00;

                dataSend[16] = (Global.FaXiang16_PWM1_8_OnOff & 0x40) != 0 || (Global.FaXiang16_PWM1_8_OnOff & 0x80) != 0 ? (byte)0x3f : (byte)0x00;    // PWMOpenLoop_7_8
                dataSend[17] = (Global.FaXiang16_PWM1_8_OnOff & 0x40) != 0 || (Global.FaXiang16_PWM1_8_OnOff & 0x80) != 0 ? (byte)0xff : (byte)0x00;

                dataSend[18] = (Global.FaXiang16_PWM9_16_OnOff & 0x01) != 0 || (Global.FaXiang16_PWM9_16_OnOff & 0x02) != 0 ? (byte)0x3f : (byte)0x00;    // PWMOpenLoop_9_10
                dataSend[19] = (Global.FaXiang16_PWM9_16_OnOff & 0x01) != 0 || (Global.FaXiang16_PWM9_16_OnOff & 0x02) != 0 ? (byte)0xff : (byte)0x00;

                dataSend[20] = (Global.FaXiang16_PWM9_16_OnOff & 0x04) != 0 || (Global.FaXiang16_PWM9_16_OnOff & 0x08) != 0 ? (byte)0x3f : (byte)0x00;    // PWMOpenLoop_11_12
                dataSend[21] = (Global.FaXiang16_PWM9_16_OnOff & 0x04) != 0 || (Global.FaXiang16_PWM9_16_OnOff & 0x08) != 0 ? (byte)0xff : (byte)0x00;

                dataSend[22] = (Global.FaXiang16_PWM9_16_OnOff & 0x10) != 0 || (Global.FaXiang16_PWM9_16_OnOff & 0x20) != 0 ? (byte)0x3f : (byte)0x00;    // PWMOpenLoop_13_14
                dataSend[23] = (Global.FaXiang16_PWM9_16_OnOff & 0x10) != 0 || (Global.FaXiang16_PWM9_16_OnOff & 0x20) != 0 ? (byte)0xff : (byte)0x00;

                dataSend[24] = (Global.FaXiang16_PWM9_16_OnOff & 0x40) != 0 || (Global.FaXiang16_PWM9_16_OnOff & 0x80) != 0 ? (byte)0x3f : (byte)0x00;    // PWMOpenLoop_15_16
                dataSend[25] = (Global.FaXiang16_PWM9_16_OnOff & 0x40) != 0 || (Global.FaXiang16_PWM9_16_OnOff & 0x80) != 0 ? (byte)0xff : (byte)0x00;

                dataSend[26] = 0x00;   //textBox_RS232Ch2_T

                dataSend[27] = 0x00; //textBox_CAN1_T

                dataSend[28] = 0x00; //textBox_CAN2_T

                byte[] crc = Global.CRC16(dataSend, 1, 28);
                dataSend[29] = crc[0];
                dataSend[30] = crc[1];

                dataSend[31] = 0xAA;

                byte bSubstitution = 0x00;
                bool bGetSubstitutionPhilosophy = Global.getSubstitutionPhilosophy(dataSend, 1, 30, out bSubstitution);
                if (bGetSubstitutionPhilosophy == false)
                {
                    return false;
                }

                dataSend[0] = bSubstitution;
                for (int i = 1; i < 31; i++)
                {
                    if (0xAA == dataSend[i])
                    {
                        dataSend[i] = bSubstitution;
                    }
                }

                Global.myBoardServoValvePackIIClass.BCmdInsert(dataSend);

                string sQuery = "";
                for (int i = 0; i < dataSend.Length; i++)
                {
                    sQuery += dataSend[i].ToString("X2");
                }

                if (Global.myBoardServoValvePackIIClass.SQuirys.Length <= 0)
                {
                    Global.myBoardServoValvePackIIClass.SQuirys = new string[1];
                    Global.myBoardServoValvePackIIClass.SQuirys[0] = sQuery;
                }
                else
                {
                    Global.myBoardServoValvePackIIClass.SQuirys[0] = sQuery;
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private void btn_SetDataIntoPCB_Click(object sender, EventArgs e)
        {
            try
            {
                byte[] dataSend = new byte[32];

                dataSend[1] = 0x01;//Message I.D.Unique message identifier currently set to 0x01.

                #region dataSend[2],Control Serial and Message Address
                //(Upper Nibble) Adjust baud rate of slave serial port, not currently implemented, set to 0x0_.
                //(Lower Nibble) Message address – intended recipient board address to match jumper links (for RS485 multidrop applications.)
                #endregion
                dataSend[2] = 0x00;

                dataSend[3] = 0x00;//Adjust range of external analogue inputs, not currently implemented, set to 0x00.

                dataSend[4] = byte_PWM1_8_OnOff;
                dataSend[5] = byte_PWM9_16_OnOff;
                dataSend[6] = byte_DOUT1_8_OnOff;
                dataSend[7] = byte_DOUT9_16_OnOff;
                dataSend[8] = byte_SSUP1_4_OnOff;
                dataSend[9] = byte_PWM1_16_Open_Loop_OnOff;

                byte[] bChange;
                enumPWMCurrentSetPointMode mode =
                    checkBox_PWMOpenLoop_1_2.Checked == true ?
                   enumPWMCurrentSetPointMode.closed_loop :
                   enumPWMCurrentSetPointMode.open_loop;
                bool b = PWMCurrentSetPoint(numericUpDown_PWMCurrentSetPoint_1_2.Value, mode, out bChange);
                if (b == true)
                {
                    dataSend[10] = bChange[0];
                    dataSend[11] = bChange[1];
                }
                else
                {
                    dataSend[10] = 0x00;
                    dataSend[11] = 0x00;
                    return;
                }

                mode = checkBox_PWMOpenLoop_3_4.Checked == true ?
                   enumPWMCurrentSetPointMode.closed_loop :
                   enumPWMCurrentSetPointMode.open_loop;
                b = PWMCurrentSetPoint(numericUpDown_PWMCurrentSetPoint_3_4.Value, mode, out bChange);
                if (b == true)
                {
                    dataSend[12] = bChange[0];
                    dataSend[13] = bChange[1];
                }
                else
                {
                    dataSend[12] = 0x00;
                    dataSend[13] = 0x00;
                    return;
                }

                mode = checkBox_PWMOpenLoop_5_6.Checked == true ?
                   enumPWMCurrentSetPointMode.closed_loop :
                   enumPWMCurrentSetPointMode.open_loop;
                b = PWMCurrentSetPoint(numericUpDown_PWMCurrentSetPoint_5_6.Value, mode, out bChange);
                if (b == true)
                {
                    dataSend[14] = bChange[0];
                    dataSend[15] = bChange[1];
                }
                else
                {
                    dataSend[14] = 0x00;
                    dataSend[15] = 0x00;
                    return;
                }

                mode = checkBox_PWMOpenLoop_7_8.Checked == true ?
                   enumPWMCurrentSetPointMode.closed_loop :
                   enumPWMCurrentSetPointMode.open_loop;
                b = PWMCurrentSetPoint(numericUpDown_PWMCurrentSetPoint_7_8.Value, mode, out bChange);
                if (b == true)
                {
                    dataSend[16] = bChange[0];
                    dataSend[17] = bChange[1];
                }
                else
                {
                    dataSend[16] = 0x00;
                    dataSend[17] = 0x00;
                    return;
                }

                mode = checkBox_PWMOpenLoop_9_10.Checked == true ?
                   enumPWMCurrentSetPointMode.closed_loop :
                   enumPWMCurrentSetPointMode.open_loop;
                b = PWMCurrentSetPoint(numericUpDown_PWMCurrentSetPoint_9_10.Value, mode, out bChange);
                if (b == true)
                {
                    dataSend[18] = bChange[0];
                    dataSend[19] = bChange[1];
                }
                else
                {
                    dataSend[18] = 0x00;
                    dataSend[19] = 0x00;
                    return;
                }

                mode = checkBox_PWMOpenLoop_11_12.Checked == true ?
                   enumPWMCurrentSetPointMode.closed_loop :
                   enumPWMCurrentSetPointMode.open_loop;
                b = PWMCurrentSetPoint(numericUpDown_PWMCurrentSetPoint_11_12.Value, mode, out bChange);
                if (b == true)
                {
                    dataSend[20] = bChange[0];
                    dataSend[21] = bChange[1];
                }
                else
                {
                    dataSend[20] = 0x00;
                    dataSend[21] = 0x00;
                    return;
                }

                mode = checkBox_PWMOpenLoop_13_14.Checked == true ?
                   enumPWMCurrentSetPointMode.closed_loop :
                   enumPWMCurrentSetPointMode.open_loop;
                b = PWMCurrentSetPoint(numericUpDown_PWMCurrentSetPoint_13_14.Value, mode, out bChange);
                if (b == true)
                {
                    dataSend[22] = bChange[0];
                    dataSend[23] = bChange[1];
                }
                else
                {
                    dataSend[22] = 0x00;
                    dataSend[23] = 0x00;
                    return;
                }

                mode = checkBox_PWMOpenLoop_15_16.Checked == true ?
                   enumPWMCurrentSetPointMode.closed_loop :
                   enumPWMCurrentSetPointMode.open_loop;
                b = PWMCurrentSetPoint(numericUpDown_PWMCurrentSetPoint_15_16.Value, mode, out bChange);
                if (b == true)
                {
                    dataSend[24] = bChange[0];
                    dataSend[25] = bChange[1];
                }
                else
                {
                    dataSend[24] = 0x00;
                    dataSend[25] = 0x00;
                    return;
                }

                string s = textBox_RS232Ch2_T.Text.Replace(" ", "");
                for (int i = s.Length; i < 2; i--)
                {
                    s = "0" + s;
                }
                byte bb = Convert.ToByte(s.Substring(0, 2), 16);
                dataSend[26] = bb;

                s = textBox_CAN1_T.Text.Replace(" ", "");
                for (int i = s.Length; i < 2; i--)
                {
                    s = "0" + s;
                }
                bb = Convert.ToByte(s.Substring(0, 2), 16);
                dataSend[27] = bb;

                s = textBox_CAN2_T.Text.Replace(" ", "");
                for (int i = s.Length; i < 2; i--)
                {
                    s = "0" + s;
                }
                bb = Convert.ToByte(s.Substring(0, 2), 16);
                dataSend[28] = bb;

                byte[] crc = Global.CRC16(dataSend, 1, 28);
                dataSend[29] = crc[0];
                dataSend[30] = crc[1];

                dataSend[31] = 0xAA;

                byte bSubstitution = 0x00;
                bool bGetSubstitutionPhilosophy = Global.getSubstitutionPhilosophy(dataSend, 1, 30, out bSubstitution);
                if (bGetSubstitutionPhilosophy == false)
                {
                    richTextBox_TransmitInfo.Text = "获取Substitution Philosophy错误！";
                    return;
                }

                List<int> indexSubstitution = new List<int>();
                indexSubstitution.Add(0);
                dataSend[0] = bSubstitution;
                for (int i = 1; i < 31; i++)
                {
                    if (0xAA == dataSend[i])
                    {
                        dataSend[i] = bSubstitution;
                        indexSubstitution.Add(i);
                    }
                }

                string sDataOut = "";
                string sDataOut1 = "";
                for (int i = 0; i <= 31; i++)
                {
                    sDataOut += dataSend[i].ToString("X2");
                    sDataOut1 += dataSend[i].ToString("X2") + " ";
                }
                richTextBox_TransmitInfo.Clear();
                richTextBox_TransmitInfo.SelectionColor = Color.Black;

                richTextBox_TransmitInfo.Text = sDataOut;
                textBox1.Text = sDataOut1;

                for (int i = 0; i < indexSubstitution.Count; i++)
                {
                    richTextBox_TransmitInfo.Select(indexSubstitution[i] * 2, 2);
                    richTextBox_TransmitInfo.SelectionColor = Color.Red;
                    richTextBox_TransmitInfo.SelectedText = "";
                }

                richTextBox_TransmitInfo.SelectionStart = 29 * 2;
                richTextBox_TransmitInfo.SelectionLength = 4;
                //richTextBox_TransmitInfo.Select(29 * 2, 4);
                richTextBox_TransmitInfo.SelectionColor = Color.Blue;
                richTextBox_TransmitInfo.SelectionLength = 0;

                Global.myBoardServoValvePackIIClass.BCmdInsert(dataSend);

                string sQuery = "";
                for (int i = 0; i < dataSend.Length; i++)
                {
                    sQuery += dataSend[i].ToString("X2");
                }

                string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "16功能阀箱指令下发：" + sQuery;
                sInfo += "\t\n";

                GEventArgs gEventArgs = new GEventArgs();
                gEventArgs.dataType = 14;////14-执行设置操作，将设置指令传出，对于16功能阀箱
                gEventArgs.obj = sInfo;
                if (EventBtnStatusChanged != null)
                {
                    EventBtnStatusChanged(this, gEventArgs);
                }

                if (Global.myBoardServoValvePackIIClass.SQuirys.Length <= 0)
                {
                    Global.myBoardServoValvePackIIClass.SQuirys = new string[1];
                    Global.myBoardServoValvePackIIClass.SQuirys[0] = sQuery;
                }
                else
                {
                    Global.myBoardServoValvePackIIClass.SQuirys[0] = sQuery;
                }
            }
            catch (Exception ex)
            {
                richTextBox_TransmitInfo.Text = "指令生成异常！";

                string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "16功能阀箱指令生成异常！";
                sInfo += "\t\n";

                GEventArgs gEventArgs = new GEventArgs();
                gEventArgs.dataType = 14;////14-执行设置操作，将设置指令传出，对于16功能阀箱
                gEventArgs.obj = sInfo;
                if (EventBtnStatusChanged != null)
                {
                    EventBtnStatusChanged(this, gEventArgs);
                }
            }
        }

        private void FormBoardII_VisibleChanged(object sender, EventArgs e)
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

        private void FormBoardII_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                StopSerial();
            }
            catch (Exception ex)
            { }
        }

        private void FormBoardII_Load(object sender, EventArgs e)
        {
            try
            {
                checkBox_PWMOpenLoop_1_2.Checked = false;
                checkBox_PWMOpenLoop_3_4.Checked = false;
                checkBox_PWMOpenLoop_5_6.Checked = false;
                checkBox_PWMOpenLoop_7_8.Checked = false;
                checkBox_PWMOpenLoop_9_10.Checked = false;
                checkBox_PWMOpenLoop_11_12.Checked = false;
                checkBox_PWMOpenLoop_13_14.Checked = false;
                checkBox_PWMOpenLoop_15_16.Checked = false;

                numericUpDown_PWMCurrentSetPoint_1_2.Value = 99;
                numericUpDown_PWMCurrentSetPoint_3_4.Value = 99;
                numericUpDown_PWMCurrentSetPoint_5_6.Value = 99;
                numericUpDown_PWMCurrentSetPoint_7_8.Value = 99;
                numericUpDown_PWMCurrentSetPoint_9_10.Value = 99;
                numericUpDown_PWMCurrentSetPoint_11_12.Value = 99;
                numericUpDown_PWMCurrentSetPoint_13_14.Value = 99;
                numericUpDown_PWMCurrentSetPoint_15_16.Value = 99;

            }
            catch (Exception ex)
            { }
        }




    }
}
