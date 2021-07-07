using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.Reflection;

namespace MonitorProj
{
    public partial class FormSerialJuYuanJianCe1Ctl : Form
    {
        public event EventHandler EventBtnStatusChanged;

        #region 参数定义

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

        //名称Name
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
            }
        }

        //运行状态
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

        //端口名称
        private string portName;
        public string PortName
        {
            get
            {
                return portName;
            }
            set
            {
                portName = value;
            }
        }
        //波特率
        private int bandRate;
        public int BandRate
        {
            get
            {
                return bandRate;
            }
            set
            {
                bandRate = value;
            }
        }
        //数据位
        private int dataBits;
        public int DataBits
        {
            get
            {
                return dataBits;
            }
            set
            {
                dataBits = value;
            }
        }
        //停止位
        private StopBits stopBits;
        public StopBits StopBits
        {
            get
            {
                return stopBits;
            }
            set
            {
                stopBits = value;
            }
        }
        //奇偶校验位
        private Parity parity;
        public Parity Parity
        {
            get
            {
                return parity;
            }
            set
            {
                parity = value;
            }
        }


        //询问指令序列
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

        //询问指令的间隔时间，单位：ms
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
        

        public event EventHandler EventSerialDataSend;


        #endregion



        public FormSerialJuYuanJianCe1Ctl()
        {
            InitializeComponent();

            Global.myBoardSerialMonCtlJuYuanJianCe1Class = new BoardSerialMonCtlClass();
            Global.myBoardSerialMonCtlJuYuanJianCe1Class.EventSerialDataSend += new EventHandler(ReceiveDataFunc);
            Global.myBoardSerialMonCtlJuYuanJianCe1Class.EventSerialDataSend += new EventHandler(Global.myDataRecvToSaveClass.ReceiveDataFromSerial);

            string[] str = Global.myBoardSerialMonCtlJuYuanJianCe1Class.SerialPortCheck();

            comboBox_SerialPort.Items.Clear();
            comboBox_SerialPort.Items.AddRange(str);
            if (comboBox_SerialPort.Items.Count > 0)
            {
                comboBox_SerialPort.SelectedIndex = 0;
            }

            comboBox_Band.Items.Clear();
            comboBox_Band.Items.Add("4800");
            comboBox_Band.Items.Add("9600");
            comboBox_Band.Items.Add("19200");
            comboBox_Band.Items.Add("38400");
            comboBox_Band.Items.Add("115200");
            comboBox_Band.SelectedIndex = 1;

            comboBox_DataBitsCount.Items.Clear();
            comboBox_DataBitsCount.Items.Add("8");
            comboBox_DataBitsCount.SelectedIndex = 0;
            comboBox_DataBitsCount.Enabled = false;

            comboBox_StopBit.Items.Clear();
            comboBox_StopBit.Items.Add("None");
            comboBox_StopBit.Items.Add("One");
            comboBox_StopBit.Items.Add("OnePointFive");
            comboBox_StopBit.Items.Add("Two");
            comboBox_StopBit.SelectedIndex = 1;
            comboBox_StopBit.Enabled = false;

            comboBox_Parity.Items.Clear();
            comboBox_Parity.Items.Add("Even");
            comboBox_Parity.Items.Add("Mark");
            comboBox_Parity.Items.Add("None");
            comboBox_Parity.Items.Add("Odd");
            comboBox_Parity.Items.Add("Space");
            comboBox_Parity.SelectedIndex = 2;
            comboBox_Parity.Enabled = false;
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

     
        private void btn_SerialOpen_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBox_SerialPort.Items.Count > 0)
                {
                    Global.myBoardSerialMonCtlJuYuanJianCe1Class.PortName = comboBox_SerialPort.SelectedItem.ToString();
                    Global.myBoardSerialMonCtlJuYuanJianCe1Class.BandRate = Convert.ToInt32(comboBox_Band.SelectedItem.ToString());
                    Global.myBoardSerialMonCtlJuYuanJianCe1Class.DataBits = Convert.ToInt32(comboBox_DataBitsCount.SelectedItem.ToString());
                    switch (comboBox_StopBit.SelectedIndex)
                    {
                        case 0:
                            Global.myBoardSerialMonCtlJuYuanJianCe1Class.StopBits = StopBits.None;
                            break;

                        case 1:
                            Global.myBoardSerialMonCtlJuYuanJianCe1Class.StopBits = StopBits.One;
                            break;

                        case 2:
                            Global.myBoardSerialMonCtlJuYuanJianCe1Class.StopBits = StopBits.OnePointFive;
                            break;

                        case 3:
                            Global.myBoardSerialMonCtlJuYuanJianCe1Class.StopBits = StopBits.Two;
                            break;

                        default:
                            Global.myBoardSerialMonCtlJuYuanJianCe1Class.StopBits = StopBits.One;
                            break;
                    }
                    switch (comboBox_Parity.SelectedIndex)
                    {
                        case 0:
                            Global.myBoardSerialMonCtlJuYuanJianCe1Class.Parity = Parity.Even;
                            break;

                        case 1:
                            Global.myBoardSerialMonCtlJuYuanJianCe1Class.Parity = Parity.Mark;
                            break;

                        case 2:
                            Global.myBoardSerialMonCtlJuYuanJianCe1Class.Parity = Parity.None;
                            break;

                        case 3:
                            Global.myBoardSerialMonCtlJuYuanJianCe1Class.Parity = Parity.Odd;
                            break;

                        case 4:
                            Global.myBoardSerialMonCtlJuYuanJianCe1Class.Parity = Parity.Space;
                            break;

                        default:
                            Global.myBoardSerialMonCtlJuYuanJianCe1Class.Parity = Parity.None;
                            break;
                    }

                    Global.myBoardSerialMonCtlJuYuanJianCe1Class.QuiryInterval = quiryInterval;
                    Global.myBoardSerialMonCtlJuYuanJianCe1Class.SQuirys = sQuirys;

                    bool isOpened = Global.myBoardSerialMonCtlJuYuanJianCe1Class.SerialOpen();
                    Global.isSerialOpenedOK_JuYuanJianCe_1 = isOpened;
                    if (isOpened)
                    {
                        string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t串口设置连接成功\t";
                        Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                        Global.myLogStreamWriter.Flush();
                    }
                    else
                    {
                        string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t串口设置连接错误\t";
                        Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                        Global.myLogStreamWriter.Flush();
                    }
                }
                else
                {
                    string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t串口设置连接错误\t";
                    Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                    Global.myLogStreamWriter.Flush();
                }
            }
            catch (Exception ex)
            { }
        }

        private void btn_SerialClose_Click(object sender, EventArgs e)
        {
            try
            {
                bool b = Global.myBoardSerialMonCtlJuYuanJianCe1Class.SerialClose();
                string sInfo = "\t";
                if (b)
                {
                    sInfo += "串口关闭成功！\t";
                }
                else
                {
                    sInfo += "串口关闭失败！\t";
                }
                string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + sInfo;
                Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                Global.myLogStreamWriter.Flush();
            }
            catch (Exception ex)
            {

            }
        }


        //串口数据接收
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
                    if (gEventArgs.addressBoard == enum_AddressBoard.JueYuanJianCe_Board_1)
                    {
                        Struct_JueYuanJianCeYi myStruct =
                            (Struct_JueYuanJianCeYi)gEventArgs.objParse;

                        textBox_JueYuanJianCe_1_MeauringValue.Text = myStruct.sMeauringValue;
                        textBox_JueYuanJianCe_1_Alarm1Value.Text = myStruct.sAlarm1Value;
                        textBox_JueYuanJianCe_1_Alarm2Value.Text = myStruct.sAlarm2Value;
                        textBox_JueYuanJianCe_1_K1K2.Text = myStruct.sK1_K2_OnOff;
                        textBox_JueYuanJianCe_1_Alarm12.Text = myStruct.sAlarm1_2_None;
                        textBox_JueYuanJianCe_1_ACDC.Text = myStruct.sAC_DC_Fault;
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
                if (Global.myBoardSerialMonCtlJuYuanJianCe1Class != null)
                {
                    Global.myBoardSerialMonCtlJuYuanJianCe1Class.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private void FormSerialMonCtl_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                StopSerial();
            }
            catch (Exception ex)
            { }
        }

        private void FormSerialJuYuanJianCe1Ctl_Load(object sender, EventArgs e)
        {
            try
            {
                Global.myBoardSerialMonCtlJuYuanJianCe1Class.NameIn = NameIn;
            }
            catch (Exception ex)
            { }
        }


    }
}
