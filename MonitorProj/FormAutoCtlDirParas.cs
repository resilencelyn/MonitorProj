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
    public partial class FormAutoCtlDirParas : Form
    {
        //保存自动定向参数
        string fileSavedBasicSys = "";
        private StreamWriter myStreamWriter;
        private FileStream fs;
        public bool SaveTuiJinQiBuChangConfig()
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

        public FormAutoCtlDirParas()
        {
            InitializeComponent();
        }

        private void FormAutoCtlDirParas_Load(object sender, EventArgs e)
        {
            try
            {
                numericUpDown_JiaoDuCha_1.Value = (decimal)Global.AutoCtlDir_JiaoDuCha_1;
                numericUpDown_JiaoDuCha_2.Value = (decimal)Global.AutoCtlDir_JiaoDuCha_2;
                numericUpDown_JiaoDuCha_3.Value = (decimal)Global.AutoCtlDir_JiaoDuCha_3;

                numericUpDown_TuiLiPercent_1.Value = (decimal)Global.AutoCtlDir_TuiJinPercent_1;
                numericUpDown_TuiLiPercent_2.Value = (decimal)Global.AutoCtlDir_TuiJinPercent_2;
                numericUpDown_TuiLiPercent_3.Value = (decimal)Global.AutoCtlDir_TuiJinPercent_3;

                numericUpDown_TuiJinQiWorkTime_1.Value = (decimal)Global.AutoCtlDir_TuiLiQiWorkTime_1;
                numericUpDown_TuiJinQiWorkTime_2.Value = (decimal)Global.AutoCtlDir_TuiLiQiWorkTime_2;
                numericUpDown_TuiJinQiWorkTime_3.Value = (decimal)Global.AutoCtlDir_TuiLiQiWorkTime_3;

                numericUpDown_CompWaitTime_1.Value = (decimal)Global.AutoCtlDir_TuiLiQiWorkTime_Compare_1;
                numericUpDown_CompWaitTime_2.Value = (decimal)Global.AutoCtlDir_TuiLiQiWorkTime_Compare_2;
                numericUpDown_CompWaitTime_3.Value = (decimal)Global.AutoCtlDir_TuiLiQiWorkTime_Compare_3;
            }
            catch (Exception ex)
            { }
        }

        private void btn_OK_Click(object sender, EventArgs e)
        {
            try
            {
                Global.AutoCtlDir_JiaoDuCha_1 = (double)numericUpDown_JiaoDuCha_1.Value;
                Global.AutoCtlDir_JiaoDuCha_2 = (double)numericUpDown_JiaoDuCha_2.Value;
                Global.AutoCtlDir_JiaoDuCha_3 = (double)numericUpDown_JiaoDuCha_3.Value;

                Global.AutoCtlDir_TuiJinPercent_1 = (double)numericUpDown_TuiLiPercent_1.Value;
                Global.AutoCtlDir_TuiJinPercent_2 = (double)numericUpDown_TuiLiPercent_2.Value;
                Global.AutoCtlDir_TuiJinPercent_3 = (double)numericUpDown_TuiLiPercent_3.Value;

                Global.AutoCtlDir_TuiLiQiWorkTime_1 = (double)numericUpDown_TuiJinQiWorkTime_1.Value;
                Global.AutoCtlDir_TuiLiQiWorkTime_2 = (double)numericUpDown_TuiJinQiWorkTime_2.Value;
                Global.AutoCtlDir_TuiLiQiWorkTime_3 = (double)numericUpDown_TuiJinQiWorkTime_3.Value;

                Global.AutoCtlDir_TuiLiQiWorkTime_Compare_1 = (double)numericUpDown_CompWaitTime_1.Value;
                Global.AutoCtlDir_TuiLiQiWorkTime_Compare_2 = (double)numericUpDown_CompWaitTime_2.Value;
                Global.AutoCtlDir_TuiLiQiWorkTime_Compare_3 = (double)numericUpDown_CompWaitTime_3.Value;
                
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
                numericUpDown_JiaoDuCha_1.Value = (decimal)Global.AutoCtlDir_JiaoDuCha_1;
                numericUpDown_JiaoDuCha_2.Value = (decimal)Global.AutoCtlDir_JiaoDuCha_2;
                numericUpDown_JiaoDuCha_3.Value = (decimal)Global.AutoCtlDir_JiaoDuCha_3;

                numericUpDown_TuiLiPercent_1.Value = (decimal)Global.AutoCtlDir_TuiJinPercent_1;
                numericUpDown_TuiLiPercent_2.Value = (decimal)Global.AutoCtlDir_TuiJinPercent_2;
                numericUpDown_TuiLiPercent_3.Value = (decimal)Global.AutoCtlDir_TuiJinPercent_3;

                numericUpDown_TuiJinQiWorkTime_1.Value = (decimal)Global.AutoCtlDir_TuiLiQiWorkTime_1;
                numericUpDown_TuiJinQiWorkTime_2.Value = (decimal)Global.AutoCtlDir_TuiLiQiWorkTime_2;
                numericUpDown_TuiJinQiWorkTime_3.Value = (decimal)Global.AutoCtlDir_TuiLiQiWorkTime_3;

                numericUpDown_CompWaitTime_1.Value = (decimal)Global.AutoCtlDir_TuiLiQiWorkTime_Compare_1;
                numericUpDown_CompWaitTime_2.Value = (decimal)Global.AutoCtlDir_TuiLiQiWorkTime_Compare_2;
                numericUpDown_CompWaitTime_3.Value = (decimal)Global.AutoCtlDir_TuiLiQiWorkTime_Compare_3;
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

        private void FormAutoCtlDirParas_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                this.Dispose();
                Global.myFormAutoCtlDirParas = null;
            }
            catch (Exception ex)
            { }
        }
    }
}
