using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace MonitorProj
{


    public class DataRecvToSaveClass
    {
        private int intervalDataToSave = 1000;
        public int IntervalDataToSave
        {
            set
            {
                intervalDataToSave = value;
                myTimer_Status_Data_To_Save.Interval = intervalDataToSave;
            }
            get
            {
                return intervalDataToSave;
            }
        }
        public Timer myTimer_Status_Data_To_Save;
        public Struct_Status_Data_To_Save myStruct_Status_Data_To_Save = new Struct_Status_Data_To_Save();

        public DataRecvToSaveClass()
        {
            try
            {
                //接收数据，各板卡状态
                Global.statusFilePathSaved = Global.statusFilePathSaved + "\\状态数据-" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".csv";
                Global.fsStatus = new FileStream(Global.statusFilePathSaved, FileMode.Create, FileAccess.Write, FileShare.Read);
                Global.myStatusStreamWriter = new StreamWriter(Global.fsStatus, Encoding.Unicode);

                //首行标题
                #region 首行标题
                string sDataLineForTxt = "时间\t" +
                    "电压A(直流绝缘检测板61)\t" +
                    "电流A(直流绝缘检测板61)\t" +
                    "绝缘A(直流绝缘检测板61)\t" +
                    "温度A(直流绝缘检测板61)\t" +
                    "电压B(直流绝缘检测板61)\t" +
                    "电流B(直流绝缘检测板61)\t" +
                    "绝缘B(直流绝缘检测板61)\t" +
                    "温度B(直流绝缘检测板61)\t" +
                    "外温(直流绝缘检测板61)\t" +
                    "电压A(直流绝缘检测板62)\t" +
                    "电流A(直流绝缘检测板62)\t" +
                    "绝缘A(直流绝缘检测板62)\t" +
                    "温度A(直流绝缘检测板62)\t" +
                    "电压B(直流绝缘检测板62)\t" +
                    "电流B(直流绝缘检测板62)\t" +
                    "绝缘B(直流绝缘检测板62)\t" +
                    "温度B(直流绝缘检测板62)\t" +
                    "外温(直流绝缘检测板62)\t" +
                    "电压A(直流绝缘检测板63)\t" +
                    "电流A(直流绝缘检测板63)\t" +
                    "绝缘A(直流绝缘检测板63)\t" +
                    "温度A(直流绝缘检测板63)\t" +
                    "电压B(直流绝缘检测板63)\t" +
                    "电流B(直流绝缘检测板63)\t" +
                    "绝缘B(直流绝缘检测板63)\t" +
                    "温度B(直流绝缘检测板63)\t" +
                    "外温(直流绝缘检测板63)\t" +

                    "电压(大功率直流绝缘检测板70)\t" +
                    "电流(大功率直流绝缘检测板70)\t" +
                    "绝缘(大功率直流绝缘检测板70)\t" +
                    "温度(大功率直流绝缘检测板70)\t" +
                    "电压(大功率直流绝缘检测板71)\t" +
                    "电流(大功率直流绝缘检测板71)\t" +
                    "绝缘(大功率直流绝缘检测板71)\t" +
                    "温度(大功率直流绝缘检测板71)\t" +
                    "电压(大功率直流绝缘检测板72)\t" +
                    "电流(大功率直流绝缘检测板72)\t" +
                    "绝缘(大功率直流绝缘检测板72)\t" +
                    "温度(大功率直流绝缘检测板72)\t" +

                    "电压(大功率直流绝缘检测控制板)\t" +
                    "电流(大功率直流绝缘检测控制板)\t" +

                    "电压(交流绝缘检测板)\t" +
                    "电流(交流绝缘检测板)\t" +

                    "漏水(摄像机电源继电器板1)\t" +
                    "漏水(摄像机电源继电器板2)\t" +
                    "漏水(传感器电源继电器板1)\t" +
                    "漏水(传感器电源继电器板2)\t" +

                    "电压1_1(油箱采集板)\t" +
                    "电压1_2(油箱采集板)\t" +
                    "电压1_3(油箱采集板)\t" +
                    "电压1_4(油箱采集板)\t" +
                    "电压2_1(油箱采集板)\t" +
                    "电压2_2(油箱采集板)\t" +
                    "电压2_3(油箱采集板)\t" +
                    "电压2_4(油箱采集板)\t" +
                    "电压3_1(油箱采集板)\t" +
                    "电压3_2(油箱采集板)\t" +
                    "电压3_3(油箱采集板)\t" +
                    "电压3_4(油箱采集板)\t" +
                    "电压4_1(油箱采集板)\t" +
                    "电压4_2(油箱采集板)\t" +
                    "电压4_3(油箱采集板)\t" +
                    "电压4_4(油箱采集板)\t" +

                    "电压(灯舱继电器板1)\t" +
                    "电压(灯舱继电器板2)\t" +

                    "转速(工况采集板)\t" +
                    "到位1(工况采集板)\t" +
                    "到位2(工况采集板)\t" +
                    "到位3(工况采集板)\t" +
                    "到位4(工况采集板)\t" +
                    "到位5(工况采集板)\t" +
                    "到位6(工况采集板)\t" +

                    "MeauringValue（绝缘检测仪1）\t" +
                    "Alarm1Value（绝缘检测仪1）\t" +
                    "Alarm2Value（绝缘检测仪1）\t" +
                    "K1_K2_OnOff（绝缘检测仪1）\t" +
                    "Alarm1_2_None（绝缘检测仪1）\t" +
                    "AC_DC_Fault（绝缘检测仪1）\t" +

                    "MeauringValue（绝缘检测仪2）\t" +
                    "Alarm1Value（绝缘检测仪2）\t" +
                    "Alarm2Value（绝缘检测仪2）\t" +
                    "K1_K2_OnOff（绝缘检测仪2）\t" +
                    "Alarm1_2_None（绝缘检测仪2）\t" +
                    "AC_DC_Fault（绝缘检测仪2）\t" +
                    
                    "高度\t" +

                    "X轴磁场\t" +
                    "Y轴磁场\t" +
                    "Z轴磁场\t" +
                    "X轴角度\t" +
                    "Y轴角度\t" +
                    "Z轴角度\t" +

                    "漏水1（电机检测）\t" +
                    "漏水2（电机检测）\t" +
                    "漏水3（电机检测）\t" +
                    "漏水4（电机检测）\t" +
                    "漏水5（电机检测）\t" +
                    "温度1（电机检测）\t" +
                    "温度2（电机检测）\t" +
                    "温度3（电机检测）\t" +

                    "X(控制盒)\t" +
                    "Y(控制盒)\t" +
                    "Z(控制盒)\t" +
                    "V(控制盒)\t" +
                    "备1(控制盒)\t" +
                    "备2(控制盒)\t" +
                    "KKInfo(控制盒)\t" +

                    "坏帧计数(8阀箱)\t" +
                    "数字量输入8(8阀箱)\t" +
                    "数字量输入7(8阀箱)\t" +
                    "数字量输入6(8阀箱)\t" +
                    "数字量输入5(8阀箱)\t" +
                    "数字量输入4(8阀箱)\t" +
                    "数字量输入3(8阀箱)\t" +
                    "数字量输入2(8阀箱)\t" +
                    "数字量输入1(8阀箱)\t" +
                    "主24V(8阀箱)\t" +
                    "模拟15V(8阀箱)\t" +
                    "数字5V(8阀箱)\t" +
                    "模拟5V(8阀箱)\t" +
                    "电流反馈1(8阀箱)\t" +
                    "电流反馈2(8阀箱)\t" +
                    "模拟输入17(8阀箱)\t" +
                    "模拟输入18(8阀箱)\t" +
                    "模拟输入19(8阀箱)\t" +
                    "模拟输入20(8阀箱)\t" +
                    "模拟输入1(8阀箱)\t" +
                    "模拟输入2(8阀箱)\t" +
                    "模拟输入3(8阀箱)\t" +
                    "模拟输入4(8阀箱)\t" +
                    "模拟输入5(8阀箱)\t" +
                    "模拟输入6(8阀箱)\t" +
                    "模拟输入7(8阀箱)\t" +
                    "模拟输入8(8阀箱)\t" +
                    "模拟输入9(8阀箱)\t" +
                    "模拟输入10(8阀箱)\t" +
                    "模拟输入11(8阀箱)\t" +
                    "模拟输入12(8阀箱)\t" +
                    "模拟输入13(8阀箱)\t" +
                    "模拟输入14(8阀箱)\t" +
                    "模拟输入15(8阀箱)\t" +
                    "模拟输入16(8阀箱)\t" +
                    "温度(8阀箱)\t" +

                    "坏帧计数(16阀箱)\t" +
                    "数字量输入8(16阀箱)\t" +
                    "数字量输入7(16阀箱)\t" +
                    "数字量输入6(16阀箱)\t" +
                    "数字量输入5(16阀箱)\t" +
                    "数字量输入4(16阀箱)\t" +
                    "数字量输入3(16阀箱)\t" +
                    "数字量输入2(16阀箱)\t" +
                    "数字量输入1(16阀箱)\t" +
                    "PWM电流反馈1_2(16阀箱)\t" +
                    "PWM电流反馈3_4(16阀箱)\t" +
                    "PWM电流反馈5_6(16阀箱)\t" +
                    "PWM电流反馈7_8(16阀箱)\t" +
                    "PWM电流反馈9_10(16阀箱)\t" +
                    "PWM电流反馈11_12(16阀箱)\t" +
                    "PWM电流反馈13_14(16阀箱)\t" +
                    "PWM电流反馈15_16(16阀箱)\t" +
                    "VCCD(16阀箱)\t" +
                    "VCCA(16阀箱)\t" +
                    "外部模拟输入1(16阀箱)\t" +
                    "外部模拟输入2(16阀箱)\t" +
                    "外部模拟输入3(16阀箱)\t" +
                    "外部模拟输入4(16阀箱)\t" +
                    "主24V(16阀箱)\t" +
                    "主15V(16阀箱)\t" +
                    "电流反馈DOUT1_8(16阀箱)\t" +
                    "电流反馈DOUT9_16(16阀箱)\t" +
                    "温度(16阀箱)\t" +

                    "电压(16阀箱)\t" +
                    "电流(16阀箱)\t" +
                    "压力(16阀箱)\t" +
                    "漏水(16阀箱)\t" +

                    "三相变压器初级A相电流\t" +
                    "三相变压器初级B相电流\t" +
                    "三相变压器初级C相电流\t" +
                    "三相变压器初级AB相电压\t" +
                    "三相变压器初级BC相电压\t" +
                    "单相变压器初级电流\t" +
                    "三相变压器次级电流\t" +
                    "单相变压器次级电流\t" +
                    "三相变压器次级电压\t" +
                    "单相变压器次级电压\t" +
                    "T1输入电压低报警\t" +
                    "T1输入电压高报警\t" +
                    "T1输出电压高报警\t" +
                    "T1输入电流高报警\t" +
                    "T1输出电流高报警\t" +

                    "T2输入电压低报警\t" +
                    "T2输入电压高报警\t" +
                    "T2输出电压高报警\t" +
                    "T2输入电流高报警\t" +
                    "T2输出电流高报警\t" +

                    "T1输入电流A相变送器故障\t" +
                    "T1输入电流B相变送器故障\t" +
                    "T1输入电流C相变送器故障\t" +
                    "T1输入电压AB相变送器故障\t" +
                    "T1输入电压BC相变送器故障\t" +
                    "T2输入电流变送器故障\t" +
                    "T1输出电流变送器故障\t" +
                    "T2输出电流变送器故障\t" +
                    "T1输出电压变送器故障\t" +
                    "T2输出电压变送器故障\t" +
                    "相序断相不平衡故障（8%）\t" +
                    "T1_高压绝缘故障\t" +
                    "T2_高压绝缘故障\t" +
                    "急停故障\t" +

                    "T1输入电压低低故障\t" +
                    "T1输入电压高高故障\t" +
                    "T1输出电压高高故障\t" +

                    "T1输入电流高高故障\t" +
                    "T1输出电流高高故障\t" +

                    "T2输入电压低低故障\t" +
                    "T2输入电压高高故障\t" +
                    "T2输出电压高高故障\t" +

                    "T2输入电流高高故障\t" +
                    "T2输出电流高高故障\t"
                    
                    ;
                Global.myStatusStreamWriter.WriteLine(sDataLineForTxt);
                Global.myStatusStreamWriter.Flush();
                #endregion

                myTimer_Status_Data_To_Save = new Timer();
                myTimer_Status_Data_To_Save.Interval = intervalDataToSave;
                myTimer_Status_Data_To_Save.Tick += new EventHandler(myTimer_Status_Data_To_Save_Tick);
                myTimer_Status_Data_To_Save.Start();
            }
            catch (Exception ex)
            { }
        }

        public void Stop()
        {
            try
            {
                myTimer_Status_Data_To_Save.Stop();
                Global.myStatusStreamWriter.Flush();
                Global.myStatusStreamWriter.Close();
            }
            catch (Exception ex)
            { }
        }

        private UInt64 isDataComingCountLast = 0; //上次计数
        private void myTimer_Status_Data_To_Save_Tick(object sender, EventArgs e)
        {
            try
            {
                if (Global.isDataComing)
                {
                    string ss = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "\t" +
                        //直流绝缘检测板数据（0x61、0x62、0x63）
                        myStruct_Status_Data_To_Save.DC_Insulation_Detection_VA_61 + "\t" +
                        myStruct_Status_Data_To_Save.DC_Insulation_Detection_IA_61 + "\t" +
                        myStruct_Status_Data_To_Save.DC_Insulation_Detection_GA_61 + "\t" +
                        myStruct_Status_Data_To_Save.DC_Insulation_Detection_TA_61 + "\t" +
                        myStruct_Status_Data_To_Save.DC_Insulation_Detection_VB_61 + "\t" +
                        myStruct_Status_Data_To_Save.DC_Insulation_Detection_IB_61 + "\t" +
                        myStruct_Status_Data_To_Save.DC_Insulation_Detection_GB_61 + "\t" +
                        myStruct_Status_Data_To_Save.DC_Insulation_Detection_TB_61 + "\t" +
                        myStruct_Status_Data_To_Save.DC_Insulation_Detection_TO_61 + "\t" +
                        myStruct_Status_Data_To_Save.DC_Insulation_Detection_VA_62 + "\t" +
                        myStruct_Status_Data_To_Save.DC_Insulation_Detection_IA_62 + "\t" +
                        myStruct_Status_Data_To_Save.DC_Insulation_Detection_GA_62 + "\t" +
                        myStruct_Status_Data_To_Save.DC_Insulation_Detection_TA_62 + "\t" +
                        myStruct_Status_Data_To_Save.DC_Insulation_Detection_VB_62 + "\t" +
                        myStruct_Status_Data_To_Save.DC_Insulation_Detection_IB_62 + "\t" +
                        myStruct_Status_Data_To_Save.DC_Insulation_Detection_GB_62 + "\t" +
                        myStruct_Status_Data_To_Save.DC_Insulation_Detection_TB_62 + "\t" +
                        myStruct_Status_Data_To_Save.DC_Insulation_Detection_TO_62 + "\t" +
                        myStruct_Status_Data_To_Save.DC_Insulation_Detection_VA_63 + "\t" +
                        myStruct_Status_Data_To_Save.DC_Insulation_Detection_IA_63 + "\t" +
                        myStruct_Status_Data_To_Save.DC_Insulation_Detection_GA_63 + "\t" +
                        myStruct_Status_Data_To_Save.DC_Insulation_Detection_TA_63 + "\t" +
                        myStruct_Status_Data_To_Save.DC_Insulation_Detection_VB_63 + "\t" +
                        myStruct_Status_Data_To_Save.DC_Insulation_Detection_IB_63 + "\t" +
                        myStruct_Status_Data_To_Save.DC_Insulation_Detection_GB_63 + "\t" +
                        myStruct_Status_Data_To_Save.DC_Insulation_Detection_TB_63 + "\t" +
                        myStruct_Status_Data_To_Save.DC_Insulation_Detection_TO_63 + "\t" +
                        //大功率直流绝缘检测板数据解析：（0x70、0x71、0x72）
                        myStruct_Status_Data_To_Save.High_Power_DC_Insulation_Detection_V_70 + "\t" +
                        myStruct_Status_Data_To_Save.High_Power_DC_Insulation_Detection_I_70 + "\t" +
                        myStruct_Status_Data_To_Save.High_Power_DC_Insulation_Detection_G_70 + "\t" +
                        myStruct_Status_Data_To_Save.High_Power_DC_Insulation_Detection_T_70 + "\t" +
                        myStruct_Status_Data_To_Save.High_Power_DC_Insulation_Detection_V_71 + "\t" +
                        myStruct_Status_Data_To_Save.High_Power_DC_Insulation_Detection_I_71 + "\t" +
                        myStruct_Status_Data_To_Save.High_Power_DC_Insulation_Detection_G_71 + "\t" +
                        myStruct_Status_Data_To_Save.High_Power_DC_Insulation_Detection_T_71 + "\t" +
                        myStruct_Status_Data_To_Save.High_Power_DC_Insulation_Detection_V_72 + "\t" +
                        myStruct_Status_Data_To_Save.High_Power_DC_Insulation_Detection_I_72 + "\t" +
                        myStruct_Status_Data_To_Save.High_Power_DC_Insulation_Detection_G_72 + "\t" +
                        myStruct_Status_Data_To_Save.High_Power_DC_Insulation_Detection_T_72 + "\t" +
                        //大功率直流绝缘检测板控制板数据解析：（0x79）
                        myStruct_Status_Data_To_Save.Control_Panel_High_Power_DC_Insulation_Detection_V_79 + "\t" +
                        myStruct_Status_Data_To_Save.Control_Panel_High_Power_DC_Insulation_Detection_I_79 + "\t" +
                        //交流绝缘监测板：(0x80)
                        myStruct_Status_Data_To_Save.AC_Insulation_Detection_V_80 + "\t" +
                        myStruct_Status_Data_To_Save.AC_Insulation_Detection_I_80 + "\t" +
                        //接口箱继电器板（0x25、0x26、0x28、0x29）
                        myStruct_Status_Data_To_Save.Power_Relay_Board_V_25 + "\t" +
                        myStruct_Status_Data_To_Save.Power_Relay_Board_V_26 + "\t" +
                        myStruct_Status_Data_To_Save.Power_Relay_Board_V_28 + "\t" +
                        myStruct_Status_Data_To_Save.Power_Relay_Board_V_29 + "\t" +
                        //传感器接口箱信号采集板,油箱检测：0x40
                        myStruct_Status_Data_To_Save.Tank_Detection_CH1_1 + "\t" +
                        myStruct_Status_Data_To_Save.Tank_Detection_CH1_2 + "\t" +
                        myStruct_Status_Data_To_Save.Tank_Detection_CH1_3 + "\t" +
                        myStruct_Status_Data_To_Save.Tank_Detection_CH1_4 + "\t" +
                        myStruct_Status_Data_To_Save.Tank_Detection_CH2_1 + "\t" +
                        myStruct_Status_Data_To_Save.Tank_Detection_CH2_2 + "\t" +
                        myStruct_Status_Data_To_Save.Tank_Detection_CH2_3 + "\t" +
                        myStruct_Status_Data_To_Save.Tank_Detection_CH2_4 + "\t" +
                        myStruct_Status_Data_To_Save.Tank_Detection_CH3_1 + "\t" +
                        myStruct_Status_Data_To_Save.Tank_Detection_CH3_2 + "\t" +
                        myStruct_Status_Data_To_Save.Tank_Detection_CH3_3 + "\t" +
                        myStruct_Status_Data_To_Save.Tank_Detection_CH3_4 + "\t" +
                        myStruct_Status_Data_To_Save.Tank_Detection_CH4_1 + "\t" +
                        myStruct_Status_Data_To_Save.Tank_Detection_CH4_2 + "\t" +
                        myStruct_Status_Data_To_Save.Tank_Detection_CH4_3 + "\t" +
                        myStruct_Status_Data_To_Save.Tank_Detection_CH4_4 + "\t" +
                        //灯舱继电器板（0x21、0x22）
                        myStruct_Status_Data_To_Save.Light_Relay_Board_V_21 + "\t" +
                        myStruct_Status_Data_To_Save.Light_Relay_Board_V_22 + "\t" +
                        //工况板
                        myStruct_Status_Data_To_Save.Work_Station_RotateSpeed + "\t" +
                        myStruct_Status_Data_To_Save.WorkStation_B + "\t" +
                        myStruct_Status_Data_To_Save.WorkStation_C + "\t" +
                        myStruct_Status_Data_To_Save.WorkStation_D + "\t" +
                        myStruct_Status_Data_To_Save.WorkStation_E + "\t" +
                        myStruct_Status_Data_To_Save.WorkStation_F + "\t" +
                        myStruct_Status_Data_To_Save.WorkStation_G + "\t" +
                        //绝缘检测仪1
                        myStruct_Status_Data_To_Save.sMeauringValue_1 + "\t" +
                        myStruct_Status_Data_To_Save.sAlarm1Value_1 + "\t" +
                        myStruct_Status_Data_To_Save.sAlarm2Value_1 + "\t" +
                        myStruct_Status_Data_To_Save.sK1_K2_OnOff_1 + "\t" +
                        myStruct_Status_Data_To_Save.sAlarm1_2_None_1 + "\t" +
                        myStruct_Status_Data_To_Save.sAC_DC_Fault_1 + "\t" +
                        //绝缘检测仪2
                        myStruct_Status_Data_To_Save.sMeauringValue_2 + "\t" +
                        myStruct_Status_Data_To_Save.sAlarm1Value_2 + "\t" +
                        myStruct_Status_Data_To_Save.sAlarm2Value_2 + "\t" +
                        myStruct_Status_Data_To_Save.sK1_K2_OnOff_2 + "\t" +
                        myStruct_Status_Data_To_Save.sAlarm1_2_None_2 + "\t" +
                        myStruct_Status_Data_To_Save.sAC_DC_Fault_2 + "\t" +
                        //高度计
                        myStruct_Status_Data_To_Save.Hight_Measure_Height + "\t" +
                        //罗盘
                        myStruct_Status_Data_To_Save.Rotate_Panel_HX + "\t" +
                        myStruct_Status_Data_To_Save.Rotate_Panel_HY + "\t" +
                        myStruct_Status_Data_To_Save.Rotate_Panel_HZ + "\t" +
                        myStruct_Status_Data_To_Save.Rotate_Panel_Roll + "\t" +
                        myStruct_Status_Data_To_Save.Rotate_Panel_Pitch + "\t" +
                        myStruct_Status_Data_To_Save.Rotate_Panel_Yaw + "\t" +

                        //电机检测
                        myStruct_Status_Data_To_Save.DanJi_T_Para_1 + "\t" +
                        myStruct_Status_Data_To_Save.DanJi_T_Para_2 + "\t" +
                        myStruct_Status_Data_To_Save.DanJi_T_Para_3 + "\t" +
                        myStruct_Status_Data_To_Save.DanJi_T_Para_4 + "\t" +
                        myStruct_Status_Data_To_Save.DanJi_T_Para_5 + "\t" +
                        myStruct_Status_Data_To_Save.DanJi_T_Para_6 + "\t" +
                        myStruct_Status_Data_To_Save.DanJi_T_Para_7 + "\t" +
                        myStruct_Status_Data_To_Save.DanJi_T_Para_8 + "\t" +

                        //控制盒（0x10）
                        myStruct_Status_Data_To_Save.Water_Control_Box_RotAxisX + "\t" +
                        myStruct_Status_Data_To_Save.Water_Control_Box_RotAxisY + "\t" +
                        myStruct_Status_Data_To_Save.Water_Control_Box_RotAxisZ + "\t" +
                        myStruct_Status_Data_To_Save.Water_Control_Box_RotAxisV + "\t" +
                        myStruct_Status_Data_To_Save.Water_Control_Box_Space1 + "\t" +
                        myStruct_Status_Data_To_Save.Water_Control_Box_Space2 + "\t" +
                        myStruct_Status_Data_To_Save.Water_Control_Box_KKInfo + "\t" +
                        //8功能阀箱
                        myStruct_Status_Data_To_Save.ServoValvePackI_Received_Bad_CRCs + "\t" +
                        myStruct_Status_Data_To_Save.ServoValvePackI_bDIN8 + "\t" +
                        myStruct_Status_Data_To_Save.ServoValvePackI_bDIN7 + "\t" +
                        myStruct_Status_Data_To_Save.ServoValvePackI_bDIN6 + "\t" +
                        myStruct_Status_Data_To_Save.ServoValvePackI_bDIN5 + "\t" +
                        myStruct_Status_Data_To_Save.ServoValvePackI_bDIN4 + "\t" +
                        myStruct_Status_Data_To_Save.ServoValvePackI_bDIN3 + "\t" +
                        myStruct_Status_Data_To_Save.ServoValvePackI_bDIN2 + "\t" +
                        myStruct_Status_Data_To_Save.ServoValvePackI_bDIN1 + "\t" +
                        myStruct_Status_Data_To_Save.ServoValvePackI_Main_Supply_Voltage_24V + "\t" +
                        myStruct_Status_Data_To_Save.ServoValvePackI_Analog_Supply_Voltage_15V + "\t" +
                        myStruct_Status_Data_To_Save.ServoValvePackI_Digital_Supply_Voltage_5V + "\t" +
                        myStruct_Status_Data_To_Save.ServoValvePackI_Analog_Supply_Voltage_5V + "\t" +
                        myStruct_Status_Data_To_Save.ServoValvePackI_Current_Feedback_1 + "\t" +
                        myStruct_Status_Data_To_Save.ServoValvePackI_Current_Feedback_2 + "\t" +
                        myStruct_Status_Data_To_Save.ServoValvePackI_Analog_Input_17 + "\t" +
                        myStruct_Status_Data_To_Save.ServoValvePackI_Analog_Input_18 + "\t" +
                        myStruct_Status_Data_To_Save.ServoValvePackI_Analog_Input_19 + "\t" +
                        myStruct_Status_Data_To_Save.ServoValvePackI_Analog_Input_20 + "\t" +
                        myStruct_Status_Data_To_Save.ServoValvePackI_Analog_Input_1 + "\t" +
                        myStruct_Status_Data_To_Save.ServoValvePackI_Analog_Input_2 + "\t" +
                        myStruct_Status_Data_To_Save.ServoValvePackI_Analog_Input_3 + "\t" +
                        myStruct_Status_Data_To_Save.ServoValvePackI_Analog_Input_4 + "\t" +
                        myStruct_Status_Data_To_Save.ServoValvePackI_Analog_Input_5 + "\t" +
                        myStruct_Status_Data_To_Save.ServoValvePackI_Analog_Input_6 + "\t" +
                        myStruct_Status_Data_To_Save.ServoValvePackI_Analog_Input_7 + "\t" +
                        myStruct_Status_Data_To_Save.ServoValvePackI_Analog_Input_8 + "\t" +
                        myStruct_Status_Data_To_Save.ServoValvePackI_Analog_Input_9 + "\t" +
                        myStruct_Status_Data_To_Save.ServoValvePackI_Analog_Input_10 + "\t" +
                        myStruct_Status_Data_To_Save.ServoValvePackI_Analog_Input_11 + "\t" +
                        myStruct_Status_Data_To_Save.ServoValvePackI_Analog_Input_12 + "\t" +
                        myStruct_Status_Data_To_Save.ServoValvePackI_Analog_Input_13 + "\t" +
                        myStruct_Status_Data_To_Save.ServoValvePackI_Analog_Input_14 + "\t" +
                        myStruct_Status_Data_To_Save.ServoValvePackI_Analog_Input_15 + "\t" +
                        myStruct_Status_Data_To_Save.ServoValvePackI_Analog_Input_16 + "\t" +
                        myStruct_Status_Data_To_Save.ServoValvePackI_Temperature + "\t" +
                        //16功能阀箱（国外版）
                        myStruct_Status_Data_To_Save.ServoValvePackII_Received_Bad_CRCs + "\t" +
                        myStruct_Status_Data_To_Save.ServoValvePackII_bDIN8 + "\t" +
                        myStruct_Status_Data_To_Save.ServoValvePackII_bDIN7 + "\t" +
                        myStruct_Status_Data_To_Save.ServoValvePackII_bDIN6 + "\t" +
                        myStruct_Status_Data_To_Save.ServoValvePackII_bDIN5 + "\t" +
                        myStruct_Status_Data_To_Save.ServoValvePackII_bDIN4 + "\t" +
                        myStruct_Status_Data_To_Save.ServoValvePackII_bDIN3 + "\t" +
                        myStruct_Status_Data_To_Save.ServoValvePackII_bDIN2 + "\t" +
                        myStruct_Status_Data_To_Save.ServoValvePackII_bDIN1 + "\t" +
                        myStruct_Status_Data_To_Save.ServoValvePackII_Current_Feedback_PWM_1_2 + "\t" +
                        myStruct_Status_Data_To_Save.ServoValvePackII_Current_Feedback_PWM_3_4 + "\t" +
                        myStruct_Status_Data_To_Save.ServoValvePackII_Current_Feedback_PWM_5_6 + "\t" +
                        myStruct_Status_Data_To_Save.ServoValvePackII_Current_Feedback_PWM_7_8 + "\t" +
                        myStruct_Status_Data_To_Save.ServoValvePackII_Current_Feedback_PWM_9_10 + "\t" +
                        myStruct_Status_Data_To_Save.ServoValvePackII_Current_Feedback_PWM_11_12 + "\t" +
                        myStruct_Status_Data_To_Save.ServoValvePackII_Current_Feedback_PWM_13_14 + "\t" +
                        myStruct_Status_Data_To_Save.ServoValvePackII_Current_Feedback_PWM_15_16 + "\t" +
                        myStruct_Status_Data_To_Save.ServoValvePackII_VCCD + "\t" +
                        myStruct_Status_Data_To_Save.ServoValvePackII_VCCA + "\t" +
                        myStruct_Status_Data_To_Save.ServoValvePackII_External_Analog_In_1 + "\t" +
                        myStruct_Status_Data_To_Save.ServoValvePackII_External_Analog_In_2 + "\t" +
                        myStruct_Status_Data_To_Save.ServoValvePackII_External_Analog_In_3 + "\t" +
                        myStruct_Status_Data_To_Save.ServoValvePackII_External_Analog_In_4 + "\t" +
                        myStruct_Status_Data_To_Save.ServoValvePackII_Main_Supply_Voltage_24VDC + "\t" +
                        myStruct_Status_Data_To_Save.ServoValvePackII_Main_Supply_Voltage_15VDC + "\t" +
                        myStruct_Status_Data_To_Save.ServoValvePackII_Current_Feedback_DOUT1_8_SSUP_1_2 + "\t" +
                        myStruct_Status_Data_To_Save.ServoValvePackII_Current_Feedback_DOUT9_16_SSUP_3_4 + "\t" +
                        myStruct_Status_Data_To_Save.ServoValvePackII_Temperature + "\t" +
                        //16功能阀箱（国产版）
                        myStruct_Status_Data_To_Save.ServoValvePackIIB_V + "\t" +
                        myStruct_Status_Data_To_Save.ServoValvePackIIB_I + "\t" +
                        myStruct_Status_Data_To_Save.ServoValvePackIIB_P + "\t" +
                        myStruct_Status_Data_To_Save.ServoValvePackIIB_isLeakage + "\t" +
                        
                        //ROV配电柜
                        myStruct_Status_Data_To_Save.I_First_A + "\t" +
                        myStruct_Status_Data_To_Save.I_First_B + "\t" +
                        myStruct_Status_Data_To_Save.I_First_C + "\t" +
                        myStruct_Status_Data_To_Save.V_First_AB + "\t" +
                        myStruct_Status_Data_To_Save.V_First_BC + "\t" +
                        myStruct_Status_Data_To_Save.I_First_S + "\t" +

                        myStruct_Status_Data_To_Save.I_Next_ABC + "\t" +
                        myStruct_Status_Data_To_Save.I_Next_S + "\t" +
                        myStruct_Status_Data_To_Save.V_Next_ABC + "\t" +
                        myStruct_Status_Data_To_Save.V_Next_S + "\t" +

                        myStruct_Status_Data_To_Save.ALARM_T1输入电压低报警 + "\t" +
                        myStruct_Status_Data_To_Save.ALARM_T1输入电压高报警 + "\t" +
                        myStruct_Status_Data_To_Save.ALARM_T1输出电压高报警 + "\t" +
                        myStruct_Status_Data_To_Save.ALARM_T1输入电流高报警 + "\t" +
                        myStruct_Status_Data_To_Save.ALARM_T1输出电流高报警 + "\t" +

                        myStruct_Status_Data_To_Save.ALARM_T2输入电压低报警 + "\t" +
                        myStruct_Status_Data_To_Save.ALARM_T2输入电压高报警 + "\t" +
                        myStruct_Status_Data_To_Save.ALARM_T2输出电压高报警 + "\t" +
                        myStruct_Status_Data_To_Save.ALARM_T2输入电流高报警 + "\t" +
                        myStruct_Status_Data_To_Save.ALARM_T2输出电流高报警 + "\t" +

                        myStruct_Status_Data_To_Save.ALARM_T1输入电流A相变送器故障 + "\t" +
                        myStruct_Status_Data_To_Save.ALARM_T1输入电流B相变送器故障 + "\t" +
                        myStruct_Status_Data_To_Save.ALARM_T1输入电流C相变送器故障 + "\t" +
                        myStruct_Status_Data_To_Save.ALARM_T1输入电压AB相变送器故障 + "\t" +
                        myStruct_Status_Data_To_Save.ALARM_T1输入电压BC相变送器故障 + "\t" +
                        myStruct_Status_Data_To_Save.ALARM_T2输入电流变送器故障 + "\t" +
                        myStruct_Status_Data_To_Save.ALARM_T1输出电流变送器故障 + "\t" +
                        myStruct_Status_Data_To_Save.ALARM_T2输出电流变送器故障 + "\t" +
                        myStruct_Status_Data_To_Save.ALARM_T1输出电压变送器故障 + "\t" +
                        myStruct_Status_Data_To_Save.ALARM_T2输出电压变送器故障 + "\t" +
                        myStruct_Status_Data_To_Save.ALARM_相序断相不平衡故障 + "\t" +
                        myStruct_Status_Data_To_Save.ALARM_T1_高压绝缘故障 + "\t" +
                        myStruct_Status_Data_To_Save.ALARM_T2_高压绝缘故障 + "\t" +
                        myStruct_Status_Data_To_Save.ALARM_急停故障 + "\t" +

                        myStruct_Status_Data_To_Save.ALARM_T1输入电压低低故障 + "\t" +
                        myStruct_Status_Data_To_Save.ALARM_T1输入电压高高故障 + "\t" +
                        myStruct_Status_Data_To_Save.ALARM_T1输出电压高高故障 + "\t" +

                        myStruct_Status_Data_To_Save.ALARM_T1输入电流高高故障 + "\t" +
                        myStruct_Status_Data_To_Save.ALARM_T1输出电流高高故障 + "\t" +

                        myStruct_Status_Data_To_Save.ALARM_T2输入电压低低故障 + "\t" +
                        myStruct_Status_Data_To_Save.ALARM_T2输入电压高高故障 + "\t" +
                        myStruct_Status_Data_To_Save.ALARM_T2输出电压高高故障 + "\t" +

                        myStruct_Status_Data_To_Save.ALARM_T2输入电流高高故障 + "\t" +
                        myStruct_Status_Data_To_Save.ALARM_T2输出电流高高故障 + "\t"
                        ;

                    Global.myStatusStreamWriter.WriteLine(ss);
                    Global.myStatusStreamWriter.Flush();


                    if (Global.isDataComingCount > isDataComingCountLast)
                    {
                        Global.isCommucationOK = true;
                        isDataComingCountLast = Global.isDataComingCount;
                    }
                    else
                    {
                        Global.isCommucationOK = false;
                        Global.isDataComing = false;
                    }
                }
            }
            catch (Exception ex)
            { }
        }

        public void ReceiveDataFromSerial(object sender, EventArgs e)
        {
            try
            {
                GEventArgs gEventArgs = (GEventArgs)e;
                Delegate_Status_Data_To_Save myDelegate = new Delegate_Status_Data_To_Save(Func_Delegate_Status_Data_To_Save);
                myDelegate.BeginInvoke(sender, gEventArgs, null, null);
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString());
            }
        }


        private delegate void Delegate_Status_Data_To_Save(object sender, GEventArgs gEventArgs);
        private void Func_Delegate_Status_Data_To_Save(object sender, GEventArgs gEventArgs)
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
                        myStruct_Status_Data_To_Save.DC_Insulation_Detection_VA_61 = Math.Round(myStruct.VA, 2).ToString();
                        myStruct_Status_Data_To_Save.DC_Insulation_Detection_IA_61 = Math.Round(myStruct.IA, 2).ToString();
                        myStruct_Status_Data_To_Save.DC_Insulation_Detection_GA_61 = Math.Round(myStruct.GA, 2).ToString();
                        myStruct_Status_Data_To_Save.DC_Insulation_Detection_TA_61 = Math.Round(myStruct.TA, 2).ToString();
                        myStruct_Status_Data_To_Save.DC_Insulation_Detection_VB_61 = Math.Round(myStruct.VB, 2).ToString();
                        myStruct_Status_Data_To_Save.DC_Insulation_Detection_IB_61 = Math.Round(myStruct.IB, 2).ToString();
                        myStruct_Status_Data_To_Save.DC_Insulation_Detection_GB_61 = Math.Round(myStruct.GB, 2).ToString();
                        myStruct_Status_Data_To_Save.DC_Insulation_Detection_TB_61 = Math.Round(myStruct.TB, 2).ToString();
                        myStruct_Status_Data_To_Save.DC_Insulation_Detection_TO_61 = Math.Round(myStruct.TO, 2).ToString();
                    }
                    else if (gEventArgs.addressBoard == enum_AddressBoard.DC_Insulation_Detection_Board_II)//直流绝缘检测板#2
                    {
                        Struct_DC_Insulation_Detection_Board myStruct = (Struct_DC_Insulation_Detection_Board)gEventArgs.objParse;
                        myStruct_Status_Data_To_Save.DC_Insulation_Detection_VA_62 = Math.Round(myStruct.VA, 2).ToString();
                        myStruct_Status_Data_To_Save.DC_Insulation_Detection_IA_62 = Math.Round(myStruct.IA, 2).ToString();
                        myStruct_Status_Data_To_Save.DC_Insulation_Detection_GA_62 = Math.Round(myStruct.GA, 2).ToString();
                        myStruct_Status_Data_To_Save.DC_Insulation_Detection_TA_62 = Math.Round(myStruct.TA, 2).ToString();
                        myStruct_Status_Data_To_Save.DC_Insulation_Detection_VB_62 = Math.Round(myStruct.VB, 2).ToString();
                        myStruct_Status_Data_To_Save.DC_Insulation_Detection_IB_62 = Math.Round(myStruct.IB, 2).ToString();
                        myStruct_Status_Data_To_Save.DC_Insulation_Detection_GB_62 = Math.Round(myStruct.GB, 2).ToString();
                        myStruct_Status_Data_To_Save.DC_Insulation_Detection_TB_62 = Math.Round(myStruct.TB, 2).ToString();
                        myStruct_Status_Data_To_Save.DC_Insulation_Detection_TO_62 = Math.Round(myStruct.TO, 2).ToString();
                    }
                    else if (gEventArgs.addressBoard == enum_AddressBoard.DC_Insulation_Detection_Board_III)//直流绝缘检测板#3
                    {
                        Struct_DC_Insulation_Detection_Board myStruct = (Struct_DC_Insulation_Detection_Board)gEventArgs.objParse;
                        myStruct_Status_Data_To_Save.DC_Insulation_Detection_VA_63 = Math.Round(myStruct.VA, 2).ToString();
                        myStruct_Status_Data_To_Save.DC_Insulation_Detection_IA_63 = Math.Round(myStruct.IA, 2).ToString();
                        myStruct_Status_Data_To_Save.DC_Insulation_Detection_GA_63 = Math.Round(myStruct.GA, 2).ToString();
                        myStruct_Status_Data_To_Save.DC_Insulation_Detection_TA_63 = Math.Round(myStruct.TA, 2).ToString();
                        myStruct_Status_Data_To_Save.DC_Insulation_Detection_VB_63 = Math.Round(myStruct.VB, 2).ToString();
                        myStruct_Status_Data_To_Save.DC_Insulation_Detection_IB_63 = Math.Round(myStruct.IB, 2).ToString();
                        myStruct_Status_Data_To_Save.DC_Insulation_Detection_GB_63 = Math.Round(myStruct.GB, 2).ToString();
                        myStruct_Status_Data_To_Save.DC_Insulation_Detection_TB_63 = Math.Round(myStruct.TB, 2).ToString();
                        myStruct_Status_Data_To_Save.DC_Insulation_Detection_TO_63 = Math.Round(myStruct.TO, 2).ToString();
                    }
                    else if (gEventArgs.addressBoard == enum_AddressBoard.High_Power_DC_Insulation_Detection_Board_I)//大功率直流绝缘检测板#1
                    {
                        Struct_High_Power_DC_Insulation_Detection_Board myStruct = (Struct_High_Power_DC_Insulation_Detection_Board)gEventArgs.objParse;
                        myStruct_Status_Data_To_Save.High_Power_DC_Insulation_Detection_V_70 = Math.Round(myStruct.V, 2).ToString();
                        myStruct_Status_Data_To_Save.High_Power_DC_Insulation_Detection_I_70 = Math.Round(myStruct.I, 2).ToString();
                        myStruct_Status_Data_To_Save.High_Power_DC_Insulation_Detection_G_70 = Math.Round(myStruct.G, 2).ToString();
                        myStruct_Status_Data_To_Save.High_Power_DC_Insulation_Detection_T_70 = Math.Round(myStruct.T, 2).ToString();
                    }
                    else if (gEventArgs.addressBoard == enum_AddressBoard.High_Power_DC_Insulation_Detection_Board_II)//大功率直流绝缘检测板#2
                    {
                        Struct_High_Power_DC_Insulation_Detection_Board myStruct = (Struct_High_Power_DC_Insulation_Detection_Board)gEventArgs.objParse;
                        myStruct_Status_Data_To_Save.High_Power_DC_Insulation_Detection_V_71 = Math.Round(myStruct.V, 2).ToString();
                        myStruct_Status_Data_To_Save.High_Power_DC_Insulation_Detection_I_71 = Math.Round(myStruct.I, 2).ToString();
                        myStruct_Status_Data_To_Save.High_Power_DC_Insulation_Detection_G_71 = Math.Round(myStruct.G, 2).ToString();
                        myStruct_Status_Data_To_Save.High_Power_DC_Insulation_Detection_T_71 = Math.Round(myStruct.T, 2).ToString();
                    }
                    else if (gEventArgs.addressBoard == enum_AddressBoard.High_Power_DC_Insulation_Detection_Board_III)//大功率直流绝缘检测板#3
                    {
                        Struct_High_Power_DC_Insulation_Detection_Board myStruct = (Struct_High_Power_DC_Insulation_Detection_Board)gEventArgs.objParse;
                        myStruct_Status_Data_To_Save.High_Power_DC_Insulation_Detection_V_72 = Math.Round(myStruct.V, 2).ToString();
                        myStruct_Status_Data_To_Save.High_Power_DC_Insulation_Detection_I_72 = Math.Round(myStruct.I, 2).ToString();
                        myStruct_Status_Data_To_Save.High_Power_DC_Insulation_Detection_G_72 = Math.Round(myStruct.G, 2).ToString();
                        myStruct_Status_Data_To_Save.High_Power_DC_Insulation_Detection_T_72 = Math.Round(myStruct.T, 2).ToString();
                    }
                    else if (gEventArgs.addressBoard == enum_AddressBoard.Control_Panel_High_Power_DC_Insulation_Detection_Board)//大功率直流绝缘检测板控制板
                    {
                        Struct_Control_Panel_High_Power_DC_Insulation_Detection_Board myStruct = (Struct_Control_Panel_High_Power_DC_Insulation_Detection_Board)gEventArgs.objParse;
                        myStruct_Status_Data_To_Save.Control_Panel_High_Power_DC_Insulation_Detection_V_79 = Math.Round(myStruct.V, 2).ToString();
                        myStruct_Status_Data_To_Save.Control_Panel_High_Power_DC_Insulation_Detection_I_79 = Math.Round(myStruct.I, 2).ToString();
                    }
                    else if (gEventArgs.addressBoard == enum_AddressBoard.AC_Insulation_Detection_Board)//交流绝缘检测板
                    {
                        Struct_AC_Insulation_Detection_Board myStruct = (Struct_AC_Insulation_Detection_Board)gEventArgs.objParse;
                        myStruct_Status_Data_To_Save.AC_Insulation_Detection_V_80 = Math.Round(myStruct.V, 2).ToString();
                        myStruct_Status_Data_To_Save.AC_Insulation_Detection_I_80 = Math.Round(myStruct.I, 2).ToString();
                    }
                    else if (gEventArgs.addressBoard == enum_AddressBoard.Camera_Power_Relay_Board_I)//摄像机电源继电器板#1
                    {
                        Struct_Power_Relay_Board myStruct = (Struct_Power_Relay_Board)gEventArgs.objParse;
                        myStruct_Status_Data_To_Save.Power_Relay_Board_V_25 = Math.Round(myStruct.V, 2).ToString();
                    }
                    else if (gEventArgs.addressBoard == enum_AddressBoard.Camera_Power_Relay_Board_II)//摄像机电源继电器板#2
                    {
                        Struct_Power_Relay_Board myStruct = (Struct_Power_Relay_Board)gEventArgs.objParse;
                        myStruct_Status_Data_To_Save.Power_Relay_Board_V_26 = Math.Round(myStruct.V, 2).ToString();
                    }
                    else if (gEventArgs.addressBoard == enum_AddressBoard.Sensor_Power_Relay_Board_I)//传感器电源继电器板#1
                    {
                        Struct_Power_Relay_Board myStruct = (Struct_Power_Relay_Board)gEventArgs.objParse;
                        myStruct_Status_Data_To_Save.Power_Relay_Board_V_28 = Math.Round(myStruct.V, 2).ToString();
                    }
                    else if (gEventArgs.addressBoard == enum_AddressBoard.Sensor_Power_Relay_Board_II)//传感器电源继电器板#2
                    {
                        Struct_Power_Relay_Board myStruct = (Struct_Power_Relay_Board)gEventArgs.objParse;
                        myStruct_Status_Data_To_Save.Power_Relay_Board_V_29 = Math.Round(myStruct.V, 2).ToString();
                    }
                    else if (gEventArgs.addressBoard == enum_AddressBoard.Tank_Detection_Board)//油箱采集板#1
                    {
                        Struct_Tank_Detection_Board myStruct = (Struct_Tank_Detection_Board)gEventArgs.objParse;
                        myStruct_Status_Data_To_Save.Tank_Detection_CH1_1 = Math.Round(myStruct.CH1_1, 2).ToString();
                        myStruct_Status_Data_To_Save.Tank_Detection_CH1_2 = Math.Round(myStruct.CH1_2, 2).ToString();
                        myStruct_Status_Data_To_Save.Tank_Detection_CH1_3 = Math.Round(myStruct.CH1_3, 2).ToString();
                        myStruct_Status_Data_To_Save.Tank_Detection_CH1_4 = Math.Round(myStruct.CH1_4, 2).ToString();
                        myStruct_Status_Data_To_Save.Tank_Detection_CH2_1 = Math.Round(myStruct.CH2_1, 2).ToString();
                        myStruct_Status_Data_To_Save.Tank_Detection_CH2_2 = Math.Round(myStruct.CH2_2, 2).ToString();
                        myStruct_Status_Data_To_Save.Tank_Detection_CH2_3 = Math.Round(myStruct.CH2_3, 2).ToString();
                        myStruct_Status_Data_To_Save.Tank_Detection_CH2_4 = Math.Round(myStruct.CH2_4, 2).ToString();
                        myStruct_Status_Data_To_Save.Tank_Detection_CH3_1 = Math.Round(myStruct.CH3_1, 2).ToString();
                        myStruct_Status_Data_To_Save.Tank_Detection_CH3_2 = Math.Round(myStruct.CH3_2, 2).ToString();
                        myStruct_Status_Data_To_Save.Tank_Detection_CH3_3 = Math.Round(myStruct.CH3_3, 2).ToString();
                        myStruct_Status_Data_To_Save.Tank_Detection_CH3_4 = Math.Round(myStruct.CH3_4, 2).ToString();
                        myStruct_Status_Data_To_Save.Tank_Detection_CH4_1 = Math.Round(myStruct.CH4_1, 2).ToString();
                        myStruct_Status_Data_To_Save.Tank_Detection_CH4_2 = Math.Round(myStruct.CH4_2, 2).ToString();
                        myStruct_Status_Data_To_Save.Tank_Detection_CH4_3 = Math.Round(myStruct.CH4_3, 2).ToString();
                        myStruct_Status_Data_To_Save.Tank_Detection_CH4_4 = Math.Round(myStruct.CH4_4, 2).ToString();
                    }
                    else if (gEventArgs.addressBoard == enum_AddressBoard.Light_Relay_Board_I)//灯继电器板#1(灯舱继电器板)
                    {
                        Struct_Light_Relay_Board myStruct = (Struct_Light_Relay_Board)gEventArgs.objParse;
                        myStruct_Status_Data_To_Save.Light_Relay_Board_V_21 = Math.Round(myStruct.V, 2).ToString();
                    }
                    else if (gEventArgs.addressBoard == enum_AddressBoard.Light_Relay_Board_II)//灯继电器板#2(灯舱继电器板)
                    {
                        Struct_Light_Relay_Board myStruct = (Struct_Light_Relay_Board)gEventArgs.objParse;
                        myStruct_Status_Data_To_Save.Light_Relay_Board_V_22 = Math.Round(myStruct.V, 2).ToString();
                    }
                    else if (gEventArgs.addressBoard == enum_AddressBoard.Work_Station_Quire_Board)
                    {
                        Struct_Work_Station_Quire_Board myStruct =
                            (Struct_Work_Station_Quire_Board)gEventArgs.objParse;
                        
                        //记录数据缓存
                        myStruct_Status_Data_To_Save.Work_Station_RotateSpeed = myStruct.RotateSpeed.ToString();
                        myStruct_Status_Data_To_Save.WorkStation_B = myStruct.WorkStation_B.ToString();
                        myStruct_Status_Data_To_Save.WorkStation_C = myStruct.WorkStation_C.ToString();
                        myStruct_Status_Data_To_Save.WorkStation_D = myStruct.WorkStation_D.ToString();
                        myStruct_Status_Data_To_Save.WorkStation_E = myStruct.WorkStation_E.ToString();
                        myStruct_Status_Data_To_Save.WorkStation_F = myStruct.WorkStation_F.ToString();
                        myStruct_Status_Data_To_Save.WorkStation_G = myStruct.WorkStation_G.ToString();                        
                    }
                    else if (gEventArgs.addressBoard == enum_AddressBoard.Rov_Power_Box)
                    {
                        Struct_ROVPower_CtlSystem myStruct = (Struct_ROVPower_CtlSystem)gEventArgs.objParse;
                        
                        //记录数据缓存
                        myStruct_Status_Data_To_Save.I_First_A = Math.Round(myStruct.I_First_A, 2).ToString();
                        myStruct_Status_Data_To_Save.I_First_B = Math.Round(myStruct.I_First_B, 2).ToString();
                        myStruct_Status_Data_To_Save.I_First_C = Math.Round(myStruct.I_First_C, 2).ToString();
                        myStruct_Status_Data_To_Save.I_First_S = Math.Round(myStruct.I_First_S, 2).ToString();
                        myStruct_Status_Data_To_Save.V_First_AB = Math.Round(myStruct.V_First_AB, 2).ToString();
                        myStruct_Status_Data_To_Save.V_First_BC = Math.Round(myStruct.V_First_BC, 2).ToString();
                        myStruct_Status_Data_To_Save.I_Next_ABC = Math.Round(myStruct.I_Next_ABC, 2).ToString();
                        myStruct_Status_Data_To_Save.I_Next_S = Math.Round(myStruct.I_Next_S, 2).ToString();
                        myStruct_Status_Data_To_Save.V_Next_ABC = Math.Round(myStruct.V_Next_ABC, 2).ToString();
                        myStruct_Status_Data_To_Save.V_Next_S = Math.Round(myStruct.V_Next_S, 2).ToString();

                        myStruct_Status_Data_To_Save.ALARM_T1输入电压低报警 = myStruct.ALARM_T1输入电压低报警.ToString();
                        myStruct_Status_Data_To_Save.ALARM_T1输入电压高报警 = myStruct.ALARM_T1输入电压高报警.ToString();
                        myStruct_Status_Data_To_Save.ALARM_T1输出电压高报警 = myStruct.ALARM_T1输出电压高报警.ToString();
                        myStruct_Status_Data_To_Save.ALARM_T1输入电流高报警 = myStruct.ALARM_T1输入电流高报警.ToString();
                        myStruct_Status_Data_To_Save.ALARM_T1输出电流高报警 = myStruct.ALARM_T1输出电流高报警.ToString();

                        myStruct_Status_Data_To_Save.ALARM_T2输入电压低报警 = myStruct.ALARM_T2输入电压低报警.ToString();
                        myStruct_Status_Data_To_Save.ALARM_T2输入电压高报警 = myStruct.ALARM_T2输入电压高报警.ToString();
                        myStruct_Status_Data_To_Save.ALARM_T2输出电压高报警 = myStruct.ALARM_T2输出电压高报警.ToString();
                        myStruct_Status_Data_To_Save.ALARM_T2输入电流高报警 = myStruct.ALARM_T2输入电流高报警.ToString();
                        myStruct_Status_Data_To_Save.ALARM_T2输出电流高报警 = myStruct.ALARM_T2输出电流高报警.ToString();

                        myStruct_Status_Data_To_Save.ALARM_T1输入电流A相变送器故障 = myStruct.ALARM_T1输入电流A相变送器故障.ToString();
                        myStruct_Status_Data_To_Save.ALARM_T1输入电流B相变送器故障 = myStruct.ALARM_T1输入电流B相变送器故障.ToString();
                        myStruct_Status_Data_To_Save.ALARM_T1输入电流C相变送器故障 = myStruct.ALARM_T1输入电流C相变送器故障.ToString();
                        myStruct_Status_Data_To_Save.ALARM_T1输入电压AB相变送器故障 = myStruct.ALARM_T1输入电压AB相变送器故障.ToString();
                        myStruct_Status_Data_To_Save.ALARM_T1输入电压BC相变送器故障 = myStruct.ALARM_T1输入电压BC相变送器故障.ToString();
                        myStruct_Status_Data_To_Save.ALARM_T2输入电流变送器故障 = myStruct.ALARM_T2输入电流变送器故障.ToString();
                        myStruct_Status_Data_To_Save.ALARM_T1输出电流变送器故障 = myStruct.ALARM_T1输出电流变送器故障.ToString();
                        myStruct_Status_Data_To_Save.ALARM_T2输出电流变送器故障 = myStruct.ALARM_T2输出电流变送器故障.ToString();
                        myStruct_Status_Data_To_Save.ALARM_T1输出电压变送器故障 = myStruct.ALARM_T1输出电压变送器故障.ToString();
                        myStruct_Status_Data_To_Save.ALARM_T2输出电压变送器故障 = myStruct.ALARM_T2输出电压变送器故障.ToString();
                        myStruct_Status_Data_To_Save.ALARM_相序断相不平衡故障 = myStruct.ALARM_相序断相不平衡故障.ToString();
                        myStruct_Status_Data_To_Save.ALARM_T1_高压绝缘故障 = myStruct.ALARM_T1_高压绝缘故障.ToString();
                        myStruct_Status_Data_To_Save.ALARM_T2_高压绝缘故障 = myStruct.ALARM_T2_高压绝缘故障.ToString();
                        myStruct_Status_Data_To_Save.ALARM_急停故障 = myStruct.ALARM_急停故障.ToString();

                        myStruct_Status_Data_To_Save.ALARM_T1输入电压低低故障 = myStruct.ALARM_T1输入电压低低故障.ToString();
                        myStruct_Status_Data_To_Save.ALARM_T1输入电压高高故障 = myStruct.ALARM_T1输入电压高高故障.ToString();
                        myStruct_Status_Data_To_Save.ALARM_T1输出电压高高故障 = myStruct.ALARM_T1输出电压高高故障.ToString();

                        myStruct_Status_Data_To_Save.ALARM_T1输入电流高高故障 = myStruct.ALARM_T1输入电流高高故障.ToString();
                        myStruct_Status_Data_To_Save.ALARM_T1输出电流高高故障 = myStruct.ALARM_T1输出电流高高故障.ToString();

                        myStruct_Status_Data_To_Save.ALARM_T2输入电压低低故障 = myStruct.ALARM_T2输入电压低低故障.ToString();
                        myStruct_Status_Data_To_Save.ALARM_T2输入电压高高故障 = myStruct.ALARM_T2输入电压高高故障.ToString();
                        myStruct_Status_Data_To_Save.ALARM_T2输出电压高高故障 = myStruct.ALARM_T2输出电压高高故障.ToString();

                        myStruct_Status_Data_To_Save.ALARM_T2输入电流高高故障 = myStruct.ALARM_T2输入电流高高故障.ToString();
                        myStruct_Status_Data_To_Save.ALARM_T2输出电流高高故障 = myStruct.ALARM_T2输出电流高高故障.ToString();
                    }
                    else if (gEventArgs.addressBoard == enum_AddressBoard.JueYuanJianCe_Board_1)
                    {
                        Struct_JueYuanJianCeYi myStruct =
                            (Struct_JueYuanJianCeYi)gEventArgs.objParse;

                        myStruct_Status_Data_To_Save.sMeauringValue_1 = myStruct.sMeauringValue;
                        myStruct_Status_Data_To_Save.sAlarm1Value_1 = myStruct.sAlarm1Value;
                        myStruct_Status_Data_To_Save.sAlarm2Value_1 = myStruct.sAlarm2Value;
                        myStruct_Status_Data_To_Save.sK1_K2_OnOff_1 = myStruct.sK1_K2_OnOff;
                        myStruct_Status_Data_To_Save.sAlarm1_2_None_1 = myStruct.sAlarm1_2_None;
                        myStruct_Status_Data_To_Save.sAC_DC_Fault_1 = myStruct.sAC_DC_Fault;
                    }
                    else if (gEventArgs.addressBoard == enum_AddressBoard.JueYuanJianCe_Board_2)
                    {
                        Struct_JueYuanJianCeYi myStruct =
                            (Struct_JueYuanJianCeYi)gEventArgs.objParse;

                        myStruct_Status_Data_To_Save.sMeauringValue_2 = myStruct.sMeauringValue;
                        myStruct_Status_Data_To_Save.sAlarm1Value_2 = myStruct.sAlarm1Value;
                        myStruct_Status_Data_To_Save.sAlarm2Value_2 = myStruct.sAlarm2Value;
                        myStruct_Status_Data_To_Save.sK1_K2_OnOff_2 = myStruct.sK1_K2_OnOff;
                        myStruct_Status_Data_To_Save.sAlarm1_2_None_2 = myStruct.sAlarm1_2_None;
                        myStruct_Status_Data_To_Save.sAC_DC_Fault_2 = myStruct.sAC_DC_Fault;
                    }

                    else if (gEventArgs.addressBoard == enum_AddressBoard.Hight_Measure_Device)//高度计
                    {
                        Struct_HightMeasureDevice myStruct = (Struct_HightMeasureDevice)gEventArgs.objParse;
                        myStruct_Status_Data_To_Save.Hight_Measure_Height = myStruct.sHight;
                    }
                    else if (gEventArgs.addressBoard == enum_AddressBoard.Rotate_Panel_Device)//罗盘
                    {
                        Struct_RotatePanelDevice myStruct = (Struct_RotatePanelDevice)gEventArgs.objParse;
                        myStruct_Status_Data_To_Save.Rotate_Panel_HX = Math.Round(myStruct.HX, 2).ToString();
                        myStruct_Status_Data_To_Save.Rotate_Panel_HY = Math.Round(myStruct.HY, 2).ToString();
                        myStruct_Status_Data_To_Save.Rotate_Panel_HZ = Math.Round(myStruct.HZ, 2).ToString();
                        myStruct_Status_Data_To_Save.Rotate_Panel_Roll = Math.Round(myStruct.Roll, 2).ToString();
                        myStruct_Status_Data_To_Save.Rotate_Panel_Pitch = Math.Round(myStruct.Pitch, 2).ToString();
                        myStruct_Status_Data_To_Save.Rotate_Panel_Yaw = Math.Round(myStruct.Yaw, 2).ToString();
                    }
                    else if (gEventArgs.addressBoard == enum_AddressBoard.Water_Control_Box)
                    {
                        Struct_Water_Control_Box myStruct_Water_Control_Box = (Struct_Water_Control_Box)gEventArgs.objParse;
                        myStruct_Status_Data_To_Save.Water_Control_Box_RotAxisX = Math.Round(myStruct_Water_Control_Box.RotAxisX, 2).ToString();
                        myStruct_Status_Data_To_Save.Water_Control_Box_RotAxisY = Math.Round(myStruct_Water_Control_Box.RotAxisY, 2).ToString();
                        myStruct_Status_Data_To_Save.Water_Control_Box_RotAxisZ = Math.Round(myStruct_Water_Control_Box.RotAxisZ, 2).ToString();
                        myStruct_Status_Data_To_Save.Water_Control_Box_RotAxisV = Math.Round(myStruct_Water_Control_Box.RotAxisV, 2).ToString();
                        myStruct_Status_Data_To_Save.Water_Control_Box_Space1 = Math.Round(myStruct_Water_Control_Box.Space1, 2).ToString();
                        myStruct_Status_Data_To_Save.Water_Control_Box_Space2 = Math.Round(myStruct_Water_Control_Box.Space2, 2).ToString();
                        myStruct_Status_Data_To_Save.Water_Control_Box_KKInfo = myStruct_Water_Control_Box.KKInfo.ToString("X2");
                    }
                    else if (gEventArgs.addressBoard == enum_AddressBoard.DianJi_T_Detection_Board)
                    {
                        Struct_DianJi_T myStruct = (Struct_DianJi_T)gEventArgs.objParse;

                        myStruct_Status_Data_To_Save.DanJi_T_Para_1 = Math.Round(myStruct.Para_1, 2).ToString();
                        myStruct_Status_Data_To_Save.DanJi_T_Para_2 = Math.Round(myStruct.Para_2, 2).ToString();
                        myStruct_Status_Data_To_Save.DanJi_T_Para_3 = Math.Round(myStruct.Para_3, 2).ToString();
                        myStruct_Status_Data_To_Save.DanJi_T_Para_4 = Math.Round(myStruct.Para_4, 2).ToString();
                        myStruct_Status_Data_To_Save.DanJi_T_Para_5 = Math.Round(myStruct.Para_5, 2).ToString();
                        myStruct_Status_Data_To_Save.DanJi_T_Para_6 = Math.Round(myStruct.Para_6, 2).ToString();
                        myStruct_Status_Data_To_Save.DanJi_T_Para_7 = Math.Round(myStruct.Para_7, 2).ToString();
                        myStruct_Status_Data_To_Save.DanJi_T_Para_8 = Math.Round(myStruct.Para_8, 2).ToString();

                    }
                    else if (gEventArgs.addressBoard == enum_AddressBoard.ServoValvePacketBoard8Func)
                    {
                        Struct_BoardA_Status myStructBoardIStatus = (Struct_BoardA_Status)gEventArgs.objParse;

                        myStruct_Status_Data_To_Save.ServoValvePackI_Received_Bad_CRCs = myStructBoardIStatus.Received_Bad_CRCs.ToString();
                        myStruct_Status_Data_To_Save.ServoValvePackI_bDIN8 = myStructBoardIStatus.bDIN8.ToString();
                        myStruct_Status_Data_To_Save.ServoValvePackI_bDIN7 = myStructBoardIStatus.bDIN7.ToString();
                        myStruct_Status_Data_To_Save.ServoValvePackI_bDIN6 = myStructBoardIStatus.bDIN6.ToString();
                        myStruct_Status_Data_To_Save.ServoValvePackI_bDIN5 = myStructBoardIStatus.bDIN5.ToString();
                        myStruct_Status_Data_To_Save.ServoValvePackI_bDIN4 = myStructBoardIStatus.bDIN4.ToString();
                        myStruct_Status_Data_To_Save.ServoValvePackI_bDIN3 = myStructBoardIStatus.bDIN3.ToString();
                        myStruct_Status_Data_To_Save.ServoValvePackI_bDIN2 = myStructBoardIStatus.bDIN2.ToString();
                        myStruct_Status_Data_To_Save.ServoValvePackI_bDIN1 = myStructBoardIStatus.bDIN1.ToString();
                        myStruct_Status_Data_To_Save.ServoValvePackI_Main_Supply_Voltage_24V = myStructBoardIStatus.Main_Supply_Voltage_24V.ToString("D2");
                        myStruct_Status_Data_To_Save.ServoValvePackI_Analog_Supply_Voltage_15V = myStructBoardIStatus.Analog_Supply_Voltage_15V.ToString("D2");
                        myStruct_Status_Data_To_Save.ServoValvePackI_Digital_Supply_Voltage_5V = myStructBoardIStatus.Analog_Supply_Voltage_5V.ToString("D2");
                        myStruct_Status_Data_To_Save.ServoValvePackI_Analog_Supply_Voltage_5V = myStructBoardIStatus.Digital_Supply_Voltage_5V.ToString("D2");
                        myStruct_Status_Data_To_Save.ServoValvePackI_Current_Feedback_1 = myStructBoardIStatus.Current_Feedback_1.ToString("D2");
                        myStruct_Status_Data_To_Save.ServoValvePackI_Current_Feedback_2 = myStructBoardIStatus.Current_Feedback_2.ToString("D2");
                        myStruct_Status_Data_To_Save.ServoValvePackI_Analog_Input_17 = myStructBoardIStatus.Analog_Input_17.ToString("D2");
                        myStruct_Status_Data_To_Save.ServoValvePackI_Analog_Input_18 = myStructBoardIStatus.Analog_Input_18.ToString("D2");
                        myStruct_Status_Data_To_Save.ServoValvePackI_Analog_Input_19 = myStructBoardIStatus.Analog_Input_19.ToString("D2");
                        myStruct_Status_Data_To_Save.ServoValvePackI_Analog_Input_20 = myStructBoardIStatus.Analog_Input_20.ToString("D2");
                        myStruct_Status_Data_To_Save.ServoValvePackI_Analog_Input_1 = myStructBoardIStatus.Analog_Input_1.ToString("D2");
                        myStruct_Status_Data_To_Save.ServoValvePackI_Analog_Input_2 = myStructBoardIStatus.Analog_Input_2.ToString("D2");
                        myStruct_Status_Data_To_Save.ServoValvePackI_Analog_Input_3 = myStructBoardIStatus.Analog_Input_3.ToString("D2");
                        myStruct_Status_Data_To_Save.ServoValvePackI_Analog_Input_4 = myStructBoardIStatus.Analog_Input_4.ToString("D2");
                        myStruct_Status_Data_To_Save.ServoValvePackI_Analog_Input_5 = myStructBoardIStatus.Analog_Input_5.ToString("D2");
                        myStruct_Status_Data_To_Save.ServoValvePackI_Analog_Input_6 = myStructBoardIStatus.Analog_Input_6.ToString("D2");
                        myStruct_Status_Data_To_Save.ServoValvePackI_Analog_Input_7 = myStructBoardIStatus.Analog_Input_7.ToString("D2");
                        myStruct_Status_Data_To_Save.ServoValvePackI_Analog_Input_8 = myStructBoardIStatus.Analog_Input_8.ToString("D2");
                        myStruct_Status_Data_To_Save.ServoValvePackI_Analog_Input_9 = myStructBoardIStatus.Analog_Input_9.ToString("D2");
                        myStruct_Status_Data_To_Save.ServoValvePackI_Analog_Input_10 = myStructBoardIStatus.Analog_Input_10.ToString("D2");
                        myStruct_Status_Data_To_Save.ServoValvePackI_Analog_Input_11 = myStructBoardIStatus.Analog_Input_11.ToString("D2");
                        myStruct_Status_Data_To_Save.ServoValvePackI_Analog_Input_12 = myStructBoardIStatus.Analog_Input_12.ToString("D2");
                        myStruct_Status_Data_To_Save.ServoValvePackI_Analog_Input_13 = myStructBoardIStatus.Analog_Input_13.ToString("D2");
                        myStruct_Status_Data_To_Save.ServoValvePackI_Analog_Input_14 = myStructBoardIStatus.Analog_Input_14.ToString("D2");
                        myStruct_Status_Data_To_Save.ServoValvePackI_Analog_Input_15 = myStructBoardIStatus.Analog_Input_15.ToString("D2");
                        myStruct_Status_Data_To_Save.ServoValvePackI_Analog_Input_16 = myStructBoardIStatus.Analog_Input_16.ToString("D2");
                        myStruct_Status_Data_To_Save.ServoValvePackI_Temperature = myStructBoardIStatus.Temperature.ToString("D2");
                    }
                    else if (gEventArgs.addressBoard == enum_AddressBoard.ServoValvePacketBoard16Func)
                    {
                        Struct_BoardB_Status myStructBoardIIStatus = (Struct_BoardB_Status)gEventArgs.objParse;
                        if (myStructBoardIIStatus.boardTypeClass == enum_BoardTypeClass.ClassA)
                        {
                            myStruct_Status_Data_To_Save.ServoValvePackII_Received_Bad_CRCs = myStructBoardIIStatus.Received_Bad_CRCs.ToString();
                            myStruct_Status_Data_To_Save.ServoValvePackII_bDIN8 = myStructBoardIIStatus.bDIN8.ToString();
                            myStruct_Status_Data_To_Save.ServoValvePackII_bDIN7 = myStructBoardIIStatus.bDIN7.ToString();
                            myStruct_Status_Data_To_Save.ServoValvePackII_bDIN6 = myStructBoardIIStatus.bDIN6.ToString();
                            myStruct_Status_Data_To_Save.ServoValvePackII_bDIN5 = myStructBoardIIStatus.bDIN5.ToString();
                            myStruct_Status_Data_To_Save.ServoValvePackII_bDIN4 = myStructBoardIIStatus.bDIN4.ToString();
                            myStruct_Status_Data_To_Save.ServoValvePackII_bDIN3 = myStructBoardIIStatus.bDIN3.ToString();
                            myStruct_Status_Data_To_Save.ServoValvePackII_bDIN2 = myStructBoardIIStatus.bDIN2.ToString();
                            myStruct_Status_Data_To_Save.ServoValvePackII_bDIN1 = myStructBoardIIStatus.bDIN1.ToString();

                            myStruct_Status_Data_To_Save.ServoValvePackII_Current_Feedback_PWM_1_2 = myStructBoardIIStatus.Current_Feedback_PWM_1_2.ToString();
                            myStruct_Status_Data_To_Save.ServoValvePackII_Current_Feedback_PWM_3_4 = myStructBoardIIStatus.Current_Feedback_PWM_3_4.ToString();
                            myStruct_Status_Data_To_Save.ServoValvePackII_Current_Feedback_PWM_5_6 = myStructBoardIIStatus.Current_Feedback_PWM_5_6.ToString();
                            myStruct_Status_Data_To_Save.ServoValvePackII_Current_Feedback_PWM_7_8 = myStructBoardIIStatus.Current_Feedback_PWM_7_8.ToString();
                            myStruct_Status_Data_To_Save.ServoValvePackII_Current_Feedback_PWM_9_10 = myStructBoardIIStatus.Current_Feedback_PWM_9_10.ToString();
                            myStruct_Status_Data_To_Save.ServoValvePackII_Current_Feedback_PWM_11_12 = myStructBoardIIStatus.Current_Feedback_PWM_11_12.ToString();
                            myStruct_Status_Data_To_Save.ServoValvePackII_Current_Feedback_PWM_13_14 = myStructBoardIIStatus.Current_Feedback_PWM_13_14.ToString();
                            myStruct_Status_Data_To_Save.ServoValvePackII_Current_Feedback_PWM_15_16 = myStructBoardIIStatus.Current_Feedback_PWM_15_16.ToString();

                            myStruct_Status_Data_To_Save.ServoValvePackII_VCCD = myStructBoardIIStatus.VCCD.ToString();
                            myStruct_Status_Data_To_Save.ServoValvePackII_VCCA = myStructBoardIIStatus.VCCA.ToString();

                            myStruct_Status_Data_To_Save.ServoValvePackII_External_Analog_In_1 = myStructBoardIIStatus.External_Analog_In_1.ToString();
                            myStruct_Status_Data_To_Save.ServoValvePackII_External_Analog_In_2 = myStructBoardIIStatus.External_Analog_In_2.ToString();
                            myStruct_Status_Data_To_Save.ServoValvePackII_External_Analog_In_3 = myStructBoardIIStatus.External_Analog_In_3.ToString();
                            myStruct_Status_Data_To_Save.ServoValvePackII_External_Analog_In_4 = myStructBoardIIStatus.External_Analog_In_4.ToString();

                            myStruct_Status_Data_To_Save.ServoValvePackII_Main_Supply_Voltage_24VDC = myStructBoardIIStatus.Main_Supply_Voltage_24VDC.ToString();
                            myStruct_Status_Data_To_Save.ServoValvePackII_Main_Supply_Voltage_15VDC = myStructBoardIIStatus.Main_Supply_Voltage_15VDC.ToString();

                            myStruct_Status_Data_To_Save.ServoValvePackII_Current_Feedback_DOUT1_8_SSUP_1_2 = myStructBoardIIStatus.Current_Feedback_DOUT1_8_SSUP_1_2.ToString();
                            myStruct_Status_Data_To_Save.ServoValvePackII_Current_Feedback_DOUT9_16_SSUP_3_4 = myStructBoardIIStatus.Current_Feedback_DOUT9_16_SSUP_3_4.ToString();
                            myStruct_Status_Data_To_Save.ServoValvePackII_Temperature = myStructBoardIIStatus.Temperature.ToString();
                        }
                        else if (myStructBoardIIStatus.boardTypeClass == enum_BoardTypeClass.ClassB)
                        {
                            myStruct_Status_Data_To_Save.ServoValvePackIIB_V = myStructBoardIIStatus.V.ToString();
                            myStruct_Status_Data_To_Save.ServoValvePackIIB_I = myStructBoardIIStatus.I.ToString();
                            myStruct_Status_Data_To_Save.ServoValvePackIIB_P = myStructBoardIIStatus.P.ToString();
                            myStruct_Status_Data_To_Save.ServoValvePackIIB_isLeakage = myStructBoardIIStatus.isLeakage.ToString();
                        }
                    }

                    Global.isDataComing = true;
                    Global.isDataComingCount++;
                }
                else if (gEventArgs.dataType == 6)
                {
                    byte[] bCmdReturn = (byte[])gEventArgs.obj;
                }
            }
            catch (Exception ex)
            { }
        }

    }
}
