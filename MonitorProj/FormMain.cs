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
using System.Threading;
using Aspose.Cells;

namespace MonitorProj
{
    public partial class FormMain : Form
    {


        //读取excel配置文件
        private void ReadExcelAlarmConfigure()
        {
            try
            {
                Global.dicStatusParasByStatusName.Clear();
                string TMConfigPath = System.Windows.Forms.Application.StartupPath + "\\configure\\AlarmConfigure.xlsx";
                if (File.Exists(TMConfigPath) == false)
                {
                    MessageBox.Show("配置文件...\\config\\AlarmConfigure.xlsx不存在！");
                    return;
                }

                Workbook workbook = new Workbook(TMConfigPath);
                Worksheet sheet = workbook.Worksheets[0];
                Cells cell = sheet.Cells;
                DataTable dtExcel = cell.ExportDataTableAsString(0, 0, cell.MaxDataRow + 1, cell.MaxDataColumn + 1, true);

                string[] strSplitSlash = new string[1] { "/" };
                string[] strSplitResInfo = new string[] { ":", "[", "]", "：", "【", "】", "，", "," };

                int count = dtExcel.Rows.Count;
                for (int i = 0; i < count; i++)
                {
                    try
                    {
                        //序号,即表序号
                        string bianHaoStr = dtExcel.Rows[i]["序号"].ToString().Replace(" ", "");
                        if (bianHaoStr == null || bianHaoStr == "")
                        {
                            continue;
                        }
                        int bianHao = Convert.ToInt32(bianHaoStr);

                        //状态名称
                        string statusNameStr = dtExcel.Rows[i]["状态名称"].ToString().Replace(" ", "");
                        //状态代号
                        string statusDaiHaoStr = dtExcel.Rows[i]["状态代号"].ToString().Replace(" ", "");
                        if (statusDaiHaoStr == "" || statusDaiHaoStr.Contains("//"))
                        {
                            continue;
                        }

                        //预警值范围
                        List<UnitResultCompareShowInfo> yellowParasTmp = new List<UnitResultCompareShowInfo>();
                        yellowParasTmp.Clear();
                        string strNormalInfo = dtExcel.Rows[i]["预警值范围"].ToString().Replace(" ", "");
                        string[] strSlash = strNormalInfo.Split(strSplitSlash, StringSplitOptions.RemoveEmptyEntries);
                        if (strSlash.Length > 0)
                        {
                            for (int j = 0; j < strSlash.Length; j++)
                            {
                                string[] strResInfo = strSlash[j].Split(strSplitResInfo, StringSplitOptions.RemoveEmptyEntries);
                                if (strResInfo.Length != 3)
                                {
                                    continue;
                                }
                                else
                                {
                                    UnitResultCompareShowInfo unit = new UnitResultCompareShowInfo();
                                    unit.info = strResInfo[0];
                                    bool b = Double.TryParse(strResInfo[1], out unit.min);
                                    if (b == false)
                                    {
                                        continue;
                                    }
                                    b = Double.TryParse(strResInfo[2], out unit.max);
                                    if (b == false)
                                    {
                                        continue;
                                    }
                                    yellowParasTmp.Add(unit);
                                }
                            }
                        }

                        //报警值范围
                        List<UnitResultCompareShowInfo> redParasTmp = new List<UnitResultCompareShowInfo>();
                        redParasTmp.Clear();
                        string strAlarmInfo = dtExcel.Rows[i]["报警值范围"].ToString().Replace(" ", "");
                        strSlash = strAlarmInfo.Split(strSplitSlash, StringSplitOptions.RemoveEmptyEntries);
                        if (strSlash.Length > 0)
                        {
                            for (int j = 0; j < strSlash.Length; j++)
                            {
                                string[] strResInfo = strSlash[j].Split(strSplitResInfo, StringSplitOptions.RemoveEmptyEntries);
                                if (strResInfo.Length != 3)
                                {
                                    continue;
                                }
                                else
                                {
                                    UnitResultCompareShowInfo unit = new UnitResultCompareShowInfo();
                                    unit.info = strResInfo[0];
                                    bool b = Double.TryParse(strResInfo[1], out unit.min);
                                    if (b == false)
                                    {
                                        continue;
                                    }
                                    b = Double.TryParse(strResInfo[2], out unit.max);
                                    if (b == false)
                                    {
                                        continue;
                                    }
                                    redParasTmp.Add(unit);
                                }
                            }
                        }

                        /*
                        //是否报警
                        string isAlarmStr = dtExcel.Rows[i]["是否报警"].ToString().Replace(" ", "");
                        bool isAlarm = false;
                        if (isAlarmStr == null || isAlarmStr == "")
                        {
                            isAlarmStr = "0";
                        }
                        if (isAlarmStr == "0")
                        {
                            isAlarm = false;
                        }
                        else
                        {
                            isAlarm = true;
                        }
                        */

                        StatusUnitParas sup = new StatusUnitParas();
                        sup.bianHao = bianHao;
                        sup.statusName = statusNameStr;
                        sup.statusDaiHao = statusDaiHaoStr;
                        sup.yellowParas = yellowParasTmp;
                        sup.redParas = redParasTmp;
                        //sup.isAlarm = isAlarm;
                        sup.isAlarm = true;

                        Global.dicStatusParasByStatusName.Add(statusDaiHaoStr, sup);
                    }
                    catch (Exception ex)
                    {
                        string sInfo = ex.ToString();
                        sInfo += "\t\n";
                    }
                }
            }
            catch (Exception ex)
            {
                string sInfo = ex.ToString();
                sInfo += "\t\n";
            }
        }



        public FormMain()
        {
            //byte[] bb = new byte[2] { 0x00, 0x01 };
            //UInt16 t = BitConverter.ToUInt16(bb, 0);
            //UInt16 tt = t;
            
            #region 测试代码

            //byte[] bb = new byte[] { 0x31, 0x30, 0x30, 0x2E, 0x30, 0x30, 0x6D, 0x0D, 0x0A };
            //string sRecv = Encoding.ASCII.GetString(bb);

            /*
            int i = 6;
            byte[] b = BitConverter.GetBytes(i);
            byte r = b[0];
            */

            /*
            byte bAddrRecv = 0x60;
            enum_AddressBoard myBoardAddress = (enum_AddressBoard)bAddrRecv;
            enum_AddressBoard myBoardAddress1 = myBoardAddress;
            */

            #endregion

            InitializeComponent();
            //this.skinEngine1.SkinFile = "Page.ssk";

            Global.logFilePathSaved = System.Windows.Forms.Application.StartupPath + "\\Log\\操作记录";
            if (!Directory.Exists(Global.logFilePathSaved))
            {
                Directory.CreateDirectory(Global.logFilePathSaved);
            }
            Global.statusFilePathSaved = System.Windows.Forms.Application.StartupPath + "\\Log\\状态数据";
            if (!Directory.Exists(Global.statusFilePathSaved))
            {
                Directory.CreateDirectory(Global.statusFilePathSaved);
            }
            
            StartLog();

            string configPath = System.Windows.Forms.Application.StartupPath + "\\configure\\configure.xml";

            readXML(configPath);
            
            //数据状态记录
            Global.myDataRecvToSaveClass = new DataRecvToSaveClass();
        }

        //Log文件记录初始化
        public void StartLog()
        {
            try
            {
                
                //发送指令
                Global.logFilePathSaved = Global.logFilePathSaved + "\\操作记录-" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".csv";
                Global.fsLog = new FileStream(Global.logFilePathSaved, FileMode.Create, FileAccess.Write, FileShare.Read);
                Global.myLogStreamWriter = new StreamWriter(Global.fsLog, Encoding.Unicode);
                //首行标题
                string sDataLineForTxt = "时间\t操作内容\t备注\t";
                Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                Global.myLogStreamWriter.Flush();

                ////接收数据，各板卡状态
                //Global.statusFilePathSaved = Global.statusFilePathSaved + "\\状态数据-" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".csv";
                //Global.fsStatus = new FileStream(Global.statusFilePathSaved, FileMode.Create, FileAccess.Write, FileShare.Read);
                //Global.myStatusStreamWriter = new StreamWriter(Global.fsStatus, Encoding.Unicode);
                ////首行标题
                //sDataLineForTxt = "时间\t" +
                //    "电压(阀箱)\t" +

                //    "高度\t"
                //    ;
                //Global.myStatusStreamWriter.WriteLine(sDataLineForTxt);
                //Global.myStatusStreamWriter.Flush();
            }
            catch (Exception ex)
            { }
        }

        //Log文件记录结束
        public void StopLog()
        {
            try
            {
                if (Global.fsLog != null && Global.fsLog.CanWrite)
                {
                    Global.fsLog.Flush();
                    Global.fsLog.Close();
                }
            }
            catch (Exception ex)
            { }
        }


        private int topOffset = 5;
        private int leftOffset = 5;
        //private string sMonText = "状态与监视";
        //private string sMonName = "FormMonMain";
        //private string sCtlText = "控制与执行";
        //private string sCtlName = "FormCtlMain";
        //private Form myFormCtl;
        //private Form myFormMon;
        private string sFormMainUserAName = "FormMainUserA";//用户界面
        private string sFormMainUserAText = "监控界面";
        private void ToolStripMenuItem_ModeI_Click(object sender, EventArgs e)
        {
            try
            {
                Global.myFormMainUserA = new FormMainUserA();
                Global.myFormMainUserA.MdiParent = this;
                Global.myFormMainUserA.Name = sFormMainUserAName;
                Global.myFormMainUserA.Text = sFormMainUserAText;


                if (ToolStripMenuItem_ModeI.Checked)
                {
                    if (Global.myFormMainModeI == null)
                    {
                        Global.myFormMainModeI = new FormMainModeI();
                        Global.myFormMainModeI.MdiParent = this;
                        Global.myFormMainModeI.Name = "FormMainModeI";
                        Global.myFormMainModeI.Text = "系统模式I";
                        Global.myFormMainModeI.Show();
                    }
                    ToolStripMenuItem_ModeII.Checked = false;
                    ToolStripMenuItem_ModeIII.Checked = false;


                    Global.m_FormMobileDrillMonCtl.EventBtnStatusChanged +=
                        new EventHandler(Global.myFormMainUserA.ReceiveDataFromButtonStatusChanged);
                    Global.m_FormBoardI.EventBtnStatusChanged +=
                        new EventHandler(Global.myFormMainUserA.ReceiveDataFromButtonStatusChanged);
                    Global.m_FormBoardII.EventBtnStatusChanged +=
                        new EventHandler(Global.myFormMainUserA.ReceiveDataFromButtonStatusChanged);
                    Global.m_FormSerialWaterBoxCtl.EventBtnStatusChanged +=
                        new EventHandler(Global.myFormMainUserA.ReceiveDataFromButtonStatusChanged);
                    Global.m_FormSerialRovPowerCtl.EventBtnStatusChanged +=
                        new EventHandler(Global.myFormMainUserA.ReceiveDataFromButtonStatusChanged);

                    Global.myBoardSerialMonCtlWaterBoxClass.EventSerialDataSend +=
                        new EventHandler(Global.myFormMainUserA.ReceiveDataFromSerial);
                    Global.myBoardSerialMonCtlRovPowerClass.EventSerialDataSend +=
                        new EventHandler(Global.myFormMainUserA.ReceiveDataFromSerial);
                    Global.myBoardSerialMonCtlJuYuanJianCe1Class.EventSerialDataSend +=
                        new EventHandler(Global.myFormMainUserA.ReceiveDataFromSerial);
                    Global.myBoardSerialMonCtlJuYuanJianCe2Class.EventSerialDataSend +=
                        new EventHandler(Global.myFormMainUserA.ReceiveDataFromSerial);
                    
                    Control ctlSplitContainer = Global.myFormMainModeI.Controls["splitContainer1"];
                    Control ctlPanelMain = ctlSplitContainer.Controls[1].Controls["panel_FormContainer"];
                    Control ctlPanelAlarmRunning = ctlSplitContainer.Controls[1].Controls["Panel_AlarmAndRunningInfo"];
                    Control ctlFormBoardCMonCtl = ctlPanelMain.Controls["BoardCMonCtl"];
                    Control ctlFormBoardAMonCtl = ctlPanelMain.Controls["BoardAMonCtl"];
                    Control ctlFormBoardBMonCtl = ctlPanelMain.Controls["BoardBMonCtl"];
                    Control ctlFormBoardDMonCtl_WaterBox = ctlPanelMain.Controls["BoardDMonCtl_WaterBox"];
                    Control ctlFormBoardDMonCtl_RovPower = ctlPanelMain.Controls["BoardDMonCtl_RovPower"];
                    Control ctlFormBoardDMonCtl_JuYuanJianCe_1 = ctlPanelMain.Controls["BoardDMonCtl_JuYuanJianCe_1"];
                    Control ctlFormBoardDMonCtl_JuYuanJianCe_2 = ctlPanelMain.Controls["BoardDMonCtl_JuYuanJianCe_2"];

                    
                    #region 控制窗口

                    Control ctlFormBoardCCtl = ctlFormBoardCMonCtl.Controls["groupBox_BoardC_Ctl"];
                    Control ctlFormBoardACtl = ctlFormBoardAMonCtl.Controls["groupBox_BoardA_Ctl"];
                    Control ctlFormBoardBCtl = ctlFormBoardBMonCtl.Controls["groupBox_BoardB_Ctl"];
                    Control ctlFormBoardDCtl_WaterBox = ctlFormBoardDMonCtl_WaterBox.Controls["groupBox_BoardD_Ctl"];
                    Control ctlFormBoardDCtl_RovPower = ctlFormBoardDMonCtl_RovPower.Controls["groupBox_BoardD_Ctl"];
                    Control ctlFormBoardDCtl_JuYuanJianCe_1 = ctlFormBoardDMonCtl_JuYuanJianCe_1.Controls["groupBox_BoardD_Ctl"];
                    Control ctlFormBoardDCtl_JuYuanJianCe_2 = ctlFormBoardDMonCtl_JuYuanJianCe_2.Controls["groupBox_BoardD_Ctl"];

                    Control ctlFormBoardCCtl2 = ctlFormBoardCMonCtl.Controls["groupBox_HeadingCircle_2"];
                    
                    
                    Global.myFormCtl = new Form();

                    SplitContainer mySplitContainerCtl = new SplitContainer();
                    mySplitContainerCtl.Name = "mySplitContainer";
                    Global.myFormCtl.Controls.Add(mySplitContainerCtl);
                    mySplitContainerCtl.Dock = DockStyle.Fill;
                    mySplitContainerCtl.BackColor = Color.DarkSeaGreen;
                    mySplitContainerCtl.Panel1.AutoScroll = true;
                    mySplitContainerCtl.Panel2.AutoScroll = true;
                    mySplitContainerCtl.SplitterDistance = 92;
                    
                    SplitContainer mySplitContainerCtl2 = new SplitContainer();
                    mySplitContainerCtl2.Name = "mySplitContainer2";
                    mySplitContainerCtl.Panel2.Controls.Add(mySplitContainerCtl2);
                    mySplitContainerCtl2.Dock = DockStyle.Fill;
                    mySplitContainerCtl2.BackColor = Color.DarkSeaGreen;
                    mySplitContainerCtl2.Panel1.AutoScroll = true;
                    mySplitContainerCtl2.Panel2.AutoScroll = true;
                    mySplitContainerCtl2.SplitterDistance = 88;

                    mySplitContainerCtl.Panel1.Controls.Add(ctlFormBoardCCtl);
                    ctlFormBoardCCtl.Location = new Point(leftOffset, topOffset);

                    //mySplitContainerCtl.Panel2.Controls.Add(ctlFormBoardACtl);
                    //ctlFormBoardACtl.Location = new Point(leftOffset, topOffset);
                    //mySplitContainerCtl.Panel2.Controls.Add(ctlFormBoardBCtl);
                    //ctlFormBoardBCtl.Location = new Point(leftOffset, topOffset + ctlFormBoardACtl.Height + topOffset);


                    mySplitContainerCtl2.Panel1.Controls.Add(ctlFormBoardACtl);
                    ctlFormBoardACtl.Location = new Point(leftOffset, topOffset);
                    mySplitContainerCtl2.Panel1.Controls.Add(ctlFormBoardDCtl_WaterBox);
                    ctlFormBoardDCtl_WaterBox.Location = new Point(leftOffset, ctlFormBoardACtl.Height + topOffset * 2);
                    mySplitContainerCtl2.Panel1.Controls.Add(ctlFormBoardDCtl_RovPower);
                    ctlFormBoardDCtl_RovPower.Location = new Point(leftOffset, ctlFormBoardACtl.Height + ctlFormBoardDCtl_WaterBox.Height + topOffset * 4);

                    mySplitContainerCtl2.Panel1.Controls.Add(ctlFormBoardDCtl_JuYuanJianCe_1);
                    ctlFormBoardDCtl_JuYuanJianCe_1.Location = new Point(leftOffset, ctlFormBoardACtl.Height + ctlFormBoardDCtl_WaterBox.Height + ctlFormBoardDCtl_RovPower.Height + topOffset * 6);
                    mySplitContainerCtl2.Panel1.Controls.Add(ctlFormBoardDCtl_JuYuanJianCe_2);
                    ctlFormBoardDCtl_JuYuanJianCe_2.Location = new Point(leftOffset, ctlFormBoardACtl.Height + ctlFormBoardDCtl_WaterBox.Height + ctlFormBoardDCtl_RovPower.Height + ctlFormBoardDCtl_JuYuanJianCe_1.Height + topOffset * 8);

                    mySplitContainerCtl2.Panel1.Controls.Add(ctlFormBoardCCtl2);
                    ctlFormBoardCCtl2.Location = new Point(leftOffset, ctlFormBoardACtl.Height + ctlFormBoardDCtl_WaterBox.Height + ctlFormBoardDCtl_RovPower.Height + ctlFormBoardDCtl_JuYuanJianCe_1.Height + ctlFormBoardDCtl_JuYuanJianCe_2.Height + topOffset * 10);

                    mySplitContainerCtl2.Panel2.Controls.Add(ctlFormBoardBCtl);
                    ctlFormBoardBCtl.Location = new Point(leftOffset, topOffset);


                    //myForm.Controls.Add(ctl4);
                    //ctl4.Location = new Point(myForm.Location.X + 10, myForm.Location.Y + 20);
                    

                    //mySplitContainerCtl.Panel2.Controls.Add(ct23);
                    //ct23.Location = new Point(mySplitContainer.Panel2.Location.X + 10, mySplitContainer.Panel2.Location.Y + 20);

                    Global.myFormCtl.FormClosing += new FormClosingEventHandler(FormCtlMon_FormClosing);
                    Global.myFormCtl.MdiParent = this;

                    //myFormCtl.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                    //myFormCtl.TopLevel = false;
                    ////TabControl_Main.TabPages["tabPage_Ctl"].Controls.Add(myFormCtl);
                    //TabControl_Main.TabPages[0].Controls.Add(myFormCtl);
                    //myFormCtl.Dock = DockStyle.Fill;

                    Global.myFormCtl.Name = Global.sCtlName;
                    Global.myFormCtl.Text = Global.sCtlText;
                    Global.myFormCtl.MinimizeBox = false;
                    Global.myFormCtl.MaximizeBox = false;
                    //myFormCtl.ControlBox = false;
                    Global.myFormCtl.Width = 1920;
                    Global.myFormCtl.Height = 1080;
                    Global.myFormCtl.WindowState = FormWindowState.Maximized;
                    Global.myFormCtl.Show();

                    #endregion

                    #region 监测窗口

                    Control ctFormBoardCMon = ctlFormBoardCMonCtl.Controls["groupBox_BoardC_Mon"];
                    Control ctlFormBoardAMon = ctlFormBoardAMonCtl.Controls["groupBox_BoardA_Mon"];
                    Control ctlFormBoardBMon = ctlFormBoardBMonCtl.Controls["groupBox_BoardB_Mon"];
                    Control ctlFormBoardDMon_WaterBox = ctlFormBoardDMonCtl_WaterBox.Controls["groupBox_BoardD_Mon"];
                    Control ctlFormBoardDMon_RovPower = ctlFormBoardDMonCtl_RovPower.Controls["groupBox_BoardD_Mon"];
                    Control ctlFormBoardDMon_JuYuanJianCe_1 = ctlFormBoardDMonCtl_JuYuanJianCe_1.Controls["groupBox_BoardD_Mon"];
                    Control ctlFormBoardDMon_JuYuanJianCe_2 = ctlFormBoardDMonCtl_JuYuanJianCe_2.Controls["groupBox_BoardD_Mon"];

                    Global.myFormMon = new Form();

                    SplitContainer mySplitContainerMon = new SplitContainer();
                    mySplitContainerMon.Name = "mySplitContainerMon";
                    Global.myFormMon.Controls.Add(mySplitContainerMon);
                    mySplitContainerMon.Dock = DockStyle.Fill;
                    mySplitContainerMon.BackColor = Color.DarkSeaGreen;
                    mySplitContainerMon.Panel1.AutoScroll = true;
                    mySplitContainerMon.Panel2.AutoScroll = true;
                    mySplitContainerMon.SplitterDistance = 133;

                    mySplitContainerMon.Panel1.Controls.Add(ctFormBoardCMon);
                    ctFormBoardCMon.Location = new Point(leftOffset, topOffset);


                    SplitContainer mySplitContainerMon2 = new SplitContainer();
                    mySplitContainerMon2.Name = "mySplitContainerMon2";
                    mySplitContainerMon.Panel2.Controls.Add(mySplitContainerMon2);
                    mySplitContainerMon2.Dock = DockStyle.Fill;
                    mySplitContainerMon2.BackColor = Color.DarkSeaGreen;
                    mySplitContainerMon2.Panel1.AutoScroll = true;
                    mySplitContainerMon2.Panel2.AutoScroll = true;
                    mySplitContainerMon2.SplitterDistance = 102;
                    
                    mySplitContainerMon2.Panel1.Controls.Add(ctlFormBoardAMon);
                    ctlFormBoardAMon.Location = new Point(leftOffset, topOffset);
                    mySplitContainerMon2.Panel1.Controls.Add(ctlFormBoardBMon);
                    ctlFormBoardBMon.Location = new Point(leftOffset, ctlFormBoardAMon.Height + topOffset * 2);
                    mySplitContainerMon2.Panel1.Controls.Add(ctlFormBoardDMon_WaterBox);
                    ctlFormBoardDMon_WaterBox.Location = new Point(leftOffset, ctlFormBoardAMon.Height + ctlFormBoardBMon.Height + topOffset * 4);
                    mySplitContainerMon2.Panel1.Controls.Add(ctlFormBoardDMon_RovPower);
                    ctlFormBoardDMon_RovPower.Location = new Point(leftOffset, ctlFormBoardAMon.Height + ctlFormBoardBMon.Height + ctlFormBoardDMon_WaterBox.Height + topOffset * 6);

                    mySplitContainerMon2.Panel1.Controls.Add(ctlFormBoardDMon_JuYuanJianCe_1);
                    ctlFormBoardDMon_JuYuanJianCe_1.Location = new Point(leftOffset, ctlFormBoardAMon.Height + ctlFormBoardBMon.Height + ctlFormBoardDMon_WaterBox.Height + ctlFormBoardDMon_RovPower.Height + topOffset * 8);
                    mySplitContainerMon2.Panel1.Controls.Add(ctlFormBoardDMon_JuYuanJianCe_2);
                    ctlFormBoardDMon_JuYuanJianCe_2.Location = new Point(leftOffset, ctlFormBoardAMon.Height + ctlFormBoardBMon.Height + ctlFormBoardDMon_WaterBox.Height + ctlFormBoardDMon_RovPower.Height + ctlFormBoardDMon_JuYuanJianCe_1.Height + topOffset * 10);

                    ctlPanelAlarmRunning.Dock = DockStyle.Fill;
                    mySplitContainerMon2.Panel2.Controls.Add(ctlPanelAlarmRunning);
                    ctlPanelAlarmRunning.Location = new Point(leftOffset, topOffset);

                    Global.dicFormHandleIntPtr["BoardCMonCtl"] = ctlFormBoardCMonCtl.Handle;
                    Global.dicFormHandleIntPtr["BoardAMonAtl"] = ctlFormBoardAMonCtl.Handle;
                    Global.dicFormHandleIntPtr["BoardBMonAtl"] = ctlFormBoardBMonCtl.Handle;
                    Global.dicFormHandleIntPtr["BoardDMonAtl_WaterBox"] = ctlFormBoardDMonCtl_WaterBox.Handle;
                    Global.dicFormHandleIntPtr["BoardDMonAtl_RovPower"] = ctlFormBoardDMonCtl_RovPower.Handle;

                    Global.myFormMon.FormClosing += new FormClosingEventHandler(FormCtlMon_FormClosing);
                    Global.myFormMon.MdiParent = this;

                    //myFormMon.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                    //myFormMon.TopLevel = false;
                    ////TabControl_Main.TabPages["tabPage_Ctl"].Controls.Add(myFormMon);
                    //TabControl_Main.TabPages[1].Controls.Add(myFormMon);
                    //myFormMon.Dock = DockStyle.Fill;

                    Global.myFormMon.Name = "FormMonMain";
                    Global.myFormMon.Text = "状态与监视";
                    Global.myFormMon.MinimizeBox = false;
                    Global.myFormMon.MaximizeBox = false;
                    //myFormMon.ControlBox = false;
                    Global.myFormMon.Width = 1920;
                    Global.myFormMon.Height = 1080;
                    Global.myFormMon.WindowState = FormWindowState.Maximized;
                    Global.myFormMon.Show();


                    Global.myFormCtl.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                    Global.myFormCtl.TopLevel = false;
                    Global.myFormMon.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                    Global.myFormMon.TopLevel = false;

                    TabControl_Debug.TabPages[0].Text = "控制与执行";
                    TabControl_Debug.TabPages[0].Controls.Add(Global.myFormCtl);
                    Global.myFormCtl.Dock = DockStyle.Fill;

                    TabControl_Debug.TabPages[1].Text = "状态与监视";
                    TabControl_Debug.TabPages[1].Controls.Add(Global.myFormMon);
                    Global.myFormMon.Dock = DockStyle.Fill;


                    #endregion

                    #region 用户监控主界面

                    //Global.myFormMainUserA = new FormMainUserA();
                    //Global.myFormMainUserA.MdiParent = this;
                    //Global.myFormMainUserA.Name = sFormMainUserAName;
                    //Global.myFormMainUserA.Text = sFormMainUserAText;
                    ////Global.myFormMainUserA.MinimizeBox = false;
                    ////Global.myFormMainUserA.MaximizeBox = false;
                    ////Global.myFormMainUserA.ControlBox = false;
                    ////Global.myFormMainUserA.WindowState = FormWindowState.Maximized;
                    ////Global.myFormMainUserA.Show();


                    Global.myFormMainUserA.Show();
                    //Global.myFormMainUserA.richTextBox_InfoShow.AppendText(Global.myFormMonCtlA.richTextBox_InfoShow.Text);

                    #region 用户主界面分为控制界面、状态界面 2019/03/06
                    ToolStrip_Main.Visible = false;
                    TabControl_Main.Dock = DockStyle.Fill;

                    //控制页面
                    Global.myFormUserMainCtl.Controls.Add(Global.myFormMainUserA.GroupBox_MainCtl);
                    Global.myFormMainUserA.GroupBox_MainCtl.Dock = DockStyle.Fill;
                    Global.myFormUserMainCtl.MdiParent = this;

                    Global.myFormUserMainCtl.Name = Global.sFormUserMainCtlName;
                    Global.myFormUserMainCtl.Text = "控制与执行";
                    Global.myFormUserMainCtl.MinimizeBox = false;
                    Global.myFormUserMainCtl.MaximizeBox = false;
                    //Global.myFormUserMainCtl.ControlBox = false;
                    Global.myFormUserMainCtl.Width = 1920;
                    Global.myFormUserMainCtl.Height = 1080;
                    //Global.myFormUserMainCtl.WindowState = FormWindowState.Maximized;
                    Global.myFormUserMainCtl.Show();
                    Global.myFormUserMainCtl.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                    Global.myFormUserMainCtl.TopLevel = false;
                    Global.myFormUserMainCtl.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
                    TabControl_Main.TabPages[0].Text = "控制与执行";
                    TabControl_Main.TabPages[0].Controls.Add(Global.myFormUserMainCtl);
                    Global.myFormUserMainCtl.Dock = DockStyle.Fill;
                    Global.myFormUserMainCtl.FormClosing += new FormClosingEventHandler(myFormUserMainCtlMon_FormClosing);

                    //状态页面
                    Global.myFormUserMainMon.Controls.Add(Global.myFormMainUserA.GroupBox_MainMon);
                    Global.myFormMainUserA.GroupBox_MainMon.Dock = DockStyle.Fill;
                    Global.myFormUserMainMon.MdiParent = this;

                    Global.myFormUserMainMon.Name = Global.sFormUserMainMonName;
                    Global.myFormUserMainMon.Text = "状态与监视";
                    Global.myFormUserMainMon.MinimizeBox = false;
                    Global.myFormUserMainMon.MaximizeBox = false;
                    //Global.myFormUserMainMon.ControlBox = false;
                    Global.myFormUserMainMon.Width = 1920;
                    Global.myFormUserMainMon.Height = 1080;
                    //Global.myFormUserMainMon.WindowState = FormWindowState.Maximized;
                    Global.myFormUserMainMon.Show();
                    Global.myFormUserMainMon.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                    Global.myFormUserMainMon.TopLevel = false;
                    Global.myFormUserMainMon.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
                    TabControl_Main.TabPages[1].Text = "状态与监视";
                    TabControl_Main.TabPages[1].Controls.Add(Global.myFormUserMainMon);
                    Global.myFormUserMainMon.Dock = DockStyle.Fill;
                    Global.myFormUserMainMon.FormClosing += new FormClosingEventHandler(myFormUserMainCtlMon_FormClosing);

                    #endregion

                    ////调试用
                    if (Global.isDebug == 1)
                    {
                        TabControl_Main.Visible = false;
                        TabControl_Debug.Dock = DockStyle.Fill;
                        TabControl_Debug.BringToFront();
                    }
                    else
                    {
                        TabControl_Main.Visible = true;
                        TabControl_Main.Dock = DockStyle.Fill;
                        TabControl_Main.BringToFront();
                    }
                    //////////////


                    #region 参考代码，old
                    /*
                    ToolStripButton tsiCtl = new ToolStripButton
                    {
                        Text = sCtlText,
                        Name = sCtlName + "Tsi",
                        ToolTipText = sCtlText
                    };
                    tsiCtl.Click += toolStripForms_Click;
                    tsiCtl.ForeColor = Color.Blue;
                    tsiCtl.Font = new System.Drawing.Font("宋体", 10, FontStyle.Regular);
                    ToolStrip_Main.Items.Add(tsiCtl);

                    ToolStripSeparator tssCtl = new ToolStripSeparator();
                    tssCtl.Name = sCtlName + "Tss";
                    ToolStrip_Main.Items.Add(tssCtl);

                    ToolStripButton tsiMon = new ToolStripButton
                    {
                        Text = sMonText,
                        Name = sMonName + "Tsi",
                        ToolTipText = sMonText
                    };
                    tsiMon.Click += toolStripForms_Click;
                    tsiMon.ForeColor = Color.Blue;
                    tsiMon.Font = new System.Drawing.Font("宋体", 10, FontStyle.Regular);
                    ToolStrip_Main.Items.Add(tsiMon);
                    tsiMon.BackColor = Color.LightPink;

                    ToolStripSeparator tssMon = new ToolStripSeparator();
                    tssMon.Name = sMonName + "Tss";
                    ToolStrip_Main.Items.Add(tssMon);
                    */

                    #endregion

                    #endregion

                    //Global.myFormMainModeI.Visible = false;
                    
                }
            }
            catch (Exception ex)
            { }
        }

        private void myFormUserMainCtlMon_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (e.CloseReason == CloseReason.UserClosing)
                {
                    e.Cancel = true;
                }
            }
            catch (Exception ex)
            { }
        }

        private void toolStripForms_Click(object sender, EventArgs e)
        {
            try
            {
                string sName = null;
                if (sender.GetType().ToString() == "System.Windows.Forms.ToolStripButton")
                {
                    ToolStripButton tsi = (ToolStripButton)sender;
                    sName = tsi.Name;
                    sName = sName.Replace("Tsi", "").Replace("Tss", "");
                    if (sName == Global.sCtlName)
                    {
                        Global.myFormCtl.WindowState = FormWindowState.Maximized;
                        Global.myFormCtl.BringToFront();
                    }
                    else if (sName == Global.sMonName)
                    {
                        Global.myFormMon.WindowState = FormWindowState.Maximized;
                        Global.myFormMon.BringToFront();
                    }

                    int count = ToolStrip_Main.Items.Count;
                    for (int i = 0; i < count; i++)
                    {
                        if (ToolStrip_Main.Items[i].Name.Contains(sName))
                        {
                            ToolStrip_Main.Items[i].BackColor = Color.LightPink;
                        }
                        else
                        {
                            ToolStrip_Main.Items[i].BackColor = Color.Transparent;
                        }
                    }
                }
            }
            catch (Exception ex)
            { }
        }

        private void FormCtlMon_FormClosing(object sender, FormClosingEventArgs e)
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

        private void ToolStripMenuItem_ScreenOtherShow_Click(object sender, EventArgs e)
        {
            #region 实现方式一Old
            /*
            try
            {
                Form activeForm = ActiveMdiChild;
                activeForm.MdiParent = null;

                Screen[] sc;
                sc = Screen.AllScreens;
                if (sc.Length <= 1)
                {
                    return;
                }

                activeForm.StartPosition = FormStartPosition.Manual;
                activeForm.Location = new Point(sc[1].Bounds.Left, sc[1].Bounds.Top);
                // If you intend the form to be maximized, change it to normal then maximized.
                activeForm.WindowState = FormWindowState.Normal;
                activeForm.WindowState = FormWindowState.Maximized;

                string sName = activeForm.Name;

                int count = ToolStrip_Main.Items.Count;
                for (int i = 0; i < count; i++)
                {
                    if (ToolStrip_Main.Items[i].Name.Replace("Tsi", "").Equals(sName) == false)
                    {
                        ToolStrip_Main.Items[i].PerformClick();
                        break;
                    }
                }
            }
            catch (Exception ex)
            { }
            */
            #endregion

            try
            {
                if (Global.isDebug == 1)
                {
                    Screen[] sc;
                    sc = Screen.AllScreens;
                    if (sc.Length <= 1)
                    {
                        return;
                    }

                    while ((Global.myFormMon.MdiParent != this))
                    {
                        Global.myFormMon.MdiParent = this;
                        Thread.Sleep(10);
                    }

                    Global.myFormMon.MdiParent = null;
                    while ((Global.myFormMon.MdiParent != null))
                    {
                        Global.myFormMon.MdiParent = this;
                        Thread.Sleep(10);
                    }

                    Global.myFormMon.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;

                    Global.myFormMon.StartPosition = FormStartPosition.Manual;
                    Global.myFormMon.Location = new Point(sc[1].Bounds.Left, sc[1].Bounds.Top);
                    // If you intend the form to be maximized, change it to normal then maximized.
                    Global.myFormMon.WindowState = FormWindowState.Normal;
                    Global.myFormMon.Width = 1920;
                    Global.myFormMon.Height = 1080;
                    Thread.Sleep(20);
                    Global.myFormMon.WindowState = FormWindowState.Maximized;

                    Thread.Sleep(20);

                    while ((Global.myFormCtl.MdiParent != this))
                    {
                        Global.myFormCtl.MdiParent = this;
                        Thread.Sleep(10);
                    }
                    Global.myFormCtl.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;

                    Global.myFormCtl.StartPosition = FormStartPosition.Manual;
                    // If you intend the form to be maximized, change it to normal then maximized.
                    Global.myFormCtl.WindowState = FormWindowState.Normal;
                    Thread.Sleep(20);
                    Global.myFormCtl.WindowState = FormWindowState.Maximized;

                    Thread.Sleep(20);
                    TabControl_Debug.Visible = false;
                }
                else
                {
                    Screen[] sc;
                    sc = Screen.AllScreens;
                    if (sc.Length <= 1)
                    {
                        //return;
                    }

                    TabControl_Main.TabPages[1].Show();
                    Thread.Sleep(10);

                    while ((Global.myFormUserMainMon.MdiParent != this))
                    {
                        Global.myFormUserMainMon.MdiParent = this;
                        Thread.Sleep(10);
                    }

                    Global.myFormUserMainMon.MdiParent = null;
                    while ((Global.myFormUserMainMon.MdiParent != null))
                    {
                        Global.myFormUserMainMon.MdiParent = this;
                        Thread.Sleep(10);
                    }

                    Global.myFormUserMainMon.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;

                    Global.myFormUserMainMon.StartPosition = FormStartPosition.Manual;

                    Global.myFormUserMainMon.Location = new Point(sc[1].Bounds.Left, sc[1].Bounds.Top);
                    //Global.myFormUserMainMon.Location = new Point(sc[0].Bounds.Left, sc[0].Bounds.Top);

                    // If you intend the form to be maximized, change it to normal then maximized.
                    Global.myFormUserMainMon.WindowState = FormWindowState.Normal;
                    Global.myFormUserMainMon.Width = 1920;
                    Global.myFormUserMainMon.Height = 1080;
                    Thread.Sleep(20);
                    Global.myFormUserMainMon.WindowState = FormWindowState.Maximized;

                    Thread.Sleep(20);

                    while ((Global.myFormUserMainCtl.MdiParent != this))
                    {
                        Global.myFormUserMainCtl.MdiParent = this;
                        Thread.Sleep(10);
                    }
                    Global.myFormUserMainCtl.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;

                    Global.myFormUserMainCtl.StartPosition = FormStartPosition.Manual;
                    // If you intend the form to be maximized, change it to normal then maximized.
                    Global.myFormUserMainCtl.WindowState = FormWindowState.Normal;
                    Thread.Sleep(20);
                    TabControl_Debug.SendToBack();
                    Global.myFormUserMainCtl.WindowState = FormWindowState.Maximized;

                    Thread.Sleep(20);
                    TabControl_Main.Visible = false;
                }
            }
            catch (Exception ex)
            { }
        }

        private void ToolStripMenuItem_ScreenOtherShowCancel_Click(object sender, EventArgs e)
        {
            #region 实现方式一Old
            /*
            try
            {
                if (Global.myFormCtl.MdiParent != this)
                {
                    Global.myFormCtl.MdiParent = this;
                    Global.myFormCtl.WindowState = FormWindowState.Maximized;
                    string sName = Global.myFormCtl.Name;
                    int count = ToolStrip_Main.Items.Count;
                    for (int i = 0; i < count; i++)
                    {
                        if (ToolStrip_Main.Items[i].Name.Replace("Tsi","").Equals(sName))
                        {
                            ToolStrip_Main.Items[i].PerformClick();
                            break;
                        }
                    }
                }
                if (Global.myFormMon.MdiParent != this)
                {
                    Global.myFormMon.MdiParent = this;
                    Global.myFormMon.WindowState = FormWindowState.Maximized;
                    string sName = Global.myFormMon.Name;
                    int count = ToolStrip_Main.Items.Count;
                    for (int i = 0; i < count; i++)
                    {
                        if (ToolStrip_Main.Items[i].Name.Replace("Tsi", "").Equals(sName))
                        {
                            ToolStrip_Main.Items[i].PerformClick();
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            { }
            */
            #endregion

            try
            {
                if (Global.isDebug == 1)
                {
                    TabControl_Debug.Visible = true;

                    Global.myFormCtl.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                    Global.myFormCtl.TopLevel = false;
                    TabControl_Debug.TabPages[0].Controls.Add(Global.myFormCtl);
                    Global.myFormCtl.Dock = DockStyle.Fill;

                    Thread.Sleep(10);

                    Global.myFormMon.MdiParent = this;
                    Thread.Sleep(10);
                    while (Global.myFormMon.MdiParent != this)
                    {
                        Global.myFormMon.MdiParent = this;
                        Thread.Sleep(5);
                    }
                    Global.myFormMon.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                    Global.myFormMon.TopLevel = false;
                    Thread.Sleep(20);
                    TabControl_Debug.TabPages[1].Controls.Add(Global.myFormMon);
                    Global.myFormMon.Dock = DockStyle.Fill;

                    TabControl_Debug.Visible = true;
                    TabControl_Debug.Dock = DockStyle.Fill;
                }
                else
                {
                    TabControl_Main.Visible = true;

                    Global.myFormUserMainCtl.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                    Global.myFormUserMainCtl.TopLevel = false;
                    TabControl_Main.TabPages[0].Controls.Add(Global.myFormUserMainCtl);
                    Global.myFormUserMainCtl.Dock = DockStyle.Fill;

                    Thread.Sleep(10);

                    Global.myFormUserMainMon.MdiParent = this;
                    Thread.Sleep(10);
                    while (Global.myFormUserMainMon.MdiParent != this)
                    {
                        Global.myFormUserMainMon.MdiParent = this;
                        Thread.Sleep(5);
                    }
                    Global.myFormUserMainMon.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                    Global.myFormUserMainMon.TopLevel = false;
                    Thread.Sleep(20);
                    TabControl_Main.TabPages[1].Controls.Add(Global.myFormUserMainMon);
                    Global.myFormUserMainMon.Dock = DockStyle.Fill;

                    TabControl_Main.Visible = true;
                    TabControl_Main.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            { }

        }

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                SaveTuiJinQiBuChangConfig();
                SaveZiDongDingXiangConfig();
                SaveZiDongDingGaoConfig();

                if (Global.myFormMainModeI != null)
                {
                    Global.myFormMainModeI.Visible = true;
                    Global.myFormMainModeI.Close();
                }

                StopLog();
            }
            catch (Exception ex)
            { }
        }



        private void readXML(string configPath)//读取配置文件
        {
            try
            {
                Global.DicModeBoardTypeSerialConf.Clear();
                Dictionary<enum_BoardType, List<struct_SerialConfParas>> dicTemp1 = new Dictionary<enum_BoardType, List<struct_SerialConfParas>>();
                Dictionary<enum_BoardType, List<struct_SerialConfParas>> dicTemp2 = new Dictionary<enum_BoardType, List<struct_SerialConfParas>>();
                Dictionary<enum_BoardType, List<struct_SerialConfParas>> dicTemp3 = new Dictionary<enum_BoardType, List<struct_SerialConfParas>>();
                Global.DicModeBoardTypeSerialConf.Add(enum_ModeType.Mode1, dicTemp1);
                Global.DicModeBoardTypeSerialConf.Add(enum_ModeType.Mode2, dicTemp2);
                Global.DicModeBoardTypeSerialConf.Add(enum_ModeType.Mode3, dicTemp3);

                if (File.Exists(configPath))
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(configPath);
                    XmlElement xmlroot = xmlDoc.DocumentElement;

                    XmlNodeList serials = xmlroot.SelectNodes("//localnet");
                    string sAddressLocal = serials[0].SelectSingleNode("address").InnerText.Replace(" ", "");
                    IPAddress.TryParse(sAddressLocal, out Global.ipAddressLocal);

                    string sIsDebug = serials[0].SelectSingleNode("debug").InnerText.Replace(" ", "");
                    int.TryParse(sIsDebug, out Global.isDebug);

                    try
                    {
                        XmlNodeList times = xmlroot.SelectNodes("//times");

                        string sDrillUp = times[0].SelectSingleNode("DrillUp").InnerText.Replace(" ", "");
                        string sDrillDown = times[0].SelectSingleNode("DrillDown").InnerText.Replace(" ", "");
                        string sRotateForward = times[0].SelectSingleNode("RotateForward").InnerText.Replace(" ", "");
                        string sRotateReverse = times[0].SelectSingleNode("RotateReverse").InnerText.Replace(" ", "");

                        string sPitchUp = times[0].SelectSingleNode("PitchUp").InnerText.Replace(" ", "");
                        string sPitchDown = times[0].SelectSingleNode("PitchDown").InnerText.Replace(" ", "");
                        string sPitchForward = times[0].SelectSingleNode("PitchForward").InnerText.Replace(" ", "");
                        string sPitchReverse = times[0].SelectSingleNode("PitchReverse").InnerText.Replace(" ", "");

                        double.TryParse(sDrillUp, out Global.dDrillUp);
                        if (Global.dDrillUp < 200)
                        {
                            Global.dDrillUp = 200;
                        }
                        double.TryParse(sDrillDown, out Global.dDrillDown);
                        if (Global.dDrillDown < 200)
                        {
                            Global.dDrillDown = 200;
                        }
                        double.TryParse(sRotateForward, out Global.dRotateForward);
                        if (Global.dRotateForward < 200)
                        {
                            Global.dRotateForward = 200;
                        }
                        double.TryParse(sRotateReverse, out Global.dRotateReverse);
                        if (Global.dRotateReverse < 200)
                        {
                            Global.dRotateReverse = 200;
                        }

                        double.TryParse(sPitchUp, out Global.dPitchUp);
                        if (Global.dPitchUp < 300)
                        {
                            Global.dPitchUp = 300;
                        }
                        double.TryParse(sPitchDown, out Global.dPitchDown);
                        if (Global.dPitchDown < 300)
                        {
                            Global.dPitchDown = 300;
                        }
                        double.TryParse(sPitchForward, out Global.dPitchForward);
                        if (Global.dPitchForward < 300)
                        {
                            Global.dPitchForward = 300;
                        }
                        double.TryParse(sPitchReverse, out Global.dPitchReverse);
                        if (Global.dPitchReverse < 300)
                        {
                            Global.dPitchReverse = 300;
                        }

                        XmlNodeList polars = xmlroot.SelectNodes("//polars");

                        string sHLeft = polars[0].SelectSingleNode("HLeft").InnerText.Replace(" ", "");
                        string sHRight = polars[0].SelectSingleNode("HRight").InnerText.Replace(" ", "");
                        string sVLeftForward = polars[0].SelectSingleNode("VLeftForward").InnerText.Replace(" ", "");
                        string sVLeftReverse = polars[0].SelectSingleNode("VLeftReverse").InnerText.Replace(" ", "");
                        string sVRightForward = polars[0].SelectSingleNode("VRightForward").InnerText.Replace(" ", "");
                        string VRightReverse = polars[0].SelectSingleNode("VRightReverse").InnerText.Replace(" ", "");

                        int.TryParse(sHLeft, out Global.TuiJinQiBuChang_Polar_HL);
                        if (Global.TuiJinQiBuChang_Polar_HL != 1 && Global.TuiJinQiBuChang_Polar_HL != -1)
                        {
                            Global.TuiJinQiBuChang_Polar_HL = 1;
                        }
                        int.TryParse(sHRight, out Global.TuiJinQiBuChang_Polar_HR);
                        if (Global.TuiJinQiBuChang_Polar_HR != 1 && Global.TuiJinQiBuChang_Polar_HR != -1)
                        {
                            Global.TuiJinQiBuChang_Polar_HR = 1;
                        }
                        int.TryParse(sVLeftForward, out Global.TuiJinQiBuChang_Polar_VLF);
                        if (Global.TuiJinQiBuChang_Polar_VLF != 1 && Global.TuiJinQiBuChang_Polar_VLF != -1)
                        {
                            Global.TuiJinQiBuChang_Polar_VLF = 1;
                        }
                        int.TryParse(sVLeftReverse, out Global.TuiJinQiBuChang_Polar_VLB);
                        if (Global.TuiJinQiBuChang_Polar_VLB != 1 && Global.TuiJinQiBuChang_Polar_VLB != -1)
                        {
                            Global.TuiJinQiBuChang_Polar_VLB = 1;
                        }
                        int.TryParse(sVRightForward, out Global.TuiJinQiBuChang_Polar_VRF);
                        if (Global.TuiJinQiBuChang_Polar_VRF != 1 && Global.TuiJinQiBuChang_Polar_VRF != -1)
                        {
                            Global.TuiJinQiBuChang_Polar_VRF = 1;
                        }
                        int.TryParse(VRightReverse, out Global.TuiJinQiBuChang_Polar_VRB);
                        if (Global.TuiJinQiBuChang_Polar_VRB != 1 && Global.TuiJinQiBuChang_Polar_VRB != -1)
                        {
                            Global.TuiJinQiBuChang_Polar_VRB = 1;
                        }
                    }
                    catch (Exception ex)
                    { }

                    serials = xmlroot.SelectNodes("//modes//mode");
                    foreach (XmlNode node in serials)
                    {
                        if (node.Attributes["type"].Value.Replace(" ", "") == "mode1")
                        {
                            XmlNodeList units = node.ChildNodes[0].ChildNodes;

                            foreach (XmlNode unit in units)
                            {
                                #region BoardA
                                if (unit.Attributes["type"].Value.Replace(" ", "") == "BoardA")
                                {
                                    string sNameSerialIn = ((XmlElement)unit.SelectSingleNode("serialname")).InnerText;
                                    string sNumSerialIn = ((XmlElement)unit.SelectSingleNode("serialnum")).InnerText.Replace(" ", "").ToUpper();

                                    IPAddress addressIn;
                                    int portIn;
                                    bool bb = IPAddress.TryParse(((XmlElement)unit.SelectSingleNode("address")).InnerText.Replace(" ", ""), out addressIn);
                                    bb = Int32.TryParse(((XmlElement)unit.SelectSingleNode("port")).InnerText.Replace(" ", ""), out portIn);
                                    int quiryIntervalIn;
                                    bb = Int32.TryParse(((XmlElement)unit.SelectSingleNode("quiryinterval")).InnerText.Replace(" ", ""), out quiryIntervalIn);

                                    XmlNodeList quirysNodes = unit.SelectSingleNode("quirys").ChildNodes;
                                    int countQuiry = quirysNodes.Count;
                                    List<string> listQuiry = new List<string>();
                                    for (int i = 0; i < countQuiry; i++)
                                    {
                                        string sQuirysIn = quirysNodes[i].InnerText.Replace(" ", "");
                                        listQuiry.Add(sQuirysIn);
                                    }

                                    struct_SerialConfParas myStruct_SerialConfParas = new struct_SerialConfParas();
                                    myStruct_SerialConfParas.type = enum_BoardType.BoardA;
                                    myStruct_SerialConfParas.serialname = sNameSerialIn;
                                    myStruct_SerialConfParas.serialnum = sNumSerialIn;
                                    myStruct_SerialConfParas.address = addressIn;
                                    myStruct_SerialConfParas.port = portIn;
                                    myStruct_SerialConfParas.quiryinterval = quiryIntervalIn;
                                    myStruct_SerialConfParas.quirys = new List<string>();
                                    if (listQuiry.Count > 0)
                                    {
                                        myStruct_SerialConfParas.quirys.AddRange(listQuiry.ToArray());
                                    }

                                    List<enum_BoardType> keysExist = Global.DicModeBoardTypeSerialConf[enum_ModeType.Mode1].Keys.ToList();
                                    if (keysExist.Exists((enum_BoardType x) => x == enum_BoardType.BoardA ? true : false) == false)
                                    {
                                        List<struct_SerialConfParas> listTemp1 = new List<struct_SerialConfParas>();
                                        listTemp1.Add(myStruct_SerialConfParas);

                                        Global.DicModeBoardTypeSerialConf[enum_ModeType.Mode1].Add(enum_BoardType.BoardA, listTemp1);
                                    }
                                    else
                                    {
                                        List<struct_SerialConfParas> listTemp1 = Global.DicModeBoardTypeSerialConf[enum_ModeType.Mode1][enum_BoardType.BoardA];
                                        listTemp1.Add(myStruct_SerialConfParas);
                                        Global.DicModeBoardTypeSerialConf[enum_ModeType.Mode1][enum_BoardType.BoardA] = listTemp1;
                                    }
                                    Global.dicNameSerialConf.Add(sNumSerialIn, myStruct_SerialConfParas);
                                }
                                #endregion

                                #region BoardB
                                else if (unit.Attributes["type"].Value.Replace(" ", "") == "BoardB")
                                {
                                    string sNameSerialIn = ((XmlElement)unit.SelectSingleNode("serialname")).InnerText;
                                    string sNumSerialIn = ((XmlElement)unit.SelectSingleNode("serialnum")).InnerText.Replace(" ", "").ToUpper();

                                    IPAddress addressIn;
                                    int portIn;
                                    bool bb = IPAddress.TryParse(((XmlElement)unit.SelectSingleNode("address")).InnerText.Replace(" ", ""), out addressIn);
                                    bb = Int32.TryParse(((XmlElement)unit.SelectSingleNode("port")).InnerText.Replace(" ", ""), out portIn);
                                    int quiryIntervalIn;
                                    bb = Int32.TryParse(((XmlElement)unit.SelectSingleNode("quiryinterval")).InnerText.Replace(" ", ""), out quiryIntervalIn);

                                    XmlNodeList quirysNodes = unit.SelectSingleNode("quirys").ChildNodes;
                                    int countQuiry = quirysNodes.Count;
                                    List<string> listQuiry = new List<string>();
                                    for (int i = 0; i < countQuiry; i++)
                                    {
                                        string sQuirysIn = quirysNodes[i].InnerText.Replace(" ", "");
                                        listQuiry.Add(sQuirysIn);
                                    }

                                    string sClass = unit.Attributes["class"].Value.Replace(" ", "").ToUpper();
                                    enum_BoardTypeClass boradClass;
                                    if (sClass == "A")
                                    {
                                        boradClass = enum_BoardTypeClass.ClassA;
                                    }
                                    else if (sClass == "B")
                                    {
                                        boradClass = enum_BoardTypeClass.ClassB;
                                    }
                                    else
                                    {
                                        boradClass = enum_BoardTypeClass.None;
                                    }

                                    struct_SerialConfParas myStruct_SerialConfParas = new struct_SerialConfParas();
                                    myStruct_SerialConfParas.type = enum_BoardType.BoardB;
                                    myStruct_SerialConfParas.boradClass = boradClass;
                                    myStruct_SerialConfParas.serialname = sNameSerialIn;
                                    myStruct_SerialConfParas.serialnum = sNumSerialIn;
                                    myStruct_SerialConfParas.address = addressIn;
                                    myStruct_SerialConfParas.port = portIn;
                                    myStruct_SerialConfParas.quiryinterval = quiryIntervalIn;
                                    myStruct_SerialConfParas.quirys = new List<string>();
                                    if (listQuiry.Count > 0)
                                    {
                                        myStruct_SerialConfParas.quirys.AddRange(listQuiry.ToArray());
                                    }

                                    List<enum_BoardType> keysExist = Global.DicModeBoardTypeSerialConf[enum_ModeType.Mode1].Keys.ToList();
                                    if (keysExist.Exists((enum_BoardType x) => x == enum_BoardType.BoardB ? true : false) == false)
                                    {
                                        List<struct_SerialConfParas> listTemp1 = new List<struct_SerialConfParas>();
                                        listTemp1.Add(myStruct_SerialConfParas);

                                        Global.DicModeBoardTypeSerialConf[enum_ModeType.Mode1].Add(enum_BoardType.BoardB, listTemp1);
                                    }
                                    else
                                    {
                                        List<struct_SerialConfParas> listTemp1 = Global.DicModeBoardTypeSerialConf[enum_ModeType.Mode1][enum_BoardType.BoardB];
                                        listTemp1.Add(myStruct_SerialConfParas);
                                        Global.DicModeBoardTypeSerialConf[enum_ModeType.Mode1][enum_BoardType.BoardB] = listTemp1;
                                    }
                                    Global.dicNameSerialConf.Add(sNumSerialIn, myStruct_SerialConfParas);
                                }
                                #endregion

                                #region BoardC
                                else if (unit.Attributes["type"].Value.Replace(" ", "") == "BoardC")
                                {
                                    string sNameSerialIn = ((XmlElement)unit.SelectSingleNode("serialname")).InnerText;
                                    string sNumSerialIn = ((XmlElement)unit.SelectSingleNode("serialnum")).InnerText.Replace(" ", "").ToUpper();

                                    IPAddress addressIn;
                                    int portIn;
                                    bool bb = IPAddress.TryParse(((XmlElement)unit.SelectSingleNode("address")).InnerText.Replace(" ", ""), out addressIn);
                                    bb = Int32.TryParse(((XmlElement)unit.SelectSingleNode("port")).InnerText.Replace(" ", ""), out portIn);
                                    int quiryIntervalIn;
                                    bb = Int32.TryParse(((XmlElement)unit.SelectSingleNode("quiryinterval")).InnerText.Replace(" ", ""), out quiryIntervalIn);

                                    XmlNodeList quirysNodes = unit.SelectSingleNode("quirys").ChildNodes;
                                    int countQuiry = quirysNodes.Count;
                                    List<string> listQuiry = new List<string>();
                                    for (int i = 0; i < countQuiry; i++)
                                    {
                                        string sQuirysIn = quirysNodes[i].InnerText.Replace(" ", "");
                                        listQuiry.Add(sQuirysIn);
                                    }

                                    struct_SerialConfParas myStruct_SerialConfParas = new struct_SerialConfParas();
                                    myStruct_SerialConfParas.type = enum_BoardType.BoardC;
                                    myStruct_SerialConfParas.serialname = sNameSerialIn;
                                    myStruct_SerialConfParas.serialnum = sNumSerialIn;
                                    myStruct_SerialConfParas.address = addressIn;
                                    myStruct_SerialConfParas.port = portIn;
                                    myStruct_SerialConfParas.quiryinterval = quiryIntervalIn;
                                    myStruct_SerialConfParas.quirys = new List<string>();
                                    if (listQuiry.Count > 0)
                                    {
                                        myStruct_SerialConfParas.quirys.AddRange(listQuiry.ToArray());
                                    }

                                    List<enum_BoardType> keysExist = Global.DicModeBoardTypeSerialConf[enum_ModeType.Mode1].Keys.ToList();
                                    if (keysExist.Exists((enum_BoardType x) => x == enum_BoardType.BoardC ? true : false) == false)
                                    {
                                        List<struct_SerialConfParas> listTemp1 = new List<struct_SerialConfParas>();
                                        listTemp1.Add(myStruct_SerialConfParas);

                                        Global.DicModeBoardTypeSerialConf[enum_ModeType.Mode1].Add(enum_BoardType.BoardC, listTemp1);
                                    }
                                    else
                                    {
                                        List<struct_SerialConfParas> listTemp1 = Global.DicModeBoardTypeSerialConf[enum_ModeType.Mode1][enum_BoardType.BoardC];
                                        listTemp1.Add(myStruct_SerialConfParas);
                                        Global.DicModeBoardTypeSerialConf[enum_ModeType.Mode1][enum_BoardType.BoardC] = listTemp1;
                                    }
                                    Global.dicNameSerialConf.Add(sNumSerialIn, myStruct_SerialConfParas);
                                }
                                #endregion

                                #region BoardD
                                else if (unit.Attributes["type"].Value.Replace(" ", "") == "BoardD")
                                {
                                    string sNameSerialIn = ((XmlElement)unit.SelectSingleNode("serialname")).InnerText;
                                    string sNumSerialIn = ((XmlElement)unit.SelectSingleNode("serialnum")).InnerText.Replace(" ", "").ToUpper();
                                    string sPortNameIn = ((XmlElement)unit.SelectSingleNode("portname")).InnerText.Replace(" ", "");
                                    string sStopbitsIn = ((XmlElement)unit.SelectSingleNode("stopbits")).InnerText.Replace(" ", "");
                                    string sParityIn = ((XmlElement)unit.SelectSingleNode("parity")).InnerText.Replace(" ", "");

                                    int bandrateIn;
                                    bool bb = Int32.TryParse(((XmlElement)unit.SelectSingleNode("bandrate")).InnerText.Replace(" ", ""), out bandrateIn);
                                    int databitsIn;
                                    bb = Int32.TryParse(((XmlElement)unit.SelectSingleNode("databits")).InnerText.Replace(" ", ""), out databitsIn);


                                    int quiryIntervalIn;
                                    bb = Int32.TryParse(((XmlElement)unit.SelectSingleNode("quiryinterval")).InnerText.Replace(" ", ""), out quiryIntervalIn);

                                    XmlNodeList quirysNodes = unit.SelectSingleNode("quirys").ChildNodes;
                                    int countQuiry = quirysNodes.Count;
                                    List<string> listQuiry = new List<string>();
                                    for (int i = 0; i < countQuiry; i++)
                                    {
                                        string sQuirysIn = quirysNodes[i].InnerText.Replace(" ", "");
                                        listQuiry.Add(sQuirysIn);
                                    }


                                    struct_SerialConfParas myStruct_SerialConfParas = new struct_SerialConfParas();
                                    myStruct_SerialConfParas.type = enum_BoardType.BoardD;
                                    myStruct_SerialConfParas.serialname = sNameSerialIn;
                                    myStruct_SerialConfParas.serialnum = sNumSerialIn;
                                    myStruct_SerialConfParas.bandrate = bandrateIn;
                                    myStruct_SerialConfParas.databits = databitsIn;
                                    myStruct_SerialConfParas.portname = sPortNameIn;
                                    myStruct_SerialConfParas.stopbits = sStopbitsIn;
                                    myStruct_SerialConfParas.parity = sParityIn;
                                    myStruct_SerialConfParas.quiryinterval = quiryIntervalIn;
                                    myStruct_SerialConfParas.quirys = new List<string>();
                                    if (listQuiry.Count > 0)
                                    {
                                        myStruct_SerialConfParas.quirys.AddRange(listQuiry.ToArray());
                                    }

                                    List<enum_BoardType> keysExist = Global.DicModeBoardTypeSerialConf[enum_ModeType.Mode1].Keys.ToList();
                                    if (keysExist.Exists((enum_BoardType x) => x == enum_BoardType.BoardD ? true : false) == false)
                                    {
                                        List<struct_SerialConfParas> listTemp1 = new List<struct_SerialConfParas>();
                                        listTemp1.Add(myStruct_SerialConfParas);

                                        Global.DicModeBoardTypeSerialConf[enum_ModeType.Mode1].Add(enum_BoardType.BoardD, listTemp1);
                                    }
                                    else
                                    {
                                        List<struct_SerialConfParas> listTemp1 = Global.DicModeBoardTypeSerialConf[enum_ModeType.Mode1][enum_BoardType.BoardD];
                                        listTemp1.Add(myStruct_SerialConfParas);
                                        Global.DicModeBoardTypeSerialConf[enum_ModeType.Mode1][enum_BoardType.BoardD] = listTemp1;
                                    }
                                    Global.dicNameSerialConf.Add(sNumSerialIn, myStruct_SerialConfParas);

                                }
                                #endregion
                            }
                        }
                        else if (node.Attributes["type"].Value.Replace(" ", "") == "mode2")
                        {
                            XmlNodeList units = node.ChildNodes;
                        }
                        else if (node.Attributes["type"].Value.Replace(" ", "") == "mode3")
                        {
                            XmlNodeList units = node.ChildNodes;
                        }
                    }

                }
                else
                {
                    string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "\\configure\\configure.xml配置文件不存在！";
                    sInfo += "\t\n";
                    //richTextBox_InfoShow.AppendText(sInfo);
                }
            }
            catch (Exception ex)
            {
                string sInfo = ex.ToString();
                sInfo += "\t\n";
                //richTextBox_InfoShow.AppendText(sInfo);
            }
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            try
            {
                ReadTuiJinQiBuChangConfig();
                ReadZiDongDingXiangConfig();
                ReadZiDongDingGaoConfig();

                ReadExcelAlarmConfigure();

                Thread.Sleep(10);
                ToolStripMenuItem_ModeI.PerformClick();
                ToolStripMenuItem_ModeI.Checked = true;
            }
            catch (Exception ex)
            { }
        }

        //读取推进补偿参数
        string fileSavedBasicSys = "";
        private StreamWriter myStreamWriter;
        private StreamReader myStreamReader;
        private FileStream fs;
        private bool ReadTuiJinQiBuChangConfig()
        {
            try
            {
                fileSavedBasicSys = System.Windows.Forms.Application.StartupPath + "\\configure\\推进补偿参数.txt";
                if (File.Exists(fileSavedBasicSys))
                {
                    fs = new FileStream(fileSavedBasicSys, FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read);
                    myStreamReader = new StreamReader(fs);
                    /**
                     * 格式为：
                     * 水平左推进器=1;
                     * 水平右推进器=-1;
                     * 前左垂直推进器=2;
                     * 后左垂直推进器=2;
                     * 前右垂直推进器=2;
                     * 后右垂直推进器=2;
                     */

                    string[] sperator = new string[] { "=", "=", ";", "；" };
                    while (myStreamReader.EndOfStream == false)
                    {
                        string attr = myStreamReader.ReadLine();
                        string[] str = attr.Split(sperator, StringSplitOptions.RemoveEmptyEntries);
                        if (str.Length == 2)
                        {
                            double dd = 0;
                            bool bb = Double.TryParse(str[1], out dd);
                            if (bb == false)
                            {
                                continue;
                            }
                            if (dd > 100 || dd < -100)
                            {
                                continue;
                            }
                            if (str[0].Replace(" ", "") == "水平左推进器悬浮补偿")
                            {
                                Global.TuiJinQiBuChang_HL = dd;
                            }
                            else if (str[0].Replace(" ", "") == "水平右推进器悬浮补偿")
                            {
                                Global.TuiJinQiBuChang_HR = dd;
                            }
                            else if (str[0].Replace(" ", "") == "前左垂直推进器悬浮补偿")
                            {
                                Global.TuiJinQiBuChang_VLF = dd;
                            }
                            else if (str[0].Replace(" ", "") == "后左垂直推进器悬浮补偿")
                            {
                                Global.TuiJinQiBuChang_VLB = dd;
                            }
                            else if (str[0].Replace(" ", "") == "前右垂直推进器悬浮补偿")
                            {
                                Global.TuiJinQiBuChang_VRF = dd;
                            }
                            else if (str[0].Replace(" ", "") == "后右垂直推进器悬浮补偿")
                            {
                                Global.TuiJinQiBuChang_VRB = dd;
                            }


                            else if (str[0].Replace(" ", "") == "水平左推进器停转补偿")
                            {
                                Global.TuiJinQiBuChang_HL_Zero = dd;
                            }
                            else if (str[0].Replace(" ", "") == "水平右推进器停转补偿")
                            {
                                Global.TuiJinQiBuChang_HR_Zero = dd;
                            }
                            else if (str[0].Replace(" ", "") == "前左垂直推进器停转补偿")
                            {
                                Global.TuiJinQiBuChang_VLF_Zero = dd;
                            }
                            else if (str[0].Replace(" ", "") == "后左垂直推进器停转补偿")
                            {
                                Global.TuiJinQiBuChang_VLB_Zero = dd;
                            }
                            else if (str[0].Replace(" ", "") == "前右垂直推进器停转补偿")
                            {
                                Global.TuiJinQiBuChang_VRF_Zero = dd;
                            }
                            else if (str[0].Replace(" ", "") == "后右垂直推进器停转补偿")
                            {
                                Global.TuiJinQiBuChang_VRB_Zero = dd;
                            }

                            else if (str[0].Replace(" ", "") == "整体悬浮补偿")
                            {
                                Global.TuiJinQiBuChang_XuanFu = dd;
                            }

                        }
                    }
                    myStreamReader.Close();
                    fs.Close();
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        
        public bool SaveTuiJinQiBuChangConfig()
        {
            try
            {
                fileSavedBasicSys = System.Windows.Forms.Application.StartupPath + "\\configure\\推进补偿参数.txt";
                if (File.Exists(fileSavedBasicSys))
                {
                    File.Delete(fileSavedBasicSys);
                }
                fs = new FileStream(fileSavedBasicSys, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
                myStreamWriter = new StreamWriter(fs);

                string ss = "";

                ss = "水平左推进器悬浮补偿=" + Global.TuiJinQiBuChang_HL.ToString() + ";";
                myStreamWriter.WriteLine(ss);
                ss = "水平右推进器悬浮补偿=" + Global.TuiJinQiBuChang_HR.ToString() + ";";
                myStreamWriter.WriteLine(ss);
                ss = "前左垂直推进器悬浮补偿=" + Global.TuiJinQiBuChang_VLF.ToString() + ";";
                myStreamWriter.WriteLine(ss);
                ss = "后左垂直推进器悬浮补偿=" + Global.TuiJinQiBuChang_VLB.ToString() + ";";
                myStreamWriter.WriteLine(ss);
                ss = "前右垂直推进器悬浮补偿=" + Global.TuiJinQiBuChang_VRF.ToString() + ";";
                myStreamWriter.WriteLine(ss);
                ss = "后右垂直推进器悬浮补偿=" + Global.TuiJinQiBuChang_VRB.ToString() + ";";
                myStreamWriter.WriteLine(ss);

                ss = "水平左推进器停转补偿=" + Global.TuiJinQiBuChang_HL_Zero.ToString() + ";";
                myStreamWriter.WriteLine(ss);
                ss = "水平右推进器停转补偿=" + Global.TuiJinQiBuChang_HR_Zero.ToString() + ";";
                myStreamWriter.WriteLine(ss);
                ss = "前左垂直推进器停转补偿=" + Global.TuiJinQiBuChang_VLF_Zero.ToString() + ";";
                myStreamWriter.WriteLine(ss);
                ss = "后左垂直推进器停转补偿=" + Global.TuiJinQiBuChang_VLB_Zero.ToString() + ";";
                myStreamWriter.WriteLine(ss);
                ss = "前右垂直推进器停转补偿=" + Global.TuiJinQiBuChang_VRF_Zero.ToString() + ";";
                myStreamWriter.WriteLine(ss);
                ss = "后右垂直推进器停转补偿=" + Global.TuiJinQiBuChang_VRB_Zero.ToString() + ";";
                myStreamWriter.WriteLine(ss);

                ss = "整体悬浮补偿=" + Global.TuiJinQiBuChang_XuanFu.ToString() + ";";
                myStreamWriter.WriteLine(ss);

                myStreamWriter.Flush();

                myStreamWriter.Close();
                fs.Close();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        //读取自动定向参数
        private bool ReadZiDongDingXiangConfig()
        {
            try
            {
                fileSavedBasicSys = System.Windows.Forms.Application.StartupPath + "\\configure\\自动定向参数.txt";
                if (File.Exists(fileSavedBasicSys))
                {
                    fs = new FileStream(fileSavedBasicSys, FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read);
                    myStreamReader = new StreamReader(fs);
                    /**
                     * 格式为：
                     * 
                        一阶角度差=5;
                        一阶推力百分比=30;
                        一阶推力器工作时间=2;
                        一阶推力器工作后等待时间=2.5;
                        二阶角度差=3;
                        二阶推力百分比=10;
                        二阶推力器工作时间=1;
                        二阶推力器工作后等待时间=1.5;
                        三阶角度差=2;
                        三阶推力百分比=5;
                        三阶推力器工作时间=0.5;
                        三阶推力器工作后等待时间=0.8;
                     * 
                     * 对应：
                        1.5m移动钻机，自动定向逻辑 (对应左右两个水平推力器)
	                    （1）角度差=＞5°，推力百分比30%，推力器工作2s，在2.5s后，判断角度差，
	                    （2）角度差＞3°，角度差＜5°，推力百分比10%，推力器工作1s，在1.5s后，判断角度差，
	                    （3）角度差＞2°角度差＜3°，推力百分比5%，推力器工作0.5s，在0.8s后，判断角度差，
	                    （4）角度差＜2°，暂停，，实时根据上报的Yaw值，判断角度差，
                     */

                    string[] sperator = new string[] { "=", "=", ";", "；" };
                    while (myStreamReader.EndOfStream == false)
                    {
                        string attr = myStreamReader.ReadLine();
                        string[] str = attr.Split(sperator, StringSplitOptions.RemoveEmptyEntries);
                        if (str.Length == 2)
                        {
                            double dd = 0;
                            bool bb = Double.TryParse(str[1], out dd);
                            if (bb == false)
                            {
                                continue;
                            }
                            if (dd < 0)
                            {
                                continue;
                            }

                            if (str[0].Replace(" ", "") == "一阶角度差")
                            {
                                Global.AutoCtlDir_JiaoDuCha_1 = dd;
                            }
                            else if (str[0].Replace(" ", "") == "一阶推力百分比")
                            {
                                Global.AutoCtlDir_TuiJinPercent_1 = dd;
                            }
                            else if (str[0].Replace(" ", "") == "一阶推力器工作时间")
                            {
                                Global.AutoCtlDir_TuiLiQiWorkTime_1 = dd;
                            }
                            else if (str[0].Replace(" ", "") == "一阶推力器工作后等待时间")
                            {
                                Global.AutoCtlDir_TuiLiQiWorkTime_Compare_1 = dd;
                            }
                            else if (str[0].Replace(" ", "") == "二阶角度差")
                            {
                                Global.AutoCtlDir_JiaoDuCha_2 = dd;
                            }
                            else if (str[0].Replace(" ", "") == "二阶推力百分比")
                            {
                                Global.AutoCtlDir_TuiJinPercent_2 = dd;
                            }
                            else if (str[0].Replace(" ", "") == "二阶推力器工作时间")
                            {
                                Global.AutoCtlDir_TuiLiQiWorkTime_2 = dd;
                            }
                            else if (str[0].Replace(" ", "") == "二阶推力器工作后等待时间")
                            {
                                Global.AutoCtlDir_TuiLiQiWorkTime_Compare_2 = dd;
                            }
                            else if (str[0].Replace(" ", "") == "三阶角度差")
                            {
                                Global.AutoCtlDir_JiaoDuCha_3 = dd;
                            }
                            else if (str[0].Replace(" ", "") == "三阶推力百分比")
                            {
                                Global.AutoCtlDir_TuiJinPercent_3 = dd;
                            }
                            else if (str[0].Replace(" ", "") == "三阶推力器工作时间")
                            {
                                Global.AutoCtlDir_TuiLiQiWorkTime_3 = dd;
                            }
                            else if (str[0].Replace(" ", "") == "三阶推力器工作后等待时间")
                            {
                                Global.AutoCtlDir_TuiLiQiWorkTime_Compare_3 = dd;
                            }
                        }
                    }
                    myStreamReader.Close();
                    fs.Close();
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool SaveZiDongDingXiangConfig()
        {
            try
            {
                fileSavedBasicSys = System.Windows.Forms.Application.StartupPath + "\\configure\\自动定向参数.txt";
                if (File.Exists(fileSavedBasicSys))
                {
                    File.Delete(fileSavedBasicSys);
                }
                fs = new FileStream(fileSavedBasicSys, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
                myStreamWriter = new StreamWriter(fs);

                string ss = "";

                ss = "一阶角度差=" + Global.AutoCtlDir_JiaoDuCha_1.ToString() + ";";
                myStreamWriter.WriteLine(ss);
                ss = "一阶推力百分比=" + Global.AutoCtlDir_TuiJinPercent_1.ToString() + ";";
                myStreamWriter.WriteLine(ss);
                ss = "一阶推力器工作时间=" + Global.AutoCtlDir_TuiLiQiWorkTime_1.ToString() + ";";
                myStreamWriter.WriteLine(ss);
                ss = "一阶推力器工作后等待时间=" + Global.AutoCtlDir_TuiLiQiWorkTime_Compare_1.ToString() + ";";
                myStreamWriter.WriteLine(ss);

                ss = "二阶角度差=" + Global.AutoCtlDir_JiaoDuCha_2.ToString() + ";";
                myStreamWriter.WriteLine(ss);
                ss = "二阶推力百分比=" + Global.AutoCtlDir_TuiJinPercent_2.ToString() + ";";
                myStreamWriter.WriteLine(ss);
                ss = "二阶推力器工作时间=" + Global.AutoCtlDir_TuiLiQiWorkTime_2.ToString() + ";";
                myStreamWriter.WriteLine(ss);
                ss = "二阶推力器工作后等待时间=" + Global.AutoCtlDir_TuiLiQiWorkTime_Compare_2.ToString() + ";";
                myStreamWriter.WriteLine(ss);

                ss = "三阶角度差=" + Global.AutoCtlDir_JiaoDuCha_3.ToString() + ";";
                myStreamWriter.WriteLine(ss);
                ss = "三阶推力百分比=" + Global.AutoCtlDir_TuiJinPercent_3.ToString() + ";";
                myStreamWriter.WriteLine(ss);
                ss = "三阶推力器工作时间=" + Global.AutoCtlDir_TuiLiQiWorkTime_3.ToString() + ";";
                myStreamWriter.WriteLine(ss);
                ss = "三阶推力器工作后等待时间=" + Global.AutoCtlDir_TuiLiQiWorkTime_Compare_3.ToString() + ";";
                myStreamWriter.WriteLine(ss);

                myStreamWriter.Flush();

                myStreamWriter.Close();
                fs.Close();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        //读取自动定高参数
        private bool ReadZiDongDingGaoConfig()
        {
            try
            {
                fileSavedBasicSys = System.Windows.Forms.Application.StartupPath + "\\configure\\自动定高参数.txt";
                if (File.Exists(fileSavedBasicSys))
                {
                    fs = new FileStream(fileSavedBasicSys, FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read);
                    myStreamReader = new StreamReader(fs);
                    /**
                     * 格式为：
                     * 
                        一阶高度差=5;
                        一阶推力百分比=30;
                        一阶推力器工作时间=2;
                        一阶推力器工作后等待时间=2.5;
                        二阶高度差=3;
                        二阶推力百分比=10;
                        二阶推力器工作时间=1;
                        二阶推力器工作后等待时间=1.5;
                        三阶高度差=2;
                        三阶推力百分比=5;
                        三阶推力器工作时间=0.5;
                        三阶推力器工作后等待时间=0.8;
                     * 
                     * 对应：
	                    1.5m移动钻机，自动定高逻辑  (对应垂直4个推力器)
	                    （1）高度差=＞5m，推力百分比10%，推力器工作2s，在2.5s后，判断高度差，
	                    （2）高度差＞3m，角度差＜5m，推力百分比8%，推力器工作1s，在1.5s后，判断高度差，
	                    （3）高度差＞1m角度差＜3m，推力百分比3%，推力器工作0.5s，在0.8s后，判断高度差，
	                    （4）高度差＜1m，暂停，，实时根据上报的高度值，判断高度差，
                     */

                    string[] sperator = new string[] { "=", "=", ";", "；" };
                    while (myStreamReader.EndOfStream == false)
                    {
                        string attr = myStreamReader.ReadLine();
                        string[] str = attr.Split(sperator, StringSplitOptions.RemoveEmptyEntries);
                        if (str.Length == 2)
                        {
                            double dd = 0;
                            bool bb = Double.TryParse(str[1], out dd);
                            if (bb == false)
                            {
                                continue;
                            }
                            if (dd < 0)
                            {
                                continue;
                            }

                            if (str[0].Replace(" ", "") == "一阶高度差")
                            {
                                Global.AutoCtlHigh_GaoDuCha_1 = dd;
                            }
                            else if (str[0].Replace(" ", "") == "一阶推力百分比")
                            {
                                Global.AutoCtlHigh_TuiJinPercent_1 = dd;
                            }
                            else if (str[0].Replace(" ", "") == "一阶推力器工作时间")
                            {
                                Global.AutoCtlHigh_TuiLiQiWorkTime_1 = dd;
                            }
                            else if (str[0].Replace(" ", "") == "一阶推力器工作后等待时间")
                            {
                                Global.AutoCtlHigh_TuiLiQiWorkTime_Compare_1 = dd;
                            }
                            else if (str[0].Replace(" ", "") == "二阶高度差")
                            {
                                Global.AutoCtlHigh_GaoDuCha_2 = dd;
                            }
                            else if (str[0].Replace(" ", "") == "二阶推力百分比")
                            {
                                Global.AutoCtlHigh_TuiJinPercent_2 = dd;
                            }
                            else if (str[0].Replace(" ", "") == "二阶推力器工作时间")
                            {
                                Global.AutoCtlHigh_TuiLiQiWorkTime_2 = dd;
                            }
                            else if (str[0].Replace(" ", "") == "二阶推力器工作后等待时间")
                            {
                                Global.AutoCtlHigh_TuiLiQiWorkTime_Compare_2 = dd;
                            }
                            else if (str[0].Replace(" ", "") == "三阶高度差")
                            {
                                Global.AutoCtlHigh_GaoDuCha_3 = dd;
                            }
                            else if (str[0].Replace(" ", "") == "三阶推力百分比")
                            {
                                Global.AutoCtlHigh_TuiJinPercent_3 = dd;
                            }
                            else if (str[0].Replace(" ", "") == "三阶推力器工作时间")
                            {
                                Global.AutoCtlHigh_TuiLiQiWorkTime_3 = dd;
                            }
                            else if (str[0].Replace(" ", "") == "三阶推力器工作后等待时间")
                            {
                                Global.AutoCtlHigh_TuiLiQiWorkTime_Compare_3 = dd;
                            }
                        }
                    }
                    myStreamReader.Close();
                    fs.Close();
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool SaveZiDongDingGaoConfig()
        {
            try
            {
                fileSavedBasicSys = System.Windows.Forms.Application.StartupPath + "\\configure\\自动定高参数.txt";
                if (File.Exists(fileSavedBasicSys))
                {
                    File.Delete(fileSavedBasicSys);
                }
                fs = new FileStream(fileSavedBasicSys, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
                myStreamWriter = new StreamWriter(fs);

                string ss = "";

                ss = "一阶高度差=" + Global.AutoCtlHigh_GaoDuCha_1.ToString() + ";";
                myStreamWriter.WriteLine(ss);
                ss = "一阶推力百分比=" + Global.AutoCtlHigh_TuiJinPercent_1.ToString() + ";";
                myStreamWriter.WriteLine(ss);
                ss = "一阶推力器工作时间=" + Global.AutoCtlHigh_TuiLiQiWorkTime_1.ToString() + ";";
                myStreamWriter.WriteLine(ss);
                ss = "一阶推力器工作后等待时间=" + Global.AutoCtlHigh_TuiLiQiWorkTime_Compare_1.ToString() + ";";
                myStreamWriter.WriteLine(ss);

                ss = "二阶高度差=" + Global.AutoCtlHigh_GaoDuCha_2.ToString() + ";";
                myStreamWriter.WriteLine(ss);
                ss = "二阶推力百分比=" + Global.AutoCtlHigh_TuiJinPercent_2.ToString() + ";";
                myStreamWriter.WriteLine(ss);
                ss = "二阶推力器工作时间=" + Global.AutoCtlHigh_TuiLiQiWorkTime_2.ToString() + ";";
                myStreamWriter.WriteLine(ss);
                ss = "二阶推力器工作后等待时间=" + Global.AutoCtlHigh_TuiLiQiWorkTime_Compare_2.ToString() + ";";
                myStreamWriter.WriteLine(ss);

                ss = "三阶高度差=" + Global.AutoCtlHigh_GaoDuCha_3.ToString() + ";";
                myStreamWriter.WriteLine(ss);
                ss = "三阶推力百分比=" + Global.AutoCtlHigh_TuiJinPercent_3.ToString() + ";";
                myStreamWriter.WriteLine(ss);
                ss = "三阶推力器工作时间=" + Global.AutoCtlHigh_TuiLiQiWorkTime_3.ToString() + ";";
                myStreamWriter.WriteLine(ss);
                ss = "三阶推力器工作后等待时间=" + Global.AutoCtlHigh_TuiLiQiWorkTime_Compare_3.ToString() + ";";
                myStreamWriter.WriteLine(ss);

                myStreamWriter.Flush();

                myStreamWriter.Close();
                fs.Close();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        private void ToolStripMenuItem_ModeII_Click(object sender, EventArgs e)
        {
            if (ToolStripMenuItem_ModeII.Checked)
            {
                ToolStripMenuItem_ModeI.Checked = false;
                ToolStripMenuItem_ModeIII.Checked = false;
            }
        }

        private void ToolStripMenuItem_ModeIII_Click(object sender, EventArgs e)
        {
            if (ToolStripMenuItem_ModeIII.Checked)
            {
                ToolStripMenuItem_ModeI.Checked = false;
                ToolStripMenuItem_ModeII.Checked = false;
            }
        }

        private void ToolStripMenuItem_TuiJinQinParasSave_Click(object sender, EventArgs e)
        {
            try
            {
                SaveTuiJinQiBuChangConfig();
                SaveZiDongDingXiangConfig();
                SaveZiDongDingGaoConfig();
            }
            catch (Exception ex)
            { }
        }

        private void ToolStripMenuItem_AutoCtlDirParas_Click(object sender, EventArgs e)
        {
            try
            {
                if (Global.myFormAutoCtlDirParas == null)
                {
                    Global.myFormAutoCtlDirParas = new FormAutoCtlDirParas();
                    Global.myFormAutoCtlDirParas.Show();
                }
                Global.myFormAutoCtlDirParas.Show();
            }
            catch (Exception ex)
            { }
        }

        private void ToolStripMenuItem_AutoCtlHighParas_Click(object sender, EventArgs e)
        {
            try
            {
                if (Global.myFormAutoCtlHighParas == null)
                {
                    Global.myFormAutoCtlHighParas = new FormAutoCtlHighParas();
                    Global.myFormAutoCtlHighParas.Show();
                }
                Global.myFormAutoCtlHighParas.Show();
            }
            catch (Exception ex)
            { }
        }



    }
}
