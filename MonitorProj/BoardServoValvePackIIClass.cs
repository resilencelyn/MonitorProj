using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace MonitorProj
{
    class BoardServoValvePackIIClass
    {
        //16功能阀箱，功能类

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

        //板卡地址
        private enum_AddressBoard myBoardAddress;
        public enum_AddressBoard MyBoardAddress
        {
            get
            {
                return myBoardAddress;
            }
            set
            {
                myBoardAddress = value;
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

        #endregion


        public bool Initialize()
        {
            try
            {
                GEventArgs myGEventArgs = new GEventArgs();
                myGEventArgs.nameSerial = this.name;
                myGEventArgs.dataType = 1;

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
                    SerialRecvThread.Name = name + "-SerialRecvThread";
                    SerialRecvThread.Start();

                    SerialSendThread = new Thread(new ThreadStart(SerialSendThreadProcess));
                    SerialSendThread.Name = name + "-SerialSendThread";
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


            while (stopSerialRecv == false)
            {
                try
                {
                    if (isConnected)
                    {
                        //int len = SerialSocket.Receive(dataRecv);//by lqy 20190307

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
                            //if (SerialDataSend != null)
                            //{
                            //    SerialDataSend(this, myGEventArgs);
                            //}

                            //解析
                            #region 解析

                            switch(myBoardTypeClass)
                            {
                                #region 解析，16功能阀箱，国外版
                                case enum_BoardTypeClass.ClassA:
                                    {
                                        List<int> indexSubstitution = new List<int>();
                                        indexSubstitution.Add(0);

                                        byte bSubstitution = bb[0];
                                        string sData = bb[0].ToString("X2");
                                        string sCRCResult = "";
                                        for (int i = 1; i < len; i++)
                                        {
                                            if (bb[i] == bSubstitution)
                                            {
                                                bb[i] = 0xAA;
                                                indexSubstitution.Add(i);
                                            }
                                            sData += bb[i].ToString("X2");
                                        }

                                        //CRC
                                        byte[] crc = Global.CRC16(bb, 1, bb.Length - 4);
                                        sCRCResult = ";CRC校验值为" + crc[0].ToString("X2") + crc[1].ToString("X2");

                                        Struct_BoardB_Status myStructBoardIIStatus = new Struct_BoardB_Status();
                                        myStructBoardIIStatus.boardTypeClass = enum_BoardTypeClass.ClassA;
                                        myStructBoardIIStatus.Substitution_Character = bb[0];
                                        myStructBoardIIStatus.Message_ID = bb[1];
                                        myStructBoardIIStatus.CPU_Status = bb[2];
                                        myStructBoardIIStatus.Received_Bad_CRCs = BitConverter.ToUInt16(bb, 3);
                                        myStructBoardIIStatus.Address = bb[5];

                                        myStructBoardIIStatus.Digital_Inputs = bb[6];

                                        myStructBoardIIStatus.bDIN8 = (bb[6] & 0x80) == 0 ? 0 : 1;
                                        myStructBoardIIStatus.bDIN7 = (bb[6] & 0x40) == 0 ? 0 : 1;
                                        myStructBoardIIStatus.bDIN6 = (bb[6] & 0x20) == 0 ? 0 : 1;
                                        myStructBoardIIStatus.bDIN5 = (bb[6] & 0x10) == 0 ? 0 : 1;
                                        myStructBoardIIStatus.bDIN4 = (bb[6] & 0x08) == 0 ? 0 : 1;
                                        myStructBoardIIStatus.bDIN3 = (bb[6] & 0x04) == 0 ? 0 : 1;
                                        myStructBoardIIStatus.bDIN2 = (bb[6] & 0x02) == 0 ? 0 : 1;
                                        myStructBoardIIStatus.bDIN1 = (bb[6] & 0x01) == 0 ? 0 : 1;

                                        myStructBoardIIStatus.Current_Feedback_PWM_1_2 = GetPhyResult(bb, 7, 5);
                                        myStructBoardIIStatus.Current_Feedback_PWM_3_4 = GetPhyResult(bb, 9, 5);
                                        myStructBoardIIStatus.Current_Feedback_PWM_5_6 = GetPhyResult(bb, 11, 5);
                                        myStructBoardIIStatus.Current_Feedback_PWM_7_8 = GetPhyResult(bb, 13, 5);
                                        myStructBoardIIStatus.Current_Feedback_PWM_9_10 = GetPhyResult(bb, 15, 5);
                                        myStructBoardIIStatus.Current_Feedback_PWM_11_12 = GetPhyResult(bb, 17, 5);
                                        myStructBoardIIStatus.Current_Feedback_PWM_13_14 = GetPhyResult(bb, 19, 5);
                                        myStructBoardIIStatus.Current_Feedback_PWM_15_16 = GetPhyResult(bb, 21, 5);

                                        myStructBoardIIStatus.VCCD = GetPhyResult(bb, 23, 5);
                                        myStructBoardIIStatus.VCCA = GetPhyResult(bb, 25, 5);

                                        myStructBoardIIStatus.External_Analog_In_1 = GetPhyResult(bb, 27, 5);
                                        myStructBoardIIStatus.External_Analog_In_2 = GetPhyResult(bb, 29, 5);
                                        myStructBoardIIStatus.External_Analog_In_3 = GetPhyResult(bb, 31, 5);
                                        myStructBoardIIStatus.External_Analog_In_4 = GetPhyResult(bb, 33, 5);

                                        myStructBoardIIStatus.Main_Supply_Voltage_24VDC = GetPhyResult(bb, 35, 5);
                                        myStructBoardIIStatus.Main_Supply_Voltage_15VDC = GetPhyResult(bb, 37, 5);

                                        myStructBoardIIStatus.Current_Feedback_DOUT1_8_SSUP_1_2 = GetPhyResult(bb, 39, 10);
                                        myStructBoardIIStatus.Current_Feedback_DOUT9_16_SSUP_3_4 = GetPhyResult(bb, 41, 10);

                                        myStructBoardIIStatus.Temperature = GetTemperature(bb, 43);

                                        myStructBoardIIStatus.sData = sData;
                                        myStructBoardIIStatus.sCRCResult = sCRCResult;
                                        myStructBoardIIStatus.indexSubstitution = indexSubstitution;

                                        myGEventArgs.dataType = 5;
                                        myGEventArgs.addressBoard = enum_AddressBoard.ServoValvePacketBoard16Func;
                                        myGEventArgs.connected = true;
                                        myGEventArgs.obj = bb;
                                        myGEventArgs.objParse = myStructBoardIIStatus;
                                        if (EventSerialDataSend != null)
                                        {
                                            EventSerialDataSend(this, myGEventArgs);
                                        }
                                    }
                                    break;
                                #endregion

                                #region 解析，16功能阀箱，国产版
                                case enum_BoardTypeClass.ClassB:
                                    {
                                        if (bb.Length < 13)
                                        {
                                            continue;
                                        }
                                        Struct_BoardB_Status myStructBoardIIStatus = new Struct_BoardB_Status();
                                        myStructBoardIIStatus.boardTypeClass = enum_BoardTypeClass.ClassB;
                                        byte[] bPhy = new byte[4];
                                        bPhy[0] = bb[4];
                                        bPhy[1] = bb[3];
                                        bPhy[2] = 0;
                                        bPhy[3] = 0;
                                        uint iPhy = BitConverter.ToUInt32(bPhy, 0);
                                        myStructBoardIIStatus.V = iPhy / 10.0;

                                        bPhy[0] = bb[6];
                                        bPhy[1] = bb[5];
                                        bPhy[2] = 0;
                                        bPhy[3] = 0;
                                        iPhy = BitConverter.ToUInt32(bPhy, 0);
                                        myStructBoardIIStatus.I = iPhy / 10.0;

                                        bPhy[0] = bb[8];
                                        bPhy[1] = bb[7];
                                        bPhy[2] = 0;
                                        bPhy[3] = 0;
                                        iPhy = BitConverter.ToUInt32(bPhy, 0);
                                        myStructBoardIIStatus.P = iPhy / 10.0;

                                        if (bb[9] == 0x00 && bb[10] == 0x00)
                                        {
                                            myStructBoardIIStatus.isLeakage = false;
                                        }
                                        else
                                        {
                                            myStructBoardIIStatus.isLeakage = true;
                                        }
                                        myGEventArgs.dataType = 5;
                                        myGEventArgs.addressBoard = enum_AddressBoard.ServoValvePacketBoard16Func;
                                        myGEventArgs.connected = true;
                                        myGEventArgs.obj = bb;
                                        myGEventArgs.objParse = myStructBoardIIStatus;
                                        if (EventSerialDataSend != null)
                                        {
                                            EventSerialDataSend(this, myGEventArgs);
                                        }
                                    }
                                    break;
                                #endregion

                                default:
                                    break;

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

        //analog inputs: 0V-5V equates to a range of 0x0000-0x0FFF, (12 bits).
        private double GetPhyResult(byte[] bytes, int startIndex, double VBase)
        {
            try
            {
                byte[] bb = new byte[2];
                bb[1] = bytes[startIndex];
                bb[0] = bytes[startIndex + 1];

                UInt16 t = BitConverter.ToUInt16(bb, 0);
                double res = 0;
                if (t <= 0)
                {
                    t = 0;
                }
                else if (t >= 0x0FFF)
                {
                    t = 0x0FFF;
                }
                res = t * VBase / 0x0FFF;
                res = Math.Round(res, 2);
                return res;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        //Refer to Maxim DS1620 data sheet, range 0x0000-0x01FF spans range of -55oC to +125oC.
        private double GetTemperature(byte[] bytes, int startIndex)
        {
            try
            {
                byte[] bb = new byte[2];
                bb[1] = bytes[startIndex];
                bb[0] = bytes[startIndex + 1];

                UInt16 t = BitConverter.ToUInt16(bb, 0);
                double res = 0;
                if (t <= 0)
                {
                    res = -55;
                }
                else if (t >= 0x01FF)
                {
                    res = 125;
                }
                else
                {
                    res = t * (125.0 - (-55.0)) / 0x01FF - 55;
                    res = Math.Round(res, 2);
                }
                return res;
            }
            catch (Exception ex)
            {
                return 0;
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
   

    }
}
