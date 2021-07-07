using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Net;
using System.IO.Ports;
using System.Threading;

namespace MonitorProj
{
    public partial class FormMainModeI : Form
    {
        public FormMainModeI()
        {
            InitializeComponent();


            this.treeView_MonCtlAll.ExpandAll();

            //设置自动换行
            this.DataGridView_SysAlarmInfo.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            //设置自动调整高度
            this.DataGridView_SysAlarmInfo.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
        }


        private void FormMain_Load(object sender, EventArgs e)
        {
            try
            {
                this.WindowState = FormWindowState.Maximized;

                Dictionary<enum_BoardType, List<struct_SerialConfParas>> dicType = Global.DicModeBoardTypeSerialConf[enum_ModeType.Mode1];

                int len = dicType.Count;
                List<enum_BoardType> keys = dicType.Keys.ToList();
                for (int i = 0; i < len; i++)
                {
                    enum_BoardType key = keys[i];
                    List<struct_SerialConfParas> listSerialConfParas = dicType[key];
                    int count = listSerialConfParas.Count;
                    switch (key)
                    {
                        case enum_BoardType.BoardA:

                            //for (int j = 0; j < count; j++)
                            {
                                int j = 0;
                                struct_SerialConfParas myStruct_SerialConfParas = listSerialConfParas[j];
                                //string sName = myStruct_SerialConfParas.serialnum;
                                string sName = "BoardAMonCtl";
                                TreeNode node = treeView_MonCtlAll.Nodes["MonCtlRootNode"].Nodes["BoardA"];
                                TreeNode nodeNew = new TreeNode(myStruct_SerialConfParas.serialname);
                                nodeNew.Name = sName;
                                node.Nodes.Add(nodeNew);

                                Global.m_FormBoardI = new FormBoardI()
                                {
                                    AddressLocal = Global.ipAddressLocal,
                                    PortLocal = myStruct_SerialConfParas.port + Global.portLocalOffset,
                                    AddressRemote = myStruct_SerialConfParas.address,
                                    PortRemote = myStruct_SerialConfParas.port,
                                    SQuirys = myStruct_SerialConfParas.quirys.ToArray(),
                                    QuiryInterval = myStruct_SerialConfParas.quiryinterval,
                                    NameIn = sName,
                                    MyBoardType = enum_BoardType.BoardA,
                                    MySerialCOMType = enum_SerialCOMType.ServoValvePackI
                                };
                                Global.m_FormBoardI.EventSerialDataSend += new EventHandler(ReceiveDataFromSerial);
                                Global.m_FormBoardI.EventSerialDataSend += new EventHandler(Global.myFormMainUserA.ReceiveDataFromSerial);
                                Global.m_FormBoardI.Name = sName;
                                Global.m_FormBoardI.Initialize();
                                Global.m_FormBoardI.TopLevel = false;
                                panel_FormContainer.Controls.Add(Global.m_FormBoardI);
                                Global.m_FormBoardI.Location = new Point(0, 0);
                                Global.m_FormBoardI.Dock = DockStyle.Fill;
                                Global.m_FormBoardI.Show();
                                //Global.m_FormBoardI.Visible = false;
                                Global.dicFormHandleIntPtr.Add(sName, Global.m_FormBoardI.Handle);
                                Global.dicFormNameType.Add(sName, enum_BoardTypeAndClass.BoardA);
                            }

                            break;

                        case enum_BoardType.BoardB:

                            //for (int j = 0; j < count; j++)
                            {
                                int j = 0;
                                struct_SerialConfParas myStruct_SerialConfParas = listSerialConfParas[j];
                                //string sName = myStruct_SerialConfParas.serialnum;
                                string sName = "BoardBMonCtl";
                                TreeNode node = treeView_MonCtlAll.Nodes["MonCtlRootNode"].Nodes["BoardB"];
                                TreeNode nodeNew = new TreeNode(myStruct_SerialConfParas.serialname);
                                nodeNew.Name = sName;
                                node.Nodes.Add(nodeNew);

                                if (myStruct_SerialConfParas.boradClass == enum_BoardTypeClass.ClassA)
                                {
                                    Global.m_FormBoardII = new FormBoardII()
                                    {
                                        AddressLocal = Global.ipAddressLocal,
                                        PortLocal = myStruct_SerialConfParas.port + Global.portLocalOffset,
                                        AddressRemote = myStruct_SerialConfParas.address,
                                        PortRemote = myStruct_SerialConfParas.port,
                                        SQuirys = myStruct_SerialConfParas.quirys.ToArray(),
                                        QuiryInterval = myStruct_SerialConfParas.quiryinterval,
                                        NameIn = sName,
                                        MyBoardType = enum_BoardType.BoardB,
                                        MySerialCOMType = enum_SerialCOMType.ServoValvePackII,
                                        MyBoardTypeClass = enum_BoardTypeClass.ClassA
                                    };
                                    Global.m_FormBoardII.EventSerialDataSend += new EventHandler(ReceiveDataFromSerial);
                                    Global.m_FormBoardII.EventSerialDataSend += new EventHandler(Global.myFormMainUserA.ReceiveDataFromSerial);
                                    Global.m_FormBoardII.Name = sName;
                                    Global.m_FormBoardII.Initialize();
                                    Global.m_FormBoardII.TopLevel = false;
                                    panel_FormContainer.Controls.Add(Global.m_FormBoardII);
                                    Global.m_FormBoardII.Location = new Point(0, 0);
                                    Global.m_FormBoardII.Dock = DockStyle.Fill;
                                    Global.m_FormBoardII.Show();
                                    //Global.m_FormBoardII.Visible = false;
                                    Global.dicFormHandleIntPtr.Add(sName, Global.m_FormBoardII.Handle);
                                    Global.dicFormNameType.Add(sName, enum_BoardTypeAndClass.BoardB);
                                }
                                else if (myStruct_SerialConfParas.boradClass == enum_BoardTypeClass.ClassB)
                                {
                                    Global.m_FormBoardIIB = new FormBoardIIB()
                                    {
                                        AddressLocal = Global.ipAddressLocal,
                                        PortLocal = myStruct_SerialConfParas.port + Global.portLocalOffset,
                                        AddressRemote = myStruct_SerialConfParas.address,
                                        PortRemote = myStruct_SerialConfParas.port,
                                        SQuirys = myStruct_SerialConfParas.quirys.ToArray(),
                                        QuiryInterval = myStruct_SerialConfParas.quiryinterval,
                                        NameIn = sName,
                                        MyBoardType = enum_BoardType.BoardB,
                                        MySerialCOMType = enum_SerialCOMType.ServoValvePackII,
                                        MyBoardTypeClass = enum_BoardTypeClass.ClassB
                                    };
                                    Global.m_FormBoardIIB.EventSerialDataSend += new EventHandler(ReceiveDataFromSerial);
                                    Global.m_FormBoardIIB.EventSerialDataSend += new EventHandler(Global.myFormMainUserA.ReceiveDataFromSerial);
                                    Global.m_FormBoardIIB.Name = sName;
                                    Global.m_FormBoardIIB.Initialize();
                                    Global.m_FormBoardIIB.TopLevel = false;
                                    panel_FormContainer.Controls.Add(Global.m_FormBoardIIB);
                                    Global.m_FormBoardIIB.Location = new Point(0, 0);
                                    Global.m_FormBoardIIB.Dock = DockStyle.Fill;
                                    Global.m_FormBoardIIB.Show();
                                    //Global.m_FormBoardIIB.Visible = false;
                                    Global.dicFormHandleIntPtr.Add(sName, Global.m_FormBoardIIB.Handle);
                                    Global.dicFormNameType.Add(sName, enum_BoardTypeAndClass.BoardE);
                                }
                            }

                            break;


                        case enum_BoardType.BoardC:
                            {
                                string sText = "移动钻机监控";
                                string sName = "BoardCMonCtl";
                                TreeNode nodeBoardC = treeView_MonCtlAll.Nodes["MonCtlRootNode"].Nodes["BoardC"];
                                TreeNode nodeNewBoardC = new TreeNode(sText);
                                nodeNewBoardC.Name = sName;
                                nodeBoardC.Nodes.Add(nodeNewBoardC);

                                Global.m_FormMobileDrillMonCtl = new FormMobileDrillMonCtl()
                                {
                                    ListSerialConfParas = listSerialConfParas,
                                    NameIn = sName
                                };
                                Global.m_FormMobileDrillMonCtl.Name = sName;
                                Global.m_FormMobileDrillMonCtl.EventSerialDataSend += new EventHandler(ReceiveDataFromSerial);
                                Global.m_FormMobileDrillMonCtl.DeviceFormStateEventSend += new EventHandler(ReceiveDataFromModelsState);

                                Global.m_FormMobileDrillMonCtl.EventSerialDataSend += new EventHandler(Global.myFormMainUserA.ReceiveDataFromSerial);
                                Global.m_FormMobileDrillMonCtl.DeviceFormStateEventSend += new EventHandler(Global.myFormMainUserA.ReceiveDataFromModelsState);

                                Global.m_FormMobileDrillMonCtl.Initialize();
                                Global.m_FormMobileDrillMonCtl.TopLevel = false;
                                panel_FormContainer.Controls.Add(Global.m_FormMobileDrillMonCtl);
                                Global.m_FormMobileDrillMonCtl.Location = new Point(0, 0);
                                Global.m_FormMobileDrillMonCtl.Dock = DockStyle.Fill;
                                Global.m_FormMobileDrillMonCtl.Show();
                                //Global.m_FormMobileDrillMonCtl.Visible = false;
                                Global.dicFormHandleIntPtr.Add(sName, Global.m_FormMobileDrillMonCtl.Handle);
                                Global.dicFormNameType.Add(sName, enum_BoardTypeAndClass.BoardC);
                            }
                            break;


                        case enum_BoardType.BoardD:

                            for (int j = 0; j < count; j++)
                            {
                                struct_SerialConfParas myStruct_SerialConfParas = listSerialConfParas[j];
                                //string sName = myStruct_SerialConfParas.serialnum;
                                TreeNode node = treeView_MonCtlAll.Nodes["MonCtlRootNode"].Nodes["BoardD"];
                                string sNameSerialIn = myStruct_SerialConfParas.serialname;
                                TreeNode nodeNew = new TreeNode(sNameSerialIn);
                                string sName = "BoardDMonCtl";
                                if (sNameSerialIn == "水面控制盒")
                                {
                                    sName += "_WaterBox";
                                }
                                else if (sNameSerialIn == "ROV供电系统")
                                {
                                    sName += "_RovPower";
                                }
                                else if (sNameSerialIn == "绝缘检测仪1")
                                {
                                    sName += "_JuYuanJianCe_1";
                                }
                                else if (sNameSerialIn == "绝缘检测仪2")
                                {
                                    sName += "_JuYuanJianCe_2";
                                }
                                nodeNew.Name = sName;
                                node.Nodes.Add(nodeNew);

                                //None、One、OnePointFive、Two
                                StopBits stopBits = StopBits.None;
                                switch (myStruct_SerialConfParas.stopbits)
                                {
                                    case "None":
                                        stopBits = StopBits.None;
                                        break;

                                    case "One":
                                        stopBits = StopBits.One;
                                        break;

                                    case "OnePointFive":
                                        stopBits = StopBits.OnePointFive;
                                        break;

                                    case "Two":
                                        stopBits = StopBits.Two;
                                        break;

                                    default:
                                        stopBits = StopBits.None;
                                        break;
                                }

                                //校验：Even、Mark、None、Odd、Space
                                Parity parity = Parity.None;
                                switch (myStruct_SerialConfParas.stopbits)
                                {
                                    case "Even":
                                        parity = Parity.Even;
                                        break;

                                    case "Mark":
                                        parity = Parity.Mark;
                                        break;

                                    case "None":
                                        parity = Parity.None;
                                        break;

                                    case "Odd":
                                        parity = Parity.Odd;
                                        break;

                                    case "Space":
                                        parity = Parity.Space;
                                        break;

                                    default:
                                        parity = Parity.None;
                                        break;
                                }

                                if (sNameSerialIn == "水面控制盒")
                                {
                                    Global.m_FormSerialWaterBoxCtl = new FormSerialWaterBoxCtl()
                                    {
                                        PortName = myStruct_SerialConfParas.portname,
                                        BandRate = myStruct_SerialConfParas.bandrate,
                                        DataBits = myStruct_SerialConfParas.databits,
                                        StopBits = stopBits,
                                        Parity = parity,
                                        SQuirys = myStruct_SerialConfParas.quirys.ToArray(),
                                        QuiryInterval = myStruct_SerialConfParas.quiryinterval,
                                        Name = sName,
                                        NameIn = "水面控制盒"
                                    };
                                    Global.m_FormSerialWaterBoxCtl.Name = sName;
                                    Global.m_FormSerialWaterBoxCtl.EventSerialDataSend += new EventHandler(ReceiveDataFromSerial);
                                    Global.m_FormSerialWaterBoxCtl.EventSerialDataSend += new EventHandler(Global.myFormMainUserA.ReceiveDataFromSerial);
                                    Global.m_FormSerialWaterBoxCtl.TopLevel = false;
                                    panel_FormContainer.Controls.Add(Global.m_FormSerialWaterBoxCtl);
                                    Global.m_FormSerialWaterBoxCtl.Location = new Point(0, 0);
                                    Global.m_FormSerialWaterBoxCtl.Dock = DockStyle.Fill;
                                    Global.m_FormSerialWaterBoxCtl.Show();
                                    //Global.m_FormSerialWaterBoxCtl.Visible = false;
                                    Global.dicFormHandleIntPtr.Add(sName, Global.m_FormSerialWaterBoxCtl.Handle);
                                    Global.dicFormNameType.Add(sName, enum_BoardTypeAndClass.BoardD);
                                }
                                else if (sNameSerialIn == "ROV供电系统")
                                {
                                    Global.m_FormSerialRovPowerCtl = new FormSerialRovPowerCtl()
                                    {
                                        PortName = myStruct_SerialConfParas.portname,
                                        BandRate = myStruct_SerialConfParas.bandrate,
                                        DataBits = myStruct_SerialConfParas.databits,
                                        StopBits = stopBits,
                                        Parity = parity,
                                        SQuirys = myStruct_SerialConfParas.quirys.ToArray(),
                                        QuiryInterval = myStruct_SerialConfParas.quiryinterval,
                                        Name = sName,
                                        NameIn = "ROV供电系统"
                                    };
                                    Global.m_FormSerialRovPowerCtl.Name = sName;
                                    Global.m_FormSerialRovPowerCtl.EventSerialDataSend += new EventHandler(ReceiveDataFromSerial);
                                    Global.m_FormSerialRovPowerCtl.EventSerialDataSend += new EventHandler(Global.myFormMainUserA.ReceiveDataFromSerial);
                                    Global.m_FormSerialRovPowerCtl.TopLevel = false;
                                    panel_FormContainer.Controls.Add(Global.m_FormSerialRovPowerCtl);
                                    Global.m_FormSerialRovPowerCtl.Location = new Point(0, 0);
                                    Global.m_FormSerialRovPowerCtl.Dock = DockStyle.Fill;
                                    Global.m_FormSerialRovPowerCtl.Show();
                                    //Global.m_FormSerialRovPowerCtl.Visible = false;
                                    Global.dicFormHandleIntPtr.Add(sName, Global.m_FormSerialRovPowerCtl.Handle);
                                    Global.dicFormNameType.Add(sName, enum_BoardTypeAndClass.BoardF);
                                }
                                else if (sNameSerialIn == "绝缘检测仪1")
                                {
                                    Global.m_FormSerialJuYuanJianCe1Ctl = new FormSerialJuYuanJianCe1Ctl()
                                    {
                                        PortName = myStruct_SerialConfParas.portname,
                                        BandRate = myStruct_SerialConfParas.bandrate,
                                        DataBits = myStruct_SerialConfParas.databits,
                                        StopBits = stopBits,
                                        Parity = parity,
                                        SQuirys = myStruct_SerialConfParas.quirys.ToArray(),
                                        QuiryInterval = myStruct_SerialConfParas.quiryinterval,
                                        Name = sName,
                                        NameIn = "绝缘检测仪1"
                                    };
                                    Global.m_FormSerialJuYuanJianCe1Ctl.Name = sName;
                                    Global.m_FormSerialJuYuanJianCe1Ctl.EventSerialDataSend += new EventHandler(ReceiveDataFromSerial);
                                    Global.m_FormSerialJuYuanJianCe1Ctl.EventSerialDataSend += new EventHandler(Global.myFormMainUserA.ReceiveDataFromSerial);
                                    Global.m_FormSerialJuYuanJianCe1Ctl.TopLevel = false;
                                    panel_FormContainer.Controls.Add(Global.m_FormSerialJuYuanJianCe1Ctl);
                                    Global.m_FormSerialJuYuanJianCe1Ctl.Location = new Point(0, 0);
                                    Global.m_FormSerialJuYuanJianCe1Ctl.Dock = DockStyle.Fill;
                                    Global.m_FormSerialJuYuanJianCe1Ctl.Show();
                                    //Global.m_FormSerialJuYuanJianCe1Ctl.Visible = false;
                                    Global.dicFormHandleIntPtr.Add(sName, Global.m_FormSerialJuYuanJianCe1Ctl.Handle);
                                    Global.dicFormNameType.Add(sName, enum_BoardTypeAndClass.BoardG);
                                }
                                else if (sNameSerialIn == "绝缘检测仪2")
                                {
                                    Global.m_FormSerialJuYuanJianCe2Ctl = new FormSerialJuYuanJianCe2Ctl()
                                    {
                                        PortName = myStruct_SerialConfParas.portname,
                                        BandRate = myStruct_SerialConfParas.bandrate,
                                        DataBits = myStruct_SerialConfParas.databits,
                                        StopBits = stopBits,
                                        Parity = parity,
                                        SQuirys = myStruct_SerialConfParas.quirys.ToArray(),
                                        QuiryInterval = myStruct_SerialConfParas.quiryinterval,
                                        Name = sName,
                                        NameIn = "绝缘检测仪2"
                                    };
                                    Global.m_FormSerialJuYuanJianCe2Ctl.Name = sName;
                                    Global.m_FormSerialJuYuanJianCe2Ctl.EventSerialDataSend += new EventHandler(ReceiveDataFromSerial);
                                    Global.m_FormSerialJuYuanJianCe2Ctl.EventSerialDataSend += new EventHandler(Global.myFormMainUserA.ReceiveDataFromSerial);
                                    Global.m_FormSerialJuYuanJianCe2Ctl.TopLevel = false;
                                    panel_FormContainer.Controls.Add(Global.m_FormSerialJuYuanJianCe2Ctl);
                                    Global.m_FormSerialJuYuanJianCe2Ctl.Location = new Point(0, 0);
                                    Global.m_FormSerialJuYuanJianCe2Ctl.Dock = DockStyle.Fill;
                                    Global.m_FormSerialJuYuanJianCe2Ctl.Show();
                                    //Global.m_FormSerialJuYuanJianCe2Ctl.Visible = false;
                                    Global.dicFormHandleIntPtr.Add(sName, Global.m_FormSerialJuYuanJianCe2Ctl.Handle);
                                    Global.dicFormNameType.Add(sName, enum_BoardTypeAndClass.BoardH);
                                }
                            }

                            break;

                        default:
                            break;
                    }
                }

                treeView_MonCtlAll.ExpandAll();
            }
            catch (Exception ex)
            { }
        }

        private void treeView_MonCtlAll_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                TreeNode clickedNode = treeView_MonCtlAll.GetNodeAt(e.X, e.Y);
                string sSelect = clickedNode.Name;
                List<string> keys = Global.dicFormHandleIntPtr.Keys.ToList();
                int len = keys.Count;
                for (int i = 0; i < len; i++)
                {
                    string key = keys[i];
                    IntPtr p_Form = Global.dicFormHandleIntPtr[key];
                    
                    //if (sSelect == key)
                    //{
                    //    Global.ShowWindowAsync(p_Form, 1);
                    //}
                    //else
                    //{
                    //    Global.ShowWindowAsync(p_Form, 0);
                    //}
                }
                
            }
            catch (Exception ex)
            { }
        }

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                int count = Global.dicFormHandleIntPtr.Count;
                List<string> keys = Global.dicFormHandleIntPtr.Keys.ToList();
                for (int i = 0; i < count; i++)
                {
                    string key = keys[i];
                    //Global.CloseWindow(Global.dicFormHandleIntPtr[key]);
                    Control ctl = Form.FromHandle(Global.dicFormHandleIntPtr[key]);
                    string name = ctl.Name;
                    switch (Global.dicFormNameType[name])
                    {
                        case enum_BoardTypeAndClass.BoardA:
                            FormBoardI myFormBoardI = (FormBoardI)ctl;
                            myFormBoardI.StopSerial();
                            break;

                        case enum_BoardTypeAndClass.BoardB:
                            FormBoardII myFormBoardII = (FormBoardII)ctl;
                            myFormBoardII.StopSerial();
                            break;

                        case enum_BoardTypeAndClass.BoardE:
                            FormBoardIIB myFormBoardIIB = (FormBoardIIB)ctl;
                            myFormBoardIIB.StopSerial();
                            break;

                        case enum_BoardTypeAndClass.BoardC:
                            FormMobileDrillMonCtl myFormMobileDrillMonCtl = (FormMobileDrillMonCtl)ctl;
                            myFormMobileDrillMonCtl.StopSerial();
                            break;

                        case enum_BoardTypeAndClass.BoardD:
                            FormSerialWaterBoxCtl myFormSerialWaterBoxCtl = (FormSerialWaterBoxCtl)ctl;
                            myFormSerialWaterBoxCtl.StopSerial();
                            break;

                        case enum_BoardTypeAndClass.BoardF:
                            FormSerialRovPowerCtl myFormSerialRovPowerCtl = (FormSerialRovPowerCtl)ctl;
                            myFormSerialRovPowerCtl.StopSerial();
                            break;

                        case enum_BoardTypeAndClass.BoardG:
                            FormSerialJuYuanJianCe1Ctl myFormSerialJuYuanJianCe1Ctl = (FormSerialJuYuanJianCe1Ctl)ctl;
                            myFormSerialJuYuanJianCe1Ctl.StopSerial();
                            break;

                        case enum_BoardTypeAndClass.BoardH:
                            FormSerialJuYuanJianCe2Ctl myFormSerialJuYuanJianCe2Ctl = (FormSerialJuYuanJianCe2Ctl)ctl;
                            myFormSerialJuYuanJianCe2Ctl.StopSerial();
                            break;

                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            { }
        }




        private void ReceiveDataFromSerial(object sender, EventArgs e)
        {
            try
            {
                GEventArgs gEventArgs = (GEventArgs)e;
                UpdateFormDelegate updateform = new UpdateFormDelegate(UpdateForm);
                this.BeginInvoke(updateform, new object[] { sender, gEventArgs });
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString());
            }
        }

        private delegate void UpdateFormDelegate(object sender, GEventArgs gEventArgs);
        private void UpdateForm(object sender, GEventArgs gEventArgs)
        {
            try
            {

                if (gEventArgs.dataType == 0 || gEventArgs.dataType == 5)
                {
                    try
                    {
                        List<alarmInfoUnit> listAlarmInfo = new List<alarmInfoUnit>();
                        if (gEventArgs.addressBoard == enum_AddressBoard.DC_Insulation_Detection_Board_I)
                        {
                            Struct_DC_Insulation_Detection_Board myStruct = (Struct_DC_Insulation_Detection_Board)gEventArgs.objParse;
                            listAlarmInfo = gEventArgs.listAlarmInfo.ToList();
                        }

                        //处理报警信息
                        int countAlarm = listAlarmInfo.Count;
                        int countDataGridRowsNow = DataGridView_SysAlarmInfo.Rows.Count;
                        for (int i = 0; i < countAlarm; i++)
                        {
                            try
                            {
                                int biaoHao = listAlarmInfo[i].index;
                                if (dicAlarmShow.ContainsKey(biaoHao))
                                {
                                    alarmInfoShowUnit aisu = dicAlarmShow[biaoHao];
                                    aisu.biaoHao = listAlarmInfo[i].index;
                                    aisu.alarm = listAlarmInfo[i].alarm;
                                    aisu.info = listAlarmInfo[i].info;
                                    dicAlarmShow[biaoHao] = aisu;
                                }
                                else
                                {
                                    alarmInfoShowUnit aisu = new alarmInfoShowUnit();
                                    aisu.biaoHao = listAlarmInfo[i].index;
                                    aisu.alarm = listAlarmInfo[i].alarm;
                                    aisu.info = listAlarmInfo[i].info;
                                    aisu.rowHao = countDataGridRowsNow++;
                                    dicAlarmShow.Add(aisu.biaoHao, aisu);
                                }
                            }
                            catch (Exception ex)
                            { }
                        }

                        List<int> listKeys = dicAlarmShow.Keys.ToList();
                        int countRow = dicAlarmShow.Count;
                        if (countRow > DataGridView_SysAlarmInfo.Rows.Count)
                        {
                            int countRowAdd = countRow - DataGridView_SysAlarmInfo.Rows.Count;
                            DataGridView_SysAlarmInfo.Rows.Add(countRowAdd);
                        }
                        else if (countRow < DataGridView_SysAlarmInfo.Rows.Count)
                        {
                            int countRowDel = DataGridView_SysAlarmInfo.Rows.Count - countRow;
                            for (int i = 0; i < countRowDel; i++)
                            {
                                DataGridView_SysAlarmInfo.Rows.RemoveAt(DataGridView_SysAlarmInfo.Rows.Count - 1);
                            }
                        }
                        for (int i = 0; i < countRow; i++)
                        {
                            int iRow = listKeys[i];
                            alarmInfoShowUnit aisu = dicAlarmShow[iRow];

                            DataGridView_SysAlarmInfo.Rows[aisu.rowHao].Cells[0].Value = aisu.info;
                            if (aisu.alarm == 2)
                            {
                                DataGridView_SysAlarmInfo.Rows[aisu.rowHao].DefaultCellStyle.BackColor = Color.Red;
                            }
                            else if (aisu.alarm == 1)
                            {
                                DataGridView_SysAlarmInfo.Rows[aisu.rowHao].DefaultCellStyle.BackColor = Color.Yellow;
                            }
                        }
                        DataGridView_SysAlarmInfo.ClearSelection();
                    }
                    catch (Exception ex1)
                    { }
                }
                if (gEventArgs.dataType == 1)
                {
                    if (gEventArgs.connected == false)
                    {
                        string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】串口\"" + gEventArgs.nameSerial + "\"已被远程断开！";
                        sInfo += "\t\n";
                        richTextBox_InfoShow.AppendText(sInfo);
                    }
                    else
                    {
                        string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】串口\"" + gEventArgs.nameSerial + "\"已连接！";
                        sInfo += "\t\n";
                        richTextBox_InfoShow.AppendText(sInfo);
                    }
                }
                else if (gEventArgs.dataType == 3)
                {
                    string sInfo = (string)gEventArgs.obj;
                    sInfo += "\t\n";
                    sInfo += "\t\n";
                    richTextBox_InfoShow.AppendText(sInfo);
                }
            }
            catch (Exception ex)
            { }
        }



        //各模块状态信息反馈
        private Dictionary<int, alarmInfoShowUnit> dicAlarmShow = new Dictionary<int, alarmInfoShowUnit>();
        private delegate void UpdateFormDelegateFromModelsState(object sender, GEventArgs gEventArgs);
        public void ReceiveDataFromModelsState(object sender, EventArgs e)
        {
            try
            {
                GEventArgs gEventArgs = (GEventArgs)e;
                UpdateFormDelegateFromModelsState updateformFromModelsState = new UpdateFormDelegateFromModelsState(UpdateFormFromModelsState);
                this.BeginInvoke(updateformFromModelsState, new object[] { sender, gEventArgs });
            }
            catch (Exception ex)
            { }
        }

        private void UpdateFormFromModelsState(object sender, GEventArgs gEventArgs)
        {
            try
            {
                int biaoHao = gEventArgs.messageID;
                try
                {
                    int countDataGridRowsNow = DataGridView_SysAlarmInfo.Rows.Count;
                    if (dicAlarmShow.ContainsKey(biaoHao))
                    {
                        alarmInfoShowUnit aisu = dicAlarmShow[biaoHao];
                        aisu.biaoHao = biaoHao;
                        aisu.alarm = 1;
                        aisu.info = gEventArgs.message;
                        dicAlarmShow[biaoHao] = aisu;
                    }
                    else
                    {
                        alarmInfoShowUnit aisu = new alarmInfoShowUnit();
                        aisu.biaoHao = biaoHao;
                        aisu.alarm = 1;
                        aisu.info = gEventArgs.message;
                        aisu.rowHao = countDataGridRowsNow++;
                        dicAlarmShow.Add(aisu.biaoHao, aisu);
                    }

                    List<int> listKeys = dicAlarmShow.Keys.ToList();
                    int countRow = dicAlarmShow.Count;
                    if (countRow > DataGridView_SysAlarmInfo.Rows.Count)
                    {
                        int countRowAdd = countRow - DataGridView_SysAlarmInfo.Rows.Count;
                        DataGridView_SysAlarmInfo.Rows.Add(countRowAdd);
                    }
                    else if (countRow < DataGridView_SysAlarmInfo.Rows.Count)
                    {
                        int countRowDel = DataGridView_SysAlarmInfo.Rows.Count - countRow;
                        for (int i = 0; i < countRowDel; i++)
                        {
                            DataGridView_SysAlarmInfo.Rows.RemoveAt(DataGridView_SysAlarmInfo.Rows.Count - 1);
                        }
                    }
                    for (int i = 0; i < countRow; i++)
                    {
                        int iRow = listKeys[i];
                        alarmInfoShowUnit aisu = dicAlarmShow[iRow];

                        DataGridView_SysAlarmInfo.Rows[aisu.rowHao].Cells[0].Value = aisu.info;
                        if (aisu.alarm == 2)
                        {
                            DataGridView_SysAlarmInfo.Rows[aisu.rowHao].DefaultCellStyle.BackColor = Color.Red;
                        }
                        else if (aisu.alarm == 1)
                        {
                            DataGridView_SysAlarmInfo.Rows[aisu.rowHao].DefaultCellStyle.BackColor = Color.Yellow;
                        }
                    }
                    DataGridView_SysAlarmInfo.ClearSelection();
                }
                catch (Exception ex1)
                { }
            }
            catch (Exception ex)
            { }
        }



        private ReaderWriterLock myReaderWriterLock = new ReaderWriterLock();
        private void toolStripMenuItem_DeleteSelected_Click(object sender, EventArgs e)
        {
            try
            {
                int index = DataGridView_SysAlarmInfo.CurrentCellAddress.Y;
                if (index < 0 || index > DataGridView_SysAlarmInfo.Rows.Count)
                {
                    MessageBox.Show("请先选中单元格！");
                    return;
                }


                List<int> listKeys = dicAlarmShow.Keys.ToList();
                int count = listKeys.Count;
                int indexDel = -1;
                for (int i = 0; i < count; i++)
                {
                    alarmInfoShowUnit aisu = dicAlarmShow[listKeys[i]];
                    if (aisu.rowHao == index)
                    {
                        int biaoHao = aisu.biaoHao;
                        myReaderWriterLock.AcquireWriterLock(5);
                        dicAlarmShow.Remove(biaoHao);
                        indexDel = aisu.rowHao;
                        myReaderWriterLock.ReleaseWriterLock();
                        break;
                    }
                }

                listKeys = dicAlarmShow.Keys.ToList();
                int countRow = dicAlarmShow.Count;
                for (int i = 0; i < countRow; i++)
                {
                    int iRow = listKeys[i];
                    alarmInfoShowUnit aisu = dicAlarmShow[iRow];

                    if (aisu.rowHao > indexDel)
                    {
                        aisu.rowHao -= 1;
                        myReaderWriterLock.AcquireWriterLock(2);
                        dicAlarmShow[iRow] = aisu;
                        myReaderWriterLock.ReleaseWriterLock();
                    }
                }

                listKeys = dicAlarmShow.Keys.ToList();
                if (countRow > DataGridView_SysAlarmInfo.Rows.Count)
                {
                    int countRowAdd = countRow - DataGridView_SysAlarmInfo.Rows.Count;
                    DataGridView_SysAlarmInfo.Rows.Add(countRowAdd);
                }
                else if (countRow < DataGridView_SysAlarmInfo.Rows.Count)
                {
                    int countRowDel = DataGridView_SysAlarmInfo.Rows.Count - countRow;
                    for (int i = 0; i < countRowDel; i++)
                    {
                        DataGridView_SysAlarmInfo.Rows.RemoveAt(DataGridView_SysAlarmInfo.Rows.Count - 1);
                    }
                }

                for (int i = 0; i < countRow; i++)
                {
                    int iRow = listKeys[i];
                    alarmInfoShowUnit aisu = dicAlarmShow[iRow];

                    DataGridView_SysAlarmInfo.Rows[aisu.rowHao].Cells[0].Value = aisu.info;
                    if (aisu.alarm == 2)
                    {
                        DataGridView_SysAlarmInfo.Rows[aisu.rowHao].DefaultCellStyle.BackColor = Color.Red;
                    }
                    else if (aisu.alarm == 1)
                    {
                        DataGridView_SysAlarmInfo.Rows[aisu.rowHao].DefaultCellStyle.BackColor = Color.Yellow;
                    }
                }
                DataGridView_SysAlarmInfo.ClearSelection();

            }
            catch (Exception ex)
            { }
        }

        private void toolStripMenuItem_ClearAll_Click(object sender, EventArgs e)
        {
            try
            {
                myReaderWriterLock.AcquireWriterLock(5);
                dicAlarmShow.Clear();
                DataGridView_SysAlarmInfo.Rows.Clear();
                myReaderWriterLock.ReleaseWriterLock();
            }
            catch (Exception ex)
            {

            }
        }

        private void FormMainModeI_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (e.CloseReason == CloseReason.UserClosing)
                {
                    //this.WindowState = FormWindowState.Minimized;
                    //this.Hide();
                    e.Cancel = true;
                }
            }
            catch
            { }
        }

        private void timer_Communication_Tick(object sender, EventArgs e)
        {
            try
            {
                if (Global.isCommucationOK)
                {
                    textBox_Communication.BackColor = Color.Green;
                }
                else
                {
                    textBox_Communication.BackColor = Color.Red;
                }
            }
            catch (Exception ex)
            { }
        }


    }
}
