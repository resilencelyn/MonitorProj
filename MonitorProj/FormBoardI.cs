using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Reflection;

namespace MonitorProj
{
    public partial class FormBoardI : Form
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

        public FormBoardI()
        {
            InitializeComponent();
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
                    if (gEventArgs.addressBoard == enum_AddressBoard.ServoValvePacketBoard8Func)
                    {
                        Struct_BoardA_Status myStructBoardIStatus = (Struct_BoardA_Status)gEventArgs.objParse;

                        richTextBox_RecvInfo.Clear();
                        richTextBox_RecvInfo.SelectionColor = Color.Black;
                        richTextBox_RecvInfo.Text = myStructBoardIStatus.sData;

                        for (int i = 0; i < myStructBoardIStatus.indexSubstitution.Count; i++)
                        {
                            richTextBox_RecvInfo.Select(myStructBoardIStatus.indexSubstitution[i] * 2, 2);
                            richTextBox_RecvInfo.SelectionColor = Color.Red;
                            richTextBox_RecvInfo.SelectedText = "";
                        }
                        richTextBox_RecvInfo.SelectionStart = myStructBoardIStatus.sData.Length - 6;
                        richTextBox_RecvInfo.SelectionLength = 4;
                        //richTextBox_RecvInfo.Select(46, 4);
                        richTextBox_RecvInfo.SelectionColor = Color.Blue;
                        richTextBox_RecvInfo.SelectionLength = 0;
                        richTextBox_RecvInfo.AppendText(myStructBoardIStatus.sCRCResult);


                        textBox_BadCRCS.Text = myStructBoardIStatus.Received_Bad_CRCs.ToString();

                        if (myStructBoardIStatus.bDIN8 == 1)
                        {
                            textBox_DIN8.BackColor = Color.Green;
                        }
                        else
                        {
                            textBox_DIN8.BackColor = Color.LightGray;
                        }
                        if (myStructBoardIStatus.bDIN7 == 1)
                        {
                            textBox_DIN7.BackColor = Color.Green;
                        }
                        else
                        {
                            textBox_DIN7.BackColor = Color.LightGray;
                        }
                        if (myStructBoardIStatus.bDIN6 == 1)
                        {
                            textBox_DIN6.BackColor = Color.Green;
                        }
                        else
                        {
                            textBox_DIN6.BackColor = Color.LightGray;
                        }
                        if (myStructBoardIStatus.bDIN5 == 1)
                        {
                            textBox_DIN5.BackColor = Color.Green;
                        }
                        else
                        {
                            textBox_DIN5.BackColor = Color.LightGray;
                        }
                        if (myStructBoardIStatus.bDIN4 == 1)
                        {
                            textBox_DIN4.BackColor = Color.Green;
                        }
                        else
                        {
                            textBox_DIN4.BackColor = Color.LightGray;
                        }
                        if (myStructBoardIStatus.bDIN3 == 1)
                        {
                            textBox_DIN3.BackColor = Color.Green;
                        }
                        else
                        {
                            textBox_DIN3.BackColor = Color.LightGray;
                        }
                        if (myStructBoardIStatus.bDIN2 == 1)
                        {
                            textBox_DIN2.BackColor = Color.Green;
                        }
                        else
                        {
                            textBox_DIN2.BackColor = Color.LightGray;
                        }
                        if (myStructBoardIStatus.bDIN1 == 1)
                        {
                            textBox_DIN1.BackColor = Color.Green;
                        }
                        else
                        {
                            textBox_DIN1.BackColor = Color.LightGray;
                        }

                        textBox_24V.Text = myStructBoardIStatus.Main_Supply_Voltage_24V.ToString();
                        textBox_15V.Text = myStructBoardIStatus.Analog_Supply_Voltage_15V.ToString();
                        textBox_5VAnalog.Text = myStructBoardIStatus.Analog_Supply_Voltage_5V.ToString();
                        textBox_5VDigital.Text = myStructBoardIStatus.Digital_Supply_Voltage_5V.ToString();

                        textBox_CurrentFeedback1.Text = myStructBoardIStatus.Current_Feedback_1.ToString();
                        textBox_CurrentFeedback2.Text = myStructBoardIStatus.Current_Feedback_2.ToString();

                        textBox_AI17.Text = myStructBoardIStatus.Analog_Input_17.ToString();
                        textBox_AI18.Text = myStructBoardIStatus.Analog_Input_18.ToString();
                        textBox_AI19.Text = myStructBoardIStatus.Analog_Input_19.ToString();
                        textBox_AI20.Text = myStructBoardIStatus.Analog_Input_20.ToString();
                        textBox_AI1.Text = myStructBoardIStatus.Analog_Input_1.ToString();
                        textBox_AI2.Text = myStructBoardIStatus.Analog_Input_2.ToString();
                        textBox_AI3.Text = myStructBoardIStatus.Analog_Input_3.ToString();
                        textBox_AI4.Text = myStructBoardIStatus.Analog_Input_4.ToString();
                        textBox_AI5.Text = myStructBoardIStatus.Analog_Input_5.ToString();
                        textBox_AI6.Text = myStructBoardIStatus.Analog_Input_6.ToString();
                        textBox_AI7.Text = myStructBoardIStatus.Analog_Input_7.ToString();
                        textBox_AI8.Text = myStructBoardIStatus.Analog_Input_8.ToString();
                        textBox_AI9.Text = myStructBoardIStatus.Analog_Input_9.ToString();
                        textBox_AI10.Text = myStructBoardIStatus.Analog_Input_10.ToString();
                        textBox_AI11.Text = myStructBoardIStatus.Analog_Input_11.ToString();
                        textBox_AI12.Text = myStructBoardIStatus.Analog_Input_12.ToString();
                        textBox_AI13.Text = myStructBoardIStatus.Analog_Input_13.ToString();
                        textBox_AI14.Text = myStructBoardIStatus.Analog_Input_14.ToString();
                        textBox_AI15.Text = myStructBoardIStatus.Analog_Input_15.ToString();
                        textBox_AI16.Text = myStructBoardIStatus.Analog_Input_16.ToString();

                        textBox_RS232Ch2_R.Text = myStructBoardIStatus.RS232_2_Received_Data.ToString();
                        textBox_CAN1_R.Text = myStructBoardIStatus.CAN_1_Received_Data.ToString();
                        textBox_CAN2_R.Text = myStructBoardIStatus.CAN_2_Received_Data.ToString();
                    }
                }
                else if (gEventArgs.dataType == 6)//6-接收到的原始执行指令返回数据
                {

                }
            }
            catch (Exception ex)
            { }
        }

        public bool SetDataIntoPCB()
        {
            try
            {
                byte[] dataSend = new byte[26];

                dataSend[1] = 0x01;//Message I.D.Unique message identifier.Message to 2535 PCB is 0x01.
                dataSend[2] = 0x00;//Control Serial and Message Address.Adjust baud rate of slave serial port; Not currently implemented, set to 0x00.
                dataSend[3] = 0x00;//Control Analogs.Adjust range of external analog inputs; Not currently implemented, set to 0x00.

                int index = 4;

                byte[] bAO = new byte[2];

                if (Global.FaXiang8_A01 > 10)
                {
                    Global.FaXiang8_A01 = 10;
                }
                else if (Global.FaXiang8_A01 < -10)
                {
                    Global.FaXiang8_A01 = -10;
                }
                bool bChanged = AnalogueToBytes(Global.FaXiang8_A01, out bAO);
                dataSend[index++] = bAO[1];
                dataSend[index++] = bAO[0];

                if (Global.FaXiang8_A02 > 10)
                {
                    Global.FaXiang8_A02 = 10;
                }
                else if (Global.FaXiang8_A02 < -10)
                {
                    Global.FaXiang8_A02 = -10;
                }
                bChanged = AnalogueToBytes(Global.FaXiang8_A02, out bAO);
                dataSend[index++] = bAO[1];
                dataSend[index++] = bAO[0];

                if (Global.FaXiang8_A03 > 10)
                {
                    Global.FaXiang8_A03 = 10;
                }
                else if (Global.FaXiang8_A03 < -10)
                {
                    Global.FaXiang8_A03 = -10;
                }
                bChanged = AnalogueToBytes(Global.FaXiang8_A03, out bAO);
                dataSend[index++] = bAO[1];
                dataSend[index++] = bAO[0];

                if (Global.FaXiang8_A04 > 10)
                {
                    Global.FaXiang8_A04 = 10;
                }
                else if (Global.FaXiang8_A04 < -10)
                {
                    Global.FaXiang8_A04 = -10;
                }
                bChanged = AnalogueToBytes(Global.FaXiang8_A04, out bAO);
                dataSend[index++] = bAO[1];
                dataSend[index++] = bAO[0];

                if (Global.FaXiang8_A05 > 10)
                {
                    Global.FaXiang8_A05 = 10;
                }
                else if (Global.FaXiang8_A05 < -10)
                {
                    Global.FaXiang8_A05 = -10;
                }
                bChanged = AnalogueToBytes(Global.FaXiang8_A05, out bAO);
                dataSend[index++] = bAO[1];
                dataSend[index++] = bAO[0];

                if (Global.FaXiang8_A06 > 10)
                {
                    Global.FaXiang8_A06 = 10;
                }
                else if (Global.FaXiang8_A06 < -10)
                {
                    Global.FaXiang8_A06 = -10;
                }
                bChanged = AnalogueToBytes(Global.FaXiang8_A06, out bAO);
                dataSend[index++] = bAO[1];
                dataSend[index++] = bAO[0];

                if (Global.FaXiang8_A07 > 10)
                {
                    Global.FaXiang8_A07 = 10;
                }
                else if (Global.FaXiang8_A07 < -10)
                {
                    Global.FaXiang8_A07 = -10;
                }
                bChanged = AnalogueToBytes(Global.FaXiang8_A07, out bAO);
                dataSend[index++] = bAO[1];
                dataSend[index++] = bAO[0];

                if (Global.FaXiang8_A08 > 10)
                {
                    Global.FaXiang8_A08 = 10;
                }
                else if (Global.FaXiang8_A08 < -10)
                {
                    Global.FaXiang8_A08 = -10;
                }
                bChanged = AnalogueToBytes(Global.FaXiang8_A08, out bAO);
                dataSend[index++] = bAO[1];
                dataSend[index++] = bAO[0];

                dataSend[index++] = 0x00;   //textBox_RS232Ch2_T

                dataSend[index++] = 0x00; //textBox_CAN1_T

                dataSend[index++] = 0x00; //textBox_CAN2_T

                byte[] crc = Global.CRC16(dataSend, 1, 22);
                dataSend[index++] = crc[0];
                dataSend[index++] = crc[1];

                dataSend[index++] = 0xAA;

                byte bSubstitution = 0x00;
                bool bGetSubstitutionPhilosophy = Global.getSubstitutionPhilosophy(dataSend, 1, 24, out bSubstitution);
                if (bGetSubstitutionPhilosophy == false)
                {
                    return false;
                }

                dataSend[0] = bSubstitution;
                for (int i = 1; i < 25; i++)
                {
                    if (0xAA == dataSend[i])
                    {
                        dataSend[i] = bSubstitution;
                    }
                }
                
                Global.myBoardServoValvePackIClass.BCmdInsert(dataSend);

                string sQuery = "";
                for (int i = 0; i < dataSend.Length; i++)
                {
                    sQuery += dataSend[i].ToString("X2");
                }

                if (Global.myBoardServoValvePackIClass.SQuirys.Length <= 0)
                {
                    Global.myBoardServoValvePackIClass.SQuirys = new string[1];
                    Global.myBoardServoValvePackIClass.SQuirys[0] = sQuery;
                }
                else
                {
                    Global.myBoardServoValvePackIClass.SQuirys[0] = sQuery;
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
                byte[] dataSend = new byte[26];

                dataSend[1] = 0x01;//Message I.D.Unique message identifier.Message to 2535 PCB is 0x01.
                dataSend[2] = 0x00;//Control Serial and Message Address.Adjust baud rate of slave serial port; Not currently implemented, set to 0x00.
                dataSend[3] = 0x00;//Control Analogs.Adjust range of external analog inputs; Not currently implemented, set to 0x00.

                int index = 4;

                byte[] bAO = new byte[2];
                decimal dAO = numericUpDown_AO1.Value;
                bool bChanged = AnalogueToBytes(dAO, out bAO);
                dataSend[index++] = bAO[1];
                dataSend[index++] = bAO[0];

                dAO = numericUpDown_AO2.Value;
                bChanged = AnalogueToBytes(dAO, out bAO);
                dataSend[index++] = bAO[1];
                dataSend[index++] = bAO[0];

                dAO = numericUpDown_AO3.Value;
                bChanged = AnalogueToBytes(dAO, out bAO);
                dataSend[index++] = bAO[1];
                dataSend[index++] = bAO[0];

                dAO = numericUpDown_AO4.Value;
                bChanged = AnalogueToBytes(dAO, out bAO);
                dataSend[index++] = bAO[1];
                dataSend[index++] = bAO[0];

                dAO = numericUpDown_AO5.Value;
                bChanged = AnalogueToBytes(dAO, out bAO);
                dataSend[index++] = bAO[1];
                dataSend[index++] = bAO[0];

                dAO = numericUpDown_AO6.Value;
                bChanged = AnalogueToBytes(dAO, out bAO);
                dataSend[index++] = bAO[1];
                dataSend[index++] = bAO[0];

                dAO = numericUpDown_AO7.Value;
                bChanged = AnalogueToBytes(dAO, out bAO);
                dataSend[index++] = bAO[1];
                dataSend[index++] = bAO[0];

                dAO = numericUpDown_AO8.Value;
                bChanged = AnalogueToBytes(dAO, out bAO);
                dataSend[index++] = bAO[1];
                dataSend[index++] = bAO[0];


                string s = textBox_RS232Ch2_T.Text.Replace(" ", "");
                for (int i = s.Length; i < 2; i--)
                {
                    s = "0" + s;
                }
                byte bb = Convert.ToByte(s.Substring(0, 2), 16);
                dataSend[index++] = bb;

                s = textBox_CAN1_T.Text.Replace(" ", "");
                for (int i = s.Length; i < 2; i--)
                {
                    s = "0" + s;
                }
                bb = Convert.ToByte(s.Substring(0, 2), 16);
                dataSend[index++] = bb;

                s = textBox_CAN2_T.Text.Replace(" ", "");
                for (int i = s.Length; i < 2; i--)
                {
                    s = "0" + s;
                }
                bb = Convert.ToByte(s.Substring(0, 2), 16);
                dataSend[index++] = bb;

                byte[] crc = Global.CRC16(dataSend, 1, 22);
                dataSend[index++] = crc[0];
                dataSend[index++] = crc[1];

                dataSend[index++] = 0xAA;

                byte bSubstitution = 0x00;
                bool bGetSubstitutionPhilosophy = Global.getSubstitutionPhilosophy(dataSend, 1, 24, out bSubstitution);
                if (bGetSubstitutionPhilosophy == false)
                {
                    richTextBox_TransmitInfo.Text = "获取Substitution Philosophy错误！";
                    return;
                }

                List<int> indexSubstitution = new List<int>();
                indexSubstitution.Add(0);
                dataSend[0] = bSubstitution;
                for (int i = 1; i < 25; i++)
                {
                    if (0xAA == dataSend[i])
                    {
                        dataSend[i] = bSubstitution;
                        indexSubstitution.Add(i);
                    }
                }

                string sDataOut = "";
                string sDataOut1 = ""; 
                for (int i = 0; i <= 25; i++)
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

                richTextBox_TransmitInfo.SelectionStart = 46;
                richTextBox_TransmitInfo.SelectionLength = 4;
                //richTextBox_TransmitInfo.Select(46, 4);
                richTextBox_TransmitInfo.SelectionColor = Color.Blue;
                richTextBox_TransmitInfo.SelectionLength = 0;

                Global.myBoardServoValvePackIClass.BCmdInsert(dataSend);

                string sQuery = "";
                for (int i = 0; i < dataSend.Length; i++)
                {
                    sQuery += dataSend[i].ToString("X2");
                }

                if (Global.myBoardServoValvePackIClass.SQuirys.Length <= 0)
                {
                    Global.myBoardServoValvePackIClass.SQuirys = new string[1];
                    Global.myBoardServoValvePackIClass.SQuirys[0] = sQuery;
                }
                else
                {
                    Global.myBoardServoValvePackIClass.SQuirys[0] = sQuery;
                }
            }
            catch (Exception ex)
            {
                richTextBox_TransmitInfo.Text = "指令生成异常！"; 
            }
        }

        //模拟电压输出值转换为两个字节,Receive Frame (into the P.C.B.)
        //Analogue outputs are 16 bit, with a range +10V (0xFFFF) to 0V (0x8000) to -10V (0x0000).
        private bool AnalogueToBytes(decimal data, out byte[] result)
        {
            result = new byte[2];
            try
            {
                UInt16 uiResult = 0;
                if (data >= 10)
                {
                    uiResult = 0xFFFF;
                }
                else if (data <= -10)
                {
                    uiResult = 0x0000;
                }
                else
                {
                    decimal d = (data - (-10)) / (10 - (-10)) * (0xFFFF - 0x0000);
                    decimal di = Math.Round(d, 0, MidpointRounding.ToEven);
                    if (di >= 0xFFFF)
                    {
                        uiResult = 0xFFFF;
                    }
                    else if (di <= 0x0000)
                    {
                        uiResult = 0x0000;
                    }
                    else
                    {
                        uiResult = Convert.ToUInt16(di);
                    }
                }
                result = BitConverter.GetBytes(uiResult);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private bool AnalogueToBytes(double data, out byte[] result)
        {
            result = new byte[2];
            try
            {
                UInt16 uiResult = 0;
                if (data >= 10)
                {
                    uiResult = 0xFFFF;
                }
                else if (data <= -10)
                {
                    uiResult = 0x0000;
                }
                else
                {
                    double d = (data - (-10)) / (10 - (-10)) * (0xFFFF - 0x0000);
                    double di = Math.Round(d, 0, MidpointRounding.ToEven);
                    if (di >= 0xFFFF)
                    {
                        uiResult = 0xFFFF;
                    }
                    else if (di <= 0x0000)
                    {
                        uiResult = 0x0000;
                    }
                    else
                    {
                        uiResult = Convert.ToUInt16(di);
                    }
                }
                result = BitConverter.GetBytes(uiResult);
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
                Global.myBoardServoValvePackIClass = new BoardServoValvePackIClass()
                    {
                        AddressLocal = AddressLocal,
                        PortLocal = PortLocal,
                        AddressRemote = AddressRemote,
                        PortRemote = PortRemote,
                        SQuirys = SQuirys,
                        QuiryInterval = QuiryInterval,
                        Name = NameIn,
                        MyBoardType = MyBoardType,
                        MySerialCOMType = mySerialCOMType
                    };
                Global.myBoardServoValvePackIClass.EventSerialDataSend += new EventHandler(ReceiveDataFunc);
                Global.myBoardServoValvePackIClass.EventSerialDataSend += new EventHandler(Global.myDataRecvToSaveClass.ReceiveDataFromSerial);
                isConnected = Global.myBoardServoValvePackIClass.Initialize();

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

        public bool StopSerial()
        {
            try
            {
                Global.myBoardServoValvePackIClass.StopSerial();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private void FormBoardI_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                StopSerial();
            }
            catch (Exception ex)
            { }
        }

        private void FormBoardI_VisibleChanged(object sender, EventArgs e)
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

   
    
    }
}
