using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;

namespace MonitorProj
{
    class BoardMonCtlClass
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
        
        //名称Name
        private string name;
        public string Name
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

        //绑定的本地IP地址
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

        //绑定的本地Port
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

        //绑定的远端IP地址
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

        //绑定的远端Port
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

        //网络连接状态
        private bool isConnected = false;
        public bool IsConnected
        {
            get
            {
                return isConnected;
            }
        }

        private Socket SerialSocket;
        private Thread SerialRecvThread;
        private Thread SerialSendThread;
        private bool stopSerialRecv = false;
        private bool stopSerialSend = false;

        public event EventHandler EventSerialDataSend;

        private bool isCtlSending = false;//是否要发送控制命令

        private Dictionary<string, StatusUnitParas> dicStatusParasByStatusNameLocal = new Dictionary<string, StatusUnitParas>();
        private List<string> listStatusParasKeys = new List<string>();

        #endregion


        public bool Initialize()
        {
            try
            {
                GEventArgs myGEventArgs = new GEventArgs();
                myGEventArgs.nameSerial = this.name;
                myGEventArgs.dataType = 1;

                listStatusParasKeys = Global.dicStatusParasByStatusName.Keys.ToList(); ;
                for (int i = 0; i < Global.dicStatusParasByStatusName.Count; i++)
                {
                    dicStatusParasByStatusNameLocal.Add(listStatusParasKeys[i], Global.dicStatusParasByStatusName[listStatusParasKeys[i]]);
                }

                isConnected = Global.connectSerialTCPIP(addressLocal, portLocal, addressRemote, portRemote, out SerialSocket, out infoRunning);
                if (isConnected)
                {
                    myGEventArgs.connected = true;
                    if (EventSerialDataSend != null)
                    {
                        EventSerialDataSend(this, myGEventArgs);
                    }

                    stopSerialRecv = false;
                    stopSerialSend = false;

                    SerialRecvThread = new Thread(new ThreadStart(SerialRecvThreadProcess));
                    //SerialRecvThread.Name = name + "-SerialRecvThread";
                    SerialRecvThread.Start();

                    SerialSendThread = new Thread(new ThreadStart(SerialSendThreadProcess));
                    //SerialSendThread.Name = name + "-SerialSendThread";
                    SerialSendThread.Start();

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
                stopSerialRecv = true;
                stopSerialSend = true;

                if (SerialSocket != null)   // && SerialSocket.Connected
                {
                    try
                    {
                        SerialSocket.Dispose();
                        SerialSocket = null;
                        isConnected = false;
                    }
                    catch (Exception ee)
                    { }
                }
                if (SerialRecvThread != null && SerialRecvThread.IsAlive)
                {
                    SerialRecvThread.Abort();
                }
                if (SerialSendThread != null && SerialSendThread.IsAlive)
                {
                    SerialSendThread.Abort();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }



        private void SerialRecvThreadProcess()
        {
            byte[] dataRecv = new byte[4096];
            GEventArgs myGEventArgs = new GEventArgs();
            myGEventArgs.nameSerial = name;
            myGEventArgs.serialCOMType = mySerialCOMType;
            myGEventArgs.addressBoard = enum_AddressBoard.Others;
            string sDataLast = "";

            while (stopSerialRecv == false)
            {
                try
                {
                    if (isConnected)
                    {
                        /*          //by lqy 20190307
                        int iLen = SerialSocket.Available;
                        if (iLen <= 0)
                        {
                            Thread.Sleep(10);
                            continue;
                        }

                        int len = SerialSocket.Receive(dataRecv);
                        */

                        //by lqy 20190307，当与远端主机断开或异常情况下具备不重启软件重新连接功能/////////
                        int len = SerialSocket.Available;
                        if (len <= 1)
                        {
                            Thread.Sleep(2);
                            continue;
                        }

                        try
                        {
                            len = SerialSocket.Receive(dataRecv);
                        }
                        catch (Exception ex)
                        {
                            string ss = ex.Message.ToString();
                            if (ss == "远程主机强迫关闭了一个现有的连接。")
                            {
                                isConnected = false;
                                SerialSocket.Close();
                                SerialSocket.Dispose();
                                isConnected = Global.connectSerialTCPIP(addressLocal, portLocal, addressRemote, portRemote, out SerialSocket, out infoRunning);
                                isConnected = true;
                                continue;
                            }
                            continue;
                        }
                        //////////////////////////////////////////////////////////////////

                        if (len <= 0)
                        {
                            isConnected = false;
                            myGEventArgs.dataType = 1;
                            myGEventArgs.connected = false;
                            if (EventSerialDataSend != null)
                            {
                                EventSerialDataSend(this, myGEventArgs);
                            }
                        }
                        else
                        {
                            byte[] bb = new byte[len];
                            Array.Copy(dataRecv, 0, bb, 0, len);

                            //myGEventArgs.dataType = 2;
                            //myGEventArgs.connected = true;
                            //myGEventArgs.obj = bb;
                            //if (EventSerialDataSend != null)
                            //{
                            //    EventSerialDataSend(this, myGEventArgs);
                            //}

                            //解析
                            #region 解析

                            #region 通信协议
                            /*
                            帧头	    板卡地址    指令类型	长度	数据	校验	帧尾
                            FF FF A5	ADDRESS     TYPE        LENGTH	DATA	PP	    26
                            3字节	    1字节       1字节       1字节	0~n	    1字节	1字节

                            帧头：固定格式，FFFFA5。
                            板卡地址：每个板卡有系统内唯一地址。
                            指令类型：（目前只有以下三种类型指令）：
                            0：自检指令，反馈指令将指令类型最高位置1（即0x80），其他不变，然后返回，此指令一般不用。
	                            1：查询指令，反馈指令将指令类型最高位置1（即0x81）并携带相关数据返回；若板块有采集功能则返回相应数据，数据含义根据办卡自身定义解析。
	                            2：执行指令，反馈指令将指令类型最高位置1（即0x82），反馈根据板卡特性带数据或不带数据。
                            长度：从板卡地址开始到帧尾（含地址和帧尾）的字节数。
                            数据：如果指令或者反馈有数据则此处包含0-n个字节的数据。
                            校验：从地址开始到校验前一个字节的简单和，取低8位。
                            帧尾：固定为0x26.
                            */
                            #endregion

                            #region 数据协议格式校验

                            if (bb.Length < 3)  //数据长度不足
                            {
                                continue;
                            }

                            if (bb[0] == 0xFF && bb[1] == 0xFF && bb[2] == 0xA5)    //检验同步头
                            {
                                byte bAddrRecv = bb[3];
                                byte bType = bb[4];//指令类型:0x80-自检指令返回；0x81-查询指令返回；0x82-执行指令返回
                                byte bDataLen = bb[5];
                                if (len < (bDataLen + 3))  //数据长度符合性检查
                                {
                                    continue;
                                }
                                byte byteCheck = 0;
                                bool bCheck = SumCheck(bb, 3, bDataLen - 2, out byteCheck);
                                if (bCheck == false || byteCheck != bb[3 + bDataLen - 2])   //校验错误
                                {
                                    continue;
                                }
                                if (bb[3 + bDataLen - 1] != 0x26)   //帧尾校验
                                {
                                    continue;
                                }

                            #endregion

                                enum_AddressBoard myBoardAddress = (enum_AddressBoard)bAddrRecv;
                                myGEventArgs.addressBoard = myBoardAddress;

                                switch (myBoardAddress)
                                {

                                    #region 直流绝缘检测板数据协议：（0x61、0x62、0x63）

                                    //0x61,直流绝缘检测板#1
                                    case enum_AddressBoard.DC_Insulation_Detection_Board_I:
                                    //0x62，直流绝缘检测板#2
                                    case enum_AddressBoard.DC_Insulation_Detection_Board_II:
                                    //0x63,直流绝缘检测板#3
                                    case enum_AddressBoard.DC_Insulation_Detection_Board_III:

                                        {
                                            if (bType == 0x80)
                                            { }
                                            else if (bType == 0x82)
                                            {
                                                Struct_DC_Insulation_Detection_Board myStruct_DC_Insulation_Detection_Board = new Struct_DC_Insulation_Detection_Board();
                                                myStruct_DC_Insulation_Detection_Board.type = 0x82;
                                                myStruct_DC_Insulation_Detection_Board.Address = bAddrRecv;
                                                myStruct_DC_Insulation_Detection_Board.bCmdReturn = bb;
                                                myGEventArgs.dataType = 6;
                                                myGEventArgs.connected = true;
                                                myGEventArgs.obj = bb;
                                                myGEventArgs.objParse = myStruct_DC_Insulation_Detection_Board;
                                                if (EventSerialDataSend != null)
                                                {
                                                    EventSerialDataSend(this, myGEventArgs);
                                                }
                                            }
                                            else if (bType == 0x81)
                                            {

                                                if (bb.Length < 26)
                                                {
                                                    continue;
                                                }
                                                Struct_DC_Insulation_Detection_Board myStruct_DC_Insulation_Detection_Board = new Struct_DC_Insulation_Detection_Board();
                                                myStruct_DC_Insulation_Detection_Board.type = 0x81;
                                                myStruct_DC_Insulation_Detection_Board.Address = bAddrRecv;
                                                byte[] bData = new byte[2];
                                                bData[1] = bb[6];
                                                bData[0] = bb[7];
                                                UInt16 data = BitConverter.ToUInt16(bData, 0);
                                                myStruct_DC_Insulation_Detection_Board.VA = GetVPhyValueForGFC(data * 3.3 / 4096);

                                                bData[1] = bb[8];
                                                bData[0] = bb[9];
                                                data = BitConverter.ToUInt16(bData, 0);
                                                myStruct_DC_Insulation_Detection_Board.IA = GetIPhyValueForGFC(data * 3.3 / 4096);

                                                bData[1] = bb[10];
                                                bData[0] = bb[11];
                                                data = BitConverter.ToUInt16(bData, 0);
                                                myStruct_DC_Insulation_Detection_Board.GA = GetILeakPhyValueForGFC(data * 3.3 / 4096);

                                                bData[1] = bb[12];
                                                bData[0] = bb[13];
                                                data = BitConverter.ToUInt16(bData, 0);
                                                myStruct_DC_Insulation_Detection_Board.TA = GetTPhyValueForGFC(data * 3.3 / 4096);

                                                bData[1] = bb[14];
                                                bData[0] = bb[15];
                                                data = BitConverter.ToUInt16(bData, 0);
                                                myStruct_DC_Insulation_Detection_Board.VB = GetVPhyValueForGFC(data * 3.3 / 4096);

                                                bData[1] = bb[16];
                                                bData[0] = bb[17];
                                                data = BitConverter.ToUInt16(bData, 0);
                                                myStruct_DC_Insulation_Detection_Board.IB = GetIPhyValueForGFC(data * 3.3 / 4096);

                                                bData[1] = bb[18];
                                                bData[0] = bb[19];
                                                data = BitConverter.ToUInt16(bData, 0);
                                                myStruct_DC_Insulation_Detection_Board.GB = GetILeakPhyValueForGFC(data * 3.3 / 4096);

                                                bData[1] = bb[20];
                                                bData[0] = bb[21];
                                                data = BitConverter.ToUInt16(bData, 0);
                                                myStruct_DC_Insulation_Detection_Board.TB = GetTPhyValueForGFC(data * 3.3 / 4096);

                                                bData[1] = bb[22];
                                                bData[0] = bb[23];
                                                data = BitConverter.ToUInt16(bData, 0);
                                                myStruct_DC_Insulation_Detection_Board.TO = GetTPhyValueForGFC(data * 3.3 / 4096);

                                                myGEventArgs.dataType = 5;
                                                myGEventArgs.connected = true;
                                                myGEventArgs.obj = bb;
                                                myGEventArgs.objParse = myStruct_DC_Insulation_Detection_Board;
                                                if (EventSerialDataSend != null)
                                                {
                                                    EventSerialDataSend(this, myGEventArgs);
                                                }
                                            }
                                            else
                                            {
                                                continue;
                                            }

                                        }
                                        
                                        break;

                                    #endregion

                                    #region 大功率直流绝缘检测板数据解析：（0x70、0x71、0x72）
                                    //0x70,大功率直流绝缘检测板#1
                                    case enum_AddressBoard.High_Power_DC_Insulation_Detection_Board_I:
                                    //0x71,大功率直流绝缘检测板#2
                                    case enum_AddressBoard.High_Power_DC_Insulation_Detection_Board_II:
                                    //0x72,大功率直流绝缘检测板#3
                                    case enum_AddressBoard.High_Power_DC_Insulation_Detection_Board_III:

                                        {
                                            if (bType == 0x80)
                                            { }
                                            else if (bType == 0x82)
                                            {
                                                Struct_High_Power_DC_Insulation_Detection_Board myStruct_High_Power_DC_Insulation_Detection_Board =
                                                    new Struct_High_Power_DC_Insulation_Detection_Board();
                                                myStruct_High_Power_DC_Insulation_Detection_Board.type = 0x82;
                                                myStruct_High_Power_DC_Insulation_Detection_Board.Address = bAddrRecv;
                                                myStruct_High_Power_DC_Insulation_Detection_Board.bCmdReturn = bb;
                                                myGEventArgs.dataType = 6;
                                                myGEventArgs.connected = true;
                                                myGEventArgs.obj = bb;
                                                myGEventArgs.objParse = myStruct_High_Power_DC_Insulation_Detection_Board;
                                                if (EventSerialDataSend != null)
                                                {
                                                    EventSerialDataSend(this, myGEventArgs);
                                                }
                                            }
                                            else if (bType == 0x81)
                                            {
                                                if (bb.Length < 16)
                                                {
                                                    continue;
                                                }

                                                Struct_High_Power_DC_Insulation_Detection_Board myStruct_High_Power_DC_Insulation_Detection_Board = new Struct_High_Power_DC_Insulation_Detection_Board();
                                                myStruct_High_Power_DC_Insulation_Detection_Board.type = 0x81;
                                                myStruct_High_Power_DC_Insulation_Detection_Board.Address = bAddrRecv;
                                                byte[] bData = new byte[2];
                                                bData[1] = bb[6];
                                                bData[0] = bb[7];
                                                UInt16 data = BitConverter.ToUInt16(bData, 0);
                                                myStruct_High_Power_DC_Insulation_Detection_Board.V = GetVPhyValueForBigGFC(data * 3.3 / 4096);

                                                bData[1] = bb[8];
                                                bData[0] = bb[9];
                                                data = BitConverter.ToUInt16(bData, 0);
                                                myStruct_High_Power_DC_Insulation_Detection_Board.I = GetIPhyValueForBigGFC(data * 3.3 / 4096);

                                                bData[1] = bb[10];
                                                bData[0] = bb[11];
                                                data = BitConverter.ToUInt16(bData, 0);
                                                myStruct_High_Power_DC_Insulation_Detection_Board.G = GetILeakPhyValueForBigGFC(data * 3.3 / 4096);

                                                bData[1] = bb[12];
                                                bData[0] = bb[13];
                                                data = BitConverter.ToUInt16(bData, 0);
                                                myStruct_High_Power_DC_Insulation_Detection_Board.T = GetTPhyValueForBigGFC(data * 3.3 / 4096);

                                                myGEventArgs.dataType = 5;
                                                myGEventArgs.connected = true;
                                                myGEventArgs.obj = bb;
                                                myGEventArgs.objParse = myStruct_High_Power_DC_Insulation_Detection_Board;
                                                if (EventSerialDataSend != null)
                                                {
                                                    EventSerialDataSend(this, myGEventArgs);
                                                }
                                            }
                                            else
                                            {
                                                continue;
                                            }
                                        }

                                        break;

                                    #endregion

                                    #region 大功率直流绝缘检测板控制板数据解析：（0x79）
                                    //0x79,大功率直流绝缘检测板控制板
                                    case enum_AddressBoard.Control_Panel_High_Power_DC_Insulation_Detection_Board:

                                        {
                                            if (bType == 0x80)
                                            { }
                                            else if (bType == 0x82)
                                            {
                                                Struct_Control_Panel_High_Power_DC_Insulation_Detection_Board myStruct_Control_Panel_High_Power_DC_Insulation_Detection_Board =
                                                    new Struct_Control_Panel_High_Power_DC_Insulation_Detection_Board();
                                                myStruct_Control_Panel_High_Power_DC_Insulation_Detection_Board.type = 0x82;
                                                myStruct_Control_Panel_High_Power_DC_Insulation_Detection_Board.Address = bAddrRecv;
                                                myStruct_Control_Panel_High_Power_DC_Insulation_Detection_Board.bCmdReturn = bb;
                                                myGEventArgs.dataType = 6;
                                                myGEventArgs.connected = true;
                                                myGEventArgs.obj = bb;
                                                myGEventArgs.objParse = myStruct_Control_Panel_High_Power_DC_Insulation_Detection_Board;
                                                if (EventSerialDataSend != null)
                                                {
                                                    EventSerialDataSend(this, myGEventArgs);
                                                }
                                            }
                                            else if (bType == 0x81)
                                            {
                                                if (bb.Length < 12)
                                                {
                                                    continue;
                                                }

                                                Struct_Control_Panel_High_Power_DC_Insulation_Detection_Board myStruct_Control_Panel_High_Power_DC_Insulation_Detection_Board =
                                                    new Struct_Control_Panel_High_Power_DC_Insulation_Detection_Board();
                                                myStruct_Control_Panel_High_Power_DC_Insulation_Detection_Board.type = 0x81;
                                                myStruct_Control_Panel_High_Power_DC_Insulation_Detection_Board.Address = bAddrRecv;
                                                byte[] bData = new byte[2];
                                                bData[1] = bb[6];
                                                bData[0] = bb[7];
                                                UInt16 data = BitConverter.ToUInt16(bData, 0);
                                                myStruct_Control_Panel_High_Power_DC_Insulation_Detection_Board.V = data * 3.3 / 4096;

                                                bData[1] = bb[8];
                                                bData[0] = bb[9];
                                                data = BitConverter.ToUInt16(bData, 0);
                                                myStruct_Control_Panel_High_Power_DC_Insulation_Detection_Board.I = data * 3.3 / 4096;

                                                myGEventArgs.dataType = 5;
                                                myGEventArgs.connected = true;
                                                myGEventArgs.obj = bb;
                                                myGEventArgs.objParse = myStruct_Control_Panel_High_Power_DC_Insulation_Detection_Board;
                                                if (EventSerialDataSend != null)
                                                {
                                                    EventSerialDataSend(this, myGEventArgs);
                                                }
                                            }
                                            else
                                            {
                                                continue;
                                            }
                                        }

                                        break;

                                    #endregion

                                    #region 交流绝缘监测板：(0x80)
                                    //0x80,交流绝缘检测板
                                    case enum_AddressBoard.AC_Insulation_Detection_Board:

                                        {
                                            if (bType == 0x80)
                                            { }
                                            else if (bType == 0x82)
                                            {
                                                Struct_AC_Insulation_Detection_Board myStruct_AC_Insulation_Detection_Board =
                                                    new Struct_AC_Insulation_Detection_Board();
                                                myStruct_AC_Insulation_Detection_Board.type = 0x82;
                                                myStruct_AC_Insulation_Detection_Board.Address = bAddrRecv;
                                                myStruct_AC_Insulation_Detection_Board.bCmdReturn = bb;
                                                myGEventArgs.dataType = 6;
                                                myGEventArgs.connected = true;
                                                myGEventArgs.obj = bb;
                                                myGEventArgs.objParse = myStruct_AC_Insulation_Detection_Board;
                                                if (EventSerialDataSend != null)
                                                {
                                                    EventSerialDataSend(this, myGEventArgs);
                                                }
                                            }
                                            else if (bType == 0x81)
                                            {
                                                if (bb.Length < 12)
                                                {
                                                    continue;
                                                }

                                                Struct_AC_Insulation_Detection_Board myStruct_AC_Insulation_Detection_Board =
                                                    new Struct_AC_Insulation_Detection_Board();
                                                myStruct_AC_Insulation_Detection_Board.type = 0x81;
                                                myStruct_AC_Insulation_Detection_Board.Address = bAddrRecv;
                                                byte[] bData = new byte[2];
                                                bData[1] = bb[6];
                                                bData[0] = bb[7];
                                                UInt16 data = BitConverter.ToUInt16(bData, 0);
                                                myStruct_AC_Insulation_Detection_Board.V = GetPhyValueForACGFM(data * 3.3 / 4096);

                                                bData[1] = bb[8];
                                                bData[0] = bb[9];
                                                data = BitConverter.ToUInt16(bData, 0);
                                                myStruct_AC_Insulation_Detection_Board.I = GetPhyValueForACGFM(data * 3.3 / 4096);

                                                myGEventArgs.dataType = 5;
                                                myGEventArgs.connected = true;
                                                myGEventArgs.obj = bb;
                                                myGEventArgs.objParse = myStruct_AC_Insulation_Detection_Board;
                                                if (EventSerialDataSend != null)
                                                {
                                                    EventSerialDataSend(this, myGEventArgs);
                                                }
                                            }
                                            else
                                            {
                                                continue;
                                            }
                                        }

                                        break;

                                    #endregion

                                    #region 舱内备用电源继电器板（0x90）
                                    //0x90,舱内备用电源继电器板
                                    case enum_AddressBoard.Inboard_Backup_Power_Relay_Board:

                                        {
                                            if (bType == 0x80)
                                            { }
                                            else if (bType == 0x82)
                                            {
                                                Struct_Inboard_Backup_Power_Relay_Board myStruct_Inboard_Backup_Power_Relay_Board =
                                                    new Struct_Inboard_Backup_Power_Relay_Board();
                                                myStruct_Inboard_Backup_Power_Relay_Board.type = 0x82;
                                                myStruct_Inboard_Backup_Power_Relay_Board.Address = bAddrRecv;
                                                myStruct_Inboard_Backup_Power_Relay_Board.bCmdReturn = bb;
                                                myGEventArgs.dataType = 6;
                                                myGEventArgs.connected = true;
                                                myGEventArgs.obj = bb;
                                                myGEventArgs.objParse = myStruct_Inboard_Backup_Power_Relay_Board;
                                                if (EventSerialDataSend != null)
                                                {
                                                    EventSerialDataSend(this, myGEventArgs);
                                                }
                                            }
                                            else if (bType == 0x81)
                                            {
                                                
                                            }
                                            else
                                            {
                                                continue;
                                            }
                                        }

                                        break;

                                    #endregion

                                    #region 传感器接口箱信号采集板：0x40
                                    //0x40,油箱采集板#1,(传感器接口箱信号采集板)
                                    case enum_AddressBoard.Tank_Detection_Board:

                                        {
                                            if (bType == 0x80)
                                            { }
                                            else if (bType == 0x82)
                                            {
                                                Struct_Tank_Detection_Board myStruct_Tank_Detection_Board =
                                                    new Struct_Tank_Detection_Board();
                                                myStruct_Tank_Detection_Board.type = 0x82;
                                                myStruct_Tank_Detection_Board.Address = bAddrRecv;
                                                myStruct_Tank_Detection_Board.bCmdReturn = bb;
                                                myGEventArgs.dataType = 6;
                                                myGEventArgs.connected = true;
                                                myGEventArgs.obj = bb;
                                                myGEventArgs.objParse = myStruct_Tank_Detection_Board;
                                                if (EventSerialDataSend != null)
                                                {
                                                    EventSerialDataSend(this, myGEventArgs);
                                                }
                                            }
                                            else if (bType == 0x81)
                                            {
                                                if (bb.Length < 40)
                                                {
                                                    continue;
                                                }

                                                Struct_Tank_Detection_Board myStruct_Tank_Detection_Board =
                                                    new Struct_Tank_Detection_Board();
                                                myStruct_Tank_Detection_Board.type = 0x81;
                                                myStruct_Tank_Detection_Board.Address = bAddrRecv;
                                                byte[] bData = new byte[2];
                                                bData[1] = bb[6];
                                                bData[0] = bb[7];
                                                UInt16 data = BitConverter.ToUInt16(bData, 0);
                                                double Vo = data * 3.3 / 4096;
                                                double Vi = 6.16448 - 2.01 * Vo;
                                                myStruct_Tank_Detection_Board.CH1_1 = (Vi - 1) * (35.0 - 0) / (5 - 1) + 0;    //压力1,0-35MPa对应Vi的1V-5V;

                                                bData[1] = bb[8];
                                                bData[0] = bb[9];
                                                data = BitConverter.ToUInt16(bData, 0);
                                                Vo = data * 3.3 / 4096;
                                                Vi = 6.16448 - 2.01 * Vo;
                                                myStruct_Tank_Detection_Board.CH1_2 = (Vi - 1) * (35.0 - 0) / (5 - 1) + 0;    //压力2,0-35MPa对应Vi的1V-5V;
                                                
                                                bData[1] = bb[10];
                                                bData[0] = bb[11];
                                                data = BitConverter.ToUInt16(bData, 0);
                                                Vo = data * 3.3 / 4096;
                                                Vi = 6.16448 - 2.01 * Vo;
                                                myStruct_Tank_Detection_Board.CH1_3 = (Vi - 1) * (35.0 - 0) / (5 - 1) + 0;    //压力3,0-35MPa对应Vi的1V-5V;

                                                bData[1] = bb[12];
                                                bData[0] = bb[13];
                                                data = BitConverter.ToUInt16(bData, 0);
                                                Vo = data * 3.3 / 4096;
                                                Vi = 6.16448 - 2.01 * Vo;
                                                myStruct_Tank_Detection_Board.CH1_4 = (Vi - 1) * (35.0 - 0) / (5 - 1) + 0;    //压力4,0-35MPa对应Vi的1V-5V;

                                                bData[1] = bb[14];
                                                bData[0] = bb[15];
                                                data = BitConverter.ToUInt16(bData, 0);
                                                Vo = data * 3.3 / 4096;
                                                Vi = 6.16448 - 2.01 * Vo;
                                                myStruct_Tank_Detection_Board.CH2_1 = (Vi - 0) * (10.0 - 0) / (3.8 - 0) + 0;    //油箱补偿,0L-10L对应Vi的0V-3.8V;

                                                bData[1] = bb[16];
                                                bData[0] = bb[17];
                                                data = BitConverter.ToUInt16(bData, 0);
                                                Vo = data * 3.3 / 4096;
                                                Vi = 6.16448 - 2.01 * Vo;
                                                myStruct_Tank_Detection_Board.CH2_2 = (Vi - 0) * (10.0 - 0) / (3.8 - 0) + 0;    //油箱补偿,0L-10L对应Vi的0V-3.8V;

                                                bData[1] = bb[18];
                                                bData[0] = bb[19];
                                                data = BitConverter.ToUInt16(bData, 0);
                                                Vo = data * 3.3 / 4096;
                                                Vi = 6.16448 - 2.01 * Vo;
                                                myStruct_Tank_Detection_Board.CH2_3 = Vi;

                                                bData[1] = bb[20];
                                                bData[0] = bb[21];
                                                data = BitConverter.ToUInt16(bData, 0);
                                                Vo = data * 3.3 / 4096;
                                                Vi = 6.16448 - 2.01 * Vo;
                                                myStruct_Tank_Detection_Board.CH2_4 = Vi;

                                                bData[1] = bb[22];
                                                bData[0] = bb[23];
                                                data = BitConverter.ToUInt16(bData, 0);
                                                Vo = data * 3.3 / 4096;
                                                Vi = 6.16448 - 2.01 * Vo;
                                                myStruct_Tank_Detection_Board.CH3_1 = Vi;

                                                bData[1] = bb[24];
                                                bData[0] = bb[25];
                                                data = BitConverter.ToUInt16(bData, 0);
                                                Vo = data * 3.3 / 4096;
                                                Vi = 6.16448 - 2.01 * Vo;
                                                //myStruct_Tank_Detection_Board.CH3_2 = (Vi - 1) * (4000.0 - 0) / (5 - 1) + 0;    //深度计,0-4000m对应4mA-20mA再对应Vi的1V-5V;
                                                //(Vi - 1) * (4000.0 - 0) / (5 - 1) = 92
                                                //Vi - 1 = 0.092
                                                //Vi = 1.092
                                                myStruct_Tank_Detection_Board.CH3_2 = (Vi - 1.092) * (7000.0 - 0) / (5 - 1.092) + 0;    //深度计,0-7000m对应4mA-20mA再对应Vi的1.092V-5V;

                                                bData[1] = bb[26];
                                                bData[0] = bb[27];
                                                data = BitConverter.ToUInt16(bData, 0);
                                                Vo = data * 3.3 / 4096;
                                                Vi = 6.16448 - 2.01 * Vo;
                                                myStruct_Tank_Detection_Board.CH3_3 = Vi;
                                                myStruct_Tank_Detection_Board.CH3_3 = (Vi - 1) * (2.5 - 0) / (5 - 1) + 0;    //补偿1,0L-2.5L对应Vi的1V-5V;

                                                bData[1] = bb[28];
                                                bData[0] = bb[29];
                                                data = BitConverter.ToUInt16(bData, 0);
                                                Vo = data * 3.3 / 4096;
                                                Vi = 6.16448 - 2.01 * Vo;
                                                myStruct_Tank_Detection_Board.CH3_4 = Vi;
                                                myStruct_Tank_Detection_Board.CH3_4 = (Vi - 1) * (2.5 - 0) / (5 - 1) + 0;    //补偿2,0L-2.5L对应Vi的1V-5V;

                                                bData[1] = bb[30];
                                                bData[0] = bb[31];
                                                data = BitConverter.ToUInt16(bData, 0);
                                                Vo = data * 3.3 / 4096;
                                                Vi = 6.16448 - 2.01 * Vo;
                                                myStruct_Tank_Detection_Board.CH4_1 = Vi;
                                                myStruct_Tank_Detection_Board.CH4_1 = (Vi - 1) * (2.5 - 0) / (5 - 1) + 0;    //补偿3,0L-2.5L对应Vi的1V-5V;

                                                bData[1] = bb[32];
                                                bData[0] = bb[33];
                                                data = BitConverter.ToUInt16(bData, 0);
                                                Vo = data * 3.3 / 4096;
                                                Vi = 6.16448 - 2.01 * Vo;
                                                myStruct_Tank_Detection_Board.CH4_2 = Vi;
                                                myStruct_Tank_Detection_Board.CH4_2 = (Vi - 1) * (2.5 - 0) / (5 - 1) + 0;    //补偿4,0L-2.5L对应Vi的1V-5V;

                                                bData[1] = bb[34];
                                                bData[0] = bb[35];
                                                data = BitConverter.ToUInt16(bData, 0);
                                                Vo = data * 3.3 / 4096;
                                                Vi = 6.16448 - 2.01 * Vo;
                                                myStruct_Tank_Detection_Board.CH4_3 = Vi;

                                                bData[1] = bb[36];
                                                bData[0] = bb[37];
                                                data = BitConverter.ToUInt16(bData, 0);
                                                Vo = data * 3.3 / 4096;
                                                Vi = 6.16448 - 2.01 * Vo;
                                                myStruct_Tank_Detection_Board.CH4_4 = (Vi - 5) * (1800.0 - 0) / (1 - 5) + 0;    //位移传感器,0mm-1800mm对应20mA-4mA再对应Vi的5V-1V;
                                                
                                                myGEventArgs.dataType = 5;
                                                myGEventArgs.connected = true;
                                                myGEventArgs.obj = bb;
                                                myGEventArgs.objParse = myStruct_Tank_Detection_Board;
                                                if (EventSerialDataSend != null)
                                                {
                                                    EventSerialDataSend(this, myGEventArgs);
                                                }
                                            }
                                            else
                                            {
                                                continue;
                                            }
                                        }

                                        break;

                                    #endregion

                                    #region 接口箱继电器板（0x25、0x26、0x28、0x29）
                                    //0x25,摄像机电源继电器板#1
                                    case enum_AddressBoard.Camera_Power_Relay_Board_I:
                                    //0x26,摄像机电源继电器板#2
                                    case enum_AddressBoard.Camera_Power_Relay_Board_II:
                                    //0x28,传感器电源继电器板#1
                                    case enum_AddressBoard.Sensor_Power_Relay_Board_I:
                                    //0x29,传感器电源继电器板#2
                                    case enum_AddressBoard.Sensor_Power_Relay_Board_II:

                                        {
                                            if (bType == 0x80)
                                            { }
                                            else if (bType == 0x82)
                                            {
                                                Struct_Power_Relay_Board myStruct_Power_Relay_Board =
                                                    new Struct_Power_Relay_Board();
                                                myStruct_Power_Relay_Board.type = 0x82;
                                                myStruct_Power_Relay_Board.Address = bAddrRecv;
                                                myStruct_Power_Relay_Board.bCmdReturn = bb;
                                                myGEventArgs.dataType = 6;
                                                myGEventArgs.connected = true;
                                                myGEventArgs.obj = bb;
                                                myGEventArgs.objParse = myStruct_Power_Relay_Board;
                                                if (EventSerialDataSend != null)
                                                {
                                                    EventSerialDataSend(this, myGEventArgs);
                                                }
                                            }
                                            else if (bType == 0x81)
                                            {
                                                if (bb.Length < 10)
                                                {
                                                    continue;
                                                }

                                                Struct_Power_Relay_Board myStruct_Power_Relay_Board = new Struct_Power_Relay_Board();
                                                myStruct_Power_Relay_Board.type = 0x81;
                                                myStruct_Power_Relay_Board.Address = bAddrRecv;
                                                byte[] bData = new byte[2];
                                                bData[1] = bb[6];
                                                bData[0] = bb[7];
                                                UInt16 data = BitConverter.ToUInt16(bData, 0);
                                                myStruct_Power_Relay_Board.V = data * 3.3 / 4096;

                                                myGEventArgs.dataType = 5;
                                                myGEventArgs.connected = true;
                                                myGEventArgs.obj = bb;
                                                myGEventArgs.objParse = myStruct_Power_Relay_Board;
                                                if (EventSerialDataSend != null)
                                                {
                                                    EventSerialDataSend(this, myGEventArgs);
                                                }
                                            }
                                            else
                                            {
                                                continue;
                                            }
                                        }

                                        break;

                                    #endregion

                                    #region 灯舱继电器板（0x21、0x22）
                                    //0x21,灯继电器板#1(灯舱继电器板)
                                    case enum_AddressBoard.Light_Relay_Board_I:
                                    //0x22,灯继电器板#2(灯舱继电器板)
                                    case enum_AddressBoard.Light_Relay_Board_II:

                                        {
                                            if (bType == 0x80)
                                            { }
                                            else if (bType == 0x82)
                                            {
                                                Struct_Light_Relay_Board myStruct_Light_Relay_Board =
                                                    new Struct_Light_Relay_Board();
                                                myStruct_Light_Relay_Board.type = 0x82;
                                                myStruct_Light_Relay_Board.Address = bAddrRecv;
                                                myStruct_Light_Relay_Board.bCmdReturn = bb;
                                                myGEventArgs.dataType = 6;
                                                myGEventArgs.connected = true;
                                                myGEventArgs.obj = bb;
                                                myGEventArgs.objParse = myStruct_Light_Relay_Board;
                                                if (EventSerialDataSend != null)
                                                {
                                                    EventSerialDataSend(this, myGEventArgs);
                                                }
                                            }
                                            else if (bType == 0x81)
                                            {
                                                if (bb.Length < 10)
                                                {
                                                    continue;
                                                }

                                                Struct_Light_Relay_Board myStruct_Light_Relay_Board = new Struct_Light_Relay_Board();
                                                myStruct_Light_Relay_Board.type = 0x81;
                                                myStruct_Light_Relay_Board.Address = bAddrRecv;
                                                byte[] bData = new byte[2];
                                                bData[1] = bb[6];
                                                bData[0] = bb[7];
                                                UInt16 data = BitConverter.ToUInt16(bData, 0);
                                                myStruct_Light_Relay_Board.V = data * 3.3 / 4096;

                                                myGEventArgs.dataType = 5;
                                                myGEventArgs.connected = true;
                                                myGEventArgs.obj = bb;
                                                myGEventArgs.objParse = myStruct_Light_Relay_Board;
                                                if (EventSerialDataSend != null)
                                                {
                                                    EventSerialDataSend(this, myGEventArgs);
                                                }
                                            }
                                            else
                                            {
                                                continue;
                                            }
                                        }

                                        break;
                                    #endregion

                                    #region 灯舱控制板（0x23、0x24)
                                    //0x23,灯控制板#1(灯舱控制板)
                                    case enum_AddressBoard.Light_Control_Panel_I:
                                    //0x24,灯控制板#2(灯舱控制板)
                                    case enum_AddressBoard.Light_Control_Panel_II:

                                        {
                                            if (bType == 0x80)
                                            { }
                                            else if (bType == 0x82)
                                            {
                                                Struct_Light_Control_Panel myStruct_Light_Control_Panel =
                                                    new Struct_Light_Control_Panel();
                                                myStruct_Light_Control_Panel.type = 0x82;
                                                myStruct_Light_Control_Panel.Address = bAddrRecv;
                                                myStruct_Light_Control_Panel.bCmdReturn = bb;
                                                myGEventArgs.dataType = 6;
                                                myGEventArgs.connected = true;
                                                myGEventArgs.obj = bb;
                                                myGEventArgs.objParse = myStruct_Light_Control_Panel;
                                                if (EventSerialDataSend != null)
                                                {
                                                    EventSerialDataSend(this, myGEventArgs);
                                                }
                                            }
                                            else if (bType == 0x81)
                                            {
 
                                            }
                                            else
                                            {
                                                continue;
                                            }
                                        }

                                        break;
                                    #endregion

                                    //0x50,电池电源转换板
                                    case enum_AddressBoard.Battery_Power_Conversion_Board:

                                        break;

                                    #region 工况采集板（0x30）
                                    //工况采集板（0x30）
                                    case enum_AddressBoard.Work_Station_Quire_Board:
                                        {
                                            if (bType == 0x80)
                                            { }
                                            else if (bType == 0x82)
                                            {

                                            }
                                            else if (bType == 0x81)
                                            {
                                                if (bb.Length < 16)
                                                {
                                                    continue;
                                                }

                                                Struct_Work_Station_Quire_Board myStruct_Work_Station_Quire_Board =
                                                    new Struct_Work_Station_Quire_Board();
                                                myStruct_Work_Station_Quire_Board.type = 0x81;
                                                myStruct_Work_Station_Quire_Board.Address = bAddrRecv;
                                                byte[] bData = new byte[2];
                                                bData[1] = bb[6];
                                                bData[0] = bb[7];
                                                UInt16 data = BitConverter.ToUInt16(bData, 0);
                                                myStruct_Work_Station_Quire_Board.RotateSpeed = data;

                                                myStruct_Work_Station_Quire_Board.WorkStation_B = bb[8];
                                                myStruct_Work_Station_Quire_Board.WorkStation_C = bb[9];
                                                myStruct_Work_Station_Quire_Board.WorkStation_D = bb[10];
                                                myStruct_Work_Station_Quire_Board.WorkStation_E = bb[11];
                                                myStruct_Work_Station_Quire_Board.WorkStation_F = bb[12];
                                                myStruct_Work_Station_Quire_Board.WorkStation_G = bb[13];

                                                myGEventArgs.dataType = 5;
                                                myGEventArgs.connected = true;
                                                myGEventArgs.obj = bb;
                                                myGEventArgs.objParse = myStruct_Work_Station_Quire_Board;
                                                if (EventSerialDataSend != null)
                                                {
                                                    EventSerialDataSend(this, myGEventArgs);
                                                }
                                            }
                                            else
                                            {
                                                continue;
                                            }
                                        }
                                        break;
                                    #endregion

                                    //0x10,水面控制盒
                                    case enum_AddressBoard.Water_Control_Box:

                                        break;

                                    #region 电机温度检测板：0x45
                                    //0x45,电机温度检测板,(传感器接口箱信号采集板)
                                    case enum_AddressBoard.DianJi_T_Detection_Board:
                                        {
                                            if (bType == 0x80)
                                            { }
                                            else if (bType == 0x82)
                                            {
                                                Struct_DianJi_T myStruct_DianJi_T =
                                                    new Struct_DianJi_T();
                                                myStruct_DianJi_T.type = 0x82;
                                                myStruct_DianJi_T.Address = bAddrRecv;
                                                myStruct_DianJi_T.bCmdReturn = bb;
                                                myGEventArgs.dataType = 6;
                                                myGEventArgs.connected = true;
                                                myGEventArgs.obj = bb;
                                                myGEventArgs.objParse = myStruct_DianJi_T;
                                                if (EventSerialDataSend != null)
                                                {
                                                    EventSerialDataSend(this, myGEventArgs);
                                                }
                                            }
                                            else if (bType == 0x81)
                                            {
                                                if (bb.Length < 24)
                                                {
                                                    continue;
                                                }

                                                Struct_DianJi_T myStruct_DianJi_T =
                                                    new Struct_DianJi_T();
                                                myStruct_DianJi_T.type = 0x81;
                                                myStruct_DianJi_T.Address = bAddrRecv;

                                                //5路漏水
                                                byte[] bData = new byte[2];
                                                bData[1] = bb[6];
                                                bData[0] = bb[7];
                                                UInt16 data = BitConverter.ToUInt16(bData, 0);
                                                myStruct_DianJi_T.Para_1 = data * 3.3 / 4096;

                                                bData[1] = bb[8];
                                                bData[0] = bb[9];
                                                data = BitConverter.ToUInt16(bData, 0);
                                                myStruct_DianJi_T.Para_2 = data * 3.3 / 4096;

                                                bData[1] = bb[10];
                                                bData[0] = bb[11];
                                                data = BitConverter.ToUInt16(bData, 0);
                                                myStruct_DianJi_T.Para_3 = data * 3.3 / 4096;

                                                bData[1] = bb[12];
                                                bData[0] = bb[13];
                                                data = BitConverter.ToUInt16(bData, 0);
                                                myStruct_DianJi_T.Para_4 = data * 3.3 / 4096;

                                                bData[1] = bb[14];
                                                bData[0] = bb[15];
                                                data = BitConverter.ToUInt16(bData, 0);
                                                myStruct_DianJi_T.Para_5 = data * 3.3 / 4096;

                                                //3路温度
                                                bData[1] = bb[16];
                                                bData[0] = bb[17];
                                                data = BitConverter.ToUInt16(bData, 0);
                                                myStruct_DianJi_T.Para_6 = data * 3.3 / 4096;

                                                bData[1] = bb[18];
                                                bData[0] = bb[19];
                                                data = BitConverter.ToUInt16(bData, 0);
                                                myStruct_DianJi_T.Para_7 = data * 3.3 / 4096;

                                                bData[1] = bb[20];
                                                bData[0] = bb[21];
                                                data = BitConverter.ToUInt16(bData, 0);
                                                myStruct_DianJi_T.Para_8 = data * 3.3 / 4096;
                                                                                           
                                                myGEventArgs.dataType = 5;
                                                myGEventArgs.connected = true;
                                                myGEventArgs.obj = bb;
                                                myGEventArgs.objParse = myStruct_DianJi_T;
                                                if (EventSerialDataSend != null)
                                                {
                                                    EventSerialDataSend(this, myGEventArgs);
                                                }
                                            }
                                            else
                                            {
                                                continue;
                                            }
                                        }

                                        break;

                                         #endregion

                                    default:
                                        break;
                                }

                            }
                            else
                            {
                                switch (mySerialCOMType)
                                {
                                    case enum_SerialCOMType.Hight_Measure_Device:
                                        {
                                            string[] splitCom = new string[] { "\r", "\n" };
                                            string sRecv = Encoding.ASCII.GetString(bb);
                                            string[] sRecv2 = sRecv.Split(splitCom, StringSplitOptions.RemoveEmptyEntries);

                                            if (sRecv2.Length > 0)
                                            {
                                                Struct_HightMeasureDevice myStruct = new Struct_HightMeasureDevice();
                                                myStruct.sHight = sRecv2[0];
                                                string ss = sRecv2[0].Replace(" ", "").ToUpper().Replace("M", "");
                                                bool bHeight = double.TryParse(ss, out myStruct.Height);
                                                myStruct.Address = (int)enum_AddressBoard.Hight_Measure_Device;

                                                myGEventArgs.addressBoard = enum_AddressBoard.Hight_Measure_Device;
                                                myGEventArgs.dataType = 5;
                                                myGEventArgs.connected = true;
                                                myGEventArgs.obj = bb;
                                                myGEventArgs.objParse = myStruct;
                                                if (EventSerialDataSend != null)
                                                {
                                                    EventSerialDataSend(this, myGEventArgs);
                                                }
                                            }
                                        }
                                        break;

                                    case enum_SerialCOMType.Rotate_Panel_Device:
                                        {
                                            if (bb[0] == 0x50 && bb[1] == 0x03 && bb[2] == 0x0C)    //检验同步头
                                            {
                                                if (bb.Length >= 17)
                                                {
                                                    Struct_RotatePanelDevice myStruct = new Struct_RotatePanelDevice();
                                                    myStruct.Address = (int)enum_AddressBoard.Rotate_Panel_Device;

                                                    byte[] bData = new byte[2];
                                                    bData[1] = bb[3];
                                                    bData[0] = bb[4];
                                                    UInt16 data = BitConverter.ToUInt16(bData, 0);
                                                    myStruct.HX = data;

                                                    bData[1] = bb[5];
                                                    bData[0] = bb[6];
                                                    data = BitConverter.ToUInt16(bData, 0);
                                                    myStruct.HY = data;

                                                    bData[1] = bb[7];
                                                    bData[0] = bb[8];
                                                    data = BitConverter.ToUInt16(bData, 0);
                                                    myStruct.HZ = data;

                                                    bData[1] = bb[9];
                                                    bData[0] = bb[10];
                                                    data = BitConverter.ToUInt16(bData, 0);
                                                    myStruct.Roll = 180 / 32768.0 * data;
                                                    //myStruct.Roll = myStruct.Roll - 180;
                                                    myStruct.Roll = 360 - myStruct.Roll;

                                                    bData[1] = bb[11];
                                                    bData[0] = bb[12];
                                                    data = BitConverter.ToUInt16(bData, 0);
                                                    myStruct.Pitch = 180 / 32768.0 * data;
                                                    myStruct.Pitch = myStruct.Pitch - 180;

                                                    bData[1] = bb[13];
                                                    bData[0] = bb[14];
                                                    data = BitConverter.ToUInt16(bData, 0);
                                                    myStruct.Yaw = 360 - 180 / 32768.0 * data;


                                                    myGEventArgs.addressBoard = enum_AddressBoard.Rotate_Panel_Device;
                                                    myGEventArgs.dataType = 5;
                                                    myGEventArgs.connected = true;
                                                    myGEventArgs.obj = bb;
                                                    myGEventArgs.objParse = myStruct;
                                                    if (EventSerialDataSend != null)
                                                    {
                                                        EventSerialDataSend(this, myGEventArgs);
                                                    }
                                                }
                                            }
                                        }
                                        break;

                                    default:
                                        break;
                                }


                            }

                            #endregion

                        }
                    }
                    else
                    {
                        Thread.Sleep(50);
                    }
                }
                catch (Exception ex)
                {
                    string ss = ex.Message.ToString();
                    if (ss == "远程主机强迫关闭了一个现有的连接。")
                    {
                        isConnected = false;
                        myGEventArgs.dataType = 1;
                        myGEventArgs.connected = false;
                        myGEventArgs.nameSerial = name;
                        if (EventSerialDataSend != null)
                        {
                            EventSerialDataSend(this, myGEventArgs);
                        }
                    }
                }
            }
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

        private void SerialSendThreadProcess()
        {
            while (stopSerialSend == false)
            {
                try
                {
                    if (isConnected)
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
                                        SerialSocket.Send(bQuiry);
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

        public void BCmdInsert(byte[] cmd)
        {
            try
            {
                if (cmd != null && cmd.Length > 0)
                {
                    if (isConnected)
                    {
                        isCtlSending = true;
                        Thread.Sleep(Global.delay_CmdSend_Before);
                        SerialSocket.Send(cmd);
                        Thread.Sleep(Global.delay_CmdSend_After);
                        isCtlSending = false;
                    }
                }
            }
            catch (Exception ex)
            { }
        }

        public double GetRotateJiao(UInt16 data)
        {
            try
            {
                double d = 180 / 32768.0 * data;
                return d;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }



        #region 绝缘检测板处理公式

        //AC GFM是交流绝缘检测板(0x80)处理公式
        //y =A1*exp(-x/t1)+y0
        //y0=1031.98953
        //A1=50084.27431
        //t1=0.25399
        private double GetPhyValueForACGFM(double x)
        {
            try
            {
                double y = 0;
                double y0 = 1031.98953;
                double A1 = 50084.27431;
                double t1 = 0.25399;

                y = A1 * Math.Exp(-1 * x / t1) + y0;

                return y;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }


        //Big GFC是大绝缘检测板(0x70-0x73)处理公式
        //电压
        //y=a*(x-b)
        //a=12.38518
        //b=-0.05066

        //电流
        //y=a*(x-b)
        //a=12.32936
        //b=-0.0274

        //漏电流
        //y=A+B*x
        //A=1.18576
        //B=-1.03494

        //温度
        //y=A+B*x+C*x^2+D*x^3
        //A=-1227.88738
        //B=3002.30868
        //C=-2333.28397
        //D=625.37268
        private double GetVPhyValueForBigGFC(double x)
        {
            try
            {
                double y = 0;
                double a = 12.38518;
                double b = -0.05066;
                y = a * (x - b);

                return y;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        private double GetIPhyValueForBigGFC(double x)
        {
            try
            {
                double y = 0;
                double a = 12.32936;
                double b = -0.0274;
                y = a * (x - b);

                return y;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        private double GetILeakPhyValueForBigGFC(double x)
        {
            try
            {
                double y = 0;
                double A = 1.18576;
                double B = -1.03494;
                y = A + B * x;

                return y;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        private double GetTPhyValueForBigGFC(double x)
        {
            try
            {
                double y = 0;
                double A = -1227.88738;
                double B = 3002.30868;
                double C = -2333.28397;
                double D = 625.37268;
                y = A + B * x + C * Math.Pow(x, 2) + D * Math.Pow(x, 3);

                return y;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        //GFC是小绝缘检测板(0x61-0x63)处理公式
        //电压
        //y=A+B*x
        //A=0.43637
        //B=8.08383

        //电流
        //y=A+B*x
        //A=0.09748
        //B=5.31374

        //漏电流
        //y=A+B*x
        //A=-0.00615
        //B=0.33667

        //温度
        //y=A+B*x+C*x^2+D*x^3
        //A=-793.34213
        //B=1931.21601
        //C=-1493.23433
        //D=416.54792

        private double GetVPhyValueForGFC(double x)
        {
            try
            {
                double y = 0;
                double A = 0.43637;
                double B = 8.08383;
                y = A + B * x;

                return y;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        private double GetIPhyValueForGFC(double x)
        {
            try
            {
                double y = 0;
                double A = 0.09748;
                double B = 5.31374;
                y = A + B * x;

                return y;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        private double GetILeakPhyValueForGFC(double x)
        {
            try
            {
                double y = 0;
                double A = -0.00615;
                double B = 0.33667;
                y = A + B * x;

                return y;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        private double GetTPhyValueForGFC(double x)
        {
            try
            {
                double y = 0;
                double A = -793.34213;
                double B = 1931.21601;
                double C = -1493.23433;
                double D = 416.54792;
                y = A + B * x + C * Math.Pow(x, 2) + D * Math.Pow(x, 3);

                return y;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        #endregion




    }
}
