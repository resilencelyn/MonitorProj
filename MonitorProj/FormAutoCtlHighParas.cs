using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace MonitorProj
{
    public partial class FormAutoCtlHighParas : Form
    {//保存自动定向参数
        string fileSavedBasicSys = "";
        private StreamWriter myStreamWriter;
        private FileStream fs;
        public bool SaveTuiJinQiBuChangConfig()
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


        public FormAutoCtlHighParas()
        {
            InitializeComponent();
        }

        private void FormAutoCtlHighParas_Load(object sender, EventArgs e)
        {
            try
            {
                numericUpDown_JiaoDuCha_1.Value = (decimal)Global.AutoCtlHigh_GaoDuCha_1;
                numericUpDown_JiaoDuCha_2.Value = (decimal)Global.AutoCtlHigh_GaoDuCha_2;
                numericUpDown_JiaoDuCha_3.Value = (decimal)Global.AutoCtlHigh_GaoDuCha_3;

                numericUpDown_TuiLiPercent_1.Value = (decimal)Global.AutoCtlHigh_TuiJinPercent_1;
                numericUpDown_TuiLiPercent_2.Value = (decimal)Global.AutoCtlHigh_TuiJinPercent_2;
                numericUpDown_TuiLiPercent_3.Value = (decimal)Global.AutoCtlHigh_TuiJinPercent_3;

                numericUpDown_TuiJinQiWorkTime_1.Value = (decimal)Global.AutoCtlHigh_TuiLiQiWorkTime_1;
                numericUpDown_TuiJinQiWorkTime_2.Value = (decimal)Global.AutoCtlHigh_TuiLiQiWorkTime_2;
                numericUpDown_TuiJinQiWorkTime_3.Value = (decimal)Global.AutoCtlHigh_TuiLiQiWorkTime_3;

                numericUpDown_CompWaitTime_1.Value = (decimal)Global.AutoCtlHigh_TuiLiQiWorkTime_Compare_1;
                numericUpDown_CompWaitTime_2.Value = (decimal)Global.AutoCtlHigh_TuiLiQiWorkTime_Compare_2;
                numericUpDown_CompWaitTime_3.Value = (decimal)Global.AutoCtlHigh_TuiLiQiWorkTime_Compare_3;
            }
            catch (Exception ex)
            { }
        }

        private void FormAutoCtlHighParas_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                this.Dispose();
                Global.myFormAutoCtlHighParas = null;
            }
            catch (Exception ex)
            { }
        }

        private void btn_OK_Click(object sender, EventArgs e)
        {
            try
            {
                Global.AutoCtlHigh_GaoDuCha_1 = (double)numericUpDown_JiaoDuCha_1.Value;
                Global.AutoCtlHigh_GaoDuCha_2 = (double)numericUpDown_JiaoDuCha_2.Value;
                Global.AutoCtlHigh_GaoDuCha_3 = (double)numericUpDown_JiaoDuCha_3.Value;

                Global.AutoCtlHigh_TuiJinPercent_1 = (double)numericUpDown_TuiLiPercent_1.Value;
                Global.AutoCtlHigh_TuiJinPercent_2 = (double)numericUpDown_TuiLiPercent_2.Value;
                Global.AutoCtlHigh_TuiJinPercent_3 = (double)numericUpDown_TuiLiPercent_3.Value;

                Global.AutoCtlHigh_TuiLiQiWorkTime_1 = (double)numericUpDown_TuiJinQiWorkTime_1.Value;
                Global.AutoCtlHigh_TuiLiQiWorkTime_2 = (double)numericUpDown_TuiJinQiWorkTime_2.Value;
                Global.AutoCtlHigh_TuiLiQiWorkTime_3 = (double)numericUpDown_TuiJinQiWorkTime_3.Value;

                Global.AutoCtlHigh_TuiLiQiWorkTime_Compare_1 = (double)numericUpDown_CompWaitTime_1.Value;
                Global.AutoCtlHigh_TuiLiQiWorkTime_Compare_2 = (double)numericUpDown_CompWaitTime_2.Value;
                Global.AutoCtlHigh_TuiLiQiWorkTime_Compare_3 = (double)numericUpDown_CompWaitTime_3.Value;

                SaveTuiJinQiBuChangConfig();
                this.Close();
            }
            catch (Exception ex)
            { }
        }

        private void btn_Cancle_Click(object sender, EventArgs e)
        {
            try
            {
                numericUpDown_JiaoDuCha_1.Value = (decimal)Global.AutoCtlHigh_GaoDuCha_1;
                numericUpDown_JiaoDuCha_2.Value = (decimal)Global.AutoCtlHigh_GaoDuCha_2;
                numericUpDown_JiaoDuCha_3.Value = (decimal)Global.AutoCtlHigh_GaoDuCha_3;

                numericUpDown_TuiLiPercent_1.Value = (decimal)Global.AutoCtlHigh_TuiJinPercent_1;
                numericUpDown_TuiLiPercent_2.Value = (decimal)Global.AutoCtlHigh_TuiJinPercent_2;
                numericUpDown_TuiLiPercent_3.Value = (decimal)Global.AutoCtlHigh_TuiJinPercent_3;

                numericUpDown_TuiJinQiWorkTime_1.Value = (decimal)Global.AutoCtlHigh_TuiLiQiWorkTime_1;
                numericUpDown_TuiJinQiWorkTime_2.Value = (decimal)Global.AutoCtlHigh_TuiLiQiWorkTime_2;
                numericUpDown_TuiJinQiWorkTime_3.Value = (decimal)Global.AutoCtlHigh_TuiLiQiWorkTime_3;

                numericUpDown_CompWaitTime_1.Value = (decimal)Global.AutoCtlHigh_TuiLiQiWorkTime_Compare_1;
                numericUpDown_CompWaitTime_2.Value = (decimal)Global.AutoCtlHigh_TuiLiQiWorkTime_Compare_2;
                numericUpDown_CompWaitTime_3.Value = (decimal)Global.AutoCtlHigh_TuiLiQiWorkTime_Compare_3;
            }
            catch (Exception ex)
            { }
        }

        private void btn_Close_Click(object sender, EventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception ex)
            { }
        }
    }
}
