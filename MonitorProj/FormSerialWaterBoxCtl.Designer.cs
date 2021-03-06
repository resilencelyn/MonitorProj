namespace MonitorProj
{
    partial class FormSerialWaterBoxCtl
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox_BoardD_Ctl = new System.Windows.Forms.GroupBox();
            this.label19 = new System.Windows.Forms.Label();
            this.btn_SerialClose = new System.Windows.Forms.Button();
            this.btn_SerialOpen = new System.Windows.Forms.Button();
            this.comboBox_Parity = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.comboBox_StopBit = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.comboBox_DataBitsCount = new System.Windows.Forms.ComboBox();
            this.comboBox_Band = new System.Windows.Forms.ComboBox();
            this.comboBox_SerialPort = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox_BoardD_Mon = new System.Windows.Forms.GroupBox();
            this.textBox_KKInfo = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.textBox_Space2 = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.textBox_Space1 = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.textBox_RotAxisV = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.textBox_RotAxisZ = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.textBox_RotAxisY = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.textBox_RotAxisX = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox_BoardD_Ctl.SuspendLayout();
            this.groupBox_BoardD_Mon.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox_BoardD_Ctl
            // 
            this.groupBox_BoardD_Ctl.Controls.Add(this.label19);
            this.groupBox_BoardD_Ctl.Controls.Add(this.btn_SerialClose);
            this.groupBox_BoardD_Ctl.Controls.Add(this.btn_SerialOpen);
            this.groupBox_BoardD_Ctl.Controls.Add(this.comboBox_Parity);
            this.groupBox_BoardD_Ctl.Controls.Add(this.label5);
            this.groupBox_BoardD_Ctl.Controls.Add(this.comboBox_StopBit);
            this.groupBox_BoardD_Ctl.Controls.Add(this.label4);
            this.groupBox_BoardD_Ctl.Controls.Add(this.comboBox_DataBitsCount);
            this.groupBox_BoardD_Ctl.Controls.Add(this.comboBox_Band);
            this.groupBox_BoardD_Ctl.Controls.Add(this.comboBox_SerialPort);
            this.groupBox_BoardD_Ctl.Controls.Add(this.label3);
            this.groupBox_BoardD_Ctl.Controls.Add(this.label1);
            this.groupBox_BoardD_Ctl.Controls.Add(this.label2);
            this.groupBox_BoardD_Ctl.Font = new System.Drawing.Font("宋体", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox_BoardD_Ctl.Location = new System.Drawing.Point(9, 10);
            this.groupBox_BoardD_Ctl.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox_BoardD_Ctl.Name = "groupBox_BoardD_Ctl";
            this.groupBox_BoardD_Ctl.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox_BoardD_Ctl.Size = new System.Drawing.Size(570, 135);
            this.groupBox_BoardD_Ctl.TabIndex = 27;
            this.groupBox_BoardD_Ctl.TabStop = false;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label19.ForeColor = System.Drawing.Color.Red;
            this.label19.Location = new System.Drawing.Point(226, -2);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(69, 19);
            this.label19.TabIndex = 105;
            this.label19.Text = "控制盒";
            // 
            // btn_SerialClose
            // 
            this.btn_SerialClose.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_SerialClose.Location = new System.Drawing.Point(419, 76);
            this.btn_SerialClose.Margin = new System.Windows.Forms.Padding(2);
            this.btn_SerialClose.Name = "btn_SerialClose";
            this.btn_SerialClose.Size = new System.Drawing.Size(110, 35);
            this.btn_SerialClose.TabIndex = 34;
            this.btn_SerialClose.Text = "串口关闭";
            this.btn_SerialClose.UseVisualStyleBackColor = true;
            this.btn_SerialClose.Click += new System.EventHandler(this.btn_SerialClose_Click);
            // 
            // btn_SerialOpen
            // 
            this.btn_SerialOpen.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_SerialOpen.Location = new System.Drawing.Point(419, 25);
            this.btn_SerialOpen.Margin = new System.Windows.Forms.Padding(2);
            this.btn_SerialOpen.Name = "btn_SerialOpen";
            this.btn_SerialOpen.Size = new System.Drawing.Size(110, 35);
            this.btn_SerialOpen.TabIndex = 33;
            this.btn_SerialOpen.Text = "串口打开";
            this.btn_SerialOpen.UseVisualStyleBackColor = true;
            this.btn_SerialOpen.Click += new System.EventHandler(this.btn_SerialOpen_Click);
            // 
            // comboBox_Parity
            // 
            this.comboBox_Parity.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.comboBox_Parity.FormattingEnabled = true;
            this.comboBox_Parity.Location = new System.Drawing.Point(301, 64);
            this.comboBox_Parity.Margin = new System.Windows.Forms.Padding(2);
            this.comboBox_Parity.Name = "comboBox_Parity";
            this.comboBox_Parity.Size = new System.Drawing.Size(92, 27);
            this.comboBox_Parity.TabIndex = 32;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.Location = new System.Drawing.Point(226, 67);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(85, 19);
            this.label5.TabIndex = 31;
            this.label5.Text = "校验位：";
            // 
            // comboBox_StopBit
            // 
            this.comboBox_StopBit.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.comboBox_StopBit.FormattingEnabled = true;
            this.comboBox_StopBit.Location = new System.Drawing.Point(113, 64);
            this.comboBox_StopBit.Margin = new System.Windows.Forms.Padding(2);
            this.comboBox_StopBit.Name = "comboBox_StopBit";
            this.comboBox_StopBit.Size = new System.Drawing.Size(92, 27);
            this.comboBox_StopBit.TabIndex = 30;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(37, 67);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(85, 19);
            this.label4.TabIndex = 29;
            this.label4.Text = "终止位：";
            // 
            // comboBox_DataBitsCount
            // 
            this.comboBox_DataBitsCount.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.comboBox_DataBitsCount.FormattingEnabled = true;
            this.comboBox_DataBitsCount.Location = new System.Drawing.Point(113, 98);
            this.comboBox_DataBitsCount.Margin = new System.Windows.Forms.Padding(2);
            this.comboBox_DataBitsCount.Name = "comboBox_DataBitsCount";
            this.comboBox_DataBitsCount.Size = new System.Drawing.Size(92, 27);
            this.comboBox_DataBitsCount.TabIndex = 28;
            // 
            // comboBox_Band
            // 
            this.comboBox_Band.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.comboBox_Band.FormattingEnabled = true;
            this.comboBox_Band.Location = new System.Drawing.Point(301, 30);
            this.comboBox_Band.Margin = new System.Windows.Forms.Padding(2);
            this.comboBox_Band.Name = "comboBox_Band";
            this.comboBox_Band.Size = new System.Drawing.Size(92, 27);
            this.comboBox_Band.TabIndex = 26;
            // 
            // comboBox_SerialPort
            // 
            this.comboBox_SerialPort.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.comboBox_SerialPort.FormattingEnabled = true;
            this.comboBox_SerialPort.Location = new System.Drawing.Point(113, 30);
            this.comboBox_SerialPort.Margin = new System.Windows.Forms.Padding(2);
            this.comboBox_SerialPort.Name = "comboBox_SerialPort";
            this.comboBox_SerialPort.Size = new System.Drawing.Size(92, 27);
            this.comboBox_SerialPort.TabIndex = 24;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(18, 101);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(104, 19);
            this.label3.TabIndex = 27;
            this.label3.Text = "数据位数：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(18, 33);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(104, 19);
            this.label1.TabIndex = 23;
            this.label1.Text = "串口选择：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(226, 33);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(85, 19);
            this.label2.TabIndex = 25;
            this.label2.Text = "波特率：";
            // 
            // groupBox_BoardD_Mon
            // 
            this.groupBox_BoardD_Mon.Controls.Add(this.textBox_KKInfo);
            this.groupBox_BoardD_Mon.Controls.Add(this.label13);
            this.groupBox_BoardD_Mon.Controls.Add(this.textBox_Space2);
            this.groupBox_BoardD_Mon.Controls.Add(this.label11);
            this.groupBox_BoardD_Mon.Controls.Add(this.textBox_Space1);
            this.groupBox_BoardD_Mon.Controls.Add(this.label12);
            this.groupBox_BoardD_Mon.Controls.Add(this.textBox_RotAxisV);
            this.groupBox_BoardD_Mon.Controls.Add(this.label10);
            this.groupBox_BoardD_Mon.Controls.Add(this.textBox_RotAxisZ);
            this.groupBox_BoardD_Mon.Controls.Add(this.label9);
            this.groupBox_BoardD_Mon.Controls.Add(this.textBox_RotAxisY);
            this.groupBox_BoardD_Mon.Controls.Add(this.label8);
            this.groupBox_BoardD_Mon.Controls.Add(this.textBox_RotAxisX);
            this.groupBox_BoardD_Mon.Controls.Add(this.label7);
            this.groupBox_BoardD_Mon.Controls.Add(this.label6);
            this.groupBox_BoardD_Mon.Font = new System.Drawing.Font("宋体", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox_BoardD_Mon.Location = new System.Drawing.Point(9, 150);
            this.groupBox_BoardD_Mon.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox_BoardD_Mon.Name = "groupBox_BoardD_Mon";
            this.groupBox_BoardD_Mon.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox_BoardD_Mon.Size = new System.Drawing.Size(670, 103);
            this.groupBox_BoardD_Mon.TabIndex = 28;
            this.groupBox_BoardD_Mon.TabStop = false;
            // 
            // textBox_KKInfo
            // 
            this.textBox_KKInfo.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox_KKInfo.Location = new System.Drawing.Point(552, 61);
            this.textBox_KKInfo.Margin = new System.Windows.Forms.Padding(2);
            this.textBox_KKInfo.Name = "textBox_KKInfo";
            this.textBox_KKInfo.ReadOnly = true;
            this.textBox_KKInfo.Size = new System.Drawing.Size(70, 29);
            this.textBox_KKInfo.TabIndex = 120;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label13.Location = new System.Drawing.Point(478, 66);
            this.label13.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(58, 19);
            this.label13.TabIndex = 119;
            this.label13.Text = "IO1：";
            // 
            // textBox_Space2
            // 
            this.textBox_Space2.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox_Space2.Location = new System.Drawing.Point(241, 61);
            this.textBox_Space2.Margin = new System.Windows.Forms.Padding(2);
            this.textBox_Space2.Name = "textBox_Space2";
            this.textBox_Space2.ReadOnly = true;
            this.textBox_Space2.Size = new System.Drawing.Size(70, 29);
            this.textBox_Space2.TabIndex = 118;
            this.textBox_Space2.Visible = false;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label11.Location = new System.Drawing.Point(163, 66);
            this.label11.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(57, 19);
            this.label11.TabIndex = 117;
            this.label11.Text = "备2：";
            this.label11.Visible = false;
            // 
            // textBox_Space1
            // 
            this.textBox_Space1.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox_Space1.Location = new System.Drawing.Point(87, 61);
            this.textBox_Space1.Margin = new System.Windows.Forms.Padding(2);
            this.textBox_Space1.Name = "textBox_Space1";
            this.textBox_Space1.ReadOnly = true;
            this.textBox_Space1.Size = new System.Drawing.Size(70, 29);
            this.textBox_Space1.TabIndex = 116;
            this.textBox_Space1.Visible = false;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label12.Location = new System.Drawing.Point(5, 66);
            this.label12.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(57, 19);
            this.label12.TabIndex = 115;
            this.label12.Text = "备1：";
            this.label12.Visible = false;
            // 
            // textBox_RotAxisV
            // 
            this.textBox_RotAxisV.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox_RotAxisV.Location = new System.Drawing.Point(552, 27);
            this.textBox_RotAxisV.Margin = new System.Windows.Forms.Padding(2);
            this.textBox_RotAxisV.Name = "textBox_RotAxisV";
            this.textBox_RotAxisV.ReadOnly = true;
            this.textBox_RotAxisV.Size = new System.Drawing.Size(70, 29);
            this.textBox_RotAxisV.TabIndex = 114;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label10.Location = new System.Drawing.Point(471, 31);
            this.label10.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(95, 19);
            this.label10.TabIndex = 113;
            this.label10.Text = "上/下移：";
            // 
            // textBox_RotAxisZ
            // 
            this.textBox_RotAxisZ.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox_RotAxisZ.Location = new System.Drawing.Point(398, 26);
            this.textBox_RotAxisZ.Margin = new System.Windows.Forms.Padding(2);
            this.textBox_RotAxisZ.Name = "textBox_RotAxisZ";
            this.textBox_RotAxisZ.ReadOnly = true;
            this.textBox_RotAxisZ.Size = new System.Drawing.Size(70, 29);
            this.textBox_RotAxisZ.TabIndex = 112;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label9.Location = new System.Drawing.Point(316, 31);
            this.label9.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(95, 19);
            this.label9.TabIndex = 111;
            this.label9.Text = "左/右转：";
            // 
            // textBox_RotAxisY
            // 
            this.textBox_RotAxisY.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox_RotAxisY.Location = new System.Drawing.Point(241, 27);
            this.textBox_RotAxisY.Margin = new System.Windows.Forms.Padding(2);
            this.textBox_RotAxisY.Name = "textBox_RotAxisY";
            this.textBox_RotAxisY.ReadOnly = true;
            this.textBox_RotAxisY.Size = new System.Drawing.Size(70, 29);
            this.textBox_RotAxisY.TabIndex = 110;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label8.Location = new System.Drawing.Point(159, 31);
            this.label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(95, 19);
            this.label8.TabIndex = 109;
            this.label8.Text = "前/后移：";
            // 
            // textBox_RotAxisX
            // 
            this.textBox_RotAxisX.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox_RotAxisX.Location = new System.Drawing.Point(87, 26);
            this.textBox_RotAxisX.Margin = new System.Windows.Forms.Padding(2);
            this.textBox_RotAxisX.Name = "textBox_RotAxisX";
            this.textBox_RotAxisX.ReadOnly = true;
            this.textBox_RotAxisX.Size = new System.Drawing.Size(70, 29);
            this.textBox_RotAxisX.TabIndex = 108;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label7.Location = new System.Drawing.Point(5, 31);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(95, 19);
            this.label7.TabIndex = 107;
            this.label7.Text = "左/右移：";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.ForeColor = System.Drawing.Color.Red;
            this.label6.Location = new System.Drawing.Point(258, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(69, 19);
            this.label6.TabIndex = 106;
            this.label6.Text = "控制盒";
            // 
            // FormSerialWaterBoxCtl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.DarkSeaGreen;
            this.ClientSize = new System.Drawing.Size(805, 490);
            this.Controls.Add(this.groupBox_BoardD_Mon);
            this.Controls.Add(this.groupBox_BoardD_Ctl);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "FormSerialWaterBoxCtl";
            this.Text = "FormSerialMonCtl";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormSerialMonCtl_FormClosed);
            this.Load += new System.EventHandler(this.FormSerialWaterBoxCtl_Load);
            this.groupBox_BoardD_Ctl.ResumeLayout(false);
            this.groupBox_BoardD_Ctl.PerformLayout();
            this.groupBox_BoardD_Mon.ResumeLayout(false);
            this.groupBox_BoardD_Mon.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox_BoardD_Ctl;
        private System.Windows.Forms.ComboBox comboBox_Parity;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox comboBox_StopBit;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox comboBox_DataBitsCount;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboBox_Band;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.GroupBox groupBox_BoardD_Mon;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBox_KKInfo;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox textBox_Space2;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox textBox_Space1;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox textBox_RotAxisV;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox textBox_RotAxisZ;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox textBox_RotAxisY;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBox_RotAxisX;
        private System.Windows.Forms.Label label7;
        public System.Windows.Forms.Button btn_SerialClose;
        public System.Windows.Forms.Button btn_SerialOpen;
        public System.Windows.Forms.ComboBox comboBox_SerialPort;
    }
}