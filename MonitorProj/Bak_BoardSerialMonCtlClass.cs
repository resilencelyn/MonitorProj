using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Threading;

namespace MonitorProj
{
    class Bak_BoardSerialMonCtlClass
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



        public Bak_BoardSerialMonCtlClass()
        {
            
            mySerialPort = new SerialPort();

            SerialRecvThread = new Thread(new ThreadStart(SerialRecvThreadProcess));
            //SerialRecvThread.Name = name + "-SerialRecvThread";
            SerialRecvThread.Start();

            SerialSendThread = new Thread(new ThreadStart(SerialSendThreadProcess));
            //SerialSendThread.Name = name + "-SerialSendThread";
            SerialSendThread.Start();
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
                            Thread.Sleep(5);
                            continue;
                        }
                        else
                        {
                            byte[] readBuffer = new byte[count];
                            mySerialPort.Read(readBuffer, 0, count);


                            #region 解析

                            #region 数据协议格式校验

                            if (readBuffer.Length < 8)  //数据长度不足
                            {
                                continue;
                            }

                            if (readBuffer[0] == 0xFF && readBuffer[1] == 0xFF && readBuffer[2] == 0xA5)    //检验同步头
                            {
                                byte bAddrRecv = readBuffer[3];
                                byte bType = readBuffer[4];//指令类型:0x80-自检指令返回；0x81-查询指令返回；0x82-执行指令返回
                                byte bDataLen = readBuffer[5];
                                if (count < (bDataLen + 3))  //数据长度符合性检查
                                {
                                    continue;
                                }
                                byte byteCheck = 0;
                                bool bCheck = SumCheck(readBuffer, 3, bDataLen - 2, out byteCheck);
                                if (bCheck == false || byteCheck != readBuffer[3 + bDataLen - 2])   //校验错误
                                {
                                    continue;
                                }
                                if (readBuffer[3 + bDataLen - 1] != 0x26)   //帧尾校验
                                {
                                    continue;
                                }

                            #endregion

                                enum_AddressBoard myBoardAddress = (enum_AddressBoard)bAddrRecv;
                                myGEventArgs.addressBoard = myBoardAddress;

                                switch (myBoardAddress)
                                {
                                    //控制盒（0x10）
                                    case enum_AddressBoard.Water_Control_Box:
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
                                                myStruct_Water_Control_Box.Address = bAddrRecv;
                                                byte[] bData = new byte[2];
                                                bData[1] = readBuffer[6];
                                                bData[0] = readBuffer[7];
                                                UInt16 data = BitConverter.ToUInt16(bData, 0);
                                                myStruct_Water_Control_Box.RotAxisX = data * 3.3 / 4096;

                                                bData[1] = readBuffer[8];
                                                bData[0] = readBuffer[9];
                                                data = BitConverter.ToUInt16(bData, 0);
                                                myStruct_Water_Control_Box.RotAxisY = data * 3.3 / 4096;
                                                
                                                bData[1] = readBuffer[10];
                                                bData[0] = readBuffer[11];
                                                data = BitConverter.ToUInt16(bData, 0);
                                                myStruct_Water_Control_Box.RotAxisZ = data * 3.3 / 4096;

                                                bData[1] = readBuffer[12];
                                                bData[0] = readBuffer[13];
                                                data = BitConverter.ToUInt16(bData, 0);
                                                myStruct_Water_Control_Box.RotAxisV = data * 3.3 / 4096;

                                                bData[1] = readBuffer[14];
                                                bData[0] = readBuffer[15];
                                                data = BitConverter.ToUInt16(bData, 0);
                                                myStruct_Water_Control_Box.Space1 = data * 3.3 / 4096;

                                                bData[1] = readBuffer[16];
                                                bData[0] = readBuffer[17];
                                                data = BitConverter.ToUInt16(bData, 0);
                                                myStruct_Water_Control_Box.Space2 = data * 3.3 / 4096;

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
                        mySerialPort.Write(buf, 0, buf.Length);
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
