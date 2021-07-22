using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Threading;
using System.Reflection;

namespace MonitorProj
{
    public partial class FormMobileDrillMonCtl : Form
    {

        //指令反馈信息
        private byte[] bCmdReturn;          //从设备串口返回的指令反馈信息
        private bool isbCmdReturn = false;  //标志位，true表示有新的指令反馈信息
        private bool isOnlyPowerCtl = false;
        public event EventHandler DeviceFormStateEventSend;
        public event EventHandler EventBtnStatusChanged;

        //记忆电源板状态，电源板有61A（常开）、61B（常开）、62A、62B、63A、63B
        private int ctlPowerOpenFlag61 = 0x11;
        private int ctlPowerOpenFlag62 = 0x00;
        private int ctlPowerOpenFlag63 = 0x00;

        //界面执行加断电状态记录
        private int powerOpenFlag1 = 0;     //记录界面、指令执行后设备开关机状态
        /**
         * int b31....b0
         * 电源打开标志位次序：
         * btn_Light_8     b7
         * btn_Light_7     b6
         * btn_Light_6     b5
         * btn_Light_5     b4
         * btn_Light_4     b3
         * btn_Light_3     b2
         * btn_Light_2     b1
         * btn_Light_1     b0
         * 
         */


        //控制指令：
        //XX：0-3位分别控制第1-4路继电器。
        //0x25 控制摄像机1-4的电源（0x61A）btnCmdindex=11~14
        //0x26 控制摄像机5-8的电源（0x61A）btnCmdindex=15~18
        //0x28 第一路罗盘电源（均来自0x61A）btnCmdindex=19~22
        //0x28：2-4备用12V电源（输出功率较小）
        private int powerOpenFlag2 = 0;
        /**
         * int b31....b0
         * 电源打开标志位次序：
         * 
         * 
         * btn_DetectPanel_Space_12V3   b11
         * btn_DetectPanel_Space_12V2   b10
         * btn_DetectPanel_Space_12V1   b9
         * btn_DetectPanel_Rotate       b8
         * 
         * btn_Camera_8     b7
         * btn_Camera_7     b6
         * btn_Camera_6     b5
         * btn_Camera_5     b4
         * btn_Camera_4     b3
         * btn_Camera_3     b2
         * btn_Camera_2     b1
         * btn_Camera_1     b0
         * 
         */

        //0x29：24V输出（来自0x62B），btnCmdindex=31~34
        //第一路高度计电源
        //第二路深度计电源
        //第3-4路备用电源
        private int powerOpenFlag3 = 0;
        /**
         * int b31....b0
         * 电源打开标志位次序：
         * 
         * btn_DetectPanel_Space_Bak2   b3
         * btn_DetectPanel_Space_Bak1   b2
         * btn_DetectPanel_Deep         b1
         * btn_DetectPanel_Hight        b0
         * 
         */

        //舱内备用电源继电器板（0x90），btnCmdindex=41~44
        //控制指令：XXYY
        //XX从0-7位分别控制8路继电器，YY目前未用到。保持为00即可。
        //通道1-4为备用24V通道，显示“备用24V#1”，“备用24V#2”，“备用24V#3”，“备用24V#4”（第四路为千兆网备用24V电源）等按钮即可。（来自0x62A）
        //其中通道5为高清摄像机供电（来自0x63B），通道6为千兆网络备用电源（来自0x63B）。
        //其中通道7为12V备用通道#1（来自0x63A），通道8为12V备用通道#2（来自0x63A）。
        private int powerOpenFlag4 = 0;
        /**
         * int b31....b0
         * 电源打开标志位次序：
         * 
         * btn_InboardBackupPRB_12V2     b7
         * btn_InboardBackupPRB_12V1     b6
         * btn_InboardBackupPRB_W     b5
         * btn_InboardBackupPRB_Camera     b4
         * btn_InboardBackupPRB_24V4     b3
         * btn_InboardBackupPRB_24V3     b2
         * btn_InboardBackupPRB_24V2     b1
         * btn_InboardBackupPRB_24V1     b0
         * 
         */



        //传入线程中的参数，指示按下的是哪个按钮
        private int btnCmdindex = -1;   //第一组控制按钮序号

        private BoardMonCtlClass[] myBoardMonCtlClass;
        public event EventHandler EventSerialDataSend;


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
                if (name != null && name.Replace(" ", "") != "")
                {
                    Name = name;
                }
            }
        }

        private List<struct_SerialConfParas> listSerialConfParas;
        public List<struct_SerialConfParas> ListSerialConfParas
        {
            get
            {
                return listSerialConfParas;
            }
            set
            {
                listSerialConfParas = value;
            }
        }

        private Dictionary<enum_SerialCOMType, BoardMonCtlClass> dicBoardMonCtlClassType =
            new Dictionary<enum_SerialCOMType, BoardMonCtlClass>();

        public FormMobileDrillMonCtl()
        {
            InitializeComponent();


            #region Btn的状态变化事件

            foreach (Control ctl in groupBox_BoardC_Ctl.Controls)
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
                            }
                        }
                    }
                }
            }

            #endregion
        

            threadCmdSendClick = new Thread(new ThreadStart(threadCmdSendFunc));
            threadCmdSendClick.Name = "DeviceCtlThread";
            threadCmdSendClick.Start();
        }

        
        public bool Initialize()
        {
            try
            {
                if (listSerialConfParas != null && listSerialConfParas.Count > 0)
                {
                    int count = listSerialConfParas.Count;
                    myBoardMonCtlClass = new BoardMonCtlClass[count];

                    for (int i = 0; i < count; i++)
                    {
                        struct_SerialConfParas myStruct_SerialConfParas = listSerialConfParas[i];

                        IPAddress AddressLocal = Global.ipAddressLocal;
                        int PortLocal = myStruct_SerialConfParas.port + Global.portLocalOffset;
                        IPAddress AddressRemote = myStruct_SerialConfParas.address;
                        int PortRemote = myStruct_SerialConfParas.port;
                        string[] SQuirys = myStruct_SerialConfParas.quirys.ToArray();
                        int QuiryInterval = myStruct_SerialConfParas.quiryinterval;
                        string sName = myStruct_SerialConfParas.serialnum;

                        enum_SerialCOMType mySerialCOMType = enum_SerialCOMType.Others;
                        if (myStruct_SerialConfParas.serialname.Contains("绝缘检测板"))
                        {
                            mySerialCOMType = enum_SerialCOMType.Detection_Board;
                        }
                        else if (myStruct_SerialConfParas.serialname.Contains("舱内继电器"))
                        {
                            mySerialCOMType = enum_SerialCOMType.Control_AC_Inboard_Board;
                        }
                        else if (myStruct_SerialConfParas.serialname.Contains("信号接口箱"))
                        {
                            mySerialCOMType = enum_SerialCOMType.Camera_Sensor_Power_Relay_Board;
                        }
                        else if (myStruct_SerialConfParas.serialname.Contains("传感器接口箱"))
                        {
                            mySerialCOMType = enum_SerialCOMType.Tank_Detection_Board;
                        }
                        else if (myStruct_SerialConfParas.serialname.Contains("灯舱"))
                        {
                            mySerialCOMType = enum_SerialCOMType.Light_Relay_Control_Panel;
                        }
                        else if (myStruct_SerialConfParas.serialname.Contains("高度计"))
                        {
                            mySerialCOMType = enum_SerialCOMType.Hight_Measure_Device;
                        }
                        else if (myStruct_SerialConfParas.serialname.Contains("罗盘"))
                        {
                            mySerialCOMType = enum_SerialCOMType.Rotate_Panel_Device;
                        }

                        myBoardMonCtlClass[i] = new BoardMonCtlClass()
                        {
                            AddressLocal = AddressLocal,
                            PortLocal = PortLocal,
                            AddressRemote = AddressRemote,
                            PortRemote = PortRemote,
                            SQuirys = SQuirys,
                            QuiryInterval = QuiryInterval,
                            Name = sName,
                            MyBoardType = myStruct_SerialConfParas.type,
                            MySerialCOMType = mySerialCOMType
                        };
                        myBoardMonCtlClass[i].EventSerialDataSend += new EventHandler(ReceiveDataFromSerial);
                        myBoardMonCtlClass[i].EventSerialDataSend += new EventHandler(Global.myDataRecvToSaveClass.ReceiveDataFromSerial);
                        myBoardMonCtlClass[i].Initialize();

                        dicBoardMonCtlClassType.Add(mySerialCOMType, myBoardMonCtlClass[i]);

                        #region 注释
                        //if (myStruct_SerialConfParas.serialname.Contains("绝缘检测板"))
                        //{
                        //    dicBoardMonCtlClassType.Add(enum_SerialCOMType.Detection_Board, myBoardMonCtlClass[i]);
                        //}
                        //else if (myStruct_SerialConfParas.serialname.Contains("舱内继电器"))
                        //{
                        //    dicBoardMonCtlClassType.Add(enum_SerialCOMType.Control_AC_Inboard_Board, myBoardMonCtlClass[i]);
                        //}
                        //else if (myStruct_SerialConfParas.serialname.Contains("信号接口箱"))
                        //{
                        //    dicBoardMonCtlClassType.Add(enum_SerialCOMType.Camera_Sensor_Power_Relay_Board, myBoardMonCtlClass[i]);
                        //}
                        //else if (myStruct_SerialConfParas.serialname.Contains("传感器接口箱"))
                        //{
                        //    dicBoardMonCtlClassType.Add(enum_SerialCOMType.Tank_Detection_Board, myBoardMonCtlClass[i]);
                        //}
                        //else if (myStruct_SerialConfParas.serialname.Contains("灯舱"))
                        //{
                        //    dicBoardMonCtlClassType.Add(enum_SerialCOMType.Light_Relay_Control_Panel, myBoardMonCtlClass[i]);
                        //}

                        //else if (myStruct_SerialConfParas.serialname.Contains("高度计"))
                        //{
                        //    dicBoardMonCtlClassType.Add(enum_SerialCOMType.Hight_Measure_Device, myBoardMonCtlClass[i]);
                        //}
                        //else if (myStruct_SerialConfParas.serialname.Contains("罗盘"))
                        //{
                        //    dicBoardMonCtlClassType.Add(enum_SerialCOMType.Rotate_Panel_Device, myBoardMonCtlClass[i]);
                        //}

                        #endregion
                    }
                }
                return true;
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
                stopThread = true;
                if (threadCmdSendClick != null && threadCmdSendClick.IsAlive)
                {
                    threadCmdSendClick.Abort();
                }

                if (myBoardMonCtlClass != null && myBoardMonCtlClass.Length > 0)
                {
                    int count = myBoardMonCtlClass.Length;
                    for (int i = 0; i < count; i++)
                    {
                        myBoardMonCtlClass[i].StopSerial();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
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

                #region 如果背景色变化会有4次事件触发，仅取第一次
                /*
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
                */
                #endregion
            }
            catch (Exception ex)
            { }
        }

        private void ReceiveDataFromSerial(object sender, EventArgs e)
        {
            try
            {
                GEventArgs gEventArgs = (GEventArgs)e;
                if (EventSerialDataSend != null)
                {
                    EventSerialDataSend(this, gEventArgs);
                }
                UpdateFormDelegate updateform = new UpdateFormDelegate(UpdateForm);
                this.BeginInvoke(updateform, new object[] { sender, gEventArgs });

            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString());
            }
        }

        //记录上次艏向信息，用于计算圈数；
        double dHeadingLast = 0.1;
        int iHeadingCircle = 0;
        private delegate void UpdateFormDelegate(object sender, GEventArgs gEventArgs);
        private void UpdateForm(object sender, GEventArgs gEventArgs)
        {
            try
            {
                if (gEventArgs.dataType == 1)
                {
                    
                }
                else if (gEventArgs.dataType == 3)
                {
                    
                }
                else if (gEventArgs.dataType == 5)
                {
                    if (gEventArgs.addressBoard == enum_AddressBoard.DC_Insulation_Detection_Board_I)//直流绝缘检测板#1
                    {
                        Struct_DC_Insulation_Detection_Board myStruct = (Struct_DC_Insulation_Detection_Board)gEventArgs.objParse;
                        textBox_DCDetectBoard61_VA.Text = Math.Round(myStruct.VA, 2).ToString();
                        textBox_DCDetectBoard61_IA.Text = Math.Round(myStruct.IA, 2).ToString();
                        textBox_DCDetectBoard61_GA.Text = Math.Round(myStruct.GA, 2).ToString();
                        textBox_DCDetectBoard61_TA.Text = Math.Round(myStruct.TA, 2).ToString();
                        textBox_DCDetectBoard61_VB.Text = Math.Round(myStruct.VB, 2).ToString();
                        textBox_DCDetectBoard61_IB.Text = Math.Round(myStruct.IB, 2).ToString();
                        textBox_DCDetectBoard61_GB.Text = Math.Round(myStruct.GB, 2).ToString();
                        textBox_DCDetectBoard61_TB.Text = Math.Round(myStruct.TB, 2).ToString();
                        textBox_DCDetectBoard61_TO.Text = Math.Round(myStruct.TO, 2).ToString();
                    }
                    else if (gEventArgs.addressBoard == enum_AddressBoard.DC_Insulation_Detection_Board_II)//直流绝缘检测板#2
                    {
                        Struct_DC_Insulation_Detection_Board myStruct = (Struct_DC_Insulation_Detection_Board)gEventArgs.objParse;
                        textBox_DCDetectBoard62_VA.Text = Math.Round(myStruct.VA, 2).ToString();
                        textBox_DCDetectBoard62_IA.Text = Math.Round(myStruct.IA, 2).ToString();
                        textBox_DCDetectBoard62_GA.Text = Math.Round(myStruct.GA, 2).ToString();
                        textBox_DCDetectBoard62_TA.Text = Math.Round(myStruct.TA, 2).ToString();
                        textBox_DCDetectBoard62_VB.Text = Math.Round(myStruct.VB, 2).ToString();
                        textBox_DCDetectBoard62_IB.Text = Math.Round(myStruct.IB, 2).ToString();
                        textBox_DCDetectBoard62_GB.Text = Math.Round(myStruct.GB, 2).ToString();
                        textBox_DCDetectBoard62_TB.Text = Math.Round(myStruct.TB, 2).ToString();
                        textBox_DCDetectBoard62_TO.Text = Math.Round(myStruct.TO, 2).ToString();
                    }
                    else if (gEventArgs.addressBoard == enum_AddressBoard.DC_Insulation_Detection_Board_III)//直流绝缘检测板#3
                    {
                        Struct_DC_Insulation_Detection_Board myStruct = (Struct_DC_Insulation_Detection_Board)gEventArgs.objParse;
                        textBox_DCDetectBoard63_VA.Text = Math.Round(myStruct.VA, 2).ToString();
                        textBox_DCDetectBoard63_IA.Text = Math.Round(myStruct.IA, 2).ToString();
                        textBox_DCDetectBoard63_GA.Text = Math.Round(myStruct.GA, 2).ToString();
                        textBox_DCDetectBoard63_TA.Text = Math.Round(myStruct.TA, 2).ToString();
                        textBox_DCDetectBoard63_VB.Text = Math.Round(myStruct.VB, 2).ToString();
                        textBox_DCDetectBoard63_IB.Text = Math.Round(myStruct.IB, 2).ToString();
                        textBox_DCDetectBoard63_GB.Text = Math.Round(myStruct.GB, 2).ToString();
                        textBox_DCDetectBoard63_TB.Text = Math.Round(myStruct.TB, 2).ToString();
                        textBox_DCDetectBoard63_TO.Text = Math.Round(myStruct.TO, 2).ToString();
                    }
                    else if (gEventArgs.addressBoard == enum_AddressBoard.High_Power_DC_Insulation_Detection_Board_I)//大功率直流绝缘检测板#1
                    {
                        Struct_High_Power_DC_Insulation_Detection_Board myStruct = (Struct_High_Power_DC_Insulation_Detection_Board)gEventArgs.objParse;
                        textBox_HPDCDetectBoard70_V.Text = Math.Round(myStruct.V, 2).ToString();
                        textBox_HPDCDetectBoard70_I.Text = Math.Round(myStruct.I, 2).ToString();
                        textBox_HPDCDetectBoard70_G.Text = Math.Round(myStruct.G, 2).ToString();
                        textBox_HPDCDetectBoard70_T.Text = Math.Round(myStruct.T, 2).ToString();
                    }
                    else if (gEventArgs.addressBoard == enum_AddressBoard.High_Power_DC_Insulation_Detection_Board_II)//大功率直流绝缘检测板#2
                    {
                        Struct_High_Power_DC_Insulation_Detection_Board myStruct = (Struct_High_Power_DC_Insulation_Detection_Board)gEventArgs.objParse;
                        textBox_HPDCDetectBoard71_V.Text = Math.Round(myStruct.V, 2).ToString();
                        textBox_HPDCDetectBoard71_I.Text = Math.Round(myStruct.I, 2).ToString();
                        textBox_HPDCDetectBoard71_G.Text = Math.Round(myStruct.G, 2).ToString();
                        textBox_HPDCDetectBoard71_T.Text = Math.Round(myStruct.T, 2).ToString();
                    }
                    else if (gEventArgs.addressBoard == enum_AddressBoard.High_Power_DC_Insulation_Detection_Board_III)//大功率直流绝缘检测板#3
                    {
                        Struct_High_Power_DC_Insulation_Detection_Board myStruct = (Struct_High_Power_DC_Insulation_Detection_Board)gEventArgs.objParse;
                        textBox_HPDCDetectBoard72_V.Text = Math.Round(myStruct.V, 2).ToString();
                        textBox_HPDCDetectBoard72_I.Text = Math.Round(myStruct.I, 2).ToString();
                        textBox_HPDCDetectBoard72_G.Text = Math.Round(myStruct.G, 2).ToString();
                        textBox_HPDCDetectBoard72_T.Text = Math.Round(myStruct.T, 2).ToString();
                    }
                    else if (gEventArgs.addressBoard == enum_AddressBoard.Control_Panel_High_Power_DC_Insulation_Detection_Board)//大功率直流绝缘检测板控制板
                    {
                        Struct_Control_Panel_High_Power_DC_Insulation_Detection_Board myStruct = (Struct_Control_Panel_High_Power_DC_Insulation_Detection_Board)gEventArgs.objParse;
                        textBox_HPDCDetectBoardCtl_V.Text = Math.Round(myStruct.V, 2).ToString();
                        textBox_HPDCDetectBoardCtl_I.Text = Math.Round(myStruct.I, 2).ToString();
                    }
                    else if (gEventArgs.addressBoard == enum_AddressBoard.AC_Insulation_Detection_Board)//交流绝缘检测板
                    {
                        Struct_AC_Insulation_Detection_Board myStruct = (Struct_AC_Insulation_Detection_Board)gEventArgs.objParse;
                        textBox_ACDetectBoard_V.Text = Math.Round(myStruct.V, 2).ToString();
                        textBox_ACDetectBoard_I.Text = Math.Round(myStruct.I, 2).ToString();
                    }
                    else if (gEventArgs.addressBoard == enum_AddressBoard.Camera_Power_Relay_Board_I)//摄像机电源继电器板#1
                    {
                        Struct_Power_Relay_Board myStruct = (Struct_Power_Relay_Board)gEventArgs.objParse;
                        textBox_CameraPowerRelayBoard1_V.Text = Math.Round(myStruct.V, 2).ToString();
                    }
                    else if (gEventArgs.addressBoard == enum_AddressBoard.Camera_Power_Relay_Board_II)//摄像机电源继电器板#2
                    {
                        Struct_Power_Relay_Board myStruct = (Struct_Power_Relay_Board)gEventArgs.objParse;
                        textBox_CameraPowerRelayBoard2_V.Text = Math.Round(myStruct.V, 2).ToString();
                    }
                    else if (gEventArgs.addressBoard == enum_AddressBoard.Sensor_Power_Relay_Board_I)//传感器电源继电器板#1
                    {
                        Struct_Power_Relay_Board myStruct = (Struct_Power_Relay_Board)gEventArgs.objParse;
                        textBox_SensorPowerRelayBoard1_V.Text = Math.Round(myStruct.V, 2).ToString();
                    }
                    else if (gEventArgs.addressBoard == enum_AddressBoard.Sensor_Power_Relay_Board_II)//传感器电源继电器板#2
                    {
                        Struct_Power_Relay_Board myStruct = (Struct_Power_Relay_Board)gEventArgs.objParse;
                        textBox_SensorPowerRelayBoard2_V.Text = Math.Round(myStruct.V, 2).ToString();
                    }
                    else if (gEventArgs.addressBoard == enum_AddressBoard.Tank_Detection_Board)//油箱采集板#1
                    {
                        Struct_Tank_Detection_Board myStruct = (Struct_Tank_Detection_Board)gEventArgs.objParse;
                        textBox_TankDetectBoardCH1_1.Text = Math.Round(myStruct.CH1_1, 2).ToString();
                        textBox_TankDetectBoardCH1_2.Text = Math.Round(myStruct.CH1_2, 2).ToString();
                        textBox_TankDetectBoardCH1_3.Text = Math.Round(myStruct.CH1_3, 2).ToString();
                        textBox_TankDetectBoardCH1_4.Text = Math.Round(myStruct.CH1_4, 2).ToString();
                        textBox_TankDetectBoardCH2_1.Text = Math.Round(myStruct.CH2_1, 2).ToString();
                        textBox_TankDetectBoardCH2_2.Text = Math.Round(myStruct.CH2_2, 2).ToString();
                        textBox_TankDetectBoardCH2_3.Text = Math.Round(myStruct.CH2_3, 2).ToString();
                        textBox_TankDetectBoardCH2_4.Text = Math.Round(myStruct.CH2_4, 2).ToString();
                        textBox_TankDetectBoardCH3_1.Text = Math.Round(myStruct.CH3_1, 2).ToString();
                        textBox_TankDetectBoardCH3_2.Text = Math.Round(myStruct.CH3_2, 2).ToString();
                        textBox_TankDetectBoardCH3_3.Text = Math.Round(myStruct.CH3_3, 2).ToString();
                        textBox_TankDetectBoardCH3_4.Text = Math.Round(myStruct.CH3_4, 2).ToString();
                        textBox_TankDetectBoardCH4_1.Text = Math.Round(myStruct.CH4_1, 2).ToString();
                        textBox_TankDetectBoardCH4_2.Text = Math.Round(myStruct.CH4_2, 2).ToString();
                        textBox_TankDetectBoardCH4_3.Text = Math.Round(myStruct.CH4_3, 2).ToString();
                        textBox_TankDetectBoardCH4_4.Text = Math.Round(myStruct.CH4_4, 2).ToString();
                    }
                    else if (gEventArgs.addressBoard == enum_AddressBoard.Light_Relay_Board_I)//灯继电器板#1(灯舱继电器板)
                    {
                        Struct_Light_Relay_Board myStruct = (Struct_Light_Relay_Board)gEventArgs.objParse;
                        textBox_LightRelayBoard1.Text = Math.Round(myStruct.V, 2).ToString();
                    }
                    else if (gEventArgs.addressBoard == enum_AddressBoard.Light_Relay_Board_II)//灯继电器板#2(灯舱继电器板)
                    {
                        Struct_Light_Relay_Board myStruct = (Struct_Light_Relay_Board)gEventArgs.objParse;
                        textBox_LightRelayBoard2.Text = Math.Round(myStruct.V, 2).ToString();
                    }
                    else if (gEventArgs.addressBoard == enum_AddressBoard.Hight_Measure_Device)//高度计
                    {
                        Struct_HightMeasureDevice myStruct = (Struct_HightMeasureDevice)gEventArgs.objParse;
                        textBox_HeightMeasure_Hight.Text = myStruct.sHight;
                    }
                    else if (gEventArgs.addressBoard == enum_AddressBoard.Rotate_Panel_Device)//罗盘
                    {
                        Struct_RotatePanelDevice myStruct = (Struct_RotatePanelDevice)gEventArgs.objParse;
                        textBox_RotatePanel_HX.Text = Math.Round(myStruct.HX, 2).ToString();
                        textBox_RotatePanel_HY.Text = Math.Round(myStruct.HY, 2).ToString();
                        textBox_RotatePanel_HZ.Text = Math.Round(myStruct.HZ, 2).ToString();
                        textBox_RotatePanel_Roll.Text = Math.Round(myStruct.Roll, 2).ToString();
                        textBox_RotatePanel_Pitch.Text = Math.Round(myStruct.Pitch, 2).ToString();
                        textBox_RotatePanel_Yaw.Text = Math.Round(myStruct.Yaw, 2).ToString();

                        if ((dHeadingLast >= 0 && dHeadingLast <= 90) && (myStruct.Yaw >= 270 && myStruct.Yaw < 360))
                        {
                            iHeadingCircle--;
                            dHeadingLast = myStruct.Yaw;
                        }
                        else if ((myStruct.Yaw >= 0 && myStruct.Yaw <= 90) && (dHeadingLast >= 270 && dHeadingLast < 360))
                        {
                            iHeadingCircle++;
                            dHeadingLast = myStruct.Yaw;
                        }
                        else
                        {
                            dHeadingLast = myStruct.Yaw;
                        }
                        textBox_HeadingCircle.Text = iHeadingCircle.ToString();
                        textBox_HeadingCircle_2.Text = iHeadingCircle.ToString();

                        gaugeControl1.SetPointerValue("Pointer1", myStruct.Yaw);
                        gaugeControl1.SetPointerValue("Pointer2", iHeadingCircle);
                        gaugeControl2.SetPointerValue("Pointer1", myStruct.Yaw);
                        gaugeControl2.SetPointerValue("Pointer2", iHeadingCircle);
                    }
                    else if (gEventArgs.addressBoard == enum_AddressBoard.Work_Station_Quire_Board)
                    {
                        Struct_Work_Station_Quire_Board myStruct =
                            (Struct_Work_Station_Quire_Board)gEventArgs.objParse;

                        textBoxWork_Station_Quire_Board_RotateSpeed.Text = myStruct.RotateSpeed.ToString();
                        if (myStruct.WorkStation_B == 1)
                        {
                            textBoxWork_Station_Quire_Board_1.BackColor = Color.Green;
                        }
                        else
                        {
                            textBoxWork_Station_Quire_Board_1.BackColor = SystemColors.Control; ;
                        }
                        if (myStruct.WorkStation_C == 1)
                        {
                            textBoxWork_Station_Quire_Board_2.BackColor = Color.Green;
                        }
                        else
                        {
                            textBoxWork_Station_Quire_Board_2.BackColor = SystemColors.Control; ;
                        }
                        if (myStruct.WorkStation_D == 1)
                        {
                            textBoxWork_Station_Quire_Board_3.BackColor = Color.Green;
                        }
                        else
                        {
                            textBoxWork_Station_Quire_Board_3.BackColor = SystemColors.Control; ;
                        }
                        if (myStruct.WorkStation_E == 1)
                        {
                            textBoxWork_Station_Quire_Board_4.BackColor = Color.Green;
                        }
                        else
                        {
                            textBoxWork_Station_Quire_Board_4.BackColor = SystemColors.Control; ;
                        }
                        if (myStruct.WorkStation_F == 1)
                        {
                            textBoxWork_Station_Quire_Board_5.BackColor = Color.Green;
                        }
                        else
                        {
                            textBoxWork_Station_Quire_Board_5.BackColor = SystemColors.Control; ;
                        }
                        if (myStruct.WorkStation_G == 1)
                        {
                            textBoxWork_Station_Quire_Board_6.BackColor = Color.Green;
                        }
                        else
                        {
                            textBoxWork_Station_Quire_Board_6.BackColor = SystemColors.Control; ;
                        }
                    }
                    else if (gEventArgs.addressBoard == enum_AddressBoard.DianJi_T_Detection_Board)
                    {
                        Struct_DianJi_T myStruct =
                            (Struct_DianJi_T)gEventArgs.objParse;

                        textBox_DianJi_Detecte_Para1.Text = Math.Round(myStruct.Para_1, 2).ToString();
                        textBox_DianJi_Detecte_Para2.Text = Math.Round(myStruct.Para_2, 2).ToString();
                        textBox_DianJi_Detecte_Para3.Text = Math.Round(myStruct.Para_3, 2).ToString();
                        textBox_DianJi_Detecte_Para4.Text = Math.Round(myStruct.Para_4, 2).ToString();
                        textBox_DianJi_Detecte_Para5.Text = Math.Round(myStruct.Para_5, 2).ToString();
                        textBox_DianJi_Detecte_Para6.Text = Math.Round(myStruct.Para_6, 2).ToString();
                        textBox_DianJi_Detecte_Para7.Text = Math.Round(myStruct.Para_7, 2).ToString();
                        textBox_DianJi_Detecte_Para8.Text = Math.Round(myStruct.Para_8, 2).ToString();
                    }
                }
                else if (gEventArgs.dataType == 6)
                {
                    bCmdReturn = (byte[])gEventArgs.obj;
                    isbCmdReturn = true;
                }
            }
            catch (Exception ex)
            { }
        }



        private bool isCtlRetOK = false;
        int sleepInternal = 40;//延迟等待时间

        #region threadCmdSendFunc,板卡加断电处理线程函数，主控电源加电->板卡加电->按钮颜色变绿色；板卡断电->主控电源断电->按钮颜色变灰色
        private Thread threadCmdSendClick;
        public bool stopThread = false;
        private bool flagCmdToSend = false;     //true表示界面上有按钮按下，需要发送指令，当指令执行完后需要将其置为false
        private int typeCmdSendOpen = -1;     //1表示点击按钮，打开电源；2表示点击按钮，关闭电源
        private void threadCmdSendFunc()
        {
            try
            {
                sleepInternal = 40;//延迟等待时间
                while(stopThread == false)
                {
                    try
                    {
                        if (flagCmdToSend == false)
                        {
                            Thread.Sleep(5);
                            continue;
                        }


                        #region 确定板卡前专控电源的通道号,bChannelReturn
                        //确定总电源的通道号
                        byte bChannelReturn = 0x00;
                        if (btnCmdindex >= 1 && btnCmdindex <= 8)
                        {
                            bChannelReturn = 0x79;
                        }
                        else if (btnCmdindex >= 11 && btnCmdindex <= 22)
                        {
                            bChannelReturn = 0x61;
                        }
                        else if (btnCmdindex >= 31 && btnCmdindex <= 34)
                        {
                            bChannelReturn = 0x62;
                        }
                        else if (btnCmdindex >= 41 && btnCmdindex <= 44)
                        {
                            bChannelReturn = 0x62;
                        }
                        else if (btnCmdindex >= 45 && btnCmdindex <= 48)
                        {
                            bChannelReturn = 0x63;
                        }
                        else if (btnCmdindex >= 61 && btnCmdindex <= 62)
                        {
                            bChannelReturn = 0x79;
                        }

                        #endregion
                        
                        #region 确定主控电源发送串口
                        enum_SerialCOMType myCtlPowerSerialCOMType = enum_SerialCOMType.Others;

                        if (bChannelReturn == 0x61 ||
                            bChannelReturn == 0x62 ||
                            bChannelReturn == 0x63 ||
                            bChannelReturn == 0x70 ||
                            bChannelReturn == 0x71 ||
                            bChannelReturn == 0x72)
                        {
                            myCtlPowerSerialCOMType = enum_SerialCOMType.Detection_Board;
                        }
                        else if (bChannelReturn == 0x79 ||
                            bChannelReturn == 0x80 ||
                            bChannelReturn == 0x90)
                        {
                            myCtlPowerSerialCOMType = enum_SerialCOMType.Control_AC_Inboard_Board;
                        }
                        else if (bChannelReturn == 0x25 ||
                            bChannelReturn == 0x26 ||
                            bChannelReturn == 0x28 ||
                            bChannelReturn == 0x29)
                        {
                            myCtlPowerSerialCOMType = enum_SerialCOMType.Camera_Sensor_Power_Relay_Board;
                        }
                        else if (bChannelReturn == 0x40)
                        {
                            myCtlPowerSerialCOMType = enum_SerialCOMType.Tank_Detection_Board;
                        }
                        else if (bChannelReturn == 0x21 ||
                            bChannelReturn == 0x22 ||
                            bChannelReturn == 0x23 ||
                            bChannelReturn == 0x24)
                        {
                            myCtlPowerSerialCOMType = enum_SerialCOMType.Light_Relay_Control_Panel;
                        }

                        #endregion

                        #region 确定板卡的通道号,bChannelReturnCard
                        //根据不同组，确定板子的通道号
                        byte bChannelReturnCard = 0x21;
                        if (btnCmdindex >= 1 && btnCmdindex <= 4)
                        {
                            bChannelReturnCard = 0x21;
                        }
                        else if (btnCmdindex >= 5 && btnCmdindex <= 8)
                        {
                            bChannelReturnCard = 0x22;
                        }
                        else if (btnCmdindex >= 11 && btnCmdindex <= 14)
                        {
                            bChannelReturnCard = 0x25;
                        }
                        else if (btnCmdindex >= 15 && btnCmdindex <= 18)
                        {
                            bChannelReturnCard = 0x26;
                        }
                        else if (btnCmdindex >= 19 && btnCmdindex <= 22)
                        {
                            bChannelReturnCard = 0x28;
                        }
                        else if (btnCmdindex >= 31 && btnCmdindex <= 34)
                        {
                            bChannelReturnCard = 0x29;
                        }
                        else if (btnCmdindex >= 41 && btnCmdindex <= 48)
                        {
                            bChannelReturnCard = 0x90;
                        }
                        else if (btnCmdindex >= 51 && btnCmdindex <= 51)
                        {
                            bChannelReturnCard = 0x23;
                        }
                        else if (btnCmdindex >= 52 && btnCmdindex <= 52)
                        {
                            bChannelReturnCard = 0x24;
                        }

                        #endregion

                        #region 确定板卡电源发送串口
                        enum_SerialCOMType myCardSerialCOMType = enum_SerialCOMType.Others;

                        if (bChannelReturnCard == 0x61 ||
                            bChannelReturnCard == 0x62 ||
                            bChannelReturnCard == 0x63 ||
                            bChannelReturnCard == 0x70 ||
                            bChannelReturnCard == 0x71 ||
                            bChannelReturnCard == 0x72)
                        {
                            myCardSerialCOMType = enum_SerialCOMType.Detection_Board;
                        }
                        else if (bChannelReturnCard == 0x79 ||
                            bChannelReturnCard == 0x80 ||
                            bChannelReturnCard == 0x90)
                        {
                            myCardSerialCOMType = enum_SerialCOMType.Control_AC_Inboard_Board;
                        }
                        else if (bChannelReturnCard == 0x25 ||
                            bChannelReturnCard == 0x26 ||
                            bChannelReturnCard == 0x28 ||
                            bChannelReturnCard == 0x29)
                        {
                            myCardSerialCOMType = enum_SerialCOMType.Camera_Sensor_Power_Relay_Board;
                        }
                        else if (bChannelReturnCard == 0x40)
                        {
                            myCardSerialCOMType = enum_SerialCOMType.Tank_Detection_Board;
                        }
                        else if (bChannelReturnCard == 0x21 ||
                            bChannelReturnCard == 0x22 ||
                            bChannelReturnCard == 0x23 ||
                            bChannelReturnCard == 0x24)
                        {
                            myCardSerialCOMType = enum_SerialCOMType.Light_Relay_Control_Panel;
                        }

                        #endregion


                        Global.isCtlSending = true;

                        //发命令前，让查询等待时间
                        Thread.Sleep(30);

                        #region  发送执行数据
                        //1表示点击按钮，打开电源；
                        //2表示点击按钮，关闭电源；
                        //3表示仅仅关闭电源，不关闭板卡。

                        #region 1表示点击按钮，打开电源；
                        if (typeCmdSendOpen == 1)
                        {                            
                            if (myCtlPowerSerialCOMType == enum_SerialCOMType.Others)
                            {
                                string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t发送电源打开指令的地址错误\t";
                                Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                                Global.myLogStreamWriter.Flush();
                            }
                            else
                            {
                                isbCmdReturn = false;
                                flagCmdToSend = false;

                                //if (bChannelReturn != 0x61) //0x61的B路电源上电即输出, A路也需要控制；61B路常开,61A需要控制
                                {
                                    dicBoardMonCtlClassType[myCtlPowerSerialCOMType].BCmdInsert(cmdPowerOpen);//主控电源开
                                }

                                Thread.Sleep(sleepInternal);


                                GEventArgs myGEventArgs = new GEventArgs();
                                myGEventArgs.source = this.Name;

                                #region 确定messageID,用于DataGridView中显示报警信息的序号，从1000开始顺序排
                                switch (bChannelReturn)
                                {
                                    case 0x61:
                                        myGEventArgs.messageID = 1001;
                                        break;
                                    case 0x62:
                                        myGEventArgs.messageID = 1002;
                                        break;
                                    case 0x63:
                                        myGEventArgs.messageID = 1003;
                                        break;
                                    case 0x70:
                                        myGEventArgs.messageID = 1004;
                                        break;
                                    case 0x71:
                                        myGEventArgs.messageID = 1005;
                                        break;
                                    case 0x72:
                                        myGEventArgs.messageID = 1006;
                                        break;
                                    case 0x79:
                                        myGEventArgs.messageID = 1007;
                                        break;
                                    case 0x80:
                                        myGEventArgs.messageID = 1008;
                                        break;
                                    case 0x90:
                                        myGEventArgs.messageID = 1009;
                                        break;
                                    case 0x25:
                                        myGEventArgs.messageID = 1010;
                                        break;
                                    case 0x26:
                                        myGEventArgs.messageID = 1011;
                                        break;
                                    case 0x28:
                                        myGEventArgs.messageID = 1012;
                                        break;
                                    case 0x29:
                                        myGEventArgs.messageID = 1013;
                                        break;
                                    case 0x40:
                                        myGEventArgs.messageID = 1014;
                                        break;
                                    case 0x21:
                                        myGEventArgs.messageID = 1015;
                                        break;
                                    case 0x22:
                                        myGEventArgs.messageID = 1016;
                                        break;
                                    case 0x23:
                                        myGEventArgs.messageID = 1017;
                                        break;
                                    case 0x24:
                                        myGEventArgs.messageID = 1018;
                                        break;

                                    default:
                                        myGEventArgs.messageID = 1000;
                                        break;
                                }
                                #endregion
                                
                                if (isbCmdReturn == false)//没有数据反馈
                                {
                                    isbCmdReturn = false;
                                    flagCmdToSend = false;
                                    //continue; //总电源不开或者反馈错误或者没有反馈，仍旧发送后续继电器板开的指令
                                    
                                    myGEventArgs.message = "电源(0x" + bChannelReturn.ToString("X2") + ")没有指令反馈";
                                    if (DeviceFormStateEventSend != null && myGEventArgs.messageID != 1000)
                                    {
                                        DeviceFormStateEventSend(this, myGEventArgs);
                                    }
                                }
                                else if (bCmdReturn[0] == 0xFF && bCmdReturn[1] == 0xFF && bCmdReturn[2] == 0xA5 && bCmdReturn.Length >= 8)
                                {
                                    if (bCmdReturn[3] == bChannelReturn && bCmdReturn[4] == 0x82)//总电源已打开 
                                    {

                                    }
                                    else//电源未打开！
                                    {
                                        isbCmdReturn = false;
                                        flagCmdToSend = false;
                                        //continue; //总电源不开或者反馈错误或者没有反馈，仍旧发送后续继电器板开的指令

                                        myGEventArgs.message = "电源(0x" + bChannelReturn.ToString("X2") + ")指令反馈错误";
                                        if (DeviceFormStateEventSend != null && myGEventArgs.messageID != 1000)
                                        {
                                            DeviceFormStateEventSend(this, myGEventArgs);
                                        }
                                    }
                                }
                                else//有数据反馈，但数据错误
                                {
                                    isbCmdReturn = false;
                                    flagCmdToSend = false;
                                    //continue; //总电源不开或者反馈错误或者没有反馈，仍旧发送后续继电器板开的指令

                                    myGEventArgs.message = "电源(0x" + bChannelReturn.ToString("X2") + ")指令数据反馈错误";
                                    if (DeviceFormStateEventSend != null && myGEventArgs.messageID != 1000)
                                    {
                                        DeviceFormStateEventSend(this, myGEventArgs);
                                    }
                                }

                                //发送电源开后，不论反馈如何，标记为开状态
                                if (btnCmdindex >= 1 && btnCmdindex <= 4)//01 开电源板0x70，02开电源板0x71，04开电源板0x72，08，开备用继电器
                                {
                                    CtlPowerFlag79 |= 0x01;
                                }
                                else if (btnCmdindex >= 5 && btnCmdindex <= 8)//02开电源板0x71
                                {
                                    CtlPowerFlag79 |= 0x02;
                                }
                                else if (btnCmdindex >= 11 && btnCmdindex <= 22)    //61A
                                {
                                    CtlPowerFlag61 |= 0x01;
                                }
                                else if (btnCmdindex >= 31 && btnCmdindex <= 34)    //62B
                                {
                                    CtlPowerFlag62 |= 0x10;
                                }
                                else if (btnCmdindex >= 41 && btnCmdindex <= 44)    //62A
                                {
                                    CtlPowerFlag62 |= 0x01;
                                }
                                else if (btnCmdindex >= 45 && btnCmdindex <= 46)    //63B
                                {
                                    CtlPowerFlag63 |= 0x10;
                                }
                                else if (btnCmdindex >= 47 && btnCmdindex <= 48)    //63A
                                {
                                    CtlPowerFlag63 |= 0x01;
                                }
                            }

                            isbCmdReturn = false;//板卡加电前专控总电源指令反馈比对结束

                            #region 板卡执行数据发送
                            if (isOnlyPowerCtl == false)
                            {
                                flagCmdToSend = false;
                                dicBoardMonCtlClassType[myCardSerialCOMType].BCmdInsert(cmdOpenDevice);//板卡电源开

                                Thread.Sleep(sleepInternal);

                                GEventArgs myGEventArgs = new GEventArgs();
                                myGEventArgs.source = this.Name;

                                #region 确定messageID,用于DataGridView中显示报警信息的序号,bChannelReturnCard
                                switch (bChannelReturnCard)
                                {
                                    case 0x61:
                                        myGEventArgs.messageID = 1101;
                                        break;
                                    case 0x62:
                                        myGEventArgs.messageID = 1102;
                                        break;
                                    case 0x63:
                                        myGEventArgs.messageID = 1103;
                                        break;
                                    case 0x70:
                                        myGEventArgs.messageID = 1104;
                                        break;
                                    case 0x71:
                                        myGEventArgs.messageID = 1105;
                                        break;
                                    case 0x72:
                                        myGEventArgs.messageID = 1106;
                                        break;
                                    case 0x79:
                                        myGEventArgs.messageID = 1107;
                                        break;
                                    case 0x80:
                                        myGEventArgs.messageID = 1108;
                                        break;
                                    case 0x90:
                                        myGEventArgs.messageID = 1109;
                                        break;
                                    case 0x25:
                                        myGEventArgs.messageID = 1110;
                                        break;
                                    case 0x26:
                                        myGEventArgs.messageID = 1111;
                                        break;
                                    case 0x28:
                                        myGEventArgs.messageID = 1112;
                                        break;
                                    case 0x29:
                                        myGEventArgs.messageID = 1113;
                                        break;
                                    case 0x40:
                                        myGEventArgs.messageID = 1114;
                                        break;
                                    case 0x21:
                                        myGEventArgs.messageID = 1115;
                                        break;
                                    case 0x22:
                                        myGEventArgs.messageID = 1116;
                                        break;
                                    case 0x23:
                                        myGEventArgs.messageID = 1117;
                                        break;
                                    case 0x24:
                                        myGEventArgs.messageID = 1118;
                                        break;

                                    default:
                                        myGEventArgs.messageID = 1100;
                                        break;
                                }
                                #endregion

                                if (isbCmdReturn == false)//没有数据反馈
                                {

                                    #region 板卡加电指令发送、反馈比对结束后，更新相应控制按钮的底色，以区分加断电，此为反馈错误，变为Yellow

                                    this.BeginInvoke(new Func<object>(() =>
                                    {
                                        //第一组，btnCmdindex为1~8，更新标志字powerOpenFlag1
                                        if (btnCmdindex == 1)
                                        {
                                            btn_Light_1.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                            powerOpenFlag1 |= 0x01; //使用b0,最低位
                                        }
                                        else if (btnCmdindex == 2)
                                        {
                                            btn_Light_2.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                            powerOpenFlag1 |= 0x02;
                                        }
                                        else if (btnCmdindex == 3)
                                        {
                                            btn_Light_3.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                            powerOpenFlag1 |= 0x04;
                                        }
                                        else if (btnCmdindex == 4)
                                        {
                                            btn_Light_4.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                            powerOpenFlag1 |= 0x08;
                                        }
                                        else if (btnCmdindex == 5)
                                        {
                                            btn_Light_5.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                            powerOpenFlag1 |= 0x10;
                                        }
                                        else if (btnCmdindex == 6)
                                        {
                                            btn_Light_6.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                            powerOpenFlag1 |= 0x20;
                                        }
                                        else if (btnCmdindex == 7)
                                        {
                                            btn_Light_7.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                            powerOpenFlag1 |= 0x40;
                                        }
                                        else if (btnCmdindex == 8)
                                        {
                                            btn_Light_8.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                            powerOpenFlag1 |= 0x80;
                                        }

                                        //第一组，btnCmdindex为11~18，更新标志字powerOpenFlag2
                                        else if (btnCmdindex == 11)
                                        {
                                            btn_Camera_1.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                            powerOpenFlag2 |= 0x01;
                                        }
                                        else if (btnCmdindex == 12)
                                        {
                                            btn_Camera_2.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                            powerOpenFlag2 |= 0x02;
                                        }
                                        else if (btnCmdindex == 13)
                                        {
                                            btn_Camera_3.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                            powerOpenFlag2 |= 0x04;
                                        }
                                        else if (btnCmdindex == 14)
                                        {
                                            btn_Camera_4.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                            powerOpenFlag2 |= 0x08;
                                        }
                                        else if (btnCmdindex == 15)
                                        {
                                            btn_Camera_5.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                            powerOpenFlag2 |= 0x10;
                                        }
                                        else if (btnCmdindex == 16)
                                        {
                                            btn_Camera_6.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                            powerOpenFlag2 |= 0x20;
                                        }
                                        else if (btnCmdindex == 17)
                                        {
                                            btn_Camera_7.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                            powerOpenFlag2 |= 0x40;
                                        }
                                        else if (btnCmdindex == 18)
                                        {
                                            btn_Camera_8.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                            powerOpenFlag2 |= 0x80;
                                        }

                                        else if (btnCmdindex == 19)
                                        {
                                            btn_DetectPanel_Rotate.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                            powerOpenFlag2 |= 0x100;
                                        }
                                        else if (btnCmdindex == 20)
                                        {
                                            btn_DetectPanel_Space_12V1.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                            powerOpenFlag2 |= 0x200;
                                        }
                                        else if (btnCmdindex == 21)
                                        {
                                            btn_DetectPanel_Space_12V2.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                            powerOpenFlag2 |= 0x400;
                                        }
                                        else if (btnCmdindex == 22)
                                        {
                                            btn_DetectPanel_Space_12V3.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                            powerOpenFlag2 |= 0x800;
                                        }

                                        else if (btnCmdindex == 31)
                                        {
                                            btn_DetectPanel_Hight.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                            powerOpenFlag3 |= 0x01;
                                        }
                                        else if (btnCmdindex == 32)
                                        {
                                            btn_DetectPanel_Deep.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                            powerOpenFlag3 |= 0x02;
                                        }
                                        else if (btnCmdindex == 33)
                                        {
                                            btn_DetectPanel_Space_Bak1.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                            powerOpenFlag3 |= 0x04;
                                        }
                                        else if (btnCmdindex == 34)
                                        {
                                            btn_DetectPanel_Space_Bak2.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                            powerOpenFlag3 |= 0x08;
                                        }

                                        //第三组，舱内备用电源继电器板（0x90），41-48
                                        else if (btnCmdindex == 41)
                                        {
                                            btn_InboardBackupPRB_24V1.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                            powerOpenFlag4 |= 0x01;
                                        }
                                        else if (btnCmdindex == 42)
                                        {
                                            btn_InboardBackupPRB_24V2.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                            powerOpenFlag4 |= 0x02;
                                        }
                                        else if (btnCmdindex == 43)
                                        {
                                            btn_InboardBackupPRB_24V3.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                            powerOpenFlag4 |= 0x04;
                                        }
                                        else if (btnCmdindex == 44)
                                        {
                                            btn_InboardBackupPRB_24V4.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                            powerOpenFlag4 |= 0x08;
                                        }
                                        else if (btnCmdindex == 45)
                                        {
                                            btn_InboardBackupPRB_Camera.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                            powerOpenFlag4 |= 0x10;
                                        }
                                        else if (btnCmdindex == 46)
                                        {
                                            btn_InboardBackupPRB_W.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                            powerOpenFlag4 |= 0x20;
                                        }
                                        else if (btnCmdindex == 47)
                                        {
                                            btn_InboardBackupPRB_12V1.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                            powerOpenFlag4 |= 0x40;
                                        }
                                        else if (btnCmdindex == 48)
                                        {
                                            btn_InboardBackupPRB_12V2.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                            powerOpenFlag4 |= 0x80;
                                        }

                                        return null;
                                    }));

                                    #endregion

                                    myGEventArgs.message = "板卡(0x" + bChannelReturnCard.ToString("X2") + ")没有指令反馈";
                                    if (DeviceFormStateEventSend != null)
                                    {
                                        DeviceFormStateEventSend(this, myGEventArgs);
                                    }
                                }
                                else
                                {
                                    if (bCmdReturn[0] == 0xFF && bCmdReturn[1] == 0xFF && bCmdReturn[2] == 0xA5 && bCmdReturn.Length >= 8)
                                    {
                                        if (bCmdReturn[3] == bChannelReturnCard && bCmdReturn[4] == 0x82)
                                        {
                                            #region 板卡加电指令发送、反馈比对结束后，更新相应控制按钮的底色，以区分加断电，此为正常返回，变为green

                                            this.BeginInvoke(new Func<object>(() =>
                                            {
                                                //第一组，btnCmdindex为1~8，更新标志字powerOpenFlag1，加电OK，按钮颜色变绿
                                                if (btnCmdindex == 1)
                                                {
                                                    btn_Light_1.BackgroundImage = global::MonitorProj.Properties.Resources.Button3;
                                                    powerOpenFlag1 |= 0x01; //使用b0,最低位
                                                }
                                                else if (btnCmdindex == 2)
                                                {
                                                    btn_Light_2.BackgroundImage = global::MonitorProj.Properties.Resources.Button3;
                                                    powerOpenFlag1 |= 0x02;
                                                }
                                                else if (btnCmdindex == 3)
                                                {
                                                    btn_Light_3.BackgroundImage = global::MonitorProj.Properties.Resources.Button3;
                                                    powerOpenFlag1 |= 0x04;
                                                }
                                                else if (btnCmdindex == 4)
                                                {
                                                    btn_Light_4.BackgroundImage = global::MonitorProj.Properties.Resources.Button3;
                                                    powerOpenFlag1 |= 0x08;
                                                }
                                                else if (btnCmdindex == 5)
                                                {
                                                    btn_Light_5.BackgroundImage = global::MonitorProj.Properties.Resources.Button3;
                                                    powerOpenFlag1 |= 0x10;
                                                }
                                                else if (btnCmdindex == 6)
                                                {
                                                    btn_Light_6.BackgroundImage = global::MonitorProj.Properties.Resources.Button3;
                                                    powerOpenFlag1 |= 0x20;
                                                }
                                                else if (btnCmdindex == 7)
                                                {
                                                    btn_Light_7.BackgroundImage = global::MonitorProj.Properties.Resources.Button3;
                                                    powerOpenFlag1 |= 0x40;
                                                }
                                                else if (btnCmdindex == 8)
                                                {
                                                    btn_Light_8.BackgroundImage = global::MonitorProj.Properties.Resources.Button3;
                                                    powerOpenFlag1 |= 0x80;
                                                }
                                                //第一组，btnCmdindex为11~18，更新标志字powerOpenFlag2
                                                else if (btnCmdindex == 11)
                                                {
                                                    btn_Camera_1.BackgroundImage = global::MonitorProj.Properties.Resources.Button3;
                                                    powerOpenFlag2 |= 0x01;
                                                }
                                                else if (btnCmdindex == 12)
                                                {
                                                    btn_Camera_2.BackgroundImage = global::MonitorProj.Properties.Resources.Button3;
                                                    powerOpenFlag2 |= 0x02;
                                                }
                                                else if (btnCmdindex == 13)
                                                {
                                                    btn_Camera_3.BackgroundImage = global::MonitorProj.Properties.Resources.Button3;
                                                    powerOpenFlag2 |= 0x04;
                                                }
                                                else if (btnCmdindex == 14)
                                                {
                                                    btn_Camera_4.BackgroundImage = global::MonitorProj.Properties.Resources.Button3;
                                                    powerOpenFlag2 |= 0x08;
                                                }
                                                else if (btnCmdindex == 15)
                                                {
                                                    btn_Camera_5.BackgroundImage = global::MonitorProj.Properties.Resources.Button3;
                                                    powerOpenFlag2 |= 0x10;
                                                }
                                                else if (btnCmdindex == 16)
                                                {
                                                    btn_Camera_6.BackgroundImage = global::MonitorProj.Properties.Resources.Button3;
                                                    powerOpenFlag2 |= 0x20;
                                                }
                                                else if (btnCmdindex == 17)
                                                {
                                                    btn_Camera_7.BackgroundImage = global::MonitorProj.Properties.Resources.Button3;
                                                    powerOpenFlag2 |= 0x40;
                                                }
                                                else if (btnCmdindex == 18)
                                                {
                                                    btn_Camera_8.BackgroundImage = global::MonitorProj.Properties.Resources.Button3;
                                                    powerOpenFlag2 |= 0x80;
                                                }

                                                else if (btnCmdindex == 19)
                                                {
                                                    btn_DetectPanel_Rotate.BackgroundImage = global::MonitorProj.Properties.Resources.Button3;
                                                    powerOpenFlag2 |= 0x100;
                                                }
                                                else if (btnCmdindex == 20)
                                                {
                                                    btn_DetectPanel_Space_12V1.BackgroundImage = global::MonitorProj.Properties.Resources.Button3;
                                                    powerOpenFlag2 |= 0x200;
                                                }
                                                else if (btnCmdindex == 21)
                                                {
                                                    btn_DetectPanel_Space_12V2.BackgroundImage = global::MonitorProj.Properties.Resources.Button3;
                                                    powerOpenFlag2 |= 0x400;
                                                }
                                                else if (btnCmdindex == 22)
                                                {
                                                    btn_DetectPanel_Space_12V3.BackgroundImage = global::MonitorProj.Properties.Resources.Button3;
                                                    powerOpenFlag2 |= 0x800;
                                                }

                                                else if (btnCmdindex == 31)
                                                {
                                                    btn_DetectPanel_Hight.BackgroundImage = global::MonitorProj.Properties.Resources.Button3;
                                                    powerOpenFlag3 |= 0x01;
                                                }
                                                else if (btnCmdindex == 32)
                                                {
                                                    btn_DetectPanel_Deep.BackgroundImage = global::MonitorProj.Properties.Resources.Button3;
                                                    powerOpenFlag3 |= 0x02;
                                                }
                                                else if (btnCmdindex == 33)
                                                {
                                                    btn_DetectPanel_Space_Bak1.BackgroundImage = global::MonitorProj.Properties.Resources.Button3;
                                                    powerOpenFlag3 |= 0x04;
                                                }
                                                else if (btnCmdindex == 34)
                                                {
                                                    btn_DetectPanel_Space_Bak2.BackgroundImage = global::MonitorProj.Properties.Resources.Button3;
                                                    powerOpenFlag3 |= 0x08;
                                                }
                                                
                                                //第三组，舱内备用电源继电器板（0x90），41-48
                                                else if (btnCmdindex == 41)
                                                {
                                                    btn_InboardBackupPRB_24V1.BackgroundImage = global::MonitorProj.Properties.Resources.Button3;
                                                    powerOpenFlag4 |= 0x01;
                                                }
                                                else if (btnCmdindex == 42)
                                                {
                                                    btn_InboardBackupPRB_24V2.BackgroundImage = global::MonitorProj.Properties.Resources.Button3;
                                                    powerOpenFlag4 |= 0x02;
                                                }
                                                else if (btnCmdindex == 43)
                                                {
                                                    btn_InboardBackupPRB_24V3.BackgroundImage = global::MonitorProj.Properties.Resources.Button3;
                                                    powerOpenFlag4 |= 0x04;
                                                }
                                                else if (btnCmdindex == 44)
                                                {
                                                    btn_InboardBackupPRB_24V4.BackgroundImage = global::MonitorProj.Properties.Resources.Button3;
                                                    powerOpenFlag4 |= 0x08;
                                                }
                                                else if (btnCmdindex == 45)
                                                {
                                                    btn_InboardBackupPRB_Camera.BackgroundImage = global::MonitorProj.Properties.Resources.Button3;
                                                    powerOpenFlag4 |= 0x10;
                                                }
                                                else if (btnCmdindex == 46)
                                                {
                                                    btn_InboardBackupPRB_W.BackgroundImage = global::MonitorProj.Properties.Resources.Button3;
                                                    powerOpenFlag4 |= 0x20;
                                                }
                                                else if (btnCmdindex == 47)
                                                {
                                                    btn_InboardBackupPRB_12V1.BackgroundImage = global::MonitorProj.Properties.Resources.Button3;
                                                    powerOpenFlag4 |= 0x40;
                                                }
                                                else if (btnCmdindex == 48)
                                                {
                                                    btn_InboardBackupPRB_12V2.BackgroundImage = global::MonitorProj.Properties.Resources.Button3;
                                                    powerOpenFlag4 |= 0x80;
                                                }


                                                return null;
                                            }));

                                            #endregion
                                        }
                                        else
                                        {
                                            #region 板卡加电指令发送、反馈比对结束后，更新相应控制按钮的底色，以区分加断电，此为反馈错误，变为Yellow

                                            this.BeginInvoke(new Func<object>(() =>
                                            {
                                                //第一组，btnCmdindex为1~8，更新标志字powerOpenFlag1
                                                if (btnCmdindex == 1)
                                                {
                                                    btn_Light_1.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                                    powerOpenFlag1 |= 0x01;
                                                }
                                                else if (btnCmdindex == 2)
                                                {
                                                    btn_Light_2.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                                    powerOpenFlag1 |= 0x02;
                                                }
                                                else if (btnCmdindex == 3)
                                                {
                                                    btn_Light_3.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                                    powerOpenFlag1 |= 0x04;
                                                }
                                                else if (btnCmdindex == 4)
                                                {
                                                    btn_Light_4.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                                    powerOpenFlag1 |= 0x08;
                                                }
                                                else if (btnCmdindex == 5)
                                                {
                                                    btn_Light_5.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                                    powerOpenFlag1 |= 0x10;
                                                }
                                                else if (btnCmdindex == 6)
                                                {
                                                    btn_Light_6.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                                    powerOpenFlag1 |= 0x20;
                                                }
                                                else if (btnCmdindex == 7)
                                                {
                                                    btn_Light_7.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                                    powerOpenFlag1 |= 0x40;
                                                }
                                                else if (btnCmdindex == 8)
                                                {
                                                    btn_Light_8.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                                    powerOpenFlag1 |= 0x80;
                                                }

                                                //第一组，btnCmdindex为11~18，更新标志字powerOpenFlag2
                                                else if (btnCmdindex == 11)
                                                {
                                                    btn_Camera_1.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                                    powerOpenFlag2 |= 0x01;
                                                }
                                                else if (btnCmdindex == 12)
                                                {
                                                    btn_Camera_2.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                                    powerOpenFlag2 |= 0x02;
                                                }
                                                else if (btnCmdindex == 13)
                                                {
                                                    btn_Camera_3.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                                    powerOpenFlag2 |= 0x04;
                                                }
                                                else if (btnCmdindex == 14)
                                                {
                                                    btn_Camera_4.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                                    powerOpenFlag2 |= 0x08;
                                                }
                                                else if (btnCmdindex == 15)
                                                {
                                                    btn_Camera_5.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                                    powerOpenFlag2 |= 0x10;
                                                }
                                                else if (btnCmdindex == 16)
                                                {
                                                    btn_Camera_6.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                                    powerOpenFlag2 |= 0x20;
                                                }
                                                else if (btnCmdindex == 17)
                                                {
                                                    btn_Camera_7.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                                    powerOpenFlag2 |= 0x40;
                                                }
                                                else if (btnCmdindex == 18)
                                                {
                                                    btn_Camera_8.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                                    powerOpenFlag2 |= 0x80;
                                                }

                                                else if (btnCmdindex == 19)
                                                {
                                                    btn_DetectPanel_Rotate.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                                    powerOpenFlag2 |= 0x100;
                                                }
                                                else if (btnCmdindex == 20)
                                                {
                                                    btn_DetectPanel_Space_12V1.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                                    powerOpenFlag2 |= 0x200;
                                                }
                                                else if (btnCmdindex == 21)
                                                {
                                                    btn_DetectPanel_Space_12V2.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                                    powerOpenFlag2 |= 0x400;
                                                }
                                                else if (btnCmdindex == 22)
                                                {
                                                    btn_DetectPanel_Space_12V3.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                                    powerOpenFlag2 |= 0x800;
                                                }
                                                    
                                                else if (btnCmdindex == 31)
                                                {
                                                    btn_DetectPanel_Hight.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                                    powerOpenFlag3 |= 0x01;
                                                }
                                                else if (btnCmdindex == 32)
                                                {
                                                    btn_DetectPanel_Deep.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                                    powerOpenFlag3 |= 0x02;
                                                }
                                                else if (btnCmdindex == 33)
                                                {
                                                    btn_DetectPanel_Space_Bak1.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                                    powerOpenFlag3 |= 0x04;
                                                }
                                                else if (btnCmdindex == 34)
                                                {
                                                    btn_DetectPanel_Space_Bak2.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                                    powerOpenFlag3 |= 0x08;
                                                }

                                                
                                                //第三组，舱内备用电源继电器板（0x90），41-48
                                                else if (btnCmdindex == 41)
                                                {
                                                    btn_InboardBackupPRB_24V1.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                                    powerOpenFlag4 |= 0x01;
                                                }
                                                else if (btnCmdindex == 42)
                                                {
                                                    btn_InboardBackupPRB_24V2.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                                    powerOpenFlag4 |= 0x02;
                                                }
                                                else if (btnCmdindex == 43)
                                                {
                                                    btn_InboardBackupPRB_24V3.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                                    powerOpenFlag4 |= 0x04;
                                                }
                                                else if (btnCmdindex == 44)
                                                {
                                                    btn_InboardBackupPRB_24V4.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                                    powerOpenFlag4 |= 0x08;
                                                }
                                                else if (btnCmdindex == 45)
                                                {
                                                    btn_InboardBackupPRB_Camera.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                                    powerOpenFlag4 |= 0x10;
                                                }
                                                else if (btnCmdindex == 46)
                                                {
                                                    btn_InboardBackupPRB_W.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                                    powerOpenFlag4 |= 0x20;
                                                }
                                                else if (btnCmdindex == 47)
                                                {
                                                    btn_InboardBackupPRB_12V1.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                                    powerOpenFlag4 |= 0x40;
                                                }
                                                else if (btnCmdindex == 48)
                                                {
                                                    btn_InboardBackupPRB_12V2.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                                    powerOpenFlag4 |= 0x80;
                                                }

                                                return null;
                                            }));

                                            #endregion

                                            myGEventArgs.message = "板卡(0x" + bChannelReturnCard.ToString("X2") + ")指令反馈错误";
                                            if (DeviceFormStateEventSend != null)
                                            {
                                                DeviceFormStateEventSend(this, myGEventArgs);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        #region 板卡加电指令发送、反馈比对结束后，更新相应控制按钮的底色，以区分加断电，此为反馈错误，变为Yellow

                                        this.BeginInvoke(new Func<object>(() =>
                                        {
                                            //第一组，btnCmdindex为1~8，更新标志字powerOpenFlag1
                                            if (btnCmdindex == 1)
                                            {
                                                btn_Light_1.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                                powerOpenFlag1 |= 0x01; //使用b0,最低位
                                            }
                                            else if (btnCmdindex == 2)
                                            {
                                                btn_Light_2.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                                powerOpenFlag1 |= 0x02;
                                            }
                                            else if (btnCmdindex == 3)
                                            {
                                                btn_Light_3.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                                powerOpenFlag1 |= 0x04;
                                            }
                                            else if (btnCmdindex == 4)
                                            {
                                                btn_Light_4.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                                powerOpenFlag1 |= 0x08;
                                            }
                                            else if (btnCmdindex == 5)
                                            {
                                                btn_Light_5.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                                powerOpenFlag1 |= 0x10;
                                            }
                                            else if (btnCmdindex == 6)
                                            {
                                                btn_Light_6.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                                powerOpenFlag1 |= 0x20;
                                            }
                                            else if (btnCmdindex == 7)
                                            {
                                                btn_Light_7.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                                powerOpenFlag1 |= 0x40;
                                            }
                                            else if (btnCmdindex == 8)
                                            {
                                                btn_Light_8.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                                powerOpenFlag1 |= 0x80;
                                            }

                                            //第一组，btnCmdindex为11~18，更新标志字powerOpenFlag2
                                            else if (btnCmdindex == 11)
                                            {
                                                btn_Camera_1.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                                powerOpenFlag2 |= 0x01;
                                            }
                                            else if (btnCmdindex == 12)
                                            {
                                                btn_Camera_2.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                                powerOpenFlag2 |= 0x02;
                                            }
                                            else if (btnCmdindex == 13)
                                            {
                                                btn_Camera_3.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                                powerOpenFlag2 |= 0x04;
                                            }
                                            else if (btnCmdindex == 14)
                                            {
                                                btn_Camera_4.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                                powerOpenFlag2 |= 0x08;
                                            }
                                            else if (btnCmdindex == 15)
                                            {
                                                btn_Camera_5.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                                powerOpenFlag2 |= 0x10;
                                            }
                                            else if (btnCmdindex == 16)
                                            {
                                                btn_Camera_6.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                                powerOpenFlag2 |= 0x20;
                                            }
                                            else if (btnCmdindex == 17)
                                            {
                                                btn_Camera_7.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                                powerOpenFlag2 |= 0x40;
                                            }
                                            else if (btnCmdindex == 18)
                                            {
                                                btn_Camera_8.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                                powerOpenFlag2 |= 0x80;
                                            }

                                            else if (btnCmdindex == 19)
                                            {
                                                btn_DetectPanel_Rotate.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                                powerOpenFlag2 |= 0x100;
                                            }
                                            else if (btnCmdindex == 20)
                                            {
                                                btn_DetectPanel_Space_12V1.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                                powerOpenFlag2 |= 0x200;
                                            }
                                            else if (btnCmdindex == 21)
                                            {
                                                btn_DetectPanel_Space_12V2.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                                powerOpenFlag2 |= 0x400;
                                            }
                                            else if (btnCmdindex == 22)
                                            {
                                                btn_DetectPanel_Space_12V3.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                                powerOpenFlag2 |= 0x800;
                                            }

                                            else if (btnCmdindex == 31)
                                            {
                                                btn_DetectPanel_Hight.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                                powerOpenFlag3 |= 0x01;
                                            }
                                            else if (btnCmdindex == 32)
                                            {
                                                btn_DetectPanel_Deep.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                                powerOpenFlag3 |= 0x02;
                                            }
                                            else if (btnCmdindex == 33)
                                            {
                                                btn_DetectPanel_Space_Bak1.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                                powerOpenFlag3 |= 0x04;
                                            }
                                            else if (btnCmdindex == 34)
                                            {
                                                btn_DetectPanel_Space_Bak2.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                                powerOpenFlag3 |= 0x08;
                                            }

                                            
                                            //第三组，舱内备用电源继电器板（0x90），41-48
                                            else if (btnCmdindex == 41)
                                            {
                                                btn_InboardBackupPRB_24V1.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                                powerOpenFlag4 |= 0x01;
                                            }
                                            else if (btnCmdindex == 42)
                                            {
                                                btn_InboardBackupPRB_24V2.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                                powerOpenFlag4 |= 0x02;
                                            }
                                            else if (btnCmdindex == 43)
                                            {
                                                btn_InboardBackupPRB_24V3.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                                powerOpenFlag4 |= 0x04;
                                            }
                                            else if (btnCmdindex == 44)
                                            {
                                                btn_InboardBackupPRB_24V4.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                                powerOpenFlag4 |= 0x08;
                                            }
                                            else if (btnCmdindex == 45)
                                            {
                                                btn_InboardBackupPRB_Camera.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                                powerOpenFlag4 |= 0x10;
                                            }
                                            else if (btnCmdindex == 46)
                                            {
                                                btn_InboardBackupPRB_W.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                                powerOpenFlag4 |= 0x20;
                                            }
                                            else if (btnCmdindex == 47)
                                            {
                                                btn_InboardBackupPRB_12V1.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                                powerOpenFlag4 |= 0x40;
                                            }
                                            else if (btnCmdindex == 48)
                                            {
                                                btn_InboardBackupPRB_12V2.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                                powerOpenFlag4 |= 0x80;
                                            }

                                            return null;
                                        }));

                                        #endregion

                                        myGEventArgs.message = "板卡(0x" + bChannelReturnCard.ToString("X2") + ")指令反馈错误";
                                        if (DeviceFormStateEventSend != null)
                                        {
                                            DeviceFormStateEventSend(this, myGEventArgs);
                                        }
                                    }
                                }
                                
                                isbCmdReturn = false;
                            }
                            #endregion
                            
                            isOnlyPowerCtl = false;
                        }
                        #endregion

                        #region 2表示点击按钮，关闭电源；
                        else if (typeCmdSendOpen == 2)
                        {
                            isCtlRetOK = false;
                            isbCmdReturn = false;

                            #region 发送板卡关闭
                            flagCmdToSend = false;
                            dicBoardMonCtlClassType[myCardSerialCOMType].BCmdInsert(cmdCloseDevice);//板卡电源关闭

                            Thread.Sleep(sleepInternal);

                            GEventArgs myGEventArgs = new GEventArgs();
                            myGEventArgs.source = this.Name;

                            #region 确定messageID,用于DataGridView中显示报警信息的序号,bChannelReturnCard
                            switch (bChannelReturnCard)
                            {
                                case 0x61:
                                    myGEventArgs.messageID = 1101;
                                    break;
                                case 0x62:
                                    myGEventArgs.messageID = 1102;
                                    break;
                                case 0x63:
                                    myGEventArgs.messageID = 1103;
                                    break;
                                case 0x70:
                                    myGEventArgs.messageID = 1104;
                                    break;
                                case 0x71:
                                    myGEventArgs.messageID = 1105;
                                    break;
                                case 0x72:
                                    myGEventArgs.messageID = 1106;
                                    break;
                                case 0x79:
                                    myGEventArgs.messageID = 1107;
                                    break;
                                case 0x80:
                                    myGEventArgs.messageID = 1108;
                                    break;
                                case 0x90:
                                    myGEventArgs.messageID = 1109;
                                    break;
                                case 0x25:
                                    myGEventArgs.messageID = 1110;
                                    break;
                                case 0x26:
                                    myGEventArgs.messageID = 1111;
                                    break;
                                case 0x28:
                                    myGEventArgs.messageID = 1112;
                                    break;
                                case 0x29:
                                    myGEventArgs.messageID = 1113;
                                    break;
                                case 0x40:
                                    myGEventArgs.messageID = 1114;
                                    break;
                                case 0x21:
                                    myGEventArgs.messageID = 1115;
                                    break;
                                case 0x22:
                                    myGEventArgs.messageID = 1116;
                                    break;
                                case 0x23:
                                    myGEventArgs.messageID = 1117;
                                    break;
                                case 0x24:
                                    myGEventArgs.messageID = 1118;
                                    break;

                                default:
                                    myGEventArgs.messageID = 1100;
                                    break;
                            }
                            #endregion

                            if (isbCmdReturn == false)
                            {
                                myGEventArgs.message = "板卡(0x" + bChannelReturnCard.ToString("X2") + ")没有指令反馈";
                                if (DeviceFormStateEventSend != null)
                                {
                                    DeviceFormStateEventSend(this, myGEventArgs);
                                }
                            }
                            else
                            {
                                if (bCmdReturn[0] == 0xFF && bCmdReturn[1] == 0xFF && bCmdReturn[2] == 0xA5 && bCmdReturn.Length >= 8)
                                {
                                    if (bCmdReturn[3] == bChannelReturnCard && bCmdReturn[4] == 0x82)
                                    {
                                        isCtlRetOK = true;
                                    }
                                    else
                                    {
                                        myGEventArgs.message = "板卡(0x" + bChannelReturnCard.ToString("X2") + ")指令反馈错误";
                                        if (DeviceFormStateEventSend != null)
                                        {
                                            DeviceFormStateEventSend(this, myGEventArgs);
                                        }
                                    }
                                    isbCmdReturn = false;
                                }
                                else
                                {
                                    myGEventArgs.message = "板卡(0x" + bChannelReturnCard.ToString("X2") + ")指令数据反馈错误";
                                    if (DeviceFormStateEventSend != null)
                                    {
                                        DeviceFormStateEventSend(this, myGEventArgs);
                                    }
                                }
                            }

                            #endregion

                            #region 板卡短电指令发送、反馈比对结束后，更新相应控制按钮的底色，以区分加断电

                            this.BeginInvoke(new Func<object>(() =>
                            {
                                //第一组，btnCmdindex为1~8，断电OK，按钮颜色变灰色
                                if (btnCmdindex == 1)
                                {
                                    btn_Light_1.BackgroundImage = global::MonitorProj.Properties.Resources.Button1;
                                    powerOpenFlag1 &= ~(0x01);
                                }
                                else if (btnCmdindex == 2)
                                {
                                    btn_Light_2.BackgroundImage = global::MonitorProj.Properties.Resources.Button1;
                                    powerOpenFlag1 &= ~(0x02);
                                }
                                else if (btnCmdindex == 3)
                                {
                                    btn_Light_3.BackgroundImage = global::MonitorProj.Properties.Resources.Button1;
                                    powerOpenFlag1 &= ~(0x04);
                                }
                                else if (btnCmdindex == 4)
                                {
                                    btn_Light_4.BackgroundImage = global::MonitorProj.Properties.Resources.Button1;
                                    powerOpenFlag1 &= ~(0x08);
                                }
                                else if (btnCmdindex == 5)
                                {
                                    btn_Light_5.BackgroundImage = global::MonitorProj.Properties.Resources.Button1;
                                    powerOpenFlag1 &= ~(0x10);
                                }
                                else if (btnCmdindex == 6)
                                {
                                    btn_Light_6.BackgroundImage = global::MonitorProj.Properties.Resources.Button1;
                                    powerOpenFlag1 &= ~(0x20);
                                }
                                else if (btnCmdindex == 7)
                                {
                                    btn_Light_7.BackgroundImage = global::MonitorProj.Properties.Resources.Button1;
                                    powerOpenFlag1 &= ~(0x40);
                                }
                                else if (btnCmdindex == 8)
                                {
                                    btn_Light_8.BackgroundImage = global::MonitorProj.Properties.Resources.Button1;
                                    powerOpenFlag1 &= ~(0x80);
                                }

                                //第一组，btnCmdindex为11~18，更新标志字powerOpenFlag2
                                else if (btnCmdindex == 11)
                                {
                                    btn_Camera_1.BackgroundImage = global::MonitorProj.Properties.Resources.Button1;
                                    powerOpenFlag2 &= ~(0x01);
                                }
                                else if (btnCmdindex == 12)
                                {
                                    btn_Camera_2.BackgroundImage = global::MonitorProj.Properties.Resources.Button1;
                                    powerOpenFlag2 &= ~(0x02);
                                }
                                else if (btnCmdindex == 13)
                                {
                                    btn_Camera_3.BackgroundImage = global::MonitorProj.Properties.Resources.Button1;
                                    powerOpenFlag2 &= ~(0x04);
                                }
                                else if (btnCmdindex == 14)
                                {
                                    btn_Camera_4.BackgroundImage = global::MonitorProj.Properties.Resources.Button1;
                                    powerOpenFlag2 &= ~(0x08);
                                }
                                else if (btnCmdindex == 15)
                                {
                                    btn_Camera_5.BackgroundImage = global::MonitorProj.Properties.Resources.Button1;
                                    powerOpenFlag2 &= ~(0x10);
                                }
                                else if (btnCmdindex == 16)
                                {
                                    btn_Camera_6.BackgroundImage = global::MonitorProj.Properties.Resources.Button1;
                                    powerOpenFlag2 &= ~(0x20);
                                }
                                else if (btnCmdindex == 17)
                                {
                                    btn_Camera_7.BackgroundImage = global::MonitorProj.Properties.Resources.Button1;
                                    powerOpenFlag2 &= ~(0x40);
                                }
                                else if (btnCmdindex == 18)
                                {
                                    btn_Camera_8.BackgroundImage = global::MonitorProj.Properties.Resources.Button1;
                                    powerOpenFlag2 &= ~(0x80);
                                }

                                else if (btnCmdindex == 19)
                                {
                                    btn_DetectPanel_Rotate.BackgroundImage = global::MonitorProj.Properties.Resources.Button1;
                                    powerOpenFlag2 &= ~(0x100);
                                }
                                else if (btnCmdindex == 20)
                                {
                                    btn_DetectPanel_Space_12V1.BackgroundImage = global::MonitorProj.Properties.Resources.Button1;
                                    powerOpenFlag2 &= ~(0x200);
                                }
                                else if (btnCmdindex == 21)
                                {
                                    btn_DetectPanel_Space_12V2.BackgroundImage = global::MonitorProj.Properties.Resources.Button1;
                                    powerOpenFlag2 &= ~(0x400);
                                }
                                else if (btnCmdindex == 22)
                                {
                                    btn_DetectPanel_Space_12V3.BackgroundImage = global::MonitorProj.Properties.Resources.Button1;
                                    powerOpenFlag2 &= ~(0x800);
                                }
                                    
                                else if (btnCmdindex == 31)
                                {
                                    btn_DetectPanel_Hight.BackgroundImage = global::MonitorProj.Properties.Resources.Button1;
                                    powerOpenFlag3 &= ~(0x01);
                                }
                                else if (btnCmdindex == 32)
                                {
                                    btn_DetectPanel_Deep.BackgroundImage = global::MonitorProj.Properties.Resources.Button1;
                                    powerOpenFlag3 &= ~(0x02);
                                }
                                else if (btnCmdindex == 33)
                                {
                                    btn_DetectPanel_Space_Bak1.BackgroundImage = global::MonitorProj.Properties.Resources.Button1;
                                    powerOpenFlag3 &= ~(0x04);
                                }
                                else if (btnCmdindex == 34)
                                {
                                    btn_DetectPanel_Space_Bak2.BackgroundImage = global::MonitorProj.Properties.Resources.Button1;
                                    powerOpenFlag3 &= ~(0x08);
                                }

                                //第三组，舱内备用电源继电器板（0x90），41-48
                                else if (btnCmdindex == 41)
                                {
                                    btn_InboardBackupPRB_24V1.BackgroundImage = global::MonitorProj.Properties.Resources.Button1;
                                    powerOpenFlag4 &= ~(0x01);
                                }
                                else if (btnCmdindex == 42)
                                {
                                    btn_InboardBackupPRB_24V2.BackgroundImage = global::MonitorProj.Properties.Resources.Button1;
                                    powerOpenFlag4 &= ~(0x02);
                                }
                                else if (btnCmdindex == 43)
                                {
                                    btn_InboardBackupPRB_24V3.BackgroundImage = global::MonitorProj.Properties.Resources.Button1;
                                    powerOpenFlag4 &= ~(0x04);
                                }
                                else if (btnCmdindex == 44)
                                {
                                    btn_InboardBackupPRB_24V4.BackgroundImage = global::MonitorProj.Properties.Resources.Button1;
                                    powerOpenFlag4 &= ~(0x08);
                                }
                                else if (btnCmdindex == 45)
                                {
                                    btn_InboardBackupPRB_Camera.BackgroundImage = global::MonitorProj.Properties.Resources.Button1;
                                    powerOpenFlag4 &= ~(0x10);
                                }
                                else if (btnCmdindex == 46)
                                {
                                    btn_InboardBackupPRB_W.BackgroundImage = global::MonitorProj.Properties.Resources.Button1;
                                    powerOpenFlag4 &= ~(0x20);
                                }
                                else if (btnCmdindex == 47)
                                {
                                    btn_InboardBackupPRB_12V1.BackgroundImage = global::MonitorProj.Properties.Resources.Button1;
                                    powerOpenFlag4 &= ~(0x40);
                                }
                                else if (btnCmdindex == 48)
                                {
                                    btn_InboardBackupPRB_12V2.BackgroundImage = global::MonitorProj.Properties.Resources.Button1;
                                    powerOpenFlag4 &= ~(0x80);
                                }


                                //(powerOpenFlag4 & 0x0F) == 0 && btnCmdindex >= 41 && btnCmdindex <= 44)   //62A
                                //(powerOpenFlag3 == 0 && btnCmdindex >= 31 && btnCmdindex <= 34)           //62B
                                //((powerOpenFlag3 == 0 && btnCmdindex >= 31 && btnCmdindex <= 34) && ((powerOpenFlag4 & 0x0F) == 0 && btnCmdindex >= 41 && btnCmdindex <= 44)) 

                                //当每组所有按钮都关闭时，关闭总电源
                                if (((powerOpenFlag1 & 0x0F) == 0 && btnCmdindex >= 1 && btnCmdindex <= 4) ||
                                    ((powerOpenFlag1 & 0xF0) == 0 && btnCmdindex >= 5 && btnCmdindex <= 8) ||
                                    (powerOpenFlag2 == 0 && btnCmdindex >= 11 && btnCmdindex <= 22) ||
                                    ((powerOpenFlag3 == 0 && (powerOpenFlag4 & 0x0F) == 0) && ((btnCmdindex >= 41 && btnCmdindex <= 44) || (btnCmdindex >= 31 && btnCmdindex <= 34))) ||
                                    ((powerOpenFlag4 & 0xF0) == 0 && btnCmdindex >= 45 && btnCmdindex <= 48))
                                {
                                    flagCmdToSend = false;
                                    //if (bChannelReturn != 0x61 && bChannelReturn != 0x61) //0x61的B路电源上电即输出, A路也需要控制;61B常开，61A需要控制
                                    {
                                        dicBoardMonCtlClassType[myCtlPowerSerialCOMType].BCmdInsert(cmdPowerClose);//主控电源关
                                    }
                                    //发送电源关闭后，不论反馈如何，标记为关状态
                                    if (btnCmdindex >= 1 && btnCmdindex <= 4)//01 开电源板0x70，02开电源板0x71，04开电源板0x72，08，开备用继电器
                                    {
                                        CtlPowerFlag79 &= ~(0x01);
                                    }
                                    else if (btnCmdindex >= 5 && btnCmdindex <= 8)
                                    {
                                        CtlPowerFlag79 &= ~(0x02);
                                    }
                                    else if (btnCmdindex >= 11 && btnCmdindex <= 22)
                                    {
                                        CtlPowerFlag61 &= ~(0x01);
                                    }
                                    else if (btnCmdindex >= 31 && btnCmdindex <= 34)
                                    {
                                        CtlPowerFlag62 &= ~(0x10);
                                    }
                                    else if (btnCmdindex >= 41 && btnCmdindex <= 44)    //62A
                                    {
                                        CtlPowerFlag62 &= ~(0x01);
                                    }
                                    else if (btnCmdindex >= 45 && btnCmdindex <= 46)    //63B
                                    {
                                        CtlPowerFlag63 &= ~(0x10);
                                    }
                                    else if (btnCmdindex >= 47 && btnCmdindex <= 48)    //63A
                                    {
                                        CtlPowerFlag63 &= ~(0x01);
                                    }
                                }

                                return null;
                            }));

                            #endregion
                        
                        }
                        #endregion

                        #region 3表示仅仅关闭电源，不关闭板卡
                        else if (typeCmdSendOpen == 3)
                        {

                        }
                        #endregion

                        #region 5表示调整灯的亮度
                        else if (typeCmdSendOpen == 5)
                        {

                            isbCmdReturn = false;

                            #region 发送板卡关闭
                            flagCmdToSend = false;
                            dicBoardMonCtlClassType[myCardSerialCOMType].BCmdInsert(cmdLightDensitySet);

                            Thread.Sleep(sleepInternal);

                            GEventArgs myGEventArgs = new GEventArgs();
                            myGEventArgs.source = this.Name;


                            #region 确定messageID,用于DataGridView中显示报警信息的序号,bChannelReturnCard
                            switch (bChannelReturnCard)
                            {
                                case 0x61:
                                    myGEventArgs.messageID = 1501;
                                    break;
                                case 0x62:
                                    myGEventArgs.messageID = 1502;
                                    break;
                                case 0x63:
                                    myGEventArgs.messageID = 1503;
                                    break;
                                case 0x70:
                                    myGEventArgs.messageID = 1504;
                                    break;
                                case 0x71:
                                    myGEventArgs.messageID = 1505;
                                    break;
                                case 0x72:
                                    myGEventArgs.messageID = 1506;
                                    break;
                                case 0x79:
                                    myGEventArgs.messageID = 1507;
                                    break;
                                case 0x80:
                                    myGEventArgs.messageID = 1508;
                                    break;
                                case 0x90:
                                    myGEventArgs.messageID = 1509;
                                    break;
                                case 0x25:
                                    myGEventArgs.messageID = 1510;
                                    break;
                                case 0x26:
                                    myGEventArgs.messageID = 1511;
                                    break;
                                case 0x28:
                                    myGEventArgs.messageID = 1512;
                                    break;
                                case 0x29:
                                    myGEventArgs.messageID = 1513;
                                    break;
                                case 0x40:
                                    myGEventArgs.messageID = 1514;
                                    break;
                                case 0x21:
                                    myGEventArgs.messageID = 1515;
                                    break;
                                case 0x22:
                                    myGEventArgs.messageID = 1516;
                                    break;
                                case 0x23:
                                    myGEventArgs.messageID = 1517;
                                    break;
                                case 0x24:
                                    myGEventArgs.messageID = 1518;
                                    break;

                                default:
                                    myGEventArgs.messageID = 1500;
                                    break;
                            }
                            #endregion

                            if (isbCmdReturn == false)
                            {
                                myGEventArgs.message = "板卡(0x" + bChannelReturnCard.ToString("X2") + ")没有指令反馈";
                                if (DeviceFormStateEventSend != null)
                                {
                                    DeviceFormStateEventSend(this, myGEventArgs);
                                }
                            }
                            else
                            {
                                if (bCmdReturn[0] == 0xFF && bCmdReturn[1] == 0xFF && bCmdReturn[2] == 0xA5 && bCmdReturn.Length >= 8)
                                {
                                    if (bCmdReturn[3] == bChannelReturnCard && bCmdReturn[4] == 0x82)
                                    {
                                        isCtlRetOK = true;
                                    }
                                    else
                                    {
                                        myGEventArgs.message = "板卡(0x" + bChannelReturnCard.ToString("X2") + ")指令反馈错误";
                                        if (DeviceFormStateEventSend != null)
                                        {
                                            DeviceFormStateEventSend(this, myGEventArgs);
                                        }
                                    }
                                    isbCmdReturn = false;
                                }
                                else
                                {
                                    myGEventArgs.message = "板卡(0x" + bChannelReturnCard.ToString("X2") + ")指令数据反馈错误";
                                    if (DeviceFormStateEventSend != null)
                                    {
                                        DeviceFormStateEventSend(this, myGEventArgs);
                                    }
                                }
                            }

                            #endregion
                        }

                        #endregion


                        #region 6、7表示主控电源的打开、关闭
                        else if (typeCmdSendOpen == 6 || typeCmdSendOpen == 7)
                        {

                            isbCmdReturn = false;

                            #region 发送板卡关闭
                            flagCmdToSend = false;
                            dicBoardMonCtlClassType[myCtlPowerSerialCOMType].BCmdInsert(cmdPowerCtlOnly);

                            Thread.Sleep(sleepInternal);

                            GEventArgs myGEventArgs = new GEventArgs();
                            myGEventArgs.source = this.Name;


                            #region 确定messageID,用于DataGridView中显示报警信息的序号,bChannelReturn
                            switch (bChannelReturn)
                            {
                                case 0x61:
                                    myGEventArgs.messageID = 1601;
                                    break;
                                case 0x62:
                                    myGEventArgs.messageID = 1602;
                                    break;
                                case 0x63:
                                    myGEventArgs.messageID = 1603;
                                    break;
                                case 0x70:
                                    myGEventArgs.messageID = 1604;
                                    break;
                                case 0x71:
                                    myGEventArgs.messageID = 1605;
                                    break;
                                case 0x72:
                                    myGEventArgs.messageID = 1606;
                                    break;
                                case 0x79:
                                    myGEventArgs.messageID = 1607;
                                    break;
                                case 0x80:
                                    myGEventArgs.messageID = 1608;
                                    break;
                                case 0x90:
                                    myGEventArgs.messageID = 1609;
                                    break;
                                case 0x25:
                                    myGEventArgs.messageID = 1610;
                                    break;
                                case 0x26:
                                    myGEventArgs.messageID = 1611;
                                    break;
                                case 0x28:
                                    myGEventArgs.messageID = 1612;
                                    break;
                                case 0x29:
                                    myGEventArgs.messageID = 1613;
                                    break;
                                case 0x40:
                                    myGEventArgs.messageID = 1614;
                                    break;
                                case 0x21:
                                    myGEventArgs.messageID = 1615;
                                    break;
                                case 0x22:
                                    myGEventArgs.messageID = 1616;
                                    break;
                                case 0x23:
                                    myGEventArgs.messageID = 1617;
                                    break;
                                case 0x24:
                                    myGEventArgs.messageID = 1618;
                                    break;

                                default:
                                    myGEventArgs.messageID = 1600;
                                    break;
                            }
                            #endregion

                            if (isbCmdReturn == false)
                            {
                                myGEventArgs.message = "板卡(0x" + bChannelReturn.ToString("X2") + ")没有指令反馈";
                                if (DeviceFormStateEventSend != null)
                                {
                                    DeviceFormStateEventSend(this, myGEventArgs);
                                }


                                #region 板卡短电指令发送、反馈比对结束后，更新相应控制按钮的底色，以区分加断电

                                this.BeginInvoke(new Func<object>(() =>
                                {
                                    if (btnCmdindex == 61)
                                    {
                                        if (typeCmdSendOpen == 6)
                                        {
                                            btn_FaBox_72.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                            CtlPowerFlag79 |= 0x04;
                                        }
                                        else if (typeCmdSendOpen == 7)
                                        {
                                            btn_FaBox_72.BackgroundImage = global::MonitorProj.Properties.Resources.Button1;
                                            CtlPowerFlag79 &= ~(0x04);
                                        }
                                    }
                                    else if (btnCmdindex == 62)
                                    {
                                        if (typeCmdSendOpen == 6)
                                        {
                                            btn_FaBox_Space.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                            CtlPowerFlag79 |= 0x08;
                                        }
                                        else if (typeCmdSendOpen == 7)
                                        {
                                            btn_FaBox_Space.BackgroundImage = global::MonitorProj.Properties.Resources.Button1;
                                            CtlPowerFlag79 &= ~(0x08);
                                        }
                                    }

                                    return null;
                                }));

                                #endregion
                        
                            }
                            else
                            {
                                if (bCmdReturn[0] == 0xFF && bCmdReturn[1] == 0xFF && bCmdReturn[2] == 0xA5 && bCmdReturn.Length >= 8)
                                {
                                    if (bCmdReturn[3] == bChannelReturn && bCmdReturn[4] == 0x82)
                                    {
                                        isCtlRetOK = true;

                                        #region 板卡短电指令发送、反馈比对结束后，更新相应控制按钮的底色，以区分加断电

                                        this.BeginInvoke(new Func<object>(() =>
                                        {
                                            if (btnCmdindex == 61)
                                            {
                                                if (typeCmdSendOpen == 6)
                                                {
                                                    btn_FaBox_72.BackgroundImage = global::MonitorProj.Properties.Resources.Button3;
                                                    CtlPowerFlag79 |= 0x04;
                                                }
                                                else if (typeCmdSendOpen == 7)
                                                {
                                                    btn_FaBox_72.BackgroundImage = global::MonitorProj.Properties.Resources.Button1;
                                                    CtlPowerFlag79 &= ~(0x04);
                                                }
                                            }
                                            else if (btnCmdindex == 62)
                                            {
                                                if (typeCmdSendOpen == 6)
                                                {
                                                    btn_FaBox_Space.BackgroundImage = global::MonitorProj.Properties.Resources.Button3;
                                                    CtlPowerFlag79 |= 0x08;
                                                }
                                                else if (typeCmdSendOpen == 7)
                                                {
                                                    btn_FaBox_Space.BackgroundImage = global::MonitorProj.Properties.Resources.Button1;
                                                    CtlPowerFlag79 &= ~(0x08);
                                                }
                                            }

                                            return null;
                                        }));

                                        #endregion
                        
                                    }
                                    else
                                    {
                                        myGEventArgs.message = "板卡(0x" + bChannelReturn.ToString("X2") + ")指令反馈错误";
                                        if (DeviceFormStateEventSend != null)
                                        {
                                            DeviceFormStateEventSend(this, myGEventArgs);
                                        }

                                        #region 板卡短电指令发送、反馈比对结束后，更新相应控制按钮的底色，以区分加断电

                                        this.BeginInvoke(new Func<object>(() =>
                                        {
                                            if (btnCmdindex == 61)
                                            {
                                                if (typeCmdSendOpen == 6)
                                                {
                                                    btn_FaBox_72.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                                    CtlPowerFlag79 |= 0x04;
                                                }
                                                else if (typeCmdSendOpen == 7)
                                                {
                                                    btn_FaBox_72.BackgroundImage = global::MonitorProj.Properties.Resources.Button1;
                                                    CtlPowerFlag79 &= ~(0x04);
                                                }
                                            }
                                            else if (btnCmdindex == 62)
                                            {
                                                if (typeCmdSendOpen == 6)
                                                {
                                                    btn_FaBox_Space.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                                    CtlPowerFlag79 |= 0x08;
                                                }
                                                else if (typeCmdSendOpen == 7)
                                                {
                                                    btn_FaBox_Space.BackgroundImage = global::MonitorProj.Properties.Resources.Button1;
                                                    CtlPowerFlag79 &= ~(0x08);
                                                }
                                            }

                                            return null;
                                        }));

                                        #endregion
                        
                                    }
                                    isbCmdReturn = false;
                                }
                                else
                                {
                                    myGEventArgs.message = "板卡(0x" + bChannelReturn.ToString("X2") + ")指令数据反馈错误";
                                    if (DeviceFormStateEventSend != null)
                                    {
                                        DeviceFormStateEventSend(this, myGEventArgs);
                                    }

                                    #region 板卡短电指令发送、反馈比对结束后，更新相应控制按钮的底色，以区分加断电

                                    this.BeginInvoke(new Func<object>(() =>
                                    {
                                        if (btnCmdindex == 61)
                                        {
                                            if (typeCmdSendOpen == 6)
                                            {
                                                btn_FaBox_72.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                                CtlPowerFlag79 |= 0x04;
                                            }
                                            else if (typeCmdSendOpen == 7)
                                            {
                                                btn_FaBox_72.BackgroundImage = global::MonitorProj.Properties.Resources.Button1;
                                                CtlPowerFlag79 &= ~(0x04);
                                            }
                                        }
                                        else if (btnCmdindex == 62)
                                        {
                                            if (typeCmdSendOpen == 6)
                                            {
                                                btn_FaBox_Space.BackgroundImage = global::MonitorProj.Properties.Resources.Button4;
                                                CtlPowerFlag79 |= 0x08;
                                            }
                                            else if (typeCmdSendOpen == 7)
                                            {
                                                btn_FaBox_Space.BackgroundImage = global::MonitorProj.Properties.Resources.Button1;
                                                CtlPowerFlag79 &= ~(0x08);
                                            }
                                        }

                                        return null;
                                    }));

                                    #endregion
                        
                                }
                            }


                            #region 板卡短电指令发送、反馈比对结束后，更新相应控制按钮的底色，以区分加断电
                            /*
                            this.BeginInvoke(new Func<object>(() =>
                            {
                                if (btnCmdindex == 61)
                                {
                                    if (typeCmdSendOpen == 6)
                                    {
                                        btn_FaBox_72.BackgroundImage = global::MonitorProj.Properties.Resources.Button3;
                                        CtlPowerFlag79 |= 0x04;
                                    }
                                    else if (typeCmdSendOpen == 7)
                                    {
                                        btn_FaBox_72.BackgroundImage = global::MonitorProj.Properties.Resources.Button1;
                                        CtlPowerFlag79 &= ~(0x04);
                                    }
                                }
                                else if (btnCmdindex == 62)
                                {
                                    if (typeCmdSendOpen == 6)
                                    {
                                        btn_FaBox_Space.BackgroundImage = global::MonitorProj.Properties.Resources.Button3;
                                        CtlPowerFlag79 |= 0x08;
                                    }
                                    else if (typeCmdSendOpen == 7)
                                    {
                                        btn_FaBox_Space.BackgroundImage = global::MonitorProj.Properties.Resources.Button1;
                                        CtlPowerFlag79 &= ~(0x08);
                                    }
                                }

                                return null;
                            }));
                            */
                            #endregion
                        

                            #endregion
                        }

                        #endregion


                        #endregion

                        flagCmdToSend = false;
                        //发命令后，让查询等待时间
                        Thread.Sleep(30);
                        Global.isCtlSending = false;


                    }
                    catch (Exception ee)
                    { }
                }
            }
            catch (Exception ex)
            { 
            }
        }
        #endregion


        //四组控制指令码字，分为总电源上电、板卡上电、板卡下电、总电源下电
        byte[] cmdPowerOpen = new byte[] { 0xFF, 0xFF, 0xA5, 0x62, 0x02, 0x06, 0x00, 0x6E, 0x26 };
        byte[] cmdOpenDevice;
        byte[] cmdPowerClose = new byte[] { 0xFF, 0xFF, 0xA5, 0x62, 0x02, 0x06, 0x00, 0x6E, 0x26 };
        byte[] cmdCloseDevice;


        private void FormMobileDrillMonCtl_VisibleChanged(object sender, EventArgs e)
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


        /**
         * 求和校验
         * 
         */
        public byte CheckCtlDataSum(byte[] cmd, int start, int len)
        {
            byte res = 0x00;
            if (start >= 0 && len > 1 && (start + len <= cmd.Length))
            {
                for (int i = start; i < start + len; i++)
                {
                    int tmp = res + cmd[i];
                    res = BitConverter.GetBytes(tmp)[0];
                }
            }
            return res;
        }

        #region 舱内备用电源继电器板（0x90）

        //通道1-4为备用24V通道，显示“备用24V#1”，“备用24V#2”，“备用24V#3”，“备用24V#4”（第四路为千兆网备用24V电源）等按钮即可。
        //（来自0x62A）
        private void btn_InboardBackupPRB_24V1_Click(object sender, EventArgs e)
        {
            try
            {
                btnCmdindex = 41;

                int cmdToCtlPower = CtlPowerFlag62;
                int cmdToDevice = powerOpenFlag4;

                if (btn_InboardBackupPRB_24V1.BackgroundImage == global::MonitorProj.Properties.Resources.Button1)
                {
                    cmdPowerOpen = new byte[] { 0xFF, 0xFF, 0xA5, 0x62, 0x02, 0x06, 0x00, 0x00, 0x26 };
                    cmdOpenDevice = new byte[] { 0xFF, 0xFF, 0xA5, 0x90, 0x02, 0x07, 0x00, 0x00, 0x00, 0x26 };
                    typeCmdSendOpen = 1;    //1表示打开  

                    //获得供电电源其他bit的状态
                    cmdToCtlPower |= 0x01;//开电源板0x62A
                    byte[] lastState = BitConverter.GetBytes(cmdToCtlPower);
                    cmdPowerOpen[6] = lastState[0];

                    //获得开关其他bit的状态
                    cmdToDevice |= 0x01;
                    lastState = BitConverter.GetBytes(cmdToDevice);
                    cmdOpenDevice[6] = lastState[0];

                    //校验
                    byte check = CheckCtlDataSum(cmdPowerOpen, 3, 4);
                    cmdPowerOpen[7] = check;
                    check = CheckCtlDataSum(cmdOpenDevice, 3, 5);
                    cmdOpenDevice[8] = check;

                    flagCmdToSend = true;

                    string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t备用24V#1打开\t";
                    Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                    Global.myLogStreamWriter.Flush();
                }
                else//关闭
                {
                    cmdPowerClose = new byte[] { 0xFF, 0xFF, 0xA5, 0x62, 0x02, 0x06, 0x00, 0x00, 0x26 };
                    cmdCloseDevice = new byte[] { 0xFF, 0xFF, 0xA5, 0x90, 0x02, 0x07, 0x00, 0x00, 0x00, 0x26 };
                    typeCmdSendOpen = 2;    //2表示关闭

                    //获得供电电源其他bit的状态
                    cmdToCtlPower &= ~(0x01);
                    byte[] lastState = BitConverter.GetBytes(cmdToCtlPower);
                    cmdPowerClose[6] = lastState[0];

                    //获得开关其他bit的状态
                    cmdToDevice &= ~(0x01);
                    lastState = BitConverter.GetBytes(cmdToDevice);
                    cmdCloseDevice[6] = lastState[0];

                    //校验
                    byte check = CheckCtlDataSum(cmdPowerClose, 3, 4);
                    cmdPowerClose[7] = check;
                    check = CheckCtlDataSum(cmdCloseDevice, 3, 5);
                    cmdCloseDevice[8] = check;
                    flagCmdToSend = true;

                    string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t备用24V#1关闭\t";
                    Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                    Global.myLogStreamWriter.Flush();
                }
            }
            catch (Exception ex)
            { }
        }

        private void btn_InboardBackupPRB_24V2_Click(object sender, EventArgs e)
        {
            try
            {
                btnCmdindex = 42;

                int cmdToCtlPower = CtlPowerFlag62;
                int cmdToDevice = powerOpenFlag4;

                if (btn_InboardBackupPRB_24V2.BackgroundImage == global::MonitorProj.Properties.Resources.Button1)
                {
                    cmdPowerOpen = new byte[] { 0xFF, 0xFF, 0xA5, 0x62, 0x02, 0x06, 0x00, 0x00, 0x26 };
                    cmdOpenDevice = new byte[] { 0xFF, 0xFF, 0xA5, 0x90, 0x02, 0x07, 0x00, 0x00, 0x00, 0x26 };
                    typeCmdSendOpen = 1;    //1表示打开  

                    //获得供电电源其他bit的状态
                    cmdToCtlPower |= 0x01;//开电源板0x62A
                    byte[] lastState = BitConverter.GetBytes(cmdToCtlPower);
                    cmdPowerOpen[6] = lastState[0];

                    //获得开关其他bit的状态
                    cmdToDevice |= 0x02;
                    lastState = BitConverter.GetBytes(cmdToDevice);
                    cmdOpenDevice[6] = lastState[0];

                    //校验
                    byte check = CheckCtlDataSum(cmdPowerOpen, 3, 4);
                    cmdPowerOpen[7] = check;
                    check = CheckCtlDataSum(cmdOpenDevice, 3, 5);
                    cmdOpenDevice[8] = check;

                    flagCmdToSend = true;

                    string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t备用24V#2打开\t";
                    Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                    Global.myLogStreamWriter.Flush();
                }
                else//关闭
                {
                    cmdPowerClose = new byte[] { 0xFF, 0xFF, 0xA5, 0x62, 0x02, 0x06, 0x00, 0x00, 0x26 };
                    cmdCloseDevice = new byte[] { 0xFF, 0xFF, 0xA5, 0x90, 0x02, 0x07, 0x00, 0x00, 0x00, 0x26 };
                    typeCmdSendOpen = 2;    //2表示关闭

                    //获得供电电源其他bit的状态
                    cmdToCtlPower &= ~(0x01);
                    byte[] lastState = BitConverter.GetBytes(cmdToCtlPower);
                    cmdPowerClose[6] = lastState[0];

                    //获得开关其他bit的状态
                    cmdToDevice &= ~(0x02);
                    lastState = BitConverter.GetBytes(cmdToDevice);
                    cmdCloseDevice[6] = lastState[0];

                    //校验
                    byte check = CheckCtlDataSum(cmdPowerClose, 3, 4);
                    cmdPowerClose[7] = check;
                    check = CheckCtlDataSum(cmdCloseDevice, 3, 5);
                    cmdCloseDevice[8] = check;
                    flagCmdToSend = true;

                    string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t备用24V#2关闭\t";
                    Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                    Global.myLogStreamWriter.Flush();
                }
            }
            catch (Exception ex)
            { }
        }

        private void btn_InboardBackupPRB_24V3_Click(object sender, EventArgs e)
        {
            try
            {
                btnCmdindex = 43;

                int cmdToCtlPower = CtlPowerFlag62;
                int cmdToDevice = powerOpenFlag4;

                if (btn_InboardBackupPRB_24V3.BackgroundImage == global::MonitorProj.Properties.Resources.Button1)
                {
                    cmdPowerOpen = new byte[] { 0xFF, 0xFF, 0xA5, 0x62, 0x02, 0x06, 0x00, 0x00, 0x26 };
                    cmdOpenDevice = new byte[] { 0xFF, 0xFF, 0xA5, 0x90, 0x02, 0x07, 0x00, 0x00, 0x00, 0x26 };
                    typeCmdSendOpen = 1;    //1表示打开  

                    //获得供电电源其他bit的状态
                    cmdToCtlPower |= 0x01;//开电源板0x62A
                    byte[] lastState = BitConverter.GetBytes(cmdToCtlPower);
                    cmdPowerOpen[6] = lastState[0];

                    //获得开关其他bit的状态
                    cmdToDevice |= 0x04;
                    lastState = BitConverter.GetBytes(cmdToDevice);
                    cmdOpenDevice[6] = lastState[0];

                    //校验
                    byte check = CheckCtlDataSum(cmdPowerOpen, 3, 4);
                    cmdPowerOpen[7] = check;
                    check = CheckCtlDataSum(cmdOpenDevice, 3, 5);
                    cmdOpenDevice[8] = check;

                    flagCmdToSend = true;

                    string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t备用24V#3打开\t";
                    Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                    Global.myLogStreamWriter.Flush();
                }
                else//关闭
                {
                    cmdPowerClose = new byte[] { 0xFF, 0xFF, 0xA5, 0x62, 0x02, 0x06, 0x00, 0x00, 0x26 };
                    cmdCloseDevice = new byte[] { 0xFF, 0xFF, 0xA5, 0x90, 0x02, 0x07, 0x00, 0x00, 0x00, 0x26 };
                    typeCmdSendOpen = 2;    //2表示关闭

                    //获得供电电源其他bit的状态
                    cmdToCtlPower &= ~(0x01);
                    byte[] lastState = BitConverter.GetBytes(cmdToCtlPower);
                    cmdPowerClose[6] = lastState[0];

                    //获得开关其他bit的状态
                    cmdToDevice &= ~(0x04);
                    lastState = BitConverter.GetBytes(cmdToDevice);
                    cmdCloseDevice[6] = lastState[0];

                    //校验
                    byte check = CheckCtlDataSum(cmdPowerClose, 3, 4);
                    cmdPowerClose[7] = check;
                    check = CheckCtlDataSum(cmdCloseDevice, 3, 5);
                    cmdCloseDevice[8] = check;
                    flagCmdToSend = true;

                    string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t备用24V#3关闭\t";
                    Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                    Global.myLogStreamWriter.Flush();
                }
            }
            catch (Exception ex)
            { }
        }

        private void btn_InboardBackupPRB_24V4_Click(object sender, EventArgs e)
        {
            try
            {
                btnCmdindex = 44;

                int cmdToCtlPower = CtlPowerFlag62;
                int cmdToDevice = powerOpenFlag4;

                if (btn_InboardBackupPRB_24V4.BackgroundImage == global::MonitorProj.Properties.Resources.Button1)
                {
                    cmdPowerOpen = new byte[] { 0xFF, 0xFF, 0xA5, 0x62, 0x02, 0x06, 0x00, 0x00, 0x26 };
                    cmdOpenDevice = new byte[] { 0xFF, 0xFF, 0xA5, 0x90, 0x02, 0x07, 0x00, 0x00, 0x00, 0x26 };
                    typeCmdSendOpen = 1;    //1表示打开  

                    //获得供电电源其他bit的状态
                    cmdToCtlPower |= 0x01;//开电源板0x62A
                    byte[] lastState = BitConverter.GetBytes(cmdToCtlPower);
                    cmdPowerOpen[6] = lastState[0];

                    //获得开关其他bit的状态
                    cmdToDevice |= 0x08;
                    lastState = BitConverter.GetBytes(cmdToDevice);
                    cmdOpenDevice[6] = lastState[0];

                    //校验
                    byte check = CheckCtlDataSum(cmdPowerOpen, 3, 4);
                    cmdPowerOpen[7] = check;
                    check = CheckCtlDataSum(cmdOpenDevice, 3, 5);
                    cmdOpenDevice[8] = check;

                    flagCmdToSend = true;

                    string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t备用24V#4打开\t";
                    Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                    Global.myLogStreamWriter.Flush();
                }
                else//关闭
                {
                    cmdPowerClose = new byte[] { 0xFF, 0xFF, 0xA5, 0x62, 0x02, 0x06, 0x00, 0x00, 0x26 };
                    cmdCloseDevice = new byte[] { 0xFF, 0xFF, 0xA5, 0x90, 0x02, 0x07, 0x00, 0x00, 0x00, 0x26 };
                    typeCmdSendOpen = 2;    //2表示关闭

                    //获得供电电源其他bit的状态
                    cmdToCtlPower &= ~(0x01);
                    byte[] lastState = BitConverter.GetBytes(cmdToCtlPower);
                    cmdPowerClose[6] = lastState[0];

                    //获得开关其他bit的状态
                    cmdToDevice &= ~(0x08);
                    lastState = BitConverter.GetBytes(cmdToDevice);
                    cmdCloseDevice[6] = lastState[0];

                    //校验
                    byte check = CheckCtlDataSum(cmdPowerClose, 3, 4);
                    cmdPowerClose[7] = check;
                    check = CheckCtlDataSum(cmdCloseDevice, 3, 5);
                    cmdCloseDevice[8] = check;
                    flagCmdToSend = true;

                    string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t备用24V#4关闭\t";
                    Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                    Global.myLogStreamWriter.Flush();
                }
            }
            catch (Exception ex)
            { }
        }


        private int CtlPowerFlag63 = 0x00;//0x61的B路电源上电即输出,常开
        //其中通道5为高清摄像机供电（来自0x63B），通道6为千兆网络备用电源（来自0x63B）。
        //其中通道7为12V备用通道#1（来自0x63A），通道8为12V备用通道#2（来自0x63A）。
        private void btn_InboardBackupPRB_Camera_Click(object sender, EventArgs e)
        {
            try
            {
                btnCmdindex = 45;

                int cmdToCtlPower = CtlPowerFlag63;
                int cmdToDevice = powerOpenFlag4;

                if (btn_InboardBackupPRB_Camera.BackgroundImage == global::MonitorProj.Properties.Resources.Button1)
                {
                    cmdPowerOpen = new byte[] { 0xFF, 0xFF, 0xA5, 0x63, 0x02, 0x06, 0x00, 0x00, 0x26 };
                    cmdOpenDevice = new byte[] { 0xFF, 0xFF, 0xA5, 0x90, 0x02, 0x07, 0x00, 0x00, 0x00, 0x26 };
                    typeCmdSendOpen = 1;    //1表示打开  

                    //获得供电电源其他bit的状态
                    cmdToCtlPower |= 0x10;//开电源板0x63B
                    byte[] lastState = BitConverter.GetBytes(cmdToCtlPower);
                    cmdPowerOpen[6] = lastState[0];

                    //获得开关其他bit的状态
                    cmdToDevice |= 0x10;
                    lastState = BitConverter.GetBytes(cmdToDevice);
                    cmdOpenDevice[6] = lastState[0];

                    //校验
                    byte check = CheckCtlDataSum(cmdPowerOpen, 3, 4);
                    cmdPowerOpen[7] = check;
                    check = CheckCtlDataSum(cmdOpenDevice, 3, 5);
                    cmdOpenDevice[8] = check;

                    flagCmdToSend = true;

                    string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t高清摄像机打开\t";
                    Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                    Global.myLogStreamWriter.Flush();
                }
                else//关闭
                {
                    cmdPowerClose = new byte[] { 0xFF, 0xFF, 0xA5, 0x62, 0x02, 0x06, 0x00, 0x00, 0x26 };
                    cmdCloseDevice = new byte[] { 0xFF, 0xFF, 0xA5, 0x90, 0x02, 0x07, 0x00, 0x00, 0x00, 0x26 };
                    typeCmdSendOpen = 2;    //2表示关闭

                    //获得供电电源其他bit的状态
                    cmdToCtlPower &= ~(0x10);
                    byte[] lastState = BitConverter.GetBytes(cmdToCtlPower);
                    cmdPowerClose[6] = lastState[0];

                    //获得开关其他bit的状态
                    cmdToDevice &= ~(0x10);
                    lastState = BitConverter.GetBytes(cmdToDevice);
                    cmdCloseDevice[6] = lastState[0];

                    //校验
                    byte check = CheckCtlDataSum(cmdPowerClose, 3, 4);
                    cmdPowerClose[7] = check;
                    check = CheckCtlDataSum(cmdCloseDevice, 3, 5);
                    cmdCloseDevice[8] = check;
                    flagCmdToSend = true;

                    string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t高清摄像机关闭\t";
                    Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                    Global.myLogStreamWriter.Flush();
                }
            }
            catch (Exception ex)
            { }
        }

        private void btn_InboardBackupPRB_W_Click(object sender, EventArgs e)
        {
            try
            {
                btnCmdindex = 46;

                int cmdToCtlPower = CtlPowerFlag63;
                int cmdToDevice = powerOpenFlag4;

                if (btn_InboardBackupPRB_W.BackgroundImage == global::MonitorProj.Properties.Resources.Button1)
                {
                    cmdPowerOpen = new byte[] { 0xFF, 0xFF, 0xA5, 0x63, 0x02, 0x06, 0x00, 0x00, 0x26 };
                    cmdOpenDevice = new byte[] { 0xFF, 0xFF, 0xA5, 0x90, 0x02, 0x07, 0x00, 0x00, 0x00, 0x26 };
                    typeCmdSendOpen = 1;    //1表示打开  

                    //获得供电电源其他bit的状态
                    cmdToCtlPower |= 0x10;//开电源板0x63B
                    byte[] lastState = BitConverter.GetBytes(cmdToCtlPower);
                    cmdPowerOpen[6] = lastState[0];

                    //获得开关其他bit的状态
                    cmdToDevice |= 0x20;
                    lastState = BitConverter.GetBytes(cmdToDevice);
                    cmdOpenDevice[6] = lastState[0];

                    //校验
                    byte check = CheckCtlDataSum(cmdPowerOpen, 3, 4);
                    cmdPowerOpen[7] = check;
                    check = CheckCtlDataSum(cmdOpenDevice, 3, 5);
                    cmdOpenDevice[8] = check;

                    flagCmdToSend = true;

                    string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t千兆网络备用电源打开\t";
                    Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                    Global.myLogStreamWriter.Flush();
                }
                else//关闭
                {
                    cmdPowerClose = new byte[] { 0xFF, 0xFF, 0xA5, 0x62, 0x02, 0x06, 0x00, 0x00, 0x26 };
                    cmdCloseDevice = new byte[] { 0xFF, 0xFF, 0xA5, 0x90, 0x02, 0x07, 0x00, 0x00, 0x00, 0x26 };
                    typeCmdSendOpen = 2;    //2表示关闭

                    //获得供电电源其他bit的状态
                    cmdToCtlPower &= ~(0x10);
                    byte[] lastState = BitConverter.GetBytes(cmdToCtlPower);
                    cmdPowerClose[6] = lastState[0];

                    //获得开关其他bit的状态
                    cmdToDevice &= ~(0x20);
                    lastState = BitConverter.GetBytes(cmdToDevice);
                    cmdCloseDevice[6] = lastState[0];

                    //校验
                    byte check = CheckCtlDataSum(cmdPowerClose, 3, 4);
                    cmdPowerClose[7] = check;
                    check = CheckCtlDataSum(cmdCloseDevice, 3, 5);
                    cmdCloseDevice[8] = check;
                    flagCmdToSend = true;

                    string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t千兆网络备用电源关闭\t";
                    Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                    Global.myLogStreamWriter.Flush();
                }
            }
            catch (Exception ex)
            { }
        }

        private void btn_InboardBackupPRB_12V1_Click(object sender, EventArgs e)
        {
            try
            {
                btnCmdindex = 47;

                int cmdToCtlPower = CtlPowerFlag63;
                int cmdToDevice = powerOpenFlag4;

                if (btn_InboardBackupPRB_12V1.BackgroundImage == global::MonitorProj.Properties.Resources.Button1)
                {
                    cmdPowerOpen = new byte[] { 0xFF, 0xFF, 0xA5, 0x63, 0x02, 0x06, 0x00, 0x00, 0x26 };
                    cmdOpenDevice = new byte[] { 0xFF, 0xFF, 0xA5, 0x90, 0x02, 0x07, 0x00, 0x00, 0x00, 0x26 };
                    typeCmdSendOpen = 1;    //1表示打开  

                    //获得供电电源其他bit的状态
                    cmdToCtlPower |= 0x01;//开电源板0x63A
                    byte[] lastState = BitConverter.GetBytes(cmdToCtlPower);
                    cmdPowerOpen[6] = lastState[0];

                    //获得开关其他bit的状态
                    cmdToDevice |= 0x40;
                    lastState = BitConverter.GetBytes(cmdToDevice);
                    cmdOpenDevice[6] = lastState[0];

                    //校验
                    byte check = CheckCtlDataSum(cmdPowerOpen, 3, 4);
                    cmdPowerOpen[7] = check;
                    check = CheckCtlDataSum(cmdOpenDevice, 3, 5);
                    cmdOpenDevice[8] = check;

                    flagCmdToSend = true;

                    string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t12V备用通道1打开\t";
                    Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                    Global.myLogStreamWriter.Flush();
                }
                else//关闭
                {
                    cmdPowerClose = new byte[] { 0xFF, 0xFF, 0xA5, 0x62, 0x02, 0x06, 0x00, 0x00, 0x26 };
                    cmdCloseDevice = new byte[] { 0xFF, 0xFF, 0xA5, 0x90, 0x02, 0x07, 0x00, 0x00, 0x00, 0x26 };
                    typeCmdSendOpen = 2;    //2表示关闭

                    //获得供电电源其他bit的状态
                    cmdToCtlPower &= ~(0x01);
                    byte[] lastState = BitConverter.GetBytes(cmdToCtlPower);
                    cmdPowerClose[6] = lastState[0];

                    //获得开关其他bit的状态
                    cmdToDevice &= ~(0x40);
                    lastState = BitConverter.GetBytes(cmdToDevice);
                    cmdCloseDevice[6] = lastState[0];

                    //校验
                    byte check = CheckCtlDataSum(cmdPowerClose, 3, 4);
                    cmdPowerClose[7] = check;
                    check = CheckCtlDataSum(cmdCloseDevice, 3, 5);
                    cmdCloseDevice[8] = check;
                    flagCmdToSend = true;

                    string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t12V备用通道1关闭\t";
                    Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                    Global.myLogStreamWriter.Flush();
                }
            }
            catch (Exception ex)
            { }
        }

        private void btn_InboardBackupPRB_12V2_Click(object sender, EventArgs e)
        {
            try
            {
                btnCmdindex = 48;

                int cmdToCtlPower = CtlPowerFlag63;
                int cmdToDevice = powerOpenFlag4;

                if (btn_InboardBackupPRB_12V2.BackgroundImage == global::MonitorProj.Properties.Resources.Button1)
                {
                    cmdPowerOpen = new byte[] { 0xFF, 0xFF, 0xA5, 0x63, 0x02, 0x06, 0x00, 0x00, 0x26 };
                    cmdOpenDevice = new byte[] { 0xFF, 0xFF, 0xA5, 0x90, 0x02, 0x07, 0x00, 0x00, 0x00, 0x26 };
                    typeCmdSendOpen = 1;    //1表示打开  

                    //获得供电电源其他bit的状态
                    cmdToCtlPower |= 0x01;//开电源板0x63A
                    byte[] lastState = BitConverter.GetBytes(cmdToCtlPower);
                    cmdPowerOpen[6] = lastState[0];

                    //获得开关其他bit的状态
                    cmdToDevice |= 0x80;
                    lastState = BitConverter.GetBytes(cmdToDevice);
                    cmdOpenDevice[6] = lastState[0];

                    //校验
                    byte check = CheckCtlDataSum(cmdPowerOpen, 3, 4);
                    cmdPowerOpen[7] = check;
                    check = CheckCtlDataSum(cmdOpenDevice, 3, 5);
                    cmdOpenDevice[8] = check;

                    flagCmdToSend = true;

                    string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t12V备用通道2打开\t";
                    Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                    Global.myLogStreamWriter.Flush();
                }
                else//关闭
                {
                    cmdPowerClose = new byte[] { 0xFF, 0xFF, 0xA5, 0x62, 0x02, 0x06, 0x00, 0x00, 0x26 };
                    cmdCloseDevice = new byte[] { 0xFF, 0xFF, 0xA5, 0x90, 0x02, 0x07, 0x00, 0x00, 0x00, 0x26 };
                    typeCmdSendOpen = 2;    //2表示关闭

                    //获得供电电源其他bit的状态
                    cmdToCtlPower &= ~(0x01);
                    byte[] lastState = BitConverter.GetBytes(cmdToCtlPower);
                    cmdPowerClose[6] = lastState[0];

                    //获得开关其他bit的状态
                    cmdToDevice &= ~(0x80);
                    lastState = BitConverter.GetBytes(cmdToDevice);
                    cmdCloseDevice[6] = lastState[0];

                    //校验
                    byte check = CheckCtlDataSum(cmdPowerClose, 3, 4);
                    cmdPowerClose[7] = check;
                    check = CheckCtlDataSum(cmdCloseDevice, 3, 5);
                    cmdCloseDevice[8] = check;
                    flagCmdToSend = true;

                    string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t12V备用通道2关闭\t";
                    Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                    Global.myLogStreamWriter.Flush();
                }
            }
            catch (Exception ex)
            { }
        }

        #endregion


        #region 接口箱继电器板（0x25、0x26、0x28、0x29）,第二组控制，btnCmdindex为11~18
        //接口箱继电器板（0x25、0x26、0x28、0x29）
        //XX：0-3位分别控制第1-4路继电器。
        //0x25 控制摄像机1-4的电源（0x61A）
        //0x26 控制摄像机5-8的电源（0x61A）
        //0x28 第一路罗盘电源（均来自0x61A）
        //0x28：2-4备用12V电源（输出功率较小）。

        //0x29：24输出（来自0x62B），
        //第一路高度计电源
        //第二路深度计电源
        //第3-4路备用电源

        //0x25 控制摄像机1-4的电源（0x61A）
        private int CtlPowerFlag61 = 0x10;//0x61的B路电源上电即输出,常开
        private void btn_Camera_1_Click(object sender, EventArgs e)
        {
            try
            {
                btnCmdindex = 11;

                int cmdToCtlPower = CtlPowerFlag61;
                int cmdToDevice = powerOpenFlag2;

                if (btn_Camera_1.BackgroundImage == global::MonitorProj.Properties.Resources.Button1)
                {
                    cmdPowerOpen = new byte[] { 0xFF, 0xFF, 0xA5, 0x61, 0x02, 0x06, 0x00, 0x00, 0x26 };
                    cmdOpenDevice = new byte[] { 0xFF, 0xFF, 0xA5, 0x25, 0x02, 0x06, 0x00, 0x00, 0x26 };
                    typeCmdSendOpen = 1;    //1表示打开  

                    //获得供电电源其他bit的状态
                    cmdToCtlPower |= 0x01;//开电源板0x61A
                    byte[] lastState = BitConverter.GetBytes(cmdToCtlPower);
                    cmdPowerOpen[6] = lastState[0];

                    //获得开关其他bit的状态
                    cmdToDevice |= 0x01;
                    cmdToDevice &= 0x0F;//取低4位
                    lastState = BitConverter.GetBytes(cmdToDevice);
                    cmdOpenDevice[6] = lastState[0];

                    //校验
                    byte check = CheckCtlDataSum(cmdPowerOpen, 3, 4);
                    cmdPowerOpen[7] = check;
                    check = CheckCtlDataSum(cmdOpenDevice, 3, 4);
                    cmdOpenDevice[7] = check;

                    flagCmdToSend = true;

                    string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t摄像机1打开\t";
                    Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                    Global.myLogStreamWriter.Flush();
                }
                else//关闭
                {
                    cmdPowerClose = new byte[] { 0xFF, 0xFF, 0xA5, 0x61, 0x02, 0x06, 0x00, 0x00, 0x26 };
                    cmdCloseDevice = new byte[] { 0xFF, 0xFF, 0xA5, 0x25, 0x02, 0x06, 0x00, 0x00, 0x26 };
                    typeCmdSendOpen = 2;    //2表示关闭

                    //获得供电电源其他bit的状态
                    cmdToCtlPower &= ~(0x01);
                    byte[] lastState = BitConverter.GetBytes(cmdToCtlPower);
                    cmdPowerClose[6] = lastState[0];

                    //获得开关其他bit的状态
                    cmdToDevice &= ~(0x01);
                    cmdToDevice &= 0x0F;
                    lastState = BitConverter.GetBytes(cmdToDevice);
                    cmdCloseDevice[6] = lastState[0];

                    //校验
                    byte check = CheckCtlDataSum(cmdPowerClose, 3, 4);
                    cmdPowerClose[7] = check;
                    check = CheckCtlDataSum(cmdCloseDevice, 3, 4);
                    cmdCloseDevice[7] = check;
                    flagCmdToSend = true;

                    string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t摄像机1关闭\t";
                    Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                    Global.myLogStreamWriter.Flush();
                }
            }
            catch (Exception ex)
            { }
        }

        private void btn_Camera_2_Click(object sender, EventArgs e)
        {
            try
            {
                btnCmdindex = 12;

                int cmdToCtlPower = CtlPowerFlag61;
                int cmdToDevice = powerOpenFlag2;

                if (btn_Camera_2.BackgroundImage == global::MonitorProj.Properties.Resources.Button1)
                {
                    cmdPowerOpen = new byte[] { 0xFF, 0xFF, 0xA5, 0x61, 0x02, 0x06, 0x00, 0x00, 0x26 };
                    cmdOpenDevice = new byte[] { 0xFF, 0xFF, 0xA5, 0x25, 0x02, 0x06, 0x00, 0x00, 0x26 };
                    typeCmdSendOpen = 1;    //1表示打开  

                    //获得供电电源其他bit的状态
                    cmdToCtlPower |= 0x01;//开电源板0x61A
                    byte[] lastState = BitConverter.GetBytes(cmdToCtlPower);
                    cmdPowerOpen[6] = lastState[0];

                    //获得开关其他bit的状态
                    cmdToDevice |= 0x02;
                    cmdToDevice &= 0x0F;//取低4位
                    lastState = BitConverter.GetBytes(cmdToDevice);
                    cmdOpenDevice[6] = lastState[0];

                    //校验
                    byte check = CheckCtlDataSum(cmdPowerOpen, 3, 4);
                    cmdPowerOpen[7] = check;
                    check = CheckCtlDataSum(cmdOpenDevice, 3, 4);
                    cmdOpenDevice[7] = check;

                    flagCmdToSend = true;

                    string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t摄像机2打开\t";
                    Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                    Global.myLogStreamWriter.Flush();
                }
                else//关闭
                {
                    cmdPowerClose = new byte[] { 0xFF, 0xFF, 0xA5, 0x61, 0x02, 0x06, 0x00, 0x00, 0x26 };
                    cmdCloseDevice = new byte[] { 0xFF, 0xFF, 0xA5, 0x25, 0x02, 0x06, 0x00, 0x00, 0x26 };
                    typeCmdSendOpen = 2;    //2表示关闭

                    //获得供电电源其他bit的状态
                    cmdToCtlPower &= ~(0x01);
                    byte[] lastState = BitConverter.GetBytes(cmdToCtlPower);
                    cmdPowerClose[6] = lastState[0];

                    //获得开关其他bit的状态
                    cmdToDevice &= ~(0x02);
                    cmdToDevice &= 0x0F;
                    lastState = BitConverter.GetBytes(cmdToDevice);
                    cmdCloseDevice[6] = lastState[0];

                    //校验
                    byte check = CheckCtlDataSum(cmdPowerClose, 3, 4);
                    cmdPowerClose[7] = check;
                    check = CheckCtlDataSum(cmdCloseDevice, 3, 4);
                    cmdCloseDevice[7] = check;
                    flagCmdToSend = true;

                    string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t摄像机2关闭\t";
                    Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                    Global.myLogStreamWriter.Flush();
                }
            }
            catch (Exception ex)
            { }
        }

        private void btn_Camera_3_Click(object sender, EventArgs e)
        {
            try
            {
                btnCmdindex = 13;

                int cmdToCtlPower = CtlPowerFlag61;
                int cmdToDevice = powerOpenFlag2;

                if (btn_Camera_3.BackgroundImage == global::MonitorProj.Properties.Resources.Button1)
                {
                    cmdPowerOpen = new byte[] { 0xFF, 0xFF, 0xA5, 0x61, 0x02, 0x06, 0x00, 0x00, 0x26 };
                    cmdOpenDevice = new byte[] { 0xFF, 0xFF, 0xA5, 0x25, 0x02, 0x06, 0x00, 0x00, 0x26 };
                    typeCmdSendOpen = 1;    //1表示打开  

                    //获得供电电源其他bit的状态
                    cmdToCtlPower |= 0x01;//开电源板0x61A
                    byte[] lastState = BitConverter.GetBytes(cmdToCtlPower);
                    cmdPowerOpen[6] = lastState[0];

                    //获得开关其他bit的状态
                    cmdToDevice |= 0x04;
                    cmdToDevice &= 0x0F;//取低4位
                    lastState = BitConverter.GetBytes(cmdToDevice);
                    cmdOpenDevice[6] = lastState[0];

                    //校验
                    byte check = CheckCtlDataSum(cmdPowerOpen, 3, 4);
                    cmdPowerOpen[7] = check;
                    check = CheckCtlDataSum(cmdOpenDevice, 3, 4);
                    cmdOpenDevice[7] = check;

                    flagCmdToSend = true;

                    string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t摄像机3打开\t";
                    Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                    Global.myLogStreamWriter.Flush();
                }
                else//关闭
                {
                    cmdPowerClose = new byte[] { 0xFF, 0xFF, 0xA5, 0x61, 0x02, 0x06, 0x00, 0x00, 0x26 };
                    cmdCloseDevice = new byte[] { 0xFF, 0xFF, 0xA5, 0x25, 0x02, 0x06, 0x00, 0x00, 0x26 };
                    typeCmdSendOpen = 2;    //2表示关闭

                    //获得供电电源其他bit的状态
                    cmdToCtlPower &= ~(0x01);
                    byte[] lastState = BitConverter.GetBytes(cmdToCtlPower);
                    cmdPowerClose[6] = lastState[0];

                    //获得开关其他bit的状态
                    cmdToDevice &= ~(0x04);
                    cmdToDevice &= 0x0F;
                    lastState = BitConverter.GetBytes(cmdToDevice);
                    cmdCloseDevice[6] = lastState[0];

                    //校验
                    byte check = CheckCtlDataSum(cmdPowerClose, 3, 4);
                    cmdPowerClose[7] = check;
                    check = CheckCtlDataSum(cmdCloseDevice, 3, 4);
                    cmdCloseDevice[7] = check;
                    flagCmdToSend = true;

                    string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t摄像机3关闭\t";
                    Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                    Global.myLogStreamWriter.Flush();
                }
            }
            catch (Exception ex)
            { }
        }

        private void btn_Camera_4_Click(object sender, EventArgs e)
        {
            try
            {
                btnCmdindex = 14;

                int cmdToCtlPower = CtlPowerFlag61;
                int cmdToDevice = powerOpenFlag2;

                if (btn_Camera_4.BackgroundImage == global::MonitorProj.Properties.Resources.Button1)
                {
                    cmdPowerOpen = new byte[] { 0xFF, 0xFF, 0xA5, 0x61, 0x02, 0x06, 0x00, 0x00, 0x26 };
                    cmdOpenDevice = new byte[] { 0xFF, 0xFF, 0xA5, 0x25, 0x02, 0x06, 0x00, 0x00, 0x26 };
                    typeCmdSendOpen = 1;    //1表示打开  

                    //获得供电电源其他bit的状态
                    cmdToCtlPower |= 0x01;//开电源板0x61A
                    byte[] lastState = BitConverter.GetBytes(cmdToCtlPower);
                    cmdPowerOpen[6] = lastState[0];

                    //获得开关其他bit的状态
                    cmdToDevice |= 0x08;
                    cmdToDevice &= 0x0F;//取低4位
                    lastState = BitConverter.GetBytes(cmdToDevice);
                    cmdOpenDevice[6] = lastState[0];

                    //校验
                    byte check = CheckCtlDataSum(cmdPowerOpen, 3, 4);
                    cmdPowerOpen[7] = check;
                    check = CheckCtlDataSum(cmdOpenDevice, 3, 4);
                    cmdOpenDevice[7] = check;

                    flagCmdToSend = true;

                    string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t摄像机4打开\t";
                    Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                    Global.myLogStreamWriter.Flush();
                }
                else//关闭
                {
                    cmdPowerClose = new byte[] { 0xFF, 0xFF, 0xA5, 0x61, 0x02, 0x06, 0x00, 0x00, 0x26 };
                    cmdCloseDevice = new byte[] { 0xFF, 0xFF, 0xA5, 0x25, 0x02, 0x06, 0x00, 0x00, 0x26 };
                    typeCmdSendOpen = 2;    //2表示关闭

                    //获得供电电源其他bit的状态
                    cmdToCtlPower &= ~(0x01);
                    byte[] lastState = BitConverter.GetBytes(cmdToCtlPower);
                    cmdPowerClose[6] = lastState[0];

                    //获得开关其他bit的状态
                    cmdToDevice &= ~(0x08);
                    cmdToDevice &= 0x0F;
                    lastState = BitConverter.GetBytes(cmdToDevice);
                    cmdCloseDevice[6] = lastState[0];

                    //校验
                    byte check = CheckCtlDataSum(cmdPowerClose, 3, 4);
                    cmdPowerClose[7] = check;
                    check = CheckCtlDataSum(cmdCloseDevice, 3, 4);
                    cmdCloseDevice[7] = check;
                    flagCmdToSend = true;

                    string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t摄像机4关闭\t";
                    Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                    Global.myLogStreamWriter.Flush();
                }
            }
            catch (Exception ex)
            { }
        }

        //0x26 控制摄像机5-8的电源（0x61A）
        private void btn_Camera_5_Click(object sender, EventArgs e)
        {
            try
            {
                btnCmdindex = 15;

                int cmdToCtlPower = CtlPowerFlag61;
                int cmdToDevice = powerOpenFlag2;

                if (btn_Camera_5.BackgroundImage == global::MonitorProj.Properties.Resources.Button1)
                {
                    cmdPowerOpen = new byte[] { 0xFF, 0xFF, 0xA5, 0x61, 0x02, 0x06, 0x00, 0x00, 0x26 };
                    cmdOpenDevice = new byte[] { 0xFF, 0xFF, 0xA5, 0x26, 0x02, 0x06, 0x00, 0x00, 0x26 };
                    typeCmdSendOpen = 1;    //1表示打开  

                    //获得供电电源其他bit的状态
                    cmdToCtlPower |= 0x01;//开电源板0x61A
                    byte[] lastState = BitConverter.GetBytes(cmdToCtlPower);
                    cmdPowerOpen[6] = lastState[0];

                    //获得开关其他bit的状态
                    cmdToDevice |= 0x10;
                    cmdToDevice &= 0xF0;
                    cmdToDevice = (cmdToDevice >> 4);
                    lastState = BitConverter.GetBytes(cmdToDevice);
                    cmdOpenDevice[6] = lastState[0];

                    //校验
                    byte check = CheckCtlDataSum(cmdPowerOpen, 3, 4);
                    cmdPowerOpen[7] = check;
                    check = CheckCtlDataSum(cmdOpenDevice, 3, 4);
                    cmdOpenDevice[7] = check;

                    flagCmdToSend = true;

                    string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t摄像机5打开\t";
                    Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                    Global.myLogStreamWriter.Flush();
                }
                else//关闭
                {
                    cmdPowerClose = new byte[] { 0xFF, 0xFF, 0xA5, 0x61, 0x02, 0x06, 0x00, 0x00, 0x26 };
                    cmdCloseDevice = new byte[] { 0xFF, 0xFF, 0xA5, 0x26, 0x02, 0x06, 0x00, 0x00, 0x26 };
                    typeCmdSendOpen = 2;    //2表示关闭

                    //获得供电电源其他bit的状态
                    cmdToCtlPower &= ~(0x01);
                    byte[] lastState = BitConverter.GetBytes(cmdToCtlPower);
                    cmdPowerClose[6] = lastState[0];

                    //获得开关其他bit的状态
                    cmdToDevice &= ~(0x10);
                    cmdToDevice &= 0xF0;
                    cmdToDevice = (cmdToDevice >> 4);
                    lastState = BitConverter.GetBytes(cmdToDevice);
                    cmdCloseDevice[6] = lastState[0];

                    //校验
                    byte check = CheckCtlDataSum(cmdPowerClose, 3, 4);
                    cmdPowerClose[7] = check;
                    check = CheckCtlDataSum(cmdCloseDevice, 3, 4);
                    cmdCloseDevice[7] = check;
                    flagCmdToSend = true;

                    string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t摄像机5关闭\t";
                    Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                    Global.myLogStreamWriter.Flush();
                }
            }
            catch (Exception ex)
            { }
        }

        private void btn_Camera_6_Click(object sender, EventArgs e)
        {
            try
            {
                btnCmdindex = 16;

                int cmdToCtlPower = CtlPowerFlag61;
                int cmdToDevice = powerOpenFlag2;

                if (btn_Camera_6.BackgroundImage == global::MonitorProj.Properties.Resources.Button1)
                {
                    cmdPowerOpen = new byte[] { 0xFF, 0xFF, 0xA5, 0x61, 0x02, 0x06, 0x00, 0x00, 0x26 };
                    cmdOpenDevice = new byte[] { 0xFF, 0xFF, 0xA5, 0x26, 0x02, 0x06, 0x00, 0x00, 0x26 };
                    typeCmdSendOpen = 1;    //1表示打开  

                    //获得供电电源其他bit的状态
                    cmdToCtlPower |= 0x01;//开电源板0x61A
                    byte[] lastState = BitConverter.GetBytes(cmdToCtlPower);
                    cmdPowerOpen[6] = lastState[0];

                    //获得开关其他bit的状态
                    cmdToDevice |= 0x20;
                    cmdToDevice &= 0xF0;
                    cmdToDevice = (cmdToDevice >> 4);
                    lastState = BitConverter.GetBytes(cmdToDevice);
                    cmdOpenDevice[6] = lastState[0];

                    //校验
                    byte check = CheckCtlDataSum(cmdPowerOpen, 3, 4);
                    cmdPowerOpen[7] = check;
                    check = CheckCtlDataSum(cmdOpenDevice, 3, 4);
                    cmdOpenDevice[7] = check;

                    flagCmdToSend = true;

                    string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t摄像机6打开\t";
                    Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                    Global.myLogStreamWriter.Flush();
                }
                else//关闭
                {
                    cmdPowerClose = new byte[] { 0xFF, 0xFF, 0xA5, 0x61, 0x02, 0x06, 0x00, 0x00, 0x26 };
                    cmdCloseDevice = new byte[] { 0xFF, 0xFF, 0xA5, 0x26, 0x02, 0x06, 0x00, 0x00, 0x26 };
                    typeCmdSendOpen = 2;    //2表示关闭

                    //获得供电电源其他bit的状态
                    cmdToCtlPower &= ~(0x01);
                    byte[] lastState = BitConverter.GetBytes(cmdToCtlPower);
                    cmdPowerClose[6] = lastState[0];

                    //获得开关其他bit的状态
                    cmdToDevice &= ~(0x20);
                    cmdToDevice &= 0xF0;
                    cmdToDevice = (cmdToDevice >> 4);
                    lastState = BitConverter.GetBytes(cmdToDevice);
                    cmdCloseDevice[6] = lastState[0];

                    //校验
                    byte check = CheckCtlDataSum(cmdPowerClose, 3, 4);
                    cmdPowerClose[7] = check;
                    check = CheckCtlDataSum(cmdCloseDevice, 3, 4);
                    cmdCloseDevice[7] = check;
                    flagCmdToSend = true;

                    string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t摄像机6关闭\t";
                    Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                    Global.myLogStreamWriter.Flush();
                }
            }
            catch (Exception ex)
            { }
        }

        private void btn_Camera_7_Click(object sender, EventArgs e)
        {
            try
            {
                btnCmdindex = 17;

                int cmdToCtlPower = CtlPowerFlag61;
                int cmdToDevice = powerOpenFlag2;

                if (btn_Camera_7.BackgroundImage == global::MonitorProj.Properties.Resources.Button1)
                {
                    cmdPowerOpen = new byte[] { 0xFF, 0xFF, 0xA5, 0x61, 0x02, 0x06, 0x00, 0x00, 0x26 };
                    cmdOpenDevice = new byte[] { 0xFF, 0xFF, 0xA5, 0x26, 0x02, 0x06, 0x00, 0x00, 0x26 };
                    typeCmdSendOpen = 1;    //1表示打开  

                    //获得供电电源其他bit的状态
                    cmdToCtlPower |= 0x01;//开电源板0x61A
                    byte[] lastState = BitConverter.GetBytes(cmdToCtlPower);
                    cmdPowerOpen[6] = lastState[0];

                    //获得开关其他bit的状态
                    cmdToDevice |= 0x40;
                    cmdToDevice &= 0xF0;
                    cmdToDevice = (cmdToDevice >> 4);
                    lastState = BitConverter.GetBytes(cmdToDevice);
                    cmdOpenDevice[6] = lastState[0];

                    //校验
                    byte check = CheckCtlDataSum(cmdPowerOpen, 3, 4);
                    cmdPowerOpen[7] = check;
                    check = CheckCtlDataSum(cmdOpenDevice, 3, 4);
                    cmdOpenDevice[7] = check;

                    flagCmdToSend = true;

                    string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t摄像机7打开\t";
                    Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                    Global.myLogStreamWriter.Flush();
                }
                else//关闭
                {
                    cmdPowerClose = new byte[] { 0xFF, 0xFF, 0xA5, 0x61, 0x02, 0x06, 0x00, 0x00, 0x26 };
                    cmdCloseDevice = new byte[] { 0xFF, 0xFF, 0xA5, 0x26, 0x02, 0x06, 0x00, 0x00, 0x26 };
                    typeCmdSendOpen = 2;    //2表示关闭

                    //获得供电电源其他bit的状态
                    cmdToCtlPower &= ~(0x01);
                    byte[] lastState = BitConverter.GetBytes(cmdToCtlPower);
                    cmdPowerClose[6] = lastState[0];

                    //获得开关其他bit的状态
                    cmdToDevice &= ~(0x40);
                    cmdToDevice &= 0xF0;
                    cmdToDevice = (cmdToDevice >> 4);
                    lastState = BitConverter.GetBytes(cmdToDevice);
                    cmdCloseDevice[6] = lastState[0];

                    //校验
                    byte check = CheckCtlDataSum(cmdPowerClose, 3, 4);
                    cmdPowerClose[7] = check;
                    check = CheckCtlDataSum(cmdCloseDevice, 3, 4);
                    cmdCloseDevice[7] = check;
                    flagCmdToSend = true;

                    string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t摄像机7关闭\t";
                    Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                    Global.myLogStreamWriter.Flush();
                }
            }
            catch (Exception ex)
            { }
        }

        private void btn_Camera_8_Click(object sender, EventArgs e)
        {
            try
            {
                btnCmdindex = 18;

                int cmdToCtlPower = CtlPowerFlag61;
                int cmdToDevice = powerOpenFlag2;

                if (btn_Camera_8.BackgroundImage == global::MonitorProj.Properties.Resources.Button1)
                {
                    cmdPowerOpen = new byte[] { 0xFF, 0xFF, 0xA5, 0x61, 0x02, 0x06, 0x00, 0x00, 0x26 };
                    cmdOpenDevice = new byte[] { 0xFF, 0xFF, 0xA5, 0x26, 0x02, 0x06, 0x00, 0x00, 0x26 };
                    typeCmdSendOpen = 1;    //1表示打开  

                    //获得供电电源其他bit的状态
                    cmdToCtlPower |= 0x01;//开电源板0x61A
                    byte[] lastState = BitConverter.GetBytes(cmdToCtlPower);
                    cmdPowerOpen[6] = lastState[0];

                    //获得开关其他bit的状态
                    cmdToDevice |= 0x80;
                    cmdToDevice &= 0xF0;
                    cmdToDevice = (cmdToDevice >> 4);
                    lastState = BitConverter.GetBytes(cmdToDevice);
                    cmdOpenDevice[6] = lastState[0];

                    //校验
                    byte check = CheckCtlDataSum(cmdPowerOpen, 3, 4);
                    cmdPowerOpen[7] = check;
                    check = CheckCtlDataSum(cmdOpenDevice, 3, 4);
                    cmdOpenDevice[7] = check;

                    flagCmdToSend = true;

                    string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t摄像机8打开\t";
                    Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                    Global.myLogStreamWriter.Flush();
                }
                else//关闭
                {
                    cmdPowerClose = new byte[] { 0xFF, 0xFF, 0xA5, 0x61, 0x02, 0x06, 0x00, 0x00, 0x26 };
                    cmdCloseDevice = new byte[] { 0xFF, 0xFF, 0xA5, 0x26, 0x02, 0x06, 0x00, 0x00, 0x26 };
                    typeCmdSendOpen = 2;    //2表示关闭

                    //获得供电电源其他bit的状态
                    cmdToCtlPower &= ~(0x01);
                    byte[] lastState = BitConverter.GetBytes(cmdToCtlPower);
                    cmdPowerClose[6] = lastState[0];

                    //获得开关其他bit的状态
                    cmdToDevice &= ~(0x80);
                    cmdToDevice &= 0xF0;
                    cmdToDevice = (cmdToDevice >> 4);
                    lastState = BitConverter.GetBytes(cmdToDevice);
                    cmdCloseDevice[6] = lastState[0];

                    //校验
                    byte check = CheckCtlDataSum(cmdPowerClose, 3, 4);
                    cmdPowerClose[7] = check;
                    check = CheckCtlDataSum(cmdCloseDevice, 3, 4);
                    cmdCloseDevice[7] = check;
                    flagCmdToSend = true;

                    string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t摄像机8关闭\t";
                    Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                    Global.myLogStreamWriter.Flush();
                }
            }
            catch (Exception ex)
            { }
        }

        #endregion


        #region 灯舱继电器板（0x21、0x22）,第一组控制，btnCmdindex为01~08
        //指令和数据同"接口箱继电器板"
        //0x21控制1-4路灯(电源来自0x70)；
        //0x22控制5-8路灯(电源来自0x71)

        //控制指令：大功率直流绝缘检测板控制板数据解析：（0x79）
        //CC，01 开电源板0x70，02开电源板0x71，04开电源板0x72，08，开备用继电器。

        //1-4号灯电源来自0x70绝缘检测板，要开灯需先开第一个继电器；
        //5-8号灯电源来自0x71绝缘检测板，要开灯需要先开第二个继电器；
        //两个阀箱电源来自0x72绝缘检测板，打开阀箱电源需打开第三个继电器。
        //一个备用继电器，界面按钮显示“备用通道”即可。

        private int CtlPowerFlag79 = 0x00;//记录大功率直流绝缘检测板控制板的电源开关状态
        private void btn_Light_1_Click(object sender, EventArgs e)
        {
            try
            {
                btnCmdindex = 1;

                int cmdToCtlPower = CtlPowerFlag79;
                int cmdToDevice = powerOpenFlag1;

                if (btn_Light_1.BackgroundImage == global::MonitorProj.Properties.Resources.Button1)
                {
                    cmdPowerOpen = new byte[] { 0xFF, 0xFF, 0xA5, 0x79, 0x02, 0x06, 0x00, 0x6E, 0x26 };
                    cmdOpenDevice = new byte[] { 0xFF, 0xFF, 0xA5, 0x21, 0x02, 0x06, 0x00, 0x36, 0x26 };
                    typeCmdSendOpen = 1;    //1表示打开  

                    //获得供电电源其他bit的状态
                    cmdToCtlPower |= 0x01;//开电源板0x70
                    byte[] lastState = BitConverter.GetBytes(cmdToCtlPower);
                    cmdPowerOpen[6] = lastState[0];

                    //获得灯开关其他bit的状态
                    cmdToDevice |= 0x01;
                    cmdToDevice &= 0x0F;//取低4位
                    lastState = BitConverter.GetBytes(cmdToDevice);
                    cmdOpenDevice[6] = lastState[0];

                    //校验
                    byte check = CheckCtlDataSum(cmdPowerOpen, 3, 4);
                    cmdPowerOpen[7] = check;
                    check = CheckCtlDataSum(cmdOpenDevice, 3, 4);
                    cmdOpenDevice[7] = check;

                    flagCmdToSend = true;

                    string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t灯1打开\t";
                    Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                    Global.myLogStreamWriter.Flush();
                }
                else//关闭
                {
                    cmdPowerClose = new byte[] { 0xFF, 0xFF, 0xA5, 0x79, 0x02, 0x06, 0x00, 0x6E, 0x26 };
                    cmdCloseDevice = new byte[] { 0xFF, 0xFF, 0xA5, 0x21, 0x02, 0x06, 0x00, 0x36, 0x26 };
                    typeCmdSendOpen = 2;    //2表示关闭

                    //获得供电电源其他bit的状态
                    cmdToCtlPower &= ~(0x01);
                    byte[] lastState = BitConverter.GetBytes(cmdToCtlPower);
                    cmdPowerClose[6] = lastState[0];

                    //获得灯开关其他bit的状态
                    cmdToDevice &= ~(0x01);
                    cmdToDevice &= 0x0F;
                    lastState = BitConverter.GetBytes(cmdToDevice);
                    cmdCloseDevice[6] = lastState[0];
                    
                    //校验
                    byte check = CheckCtlDataSum(cmdPowerClose, 3, 4);
                    cmdPowerClose[7] = check;
                    check = CheckCtlDataSum(cmdCloseDevice, 3, 4);
                    cmdCloseDevice[7] = check;
                    flagCmdToSend = true;

                    string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t灯1关闭\t";
                    Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                    Global.myLogStreamWriter.Flush();
                }
            }
            catch (Exception ex)
            { }
        }

        private void btn_Light_2_Click(object sender, EventArgs e)
        {
            try
            {
                btnCmdindex = 2;

                int cmdToCtlPower = CtlPowerFlag79;
                int cmdToDevice = powerOpenFlag1;

                if (btn_Light_2.BackgroundImage == global::MonitorProj.Properties.Resources.Button1)
                {
                    cmdPowerOpen = new byte[] { 0xFF, 0xFF, 0xA5, 0x79, 0x02, 0x06, 0x00, 0x6E, 0x26 };
                    cmdOpenDevice = new byte[] { 0xFF, 0xFF, 0xA5, 0x21, 0x02, 0x06, 0x00, 0x36, 0x26 };
                    typeCmdSendOpen = 1;    //1表示打开  

                    //获得供电电源其他bit的状态
                    cmdToCtlPower |= 0x01;//开电源板0x70
                    byte[] lastState = BitConverter.GetBytes(cmdToCtlPower);
                    cmdPowerOpen[6] = lastState[0];

                    //获得灯开关其他bit的状态
                    cmdToDevice |= 0x02;
                    cmdToDevice &= 0x0F;
                    lastState = BitConverter.GetBytes(cmdToDevice);
                    cmdOpenDevice[6] = lastState[0];

                    //校验
                    byte check = CheckCtlDataSum(cmdPowerOpen, 3, 4);
                    cmdPowerOpen[7] = check;
                    check = CheckCtlDataSum(cmdOpenDevice, 3, 4);
                    cmdOpenDevice[7] = check;

                    flagCmdToSend = true;

                    string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t灯2打开\t";
                    Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                    Global.myLogStreamWriter.Flush();
                }
                else//关闭
                {
                    cmdPowerClose = new byte[] { 0xFF, 0xFF, 0xA5, 0x79, 0x02, 0x06, 0x00, 0x6E, 0x26 };
                    cmdCloseDevice = new byte[] { 0xFF, 0xFF, 0xA5, 0x21, 0x02, 0x06, 0x00, 0x36, 0x26 };
                    typeCmdSendOpen = 2;    //2表示关闭

                    //获得供电电源其他bit的状态
                    cmdToCtlPower &= ~(0x01);
                    byte[] lastState = BitConverter.GetBytes(cmdToCtlPower);
                    cmdPowerClose[6] = lastState[0];

                    //获得灯开关其他bit的状态
                    cmdToDevice &= ~(0x02);
                    cmdToDevice &= 0x0F;
                    lastState = BitConverter.GetBytes(cmdToDevice);
                    cmdCloseDevice[6] = lastState[0];

                    //校验
                    byte check = CheckCtlDataSum(cmdPowerClose, 3, 4);
                    cmdPowerClose[7] = check;
                    check = CheckCtlDataSum(cmdCloseDevice, 3, 4);
                    cmdCloseDevice[7] = check;
                    flagCmdToSend = true;

                    string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t灯2关闭\t";
                    Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                    Global.myLogStreamWriter.Flush();
                }
            }
            catch (Exception ex)
            { }
        }

        private void btn_Light_3_Click(object sender, EventArgs e)
        {
            try
            {
                btnCmdindex = 3;

                int cmdToCtlPower = CtlPowerFlag79;
                int cmdToDevice = powerOpenFlag1;

                if (btn_Light_3.BackgroundImage == global::MonitorProj.Properties.Resources.Button1)
                {
                    cmdPowerOpen = new byte[] { 0xFF, 0xFF, 0xA5, 0x79, 0x02, 0x06, 0x00, 0x6E, 0x26 };
                    cmdOpenDevice = new byte[] { 0xFF, 0xFF, 0xA5, 0x21, 0x02, 0x06, 0x00, 0x36, 0x26 };
                    typeCmdSendOpen = 1;    //1表示打开  

                    //获得供电电源其他bit的状态
                    cmdToCtlPower |= 0x01;//开电源板0x70
                    byte[] lastState = BitConverter.GetBytes(cmdToCtlPower);
                    cmdPowerOpen[6] = lastState[0];

                    //获得灯开关其他bit的状态
                    cmdToDevice |= 0x04;
                    cmdToDevice &= 0x0F;
                    lastState = BitConverter.GetBytes(cmdToDevice);
                    cmdOpenDevice[6] = lastState[0];

                    //校验
                    byte check = CheckCtlDataSum(cmdPowerOpen, 3, 4);
                    cmdPowerOpen[7] = check;
                    check = CheckCtlDataSum(cmdOpenDevice, 3, 4);
                    cmdOpenDevice[7] = check;

                    flagCmdToSend = true;

                    string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t灯3打开\t";
                    Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                    Global.myLogStreamWriter.Flush();
                }
                else//关闭
                {
                    cmdPowerClose = new byte[] { 0xFF, 0xFF, 0xA5, 0x79, 0x02, 0x06, 0x00, 0x6E, 0x26 };
                    cmdCloseDevice = new byte[] { 0xFF, 0xFF, 0xA5, 0x21, 0x02, 0x06, 0x00, 0x36, 0x26 };
                    typeCmdSendOpen = 2;    //2表示关闭

                    //获得供电电源其他bit的状态
                    cmdToCtlPower &= ~(0x01);
                    byte[] lastState = BitConverter.GetBytes(cmdToCtlPower);
                    cmdPowerClose[6] = lastState[0];

                    //获得灯开关其他bit的状态
                    cmdToDevice &= ~(0x04);
                    cmdToDevice &= 0x0F;
                    lastState = BitConverter.GetBytes(cmdToDevice);
                    cmdCloseDevice[6] = lastState[0];

                    //校验
                    byte check = CheckCtlDataSum(cmdPowerClose, 3, 4);
                    cmdPowerClose[7] = check;
                    check = CheckCtlDataSum(cmdCloseDevice, 3, 4);
                    cmdCloseDevice[7] = check;
                    flagCmdToSend = true;

                    string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t灯3关闭\t";
                    Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                    Global.myLogStreamWriter.Flush();
                }
            }
            catch (Exception ex)
            { }
        }

        private void btn_Light_4_Click(object sender, EventArgs e)
        {
            try
            {
                btnCmdindex = 4;

                int cmdToCtlPower = CtlPowerFlag79;
                int cmdToDevice = powerOpenFlag1;

                if (btn_Light_4.BackgroundImage == global::MonitorProj.Properties.Resources.Button1)
                {
                    cmdPowerOpen = new byte[] { 0xFF, 0xFF, 0xA5, 0x79, 0x02, 0x06, 0x00, 0x6E, 0x26 };
                    cmdOpenDevice = new byte[] { 0xFF, 0xFF, 0xA5, 0x21, 0x02, 0x06, 0x00, 0x36, 0x26 };
                    typeCmdSendOpen = 1;    //1表示打开  

                    //获得供电电源其他bit的状态
                    cmdToCtlPower |= 0x01;//开电源板0x70
                    byte[] lastState = BitConverter.GetBytes(cmdToCtlPower);
                    cmdPowerOpen[6] = lastState[0];

                    //获得灯开关其他bit的状态
                    cmdToDevice |= 0x08;
                    cmdToDevice &= 0x0F;
                    lastState = BitConverter.GetBytes(cmdToDevice);
                    cmdOpenDevice[6] = lastState[0];

                    //校验
                    byte check = CheckCtlDataSum(cmdPowerOpen, 3, 4);
                    cmdPowerOpen[7] = check;
                    check = CheckCtlDataSum(cmdOpenDevice, 3, 4);
                    cmdOpenDevice[7] = check;

                    flagCmdToSend = true;

                    string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t灯4打开\t";
                    Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                    Global.myLogStreamWriter.Flush();
                }
                else//关闭
                {
                    cmdPowerClose = new byte[] { 0xFF, 0xFF, 0xA5, 0x79, 0x02, 0x06, 0x00, 0x6E, 0x26 };
                    cmdCloseDevice = new byte[] { 0xFF, 0xFF, 0xA5, 0x21, 0x02, 0x06, 0x00, 0x36, 0x26 };
                    typeCmdSendOpen = 2;    //2表示关闭

                    //获得供电电源其他bit的状态
                    cmdToCtlPower &= ~(0x01);
                    byte[] lastState = BitConverter.GetBytes(cmdToCtlPower);
                    cmdPowerClose[6] = lastState[0];

                    //获得灯开关其他bit的状态
                    cmdToDevice &= ~(0x08);
                    cmdToDevice &= 0x0F;
                    lastState = BitConverter.GetBytes(cmdToDevice);
                    cmdCloseDevice[6] = lastState[0];

                    //校验
                    byte check = CheckCtlDataSum(cmdPowerClose, 3, 4);
                    cmdPowerClose[7] = check;
                    check = CheckCtlDataSum(cmdCloseDevice, 3, 4);
                    cmdCloseDevice[7] = check;
                    flagCmdToSend = true;

                    string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t灯4关闭\t";
                    Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                    Global.myLogStreamWriter.Flush();
                }
            }
            catch (Exception ex)
            { }
        }

        private void btn_Light_5_Click(object sender, EventArgs e)
        {
            try
            {
                btnCmdindex = 5;

                int cmdToCtlPower = CtlPowerFlag79;
                int cmdToDevice = powerOpenFlag1;

                if (btn_Light_5.BackgroundImage == global::MonitorProj.Properties.Resources.Button1)
                {
                    cmdPowerOpen = new byte[] { 0xFF, 0xFF, 0xA5, 0x79, 0x02, 0x06, 0x00, 0x6E, 0x26 };
                    cmdOpenDevice = new byte[] { 0xFF, 0xFF, 0xA5, 0x22, 0x02, 0x06, 0x00, 0x36, 0x26 };
                    typeCmdSendOpen = 1;    //1表示打开  

                    //获得供电电源其他bit的状态
                    cmdToCtlPower |= 0x02;//02开电源板0x71
                    byte[] lastState = BitConverter.GetBytes(cmdToCtlPower);
                    cmdPowerOpen[6] = lastState[0];

                    //获得灯开关其他bit的状态
                    cmdToDevice |= 0x10;
                    cmdToDevice &= 0xF0;
                    cmdToDevice = (cmdToDevice >> 4);
                    lastState = BitConverter.GetBytes(cmdToDevice);
                    cmdOpenDevice[6] = lastState[0];

                    //校验
                    byte check = CheckCtlDataSum(cmdPowerOpen, 3, 4);
                    cmdPowerOpen[7] = check;
                    check = CheckCtlDataSum(cmdOpenDevice, 3, 4);
                    cmdOpenDevice[7] = check;

                    flagCmdToSend = true;

                    string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t灯5打开\t";
                    Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                    Global.myLogStreamWriter.Flush();
                }
                else//关闭
                {
                    cmdPowerClose = new byte[] { 0xFF, 0xFF, 0xA5, 0x79, 0x02, 0x06, 0x00, 0x6E, 0x26 };
                    cmdCloseDevice = new byte[] { 0xFF, 0xFF, 0xA5, 0x22, 0x02, 0x06, 0x00, 0x36, 0x26 };
                    typeCmdSendOpen = 2;    //2表示关闭

                    //获得供电电源其他bit的状态
                    cmdToCtlPower &= ~(0x02);
                    byte[] lastState = BitConverter.GetBytes(cmdToCtlPower);
                    cmdPowerClose[6] = lastState[0];

                    //获得灯开关其他bit的状态
                    cmdToDevice &= ~(0x10);
                    cmdToDevice &= 0xF0;
                    cmdToDevice = (cmdToDevice >> 4);
                    lastState = BitConverter.GetBytes(cmdToDevice);
                    cmdCloseDevice[6] = lastState[0];

                    //校验
                    byte check = CheckCtlDataSum(cmdPowerClose, 3, 4);
                    cmdPowerClose[7] = check;
                    check = CheckCtlDataSum(cmdCloseDevice, 3, 4);
                    cmdCloseDevice[7] = check;
                    flagCmdToSend = true;

                    string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t灯5关闭\t";
                    Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                    Global.myLogStreamWriter.Flush();
                }
            }
            catch (Exception ex)
            { }
        }

        private void btn_Light_6_Click(object sender, EventArgs e)
        {
            try
            {
                btnCmdindex = 6;

                int cmdToCtlPower = CtlPowerFlag79;
                int cmdToDevice = powerOpenFlag1;

                if (btn_Light_6.BackgroundImage == global::MonitorProj.Properties.Resources.Button1)
                {
                    cmdPowerOpen = new byte[] { 0xFF, 0xFF, 0xA5, 0x79, 0x02, 0x06, 0x00, 0x6E, 0x26 };
                    cmdOpenDevice = new byte[] { 0xFF, 0xFF, 0xA5, 0x22, 0x02, 0x06, 0x00, 0x36, 0x26 };
                    typeCmdSendOpen = 1;    //1表示打开  

                    //获得供电电源其他bit的状态
                    cmdToCtlPower |= 0x02;//02开电源板0x71
                    byte[] lastState = BitConverter.GetBytes(cmdToCtlPower);
                    cmdPowerOpen[6] = lastState[0];

                    //获得灯开关其他bit的状态
                    cmdToDevice |= 0x20;
                    cmdToDevice &= 0xF0;
                    cmdToDevice = (cmdToDevice >> 4);
                    lastState = BitConverter.GetBytes(cmdToDevice);
                    cmdOpenDevice[6] = lastState[0];

                    //校验
                    byte check = CheckCtlDataSum(cmdPowerOpen, 3, 4);
                    cmdPowerOpen[7] = check;
                    check = CheckCtlDataSum(cmdOpenDevice, 3, 4);
                    cmdOpenDevice[7] = check;

                    flagCmdToSend = true;

                    string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t灯6打开\t";
                    Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                    Global.myLogStreamWriter.Flush();
                }
                else//关闭
                {
                    cmdPowerClose = new byte[] { 0xFF, 0xFF, 0xA5, 0x79, 0x02, 0x06, 0x00, 0x6E, 0x26 };
                    cmdCloseDevice = new byte[] { 0xFF, 0xFF, 0xA5, 0x22, 0x02, 0x06, 0x00, 0x36, 0x26 };
                    typeCmdSendOpen = 2;    //2表示关闭

                    //获得供电电源其他bit的状态
                    cmdToCtlPower &= ~(0x02);
                    byte[] lastState = BitConverter.GetBytes(cmdToCtlPower);
                    cmdPowerClose[6] = lastState[0];

                    //获得灯开关其他bit的状态
                    cmdToDevice &= ~(0x20);
                    cmdToDevice &= 0xF0;
                    cmdToDevice = (cmdToDevice >> 4);
                    lastState = BitConverter.GetBytes(cmdToDevice);
                    cmdCloseDevice[6] = lastState[0];

                    //校验
                    byte check = CheckCtlDataSum(cmdPowerClose, 3, 4);
                    cmdPowerClose[7] = check;
                    check = CheckCtlDataSum(cmdCloseDevice, 3, 4);
                    cmdCloseDevice[7] = check;
                    flagCmdToSend = true;

                    string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t灯6关闭\t";
                    Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                    Global.myLogStreamWriter.Flush();
                }
            }
            catch (Exception ex)
            { }
        }

        private void btn_Light_7_Click(object sender, EventArgs e)
        {
            try
            {
                btnCmdindex = 7;

                int cmdToCtlPower = CtlPowerFlag79;
                int cmdToDevice = powerOpenFlag1;

                if (btn_Light_7.BackgroundImage == global::MonitorProj.Properties.Resources.Button1)
                {
                    cmdPowerOpen = new byte[] { 0xFF, 0xFF, 0xA5, 0x79, 0x02, 0x06, 0x00, 0x6E, 0x26 };
                    cmdOpenDevice = new byte[] { 0xFF, 0xFF, 0xA5, 0x22, 0x02, 0x06, 0x00, 0x36, 0x26 };
                    typeCmdSendOpen = 1;    //1表示打开  

                    //获得供电电源其他bit的状态
                    cmdToCtlPower |= 0x02;//02开电源板0x71
                    byte[] lastState = BitConverter.GetBytes(cmdToCtlPower);
                    cmdPowerOpen[6] = lastState[0];

                    //获得灯开关其他bit的状态
                    cmdToDevice |= 0x40;
                    cmdToDevice &= 0xF0;
                    cmdToDevice = (cmdToDevice >> 4);
                    lastState = BitConverter.GetBytes(cmdToDevice);
                    cmdOpenDevice[6] = lastState[0];

                    //校验
                    byte check = CheckCtlDataSum(cmdPowerOpen, 3, 4);
                    cmdPowerOpen[7] = check;
                    check = CheckCtlDataSum(cmdOpenDevice, 3, 4);
                    cmdOpenDevice[7] = check;

                    flagCmdToSend = true;

                    string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t灯7打开\t";
                    Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                    Global.myLogStreamWriter.Flush();
                }
                else//关闭
                {
                    cmdPowerClose = new byte[] { 0xFF, 0xFF, 0xA5, 0x79, 0x02, 0x06, 0x00, 0x6E, 0x26 };
                    cmdCloseDevice = new byte[] { 0xFF, 0xFF, 0xA5, 0x22, 0x02, 0x06, 0x00, 0x36, 0x26 };
                    typeCmdSendOpen = 2;    //2表示关闭

                    //获得供电电源其他bit的状态
                    cmdToCtlPower &= ~(0x02);
                    byte[] lastState = BitConverter.GetBytes(cmdToCtlPower);
                    cmdPowerClose[6] = lastState[0];

                    //获得灯开关其他bit的状态
                    cmdToDevice &= ~(0x40);
                    cmdToDevice &= 0xF0;
                    cmdToDevice = (cmdToDevice >> 4);
                    lastState = BitConverter.GetBytes(cmdToDevice);
                    cmdCloseDevice[6] = lastState[0];

                    //校验
                    byte check = CheckCtlDataSum(cmdPowerClose, 3, 4);
                    cmdPowerClose[7] = check;
                    check = CheckCtlDataSum(cmdCloseDevice, 3, 4);
                    cmdCloseDevice[7] = check;
                    flagCmdToSend = true;

                    string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t灯7关闭\t";
                    Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                    Global.myLogStreamWriter.Flush();
                }
            }
            catch (Exception ex)
            { }
        }

        private void btn_Light_8_Click(object sender, EventArgs e)
        {
            try
            {
                btnCmdindex = 8;

                int cmdToCtlPower = CtlPowerFlag79;
                int cmdToDevice = powerOpenFlag1;

                if (btn_Light_8.BackgroundImage == global::MonitorProj.Properties.Resources.Button1)
                {
                    cmdPowerOpen = new byte[] { 0xFF, 0xFF, 0xA5, 0x79, 0x02, 0x06, 0x00, 0x6E, 0x26 };
                    cmdOpenDevice = new byte[] { 0xFF, 0xFF, 0xA5, 0x22, 0x02, 0x06, 0x00, 0x36, 0x26 };
                    typeCmdSendOpen = 1;    //1表示打开  

                    //获得供电电源其他bit的状态
                    cmdToCtlPower |= 0x02;//02开电源板0x71
                    byte[] lastState = BitConverter.GetBytes(cmdToCtlPower);
                    cmdPowerOpen[6] = lastState[0];

                    //获得灯开关其他bit的状态
                    cmdToDevice |= 0x80;
                    cmdToDevice &= 0xF0;
                    cmdToDevice = (cmdToDevice >> 4);
                    lastState = BitConverter.GetBytes(cmdToDevice);
                    cmdOpenDevice[6] = lastState[0];

                    //校验
                    byte check = CheckCtlDataSum(cmdPowerOpen, 3, 4);
                    cmdPowerOpen[7] = check;
                    check = CheckCtlDataSum(cmdOpenDevice, 3, 4);
                    cmdOpenDevice[7] = check;

                    flagCmdToSend = true;

                    string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t灯8打开\t";
                    Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                    Global.myLogStreamWriter.Flush();
                }
                else//关闭
                {
                    cmdPowerClose = new byte[] { 0xFF, 0xFF, 0xA5, 0x79, 0x02, 0x06, 0x00, 0x6E, 0x26 };
                    cmdCloseDevice = new byte[] { 0xFF, 0xFF, 0xA5, 0x22, 0x02, 0x06, 0x00, 0x36, 0x26 };
                    typeCmdSendOpen = 2;    //2表示关闭

                    //获得供电电源其他bit的状态
                    cmdToCtlPower &= ~(0x02);
                    byte[] lastState = BitConverter.GetBytes(cmdToCtlPower);
                    cmdPowerClose[6] = lastState[0];

                    //获得灯开关其他bit的状态
                    cmdToDevice &= ~(0x80);
                    cmdToDevice &= 0xF0;
                    cmdToDevice = (cmdToDevice >> 4);
                    lastState = BitConverter.GetBytes(cmdToDevice);
                    cmdCloseDevice[6] = lastState[0];

                    //校验
                    byte check = CheckCtlDataSum(cmdPowerClose, 3, 4);
                    cmdPowerClose[7] = check;
                    check = CheckCtlDataSum(cmdCloseDevice, 3, 4);
                    cmdCloseDevice[7] = check;
                    flagCmdToSend = true;

                    string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t灯8关闭\t";
                    Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                    Global.myLogStreamWriter.Flush();
                }
            }
            catch (Exception ex)
            { }
        }

        #endregion


        #region 灯舱继电器板（0x21、0x22）
        //0x28 第一路罗盘电源（均来自0x61A）
        //0x28：2-4备用12V电源（输出功率较小）。

        //0x29：24输出（来自0x62B），
        //第一路高度计电源
        //第二路深度计电源
        //第3-4路备用电源

        private void btn_DetectPanel_Rotate_Click(object sender, EventArgs e)
        {
            try
            {
                btnCmdindex = 19;

                int cmdToCtlPower = CtlPowerFlag61;
                int cmdToDevice = powerOpenFlag2;

                if (btn_DetectPanel_Rotate.BackgroundImage == global::MonitorProj.Properties.Resources.Button1)
                {
                    cmdPowerOpen = new byte[] { 0xFF, 0xFF, 0xA5, 0x61, 0x02, 0x06, 0x00, 0x00, 0x26 };
                    cmdOpenDevice = new byte[] { 0xFF, 0xFF, 0xA5, 0x28, 0x02, 0x06, 0x00, 0x00, 0x26 };
                    typeCmdSendOpen = 1;    //1表示打开  

                    //获得供电电源其他bit的状态
                    cmdToCtlPower |= 0x01;//开电源板0x61A
                    byte[] lastState = BitConverter.GetBytes(cmdToCtlPower);
                    cmdPowerOpen[6] = lastState[0];

                    //获得开关其他bit的状态
                    cmdToDevice |= 0x100;
                    lastState = BitConverter.GetBytes(cmdToDevice);
                    cmdOpenDevice[6] = lastState[1];

                    //校验
                    byte check = CheckCtlDataSum(cmdPowerOpen, 3, 4);
                    cmdPowerOpen[7] = check;
                    check = CheckCtlDataSum(cmdOpenDevice, 3, 4);
                    cmdOpenDevice[7] = check;

                    flagCmdToSend = true;

                    string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t第一路罗盘打开\t";
                    Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                    Global.myLogStreamWriter.Flush();
                }
                else//关闭
                {
                    cmdPowerClose = new byte[] { 0xFF, 0xFF, 0xA5, 0x61, 0x02, 0x06, 0x00, 0x00, 0x26 };
                    cmdCloseDevice = new byte[] { 0xFF, 0xFF, 0xA5, 0x28, 0x02, 0x06, 0x00, 0x00, 0x26 };
                    typeCmdSendOpen = 2;    //2表示关闭

                    //获得供电电源其他bit的状态
                    cmdToCtlPower &= ~(0x01);
                    byte[] lastState = BitConverter.GetBytes(cmdToCtlPower);
                    cmdPowerClose[6] = lastState[0];

                    //获得开关其他bit的状态
                    cmdToDevice &= ~(0x100);
                    lastState = BitConverter.GetBytes(cmdToDevice);
                    cmdCloseDevice[6] = lastState[1];

                    //校验
                    byte check = CheckCtlDataSum(cmdPowerClose, 3, 4);
                    cmdPowerClose[7] = check;
                    check = CheckCtlDataSum(cmdCloseDevice, 3, 4);
                    cmdCloseDevice[7] = check;
                    flagCmdToSend = true;

                    string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t第一路罗盘关闭\t";
                    Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                    Global.myLogStreamWriter.Flush();
                }
            }
            catch (Exception ex)
            { }
        }

        private void btn_DetectPanel_Space_12V1_Click(object sender, EventArgs e)
        {
            try
            {
                btnCmdindex = 20;

                int cmdToCtlPower = CtlPowerFlag61;
                int cmdToDevice = powerOpenFlag2;

                if (btn_DetectPanel_Space_12V1.BackgroundImage == global::MonitorProj.Properties.Resources.Button1)
                {
                    cmdPowerOpen = new byte[] { 0xFF, 0xFF, 0xA5, 0x61, 0x02, 0x06, 0x00, 0x00, 0x26 };
                    cmdOpenDevice = new byte[] { 0xFF, 0xFF, 0xA5, 0x28, 0x02, 0x06, 0x00, 0x00, 0x26 };
                    typeCmdSendOpen = 1;    //1表示打开  

                    //获得供电电源其他bit的状态
                    cmdToCtlPower |= 0x01;//开电源板0x61A
                    byte[] lastState = BitConverter.GetBytes(cmdToCtlPower);
                    cmdPowerOpen[6] = lastState[0];

                    //获得开关其他bit的状态
                    cmdToDevice |= 0x200;
                    lastState = BitConverter.GetBytes(cmdToDevice);
                    cmdOpenDevice[6] = lastState[1];

                    //校验
                    byte check = CheckCtlDataSum(cmdPowerOpen, 3, 4);
                    cmdPowerOpen[7] = check;
                    check = CheckCtlDataSum(cmdOpenDevice, 3, 4);
                    cmdOpenDevice[7] = check;

                    flagCmdToSend = true;

                    string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t备用12V电源1打开\t";
                    Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                    Global.myLogStreamWriter.Flush();
                }
                else//关闭
                {
                    cmdPowerClose = new byte[] { 0xFF, 0xFF, 0xA5, 0x61, 0x02, 0x06, 0x00, 0x00, 0x26 };
                    cmdCloseDevice = new byte[] { 0xFF, 0xFF, 0xA5, 0x28, 0x02, 0x06, 0x00, 0x00, 0x26 };
                    typeCmdSendOpen = 2;    //2表示关闭

                    //获得供电电源其他bit的状态
                    cmdToCtlPower &= ~(0x01);
                    byte[] lastState = BitConverter.GetBytes(cmdToCtlPower);
                    cmdPowerClose[6] = lastState[0];

                    //获得开关其他bit的状态
                    cmdToDevice &= ~(0x200);
                    lastState = BitConverter.GetBytes(cmdToDevice);
                    cmdCloseDevice[6] = lastState[1];

                    //校验
                    byte check = CheckCtlDataSum(cmdPowerClose, 3, 4);
                    cmdPowerClose[7] = check;
                    check = CheckCtlDataSum(cmdCloseDevice, 3, 4);
                    cmdCloseDevice[7] = check;
                    flagCmdToSend = true;

                    string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t备用12V电源1关闭\t";
                    Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                    Global.myLogStreamWriter.Flush();
                }
            }
            catch (Exception ex)
            { }
        }

        private void btn_DetectPanel_Space_12V2_Click(object sender, EventArgs e)
        {
            try
            {
                btnCmdindex = 21;

                int cmdToCtlPower = CtlPowerFlag61;
                int cmdToDevice = powerOpenFlag2;

                if (btn_DetectPanel_Space_12V2.BackgroundImage == global::MonitorProj.Properties.Resources.Button1)
                {
                    cmdPowerOpen = new byte[] { 0xFF, 0xFF, 0xA5, 0x61, 0x02, 0x06, 0x00, 0x00, 0x26 };
                    cmdOpenDevice = new byte[] { 0xFF, 0xFF, 0xA5, 0x28, 0x02, 0x06, 0x00, 0x00, 0x26 };
                    typeCmdSendOpen = 1;    //1表示打开  

                    //获得供电电源其他bit的状态
                    cmdToCtlPower |= 0x01;//开电源板0x61A
                    byte[] lastState = BitConverter.GetBytes(cmdToCtlPower);
                    cmdPowerOpen[6] = lastState[0];

                    //获得开关其他bit的状态
                    cmdToDevice |= 0x400;
                    lastState = BitConverter.GetBytes(cmdToDevice);
                    cmdOpenDevice[6] = lastState[1];

                    //校验
                    byte check = CheckCtlDataSum(cmdPowerOpen, 3, 4);
                    cmdPowerOpen[7] = check;
                    check = CheckCtlDataSum(cmdOpenDevice, 3, 4);
                    cmdOpenDevice[7] = check;

                    flagCmdToSend = true;

                    string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t备用12V电源2打开\t";
                    Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                    Global.myLogStreamWriter.Flush();
                }
                else//关闭
                {
                    cmdPowerClose = new byte[] { 0xFF, 0xFF, 0xA5, 0x61, 0x02, 0x06, 0x00, 0x00, 0x26 };
                    cmdCloseDevice = new byte[] { 0xFF, 0xFF, 0xA5, 0x28, 0x02, 0x06, 0x00, 0x00, 0x26 };
                    typeCmdSendOpen = 2;    //2表示关闭

                    //获得供电电源其他bit的状态
                    cmdToCtlPower &= ~(0x01);
                    byte[] lastState = BitConverter.GetBytes(cmdToCtlPower);
                    cmdPowerClose[6] = lastState[0];

                    //获得开关其他bit的状态
                    cmdToDevice &= ~(0x400);
                    lastState = BitConverter.GetBytes(cmdToDevice);
                    cmdCloseDevice[6] = lastState[1];

                    //校验
                    byte check = CheckCtlDataSum(cmdPowerClose, 3, 4);
                    cmdPowerClose[7] = check;
                    check = CheckCtlDataSum(cmdCloseDevice, 3, 4);
                    cmdCloseDevice[7] = check;
                    flagCmdToSend = true;

                    string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t备用12V电源2关闭\t";
                    Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                    Global.myLogStreamWriter.Flush();
                }
            }
            catch (Exception ex)
            { }
        }

        private void btn_DetectPanel_Space_12V3_Click(object sender, EventArgs e)
        {
            try
            {
                btnCmdindex = 22;

                int cmdToCtlPower = CtlPowerFlag61;
                int cmdToDevice = powerOpenFlag2;

                if (btn_DetectPanel_Space_12V3.BackgroundImage == global::MonitorProj.Properties.Resources.Button1)
                {
                    cmdPowerOpen = new byte[] { 0xFF, 0xFF, 0xA5, 0x61, 0x02, 0x06, 0x00, 0x00, 0x26 };
                    cmdOpenDevice = new byte[] { 0xFF, 0xFF, 0xA5, 0x28, 0x02, 0x06, 0x00, 0x00, 0x26 };
                    typeCmdSendOpen = 1;    //1表示打开  

                    //获得供电电源其他bit的状态
                    cmdToCtlPower |= 0x01;//开电源板0x61A
                    byte[] lastState = BitConverter.GetBytes(cmdToCtlPower);
                    cmdPowerOpen[6] = lastState[0];

                    //获得开关其他bit的状态
                    cmdToDevice |= 0x800;
                    lastState = BitConverter.GetBytes(cmdToDevice);
                    cmdOpenDevice[6] = lastState[1];

                    //校验
                    byte check = CheckCtlDataSum(cmdPowerOpen, 3, 4);
                    cmdPowerOpen[7] = check;
                    check = CheckCtlDataSum(cmdOpenDevice, 3, 4);
                    cmdOpenDevice[7] = check;

                    flagCmdToSend = true;

                    string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t备用12V电源3打开\t";
                    Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                    Global.myLogStreamWriter.Flush();
                }
                else//关闭
                {
                    cmdPowerClose = new byte[] { 0xFF, 0xFF, 0xA5, 0x61, 0x02, 0x06, 0x00, 0x00, 0x26 };
                    cmdCloseDevice = new byte[] { 0xFF, 0xFF, 0xA5, 0x28, 0x02, 0x06, 0x00, 0x00, 0x26 };
                    typeCmdSendOpen = 2;    //2表示关闭

                    //获得供电电源其他bit的状态
                    cmdToCtlPower &= ~(0x01);
                    byte[] lastState = BitConverter.GetBytes(cmdToCtlPower);
                    cmdPowerClose[6] = lastState[0];

                    //获得开关其他bit的状态
                    cmdToDevice &= ~(0x800);
                    lastState = BitConverter.GetBytes(cmdToDevice);
                    cmdCloseDevice[6] = lastState[1];

                    //校验
                    byte check = CheckCtlDataSum(cmdPowerClose, 3, 4);
                    cmdPowerClose[7] = check;
                    check = CheckCtlDataSum(cmdCloseDevice, 3, 4);
                    cmdCloseDevice[7] = check;
                    flagCmdToSend = true;

                    string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t备用12V电源3关闭\t";
                    Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                    Global.myLogStreamWriter.Flush();
                }
            }
            catch (Exception ex)
            { }
        }


        private int CtlPowerFlag62 = 0x00;//0x62B 24V供应接口箱内继电器板（0x29）的电源，供应高度计和三个扩展设备。（也就是说开高度计时，此电源应该发送打开指令以提供电源）。
        private void btn_DetectPanel_Hight_Click(object sender, EventArgs e)
        {
            try
            {
                btnCmdindex = 31;

                int cmdToCtlPower = CtlPowerFlag62;
                int cmdToDevice = powerOpenFlag3;

                if (btn_DetectPanel_Hight.BackgroundImage == global::MonitorProj.Properties.Resources.Button1)
                {
                    cmdPowerOpen = new byte[] { 0xFF, 0xFF, 0xA5, 0x62, 0x02, 0x06, 0x00, 0x00, 0x26 };
                    cmdOpenDevice = new byte[] { 0xFF, 0xFF, 0xA5, 0x29, 0x02, 0x06, 0x00, 0x00, 0x26 };
                    typeCmdSendOpen = 1;    //1表示打开  

                    //获得供电电源其他bit的状态
                    cmdToCtlPower |= 0x10;//开电源板0x62B
                    byte[] lastState = BitConverter.GetBytes(cmdToCtlPower);
                    cmdPowerOpen[6] = lastState[0];

                    //获得开关其他bit的状态
                    cmdToDevice |= 0x01;
                    cmdToDevice &= 0x0F;
                    lastState = BitConverter.GetBytes(cmdToDevice);
                    cmdOpenDevice[6] = lastState[0];

                    //校验
                    byte check = CheckCtlDataSum(cmdPowerOpen, 3, 4);
                    cmdPowerOpen[7] = check;
                    check = CheckCtlDataSum(cmdOpenDevice, 3, 4);
                    cmdOpenDevice[7] = check;

                    flagCmdToSend = true;

                    string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t高度计电源打开\t";
                    Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                    Global.myLogStreamWriter.Flush();
                }
                else//关闭
                {
                    cmdPowerClose = new byte[] { 0xFF, 0xFF, 0xA5, 0x62, 0x02, 0x06, 0x00, 0x00, 0x26 };
                    cmdCloseDevice = new byte[] { 0xFF, 0xFF, 0xA5, 0x29, 0x02, 0x06, 0x00, 0x00, 0x26 };
                    typeCmdSendOpen = 2;    //2表示关闭

                    //获得供电电源其他bit的状态
                    cmdToCtlPower &= ~(0x10);
                    byte[] lastState = BitConverter.GetBytes(cmdToCtlPower);
                    cmdPowerClose[6] = lastState[0];

                    //获得开关其他bit的状态
                    cmdToDevice &= ~(0x01);
                    cmdToDevice &= 0x0F;
                    lastState = BitConverter.GetBytes(cmdToDevice);
                    cmdCloseDevice[6] = lastState[0];

                    //校验
                    byte check = CheckCtlDataSum(cmdPowerClose, 3, 4);
                    cmdPowerClose[7] = check;
                    check = CheckCtlDataSum(cmdCloseDevice, 3, 4);
                    cmdCloseDevice[7] = check;
                    flagCmdToSend = true;

                    string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t高度计电源关闭\t";
                    Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                    Global.myLogStreamWriter.Flush();
                }
            }
            catch (Exception ex)
            { }
        }

        private void btn_DetectPanel_Deep_Click(object sender, EventArgs e)
        {
            try
            {
                btnCmdindex = 32;

                int cmdToCtlPower = CtlPowerFlag62;
                int cmdToDevice = powerOpenFlag3;

                if (btn_DetectPanel_Deep.BackgroundImage == global::MonitorProj.Properties.Resources.Button1)
                {
                    cmdPowerOpen = new byte[] { 0xFF, 0xFF, 0xA5, 0x62, 0x02, 0x06, 0x00, 0x00, 0x26 };
                    cmdOpenDevice = new byte[] { 0xFF, 0xFF, 0xA5, 0x29, 0x02, 0x06, 0x00, 0x00, 0x26 };
                    typeCmdSendOpen = 1;    //1表示打开  

                    //获得供电电源其他bit的状态
                    cmdToCtlPower |= 0x10;//开电源板0x62B
                    byte[] lastState = BitConverter.GetBytes(cmdToCtlPower);
                    cmdPowerOpen[6] = lastState[0];

                    //获得开关其他bit的状态
                    cmdToDevice |= 0x02;
                    cmdToDevice &= 0x0F;
                    lastState = BitConverter.GetBytes(cmdToDevice);
                    cmdOpenDevice[6] = lastState[0];

                    //校验
                    byte check = CheckCtlDataSum(cmdPowerOpen, 3, 4);
                    cmdPowerOpen[7] = check;
                    check = CheckCtlDataSum(cmdOpenDevice, 3, 4);
                    cmdOpenDevice[7] = check;

                    flagCmdToSend = true;

                    string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t深度计电源打开\t";
                    Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                    Global.myLogStreamWriter.Flush();
                }
                else//关闭
                {
                    cmdPowerClose = new byte[] { 0xFF, 0xFF, 0xA5, 0x62, 0x02, 0x06, 0x00, 0x00, 0x26 };
                    cmdCloseDevice = new byte[] { 0xFF, 0xFF, 0xA5, 0x29, 0x02, 0x06, 0x00, 0x00, 0x26 };
                    typeCmdSendOpen = 2;    //2表示关闭

                    //获得供电电源其他bit的状态
                    cmdToCtlPower &= ~(0x10);
                    byte[] lastState = BitConverter.GetBytes(cmdToCtlPower);
                    cmdPowerClose[6] = lastState[0];

                    //获得开关其他bit的状态
                    cmdToDevice &= ~(0x02);
                    cmdToDevice &= 0x0F;
                    lastState = BitConverter.GetBytes(cmdToDevice);
                    cmdCloseDevice[6] = lastState[0];

                    //校验
                    byte check = CheckCtlDataSum(cmdPowerClose, 3, 4);
                    cmdPowerClose[7] = check;
                    check = CheckCtlDataSum(cmdCloseDevice, 3, 4);
                    cmdCloseDevice[7] = check;
                    flagCmdToSend = true;

                    string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t深度计电源关闭\t";
                    Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                    Global.myLogStreamWriter.Flush();
                }
            }
            catch (Exception ex)
            { }
        }

        private void btn_DetectPanel_Space_Bak1_Click(object sender, EventArgs e)
        {
            try
            {
                btnCmdindex = 33;

                int cmdToCtlPower = CtlPowerFlag62;
                int cmdToDevice = powerOpenFlag3;

                if (btn_DetectPanel_Space_Bak1.BackgroundImage == global::MonitorProj.Properties.Resources.Button1)
                {
                    cmdPowerOpen = new byte[] { 0xFF, 0xFF, 0xA5, 0x62, 0x02, 0x06, 0x00, 0x00, 0x26 };
                    cmdOpenDevice = new byte[] { 0xFF, 0xFF, 0xA5, 0x29, 0x02, 0x06, 0x00, 0x00, 0x26 };
                    typeCmdSendOpen = 1;    //1表示打开  

                    //获得供电电源其他bit的状态
                    cmdToCtlPower |= 0x10;//开电源板0x62B
                    byte[] lastState = BitConverter.GetBytes(cmdToCtlPower);
                    cmdPowerOpen[6] = lastState[0];

                    //获得开关其他bit的状态
                    cmdToDevice |= 0x04;
                    cmdToDevice &= 0x0F;
                    lastState = BitConverter.GetBytes(cmdToDevice);
                    cmdOpenDevice[6] = lastState[0];

                    //校验
                    byte check = CheckCtlDataSum(cmdPowerOpen, 3, 4);
                    cmdPowerOpen[7] = check;
                    check = CheckCtlDataSum(cmdOpenDevice, 3, 4);
                    cmdOpenDevice[7] = check;

                    flagCmdToSend = true;

                    string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t备用电源(#29)打开\t";
                    Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                    Global.myLogStreamWriter.Flush();
                }
                else//关闭
                {
                    cmdPowerClose = new byte[] { 0xFF, 0xFF, 0xA5, 0x62, 0x02, 0x06, 0x00, 0x00, 0x26 };
                    cmdCloseDevice = new byte[] { 0xFF, 0xFF, 0xA5, 0x29, 0x02, 0x06, 0x00, 0x00, 0x26 };
                    typeCmdSendOpen = 2;    //2表示关闭

                    //获得供电电源其他bit的状态
                    cmdToCtlPower &= ~(0x10);
                    byte[] lastState = BitConverter.GetBytes(cmdToCtlPower);
                    cmdPowerClose[6] = lastState[0];

                    //获得开关其他bit的状态
                    cmdToDevice &= ~(0x04);
                    cmdToDevice &= 0x0F;
                    lastState = BitConverter.GetBytes(cmdToDevice);
                    cmdCloseDevice[6] = lastState[0];

                    //校验
                    byte check = CheckCtlDataSum(cmdPowerClose, 3, 4);
                    cmdPowerClose[7] = check;
                    check = CheckCtlDataSum(cmdCloseDevice, 3, 4);
                    cmdCloseDevice[7] = check;
                    flagCmdToSend = true;

                    string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t备用电源(#29)关闭\t";
                    Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                    Global.myLogStreamWriter.Flush();
                }
            }
            catch (Exception ex)
            { }
        }

        private void btn_DetectPanel_Space_Bak2_Click(object sender, EventArgs e)
        {
            try
            {
                btnCmdindex = 34;

                int cmdToCtlPower = CtlPowerFlag62;
                int cmdToDevice = powerOpenFlag3;

                if (btn_DetectPanel_Space_Bak2.BackgroundImage == global::MonitorProj.Properties.Resources.Button1)
                {
                    cmdPowerOpen = new byte[] { 0xFF, 0xFF, 0xA5, 0x62, 0x02, 0x06, 0x00, 0x00, 0x26 };
                    cmdOpenDevice = new byte[] { 0xFF, 0xFF, 0xA5, 0x29, 0x02, 0x06, 0x00, 0x00, 0x26 };
                    typeCmdSendOpen = 1;    //1表示打开  

                    //获得供电电源其他bit的状态
                    cmdToCtlPower |= 0x10;//开电源板0x62B
                    byte[] lastState = BitConverter.GetBytes(cmdToCtlPower);
                    cmdPowerOpen[6] = lastState[0];

                    //获得开关其他bit的状态
                    cmdToDevice |= 0x08;
                    cmdToDevice &= 0x0F;
                    lastState = BitConverter.GetBytes(cmdToDevice);
                    cmdOpenDevice[6] = lastState[0];

                    //校验
                    byte check = CheckCtlDataSum(cmdPowerOpen, 3, 4);
                    cmdPowerOpen[7] = check;
                    check = CheckCtlDataSum(cmdOpenDevice, 3, 4);
                    cmdOpenDevice[7] = check;

                    flagCmdToSend = true;

                    string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t备用电源(#29)2打开\t";
                    Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                    Global.myLogStreamWriter.Flush();
                }
                else//关闭
                {
                    cmdPowerClose = new byte[] { 0xFF, 0xFF, 0xA5, 0x62, 0x02, 0x06, 0x00, 0x00, 0x26 };
                    cmdCloseDevice = new byte[] { 0xFF, 0xFF, 0xA5, 0x29, 0x02, 0x06, 0x00, 0x00, 0x26 };
                    typeCmdSendOpen = 2;    //2表示关闭

                    //获得供电电源其他bit的状态
                    cmdToCtlPower &= ~(0x10);
                    byte[] lastState = BitConverter.GetBytes(cmdToCtlPower);
                    cmdPowerClose[6] = lastState[0];

                    //获得开关其他bit的状态
                    cmdToDevice &= ~(0x08);
                    cmdToDevice &= 0x0F;
                    lastState = BitConverter.GetBytes(cmdToDevice);
                    cmdCloseDevice[6] = lastState[0];

                    //校验
                    byte check = CheckCtlDataSum(cmdPowerClose, 3, 4);
                    cmdPowerClose[7] = check;
                    check = CheckCtlDataSum(cmdCloseDevice, 3, 4);
                    cmdCloseDevice[7] = check;
                    flagCmdToSend = true;

                    string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t备用电源(#29)2关闭\t";
                    Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                    Global.myLogStreamWriter.Flush();
                }
            }
            catch (Exception ex)
            { }
        }
        
        #endregion



        //灯舱控制板（0x23、0x24）
        //指令：X1 X2 X3 X4 X5 X6 X7 X8
        //X1-X4输出4路模拟量，数值范围0-100，对应实际输出0-5V。
        //X5-X8未用
        byte[] cmdLightDensitySet;
        private void btn_Light_DensitySet_23_Click(object sender, EventArgs e)
        {
            try
            {
                btnCmdindex = 51;

                int v1 = trackBar_Light_1.Value;
                byte b1 = BitConverter.GetBytes(v1)[0];
                int v2 = trackBar_Light_2.Value;
                byte b2 = BitConverter.GetBytes(v2)[0];
                int v3 = trackBar_Light_3.Value;
                byte b3 = BitConverter.GetBytes(v3)[0];
                int v4 = trackBar_Light_4.Value;
                byte b4 = BitConverter.GetBytes(v4)[0];

                byte[] cmdOpenDevice23 = new byte[16];
                cmdOpenDevice23[0] = 0xFF;
                cmdOpenDevice23[1] = 0xFF;
                cmdOpenDevice23[2] = 0xA5;
                cmdOpenDevice23[3] = 0x23;
                cmdOpenDevice23[4] = 0x02;
                cmdOpenDevice23[5] = 0x0D;
                cmdOpenDevice23[6] = b1;
                cmdOpenDevice23[7] = b2;
                cmdOpenDevice23[8] = b3;
                cmdOpenDevice23[9] = b4;
                cmdOpenDevice23[10] = b1;
                cmdOpenDevice23[11] = b2;
                cmdOpenDevice23[12] = b3;
                cmdOpenDevice23[13] = b4;
                cmdOpenDevice23[14] = 0x00;
                cmdOpenDevice23[15] = 0x26;
                byte check = CheckCtlDataSum(cmdOpenDevice23, 3, 11);
                cmdOpenDevice23[14] = check;

                cmdLightDensitySet = cmdOpenDevice23;
                
                string sQuery = "";
                for (int i = 0; i < cmdLightDensitySet.Length; i++)
                {
                    sQuery += cmdLightDensitySet[i].ToString("X2");
                }

                string[] sQuirys = dicBoardMonCtlClassType[enum_SerialCOMType.Light_Relay_Control_Panel].SQuirys;
                sQuirys[2] = sQuery;
                dicBoardMonCtlClassType[enum_SerialCOMType.Light_Relay_Control_Panel].SQuirys = sQuirys;

                //dicBoardMonCtlClassType[enum_SerialCOMType.Light_Relay_Control_Panel].BCmdInsert(cmdOpenDevice23);

                typeCmdSendOpen = 5;    //5表示发送灯亮度调节指令
                flagCmdToSend = true;

                string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t发送灯亮度调节指令23#（" +
                    v1.ToString() + "；" + v2.ToString() + "；" + v3.ToString() + "；" + v4.ToString() + ")\t";
                Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                Global.myLogStreamWriter.Flush();
            }
            catch (Exception ex)
            { }
        }

        private void btn_Light_DensitySet_24_Click(object sender, EventArgs e)
        {
            try
            {
                btnCmdindex = 52;

                int v5 = trackBar_Light_5.Value;
                byte b5 = BitConverter.GetBytes(v5)[0];
                int v6 = trackBar_Light_6.Value;
                byte b6 = BitConverter.GetBytes(v6)[0];
                int v7 = trackBar_Light_7.Value;
                byte b7 = BitConverter.GetBytes(v7)[0];
                int v8 = trackBar_Light_8.Value;
                byte b8 = BitConverter.GetBytes(v8)[0];

                
                byte[] cmdOpenDevice24 = new byte[16];
                cmdOpenDevice24[0] = 0xFF;
                cmdOpenDevice24[1] = 0xFF;
                cmdOpenDevice24[2] = 0xA5;
                cmdOpenDevice24[3] = 0x24;
                cmdOpenDevice24[4] = 0x02;
                cmdOpenDevice24[5] = 0x0D;
                cmdOpenDevice24[6] = b5;
                cmdOpenDevice24[7] = b6;
                cmdOpenDevice24[8] = b7;
                cmdOpenDevice24[9] = b8;
                cmdOpenDevice24[10] = b5;
                cmdOpenDevice24[11] = b6;
                cmdOpenDevice24[12] = b7;
                cmdOpenDevice24[13] = b8;
                cmdOpenDevice24[14] = 0x00;
                cmdOpenDevice24[15] = 0x26;
                byte check = CheckCtlDataSum(cmdOpenDevice24, 3, 11);
                cmdOpenDevice24[14] = check;


                cmdLightDensitySet = cmdOpenDevice24;

                string sQuery = "";
                for (int i = 0; i < cmdLightDensitySet.Length; i++)
                {
                    sQuery += cmdLightDensitySet[i].ToString("X2");
                }

                string[] sQuirys = dicBoardMonCtlClassType[enum_SerialCOMType.Light_Relay_Control_Panel].SQuirys;
                sQuirys[3] = sQuery;
                dicBoardMonCtlClassType[enum_SerialCOMType.Light_Relay_Control_Panel].SQuirys = sQuirys;

                //dicBoardMonCtlClassType[enum_SerialCOMType.Light_Relay_Control_Panel].BCmdInsert(cmdOpenDevice24);

                typeCmdSendOpen = 5;    //5表示发送灯亮度调节指令
                flagCmdToSend = true;

                string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t发送灯亮度调节指令23#（" +
                    v5.ToString() + "；" + v6.ToString() + "；" + v7.ToString() + "；" + v8.ToString() + ")\t";
                Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                Global.myLogStreamWriter.Flush();
            }
            catch (Exception ex)
            { }
        }

        byte[] cmdPowerCtlOnly;
        private void btn_FaBox_72_Click(object sender, EventArgs e)
        {
            try
            {
                btnCmdindex = 61;
                int cmdToCtlPower = CtlPowerFlag79;

                if (btn_FaBox_72.BackgroundImage == global::MonitorProj.Properties.Resources.Button1)
                {
                    cmdPowerCtlOnly = new byte[] { 0xFF, 0xFF, 0xA5, 0x79, 0x02, 0x06, 0x00, 0x6E, 0x26 };
                    typeCmdSendOpen = 6;

                    //获得供电电源其他bit的状态,01 开电源板0x70，02开电源板0x71，04开电源板0x72，08，开备用继电器。
                    cmdToCtlPower |= 0x04;//04开电源板0x72
                    byte[] lastState = BitConverter.GetBytes(cmdToCtlPower);
                    cmdPowerCtlOnly[6] = lastState[0];

                    //校验
                    byte check = CheckCtlDataSum(cmdPowerCtlOnly, 3, 4);
                    cmdPowerCtlOnly[7] = check;

                    flagCmdToSend = true;

                    string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t电源板0x72打开\t";
                    Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                    Global.myLogStreamWriter.Flush();
                }
                else
                {
                    cmdPowerCtlOnly = new byte[] { 0xFF, 0xFF, 0xA5, 0x79, 0x02, 0x06, 0x00, 0x6E, 0x26 };
                    typeCmdSendOpen = 7;

                    //获得供电电源其他bit的状态,01 开电源板0x70，02开电源板0x71，04开电源板0x72，08，开备用继电器。
                    cmdToCtlPower  &= ~(0x04);//04开电源板0x72
                    byte[] lastState = BitConverter.GetBytes(cmdToCtlPower);
                    cmdPowerCtlOnly[6] = lastState[0];

                    //校验
                    byte check = CheckCtlDataSum(cmdPowerCtlOnly, 3, 4);
                    cmdPowerCtlOnly[7] = check;

                    flagCmdToSend = true;

                    string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t电源板0x72关闭\t";
                    Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                    Global.myLogStreamWriter.Flush();
                }
            }
            catch (Exception ex)
            { }
        }

        private void btn_FaBox_Space_Click(object sender, EventArgs e)
        {
            try
            {
                try
                {
                    btnCmdindex = 62;
                    int cmdToCtlPower = CtlPowerFlag79;

                    if (btn_FaBox_Space.BackgroundImage == global::MonitorProj.Properties.Resources.Button1)
                    {
                        cmdPowerCtlOnly = new byte[] { 0xFF, 0xFF, 0xA5, 0x79, 0x02, 0x06, 0x00, 0x6E, 0x26 };
                        typeCmdSendOpen = 6;

                        //获得供电电源其他bit的状态,01 开电源板0x70，02开电源板0x71，04开电源板0x72，08，开备用继电器。
                        cmdToCtlPower |= 0x08;
                        byte[] lastState = BitConverter.GetBytes(cmdToCtlPower);
                        cmdPowerCtlOnly[6] = lastState[0];

                        //校验
                        byte check = CheckCtlDataSum(cmdPowerCtlOnly, 3, 4);
                        cmdPowerCtlOnly[7] = check;

                        flagCmdToSend = true;

                        string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t电源板备用继电器打开\t";
                        Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                        Global.myLogStreamWriter.Flush();
                    }
                    else
                    {
                        cmdPowerCtlOnly = new byte[] { 0xFF, 0xFF, 0xA5, 0x79, 0x02, 0x06, 0x00, 0x6E, 0x26 };
                        typeCmdSendOpen = 7;

                        //获得供电电源其他bit的状态,01 开电源板0x70，02开电源板0x71，04开电源板0x72，08，开备用继电器。
                        cmdToCtlPower &= ~(0x08);
                        byte[] lastState = BitConverter.GetBytes(cmdToCtlPower);
                        cmdPowerCtlOnly[6] = lastState[0];

                        //校验
                        byte check = CheckCtlDataSum(cmdPowerCtlOnly, 3, 4);
                        cmdPowerCtlOnly[7] = check;

                        flagCmdToSend = true;

                        string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t电源板备用继电器关闭\t";
                        Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                        Global.myLogStreamWriter.Flush();
                    }
                }
                catch (Exception ex)
                { }
            }
            catch (Exception ex)
            { }
        }

        private void btn_HeadingCircleClear_Click(object sender, EventArgs e)
        {
            try
            {
                iHeadingCircle = 0;
                textBox_HeadingCircle.Text = iHeadingCircle.ToString();
                textBox_HeadingCircle_2.Text = iHeadingCircle.ToString();

                gaugeControl1.SetPointerValue("Pointer2", iHeadingCircle);
                gaugeControl2.SetPointerValue("Pointer2", iHeadingCircle);
            }
            catch (Exception ex)
            { }
        }
        






    }
}
