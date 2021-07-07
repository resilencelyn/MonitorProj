using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Threading;

namespace MonitorProj
{
    class BoardSerialMonCtlClass
    {

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
                mySerialPort.PortName = portName;
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
                mySerialPort.BaudRate = bandRate;
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
                mySerialPort.DataBits = dataBits;
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
                mySerialPort.StopBits = stopBits;
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
                mySerialPort.Parity = parity;
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

        #endregion

        public SerialPort mySerialPort;
        private Thread SerialSendThread;
        private Thread SerialRecvThread;
        private bool stopSerialSend = false;
        private bool stopSerialRecv = false;
        private bool isCtlSending = false;//是否要发送控制命令

        public event EventHandler EventSerialDataSend;



        public BoardSerialMonCtlClass()
        {
            try
            {
                mySerialPort = new SerialPort();
                mySerialPort.DataReceived += new SerialDataReceivedEventHandler(mySerialPort_DataReceived);

                SerialRecvThread = new Thread(new ThreadStart(SerialRecvThreadProcess));
                //SerialRecvThread.Name = name + "-SerialRecvThread";
                SerialRecvThread.Start();

                SerialSendThread = new Thread(new ThreadStart(SerialSendThreadProcess));
                //SerialSendThread.Name = name + "-SerialSendThread";
                SerialSendThread.Start();
            }
            catch (Exception ex)
            { }
        }

        ~BoardSerialMonCtlClass()
        { }

        public bool flag_SerialPort_DataRecieved = false;
        private void mySerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                flag_SerialPort_DataRecieved = true;
            }
            catch (Exception ex)
            { }
        }


        private void SerialSendThreadProcess()
        {
            while (stopSerialSend == false)
            {
                try
                {
                    if (mySerialPort != null && mySerialPort.IsOpen)
                    {
                        if (sQuirys != null && sQuirys.Length > 0)
                        {
                            int len = sQuirys.Length;
                            for (int i = 0; i < len; i++)
                            {
                                try
                                {
                                    string ss = sQuirys[i].Replace(" ", "");
                                    byte[] bQuiry;

                                    //十六进制形式
                                    bQuiry = HexStringToBytes(ss);

                                    ////字符形式
                                    //byte[] bQuiryTmp = Encoding.ASCII.GetBytes(ss);
                                    //bQuiry = new byte[bQuiryTmp.Length + 2];
                                    //Array.Copy(bQuiryTmp, 0, bQuiry, 0, bQuiryTmp.Length);
                                    //bQuiry[bQuiry.Length - 2] = 0x0D;
                                    //bQuiry[bQuiry.Length - 1] = 0x0A;

                                    //发送数据
                                    if (isCtlSending == true)
                                    {
                                        Thread.Sleep(quiryInterval);
                                        i--;
                                    }
                                    else
                                    {
                                        mySerialPort.Write(bQuiry, 0, bQuiry.Length);
                                    }
                                }
                                catch (Exception ex)
                                {

                                }

                                Thread.Sleep(quiryInterval);
                            }
                        }
                        else
                        {
                            Thread.Sleep(50);
                        }
                    }
                    else
                    {
                        Thread.Sleep(50);
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }


        private void SerialRecvThreadProcess()
        {
            GEventArgs myGEventArgs = new GEventArgs();
            myGEventArgs.nameSerial = name;
            
            while (!stopSerialRecv)
            {
                try
                {
                    if (mySerialPort != null && mySerialPort.IsOpen)
                    {
                        int count = mySerialPort.BytesToRead;
                        if (count <= 0)
                        {
                            if (stopSerialRecv)
                            {
                                break;
                            }
                            Thread.Sleep(82);
                            continue;
                        }
                        else
                        {
                            Thread.Sleep(70);
                            count = mySerialPort.BytesToRead;
                            byte[] readBuffer = new byte[count];
                            mySerialPort.Read(readBuffer, 0, count);


                            if (NameIn == "水面控制盒")
                            {
                                #region 解析

                                #region 数据协议格式校验

                                /*
                                 * 20190921，新协议
                                 * 新控制盒通讯协议
                                    查询数据：
                                    FF FF A5 23 01 05 29 26
                                    返回数据
                                    FF FF A5 23 81 30 IO1 IO2 IO3 IO4 IO5 IO6 IO7 IO8 A1H A1L A2H A2L ……A16H A16L PP 26

                                 */

                                if (readBuffer.Length < 8 + 8 + 32)  //数据长度不足
                                {
                                    continue;
                                }

                                if (readBuffer[0] == 0xFF && readBuffer[1] == 0xFF && readBuffer[2] == 0xA5)    //检验同步头
                                {
                                    byte bAddrRecv = readBuffer[3];
                                    byte bType = readBuffer[4];//指令类型:0x80-自检指令返回；0x81-查询指令返回；0x82-执行指令返回
                                    byte bDataLen = readBuffer[5];
                                    //if (count < (bDataLen + 3))  //数据长度符合性检查
                                    //{
                                    //    continue;
                                    //}
                                    //byte byteCheck = 0;
                                    //bool bCheck = SumCheck(readBuffer, 3, bDataLen - 2, out byteCheck);
                                    //if (bCheck == false || byteCheck != readBuffer[3 + bDataLen - 2])   //校验错误
                                    //{
                                    //    continue;
                                    //}
                                    //if (readBuffer[3 + bDataLen - 1] != 0x26)   //帧尾校验
                                    //{
                                    //    continue;
                                    //}

                                    if (readBuffer[47] != 0x26)   //帧尾校验
                                    {
                                        continue;
                                    }

                                #endregion

                                    //enum_AddressBoard myBoardAddress = (enum_AddressBoard)bAddrRecv;
                                    enum_AddressBoard myBoardAddress = enum_AddressBoard.Water_Control_Box_New;
                                    myGEventArgs.addressBoard = myBoardAddress;

                                    switch (myBoardAddress)
                                    {
                                        //控制盒（0x10）
                                        case enum_AddressBoard.Water_Control_Box:
                                            #region 控制盒（0x10）
                                            {
                                                if (bType == 0x80)
                                                { }
                                                else if (bType == 0x81)
                                                {
                                                    if (readBuffer.Length < 21)
                                                    {
                                                        continue;
                                                    }

                                                    Struct_Water_Control_Box myStruct_Water_Control_Box = new Struct_Water_Control_Box();

                                                    myStruct_Water_Control_Box.type = 0x81;
                                                    myStruct_Water_Control_Box.Address = (int)enum_AddressBoard.Water_Control_Box;
                                                    byte[] bData = new byte[2];
                                                    bData[1] = readBuffer[6];
                                                    bData[0] = readBuffer[7];
                                                    UInt16 data = BitConverter.ToUInt16(bData, 0);
                                                    double Vo = data * 3.3 / 4096;
                                                    double Vi = 6.16448 - 2.01 * Vo;
                                                    myStruct_Water_Control_Box.RotAxisX = Vi;

                                                    bData[1] = readBuffer[8];
                                                    bData[0] = readBuffer[9];
                                                    data = BitConverter.ToUInt16(bData, 0);
                                                    Vo = data * 3.3 / 4096;
                                                    Vi = 6.16448 - 2.01 * Vo;
                                                    myStruct_Water_Control_Box.RotAxisY = Vi;

                                                    bData[1] = readBuffer[10];
                                                    bData[0] = readBuffer[11];
                                                    data = BitConverter.ToUInt16(bData, 0);
                                                    Vo = data * 3.3 / 4096;
                                                    Vi = 6.16448 - 2.01 * Vo;
                                                    myStruct_Water_Control_Box.RotAxisZ = Vi;

                                                    bData[1] = readBuffer[12];
                                                    bData[0] = readBuffer[13];
                                                    data = BitConverter.ToUInt16(bData, 0);
                                                    Vo = data * 3.3 / 4096;
                                                    Vi = 6.16448 - 2.01 * Vo;
                                                    myStruct_Water_Control_Box.RotAxisV = Vi;

                                                    bData[1] = readBuffer[14];
                                                    bData[0] = readBuffer[15];
                                                    data = BitConverter.ToUInt16(bData, 0);
                                                    Vo = data * 3.3 / 4096;
                                                    Vi = 6.16448 - 2.01 * Vo;
                                                    myStruct_Water_Control_Box.Space1 = Vi;

                                                    bData[1] = readBuffer[16];
                                                    bData[0] = readBuffer[17];
                                                    data = BitConverter.ToUInt16(bData, 0);
                                                    Vo = data * 3.3 / 4096;
                                                    Vi = 6.16448 - 2.01 * Vo;
                                                    myStruct_Water_Control_Box.Space2 = Vi;

                                                    myStruct_Water_Control_Box.KKInfo = readBuffer[18];

                                                    myGEventArgs.dataType = 5;
                                                    myGEventArgs.connected = true;
                                                    myGEventArgs.obj = readBuffer;
                                                    myGEventArgs.objParse = myStruct_Water_Control_Box;
                                                    if (EventSerialDataSend != null)
                                                    {
                                                        EventSerialDataSend(this, myGEventArgs);
                                                    }
                                                }
                                                else if (bType == 0x82)
                                                {
                                                }
                                                else
                                                {
                                                    continue;
                                                }
                                            }
                                            #endregion
                                            break;

                                        //控制盒（0x14），新协议版本，20190921
                                        case enum_AddressBoard.Water_Control_Box_New:
                                            #region 控制盒（0x14，新协议版本）
                                            {
                                                if (bType == 0x80)
                                                { }
                                                else if (bType == 0x81)
                                                {
                                                    if (readBuffer.Length < 48)
                                                    {
                                                        continue;
                                                    }

                                                    Struct_Water_Control_Box_New myStruct_Water_Control_Box = new Struct_Water_Control_Box_New();
                                                    myStruct_Water_Control_Box.IO = new byte[8];
                                                    myStruct_Water_Control_Box.A = new double[16];
                                                    myStruct_Water_Control_Box.type = 0x81;
                                                    myStruct_Water_Control_Box.Address = (int)enum_AddressBoard.Water_Control_Box_New;
                                                    int index = 6;
                                                    for (int i = 0; i < 8; i++)
                                                    {
                                                        myStruct_Water_Control_Box.IO[i] = readBuffer[index];
                                                        index++;
                                                    }

                                                    if ((myStruct_Water_Control_Box.IO[0] & 0x01) == 0x00)
                                                    {
                                                        byte[] cmdToWaterBox = new byte[] { 0xFF, 0xFF, 0xA5, 0x24, 0x02, 0x09, 0x01, 0x00, 0x00, 0x00, 0x30, 0x26 };
                                                        SerialDataSend(cmdToWaterBox);
                                                    }

                                                    for (int i = 0; i < 16; i++)
                                                    {
                                                        byte[] bData = new byte[2];
                                                        bData[1] = readBuffer[index++];
                                                        bData[0] = readBuffer[index++];
                                                        UInt16 data = BitConverter.ToUInt16(bData, 0);
                                                        double Vo = data * 5.0 / 4096;
                                                        double Vi = Vo;
                                                        myStruct_Water_Control_Box.A[i] = Vi;
                                                    }

                                                    myGEventArgs.dataType = 5;
                                                    myGEventArgs.connected = true;
                                                    myGEventArgs.obj = readBuffer;
                                                    myGEventArgs.objParse = myStruct_Water_Control_Box;
                                                    if (EventSerialDataSend != null)
                                                    {
                                                        EventSerialDataSend(this, myGEventArgs);
                                                    }
                                                }
                                                else if (bType == 0x82)
                                                {
                                                }
                                                else
                                                {
                                                    continue;
                                                }
                                            }
                                            #endregion
                                            break;

                                        default:
                                            break;
                                    }
                                }


                                #endregion

                            }
                            else if (NameIn == "ROV供电系统")
                            {
                                myGEventArgs.addressBoard = enum_AddressBoard.Rov_Power_Box;

                                Struct_ROVPower_CtlSystem myStruct_ROVPower_CtlSystem = new Struct_ROVPower_CtlSystem();

                                myStruct_ROVPower_CtlSystem.type = 0x81;
                                myStruct_ROVPower_CtlSystem.Address = (int)enum_AddressBoard.Rov_Power_Box;

                                #region 前面12个量的测试数据解析
                                ////方式一：2、1、4、3
                                if (readBuffer[0] == 0x02 && readBuffer[1] == 0x03 && readBuffer[2] == 0x30 && readBuffer.Length >= 53)
                                {
                                    myStruct_ROVPower_CtlSystem.typeMidData = 1;

                                    byte[] bData = new byte[4];
                                    bData[0] = readBuffer[4];
                                    bData[1] = readBuffer[3];
                                    bData[2] = readBuffer[6];
                                    bData[3] = readBuffer[5];
                                    float data = BitConverter.ToSingle(bData, 0);
                                    myStruct_ROVPower_CtlSystem.I_First_A = data;

                                    bData[0] = readBuffer[8];
                                    bData[1] = readBuffer[7];
                                    bData[2] = readBuffer[10];
                                    bData[3] = readBuffer[9];
                                    data = BitConverter.ToSingle(bData, 0);
                                    myStruct_ROVPower_CtlSystem.I_First_B = data;

                                    bData[0] = readBuffer[12];
                                    bData[1] = readBuffer[11];
                                    bData[2] = readBuffer[14];
                                    bData[3] = readBuffer[13];
                                    data = BitConverter.ToSingle(bData, 0);
                                    myStruct_ROVPower_CtlSystem.I_First_C = data;

                                    bData[0] = readBuffer[16];
                                    bData[1] = readBuffer[15];
                                    bData[2] = readBuffer[18];
                                    bData[3] = readBuffer[17];
                                    data = BitConverter.ToSingle(bData, 0);
                                    myStruct_ROVPower_CtlSystem.V_First_AB = data;

                                    bData[0] = readBuffer[20];
                                    bData[1] = readBuffer[19];
                                    bData[2] = readBuffer[22];
                                    bData[3] = readBuffer[21];
                                    data = BitConverter.ToSingle(bData, 0);
                                    myStruct_ROVPower_CtlSystem.V_First_BC = data;

                                    bData[0] = readBuffer[24];
                                    bData[1] = readBuffer[23];
                                    bData[2] = readBuffer[26];
                                    bData[3] = readBuffer[25];
                                    data = BitConverter.ToSingle(bData, 0);
                                    myStruct_ROVPower_CtlSystem.I_First_S = data;
                                }
                                #endregion

                                #region 报警参数解析
                                else if (readBuffer[0] == 0x02 && readBuffer[1] == 0x03 && readBuffer[2] == 0x0A && readBuffer.Length >= 15)
                                {
                                    myStruct_ROVPower_CtlSystem.typeMidData = 2;

                                    byte[] bData = new byte[2];

                                    //Warnning WORD1	40100	WORD	VW90/高低反	VW1200
                                    bData[0] = readBuffer[3];
                                    bData[1] = readBuffer[4];
                                    UInt16 data = BitConverter.ToUInt16(bData, 0);
                                    myStruct_ROVPower_CtlSystem.ALARM_T1输入电压低报警 = (readBuffer[3] & (0x01 << 0)) == (0x01 << 0) ? true : false;
                                    myStruct_ROVPower_CtlSystem.ALARM_T1输入电压高报警 = (readBuffer[3] & (0x01 << 1)) == (0x01 << 1) ? true : false;
                                    myStruct_ROVPower_CtlSystem.ALARM_T1输出电压高报警 = (readBuffer[3] & (0x01 << 2)) == (0x01 << 2) ? true : false;
                                    myStruct_ROVPower_CtlSystem.ALARM_T1输入电流高报警 = (readBuffer[3] & (0x01 << 3)) == (0x01 << 3) ? true : false;
                                    myStruct_ROVPower_CtlSystem.ALARM_T1输出电流高报警 = (readBuffer[3] & (0x01 << 4)) == (0x01 << 4) ? true : false;

                                    myStruct_ROVPower_CtlSystem.ALARM_T2输入电压低报警 = (readBuffer[4] & (0x01 << 0)) == (0x01 << 0) ? true : false;
                                    myStruct_ROVPower_CtlSystem.ALARM_T2输入电压高报警 = (readBuffer[4] & (0x01 << 1)) == (0x01 << 1) ? true : false;
                                    myStruct_ROVPower_CtlSystem.ALARM_T2输出电压高报警 = (readBuffer[4] & (0x01 << 2)) == (0x01 << 2) ? true : false;
                                    myStruct_ROVPower_CtlSystem.ALARM_T2输入电流高报警 = (readBuffer[4] & (0x01 << 3)) == (0x01 << 3) ? true : false;
                                    myStruct_ROVPower_CtlSystem.ALARM_T2输出电流高报警 = (readBuffer[4] & (0x01 << 4)) == (0x01 << 4) ? true : false;

                                    //Warnning WORD2	40101	WORD	VW92/高低反	VW1202
                                    bData[0] = readBuffer[5];
                                    bData[1] = readBuffer[6];
                                    data = BitConverter.ToUInt16(bData, 0);

                                    //ALARM WORD1	40102	WORD	VW94/高低反	VW1204
                                    bData[0] = readBuffer[7];
                                    bData[1] = readBuffer[8];
                                    data = BitConverter.ToUInt16(bData, 0);

                                    myStruct_ROVPower_CtlSystem.ALARM_T1输入电流A相变送器故障 = (readBuffer[7] & (0x01 << 0)) == (0x01 << 0) ? true : false;
                                    myStruct_ROVPower_CtlSystem.ALARM_T1输入电流B相变送器故障 = (readBuffer[7] & (0x01 << 1)) == (0x01 << 1) ? true : false;
                                    myStruct_ROVPower_CtlSystem.ALARM_T1输入电流C相变送器故障 = (readBuffer[7] & (0x01 << 2)) == (0x01 << 2) ? true : false;
                                    myStruct_ROVPower_CtlSystem.ALARM_T1输入电压AB相变送器故障 = (readBuffer[7] & (0x01 << 3)) == (0x01 << 3) ? true : false;
                                    myStruct_ROVPower_CtlSystem.ALARM_T1输入电压BC相变送器故障 = (readBuffer[7] & (0x01 << 4)) == (0x01 << 4) ? true : false;
                                    myStruct_ROVPower_CtlSystem.ALARM_T2输入电流变送器故障 = (readBuffer[7] & (0x01 << 5)) == (0x01 << 5) ? true : false;
                                    myStruct_ROVPower_CtlSystem.ALARM_T1输出电流变送器故障 = (readBuffer[7] & (0x01 << 6)) == (0x01 << 6) ? true : false;
                                    myStruct_ROVPower_CtlSystem.ALARM_T2输出电流变送器故障 = (readBuffer[7] & (0x01 << 7)) == (0x01 << 7) ? true : false;
                                    myStruct_ROVPower_CtlSystem.ALARM_T1输出电压变送器故障 = (readBuffer[8] & (0x01 << 0)) == (0x01 << 0) ? true : false;
                                    myStruct_ROVPower_CtlSystem.ALARM_T2输出电压变送器故障 = (readBuffer[8] & (0x01 << 1)) == (0x01 << 1) ? true : false;
                                    myStruct_ROVPower_CtlSystem.ALARM_相序断相不平衡故障 = (readBuffer[8] & (0x01 << 2)) == (0x01 << 2) ? true : false;
                                    myStruct_ROVPower_CtlSystem.ALARM_T1_高压绝缘故障 = (readBuffer[8] & (0x01 << 3)) == (0x01 << 3) ? true : false;
                                    myStruct_ROVPower_CtlSystem.ALARM_T2_高压绝缘故障 = (readBuffer[8] & (0x01 << 4)) == (0x01 << 4) ? true : false;
                                    myStruct_ROVPower_CtlSystem.ALARM_急停故障 = (readBuffer[8] & (0x01 << 5)) == (0x01 << 5) ? true : false;

                                    //ALARM WORD2	40103	WORD	VW96/高低反	VW1206
                                    bData[0] = readBuffer[9];
                                    bData[1] = readBuffer[10];
                                    data = BitConverter.ToUInt16(bData, 0);

                                    myStruct_ROVPower_CtlSystem.ALARM_T1输入电压低低故障 = (readBuffer[9] & (0x01 << 0)) == (0x01 << 0) ? true : false;
                                    myStruct_ROVPower_CtlSystem.ALARM_T1输入电压高高故障 = (readBuffer[9] & (0x01 << 1)) == (0x01 << 1) ? true : false;
                                    myStruct_ROVPower_CtlSystem.ALARM_T1输出电压高高故障 = (readBuffer[9] & (0x01 << 2)) == (0x01 << 2) ? true : false;

                                    myStruct_ROVPower_CtlSystem.ALARM_T1输入电流高高故障 = (readBuffer[9] & (0x01 << 4)) == (0x01 << 4) ? true : false;
                                    myStruct_ROVPower_CtlSystem.ALARM_T1输出电流高高故障 = (readBuffer[9] & (0x01 << 5)) == (0x01 << 5) ? true : false;


                                    myStruct_ROVPower_CtlSystem.ALARM_T2输入电压低低故障 = (readBuffer[10] & (0x01 << 0)) == (0x01 << 0) ? true : false;
                                    myStruct_ROVPower_CtlSystem.ALARM_T2输入电压高高故障 = (readBuffer[10] & (0x01 << 1)) == (0x01 << 1) ? true : false;
                                    myStruct_ROVPower_CtlSystem.ALARM_T2输出电压高高故障 = (readBuffer[10] & (0x01 << 2)) == (0x01 << 2) ? true : false;

                                    myStruct_ROVPower_CtlSystem.ALARM_T2输入电流高高故障 = (readBuffer[10] & (0x01 << 4)) == (0x01 << 4) ? true : false;
                                    myStruct_ROVPower_CtlSystem.ALARM_T2输出电流高高故障 = (readBuffer[10] & (0x01 << 5)) == (0x01 << 5) ? true : false;

                                    //ALARM WORD3	40104	WORD	VW98/高低反	VW1208
                                    bData[0] = readBuffer[11];
                                    bData[1] = readBuffer[12];
                                    data = BitConverter.ToUInt16(bData, 0);
                                }
                                #endregion

                                myGEventArgs.dataType = 5;
                                myGEventArgs.connected = true;
                                myGEventArgs.obj = readBuffer;
                                myGEventArgs.objParse = myStruct_ROVPower_CtlSystem;
                                if (EventSerialDataSend != null)
                                {
                                    EventSerialDataSend(this, myGEventArgs);
                                }
                            }
                            else if (NameIn == "绝缘检测仪1" || NameIn == "绝缘检测仪2")
                            {
                                #region 解析


                                if (readBuffer.Length < 30)  //数据长度不足, LF CR 暂未计入
                                {
                                    continue;
                                }

                                if (readBuffer[0] != 0x02 || readBuffer[1] != 0x0F || readBuffer[8] != 0x0F || readBuffer[15] != 0x0F ||
                                    readBuffer[22] != 0x0F || readBuffer[24] != 0x0F || readBuffer[26] != 0x0F || readBuffer[28] != 0x0F ||
                                    readBuffer[29] != 0x03) // || readBuffer[30] != 0x10 || readBuffer[31] != 0x13 可能是 || readBuffer[30] != 0x0A || readBuffer[31] != 0x0D
                                {
                                    continue;
                                }

                                if (NameIn == "绝缘检测仪1")
                                {
                                    myGEventArgs.addressBoard = enum_AddressBoard.JueYuanJianCe_Board_1;
                                }
                                else if (NameIn == "绝缘检测仪2")
                                {
                                    myGEventArgs.addressBoard = enum_AddressBoard.JueYuanJianCe_Board_2;
                                }
                                else
                                {
                                    continue;
                                }

                                Struct_JueYuanJianCeYi myStruct_JueYuanJianCeYi = new Struct_JueYuanJianCeYi();

                                byte[] bCV = new byte[6];
                                Array.Copy(readBuffer, 2, bCV, 0, 6);
                                string sCV = Encoding.ASCII.GetString(bCV);
                                myStruct_JueYuanJianCeYi.sMeauringValue = sCV;

                                Array.Copy(readBuffer, 9, bCV, 0, 6);
                                sCV = Encoding.ASCII.GetString(bCV);
                                myStruct_JueYuanJianCeYi.sAlarm1Value = sCV;

                                Array.Copy(readBuffer, 16, bCV, 0, 6);
                                sCV = Encoding.ASCII.GetString(bCV);
                                myStruct_JueYuanJianCeYi.sAlarm2Value = sCV;

                                if (readBuffer[23] == 0x30)
                                {
                                    myStruct_JueYuanJianCeYi.sK1_K2_OnOff = "K1 off,K2 off";
                                }
                                else if (readBuffer[23] == 0x31)
                                {
                                    myStruct_JueYuanJianCeYi.sK1_K2_OnOff = "K1 on,K2 off";
                                }
                                else if (readBuffer[23] == 0x32)
                                {
                                    myStruct_JueYuanJianCeYi.sK1_K2_OnOff = "K1 off,K2 on";
                                }
                                else if (readBuffer[23] == 0x33)
                                {
                                    myStruct_JueYuanJianCeYi.sK1_K2_OnOff = "K1 on,K2 on";
                                }
                                else
                                {
                                    myStruct_JueYuanJianCeYi.sK1_K2_OnOff = "Error";
                                }

                                if (readBuffer[25] == 0x30)
                                {
                                    myStruct_JueYuanJianCeYi.sAlarm1_2_None = "No Alarm";
                                }
                                else if (readBuffer[25] == 0x31)
                                {
                                    myStruct_JueYuanJianCeYi.sAlarm1_2_None = "Alarm1";
                                }
                                else if (readBuffer[25] == 0x32)
                                {
                                    myStruct_JueYuanJianCeYi.sAlarm1_2_None = "Alarm2";
                                }
                                else if (readBuffer[25] == 0x33)
                                {
                                    myStruct_JueYuanJianCeYi.sAlarm1_2_None = "Alarm1/2";
                                }
                                else
                                {
                                    myStruct_JueYuanJianCeYi.sAlarm1_2_None = "Error";
                                }

                                if (readBuffer[27] == 0x30)
                                {
                                    myStruct_JueYuanJianCeYi.sAC_DC_Fault = "AC Fault";
                                }
                                else if (readBuffer[27] == 0x31)
                                {
                                    myStruct_JueYuanJianCeYi.sAC_DC_Fault = "DC- Fault";
                                }
                                else if (readBuffer[27] == 0x32)
                                {
                                    myStruct_JueYuanJianCeYi.sAC_DC_Fault = "DC+ Fault";
                                }
                                else
                                {
                                    myStruct_JueYuanJianCeYi.sAC_DC_Fault = "Error";
                                }

                                myGEventArgs.dataType = 5;
                                myGEventArgs.connected = true;
                                myGEventArgs.obj = readBuffer;
                                myGEventArgs.objParse = myStruct_JueYuanJianCeYi;
                                if (EventSerialDataSend != null)
                                {
                                    EventSerialDataSend(this, myGEventArgs);
                                }

                                #endregion

                            }
                        }
                    }
                    else
                    {
                        Thread.Sleep(5);
                        continue;
                    }
                }
                catch (Exception ex)
                { }
            }
        }

        public void Close()
        {
            try
            {
                stopSerialRecv = true;
                stopSerialSend = true;
                SerialClose();
            }
            catch (Exception ex)
            { }
        }

        public byte[] HexStringToBytes(string hexString)
        {
            int lenString = hexString.Length;
            if (lenString % 2 == 1)
            {
                hexString += "0";
            }
            lenString = hexString.Length / 2;
            byte[] data = new byte[lenString];
            for (int i = 0; i < lenString; i++)
            {
                try
                {
                    string s = hexString.Substring(i * 2, 2);
                    data[i] = Convert.ToByte(s, 16);
                }
                catch (Exception ex)
                { }
            }
            return data;
        }

        private bool SumCheck(byte[] data, int indexStart, int len, out byte byteCheck)
        {
            byteCheck = 0x00;
            try
            {
                if (data.Length < indexStart + len)
                {
                    return false;
                }
                uint iRes = 0;
                for (int i = indexStart; i < indexStart + len; i++)
                {
                    iRes += data[i];
                }
                byte[] bRes = BitConverter.GetBytes(iRes);
                byteCheck = bRes[0];
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        //串口发送数据
        public int SerialDataSend(byte[] buf)
        {
            try
            {
                if (mySerialPort != null && mySerialPort.IsOpen)
                {
                    if (buf != null && buf.Length > 0)
                    {
                        isCtlSending = true;
                        Thread.Sleep(Global.delay_CmdSend_Before);
                        mySerialPort.Write(buf, 0, buf.Length);
                        Thread.Sleep(Global.delay_CmdSend_After);
                        isCtlSending = false;
                        return 0;   //0-代表发送成功
                    }
                    else
                    {
                        return 3;   //3-发送数据缓存长度错误
                    }
                }
                else
                {
                    return 1;   //1-代表串口未开启
                }
            }
            catch (Exception ex)
            {
                return 2;   //2-代表处理异常
            }
        }


        public string[] SerialPortCheck()
        {
            return SerialPort.GetPortNames();
        }

        public void BCmdInsert(byte[] cmd)
        {
            try
            {
                if (cmd != null && cmd.Length > 0)
                {
                    if (mySerialPort != null && mySerialPort.IsOpen)
                    {
                        isCtlSending = true;
                        Thread.Sleep(100);
                        mySerialPort.Write(cmd, 0, cmd.Length);
                        Thread.Sleep(100);
                        isCtlSending = false;
                    }
                }
            }
            catch (Exception ex)
            { }
        }

        public bool SerialOpen()
        {
            try
            {
                if (mySerialPort.IsOpen)
                {
                    mySerialPort.Close();
                }
                mySerialPort.Open();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool SerialClose()
        {
            try
            {
                if (mySerialPort != null && mySerialPort.IsOpen)
                {
                    mySerialPort.Close();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }



    }
}
