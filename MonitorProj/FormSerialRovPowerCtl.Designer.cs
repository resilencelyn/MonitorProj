namespace MonitorProj
{
    partial class FormSerialRovPowerCtl
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
            this.textBox_ROVPower = new System.Windows.Forms.TextBox();
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
            this.label19.Location = new System.Drawing.Point(231, 0);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(102, 19);
            this.label19.TabIndex = 105;
            this.label19.Text = "ROV控制柜";
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
            this.groupBox_BoardD_Mon.Controls.Add(this.textBox_ROVPower);
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
            // textBox_ROVPower
            // 
            this.textBox_ROVPower.Font = new System.Drawing.Font("宋体", 13.8F);
            this.textBox_ROVPower.Location = new System.Drawing.Point(22, 26);
            this.textBox_ROVPower.Multiline = true;
            this.textBox_ROVPower.Name = "textBox_ROVPower";
            this.textBox_ROVPower.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox_ROVPower.Size = new System.Drawing.Size(623, 59);
            this.textBox_ROVPower.TabIndex = 138;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.ForeColor = System.Drawing.Color.Red;
            this.label6.Location = new System.Drawing.Point(258, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(102, 19);
            this.label6.TabIndex = 106;
            this.label6.Text = "ROV控制柜";
            // 
            // FormSerialRovPowerCtl
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
            this.Name = "FormSerialRovPowerCtl";
            this.Text = "FormSerialMonCtl";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormSerialMonCtl_FormClosed);
            this.Load += new System.EventHandler(this.FormSerialRovPowerCtl_Load);
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
        public System.Windows.Forms.Button btn_SerialClose;
        public System.Windows.Forms.Button btn_SerialOpen;
        public System.Windows.Forms.ComboBox comboBox_SerialPort;
        private System.Windows.Forms.TextBox textBox_ROVPower;
    }
}