using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;

namespace MonitorProj
{
    public partial class FormMainUserA : Form
    {

        //报警信息
        private Dictionary<string, StatusUnitParas> dicStatusParasByStatusNameLocal = new Dictionary<string, StatusUnitParas>();
        private List<string> listStatusParasKeys = new List<string>();

        //指令反馈信息
        private byte[] bCmdReturn;          //从设备串口返回的指令反馈信息
        private bool isbCmdReturn = false;  //标志位，true表示有新的指令反馈信息
        private bool isOnlyPowerCtl = false;
        public event EventHandler DeviceFormStateEventSend;

        private void groupBox_Paint_1(object sender, PaintEventArgs e)
        {
            try
            {
                //GroupBox gBox = (GroupBox)sender;

                //e.Graphics.Clear(gBox.BackColor);
                //e.Graphics.DrawString(gBox.Text, gBox.Font, Brushes.Red, 10, 1);
                //var vSize = e.Graphics.MeasureString(gBox.Text, gBox.Font);
                //e.Graphics.DrawLine(Pens.Red, 1, vSize.Height / 2, 8, vSize.Height / 2);
                //e.Graphics.DrawLine(Pens.Red, vSize.Width + 8, vSize.Height / 2, gBox.Width - 2, vSize.Height / 2);
                //e.Graphics.DrawLine(Pens.Red, 1, vSize.Height / 2, 1, gBox.Height - 2);
                //e.Graphics.DrawLine(Pens.Red, 1, gBox.Height - 2, gBox.Width - 2, gBox.Height - 2);
                //e.Graphics.DrawLine(Pens.Red, gBox.Width - 2, vSize.Height / 2, gBox.Width - 2, gBox.Height - 2);

                GroupBox gBox = (GroupBox)sender;

                e.Graphics.Clear(gBox.BackColor);
                e.Graphics.DrawString(gBox.Text, gBox.Font, Brushes.Red, 10, 1);
                var vSize = e.Graphics.MeasureString(gBox.Text, gBox.Font);
                e.Graphics.DrawLine(Pens.Red, 1, vSize.Height / 2, 8, vSize.Height / 2);
                e.Graphics.DrawLine(Pens.Red, vSize.Width + 8, vSize.Height / 2, gBox.Width - 2, vSize.Height / 2);
                e.Graphics.DrawLine(Pens.Red, 1, vSize.Height / 2, 1, gBox.Height - 2);
                e.Graphics.DrawLine(Pens.Red, 1, gBox.Height - 2, gBox.Width - 2, gBox.Height - 2);
                e.Graphics.DrawLine(Pens.Red, gBox.Width - 2, vSize.Height / 2, gBox.Width - 2, gBox.Height - 2);
            }
            catch (Exception ex)
            {
            }
        }

        private void groupBox_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                GroupBox gBox = (GroupBox)sender;

                e.Graphics.Clear(gBox.BackColor);

                var vSize = e.Graphics.MeasureString(gBox.Text, gBox.Font);

                e.Graphics.DrawString(gBox.Text, gBox.Font, Brushes.Blue, gBox.Width / 2 - vSize.Width / 2, 1);

                e.Graphics.DrawLine(Pens.Red, 1, 7, gBox.Width / 2 - vSize.Width / 2, 7);
                e.Graphics.DrawLine(Pens.Red, gBox.Width / 2 + vSize.Width / 2, 7, gBox.Width - 2, 7);
                e.Graphics.DrawLine(Pens.Red, 1, 7, 1, gBox.Height - 2);
                e.Graphics.DrawLine(Pens.Red, 1, gBox.Height - 2, gBox.Width - 2, gBox.Height - 2);
                e.Graphics.DrawLine(Pens.Red, gBox.Width - 2, 7, gBox.Width - 2, gBox.Height - 2);
            }
            catch (Exception ex)
            { }
        }

        private void groupBox_Paint_2(object sender, PaintEventArgs e)
        {
            try
            {
                GroupBox gBox = (GroupBox)sender;

                e.Graphics.Clear(gBox.BackColor);

                var vSize = e.Graphics.MeasureString(gBox.Text, gBox.Font);

                e.Graphics.DrawString(gBox.Text, gBox.Font, Brushes.Red, gBox.Width / 2 - vSize.Width / 2, 1);

                e.Graphics.DrawLine(Pens.Green, 1, 7, gBox.Width / 2 - vSize.Width / 2, 7);
                e.Graphics.DrawLine(Pens.Green, gBox.Width / 2 + vSize.Width / 2, 7, gBox.Width - 2, 7);
                e.Graphics.DrawLine(Pens.Green, 1, 7, 1, gBox.Height - 2);
                e.Graphics.DrawLine(Pens.Green, 1, gBox.Height - 2, gBox.Width - 2, gBox.Height - 2);
                e.Graphics.DrawLine(Pens.Green, gBox.Width - 2, 7, gBox.Width - 2, gBox.Height - 2);
            }
            catch (Exception ex)
            { }
        }

        //读写基准参数
        string fileSavedBasicParas = "";
        private StreamWriter myStreamWriter;
        private StreamReader myStreamReader;
        private FileStream fs;
        private bool ReadBasicParasConfig()
        {
            try
            {
                fileSavedBasicParas = System.Windows.Forms.Application.StartupPath + "\\configure\\基准参数.txt";
                if (File.Exists(fileSavedBasicParas))
                {
                    fs = new FileStream(fileSavedBasicParas, FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read);
                    myStreamReader = new StreamReader(fs);
                    /**
                     * 格式为：
                     * 
                        钻杆深度基准=0;
                     * 
                     */

                    string[] sperator = new string[] { "=", "=", ";", "；" };
                    while (myStreamReader.EndOfStream == false)
                    {
                        string attr = myStreamReader.ReadLine();
                        string[] str = attr.Split(sperator, StringSplitOptions.RemoveEmptyEntries);
                        if (str.Length == 2)
                        {
                            if (str[0].Replace(" ", "") == "钻杆深度基准")
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

                                Global.BasicParas_ZuanJin_ShenDu = dd;
                            }
                            else if (str[0].Replace(" ", "") == "灯1名称")
                            {
                                Global.sName_Light_1 = str[1];
                            }
                            else if (str[0].Replace(" ", "") == "灯2名称")
                            {
                                Global.sName_Light_2 = str[1];
                            }
                            else if (str[0].Replace(" ", "") == "灯3名称")
                            {
                                Global.sName_Light_3 = str[1];
                            }
                            else if (str[0].Replace(" ", "") == "灯4名称")
                            {
                                Global.sName_Light_4 = str[1];
                            }
                            else if (str[0].Replace(" ", "") == "灯5名称")
                            {
                                Global.sName_Light_5 = str[1];
                            }
                            else if (str[0].Replace(" ", "") == "灯6名称")
                            {
                                Global.sName_Light_6 = str[1];
                            }
                            else if (str[0].Replace(" ", "") == "灯7名称")
                            {
                                Global.sName_Light_7 = str[1];
                            }
                            else if (str[0].Replace(" ", "") == "灯8名称")
                            {
                                Global.sName_Light_8 = str[1];
                            }
                            else if (str[0].Replace(" ", "") == "摄像机1名称")
                            {
                                Global.sName_Camera_1 = str[1];
                            }
                            else if (str[0].Replace(" ", "") == "摄像机2名称")
                            {
                                Global.sName_Camera_2 = str[1];
                            }
                            else if (str[0].Replace(" ", "") == "摄像机3名称")
                            {
                                Global.sName_Camera_3 = str[1];
                            }
                            else if (str[0].Replace(" ", "") == "摄像机4名称")
                            {
                                Global.sName_Camera_4 = str[1];
                            }
                            else if (str[0].Replace(" ", "") == "摄像机5名称")
                            {
                                Global.sName_Camera_5 = str[1];
                            }
                            else if (str[0].Replace(" ", "") == "摄像机6名称")
                            {
                                Global.sName_Camera_6 = str[1];
                            }
                            else if (str[0].Replace(" ", "") == "摄像机7名称")
                            {
                                Global.sName_Camera_7 = str[1];
                            }
                            else if (str[0].Replace(" ", "") == "摄像机8名称")
                            {
                                Global.sName_Camera_8 = str[1];
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

        public bool SaveBasicParasConfig()
        {
            try
            {
                fileSavedBasicParas = System.Windows.Forms.Application.StartupPath + "\\configure\\基准参数.txt";
                if (File.Exists(fileSavedBasicParas))
                {
                    File.Delete(fileSavedBasicParas);
                }
                fs = new FileStream(fileSavedBasicParas, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
                myStreamWriter = new StreamWriter(fs);

                string ss = "";

                ss = "钻杆深度基准=" + Global.BasicParas_ZuanJin_ShenDu.ToString() + ";";
                myStreamWriter.WriteLine(ss);

                ss = "灯1名称=" + Global.sName_Light_1 + ";";
                myStreamWriter.WriteLine(ss);

                ss = "灯2名称=" + Global.sName_Light_2 + ";";
                myStreamWriter.WriteLine(ss);

                ss = "灯3名称=" + Global.sName_Light_3 + ";";
                myStreamWriter.WriteLine(ss);

                ss = "灯4名称=" + Global.sName_Light_4 + ";";
                myStreamWriter.WriteLine(ss);

                ss = "灯5名称=" + Global.sName_Light_5 + ";";
                myStreamWriter.WriteLine(ss);

                ss = "灯6名称=" + Global.sName_Light_6 + ";";
                myStreamWriter.WriteLine(ss);

                ss = "灯7名称=" + Global.sName_Light_7 + ";";
                myStreamWriter.WriteLine(ss);

                ss = "灯8名称=" + Global.sName_Light_8 + ";";
                myStreamWriter.WriteLine(ss);

                ss = "摄像机1名称=" + Global.sName_Camera_1 + ";";
                myStreamWriter.WriteLine(ss);

                ss = "摄像机2名称=" + Global.sName_Camera_2 + ";";
                myStreamWriter.WriteLine(ss);

                ss = "摄像机3名称=" + Global.sName_Camera_3 + ";";
                myStreamWriter.WriteLine(ss);

                ss = "摄像机4名称=" + Global.sName_Camera_4 + ";";
                myStreamWriter.WriteLine(ss);

                ss = "摄像机5名称=" + Global.sName_Camera_5 + ";";
                myStreamWriter.WriteLine(ss);

                ss = "摄像机6名称=" + Global.sName_Camera_6 + ";";
                myStreamWriter.WriteLine(ss);

                ss = "摄像机7名称=" + Global.sName_Camera_7 + ";";
                myStreamWriter.WriteLine(ss);

                ss = "摄像机8名称=" + Global.sName_Camera_8 + ";";
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

        #region 各板卡通信状态标志

        private bool flag_Communication_61 = false;//直流绝缘检测板#1（0x61）
        private bool flag_Communication_62 = false;//直流绝缘检测板#2（0x62）
        private bool flag_Communication_63 = false;//直流绝缘检测板#3（0x63）

        private bool flag_Communication_70 = false;//大功率直流绝缘检测板#1（0x70）
        private bool flag_Communication_71 = false;//大功率直流绝缘检测板#2（0x71）
        private bool flag_Communication_72 = false;//大功率直流绝缘检测板#3（0x72）

        private bool flag_Communication_79 = false;//大功率直流绝缘检测板控制板（0x79）

        private bool flag_Communication_80 = false;//交流绝缘检测板（0x80）

        private bool flag_Communication_90 = false;//舱内备用电源继电器板（0x90）

        private bool flag_Communication_40 = false;//油箱采集板（0x40）

        private bool flag_Communication_25 = false;//摄像机电源继电器板#1（0x25）
        private bool flag_Communication_26 = false;//摄像机电源继电器板#2（0x26）
        private bool flag_Communication_28 = false;//传感器电源继电器板#1（0x28）
        private bool flag_Communication_29 = false;//传感器电源继电器板#2（0x29）

        private bool flag_Communication_30 = false;//工况采集板（0x30）

        private bool flag_Communication_21 = false;//灯继电器板#1(0x21)
        private bool flag_Communication_22 = false;//灯继电器板#2(0x22)
        private bool flag_Communication_23 = false;//灯控制板#1(0x23)
        private bool flag_Communication_24 = false;//灯控制板#2(0x24)

        private bool flag_Communication_50 = false;//电池电源转换板（0x50）

        private bool flag_Communication_FaXiang16 = false;//16功能阀箱(国外阀箱）
        private bool flag_Communication_FaXiang16_A = false;//16功能阀箱(2板卡版本，A）
        private bool flag_Communication_FaXiang16_B = false;//16功能阀箱(2板卡版本，B）
        private bool flag_Communication_FaXiang8 = false;//8功能阀箱

        private bool flag_Communication_LuoPan = false;//罗盘

        #endregion

        public FormMainUserA()
        {
            try
            {
                InitializeComponent();

                threadServoValvePackOper = new Thread(new ThreadStart(threadServoValvePackOperFunc));
                threadServoValvePackOper.Name = "threadServoValvePackOper";
                threadServoValvePackOper.Start();

                threadFuQianOper = new Thread(new ThreadStart(threadFuQianOperFunc));
                threadFuQianOper.Name = "threadFuQianOper";
                threadFuQianOper.Start();

                threadAutoDirCtlOper = new Thread(new ThreadStart(threadAutoDirCtlOperFunc));
                threadAutoDirCtlOper.Name = "threadDiCiJiaJiaoOper";
                threadAutoDirCtlOper.Start();

                thread_TuiJinQi_HMove_Oper = new Thread(new ThreadStart(thread_TuiJinQi_HMove_OperFunc));
                thread_TuiJinQi_HMove_Oper.Name = "thread_TuiJinQi_HMove_Oper";
                thread_TuiJinQi_HMove_Oper.Start();

                threadAutoHighOper = new Thread(new ThreadStart(threadAutoHighOperFunc));
                threadAutoHighOper.Name = "threadAutoHighOper";
                threadAutoHighOper.Start();


                listStatusParasKeys = Global.dicStatusParasByStatusName.Keys.ToList(); ;
                for (int i = 0; i < Global.dicStatusParasByStatusName.Count; i++)
                {
                    dicStatusParasByStatusNameLocal.Add(listStatusParasKeys[i], Global.dicStatusParasByStatusName[listStatusParasKeys[i]]);
                }
            }
            catch (Exception ex)
            { }
        }



        private void FormMainUserA_Load(object sender, EventArgs e)
        {
            try
            {
                numericUpDown_HL.Value = (Decimal)Global.TuiJinQiBuChang_HL;
                numericUpDown_HR.Value = (Decimal)Global.TuiJinQiBuChang_HR;
                numericUpDown_VLF.Value = (Decimal)Global.TuiJinQiBuChang_VLF;
                numericUpDown_VLB.Value = (Decimal)Global.TuiJinQiBuChang_VLB;
                numericUpDown_VRF.Value = (Decimal)Global.TuiJinQiBuChang_VRF;
                numericUpDown_VRB.Value = (Decimal)Global.TuiJinQiBuChang_VRB;

                numericUpDown_HL_Zero.Value = (Decimal)Global.TuiJinQiBuChang_HL_Zero;
                numericUpDown_HR_Zero.Value = (Decimal)Global.TuiJinQiBuChang_HR_Zero;
                numericUpDown_VLF_Zero.Value = (Decimal)Global.TuiJinQiBuChang_VLF_Zero;
                numericUpDown_VLB_Zero.Value = (Decimal)Global.TuiJinQiBuChang_VLB_Zero;
                numericUpDown_VRF_Zero.Value = (Decimal)Global.TuiJinQiBuChang_VRF_Zero;
                numericUpDown_VRB_Zero.Value = (Decimal)Global.TuiJinQiBuChang_VRB_Zero;

                comboBox_WaterCtlBox_SerialPort.Items.Clear();
                for (int i = 0; i < Global.m_FormSerialWaterBoxCtl.comboBox_SerialPort.Items.Count; i++)
                {
                    comboBox_WaterCtlBox_SerialPort.Items.Add(Global.m_FormSerialWaterBoxCtl.comboBox_SerialPort.Items[i]);
                }
                comboBox_WaterCtlBox_SerialPort.SelectedItem = Global.m_FormSerialWaterBoxCtl.comboBox_SerialPort.SelectedItem;

                comboBox_ROVPower_SerialPort.Items.Clear();
                for (int i = 0; i < Global.m_FormSerialRovPowerCtl.comboBox_SerialPort.Items.Count; i++)
                {
                    comboBox_ROVPower_SerialPort.Items.Add(Global.m_FormSerialRovPowerCtl.comboBox_SerialPort.Items[i]);
                }
                comboBox_ROVPower_SerialPort.SelectedItem = Global.m_FormSerialRovPowerCtl.comboBox_SerialPort.SelectedItem;

                comboBox_JuYuanJianCe_1_SerialPort.Items.Clear();
                for (int i = 0; i < Global.m_FormSerialJuYuanJianCe1Ctl.comboBox_SerialPort.Items.Count; i++)
                {
                    comboBox_JuYuanJianCe_1_SerialPort.Items.Add(Global.m_FormSerialJuYuanJianCe1Ctl.comboBox_SerialPort.Items[i]);
                }
                comboBox_JuYuanJianCe_1_SerialPort.SelectedItem = Global.m_FormSerialJuYuanJianCe1Ctl.comboBox_SerialPort.SelectedItem;

                comboBox_JuYuanJianCe_2_SerialPort.Items.Clear();
                for (int i = 0; i < Global.m_FormSerialJuYuanJianCe2Ctl.comboBox_SerialPort.Items.Count; i++)
                {
                    comboBox_JuYuanJianCe_2_SerialPort.Items.Add(Global.m_FormSerialJuYuanJianCe2Ctl.comboBox_SerialPort.Items[i]);
                }
                comboBox_JuYuanJianCe_2_SerialPort.SelectedItem = Global.m_FormSerialJuYuanJianCe2Ctl.comboBox_SerialPort.SelectedItem;

                ReadBasicParasConfig();
                // label_ZuanJin_JiZhun.Text = Math.Round(Global.BasicParas_ZuanJin_ShenDu, 1).ToString();

                btn_Light_1.Text = Global.sName_Light_1;
                btn_Light_2.Text = Global.sName_Light_2;
                btn_Light_3.Text = Global.sName_Light_3;
                btn_Light_4.Text = Global.sName_Light_4;
                btn_Light_5.Text = Global.sName_Light_5;
                btn_Light_6.Text = Global.sName_Light_6;
                btn_Light_7.Text = Global.sName_Light_7;
                btn_Light_8.Text = Global.sName_Light_8;
                btn_Camera_1.Text = Global.sName_Camera_1;
                btn_Camera_2.Text = Global.sName_Camera_2;
                btn_Camera_3.Text = Global.sName_Camera_3;
                btn_Camera_4.Text = Global.sName_Camera_4;
                btn_Camera_5.Text = Global.sName_Camera_5;
                btn_Camera_6.Text = Global.sName_Camera_6;
                btn_Camera_7.Text = Global.sName_Camera_7;
                btn_Camera_8.Text = Global.sName_Camera_8;

                gaugeControl_BuChang_1.SetPointerValue("Pointer1", 0);
                gaugeControl_BuChang_2.SetPointerValue("Pointer1", 0);
                gaugeControl_BuChang_3.SetPointerValue("Pointer1", 0);
                gaugeControl_BuChang_4.SetPointerValue("Pointer1", 0);
                gaugeControl_YouXiang.SetPointerValue("Pointer1", 0);

                textBox_XuanFuJiZhun.Text = Global.TuiJinQiBuChang_XuanFu.ToString();
                numericUpDown_FuQian.Value = Convert.ToDecimal(Global.TuiJinQiBuChang_XuanFu);
            }
            catch (Exception ex)
            { }
        }



        private void FormMainUserA_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                stopThread = true;
                stopThreadFuQianOper = true;
                stopThreadDiCiJiaJiaoOper = true;
                stopThread_TuiJinQi_HMove_Oper = true;
                stopThreadAutoHighOper = true;

                if (threadServoValvePackOper != null && threadServoValvePackOper.IsAlive)
                {
                    threadServoValvePackOper.Abort();
                }
                if (threadFuQianOper != null && threadFuQianOper.IsAlive)
                {
                    threadFuQianOper.Abort();
                }

                if (thread_TuiJinQi_HMove_Oper != null && thread_TuiJinQi_HMove_Oper.IsAlive)
                {
                    thread_TuiJinQi_HMove_Oper.Abort();
                }
                if (threadAutoDirCtlOper != null && threadAutoDirCtlOper.IsAlive)
                {
                    threadAutoDirCtlOper.Abort();
                }

                if (threadAutoHighOper != null && threadAutoHighOper.IsAlive)
                {
                    threadAutoHighOper.Abort();
                }

            }
            catch (Exception ex)
            { }
        }


        #region DataGridView右键清除菜单
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

        #endregion


        #region Button按钮状态变化接收
        private delegate void UpdateFormButtonDelegate(object sender, GEventArgs gEventArgs);
        public void ReceiveDataFromButtonStatusChanged(object sender, EventArgs e)
        {
            try
            {
                GEventArgs gEventArgs = (GEventArgs)e;
                UpdateFormButtonDelegate updateFormButtonDelegate = new UpdateFormButtonDelegate(UpdateFormButton);
                this.BeginInvoke(updateFormButtonDelegate, new object[] { sender, gEventArgs });
            }
            catch (Exception ex)
            { }
        }


        private bool bSerialValvePackOperOK = false;//16功能阀箱控制操作执行结果
        private void UpdateFormButton(object sender, GEventArgs gEventArgs)
        {
            try
            {
                Struct_Btn_Status_EventSend myStruct_Btn_Status_EventSend = gEventArgs.myStruct_Btn_Status_EventSend;
                if (gEventArgs.dataType == 10)
                {
                    string sName = myStruct_Btn_Status_EventSend.sName;
                    Control ctl = GroupBox_CtlBtns.Controls.Find(sName, true)[0];
                    Button btn = ctl as Button;
                    btn.BackColor = myStruct_Btn_Status_EventSend.backColor;
                }
                else if (gEventArgs.dataType == 11)
                {
                }
                else if (gEventArgs.dataType == 12)
                {
                    string sInfo = myStruct_Btn_Status_EventSend.sInfo;
                    sInfo += "\t\n";
                    if (richTextBox_InfoShow.Lines.Length > 30)
                    {
                        richTextBox_InfoShow.Clear();
                    }
                    richTextBox_InfoShow.AppendText(sInfo);
                }
                else if (gEventArgs.dataType == 13)//功能阀箱执行的最终结果
                {
                    bool bRes = gEventArgs.isBtnOperOK;
                    if (bRes == false)
                    {
                        bSerialValvePackOperOK = false;
                    }
                    else
                    {
                        bSerialValvePackOperOK = true;
                    }
                }
                else if (gEventArgs.dataType == 14)//14-执行设置操作，将设置指令传出，对于16功能阀箱
                {
                    richTextBox_InfoShow.AppendText((string)gEventArgs.obj);
                }
            }
            catch (Exception ex)
            { }
        }

        #endregion


        private string GetAlarmResult(double phy, StatusUnitParas listAlarmParas, ref int alarm)
        {
            try
            {
                List<UnitResultCompareShowInfo> yellowParas = listAlarmParas.yellowParas; //预警值范围
                List<UnitResultCompareShowInfo> redParas = listAlarmParas.redParas; ;    //报警值范围
                alarm = 0;
                string infoShow = "";
                bool bAlarmYellow = false;
                string sAlarmYellow = "";
                bool bAlarmRed = false;
                string sAlarmRed = "";
                //判断黄色预警
                if (yellowParas == null || yellowParas.Count <= 0)
                {
                    bAlarmYellow = false;
                }
                else
                {
                    int count = yellowParas.Count;
                    for (int i = 0; i < count; i++)
                    {
                        if (phy >= yellowParas[i].min && phy <= yellowParas[i].max)
                        {
                            sAlarmYellow = yellowParas[i].info;
                            bAlarmYellow = true;
                            break;
                        }
                    }
                }
                //判断红色预警
                if (redParas == null || redParas.Count <= 0)
                {
                    bAlarmRed = false;
                }
                else
                {
                    int count = redParas.Count;
                    for (int i = 0; i < count; i++)
                    {
                        if (phy >= redParas[i].min && phy <= redParas[i].max)
                        {
                            sAlarmRed = redParas[i].info;
                            bAlarmRed = true;
                            break;
                        }
                    }
                }
                //有红色报警，则忽略黄色预警
                if (bAlarmRed)
                {
                    alarm = 2;
                    infoShow = sAlarmRed;
                }
                else if (bAlarmYellow)
                {
                    alarm = 1;
                    infoShow = sAlarmYellow;
                }
                else
                {
                    alarm = 0;
                    infoShow = "";
                }

                return infoShow;
            }
            catch (Exception ex)
            {
                return "";
            }
        }



        #region 数据接收与显示
        //数据接收
        public void ReceiveDataFromSerial(object sender, EventArgs e)
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

        //记录上次艏向信息，用于计算圈数；
        double dHeadingLast = 0.1;
        int iHeadingCircle = 0;
        private delegate void UpdateFormDelegate(object sender, GEventArgs gEventArgs);
        private void UpdateForm(object sender, GEventArgs gEventArgs)
        {
            try
            {
                List<alarmInfoUnit> listAlarmInfo = new List<alarmInfoUnit>();

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
                else if (gEventArgs.dataType == 5)
                {
                    #region 基本板卡
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

                        flag_Communication_61 = true;

                        #region 报警信息判断

                        int alarmres = 0;
                        string alarmstr = "";
                        int bianHao = 0;
                        if (listStatusParasKeys.Exists((string x) => x == "DC_Insulation_Detection_VA_61" ? true : false))
                        {
                            alarmstr = GetAlarmResult(myStruct.VA, dicStatusParasByStatusNameLocal["DC_Insulation_Detection_VA_61"], ref alarmres);
                            bianHao = dicStatusParasByStatusNameLocal["DC_Insulation_Detection_VA_61"].bianHao;
                            if (alarmres > 0)
                            {
                                alarmInfoUnit aiu = new alarmInfoUnit();
                                aiu.index = bianHao;
                                aiu.alarm = alarmres;
                                aiu.info = alarmstr;
                                listAlarmInfo.Add(aiu);
                                if (alarmres == 1)
                                {
                                    textBox_DCDetectBoard61_VA.BackColor = Color.Yellow;
                                }
                                else if (alarmres == 2)
                                {
                                    textBox_DCDetectBoard61_VA.BackColor = Color.Red;
                                }
                            }
                            else
                            {
                                textBox_DCDetectBoard61_VA.BackColor = Color.Transparent;
                            }
                        }

                        if (listStatusParasKeys.Exists((string x) => x == "DC_Insulation_Detection_IA_61" ? true : false))
                        {
                            alarmstr = GetAlarmResult(myStruct.IA, dicStatusParasByStatusNameLocal["DC_Insulation_Detection_IA_61"], ref alarmres);
                            bianHao = dicStatusParasByStatusNameLocal["DC_Insulation_Detection_IA_61"].bianHao;
                            if (alarmres > 0)
                            {
                                alarmInfoUnit aiu = new alarmInfoUnit();
                                aiu.index = bianHao;
                                aiu.alarm = alarmres;
                                aiu.info = alarmstr;
                                listAlarmInfo.Add(aiu);
                                if (alarmres == 1)
                                {
                                    textBox_DCDetectBoard61_IA.BackColor = Color.Yellow;
                                }
                                else if (alarmres == 2)
                                {
                                    textBox_DCDetectBoard61_IA.BackColor = Color.Red;
                                }
                            }
                            else
                            {
                                textBox_DCDetectBoard61_IA.BackColor = Color.Transparent;
                            }
                        }

                        if (listStatusParasKeys.Exists((string x) => x == "DC_Insulation_Detection_GA_61" ? true : false))
                        {
                            alarmstr = GetAlarmResult(myStruct.GA, dicStatusParasByStatusNameLocal["DC_Insulation_Detection_GA_61"], ref alarmres);
                            bianHao = dicStatusParasByStatusNameLocal["DC_Insulation_Detection_GA_61"].bianHao;
                            if (alarmres > 0)
                            {
                                alarmInfoUnit aiu = new alarmInfoUnit();
                                aiu.index = bianHao;
                                aiu.alarm = alarmres;
                                aiu.info = alarmstr;
                                listAlarmInfo.Add(aiu);
                                if (alarmres == 1)
                                {
                                    textBox_DCDetectBoard61_GA.BackColor = Color.Yellow;
                                }
                                else if (alarmres == 2)
                                {
                                    textBox_DCDetectBoard61_GA.BackColor = Color.Red;
                                }
                            }
                            else
                            {
                                textBox_DCDetectBoard61_GA.BackColor = Color.Transparent;
                            }
                        }

                        if (listStatusParasKeys.Exists((string x) => x == "DC_Insulation_Detection_TA_61" ? true : false))
                        {
                            alarmstr = GetAlarmResult(myStruct.TA, dicStatusParasByStatusNameLocal["DC_Insulation_Detection_TA_61"], ref alarmres);
                            bianHao = dicStatusParasByStatusNameLocal["DC_Insulation_Detection_TA_61"].bianHao;
                            if (alarmres > 0)
                            {
                                alarmInfoUnit aiu = new alarmInfoUnit();
                                aiu.index = bianHao;
                                aiu.alarm = alarmres;
                                aiu.info = alarmstr;
                                listAlarmInfo.Add(aiu);
                                if (alarmres == 1)
                                {
                                    textBox_DCDetectBoard61_TA.BackColor = Color.Yellow;
                                }
                                else if (alarmres == 2)
                                {
                                    textBox_DCDetectBoard61_TA.BackColor = Color.Red;
                                }
                            }
                            else
                            {
                                textBox_DCDetectBoard61_TA.BackColor = Color.Transparent;
                            }
                        }

                        if (listStatusParasKeys.Exists((string x) => x == "DC_Insulation_Detection_VB_61" ? true : false))
                        {
                            alarmstr = GetAlarmResult(myStruct.VB, dicStatusParasByStatusNameLocal["DC_Insulation_Detection_VB_61"], ref alarmres);
                            bianHao = dicStatusParasByStatusNameLocal["DC_Insulation_Detection_VB_61"].bianHao;
                            if (alarmres > 0)
                            {
                                alarmInfoUnit aiu = new alarmInfoUnit();
                                aiu.index = bianHao;
                                aiu.alarm = alarmres;
                                aiu.info = alarmstr;
                                listAlarmInfo.Add(aiu);
                                if (alarmres == 1)
                                {
                                    textBox_DCDetectBoard61_VB.BackColor = Color.Yellow;
                                }
                                else if (alarmres == 2)
                                {
                                    textBox_DCDetectBoard61_VB.BackColor = Color.Red;
                                }
                            }
                            else
                            {
                                textBox_DCDetectBoard61_VB.BackColor = Color.Transparent;
                            }
                        }

                        if (listStatusParasKeys.Exists((string x) => x == "DC_Insulation_Detection_IB_61" ? true : false))
                        {
                            alarmstr = GetAlarmResult(myStruct.IB, dicStatusParasByStatusNameLocal["DC_Insulation_Detection_IB_61"], ref alarmres);
                            bianHao = dicStatusParasByStatusNameLocal["DC_Insulation_Detection_IB_61"].bianHao;
                            if (alarmres > 0)
                            {
                                alarmInfoUnit aiu = new alarmInfoUnit();
                                aiu.index = bianHao;
                                aiu.alarm = alarmres;
                                aiu.info = alarmstr;
                                listAlarmInfo.Add(aiu);
                                if (alarmres == 1)
                                {
                                    textBox_DCDetectBoard61_IB.BackColor = Color.Yellow;
                                }
                                else if (alarmres == 2)
                                {
                                    textBox_DCDetectBoard61_IB.BackColor = Color.Red;
                                }
                            }
                            else
                            {
                                textBox_DCDetectBoard61_IB.BackColor = Color.Transparent;
                            }
                        }

                        if (listStatusParasKeys.Exists((string x) => x == "DC_Insulation_Detection_GB_61" ? true : false))
                        {
                            alarmstr = GetAlarmResult(myStruct.GB, dicStatusParasByStatusNameLocal["DC_Insulation_Detection_GB_61"], ref alarmres);
                            bianHao = dicStatusParasByStatusNameLocal["DC_Insulation_Detection_GB_61"].bianHao;
                            if (alarmres > 0)
                            {
                                alarmInfoUnit aiu = new alarmInfoUnit();
                                aiu.index = bianHao;
                                aiu.alarm = alarmres;
                                aiu.info = alarmstr;
                                listAlarmInfo.Add(aiu);
                                if (alarmres == 1)
                                {
                                    textBox_DCDetectBoard61_GB.BackColor = Color.Yellow;
                                }
                                else if (alarmres == 2)
                                {
                                    textBox_DCDetectBoard61_GB.BackColor = Color.Red;
                                }
                            }
                            else
                            {
                                textBox_DCDetectBoard61_GB.BackColor = Color.Transparent;
                            }
                        }

                        if (listStatusParasKeys.Exists((string x) => x == "DC_Insulation_Detection_TB_61" ? true : false))
                        {
                            alarmstr = GetAlarmResult(myStruct.TB, dicStatusParasByStatusNameLocal["DC_Insulation_Detection_TB_61"], ref alarmres);
                            bianHao = dicStatusParasByStatusNameLocal["DC_Insulation_Detection_TB_61"].bianHao;
                            if (alarmres > 0)
                            {
                                alarmInfoUnit aiu = new alarmInfoUnit();
                                aiu.index = bianHao;
                                aiu.alarm = alarmres;
                                aiu.info = alarmstr;
                                listAlarmInfo.Add(aiu);
                                if (alarmres == 1)
                                {
                                    textBox_DCDetectBoard61_TB.BackColor = Color.Yellow;
                                }
                                else if (alarmres == 2)
                                {
                                    textBox_DCDetectBoard61_TB.BackColor = Color.Red;
                                }
                            }
                            else
                            {
                                textBox_DCDetectBoard61_TB.BackColor = Color.Transparent;
                            }
                        }

                        if (listStatusParasKeys.Exists((string x) => x == "DC_Insulation_Detection_TO_61" ? true : false))
                        {
                            alarmstr = GetAlarmResult(myStruct.TO, dicStatusParasByStatusNameLocal["DC_Insulation_Detection_TO_61"], ref alarmres);
                            bianHao = dicStatusParasByStatusNameLocal["DC_Insulation_Detection_TO_61"].bianHao;
                            if (alarmres > 0)
                            {
                                alarmInfoUnit aiu = new alarmInfoUnit();
                                aiu.index = bianHao;
                                aiu.alarm = alarmres;
                                aiu.info = alarmstr;
                                listAlarmInfo.Add(aiu);
                                if (alarmres == 1)
                                {
                                    textBox_DCDetectBoard61_TO.BackColor = Color.Yellow;
                                }
                                else if (alarmres == 2)
                                {
                                    textBox_DCDetectBoard61_TO.BackColor = Color.Red;
                                }
                            }
                            else
                            {
                                textBox_DCDetectBoard61_TO.BackColor = Color.Transparent;
                            }
                        }


                        #endregion
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

                        flag_Communication_62 = true;

                        #region 报警信息判断

                        int alarmres = 0;
                        string alarmstr = "";
                        int bianHao = 0;
                        if (listStatusParasKeys.Exists((string x) => x == "DC_Insulation_Detection_VA_62" ? true : false))
                        {
                            alarmstr = GetAlarmResult(myStruct.VA, dicStatusParasByStatusNameLocal["DC_Insulation_Detection_VA_62"], ref alarmres);
                            bianHao = dicStatusParasByStatusNameLocal["DC_Insulation_Detection_VA_62"].bianHao;
                            if (alarmres > 0)
                            {
                                alarmInfoUnit aiu = new alarmInfoUnit();
                                aiu.index = bianHao;
                                aiu.alarm = alarmres;
                                aiu.info = alarmstr;
                                listAlarmInfo.Add(aiu);
                                if (alarmres == 1)
                                {
                                    textBox_DCDetectBoard62_VA.BackColor = Color.Yellow;
                                }
                                else if (alarmres == 2)
                                {
                                    textBox_DCDetectBoard62_VA.BackColor = Color.Red;
                                }
                            }
                            else
                            {
                                textBox_DCDetectBoard62_VA.BackColor = Color.Transparent;
                            }
                        }

                        if (listStatusParasKeys.Exists((string x) => x == "DC_Insulation_Detection_IA_62" ? true : false))
                        {
                            alarmstr = GetAlarmResult(myStruct.IA, dicStatusParasByStatusNameLocal["DC_Insulation_Detection_IA_62"], ref alarmres);
                            bianHao = dicStatusParasByStatusNameLocal["DC_Insulation_Detection_IA_62"].bianHao;
                            if (alarmres > 0)
                            {
                                alarmInfoUnit aiu = new alarmInfoUnit();
                                aiu.index = bianHao;
                                aiu.alarm = alarmres;
                                aiu.info = alarmstr;
                                listAlarmInfo.Add(aiu);
                                if (alarmres == 1)
                                {
                                    textBox_DCDetectBoard62_IA.BackColor = Color.Yellow;
                                }
                                else if (alarmres == 2)
                                {
                                    textBox_DCDetectBoard62_IA.BackColor = Color.Red;
                                }
                            }
                            else
                            {
                                textBox_DCDetectBoard62_IA.BackColor = Color.Transparent;
                            }
                        }

                        if (listStatusParasKeys.Exists((string x) => x == "DC_Insulation_Detection_GA_62" ? true : false))
                        {
                            alarmstr = GetAlarmResult(myStruct.GA, dicStatusParasByStatusNameLocal["DC_Insulation_Detection_GA_62"], ref alarmres);
                            bianHao = dicStatusParasByStatusNameLocal["DC_Insulation_Detection_GA_62"].bianHao;
                            if (alarmres > 0)
                            {
                                alarmInfoUnit aiu = new alarmInfoUnit();
                                aiu.index = bianHao;
                                aiu.alarm = alarmres;
                                aiu.info = alarmstr;
                                listAlarmInfo.Add(aiu);
                                if (alarmres == 1)
                                {
                                    textBox_DCDetectBoard62_GA.BackColor = Color.Yellow;
                                }
                                else if (alarmres == 2)
                                {
                                    textBox_DCDetectBoard62_GA.BackColor = Color.Red;
                                }
                            }
                            else
                            {
                                textBox_DCDetectBoard62_GA.BackColor = Color.Transparent;
                            }
                        }

                        if (listStatusParasKeys.Exists((string x) => x == "DC_Insulation_Detection_TA_62" ? true : false))
                        {
                            alarmstr = GetAlarmResult(myStruct.TA, dicStatusParasByStatusNameLocal["DC_Insulation_Detection_TA_62"], ref alarmres);
                            bianHao = dicStatusParasByStatusNameLocal["DC_Insulation_Detection_TA_62"].bianHao;
                            if (alarmres > 0)
                            {
                                alarmInfoUnit aiu = new alarmInfoUnit();
                                aiu.index = bianHao;
                                aiu.alarm = alarmres;
                                aiu.info = alarmstr;
                                listAlarmInfo.Add(aiu);
                                if (alarmres == 1)
                                {
                                    textBox_DCDetectBoard62_TA.BackColor = Color.Yellow;
                                }
                                else if (alarmres == 2)
                                {
                                    textBox_DCDetectBoard62_TA.BackColor = Color.Red;
                                }
                            }
                            else
                            {
                                textBox_DCDetectBoard62_TA.BackColor = Color.Transparent;
                            }
                        }

                        if (listStatusParasKeys.Exists((string x) => x == "DC_Insulation_Detection_VB_62" ? true : false))
                        {
                            alarmstr = GetAlarmResult(myStruct.VB, dicStatusParasByStatusNameLocal["DC_Insulation_Detection_VB_62"], ref alarmres);
                            bianHao = dicStatusParasByStatusNameLocal["DC_Insulation_Detection_VB_62"].bianHao;
                            if (alarmres > 0)
                            {
                                alarmInfoUnit aiu = new alarmInfoUnit();
                                aiu.index = bianHao;
                                aiu.alarm = alarmres;
                                aiu.info = alarmstr;
                                listAlarmInfo.Add(aiu);
                                if (alarmres == 1)
                                {
                                    textBox_DCDetectBoard62_VB.BackColor = Color.Yellow;
                                }
                                else if (alarmres == 2)
                                {
                                    textBox_DCDetectBoard62_VB.BackColor = Color.Red;
                                }
                            }
                            else
                            {
                                textBox_DCDetectBoard62_VB.BackColor = Color.Transparent;
                            }
                        }

                        if (listStatusParasKeys.Exists((string x) => x == "DC_Insulation_Detection_IB_62" ? true : false))
                        {
                            alarmstr = GetAlarmResult(myStruct.IB, dicStatusParasByStatusNameLocal["DC_Insulation_Detection_IB_62"], ref alarmres);
                            bianHao = dicStatusParasByStatusNameLocal["DC_Insulation_Detection_IB_62"].bianHao;
                            if (alarmres > 0)
                            {
                                alarmInfoUnit aiu = new alarmInfoUnit();
                                aiu.index = bianHao;
                                aiu.alarm = alarmres;
                                aiu.info = alarmstr;
                                listAlarmInfo.Add(aiu);
                                if (alarmres == 1)
                                {
                                    textBox_DCDetectBoard62_IB.BackColor = Color.Yellow;
                                }
                                else if (alarmres == 2)
                                {
                                    textBox_DCDetectBoard62_IB.BackColor = Color.Red;
                                }
                            }
                            else
                            {
                                textBox_DCDetectBoard62_IB.BackColor = Color.Transparent;
                            }
                        }

                        if (listStatusParasKeys.Exists((string x) => x == "DC_Insulation_Detection_GB_62" ? true : false))
                        {
                            alarmstr = GetAlarmResult(myStruct.GB, dicStatusParasByStatusNameLocal["DC_Insulation_Detection_GB_62"], ref alarmres);
                            bianHao = dicStatusParasByStatusNameLocal["DC_Insulation_Detection_GB_62"].bianHao;
                            if (alarmres > 0)
                            {
                                alarmInfoUnit aiu = new alarmInfoUnit();
                                aiu.index = bianHao;
                                aiu.alarm = alarmres;
                                aiu.info = alarmstr;
                                listAlarmInfo.Add(aiu);
                                if (alarmres == 1)
                                {
                                    textBox_DCDetectBoard62_GB.BackColor = Color.Yellow;
                                }
                                else if (alarmres == 2)
                                {
                                    textBox_DCDetectBoard62_GB.BackColor = Color.Red;
                                }
                            }
                            else
                            {
                                textBox_DCDetectBoard62_GB.BackColor = Color.Transparent;
                            }
                        }

                        if (listStatusParasKeys.Exists((string x) => x == "DC_Insulation_Detection_TB_62" ? true : false))
                        {
                            alarmstr = GetAlarmResult(myStruct.TB, dicStatusParasByStatusNameLocal["DC_Insulation_Detection_TB_62"], ref alarmres);
                            bianHao = dicStatusParasByStatusNameLocal["DC_Insulation_Detection_TB_62"].bianHao;
                            if (alarmres > 0)
                            {
                                alarmInfoUnit aiu = new alarmInfoUnit();
                                aiu.index = bianHao;
                                aiu.alarm = alarmres;
                                aiu.info = alarmstr;
                                listAlarmInfo.Add(aiu);
                                if (alarmres == 1)
                                {
                                    textBox_DCDetectBoard62_TB.BackColor = Color.Yellow;
                                }
                                else if (alarmres == 2)
                                {
                                    textBox_DCDetectBoard62_TB.BackColor = Color.Red;
                                }
                            }
                            else
                            {
                                textBox_DCDetectBoard62_TB.BackColor = Color.Transparent;
                            }
                        }

                        if (listStatusParasKeys.Exists((string x) => x == "DC_Insulation_Detection_TO_62" ? true : false))
                        {
                            alarmstr = GetAlarmResult(myStruct.TO, dicStatusParasByStatusNameLocal["DC_Insulation_Detection_TO_62"], ref alarmres);
                            bianHao = dicStatusParasByStatusNameLocal["DC_Insulation_Detection_TO_62"].bianHao;
                            if (alarmres > 0)
                            {
                                alarmInfoUnit aiu = new alarmInfoUnit();
                                aiu.index = bianHao;
                                aiu.alarm = alarmres;
                                aiu.info = alarmstr;
                                listAlarmInfo.Add(aiu);
                                if (alarmres == 1)
                                {
                                    textBox_DCDetectBoard62_TO.BackColor = Color.Yellow;
                                }
                                else if (alarmres == 2)
                                {
                                    textBox_DCDetectBoard62_TO.BackColor = Color.Red;
                                }
                            }
                            else
                            {
                                textBox_DCDetectBoard62_TO.BackColor = Color.Transparent;
                            }
                        }


                        #endregion
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

                        flag_Communication_63 = true;

                        #region 报警信息判断

                        int alarmres = 0;
                        string alarmstr = "";
                        int bianHao = 0;
                        if (listStatusParasKeys.Exists((string x) => x == "DC_Insulation_Detection_VA_63" ? true : false))
                        {
                            alarmstr = GetAlarmResult(myStruct.VA, dicStatusParasByStatusNameLocal["DC_Insulation_Detection_VA_63"], ref alarmres);
                            bianHao = dicStatusParasByStatusNameLocal["DC_Insulation_Detection_VA_63"].bianHao;
                            if (alarmres > 0)
                            {
                                alarmInfoUnit aiu = new alarmInfoUnit();
                                aiu.index = bianHao;
                                aiu.alarm = alarmres;
                                aiu.info = alarmstr;
                                listAlarmInfo.Add(aiu);
                                if (alarmres == 1)
                                {
                                    textBox_DCDetectBoard63_VA.BackColor = Color.Yellow;
                                }
                                else if (alarmres == 2)
                                {
                                    textBox_DCDetectBoard63_VA.BackColor = Color.Red;
                                }
                            }
                            else
                            {
                                textBox_DCDetectBoard63_VA.BackColor = Color.Transparent;
                            }
                        }

                        if (listStatusParasKeys.Exists((string x) => x == "DC_Insulation_Detection_IA_63" ? true : false))
                        {
                            alarmstr = GetAlarmResult(myStruct.IA, dicStatusParasByStatusNameLocal["DC_Insulation_Detection_IA_63"], ref alarmres);
                            bianHao = dicStatusParasByStatusNameLocal["DC_Insulation_Detection_IA_63"].bianHao;
                            if (alarmres > 0)
                            {
                                alarmInfoUnit aiu = new alarmInfoUnit();
                                aiu.index = bianHao;
                                aiu.alarm = alarmres;
                                aiu.info = alarmstr;
                                listAlarmInfo.Add(aiu);
                                if (alarmres == 1)
                                {
                                    textBox_DCDetectBoard63_IA.BackColor = Color.Yellow;
                                }
                                else if (alarmres == 2)
                                {
                                    textBox_DCDetectBoard63_IA.BackColor = Color.Red;
                                }
                            }
                            else
                            {
                                textBox_DCDetectBoard63_IA.BackColor = Color.Transparent;
                            }
                        }

                        if (listStatusParasKeys.Exists((string x) => x == "DC_Insulation_Detection_GA_63" ? true : false))
                        {
                            alarmstr = GetAlarmResult(myStruct.GA, dicStatusParasByStatusNameLocal["DC_Insulation_Detection_GA_63"], ref alarmres);
                            bianHao = dicStatusParasByStatusNameLocal["DC_Insulation_Detection_GA_63"].bianHao;
                            if (alarmres > 0)
                            {
                                alarmInfoUnit aiu = new alarmInfoUnit();
                                aiu.index = bianHao;
                                aiu.alarm = alarmres;
                                aiu.info = alarmstr;
                                listAlarmInfo.Add(aiu);
                                if (alarmres == 1)
                                {
                                    textBox_DCDetectBoard63_GA.BackColor = Color.Yellow;
                                }
                                else if (alarmres == 2)
                                {
                                    textBox_DCDetectBoard63_GA.BackColor = Color.Red;
                                }
                            }
                            else
                            {
                                textBox_DCDetectBoard63_GA.BackColor = Color.Transparent;
                            }
                        }

                        if (listStatusParasKeys.Exists((string x) => x == "DC_Insulation_Detection_TA_63" ? true : false))
                        {
                            alarmstr = GetAlarmResult(myStruct.TA, dicStatusParasByStatusNameLocal["DC_Insulation_Detection_TA_63"], ref alarmres);
                            bianHao = dicStatusParasByStatusNameLocal["DC_Insulation_Detection_TA_63"].bianHao;
                            if (alarmres > 0)
                            {
                                alarmInfoUnit aiu = new alarmInfoUnit();
                                aiu.index = bianHao;
                                aiu.alarm = alarmres;
                                aiu.info = alarmstr;
                                listAlarmInfo.Add(aiu);
                                if (alarmres == 1)
                                {
                                    textBox_DCDetectBoard63_TA.BackColor = Color.Yellow;
                                }
                                else if (alarmres == 2)
                                {
                                    textBox_DCDetectBoard63_TA.BackColor = Color.Red;
                                }
                            }
                            else
                            {
                                textBox_DCDetectBoard63_TA.BackColor = Color.Transparent;
                            }
                        }

                        if (listStatusParasKeys.Exists((string x) => x == "DC_Insulation_Detection_VB_63" ? true : false))
                        {
                            alarmstr = GetAlarmResult(myStruct.VB, dicStatusParasByStatusNameLocal["DC_Insulation_Detection_VB_63"], ref alarmres);
                            bianHao = dicStatusParasByStatusNameLocal["DC_Insulation_Detection_VB_63"].bianHao;
                            if (alarmres > 0)
                            {
                                alarmInfoUnit aiu = new alarmInfoUnit();
                                aiu.index = bianHao;
                                aiu.alarm = alarmres;
                                aiu.info = alarmstr;
                                listAlarmInfo.Add(aiu);
                                if (alarmres == 1)
                                {
                                    textBox_DCDetectBoard63_VB.BackColor = Color.Yellow;
                                }
                                else if (alarmres == 2)
                                {
                                    textBox_DCDetectBoard63_VB.BackColor = Color.Red;
                                }
                            }
                            else
                            {
                                textBox_DCDetectBoard63_VB.BackColor = Color.Transparent;
                            }
                        }

                        if (listStatusParasKeys.Exists((string x) => x == "DC_Insulation_Detection_IB_63" ? true : false))
                        {
                            alarmstr = GetAlarmResult(myStruct.IB, dicStatusParasByStatusNameLocal["DC_Insulation_Detection_IB_63"], ref alarmres);
                            bianHao = dicStatusParasByStatusNameLocal["DC_Insulation_Detection_IB_63"].bianHao;
                            if (alarmres > 0)
                            {
                                alarmInfoUnit aiu = new alarmInfoUnit();
                                aiu.index = bianHao;
                                aiu.alarm = alarmres;
                                aiu.info = alarmstr;
                                listAlarmInfo.Add(aiu);
                                if (alarmres == 1)
                                {
                                    textBox_DCDetectBoard63_IB.BackColor = Color.Yellow;
                                }
                                else if (alarmres == 2)
                                {
                                    textBox_DCDetectBoard63_IB.BackColor = Color.Red;
                                }
                            }
                            else
                            {
                                textBox_DCDetectBoard63_IB.BackColor = Color.Transparent;
                            }
                        }

                        if (listStatusParasKeys.Exists((string x) => x == "DC_Insulation_Detection_GB_63" ? true : false))
                        {
                            alarmstr = GetAlarmResult(myStruct.GB, dicStatusParasByStatusNameLocal["DC_Insulation_Detection_GB_63"], ref alarmres);
                            bianHao = dicStatusParasByStatusNameLocal["DC_Insulation_Detection_GB_63"].bianHao;
                            if (alarmres > 0)
                            {
                                alarmInfoUnit aiu = new alarmInfoUnit();
                                aiu.index = bianHao;
                                aiu.alarm = alarmres;
                                aiu.info = alarmstr;
                                listAlarmInfo.Add(aiu);
                                if (alarmres == 1)
                                {
                                    textBox_DCDetectBoard63_GB.BackColor = Color.Yellow;
                                }
                                else if (alarmres == 2)
                                {
                                    textBox_DCDetectBoard63_GB.BackColor = Color.Red;
                                }
                            }
                            else
                            {
                                textBox_DCDetectBoard63_GB.BackColor = Color.Transparent;
                            }
                        }

                        if (listStatusParasKeys.Exists((string x) => x == "DC_Insulation_Detection_TB_63" ? true : false))
                        {
                            alarmstr = GetAlarmResult(myStruct.TB, dicStatusParasByStatusNameLocal["DC_Insulation_Detection_TB_63"], ref alarmres);
                            bianHao = dicStatusParasByStatusNameLocal["DC_Insulation_Detection_TB_63"].bianHao;
                            if (alarmres > 0)
                            {
                                alarmInfoUnit aiu = new alarmInfoUnit();
                                aiu.index = bianHao;
                                aiu.alarm = alarmres;
                                aiu.info = alarmstr;
                                listAlarmInfo.Add(aiu);
                                if (alarmres == 1)
                                {
                                    textBox_DCDetectBoard63_TB.BackColor = Color.Yellow;
                                }
                                else if (alarmres == 2)
                                {
                                    textBox_DCDetectBoard63_TB.BackColor = Color.Red;
                                }
                            }
                            else
                            {
                                textBox_DCDetectBoard63_TB.BackColor = Color.Transparent;
                            }
                        }

                        if (listStatusParasKeys.Exists((string x) => x == "DC_Insulation_Detection_TO_63" ? true : false))
                        {
                            alarmstr = GetAlarmResult(myStruct.TO, dicStatusParasByStatusNameLocal["DC_Insulation_Detection_TO_63"], ref alarmres);
                            bianHao = dicStatusParasByStatusNameLocal["DC_Insulation_Detection_TO_63"].bianHao;
                            if (alarmres > 0)
                            {
                                alarmInfoUnit aiu = new alarmInfoUnit();
                                aiu.index = bianHao;
                                aiu.alarm = alarmres;
                                aiu.info = alarmstr;
                                listAlarmInfo.Add(aiu);
                                if (alarmres == 1)
                                {
                                    textBox_DCDetectBoard63_TO.BackColor = Color.Yellow;
                                }
                                else if (alarmres == 2)
                                {
                                    textBox_DCDetectBoard63_TO.BackColor = Color.Red;
                                }
                            }
                            else
                            {
                                textBox_DCDetectBoard63_TO.BackColor = Color.Transparent;
                            }
                        }


                        #endregion
                    }
                    else if (gEventArgs.addressBoard == enum_AddressBoard.High_Power_DC_Insulation_Detection_Board_I)//大功率直流绝缘检测板#1
                    {
                        Struct_High_Power_DC_Insulation_Detection_Board myStruct = (Struct_High_Power_DC_Insulation_Detection_Board)gEventArgs.objParse;
                        textBox_HPDCDetectBoard70_V.Text = Math.Round(myStruct.V, 2).ToString();
                        textBox_HPDCDetectBoard70_I.Text = Math.Round(myStruct.I, 2).ToString();
                        textBox_HPDCDetectBoard70_G.Text = Math.Round(myStruct.G, 2).ToString();
                        textBox_HPDCDetectBoard70_T.Text = Math.Round(myStruct.T, 2).ToString();

                        flag_Communication_70 = true;

                        #region 报警信息判断

                        int alarmres = 0;
                        string alarmstr = "";
                        int bianHao = 0;

                        if (listStatusParasKeys.Exists((string x) => x == "High_Power_DC_Insulation_Detection_V_70" ? true : false))
                        {
                            alarmstr = GetAlarmResult(myStruct.V, dicStatusParasByStatusNameLocal["High_Power_DC_Insulation_Detection_V_70"], ref alarmres);
                            bianHao = dicStatusParasByStatusNameLocal["High_Power_DC_Insulation_Detection_V_70"].bianHao;
                            if (alarmres > 0)
                            {
                                alarmInfoUnit aiu = new alarmInfoUnit();
                                aiu.index = bianHao;
                                aiu.alarm = alarmres;
                                aiu.info = alarmstr;
                                listAlarmInfo.Add(aiu);
                                if (alarmres == 1)
                                {
                                    textBox_HPDCDetectBoard70_V.BackColor = Color.Yellow;
                                }
                                else if (alarmres == 2)
                                {
                                    textBox_HPDCDetectBoard70_V.BackColor = Color.Red;
                                }
                            }
                            else
                            {
                                textBox_HPDCDetectBoard70_V.BackColor = Color.Transparent;
                            }
                        }

                        if (listStatusParasKeys.Exists((string x) => x == "High_Power_DC_Insulation_Detection_I_70" ? true : false))
                        {
                            alarmstr = GetAlarmResult(myStruct.I, dicStatusParasByStatusNameLocal["High_Power_DC_Insulation_Detection_I_70"], ref alarmres);
                            bianHao = dicStatusParasByStatusNameLocal["High_Power_DC_Insulation_Detection_I_70"].bianHao;
                            if (alarmres > 0)
                            {
                                alarmInfoUnit aiu = new alarmInfoUnit();
                                aiu.index = bianHao;
                                aiu.alarm = alarmres;
                                aiu.info = alarmstr;
                                listAlarmInfo.Add(aiu);
                                if (alarmres == 1)
                                {
                                    textBox_HPDCDetectBoard70_I.BackColor = Color.Yellow;
                                }
                                else if (alarmres == 2)
                                {
                                    textBox_HPDCDetectBoard70_I.BackColor = Color.Red;
                                }
                            }
                            else
                            {
                                textBox_HPDCDetectBoard70_I.BackColor = Color.Transparent;
                            }
                        }

                        if (listStatusParasKeys.Exists((string x) => x == "High_Power_DC_Insulation_Detection_G_70" ? true : false))
                        {
                            alarmstr = GetAlarmResult(myStruct.G, dicStatusParasByStatusNameLocal["High_Power_DC_Insulation_Detection_G_70"], ref alarmres);
                            bianHao = dicStatusParasByStatusNameLocal["High_Power_DC_Insulation_Detection_G_70"].bianHao;
                            if (alarmres > 0)
                            {
                                alarmInfoUnit aiu = new alarmInfoUnit();
                                aiu.index = bianHao;
                                aiu.alarm = alarmres;
                                aiu.info = alarmstr;
                                listAlarmInfo.Add(aiu);
                                if (alarmres == 1)
                                {
                                    textBox_HPDCDetectBoard70_G.BackColor = Color.Yellow;
                                }
                                else if (alarmres == 2)
                                {
                                    textBox_HPDCDetectBoard70_G.BackColor = Color.Red;
                                }
                            }
                            else
                            {
                                textBox_HPDCDetectBoard70_G.BackColor = Color.Transparent;
                            }
                        }

                        if (listStatusParasKeys.Exists((string x) => x == "High_Power_DC_Insulation_Detection_T_70" ? true : false))
                        {
                            alarmstr = GetAlarmResult(myStruct.T, dicStatusParasByStatusNameLocal["High_Power_DC_Insulation_Detection_T_70"], ref alarmres);
                            bianHao = dicStatusParasByStatusNameLocal["High_Power_DC_Insulation_Detection_T_70"].bianHao;
                            if (alarmres > 0)
                            {
                                alarmInfoUnit aiu = new alarmInfoUnit();
                                aiu.index = bianHao;
                                aiu.alarm = alarmres;
                                aiu.info = alarmstr;
                                listAlarmInfo.Add(aiu);
                                if (alarmres == 1)
                                {
                                    textBox_HPDCDetectBoard70_T.BackColor = Color.Yellow;
                                }
                                else if (alarmres == 2)
                                {
                                    textBox_HPDCDetectBoard70_T.BackColor = Color.Red;
                                }
                            }
                            else
                            {
                                textBox_HPDCDetectBoard70_T.BackColor = Color.Transparent;
                            }
                        }

                        #endregion
                    }
                    else if (gEventArgs.addressBoard == enum_AddressBoard.High_Power_DC_Insulation_Detection_Board_II)//大功率直流绝缘检测板#2
                    {
                        Struct_High_Power_DC_Insulation_Detection_Board myStruct = (Struct_High_Power_DC_Insulation_Detection_Board)gEventArgs.objParse;
                        textBox_HPDCDetectBoard71_V.Text = Math.Round(myStruct.V, 2).ToString();
                        textBox_HPDCDetectBoard71_I.Text = Math.Round(myStruct.I, 2).ToString();
                        textBox_HPDCDetectBoard71_G.Text = Math.Round(myStruct.G, 2).ToString();
                        textBox_HPDCDetectBoard71_T.Text = Math.Round(myStruct.T, 2).ToString();

                        flag_Communication_71 = true;

                        #region 报警信息判断

                        int alarmres = 0;
                        string alarmstr = "";
                        int bianHao = 0;

                        if (listStatusParasKeys.Exists((string x) => x == "High_Power_DC_Insulation_Detection_V_71" ? true : false))
                        {
                            alarmstr = GetAlarmResult(myStruct.V, dicStatusParasByStatusNameLocal["High_Power_DC_Insulation_Detection_V_71"], ref alarmres);
                            bianHao = dicStatusParasByStatusNameLocal["High_Power_DC_Insulation_Detection_V_71"].bianHao;
                            if (alarmres > 0)
                            {
                                alarmInfoUnit aiu = new alarmInfoUnit();
                                aiu.index = bianHao;
                                aiu.alarm = alarmres;
                                aiu.info = alarmstr;
                                listAlarmInfo.Add(aiu);
                                if (alarmres == 1)
                                {
                                    textBox_HPDCDetectBoard71_V.BackColor = Color.Yellow;
                                }
                                else if (alarmres == 2)
                                {
                                    textBox_HPDCDetectBoard71_V.BackColor = Color.Red;
                                }
                            }
                            else
                            {
                                textBox_HPDCDetectBoard71_V.BackColor = Color.Transparent;
                            }
                        }

                        if (listStatusParasKeys.Exists((string x) => x == "High_Power_DC_Insulation_Detection_I_71" ? true : false))
                        {
                            alarmstr = GetAlarmResult(myStruct.I, dicStatusParasByStatusNameLocal["High_Power_DC_Insulation_Detection_I_71"], ref alarmres);
                            bianHao = dicStatusParasByStatusNameLocal["High_Power_DC_Insulation_Detection_I_71"].bianHao;
                            if (alarmres > 0)
                            {
                                alarmInfoUnit aiu = new alarmInfoUnit();
                                aiu.index = bianHao;
                                aiu.alarm = alarmres;
                                aiu.info = alarmstr;
                                listAlarmInfo.Add(aiu);
                                if (alarmres == 1)
                                {
                                    textBox_HPDCDetectBoard71_I.BackColor = Color.Yellow;
                                }
                                else if (alarmres == 2)
                                {
                                    textBox_HPDCDetectBoard71_I.BackColor = Color.Red;
                                }
                            }
                            else
                            {
                                textBox_HPDCDetectBoard71_I.BackColor = Color.Transparent;
                            }
                        }

                        if (listStatusParasKeys.Exists((string x) => x == "High_Power_DC_Insulation_Detection_G_71" ? true : false))
                        {
                            alarmstr = GetAlarmResult(myStruct.G, dicStatusParasByStatusNameLocal["High_Power_DC_Insulation_Detection_G_71"], ref alarmres);
                            bianHao = dicStatusParasByStatusNameLocal["High_Power_DC_Insulation_Detection_G_71"].bianHao;
                            if (alarmres > 0)
                            {
                                alarmInfoUnit aiu = new alarmInfoUnit();
                                aiu.index = bianHao;
                                aiu.alarm = alarmres;
                                aiu.info = alarmstr;
                                listAlarmInfo.Add(aiu);
                                if (alarmres == 1)
                                {
                                    textBox_HPDCDetectBoard71_G.BackColor = Color.Yellow;
                                }
                                else if (alarmres == 2)
                                {
                                    textBox_HPDCDetectBoard71_G.BackColor = Color.Red;
                                }
                            }
                            else
                            {
                                textBox_HPDCDetectBoard71_G.BackColor = Color.Transparent;
                            }
                        }

                        if (listStatusParasKeys.Exists((string x) => x == "High_Power_DC_Insulation_Detection_T_71" ? true : false))
                        {
                            alarmstr = GetAlarmResult(myStruct.T, dicStatusParasByStatusNameLocal["High_Power_DC_Insulation_Detection_T_71"], ref alarmres);
                            bianHao = dicStatusParasByStatusNameLocal["High_Power_DC_Insulation_Detection_T_71"].bianHao;
                            if (alarmres > 0)
                            {
                                alarmInfoUnit aiu = new alarmInfoUnit();
                                aiu.index = bianHao;
                                aiu.alarm = alarmres;
                                aiu.info = alarmstr;
                                listAlarmInfo.Add(aiu);
                                if (alarmres == 1)
                                {
                                    textBox_HPDCDetectBoard71_T.BackColor = Color.Yellow;
                                }
                                else if (alarmres == 2)
                                {
                                    textBox_HPDCDetectBoard71_T.BackColor = Color.Red;
                                }
                            }
                            else
                            {
                                textBox_HPDCDetectBoard71_T.BackColor = Color.Transparent;
                            }
                        }

                        #endregion
                    }
                    else if (gEventArgs.addressBoard == enum_AddressBoard.High_Power_DC_Insulation_Detection_Board_III)//大功率直流绝缘检测板#3
                    {
                        Struct_High_Power_DC_Insulation_Detection_Board myStruct = (Struct_High_Power_DC_Insulation_Detection_Board)gEventArgs.objParse;
                        textBox_HPDCDetectBoard72_V.Text = Math.Round(myStruct.V, 2).ToString();
                        textBox_HPDCDetectBoard72_I.Text = Math.Round(myStruct.I, 2).ToString();
                        textBox_HPDCDetectBoard72_G.Text = Math.Round(myStruct.G, 2).ToString();
                        textBox_HPDCDetectBoard72_T.Text = Math.Round(myStruct.T, 2).ToString();

                        flag_Communication_72 = true;

                        #region 报警信息判断

                        int alarmres = 0;
                        string alarmstr = "";
                        int bianHao = 0;

                        if (listStatusParasKeys.Exists((string x) => x == "High_Power_DC_Insulation_Detection_V_72" ? true : false))
                        {
                            alarmstr = GetAlarmResult(myStruct.V, dicStatusParasByStatusNameLocal["High_Power_DC_Insulation_Detection_V_72"], ref alarmres);
                            bianHao = dicStatusParasByStatusNameLocal["High_Power_DC_Insulation_Detection_V_72"].bianHao;
                            if (alarmres > 0)
                            {
                                alarmInfoUnit aiu = new alarmInfoUnit();
                                aiu.index = bianHao;
                                aiu.alarm = alarmres;
                                aiu.info = alarmstr;
                                listAlarmInfo.Add(aiu);
                                if (alarmres == 1)
                                {
                                    textBox_HPDCDetectBoard72_V.BackColor = Color.Yellow;
                                }
                                else if (alarmres == 2)
                                {
                                    textBox_HPDCDetectBoard72_V.BackColor = Color.Red;
                                }
                            }
                            else
                            {
                                textBox_HPDCDetectBoard72_V.BackColor = Color.Transparent;
                            }
                        }

                        if (listStatusParasKeys.Exists((string x) => x == "High_Power_DC_Insulation_Detection_I_72" ? true : false))
                        {
                            alarmstr = GetAlarmResult(myStruct.I, dicStatusParasByStatusNameLocal["High_Power_DC_Insulation_Detection_I_72"], ref alarmres);
                            bianHao = dicStatusParasByStatusNameLocal["High_Power_DC_Insulation_Detection_I_72"].bianHao;
                            if (alarmres > 0)
                            {
                                alarmInfoUnit aiu = new alarmInfoUnit();
                                aiu.index = bianHao;
                                aiu.alarm = alarmres;
                                aiu.info = alarmstr;
                                listAlarmInfo.Add(aiu);
                                if (alarmres == 1)
                                {
                                    textBox_HPDCDetectBoard72_I.BackColor = Color.Yellow;
                                }
                                else if (alarmres == 2)
                                {
                                    textBox_HPDCDetectBoard72_I.BackColor = Color.Red;
                                }
                            }
                            else
                            {
                                textBox_HPDCDetectBoard72_I.BackColor = Color.Transparent;
                            }
                        }

                        if (listStatusParasKeys.Exists((string x) => x == "High_Power_DC_Insulation_Detection_G_72" ? true : false))
                        {
                            alarmstr = GetAlarmResult(myStruct.G, dicStatusParasByStatusNameLocal["High_Power_DC_Insulation_Detection_G_72"], ref alarmres);
                            bianHao = dicStatusParasByStatusNameLocal["High_Power_DC_Insulation_Detection_G_72"].bianHao;
                            if (alarmres > 0)
                            {
                                alarmInfoUnit aiu = new alarmInfoUnit();
                                aiu.index = bianHao;
                                aiu.alarm = alarmres;
                                aiu.info = alarmstr;
                                listAlarmInfo.Add(aiu);
                                if (alarmres == 1)
                                {
                                    textBox_HPDCDetectBoard72_G.BackColor = Color.Yellow;
                                }
                                else if (alarmres == 2)
                                {
                                    textBox_HPDCDetectBoard72_G.BackColor = Color.Red;
                                }
                            }
                            else
                            {
                                textBox_HPDCDetectBoard72_G.BackColor = Color.Transparent;
                            }
                        }

                        if (listStatusParasKeys.Exists((string x) => x == "High_Power_DC_Insulation_Detection_T_72" ? true : false))
                        {
                            alarmstr = GetAlarmResult(myStruct.T, dicStatusParasByStatusNameLocal["High_Power_DC_Insulation_Detection_T_72"], ref alarmres);
                            bianHao = dicStatusParasByStatusNameLocal["High_Power_DC_Insulation_Detection_T_72"].bianHao;
                            if (alarmres > 0)
                            {
                                alarmInfoUnit aiu = new alarmInfoUnit();
                                aiu.index = bianHao;
                                aiu.alarm = alarmres;
                                aiu.info = alarmstr;
                                listAlarmInfo.Add(aiu);
                                if (alarmres == 1)
                                {
                                    textBox_HPDCDetectBoard72_T.BackColor = Color.Yellow;
                                }
                                else if (alarmres == 2)
                                {
                                    textBox_HPDCDetectBoard72_T.BackColor = Color.Red;
                                }
                            }
                            else
                            {
                                textBox_HPDCDetectBoard72_T.BackColor = Color.Transparent;
                            }
                        }

                        #endregion

                    }
                    else if (gEventArgs.addressBoard == enum_AddressBoard.Control_Panel_High_Power_DC_Insulation_Detection_Board)//大功率直流绝缘检测板控制板
                    {
                        Struct_Control_Panel_High_Power_DC_Insulation_Detection_Board myStruct = (Struct_Control_Panel_High_Power_DC_Insulation_Detection_Board)gEventArgs.objParse;
                        textBox_HPDCDetectBoardCtl_V.Text = Math.Round(myStruct.V, 2).ToString();
                        textBox_HPDCDetectBoardCtl_I.Text = Math.Round(myStruct.I, 2).ToString();

                        flag_Communication_79 = true;

                        #region 报警信息判断

                        int alarmres = 0;
                        string alarmstr = "";
                        int bianHao = 0;

                        if (listStatusParasKeys.Exists((string x) => x == "Control_Panel_High_Power_DC_Insulation_Detection_V" ? true : false))
                        {
                            alarmstr = GetAlarmResult(myStruct.V, dicStatusParasByStatusNameLocal["Control_Panel_High_Power_DC_Insulation_Detection_V"], ref alarmres);
                            bianHao = dicStatusParasByStatusNameLocal["Control_Panel_High_Power_DC_Insulation_Detection_V"].bianHao;
                            if (alarmres > 0)
                            {
                                alarmInfoUnit aiu = new alarmInfoUnit();
                                aiu.index = bianHao;
                                aiu.alarm = alarmres;
                                aiu.info = alarmstr;
                                listAlarmInfo.Add(aiu);
                                if (alarmres == 1)
                                {
                                    textBox_HPDCDetectBoardCtl_V.BackColor = Color.Yellow;
                                }
                                else if (alarmres == 2)
                                {
                                    textBox_HPDCDetectBoardCtl_V.BackColor = Color.Red;
                                }
                            }
                            else
                            {
                                textBox_HPDCDetectBoardCtl_V.BackColor = Color.Transparent;
                            }
                        }

                        if (listStatusParasKeys.Exists((string x) => x == "Control_Panel_High_Power_DC_Insulation_Detection_I" ? true : false))
                        {
                            alarmstr = GetAlarmResult(myStruct.I, dicStatusParasByStatusNameLocal["Control_Panel_High_Power_DC_Insulation_Detection_I"], ref alarmres);
                            bianHao = dicStatusParasByStatusNameLocal["Control_Panel_High_Power_DC_Insulation_Detection_I"].bianHao;
                            if (alarmres > 0)
                            {
                                alarmInfoUnit aiu = new alarmInfoUnit();
                                aiu.index = bianHao;
                                aiu.alarm = alarmres;
                                aiu.info = alarmstr;
                                listAlarmInfo.Add(aiu);
                                if (alarmres == 1)
                                {
                                    textBox_HPDCDetectBoardCtl_I.BackColor = Color.Yellow;
                                }
                                else if (alarmres == 2)
                                {
                                    textBox_HPDCDetectBoardCtl_I.BackColor = Color.Red;
                                }
                            }
                            else
                            {
                                textBox_HPDCDetectBoardCtl_I.BackColor = Color.Transparent;
                            }
                        }


                        #endregion
                    }
                    else if (gEventArgs.addressBoard == enum_AddressBoard.AC_Insulation_Detection_Board)//交流绝缘检测板
                    {
                        Struct_AC_Insulation_Detection_Board myStruct = (Struct_AC_Insulation_Detection_Board)gEventArgs.objParse;
                        textBox_ACDetectBoard_V.Text = Math.Round(myStruct.V, 2).ToString();
                        textBox_ACDetectBoard_I.Text = Math.Round(myStruct.I, 2).ToString();

                        flag_Communication_80 = true;

                        #region 报警信息判断

                        int alarmres = 0;
                        string alarmstr = "";
                        int bianHao = 0;

                        if (listStatusParasKeys.Exists((string x) => x == "AC_Insulation_Detection_V" ? true : false))
                        {
                            alarmstr = GetAlarmResult(myStruct.V, dicStatusParasByStatusNameLocal["AC_Insulation_Detection_V"], ref alarmres);
                            bianHao = dicStatusParasByStatusNameLocal["AC_Insulation_Detection_V"].bianHao;
                            if (alarmres > 0)
                            {
                                alarmInfoUnit aiu = new alarmInfoUnit();
                                aiu.index = bianHao;
                                aiu.alarm = alarmres;
                                aiu.info = alarmstr;
                                listAlarmInfo.Add(aiu);
                                if (alarmres == 1)
                                {
                                    textBox_ACDetectBoard_V.BackColor = Color.Yellow;
                                }
                                else if (alarmres == 2)
                                {
                                    textBox_ACDetectBoard_V.BackColor = Color.Red;
                                }
                            }
                            else
                            {
                                textBox_ACDetectBoard_V.BackColor = Color.Transparent;
                            }
                        }

                        if (listStatusParasKeys.Exists((string x) => x == "AC_Insulation_Detection_I" ? true : false))
                        {
                            alarmstr = GetAlarmResult(myStruct.I, dicStatusParasByStatusNameLocal["AC_Insulation_Detection_I"], ref alarmres);
                            bianHao = dicStatusParasByStatusNameLocal["AC_Insulation_Detection_I"].bianHao;
                            if (alarmres > 0)
                            {
                                alarmInfoUnit aiu = new alarmInfoUnit();
                                aiu.index = bianHao;
                                aiu.alarm = alarmres;
                                aiu.info = alarmstr;
                                listAlarmInfo.Add(aiu);
                                if (alarmres == 1)
                                {
                                    textBox_ACDetectBoard_I.BackColor = Color.Yellow;
                                }
                                else if (alarmres == 2)
                                {
                                    textBox_ACDetectBoard_I.BackColor = Color.Red;
                                }
                            }
                            else
                            {
                                textBox_ACDetectBoard_I.BackColor = Color.Transparent;
                            }
                        }

                        #endregion
                    }
                    else if (gEventArgs.addressBoard == enum_AddressBoard.Camera_Power_Relay_Board_I)//摄像机电源继电器板#1
                    {
                        Struct_Power_Relay_Board myStruct = (Struct_Power_Relay_Board)gEventArgs.objParse;
                        textBox_CameraPowerRelayBoard1_V.Text = Math.Round(myStruct.V, 2).ToString();

                        flag_Communication_25 = true;

                        #region 报警信息判断

                        int alarmres = 0;
                        string alarmstr = "";
                        int bianHao = 0;

                        if (listStatusParasKeys.Exists((string x) => x == "V_25" ? true : false))
                        {
                            alarmstr = GetAlarmResult(myStruct.V, dicStatusParasByStatusNameLocal["V_25"], ref alarmres);
                            bianHao = dicStatusParasByStatusNameLocal["V_25"].bianHao;
                            if (alarmres > 0)
                            {
                                alarmInfoUnit aiu = new alarmInfoUnit();
                                aiu.index = bianHao;
                                aiu.alarm = alarmres;
                                aiu.info = alarmstr;
                                listAlarmInfo.Add(aiu);
                                if (alarmres == 1)
                                {
                                    textBox_CameraPowerRelayBoard1_V.BackColor = Color.Yellow;
                                }
                                else if (alarmres == 2)
                                {
                                    textBox_CameraPowerRelayBoard1_V.BackColor = Color.Red;
                                }
                            }
                            else
                            {
                                textBox_CameraPowerRelayBoard1_V.BackColor = Color.Transparent;
                            }
                        }

                        #endregion
                    }
                    else if (gEventArgs.addressBoard == enum_AddressBoard.Camera_Power_Relay_Board_II)//摄像机电源继电器板#2
                    {
                        Struct_Power_Relay_Board myStruct = (Struct_Power_Relay_Board)gEventArgs.objParse;
                        textBox_CameraPowerRelayBoard2_V.Text = Math.Round(myStruct.V, 2).ToString();

                        flag_Communication_26 = true;

                        #region 报警信息判断

                        int alarmres = 0;
                        string alarmstr = "";
                        int bianHao = 0;

                        if (listStatusParasKeys.Exists((string x) => x == "V_26" ? true : false))
                        {
                            alarmstr = GetAlarmResult(myStruct.V, dicStatusParasByStatusNameLocal["V_26"], ref alarmres);
                            bianHao = dicStatusParasByStatusNameLocal["V_26"].bianHao;
                            if (alarmres > 0)
                            {
                                alarmInfoUnit aiu = new alarmInfoUnit();
                                aiu.index = bianHao;
                                aiu.alarm = alarmres;
                                aiu.info = alarmstr;
                                listAlarmInfo.Add(aiu);
                                if (alarmres == 1)
                                {
                                    textBox_CameraPowerRelayBoard2_V.BackColor = Color.Yellow;
                                }
                                else if (alarmres == 2)
                                {
                                    textBox_CameraPowerRelayBoard2_V.BackColor = Color.Red;
                                }
                            }
                            else
                            {
                                textBox_CameraPowerRelayBoard2_V.BackColor = Color.Transparent;
                            }
                        }

                        #endregion
                    }
                    else if (gEventArgs.addressBoard == enum_AddressBoard.Sensor_Power_Relay_Board_I)//传感器电源继电器板#1
                    {
                        Struct_Power_Relay_Board myStruct = (Struct_Power_Relay_Board)gEventArgs.objParse;
                        textBox_SensorPowerRelayBoard1_V.Text = Math.Round(myStruct.V, 2).ToString();

                        flag_Communication_28 = true;

                        #region 报警信息判断

                        int alarmres = 0;
                        string alarmstr = "";
                        int bianHao = 0;

                        if (listStatusParasKeys.Exists((string x) => x == "V_28" ? true : false))
                        {
                            alarmstr = GetAlarmResult(myStruct.V, dicStatusParasByStatusNameLocal["V_28"], ref alarmres);
                            bianHao = dicStatusParasByStatusNameLocal["V_28"].bianHao;
                            if (alarmres > 0)
                            {
                                alarmInfoUnit aiu = new alarmInfoUnit();
                                aiu.index = bianHao;
                                aiu.alarm = alarmres;
                                aiu.info = alarmstr;
                                listAlarmInfo.Add(aiu);
                                if (alarmres == 1)
                                {
                                    textBox_SensorPowerRelayBoard1_V.BackColor = Color.Yellow;
                                }
                                else if (alarmres == 2)
                                {
                                    textBox_SensorPowerRelayBoard1_V.BackColor = Color.Red;
                                }
                            }
                            else
                            {
                                textBox_SensorPowerRelayBoard1_V.BackColor = Color.Transparent;
                            }
                        }

                        #endregion
                    }
                    else if (gEventArgs.addressBoard == enum_AddressBoard.Sensor_Power_Relay_Board_II)//传感器电源继电器板#2
                    {
                        Struct_Power_Relay_Board myStruct = (Struct_Power_Relay_Board)gEventArgs.objParse;
                        textBox_SensorPowerRelayBoard2_V.Text = Math.Round(myStruct.V, 2).ToString();

                        flag_Communication_29 = true;

                        #region 报警信息判断

                        int alarmres = 0;
                        string alarmstr = "";
                        int bianHao = 0;

                        if (listStatusParasKeys.Exists((string x) => x == "V_29" ? true : false))
                        {
                            alarmstr = GetAlarmResult(myStruct.V, dicStatusParasByStatusNameLocal["V_29"], ref alarmres);
                            bianHao = dicStatusParasByStatusNameLocal["V_29"].bianHao;
                            if (alarmres > 0)
                            {
                                alarmInfoUnit aiu = new alarmInfoUnit();
                                aiu.index = bianHao;
                                aiu.alarm = alarmres;
                                aiu.info = alarmstr;
                                listAlarmInfo.Add(aiu);
                                if (alarmres == 1)
                                {
                                    textBox_SensorPowerRelayBoard2_V.BackColor = Color.Yellow;
                                }
                                else if (alarmres == 2)
                                {
                                    textBox_SensorPowerRelayBoard2_V.BackColor = Color.Red;
                                }
                            }
                            else
                            {
                                textBox_SensorPowerRelayBoard2_V.BackColor = Color.Transparent;
                            }
                        }

                        #endregion
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

                        flag_Communication_40 = true;

                        //textBox_TankDetectBoardCH3_2_2.Text = Math.Round(myStruct.CH3_2, 2).ToString();

                        //补偿1
                        if (myStruct.CH3_3 >= 0 && myStruct.CH3_3 <= 2.5)
                        {
                            gaugeControl_BuChang_1.SetPointerValue("Pointer1", myStruct.CH3_3);
                            label_BuChang_1.Text = Math.Round(myStruct.CH3_3, 2).ToString();
                        }
                        else if (myStruct.CH3_3 < 0)
                        {
                            gaugeControl_BuChang_1.SetPointerValue("Pointer1", 0);
                            label_BuChang_1.Text = "0";
                        }
                        else if (myStruct.CH3_3 > 2.5)
                        {
                            gaugeControl_BuChang_1.SetPointerValue("Pointer1", 2.5);
                            label_BuChang_1.Text = "2.5";
                        }

                        //补偿2
                        if (myStruct.CH3_4 >= 0 && myStruct.CH3_4 <= 2.5)
                        {
                            gaugeControl_BuChang_2.SetPointerValue("Pointer1", myStruct.CH3_4);
                            label_BuChang_2.Text = Math.Round(myStruct.CH3_4, 2).ToString();
                        }
                        else if (myStruct.CH3_4 < 0)
                        {
                            gaugeControl_BuChang_2.SetPointerValue("Pointer1", 0);
                            label_BuChang_2.Text = "0";
                        }
                        else if (myStruct.CH3_4 > 2.5)
                        {
                            gaugeControl_BuChang_2.SetPointerValue("Pointer1", 2.5);
                            label_BuChang_2.Text = "2.5";
                        }

                        //补偿3
                        if (myStruct.CH4_1 >= 0 && myStruct.CH4_1 <= 2.5)
                        {
                            gaugeControl_BuChang_3.SetPointerValue("Pointer1", myStruct.CH4_1);
                            label_BuChang_3.Text = Math.Round(myStruct.CH4_1, 2).ToString();
                        }
                        else if (myStruct.CH4_1 < 0)
                        {
                            gaugeControl_BuChang_3.SetPointerValue("Pointer1", 0);
                            label_BuChang_3.Text = "0";
                        }
                        else if (myStruct.CH4_1 > 2.5)
                        {
                            gaugeControl_BuChang_3.SetPointerValue("Pointer1", 2.5);
                            label_BuChang_3.Text = "2.5";
                        }

                        //补偿4
                        if (myStruct.CH4_2 >= 0 && myStruct.CH4_2 <= 2.5)
                        {
                            gaugeControl_BuChang_4.SetPointerValue("Pointer1", myStruct.CH4_2);
                            label_BuChang_4.Text = Math.Round(myStruct.CH4_2, 2).ToString();
                        }
                        else if (myStruct.CH4_2 < 0)
                        {
                            gaugeControl_BuChang_4.SetPointerValue("Pointer1", 0);
                            label_BuChang_4.Text = "0";
                        }
                        else if (myStruct.CH4_2 > 2.5)
                        {
                            gaugeControl_BuChang_4.SetPointerValue("Pointer1", 2.5);
                            label_BuChang_4.Text = "2.5";
                        }

                        //油箱补偿
                        if (myStruct.CH2_2 >= 0 && myStruct.CH2_2 <= 10)
                        {
                            gaugeControl_YouXiang.SetPointerValue("Pointer1", myStruct.CH2_2);
                            label_BuChang_5.Text = Math.Round(myStruct.CH2_2, 2).ToString();
                        }
                        else if (myStruct.CH2_2 < 0)
                        {
                            gaugeControl_YouXiang.SetPointerValue("Pointer1", 0);
                            label_BuChang_5.Text = "0";
                        }
                        else if (myStruct.CH2_2 > 10)
                        {
                            gaugeControl_YouXiang.SetPointerValue("Pointer1", 10);
                            label_BuChang_5.Text = "2.5";
                        }

                        now_BasicParas_ZuanJin_ShenDu = myStruct.CH4_4;

                        //位移
                        //5.12
                        //if (myStruct.CH4_4 - Global.BasicParas_ZuanJin_ShenDu >= 0 && myStruct.CH4_4 - Global.BasicParas_ZuanJin_ShenDu <= 1800)
                        //{
                        //    gaugeControl3.SetPointerValue("Pointer1", myStruct.CH4_4 - Global.BasicParas_ZuanJin_ShenDu);
                        //}
                        //else if (myStruct.CH4_4 - Global.BasicParas_ZuanJin_ShenDu < 0)
                        //{
                        //    gaugeControl3.SetPointerValue("Pointer1", 0);
                        //}
                        //else if (myStruct.CH4_4 - Global.BasicParas_ZuanJin_ShenDu > 1800)
                        //{
                        //    gaugeControl3.SetPointerValue("Pointer1", 1800);
                        //}

                        #region 报警信息判断

                        int alarmres = 0;
                        string alarmstr = "";
                        int bianHao = 0;

                        if (listStatusParasKeys.Exists((string x) => x == "Tank_Detection_Board_CH1_1" ? true : false))
                        {
                            alarmstr = GetAlarmResult(myStruct.CH1_1, dicStatusParasByStatusNameLocal["Tank_Detection_Board_CH1_1"], ref alarmres);
                            bianHao = dicStatusParasByStatusNameLocal["Tank_Detection_Board_CH1_1"].bianHao;
                            if (alarmres > 0)
                            {
                                alarmInfoUnit aiu = new alarmInfoUnit();
                                aiu.index = bianHao;
                                aiu.alarm = alarmres;
                                aiu.info = alarmstr;
                                listAlarmInfo.Add(aiu);
                                if (alarmres == 1)
                                {
                                    textBox_TankDetectBoardCH1_1.BackColor = Color.Yellow;
                                }
                                else if (alarmres == 2)
                                {
                                    textBox_TankDetectBoardCH1_1.BackColor = Color.Red;
                                }
                            }
                            else
                            {
                                textBox_TankDetectBoardCH1_1.BackColor = Color.Transparent;
                            }
                        }

                        if (listStatusParasKeys.Exists((string x) => x == "Tank_Detection_Board_CH1_2" ? true : false))
                        {
                            alarmstr = GetAlarmResult(myStruct.CH1_2, dicStatusParasByStatusNameLocal["Tank_Detection_Board_CH1_2"], ref alarmres);
                            bianHao = dicStatusParasByStatusNameLocal["Tank_Detection_Board_CH1_2"].bianHao;
                            if (alarmres > 0)
                            {
                                alarmInfoUnit aiu = new alarmInfoUnit();
                                aiu.index = bianHao;
                                aiu.alarm = alarmres;
                                aiu.info = alarmstr;
                                listAlarmInfo.Add(aiu);
                                if (alarmres == 1)
                                {
                                    textBox_TankDetectBoardCH1_2.BackColor = Color.Yellow;
                                }
                                else if (alarmres == 2)
                                {
                                    textBox_TankDetectBoardCH1_2.BackColor = Color.Red;
                                }
                            }
                            else
                            {
                                textBox_TankDetectBoardCH1_2.BackColor = Color.Transparent;
                            }
                        }

                        if (listStatusParasKeys.Exists((string x) => x == "Tank_Detection_Board_CH1_3" ? true : false))
                        {
                            alarmstr = GetAlarmResult(myStruct.CH1_3, dicStatusParasByStatusNameLocal["Tank_Detection_Board_CH1_3"], ref alarmres);
                            bianHao = dicStatusParasByStatusNameLocal["Tank_Detection_Board_CH1_3"].bianHao;
                            if (alarmres > 0)
                            {
                                alarmInfoUnit aiu = new alarmInfoUnit();
                                aiu.index = bianHao;
                                aiu.alarm = alarmres;
                                aiu.info = alarmstr;
                                listAlarmInfo.Add(aiu);
                                if (alarmres == 1)
                                {
                                    textBox_TankDetectBoardCH1_3.BackColor = Color.Yellow;
                                }
                                else if (alarmres == 2)
                                {
                                    textBox_TankDetectBoardCH1_3.BackColor = Color.Red;
                                }
                            }
                            else
                            {
                                textBox_TankDetectBoardCH1_3.BackColor = Color.Transparent;
                            }
                        }

                        if (listStatusParasKeys.Exists((string x) => x == "Tank_Detection_Board_CH1_4" ? true : false))
                        {
                            alarmstr = GetAlarmResult(myStruct.CH1_4, dicStatusParasByStatusNameLocal["Tank_Detection_Board_CH1_4"], ref alarmres);
                            bianHao = dicStatusParasByStatusNameLocal["Tank_Detection_Board_CH1_4"].bianHao;
                            if (alarmres > 0)
                            {
                                alarmInfoUnit aiu = new alarmInfoUnit();
                                aiu.index = bianHao;
                                aiu.alarm = alarmres;
                                aiu.info = alarmstr;
                                listAlarmInfo.Add(aiu);
                                if (alarmres == 1)
                                {
                                    textBox_TankDetectBoardCH1_4.BackColor = Color.Yellow;
                                }
                                else if (alarmres == 2)
                                {
                                    textBox_TankDetectBoardCH1_4.BackColor = Color.Red;
                                }
                            }
                            else
                            {
                                textBox_TankDetectBoardCH1_4.BackColor = Color.Transparent;
                            }
                        }

                        if (listStatusParasKeys.Exists((string x) => x == "Tank_Detection_Board_CH2_1" ? true : false))
                        {
                            alarmstr = GetAlarmResult(myStruct.CH2_1, dicStatusParasByStatusNameLocal["Tank_Detection_Board_CH2_1"], ref alarmres);
                            bianHao = dicStatusParasByStatusNameLocal["Tank_Detection_Board_CH2_1"].bianHao;
                            if (alarmres > 0)
                            {
                                alarmInfoUnit aiu = new alarmInfoUnit();
                                aiu.index = bianHao;
                                aiu.alarm = alarmres;
                                aiu.info = alarmstr;
                                listAlarmInfo.Add(aiu);
                                if (alarmres == 1)
                                {
                                    textBox_TankDetectBoardCH2_1.BackColor = Color.Yellow;
                                }
                                else if (alarmres == 2)
                                {
                                    textBox_TankDetectBoardCH2_1.BackColor = Color.Red;
                                }
                            }
                            else
                            {
                                textBox_TankDetectBoardCH2_1.BackColor = Color.Transparent;
                            }
                        }

                        if (listStatusParasKeys.Exists((string x) => x == "Tank_Detection_Board_CH2_2" ? true : false))
                        {
                            alarmstr = GetAlarmResult(myStruct.CH2_2, dicStatusParasByStatusNameLocal["Tank_Detection_Board_CH2_2"], ref alarmres);
                            bianHao = dicStatusParasByStatusNameLocal["Tank_Detection_Board_CH2_2"].bianHao;
                            if (alarmres > 0)
                            {
                                alarmInfoUnit aiu = new alarmInfoUnit();
                                aiu.index = bianHao;
                                aiu.alarm = alarmres;
                                aiu.info = alarmstr;
                                listAlarmInfo.Add(aiu);
                                if (alarmres == 1)
                                {
                                    textBox_TankDetectBoardCH2_2.BackColor = Color.Yellow;
                                }
                                else if (alarmres == 2)
                                {
                                    textBox_TankDetectBoardCH2_2.BackColor = Color.Red;
                                }
                            }
                            else
                            {
                                textBox_TankDetectBoardCH2_2.BackColor = Color.Transparent;
                            }
                        }

                        if (listStatusParasKeys.Exists((string x) => x == "Tank_Detection_Board_CH2_3" ? true : false))
                        {
                            alarmstr = GetAlarmResult(myStruct.CH2_3, dicStatusParasByStatusNameLocal["Tank_Detection_Board_CH2_3"], ref alarmres);
                            bianHao = dicStatusParasByStatusNameLocal["Tank_Detection_Board_CH2_3"].bianHao;
                            if (alarmres > 0)
                            {
                                alarmInfoUnit aiu = new alarmInfoUnit();
                                aiu.index = bianHao;
                                aiu.alarm = alarmres;
                                aiu.info = alarmstr;
                                listAlarmInfo.Add(aiu);
                                if (alarmres == 1)
                                {
                                    textBox_TankDetectBoardCH2_3.BackColor = Color.Yellow;
                                }
                                else if (alarmres == 2)
                                {
                                    textBox_TankDetectBoardCH2_3.BackColor = Color.Red;
                                }
                            }
                            else
                            {
                                textBox_TankDetectBoardCH2_3.BackColor = Color.Transparent;
                            }
                        }

                        if (listStatusParasKeys.Exists((string x) => x == "Tank_Detection_Board_CH2_4" ? true : false))
                        {
                            alarmstr = GetAlarmResult(myStruct.CH2_4, dicStatusParasByStatusNameLocal["Tank_Detection_Board_CH2_4"], ref alarmres);
                            bianHao = dicStatusParasByStatusNameLocal["Tank_Detection_Board_CH2_4"].bianHao;
                            if (alarmres > 0)
                            {
                                alarmInfoUnit aiu = new alarmInfoUnit();
                                aiu.index = bianHao;
                                aiu.alarm = alarmres;
                                aiu.info = alarmstr;
                                listAlarmInfo.Add(aiu);
                                if (alarmres == 1)
                                {
                                    textBox_TankDetectBoardCH2_4.BackColor = Color.Yellow;
                                }
                                else if (alarmres == 2)
                                {
                                    textBox_TankDetectBoardCH2_4.BackColor = Color.Red;
                                }
                            }
                            else
                            {
                                textBox_TankDetectBoardCH2_4.BackColor = Color.Transparent;
                            }
                        }

                        if (listStatusParasKeys.Exists((string x) => x == "Tank_Detection_Board_CH3_1" ? true : false))
                        {
                            alarmstr = GetAlarmResult(myStruct.CH3_1, dicStatusParasByStatusNameLocal["Tank_Detection_Board_CH3_1"], ref alarmres);
                            bianHao = dicStatusParasByStatusNameLocal["Tank_Detection_Board_CH3_1"].bianHao;
                            if (alarmres > 0)
                            {
                                alarmInfoUnit aiu = new alarmInfoUnit();
                                aiu.index = bianHao;
                                aiu.alarm = alarmres;
                                aiu.info = alarmstr;
                                listAlarmInfo.Add(aiu);
                                if (alarmres == 1)
                                {
                                    textBox_TankDetectBoardCH3_1.BackColor = Color.Yellow;
                                }
                                else if (alarmres == 2)
                                {
                                    textBox_TankDetectBoardCH3_1.BackColor = Color.Red;
                                }
                            }
                            else
                            {
                                textBox_TankDetectBoardCH3_1.BackColor = Color.Transparent;
                            }
                        }

                        if (listStatusParasKeys.Exists((string x) => x == "Tank_Detection_Board_CH3_2" ? true : false))
                        {
                            alarmstr = GetAlarmResult(myStruct.CH3_2, dicStatusParasByStatusNameLocal["Tank_Detection_Board_CH3_2"], ref alarmres);
                            bianHao = dicStatusParasByStatusNameLocal["Tank_Detection_Board_CH3_2"].bianHao;
                            if (alarmres > 0)
                            {
                                alarmInfoUnit aiu = new alarmInfoUnit();
                                aiu.index = bianHao;
                                aiu.alarm = alarmres;
                                aiu.info = alarmstr;
                                listAlarmInfo.Add(aiu);
                                if (alarmres == 1)
                                {
                                    textBox_TankDetectBoardCH3_2.BackColor = Color.Yellow;
                                }
                                else if (alarmres == 2)
                                {
                                    textBox_TankDetectBoardCH3_2.BackColor = Color.Red;
                                }
                            }
                            else
                            {
                                textBox_TankDetectBoardCH3_2.BackColor = Color.Transparent;
                            }
                        }

                        if (listStatusParasKeys.Exists((string x) => x == "Tank_Detection_Board_CH3_3" ? true : false))
                        {
                            alarmstr = GetAlarmResult(myStruct.CH3_3, dicStatusParasByStatusNameLocal["Tank_Detection_Board_CH3_3"], ref alarmres);
                            bianHao = dicStatusParasByStatusNameLocal["Tank_Detection_Board_CH3_3"].bianHao;
                            if (alarmres > 0)
                            {
                                alarmInfoUnit aiu = new alarmInfoUnit();
                                aiu.index = bianHao;
                                aiu.alarm = alarmres;
                                aiu.info = alarmstr;
                                listAlarmInfo.Add(aiu);
                                if (alarmres == 1)
                                {
                                    textBox_TankDetectBoardCH3_3.BackColor = Color.Yellow;
                                }
                                else if (alarmres == 2)
                                {
                                    textBox_TankDetectBoardCH3_3.BackColor = Color.Red;
                                }
                            }
                            else
                            {
                                textBox_TankDetectBoardCH3_3.BackColor = Color.Transparent;
                            }
                        }

                        if (listStatusParasKeys.Exists((string x) => x == "Tank_Detection_Board_CH3_4" ? true : false))
                        {
                            alarmstr = GetAlarmResult(myStruct.CH3_4, dicStatusParasByStatusNameLocal["Tank_Detection_Board_CH3_4"], ref alarmres);
                            bianHao = dicStatusParasByStatusNameLocal["Tank_Detection_Board_CH3_4"].bianHao;
                            if (alarmres > 0)
                            {
                                alarmInfoUnit aiu = new alarmInfoUnit();
                                aiu.index = bianHao;
                                aiu.alarm = alarmres;
                                aiu.info = alarmstr;
                                listAlarmInfo.Add(aiu);
                                if (alarmres == 1)
                                {
                                    textBox_TankDetectBoardCH3_4.BackColor = Color.Yellow;
                                }
                                else if (alarmres == 2)
                                {
                                    textBox_TankDetectBoardCH3_4.BackColor = Color.Red;
                                }
                            }
                            else
                            {
                                textBox_TankDetectBoardCH3_4.BackColor = Color.Transparent;
                            }
                        }

                        if (listStatusParasKeys.Exists((string x) => x == "Tank_Detection_Board_CH4_1" ? true : false))
                        {
                            alarmstr = GetAlarmResult(myStruct.CH4_1, dicStatusParasByStatusNameLocal["Tank_Detection_Board_CH4_1"], ref alarmres);
                            bianHao = dicStatusParasByStatusNameLocal["Tank_Detection_Board_CH4_1"].bianHao;
                            if (alarmres > 0)
                            {
                                alarmInfoUnit aiu = new alarmInfoUnit();
                                aiu.index = bianHao;
                                aiu.alarm = alarmres;
                                aiu.info = alarmstr;
                                listAlarmInfo.Add(aiu);
                                if (alarmres == 1)
                                {
                                    textBox_TankDetectBoardCH4_1.BackColor = Color.Yellow;
                                }
                                else if (alarmres == 2)
                                {
                                    textBox_TankDetectBoardCH4_1.BackColor = Color.Red;
                                }
                            }
                            else
                            {
                                textBox_TankDetectBoardCH4_1.BackColor = Color.Transparent;
                            }
                        }

                        if (listStatusParasKeys.Exists((string x) => x == "Tank_Detection_Board_CH4_2" ? true : false))
                        {
                            alarmstr = GetAlarmResult(myStruct.CH4_2, dicStatusParasByStatusNameLocal["Tank_Detection_Board_CH4_2"], ref alarmres);
                            bianHao = dicStatusParasByStatusNameLocal["Tank_Detection_Board_CH4_2"].bianHao;
                            if (alarmres > 0)
                            {
                                alarmInfoUnit aiu = new alarmInfoUnit();
                                aiu.index = bianHao;
                                aiu.alarm = alarmres;
                                aiu.info = alarmstr;
                                listAlarmInfo.Add(aiu);
                                if (alarmres == 1)
                                {
                                    textBox_TankDetectBoardCH4_2.BackColor = Color.Yellow;
                                }
                                else if (alarmres == 2)
                                {
                                    textBox_TankDetectBoardCH4_2.BackColor = Color.Red;
                                }
                            }
                            else
                            {
                                textBox_TankDetectBoardCH4_2.BackColor = Color.Transparent;
                            }
                        }

                        if (listStatusParasKeys.Exists((string x) => x == "Tank_Detection_Board_CH4_3" ? true : false))
                        {
                            alarmstr = GetAlarmResult(myStruct.CH4_3, dicStatusParasByStatusNameLocal["Tank_Detection_Board_CH4_3"], ref alarmres);
                            bianHao = dicStatusParasByStatusNameLocal["Tank_Detection_Board_CH4_3"].bianHao;
                            if (alarmres > 0)
                            {
                                alarmInfoUnit aiu = new alarmInfoUnit();
                                aiu.index = bianHao;
                                aiu.alarm = alarmres;
                                aiu.info = alarmstr;
                                listAlarmInfo.Add(aiu);
                                if (alarmres == 1)
                                {
                                    textBox_TankDetectBoardCH4_3.BackColor = Color.Yellow;
                                }
                                else if (alarmres == 2)
                                {
                                    textBox_TankDetectBoardCH4_3.BackColor = Color.Red;
                                }
                            }
                            else
                            {
                                textBox_TankDetectBoardCH4_3.BackColor = Color.Transparent;
                            }
                        }

                        if (listStatusParasKeys.Exists((string x) => x == "Tank_Detection_Board_CH4_4" ? true : false))
                        {
                            alarmstr = GetAlarmResult(myStruct.CH4_4, dicStatusParasByStatusNameLocal["Tank_Detection_Board_CH4_4"], ref alarmres);
                            bianHao = dicStatusParasByStatusNameLocal["Tank_Detection_Board_CH4_4"].bianHao;
                            if (alarmres > 0)
                            {
                                alarmInfoUnit aiu = new alarmInfoUnit();
                                aiu.index = bianHao;
                                aiu.alarm = alarmres;
                                aiu.info = alarmstr;
                                listAlarmInfo.Add(aiu);
                                if (alarmres == 1)
                                {
                                    textBox_TankDetectBoardCH4_4.BackColor = Color.Yellow;
                                }
                                else if (alarmres == 2)
                                {
                                    textBox_TankDetectBoardCH4_4.BackColor = Color.Red;
                                }
                            }
                            else
                            {
                                textBox_TankDetectBoardCH4_4.BackColor = Color.Transparent;
                            }
                        }

                        #endregion
                    }
                    else if (gEventArgs.addressBoard == enum_AddressBoard.Light_Relay_Board_I)//灯继电器板#1(灯舱继电器板)
                    {
                        Struct_Light_Relay_Board myStruct = (Struct_Light_Relay_Board)gEventArgs.objParse;
                        textBox_LightRelayBoard1.Text = Math.Round(myStruct.V, 2).ToString();

                        flag_Communication_21 = true;

                        #region 报警信息判断

                        int alarmres = 0;
                        string alarmstr = "";
                        int bianHao = 0;

                        if (listStatusParasKeys.Exists((string x) => x == "V_21" ? true : false))
                        {
                            alarmstr = GetAlarmResult(myStruct.V, dicStatusParasByStatusNameLocal["V_21"], ref alarmres);
                            bianHao = dicStatusParasByStatusNameLocal["V_21"].bianHao;
                            if (alarmres > 0)
                            {
                                alarmInfoUnit aiu = new alarmInfoUnit();
                                aiu.index = bianHao;
                                aiu.alarm = alarmres;
                                aiu.info = alarmstr;
                                listAlarmInfo.Add(aiu);
                                if (alarmres == 1)
                                {
                                    textBox_LightRelayBoard1.BackColor = Color.Yellow;
                                }
                                else if (alarmres == 2)
                                {
                                    textBox_LightRelayBoard1.BackColor = Color.Red;
                                }
                            }
                            else
                            {
                                textBox_LightRelayBoard1.BackColor = Color.Transparent;
                            }
                        }

                        #endregion
                    }
                    else if (gEventArgs.addressBoard == enum_AddressBoard.Light_Relay_Board_II)//灯继电器板#2(灯舱继电器板)
                    {
                        Struct_Light_Relay_Board myStruct = (Struct_Light_Relay_Board)gEventArgs.objParse;
                        textBox_LightRelayBoard2.Text = Math.Round(myStruct.V, 2).ToString();

                        flag_Communication_22 = true;

                        #region 报警信息判断

                        int alarmres = 0;
                        string alarmstr = "";
                        int bianHao = 0;

                        if (listStatusParasKeys.Exists((string x) => x == "V_22" ? true : false))
                        {
                            alarmstr = GetAlarmResult(myStruct.V, dicStatusParasByStatusNameLocal["V_22"], ref alarmres);
                            bianHao = dicStatusParasByStatusNameLocal["V_22"].bianHao;
                            if (alarmres > 0)
                            {
                                alarmInfoUnit aiu = new alarmInfoUnit();
                                aiu.index = bianHao;
                                aiu.alarm = alarmres;
                                aiu.info = alarmstr;
                                listAlarmInfo.Add(aiu);
                                if (alarmres == 1)
                                {
                                    textBox_LightRelayBoard2.BackColor = Color.Yellow;
                                }
                                else if (alarmres == 2)
                                {
                                    textBox_LightRelayBoard2.BackColor = Color.Red;
                                }
                            }
                            else
                            {
                                textBox_LightRelayBoard2.BackColor = Color.Transparent;
                            }
                        }

                        #endregion
                    }
                    else if (gEventArgs.addressBoard == enum_AddressBoard.Hight_Measure_Device)//高度计
                    {
                        Struct_HightMeasureDevice myStruct = (Struct_HightMeasureDevice)gEventArgs.objParse;
                        textBox_HeightMeasure_Hight.Text = myStruct.sHight;
                        //textBox_HeightMeasure_Hight_2.Text = myStruct.sHight;

                        value_High_Now = myStruct.Height;
                        if (flag_AutoHigh_Oper == false)
                        {
                            numericUpDown_AutoHigh.Value = (decimal)myStruct.Height;
                        }
                    }
                    else if (gEventArgs.addressBoard == enum_AddressBoard.Work_Station_Quire_Board)
                    {
                        Struct_Work_Station_Quire_Board myStruct =
                            (Struct_Work_Station_Quire_Board)gEventArgs.objParse;

                        textBoxWork_Station_Quire_Board_RotateSpeed.Text = myStruct.RotateSpeed.ToString();
                        // textBox_Speed.Text = myStruct.RotateSpeed.ToString();

                        if (myStruct.WorkStation_B == 1)
                        {
                            textBoxWork_Station_Quire_Board_1.BackColor = Color.Green;
                        }
                        else
                        {
                            textBoxWork_Station_Quire_Board_1.BackColor = Color.Transparent; ;
                        }
                        if (myStruct.WorkStation_C == 1)
                        {
                            textBoxWork_Station_Quire_Board_2.BackColor = Color.Green;
                        }
                        else
                        {
                            textBoxWork_Station_Quire_Board_2.BackColor = Color.Transparent; ;
                        }
                        if (myStruct.WorkStation_D == 1)
                        {
                            textBoxWork_Station_Quire_Board_3.BackColor = Color.Green;
                        }
                        else
                        {
                            textBoxWork_Station_Quire_Board_3.BackColor = Color.Transparent; ;
                        }
                        if (myStruct.WorkStation_E == 1)
                        {
                            textBoxWork_Station_Quire_Board_4.BackColor = Color.Green;
                        }
                        else
                        {
                            textBoxWork_Station_Quire_Board_4.BackColor = Color.Transparent; ;
                        }
                        if (myStruct.WorkStation_F == 1)
                        {
                            textBoxWork_Station_Quire_Board_5.BackColor = Color.Green;
                        }
                        else
                        {
                            textBoxWork_Station_Quire_Board_5.BackColor = Color.Transparent; ;
                        }
                        if (myStruct.WorkStation_G == 1)
                        {
                            textBoxWork_Station_Quire_Board_6.BackColor = Color.Green;
                        }
                        else
                        {
                            textBoxWork_Station_Quire_Board_6.BackColor = Color.Transparent; ;
                        }

                        flag_Communication_30 = true;
                    }
                    else if (gEventArgs.addressBoard == enum_AddressBoard.DianJi_T_Detection_Board)
                    {
                        Struct_DianJi_T myStruct = (Struct_DianJi_T)gEventArgs.objParse;

                        textBox_DianJi_Detecte_Para1.Text = Math.Round(myStruct.Para_1, 2).ToString();
                        textBox_DianJi_Detecte_Para2.Text = Math.Round(myStruct.Para_2, 2).ToString();
                        textBox_DianJi_Detecte_Para3.Text = Math.Round(myStruct.Para_3, 2).ToString();
                        textBox_DianJi_Detecte_Para4.Text = Math.Round(myStruct.Para_4, 2).ToString();
                        textBox_DianJi_Detecte_Para5.Text = Math.Round(myStruct.Para_5, 2).ToString();
                        textBox_DianJi_Detecte_Para6.Text = Math.Round(myStruct.Para_6, 2).ToString();
                        textBox_DianJi_Detecte_Para7.Text = Math.Round(myStruct.Para_7, 2).ToString();
                        textBox_DianJi_Detecte_Para8.Text = Math.Round(myStruct.Para_8, 2).ToString();

                        #region 报警信息判断

                        int alarmres = 0;
                        string alarmstr = "";
                        int bianHao = 0;

                        if (listStatusParasKeys.Exists((string x) => x == "DanJi_T_Para_1" ? true : false))
                        {
                            alarmstr = GetAlarmResult(myStruct.Para_1, dicStatusParasByStatusNameLocal["DanJi_T_Para_1"], ref alarmres);
                            bianHao = dicStatusParasByStatusNameLocal["DanJi_T_Para_1"].bianHao;
                            if (alarmres > 0)
                            {
                                alarmInfoUnit aiu = new alarmInfoUnit();
                                aiu.index = bianHao;
                                aiu.alarm = alarmres;
                                aiu.info = alarmstr;
                                listAlarmInfo.Add(aiu);
                                if (alarmres == 1)
                                {
                                    textBox_DianJi_Detecte_Para1.BackColor = Color.Yellow;
                                }
                                else if (alarmres == 2)
                                {
                                    textBox_DianJi_Detecte_Para1.BackColor = Color.Red;
                                }
                            }
                            else
                            {
                                textBox_DianJi_Detecte_Para1.BackColor = Color.Transparent;
                            }
                        }

                        if (listStatusParasKeys.Exists((string x) => x == "DanJi_T_Para_2" ? true : false))
                        {
                            alarmstr = GetAlarmResult(myStruct.Para_2, dicStatusParasByStatusNameLocal["DanJi_T_Para_2"], ref alarmres);
                            bianHao = dicStatusParasByStatusNameLocal["DanJi_T_Para_2"].bianHao;
                            if (alarmres > 0)
                            {
                                alarmInfoUnit aiu = new alarmInfoUnit();
                                aiu.index = bianHao;
                                aiu.alarm = alarmres;
                                aiu.info = alarmstr;
                                listAlarmInfo.Add(aiu);
                                if (alarmres == 1)
                                {
                                    textBox_DianJi_Detecte_Para2.BackColor = Color.Yellow;
                                }
                                else if (alarmres == 2)
                                {
                                    textBox_DianJi_Detecte_Para2.BackColor = Color.Red;
                                }
                            }
                            else
                            {
                                textBox_DianJi_Detecte_Para2.BackColor = Color.Transparent;
                            }

                            if (listStatusParasKeys.Exists((string x) => x == "DanJi_T_Para_3" ? true : false))
                            {
                                alarmstr = GetAlarmResult(myStruct.Para_3, dicStatusParasByStatusNameLocal["DanJi_T_Para_3"], ref alarmres);
                                bianHao = dicStatusParasByStatusNameLocal["DanJi_T_Para_3"].bianHao;
                                if (alarmres > 0)
                                {
                                    alarmInfoUnit aiu = new alarmInfoUnit();
                                    aiu.index = bianHao;
                                    aiu.alarm = alarmres;
                                    aiu.info = alarmstr;
                                    listAlarmInfo.Add(aiu);
                                    if (alarmres == 1)
                                    {
                                        textBox_DianJi_Detecte_Para3.BackColor = Color.Yellow;
                                    }
                                    else if (alarmres == 2)
                                    {
                                        textBox_DianJi_Detecte_Para3.BackColor = Color.Red;
                                    }
                                }
                                else
                                {
                                    textBox_DianJi_Detecte_Para3.BackColor = Color.Transparent;
                                }
                            }

                            if (listStatusParasKeys.Exists((string x) => x == "DanJi_T_Para_4" ? true : false))
                            {
                                alarmstr = GetAlarmResult(myStruct.Para_4, dicStatusParasByStatusNameLocal["DanJi_T_Para_4"], ref alarmres);
                                bianHao = dicStatusParasByStatusNameLocal["DanJi_T_Para_4"].bianHao;
                                if (alarmres > 0)
                                {
                                    alarmInfoUnit aiu = new alarmInfoUnit();
                                    aiu.index = bianHao;
                                    aiu.alarm = alarmres;
                                    aiu.info = alarmstr;
                                    listAlarmInfo.Add(aiu);
                                    if (alarmres == 1)
                                    {
                                        textBox_DianJi_Detecte_Para4.BackColor = Color.Yellow;
                                    }
                                    else if (alarmres == 2)
                                    {
                                        textBox_DianJi_Detecte_Para4.BackColor = Color.Red;
                                    }
                                }
                                else
                                {
                                    textBox_DianJi_Detecte_Para4.BackColor = Color.Transparent;
                                }
                            }

                            if (listStatusParasKeys.Exists((string x) => x == "DanJi_T_Para_5" ? true : false))
                            {
                                alarmstr = GetAlarmResult(myStruct.Para_5, dicStatusParasByStatusNameLocal["DanJi_T_Para_5"], ref alarmres);
                                bianHao = dicStatusParasByStatusNameLocal["DanJi_T_Para_5"].bianHao;
                                if (alarmres > 0)
                                {
                                    alarmInfoUnit aiu = new alarmInfoUnit();
                                    aiu.index = bianHao;
                                    aiu.alarm = alarmres;
                                    aiu.info = alarmstr;
                                    listAlarmInfo.Add(aiu);
                                    if (alarmres == 1)
                                    {
                                        textBox_DianJi_Detecte_Para5.BackColor = Color.Yellow;
                                    }
                                    else if (alarmres == 2)
                                    {
                                        textBox_DianJi_Detecte_Para5.BackColor = Color.Red;
                                    }
                                }
                                else
                                {
                                    textBox_DianJi_Detecte_Para5.BackColor = Color.Transparent;
                                }
                            }

                            if (listStatusParasKeys.Exists((string x) => x == "DanJi_T_Para_6" ? true : false))
                            {
                                alarmstr = GetAlarmResult(myStruct.Para_6, dicStatusParasByStatusNameLocal["DanJi_T_Para_6"], ref alarmres);
                                bianHao = dicStatusParasByStatusNameLocal["DanJi_T_Para_6"].bianHao;
                                if (alarmres > 0)
                                {
                                    alarmInfoUnit aiu = new alarmInfoUnit();
                                    aiu.index = bianHao;
                                    aiu.alarm = alarmres;
                                    aiu.info = alarmstr;
                                    listAlarmInfo.Add(aiu);
                                    if (alarmres == 1)
                                    {
                                        textBox_DianJi_Detecte_Para6.BackColor = Color.Yellow;
                                    }
                                    else if (alarmres == 2)
                                    {
                                        textBox_DianJi_Detecte_Para6.BackColor = Color.Red;
                                    }
                                }
                                else
                                {
                                    textBox_DianJi_Detecte_Para6.BackColor = Color.Transparent;
                                }
                            }

                            if (listStatusParasKeys.Exists((string x) => x == "DanJi_T_Para_7" ? true : false))
                            {
                                alarmstr = GetAlarmResult(myStruct.Para_7, dicStatusParasByStatusNameLocal["DanJi_T_Para_7"], ref alarmres);
                                bianHao = dicStatusParasByStatusNameLocal["DanJi_T_Para_7"].bianHao;
                                if (alarmres > 0)
                                {
                                    alarmInfoUnit aiu = new alarmInfoUnit();
                                    aiu.index = bianHao;
                                    aiu.alarm = alarmres;
                                    aiu.info = alarmstr;
                                    listAlarmInfo.Add(aiu);
                                    if (alarmres == 1)
                                    {
                                        textBox_DianJi_Detecte_Para7.BackColor = Color.Yellow;
                                    }
                                    else if (alarmres == 2)
                                    {
                                        textBox_DianJi_Detecte_Para7.BackColor = Color.Red;
                                    }
                                }
                                else
                                {
                                    textBox_DianJi_Detecte_Para7.BackColor = Color.Transparent;
                                }
                            }

                            if (listStatusParasKeys.Exists((string x) => x == "DanJi_T_Para_8" ? true : false))
                            {
                                alarmstr = GetAlarmResult(myStruct.Para_8, dicStatusParasByStatusNameLocal["DanJi_T_Para_8"], ref alarmres);
                                bianHao = dicStatusParasByStatusNameLocal["DanJi_T_Para_8"].bianHao;
                                if (alarmres > 0)
                                {
                                    alarmInfoUnit aiu = new alarmInfoUnit();
                                    aiu.index = bianHao;
                                    aiu.alarm = alarmres;
                                    aiu.info = alarmstr;
                                    listAlarmInfo.Add(aiu);
                                    if (alarmres == 1)
                                    {
                                        textBox_DianJi_Detecte_Para8.BackColor = Color.Yellow;
                                    }
                                    else if (alarmres == 2)
                                    {
                                        textBox_DianJi_Detecte_Para8.BackColor = Color.Red;
                                    }
                                }
                                else
                                {
                                    textBox_DianJi_Detecte_Para8.BackColor = Color.Transparent;
                                }
                            }
                        }

                        #endregion
                    }

                    #endregion

                    #region 罗盘
                    else if (gEventArgs.addressBoard == enum_AddressBoard.Rotate_Panel_Device)//罗盘
                    {
                        
                        Struct_RotatePanelDevice myStruct = (Struct_RotatePanelDevice)gEventArgs.objParse;
                        gyroAngle1.HX = (float)Math.Round(myStruct.HX, 2);
                        gyroAngle1.HY = (float)Math.Round(myStruct.HY, 2);
                        gyroAngle1.HZ = (float)Math.Round(myStruct.HZ, 2);
                        gyroAngle1.Roll = (float)Math.Round(myStruct.Roll, 2);
                        gyroAngle1.Pitch = (float)Math.Round(myStruct.Pitch, 2);
                        gyroAngle1.Yaw = (float)Math.Round(myStruct.Yaw, 2);
                        /*
                        textBox_RotatePanel_HX_2.Text = Math.Round(myStruct.HX, 2).ToString();
                        textBox_RotatePanel_HY_2.Text = Math.Round(myStruct.HY, 2).ToString();
                        textBox_RotatePanel_HZ_2.Text = Math.Round(myStruct.HZ, 2).ToString();
                        textBox_RotatePanel_Roll_2.Text = Math.Round(myStruct.Roll, 2).ToString();
                        textBox_RotatePanel_Pitch_2.Text = Math.Round(myStruct.Pitch, 2).ToString();
                        textBox_RotatePanel_Yaw_2.Text = Math.Round(myStruct.Yaw, 2).ToString();
                        */
                        flag_Communication_LuoPan = true;

                        value_DiCiJiaJiao_Now = myStruct.Yaw;//地磁夹角目前方位
                        gaugeControl4.SetPointerValue("Pointer1", value_DiCiJiaJiao_Now);

                        //自动定向目标跟随值
                        if (flag_AutoDirCtl_Oper == false)
                        {
                            numericUpDown_DiCiJiaJiaoSet.Value = (decimal)myStruct.Yaw;
                        }


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
                        //textBox_HeadingCircle_2.Text = iHeadingCircle.ToString();

                        gaugeControl1.SetPointerValue("Pointer1", myStruct.Yaw);
                        gaugeControl1.SetPointerValue("Pointer2", iHeadingCircle);
                        //gaugeControl2.SetPointerValue("Pointer1", myStruct.Yaw);
                        //gaugeControl2.SetPointerValue("Pointer2", iHeadingCircle);
                        
                    }
                    #endregion

                    #region 16功能阀箱
                    else if (gEventArgs.addressBoard == enum_AddressBoard.ServoValvePacketBoard16Func)
                    {
                        Struct_BoardB_Status myStructBoardIIStatus = (Struct_BoardB_Status)gEventArgs.objParse;

                        flag_Communication_FaXiang16 = true;

                        //各解析量显示
                        richTextBox_Fun16_RecvInfo.Clear();
                        richTextBox_Fun16_RecvInfo.SelectionColor = Color.Black;
                        richTextBox_Fun16_RecvInfo.Text = myStructBoardIIStatus.sData;

                        for (int i = 0; i < myStructBoardIIStatus.indexSubstitution.Count; i++)
                        {
                            richTextBox_Fun16_RecvInfo.Select(myStructBoardIIStatus.indexSubstitution[i] * 2, 2);
                            richTextBox_Fun16_RecvInfo.SelectionColor = Color.Red;
                            richTextBox_Fun16_RecvInfo.SelectedText = "";
                        }
                        richTextBox_Fun16_RecvInfo.SelectionStart = myStructBoardIIStatus.sData.Length - 6;
                        richTextBox_Fun16_RecvInfo.SelectionLength = 4;
                        richTextBox_Fun16_RecvInfo.SelectionColor = Color.Blue;
                        richTextBox_Fun16_RecvInfo.SelectionLength = 0;
                        richTextBox_Fun16_RecvInfo.AppendText(myStructBoardIIStatus.sCRCResult);

                        if (myStructBoardIIStatus.bDIN8 == 1)
                        {
                            textBox_Fun16_DIN8.BackColor = Color.Green;
                        }
                        else
                        {
                            textBox_Fun16_DIN8.BackColor = Color.Transparent;
                        }
                        if (myStructBoardIIStatus.bDIN7 == 1)
                        {
                            textBox_Fun16_DIN7.BackColor = Color.Green;
                        }
                        else
                        {
                            textBox_Fun16_DIN7.BackColor = Color.Transparent;
                        }
                        if (myStructBoardIIStatus.bDIN6 == 1)
                        {
                            textBox_Fun16_DIN6.BackColor = Color.Green;
                        }
                        else
                        {
                            textBox_Fun16_DIN6.BackColor = Color.Transparent;
                        }
                        if (myStructBoardIIStatus.bDIN5 == 1)
                        {
                            textBox_Fun16_DIN5.BackColor = Color.Green;
                        }
                        else
                        {
                            textBox_Fun16_DIN5.BackColor = Color.Transparent;
                        }
                        if (myStructBoardIIStatus.bDIN4 == 1)
                        {
                            textBox_Fun16_DIN4.BackColor = Color.Green;
                        }
                        else
                        {
                            textBox_Fun16_DIN4.BackColor = Color.Transparent;
                        }
                        if (myStructBoardIIStatus.bDIN3 == 1)
                        {
                            textBox_Fun16_DIN3.BackColor = Color.Green;
                        }
                        else
                        {
                            textBox_Fun16_DIN3.BackColor = Color.Transparent;
                        }
                        if (myStructBoardIIStatus.bDIN2 == 1)
                        {
                            textBox_Fun16_DIN2.BackColor = Color.Green;
                        }
                        else
                        {
                            textBox_Fun16_DIN2.BackColor = Color.Transparent;
                        }
                        if (myStructBoardIIStatus.bDIN1 == 1)
                        {
                            textBox_Fun16_DIN1.BackColor = Color.Green;
                        }
                        else
                        {
                            textBox_Fun16_DIN1.BackColor = Color.Transparent;
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
                        textBox_Fun16_Temperature.Text = myStructBoardIIStatus.Temperature.ToString();

                        textBox_Fun16_BadCRCS.Text = myStructBoardIIStatus.Received_Bad_CRCs.ToString();
                        textBox_Fun16_RS232Ch2_R.Text = myStructBoardIIStatus.RS232_2_Received_Data.ToString();
                        textBox_Fun16_CAN1_R.Text = myStructBoardIIStatus.CAN_1_Received_Data.ToString();
                        textBox_Fun16_CAN2_R.Text = myStructBoardIIStatus.CAN_2_Received_Data.ToString();
                    }
                    #endregion

                    #region 8功能阀箱
                    else if (gEventArgs.addressBoard == enum_AddressBoard.ServoValvePacketBoard8Func)
                    {
                        Struct_BoardA_Status myStructBoardIStatus = (Struct_BoardA_Status)gEventArgs.objParse;

                        richTextBox_Fun8_RecvInfo.Clear();
                        richTextBox_Fun8_RecvInfo.SelectionColor = Color.Black;
                        richTextBox_Fun8_RecvInfo.Text = myStructBoardIStatus.sData;

                        for (int i = 0; i < myStructBoardIStatus.indexSubstitution.Count; i++)
                        {
                            richTextBox_Fun8_RecvInfo.Select(myStructBoardIStatus.indexSubstitution[i] * 2, 2);
                            richTextBox_Fun8_RecvInfo.SelectionColor = Color.Red;
                            richTextBox_Fun8_RecvInfo.SelectedText = "";
                        }
                        richTextBox_Fun8_RecvInfo.SelectionStart = myStructBoardIStatus.sData.Length - 6;
                        richTextBox_Fun8_RecvInfo.SelectionLength = 4;
                        //richTextBox_RecvInfo.Select(46, 4);
                        richTextBox_Fun8_RecvInfo.SelectionColor = Color.Blue;
                        richTextBox_Fun8_RecvInfo.SelectionLength = 0;
                        richTextBox_Fun8_RecvInfo.AppendText(myStructBoardIStatus.sCRCResult);


                        textBox_Fun8_BadCRCS.Text = myStructBoardIStatus.Received_Bad_CRCs.ToString();

                        if (myStructBoardIStatus.bDIN8 == 1)
                        {
                            textBox_Fun8_DIN8.BackColor = Color.Green;
                        }
                        else
                        {
                            textBox_Fun8_DIN8.BackColor = Color.Transparent;
                        }
                        if (myStructBoardIStatus.bDIN7 == 1)
                        {
                            textBox_Fun8_DIN7.BackColor = Color.Green;
                        }
                        else
                        {
                            textBox_Fun8_DIN7.BackColor = Color.Transparent;
                        }
                        if (myStructBoardIStatus.bDIN6 == 1)
                        {
                            textBox_Fun8_DIN6.BackColor = Color.Green;
                        }
                        else
                        {
                            textBox_Fun8_DIN6.BackColor = Color.Transparent;
                        }
                        if (myStructBoardIStatus.bDIN5 == 1)
                        {
                            textBox_Fun8_DIN5.BackColor = Color.Green;
                        }
                        else
                        {
                            textBox_Fun8_DIN5.BackColor = Color.Transparent;
                        }
                        if (myStructBoardIStatus.bDIN4 == 1)
                        {
                            textBox_Fun8_DIN4.BackColor = Color.Green;
                        }
                        else
                        {
                            textBox_Fun8_DIN4.BackColor = Color.Transparent;
                        }
                        if (myStructBoardIStatus.bDIN3 == 1)
                        {
                            textBox_Fun8_DIN3.BackColor = Color.Green;
                        }
                        else
                        {
                            textBox_Fun8_DIN3.BackColor = Color.Transparent;
                        }
                        if (myStructBoardIStatus.bDIN2 == 1)
                        {
                            textBox_Fun8_DIN2.BackColor = Color.Green;
                        }
                        else
                        {
                            textBox_Fun8_DIN2.BackColor = Color.Transparent;
                        }
                        if (myStructBoardIStatus.bDIN1 == 1)
                        {
                            textBox_Fun8_DIN1.BackColor = Color.Green;
                        }
                        else
                        {
                            textBox_Fun8_DIN1.BackColor = Color.Transparent;
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

                        textBox_Fun8_RS232Ch2_R.Text = myStructBoardIStatus.RS232_2_Received_Data.ToString();
                        textBox_Fun8_CAN1_R.Text = myStructBoardIStatus.CAN_1_Received_Data.ToString();
                        textBox_Fun8_CAN2_R.Text = myStructBoardIStatus.CAN_2_Received_Data.ToString();
                    }
                    #endregion

                    #region 水面控制盒
                    else if (gEventArgs.addressBoard == enum_AddressBoard.Water_Control_Box)
                    {
                        Struct_Water_Control_Box myStruct_Water_Control_Box = (Struct_Water_Control_Box)gEventArgs.objParse;
                        if (myStruct_Water_Control_Box.type == 0x81)
                        {
                            textBox_RotAxisX.Text = Math.Round(myStruct_Water_Control_Box.RotAxisX, 2).ToString();
                            textBox_RotAxisY.Text = Math.Round(myStruct_Water_Control_Box.RotAxisY, 2).ToString();
                            textBox_RotAxisZ.Text = Math.Round(myStruct_Water_Control_Box.RotAxisZ, 2).ToString();
                            textBox_RotAxisV.Text = Math.Round(myStruct_Water_Control_Box.RotAxisV, 2).ToString();
                            textBox_Space1.Text = Math.Round(myStruct_Water_Control_Box.Space1, 2).ToString();
                            textBox_Space2.Text = Math.Round(myStruct_Water_Control_Box.Space2, 2).ToString();
                            textBox_KKInfo.Text = myStruct_Water_Control_Box.KKInfo.ToString("X2");

                            paras_WaterBoxCtl_QianHou = myStruct_Water_Control_Box.RotAxisX;
                            paras_WaterBoxCtl_ZuoYou = myStruct_Water_Control_Box.RotAxisY;
                            paras_WaterBoxCtl_Rotate = myStruct_Water_Control_Box.RotAxisZ;
                            paras_WaterBoxCtl_FuQian = myStruct_Water_Control_Box.RotAxisV;

                            flag_WaterBoxCtl_AutoDir = myStruct_Water_Control_Box.IsAutoDir;
                            flag_WaterBoxCtl_AutoHigh = myStruct_Water_Control_Box.IsAutoHigh;

                            Global.flag_WaterBoxCtl_Dir_Refresh = true;
                            Global.flag_WaterBoxCtl_High_Refresh = true;
                        }
                    }
                    #endregion

                    #region 水面控制盒，20190921，新协议
                    else if (gEventArgs.addressBoard == enum_AddressBoard.Water_Control_Box_New)
                    {
                        Struct_Water_Control_Box_New myStruct_Water_Control_Box = (Struct_Water_Control_Box_New)gEventArgs.objParse;
                        if (myStruct_Water_Control_Box.type == 0x81)
                        {
                            textBox_RotAxisX.Text = Math.Round(myStruct_Water_Control_Box.A[0], 2).ToString();
                            textBox_RotAxisY.Text = Math.Round(myStruct_Water_Control_Box.A[1], 2).ToString();
                            textBox_RotAxisZ.Text = Math.Round(myStruct_Water_Control_Box.A[2], 2).ToString();
                            textBox_RotAxisV.Text = Math.Round(myStruct_Water_Control_Box.A[3], 2).ToString();
                            textBox_KKInfo.Text = myStruct_Water_Control_Box.IO[0].ToString("X2");

                            paras_WaterBoxCtl_QianHou = myStruct_Water_Control_Box.A[1];
                            paras_WaterBoxCtl_ZuoYou = myStruct_Water_Control_Box.A[0];
                            paras_WaterBoxCtl_Rotate = myStruct_Water_Control_Box.A[2];
                            paras_WaterBoxCtl_FuQian = myStruct_Water_Control_Box.A[3];


                            //自动定高>上浮下潜>前后移动>左右移动

                            //第一路模拟量：0-2.3 左移，2.7-5右移，2.3-2.7之间作为死区，不动作
                            if ((paras_WaterBoxCtl_ZuoYou < 2.3 && paras_WaterBoxCtl_ZuoYou > 0) ||
                                (paras_WaterBoxCtl_ZuoYou > 2.7 && paras_WaterBoxCtl_ZuoYou < 5))
                            {
                                flag_WaterBoxCtl_AutoHigh = false;
                                flag_WaterBoxCtl_ForwardBack = false;
                                flag_WaterBoxCtl_RightLeft = true;
                                flag_WaterBoxCtl_UpDown = false;
                            }
                            else
                            {
                                flag_WaterBoxCtl_RightLeft = false;
                            }

                            //第二路模拟量：0-2.3 前进，2.7-5后退，2.3-2.7之间作为死区，不动作
                            if ((paras_WaterBoxCtl_QianHou < 2.3 && paras_WaterBoxCtl_QianHou > 0) ||
                                (paras_WaterBoxCtl_QianHou > 2.7 && paras_WaterBoxCtl_QianHou < 5))
                            {
                                flag_WaterBoxCtl_AutoHigh = false;
                                flag_WaterBoxCtl_ForwardBack = true;
                                flag_WaterBoxCtl_RightLeft = false;
                                flag_WaterBoxCtl_UpDown = false;
                            }
                            else
                            {
                                flag_WaterBoxCtl_ForwardBack = false;
                            }

                            //第四路模拟量：0-2.3 上浮，2.7-5下潜，2.3-2.7之间作为死区，不动作
                            if ((paras_WaterBoxCtl_FuQian < 2.3 && paras_WaterBoxCtl_FuQian > 0) ||
                                (paras_WaterBoxCtl_FuQian > 2.7 && paras_WaterBoxCtl_FuQian < 5))
                            {
                                flag_WaterBoxCtl_AutoHigh = false;
                                flag_WaterBoxCtl_ForwardBack = false;
                                flag_WaterBoxCtl_RightLeft = false;
                                flag_WaterBoxCtl_UpDown = true;
                            }
                            else
                            {
                                flag_WaterBoxCtl_UpDown = false;
                            }

                            flag_WaterBoxCtl_AutoHigh = myStruct_Water_Control_Box.IsAutoHigh;



                            //IO1的第0位为1时没动作，为0时定向执行指令后，要向控制盒发送一个指令
                            //FF FFA5 23 02 09 01 00 00 00 2F 26，以点亮对应指示灯
                            //这条指令会反馈一个：FF FF A5 23 82 05 AA 26

                            if ((myStruct_Water_Control_Box.IO[0] & 0x01) == 0x00)
                            {
                                flag_WaterBoxCtl_AutoDir = true;

                            }
                            else
                            {
                                flag_WaterBoxCtl_AutoDir = false;
                            }


                            Global.flag_WaterBoxCtl_Dir_Refresh = true;
                            Global.flag_WaterBoxCtl_High_Refresh = true;
                        }
                    }
                    #endregion

                    else if (gEventArgs.addressBoard == enum_AddressBoard.Rov_Power_Box)
                    {
                        byte[] bData = (byte[])gEventArgs.obj;
                        string sData = "";
                        for (int i = 0; i < bData.Length; i++)
                        {
                            sData += bData[i].ToString("X2");
                        }
                        textBox_ROVPower.Text = sData;

                        Struct_ROVPower_CtlSystem myStruct = (Struct_ROVPower_CtlSystem)gEventArgs.objParse;
                        listAlarmInfo = gEventArgs.listAlarmInfo.ToList();

                        textBox_I_First_A.Text = Math.Round(myStruct.I_First_A, 2).ToString();
                        textBox_I_First_B.Text = Math.Round(myStruct.I_First_B, 2).ToString();
                        textBox_I_First_C.Text = Math.Round(myStruct.I_First_C, 2).ToString();
                        textBox_I_First_S.Text = Math.Round(myStruct.I_First_S, 2).ToString();
                        textBox_V_First_AB.Text = Math.Round(myStruct.V_First_AB, 2).ToString();
                        textBox_V_First_BC.Text = Math.Round(myStruct.V_First_BC, 2).ToString();
                        textBox_I_Next_ABC.Text = Math.Round(myStruct.I_Next_ABC, 2).ToString();
                        textBox_I_Next_S.Text = Math.Round(myStruct.I_Next_S, 2).ToString();
                        textBox_V_Next_ABC.Text = Math.Round(myStruct.V_Next_ABC, 2).ToString();
                        textBox_V_Next_S.Text = Math.Round(myStruct.V_Next_S, 2).ToString();

                        #region 报警信息判断

                        int alarmres = 0;
                        string alarmstr = "";
                        int bianHao = 0;

                        if (listStatusParasKeys.Exists((string x) => x == "ROVPower_I_First_A" ? true : false))
                        {
                            alarmstr = GetAlarmResult(myStruct.I_First_A, dicStatusParasByStatusNameLocal["ROVPower_I_First_A"], ref alarmres);
                            bianHao = dicStatusParasByStatusNameLocal["ROVPower_I_First_A"].bianHao;
                            if (alarmres > 0)
                            {
                                alarmInfoUnit aiu = new alarmInfoUnit();
                                aiu.index = bianHao;
                                aiu.alarm = alarmres;
                                aiu.info = alarmstr;
                                listAlarmInfo.Add(aiu);
                                if (alarmres == 1)
                                {
                                    textBox_I_First_A.BackColor = Color.Yellow;
                                }
                                else if (alarmres == 2)
                                {
                                    textBox_I_First_A.BackColor = Color.Red;
                                }
                            }
                            else
                            {
                                textBox_I_First_A.BackColor = Color.Transparent;
                            }
                        }

                        if (listStatusParasKeys.Exists((string x) => x == "ROVPower_I_First_B" ? true : false))
                        {
                            alarmstr = GetAlarmResult(myStruct.I_First_B, dicStatusParasByStatusNameLocal["ROVPower_I_First_B"], ref alarmres);
                            bianHao = dicStatusParasByStatusNameLocal["ROVPower_I_First_B"].bianHao;
                            if (alarmres > 0)
                            {
                                alarmInfoUnit aiu = new alarmInfoUnit();
                                aiu.index = bianHao;
                                aiu.alarm = alarmres;
                                aiu.info = alarmstr;
                                listAlarmInfo.Add(aiu);
                                if (alarmres == 1)
                                {
                                    textBox_I_First_B.BackColor = Color.Yellow;
                                }
                                else if (alarmres == 2)
                                {
                                    textBox_I_First_B.BackColor = Color.Red;
                                }
                            }
                            else
                            {
                                textBox_I_First_B.BackColor = Color.Transparent;
                            }
                        }

                        if (listStatusParasKeys.Exists((string x) => x == "ROVPower_I_First_C" ? true : false))
                        {
                            alarmstr = GetAlarmResult(myStruct.I_First_C, dicStatusParasByStatusNameLocal["ROVPower_I_First_C"], ref alarmres);
                            bianHao = dicStatusParasByStatusNameLocal["ROVPower_I_First_C"].bianHao;
                            if (alarmres > 0)
                            {
                                alarmInfoUnit aiu = new alarmInfoUnit();
                                aiu.index = bianHao;
                                aiu.alarm = alarmres;
                                aiu.info = alarmstr;
                                listAlarmInfo.Add(aiu);
                                if (alarmres == 1)
                                {
                                    textBox_I_First_C.BackColor = Color.Yellow;
                                }
                                else if (alarmres == 2)
                                {
                                    textBox_I_First_C.BackColor = Color.Red;
                                }
                            }
                            else
                            {
                                textBox_I_First_C.BackColor = Color.Transparent;
                            }
                        }

                        if (listStatusParasKeys.Exists((string x) => x == "ROVPower_V_First_AB" ? true : false))
                        {
                            alarmstr = GetAlarmResult(myStruct.V_First_AB, dicStatusParasByStatusNameLocal["ROVPower_V_First_AB"], ref alarmres);
                            bianHao = dicStatusParasByStatusNameLocal["ROVPower_V_First_AB"].bianHao;
                            if (alarmres > 0)
                            {
                                alarmInfoUnit aiu = new alarmInfoUnit();
                                aiu.index = bianHao;
                                aiu.alarm = alarmres;
                                aiu.info = alarmstr;
                                listAlarmInfo.Add(aiu);
                                if (alarmres == 1)
                                {
                                    textBox_V_First_AB.BackColor = Color.Yellow;
                                }
                                else if (alarmres == 2)
                                {
                                    textBox_V_First_AB.BackColor = Color.Red;
                                }
                            }
                            else
                            {
                                textBox_V_First_AB.BackColor = Color.Transparent;
                            }
                        }

                        if (listStatusParasKeys.Exists((string x) => x == "ROVPower_V_First_BC" ? true : false))
                        {
                            alarmstr = GetAlarmResult(myStruct.V_First_BC, dicStatusParasByStatusNameLocal["ROVPower_V_First_BC"], ref alarmres);
                            bianHao = dicStatusParasByStatusNameLocal["ROVPower_V_First_BC"].bianHao;
                            if (alarmres > 0)
                            {
                                alarmInfoUnit aiu = new alarmInfoUnit();
                                aiu.index = bianHao;
                                aiu.alarm = alarmres;
                                aiu.info = alarmstr;
                                listAlarmInfo.Add(aiu);
                                if (alarmres == 1)
                                {
                                    textBox_V_First_BC.BackColor = Color.Yellow;
                                }
                                else if (alarmres == 2)
                                {
                                    textBox_V_First_BC.BackColor = Color.Red;
                                }
                            }
                            else
                            {
                                textBox_V_First_BC.BackColor = Color.Transparent;
                            }
                        }

                        if (listStatusParasKeys.Exists((string x) => x == "ROVPower_I_First_S" ? true : false))
                        {
                            alarmstr = GetAlarmResult(myStruct.I_First_S, dicStatusParasByStatusNameLocal["ROVPower_I_First_S"], ref alarmres);
                            bianHao = dicStatusParasByStatusNameLocal["ROVPower_I_First_S"].bianHao;
                            if (alarmres > 0)
                            {
                                alarmInfoUnit aiu = new alarmInfoUnit();
                                aiu.index = bianHao;
                                aiu.alarm = alarmres;
                                aiu.info = alarmstr;
                                listAlarmInfo.Add(aiu);
                                if (alarmres == 1)
                                {
                                    textBox_I_First_S.BackColor = Color.Yellow;
                                }
                                else if (alarmres == 2)
                                {
                                    textBox_I_First_S.BackColor = Color.Red;
                                }
                            }
                            else
                            {
                                textBox_I_First_S.BackColor = Color.Transparent;
                            }
                        }

                        if (listStatusParasKeys.Exists((string x) => x == "ROVPower_I_Next_ABC" ? true : false))
                        {
                            alarmstr = GetAlarmResult(myStruct.I_Next_ABC, dicStatusParasByStatusNameLocal["ROVPower_I_Next_ABC"], ref alarmres);
                            bianHao = dicStatusParasByStatusNameLocal["ROVPower_I_Next_ABC"].bianHao;
                            if (alarmres > 0)
                            {
                                alarmInfoUnit aiu = new alarmInfoUnit();
                                aiu.index = bianHao;
                                aiu.alarm = alarmres;
                                aiu.info = alarmstr;
                                listAlarmInfo.Add(aiu);
                                if (alarmres == 1)
                                {
                                    textBox_I_Next_ABC.BackColor = Color.Yellow;
                                }
                                else if (alarmres == 2)
                                {
                                    textBox_I_Next_ABC.BackColor = Color.Red;
                                }
                            }
                            else
                            {
                                textBox_I_Next_ABC.BackColor = Color.Transparent;
                            }
                        }

                        if (listStatusParasKeys.Exists((string x) => x == "ROVPower_I_Next_S" ? true : false))
                        {
                            alarmstr = GetAlarmResult(myStruct.I_Next_S, dicStatusParasByStatusNameLocal["ROVPower_I_Next_S"], ref alarmres);
                            bianHao = dicStatusParasByStatusNameLocal["ROVPower_I_Next_S"].bianHao;
                            if (alarmres > 0)
                            {
                                alarmInfoUnit aiu = new alarmInfoUnit();
                                aiu.index = bianHao;
                                aiu.alarm = alarmres;
                                aiu.info = alarmstr;
                                listAlarmInfo.Add(aiu);
                                if (alarmres == 1)
                                {
                                    textBox_I_Next_S.BackColor = Color.Yellow;
                                }
                                else if (alarmres == 2)
                                {
                                    textBox_I_Next_S.BackColor = Color.Red;
                                }
                            }
                            else
                            {
                                textBox_I_Next_S.BackColor = Color.Transparent;
                            }
                        }

                        if (listStatusParasKeys.Exists((string x) => x == "ROVPower_V_Next_ABC" ? true : false))
                        {
                            alarmstr = GetAlarmResult(myStruct.V_Next_ABC, dicStatusParasByStatusNameLocal["ROVPower_V_Next_ABC"], ref alarmres);
                            bianHao = dicStatusParasByStatusNameLocal["ROVPower_V_Next_ABC"].bianHao;
                            if (alarmres > 0)
                            {
                                alarmInfoUnit aiu = new alarmInfoUnit();
                                aiu.index = bianHao;
                                aiu.alarm = alarmres;
                                aiu.info = alarmstr;
                                listAlarmInfo.Add(aiu);
                                if (alarmres == 1)
                                {
                                    textBox_V_Next_ABC.BackColor = Color.Yellow;
                                }
                                else if (alarmres == 2)
                                {
                                    textBox_V_Next_ABC.BackColor = Color.Red;
                                }
                            }
                            else
                            {
                                textBox_V_Next_ABC.BackColor = Color.Transparent;
                            }
                        }

                        if (listStatusParasKeys.Exists((string x) => x == "ROVPower_V_Next_S" ? true : false))
                        {
                            alarmstr = GetAlarmResult(myStruct.V_Next_S, dicStatusParasByStatusNameLocal["ROVPower_V_Next_S"], ref alarmres);
                            bianHao = dicStatusParasByStatusNameLocal["ROVPower_V_Next_S"].bianHao;
                            if (alarmres > 0)
                            {
                                alarmInfoUnit aiu = new alarmInfoUnit();
                                aiu.index = bianHao;
                                aiu.alarm = alarmres;
                                aiu.info = alarmstr;
                                listAlarmInfo.Add(aiu);
                                if (alarmres == 1)
                                {
                                    textBox_V_Next_S.BackColor = Color.Yellow;
                                }
                                else if (alarmres == 2)
                                {
                                    textBox_V_Next_S.BackColor = Color.Red;
                                }
                            }
                            else
                            {
                                textBox_V_Next_S.BackColor = Color.Transparent;
                            }
                        }

                        #endregion
                    }
                    else if (gEventArgs.addressBoard == enum_AddressBoard.JueYuanJianCe_Board_1)
                    {
                        Struct_JueYuanJianCeYi myStruct =
                            (Struct_JueYuanJianCeYi)gEventArgs.objParse;

                        textBox_JueYuanJianCe_1_MeauringValue.Text = myStruct.sMeauringValue;
                        textBox_JueYuanJianCe_1_Alarm1Value.Text = myStruct.sAlarm1Value;
                        textBox_JueYuanJianCe_1_Alarm2Value.Text = myStruct.sAlarm2Value;
                        textBox_JueYuanJianCe_1_K1K2.Text = myStruct.sK1_K2_OnOff;
                        textBox_JueYuanJianCe_1_Alarm12.Text = myStruct.sAlarm1_2_None;
                        textBox_JueYuanJianCe_1_ACDC.Text = myStruct.sAC_DC_Fault;


                        #region 报警信息判断

                        int alarmres = 0;
                        string alarmstr = "";
                        int bianHao = 0;

                        #endregion
                    }
                    else if (gEventArgs.addressBoard == enum_AddressBoard.JueYuanJianCe_Board_2)
                    {
                        Struct_JueYuanJianCeYi myStruct =
                            (Struct_JueYuanJianCeYi)gEventArgs.objParse;

                        textBox_JueYuanJianCe_2_MeauringValue.Text = myStruct.sMeauringValue;
                        textBox_JueYuanJianCe_2_Alarm1Value.Text = myStruct.sAlarm1Value;
                        textBox_JueYuanJianCe_2_Alarm2Value.Text = myStruct.sAlarm2Value;
                        textBox_JueYuanJianCe_2_K1K2.Text = myStruct.sK1_K2_OnOff;
                        textBox_JueYuanJianCe_2_Alarm12.Text = myStruct.sAlarm1_2_None;
                        textBox_JueYuanJianCe_2_ACDC.Text = myStruct.sAC_DC_Fault;

                        #region 报警信息判断

                        int alarmres = 0;
                        string alarmstr = "";
                        int bianHao = 0;

                        #endregion
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

        #endregion

        #region 串口发送控制状态显示
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

        #endregion


        #region threadCmdSendFunc,执行逻辑线程
        private Thread threadServoValvePackOper;
        public bool stopThread = false;
        private bool flagCmdToSend = false;
        private int SleepSerialValvePackOperExeDelayRecv = 500;
        private void threadServoValvePackOperFunc()
        {
            try
            {
                while (stopThread == false)
                {
                    try
                    {
                        if (flagCmdToSend == false)
                        {
                            Thread.Sleep(5);
                            continue;
                        }

                        /*
                         * 16阀箱
                         * 1A-PWM1、1B-PWM2、2A-PWM3、2B-PWM4、......8A-PWM15、8B-PWM16
                         * 9A-DOUT1、9B-DOUT2、......16A-DOUT15、16B-DOUT16
                         * 
                         * ?A:即(?×2 - 1)
                         * ?B:即(?×2)
                         */

                        #region 钻进
                        if (myEnum_UserButtonOperIdentify == Enum_UserButtonOperIdentify.ZuanJin)
                        {
                            try
                            {
                                bSerialValvePackOperOK = false;
                                this.BeginInvoke(new Func<object>(() =>
                                {
                                    Global.m_FormBoardII.btn_PWM7.ImageIndex = 0;//4A关
                                    Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_PWM7, null);
                                    Global.m_FormBoardII.btn_PWM8.ImageIndex = 1;//4B开
                                    Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_PWM8, null);
                                    Global.m_FormBoardII.btn_Perform_Click(Global.m_FormBoardII.btn_SetDataIntoPCB, null);

                                    //btn_ZuanJin.BackColor = Color.Green;
                                    //btn_ZuanJin_Step.BackColor = Color.Transparent;
                                    //btn_TiZuan.BackColor = Color.Transparent;
                                    //btn_TiZuan_Step.BackColor = Color.Transparent;

                                    return null;
                                }));

                                //Thread.Sleep(SleepSerialValvePackOperExeDelayRecv);

                            }
                            catch (Exception ex)
                            {
                            }
                        }
                        #endregion

                        #region 点动钻进
                        else if (myEnum_UserButtonOperIdentify == Enum_UserButtonOperIdentify.ZuanJin_Step)
                        {
                            try
                            {
                                bSerialValvePackOperOK = false;
                                this.BeginInvoke(new Func<object>(() =>
                                {
                                    Global.m_FormBoardII.btn_PWM7.ImageIndex = 0;//4A关
                                    Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_PWM7, null);
                                    Global.m_FormBoardII.btn_PWM8.ImageIndex = 1;//4B开
                                    Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_PWM8, null);
                                    Global.m_FormBoardII.btn_Perform_Click(Global.m_FormBoardII.btn_SetDataIntoPCB, null);

                                    //btn_ZuanJin.BackColor = Color.Transparent;
                                    //btn_ZuanJin_Step.BackColor = Color.Green;
                                    //btn_TiZuan.BackColor = Color.Transparent;
                                    //btn_TiZuan_Step.BackColor = Color.Transparent;

                                    return null;
                                }));

                                //Thread.Sleep(SleepSerialValvePackOperExeDelayRecv);

                                Thread.Sleep((int)Global.dDrillDown);

                                this.BeginInvoke(new Func<object>(() =>
                                {
                                    Global.m_FormBoardII.btn_PWM7.ImageIndex = 0;//4A关
                                    Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_PWM7, null);
                                    Global.m_FormBoardII.btn_PWM8.ImageIndex = 0;//4B关
                                    Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_PWM8, null);
                                    Global.m_FormBoardII.btn_Perform_Click(Global.m_FormBoardII.btn_SetDataIntoPCB, null);

                                    //btn_ZuanJin.BackColor = Color.Transparent;
                                    //btn_ZuanJin_Step.BackColor = Color.Transparent;
                                    //btn_TiZuan.BackColor = Color.Transparent;
                                    //btn_TiZuan_Step.BackColor = Color.Transparent;

                                    return null;
                                }));
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                        #endregion

                        #region 提钻
                        else if (myEnum_UserButtonOperIdentify == Enum_UserButtonOperIdentify.TiZuan)
                        {
                            try
                            {
                                bSerialValvePackOperOK = false;
                                this.BeginInvoke(new Func<object>(() =>
                                {
                                    Global.m_FormBoardII.btn_PWM8.ImageIndex = 0;//4B关
                                    Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_PWM8, null);
                                    Global.m_FormBoardII.btn_PWM7.ImageIndex = 1;//4A开
                                    Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_PWM7, null);
                                    Global.m_FormBoardII.btn_Perform_Click(Global.m_FormBoardII.btn_SetDataIntoPCB, null);

                                    //btn_ZuanJin.BackColor = Color.Transparent;
                                    //btn_ZuanJin_Step.BackColor = Color.Transparent;
                                    //btn_TiZuan.BackColor = Color.Green;
                                    //btn_TiZuan_Step.BackColor = Color.Transparent;

                                    return null;
                                }));

                                //Thread.Sleep(SleepSerialValvePackOperExeDelayRecv);

                            }
                            catch (Exception ex)
                            {
                            }
                        }
                        #endregion

                        #region 点动提钻
                        else if (myEnum_UserButtonOperIdentify == Enum_UserButtonOperIdentify.TiZuan_Step)
                        {
                            try
                            {
                                bSerialValvePackOperOK = false;
                                this.BeginInvoke(new Func<object>(() =>
                                {
                                    Global.m_FormBoardII.btn_PWM8.ImageIndex = 0;//4B关
                                    Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_PWM8, null);
                                    Global.m_FormBoardII.btn_PWM7.ImageIndex = 1;//4A开
                                    Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_PWM7, null);
                                    Global.m_FormBoardII.btn_Perform_Click(Global.m_FormBoardII.btn_SetDataIntoPCB, null);

                                    //btn_ZuanJin.BackColor = Color.Transparent;
                                    //btn_ZuanJin_Step.BackColor = Color.Transparent;
                                    //btn_TiZuan.BackColor = Color.Transparent;
                                    //btn_TiZuan_Step.BackColor = Color.Green;

                                    return null;
                                }));

                                //Thread.Sleep(SleepSerialValvePackOperExeDelayRecv);

                                Thread.Sleep((int)Global.dDrillUp);

                                this.BeginInvoke(new Func<object>(() =>
                                {
                                    Global.m_FormBoardII.btn_PWM7.ImageIndex = 0;//4A关
                                    Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_PWM7, null);
                                    Global.m_FormBoardII.btn_PWM8.ImageIndex = 0;//4B关
                                    Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_PWM8, null);
                                    Global.m_FormBoardII.btn_Perform_Click(Global.m_FormBoardII.btn_SetDataIntoPCB, null);

                                    //btn_ZuanJin.BackColor = Color.Transparent;
                                    //btn_ZuanJin_Step.BackColor = Color.Transparent;
                                    //btn_TiZuan.BackColor = Color.Transparent;
                                    //btn_TiZuan_Step.BackColor = Color.Transparent;

                                    return null;
                                }));
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                        #endregion

                        #region 钻进、提钻均停止
                        else if (myEnum_UserButtonOperIdentify == Enum_UserButtonOperIdentify.ZuanJin_TiZuan_Stop)
                        {
                            try
                            {
                                bSerialValvePackOperOK = false;
                                this.BeginInvoke(new Func<object>(() =>
                                {
                                    Global.m_FormBoardII.btn_PWM7.ImageIndex = 0;//4A关
                                    Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_PWM7, null);
                                    Global.m_FormBoardII.btn_PWM8.ImageIndex = 0;//4B关
                                    Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_PWM8, null);
                                    Global.m_FormBoardII.btn_Perform_Click(Global.m_FormBoardII.btn_SetDataIntoPCB, null);

                                    //btn_ZuanJin.BackColor = Color.Transparent;
                                    //btn_ZuanJin_Step.BackColor = Color.Transparent;
                                    //btn_TiZuan.BackColor = Color.Transparent;
                                    //btn_TiZuan_Step.BackColor = Color.Transparent;

                                    return null;
                                }));

                                //Thread.Sleep(SleepSerialValvePackOperExeDelayRecv);

                            }
                            catch (Exception ex)
                            {
                            }
                        }
                        #endregion



                        #region 钻进阀正转
                        else if (myEnum_UserButtonOperIdentify == Enum_UserButtonOperIdentify.ZuanJinFa_ZhengZhuan)
                        {
                            try
                            {
                                bSerialValvePackOperOK = false;
                                this.BeginInvoke(new Func<object>(() =>
                                {
                                    Global.m_FormBoardII.btn_PWM6.ImageIndex = 0;//3B关
                                    Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_PWM6, null);
                                    Global.m_FormBoardII.btn_PWM5.ImageIndex = 1;//3A开
                                    Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_PWM5, null);
                                    Global.m_FormBoardII.btn_Perform_Click(Global.m_FormBoardII.btn_SetDataIntoPCB, null);

                                    //btn_ZuanJin_ZhengZhuan.BackColor = Color.Green;
                                    //btn_ZuanJin_FanZhuan.BackColor = Color.Transparent;

                                    return null;
                                }));

                                //Thread.Sleep(SleepSerialValvePackOperExeDelayRecv);

                            }
                            catch (Exception ex)
                            {
                            }
                        }
                        #endregion

                        #region 钻进阀反转
                        else if (myEnum_UserButtonOperIdentify == Enum_UserButtonOperIdentify.ZuanJinFa_FanZhuan)
                        {
                            try
                            {
                                bSerialValvePackOperOK = false;
                                this.BeginInvoke(new Func<object>(() =>
                                {
                                    Global.m_FormBoardII.btn_PWM5.ImageIndex = 0;//3A关
                                    Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_PWM5, null);
                                    Global.m_FormBoardII.btn_PWM6.ImageIndex = 1;//3B开
                                    Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_PWM6, null);
                                    Global.m_FormBoardII.btn_Perform_Click(Global.m_FormBoardII.btn_SetDataIntoPCB, null);

                                    //btn_ZuanJin_ZhengZhuan.BackColor = Color.Transparent;
                                    //btn_ZuanJin_FanZhuan.BackColor = Color.Green;

                                    return null;
                                }));

                                //Thread.Sleep(SleepSerialValvePackOperExeDelayRecv);

                            }
                            catch (Exception ex)
                            {
                            }
                        }
                        #endregion

                        #region 钻进阀正转、反转均停止
                        else if (myEnum_UserButtonOperIdentify == Enum_UserButtonOperIdentify.ZuanJinFa_ZhengZhuan_FanZhuan_Stop)
                        {
                            try
                            {
                                bSerialValvePackOperOK = false;
                                this.BeginInvoke(new Func<object>(() =>
                                {
                                    Global.m_FormBoardII.btn_PWM5.ImageIndex = 0;//3A关
                                    Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_PWM5, null);
                                    Global.m_FormBoardII.btn_PWM6.ImageIndex = 0;//3B关
                                    Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_PWM6, null);
                                    Global.m_FormBoardII.btn_Perform_Click(Global.m_FormBoardII.btn_SetDataIntoPCB, null);

                                    //btn_ZuanJin_ZhengZhuan.BackColor = Color.Transparent;
                                    //btn_ZuanJin_FanZhuan.BackColor = Color.Transparent;

                                    return null;
                                }));

                                //Thread.Sleep(SleepSerialValvePackOperExeDelayRecv);

                            }
                            catch (Exception ex)
                            {
                            }
                        }
                        #endregion



                        #region 云台顺转
                        else if (myEnum_UserButtonOperIdentify == Enum_UserButtonOperIdentify.YunTai_ShunZhuan)
                        {
                            try
                            {
                                bSerialValvePackOperOK = false;
                                this.BeginInvoke(new Func<object>(() =>
                                {
                                    Global.m_FormBoardII.btn_PWM10.ImageIndex = 0;//5B关
                                    Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_PWM10, null);
                                    Global.m_FormBoardII.btn_PWM9.ImageIndex = 1;//5A开
                                    Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_PWM9, null);
                                    Global.m_FormBoardII.btn_Perform_Click(Global.m_FormBoardII.btn_SetDataIntoPCB, null);

                                    //btn_YunTai_ShunZhuan.BackColor = Color.Green;
                                    //btn_YunTai_NiZhuan.BackColor = Color.Transparent;

                                    return null;
                                }));

                                Thread.Sleep((int)Global.dPitchForward - 200);

                                this.BeginInvoke(new Func<object>(() =>
                                {
                                    Global.m_FormBoardII.btn_PWM9.ImageIndex = 0;//5A关
                                    Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_PWM9, null);
                                    Global.m_FormBoardII.btn_PWM10.ImageIndex = 0;//5B关
                                    Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_PWM10, null);
                                    Global.m_FormBoardII.btn_Perform_Click(Global.m_FormBoardII.btn_SetDataIntoPCB, null);

                                    //btn_YunTai_ShunZhuan.BackColor = Color.Transparent;
                                    //btn_YunTai_NiZhuan.BackColor = Color.Transparent;

                                    return null;
                                }));
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                        #endregion

                        #region 云台逆转
                        else if (myEnum_UserButtonOperIdentify == Enum_UserButtonOperIdentify.YunTai_NiZhuan)
                        {
                            try
                            {
                                bSerialValvePackOperOK = false;
                                this.BeginInvoke(new Func<object>(() =>
                                {
                                    Global.m_FormBoardII.btn_PWM9.ImageIndex = 0;//5A关
                                    Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_PWM9, null);
                                    Global.m_FormBoardII.btn_PWM10.ImageIndex = 1;//5B开
                                    Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_PWM10, null);
                                    Global.m_FormBoardII.btn_Perform_Click(Global.m_FormBoardII.btn_SetDataIntoPCB, null);

                                    //btn_YunTai_ShunZhuan.BackColor = Color.Transparent;
                                    //btn_YunTai_NiZhuan.BackColor = Color.Green;

                                    return null;
                                }));

                                Thread.Sleep((int)Global.dPitchReverse - 200);

                                this.BeginInvoke(new Func<object>(() =>
                                {
                                    Global.m_FormBoardII.btn_PWM9.ImageIndex = 0;//5A关
                                    Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_PWM9, null);
                                    Global.m_FormBoardII.btn_PWM10.ImageIndex = 0;//5B关
                                    Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_PWM10, null);
                                    Global.m_FormBoardII.btn_Perform_Click(Global.m_FormBoardII.btn_SetDataIntoPCB, null);

                                    //btn_YunTai_ShunZhuan.BackColor = Color.Transparent;
                                    //btn_YunTai_NiZhuan.BackColor = Color.Transparent;

                                    return null;
                                }));
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                        #endregion

                        #region 云台顺转、逆转均停止
                        else if (myEnum_UserButtonOperIdentify == Enum_UserButtonOperIdentify.YunTai_ShunZhuan_NiZhuan_Stop)
                        {
                            try
                            {
                                bSerialValvePackOperOK = false;
                                this.BeginInvoke(new Func<object>(() =>
                                {
                                    Global.m_FormBoardII.btn_PWM9.ImageIndex = 0;//5A关
                                    Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_PWM9, null);
                                    Global.m_FormBoardII.btn_PWM10.ImageIndex = 0;//5B关
                                    Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_PWM10, null);
                                    Global.m_FormBoardII.btn_Perform_Click(Global.m_FormBoardII.btn_SetDataIntoPCB, null);

                                    //btn_YunTai_ShunZhuan.BackColor = Color.Transparent;
                                    //btn_YunTai_NiZhuan.BackColor = Color.Transparent;

                                    return null;
                                }));

                                //Thread.Sleep(SleepSerialValvePackOperExeDelayRecv);

                            }
                            catch (Exception ex)
                            {
                            }
                        }
                        #endregion



                        #region 云台俯
                        else if (myEnum_UserButtonOperIdentify == Enum_UserButtonOperIdentify.YunTai_Fu)
                        {
                            try
                            {
                                bSerialValvePackOperOK = false;
                                this.BeginInvoke(new Func<object>(() =>
                                {
                                    Global.m_FormBoardII.btn_PWM12.ImageIndex = 0;//6B关
                                    Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_PWM12, null);
                                    Global.m_FormBoardII.btn_PWM11.ImageIndex = 1;//6A开
                                    Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_PWM11, null);
                                    Global.m_FormBoardII.btn_Perform_Click(Global.m_FormBoardII.btn_SetDataIntoPCB, null);

                                    //btn_YunTai_Fu.BackColor = Color.Green;
                                    //btn_YunTai_Yang.BackColor = Color.Transparent;

                                    return null;
                                }));

                                Thread.Sleep((int)Global.dPitchDown - 200);

                                this.BeginInvoke(new Func<object>(() =>
                                {
                                    Global.m_FormBoardII.btn_PWM11.ImageIndex = 0;//6A关
                                    Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_PWM11, null);
                                    Global.m_FormBoardII.btn_PWM12.ImageIndex = 0;//6B关
                                    Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_PWM12, null);
                                    Global.m_FormBoardII.btn_Perform_Click(Global.m_FormBoardII.btn_SetDataIntoPCB, null);

                                    //btn_YunTai_Fu.BackColor = Color.Transparent;
                                    //btn_YunTai_Yang.BackColor = Color.Transparent;

                                    return null;
                                }));
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                        #endregion

                        #region 云台仰
                        else if (myEnum_UserButtonOperIdentify == Enum_UserButtonOperIdentify.YunTai_Yang)
                        {
                            try
                            {
                                bSerialValvePackOperOK = false;
                                this.BeginInvoke(new Func<object>(() =>
                                {
                                    Global.m_FormBoardII.btn_PWM11.ImageIndex = 0;//6A关
                                    Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_PWM11, null);
                                    Global.m_FormBoardII.btn_PWM12.ImageIndex = 1;//6B开
                                    Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_PWM12, null);
                                    Global.m_FormBoardII.btn_Perform_Click(Global.m_FormBoardII.btn_SetDataIntoPCB, null);

                                    //btn_YunTai_Fu.BackColor = Color.Transparent;
                                    //btn_YunTai_Yang.BackColor = Color.Green;

                                    return null;
                                }));

                                Thread.Sleep((int)Global.dPitchUp - 200);

                                this.BeginInvoke(new Func<object>(() =>
                                {
                                    Global.m_FormBoardII.btn_PWM11.ImageIndex = 0;//6A关
                                    Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_PWM11, null);
                                    Global.m_FormBoardII.btn_PWM12.ImageIndex = 0;//6B关
                                    Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_PWM12, null);
                                    Global.m_FormBoardII.btn_Perform_Click(Global.m_FormBoardII.btn_SetDataIntoPCB, null);

                                    //btn_YunTai_Fu.BackColor = Color.Transparent;
                                    //btn_YunTai_Yang.BackColor = Color.Transparent;

                                    return null;
                                }));
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                        #endregion

                        #region 云台俯仰均停止
                        else if (myEnum_UserButtonOperIdentify == Enum_UserButtonOperIdentify.YunTai_FuYang_Stop)
                        {
                            try
                            {
                                bSerialValvePackOperOK = false;
                                this.BeginInvoke(new Func<object>(() =>
                                {
                                    Global.m_FormBoardII.btn_PWM11.ImageIndex = 0;//6A关
                                    Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_PWM11, null);
                                    Global.m_FormBoardII.btn_PWM12.ImageIndex = 0;//6B关
                                    Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_PWM12, null);
                                    Global.m_FormBoardII.btn_Perform_Click(Global.m_FormBoardII.btn_SetDataIntoPCB, null);

                                    //btn_YunTai_Fu.BackColor = Color.Transparent;
                                    //btn_YunTai_Yang.BackColor = Color.Transparent;

                                    return null;
                                }));

                                //Thread.Sleep(SleepSerialValvePackOperExeDelayRecv);

                            }
                            catch (Exception ex)
                            {
                            }
                        }
                        #endregion



                        #region 油源高压1
                        else if (myEnum_UserButtonOperIdentify == Enum_UserButtonOperIdentify.YouYuanGaoYa_1)
                        {
                            try
                            {
                                bSerialValvePackOperOK = false;
                                this.BeginInvoke(new Func<object>(() =>
                                {
                                    Global.m_FormBoardII.btn_PWM1.ImageIndex = 0;//1A关
                                    Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_PWM1, null);
                                    Global.m_FormBoardII.btn_PWM2.ImageIndex = 1;//1B开
                                    Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_PWM2, null);
                                    Global.m_FormBoardII.btn_Perform_Click(Global.m_FormBoardII.btn_SetDataIntoPCB, null);

                                    //btn_YouYuanGaoYa_1.BackColor = Color.Green;

                                    return null;
                                }));

                                //Thread.Sleep(SleepSerialValvePackOperExeDelayRecv);

                            }
                            catch (Exception ex)
                            {
                            }
                        }
                        #endregion

                        #region 油源高压停止1
                        else if (myEnum_UserButtonOperIdentify == Enum_UserButtonOperIdentify.YouYuanGaoYa_Stop_1)
                        {
                            try
                            {
                                bSerialValvePackOperOK = false;

                                //油源高压对面阀上电2s后断电
                                this.BeginInvoke(new Func<object>(() =>
                                {
                                    Global.m_FormBoardII.btn_PWM1.ImageIndex = 1;//1A开
                                    Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_PWM1, null);
                                    Global.m_FormBoardII.btn_PWM2.ImageIndex = 0;//1B关
                                    Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_PWM2, null);
                                    Global.m_FormBoardII.btn_Perform_Click(Global.m_FormBoardII.btn_SetDataIntoPCB, null);

                                    return null;
                                }));

                                Thread.Sleep(2000);

                                this.BeginInvoke(new Func<object>(() =>
                                {
                                    Global.m_FormBoardII.btn_PWM1.ImageIndex = 0;//1A关
                                    Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_PWM1, null);
                                    Global.m_FormBoardII.btn_PWM2.ImageIndex = 0;//1B关
                                    Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_PWM2, null);
                                    Global.m_FormBoardII.btn_Perform_Click(Global.m_FormBoardII.btn_SetDataIntoPCB, null);

                                    //btn_YouYuanGaoYa_1.BackColor = Color.Transparent;


                                    return null;
                                }));

                                //Thread.Sleep(SleepSerialValvePackOperExeDelayRecv);

                            }
                            catch (Exception ex)
                            {
                            }
                        }
                        #endregion

                        #region 油源高压2
                        else if (myEnum_UserButtonOperIdentify == Enum_UserButtonOperIdentify.YouYuanGaoYa)
                        {
                            try
                            {
                                bSerialValvePackOperOK = false;
                                this.BeginInvoke(new Func<object>(() =>
                                {
                                    Global.m_FormBoardII.btn_PWM3.ImageIndex = 1;//2A开
                                    Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_PWM3, null);
                                    Global.m_FormBoardII.btn_PWM4.ImageIndex = 0;//2B关
                                    Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_PWM4, null);
                                    Global.m_FormBoardII.btn_Perform_Click(Global.m_FormBoardII.btn_SetDataIntoPCB, null);

                                    // btn_YouYuanGaoYa.BackColor = Color.Green;

                                    return null;
                                }));

                                //Thread.Sleep(SleepSerialValvePackOperExeDelayRecv);

                            }
                            catch (Exception ex)
                            {
                            }
                        }
                        #endregion

                        #region 油源高压停止2
                        else if (myEnum_UserButtonOperIdentify == Enum_UserButtonOperIdentify.YouYuanGaoYa_Stop)
                        {
                            try
                            {
                                bSerialValvePackOperOK = false;

                                //油源高压对面阀上电2s后断电
                                this.BeginInvoke(new Func<object>(() =>
                                {
                                    Global.m_FormBoardII.btn_PWM3.ImageIndex = 0;//2A关
                                    Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_PWM3, null);
                                    Global.m_FormBoardII.btn_PWM4.ImageIndex = 1;//2B开
                                    Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_PWM4, null);
                                    Global.m_FormBoardII.btn_Perform_Click(Global.m_FormBoardII.btn_SetDataIntoPCB, null);

                                    return null;
                                }));

                                Thread.Sleep(2000);

                                this.BeginInvoke(new Func<object>(() =>
                                {
                                    Global.m_FormBoardII.btn_PWM3.ImageIndex = 0;//2A关
                                    Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_PWM3, null);
                                    Global.m_FormBoardII.btn_PWM4.ImageIndex = 0;//2B关
                                    Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_PWM4, null);
                                    Global.m_FormBoardII.btn_Perform_Click(Global.m_FormBoardII.btn_SetDataIntoPCB, null);

                                    // btn_YouYuanGaoYa.BackColor = Color.Transparent;


                                    return null;
                                }));

                                //Thread.Sleep(SleepSerialValvePackOperExeDelayRecv);

                            }
                            catch (Exception ex)
                            {
                            }
                        }
                        #endregion



                        #region 水泵开
                        else if (myEnum_UserButtonOperIdentify == Enum_UserButtonOperIdentify.ShuiBengKai)
                        {
                            try
                            {
                                bSerialValvePackOperOK = false;
                                this.BeginInvoke(new Func<object>(() =>
                                {
                                    Global.m_FormBoardII.btn_DOUT15.ImageIndex = 0;//16A关
                                    Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT15, null);
                                    Global.m_FormBoardII.btn_DOUT16.ImageIndex = 1;//16B开
                                    Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT16, null);
                                    Global.m_FormBoardII.btn_Perform_Click(Global.m_FormBoardII.btn_SetDataIntoPCB, null);

                                    //btn_ShuiBengKai.BackColor = Color.Green;

                                    return null;
                                }));

                                //Thread.Sleep(SleepSerialValvePackOperExeDelayRecv);

                            }
                            catch (Exception ex)
                            {
                            }
                        }
                        #endregion

                        #region 水泵关
                        else if (myEnum_UserButtonOperIdentify == Enum_UserButtonOperIdentify.ShuiBengKai_Stop)
                        {
                            try
                            {
                                bSerialValvePackOperOK = false;

                                //水泵关，对面阀上电2s后断电
                                this.BeginInvoke(new Func<object>(() =>
                                {
                                    Global.m_FormBoardII.btn_DOUT15.ImageIndex = 1;//16A开
                                    Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT15, null);
                                    Global.m_FormBoardII.btn_DOUT16.ImageIndex = 0;//16B关
                                    Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT16, null);
                                    Global.m_FormBoardII.btn_Perform_Click(Global.m_FormBoardII.btn_SetDataIntoPCB, null);

                                    return null;
                                }));

                                Thread.Sleep(2000);

                                this.BeginInvoke(new Func<object>(() =>
                                {
                                    Global.m_FormBoardII.btn_DOUT15.ImageIndex = 0;//16A关
                                    Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT15, null);
                                    Global.m_FormBoardII.btn_DOUT16.ImageIndex = 0;//16B关
                                    Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT16, null);
                                    Global.m_FormBoardII.btn_Perform_Click(Global.m_FormBoardII.btn_SetDataIntoPCB, null);

                                    //btn_ShuiBengKai.BackColor = Color.Transparent;

                                    return null;
                                }));

                                //Thread.Sleep(SleepSerialValvePackOperExeDelayRecv);

                            }
                            catch (Exception ex)
                            {
                            }
                        }
                        #endregion




                        #region 减压阀低压开
                        else if (myEnum_UserButtonOperIdentify == Enum_UserButtonOperIdentify.JianYaFa_DiYa)
                        {
                            try
                            {
                                bSerialValvePackOperOK = false;

                                //减压阀低压开，中压、高压对面阀上电2s后再断电

                                if (color_Button_JianYaFa_DiYa == Color.Transparent &&
                                        color_Button_JianYaFa_ZhongYa == Color.Transparent &&
                                        color_Button_JianYaFa_GaoYa == Color.Transparent)    //首次上电
                                {
                                    this.BeginInvoke(new Func<object>(() =>
                                    {
                                        Global.m_FormBoardII.btn_DOUT7.ImageIndex = 0;//12A关
                                        Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT7, null);
                                        Global.m_FormBoardII.btn_DOUT8.ImageIndex = 1;//12B开
                                        Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT8, null);

                                        Global.m_FormBoardII.btn_DOUT9.ImageIndex = 0;
                                        Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT9, null);
                                        Global.m_FormBoardII.btn_DOUT10.ImageIndex = 0;
                                        Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT10, null);

                                        Global.m_FormBoardII.btn_DOUT11.ImageIndex = 0;
                                        Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT11, null);
                                        Global.m_FormBoardII.btn_DOUT12.ImageIndex = 0;
                                        Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT12, null);

                                        Global.m_FormBoardII.btn_Perform_Click(Global.m_FormBoardII.btn_SetDataIntoPCB, null);

                                        //btn_JianYaFa_DiYa.BackColor = Color.Green;
                                        //btn_JianYaFa_ZhongYa.BackColor = Color.Transparent;
                                        //btn_JianYaFa_GaoYa.BackColor = Color.Transparent;

                                        return null;
                                    }));
                                }
                                else if (color_Button_JianYaFa_DiYa == Color.Transparent &&
                                        color_Button_JianYaFa_ZhongYa == Color.Green &&
                                        color_Button_JianYaFa_GaoYa == Color.Transparent)
                                {
                                    this.BeginInvoke(new Func<object>(() =>
                                    {
                                        Global.m_FormBoardII.btn_DOUT7.ImageIndex = 0;//12A关
                                        Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT7, null);
                                        Global.m_FormBoardII.btn_DOUT8.ImageIndex = 1;//12B开
                                        Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT8, null);

                                        Global.m_FormBoardII.btn_DOUT9.ImageIndex = 1;
                                        Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT9, null);
                                        Global.m_FormBoardII.btn_DOUT10.ImageIndex = 0;
                                        Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT10, null);

                                        Global.m_FormBoardII.btn_DOUT11.ImageIndex = 0;
                                        Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT11, null);
                                        Global.m_FormBoardII.btn_DOUT12.ImageIndex = 0;
                                        Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT12, null);

                                        Global.m_FormBoardII.btn_Perform_Click(Global.m_FormBoardII.btn_SetDataIntoPCB, null);

                                        //btn_JianYaFa_DiYa.BackColor = Color.Green;
                                        //btn_JianYaFa_ZhongYa.BackColor = Color.Transparent;
                                        //btn_JianYaFa_GaoYa.BackColor = Color.Transparent;

                                        return null;
                                    }));

                                    Thread.Sleep(2000);

                                    this.BeginInvoke(new Func<object>(() =>
                                    {
                                        Global.m_FormBoardII.btn_DOUT7.ImageIndex = 0;//12A关
                                        Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT7, null);
                                        Global.m_FormBoardII.btn_DOUT8.ImageIndex = 1;//12B开
                                        Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT8, null);

                                        Global.m_FormBoardII.btn_DOUT9.ImageIndex = 0;//13A关，中压对面阀上电2s后断电
                                        Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT9, null);
                                        Global.m_FormBoardII.btn_DOUT10.ImageIndex = 0;//13B关
                                        Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT10, null);

                                        Global.m_FormBoardII.btn_DOUT11.ImageIndex = 0;//14A关，高压对面阀上电2s后断电
                                        Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT11, null);
                                        Global.m_FormBoardII.btn_DOUT12.ImageIndex = 0;//14B关
                                        Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT12, null);

                                        Global.m_FormBoardII.btn_Perform_Click(Global.m_FormBoardII.btn_SetDataIntoPCB, null);

                                        //btn_JianYaFa_DiYa.BackColor = Color.Green;
                                        //btn_JianYaFa_ZhongYa.BackColor = Color.Transparent;
                                        //btn_JianYaFa_GaoYa.BackColor = Color.Transparent;

                                        return null;
                                    }));
                                }
                                else if (color_Button_JianYaFa_DiYa == Color.Transparent &&
                                       color_Button_JianYaFa_ZhongYa == Color.Transparent &&
                                       color_Button_JianYaFa_GaoYa == Color.Green)
                                {
                                    this.BeginInvoke(new Func<object>(() =>
                                    {
                                        Global.m_FormBoardII.btn_DOUT7.ImageIndex = 0;//12A关
                                        Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT7, null);
                                        Global.m_FormBoardII.btn_DOUT8.ImageIndex = 1;//12B开
                                        Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT8, null);

                                        Global.m_FormBoardII.btn_DOUT9.ImageIndex = 1;
                                        Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT9, null);
                                        Global.m_FormBoardII.btn_DOUT10.ImageIndex = 0;
                                        Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT10, null);

                                        Global.m_FormBoardII.btn_DOUT11.ImageIndex = 1;
                                        Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT11, null);
                                        Global.m_FormBoardII.btn_DOUT12.ImageIndex = 0;
                                        Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT12, null);

                                        Global.m_FormBoardII.btn_Perform_Click(Global.m_FormBoardII.btn_SetDataIntoPCB, null);

                                        //btn_JianYaFa_DiYa.BackColor = Color.Green;
                                        //btn_JianYaFa_ZhongYa.BackColor = Color.Transparent;
                                        //btn_JianYaFa_GaoYa.BackColor = Color.Transparent;

                                        return null;
                                    }));

                                    Thread.Sleep(2000);

                                    this.BeginInvoke(new Func<object>(() =>
                                    {
                                        Global.m_FormBoardII.btn_DOUT7.ImageIndex = 0;//12A关
                                        Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT7, null);
                                        Global.m_FormBoardII.btn_DOUT8.ImageIndex = 1;//12B开
                                        Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT8, null);

                                        Global.m_FormBoardII.btn_DOUT9.ImageIndex = 0;//13A关，中压对面阀上电2s后断电
                                        Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT9, null);
                                        Global.m_FormBoardII.btn_DOUT10.ImageIndex = 0;//13B关
                                        Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT10, null);

                                        Global.m_FormBoardII.btn_DOUT11.ImageIndex = 0;//14A关，高压对面阀上电2s后断电
                                        Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT11, null);
                                        Global.m_FormBoardII.btn_DOUT12.ImageIndex = 0;//14B关
                                        Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT12, null);

                                        Global.m_FormBoardII.btn_Perform_Click(Global.m_FormBoardII.btn_SetDataIntoPCB, null);

                                        //btn_JianYaFa_DiYa.BackColor = Color.Green;
                                        //btn_JianYaFa_ZhongYa.BackColor = Color.Transparent;
                                        //btn_JianYaFa_GaoYa.BackColor = Color.Transparent;


                                        return null;
                                    }));
                                }

                                //Thread.Sleep(SleepSerialValvePackOperExeDelayRecv);

                            }
                            catch (Exception ex)
                            {
                            }
                        }
                        #endregion


                        #region 减压阀中压开
                        else if (myEnum_UserButtonOperIdentify == Enum_UserButtonOperIdentify.JianYaFa_ZhongYa)
                        {
                            try
                            {
                                bSerialValvePackOperOK = false;

                                if (color_Button_JianYaFa_GaoYa == Color.Transparent)
                                {
                                    this.BeginInvoke(new Func<object>(() =>
                                    {
                                        Global.m_FormBoardII.btn_DOUT7.ImageIndex = 0;//12A关
                                        Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT7, null);
                                        Global.m_FormBoardII.btn_DOUT8.ImageIndex = 1;//12B开
                                        Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT8, null);

                                        Global.m_FormBoardII.btn_DOUT9.ImageIndex = 0;//13A关
                                        Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT9, null);
                                        Global.m_FormBoardII.btn_DOUT10.ImageIndex = 1;//13B开
                                        Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT10, null);

                                        Global.m_FormBoardII.btn_DOUT11.ImageIndex = 0;
                                        Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT11, null);
                                        Global.m_FormBoardII.btn_DOUT12.ImageIndex = 0;
                                        Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT12, null);

                                        Global.m_FormBoardII.btn_Perform_Click(Global.m_FormBoardII.btn_SetDataIntoPCB, null);

                                        //btn_JianYaFa_DiYa.BackColor = Color.Transparent;
                                        //btn_JianYaFa_ZhongYa.BackColor = Color.Green;
                                        //btn_JianYaFa_GaoYa.BackColor = Color.Transparent;

                                        return null;
                                    }));

                                }
                                else
                                {
                                    //高压对面阀上电2s后再断电
                                    this.BeginInvoke(new Func<object>(() =>
                                    {
                                        Global.m_FormBoardII.btn_DOUT7.ImageIndex = 0;//12A
                                        Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT7, null);
                                        Global.m_FormBoardII.btn_DOUT8.ImageIndex = 1;//12B
                                        Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT8, null);

                                        Global.m_FormBoardII.btn_DOUT9.ImageIndex = 0;//13A
                                        Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT9, null);
                                        Global.m_FormBoardII.btn_DOUT10.ImageIndex = 1;//13B
                                        Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT10, null);

                                        Global.m_FormBoardII.btn_DOUT11.ImageIndex = 1;//14A
                                        Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT11, null);
                                        Global.m_FormBoardII.btn_DOUT12.ImageIndex = 0;//14B
                                        Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT12, null);

                                        Global.m_FormBoardII.btn_Perform_Click(Global.m_FormBoardII.btn_SetDataIntoPCB, null);

                                        //btn_JianYaFa_DiYa.BackColor = Color.Transparent;
                                        //btn_JianYaFa_ZhongYa.BackColor = Color.Green;
                                        //btn_JianYaFa_GaoYa.BackColor = Color.Transparent;

                                        return null;
                                    }));

                                    Thread.Sleep(2000);

                                    this.BeginInvoke(new Func<object>(() =>
                                    {
                                        Global.m_FormBoardII.btn_DOUT7.ImageIndex = 0;//12A关
                                        Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT7, null);
                                        Global.m_FormBoardII.btn_DOUT8.ImageIndex = 1;//12B开
                                        Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT8, null);

                                        Global.m_FormBoardII.btn_DOUT9.ImageIndex = 0;//13A关
                                        Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT9, null);
                                        Global.m_FormBoardII.btn_DOUT10.ImageIndex = 1;//13B开
                                        Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT10, null);

                                        Global.m_FormBoardII.btn_DOUT11.ImageIndex = 0;//14A关
                                        Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT11, null);
                                        Global.m_FormBoardII.btn_DOUT12.ImageIndex = 0;//14B关
                                        Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT12, null);

                                        Global.m_FormBoardII.btn_Perform_Click(Global.m_FormBoardII.btn_SetDataIntoPCB, null);

                                        //btn_JianYaFa_DiYa.BackColor = Color.Transparent;
                                        //btn_JianYaFa_ZhongYa.BackColor = Color.Green;
                                        //btn_JianYaFa_GaoYa.BackColor = Color.Transparent;

                                        return null;
                                    }));
                                }

                                //Thread.Sleep(SleepSerialValvePackOperExeDelayRecv);

                            }
                            catch (Exception ex)
                            {
                            }
                        }
                        #endregion


                        #region 减压阀高压开
                        else if (myEnum_UserButtonOperIdentify == Enum_UserButtonOperIdentify.JianYaFa_GaoYa)
                        {
                            try
                            {
                                bSerialValvePackOperOK = false;


                                this.BeginInvoke(new Func<object>(() =>
                                {
                                    Global.m_FormBoardII.btn_DOUT7.ImageIndex = 0;//12A关
                                    Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT7, null);
                                    Global.m_FormBoardII.btn_DOUT8.ImageIndex = 1;//12B开
                                    Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT8, null);

                                    Global.m_FormBoardII.btn_DOUT9.ImageIndex = 0;//13A关
                                    Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT9, null);
                                    Global.m_FormBoardII.btn_DOUT10.ImageIndex = 1;//13B开
                                    Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT10, null);

                                    Global.m_FormBoardII.btn_DOUT11.ImageIndex = 0;//14A关
                                    Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT11, null);
                                    Global.m_FormBoardII.btn_DOUT12.ImageIndex = 1;//14B开
                                    Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT12, null);
                                    Global.m_FormBoardII.btn_Perform_Click(Global.m_FormBoardII.btn_SetDataIntoPCB, null);

                                    //btn_JianYaFa_DiYa.BackColor = Color.Transparent;
                                    //btn_JianYaFa_ZhongYa.BackColor = Color.Transparent;
                                    //btn_JianYaFa_GaoYa.BackColor = Color.Green;


                                    return null;
                                }));

                                //Thread.Sleep(SleepSerialValvePackOperExeDelayRecv);

                            }
                            catch (Exception ex)
                            {
                            }
                        }
                        #endregion


                        #region 减压阀中、低、高压均关闭
                        else if (myEnum_UserButtonOperIdentify == Enum_UserButtonOperIdentify.JianYaFa_DiYa_ZhongYa_GaoYa_Stop)
                        {
                            try
                            {
                                bSerialValvePackOperOK = false;

                                //减压阀中、低、高压均关闭，中、低、高压对面阀上电2s后再断电
                                this.BeginInvoke(new Func<object>(() =>
                                {
                                    Global.m_FormBoardII.btn_DOUT7.ImageIndex = 1;//12A开
                                    Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT7, null);
                                    Global.m_FormBoardII.btn_DOUT8.ImageIndex = 0;//12B关
                                    Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT8, null);

                                    Global.m_FormBoardII.btn_DOUT9.ImageIndex = 1;//13A开
                                    Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT9, null);
                                    Global.m_FormBoardII.btn_DOUT10.ImageIndex = 0;//13B关
                                    Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT10, null);

                                    Global.m_FormBoardII.btn_DOUT11.ImageIndex = 1;//14A开，高压对面阀上电
                                    Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT11, null);
                                    Global.m_FormBoardII.btn_DOUT12.ImageIndex = 0;//14B关
                                    Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT12, null);

                                    Global.m_FormBoardII.btn_Perform_Click(Global.m_FormBoardII.btn_SetDataIntoPCB, null);

                                    return null;
                                }));

                                Thread.Sleep(2000);


                                this.BeginInvoke(new Func<object>(() =>
                                {
                                    Global.m_FormBoardII.btn_DOUT7.ImageIndex = 0;//12A关
                                    Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT7, null);
                                    Global.m_FormBoardII.btn_DOUT8.ImageIndex = 0;//12B关
                                    Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT8, null);

                                    Global.m_FormBoardII.btn_DOUT9.ImageIndex = 0;//13A关
                                    Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT9, null);
                                    Global.m_FormBoardII.btn_DOUT10.ImageIndex = 0;//13B关
                                    Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT10, null);

                                    Global.m_FormBoardII.btn_DOUT11.ImageIndex = 0;//14A关
                                    Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT11, null);
                                    Global.m_FormBoardII.btn_DOUT12.ImageIndex = 0;//14B关
                                    Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT12, null);

                                    Global.m_FormBoardII.btn_Perform_Click(Global.m_FormBoardII.btn_SetDataIntoPCB, null);

                                    //btn_JianYaFa_DiYa.BackColor = Color.Transparent;
                                    //btn_JianYaFa_ZhongYa.BackColor = Color.Transparent;
                                    //btn_JianYaFa_GaoYa.BackColor = Color.Transparent;

                                    return null;
                                }));

                                //Thread.Sleep(SleepSerialValvePackOperExeDelayRecv);

                            }
                            catch (Exception ex)
                            {
                            }
                        }
                        #endregion




                        #region 钻进高速
                        else if (myEnum_UserButtonOperIdentify == Enum_UserButtonOperIdentify.ZuanJinGaoSu)
                        {
                            try
                            {
                                bSerialValvePackOperOK = false;
                                this.BeginInvoke(new Func<object>(() =>
                                {
                                    Global.m_FormBoardII.btn_DOUT13.ImageIndex = 0;//15A关
                                    Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT13, null);
                                    Global.m_FormBoardII.btn_DOUT14.ImageIndex = 1;//15B开
                                    Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT14, null);
                                    Global.m_FormBoardII.btn_Perform_Click(Global.m_FormBoardII.btn_SetDataIntoPCB, null);

                                    //btn_ZuanJinFa_GaoSu.BackColor = Color.Green;

                                    return null;
                                }));

                                //Thread.Sleep(SleepSerialValvePackOperExeDelayRecv);

                            }
                            catch (Exception ex)
                            {
                            }
                        }
                        #endregion

                        #region 钻进高速关
                        else if (myEnum_UserButtonOperIdentify == Enum_UserButtonOperIdentify.ZuanJinGaoSu_Stop)
                        {
                            try
                            {
                                bSerialValvePackOperOK = false;

                                //钻进高速关，对面阀上电2s后断电
                                this.BeginInvoke(new Func<object>(() =>
                                {
                                    Global.m_FormBoardII.btn_DOUT13.ImageIndex = 1;//15A开
                                    Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT13, null);
                                    Global.m_FormBoardII.btn_DOUT14.ImageIndex = 0;//15B关
                                    Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT14, null);
                                    Global.m_FormBoardII.btn_Perform_Click(Global.m_FormBoardII.btn_SetDataIntoPCB, null);

                                    return null;
                                }));

                                Thread.Sleep(2000);

                                this.BeginInvoke(new Func<object>(() =>
                                {
                                    Global.m_FormBoardII.btn_DOUT13.ImageIndex = 0;//15A关
                                    Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT13, null);
                                    Global.m_FormBoardII.btn_DOUT14.ImageIndex = 0;//15B关
                                    Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT14, null);
                                    Global.m_FormBoardII.btn_Perform_Click(Global.m_FormBoardII.btn_SetDataIntoPCB, null);

                                    //btn_ZuanJinFa_GaoSu.BackColor = Color.Transparent;

                                    return null;
                                }));

                                //Thread.Sleep(SleepSerialValvePackOperExeDelayRecv);

                            }
                            catch (Exception ex)
                            {
                            }
                        }
                        #endregion


                        //5.12
                        //阀箱备用通道
                        //#region 阀箱备用通道A7
                        //else if (myEnum_UserButtonOperIdentify == Enum_UserButtonOperIdentify.FaXiang_Space_A7)
                        //{
                        //    try
                        //    {
                        //        bSerialValvePackOperOK = false;

                        //        this.BeginInvoke(new Func<object>(() =>
                        //        {
                        //            Global.m_FormBoardII.btn_PWM13.ImageIndex = 1;
                        //            Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_PWM13, null);
                        //            Global.m_FormBoardII.btn_PWM14.ImageIndex = 0;
                        //            Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_PWM14, null);
                        //            Global.m_FormBoardII.btn_Perform_Click(Global.m_FormBoardII.btn_SetDataIntoPCB, null);

                        //            return null;
                        //        }));

                        //        Thread.Sleep(SleepSerialValvePackOperExeDelayRecv);

                        //        if (bSerialValvePackOperOK)
                        //        {
                        //            this.BeginInvoke(new Func<object>(() =>
                        //            {
                        //                btn_A7.BackColor = Color.Green;
                        //                btn_B7.BackColor = Color.Transparent;
                        //                btn_AB7_Stop.BackColor = Color.Transparent;
                        //                return null;
                        //            }));
                        //        }
                        //        else
                        //        {
                        //            this.BeginInvoke(new Func<object>(() =>
                        //            {
                        //                btn_A7.BackColor = Color.Yellow;
                        //                btn_B7.BackColor = Color.Transparent;
                        //                btn_AB7_Stop.BackColor = Color.Transparent;
                        //                return null;
                        //            }));
                        //        }

                        //    }
                        //    catch (Exception ex)
                        //    {
                        //    }
                        //}
                        //#endregion

                        //#region 阀箱备用通道B7
                        //else if (myEnum_UserButtonOperIdentify == Enum_UserButtonOperIdentify.FaXiang_Space_B7)
                        //{
                        //    try
                        //    {
                        //        bSerialValvePackOperOK = false;

                        //        this.BeginInvoke(new Func<object>(() =>
                        //        {
                        //            Global.m_FormBoardII.btn_PWM13.ImageIndex = 0;
                        //            Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_PWM13, null);
                        //            Global.m_FormBoardII.btn_PWM14.ImageIndex = 1;
                        //            Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_PWM14, null);
                        //            Global.m_FormBoardII.btn_Perform_Click(Global.m_FormBoardII.btn_SetDataIntoPCB, null);

                        //            return null;
                        //        }));

                        //        Thread.Sleep(SleepSerialValvePackOperExeDelayRecv);

                        //        if (bSerialValvePackOperOK)
                        //        {
                        //            this.BeginInvoke(new Func<object>(() =>
                        //            {
                        //                btn_A7.BackColor = Color.Transparent;
                        //                btn_B7.BackColor = Color.Green;
                        //                btn_AB7_Stop.BackColor = Color.Transparent;
                        //                return null;
                        //            }));
                        //        }
                        //        else
                        //        {
                        //            this.BeginInvoke(new Func<object>(() =>
                        //            {
                        //                btn_A7.BackColor = Color.Transparent;
                        //                btn_B7.BackColor = Color.Yellow;
                        //                btn_AB7_Stop.BackColor = Color.Transparent;
                        //                return null;
                        //            }));
                        //        }

                        //    }
                        //    catch (Exception ex)
                        //    {
                        //    }
                        //}
                        //#endregion

                        //#region 阀箱备用通道AB7，停止
                        //else if (myEnum_UserButtonOperIdentify == Enum_UserButtonOperIdentify.FaXiang_Space_AB7_Stop)
                        //{
                        //    try
                        //    {
                        //        bSerialValvePackOperOK = false;

                        //        this.BeginInvoke(new Func<object>(() =>
                        //        {
                        //            Global.m_FormBoardII.btn_PWM13.ImageIndex = 0;
                        //            Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_PWM13, null);
                        //            Global.m_FormBoardII.btn_PWM14.ImageIndex = 0;
                        //            Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_PWM14, null);
                        //            Global.m_FormBoardII.btn_Perform_Click(Global.m_FormBoardII.btn_SetDataIntoPCB, null);

                        //            return null;
                        //        }));

                        //        Thread.Sleep(SleepSerialValvePackOperExeDelayRecv);

                        //        if (bSerialValvePackOperOK)
                        //        {
                        //            this.BeginInvoke(new Func<object>(() =>
                        //            {
                        //                btn_A7.BackColor = Color.Transparent;
                        //                btn_B7.BackColor = Color.Transparent;
                        //                btn_AB7_Stop.BackColor = Color.Green;
                        //                return null;
                        //            }));
                        //        }
                        //        else
                        //        {
                        //            this.BeginInvoke(new Func<object>(() =>
                        //            {
                        //                btn_A7.BackColor = Color.Transparent;
                        //                btn_B7.BackColor = Color.Transparent;
                        //                btn_AB7_Stop.BackColor = Color.Yellow;
                        //                return null;
                        //            }));
                        //        }

                        //    }
                        //    catch (Exception ex)
                        //    {
                        //    }
                        //}
                        //#endregion




                        //#region 阀箱备用通道A8
                        //else if (myEnum_UserButtonOperIdentify == Enum_UserButtonOperIdentify.FaXiang_Space_A8)
                        //{
                        //    try
                        //    {
                        //        bSerialValvePackOperOK = false;

                        //        this.BeginInvoke(new Func<object>(() =>
                        //        {
                        //            Global.m_FormBoardII.btn_PWM15.ImageIndex = 1;
                        //            Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_PWM15, null);
                        //            Global.m_FormBoardII.btn_PWM16.ImageIndex = 0;
                        //            Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_PWM16, null);
                        //            Global.m_FormBoardII.btn_Perform_Click(Global.m_FormBoardII.btn_SetDataIntoPCB, null);

                        //            return null;
                        //        }));

                        //        Thread.Sleep(SleepSerialValvePackOperExeDelayRecv);

                        //        if (bSerialValvePackOperOK)
                        //        {
                        //            this.BeginInvoke(new Func<object>(() =>
                        //            {
                        //                btn_A8.BackColor = Color.Green;
                        //                btn_B8.BackColor = Color.Transparent;
                        //                btn_AB8_Stop.BackColor = Color.Transparent;
                        //                return null;
                        //            }));
                        //        }
                        //        else
                        //        {
                        //            this.BeginInvoke(new Func<object>(() =>
                        //            {
                        //                btn_A8.BackColor = Color.Yellow;
                        //                btn_B8.BackColor = Color.Transparent;
                        //                btn_AB8_Stop.BackColor = Color.Transparent;
                        //                return null;
                        //            }));
                        //        }

                        //    }
                        //    catch (Exception ex)
                        //    {
                        //    }
                        //}
                        //#endregion

                        //#region 阀箱备用通道B8
                        //else if (myEnum_UserButtonOperIdentify == Enum_UserButtonOperIdentify.FaXiang_Space_B8)
                        //{
                        //    try
                        //    {
                        //        bSerialValvePackOperOK = false;

                        //        this.BeginInvoke(new Func<object>(() =>
                        //        {
                        //            Global.m_FormBoardII.btn_PWM15.ImageIndex = 0;
                        //            Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_PWM15, null);
                        //            Global.m_FormBoardII.btn_PWM16.ImageIndex = 1;
                        //            Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_PWM16, null);
                        //            Global.m_FormBoardII.btn_Perform_Click(Global.m_FormBoardII.btn_SetDataIntoPCB, null);

                        //            return null;
                        //        }));

                        //        Thread.Sleep(SleepSerialValvePackOperExeDelayRecv);

                        //        if (bSerialValvePackOperOK)
                        //        {
                        //            this.BeginInvoke(new Func<object>(() =>
                        //            {
                        //                btn_A8.BackColor = Color.Transparent;
                        //                btn_B8.BackColor = Color.Green;
                        //                btn_AB8_Stop.BackColor = Color.Transparent;
                        //                return null;
                        //            }));
                        //        }
                        //        else
                        //        {
                        //            this.BeginInvoke(new Func<object>(() =>
                        //            {
                        //                btn_A8.BackColor = Color.Transparent;
                        //                btn_B8.BackColor = Color.Yellow;
                        //                btn_AB8_Stop.BackColor = Color.Transparent;
                        //                return null;
                        //            }));
                        //        }

                        //    }
                        //    catch (Exception ex)
                        //    {
                        //    }
                        //}
                        //#endregion

                        //#region 阀箱备用通道AB8，停止
                        //else if (myEnum_UserButtonOperIdentify == Enum_UserButtonOperIdentify.FaXiang_Space_AB8_Stop)
                        //{
                        //    try
                        //    {
                        //        bSerialValvePackOperOK = false;

                        //        this.BeginInvoke(new Func<object>(() =>
                        //        {
                        //            Global.m_FormBoardII.btn_PWM15.ImageIndex = 0;
                        //            Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_PWM15, null);
                        //            Global.m_FormBoardII.btn_PWM16.ImageIndex = 0;
                        //            Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_PWM16, null);
                        //            Global.m_FormBoardII.btn_Perform_Click(Global.m_FormBoardII.btn_SetDataIntoPCB, null);

                        //            return null;
                        //        }));

                        //        Thread.Sleep(SleepSerialValvePackOperExeDelayRecv);

                        //        if (bSerialValvePackOperOK)
                        //        {
                        //            this.BeginInvoke(new Func<object>(() =>
                        //            {
                        //                btn_A8.BackColor = Color.Transparent;
                        //                btn_B8.BackColor = Color.Transparent;
                        //                btn_AB8_Stop.BackColor = Color.Green;
                        //                return null;
                        //            }));
                        //        }
                        //        else
                        //        {
                        //            this.BeginInvoke(new Func<object>(() =>
                        //            {
                        //                btn_A8.BackColor = Color.Transparent;
                        //                btn_B8.BackColor = Color.Transparent;
                        //                btn_AB8_Stop.BackColor = Color.Yellow;
                        //                return null;
                        //            }));
                        //        }

                        //    }
                        //    catch (Exception ex)
                        //    {
                        //    }
                        //}
                        //#endregion



                        //#region 阀箱备用通道A9
                        //else if (myEnum_UserButtonOperIdentify == Enum_UserButtonOperIdentify.FaXiang_Space_A9)
                        //{
                        //    try
                        //    {
                        //        bSerialValvePackOperOK = false;

                        //        this.BeginInvoke(new Func<object>(() =>
                        //        {
                        //            Global.m_FormBoardII.btn_DOUT1.ImageIndex = 1;
                        //            Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT1, null);
                        //            Global.m_FormBoardII.btn_DOUT2.ImageIndex = 0;
                        //            Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT2, null);
                        //            Global.m_FormBoardII.btn_Perform_Click(Global.m_FormBoardII.btn_SetDataIntoPCB, null);

                        //            return null;
                        //        }));

                        //        Thread.Sleep(SleepSerialValvePackOperExeDelayRecv);

                        //        if (bSerialValvePackOperOK)
                        //        {
                        //            this.BeginInvoke(new Func<object>(() =>
                        //            {
                        //                btn_A9.BackColor = Color.Green;
                        //                btn_B9.BackColor = Color.Transparent;
                        //                btn_AB9_Stop.BackColor = Color.Transparent;
                        //                return null;
                        //            }));
                        //        }
                        //        else
                        //        {
                        //            this.BeginInvoke(new Func<object>(() =>
                        //            {
                        //                btn_A9.BackColor = Color.Yellow;
                        //                btn_B9.BackColor = Color.Transparent;
                        //                btn_AB9_Stop.BackColor = Color.Transparent;
                        //                return null;
                        //            }));
                        //        }

                        //    }
                        //    catch (Exception ex)
                        //    {
                        //    }
                        //}
                        //#endregion

                        //#region 阀箱备用通道B9
                        //else if (myEnum_UserButtonOperIdentify == Enum_UserButtonOperIdentify.FaXiang_Space_B9)
                        //{
                        //    try
                        //    {
                        //        bSerialValvePackOperOK = false;

                        //        this.BeginInvoke(new Func<object>(() =>
                        //        {
                        //            Global.m_FormBoardII.btn_DOUT1.ImageIndex = 0;
                        //            Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT1, null);
                        //            Global.m_FormBoardII.btn_DOUT2.ImageIndex = 1;
                        //            Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT2, null);
                        //            Global.m_FormBoardII.btn_Perform_Click(Global.m_FormBoardII.btn_SetDataIntoPCB, null);

                        //            return null;
                        //        }));

                        //        Thread.Sleep(SleepSerialValvePackOperExeDelayRecv);

                        //        if (bSerialValvePackOperOK)
                        //        {
                        //            this.BeginInvoke(new Func<object>(() =>
                        //            {
                        //                btn_A9.BackColor = Color.Transparent;
                        //                btn_B9.BackColor = Color.Green;
                        //                btn_AB9_Stop.BackColor = Color.Transparent;
                        //                return null;
                        //            }));
                        //        }
                        //        else
                        //        {
                        //            this.BeginInvoke(new Func<object>(() =>
                        //            {
                        //                btn_A9.BackColor = Color.Transparent;
                        //                btn_B9.BackColor = Color.Yellow;
                        //                btn_AB9_Stop.BackColor = Color.Transparent;
                        //                return null;
                        //            }));
                        //        }

                        //    }
                        //    catch (Exception ex)
                        //    {
                        //    }
                        //}
                        //#endregion

                        //#region 阀箱备用通道AB9，停止
                        //else if (myEnum_UserButtonOperIdentify == Enum_UserButtonOperIdentify.FaXiang_Space_AB9_Stop)
                        //{
                        //    try
                        //    {
                        //        bSerialValvePackOperOK = false;

                        //        this.BeginInvoke(new Func<object>(() =>
                        //        {
                        //            Global.m_FormBoardII.btn_DOUT1.ImageIndex = 0;
                        //            Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT1, null);
                        //            Global.m_FormBoardII.btn_DOUT2.ImageIndex = 1;
                        //            Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT2, null);
                        //            Global.m_FormBoardII.btn_Perform_Click(Global.m_FormBoardII.btn_SetDataIntoPCB, null);

                        //            return null;
                        //        }));

                        //        Thread.Sleep(SleepSerialValvePackOperExeDelayRecv);

                        //        if (bSerialValvePackOperOK)
                        //        {
                        //            this.BeginInvoke(new Func<object>(() =>
                        //            {
                        //                btn_A9.BackColor = Color.Transparent;
                        //                btn_B9.BackColor = Color.Transparent;
                        //                btn_AB9_Stop.BackColor = Color.Green;
                        //                return null;
                        //            }));
                        //        }
                        //        else
                        //        {
                        //            this.BeginInvoke(new Func<object>(() =>
                        //            {
                        //                btn_A9.BackColor = Color.Transparent;
                        //                btn_B9.BackColor = Color.Transparent;
                        //                btn_AB9_Stop.BackColor = Color.Yellow;
                        //                return null;
                        //            }));
                        //        }

                        //    }
                        //    catch (Exception ex)
                        //    {
                        //    }
                        //}
                        //#endregion




                        //#region 阀箱备用通道A10
                        //else if (myEnum_UserButtonOperIdentify == Enum_UserButtonOperIdentify.FaXiang_Space_A10)
                        //{
                        //    try
                        //    {
                        //        bSerialValvePackOperOK = false;

                        //        this.BeginInvoke(new Func<object>(() =>
                        //        {
                        //            Global.m_FormBoardII.btn_DOUT3.ImageIndex = 1;
                        //            Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT3, null);
                        //            Global.m_FormBoardII.btn_DOUT4.ImageIndex = 0;
                        //            Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT4, null);
                        //            Global.m_FormBoardII.btn_Perform_Click(Global.m_FormBoardII.btn_SetDataIntoPCB, null);

                        //            return null;
                        //        }));

                        //        Thread.Sleep(SleepSerialValvePackOperExeDelayRecv);

                        //        if (bSerialValvePackOperOK)
                        //        {
                        //            this.BeginInvoke(new Func<object>(() =>
                        //            {
                        //                btn_A10.BackColor = Color.Green;
                        //                btn_B10.BackColor = Color.Transparent;
                        //                btn_AB10_Stop.BackColor = Color.Transparent;
                        //                return null;
                        //            }));
                        //        }
                        //        else
                        //        {
                        //            this.BeginInvoke(new Func<object>(() =>
                        //            {
                        //                btn_A10.BackColor = Color.Yellow;
                        //                btn_B10.BackColor = Color.Transparent;
                        //                btn_AB10_Stop.BackColor = Color.Transparent;
                        //                return null;
                        //            }));
                        //        }

                        //    }
                        //    catch (Exception ex)
                        //    {
                        //    }
                        //}
                        //#endregion

                        //#region 阀箱备用通道B10
                        //else if (myEnum_UserButtonOperIdentify == Enum_UserButtonOperIdentify.FaXiang_Space_B10)
                        //{
                        //    try
                        //    {
                        //        bSerialValvePackOperOK = false;

                        //        this.BeginInvoke(new Func<object>(() =>
                        //        {
                        //            Global.m_FormBoardII.btn_DOUT3.ImageIndex = 0;
                        //            Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT3, null);
                        //            Global.m_FormBoardII.btn_DOUT4.ImageIndex = 1;
                        //            Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT4, null);
                        //            Global.m_FormBoardII.btn_Perform_Click(Global.m_FormBoardII.btn_SetDataIntoPCB, null);

                        //            return null;
                        //        }));

                        //        Thread.Sleep(SleepSerialValvePackOperExeDelayRecv);

                        //        if (bSerialValvePackOperOK)
                        //        {
                        //            this.BeginInvoke(new Func<object>(() =>
                        //            {
                        //                btn_A10.BackColor = Color.Transparent;
                        //                btn_B10.BackColor = Color.Green;
                        //                btn_AB10_Stop.BackColor = Color.Transparent;
                        //                return null;
                        //            }));
                        //        }
                        //        else
                        //        {
                        //            this.BeginInvoke(new Func<object>(() =>
                        //            {
                        //                btn_A10.BackColor = Color.Transparent;
                        //                btn_B10.BackColor = Color.Yellow;
                        //                btn_AB10_Stop.BackColor = Color.Transparent;
                        //                return null;
                        //            }));
                        //        }

                        //    }
                        //    catch (Exception ex)
                        //    {
                        //    }
                        //}
                        //#endregion

                        //#region 阀箱备用通道AB10，停止
                        //else if (myEnum_UserButtonOperIdentify == Enum_UserButtonOperIdentify.FaXiang_Space_AB10_Stop)
                        //{
                        //    try
                        //    {
                        //        bSerialValvePackOperOK = false;

                        //        this.BeginInvoke(new Func<object>(() =>
                        //        {
                        //            Global.m_FormBoardII.btn_DOUT3.ImageIndex = 0;
                        //            Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT3, null);
                        //            Global.m_FormBoardII.btn_DOUT4.ImageIndex = 0;
                        //            Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT4, null);
                        //            Global.m_FormBoardII.btn_Perform_Click(Global.m_FormBoardII.btn_SetDataIntoPCB, null);

                        //            return null;
                        //        }));

                        //        Thread.Sleep(SleepSerialValvePackOperExeDelayRecv);

                        //        if (bSerialValvePackOperOK)
                        //        {
                        //            this.BeginInvoke(new Func<object>(() =>
                        //            {
                        //                btn_A10.BackColor = Color.Transparent;
                        //                btn_B10.BackColor = Color.Transparent;
                        //                btn_AB10_Stop.BackColor = Color.Green;
                        //                return null;
                        //            }));
                        //        }
                        //        else
                        //        {
                        //            this.BeginInvoke(new Func<object>(() =>
                        //            {
                        //                btn_A10.BackColor = Color.Transparent;
                        //                btn_B10.BackColor = Color.Transparent;
                        //                btn_AB10_Stop.BackColor = Color.Yellow;
                        //                return null;
                        //            }));
                        //        }

                        //    }
                        //    catch (Exception ex)
                        //    {
                        //    }
                        //}
                        //#endregion


                        //#region 阀箱备用通道A11
                        //else if (myEnum_UserButtonOperIdentify == Enum_UserButtonOperIdentify.FaXiang_Space_A11)
                        //{
                        //    try
                        //    {
                        //        bSerialValvePackOperOK = false;

                        //        this.BeginInvoke(new Func<object>(() =>
                        //        {
                        //            Global.m_FormBoardII.btn_DOUT5.ImageIndex = 1;
                        //            Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT5, null);
                        //            Global.m_FormBoardII.btn_DOUT6.ImageIndex = 0;
                        //            Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT6, null);
                        //            Global.m_FormBoardII.btn_Perform_Click(Global.m_FormBoardII.btn_SetDataIntoPCB, null);

                        //            return null;
                        //        }));

                        //        Thread.Sleep(SleepSerialValvePackOperExeDelayRecv);

                        //        if (bSerialValvePackOperOK)
                        //        {
                        //            this.BeginInvoke(new Func<object>(() =>
                        //            {
                        //                btn_A11.BackColor = Color.Green;
                        //                btn_B11.BackColor = Color.Transparent;
                        //                btn_AB11_Stop.BackColor = Color.Transparent;
                        //                return null;
                        //            }));
                        //        }
                        //        else
                        //        {
                        //            this.BeginInvoke(new Func<object>(() =>
                        //            {
                        //                btn_A11.BackColor = Color.Yellow;
                        //                btn_B11.BackColor = Color.Transparent;
                        //                btn_AB11_Stop.BackColor = Color.Transparent;
                        //                return null;
                        //            }));
                        //        }

                        //    }
                        //    catch (Exception ex)
                        //    {
                        //    }
                        //}
                        //#endregion

                        //#region 阀箱备用通道B11
                        //else if (myEnum_UserButtonOperIdentify == Enum_UserButtonOperIdentify.FaXiang_Space_B11)
                        //{
                        //    try
                        //    {
                        //        bSerialValvePackOperOK = false;

                        //        this.BeginInvoke(new Func<object>(() =>
                        //        {
                        //            Global.m_FormBoardII.btn_DOUT5.ImageIndex = 0;
                        //            Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT5, null);
                        //            Global.m_FormBoardII.btn_DOUT6.ImageIndex = 1;
                        //            Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT6, null);
                        //            Global.m_FormBoardII.btn_Perform_Click(Global.m_FormBoardII.btn_SetDataIntoPCB, null);

                        //            return null;
                        //        }));

                        //        Thread.Sleep(SleepSerialValvePackOperExeDelayRecv);

                        //        if (bSerialValvePackOperOK)
                        //        {
                        //            this.BeginInvoke(new Func<object>(() =>
                        //            {
                        //                btn_A11.BackColor = Color.Transparent;
                        //                btn_B11.BackColor = Color.Green;
                        //                btn_AB11_Stop.BackColor = Color.Transparent;
                        //                return null;
                        //            }));
                        //        }
                        //        else
                        //        {
                        //            this.BeginInvoke(new Func<object>(() =>
                        //            {
                        //                btn_A11.BackColor = Color.Transparent;
                        //                btn_B11.BackColor = Color.Yellow;
                        //                btn_AB11_Stop.BackColor = Color.Transparent;
                        //                return null;
                        //            }));
                        //        }

                        //    }
                        //    catch (Exception ex)
                        //    {
                        //    }
                        //}
                        //#endregion

                        //#region 阀箱备用通道AB11，停止
                        //else if (myEnum_UserButtonOperIdentify == Enum_UserButtonOperIdentify.FaXiang_Space_AB11_Stop)
                        //{
                        //    try
                        //    {
                        //        bSerialValvePackOperOK = false;

                        //        this.BeginInvoke(new Func<object>(() =>
                        //        {
                        //            Global.m_FormBoardII.btn_DOUT5.ImageIndex = 0;
                        //            Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT5, null);
                        //            Global.m_FormBoardII.btn_DOUT6.ImageIndex = 0;
                        //            Global.m_FormBoardII.btn_Operate_Click_Remote(Global.m_FormBoardII.btn_DOUT6, null);
                        //            Global.m_FormBoardII.btn_Perform_Click(Global.m_FormBoardII.btn_SetDataIntoPCB, null);

                        //            return null;
                        //        }));

                        //        Thread.Sleep(SleepSerialValvePackOperExeDelayRecv);

                        //        if (bSerialValvePackOperOK)
                        //        {
                        //            this.BeginInvoke(new Func<object>(() =>
                        //            {
                        //                btn_A11.BackColor = Color.Transparent;
                        //                btn_B11.BackColor = Color.Transparent;
                        //                btn_AB11_Stop.BackColor = Color.Green;
                        //                return null;
                        //            }));
                        //        }
                        //        else
                        //        {
                        //            this.BeginInvoke(new Func<object>(() =>
                        //            {
                        //                btn_A11.BackColor = Color.Transparent;
                        //                btn_B11.BackColor = Color.Transparent;
                        //                btn_AB11_Stop.BackColor = Color.Yellow;
                        //                return null;
                        //            }));
                        //        }

                        //    }
                        //    catch (Exception ex)
                        //    {
                        //    }
                        //}
                        //#endregion





                    }
                    catch (Exception ex)
                    { }

                    flagCmdToSend = false;
                }
            }
            catch (Exception ex)
            { }
        }

        #endregion

        public void Stop()
        {
            try
            {
                stopThread = true;
                stopThreadFuQianOper = true;
                stopThreadDiCiJiaJiaoOper = true;
                stopThread_TuiJinQi_HMove_Oper = true;
                stopThreadAutoHighOper = true;

                if (threadServoValvePackOper != null && threadServoValvePackOper.IsAlive)
                {
                    threadServoValvePackOper.Abort();
                }
                if (threadFuQianOper != null && threadFuQianOper.IsAlive)
                {
                    threadFuQianOper.Abort();
                }

                if (thread_TuiJinQi_HMove_Oper != null && thread_TuiJinQi_HMove_Oper.IsAlive)
                {
                    thread_TuiJinQi_HMove_Oper.Abort();
                }
                if (threadAutoDirCtlOper != null && threadAutoDirCtlOper.IsAlive)
                {
                    threadAutoDirCtlOper.Abort();
                }

                if (threadAutoHighOper != null && threadAutoHighOper.IsAlive)
                {
                    threadAutoHighOper.Abort();
                }
            }
            catch (Exception ex)
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


                    #region Btn的状态变化事件
                    //GroupBox_ZuanJin
                    //GroupBox_FaXiang_Space
                    //GroupBox_CtlBtns
                    //GroupBox_TuiJinQi
                    //foreach (Control ctl in this.GroupBox_ZuanJin.Controls)
                    //{
                    //    if (ctl is Button)
                    //    {
                    //        Button btn = ctl as Button;
                    //        if (btn.BackColor == Color.Green)
                    //        {
                    //            btn.BackColor = Color.Yellow;
                    //        }
                    //    }
                    //    else
                    //    {
                    //        foreach (Control ctl1 in ctl.Controls)
                    //        {
                    //            if (ctl1 is Button)
                    //            {
                    //                Button btn = ctl1 as Button;
                    //                if (btn.BackColor == Color.Green)
                    //                {
                    //                    btn.BackColor = Color.Yellow;
                    //                }
                    //            }
                    //        }
                    //    }
                    //}

                    foreach (Control ctl in this.GroupBox_FaXiang_Space.Controls)
                    {
                        if (ctl is Button)
                        {
                            Button btn = ctl as Button;
                            if (btn.BackColor == Color.Green)
                            {
                                btn.BackColor = Color.Yellow;
                            }
                        }
                        else
                        {
                            foreach (Control ctl1 in ctl.Controls)
                            {
                                if (ctl1 is Button)
                                {
                                    Button btn = ctl1 as Button;
                                    if (btn.BackColor == Color.Green)
                                    {
                                        btn.BackColor = Color.Yellow;
                                    }
                                }
                            }
                        }
                    }

                    foreach (Control ctl in this.GroupBox_CtlBtns.Controls)
                    {
                        if (ctl is Button)
                        {
                            Button btn = ctl as Button;
                            if (btn.BackColor == Color.Green)
                            {
                                btn.BackColor = Color.Yellow;
                            }
                        }
                        else
                        {
                            foreach (Control ctl1 in ctl.Controls)
                            {
                                if (ctl1 is Button)
                                {
                                    Button btn = ctl1 as Button;
                                    if (btn.BackColor == Color.Green)
                                    {
                                        btn.BackColor = Color.Yellow;
                                    }
                                }
                            }
                        }
                    }

                    foreach (Control ctl in this.GroupBox_TuiJinQi.Controls)
                    {
                        if (ctl is Button)
                        {
                            Button btn = ctl as Button;
                            if (btn.BackColor == Color.Green)
                            {
                                btn.BackColor = Color.Yellow;
                            }
                        }
                        else
                        {
                            foreach (Control ctl1 in ctl.Controls)
                            {
                                if (ctl1 is Button)
                                {
                                    Button btn = ctl1 as Button;
                                    if (btn.BackColor == Color.Green)
                                    {
                                        btn.BackColor = Color.Yellow;
                                    }
                                }
                            }
                        }
                    }

                    #endregion

                }
                if (richTextBox_InfoShow.Lines.Length > 80)
                {
                    richTextBox_InfoShow.Clear();
                }

                textBox_XuanFuJiZhun.Text = Global.TuiJinQiBuChang_XuanFu.ToString();

                this.BeginInvoke(new Func<object>(() =>
                {
                    textBox_V_HL.Text = Global.m_FormBoardI.numericUpDown_AO1.Value.ToString();
                    textBox_V_HR.Text = Global.m_FormBoardI.numericUpDown_AO8.Value.ToString();
                    textBox_V_VLF.Text = Global.m_FormBoardI.numericUpDown_AO3.Value.ToString();
                    textBox_V_VLB.Text = Global.m_FormBoardI.numericUpDown_AO4.Value.ToString();
                    textBox_V_VRF.Text = Global.m_FormBoardI.numericUpDown_AO5.Value.ToString();
                    textBox_V_VRB.Text = Global.m_FormBoardI.numericUpDown_AO6.Value.ToString();

                    return null;
                }));

                #region 各板卡的通信状态检测显示
                if (flag_Communication_61)
                {
                    textBox_Communication_0x61.BackColor = Color.Green;
                }
                else
                {
                    textBox_Communication_0x61.BackColor = Color.Red;
                }
                flag_Communication_61 = false;


                if (flag_Communication_62)
                {
                    textBox_Communication_0x62.BackColor = Color.Green;
                }
                else
                {
                    textBox_Communication_0x62.BackColor = Color.Red;
                }
                flag_Communication_62 = false;

                if (flag_Communication_63)
                {
                    textBox_Communication_0x63.BackColor = Color.Green;
                }
                else
                {
                    textBox_Communication_0x63.BackColor = Color.Red;
                }
                flag_Communication_63 = false;

                if (flag_Communication_70)
                {
                    textBox_Communication_0x70.BackColor = Color.Green;
                }
                else
                {
                    textBox_Communication_0x70.BackColor = Color.Red;

                    #region 灯1~4
                    if (btn_Light_1.BackColor == Color.Green)
                    {
                        btn_Light_1.BackColor = Color.Yellow;
                    }
                    if (btn_Light_2.BackColor == Color.Green)
                    {
                        btn_Light_2.BackColor = Color.Yellow;
                    }
                    if (btn_Light_3.BackColor == Color.Green)
                    {
                        btn_Light_3.BackColor = Color.Yellow;
                    }
                    if (btn_Light_4.BackColor == Color.Green)
                    {
                        btn_Light_4.BackColor = Color.Yellow;
                    }
                    #endregion
                }
                flag_Communication_70 = false;

                if (flag_Communication_71)
                {
                    textBox_Communication_0x71.BackColor = Color.Green;
                }
                else
                {
                    textBox_Communication_0x71.BackColor = Color.Red;

                    #region 灯5~8
                    if (btn_Light_5.BackColor == Color.Green)
                    {
                        btn_Light_5.BackColor = Color.Yellow;
                    }
                    if (btn_Light_6.BackColor == Color.Green)
                    {
                        btn_Light_6.BackColor = Color.Yellow;
                    }
                    if (btn_Light_7.BackColor == Color.Green)
                    {
                        btn_Light_7.BackColor = Color.Yellow;
                    }
                    if (btn_Light_8.BackColor == Color.Green)
                    {
                        btn_Light_8.BackColor = Color.Yellow;
                    }
                    #endregion
                }
                flag_Communication_71 = false;

                if (flag_Communication_72)
                {
                    textBox_Communication_0x72.BackColor = Color.Green;
                }
                else
                {
                    textBox_Communication_0x72.BackColor = Color.Red;
                }
                flag_Communication_72 = false;

                if (flag_Communication_79)
                {
                    textBox_Communication_0x79.BackColor = Color.Green;
                }
                else
                {
                    textBox_Communication_0x79.BackColor = Color.Red;
                }
                flag_Communication_79 = false;

                if (flag_Communication_80)
                {
                    textBox_Communication_0x80.BackColor = Color.Green;
                }
                else
                {
                    textBox_Communication_0x80.BackColor = Color.Red;
                }
                flag_Communication_80 = false;

                if (flag_Communication_25)
                {
                    textBox_Communication_0x25.BackColor = Color.Green;
                }
                else
                {
                    textBox_Communication_0x25.BackColor = Color.Red;
                }
                flag_Communication_25 = false;

                if (flag_Communication_26)
                {
                    textBox_Communication_0x26.BackColor = Color.Green;
                }
                else
                {
                    textBox_Communication_0x26.BackColor = Color.Red;
                }
                flag_Communication_26 = false;

                if (flag_Communication_28)
                {
                    textBox_Communication_0x28.BackColor = Color.Green;
                }
                else
                {
                    textBox_Communication_0x28.BackColor = Color.Red;
                }
                flag_Communication_28 = false;

                if (flag_Communication_29)
                {
                    textBox_Communication_0x29.BackColor = Color.Green;
                }
                else
                {
                    textBox_Communication_0x29.BackColor = Color.Red;
                }
                flag_Communication_29 = false;

                if (flag_Communication_40)
                {
                    textBox_Communication_0x40.BackColor = Color.Green;
                }
                else
                {
                    textBox_Communication_0x40.BackColor = Color.Red;
                }
                flag_Communication_40 = false;

                if (flag_Communication_50)
                {
                    textBox_Communication_0x50.BackColor = Color.Green;
                }
                else
                {
                    textBox_Communication_0x50.BackColor = Color.Red;
                }
                flag_Communication_50 = false;

                if (flag_Communication_30)
                {
                    textBox_Communication_0x30.BackColor = Color.Green;
                }
                else
                {
                    textBox_Communication_0x30.BackColor = Color.Red;
                }
                flag_Communication_30 = false;

                if (flag_Communication_21)
                {
                    textBox_Communication_0x21.BackColor = Color.Green;
                }
                else
                {
                    textBox_Communication_0x21.BackColor = Color.Red;
                }
                flag_Communication_21 = false;

                if (flag_Communication_22)
                {
                    textBox_Communication_0x22.BackColor = Color.Green;
                }
                else
                {
                    textBox_Communication_0x22.BackColor = Color.Red;
                }
                flag_Communication_22 = false;

                if (flag_Communication_LuoPan)
                {
                    textBox_Communication_LuoPan.BackColor = Color.Green;
                }
                else
                {
                    textBox_Communication_LuoPan.BackColor = Color.Red;
                }
                flag_Communication_LuoPan = false;

                if (flag_Communication_FaXiang16)
                {
                    textBox_Communication_FaXiang16.BackColor = Color.Green;
                }
                else
                {
                    textBox_Communication_FaXiang16.BackColor = Color.Red;

                    #region 阀箱通信错误，将显示为绿色的阀箱相关按钮显示为黄色
                    //if (btn_ZuanJin_ZhengZhuan.BackColor == Color.Green)
                    //{
                    //    btn_ZuanJin_ZhengZhuan.BackColor = Color.Yellow;
                    //}
                    //if (btn_ZuanJin_FanZhuan.BackColor == Color.Green)
                    //{
                    //    btn_ZuanJin_FanZhuan.BackColor = Color.Yellow;
                    //}
                    //if (btn_ZuanJin_ZhengZhuanFanZhuan_Stop.BackColor == Color.Green)
                    //{
                    //    btn_ZuanJin_ZhengZhuanFanZhuan_Stop.BackColor = Color.Yellow;
                    //}

                    //if (btn_ZuanJinFa_GaoSu.BackColor == Color.Green)
                    //{
                    //    btn_ZuanJinFa_GaoSu.BackColor = Color.Yellow;
                    //}
                    //if (btn_ZuanJinFa_GaoSu_Stop.BackColor == Color.Green)
                    //{
                    //    btn_ZuanJinFa_GaoSu_Stop.BackColor = Color.Yellow;
                    //}

                    //if (btn_ZuanJin.BackColor == Color.Green)
                    //{
                    //    btn_ZuanJin.BackColor = Color.Yellow;
                    //}
                    //if (btn_ZuanJin_Step.BackColor == Color.Green)
                    //{
                    //    btn_ZuanJin_Step.BackColor = Color.Yellow;
                    //}
                    //if (btn_TiZuan.BackColor == Color.Green)
                    //{
                    //    btn_TiZuan.BackColor = Color.Yellow;
                    //}
                    //if (btn_TiZuan_Step.BackColor == Color.Green)
                    //{
                    //    btn_TiZuan_Step.BackColor = Color.Yellow;
                    //}
                    //if (btn_ZuanJinTiZuan_Stop.BackColor == Color.Green)
                    //{
                    //    btn_ZuanJinTiZuan_Stop.BackColor = Color.Yellow;
                    //}

                    //if (btn_JianYaFa_DiYa.BackColor == Color.Green)
                    //{
                    //    btn_JianYaFa_DiYa.BackColor = Color.Yellow;
                    //}
                    //if (btn_JianYaFa_ZhongYa.BackColor == Color.Green)
                    //{
                    //    btn_JianYaFa_ZhongYa.BackColor = Color.Yellow;
                    //}
                    //if (btn_JianYaFa_GaoYa.BackColor == Color.Green)
                    //{
                    //    btn_JianYaFa_GaoYa.BackColor = Color.Yellow;
                    //}
                    //if (btn_JianYaFa_Stop.BackColor == Color.Green)
                    //{
                    //    btn_JianYaFa_Stop.BackColor = Color.Yellow;
                    //}

                    //if (btn_YouYuanGaoYa_1.BackColor == Color.Green)
                    //{
                    //    btn_YouYuanGaoYa_1.BackColor = Color.Yellow;
                    //}
                    //if (btn_YouYuanGaoYa_Stop_1.BackColor == Color.Green)
                    //{
                    //    btn_YouYuanGaoYa_Stop_1.BackColor = Color.Yellow;
                    //}
                    //if (btn_YouYuanGaoYa.BackColor == Color.Green)
                    //{
                    //    btn_YouYuanGaoYa.BackColor = Color.Yellow;
                    //}
                    //if (btn_YouYuanGaoYa_Stop.BackColor == Color.Green)
                    //{
                    //    btn_YouYuanGaoYa_Stop.BackColor = Color.Yellow;
                    //}

                    //if (btn_ShuiBengKai.BackColor == Color.Green)
                    //{
                    //    btn_ShuiBengKai.BackColor = Color.Yellow;
                    //}
                    //if (btn_ShuiBengKai_Stop.BackColor == Color.Green)
                    //{
                    //    btn_ShuiBengKai_Stop.BackColor = Color.Yellow;
                    //}

                    //if (btn_YunTai_NiZhuan.BackColor == Color.Green)
                    //{
                    //    btn_YunTai_NiZhuan.BackColor = Color.Yellow;
                    //}
                    //if (btn_YunTai_ShunZhuan.BackColor == Color.Green)
                    //{
                    //    btn_YunTai_ShunZhuan.BackColor = Color.Yellow;
                    //}
                    //if (btn_YunTai_Yang.BackColor == Color.Green)
                    //{
                    //    btn_YunTai_Yang.BackColor = Color.Yellow;
                    //}
                    //if (btn_YunTai_Fu.BackColor == Color.Green)
                    //{
                    //    btn_YunTai_Fu.BackColor = Color.Yellow;
                    //}
                    // 5.12
                    //if (btn_A7.BackColor == Color.Green)
                    //{
                    //    btn_A7.BackColor = Color.Yellow;
                    //}
                    //if (btn_B7.BackColor == Color.Green)
                    //{
                    //    btn_B7.BackColor = Color.Yellow;
                    //}
                    //if (btn_AB7_Stop.BackColor == Color.Green)
                    //{
                    //    btn_AB7_Stop.BackColor = Color.Yellow;
                    //}

                    //if (btn_A8.BackColor == Color.Green)
                    //{
                    //    btn_A8.BackColor = Color.Yellow;
                    //}
                    //if (btn_B8.BackColor == Color.Green)
                    //{
                    //    btn_B8.BackColor = Color.Yellow;
                    //}
                    //if (btn_AB8_Stop.BackColor == Color.Green)
                    //{
                    //    btn_AB8_Stop.BackColor = Color.Yellow;
                    //}

                    //if (btn_A9.BackColor == Color.Green)
                    //{
                    //    btn_A9.BackColor = Color.Yellow;
                    //}
                    //if (btn_B9.BackColor == Color.Green)
                    //{
                    //    btn_B9.BackColor = Color.Yellow;
                    //}
                    //if (btn_AB9_Stop.BackColor == Color.Green)
                    //{
                    //    btn_AB9_Stop.BackColor = Color.Yellow;
                    //}

                    //if (btn_A10.BackColor == Color.Green)
                    //{
                    //    btn_A10.BackColor = Color.Yellow;
                    //}
                    //if (btn_B10.BackColor == Color.Green)
                    //{
                    //    btn_B10.BackColor = Color.Yellow;
                    //}
                    //if (btn_AB10_Stop.BackColor == Color.Green)
                    //{
                    //    btn_AB10_Stop.BackColor = Color.Yellow;
                    //}

                    //if (btn_A11.BackColor == Color.Green)
                    //{
                    //    btn_A11.BackColor = Color.Yellow;
                    //}
                    //if (btn_B11.BackColor == Color.Green)
                    //{
                    //    btn_B11.BackColor = Color.Yellow;
                    //}
                    //if (btn_AB11_Stop.BackColor == Color.Green)
                    //{
                    //    btn_AB11_Stop.BackColor = Color.Yellow;
                    //}

                    //#endregion
                }
                flag_Communication_FaXiang16 = false;


                #endregion
            }
            catch (Exception ex)
            { }
        }

        private void btn_HeadingCircleClear_Click(object sender, EventArgs e)
        {/*
            try
            {
                if (MessageBox.Show("圈数清零提示", "提示", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    iHeadingCircle = 0;
                    textBox_HeadingCircle.Text = iHeadingCircle.ToString();
                    gaugeControl2.SetPointerValue("Pointer2", 0);

                    textBox_HeadingCircle_2.Text = iHeadingCircle.ToString();
                    gaugeControl2.SetPointerValue("Pointer2", 0);
                }
            }
            catch (Exception ex)
            { }*/
        }



        //5.12 舱内备用电源继电器板

        //private void btn_InboardBackupPRB_24V1_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        Global.m_FormMobileDrillMonCtl.btn_Perform_Click(Global.m_FormMobileDrillMonCtl.btn_InboardBackupPRB_24V1, null);
        //    }
        //    catch (Exception ex)
        //    { }
        //}

        //private void btn_InboardBackupPRB_24V2_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        Global.m_FormMobileDrillMonCtl.btn_Perform_Click(Global.m_FormMobileDrillMonCtl.btn_InboardBackupPRB_24V2, null);
        //    }
        //    catch (Exception ex)
        //    { }
        //}

        //private void btn_InboardBackupPRB_24V3_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        Global.m_FormMobileDrillMonCtl.btn_Perform_Click(Global.m_FormMobileDrillMonCtl.btn_InboardBackupPRB_24V3, null);
        //    }
        //    catch (Exception ex)
        //    { }
        //}

        //private void btn_InboardBackupPRB_24V4_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        Global.m_FormMobileDrillMonCtl.btn_Perform_Click(Global.m_FormMobileDrillMonCtl.btn_InboardBackupPRB_24V4, null);
        //    }
        //    catch (Exception ex)
        //    { }
        //}

        //private void btn_InboardBackupPRB_Camera_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        Global.m_FormMobileDrillMonCtl.btn_Perform_Click(Global.m_FormMobileDrillMonCtl.btn_InboardBackupPRB_Camera, null);
        //    }
        //    catch (Exception ex)
        //    { }
        //}

        //private void btn_InboardBackupPRB_W_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        Global.m_FormMobileDrillMonCtl.btn_Perform_Click(Global.m_FormMobileDrillMonCtl.btn_InboardBackupPRB_W, null);
        //    }
        //    catch (Exception ex)
        //    { }
        //}

        //private void btn_InboardBackupPRB_12V1_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        Global.m_FormMobileDrillMonCtl.btn_Perform_Click(Global.m_FormMobileDrillMonCtl.btn_InboardBackupPRB_12V1, null);
        //    }
        //    catch (Exception ex)
        //    { }
        //}

        //private void btn_InboardBackupPRB_12V2_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        Global.m_FormMobileDrillMonCtl.btn_Perform_Click(Global.m_FormMobileDrillMonCtl.btn_InboardBackupPRB_12V2, null);
        //    }
        //    catch (Exception ex)
        //    { }
        //}

        private void btn_Camera_1_Click(object sender, EventArgs e)
        {
            try
            {
                Global.m_FormMobileDrillMonCtl.btn_Perform_Click(Global.m_FormMobileDrillMonCtl.btn_Camera_1, null);
            }
            catch (Exception ex)
            { }
        }

        private void btn_Camera_2_Click(object sender, EventArgs e)
        {
            try
            {
                Global.m_FormMobileDrillMonCtl.btn_Perform_Click(Global.m_FormMobileDrillMonCtl.btn_Camera_2, null);
            }
            catch (Exception ex)
            { }
        }

        private void btn_Camera_3_Click(object sender, EventArgs e)
        {
            try
            {
                Global.m_FormMobileDrillMonCtl.btn_Perform_Click(Global.m_FormMobileDrillMonCtl.btn_Camera_3, null);
            }
            catch (Exception ex)
            { }
        }

        private void btn_Camera_4_Click(object sender, EventArgs e)
        {
            try
            {
                Global.m_FormMobileDrillMonCtl.btn_Perform_Click(Global.m_FormMobileDrillMonCtl.btn_Camera_4, null);
            }
            catch (Exception ex)
            { }
        }

        private void btn_Camera_5_Click(object sender, EventArgs e)
        {
            try
            {
                Global.m_FormMobileDrillMonCtl.btn_Perform_Click(Global.m_FormMobileDrillMonCtl.btn_Camera_5, null);
            }
            catch (Exception ex)
            { }
        }

        private void btn_Camera_6_Click(object sender, EventArgs e)
        {
            try
            {
                Global.m_FormMobileDrillMonCtl.btn_Perform_Click(Global.m_FormMobileDrillMonCtl.btn_Camera_6, null);
            }
            catch (Exception ex)
            { }
        }

        private void btn_Camera_7_Click(object sender, EventArgs e)
        {
            try
            {
                Global.m_FormMobileDrillMonCtl.btn_Perform_Click(Global.m_FormMobileDrillMonCtl.btn_Camera_7, null);
            }
            catch (Exception ex)
            { }
        }

        private void btn_Camera_8_Click(object sender, EventArgs e)
        {
            try
            {
                Global.m_FormMobileDrillMonCtl.btn_Perform_Click(Global.m_FormMobileDrillMonCtl.btn_Camera_8, null);
            }
            catch (Exception ex)
            { }
        }

        private void btn_DetectPanel_Rotate_Click(object sender, EventArgs e)
        {
            try
            {
                Global.m_FormMobileDrillMonCtl.btn_Perform_Click(Global.m_FormMobileDrillMonCtl.btn_DetectPanel_Rotate, null);
            }
            catch (Exception ex)
            { }
        }

        private void btn_DetectPanel_Space_12V1_Click(object sender, EventArgs e)
        {
            try
            {
                Global.m_FormMobileDrillMonCtl.btn_Perform_Click(Global.m_FormMobileDrillMonCtl.btn_DetectPanel_Space_12V1, null);
            }
            catch (Exception ex)
            { }
        }

        private void btn_DetectPanel_Space_12V2_Click(object sender, EventArgs e)
        {
            try
            {
                Global.m_FormMobileDrillMonCtl.btn_Perform_Click(Global.m_FormMobileDrillMonCtl.btn_DetectPanel_Space_12V2, null);
            }
            catch (Exception ex)
            { }
        }

        private void btn_DetectPanel_Space_12V3_Click(object sender, EventArgs e)
        {
            try
            {
                Global.m_FormMobileDrillMonCtl.btn_Perform_Click(Global.m_FormMobileDrillMonCtl.btn_DetectPanel_Space_12V3, null);
            }
            catch (Exception ex)
            { }
        }

        private void btn_DetectPanel_Hight_Click(object sender, EventArgs e)
        {
            try
            {
                Global.m_FormMobileDrillMonCtl.btn_Perform_Click(Global.m_FormMobileDrillMonCtl.btn_DetectPanel_Hight, null);
            }
            catch (Exception ex)
            { }
        }

        private void btn_DetectPanel_Deep_Click(object sender, EventArgs e)
        {
            try
            {
                Global.m_FormMobileDrillMonCtl.btn_Perform_Click(Global.m_FormMobileDrillMonCtl.btn_DetectPanel_Deep, null);
            }
            catch (Exception ex)
            { }
        }

        private void btn_DetectPanel_Space_Bak1_Click(object sender, EventArgs e)
        {
            try
            {
                Global.m_FormMobileDrillMonCtl.btn_Perform_Click(Global.m_FormMobileDrillMonCtl.btn_DetectPanel_Space_Bak1, null);
            }
            catch (Exception ex)
            { }
        }

        private void btn_DetectPanel_Space_Bak2_Click(object sender, EventArgs e)
        {
            try
            {
                Global.m_FormMobileDrillMonCtl.btn_Perform_Click(Global.m_FormMobileDrillMonCtl.btn_DetectPanel_Space_Bak2, null);
            }
            catch (Exception ex)
            { }
        }

        private void btn_Light_1_Click(object sender, EventArgs e)
        {
            try
            {
                Global.m_FormMobileDrillMonCtl.btn_Perform_Click(Global.m_FormMobileDrillMonCtl.btn_Light_1, null);
            }
            catch (Exception ex)
            { }
        }

        private void btn_Light_2_Click(object sender, EventArgs e)
        {
            try
            {
                Global.m_FormMobileDrillMonCtl.btn_Perform_Click(Global.m_FormMobileDrillMonCtl.btn_Light_2, null);
            }
            catch (Exception ex)
            { }
        }

        private void btn_Light_3_Click(object sender, EventArgs e)
        {
            try
            {
                Global.m_FormMobileDrillMonCtl.btn_Perform_Click(Global.m_FormMobileDrillMonCtl.btn_Light_3, null);
            }
            catch (Exception ex)
            { }
        }

        private void btn_Light_4_Click(object sender, EventArgs e)
        {
            try
            {
                Global.m_FormMobileDrillMonCtl.btn_Perform_Click(Global.m_FormMobileDrillMonCtl.btn_Light_4, null);
            }
            catch (Exception ex)
            { }
        }

        private void btn_Light_5_Click(object sender, EventArgs e)
        {
            try
            {
                Global.m_FormMobileDrillMonCtl.btn_Perform_Click(Global.m_FormMobileDrillMonCtl.btn_Light_5, null);
            }
            catch (Exception ex)
            { }
        }

        private void btn_Light_6_Click(object sender, EventArgs e)
        {
            try
            {
                Global.m_FormMobileDrillMonCtl.btn_Perform_Click(Global.m_FormMobileDrillMonCtl.btn_Light_6, null);
            }
            catch (Exception ex)
            { }
        }

        private void btn_Light_7_Click(object sender, EventArgs e)
        {
            try
            {
                Global.m_FormMobileDrillMonCtl.btn_Perform_Click(Global.m_FormMobileDrillMonCtl.btn_Light_7, null);
            }
            catch (Exception ex)
            { }
        }

        private void btn_Light_8_Click(object sender, EventArgs e)
        {
            try
            {
                Global.m_FormMobileDrillMonCtl.btn_Perform_Click(Global.m_FormMobileDrillMonCtl.btn_Light_8, null);
            }
            catch (Exception ex)
            { }
        }


        private void trackBar_Light_1_4_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                int v1 = trackBar_Light_1.Value;
                int v2 = trackBar_Light_2.Value;
                int v3 = trackBar_Light_3.Value;
                int v4 = trackBar_Light_4.Value;
                Global.m_FormMobileDrillMonCtl.trackBar_Light_1.Value = v1;
                Global.m_FormMobileDrillMonCtl.trackBar_Light_2.Value = v2;
                Global.m_FormMobileDrillMonCtl.trackBar_Light_3.Value = v3;
                Global.m_FormMobileDrillMonCtl.trackBar_Light_4.Value = v4;
                Global.m_FormMobileDrillMonCtl.btn_Perform_Click(Global.m_FormMobileDrillMonCtl.btn_Light_DensitySet_23, null);
            }
            catch (Exception ex)
            { }
        }



        private void trackBar_Light_5_8_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                int v5 = trackBar_Light_5.Value;
                int v6 = trackBar_Light_6.Value;
                int v7 = trackBar_Light_7.Value;
                int v8 = trackBar_Light_8.Value;
                Global.m_FormMobileDrillMonCtl.trackBar_Light_5.Value = v5;
                Global.m_FormMobileDrillMonCtl.trackBar_Light_6.Value = v6;
                Global.m_FormMobileDrillMonCtl.trackBar_Light_7.Value = v7;
                Global.m_FormMobileDrillMonCtl.trackBar_Light_8.Value = v8;
                Global.m_FormMobileDrillMonCtl.btn_Perform_Click(Global.m_FormMobileDrillMonCtl.btn_Light_DensitySet_24, null);
            }
            catch (Exception ex)
            { }
        }


        private void btn_FaBox_72_Click(object sender, EventArgs e)
        {
            try
            {
                Global.m_FormMobileDrillMonCtl.btn_Perform_Click(Global.m_FormMobileDrillMonCtl.btn_FaBox_72, null);
            }
            catch (Exception ex)
            { }
        }

        private void btn_FaBox_Space_Click(object sender, EventArgs e)
        {
            try
            {
                Global.m_FormMobileDrillMonCtl.btn_Perform_Click(Global.m_FormMobileDrillMonCtl.btn_FaBox_Space, null);
            }
            catch (Exception ex)
            { }
        }

        /**
         * 功能阀箱功能按钮
         */
        private Enum_UserButtonOperIdentify myEnum_UserButtonOperIdentify = Enum_UserButtonOperIdentify.Others;
        //钻进
        private void btn_ZuanJin_Click(object sender, EventArgs e)
        {
            try
            {
                if (Global.m_FormBoardII.IsConnected == false)
                {
                    string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "16功能阀箱网络未连接，操作取消！";
                    sInfo += "\t\n";
                    richTextBox_InfoShow.AppendText(sInfo);
                    return;
                }

                if (flagCmdToSend == true)
                {
                    string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "尚有操作未完成，请稍后再操作！";
                    sInfo += "\t\n";
                    richTextBox_InfoShow.AppendText(sInfo);

                    return;
                }

                flagCmdToSend = true;
                myEnum_UserButtonOperIdentify = Enum_UserButtonOperIdentify.ZuanJin;

                //操作记录
                string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t钻进\t";
                Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                Global.myLogStreamWriter.Flush();

                {
                    string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "执行\"钻进\"操作";
                    sInfo += "\t\n";
                    richTextBox_InfoShow.AppendText(sInfo);
                }
            }
            catch (Exception ex)
            { }
        }

        //点动钻进
        private void btn_ZuanJin_Step_Click(object sender, EventArgs e)
        {
            try
            {
                if (Global.m_FormBoardII.IsConnected == false)
                {
                    string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "16功能阀箱网络未连接，操作取消！";
                    sInfo += "\t\n";
                    richTextBox_InfoShow.AppendText(sInfo);
                    return;
                }

                if (flagCmdToSend == true)
                {
                    string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "尚有操作未完成，请稍后再操作！";
                    sInfo += "\t\n";
                    richTextBox_InfoShow.AppendText(sInfo);

                    return;
                }

                flagCmdToSend = true;
                myEnum_UserButtonOperIdentify = Enum_UserButtonOperIdentify.ZuanJin_Step;

                //操作记录
                string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t点动钻进\t";
                Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                Global.myLogStreamWriter.Flush();

                {
                    string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "执行\"点动钻进\"操作";
                    sInfo += "\t\n";
                    richTextBox_InfoShow.AppendText(sInfo);
                }
            }
            catch (Exception ex)
            { }
        }

        //提钻
        private void btn_TiZuan_Click(object sender, EventArgs e)
        {
            try
            {
                if (Global.m_FormBoardII.IsConnected == false)
                {
                    string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "16功能阀箱网络未连接，操作取消！";
                    sInfo += "\t\n";
                    richTextBox_InfoShow.AppendText(sInfo);
                    return;
                }

                if (flagCmdToSend == true)
                {
                    string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "尚有操作未完成，请稍后再操作！";
                    sInfo += "\t\n";
                    richTextBox_InfoShow.AppendText(sInfo);

                    return;
                }

                flagCmdToSend = true;
                myEnum_UserButtonOperIdentify = Enum_UserButtonOperIdentify.TiZuan;

                //操作记录
                string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t提钻\t";
                Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                Global.myLogStreamWriter.Flush();

                {
                    string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "执行\"提钻\"操作";
                    sInfo += "\t\n";
                    richTextBox_InfoShow.AppendText(sInfo);
                }
            }
            catch (Exception ex)
            { }
        }

        //点动提升
        private void btn_TiZuan_Step_Click(object sender, EventArgs e)
        {
            try
            {
                if (Global.m_FormBoardII.IsConnected == false)
                {
                    string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "16功能阀箱网络未连接，操作取消！";
                    sInfo += "\t\n";
                    richTextBox_InfoShow.AppendText(sInfo);
                    return;
                }

                if (flagCmdToSend == true)
                {
                    string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "尚有操作未完成，请稍后再操作！";
                    sInfo += "\t\n";
                    richTextBox_InfoShow.AppendText(sInfo);

                    return;
                }

                flagCmdToSend = true;
                myEnum_UserButtonOperIdentify = Enum_UserButtonOperIdentify.TiZuan_Step;

                //操作记录
                string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t点动提升\t";
                Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                Global.myLogStreamWriter.Flush();

                {
                    string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "执行\"点动提升\"操作";
                    sInfo += "\t\n";
                    richTextBox_InfoShow.AppendText(sInfo);
                }
            }
            catch (Exception ex)
            { }
        }

        //钻进、提钻，均停止
        private void btn_ZuanJinTiZuan_Stop_Click(object sender, EventArgs e)
        {
            try
            {
                if (Global.m_FormBoardII.IsConnected == false)
                {
                    string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "16功能阀箱网络未连接，操作取消！";
                    sInfo += "\t\n";
                    richTextBox_InfoShow.AppendText(sInfo);
                    return;
                }

                if (flagCmdToSend == true)
                {
                    string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "尚有操作未完成，请稍后再操作！";
                    sInfo += "\t\n";
                    richTextBox_InfoShow.AppendText(sInfo);

                    return;
                }

                flagCmdToSend = true;
                myEnum_UserButtonOperIdentify = Enum_UserButtonOperIdentify.ZuanJin_TiZuan_Stop;

                //操作记录
                string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t钻进提钻均停止\t";
                Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                Global.myLogStreamWriter.Flush();

                {
                    string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "执行\"钻进、提钻，均停止\"操作";
                    sInfo += "\t\n";
                    richTextBox_InfoShow.AppendText(sInfo);
                }
            }
            catch (Exception ex)
            { }
        }

        //钻进阀正转
        private void btn_ZuanJin_ZhengZhuan_Click(object sender, EventArgs e)
        {
            try
            {
                if (Global.m_FormBoardII.IsConnected == false)
                {
                    string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "16功能阀箱网络未连接，操作取消！";
                    sInfo += "\t\n";
                    richTextBox_InfoShow.AppendText(sInfo);
                    return;
                }

                if (flagCmdToSend == true)
                {
                    string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "尚有操作未完成，请稍后再操作！";
                    sInfo += "\t\n";
                    richTextBox_InfoShow.AppendText(sInfo);

                    return;
                }

                flagCmdToSend = true;
                myEnum_UserButtonOperIdentify = Enum_UserButtonOperIdentify.ZuanJinFa_ZhengZhuan;

                //操作记录
                string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t钻进阀正转\t";
                Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                Global.myLogStreamWriter.Flush();

                {
                    string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "执行\"钻进阀正转\"操作";
                    sInfo += "\t\n";
                    richTextBox_InfoShow.AppendText(sInfo);
                }
            }
            catch (Exception ex)
            { }
        }

        //钻进阀反转
        private void btn_ZuanJin_FanZhuan_Click(object sender, EventArgs e)
        {
            try
            {
                if (Global.m_FormBoardII.IsConnected == false)
                {
                    string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "16功能阀箱网络未连接，操作取消！";
                    sInfo += "\t\n";
                    richTextBox_InfoShow.AppendText(sInfo);
                    return;
                }

                if (flagCmdToSend == true)
                {
                    string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "尚有操作未完成，请稍后再操作！";
                    sInfo += "\t\n";
                    richTextBox_InfoShow.AppendText(sInfo);

                    return;
                }

                flagCmdToSend = true;
                myEnum_UserButtonOperIdentify = Enum_UserButtonOperIdentify.ZuanJinFa_FanZhuan;

                //操作记录
                string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t钻进阀反转\t";
                Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                Global.myLogStreamWriter.Flush();

                {
                    string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "执行\"钻进阀反转\"操作";
                    sInfo += "\t\n";
                    richTextBox_InfoShow.AppendText(sInfo);
                }
            }
            catch (Exception ex)
            { }
        }

        //钻进阀正反转均停止
        private void btn_ZuanJin_ZhengZhuanFanZhuan_Stop_Click(object sender, EventArgs e)
        {
            try
            {
                if (Global.m_FormBoardII.IsConnected == false)
                {
                    string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "16功能阀箱网络未连接，操作取消！";
                    sInfo += "\t\n";
                    richTextBox_InfoShow.AppendText(sInfo);
                    return;
                }

                if (flagCmdToSend == true)
                {
                    string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "尚有操作未完成，请稍后再操作！";
                    sInfo += "\t\n";
                    richTextBox_InfoShow.AppendText(sInfo);

                    return;
                }

                flagCmdToSend = true;
                myEnum_UserButtonOperIdentify = Enum_UserButtonOperIdentify.ZuanJinFa_ZhengZhuan_FanZhuan_Stop;

                //操作记录
                string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t钻进阀正反转均停止\t";
                Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                Global.myLogStreamWriter.Flush();

                {
                    string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "执行\"钻进阀正反转均停止\"操作";
                    sInfo += "\t\n";
                    richTextBox_InfoShow.AppendText(sInfo);
                }
            }
            catch (Exception ex)
            { }
        }

        //云台顺转
        private void btn_YunTai_ShunZhuan_Click(object sender, EventArgs e)
        {
            try
            {
                if (Global.m_FormBoardII.IsConnected == false)
                {
                    string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "16功能阀箱网络未连接，操作取消！";
                    sInfo += "\t\n";
                    richTextBox_InfoShow.AppendText(sInfo);
                    return;
                }

                if (flagCmdToSend == true)
                {
                    string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "尚有操作未完成，请稍后再操作！";
                    sInfo += "\t\n";
                    richTextBox_InfoShow.AppendText(sInfo);

                    return;
                }

                flagCmdToSend = true;
                myEnum_UserButtonOperIdentify = Enum_UserButtonOperIdentify.YunTai_ShunZhuan;

                //操作记录
                string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t云台顺转\t";
                Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                Global.myLogStreamWriter.Flush();

                {
                    string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "执行\"云台顺转\"操作";
                    sInfo += "\t\n";
                    richTextBox_InfoShow.AppendText(sInfo);
                }
            }
            catch (Exception ex)
            { }
        }

        //云台逆转
        private void btn_YunTai_NiZhuan_Click(object sender, EventArgs e)
        {
            try
            {
                if (Global.m_FormBoardII.IsConnected == false)
                {
                    string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "16功能阀箱网络未连接，操作取消！";
                    sInfo += "\t\n";
                    richTextBox_InfoShow.AppendText(sInfo);
                    return;
                }

                if (flagCmdToSend == true)
                {
                    string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "尚有操作未完成，请稍后再操作！";
                    sInfo += "\t\n";
                    richTextBox_InfoShow.AppendText(sInfo);

                    return;
                }

                flagCmdToSend = true;
                myEnum_UserButtonOperIdentify = Enum_UserButtonOperIdentify.YunTai_NiZhuan;

                //操作记录
                string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t云台逆转\t";
                Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                Global.myLogStreamWriter.Flush();

                {
                    string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "执行\"云台逆转\"操作";
                    sInfo += "\t\n";
                    richTextBox_InfoShow.AppendText(sInfo);
                }
            }
            catch (Exception ex)
            { }
        }

        //云台顺转逆转均停止
        private void btn_YunTai_ShunZhuan_NiZhuan_Stop_Click(object sender, EventArgs e)
        {
            try
            {
                if (Global.m_FormBoardII.IsConnected == false)
                {
                    string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "16功能阀箱网络未连接，操作取消！";
                    sInfo += "\t\n";
                    richTextBox_InfoShow.AppendText(sInfo);
                    return;
                }

                if (flagCmdToSend == true)
                {
                    string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "尚有操作未完成，请稍后再操作！";
                    sInfo += "\t\n";
                    richTextBox_InfoShow.AppendText(sInfo);

                    return;
                }

                flagCmdToSend = true;
                myEnum_UserButtonOperIdentify = Enum_UserButtonOperIdentify.YunTai_ShunZhuan_NiZhuan_Stop;

                //操作记录
                string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t云台顺转逆转均停止\t";
                Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                Global.myLogStreamWriter.Flush();

                {
                    string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "执行\"云台顺转逆转均停止\"操作";
                    sInfo += "\t\n";
                    richTextBox_InfoShow.AppendText(sInfo);
                }
            }
            catch (Exception ex)
            { }
        }

        //云台俯
        private void btn_YunTai_Fu_Click(object sender, EventArgs e)
        {
            try
            {
                if (Global.m_FormBoardII.IsConnected == false)
                {
                    string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "16功能阀箱网络未连接，操作取消！";
                    sInfo += "\t\n";
                    richTextBox_InfoShow.AppendText(sInfo);
                    return;
                }

                if (flagCmdToSend == true)
                {
                    string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "尚有操作未完成，请稍后再操作！";
                    sInfo += "\t\n";
                    richTextBox_InfoShow.AppendText(sInfo);

                    return;
                }

                flagCmdToSend = true;
                myEnum_UserButtonOperIdentify = Enum_UserButtonOperIdentify.YunTai_Fu;

                //操作记录
                string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t云台俯\t";
                Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                Global.myLogStreamWriter.Flush();

                {
                    string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "执行\"云台俯\"操作";
                    sInfo += "\t\n";
                    richTextBox_InfoShow.AppendText(sInfo);
                }
            }
            catch (Exception ex)
            { }
        }

        //云台仰
        private void btn_YunTai_Yang_Click(object sender, EventArgs e)
        {
            try
            {
                if (Global.m_FormBoardII.IsConnected == false)
                {
                    string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "16功能阀箱网络未连接，操作取消！";
                    sInfo += "\t\n";
                    richTextBox_InfoShow.AppendText(sInfo);
                    return;
                }

                if (flagCmdToSend == true)
                {
                    string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "尚有操作未完成，请稍后再操作！";
                    sInfo += "\t\n";
                    richTextBox_InfoShow.AppendText(sInfo);

                    return;
                }

                flagCmdToSend = true;
                myEnum_UserButtonOperIdentify = Enum_UserButtonOperIdentify.YunTai_Yang;

                //操作记录
                string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t云台仰\t";
                Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                Global.myLogStreamWriter.Flush();

                {
                    string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "执行\"云台仰\"操作";
                    sInfo += "\t\n";
                    richTextBox_InfoShow.AppendText(sInfo);
                }
            }
            catch (Exception ex)
            { }
        }

        //云台俯仰停止
        private void btn_YunTai_FuYang_Stop_Click(object sender, EventArgs e)
        {
            try
            {
                if (Global.m_FormBoardII.IsConnected == false)
                {
                    string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "16功能阀箱网络未连接，操作取消！";
                    sInfo += "\t\n";
                    richTextBox_InfoShow.AppendText(sInfo);
                    return;
                }

                if (flagCmdToSend == true)
                {
                    string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "尚有操作未完成，请稍后再操作！";
                    sInfo += "\t\n";
                    richTextBox_InfoShow.AppendText(sInfo);

                    return;
                }

                flagCmdToSend = true;
                myEnum_UserButtonOperIdentify = Enum_UserButtonOperIdentify.YunTai_FuYang_Stop;

                //操作记录
                string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t云台俯仰停止\t";
                Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                Global.myLogStreamWriter.Flush();

                {
                    string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "执行\"云台俯仰停止\"操作";
                    sInfo += "\t\n";
                    richTextBox_InfoShow.AppendText(sInfo);
                }
            }
            catch (Exception ex)
            { }
        }

        //油源高压1
        private void btn_YouYuanGaoYa_1_Click(object sender, EventArgs e)
        {
            try
            {
                if (Global.m_FormBoardII.IsConnected == false)
                {
                    string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "16功能阀箱网络未连接，操作取消！";
                    sInfo += "\t\n";
                    richTextBox_InfoShow.AppendText(sInfo);
                    return;
                }

                if (flagCmdToSend == true)
                {
                    string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "尚有操作未完成，请稍后再操作！";
                    sInfo += "\t\n";
                    richTextBox_InfoShow.AppendText(sInfo);

                    return;
                }

                flagCmdToSend = true;
                myEnum_UserButtonOperIdentify = Enum_UserButtonOperIdentify.YouYuanGaoYa_1;

                //操作记录
                string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t油源高压1\t";
                Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                Global.myLogStreamWriter.Flush();

                {
                    string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "执行\"油源高压1\"操作";
                    sInfo += "\t\n";
                    richTextBox_InfoShow.AppendText(sInfo);
                }
            }
            catch (Exception ex)
            { }
        }

        //油源高压停止1
        private void btn_YouYuanGaoYa_Stop_1_Click(object sender, EventArgs e)
        {
            try
            {
                if (Global.m_FormBoardII.IsConnected == false)
                {
                    string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "16功能阀箱网络未连接，操作取消！";
                    sInfo += "\t\n";
                    richTextBox_InfoShow.AppendText(sInfo);
                    return;
                }

                if (flagCmdToSend == true)
                {
                    string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "尚有操作未完成，请稍后再操作！";
                    sInfo += "\t\n";
                    richTextBox_InfoShow.AppendText(sInfo);

                    return;
                }

                flagCmdToSend = true;
                myEnum_UserButtonOperIdentify = Enum_UserButtonOperIdentify.YouYuanGaoYa_Stop_1;

                //操作记录
                string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t油源高压停止1\t";
                Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                Global.myLogStreamWriter.Flush();

                {
                    string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "执行\"油源高压停止1\"操作";
                    sInfo += "\t\n";
                    richTextBox_InfoShow.AppendText(sInfo);
                }
            }
            catch (Exception ex)
            { }
        }

        //油源高压2
        private void btn_YouYuanGaoYa_Click(object sender, EventArgs e)
        {
            try
            {
                if (Global.m_FormBoardII.IsConnected == false)
                {
                    string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "16功能阀箱网络未连接，操作取消！";
                    sInfo += "\t\n";
                    richTextBox_InfoShow.AppendText(sInfo);
                    return;
                }

                if (flagCmdToSend == true)
                {
                    string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "尚有操作未完成，请稍后再操作！";
                    sInfo += "\t\n";
                    richTextBox_InfoShow.AppendText(sInfo);

                    return;
                }

                flagCmdToSend = true;
                myEnum_UserButtonOperIdentify = Enum_UserButtonOperIdentify.YouYuanGaoYa;

                //操作记录
                string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t油源高压2\t";
                Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                Global.myLogStreamWriter.Flush();

                {
                    string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "执行\"油源高压2\"操作";
                    sInfo += "\t\n";
                    richTextBox_InfoShow.AppendText(sInfo);
                }
            }
            catch (Exception ex)
            { }
        }

        //油源高压停止2
        private void btn_YouYuanGaoYa_Stop_Click(object sender, EventArgs e)
        {
            try
            {
                if (Global.m_FormBoardII.IsConnected == false)
                {
                    string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "16功能阀箱网络未连接，操作取消！";
                    sInfo += "\t\n";
                    richTextBox_InfoShow.AppendText(sInfo);
                    return;
                }

                if (flagCmdToSend == true)
                {
                    string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "尚有操作未完成，请稍后再操作！";
                    sInfo += "\t\n";
                    richTextBox_InfoShow.AppendText(sInfo);

                    return;
                }

                flagCmdToSend = true;
                myEnum_UserButtonOperIdentify = Enum_UserButtonOperIdentify.YouYuanGaoYa_Stop;

                //操作记录
                string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t油源高压停止1\t";
                Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                Global.myLogStreamWriter.Flush();

                {
                    string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "执行\"油源高压停止1\"操作";
                    sInfo += "\t\n";
                    richTextBox_InfoShow.AppendText(sInfo);
                }
            }
            catch (Exception ex)
            { }
        }

        //水泵开
        private void btn_ShuiBengKai_Click(object sender, EventArgs e)
        {
            try
            {
                if (Global.m_FormBoardII.IsConnected == false)
                {
                    string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "16功能阀箱网络未连接，操作取消！";
                    sInfo += "\t\n";
                    richTextBox_InfoShow.AppendText(sInfo);
                    return;
                }

                if (flagCmdToSend == true)
                {
                    string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "尚有操作未完成，请稍后再操作！";
                    sInfo += "\t\n";
                    richTextBox_InfoShow.AppendText(sInfo);

                    return;
                }

                flagCmdToSend = true;
                myEnum_UserButtonOperIdentify = Enum_UserButtonOperIdentify.ShuiBengKai;

                //操作记录
                string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t水泵开\t";
                Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                Global.myLogStreamWriter.Flush();

                {
                    string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "执行\"水泵开\"操作";
                    sInfo += "\t\n";
                    richTextBox_InfoShow.AppendText(sInfo);
                }
            }
            catch (Exception ex)
            { }
        }

        //水泵开停止
        private void btn_ShuiBengKai_Stop_Click(object sender, EventArgs e)
        {
            try
            {
                if (Global.m_FormBoardII.IsConnected == false)
                {
                    string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "16功能阀箱网络未连接，操作取消！";
                    sInfo += "\t\n";
                    richTextBox_InfoShow.AppendText(sInfo);
                    return;
                }

                if (flagCmdToSend == true)
                {
                    string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "尚有操作未完成，请稍后再操作！";
                    sInfo += "\t\n";
                    richTextBox_InfoShow.AppendText(sInfo);

                    return;
                }

                flagCmdToSend = true;
                myEnum_UserButtonOperIdentify = Enum_UserButtonOperIdentify.ShuiBengKai_Stop;

                //操作记录
                string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t水泵开停止\t";
                Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                Global.myLogStreamWriter.Flush();

                {
                    string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "执行\"水泵开停止\"操作";
                    sInfo += "\t\n";
                    richTextBox_InfoShow.AppendText(sInfo);
                }
            }
            catch (Exception ex)
            { }
        }

        //减压阀低压
        private void btn_JianYaFa_DiYa_Click(object sender, EventArgs e)
        {
            try
            {
                if (Global.m_FormBoardII.IsConnected == false)
                {
                    string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "16功能阀箱网络未连接，操作取消！";
                    sInfo += "\t\n";
                    richTextBox_InfoShow.AppendText(sInfo);
                    return;
                }

                if (flagCmdToSend == true)
                {
                    string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "尚有操作未完成，请稍后再操作！";
                    sInfo += "\t\n";
                    richTextBox_InfoShow.AppendText(sInfo);

                    return;
                }

                flagCmdToSend = true;
                myEnum_UserButtonOperIdentify = Enum_UserButtonOperIdentify.JianYaFa_DiYa;

                //操作记录
                string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t减压阀低压\t";
                Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                Global.myLogStreamWriter.Flush();

                {
                    string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "执行\"减压阀高压\"操作";
                    sInfo += "\t\n";
                    richTextBox_InfoShow.AppendText(sInfo);
                }
            }
            catch (Exception ex)
            { }
        }

        //减压阀中压
        private void btn_JianYaFa_ZhongYa_Click(object sender, EventArgs e)
        {
            try
            {
                if (Global.m_FormBoardII.IsConnected == false)
                {
                    string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "16功能阀箱网络未连接，操作取消！";
                    sInfo += "\t\n";
                    richTextBox_InfoShow.AppendText(sInfo);
                    return;
                }

                if (flagCmdToSend == true)
                {
                    string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "尚有操作未完成，请稍后再操作！";
                    sInfo += "\t\n";
                    richTextBox_InfoShow.AppendText(sInfo);

                    return;
                }

                flagCmdToSend = true;
                myEnum_UserButtonOperIdentify = Enum_UserButtonOperIdentify.JianYaFa_ZhongYa;

                //操作记录
                string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t减压阀中压\t";
                Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                Global.myLogStreamWriter.Flush();

                {
                    string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "执行\"减压阀高压\"操作";
                    sInfo += "\t\n";
                    richTextBox_InfoShow.AppendText(sInfo);
                }
            }
            catch (Exception ex)
            { }
        }

        //减压阀高压
        private void btn_JianYaFa_GaoYa_Click(object sender, EventArgs e)
        {
            try
            {
                if (Global.m_FormBoardII.IsConnected == false)
                {
                    string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "16功能阀箱网络未连接，操作取消！";
                    sInfo += "\t\n";
                    richTextBox_InfoShow.AppendText(sInfo);
                    return;
                }

                if (flagCmdToSend == true)
                {
                    string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "尚有操作未完成，请稍后再操作！";
                    sInfo += "\t\n";
                    richTextBox_InfoShow.AppendText(sInfo);

                    return;
                }

                flagCmdToSend = true;
                myEnum_UserButtonOperIdentify = Enum_UserButtonOperIdentify.JianYaFa_GaoYa;

                //操作记录
                string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t减压阀高压\t";
                Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                Global.myLogStreamWriter.Flush();

                {
                    string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "执行\"减压阀高压\"操作";
                    sInfo += "\t\n";
                    richTextBox_InfoShow.AppendText(sInfo);
                }
            }
            catch (Exception ex)
            { }
        }

        //减压阀低、中高压停止
        private void btn_JianYaFa_Stop_Click(object sender, EventArgs e)
        {
            try
            {
                if (Global.m_FormBoardII.IsConnected == false)
                {
                    string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "16功能阀箱网络未连接，操作取消！";
                    sInfo += "\t\n";
                    richTextBox_InfoShow.AppendText(sInfo);
                    return;
                }

                if (flagCmdToSend == true)
                {
                    string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "尚有操作未完成，请稍后再操作！";
                    sInfo += "\t\n";
                    richTextBox_InfoShow.AppendText(sInfo);

                    return;
                }

                flagCmdToSend = true;
                myEnum_UserButtonOperIdentify = Enum_UserButtonOperIdentify.JianYaFa_DiYa_ZhongYa_GaoYa_Stop;

                //操作记录
                string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t减压阀低、中高压停止\t";
                Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                Global.myLogStreamWriter.Flush();

                {
                    string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "执行\"减压阀低、中高压停止\"操作";
                    sInfo += "\t\n";
                    richTextBox_InfoShow.AppendText(sInfo);
                }
            }
            catch (Exception ex)
            { }
        }

        //钻进高速
        private void btn_ZuanJinFa_GaoSu_Click(object sender, EventArgs e)
        {
            try
            {
                if (Global.m_FormBoardII.IsConnected == false)
                {
                    string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "16功能阀箱网络未连接，操作取消！";
                    sInfo += "\t\n";
                    richTextBox_InfoShow.AppendText(sInfo);
                    return;
                }

                if (flagCmdToSend == true)
                {
                    string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "尚有操作未完成，请稍后再操作！";
                    sInfo += "\t\n";
                    richTextBox_InfoShow.AppendText(sInfo);

                    return;
                }

                flagCmdToSend = true;
                myEnum_UserButtonOperIdentify = Enum_UserButtonOperIdentify.ZuanJinGaoSu;

                //操作记录
                string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t钻进高速\t";
                Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                Global.myLogStreamWriter.Flush();

                {
                    string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "执行\"钻进高速\"操作";
                    sInfo += "\t\n";
                    richTextBox_InfoShow.AppendText(sInfo);
                }
            }
            catch (Exception ex)
            { }
        }

        //钻进高速停止
        private void btn_ZuanJinFa_GaoSu_Stop_Click(object sender, EventArgs e)
        {
            try
            {
                if (Global.m_FormBoardII.IsConnected == false)
                {
                    string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "16功能阀箱网络未连接，操作取消！";
                    sInfo += "\t\n";
                    richTextBox_InfoShow.AppendText(sInfo);
                    return;
                }

                if (flagCmdToSend == true)
                {
                    string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "尚有操作未完成，请稍后再操作！";
                    sInfo += "\t\n";
                    richTextBox_InfoShow.AppendText(sInfo);

                    return;
                }

                flagCmdToSend = true;
                myEnum_UserButtonOperIdentify = Enum_UserButtonOperIdentify.ZuanJinGaoSu_Stop;

                //操作记录
                string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t钻进高速停止\t";
                Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
                Global.myLogStreamWriter.Flush();

                {
                    string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "执行\"钻进高速停止\"操作";
                    sInfo += "\t\n";
                    richTextBox_InfoShow.AppendText(sInfo);
                }
            }
            catch (Exception ex)
            { }
        }




        private void btn_WaterCtlBox_SerialOpen_Click(object sender, EventArgs e)
        {
            try
            {
                Global.m_FormSerialWaterBoxCtl.comboBox_SerialPort.SelectedIndex = this.comboBox_WaterCtlBox_SerialPort.SelectedIndex;
                Global.m_FormSerialWaterBoxCtl.btn_Perform_Click(Global.m_FormSerialWaterBoxCtl.btn_SerialOpen, null);

                if (Global.isSerialOpenedOK_WaterCtlBox)
                {
                    btn_WaterCtlBox_SerialOpen.BackColor = Color.Green;
                    btn_WaterCtlBox_SerialClose.BackColor = Color.Transparent;
                    {
                        string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "水面控制盒串口打开成功！";
                        sInfo += "\t\n";
                        richTextBox_InfoShow.AppendText(sInfo);
                    }
                }
                else
                {
                    btn_WaterCtlBox_SerialOpen.BackColor = Color.Yellow;
                    btn_WaterCtlBox_SerialClose.BackColor = Color.Transparent;
                    {
                        string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "水面控制盒串口打开失败！";
                        sInfo += "\t\n";
                        richTextBox_InfoShow.AppendText(sInfo);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void btn_WaterCtlBox_SerialClose_Click(object sender, EventArgs e)
        {
            try
            {
                Global.m_FormSerialWaterBoxCtl.btn_Perform_Click(Global.m_FormSerialWaterBoxCtl.btn_SerialClose, null);

                btn_WaterCtlBox_SerialOpen.BackColor = Color.Transparent;
                btn_WaterCtlBox_SerialClose.BackColor = Color.Green;
                {
                    string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "水面控制盒串口关闭成功！";
                    sInfo += "\t\n";
                    richTextBox_InfoShow.AppendText(sInfo);
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void btn_ROVPower_SerialOpen_Click(object sender, EventArgs e)
        {
            try
            {
                Global.m_FormSerialRovPowerCtl.comboBox_SerialPort.SelectedIndex = this.comboBox_ROVPower_SerialPort.SelectedIndex;
                Global.m_FormSerialRovPowerCtl.btn_Perform_Click(Global.m_FormSerialRovPowerCtl.btn_SerialOpen, null);

                if (Global.isSerialOpenedOK_ROVPower)
                {
                    btn_ROVPower_SerialOpen.BackColor = Color.Green;
                    btn_ROVPower_SerialClose.BackColor = Color.Transparent;
                    {
                        string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "ROV供电柜串口打开成功！";
                        sInfo += "\t\n";
                        richTextBox_InfoShow.AppendText(sInfo);
                    }
                }
                else
                {
                    btn_ROVPower_SerialOpen.BackColor = Color.Yellow;
                    btn_ROVPower_SerialClose.BackColor = Color.Transparent;
                    {
                        string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "ROV供电柜串口打开失败！";
                        sInfo += "\t\n";
                        richTextBox_InfoShow.AppendText(sInfo);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void btn_ROVPower_SerialClose_Click(object sender, EventArgs e)
        {
            try
            {
                Global.m_FormSerialRovPowerCtl.btn_Perform_Click(Global.m_FormSerialRovPowerCtl.btn_SerialClose, null);

                btn_ROVPower_SerialOpen.BackColor = Color.Transparent;
                btn_ROVPower_SerialClose.BackColor = Color.Green;
                {
                    string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "ROV供电柜串口关闭成功！";
                    sInfo += "\t\n";
                    richTextBox_InfoShow.AppendText(sInfo);
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void btn_JuYuanJianCe_1_SerialOpen_Click(object sender, EventArgs e)
        {
            try
            {
                Global.m_FormSerialJuYuanJianCe1Ctl.comboBox_SerialPort.SelectedIndex = this.comboBox_JuYuanJianCe_1_SerialPort.SelectedIndex;
                Global.m_FormSerialJuYuanJianCe1Ctl.btn_Perform_Click(Global.m_FormSerialJuYuanJianCe1Ctl.btn_SerialOpen, null);

                if (Global.isSerialOpenedOK_JuYuanJianCe_1)
                {
                    btn_JuYuanJianCe_1_SerialOpen.BackColor = Color.Green;
                    btn_JuYuanJianCe_1_SerialClose.BackColor = Color.Transparent;
                    {
                        string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "绝缘检测仪1串口打开成功！";
                        sInfo += "\t\n";
                        richTextBox_InfoShow.AppendText(sInfo);
                    }
                }
                else
                {
                    btn_JuYuanJianCe_1_SerialOpen.BackColor = Color.Yellow;
                    btn_JuYuanJianCe_1_SerialClose.BackColor = Color.Transparent;
                    {
                        string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "绝缘检测仪1串口打开失败！";
                        sInfo += "\t\n";
                        richTextBox_InfoShow.AppendText(sInfo);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void btn_JuYuanJianCe_1_SerialClose_Click(object sender, EventArgs e)
        {
            try
            {
                Global.m_FormSerialJuYuanJianCe1Ctl.btn_Perform_Click(Global.m_FormSerialJuYuanJianCe1Ctl.btn_SerialClose, null);

                btn_JuYuanJianCe_1_SerialOpen.BackColor = Color.Transparent;
                btn_JuYuanJianCe_1_SerialClose.BackColor = Color.Green;
                {
                    string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "绝缘检测仪1串口关闭成功！";
                    sInfo += "\t\n";
                    richTextBox_InfoShow.AppendText(sInfo);
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void btn_JuYuanJianCe_2_SerialOpen_Click(object sender, EventArgs e)
        {
            try
            {
                Global.m_FormSerialJuYuanJianCe2Ctl.comboBox_SerialPort.SelectedIndex = this.comboBox_JuYuanJianCe_2_SerialPort.SelectedIndex;
                Global.m_FormSerialJuYuanJianCe2Ctl.btn_Perform_Click(Global.m_FormSerialJuYuanJianCe2Ctl.btn_SerialOpen, null);

                if (Global.isSerialOpenedOK_JuYuanJianCe_2)
                {
                    btn_JuYuanJianCe_2_SerialOpen.BackColor = Color.Green;
                    btn_JuYuanJianCe_2_SerialClose.BackColor = Color.Transparent;
                    {
                        string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "绝缘检测仪2串口打开成功！";
                        sInfo += "\t\n";
                        richTextBox_InfoShow.AppendText(sInfo);
                    }
                }
                else
                {
                    btn_JuYuanJianCe_2_SerialOpen.BackColor = Color.Yellow;
                    btn_JuYuanJianCe_2_SerialClose.BackColor = Color.Transparent;
                    {
                        string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "绝缘检测仪2串口打开失败！";
                        sInfo += "\t\n";
                        richTextBox_InfoShow.AppendText(sInfo);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void btn_JuYuanJianCe_2_SerialClose_Click(object sender, EventArgs e)
        {
            try
            {
                Global.m_FormSerialJuYuanJianCe2Ctl.btn_Perform_Click(Global.m_FormSerialJuYuanJianCe2Ctl.btn_SerialClose, null);

                btn_JuYuanJianCe_2_SerialOpen.BackColor = Color.Transparent;
                btn_JuYuanJianCe_2_SerialClose.BackColor = Color.Green;
                {
                    string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "绝缘检测仪2串口关闭成功！";
                    sInfo += "\t\n";
                    richTextBox_InfoShow.AppendText(sInfo);
                }
            }
            catch (Exception ex)
            {

            }
        }



        //控制控制与否的选择
        private bool isWaterBoxCtl = false;
        private void checkBox_AutoCtl_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkBox_AutoCtl.Checked)
                {

                    //停止界面浮潜
                    btn_FuQian.BackColor = Color.Transparent;
                    flag_FuQian_Oper = false;

                    //停止界面自动定向
                    flag_AutoDirCtl_Oper = false;
                    flag_ZhengZhuan_Oper = false;
                    flag_FanZhuan_Oper = false;
                    flag_ZhengZhuanToQianJin_Oper = false;
                    flag_FanZhuanToHouTui_Oper = false;

                    //停止界面自动定高
                    flag_AutoHigh_Oper = false;

                    //GroupBox_TuiJinQi.Enabled = false;
                    trackBar_FuQian.Enabled = false;
                    btn_FuQian.Enabled = false;
                    btn_ShangFu_XiaQIan_Stop.Enabled = false;
                    btn_DiCiJiaJiaoSet.Enabled = false;
                    btn_DiCiJiaJiaoSet_Stop.Enabled = false;
                    btn_AutoHigh_Start.Enabled = false;
                    btn_AutoHigh_Stop.Enabled = false;
                    groupBox_TuiJinQ_Rotate.Enabled = false;
                    groupBox_TuiJinQi_PingYi.Enabled = false;

                    isWaterBoxCtl = true;
                }
                else
                {
                    isWaterBoxCtl = false;

                    //GroupBox_TuiJinQi.Enabled = true;
                    trackBar_FuQian.Enabled = true;
                    btn_FuQian.Enabled = true;
                    btn_ShangFu_XiaQIan_Stop.Enabled = true;
                    btn_DiCiJiaJiaoSet.Enabled = true;
                    btn_DiCiJiaJiaoSet_Stop.Enabled = true;
                    btn_AutoHigh_Start.Enabled = true;
                    btn_AutoHigh_Stop.Enabled = true;
                    groupBox_TuiJinQ_Rotate.Enabled = true;
                    groupBox_TuiJinQi_PingYi.Enabled = true;
                }
            }
            catch (Exception ex)
            { }
        }

        private double value_DiCiJiaJiaoSet = 0;
        private void numericUpDown_DiCiJiaJiaoSet_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (numericUpDown_DiCiJiaJiaoSet.Value > 360)
                {
                    numericUpDown_DiCiJiaJiaoSet.Value = 0;
                    return;
                }
                else if (numericUpDown_DiCiJiaJiaoSet.Value < 0)
                {
                    numericUpDown_DiCiJiaJiaoSet.Value = 360;
                    return;
                }
                value_DiCiJiaJiaoSet = (double)numericUpDown_DiCiJiaJiaoSet.Value;
                gaugeControl4.SetPointerValue("Pointer2", value_DiCiJiaJiaoSet);
            }
            catch (Exception ex)
            { }
        }


        //潜浮、停止浮潜
        private void btn_FuQian_Click(object sender, EventArgs e)
        {
            try
            {
                btn_FuQian.BackColor = Color.Green;
                flag_FuQian_Oper = true;
            }
            catch (Exception ex)
            { }
        }

        private void btn_ShangFu_XiaQIan_Stop_Click(object sender, EventArgs e)
        {
            try
            {
                btn_FuQian.BackColor = Color.Transparent;
                flag_FuQian_Oper = false;
            }
            catch (Exception ex)
            { }
        }

        //获取并存储悬浮时的参数
        private void btn_FuQian_QuCanShu_Click(object sender, EventArgs e)
        {
            try
            {
                Global.TuiJinQiBuChang_XuanFu = trackBar_FuQian.Value;
            }
            catch (Exception ex)
            { }
        }

        private UInt64 Count_NumericUpDown_TrackBar_Changed = 0;
        private UInt64 Count_NumericUpDown_TrackBar_ChangedLast = 0;
        private void trackBar_FuQian_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (Count_NumericUpDown_TrackBar_Changed == Count_NumericUpDown_TrackBar_ChangedLast)
                {
                    Count_NumericUpDown_TrackBar_Changed++;
                    Global.TuiJinQiBuChang_XuanFu = trackBar_FuQian.Value;
                    precentFuQian = trackBar_FuQian.Value;
                    label__XiaQian.Text = trackBar_FuQian.Value.ToString() + "%";
                    numericUpDown_FuQian.Value = trackBar_FuQian.Value;
                }
                else
                {
                    Count_NumericUpDown_TrackBar_ChangedLast = Count_NumericUpDown_TrackBar_Changed;
                }
            }
            catch (Exception ex)
            { }
        }

        private void numericUpDown_FuQian_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (Count_NumericUpDown_TrackBar_Changed == Count_NumericUpDown_TrackBar_ChangedLast)
                {
                    Count_NumericUpDown_TrackBar_Changed++;
                    Global.TuiJinQiBuChang_XuanFu = Convert.ToDouble(numericUpDown_FuQian.Value);
                    precentFuQian = Convert.ToDouble(numericUpDown_FuQian.Value);
                    label__XiaQian.Text = numericUpDown_FuQian.Value.ToString() + "%";
                    if (trackBar_FuQian.Value == Convert.ToInt32(numericUpDown_FuQian.Value))
                    {
                        Count_NumericUpDown_TrackBar_ChangedLast = Count_NumericUpDown_TrackBar_Changed;
                    }
                    else
                    {
                        trackBar_FuQian.Value = Convert.ToInt32(numericUpDown_FuQian.Value);
                    }
                }
                else
                {
                    Count_NumericUpDown_TrackBar_ChangedLast = Count_NumericUpDown_TrackBar_Changed;
                }
            }
            catch (Exception ex)
            { }
        }

        private void trackBar_FuQian_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                flag_FuQian_NewSet = true;
            }
            catch (Exception ex)
            { }
        }

        #region threadFuQianOperFunc,浮潜--执行逻辑线程
        private double precentFuQian = 0;
        private bool flag_FuQian_NewSet = false;
        private Thread threadFuQianOper;
        public bool stopThreadFuQianOper = false;
        private bool flag_FuQian_Oper = false;
        private void threadFuQianOperFunc()
        {
            try
            {
                while (stopThreadFuQianOper == false)
                {
                    try
                    {
                        if (flag_FuQian_Oper)
                        {
                            //发送指令
                            Global.FaXiang8_A03 = 10.0 * (precentFuQian + Global.TuiJinQiBuChang_VLF_Zero + Global.TuiJinQiBuChang_VLF) / 100 * Global.TuiJinQiBuChang_Polar_VLF;
                            Global.FaXiang8_A04 = 10.0 * (precentFuQian + Global.TuiJinQiBuChang_VLB_Zero + Global.TuiJinQiBuChang_VLB) / 100 * Global.TuiJinQiBuChang_Polar_VLB;
                            Global.FaXiang8_A05 = 10.0 * (precentFuQian + Global.TuiJinQiBuChang_VRF_Zero + Global.TuiJinQiBuChang_VRF) / 100 * Global.TuiJinQiBuChang_Polar_VRF;
                            Global.FaXiang8_A06 = 10.0 * (precentFuQian + Global.TuiJinQiBuChang_VRB_Zero + Global.TuiJinQiBuChang_VRB) / 100 * Global.TuiJinQiBuChang_Polar_VRB;

                            Global.m_FormBoardI.SetDataIntoPCB();

                            Thread.Sleep(1000);

                            while (flag_FuQian_Oper && (stopThreadFuQianOper == false))
                            {
                                if (flag_FuQian_NewSet)
                                {
                                    Global.FaXiang8_A03 = 10.0 * (precentFuQian + Global.TuiJinQiBuChang_VLF_Zero + Global.TuiJinQiBuChang_VLF) / 100 * Global.TuiJinQiBuChang_Polar_VLF;
                                    Global.FaXiang8_A04 = 10.0 * (precentFuQian + Global.TuiJinQiBuChang_VLB_Zero + Global.TuiJinQiBuChang_VLB) / 100 * Global.TuiJinQiBuChang_Polar_VLB;
                                    Global.FaXiang8_A05 = 10.0 * (precentFuQian + Global.TuiJinQiBuChang_VRF_Zero + Global.TuiJinQiBuChang_VRF) / 100 * Global.TuiJinQiBuChang_Polar_VRF;
                                    Global.FaXiang8_A06 = 10.0 * (precentFuQian + Global.TuiJinQiBuChang_VRB_Zero + Global.TuiJinQiBuChang_VRB) / 100 * Global.TuiJinQiBuChang_Polar_VRB;

                                    Global.m_FormBoardI.SetDataIntoPCB();

                                    flag_FuQian_NewSet = false;
                                }
                                else
                                {
                                    Thread.Sleep(200);
                                }

                                Thread.Sleep(1000);
                            }
                        }
                        else
                        {
                            Thread.Sleep(50);
                        }
                    }
                    catch (Exception ex)
                    { }
                }
            }
            catch (Exception ex)
            { }
        }

        #endregion

        private void numericUpDown_HL_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                Global.TuiJinQiBuChang_HL = (double)numericUpDown_HL.Value;
            }
            catch (Exception ex)
            { }
        }

        private void numericUpDown_HR_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                Global.TuiJinQiBuChang_HR = (double)numericUpDown_HR.Value;
            }
            catch (Exception ex)
            { }
        }

        private void numericUpDown_VLF_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                Global.TuiJinQiBuChang_VLF = (double)numericUpDown_VLF.Value;
            }
            catch (Exception ex)
            { }
        }

        private void numericUpDown_VRF_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                Global.TuiJinQiBuChang_VRF = (double)numericUpDown_VRF.Value;
            }
            catch (Exception ex)
            { }
        }

        private void numericUpDown_VLB_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                Global.TuiJinQiBuChang_VLB = (double)numericUpDown_VLB.Value;
            }
            catch (Exception ex)
            { }
        }

        private void numericUpDown_VRB_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                Global.TuiJinQiBuChang_VRB = (double)numericUpDown_VRB.Value;
            }
            catch (Exception ex)
            { }
        }

        private void numericUpDown_HL_Zero_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                Global.TuiJinQiBuChang_HL_Zero = (double)numericUpDown_HL_Zero.Value;
                TuiJinQi_TiaoLing_CtlFunc();
            }
            catch (Exception ex)
            { }
        }

        private void numericUpDown_HR_Zero_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                Global.TuiJinQiBuChang_HR_Zero = (double)numericUpDown_HR_Zero.Value;
                TuiJinQi_TiaoLing_CtlFunc();
            }
            catch (Exception ex)
            { }
        }

        private void numericUpDown_VLF_Zero_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                Global.TuiJinQiBuChang_VLF_Zero = (double)numericUpDown_VLF_Zero.Value;
                TuiJinQi_TiaoLing_CtlFunc();
            }
            catch (Exception ex)
            { }
        }

        private void numericUpDown_VRF_Zero_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                Global.TuiJinQiBuChang_VRF_Zero = (double)numericUpDown_VRF_Zero.Value;
                TuiJinQi_TiaoLing_CtlFunc();
            }
            catch (Exception ex)
            { }
        }

        private void numericUpDown_VLB_Zero_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                Global.TuiJinQiBuChang_VLB_Zero = (double)numericUpDown_VLB_Zero.Value;
                TuiJinQi_TiaoLing_CtlFunc();
            }
            catch (Exception ex)
            { }
        }

        private void numericUpDown_VRB_Zero_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                Global.TuiJinQiBuChang_VRB_Zero = (double)numericUpDown_VRB_Zero.Value;
                TuiJinQi_TiaoLing_CtlFunc();
            }
            catch (Exception ex)
            { }
        }

        private void TuiJinQi_TiaoLing_CtlFunc()
        {
            try
            {
                if (checkBox_TingZhuanWeiTiao_Ctl.Checked)
                {
                    Global.FaXiang8_A01 = 10.0 * Global.TuiJinQiBuChang_HL_Zero / 100 * Global.TuiJinQiBuChang_Polar_HL;//水平左推进器
                    Global.FaXiang8_A08 = 10.0 * Global.TuiJinQiBuChang_HR_Zero / 100 * Global.TuiJinQiBuChang_Polar_HR;//水平右推进器

                    Global.FaXiang8_A03 = 10.0 * Global.TuiJinQiBuChang_VLF_Zero / 100 * Global.TuiJinQiBuChang_Polar_VLF;//前左垂直推进器
                    Global.FaXiang8_A04 = 10.0 * Global.TuiJinQiBuChang_VLB_Zero / 100 * Global.TuiJinQiBuChang_Polar_VLB;//后左垂直推进器
                    Global.FaXiang8_A05 = 10.0 * Global.TuiJinQiBuChang_VRF_Zero / 100 * Global.TuiJinQiBuChang_Polar_VRF;//前右垂直推进器
                    Global.FaXiang8_A06 = 10.0 * Global.TuiJinQiBuChang_VRB_Zero / 100 * Global.TuiJinQiBuChang_Polar_VRB;//后右垂直推进器

                    Global.m_FormBoardI.SetDataIntoPCB();
                }
            }
            catch (Exception ex)
            { }
        }



        private double percent_DiCiXuanZhuan = 0;
        private bool flag_DiCiXuanZhuan_NewSet = false;

        private void trackBar_DiCiJiaJiaoSet_Scroll(object sender, EventArgs e)
        {
            try
            {
                label_DiCiJiaJiaoSet.Text = trackBar_DiCiJiaJiaoSet.Value.ToString() + "%";
                percent_DiCiXuanZhuan = (double)trackBar_DiCiJiaJiaoSet.Value;
            }
            catch (Exception ex)
            { }
        }

        private void trackBar_DiCiJiaJiaoSet_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                flag_DiCiXuanZhuan_NewSet = true;
            }
            catch (Exception ex)
            { }
        }

        private void btn_DiCiJiaJiaoSet_Click(object sender, EventArgs e)
        {
            try
            {
                flag_AutoDirCtl_Oper = true;
            }
            catch (Exception ex)
            { }
        }

        private void btn_DiCiJiaJiaoSet_Stop_Click(object sender, EventArgs e)
        {
            try
            {
                flag_AutoDirCtl_Oper = false;
                flag_ZhengZhuan_Oper = false;
                flag_FanZhuan_Oper = false;
                flag_ZhengZhuanToQianJin_Oper = false;
                flag_FanZhuanToHouTui_Oper = false;
            }
            catch (Exception ex)
            { }
        }

        //移动or转动的速度
        private double percent_ZhuanDong_YiDong_Speed = 0;
        private bool flag_ZhuanDongYiDong_NewSet = false;
        private void numericUpDown_ZhuanDong_YiDong_Speed_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                percent_ZhuanDong_YiDong_Speed = (double)numericUpDown_ZhuanDong_YiDong_Speed.Value;
                flag_ZhuanDongYiDong_NewSet = true;
            }
            catch (Exception ex)
            { }
        }


        //功能应用-- 正转、反转
        private int isAddParasToAutoDir = 0;//在自动定向中，可以操作前进与后退，左转与右转，驱动水平推力器
        private void btn_TuiJinQin_ZhengZhuan_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if (flag_AutoDirCtl_Oper == false)
                {
                    if (checkBox_ZhuanDong_YiDong.Checked)
                    {
                        flag_ZhengZhuanToQianJin_Oper = true;
                    }
                    else
                    {
                        flag_ZhengZhuan_Oper = true;
                    }
                }
                else
                {
                    isAddParasToAutoDir = 1;
                    btn_TuiJinQin_ZhengZhuan.BackColor = Color.Green;
                }
            }
            catch (Exception ex)
            { }
        }

        private void btn_TuiJinQin_ZhengZhuan_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                if (flag_AutoDirCtl_Oper == false)
                {
                    flag_AutoDirCtl_Oper = false;
                    flag_ZhengZhuan_Oper = false;
                    flag_FanZhuan_Oper = false;
                    flag_ZhengZhuanToQianJin_Oper = false;
                    flag_FanZhuanToHouTui_Oper = false;
                }
                else
                {
                    isAddParasToAutoDir = 0;
                    btn_TuiJinQin_ZhengZhuan.BackColor = Color.Transparent;
                }
            }
            catch (Exception ex)
            { }
        }

        private void btn_TuiJinQin_FanZhuan_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if (flag_AutoDirCtl_Oper == false)
                {
                    if (checkBox_ZhuanDong_YiDong.Checked)
                    {
                        flag_FanZhuanToHouTui_Oper = true;
                    }
                    else
                    {
                        flag_FanZhuan_Oper = true;
                    }
                }
                else
                {
                    isAddParasToAutoDir = -1;
                    btn_TuiJinQin_FanZhuan.BackColor = Color.Green;
                }
            }
            catch (Exception ex)
            { }
        }

        private void btn_TuiJinQin_FanZhuan_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                if (flag_AutoDirCtl_Oper == false)
                {
                    flag_AutoDirCtl_Oper = false;
                    flag_ZhengZhuan_Oper = false;
                    flag_FanZhuan_Oper = false;
                    flag_ZhengZhuanToQianJin_Oper = false;
                    flag_FanZhuanToHouTui_Oper = false;
                }
                else
                {
                    isAddParasToAutoDir = 0;
                    btn_TuiJinQin_FanZhuan.BackColor = Color.Transparent;
                }
            }
            catch (Exception ex)
            { }
        }



        #region threadAutoDirCtlOperFunc,地磁夹角设置--执行逻辑线程
        private Thread threadAutoDirCtlOper;
        public bool stopThreadDiCiJiaJiaoOper = false;
        private bool flag_AutoDirCtl_Oper = false;//自动定向，标志
        private bool flag_ZhengZhuan_Oper = false;//正转，标志
        private bool flag_FanZhuan_Oper = false;//逆转，标志
        private bool flag_ZhengZhuanToQianJin_Oper = false;//checkBox_ZhuanDong_YiDong为勾选时，正转变为前进，标志
        private bool flag_FanZhuanToHouTui_Oper = false;//checkBox_ZhuanDong_YiDong为勾选时，逆转变为后退，标志
        private double value_DiCiJiaJiao_Now = 0;
        private void threadAutoDirCtlOperFunc()
        {
            try
            {
                while (stopThreadDiCiJiaJiaoOper == false)
                {
                    try
                    {
                        if (flag_AutoDirCtl_Oper)
                        {
                            int direct_DiCiXuanZhuan = 0;//旋转系数，1为顺转，-1为逆转

                            this.BeginInvoke(new Func<object>(() =>
                            {
                                btn_DiCiJiaJiaoSet.BackColor = Color.Green;
                                btn_DiCiJiaJiaoSet_Stop.BackColor = Color.Transparent;
                                btn_TuiJinQin_ZhengZhuan.BackColor = Color.Transparent;
                                btn_TuiJinQin_FanZhuan.BackColor = Color.Transparent;

                                //btn_TuiJinQin_ZhengZhuan.Enabled = false;
                                //btn_TuiJinQin_FanZhuan.Enabled = false;
                                btn_DiCiJiaJiaoSet.Enabled = false;
                                //numericUpDown_DiCiJiaJiaoSet.Enabled = false;

                                {
                                    string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "执行\"自动定向\"操作";
                                    sInfo += "\t\n";
                                    richTextBox_InfoShow.AppendText(sInfo);
                                }
                                return null;
                            }));

                            Thread.Sleep(1000);

                            //1.5m移动钻机，自动定向逻辑 (对应左右两个水平推力器)
                            //（1）角度差=＞5°，推力百分比30%，推力器工作2s，在2.5s后，判断角度差，
                            //（2）角度差＞3°，角度差＜5°，推力百分比10%，推力器工作1s，在1.5s后，判断角度差，
                            //（3）角度差＞2°角度差＜3°，推力百分比5%，推力器工作0.5s，在0.8s后，判断角度差，
                            //（4）角度差＜2°，暂停，，实时根据上报的Yaw值，判断角度差，
                            while (flag_AutoDirCtl_Oper && (stopThreadDiCiJiaJiaoOper == false))
                            {
                                #region 角度差=＞5°，推力百分比30%，推力器工作2s，在2.5s后，判断角度差，
                                if (Math.Abs(value_DiCiJiaJiaoSet - value_DiCiJiaJiao_Now) >= Global.AutoCtlDir_JiaoDuCha_1)
                                {
                                    //确定旋转方向
                                    if ((value_DiCiJiaJiaoSet - value_DiCiJiaJiao_Now) >= 180 ||
                                    ((value_DiCiJiaJiaoSet - value_DiCiJiaJiao_Now) < 0 && (value_DiCiJiaJiaoSet - value_DiCiJiaJiao_Now) >= -180))
                                    {
                                        direct_DiCiXuanZhuan = -1;
                                    }
                                    else if (((value_DiCiJiaJiaoSet - value_DiCiJiaJiao_Now) >= 0 && (value_DiCiJiaJiaoSet - value_DiCiJiaJiao_Now) < 180) ||
                                        ((value_DiCiJiaJiaoSet - value_DiCiJiaJiao_Now) < -180 && (value_DiCiJiaJiaoSet - value_DiCiJiaJiao_Now) >= -360))
                                    {
                                        direct_DiCiXuanZhuan = 1;
                                    }

                                    //驱动推力器
                                    Global.FaXiang8_A01 = direct_DiCiXuanZhuan * 10.0 * (percent_DiCiXuanZhuan + isAddParasToAutoDir * percent_ZhuanDong_YiDong_Speed + Global.AutoCtlDir_TuiJinPercent_1 + Global.TuiJinQiBuChang_HL_Zero + Global.TuiJinQiBuChang_HL) / 100 * Global.TuiJinQiBuChang_Polar_HL;//水平左推进器
                                    Global.FaXiang8_A08 = -1 * direct_DiCiXuanZhuan * 10.0 * (percent_DiCiXuanZhuan + isAddParasToAutoDir * percent_ZhuanDong_YiDong_Speed + Global.AutoCtlDir_TuiJinPercent_1 + Global.TuiJinQiBuChang_HR_Zero + Global.TuiJinQiBuChang_HR) / 100 * Global.TuiJinQiBuChang_Polar_HR;//水平右推进器
                                    Global.m_FormBoardI.SetDataIntoPCB();

                                    //推力器工作时间
                                    Thread.Sleep((int)Global.AutoCtlDir_TuiLiQiWorkTime_1 * 1000);

                                    //停止
                                    Global.FaXiang8_A01 = 10.0 * (Global.TuiJinQiBuChang_HL_Zero + Global.TuiJinQiBuChang_HL + isAddParasToAutoDir * percent_ZhuanDong_YiDong_Speed) / 100 * Global.TuiJinQiBuChang_Polar_HL;//水平左推进器
                                    Global.FaXiang8_A08 = 10.0 * (Global.TuiJinQiBuChang_HR_Zero + Global.TuiJinQiBuChang_HR + isAddParasToAutoDir * percent_ZhuanDong_YiDong_Speed) / 100 * Global.TuiJinQiBuChang_Polar_HR;//水平右推进器
                                    Global.m_FormBoardI.SetDataIntoPCB();

                                    //等待时间
                                    Thread.Sleep((int)Global.AutoCtlDir_TuiLiQiWorkTime_Compare_1 * 1000);
                                    Thread.Sleep(500);
                                }

                                #endregion

                                #region 角度差＞3°，角度差＜5°，推力百分比10%，推力器工作1s，在1.5s后，判断角度差，
                                else if (Math.Abs(value_DiCiJiaJiaoSet - value_DiCiJiaJiao_Now) >= Global.AutoCtlDir_JiaoDuCha_2)
                                {
                                    //确定旋转方向
                                    if ((value_DiCiJiaJiaoSet - value_DiCiJiaJiao_Now) >= 180 ||
                                    ((value_DiCiJiaJiaoSet - value_DiCiJiaJiao_Now) < 0 && (value_DiCiJiaJiaoSet - value_DiCiJiaJiao_Now) >= -180))
                                    {
                                        direct_DiCiXuanZhuan = -1;
                                    }
                                    else if (((value_DiCiJiaJiaoSet - value_DiCiJiaJiao_Now) >= 0 && (value_DiCiJiaJiaoSet - value_DiCiJiaJiao_Now) < 180) ||
                                        ((value_DiCiJiaJiaoSet - value_DiCiJiaJiao_Now) < -180 && (value_DiCiJiaJiaoSet - value_DiCiJiaJiao_Now) >= -360))
                                    {
                                        direct_DiCiXuanZhuan = 1;
                                    }

                                    //驱动推力器
                                    Global.FaXiang8_A01 = direct_DiCiXuanZhuan * 10.0 * (percent_DiCiXuanZhuan + isAddParasToAutoDir * percent_ZhuanDong_YiDong_Speed + Global.AutoCtlDir_TuiJinPercent_2 + Global.TuiJinQiBuChang_HL_Zero + Global.TuiJinQiBuChang_HL) / 100 * Global.TuiJinQiBuChang_Polar_HL;//水平左推进器
                                    Global.FaXiang8_A08 = -1 * direct_DiCiXuanZhuan * 10.0 * (percent_DiCiXuanZhuan + isAddParasToAutoDir * percent_ZhuanDong_YiDong_Speed + Global.AutoCtlDir_TuiJinPercent_2 + Global.TuiJinQiBuChang_HR_Zero + Global.TuiJinQiBuChang_HR) / 100 * Global.TuiJinQiBuChang_Polar_HR;//水平右推进器
                                    Global.m_FormBoardI.SetDataIntoPCB();

                                    //推力器工作时间
                                    Thread.Sleep((int)Global.AutoCtlDir_TuiLiQiWorkTime_2 * 1000);

                                    //停止
                                    Global.FaXiang8_A01 = 10.0 * (Global.TuiJinQiBuChang_HL_Zero + Global.TuiJinQiBuChang_HL + isAddParasToAutoDir * percent_ZhuanDong_YiDong_Speed) / 100 * Global.TuiJinQiBuChang_Polar_HL;//水平左推进器
                                    Global.FaXiang8_A08 = 10.0 * (Global.TuiJinQiBuChang_HR_Zero + Global.TuiJinQiBuChang_HR + isAddParasToAutoDir * percent_ZhuanDong_YiDong_Speed) / 100 * Global.TuiJinQiBuChang_Polar_HR;//水平右推进器
                                    Global.m_FormBoardI.SetDataIntoPCB();

                                    //等待时间
                                    Thread.Sleep((int)Global.AutoCtlDir_TuiLiQiWorkTime_Compare_2 * 1000);
                                    Thread.Sleep(500);
                                }

                                #endregion

                                #region 角度差＞2°角度差＜3°，推力百分比5%，推力器工作0.5s，在0.8s后，判断角度差，
                                else if (Math.Abs(value_DiCiJiaJiaoSet - value_DiCiJiaJiao_Now) >= Global.AutoCtlDir_JiaoDuCha_3)
                                {
                                    //确定旋转方向
                                    if ((value_DiCiJiaJiaoSet - value_DiCiJiaJiao_Now) >= 180 ||
                                    ((value_DiCiJiaJiaoSet - value_DiCiJiaJiao_Now) < 0 && (value_DiCiJiaJiaoSet - value_DiCiJiaJiao_Now) >= -180))
                                    {
                                        direct_DiCiXuanZhuan = -1;
                                    }
                                    else if (((value_DiCiJiaJiaoSet - value_DiCiJiaJiao_Now) >= 0 && (value_DiCiJiaJiaoSet - value_DiCiJiaJiao_Now) < 180) ||
                                        ((value_DiCiJiaJiaoSet - value_DiCiJiaJiao_Now) < -180 && (value_DiCiJiaJiaoSet - value_DiCiJiaJiao_Now) >= -360))
                                    {
                                        direct_DiCiXuanZhuan = 1;
                                    }

                                    //驱动推力器
                                    Global.FaXiang8_A01 = direct_DiCiXuanZhuan * 10.0 * (percent_DiCiXuanZhuan + isAddParasToAutoDir * percent_ZhuanDong_YiDong_Speed + Global.AutoCtlDir_TuiJinPercent_3 + Global.TuiJinQiBuChang_HL_Zero + Global.TuiJinQiBuChang_HL) / 100 * Global.TuiJinQiBuChang_Polar_HL;//水平左推进器
                                    Global.FaXiang8_A08 = -1 * direct_DiCiXuanZhuan * 10.0 * (percent_DiCiXuanZhuan + isAddParasToAutoDir * percent_ZhuanDong_YiDong_Speed + Global.AutoCtlDir_TuiJinPercent_3 + Global.TuiJinQiBuChang_HR_Zero + Global.TuiJinQiBuChang_HR) / 100 * Global.TuiJinQiBuChang_Polar_HR;//水平右推进器
                                    Global.m_FormBoardI.SetDataIntoPCB();

                                    //推力器工作时间
                                    Thread.Sleep((int)Global.AutoCtlDir_TuiLiQiWorkTime_3 * 1000);

                                    //停止
                                    Global.FaXiang8_A01 = 10.0 * (Global.TuiJinQiBuChang_HL_Zero + Global.TuiJinQiBuChang_HL + isAddParasToAutoDir * percent_ZhuanDong_YiDong_Speed) / 100 * Global.TuiJinQiBuChang_Polar_HL;//水平左推进器
                                    Global.FaXiang8_A08 = 10.0 * (Global.TuiJinQiBuChang_HR_Zero + Global.TuiJinQiBuChang_HR + isAddParasToAutoDir * percent_ZhuanDong_YiDong_Speed) / 100 * Global.TuiJinQiBuChang_Polar_HR;//水平右推进器
                                    Global.m_FormBoardI.SetDataIntoPCB();

                                    //等待时间
                                    Thread.Sleep((int)Global.AutoCtlDir_TuiLiQiWorkTime_Compare_3 * 1000);
                                    Thread.Sleep(500);
                                }

                                #endregion

                                #region 角度差＜2°，暂停，实时根据上报的Yaw值，判断角度差，
                                else
                                {
                                    Thread.Sleep(50);
                                }
                                #endregion

                                Thread.Sleep(1500);
                            }

                            //停止
                            this.BeginInvoke(new Func<object>(() =>
                            {
                                Global.FaXiang8_A01 = 10.0 * (Global.TuiJinQiBuChang_HL_Zero + Global.TuiJinQiBuChang_HL) / 100 * Global.TuiJinQiBuChang_Polar_HL;//水平左推进器
                                Global.FaXiang8_A08 = 10.0 * (Global.TuiJinQiBuChang_HR_Zero + Global.TuiJinQiBuChang_HR) / 100 * Global.TuiJinQiBuChang_Polar_HR;//水平右推进器

                                Global.m_FormBoardI.SetDataIntoPCB();

                                btn_TuiJinQin_ZhengZhuan.BackColor = Color.Transparent;
                                btn_TuiJinQin_FanZhuan.BackColor = Color.Transparent;
                                btn_DiCiJiaJiaoSet.BackColor = Color.Transparent;
                                btn_DiCiJiaJiaoSet_Stop.BackColor = Color.Transparent;

                                btn_TuiJinQin_ZhengZhuan.Enabled = true;
                                btn_TuiJinQin_FanZhuan.Enabled = true;
                                btn_DiCiJiaJiaoSet.Enabled = true;
                                numericUpDown_DiCiJiaJiaoSet.Enabled = true;

                                {
                                    string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "终止自动定向";
                                    sInfo += "\t\n";
                                    richTextBox_InfoShow.AppendText(sInfo);
                                }

                                return null;
                            }));
                        }

                        #region 正转
                        else if (flag_ZhengZhuan_Oper)
                        {
                            Global.FaXiang8_A01 = 10.0 * (percent_ZhuanDong_YiDong_Speed + Global.TuiJinQiBuChang_HL_Zero + Global.TuiJinQiBuChang_HL) / 100 * Global.TuiJinQiBuChang_Polar_HL;//水平左推进器
                            Global.FaXiang8_A08 = -10.0 * (percent_ZhuanDong_YiDong_Speed + Global.TuiJinQiBuChang_HR_Zero + Global.TuiJinQiBuChang_HR) / 100 * Global.TuiJinQiBuChang_Polar_HR;//水平右推进器

                            Global.m_FormBoardI.SetDataIntoPCB();

                            this.BeginInvoke(new Func<object>(() =>
                            {
                                string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "执行\"正转\"操作";
                                sInfo += "\t\n";
                                richTextBox_InfoShow.AppendText(sInfo);

                                return null;
                            }));

                            while (flag_ZhengZhuan_Oper && (stopThreadDiCiJiaJiaoOper == false))
                            {
                                if (flag_DiCiXuanZhuan_NewSet)
                                {
                                    Global.FaXiang8_A01 = 10.0 * (percent_ZhuanDong_YiDong_Speed + Global.TuiJinQiBuChang_HL_Zero + Global.TuiJinQiBuChang_HL) / 100 * Global.TuiJinQiBuChang_Polar_HL;//水平左推进器
                                    Global.FaXiang8_A08 = -10.0 * (percent_ZhuanDong_YiDong_Speed + Global.TuiJinQiBuChang_HR_Zero + Global.TuiJinQiBuChang_HR) / 100 * Global.TuiJinQiBuChang_Polar_HR;//水平右推进器

                                    Global.m_FormBoardI.SetDataIntoPCB();

                                    flag_DiCiXuanZhuan_NewSet = false;
                                }

                                Thread.Sleep(100);
                            }

                            //点击“停止”设置悬浮态
                            Global.FaXiang8_A01 = 10.0 * (Global.TuiJinQiBuChang_HL_Zero + Global.TuiJinQiBuChang_HL) / 100 * Global.TuiJinQiBuChang_Polar_HL;//水平左推进器
                            Global.FaXiang8_A08 = 10.0 * (Global.TuiJinQiBuChang_HR_Zero + Global.TuiJinQiBuChang_HR) / 100 * Global.TuiJinQiBuChang_Polar_HR;//水平右推进器
                            Global.m_FormBoardI.SetDataIntoPCB();

                            this.BeginInvoke(new Func<object>(() =>
                            {
                                string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "\"停转\"";
                                sInfo += "\t\n";
                                richTextBox_InfoShow.AppendText(sInfo);

                                return null;
                            }));
                        }
                        #endregion

                        #region 反转
                        else if (flag_FanZhuan_Oper)
                        {
                            Global.FaXiang8_A01 = -10.0 * (percent_ZhuanDong_YiDong_Speed + Global.TuiJinQiBuChang_HL_Zero + Global.TuiJinQiBuChang_HL) / 100 * Global.TuiJinQiBuChang_Polar_HL;//水平左推进器
                            Global.FaXiang8_A08 = 10.0 * (percent_ZhuanDong_YiDong_Speed + Global.TuiJinQiBuChang_HR_Zero + Global.TuiJinQiBuChang_HR) / 100 * Global.TuiJinQiBuChang_Polar_HR;//水平右推进器
                            Global.m_FormBoardI.SetDataIntoPCB();

                            this.BeginInvoke(new Func<object>(() =>
                            {
                                string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "执行\"反转\"操作";
                                sInfo += "\t\n";
                                richTextBox_InfoShow.AppendText(sInfo);

                                return null;
                            }));

                            while (flag_FanZhuan_Oper && (stopThreadDiCiJiaJiaoOper == false))
                            {
                                if (flag_DiCiXuanZhuan_NewSet)
                                {
                                    Global.FaXiang8_A01 = -10.0 * (percent_ZhuanDong_YiDong_Speed + Global.TuiJinQiBuChang_HL_Zero + Global.TuiJinQiBuChang_HL) / 100 * Global.TuiJinQiBuChang_Polar_HL;//水平左推进器
                                    Global.FaXiang8_A08 = 10.0 * (percent_ZhuanDong_YiDong_Speed + Global.TuiJinQiBuChang_HR_Zero + Global.TuiJinQiBuChang_HR) / 100 * Global.TuiJinQiBuChang_Polar_HR;//水平右推进器

                                    Global.m_FormBoardI.SetDataIntoPCB();

                                    flag_DiCiXuanZhuan_NewSet = false;
                                }

                                Thread.Sleep(100);
                            }

                            //点击“停止”设置悬浮态
                            Global.FaXiang8_A01 = 10.0 * (Global.TuiJinQiBuChang_HL_Zero + Global.TuiJinQiBuChang_HL) / 100 * Global.TuiJinQiBuChang_Polar_HL;//水平左推进器
                            Global.FaXiang8_A08 = 10.0 * (Global.TuiJinQiBuChang_HR_Zero + Global.TuiJinQiBuChang_HR) / 100 * Global.TuiJinQiBuChang_Polar_HR;//水平右推进器
                            Global.m_FormBoardI.SetDataIntoPCB();
                            this.BeginInvoke(new Func<object>(() =>
                            {
                                string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "\"停转\"";
                                sInfo += "\t\n";
                                richTextBox_InfoShow.AppendText(sInfo);

                                return null;
                            }));

                        }
                        #endregion

                        #region 正转To前进
                        else if (flag_ZhengZhuanToQianJin_Oper)
                        {
                            Global.FaXiang8_A01 = 10.0 * (percent_ZhuanDong_YiDong_Speed + Global.TuiJinQiBuChang_HL_Zero + Global.TuiJinQiBuChang_HL) / 100 * Global.TuiJinQiBuChang_Polar_HL;//水平左推进器
                            Global.FaXiang8_A08 = 10.0 * (percent_ZhuanDong_YiDong_Speed + Global.TuiJinQiBuChang_HR_Zero + Global.TuiJinQiBuChang_HR) / 100 * Global.TuiJinQiBuChang_Polar_HR;//水平右推进器
                            Global.m_FormBoardI.SetDataIntoPCB();

                            this.BeginInvoke(new Func<object>(() =>
                            {
                                string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "执行\"前进（水平推力器）\"操作";
                                sInfo += "\t\n";
                                richTextBox_InfoShow.AppendText(sInfo);

                                return null;
                            }));

                            while (flag_ZhengZhuanToQianJin_Oper && (stopThreadDiCiJiaJiaoOper == false))
                            {
                                if (flag_ZhuanDongYiDong_NewSet)
                                {
                                    Global.FaXiang8_A01 = 10.0 * (percent_ZhuanDong_YiDong_Speed + Global.TuiJinQiBuChang_HL_Zero + Global.TuiJinQiBuChang_HL) / 100 * Global.TuiJinQiBuChang_Polar_HL;//水平左推进器
                                    Global.FaXiang8_A08 = 10.0 * (percent_ZhuanDong_YiDong_Speed + Global.TuiJinQiBuChang_HR_Zero + Global.TuiJinQiBuChang_HR) / 100 * Global.TuiJinQiBuChang_Polar_HR;//水平右推进器
                                    Global.m_FormBoardI.SetDataIntoPCB();

                                    flag_ZhuanDongYiDong_NewSet = false;
                                }

                                Thread.Sleep(100);
                            }

                            //点击“停止”设置悬浮态
                            Global.FaXiang8_A01 = 10.0 * (Global.TuiJinQiBuChang_HL_Zero + Global.TuiJinQiBuChang_HL) / 100 * Global.TuiJinQiBuChang_Polar_HL;//水平左推进器
                            Global.FaXiang8_A08 = 10.0 * (Global.TuiJinQiBuChang_HR_Zero + Global.TuiJinQiBuChang_HR) / 100 * Global.TuiJinQiBuChang_Polar_HR;//水平右推进器
                            Global.m_FormBoardI.SetDataIntoPCB();
                            this.BeginInvoke(new Func<object>(() =>
                            {
                                string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "\"停转\"";
                                sInfo += "\t\n";
                                richTextBox_InfoShow.AppendText(sInfo);

                                return null;
                            }));
                        }
                        #endregion

                        #region 反转To后退
                        else if (flag_FanZhuanToHouTui_Oper)
                        {
                            Global.FaXiang8_A01 = -10.0 * (percent_ZhuanDong_YiDong_Speed + Global.TuiJinQiBuChang_HL_Zero + Global.TuiJinQiBuChang_HL) / 100 * Global.TuiJinQiBuChang_Polar_HL;//水平左推进器
                            Global.FaXiang8_A08 = -10.0 * (percent_ZhuanDong_YiDong_Speed + Global.TuiJinQiBuChang_HR_Zero + Global.TuiJinQiBuChang_HR) / 100 * Global.TuiJinQiBuChang_Polar_HR;//水平右推进器
                            Global.m_FormBoardI.SetDataIntoPCB();
                            this.BeginInvoke(new Func<object>(() =>
                            {
                                string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "执行\"后退（水平推力器）\"操作";
                                sInfo += "\t\n";
                                richTextBox_InfoShow.AppendText(sInfo);

                                return null;
                            }));

                            while (flag_FanZhuanToHouTui_Oper && (stopThreadDiCiJiaJiaoOper == false))
                            {
                                if (flag_ZhuanDongYiDong_NewSet)
                                {
                                    Global.FaXiang8_A01 = -10.0 * (percent_ZhuanDong_YiDong_Speed + Global.TuiJinQiBuChang_HL_Zero + Global.TuiJinQiBuChang_HL) / 100 * Global.TuiJinQiBuChang_Polar_HL;//水平左推进器
                                    Global.FaXiang8_A08 = -10.0 * (percent_ZhuanDong_YiDong_Speed + Global.TuiJinQiBuChang_HR_Zero + Global.TuiJinQiBuChang_HR) / 100 * Global.TuiJinQiBuChang_Polar_HR;//水平右推进器

                                    Global.m_FormBoardI.SetDataIntoPCB();

                                    flag_ZhuanDongYiDong_NewSet = false;
                                }

                                Thread.Sleep(100);
                            }

                            //点击“停止”设置悬浮态
                            Global.FaXiang8_A01 = 10.0 * (Global.TuiJinQiBuChang_HL_Zero + Global.TuiJinQiBuChang_HL) / 100 * Global.TuiJinQiBuChang_Polar_HL;//水平左推进器
                            Global.FaXiang8_A08 = 10.0 * (Global.TuiJinQiBuChang_HR_Zero + Global.TuiJinQiBuChang_HR) / 100 * Global.TuiJinQiBuChang_Polar_HR;//水平右推进器
                            Global.m_FormBoardI.SetDataIntoPCB();
                            this.BeginInvoke(new Func<object>(() =>
                            {
                                string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "\"停转\"";
                                sInfo += "\t\n";
                                richTextBox_InfoShow.AppendText(sInfo);

                                return null;
                            }));

                        }
                        #endregion


                        else
                        {
                            Thread.Sleep(50);
                        }
                    }
                    catch (Exception ex)
                    { }
                }
            }
            catch (Exception ex)
            { }
        }

        #endregion

        private double Zengliang_TuiJinQi_HMove_JiZhun = 0;
        private bool flag_TuiJinQi_HMove_JiZhun_NewSet = false;

        //相对于基准的增量，使用前➗100
        private void numericUpDown_TuiJinQi_HMove_ZengLiang_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                Zengliang_TuiJinQi_HMove_JiZhun = (double)numericUpDown_TuiJinQi_HMove_ZengLiang.Value;
                if (flag_TuiJinQi_HMove_Oper)
                {
                    flag_TuiJinQi_HMove_JiZhun_NewSet = true;
                }
            }
            catch (Exception ex)
            { }
        }

        private void btn_TuiJinQi_HMove_QianJin_Click(object sender, EventArgs e)
        {
            try
            {
                myEnum_HMove_Direction = Enum_HMove_Direction.Qian;
                btn_TuiJinQi_HMove_QianJin.BackColor = Color.Green;
                btn_TuiJinQi_HMove_HouTui.BackColor = Color.Transparent;
                btn_TuiJinQi_HMove_ZuoYi.BackColor = Color.Transparent;
                btn_TuiJinQi_HMove_YouYi.BackColor = Color.Transparent;
                flag_TuiJinQi_HMove_Oper = true;
            }
            catch (Exception ex)
            { }
        }

        private void btn_TuiJinQi_HMove_HouTui_Click(object sender, EventArgs e)
        {
            try
            {
                myEnum_HMove_Direction = Enum_HMove_Direction.Hou;
                btn_TuiJinQi_HMove_QianJin.BackColor = Color.Transparent;
                btn_TuiJinQi_HMove_HouTui.BackColor = Color.Green;
                btn_TuiJinQi_HMove_ZuoYi.BackColor = Color.Transparent;
                btn_TuiJinQi_HMove_YouYi.BackColor = Color.Transparent;
                flag_TuiJinQi_HMove_Oper = true;

            }
            catch (Exception ex)
            { }
        }

        private void btn_TuiJinQi_HMove_ZuoYi_Click(object sender, EventArgs e)
        {
            try
            {
                myEnum_HMove_Direction = Enum_HMove_Direction.Zuo;
                btn_TuiJinQi_HMove_QianJin.BackColor = Color.Transparent;
                btn_TuiJinQi_HMove_HouTui.BackColor = Color.Transparent;
                btn_TuiJinQi_HMove_ZuoYi.BackColor = Color.Green;
                btn_TuiJinQi_HMove_YouYi.BackColor = Color.Transparent;
                flag_TuiJinQi_HMove_Oper = true;
            }
            catch (Exception ex)
            { }
        }

        private void btn_TuiJinQi_HMove_YouYi_Click(object sender, EventArgs e)
        {
            try
            {
                myEnum_HMove_Direction = Enum_HMove_Direction.You;
                btn_TuiJinQi_HMove_QianJin.BackColor = Color.Transparent;
                btn_TuiJinQi_HMove_HouTui.BackColor = Color.Transparent;
                btn_TuiJinQi_HMove_ZuoYi.BackColor = Color.Transparent;
                btn_TuiJinQi_HMove_YouYi.BackColor = Color.Green;
                flag_TuiJinQi_HMove_Oper = true;
            }
            catch (Exception ex)
            { }
        }

        private void btn_TuiJinQi_HMove_Stop_Click(object sender, EventArgs e)
        {
            try
            {
                myEnum_HMove_Direction = Enum_HMove_Direction.Stop;
                btn_TuiJinQi_HMove_QianJin.BackColor = Color.Transparent;
                btn_TuiJinQi_HMove_HouTui.BackColor = Color.Transparent;
                btn_TuiJinQi_HMove_ZuoYi.BackColor = Color.Transparent;
                btn_TuiJinQi_HMove_YouYi.BackColor = Color.Transparent;
                flag_TuiJinQi_HMove_Oper = false;
            }
            catch (Exception ex)
            { }
        }

        /*
        前左垂直推进器顺转	后左垂直推进器顺转	前右垂直推进器顺转	后右垂直推进器顺转
        3B              	4B              	5B              	6B
        3A              	4A              	5A              	6A
        前左垂直推进器逆转	后左垂直推进器逆转	前右垂直推进器逆转	后右垂直推进器逆转
        */

        #region thread_TuiJinQi_HMove_OperFunc,推进器平移，前、后、左、右
        private Thread thread_TuiJinQi_HMove_Oper;
        public bool stopThread_TuiJinQi_HMove_Oper = false;
        private bool flag_TuiJinQi_HMove_Oper = false;
        private Enum_HMove_Direction myEnum_HMove_Direction = Enum_HMove_Direction.Others;
        private void thread_TuiJinQi_HMove_OperFunc()
        {
            try
            {
                while (stopThread_TuiJinQi_HMove_Oper == false)
                {
                    try
                    {

                        if (flag_TuiJinQi_HMove_Oper && (stopThread_TuiJinQi_HMove_Oper == false))
                        {
                            this.BeginInvoke(new Func<object>(() =>
                            {
                                groupBox_TuiJinQi_AutoHigh.Enabled = false;

                                {
                                    string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "执行\"平移\"操作";
                                    sInfo += "\t\n";
                                    richTextBox_InfoShow.AppendText(sInfo);
                                }
                                return null;
                            }));

                            #region 增量计算
                            double dPercent_TuiJinQi_HMove_JiZhun_And_ZengLiang = 0;
                            if (Global.TuiJinQiBuChang_XuanFu >= 0)
                            {
                                dPercent_TuiJinQi_HMove_JiZhun_And_ZengLiang = Global.TuiJinQiBuChang_XuanFu + Zengliang_TuiJinQi_HMove_JiZhun;
                            }
                            else
                            {
                                dPercent_TuiJinQi_HMove_JiZhun_And_ZengLiang = Global.TuiJinQiBuChang_XuanFu - Zengliang_TuiJinQi_HMove_JiZhun;
                            }

                            if (dPercent_TuiJinQi_HMove_JiZhun_And_ZengLiang > 100)
                            {
                                dPercent_TuiJinQi_HMove_JiZhun_And_ZengLiang = 100;
                            }
                            else if (dPercent_TuiJinQi_HMove_JiZhun_And_ZengLiang < -100)
                            {
                                dPercent_TuiJinQi_HMove_JiZhun_And_ZengLiang = -100;
                            }
                            #endregion

                            #region 前进
                            if (myEnum_HMove_Direction == Enum_HMove_Direction.Qian)
                            {
                                Global.FaXiang8_A03 = 10.0 * (Global.TuiJinQiBuChang_XuanFu + Global.TuiJinQiBuChang_VLF_Zero + Global.TuiJinQiBuChang_VLF) / 100 * Global.TuiJinQiBuChang_Polar_VLF;//前左垂直推进器
                                Global.FaXiang8_A04 = 10.0 * (dPercent_TuiJinQi_HMove_JiZhun_And_ZengLiang + Global.TuiJinQiBuChang_VLB_Zero + Global.TuiJinQiBuChang_VLB) / 100 * Global.TuiJinQiBuChang_Polar_VLB;//后左垂直推进器
                                Global.FaXiang8_A05 = 10.0 * (Global.TuiJinQiBuChang_XuanFu + Global.TuiJinQiBuChang_VRF_Zero + Global.TuiJinQiBuChang_VRF) / 100 * Global.TuiJinQiBuChang_Polar_VRF;//前右垂直推进器
                                Global.FaXiang8_A06 = 10.0 * (dPercent_TuiJinQi_HMove_JiZhun_And_ZengLiang + Global.TuiJinQiBuChang_VRB_Zero + Global.TuiJinQiBuChang_VRB) / 100 * Global.TuiJinQiBuChang_Polar_VRB;//后右垂直推进器

                                Global.m_FormBoardI.SetDataIntoPCB();
                            }
                            #endregion

                            #region 后退
                            else if (myEnum_HMove_Direction == Enum_HMove_Direction.Hou)
                            {
                                Global.FaXiang8_A03 = 10.0 * (dPercent_TuiJinQi_HMove_JiZhun_And_ZengLiang + Global.TuiJinQiBuChang_VLF_Zero + Global.TuiJinQiBuChang_VLF) / 100 * Global.TuiJinQiBuChang_Polar_VLF;//前左垂直推进器
                                Global.FaXiang8_A04 = 10.0 * (Global.TuiJinQiBuChang_XuanFu + Global.TuiJinQiBuChang_VLB_Zero + Global.TuiJinQiBuChang_VLB) / 100 * Global.TuiJinQiBuChang_Polar_VLB;//后左垂直推进器
                                Global.FaXiang8_A05 = 10.0 * (dPercent_TuiJinQi_HMove_JiZhun_And_ZengLiang + Global.TuiJinQiBuChang_VRF_Zero + Global.TuiJinQiBuChang_VRF) / 100 * Global.TuiJinQiBuChang_Polar_VRF;//前右垂直推进器
                                Global.FaXiang8_A06 = 10.0 * (Global.TuiJinQiBuChang_XuanFu + Global.TuiJinQiBuChang_VRB_Zero + Global.TuiJinQiBuChang_VRB) / 100 * Global.TuiJinQiBuChang_Polar_VRB;//后右垂直推进器

                                Global.m_FormBoardI.SetDataIntoPCB();
                            }
                            #endregion

                            #region 左移
                            else if (myEnum_HMove_Direction == Enum_HMove_Direction.Zuo)
                            {
                                Global.FaXiang8_A03 = 10.0 * (Global.TuiJinQiBuChang_XuanFu + Global.TuiJinQiBuChang_VLF_Zero + Global.TuiJinQiBuChang_VLF) / 100 * Global.TuiJinQiBuChang_Polar_VLF;//前左垂直推进器
                                Global.FaXiang8_A04 = 10.0 * (Global.TuiJinQiBuChang_XuanFu + Global.TuiJinQiBuChang_VLB_Zero + Global.TuiJinQiBuChang_VLB) / 100 * Global.TuiJinQiBuChang_Polar_VLB;//后左垂直推进器
                                Global.FaXiang8_A05 = 10.0 * (dPercent_TuiJinQi_HMove_JiZhun_And_ZengLiang + Global.TuiJinQiBuChang_VRF_Zero + Global.TuiJinQiBuChang_VRF) / 100 * Global.TuiJinQiBuChang_Polar_VRF;//前右垂直推进器
                                Global.FaXiang8_A06 = 10.0 * (dPercent_TuiJinQi_HMove_JiZhun_And_ZengLiang + Global.TuiJinQiBuChang_VRB_Zero + Global.TuiJinQiBuChang_VRB) / 100 * Global.TuiJinQiBuChang_Polar_VRB;//后右垂直推进器

                                Global.m_FormBoardI.SetDataIntoPCB();
                            }
                            #endregion

                            #region 右移
                            else if (myEnum_HMove_Direction == Enum_HMove_Direction.You)
                            {
                                Global.FaXiang8_A03 = 10.0 * (dPercent_TuiJinQi_HMove_JiZhun_And_ZengLiang + Global.TuiJinQiBuChang_VLF_Zero + Global.TuiJinQiBuChang_VLF) / 100 * Global.TuiJinQiBuChang_Polar_VLF;//前左垂直推进器
                                Global.FaXiang8_A04 = 10.0 * (dPercent_TuiJinQi_HMove_JiZhun_And_ZengLiang + Global.TuiJinQiBuChang_VLB_Zero + Global.TuiJinQiBuChang_VLB) / 100 * Global.TuiJinQiBuChang_Polar_VLB;//后左垂直推进器
                                Global.FaXiang8_A05 = 10.0 * (Global.TuiJinQiBuChang_XuanFu + Global.TuiJinQiBuChang_VRF_Zero + Global.TuiJinQiBuChang_VRF) / 100 * Global.TuiJinQiBuChang_Polar_VRF;//前右垂直推进器
                                Global.FaXiang8_A06 = 10.0 * (Global.TuiJinQiBuChang_XuanFu + Global.TuiJinQiBuChang_VRB_Zero + Global.TuiJinQiBuChang_VRB) / 100 * Global.TuiJinQiBuChang_Polar_VRB;//后右垂直推进器

                                Global.m_FormBoardI.SetDataIntoPCB();
                            }
                            #endregion

                            Thread.Sleep(100);

                            while (flag_TuiJinQi_HMove_Oper && (stopThread_TuiJinQi_HMove_Oper == false))
                            {
                                if (flag_TuiJinQi_HMove_JiZhun_NewSet)
                                {

                                    #region 根据新值进行设置

                                    #region 增量计算
                                    if (Global.TuiJinQiBuChang_XuanFu >= 0)
                                    {
                                        dPercent_TuiJinQi_HMove_JiZhun_And_ZengLiang = Global.TuiJinQiBuChang_XuanFu + Zengliang_TuiJinQi_HMove_JiZhun;
                                    }
                                    else
                                    {
                                        dPercent_TuiJinQi_HMove_JiZhun_And_ZengLiang = Global.TuiJinQiBuChang_XuanFu - Zengliang_TuiJinQi_HMove_JiZhun;
                                    }

                                    if (dPercent_TuiJinQi_HMove_JiZhun_And_ZengLiang > 100)
                                    {
                                        dPercent_TuiJinQi_HMove_JiZhun_And_ZengLiang = 100;
                                    }
                                    else if (dPercent_TuiJinQi_HMove_JiZhun_And_ZengLiang < -100)
                                    {
                                        dPercent_TuiJinQi_HMove_JiZhun_And_ZengLiang = -100;
                                    }
                                    #endregion

                                    #region 前进
                                    if (myEnum_HMove_Direction == Enum_HMove_Direction.Qian)
                                    {
                                        Global.FaXiang8_A03 = 10.0 * (Global.TuiJinQiBuChang_XuanFu + Global.TuiJinQiBuChang_VLF_Zero + Global.TuiJinQiBuChang_VLF) / 100 * Global.TuiJinQiBuChang_Polar_VLF;//前左垂直推进器
                                        Global.FaXiang8_A04 = 10.0 * (dPercent_TuiJinQi_HMove_JiZhun_And_ZengLiang + Global.TuiJinQiBuChang_VLB_Zero + Global.TuiJinQiBuChang_VLB) / 100 * Global.TuiJinQiBuChang_Polar_VLB;//后左垂直推进器
                                        Global.FaXiang8_A05 = 10.0 * (Global.TuiJinQiBuChang_XuanFu + Global.TuiJinQiBuChang_VRF_Zero + Global.TuiJinQiBuChang_VRF) / 100 * Global.TuiJinQiBuChang_Polar_VRF;//前右垂直推进器
                                        Global.FaXiang8_A06 = 10.0 * (dPercent_TuiJinQi_HMove_JiZhun_And_ZengLiang + Global.TuiJinQiBuChang_VRB_Zero + Global.TuiJinQiBuChang_VRB) / 100 * Global.TuiJinQiBuChang_Polar_VRB;//后右垂直推进器

                                        Global.m_FormBoardI.SetDataIntoPCB();
                                    }
                                    #endregion

                                    #region 后退
                                    else if (myEnum_HMove_Direction == Enum_HMove_Direction.Hou)
                                    {
                                        Global.FaXiang8_A03 = 10.0 * (dPercent_TuiJinQi_HMove_JiZhun_And_ZengLiang + Global.TuiJinQiBuChang_VLF_Zero + Global.TuiJinQiBuChang_VLF) / 100 * Global.TuiJinQiBuChang_Polar_VLF;//前左垂直推进器
                                        Global.FaXiang8_A04 = 10.0 * (Global.TuiJinQiBuChang_XuanFu + Global.TuiJinQiBuChang_VLB_Zero + Global.TuiJinQiBuChang_VLB) / 100 * Global.TuiJinQiBuChang_Polar_VLB;//后左垂直推进器
                                        Global.FaXiang8_A05 = 10.0 * (dPercent_TuiJinQi_HMove_JiZhun_And_ZengLiang + Global.TuiJinQiBuChang_VRF_Zero + Global.TuiJinQiBuChang_VRF) / 100 * Global.TuiJinQiBuChang_Polar_VRF;//前右垂直推进器
                                        Global.FaXiang8_A06 = 10.0 * (Global.TuiJinQiBuChang_XuanFu + Global.TuiJinQiBuChang_VRB_Zero + Global.TuiJinQiBuChang_VRB) / 100 * Global.TuiJinQiBuChang_Polar_VRB;//后右垂直推进器

                                        Global.m_FormBoardI.SetDataIntoPCB();
                                    }
                                    #endregion

                                    #region 左移
                                    else if (myEnum_HMove_Direction == Enum_HMove_Direction.Zuo)
                                    {
                                        Global.FaXiang8_A03 = 10.0 * (Global.TuiJinQiBuChang_XuanFu + Global.TuiJinQiBuChang_VLF_Zero + Global.TuiJinQiBuChang_VLF) / 100 * Global.TuiJinQiBuChang_Polar_VLF;//前左垂直推进器
                                        Global.FaXiang8_A04 = 10.0 * (Global.TuiJinQiBuChang_XuanFu + Global.TuiJinQiBuChang_VLB_Zero + Global.TuiJinQiBuChang_VLB) / 100 * Global.TuiJinQiBuChang_Polar_VLB;//后左垂直推进器
                                        Global.FaXiang8_A05 = 10.0 * (dPercent_TuiJinQi_HMove_JiZhun_And_ZengLiang + Global.TuiJinQiBuChang_VRF_Zero + Global.TuiJinQiBuChang_VRF) / 100 * Global.TuiJinQiBuChang_Polar_VRF;//前右垂直推进器
                                        Global.FaXiang8_A06 = 10.0 * (dPercent_TuiJinQi_HMove_JiZhun_And_ZengLiang + Global.TuiJinQiBuChang_VRB_Zero + Global.TuiJinQiBuChang_VRB) / 100 * Global.TuiJinQiBuChang_Polar_VRB;//后右垂直推进器

                                        Global.m_FormBoardI.SetDataIntoPCB();
                                    }
                                    #endregion

                                    #region 右移
                                    else if (myEnum_HMove_Direction == Enum_HMove_Direction.You)
                                    {
                                        Global.FaXiang8_A03 = 10.0 * (dPercent_TuiJinQi_HMove_JiZhun_And_ZengLiang + Global.TuiJinQiBuChang_VLF_Zero + Global.TuiJinQiBuChang_VLF) / 100 * Global.TuiJinQiBuChang_Polar_VLF;//前左垂直推进器
                                        Global.FaXiang8_A04 = 10.0 * (dPercent_TuiJinQi_HMove_JiZhun_And_ZengLiang + Global.TuiJinQiBuChang_VLB_Zero + Global.TuiJinQiBuChang_VLB) / 100 * Global.TuiJinQiBuChang_Polar_VLB;//后左垂直推进器
                                        Global.FaXiang8_A05 = 10.0 * (Global.TuiJinQiBuChang_XuanFu + Global.TuiJinQiBuChang_VRF_Zero + Global.TuiJinQiBuChang_VRF) / 100 * Global.TuiJinQiBuChang_Polar_VRF;//前右垂直推进器
                                        Global.FaXiang8_A06 = 10.0 * (Global.TuiJinQiBuChang_XuanFu + Global.TuiJinQiBuChang_VRB_Zero + Global.TuiJinQiBuChang_VRB) / 100 * Global.TuiJinQiBuChang_Polar_VRB;//后右垂直推进器

                                        Global.m_FormBoardI.SetDataIntoPCB();
                                    }
                                    #endregion


                                    #endregion

                                    flag_TuiJinQi_HMove_JiZhun_NewSet = false;
                                }

                                Thread.Sleep(100);

                            }
                        }
                        else
                        {
                            if (myEnum_HMove_Direction == Enum_HMove_Direction.Stop)
                            {
                                this.BeginInvoke(new Func<object>(() =>
                                {
                                    groupBox_TuiJinQi_AutoHigh.Enabled = true;

                                    {
                                        string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "执行\"自动推进器前后左右操作终止\"操作";
                                        sInfo += "\t\n";
                                        richTextBox_InfoShow.AppendText(sInfo);
                                    }

                                    myEnum_HMove_Direction = Enum_HMove_Direction.Others;

                                    return null;
                                }));
                            }
                            Thread.Sleep(50);
                        }
                    }
                    catch (Exception ex)
                    { }
                }
            }
            catch (Exception ex)
            { }
        }

        #endregion


        private Color color_Button_JianYaFa_DiYa = Color.Transparent;
        private Color color_Button_JianYaFa_ZhongYa = Color.Transparent;
        private Color color_Button_JianYaFa_GaoYa = Color.Transparent;
        //private void btn_JianYaFa_DiYa_BackColorChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        color_Button_JianYaFa_DiYa = btn_JianYaFa_DiYa.BackColor;
        //    }
        //    catch (Exception ex)
        //    { }
        //}

        //private void btn_JianYaFa_ZhongYa_BackColorChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        color_Button_JianYaFa_ZhongYa = btn_JianYaFa_ZhongYa.BackColor;
        //    }
        //    catch (Exception ex)
        //    { }
        //}

        //private void btn_JianYaFa_GaoYa_BackColorChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        color_Button_JianYaFa_GaoYa = btn_JianYaFa_GaoYa.BackColor;
        //    }
        //    catch (Exception ex)
        //    { }
        //}

        private void checkBox_ZhuanDong_YiDong_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkBox_ZhuanDong_YiDong.Checked)
                {
                    btn_TuiJinQin_ZhengZhuan.Text = "前进";
                    btn_TuiJinQin_FanZhuan.Text = "后退";
                }
                else
                {
                    btn_TuiJinQin_ZhengZhuan.Text = "正转";
                    btn_TuiJinQin_FanZhuan.Text = "反转";
                }
            }
            catch (Exception ex)
            { }
        }


        //自动定高

        private void btn_AutoHigh_Start_Click(object sender, EventArgs e)
        {
            try
            {
                flag_AutoHigh_Oper = true;
            }
            catch (Exception ex)
            { }
        }

        private void btn_AutoHigh_Stop_Click(object sender, EventArgs e)
        {
            try
            {
                flag_AutoHigh_Oper = false;
            }
            catch (Exception ex)
            { }
        }

        private void numericUpDown_AutoHigh_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                value_High_Set = (double)numericUpDown_AutoHigh.Value;
            }
            catch (Exception ex)
            { }
        }



        #region threadAutoHighOperFunc,自动定高--执行逻辑线程
        private Thread threadAutoHighOper;
        public bool stopThreadAutoHighOper = false;
        private bool flag_AutoHigh_Oper = false;//自动定向，标志
        private double value_High_Now = 0;
        private double value_High_Set = 0;
        private void threadAutoHighOperFunc()
        {
            try
            {
                while (stopThreadAutoHighOper == false)
                {
                    try
                    {
                        if (flag_AutoHigh_Oper)
                        {
                            this.BeginInvoke(new Func<object>(() =>
                            {
                                btn_AutoHigh_Start.BackColor = Color.Green;
                                btn_AutoHigh_Stop.BackColor = Color.Transparent;
                                btn_TuiJinQi_HMove_QianJin.BackColor = Color.Transparent;
                                btn_TuiJinQi_HMove_HouTui.BackColor = Color.Transparent;
                                btn_TuiJinQi_HMove_ZuoYi.BackColor = Color.Transparent;
                                btn_TuiJinQi_HMove_YouYi.BackColor = Color.Transparent;

                                groupBox_TuiJinQi_PingYi.Enabled = false;

                                {
                                    string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "执行\"自动定高\"操作";
                                    sInfo += "\t\n";
                                    richTextBox_InfoShow.AppendText(sInfo);
                                }
                                return null;
                            }));

                            Thread.Sleep(1000);


                            //1.5m移动钻机，自动定高逻辑  (对应垂直4个推力器)
                            //（1）高度差=＞5m，推力百分比10%，推力器工作2s，在2.5s后，判断高度差，
                            //（2）高度差＞3m，角度差＜5m，推力百分比8%，推力器工作1s，在1.5s后，判断高度差，
                            //（3）高度差＞1m角度差＜3m，推力百分比3%，推力器工作0.5s，在0.8s后，判断高度差，
                            //（4）高度差＜1m，暂停，，实时根据上报的高度值，判断高度差，
                            while (flag_AutoHigh_Oper && (stopThreadDiCiJiaJiaoOper == false))
                            {
                                #region 高度差=＞5m，推力百分比10%，推力器工作2s，在2.5s后，判断高度差，
                                if (Math.Abs(value_High_Set - value_High_Now) >= Global.AutoCtlHigh_GaoDuCha_1)
                                {
                                    delegateAutoCtlHigh sd = new delegateAutoCtlHigh(delegateAutoCtlHighFunc);
                                    sd.BeginInvoke(1, null, null);
                                    flag_delegateAutoCtlHighFunc_Finished = false;
                                    while (flag_AutoDirCtl_Oper && flag_delegateAutoCtlHighFunc_Finished == false)
                                    {
                                        Thread.Sleep(10);
                                    }

                                    Thread.Sleep(10);
                                }

                                #endregion

                                #region 高度差＞3m，角度差＜5m，推力百分比8%，推力器工作1s，在1.5s后，判断高度差，
                                else if (Math.Abs(value_High_Set - value_High_Now) >= Global.AutoCtlHigh_GaoDuCha_2)
                                {
                                    delegateAutoCtlHigh sd = new delegateAutoCtlHigh(delegateAutoCtlHighFunc);
                                    sd.BeginInvoke(2, null, null);
                                    flag_delegateAutoCtlHighFunc_Finished = false;
                                    while (flag_AutoDirCtl_Oper && flag_delegateAutoCtlHighFunc_Finished == false)
                                    {
                                        Thread.Sleep(10);
                                    }

                                    Thread.Sleep(10);
                                }

                                #endregion

                                #region 高度差＞1m角度差＜3m，推力百分比3%，推力器工作0.5s，在0.8s后，判断高度差，
                                else if (Math.Abs(value_High_Set - value_High_Now) >= Global.AutoCtlHigh_GaoDuCha_3)
                                {
                                    delegateAutoCtlHigh sd = new delegateAutoCtlHigh(delegateAutoCtlHighFunc);
                                    sd.BeginInvoke(3, null, null);
                                    flag_delegateAutoCtlHighFunc_Finished = false;
                                    while (flag_AutoDirCtl_Oper && flag_delegateAutoCtlHighFunc_Finished == false)
                                    {
                                        Thread.Sleep(10);
                                    }

                                    Thread.Sleep(10);
                                }

                                #endregion

                                #region 高度差＜1m，暂停，，实时根据上报的高度值，判断高度差，
                                else
                                {
                                    Thread.Sleep(50);
                                }
                                #endregion

                                Thread.Sleep(1000);
                            }


                            //停止
                            this.BeginInvoke(new Func<object>(() =>
                            {
                                btn_AutoHigh_Start.BackColor = Color.Transparent;
                                btn_AutoHigh_Stop.BackColor = Color.Transparent;
                                btn_TuiJinQi_HMove_QianJin.BackColor = Color.Transparent;
                                btn_TuiJinQi_HMove_HouTui.BackColor = Color.Transparent;
                                btn_TuiJinQi_HMove_ZuoYi.BackColor = Color.Transparent;
                                btn_TuiJinQi_HMove_YouYi.BackColor = Color.Transparent;

                                groupBox_TuiJinQi_PingYi.Enabled = true;

                                {
                                    string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "终止自动定高";
                                    sInfo += "\t\n";
                                    richTextBox_InfoShow.AppendText(sInfo);
                                }

                                return null;
                            }));
                        }

                        else
                        {
                            Thread.Sleep(50);
                        }
                    }
                    catch (Exception ex)
                    { }
                }
            }
            catch (Exception ex)
            { }
        }

        private bool flag_delegateAutoCtlHighFunc_Finished = true;
        private delegate void delegateAutoCtlHigh(int type);
        private void delegateAutoCtlHighFunc(int type)
        {
            try
            {
                int direct_AutoHigh = 0;//1为上浮，-1为下潜

                #region 高度差=＞5m，推力百分比10%，推力器工作2s，在2.5s后，判断高度差，
                if (type == 1)
                {
                    //确定旋转方向
                    if ((value_High_Now - value_High_Set) > 0)
                    {
                        direct_AutoHigh = -1;
                    }
                    else if ((value_High_Now - value_High_Set) < 0)
                    {
                        direct_AutoHigh = 1;
                    }
                    else
                    {
                        direct_AutoHigh = 0;
                    }

                    //驱动推力器
                    Global.FaXiang8_A03 = direct_AutoHigh * 10.0 * (Global.AutoCtlHigh_TuiJinPercent_1 + Global.TuiJinQiBuChang_XuanFu + Global.TuiJinQiBuChang_VLF_Zero + Global.TuiJinQiBuChang_VLF) / 100 * Global.TuiJinQiBuChang_Polar_VLF;//前左垂直推进器
                    Global.FaXiang8_A04 = direct_AutoHigh * 10.0 * (Global.AutoCtlHigh_TuiJinPercent_1 + Global.TuiJinQiBuChang_XuanFu + Global.TuiJinQiBuChang_VLB_Zero + Global.TuiJinQiBuChang_VLB) / 100 * Global.TuiJinQiBuChang_Polar_VLB;//后左垂直推进器
                    Global.FaXiang8_A05 = direct_AutoHigh * 10.0 * (Global.AutoCtlHigh_TuiJinPercent_1 + Global.TuiJinQiBuChang_XuanFu + Global.TuiJinQiBuChang_VRF_Zero + Global.TuiJinQiBuChang_VRF) / 100 * Global.TuiJinQiBuChang_Polar_VRF;//前右垂直推进器
                    Global.FaXiang8_A06 = direct_AutoHigh * 10.0 * (Global.AutoCtlHigh_TuiJinPercent_1 + Global.TuiJinQiBuChang_XuanFu + Global.TuiJinQiBuChang_VRB_Zero + Global.TuiJinQiBuChang_VRB) / 100 * Global.TuiJinQiBuChang_Polar_VRB;//后右垂直推进器
                    Global.m_FormBoardI.SetDataIntoPCB();

                    //推力器工作时间
                    Thread.Sleep((int)Global.AutoCtlHigh_TuiLiQiWorkTime_1 * 1000);

                    //停止
                    Global.FaXiang8_A03 = 10.0 * (Global.TuiJinQiBuChang_XuanFu + Global.TuiJinQiBuChang_VLF_Zero + Global.TuiJinQiBuChang_VLF) / 100 * Global.TuiJinQiBuChang_Polar_VLF;//前左垂直推进器
                    Global.FaXiang8_A04 = 10.0 * (Global.TuiJinQiBuChang_XuanFu + Global.TuiJinQiBuChang_VLB_Zero + Global.TuiJinQiBuChang_VLB) / 100 * Global.TuiJinQiBuChang_Polar_VLB;//后左垂直推进器
                    Global.FaXiang8_A05 = 10.0 * (Global.TuiJinQiBuChang_XuanFu + Global.TuiJinQiBuChang_VRF_Zero + Global.TuiJinQiBuChang_VRF) / 100 * Global.TuiJinQiBuChang_Polar_VRF;//前右垂直推进器
                    Global.FaXiang8_A06 = 10.0 * (Global.TuiJinQiBuChang_XuanFu + Global.TuiJinQiBuChang_VRB_Zero + Global.TuiJinQiBuChang_VRB) / 100 * Global.TuiJinQiBuChang_Polar_VRB;//后右垂直推进器
                    Global.m_FormBoardI.SetDataIntoPCB();

                    //等待时间
                    Thread.Sleep((int)Global.AutoCtlHigh_TuiLiQiWorkTime_Compare_1 * 1000);
                }

                #endregion

                #region 角度差＞3°，角度差＜5°，推力百分比10%，推力器工作1s，在1.5s后，判断角度差
                else if (type == 2)
                {
                    //确定旋转方向
                    if ((value_High_Now - value_High_Set) > 0)
                    {
                        direct_AutoHigh = -1;
                    }
                    else if ((value_High_Now - value_High_Set) < 0)
                    {
                        direct_AutoHigh = 1;
                    }
                    else
                    {
                        direct_AutoHigh = 0;
                    }

                    //驱动推力器
                    Global.FaXiang8_A03 = direct_AutoHigh * 10.0 * (Global.AutoCtlHigh_TuiJinPercent_2 + Global.TuiJinQiBuChang_XuanFu + Global.TuiJinQiBuChang_VLF_Zero + Global.TuiJinQiBuChang_VLF) / 100 * Global.TuiJinQiBuChang_Polar_VLF;//前左垂直推进器
                    Global.FaXiang8_A04 = direct_AutoHigh * 10.0 * (Global.AutoCtlHigh_TuiJinPercent_2 + Global.TuiJinQiBuChang_XuanFu + Global.TuiJinQiBuChang_VLB_Zero + Global.TuiJinQiBuChang_VLB) / 100 * Global.TuiJinQiBuChang_Polar_VLB;//后左垂直推进器
                    Global.FaXiang8_A05 = direct_AutoHigh * 10.0 * (Global.AutoCtlHigh_TuiJinPercent_2 + Global.TuiJinQiBuChang_XuanFu + Global.TuiJinQiBuChang_VRF_Zero + Global.TuiJinQiBuChang_VRF) / 100 * Global.TuiJinQiBuChang_Polar_VRF;//前右垂直推进器
                    Global.FaXiang8_A06 = direct_AutoHigh * 10.0 * (Global.AutoCtlHigh_TuiJinPercent_2 + Global.TuiJinQiBuChang_XuanFu + Global.TuiJinQiBuChang_VRB_Zero + Global.TuiJinQiBuChang_VRB) / 100 * Global.TuiJinQiBuChang_Polar_VRB;//后右垂直推进器
                    Global.m_FormBoardI.SetDataIntoPCB();

                    //推力器工作时间
                    Thread.Sleep((int)Global.AutoCtlHigh_TuiLiQiWorkTime_2 * 1000);

                    //停止
                    Global.FaXiang8_A03 = 10.0 * (Global.TuiJinQiBuChang_XuanFu + Global.TuiJinQiBuChang_VLF_Zero + Global.TuiJinQiBuChang_VLF) / 100 * Global.TuiJinQiBuChang_Polar_VLF;//前左垂直推进器
                    Global.FaXiang8_A04 = 10.0 * (Global.TuiJinQiBuChang_XuanFu + Global.TuiJinQiBuChang_VLB_Zero + Global.TuiJinQiBuChang_VLB) / 100 * Global.TuiJinQiBuChang_Polar_VLB;//后左垂直推进器
                    Global.FaXiang8_A05 = 10.0 * (Global.TuiJinQiBuChang_XuanFu + Global.TuiJinQiBuChang_VRF_Zero + Global.TuiJinQiBuChang_VRF) / 100 * Global.TuiJinQiBuChang_Polar_VRF;//前右垂直推进器
                    Global.FaXiang8_A06 = 10.0 * (Global.TuiJinQiBuChang_XuanFu + Global.TuiJinQiBuChang_VRB_Zero + Global.TuiJinQiBuChang_VRB) / 100 * Global.TuiJinQiBuChang_Polar_VRB;//后右垂直推进器
                    Global.m_FormBoardI.SetDataIntoPCB();

                    //等待时间
                    Thread.Sleep((int)Global.AutoCtlHigh_TuiLiQiWorkTime_Compare_2 * 1000);
                }

                #endregion

                #region 角度差＞2°角度差＜3°，推力百分比5%，推力器工作0.5s，在0.8s后，判断角度差，
                else if (type == 3)
                {
                    //确定旋转方向
                    if ((value_High_Now - value_High_Set) > 0)
                    {
                        direct_AutoHigh = -1;
                    }
                    else if ((value_High_Now - value_High_Set) < 0)
                    {
                        direct_AutoHigh = 1;
                    }
                    else
                    {
                        direct_AutoHigh = 0;
                    }

                    //驱动推力器
                    Global.FaXiang8_A03 = direct_AutoHigh * 10.0 * (Global.AutoCtlHigh_TuiJinPercent_3 + Global.TuiJinQiBuChang_XuanFu + Global.TuiJinQiBuChang_VLF_Zero + Global.TuiJinQiBuChang_VLF) / 100 * Global.TuiJinQiBuChang_Polar_VLF;//前左垂直推进器
                    Global.FaXiang8_A04 = direct_AutoHigh * 10.0 * (Global.AutoCtlHigh_TuiJinPercent_3 + Global.TuiJinQiBuChang_XuanFu + Global.TuiJinQiBuChang_VLB_Zero + Global.TuiJinQiBuChang_VLB) / 100 * Global.TuiJinQiBuChang_Polar_VLB;//后左垂直推进器
                    Global.FaXiang8_A05 = direct_AutoHigh * 10.0 * (Global.AutoCtlHigh_TuiJinPercent_3 + Global.TuiJinQiBuChang_XuanFu + Global.TuiJinQiBuChang_VRF_Zero + Global.TuiJinQiBuChang_VRF) / 100 * Global.TuiJinQiBuChang_Polar_VRF;//前右垂直推进器
                    Global.FaXiang8_A06 = direct_AutoHigh * 10.0 * (Global.AutoCtlHigh_TuiJinPercent_3 + Global.TuiJinQiBuChang_XuanFu + Global.TuiJinQiBuChang_VRB_Zero + Global.TuiJinQiBuChang_VRB) / 100 * Global.TuiJinQiBuChang_Polar_VRB;//后右垂直推进器
                    Global.m_FormBoardI.SetDataIntoPCB();

                    //推力器工作时间
                    Thread.Sleep((int)Global.AutoCtlHigh_TuiLiQiWorkTime_3 * 1000);

                    //停止
                    Global.FaXiang8_A03 = 10.0 * (Global.TuiJinQiBuChang_XuanFu + Global.TuiJinQiBuChang_VLF_Zero + Global.TuiJinQiBuChang_VLF) / 100 * Global.TuiJinQiBuChang_Polar_VLF;//前左垂直推进器
                    Global.FaXiang8_A04 = 10.0 * (Global.TuiJinQiBuChang_XuanFu + Global.TuiJinQiBuChang_VLB_Zero + Global.TuiJinQiBuChang_VLB) / 100 * Global.TuiJinQiBuChang_Polar_VLB;//后左垂直推进器
                    Global.FaXiang8_A05 = 10.0 * (Global.TuiJinQiBuChang_XuanFu + Global.TuiJinQiBuChang_VRF_Zero + Global.TuiJinQiBuChang_VRF) / 100 * Global.TuiJinQiBuChang_Polar_VRF;//前右垂直推进器
                    Global.FaXiang8_A06 = 10.0 * (Global.TuiJinQiBuChang_XuanFu + Global.TuiJinQiBuChang_VRB_Zero + Global.TuiJinQiBuChang_VRB) / 100 * Global.TuiJinQiBuChang_Polar_VRB;//后右垂直推进器
                    Global.m_FormBoardI.SetDataIntoPCB();

                    //等待时间
                    Thread.Sleep((int)Global.AutoCtlHigh_TuiLiQiWorkTime_Compare_3 * 1000);
                }

                #endregion


                flag_delegateAutoCtlHighFunc_Finished = true;
            }
            catch (Exception ex)
            { }
        }


        #endregion

        private void btn_TuiJinQin_ZhengZhuan_Click(object sender, EventArgs e)
        {

        }

        private void btn_TuiJinQin_FanZhuan_Click(object sender, EventArgs e)
        {

        }




        #region threadWaterBoxCtlDirOperFunc,水面控制盒控制，自动定向>旋转
        private Thread threadWaterBoxCtlDirOper;
        public bool stopThreadWaterBoxCtlDirOper = false;
        public bool flag_WaterBoxCtl_AutoDir = false;
        public double paras_WaterBoxCtl_Rotate = 0;//旋转
        private void threadWaterBoxCtlDirOperFunc()
        {
            try
            {
                while (stopThreadWaterBoxCtlDirOper == false)
                {
                    try
                    {
                        if (isWaterBoxCtl)
                        {

                            if (Global.flag_WaterBoxCtl_Dir_Refresh)
                            {
                                if (flag_WaterBoxCtl_AutoDir)//自动定向
                                {
                                    #region 自动定向


                                    this.BeginInvoke(new Func<object>(() =>
                                    {
                                        string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "开始控制盒自动定向";
                                        sInfo += "\t\n";
                                        richTextBox_InfoShow.AppendText(sInfo);
                                        return null;
                                    }));

                                    int direct_DiCiXuanZhuan = 0;

                                    //1.5m移动钻机，自动定向逻辑 (对应左右两个水平推力器)
                                    //（1）角度差=＞5°，推力百分比30%，推力器工作2s，在2.5s后，判断角度差，
                                    //（2）角度差＞3°，角度差＜5°，推力百分比10%，推力器工作1s，在1.5s后，判断角度差，
                                    //（3）角度差＞2°角度差＜3°，推力百分比5%，推力器工作0.5s，在0.8s后，判断角度差，
                                    //（4）角度差＜2°，暂停，，实时根据上报的Yaw值，判断角度差，
                                    while (isWaterBoxCtl && flag_WaterBoxCtl_AutoDir && (stopThreadWaterBoxCtlDirOper == false))
                                    {
                                        try
                                        {
                                            #region 角度差=＞5°，推力百分比30%，推力器工作2s，在2.5s后，判断角度差，
                                            if (Math.Abs(value_DiCiJiaJiaoSet - value_DiCiJiaJiao_Now) >= Global.AutoCtlDir_JiaoDuCha_1)
                                            {
                                                //确定旋转方向
                                                if ((value_DiCiJiaJiaoSet - value_DiCiJiaJiao_Now) >= 180 ||
                                                ((value_DiCiJiaJiaoSet - value_DiCiJiaJiao_Now) < 0 && (value_DiCiJiaJiaoSet - value_DiCiJiaJiao_Now) >= -180))
                                                {
                                                    direct_DiCiXuanZhuan = -1;
                                                }
                                                else if (((value_DiCiJiaJiaoSet - value_DiCiJiaJiao_Now) >= 0 && (value_DiCiJiaJiaoSet - value_DiCiJiaJiao_Now) < 180) ||
                                                    ((value_DiCiJiaJiaoSet - value_DiCiJiaJiao_Now) < -180 && (value_DiCiJiaJiaoSet - value_DiCiJiaJiao_Now) >= -360))
                                                {
                                                    direct_DiCiXuanZhuan = 1;
                                                }

                                                //驱动推力器
                                                Global.FaXiang8_A01 = direct_DiCiXuanZhuan * 10.0 * (percent_DiCiXuanZhuan + Global.AutoCtlDir_TuiJinPercent_1 + Global.TuiJinQiBuChang_HL_Zero + Global.TuiJinQiBuChang_HL) / 100 * Global.TuiJinQiBuChang_Polar_HL;//水平左推进器
                                                Global.FaXiang8_A08 = -1 * direct_DiCiXuanZhuan * 10.0 * (percent_DiCiXuanZhuan + Global.AutoCtlDir_TuiJinPercent_1 + Global.TuiJinQiBuChang_HR_Zero + Global.TuiJinQiBuChang_HR) / 100 * Global.TuiJinQiBuChang_Polar_HR;//水平右推进器
                                                Global.m_FormBoardI.SetDataIntoPCB();

                                                //推力器工作时间
                                                Thread.Sleep((int)Global.AutoCtlDir_TuiLiQiWorkTime_1 * 1000);

                                                //停止
                                                Global.FaXiang8_A01 = 10.0 * (Global.TuiJinQiBuChang_HL_Zero + Global.TuiJinQiBuChang_HL) / 100 * Global.TuiJinQiBuChang_Polar_HL;//水平左推进器
                                                Global.FaXiang8_A08 = 10.0 * (Global.TuiJinQiBuChang_HR_Zero + Global.TuiJinQiBuChang_HR) / 100 * Global.TuiJinQiBuChang_Polar_HR;//水平右推进器
                                                Global.m_FormBoardI.SetDataIntoPCB();

                                                //等待时间
                                                Thread.Sleep((int)Global.AutoCtlDir_TuiLiQiWorkTime_Compare_1 * 1000);
                                                Thread.Sleep(500);
                                            }

                                            #endregion

                                            #region 角度差＞3°，角度差＜5°，推力百分比10%，推力器工作1s，在1.5s后，判断角度差，
                                            else if (Math.Abs(value_DiCiJiaJiaoSet - value_DiCiJiaJiao_Now) >= Global.AutoCtlDir_JiaoDuCha_2)
                                            {
                                                //确定旋转方向
                                                if ((value_DiCiJiaJiaoSet - value_DiCiJiaJiao_Now) >= 180 ||
                                                ((value_DiCiJiaJiaoSet - value_DiCiJiaJiao_Now) < 0 && (value_DiCiJiaJiaoSet - value_DiCiJiaJiao_Now) >= -180))
                                                {
                                                    direct_DiCiXuanZhuan = -1;
                                                }
                                                else if (((value_DiCiJiaJiaoSet - value_DiCiJiaJiao_Now) >= 0 && (value_DiCiJiaJiaoSet - value_DiCiJiaJiao_Now) < 180) ||
                                                    ((value_DiCiJiaJiaoSet - value_DiCiJiaJiao_Now) < -180 && (value_DiCiJiaJiaoSet - value_DiCiJiaJiao_Now) >= -360))
                                                {
                                                    direct_DiCiXuanZhuan = 1;
                                                }

                                                //驱动推力器
                                                Global.FaXiang8_A01 = direct_DiCiXuanZhuan * 10.0 * (percent_DiCiXuanZhuan + Global.AutoCtlDir_TuiJinPercent_2 + Global.TuiJinQiBuChang_HL_Zero + Global.TuiJinQiBuChang_HL) / 100 * Global.TuiJinQiBuChang_Polar_HL;//水平左推进器
                                                Global.FaXiang8_A08 = -1 * direct_DiCiXuanZhuan * 10.0 * (percent_DiCiXuanZhuan + Global.AutoCtlDir_TuiJinPercent_2 + Global.TuiJinQiBuChang_HR_Zero + Global.TuiJinQiBuChang_HR) / 100 * Global.TuiJinQiBuChang_Polar_HR;//水平右推进器
                                                Global.m_FormBoardI.SetDataIntoPCB();

                                                //推力器工作时间
                                                Thread.Sleep((int)Global.AutoCtlDir_TuiLiQiWorkTime_2 * 1000);

                                                //停止
                                                Global.FaXiang8_A01 = 10.0 * (Global.TuiJinQiBuChang_HL_Zero + Global.TuiJinQiBuChang_HL) / 100 * Global.TuiJinQiBuChang_Polar_HL;//水平左推进器
                                                Global.FaXiang8_A08 = 10.0 * (Global.TuiJinQiBuChang_HR_Zero + Global.TuiJinQiBuChang_HR) / 100 * Global.TuiJinQiBuChang_Polar_HR;//水平右推进器
                                                Global.m_FormBoardI.SetDataIntoPCB();

                                                //等待时间
                                                Thread.Sleep((int)Global.AutoCtlDir_TuiLiQiWorkTime_Compare_2 * 1000);
                                                Thread.Sleep(500);
                                            }

                                            #endregion

                                            #region 角度差＞2°角度差＜3°，推力百分比5%，推力器工作0.5s，在0.8s后，判断角度差，
                                            else if (Math.Abs(value_DiCiJiaJiaoSet - value_DiCiJiaJiao_Now) >= Global.AutoCtlDir_JiaoDuCha_3)
                                            {
                                                //确定旋转方向
                                                if ((value_DiCiJiaJiaoSet - value_DiCiJiaJiao_Now) >= 180 ||
                                                ((value_DiCiJiaJiaoSet - value_DiCiJiaJiao_Now) < 0 && (value_DiCiJiaJiaoSet - value_DiCiJiaJiao_Now) >= -180))
                                                {
                                                    direct_DiCiXuanZhuan = -1;
                                                }
                                                else if (((value_DiCiJiaJiaoSet - value_DiCiJiaJiao_Now) >= 0 && (value_DiCiJiaJiaoSet - value_DiCiJiaJiao_Now) < 180) ||
                                                    ((value_DiCiJiaJiaoSet - value_DiCiJiaJiao_Now) < -180 && (value_DiCiJiaJiaoSet - value_DiCiJiaJiao_Now) >= -360))
                                                {
                                                    direct_DiCiXuanZhuan = 1;
                                                }

                                                //驱动推力器
                                                Global.FaXiang8_A01 = direct_DiCiXuanZhuan * 10.0 * (percent_DiCiXuanZhuan + Global.AutoCtlDir_TuiJinPercent_3 + Global.TuiJinQiBuChang_HL_Zero + Global.TuiJinQiBuChang_HL) / 100 * Global.TuiJinQiBuChang_Polar_HL;//水平左推进器
                                                Global.FaXiang8_A08 = -1 * direct_DiCiXuanZhuan * 10.0 * (percent_DiCiXuanZhuan + Global.AutoCtlDir_TuiJinPercent_3 + Global.TuiJinQiBuChang_HR_Zero + Global.TuiJinQiBuChang_HR) / 100 * Global.TuiJinQiBuChang_Polar_HR;//水平右推进器
                                                Global.m_FormBoardI.SetDataIntoPCB();

                                                //推力器工作时间
                                                Thread.Sleep((int)Global.AutoCtlDir_TuiLiQiWorkTime_3 * 1000);

                                                //停止
                                                Global.FaXiang8_A01 = 10.0 * (Global.TuiJinQiBuChang_HL_Zero + Global.TuiJinQiBuChang_HL) / 100 * Global.TuiJinQiBuChang_Polar_HL;//水平左推进器
                                                Global.FaXiang8_A08 = 10.0 * (Global.TuiJinQiBuChang_HR_Zero + Global.TuiJinQiBuChang_HR) / 100 * Global.TuiJinQiBuChang_Polar_HR;//水平右推进器
                                                Global.m_FormBoardI.SetDataIntoPCB();

                                                //等待时间
                                                Thread.Sleep((int)Global.AutoCtlDir_TuiLiQiWorkTime_Compare_3 * 1000);
                                                Thread.Sleep(500);
                                            }

                                            #endregion

                                            #region 角度差＜2°，暂停，实时根据上报的Yaw值，判断角度差，
                                            else
                                            {
                                                Thread.Sleep(50);
                                            }
                                            #endregion

                                            //Thread.Sleep(1500);
                                        }
                                        catch (Exception ex)
                                        {
                                            Thread.Sleep(50);
                                        }
                                    }


                                    //停止
                                    try
                                    {
                                        Global.FaXiang8_A01 = 10.0 * (Global.TuiJinQiBuChang_HL_Zero + Global.TuiJinQiBuChang_HL) / 100 * Global.TuiJinQiBuChang_Polar_HL;//水平左推进器
                                        Global.FaXiang8_A08 = 10.0 * (Global.TuiJinQiBuChang_HR_Zero + Global.TuiJinQiBuChang_HR) / 100 * Global.TuiJinQiBuChang_Polar_HR;//水平右推进器
                                        Global.m_FormBoardI.SetDataIntoPCB();

                                        this.BeginInvoke(new Func<object>(() =>
                                        {
                                            string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "终止控制盒自动定向";
                                            sInfo += "\t\n";
                                            richTextBox_InfoShow.AppendText(sInfo);
                                            return null;
                                        }));
                                    }
                                    catch (Exception ex)
                                    {
                                        Thread.Sleep(50);
                                    }

                                    #endregion
                                }
                                else
                                {
                                    //0~2.4V对应推力器的-10V~0V；2.6V-5V对应推力器0V-10V
                                    double percent_Rotate = 0;
                                    if (paras_WaterBoxCtl_Rotate >= 2.7)
                                    {
                                        percent_Rotate = (paras_WaterBoxCtl_Rotate - 2.7) / 2.3;
                                    }
                                    else if (paras_WaterBoxCtl_Rotate <= 2.3)
                                    {
                                        percent_Rotate = (2.3 - paras_WaterBoxCtl_Rotate) / 2.3;
                                    }
                                    else
                                    {
                                        percent_Rotate = 0;
                                    }
                                    Global.FaXiang8_A01 = 10.0 * (percent_Rotate + Global.TuiJinQiBuChang_HL_Zero + Global.TuiJinQiBuChang_HL) / 100 * Global.TuiJinQiBuChang_Polar_HL;//水平左推进器
                                    Global.FaXiang8_A08 = -10.0 * (percent_Rotate + Global.TuiJinQiBuChang_HR_Zero + Global.TuiJinQiBuChang_HR) / 100 * Global.TuiJinQiBuChang_Polar_HR;//水平右推进器
                                    Global.m_FormBoardI.SetDataIntoPCB();
                                }
                                Global.flag_WaterBoxCtl_Dir_Refresh = false;
                                Thread.Sleep(5);
                            }
                            else
                            {
                                Thread.Sleep(50);
                            }
                        }
                        else
                        {
                            Thread.Sleep(100);
                        }
                    }
                    catch (Exception ex)
                    { }
                }
            }
            catch (Exception ex)
            { }
        }

        #endregion


        #region threadWaterBoxCtlHighOperFunc,水面控制盒控制，自动定高>上浮下潜>前后左右
        private Thread threadWaterBoxCtlHighOper;
        public bool stopThreadWaterBoxCtlHighOper = false;
        public bool flag_WaterBoxCtl_AutoHigh = false;//水面控制盒自动定高
        public bool flag_WaterBoxCtl_UpDown = false;//水面控制盒上浮下潜
        public bool flag_WaterBoxCtl_ForwardBack = false;//水面控制盒前后移动
        public bool flag_WaterBoxCtl_RightLeft = false;//水面控制盒左右移动

        public double paras_WaterBoxCtl_QianHou = 0;//前后
        public double paras_WaterBoxCtl_ZuoYou = 0;//左右
        public double paras_WaterBoxCtl_FuQian = 0;//浮潜
        private void threadWaterBoxCtlHighOperFunc()
        {
            try
            {
                while (stopThreadWaterBoxCtlHighOper == false)
                {
                    try
                    {
                        int direct_AutoHigh = 0;//1为上浮，-1为下潜

                        if (isWaterBoxCtl)
                        {
                            if (Global.flag_WaterBoxCtl_High_Refresh)
                            {
                                if (flag_WaterBoxCtl_AutoHigh)//自动定高
                                {
                                    this.BeginInvoke(new Func<object>(() =>
                                    {
                                        string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "执行\"控制盒自动定高\"操作";
                                        sInfo += "\t\n";
                                        richTextBox_InfoShow.AppendText(sInfo);
                                        return null;
                                    }));

                                    //1.5m移动钻机，自动定高逻辑  (对应垂直4个推力器)
                                    //（1）高度差=＞5m，推力百分比10%，推力器工作2s，在2.5s后，判断高度差，
                                    //（2）高度差＞3m，角度差＜5m，推力百分比8%，推力器工作1s，在1.5s后，判断高度差，
                                    //（3）高度差＞1m角度差＜3m，推力百分比3%，推力器工作0.5s，在0.8s后，判断高度差，
                                    //（4）高度差＜1m，暂停，，实时根据上报的高度值，判断高度差，
                                    while (isWaterBoxCtl && flag_WaterBoxCtl_AutoHigh && (stopThreadWaterBoxCtlHighOper == false))
                                    {
                                        #region 高度差=＞5m，推力百分比10%，推力器工作2s，在2.5s后，判断高度差，
                                        if (Math.Abs(value_High_Set - value_High_Now) >= Global.AutoCtlHigh_GaoDuCha_1)
                                        {
                                            //确定旋转方向
                                            if ((value_High_Now - value_High_Set) > 0)
                                            {
                                                direct_AutoHigh = -1;
                                            }
                                            else if ((value_High_Now - value_High_Set) < 0)
                                            {
                                                direct_AutoHigh = 1;
                                            }
                                            else
                                            {
                                                direct_AutoHigh = 0;
                                            }

                                            //驱动推力器
                                            Global.FaXiang8_A03 = direct_AutoHigh * 10.0 * (Global.AutoCtlHigh_TuiJinPercent_1 + Global.TuiJinQiBuChang_XuanFu + Global.TuiJinQiBuChang_VLF_Zero + Global.TuiJinQiBuChang_VLF) / 100 * Global.TuiJinQiBuChang_Polar_VLF;//前左垂直推进器
                                            Global.FaXiang8_A04 = direct_AutoHigh * 10.0 * (Global.AutoCtlHigh_TuiJinPercent_1 + Global.TuiJinQiBuChang_XuanFu + Global.TuiJinQiBuChang_VLB_Zero + Global.TuiJinQiBuChang_VLB) / 100 * Global.TuiJinQiBuChang_Polar_VLB;//后左垂直推进器
                                            Global.FaXiang8_A05 = direct_AutoHigh * 10.0 * (Global.AutoCtlHigh_TuiJinPercent_1 + Global.TuiJinQiBuChang_XuanFu + Global.TuiJinQiBuChang_VRF_Zero + Global.TuiJinQiBuChang_VRF) / 100 * Global.TuiJinQiBuChang_Polar_VRF;//前右垂直推进器
                                            Global.FaXiang8_A06 = direct_AutoHigh * 10.0 * (Global.AutoCtlHigh_TuiJinPercent_1 + Global.TuiJinQiBuChang_XuanFu + Global.TuiJinQiBuChang_VRB_Zero + Global.TuiJinQiBuChang_VRB) / 100 * Global.TuiJinQiBuChang_Polar_VRB;//后右垂直推进器
                                            Global.m_FormBoardI.SetDataIntoPCB();

                                            //推力器工作时间
                                            Thread.Sleep((int)Global.AutoCtlHigh_TuiLiQiWorkTime_1 * 1000);

                                            //停止
                                            Global.FaXiang8_A03 = 10.0 * (Global.TuiJinQiBuChang_XuanFu + Global.TuiJinQiBuChang_VLF_Zero + Global.TuiJinQiBuChang_VLF) / 100 * Global.TuiJinQiBuChang_Polar_VLF;//前左垂直推进器
                                            Global.FaXiang8_A04 = 10.0 * (Global.TuiJinQiBuChang_XuanFu + Global.TuiJinQiBuChang_VLB_Zero + Global.TuiJinQiBuChang_VLB) / 100 * Global.TuiJinQiBuChang_Polar_VLB;//后左垂直推进器
                                            Global.FaXiang8_A05 = 10.0 * (Global.TuiJinQiBuChang_XuanFu + Global.TuiJinQiBuChang_VRF_Zero + Global.TuiJinQiBuChang_VRF) / 100 * Global.TuiJinQiBuChang_Polar_VRF;//前右垂直推进器
                                            Global.FaXiang8_A06 = 10.0 * (Global.TuiJinQiBuChang_XuanFu + Global.TuiJinQiBuChang_VRB_Zero + Global.TuiJinQiBuChang_VRB) / 100 * Global.TuiJinQiBuChang_Polar_VRB;//后右垂直推进器
                                            Global.m_FormBoardI.SetDataIntoPCB();

                                            //等待时间
                                            Thread.Sleep((int)Global.AutoCtlHigh_TuiLiQiWorkTime_Compare_1 * 1000);
                                        }

                                        #endregion

                                        #region 高度差＞3m，角度差＜5m，推力百分比8%，推力器工作1s，在1.5s后，判断高度差，
                                        else if (Math.Abs(value_High_Set - value_High_Now) >= Global.AutoCtlHigh_GaoDuCha_2)
                                        {
                                            //确定旋转方向
                                            if ((value_High_Now - value_High_Set) > 0)
                                            {
                                                direct_AutoHigh = -1;
                                            }
                                            else if ((value_High_Now - value_High_Set) < 0)
                                            {
                                                direct_AutoHigh = 1;
                                            }
                                            else
                                            {
                                                direct_AutoHigh = 0;
                                            }

                                            //驱动推力器
                                            Global.FaXiang8_A03 = direct_AutoHigh * 10.0 * (Global.AutoCtlHigh_TuiJinPercent_2 + Global.TuiJinQiBuChang_XuanFu + Global.TuiJinQiBuChang_VLF_Zero + Global.TuiJinQiBuChang_VLF) / 100 * Global.TuiJinQiBuChang_Polar_VLF;//前左垂直推进器
                                            Global.FaXiang8_A04 = direct_AutoHigh * 10.0 * (Global.AutoCtlHigh_TuiJinPercent_2 + Global.TuiJinQiBuChang_XuanFu + Global.TuiJinQiBuChang_VLB_Zero + Global.TuiJinQiBuChang_VLB) / 100 * Global.TuiJinQiBuChang_Polar_VLB;//后左垂直推进器
                                            Global.FaXiang8_A05 = direct_AutoHigh * 10.0 * (Global.AutoCtlHigh_TuiJinPercent_2 + Global.TuiJinQiBuChang_XuanFu + Global.TuiJinQiBuChang_VRF_Zero + Global.TuiJinQiBuChang_VRF) / 100 * Global.TuiJinQiBuChang_Polar_VRF;//前右垂直推进器
                                            Global.FaXiang8_A06 = direct_AutoHigh * 10.0 * (Global.AutoCtlHigh_TuiJinPercent_2 + Global.TuiJinQiBuChang_XuanFu + Global.TuiJinQiBuChang_VRB_Zero + Global.TuiJinQiBuChang_VRB) / 100 * Global.TuiJinQiBuChang_Polar_VRB;//后右垂直推进器
                                            Global.m_FormBoardI.SetDataIntoPCB();

                                            //推力器工作时间
                                            Thread.Sleep((int)Global.AutoCtlHigh_TuiLiQiWorkTime_2 * 1000);

                                            //停止
                                            Global.FaXiang8_A03 = 10.0 * (Global.TuiJinQiBuChang_XuanFu + Global.TuiJinQiBuChang_VLF_Zero + Global.TuiJinQiBuChang_VLF) / 100 * Global.TuiJinQiBuChang_Polar_VLF;//前左垂直推进器
                                            Global.FaXiang8_A04 = 10.0 * (Global.TuiJinQiBuChang_XuanFu + Global.TuiJinQiBuChang_VLB_Zero + Global.TuiJinQiBuChang_VLB) / 100 * Global.TuiJinQiBuChang_Polar_VLB;//后左垂直推进器
                                            Global.FaXiang8_A05 = 10.0 * (Global.TuiJinQiBuChang_XuanFu + Global.TuiJinQiBuChang_VRF_Zero + Global.TuiJinQiBuChang_VRF) / 100 * Global.TuiJinQiBuChang_Polar_VRF;//前右垂直推进器
                                            Global.FaXiang8_A06 = 10.0 * (Global.TuiJinQiBuChang_XuanFu + Global.TuiJinQiBuChang_VRB_Zero + Global.TuiJinQiBuChang_VRB) / 100 * Global.TuiJinQiBuChang_Polar_VRB;//后右垂直推进器
                                            Global.m_FormBoardI.SetDataIntoPCB();

                                            //等待时间
                                            Thread.Sleep((int)Global.AutoCtlHigh_TuiLiQiWorkTime_Compare_2 * 1000);
                                        }

                                        #endregion

                                        #region 高度差＞1m角度差＜3m，推力百分比3%，推力器工作0.5s，在0.8s后，判断高度差，
                                        else if (Math.Abs(value_High_Set - value_High_Now) >= Global.AutoCtlHigh_GaoDuCha_3)
                                        {
                                            //确定旋转方向
                                            if ((value_High_Now - value_High_Set) > 0)
                                            {
                                                direct_AutoHigh = -1;
                                            }
                                            else if ((value_High_Now - value_High_Set) < 0)
                                            {
                                                direct_AutoHigh = 1;
                                            }
                                            else
                                            {
                                                direct_AutoHigh = 0;
                                            }

                                            //驱动推力器
                                            Global.FaXiang8_A03 = direct_AutoHigh * 10.0 * (Global.AutoCtlHigh_TuiJinPercent_3 + Global.TuiJinQiBuChang_XuanFu + Global.TuiJinQiBuChang_VLF_Zero + Global.TuiJinQiBuChang_VLF) / 100 * Global.TuiJinQiBuChang_Polar_VLF;//前左垂直推进器
                                            Global.FaXiang8_A04 = direct_AutoHigh * 10.0 * (Global.AutoCtlHigh_TuiJinPercent_3 + Global.TuiJinQiBuChang_XuanFu + Global.TuiJinQiBuChang_VLB_Zero + Global.TuiJinQiBuChang_VLB) / 100 * Global.TuiJinQiBuChang_Polar_VLB;//后左垂直推进器
                                            Global.FaXiang8_A05 = direct_AutoHigh * 10.0 * (Global.AutoCtlHigh_TuiJinPercent_3 + Global.TuiJinQiBuChang_XuanFu + Global.TuiJinQiBuChang_VRF_Zero + Global.TuiJinQiBuChang_VRF) / 100 * Global.TuiJinQiBuChang_Polar_VRF;//前右垂直推进器
                                            Global.FaXiang8_A06 = direct_AutoHigh * 10.0 * (Global.AutoCtlHigh_TuiJinPercent_3 + Global.TuiJinQiBuChang_XuanFu + Global.TuiJinQiBuChang_VRB_Zero + Global.TuiJinQiBuChang_VRB) / 100 * Global.TuiJinQiBuChang_Polar_VRB;//后右垂直推进器
                                            Global.m_FormBoardI.SetDataIntoPCB();

                                            //推力器工作时间
                                            Thread.Sleep((int)Global.AutoCtlHigh_TuiLiQiWorkTime_3 * 1000);

                                            //停止
                                            Global.FaXiang8_A03 = 10.0 * (Global.TuiJinQiBuChang_XuanFu + Global.TuiJinQiBuChang_VLF_Zero + Global.TuiJinQiBuChang_VLF) / 100 * Global.TuiJinQiBuChang_Polar_VLF;//前左垂直推进器
                                            Global.FaXiang8_A04 = 10.0 * (Global.TuiJinQiBuChang_XuanFu + Global.TuiJinQiBuChang_VLB_Zero + Global.TuiJinQiBuChang_VLB) / 100 * Global.TuiJinQiBuChang_Polar_VLB;//后左垂直推进器
                                            Global.FaXiang8_A05 = 10.0 * (Global.TuiJinQiBuChang_XuanFu + Global.TuiJinQiBuChang_VRF_Zero + Global.TuiJinQiBuChang_VRF) / 100 * Global.TuiJinQiBuChang_Polar_VRF;//前右垂直推进器
                                            Global.FaXiang8_A06 = 10.0 * (Global.TuiJinQiBuChang_XuanFu + Global.TuiJinQiBuChang_VRB_Zero + Global.TuiJinQiBuChang_VRB) / 100 * Global.TuiJinQiBuChang_Polar_VRB;//后右垂直推进器
                                            Global.m_FormBoardI.SetDataIntoPCB();

                                            //等待时间
                                            Thread.Sleep((int)Global.AutoCtlHigh_TuiLiQiWorkTime_Compare_3 * 1000);
                                        }

                                        #endregion

                                        #region 高度差＜1m，暂停，，实时根据上报的高度值，判断高度差，
                                        else
                                        {
                                            Thread.Sleep(50);
                                        }
                                        #endregion

                                        //Thread.Sleep(1000);
                                    }

                                    //停止
                                    this.BeginInvoke(new Func<object>(() =>
                                    {
                                        string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "终止控制盒自动定高";
                                        sInfo += "\t\n";
                                        richTextBox_InfoShow.AppendText(sInfo);

                                        return null;
                                    }));

                                }
                                else if (flag_WaterBoxCtl_UpDown)
                                {
                                    double vWaterBoxCtl = 0;
                                    if (paras_WaterBoxCtl_FuQian < 2.3 && paras_WaterBoxCtl_FuQian > 0)
                                    {
                                        vWaterBoxCtl = 1 - paras_WaterBoxCtl_FuQian / 2.3;
                                    }
                                    else if (paras_WaterBoxCtl_FuQian > 2.7 && paras_WaterBoxCtl_FuQian < 5)
                                    {
                                        vWaterBoxCtl = -1.0 * (paras_WaterBoxCtl_FuQian - 2.7) / 2.3;
                                    }
                                    Global.FaXiang8_A03 = 10.0 * ((Global.TuiJinQiBuChang_VLF_Zero + Global.TuiJinQiBuChang_VLF) / 100 + vWaterBoxCtl) * Global.TuiJinQiBuChang_Polar_VLF;
                                    Global.FaXiang8_A04 = 10.0 * ((Global.TuiJinQiBuChang_VLB_Zero + Global.TuiJinQiBuChang_VLB) / 100 + vWaterBoxCtl) * Global.TuiJinQiBuChang_Polar_VLB;
                                    Global.FaXiang8_A05 = 10.0 * ((Global.TuiJinQiBuChang_VRF_Zero + Global.TuiJinQiBuChang_VRF) / 100 + vWaterBoxCtl) * Global.TuiJinQiBuChang_Polar_VRF;
                                    Global.FaXiang8_A06 = 10.0 * ((Global.TuiJinQiBuChang_VRB_Zero + Global.TuiJinQiBuChang_VRB) / 100 + vWaterBoxCtl) * Global.TuiJinQiBuChang_Polar_VRB;

                                    Global.m_FormBoardI.SetDataIntoPCB();

                                }
                                else if (flag_WaterBoxCtl_ForwardBack)
                                {
                                    #region 前进
                                    if (paras_WaterBoxCtl_QianHou < 2.3 && paras_WaterBoxCtl_QianHou > 0)
                                    {
                                        double vWaterBoxCtl = 1 - paras_WaterBoxCtl_QianHou / 2.3;
                                        Global.FaXiang8_A03 = 10.0 * (Global.TuiJinQiBuChang_XuanFu + Global.TuiJinQiBuChang_VLF_Zero + Global.TuiJinQiBuChang_VLF) / 100 * Global.TuiJinQiBuChang_Polar_VLF;//前左垂直推进器
                                        Global.FaXiang8_A04 = 10.0 * ((Global.TuiJinQiBuChang_XuanFu + Global.TuiJinQiBuChang_VLB_Zero + Global.TuiJinQiBuChang_VLB) / 100 + vWaterBoxCtl) * Global.TuiJinQiBuChang_Polar_VLB;//后左垂直推进器
                                        Global.FaXiang8_A05 = 10.0 * (Global.TuiJinQiBuChang_XuanFu + Global.TuiJinQiBuChang_VRF_Zero + Global.TuiJinQiBuChang_VRF) / 100 * Global.TuiJinQiBuChang_Polar_VRF;//前右垂直推进器
                                        Global.FaXiang8_A06 = 10.0 * ((Global.TuiJinQiBuChang_XuanFu + Global.TuiJinQiBuChang_VRB_Zero + Global.TuiJinQiBuChang_VRB) / 100 + vWaterBoxCtl) * Global.TuiJinQiBuChang_Polar_VRB;//后右垂直推进器

                                        Global.m_FormBoardI.SetDataIntoPCB();
                                    }
                                    #endregion

                                    #region 后退
                                    else if (paras_WaterBoxCtl_QianHou > 2.7 && paras_WaterBoxCtl_QianHou < 5)
                                    {
                                        double vWaterBoxCtl = (paras_WaterBoxCtl_QianHou - 2.7) / 2.3;
                                        Global.FaXiang8_A03 = 10.0 * ((Global.TuiJinQiBuChang_XuanFu + Global.TuiJinQiBuChang_VLF_Zero + Global.TuiJinQiBuChang_VLF) / 100 + vWaterBoxCtl) * Global.TuiJinQiBuChang_Polar_VLF;//前左垂直推进器
                                        Global.FaXiang8_A04 = 10.0 * (Global.TuiJinQiBuChang_XuanFu + Global.TuiJinQiBuChang_VLB_Zero + Global.TuiJinQiBuChang_VLB) / 100 * Global.TuiJinQiBuChang_Polar_VLB;//后左垂直推进器
                                        Global.FaXiang8_A05 = 10.0 * ((Global.TuiJinQiBuChang_XuanFu + Global.TuiJinQiBuChang_VRF_Zero + Global.TuiJinQiBuChang_VRF) / 100 + vWaterBoxCtl) * Global.TuiJinQiBuChang_Polar_VRF;//前右垂直推进器
                                        Global.FaXiang8_A06 = 10.0 * (Global.TuiJinQiBuChang_XuanFu + Global.TuiJinQiBuChang_VRB_Zero + Global.TuiJinQiBuChang_VRB) / 100 * Global.TuiJinQiBuChang_Polar_VRB;//后右垂直推进器

                                        Global.m_FormBoardI.SetDataIntoPCB();
                                    }
                                    #endregion
                                }
                                else if (flag_WaterBoxCtl_RightLeft)
                                {
                                    #region 左移
                                    if (paras_WaterBoxCtl_ZuoYou < 2.3 && paras_WaterBoxCtl_ZuoYou > 0)
                                    {
                                        double vWaterBoxCtl = 1 - paras_WaterBoxCtl_ZuoYou / 2.3;
                                        Global.FaXiang8_A03 = 10.0 * (Global.TuiJinQiBuChang_XuanFu + Global.TuiJinQiBuChang_VLF_Zero + Global.TuiJinQiBuChang_VLF) / 100 * Global.TuiJinQiBuChang_Polar_VLF;//前左垂直推进器
                                        Global.FaXiang8_A04 = 10.0 * (Global.TuiJinQiBuChang_XuanFu + Global.TuiJinQiBuChang_VLB_Zero + Global.TuiJinQiBuChang_VLB) / 100 * Global.TuiJinQiBuChang_Polar_VLB;//后左垂直推进器
                                        Global.FaXiang8_A05 = 10.0 * ((Global.TuiJinQiBuChang_XuanFu + Global.TuiJinQiBuChang_VRF_Zero + Global.TuiJinQiBuChang_VRF) / 100 + vWaterBoxCtl) * Global.TuiJinQiBuChang_Polar_VRF;//前右垂直推进器
                                        Global.FaXiang8_A06 = 10.0 * ((Global.TuiJinQiBuChang_XuanFu + Global.TuiJinQiBuChang_VRB_Zero + Global.TuiJinQiBuChang_VRB) / 100 + vWaterBoxCtl) * Global.TuiJinQiBuChang_Polar_VRB;//后右垂直推进器

                                        Global.m_FormBoardI.SetDataIntoPCB();
                                    }
                                    #endregion

                                    #region 右移
                                    else if (paras_WaterBoxCtl_ZuoYou > 2.7 && paras_WaterBoxCtl_ZuoYou < 5)
                                    {
                                        double vWaterBoxCtl = (paras_WaterBoxCtl_ZuoYou - 2.7) / 2.3;
                                        Global.FaXiang8_A03 = 10.0 * ((Global.TuiJinQiBuChang_XuanFu + Global.TuiJinQiBuChang_VLF_Zero + Global.TuiJinQiBuChang_VLF) / 100 + vWaterBoxCtl) * Global.TuiJinQiBuChang_Polar_VLF;//前左垂直推进器
                                        Global.FaXiang8_A04 = 10.0 * ((Global.TuiJinQiBuChang_XuanFu + Global.TuiJinQiBuChang_VLB_Zero + Global.TuiJinQiBuChang_VLB) / 100 + vWaterBoxCtl) * Global.TuiJinQiBuChang_Polar_VLB;//后左垂直推进器
                                        Global.FaXiang8_A05 = 10.0 * (Global.TuiJinQiBuChang_XuanFu + Global.TuiJinQiBuChang_VRF_Zero + Global.TuiJinQiBuChang_VRF) / 100 * Global.TuiJinQiBuChang_Polar_VRF;//前右垂直推进器
                                        Global.FaXiang8_A06 = 10.0 * (Global.TuiJinQiBuChang_XuanFu + Global.TuiJinQiBuChang_VRB_Zero + Global.TuiJinQiBuChang_VRB) / 100 * Global.TuiJinQiBuChang_Polar_VRB;//后右垂直推进器

                                        Global.m_FormBoardI.SetDataIntoPCB();
                                    }
                                    #endregion

                                }
                                else
                                {
                                    Global.FaXiang8_A03 = 10.0 * (Global.TuiJinQiBuChang_VLF_Zero + Global.TuiJinQiBuChang_VLF) / 100 * Global.TuiJinQiBuChang_Polar_VLF;
                                    Global.FaXiang8_A04 = 10.0 * (Global.TuiJinQiBuChang_VLB_Zero + Global.TuiJinQiBuChang_VLB) / 100 * Global.TuiJinQiBuChang_Polar_VLB;
                                    Global.FaXiang8_A05 = 10.0 * (Global.TuiJinQiBuChang_VRF_Zero + Global.TuiJinQiBuChang_VRF) / 100 * Global.TuiJinQiBuChang_Polar_VRF;
                                    Global.FaXiang8_A06 = 10.0 * (Global.TuiJinQiBuChang_VRB_Zero + Global.TuiJinQiBuChang_VRB) / 100 * Global.TuiJinQiBuChang_Polar_VRB;

                                    Global.m_FormBoardI.SetDataIntoPCB();
                                }
                                Global.flag_WaterBoxCtl_High_Refresh = false;
                                Thread.Sleep(5);
                            }
                            else
                            {
                                Thread.Sleep(50);
                            }
                        }
                        else
                        {
                            Thread.Sleep(100);
                        }
                    }
                    catch (Exception ex)
                    { }
                }
            }
            catch (Exception ex)
            { }
        }

        #endregion

        private double now_BasicParas_ZuanJin_ShenDu = 0;
        // 5.12
        //private void btn_ZuanJin_JiZhuanSet_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        Global.BasicParas_ZuanJin_ShenDu = now_BasicParas_ZuanJin_ShenDu;
        //        SaveBasicParasConfig();
        //        label_ZuanJin_JiZhun.Text = Math.Round(Global.BasicParas_ZuanJin_ShenDu, 1).ToString();
        //    }
        //    catch (Exception ex)
        //    { }
        //}

        // #region 阀箱备用通道

        // 5.12 7A
        //private void btn_A7_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (Global.m_FormBoardII.IsConnected == false)
        //        {
        //            string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "16功能阀箱网络未连接，操作取消！";
        //            sInfo += "\t\n";
        //            richTextBox_InfoShow.AppendText(sInfo);
        //            return;
        //        }

        //        if (flagCmdToSend == true)
        //        {
        //            string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "尚有操作未完成，请稍后再操作！";
        //            sInfo += "\t\n";
        //            richTextBox_InfoShow.AppendText(sInfo);

        //            return;
        //        }

        //        flagCmdToSend = true;
        //        myEnum_UserButtonOperIdentify = Enum_UserButtonOperIdentify.FaXiang_Space_A7;

        //        //操作记录
        //        string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t阀箱备用通道A7开\t";
        //        Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
        //        Global.myLogStreamWriter.Flush();

        //        {
        //            string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "执行\"阀箱备用通道A7开\"操作";
        //            sInfo += "\t\n";
        //            richTextBox_InfoShow.AppendText(sInfo);
        //        }
        //    }
        //    catch (Exception ex)
        //    { }
        //}

        //5.12 B7
        //private void btn_B7_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (Global.m_FormBoardII.IsConnected == false)
        //        {
        //            string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "16功能阀箱网络未连接，操作取消！";
        //            sInfo += "\t\n";
        //            richTextBox_InfoShow.AppendText(sInfo);
        //            return;
        //        }

        //        if (flagCmdToSend == true)
        //        {
        //            string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "尚有操作未完成，请稍后再操作！";
        //            sInfo += "\t\n";
        //            richTextBox_InfoShow.AppendText(sInfo);

        //            return;
        //        }

        //        flagCmdToSend = true;
        //        myEnum_UserButtonOperIdentify = Enum_UserButtonOperIdentify.FaXiang_Space_B7;

        //        //操作记录
        //        string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t阀箱备用通道B7开\t";
        //        Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
        //        Global.myLogStreamWriter.Flush();

        //        {
        //            string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "执行\"阀箱备用通道B7开\"操作";
        //            sInfo += "\t\n";
        //            richTextBox_InfoShow.AppendText(sInfo);
        //        }
        //    }
        //    catch (Exception ex)
        //    { }
        //}

        //private void btn_AB7_Stop_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (Global.m_FormBoardII.IsConnected == false)
        //        {
        //            string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "16功能阀箱网络未连接，操作取消！";
        //            sInfo += "\t\n";
        //            richTextBox_InfoShow.AppendText(sInfo);
        //            return;
        //        }

        //        if (flagCmdToSend == true)
        //        {
        //            string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "尚有操作未完成，请稍后再操作！";
        //            sInfo += "\t\n";
        //            richTextBox_InfoShow.AppendText(sInfo);

        //            return;
        //        }

        //        flagCmdToSend = true;
        //        myEnum_UserButtonOperIdentify = Enum_UserButtonOperIdentify.FaXiang_Space_AB7_Stop;

        //        //操作记录
        //        string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t阀箱备用通道AB7关闭\t";
        //        Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
        //        Global.myLogStreamWriter.Flush();

        //        {
        //            string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "执行\"阀箱备用通道AB7关闭\"操作";
        //            sInfo += "\t\n";
        //            richTextBox_InfoShow.AppendText(sInfo);
        //        }
        //    }
        //    catch (Exception ex)
        //    { }
        //}

        //private void btn_A8_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (Global.m_FormBoardII.IsConnected == false)
        //        {
        //            string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "16功能阀箱网络未连接，操作取消！";
        //            sInfo += "\t\n";
        //            richTextBox_InfoShow.AppendText(sInfo);
        //            return;
        //        }

        //        if (flagCmdToSend == true)
        //        {
        //            string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "尚有操作未完成，请稍后再操作！";
        //            sInfo += "\t\n";
        //            richTextBox_InfoShow.AppendText(sInfo);

        //            return;
        //        }

        //        flagCmdToSend = true;
        //        myEnum_UserButtonOperIdentify = Enum_UserButtonOperIdentify.FaXiang_Space_A8;

        //        //操作记录
        //        string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t阀箱备用通道A8开\t";
        //        Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
        //        Global.myLogStreamWriter.Flush();

        //        {
        //            string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "执行\"阀箱备用通道A8开\"操作";
        //            sInfo += "\t\n";
        //            richTextBox_InfoShow.AppendText(sInfo);
        //        }
        //    }
        //    catch (Exception ex)
        //    { }
        //}

        //private void btn_B8_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (Global.m_FormBoardII.IsConnected == false)
        //        {
        //            string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "16功能阀箱网络未连接，操作取消！";
        //            sInfo += "\t\n";
        //            richTextBox_InfoShow.AppendText(sInfo);
        //            return;
        //        }

        //        if (flagCmdToSend == true)
        //        {
        //            string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "尚有操作未完成，请稍后再操作！";
        //            sInfo += "\t\n";
        //            richTextBox_InfoShow.AppendText(sInfo);

        //            return;
        //        }

        //        flagCmdToSend = true;
        //        myEnum_UserButtonOperIdentify = Enum_UserButtonOperIdentify.FaXiang_Space_B8;

        //        //操作记录
        //        string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t阀箱备用通道B8开\t";
        //        Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
        //        Global.myLogStreamWriter.Flush();

        //        {
        //            string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "执行\"阀箱备用通道B8开\"操作";
        //            sInfo += "\t\n";
        //            richTextBox_InfoShow.AppendText(sInfo);
        //        }
        //    }
        //    catch (Exception ex)
        //    { }
        //}

        //private void btn_AB8_Stop_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (Global.m_FormBoardII.IsConnected == false)
        //        {
        //            string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "16功能阀箱网络未连接，操作取消！";
        //            sInfo += "\t\n";
        //            richTextBox_InfoShow.AppendText(sInfo);
        //            return;
        //        }

        //        if (flagCmdToSend == true)
        //        {
        //            string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "尚有操作未完成，请稍后再操作！";
        //            sInfo += "\t\n";
        //            richTextBox_InfoShow.AppendText(sInfo);

        //            return;
        //        }

        //        flagCmdToSend = true;
        //        myEnum_UserButtonOperIdentify = Enum_UserButtonOperIdentify.FaXiang_Space_AB8_Stop;

        //        //操作记录
        //        string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t阀箱备用通道AB8关闭\t";
        //        Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
        //        Global.myLogStreamWriter.Flush();

        //        {
        //            string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "执行\"阀箱备用通道AB8关闭\"操作";
        //            sInfo += "\t\n";
        //            richTextBox_InfoShow.AppendText(sInfo);
        //        }
        //    }
        //    catch (Exception ex)
        //    { }
        //}

        //private void btn_A9_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (Global.m_FormBoardII.IsConnected == false)
        //        {
        //            string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "16功能阀箱网络未连接，操作取消！";
        //            sInfo += "\t\n";
        //            richTextBox_InfoShow.AppendText(sInfo);
        //            return;
        //        }

        //        if (flagCmdToSend == true)
        //        {
        //            string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "尚有操作未完成，请稍后再操作！";
        //            sInfo += "\t\n";
        //            richTextBox_InfoShow.AppendText(sInfo);

        //            return;
        //        }

        //        flagCmdToSend = true;
        //        myEnum_UserButtonOperIdentify = Enum_UserButtonOperIdentify.FaXiang_Space_A9;

        //        //操作记录
        //        string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t阀箱备用通道A9开\t";
        //        Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
        //        Global.myLogStreamWriter.Flush();

        //        {
        //            string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "执行\"阀箱备用通道A9开\"操作";
        //            sInfo += "\t\n";
        //            richTextBox_InfoShow.AppendText(sInfo);
        //        }
        //    }
        //    catch (Exception ex)
        //    { }
        //}

        //private void btn_B9_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (Global.m_FormBoardII.IsConnected == false)
        //        {
        //            string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "16功能阀箱网络未连接，操作取消！";
        //            sInfo += "\t\n";
        //            richTextBox_InfoShow.AppendText(sInfo);
        //            return;
        //        }

        //        if (flagCmdToSend == true)
        //        {
        //            string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "尚有操作未完成，请稍后再操作！";
        //            sInfo += "\t\n";
        //            richTextBox_InfoShow.AppendText(sInfo);

        //            return;
        //        }

        //        flagCmdToSend = true;
        //        myEnum_UserButtonOperIdentify = Enum_UserButtonOperIdentify.FaXiang_Space_B9;

        //        //操作记录
        //        string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t阀箱备用通道B9开\t";
        //        Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
        //        Global.myLogStreamWriter.Flush();

        //        {
        //            string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "执行\"阀箱备用通道B9开\"操作";
        //            sInfo += "\t\n";
        //            richTextBox_InfoShow.AppendText(sInfo);
        //        }
        //    }
        //    catch (Exception ex)
        //    { }
        //}

        //private void btn_AB9_Stop_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (Global.m_FormBoardII.IsConnected == false)
        //        {
        //            string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "16功能阀箱网络未连接，操作取消！";
        //            sInfo += "\t\n";
        //            richTextBox_InfoShow.AppendText(sInfo);
        //            return;
        //        }

        //        if (flagCmdToSend == true)
        //        {
        //            string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "尚有操作未完成，请稍后再操作！";
        //            sInfo += "\t\n";
        //            richTextBox_InfoShow.AppendText(sInfo);

        //            return;
        //        }

        //        flagCmdToSend = true;
        //        myEnum_UserButtonOperIdentify = Enum_UserButtonOperIdentify.FaXiang_Space_AB9_Stop;

        //        //操作记录
        //        string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t阀箱备用通道AB9关闭\t";
        //        Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
        //        Global.myLogStreamWriter.Flush();

        //        {
        //            string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "执行\"阀箱备用通道AB9关闭\"操作";
        //            sInfo += "\t\n";
        //            richTextBox_InfoShow.AppendText(sInfo);
        //        }
        //    }
        //    catch (Exception ex)
        //    { }
        //}

        //private void btn_A10_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (Global.m_FormBoardII.IsConnected == false)
        //        {
        //            string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "16功能阀箱网络未连接，操作取消！";
        //            sInfo += "\t\n";
        //            richTextBox_InfoShow.AppendText(sInfo);
        //            return;
        //        }

        //        if (flagCmdToSend == true)
        //        {
        //            string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "尚有操作未完成，请稍后再操作！";
        //            sInfo += "\t\n";
        //            richTextBox_InfoShow.AppendText(sInfo);

        //            return;
        //        }

        //        flagCmdToSend = true;
        //        myEnum_UserButtonOperIdentify = Enum_UserButtonOperIdentify.FaXiang_Space_A10;

        //        //操作记录
        //        string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t阀箱备用通道A10开\t";
        //        Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
        //        Global.myLogStreamWriter.Flush();

        //        {
        //            string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "执行\"阀箱备用通道A10开\"操作";
        //            sInfo += "\t\n";
        //            richTextBox_InfoShow.AppendText(sInfo);
        //        }
        //    }
        //    catch (Exception ex)
        //    { }
        //}

        //private void btn_B10_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (Global.m_FormBoardII.IsConnected == false)
        //        {
        //            string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "16功能阀箱网络未连接，操作取消！";
        //            sInfo += "\t\n";
        //            richTextBox_InfoShow.AppendText(sInfo);
        //            return;
        //        }

        //        if (flagCmdToSend == true)
        //        {
        //            string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "尚有操作未完成，请稍后再操作！";
        //            sInfo += "\t\n";
        //            richTextBox_InfoShow.AppendText(sInfo);

        //            return;
        //        }

        //        flagCmdToSend = true;
        //        myEnum_UserButtonOperIdentify = Enum_UserButtonOperIdentify.FaXiang_Space_B10;

        //        //操作记录
        //        string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t阀箱备用通道B10开\t";
        //        Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
        //        Global.myLogStreamWriter.Flush();

        //        {
        //            string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "执行\"阀箱备用通道B10开\"操作";
        //            sInfo += "\t\n";
        //            richTextBox_InfoShow.AppendText(sInfo);
        //        }
        //    }
        //    catch (Exception ex)
        //    { }
        //}

        //private void btn_AB10_Stop_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (Global.m_FormBoardII.IsConnected == false)
        //        {
        //            string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "16功能阀箱网络未连接，操作取消！";
        //            sInfo += "\t\n";
        //            richTextBox_InfoShow.AppendText(sInfo);
        //            return;
        //        }

        //        if (flagCmdToSend == true)
        //        {
        //            string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "尚有操作未完成，请稍后再操作！";
        //            sInfo += "\t\n";
        //            richTextBox_InfoShow.AppendText(sInfo);

        //            return;
        //        }

        //        flagCmdToSend = true;
        //        myEnum_UserButtonOperIdentify = Enum_UserButtonOperIdentify.FaXiang_Space_AB10_Stop;

        //        //操作记录
        //        string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t阀箱备用通道AB10关闭\t";
        //        Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
        //        Global.myLogStreamWriter.Flush();

        //        {
        //            string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "执行\"阀箱备用通道AB10关闭\"操作";
        //            sInfo += "\t\n";
        //            richTextBox_InfoShow.AppendText(sInfo);
        //        }
        //    }
        //    catch (Exception ex)
        //    { }
        //}

        //private void btn_A11_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (Global.m_FormBoardII.IsConnected == false)
        //        {
        //            string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "16功能阀箱网络未连接，操作取消！";
        //            sInfo += "\t\n";
        //            richTextBox_InfoShow.AppendText(sInfo);
        //            return;
        //        }

        //        if (flagCmdToSend == true)
        //        {
        //            string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "尚有操作未完成，请稍后再操作！";
        //            sInfo += "\t\n";
        //            richTextBox_InfoShow.AppendText(sInfo);

        //            return;
        //        }

        //        flagCmdToSend = true;
        //        myEnum_UserButtonOperIdentify = Enum_UserButtonOperIdentify.FaXiang_Space_A11;

        //        //操作记录
        //        string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t阀箱备用通道A11开\t";
        //        Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
        //        Global.myLogStreamWriter.Flush();

        //        {
        //            string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "执行\"阀箱备用通道A11开\"操作";
        //            sInfo += "\t\n";
        //            richTextBox_InfoShow.AppendText(sInfo);
        //        }
        //    }
        //    catch (Exception ex)
        //    { }
        //}

        //private void btn_B11_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (Global.m_FormBoardII.IsConnected == false)
        //        {
        //            string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "16功能阀箱网络未连接，操作取消！";
        //            sInfo += "\t\n";
        //            richTextBox_InfoShow.AppendText(sInfo);
        //            return;
        //        }

        //        if (flagCmdToSend == true)
        //        {
        //            string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "尚有操作未完成，请稍后再操作！";
        //            sInfo += "\t\n";
        //            richTextBox_InfoShow.AppendText(sInfo);

        //            return;
        //        }

        //        flagCmdToSend = true;
        //        myEnum_UserButtonOperIdentify = Enum_UserButtonOperIdentify.FaXiang_Space_B11;

        //        //操作记录
        //        string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t阀箱备用通道B11开\t";
        //        Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
        //        Global.myLogStreamWriter.Flush();

        //        {
        //            string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "执行\"阀箱备用通道B11开\"操作";
        //            sInfo += "\t\n";
        //            richTextBox_InfoShow.AppendText(sInfo);
        //        }
        //    }
        //    catch (Exception ex)
        //    { }
        //}

        //private void btn_AB11_Stop_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (Global.m_FormBoardII.IsConnected == false)
        //        {
        //            string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "16功能阀箱网络未连接，操作取消！";
        //            sInfo += "\t\n";
        //            richTextBox_InfoShow.AppendText(sInfo);
        //            return;
        //        }

        //        if (flagCmdToSend == true)
        //        {
        //            string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "尚有操作未完成，请稍后再操作！";
        //            sInfo += "\t\n";
        //            richTextBox_InfoShow.AppendText(sInfo);

        //            return;
        //        }

        //        flagCmdToSend = true;
        //        myEnum_UserButtonOperIdentify = Enum_UserButtonOperIdentify.FaXiang_Space_AB11_Stop;

        //        //操作记录
        //        string sDataLineForTxt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t阀箱备用通道AB11关闭\t";
        //        Global.myLogStreamWriter.WriteLine(sDataLineForTxt);
        //        Global.myLogStreamWriter.Flush();

        //        {
        //            string sInfo = "【" + DateTime.Now.ToString("HH:mm:ss") + "】" + "执行\"阀箱备用通道AB11关闭\"操作";
        //            sInfo += "\t\n";
        //            richTextBox_InfoShow.AppendText(sInfo);
        //        }
        //    }
        //    catch (Exception ex)
        //    { }
        //}

        #endregion

        private void FormMainUserA_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                SaveBasicParasConfig();
            }
            catch (Exception ex)
            { }
        }

        private void GroupBox_FaXiang_Space_Enter(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }


       

        private void btn24v_10_Click(object sender, EventArgs e)
        {

        }

        private void label311_Click(object sender, EventArgs e)
        {

        }

        private void btn_DianJiA_kaiGuang_Click_1(object sender, EventArgs e)
        {
            
        }

        private void label312_Click(object sender, EventArgs e)
        {

        }

        private void GroupBox_MainCtl_Enter(object sender, EventArgs e)
        {

        }
    }
}
