using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Runtime.InteropServices;
using System.IO;
using System.Windows.Forms;
using System.Drawing;

namespace MonitorProj
{
    //正常值/报警值单元结构
    public struct UnitResultCompareShowInfo
    {
        public double min;
        public double max;
        public string info;
    };


    public struct StatusUnitParas
    {
        public int bianHao;         //表序号   			预警值范围		是否报警
        public string serial;       //所属串口
        public string statusName;   //状态名称
        public string statusDaiHao; //状态代号
        public int alarm;          //0-不报警，1-报警黄色，2-报警红色
        public List<UnitResultCompareShowInfo> yellowParas; //预警值范围
        public List<UnitResultCompareShowInfo> redParas;    //报警值范围
        public bool isAlarm;
    };


    //推进器平移，前、后、左、右
    public enum Enum_HMove_Direction
    {
        Others = 0,

        Qian,
        Hou,
        Zuo,
        You,
        Stop,
    };


    /*用户主界面中，按钮发送，需要多个下层功能键组合发送，并会判断每一步骤的结果反馈，因此执行逻辑放在线程中
     *此处定义用户界面功能键识别号
     */
    public enum Enum_UserButtonOperIdentify
    {
        Others = 0,

        ZuanJin,
        ZuanJin_Step,
        TiZuan,
        TiZuan_Step,
        ZuanJin_TiZuan_Stop,

        ZuanJinFa_ZhengZhuan,
        ZuanJinFa_FanZhuan,
        ZuanJinFa_ZhengZhuan_FanZhuan_Stop,

        YunTai_ShunZhuan,
        YunTai_NiZhuan,
        YunTai_ShunZhuan_NiZhuan_Stop,
        YunTai_Fu,
        YunTai_Yang,
        YunTai_FuYang_Stop,

        YouYuanGaoYa_1,
        YouYuanGaoYa_Stop_1,
        YouYuanGaoYa,
        YouYuanGaoYa_Stop,
        ShuiBengKai,
        ShuiBengKai_Stop,

        JianYaFa_DiYa,
        JianYaFa_ZhongYa,
        JianYaFa_GaoYa,
        JianYaFa_DiYa_ZhongYa_GaoYa_Stop,

        ZuanJinGaoSu,
        ZuanJinGaoSu_Stop,


        //阀箱备用通道
        FaXiang_Space_A7,
        FaXiang_Space_B7,
        FaXiang_Space_AB7_Stop,

        FaXiang_Space_A8,
        FaXiang_Space_B8,
        FaXiang_Space_AB8_Stop,

        FaXiang_Space_A9,
        FaXiang_Space_B9,
        FaXiang_Space_AB9_Stop,

        FaXiang_Space_A10,
        FaXiang_Space_B10,
        FaXiang_Space_AB10_Stop,

        FaXiang_Space_A11,
        FaXiang_Space_B11,
        FaXiang_Space_AB11_Stop,
                        
    };

    
    //状态数据存储
    public struct Struct_Status_Data_To_Save
    {
        //直流绝缘检测板数据（0x61、0x62、0x63）
        public string DC_Insulation_Detection_VA_61;
        public string DC_Insulation_Detection_IA_61;
        public string DC_Insulation_Detection_GA_61;
        public string DC_Insulation_Detection_TA_61;
        public string DC_Insulation_Detection_VB_61;
        public string DC_Insulation_Detection_IB_61;
        public string DC_Insulation_Detection_GB_61;
        public string DC_Insulation_Detection_TB_61;
        public string DC_Insulation_Detection_TO_61;
        public string DC_Insulation_Detection_VA_62;
        public string DC_Insulation_Detection_IA_62;
        public string DC_Insulation_Detection_GA_62;
        public string DC_Insulation_Detection_TA_62;
        public string DC_Insulation_Detection_VB_62;
        public string DC_Insulation_Detection_IB_62;
        public string DC_Insulation_Detection_GB_62;
        public string DC_Insulation_Detection_TB_62;
        public string DC_Insulation_Detection_TO_62;
        public string DC_Insulation_Detection_VA_63;
        public string DC_Insulation_Detection_IA_63;
        public string DC_Insulation_Detection_GA_63;
        public string DC_Insulation_Detection_TA_63;
        public string DC_Insulation_Detection_VB_63;
        public string DC_Insulation_Detection_IB_63;
        public string DC_Insulation_Detection_GB_63;
        public string DC_Insulation_Detection_TB_63;
        public string DC_Insulation_Detection_TO_63;
        //大功率直流绝缘检测板数据解析：（0x70、0x71、0x72）
        public string High_Power_DC_Insulation_Detection_V_70;
        public string High_Power_DC_Insulation_Detection_I_70;
        public string High_Power_DC_Insulation_Detection_G_70;
        public string High_Power_DC_Insulation_Detection_T_70;
        public string High_Power_DC_Insulation_Detection_V_71;
        public string High_Power_DC_Insulation_Detection_I_71;
        public string High_Power_DC_Insulation_Detection_G_71;
        public string High_Power_DC_Insulation_Detection_T_71;
        public string High_Power_DC_Insulation_Detection_V_72;
        public string High_Power_DC_Insulation_Detection_I_72;
        public string High_Power_DC_Insulation_Detection_G_72;
        public string High_Power_DC_Insulation_Detection_T_72;
        //大功率直流绝缘检测板控制板数据解析：（0x79）
        public string Control_Panel_High_Power_DC_Insulation_Detection_V_79;
        public string Control_Panel_High_Power_DC_Insulation_Detection_I_79;
        //交流绝缘监测板：(0x80)
        public string AC_Insulation_Detection_V_80;
        public string AC_Insulation_Detection_I_80;
        //接口箱继电器板（0x25、0x26、0x28、0x29）
        public string Power_Relay_Board_V_25;
        public string Power_Relay_Board_V_26;
        public string Power_Relay_Board_V_28;
        public string Power_Relay_Board_V_29;
        //传感器接口箱信号采集板,油箱检测：0x40
        public string Tank_Detection_CH1_1;
        public string Tank_Detection_CH1_2;
        public string Tank_Detection_CH1_3;
        public string Tank_Detection_CH1_4;
        public string Tank_Detection_CH2_1;
        public string Tank_Detection_CH2_2;
        public string Tank_Detection_CH2_3;
        public string Tank_Detection_CH2_4;
        public string Tank_Detection_CH3_1;
        public string Tank_Detection_CH3_2;
        public string Tank_Detection_CH3_3;
        public string Tank_Detection_CH3_4;
        public string Tank_Detection_CH4_1;
        public string Tank_Detection_CH4_2;
        public string Tank_Detection_CH4_3;
        public string Tank_Detection_CH4_4;
        //灯舱继电器板（0x21、0x22）
        public string Light_Relay_Board_V_21;
        public string Light_Relay_Board_V_22;

        //工况采集板(0x30)	
        public string Work_Station_RotateSpeed;
        public string WorkStation_B;
        public string WorkStation_C;
        public string WorkStation_D;
        public string WorkStation_E;
        public string WorkStation_F;
        public string WorkStation_G;

        //ROV配电柜
        public string I_First_A;//三相变压器初级A相电流
        public string I_First_B;//三相变压器初级B相电流
        public string I_First_C;//三相变压器初级C相电流
        public string V_First_AB;//三相变压器初级AB相电压
        public string V_First_BC;//三相变压器初级BC相电压
        public string I_First_S;//单相变压器初级电流

        public string I_Next_ABC;//三相变压器次级电流
        public string I_Next_S;//单相变压器次级电流
        public string V_Next_ABC;//三相变压器次级电压
        public string V_Next_S;//单相变压器次级电压

        public string ALARM_T1输入电压低报警;
        public string ALARM_T1输入电压高报警;
        public string ALARM_T1输出电压高报警;
        public string ALARM_T1输入电流高报警;
        public string ALARM_T1输出电流高报警;

        public string ALARM_T2输入电压低报警;
        public string ALARM_T2输入电压高报警;
        public string ALARM_T2输出电压高报警;
        public string ALARM_T2输入电流高报警;
        public string ALARM_T2输出电流高报警;

        public string ALARM_T1输入电流A相变送器故障;
        public string ALARM_T1输入电流B相变送器故障;
        public string ALARM_T1输入电流C相变送器故障;
        public string ALARM_T1输入电压AB相变送器故障;
        public string ALARM_T1输入电压BC相变送器故障;
        public string ALARM_T2输入电流变送器故障;
        public string ALARM_T1输出电流变送器故障;
        public string ALARM_T2输出电流变送器故障;
        public string ALARM_T1输出电压变送器故障;
        public string ALARM_T2输出电压变送器故障;
        public string ALARM_相序断相不平衡故障;
        public string ALARM_T1_高压绝缘故障;
        public string ALARM_T2_高压绝缘故障;
        public string ALARM_急停故障;

        public string ALARM_T1输入电压低低故障;
        public string ALARM_T1输入电压高高故障;
        public string ALARM_T1输出电压高高故障;

        public string ALARM_T1输入电流高高故障;
        public string ALARM_T1输出电流高高故障;

        public string ALARM_T2输入电压低低故障;
        public string ALARM_T2输入电压高高故障;
        public string ALARM_T2输出电压高高故障;

        public string ALARM_T2输入电流高高故障;
        public string ALARM_T2输出电流高高故障;

        //绝缘检测仪1
        public string sMeauringValue_1;
        public string sAlarm1Value_1;
        public string sAlarm2Value_1;
        public string sK1_K2_OnOff_1;
        public string sAlarm1_2_None_1;
        public string sAC_DC_Fault_1;

        //绝缘检测仪2
        public string sMeauringValue_2;
        public string sAlarm1Value_2;
        public string sAlarm2Value_2;
        public string sK1_K2_OnOff_2;
        public string sAlarm1_2_None_2;
        public string sAC_DC_Fault_2;

        //高度计
        public string Hight_Measure_Height;

        //罗盘
        public string Rotate_Panel_HX;
        public string Rotate_Panel_HY;
        public string Rotate_Panel_HZ;
        public string Rotate_Panel_Roll;
        public string Rotate_Panel_Pitch;
        public string Rotate_Panel_Yaw;

        //电机温度检测
        public string DanJi_T_Para_1;
        public string DanJi_T_Para_2;
        public string DanJi_T_Para_3;
        public string DanJi_T_Para_4;
        public string DanJi_T_Para_5;
        public string DanJi_T_Para_6;
        public string DanJi_T_Para_7;
        public string DanJi_T_Para_8;

        //控制盒（0x10）
        public string Water_Control_Box_RotAxisX;
        public string Water_Control_Box_RotAxisY;
        public string Water_Control_Box_RotAxisZ;
        public string Water_Control_Box_RotAxisV;
        public string Water_Control_Box_Space1;
        public string Water_Control_Box_Space2;
        public string Water_Control_Box_KKInfo;
        //8功能阀箱
        public byte ServoValvePackI_Message_ID;
        public string ServoValvePackI_CPU_Status;
        public string ServoValvePackI_Received_Bad_CRCs;
        public string ServoValvePackI_Address;
        public string ServoValvePackI_Digital_Inputs;
        public string ServoValvePackI_bDIN8;
        public string ServoValvePackI_bDIN7;
        public string ServoValvePackI_bDIN6;
        public string ServoValvePackI_bDIN5;
        public string ServoValvePackI_bDIN4;
        public string ServoValvePackI_bDIN3;
        public string ServoValvePackI_bDIN2;
        public string ServoValvePackI_bDIN1;
        public string ServoValvePackI_Main_Supply_Voltage_24V;
        public string ServoValvePackI_Analog_Supply_Voltage_15V;
        public string ServoValvePackI_Digital_Supply_Voltage_5V;
        public string ServoValvePackI_Analog_Supply_Voltage_5V;
        public string ServoValvePackI_Current_Feedback_1;
        public string ServoValvePackI_Current_Feedback_2;
        public string ServoValvePackI_Analog_Input_17;
        public string ServoValvePackI_Analog_Input_18;
        public string ServoValvePackI_Analog_Input_19;
        public string ServoValvePackI_Analog_Input_20;
        public string ServoValvePackI_Analog_Input_1;
        public string ServoValvePackI_Analog_Input_2;
        public string ServoValvePackI_Analog_Input_3;
        public string ServoValvePackI_Analog_Input_4;
        public string ServoValvePackI_Analog_Input_5;
        public string ServoValvePackI_Analog_Input_6;
        public string ServoValvePackI_Analog_Input_7;
        public string ServoValvePackI_Analog_Input_8;
        public string ServoValvePackI_Analog_Input_9;
        public string ServoValvePackI_Analog_Input_10;
        public string ServoValvePackI_Analog_Input_11;
        public string ServoValvePackI_Analog_Input_12;
        public string ServoValvePackI_Analog_Input_13;
        public string ServoValvePackI_Analog_Input_14;
        public string ServoValvePackI_Analog_Input_15;
        public string ServoValvePackI_Analog_Input_16;
        public string ServoValvePackI_Temperature;
        //16功能阀箱（国外版）
        public string ServoValvePackII_Message_ID;
        public string ServoValvePackII_CPU_Status;
        public string ServoValvePackII_Received_Bad_CRCs;
        public string ServoValvePackII_Address;
        public string ServoValvePackII_Digital_Inputs;
        public string ServoValvePackII_bDIN8;
        public string ServoValvePackII_bDIN7;
        public string ServoValvePackII_bDIN6;
        public string ServoValvePackII_bDIN5;
        public string ServoValvePackII_bDIN4;
        public string ServoValvePackII_bDIN3;
        public string ServoValvePackII_bDIN2;
        public string ServoValvePackII_bDIN1;
        public string ServoValvePackII_Current_Feedback_PWM_1_2;
        public string ServoValvePackII_Current_Feedback_PWM_3_4;
        public string ServoValvePackII_Current_Feedback_PWM_5_6;
        public string ServoValvePackII_Current_Feedback_PWM_7_8;
        public string ServoValvePackII_Current_Feedback_PWM_9_10;
        public string ServoValvePackII_Current_Feedback_PWM_11_12;
        public string ServoValvePackII_Current_Feedback_PWM_13_14;
        public string ServoValvePackII_Current_Feedback_PWM_15_16;
        public string ServoValvePackII_VCCD;
        public string ServoValvePackII_VCCA;
        public string ServoValvePackII_External_Analog_In_1;
        public string ServoValvePackII_External_Analog_In_2;
        public string ServoValvePackII_External_Analog_In_3;
        public string ServoValvePackII_External_Analog_In_4;
        public string ServoValvePackII_Main_Supply_Voltage_24VDC;
        public string ServoValvePackII_Main_Supply_Voltage_15VDC;
        public string ServoValvePackII_Current_Feedback_DOUT1_8_SSUP_1_2;
        public string ServoValvePackII_Current_Feedback_DOUT9_16_SSUP_3_4;
        public string ServoValvePackII_Temperature;
        //16功能阀箱（国产版）
        public string ServoValvePackIIB_V;
        public string ServoValvePackIIB_I;
        public string ServoValvePackIIB_P;
        public string ServoValvePackIIB_isLeakage;

    };


    //BoardA参数结构体
    public struct Struct_BoardA_Status
    {
        public byte Substitution_Character;
        public byte Message_ID;
        public byte CPU_Status;
        public int Received_Bad_CRCs;
        public byte Address;
        public byte Digital_Inputs;
        public int bDIN8;
        public int bDIN7;
        public int bDIN6;
        public int bDIN5;
        public int bDIN4;
        public int bDIN3;
        public int bDIN2;
        public int bDIN1;
        public double Main_Supply_Voltage_24V;
        public double Analog_Supply_Voltage_15V;
        public double Digital_Supply_Voltage_5V;
        public double Analog_Supply_Voltage_5V;
        public double Current_Feedback_1;
        public double Current_Feedback_2;
        public double Analog_Input_17;
        public double Analog_Input_18;
        public double Analog_Input_19;
        public double Analog_Input_20;
        public double Analog_Input_1;
        public double Analog_Input_2;
        public double Analog_Input_3;
        public double Analog_Input_4;
        public double Analog_Input_5;
        public double Analog_Input_6;
        public double Analog_Input_7;
        public double Analog_Input_8;
        public double Analog_Input_9;
        public double Analog_Input_10;
        public double Analog_Input_11;
        public double Analog_Input_12;
        public double Analog_Input_13;
        public double Analog_Input_14;
        public double Analog_Input_15;
        public double Analog_Input_16;
        public double Temperature;
        public byte RS232_2_Received_Data;
        public byte CAN_1_Received_Data;
        public byte CAN_2_Received_Data;
        public byte CRC16_1;
        public byte CRC16_2;
        public byte CRC16_1_Copmputed;
        public byte CRC16_2_Copmputed;

        public string sData;
        public string sCRCResult;
        public List<int> indexSubstitution;
    };

    //BoardB参数结构体
    public struct Struct_BoardB_Status
    {
        public enum_BoardTypeClass boardTypeClass;

        //国外版，状态
        public byte Substitution_Character;
        public byte Message_ID;
        public byte CPU_Status;
        public int Received_Bad_CRCs;
        public byte Address;

        public byte Digital_Inputs;
        public int bDIN8;
        public int bDIN7;
        public int bDIN6;
        public int bDIN5;
        public int bDIN4;
        public int bDIN3;
        public int bDIN2;
        public int bDIN1;

        public double Current_Feedback_PWM_1_2;
        public double Current_Feedback_PWM_3_4;
        public double Current_Feedback_PWM_5_6;
        public double Current_Feedback_PWM_7_8;
        public double Current_Feedback_PWM_9_10;
        public double Current_Feedback_PWM_11_12;
        public double Current_Feedback_PWM_13_14;
        public double Current_Feedback_PWM_15_16;

        public double VCCD;
        public double VCCA;

        public double External_Analog_In_1;
        public double External_Analog_In_2;
        public double External_Analog_In_3;
        public double External_Analog_In_4;

        public double Main_Supply_Voltage_24VDC;
        public double Main_Supply_Voltage_15VDC;

        //All previous analog inputs: 0V-5V equates to a range of 0x0000-0x0FFF, (12 bits).

        public double Current_Feedback_DOUT1_8_SSUP_1_2;
        public double Current_Feedback_DOUT9_16_SSUP_3_4;

        //All previous analog inputs: 0V-10V equates to a range of 0x0000-0x0FFF, (12 bits).

        public double Temperature;//Refer to Maxim DS1620 data sheet, range 0x0000-0x01FF spans range of -55oC to +125oC.



        public byte RS232_2_Received_Data;
        public byte CAN_1_Received_Data;
        public byte CAN_2_Received_Data;
        public byte CRC16_1;
        public byte CRC16_2;
        public byte CRC16_1_Copmputed;
        public byte CRC16_2_Copmputed;


        public string sData;
        public string sCRCResult;
        public List<int> indexSubstitution;

        //国产版，状态
        //电压、电流、压力的输出值已经过标定电压：0-300 对应 0-30V,电流：0-200 对应 0-20A，压力：0-250 对应 0-25MPa,漏水检测：FFFF 为漏水，0000 为正常
        public double V;
        public double I;
        public double P;
        public bool isLeakage;

    }

    //直流绝缘检测板数据（0x61、0x62、0x63）
    public struct Struct_DC_Insulation_Detection_Board
    {
        public int type;    //0x80-自检指令返回；0x81-查询指令返回；0x82-执行指令返回
        public int Address;

        //0x61,0x62,0x63
        public double VA;//电压
        public double IA;//电流
        public double GA;//绝缘
        public double TA;//温度
        public double VB;//电压
        public double IB;//电流
        public double GB;//绝缘
        public double TB;//温度

        public double TO;//外接温度传感器的数值

        //查询指令返回数据
        public byte[] bCmdReturn;
    }

    //大功率直流绝缘检测板数据解析：（0x70、0x71、0x72）
    public struct Struct_High_Power_DC_Insulation_Detection_Board
    {
        public int type;    //0x80-自检指令返回；0x81-查询指令返回；0x82-执行指令返回
        public int Address;

        //0x70、0x71、0x72
        public double V;//电压
        public double I;//电流
        public double G;//绝缘
        public double T;//温度

        //查询指令返回数据
        public byte[] bCmdReturn;
    }

    //大功率直流绝缘检测板控制板数据解析：（0x79）
    public struct Struct_Control_Panel_High_Power_DC_Insulation_Detection_Board
    {
        public int type;    //0x80-自检指令返回；0x81-查询指令返回；0x82-执行指令返回
        public int Address;

        //0x79
        public double V;//电压
        public double I;//电流

        //查询指令返回数据
        public byte[] bCmdReturn;
    }

    //交流绝缘监测板：(0x80)
    public struct Struct_AC_Insulation_Detection_Board
    {
        public int type;    //0x80-自检指令返回；0x81-查询指令返回；0x82-执行指令返回
        public int Address;

        //0x80
        public double V;//电压
        public double I;//电流

        //查询指令返回数据
        public byte[] bCmdReturn;
    }

    //舱内备用电源继电器板（0x90）
    public struct Struct_Inboard_Backup_Power_Relay_Board
    {
        public int type;    //0x80-自检指令返回；0x81-查询指令返回；0x82-执行指令返回
        public int Address;
                
        //查询指令返回数据
        public byte[] bCmdReturn;
    }


    //接口箱继电器板（0x25、0x26、0x28、0x29）
    public struct Struct_Power_Relay_Board
    {
        public int type;    //0x80-自检指令返回；0x81-查询指令返回；0x82-执行指令返回
        public int Address;

        //0x25、0x26、0x28、0x29
        public double V;//电压

        //查询指令返回数据
        public byte[] bCmdReturn;
    }

    //传感器接口箱信号采集板：0x40
    public struct Struct_Tank_Detection_Board
    {
        public int type;    //0x80-自检指令返回；0x81-查询指令返回；0x82-执行指令返回
        public int Address;

        //0x40
        public double CH1_1;
        public double CH1_2;
        public double CH1_3;
        public double CH1_4;
        public double CH2_1;
        public double CH2_2;
        public double CH2_3;
        public double CH2_4;
        public double CH3_1;
        public double CH3_2;
        public double CH3_3;
        public double CH3_4;
        public double CH4_1;
        public double CH4_2;
        public double CH4_3;
        public double CH4_4;

        //查询指令返回数据
        public byte[] bCmdReturn;
    }

    //灯舱继电器板（0x21、0x22）
    public struct Struct_Light_Relay_Board
    {
        public int type;    //0x80-自检指令返回；0x81-查询指令返回；0x82-执行指令返回
        public int Address;

        //0x21、0x22
        public double V;//电压

        //查询指令返回数据
        public byte[] bCmdReturn;
    }

    //灯舱控制板（0x23、0x24）
    public struct Struct_Light_Control_Panel
    {
        public int type;    //0x80-自检指令返回；0x81-查询指令返回；0x82-执行指令返回
        public int Address;


        //查询指令返回数据
        public byte[] bCmdReturn;
    }

    //工况采集板
    public struct Struct_Work_Station_Quire_Board
    {
        public int type;    //0x80-自检指令返回；0x81-查询指令返回；0x82-执行指令返回
        public int Address;

        public int RotateSpeed;//转速 AH*256+AL；
        public int WorkStation_B;//工位，0：无信号，1到达此处
        public int WorkStation_C;
        public int WorkStation_D;
        public int WorkStation_E;
        public int WorkStation_F;
        public int WorkStation_G;

        //查询指令返回数据
        public byte[] bCmdReturn;
    };

    //绝缘检测仪
    public struct Struct_JueYuanJianCeYi
    {
        public int type;
        public int Address;

        public string sMeauringValue;
        public string sAlarm1Value;
        public string sAlarm2Value;
        public string sK1_K2_OnOff;
        public string sAlarm1_2_None;
        public string sAC_DC_Fault;

        //查询指令返回数据
        public byte[] bCmdReturn;
    };

    //控制盒（0x10）
    public struct Struct_Water_Control_Box
    {
        public int type;    //0x80-自检指令返回；0x81-查询指令返回；0x82-执行指令返回
        public int Address;
        
        //操作杆向前推前进，向后推后退（第一路模拟量）；
        //向左推左移，向右推右移（第二路模拟量）；
        //旋转时钻机旋转（第三路模拟量），
        //上升下潜旋钮，（第四路模拟量），向右旋动下潜，向左旋动上浮（输入范围时0-5V，中间值2.5V），实际采集到的换算前的数值是3V-0.52V，换算公式以前给过你的，可能和实际有点偏差。

        //0~2.4V对应推力器的-10V~0V；2.6V-5V对应推力器0V-10V

        public double RotAxisX;//操作杆向前推前进，向后推后退（第一路模拟量）；
        public double RotAxisY;//操作杆向左推左移，向右推右移（第二路模拟量）；
        public double RotAxisZ;//旋转时钻机旋转（第三路模拟量）,Z轴增大右转，减小左转。
        public double RotAxisV;//上升下潜旋钮，（第四路模拟量），向右旋动下潜，向左旋动上浮
        public double Space1;
        public double Space2;

        public byte KKInfo;//KK是按键信息，为1未按下，为0表示对应键按下。目前有四个开关，占用低4位，实际中暂未使用。

        public bool IsAutoDir;
        public bool IsAutoHigh;

        //查询指令返回数据
        public byte[] bCmdReturn;
    };

    //控制盒（0x23）
    public struct Struct_Water_Control_Box_New
    {
        public int type;    //0x80-自检指令返回；0x81-查询指令返回；0x82-执行指令返回
        public int Address;

        //查询数据：
        //FF FF A5 23 01 05 29 26
        //返回数据
        //FF FF A5 23 81 30 IO1 IO2 IO3 IO4 IO5 IO6 IO7 IO8 A1H A1L A2H A2L ……A16H A16L PP 26

        //现有8路
        public byte[] IO;

        //IO1的第0位为1时没动作，为0时定向；执行指令后，要向控制盒发送一个指令
        //FF FFA5 23 02 09 01 00 00 00 2F 26，以点亮对应指示灯
        //这条指令会反馈一个：FF FF A5 23 82 05 AA 26



        //(A1H*256+A1L)*5/4096
        //现有16路
        public double[] A;

        //第一路模拟量：0-2.3 左移，2.7-5右移，2.3-2.7之间作为死区，不动作
        //第二路模拟量：0-2.3 前进，2.7-5后退，2.3-2.7之间作为死区，不动作
        //第三路模拟量：0-2.3 左转，2.7-5右转，2.3-2.7之间作为死区，不动作
        //第四路模拟量：0-2.3 上浮，2.7-5下潜，2.3-2.7之间作为死区，不动作
        //以上四路模拟量，2.7-5V之间电压越高动作幅度越大，0-2.3V之间，电压越低动作幅度越大，即将2.5V附近作为中位值，离中位值越近动作幅度越小，越远离中位，动作幅度越大。


        public bool IsAutoDir;
        public bool IsAutoHigh;

        //查询指令返回数据
        public byte[] bCmdReturn;
    };

    //高度计
    public struct Struct_HightMeasureDevice
    {
        public int type;
        public int Address;

        public string sHight;
        public double Height;

        //查询指令返回数据
        public byte[] bCmdReturn;
    };

    //罗盘
    public struct Struct_RotatePanelDevice
    {
        public int type;
        public int Address;

        public double HX;       //X 轴磁场
        public double HY;       //Y 轴磁场
        public double HZ;       //Z 轴磁场
        public double Roll;     //X 轴角度
        public double Pitch;    //Y 轴角度
        public double Yaw;      //Z 轴角度

        //查询指令返回数据
        public byte[] bCmdReturn;
    };

    //电机温度检测板
    public struct Struct_DianJi_T
    {
        public int type;
        public int Address;

        public double Para_1;
        public double Para_2;
        public double Para_3;
        public double Para_4;
        public double Para_5;

        public double Para_6;
        public double Para_7;
        public double Para_8;

        //查询指令返回数据
        public byte[] bCmdReturn;
    };


    //ROV供电系统
    /*
    1	三相变压器初级A相电流	40001	REAL	低位在前	VD1000
    2	三相变压器初级B相电流	40003	REAL	低位在前	VD1004
    3	三相变压器初级C相电流	40005	REAL	低位在前	VD1008
    4	三相变压器初级AB相电压	40007	REAL	低位在前	VD1012
    5	三相变压器初级BC相电压	40009	REAL	低位在前	VD1016
    6	单相变压器初级电流	40011	REAL	低位在前	VD1020
    7	备用模拟量1	40013	REAL	低位在前	VD1024
    8	备用模拟量2	40015	REAL	低位在前	VD1028
    9	三相变压器次级电流	40017	REAL	低位在前	VD1032
    10	单相变压器次级电流	40019	REAL	低位在前	VD1036
    11	三相变压器次级电压	40021	REAL	低位在前	VD1040
    12	单相变压器次级电压	40023	REAL	低位在前	VD1044
    
    上述12个量，共计24个寄存器，读取的命令为：02 03 00 01 00 18 14 33  
    
    28	Warnning WORD1	40100	WORD	VW90/高低反	VW1200
    29	Warnning WORD2	40101	WORD	VW92/高低反	VW1202
    30	ALARM WORD1	40102	WORD	VW94/高低反	VW1204
    31	ALARM WORD2	40103	WORD	VW96/高低反	VW1206
    32	ALARM WORD3	40104	WORD	VW98/高低反	VW1208

    上述5个量，共计4个寄存器，读取的命令为：02 03 00 64 00 05 C4 25 
    
     
类别	等级	地址类型	PLC地址 (读取)	对应Modbus地址	内容	PLC转换地址
警告	Middle	Bit	V90.0	40100.0	T1输入电压低报警	VW1200
警告	Middle	Bit	V90.1	40100.1	T1输入电压高报警	
警告	Middle	Bit	V90.2	40100.2	T1输出电压高报警	
警告	Middle	Bit	V90.3	40100.3	T1输入电流高报警	
警告	Middle	Bit	V90.4	40100.4	T1输出电流高报警	
警告	Middle	Bit	V90.5	40100.5		
警告	Middle	Bit	V90.6	40100.6		
警告	Middle	Bit	V90.7	40100.7		
警告	Middle	Bit	V91.0	40100.8	T2输入电压低报警	
警告	Middle	Bit	V91.1	40100.9	T2输入电压高报警	
警告	Middle	Bit	V91.2	40100.10	T2输出电压高报警	
警告	Middle	Bit	V91.3	40100.11	T2输入电流高报警	
警告	Middle	Bit	V91.4	40100.12	T2输出电流高报警	
警告	Middle	Bit	V91.5	40100.13		
警告	Middle	Bit	V91.6	40100.14		
警告	Middle	Bit	V91.7	40100.15		
警告	Middle	WORD	VW92	40101	警告备用字	VW1202
故障	High	Bit	V94.0	40102.0	T1输入电流A相变送器故障	VW1204
故障	High	Bit	V94.1	40102.1	T1输入电流B相变送器故障	
故障	High	Bit	V94.2	40102.2	T1输入电流C相变送器故障	
故障	High	Bit	V94.3	40102.3	T1输入电压AB相变送器故障	
故障	High	Bit	V94.4	40102.4	T1输入电压BC相变送器故障	
故障	High	Bit	V94.5	40102.5	T2输入电流变送器故障	
故障	High	Bit	V94.6	40102.6	T1输出电流变送器故障	
故障	High	Bit	V94.7	40102.7	T2输出电流变送器故障	
故障	High	Bit	V95.0	40102.8	T1输出电压变送器故障	
故障	High	Bit	V95.1	40102.9	T2输出电压变送器故障	
故障	High	Bit	V95.2	40102.10	相序/断相/不平衡故障（8%）	
故障	High	Bit	V95.3	40102.11	T1_高压绝缘故障	
故障	Emergency	Bit	V95.4	40102.12	T2_高压绝缘故障	
故障	High	Bit	V95.5	40102.13	急停故障	
故障	High	Bit	V95.6	40102.14		
故障	High	Bit	V95.7	40102.15		
故障	High	Bit	V96.0	40103.0	T1输入电压低低故障	VW1206
故障	High	Bit	V96.1	40103.1	T1输入电压高高故障	
故障	High	Bit	V96.2	40103.2	T1输出电压高高故障	
故障	High	Bit	V96.3	40103.3		
故障	High	Bit	V96.4	40103.4	T1输入电流高高故障	
故障	High	Bit	V96.5	40103.5	T1输出电流高高故障	
故障	High	Bit	V96.6	40103.6		
故障	High	Bit	V96.7	40103.7		
故障	High	Bit	V97.0	40103.8	T2输入电压低低故障	
故障	High	Bit	V97.1	40103.9	T2输入电压高高故障	
故障	High	Bit	V97.2	40103.10	T2输出电压高高故障	
故障	High	Bit	V97.3	40103.11		
故障	High	Bit	V97.4	40103.12	T2输入电流高高故障	
故障	High	Bit	V97.5	40103.13	T2输出电流高高故障	
故障	High	Bit	V97.6	40103.14	T2备用故障1	
故障	High	Bit	V97.7	40103.15	T2备用故障2	
 
    
    */
    public struct Struct_ROVPower_CtlSystem
    {
        public int type;    //0x80-自检指令返回；0x81-查询指令返回；0x82-执行指令返回
        public int Address;

        public int typeMidData;//1:前面12个量的数据；2：后面报警信息

        public double I_First_A;//三相变压器初级A相电流
        public double I_First_B;//三相变压器初级B相电流
        public double I_First_C;//三相变压器初级C相电流
        public double V_First_AB;//三相变压器初级AB相电压
        public double V_First_BC;//三相变压器初级BC相电压
        public double I_First_S;//单相变压器初级电流

        public double I_Next_ABC;//三相变压器次级电流
        public double I_Next_S;//单相变压器次级电流
        public double V_Next_ABC;//三相变压器次级电压
        public double V_Next_S;//单相变压器次级电压

        public ushort Warnning_WORD1;//Warnning WORD1
        public ushort Warnning_WORD2;//Warnning WORD2
        public ushort ALARM_WORD1;//ALARM WORD1
        public ushort ALARM_WORD2;//ALARM WORD2
        public ushort ALARM_WO3RD;//ALARM WORD3

        public bool ALARM_T1输入电压低报警;
        public bool ALARM_T1输入电压高报警;
        public bool ALARM_T1输出电压高报警;
        public bool ALARM_T1输入电流高报警;
        public bool ALARM_T1输出电流高报警;

        public bool ALARM_T2输入电压低报警;
        public bool ALARM_T2输入电压高报警;
        public bool ALARM_T2输出电压高报警;
        public bool ALARM_T2输入电流高报警;
        public bool ALARM_T2输出电流高报警;

        public bool ALARM_T1输入电流A相变送器故障;
        public bool ALARM_T1输入电流B相变送器故障;
        public bool ALARM_T1输入电流C相变送器故障;
        public bool ALARM_T1输入电压AB相变送器故障;
        public bool ALARM_T1输入电压BC相变送器故障;
        public bool ALARM_T2输入电流变送器故障;
        public bool ALARM_T1输出电流变送器故障;
        public bool ALARM_T2输出电流变送器故障;
        public bool ALARM_T1输出电压变送器故障;
        public bool ALARM_T2输出电压变送器故障;
        public bool ALARM_相序断相不平衡故障;
        public bool ALARM_T1_高压绝缘故障;
        public bool ALARM_T2_高压绝缘故障;
        public bool ALARM_急停故障;

        public bool ALARM_T1输入电压低低故障;
        public bool ALARM_T1输入电压高高故障;
        public bool ALARM_T1输出电压高高故障;

        public bool ALARM_T1输入电流高高故障;
        public bool ALARM_T1输出电流高高故障;

        public bool ALARM_T2输入电压低低故障;
        public bool ALARM_T2输入电压高高故障;
        public bool ALARM_T2输出电压高高故障;

        public bool ALARM_T2输入电流高高故障;
        public bool ALARM_T2输出电流高高故障;


        //查询指令返回数据
        public byte[] bCmdReturn;
    };


    public struct alarmInfoShowUnit
    {
        public int rowHao;  //显示表中的行号，DataGridView中的行号
        public int biaoHao; //报警对应的表序号，配置状态excel表中的序号，1000以后为系统内部报警序号
        public int alarm;   //报警级别，0-不报警；1-黄色预警；2-红色报警
        public string info; //报警显示信息
    }

    public struct alarmInfoUnit
    {
        public int index;  //报警对应的表序号
        public int alarm;  //报警级别，0-不报警；1-黄色预警；2-红色报警
        public string info;//报警显示信息
    }


    //事件参数
    public class GEventArgs : EventArgs
    {
        public object obj;          //存放数据区域，原始数据，十六进制
        public object objParse;     //存放解析后的数据，一般为定义的结构体
        public string nameSerial;   //串口名称
        public enum_AddressBoard addressBoard;
        public enum_SerialCOMType serialCOMType;
        public int dataType;
        //消息类型
        //0-上报接收到的串口数据（解析后）
        //1-串口连接状态（串口断开后上报）
        //2-接收到的原始查询指令返回数据，十六进制
        //3-软件内部信息
        //5-接收到的原始数据+解析后的数据
        //6-接收到的原始执行指令返回数据
        //10-button按钮状态(BackColor)变化信息，对应于一般板卡
        //11-button按钮状态(ImageIndex)变化信息，对于16功能阀箱
        //12-button按钮状态变化信息，对于16功能阀箱
        //13-button按钮状最终执行结果，对于16功能阀箱
        //14-执行设置操作，将设置指令传出，对于16功能阀箱


        public bool connected;      //串口连接状态
        public List<alarmInfoUnit> listAlarmInfo = new List<alarmInfoUnit>();
        public string source;   //1-DeviceForm中发送指令过程中，总电源指令发送后可能没有反馈，需要提示信息
        public string message;  //与source一起使用，报警显示的信息
        public int messageID;   //用于DataGridView中显示报警信息的序号，从1000开始顺序排

        //用于button按钮状态变化后的参数传递
        public Struct_Btn_Status_EventSend myStruct_Btn_Status_EventSend;
        public bool isBtnOperOK = false;
    }

    public struct Struct_Btn_Status_EventSend
    {
        public string sName;
        public Color backColor;//对应一般开关按钮，颜色代表开关状态；
        public int imageIndex;//对应16功能阀箱，0-红色，代表关闭；1-绿色，代表开启；
        public string sInfo;//执行信息
        public bool RecvOK;//控制数据发送后的返回数据比对结果
    };

    public enum enum_ModeType
    {
        None = 0,

        Mode1 = 1,//移动钻机
        Mode2 = 2,//6米钻机
        Mode3 = 3,//1.5米钻机
    };

    public enum enum_BoardType
    {
        None = 0,

        BoardA = 0x01,  //8功能阀箱
        BoardB = 0x02,  //16功能阀箱
        BoardC = 0x03,  //自研板卡，通过串口服务器UDP方式连接
        BoardD = 0x04,  //串口直连板卡
    };

    public enum enum_BoardTypeAndClass
    {
        None = 0,

        BoardA = 0x01,  //8功能阀箱
        BoardB = 0x02,  //16功能阀箱，国外版
        BoardC = 0x03,  //自研板卡，通过串口服务器UDP方式连接
        BoardD = 0x04,  //串口直连板卡,水面控制盒
        BoardE = 0x05,  //16功能阀箱，国产版
        BoardF = 0x06,  //串口直连板卡,ROV控制系统
        BoardG = 0x07,  //串口直连板卡,绝缘检测仪1
        BoardH = 0x08,  //串口直连板卡,绝缘检测仪2
    };

    //16功能阀箱，子类别，分为国外版、国产版
    public enum enum_BoardTypeClass
    {
        None = 0,

        ClassA = 0x01,  //国外版
        ClassB = 0x02,  //国产版
    }

    public enum enum_AddressBoard
    {
        Water_Control_Box = 0x10,//水面控制盒
        Rov_Power_Box = 0x11,//ROV供电系统
        JueYuanJianCe_Board_1 = 0x12,//绝缘检测仪1
        JueYuanJianCe_Board_2 = 0x13,//绝缘检测仪2
        Water_Control_Box_New = 0x14,//水面控制盒，新协议版本

        Light_Relay_Board_I = 0x21,// 灯继电器板#1(灯舱继电器板)
        Light_Relay_Board_II = 0x22,//灯继电器板#2(灯舱继电器板)
        Light_Control_Panel_I = 0x23,//灯控制板#1(灯舱控制板)
        Light_Control_Panel_II = 0x24,//灯控制板#2(灯舱控制板)

        Camera_Power_Relay_Board_I = 0x25,//摄像机电源继电器板#1
        Camera_Power_Relay_Board_II = 0x26,//摄像机电源继电器板#2
        Sensor_Power_Relay_Board_I = 0x28,//传感器电源继电器板#1
        Sensor_Power_Relay_Board_II = 0x29,//传感器电源继电器板#2

        Work_Station_Quire_Board = 0x30,//工况采集板

        Tank_Detection_Board = 0x40,//油箱采集板#1,(传感器接口箱信号采集板)

        DianJi_T_Detection_Board = 0x45,//电机温度检测板

        Battery_Power_Conversion_Board = 0x50,//电池电源转换板

        DC_Insulation_Detection_Board_I = 0x61,//直流绝缘检测板#1
        DC_Insulation_Detection_Board_II = 0x62,//直流绝缘检测板#2
        DC_Insulation_Detection_Board_III = 0x63,//直流绝缘检测板#3

        High_Power_DC_Insulation_Detection_Board_I = 0x70,//大功率直流绝缘检测板#1
        High_Power_DC_Insulation_Detection_Board_II = 0x71,//大功率直流绝缘检测板#2
        High_Power_DC_Insulation_Detection_Board_III = 0x72,//大功率直流绝缘检测板#3

        Control_Panel_High_Power_DC_Insulation_Detection_Board = 0x79,//大功率直流绝缘检测板控制板

        AC_Insulation_Detection_Board = 0x80,//交流绝缘检测板

        Inboard_Backup_Power_Relay_Board = 0x90,//舱内备用电源继电器板

        ServoValvePacketBoard8Func = 0xA0,//8功能阀箱
        ServoValvePacketBoard16Func = 0xB0,//16功能阀箱,进口版协议

        Hight_Measure_Device = 0xFFF0,//高度计
        Rotate_Panel_Device = 0xFFF1,//罗盘

        Others = 0x00,//其他
    };

    //IP	端口	设备
    //192.168.1.101	4001	高度计
    //192.168.1.101	4002	罗盘
    //192.168.1.103	4017	6块绝缘检测板61、62、63、70、71、72
    //192.168.1.103	4018	交流绝缘检测板、大绝缘检测板控制板、舱内继电器板79,80,90
    //192.168.1.103	4019	信号接口箱内四个继电器板，25,26,28,29
    //192.168.1.103	4020	传感器接口箱0x40
    //192.168.1.103	4021	灯舱2个继电器板和控制板21、22、23、24
    //192.168.1.103	4022	阀箱#1
    //192.168.1.103	4023	阀箱#2


    public enum enum_SerialCOMType
    {
        Detection_Board = 0x01,//192.168.1.103	4017	6块绝缘检测板61、62、63、70、71、72
        Control_AC_Inboard_Board = 0x02,//192.168.1.103	4018	交流绝缘检测板、大绝缘检测板控制板、舱内继电器板79,80,90
        Camera_Sensor_Power_Relay_Board = 0x03,//192.168.1.103	4019	信号接口箱内四个继电器板，25,26,28,29
        Tank_Detection_Board = 0x04,//192.168.1.103	4020	传感器接口箱0x40
        Light_Relay_Control_Panel = 0x05,//192.168.1.103	4021	灯舱2个继电器板和控制板21、22、23、24
        Hight_Measure_Device = 0x06,//192.168.1.101	4001	高度计
        Rotate_Panel_Device = 0x07,//192.168.1.101	4002	罗盘
        ServoValvePackI = 0x08,//192.168.1.103 4022	阀箱#1(8功能阀箱)，地址A0
        ServoValvePackII = 0x09,//192.168.1.103 4023	阀箱#2(16功能阀箱)，地址B0

        Others = 0x00

    };
    

    public struct struct_SerialConfParas
    {
        public enum_BoardType type;
        public string serialname;
        public string serialnum;
        public IPAddress address;
        public int port;
        public List<string> quirys;
        public int quiryinterval;

        //串口参数
        public string portname;//真实连接的物理串口名称
        public int bandrate;//波特率
        public int databits;//数据位数
        public string stopbits;//终止位数：None、One、OnePointFive、Two
        public string parity;//校验：Even、Mark、None、Odd、Space

        //16功能阀箱，子类别，分为国外版、国产版
        public enum_BoardTypeClass boradClass;
    };

    class Global
    {
        
        public static Dictionary<string, struct_SerialConfParas> dicNameSerialConf = new Dictionary<string, struct_SerialConfParas>();
        public static Dictionary<string, IntPtr> dicFormHandleIntPtr = new Dictionary<string, IntPtr>();
        public static Dictionary<string, enum_BoardTypeAndClass> dicFormNameType = new Dictionary<string, enum_BoardTypeAndClass>();

        #region CRC校验

        /* 高位字节的CRC 值 */
        public static byte[] auchCRCHi = {
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 
            0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 
            0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 
            0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 
            0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 
            0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 
            0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 
            0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 
            0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 
            0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 
            0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 
            0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 
            0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 
            0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 
            0x40
        };

        /* 低位字节的CRC 值 */
        public static byte[] auchCRCLo = {
            0x00, 0xC0, 0xC1, 0x01, 0xC3, 0x03, 0x02, 0xC2, 0xC6, 0x06, 0x07, 0xC7, 0x05, 0xC5, 0xC4,
            0x04, 0xCC, 0x0C, 0x0D, 0xCD, 0x0F, 0xCF, 0xCE, 0x0E, 0x0A, 0xCA, 0xCB, 0x0B, 0xC9, 0x09,
            0x08, 0xC8, 0xD8, 0x18, 0x19, 0xD9, 0x1B, 0xDB, 0xDA, 0x1A, 0x1E, 0xDE, 0xDF, 0x1F, 0xDD,
            0x1D, 0x1C, 0xDC, 0x14, 0xD4, 0xD5, 0x15, 0xD7, 0x17, 0x16, 0xD6, 0xD2, 0x12, 0x13, 0xD3,
            0x11, 0xD1, 0xD0, 0x10, 0xF0, 0x30, 0x31, 0xF1, 0x33, 0xF3, 0xF2, 0x32, 0x36, 0xF6, 0xF7,
            0x37, 0xF5, 0x35, 0x34, 0xF4, 0x3C, 0xFC, 0xFD, 0x3D, 0xFF, 0x3F, 0x3E, 0xFE, 0xFA, 0x3A,
            0x3B, 0xFB, 0x39, 0xF9, 0xF8, 0x38, 0x28, 0xE8, 0xE9, 0x29, 0xEB, 0x2B, 0x2A, 0xEA, 0xEE,
            0x2E, 0x2F, 0xEF, 0x2D, 0xED, 0xEC, 0x2C, 0xE4, 0x24, 0x25, 0xE5, 0x27, 0xE7, 0xE6, 0x26,
            0x22, 0xE2, 0xE3, 0x23, 0xE1, 0x21, 0x20, 0xE0, 0xA0, 0x60, 0x61, 0xA1, 0x63, 0xA3, 0xA2,
            0x62, 0x66, 0xA6, 0xA7, 0x67, 0xA5, 0x65, 0x64, 0xA4, 0x6C, 0xAC, 0xAD, 0x6D, 0xAF, 0x6F,
            0x6E, 0xAE, 0xAA, 0x6A, 0x6B, 0xAB, 0x69, 0xA9, 0xA8, 0x68, 0x78, 0xB8, 0xB9, 0x79, 0xBB,
            0x7B, 0x7A, 0xBA, 0xBE, 0x7E, 0x7F, 0xBF, 0x7D, 0xBD, 0xBC, 0x7C, 0xB4, 0x74, 0x75, 0xB5,
            0x77, 0xB7, 0xB6, 0x76, 0x72, 0xB2, 0xB3, 0x73, 0xB1, 0x71, 0x70, 0xB0, 0x50, 0x90, 0x91,
            0x51, 0x93, 0x53, 0x52, 0x92, 0x96, 0x56, 0x57, 0x97, 0x55, 0x95, 0x94, 0x54, 0x9C, 0x5C,
            0x5D, 0x9D, 0x5F, 0x9F, 0x9E, 0x5E, 0x5A, 0x9A, 0x9B, 0x5B, 0x99, 0x59, 0x58, 0x98, 0x88,
            0x48, 0x49, 0x89, 0x4B, 0x8B, 0x8A, 0x4A, 0x4E, 0x8E, 0x8F, 0x4F, 0x8D, 0x4D, 0x4C, 0x8C,
            0x44, 0x84, 0x85, 0x45, 0x87, 0x47, 0x46, 0x86, 0x82, 0x42, 0x43, 0x83, 0x41, 0x81, 0x80,
            0x40
        };


        // 计算CRC16的函数
        public static byte[] CRC16(byte[] puchMsg, int usDataLen)	/* 函数以 unsigned short 类型返回 CRC  */
        {
            byte[] crcRes = new byte[2];
            try
            {
                byte uchCRCHi = 0xFF;	/* CRC 的高字节初始化*/
                byte uchCRCLo = 0xFF;	/* CRC 的低字节初始化*/
                int uIndex;	/* CRC 查询表索引*/

                for (int i = 0; i < usDataLen; i++)
                {
                    uIndex = uchCRCLo ^ puchMsg[i];	/* 计算 CRC*/
                    uchCRCLo = BitConverter.GetBytes(uchCRCHi ^ auchCRCHi[uIndex])[0];
                    uchCRCHi = auchCRCLo[uIndex];
                }
                crcRes[0] = uchCRCHi;
                crcRes[1] = uchCRCLo;
                return crcRes;
            }
            catch (Exception ex)
            {
                crcRes[0] = 0xFF;
                crcRes[1] = 0xFF;
                return crcRes;
            }
        }

        // 计算CRC16的函数
        public static byte[] CRC16(byte[] puchMsg, int indexStart, int usDataLen)	/* 函数以 unsigned short 类型返回 CRC  */
        {
            byte[] crcRes = new byte[2];
            try
            {
                byte uchCRCHi = 0xFF;	/* CRC 的高字节初始化*/
                byte uchCRCLo = 0xFF;	/* CRC 的低字节初始化*/
                int uIndex;	/* CRC 查询表索引*/

                for (int i = indexStart; i < indexStart + usDataLen; i++)
                {
                    uIndex = uchCRCLo ^ puchMsg[i];	/* 计算 CRC*/
                    uchCRCLo = BitConverter.GetBytes(uchCRCHi ^ auchCRCHi[uIndex])[0];
                    uchCRCHi = auchCRCLo[uIndex];
                }

                crcRes[0] = uchCRCHi;
                crcRes[1] = uchCRCLo;
                return crcRes;
            }
            catch (Exception ex)
            {
                crcRes[0] = 0xFF;
                crcRes[1] = 0xFF;
                return crcRes;
            }
        }

        #endregion

        #region 获得Substitution Philosophy
        public static bool getSubstitutionPhilosophy(byte[] data, int indexStart, int len, out byte bResult)
        {
            bResult = 0x00;
            try
            {
                List<byte> sample = new List<byte>();
                for (int i = 0; i <= 0xFF; i++)
                {
                    if (i == 0xAA)
                    {
                        continue;
                    }
                    sample.Add((byte)i);
                }

                if (indexStart < 0)
                {
                    indexStart = 0;
                }
                if (data.Length < indexStart + len)
                {
                    len = data.Length - indexStart;
                }
                for (int i = indexStart; i < indexStart + len; i++)
                {
                    if (sample.Exists(((byte x) => x == data[i] ? true : false)))
                    {
                        sample.Remove(data[i]);
                    }
                }
                bResult = sample.Min();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion

        #region 串口TCPIP连接
        public static bool connectSerialTCPIP(
            IPAddress addressLocal, 
            int portLocal,
            IPAddress addressRemote,
            int portRemote,
            out Socket SerialSocket,
            out string sInfo)
        {
            sInfo = "";
            SerialSocket = null;
            try
            {
                if (addressRemote == null || portRemote <= 0)
                {
                    return false;
                }
                SerialSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                IPAddress addressLocalTemp = addressLocal;

                string hostName = Dns.GetHostName();//本机名
                IPAddress[] addressList = Dns.GetHostAddresses(hostName);//返回所有地址，包括IPv4和IPv6 
                foreach (IPAddress ip in addressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        addressLocalTemp = ip;
                        if (addressLocalTemp.Equals(addressLocal))
                        {
                            break;
                        }
                    }
                }
                if (addressLocal.Equals(addressLocalTemp) == false)
                {
                    sInfo += "【" + DateTime.Now.ToString("HH:mm:ss") + "】:" + addressLocal.ToString() + "不存在，将绑定" + addressLocalTemp.ToString() + "\t\n";
                }
                addressLocal = addressLocalTemp;
                IPEndPoint iepLocal = new IPEndPoint(addressLocal, portLocal);
                //绑定本机IP
                try
                {
                    SerialSocket.Bind(iepLocal);
                    sInfo += "【" + DateTime.Now.ToString("HH:mm:ss") + "】:已绑定" + iepLocal.ToString() + "\t\n";
                }
                catch (Exception ex)
                {
                    sInfo += "【" + DateTime.Now.ToString("HH:mm:ss") + "】:绑定失败" + iepLocal.ToString() + "\t\n";
                    return false;
                }
                //连接远端IP
                try
                {
                    SerialSocket.Connect(addressRemote, portRemote);
                    sInfo += "【" + DateTime.Now.ToString("HH:mm:ss") + "】:已连接" + addressRemote.ToString() + ":" + portRemote.ToString() + "\t\n";
                }
                catch (Exception ex)
                {
                    sInfo += "【" + DateTime.Now.ToString("HH:mm:ss") + "】:连接失败" + addressRemote.ToString() + ":" + portRemote.ToString() + "\t\n";
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                sInfo += "【" + DateTime.Now.ToString("HH:mm:ss") + "】:" + "地址、端口号不能绑定！";
                return false;
            }
        }

        #endregion



        // <summary>
        /// windowapi 通过句柄显示或隐藏窗体函数
        /// </summary>
        /// <param name="hWnd">窗体句柄</param>
        /// <param name="cmdShow">显示类型（0：隐藏窗体，1：默认大小窗体，2：最小化窗体，3：最大化窗体）</param>
        /// <returns>返回成功或失败</returns>
        [DllImport("user32.dll", EntryPoint = "ShowWindowAsync", SetLastError = true)]
        public static extern bool ShowWindowAsync(IntPtr hWnd, int cmdShow);

        [DllImport("user32.dll")]
        public static extern bool CloseWindow(IntPtr hWnd);


        public static string logFilePathSaved = "";
        public static bool isCtlSending = false;
        public static StreamWriter myLogStreamWriter;
        public static FileStream fsLog;
        public static string statusFilePathSaved = "";
        public static StreamWriter myStatusStreamWriter;
        public static FileStream fsStatus;

        public static IPAddress ipAddressLocal;

        public static Dictionary<enum_ModeType, Dictionary<enum_BoardType, List<struct_SerialConfParas>>> DicModeBoardTypeSerialConf =
            new Dictionary<enum_ModeType, Dictionary<enum_BoardType, List<struct_SerialConfParas>>>();

        //Debug标志
        public static int isDebug = 0;

        //推力器的正反极性
        public static int TuiJinQiBuChang_Polar_HL = 1;
        public static int TuiJinQiBuChang_Polar_HR = 1;
        public static int TuiJinQiBuChang_Polar_VLF = 1;
        public static int TuiJinQiBuChang_Polar_VLB = 1;
        public static int TuiJinQiBuChang_Polar_VRF = 1;
        public static int TuiJinQiBuChang_Polar_VRB = 1;

        //读取推进补偿参数，悬浮补偿
        public static double TuiJinQiBuChang_HL = 0;//水平左推进器
        public static double TuiJinQiBuChang_HR = 0;//水平右推进器
        public static double TuiJinQiBuChang_VLF = 0;//前左垂直推进器
        public static double TuiJinQiBuChang_VLB = 0;//后左垂直推进器
        public static double TuiJinQiBuChang_VRF = 0;//前右垂直推进器
        public static double TuiJinQiBuChang_VRB = 0;//后右垂直推进器

        //停转各推力器补偿
        public static double TuiJinQiBuChang_HL_Zero = 0;//水平左推进器
        public static double TuiJinQiBuChang_HR_Zero = 0;//水平右推进器
        public static double TuiJinQiBuChang_VLF_Zero = 0;//前左垂直推进器
        public static double TuiJinQiBuChang_VLB_Zero = 0;//后左垂直推进器
        public static double TuiJinQiBuChang_VRF_Zero = 0;//前右垂直推进器
        public static double TuiJinQiBuChang_VRB_Zero = 0;//后右垂直推进器

        //悬浮时的整体补偿参数
        public static double TuiJinQiBuChang_XuanFu = 0;

        //配置的点动执行时间
        public static double dDrillUp = 2000;
        public static double dDrillDown = 2000;
        public static double dRotateForward = 2000;
        public static double dRotateReverse = 2000;

        //俯仰点动时间
        public static double dPitchUp = 2000;
        public static double dPitchDown = 2000;
        public static double dPitchForward = 2000;
        public static double dPitchReverse = 2000;
        
        //界面
        public static string sMonText = "状态监视";
        public static string sMonName = "FormMonMain";
        public static string sCtlText = "控制与执行";
        public static string sCtlName = "FormCtlMain";
        public static Form myFormCtl = new Form();
        public static Form myFormMon = new Form();

        //返回数据记录类
        public static DataRecvToSaveClass myDataRecvToSaveClass;
        public static UInt64 isDataComingCount = 0;//状态数据驱动此变量,递增
        public static bool isDataComing = false;//状态数据首次来时，置为true
        public static bool isCommucationOK = false;//光端机通信是否正常
        
        //国产16功能阀箱，指令返回确认
        public static byte[] bCmdDataServoValvePackSend = new byte[0];
        public static bool bFlagCmdDataServoValvePackSendOK = false;

        //指令发送时间延迟，包括发送前延迟，发送后延迟、发送后判断执行结果等待时间延迟
        public static int delay_CmdSend_Before = 80;
        public static int delay_CmdSend_After = 80;
        public static int delay_CmdSend_ResultComp = 50;//16功能阀箱

        //本机测试时，偏移 + 100
        public static int portLocalOffset = 0;


        #region 界面声明

        public static FormMainUserA myFormMainUserA;
        public static FormMobileDrillMonCtl m_FormMobileDrillMonCtl;
        public static FormBoardI m_FormBoardI;
        public static FormBoardII m_FormBoardII;
        public static FormBoardIIB m_FormBoardIIB;
        public static FormSerialWaterBoxCtl m_FormSerialWaterBoxCtl;
        public static FormSerialRovPowerCtl m_FormSerialRovPowerCtl;
        public static FormSerialJuYuanJianCe1Ctl m_FormSerialJuYuanJianCe1Ctl;
        public static FormSerialJuYuanJianCe2Ctl m_FormSerialJuYuanJianCe2Ctl;

        public static Form myFormUserMainCtl = new Form();
        public static Form myFormUserMainMon = new Form();
        public static string sFormUserMainCtlName = "myFormUserMainCtl";
        public static string sFormUserMainMonName = "myFormUserMainMon";

        public static FormAutoCtlDirParas myFormAutoCtlDirParas = null;
        public static FormAutoCtlHighParas myFormAutoCtlHighParas = null;

        public static FormMainModeI myFormMainModeI;

        public static BoardSerialMonCtlClass myBoardSerialMonCtlWaterBoxClass = null;
        public static BoardSerialMonCtlClass myBoardSerialMonCtlRovPowerClass = null;
        public static BoardSerialMonCtlClass myBoardSerialMonCtlJuYuanJianCe1Class = null;
        public static BoardSerialMonCtlClass myBoardSerialMonCtlJuYuanJianCe2Class = null;

        public static BoardServoValvePackIClass myBoardServoValvePackIClass = null;
        public static BoardServoValvePackIIClass myBoardServoValvePackIIClass = null;//16功能阀箱，国外版
        public static BoardServoValvePackIIClass myBoardServoValvePackIIBClass = null;//16功能阀箱，国产版

        #endregion



        //驱动推力器作自动定向的参数
        //1.5m移动钻机，自动定向逻辑 (对应左右两个水平推力器)
        //（1）角度差=＞5°，推力百分比30%，推力器工作2s，在2.5s后，判断角度差，
        //（2）角度差＞3°，角度差＜5°，推力百分比10%，推力器工作1s，在1.5s后，判断角度差，
        //（3）角度差＞2°角度差＜3°，推力百分比5%，推力器工作0.5s，在0.8s后，判断角度差，
        //（4）角度差＜2°，暂停，，实时根据上报的Yaw值，判断角度差，
        public static double AutoCtlDir_JiaoDuCha_1 = 5;
        public static double AutoCtlDir_JiaoDuCha_2 = 3;
        public static double AutoCtlDir_JiaoDuCha_3 = 2;
        public static double AutoCtlDir_TuiJinPercent_1 = 30;
        public static double AutoCtlDir_TuiJinPercent_2 = 10;
        public static double AutoCtlDir_TuiJinPercent_3 = 5;
        public static double AutoCtlDir_TuiLiQiWorkTime_1 = 2;
        public static double AutoCtlDir_TuiLiQiWorkTime_2 = 1;
        public static double AutoCtlDir_TuiLiQiWorkTime_3 = 0.5;
        public static double AutoCtlDir_TuiLiQiWorkTime_Compare_1 = 2.5;
        public static double AutoCtlDir_TuiLiQiWorkTime_Compare_2 = 1.5;
        public static double AutoCtlDir_TuiLiQiWorkTime_Compare_3 = 0.8;


        //1.5m移动钻机，自动定高逻辑  (对应垂直4个推力器)
        //（1）高度差=＞5m，推力百分比10%，推力器工作2s，在2.5s后，判断高度差，
        //（2）高度差＞3m，角度差＜5m，推力百分比8%，推力器工作1s，在1.5s后，判断高度差，
        //（3）高度差＞1m角度差＜3m，推力百分比3%，推力器工作0.5s，在0.8s后，判断高度差，
        //（4）高度差＜1m，暂停，，实时根据上报的高度值，判断高度差，
        public static double AutoCtlHigh_GaoDuCha_1 = 5;
        public static double AutoCtlHigh_GaoDuCha_2 = 3;
        public static double AutoCtlHigh_GaoDuCha_3 = 2;
        public static double AutoCtlHigh_TuiJinPercent_1 = 30;
        public static double AutoCtlHigh_TuiJinPercent_2 = 10;
        public static double AutoCtlHigh_TuiJinPercent_3 = 5;
        public static double AutoCtlHigh_TuiLiQiWorkTime_1 = 2;
        public static double AutoCtlHigh_TuiLiQiWorkTime_2 = 1;
        public static double AutoCtlHigh_TuiLiQiWorkTime_3 = 0.5;
        public static double AutoCtlHigh_TuiLiQiWorkTime_Compare_1 = 2.5;
        public static double AutoCtlHigh_TuiLiQiWorkTime_Compare_2 = 1.5;
        public static double AutoCtlHigh_TuiLiQiWorkTime_Compare_3 = 0.8;

        //钻杆深度基准参数
        public static double BasicParas_ZuanJin_ShenDu = 0;

        //灯与摄像机的显示名字
        public static string sName_Light_1 = "灯#1";
        public static string sName_Light_2 = "灯#2";
        public static string sName_Light_3 = "灯#3";
        public static string sName_Light_4 = "灯#4";
        public static string sName_Light_5 = "灯#5";
        public static string sName_Light_6 = "灯#6";
        public static string sName_Light_7 = "灯#7";
        public static string sName_Light_8 = "灯#8";

        public static string sName_Camera_1 = "摄像1";
        public static string sName_Camera_2 = "摄像2";
        public static string sName_Camera_3 = "摄像3";
        public static string sName_Camera_4 = "摄像4";
        public static string sName_Camera_5 = "摄像5";
        public static string sName_Camera_6 = "摄像6";
        public static string sName_Camera_7 = "摄像7";
        public static string sName_Camera_8 = "摄像8";

        //串口打开成功与否标志
        public static bool isSerialOpenedOK_WaterCtlBox = false;
        public static bool isSerialOpenedOK_ROVPower = false;
        public static bool isSerialOpenedOK_JuYuanJianCe_1 = false;
        public static bool isSerialOpenedOK_JuYuanJianCe_2 = false;


        //状态报警配置参数列表，根据配置文件“\configure\AlarmConfigure.xlsx”中的关键字“状态代号”，填充字典
        public static Dictionary<string, StatusUnitParas> dicStatusParasByStatusName = new Dictionary<string, StatusUnitParas>();


        //8功能阀箱直接操作，8个参数
        public static double FaXiang8_A01 = 0;
        public static double FaXiang8_A02 = 0;
        public static double FaXiang8_A03 = 0;
        public static double FaXiang8_A04 = 0;
        public static double FaXiang8_A05 = 0;
        public static double FaXiang8_A06 = 0;
        public static double FaXiang8_A07 = 0;
        public static double FaXiang8_A08 = 0;

        //水面控制盒控制，数据更新标志
        public static bool flag_WaterBoxCtl_Dir_Refresh = false;
        public static bool flag_WaterBoxCtl_High_Refresh = false;

    }
}
