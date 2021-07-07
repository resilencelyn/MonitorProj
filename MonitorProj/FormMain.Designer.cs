namespace MonitorProj
{
    partial class FormMain
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
            this.MenuStrip_Main = new System.Windows.Forms.MenuStrip();
            this.ToolStripMenuItem_User = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_UserLogin = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.ToolStripMenuItem_Quit = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Modes = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_ModeI = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_ModeII = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_ModeIII = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_SoftConfig = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_TuiJinQinParasSave = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.ToolStripMenuItem_AutoCtlDirParas = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.ToolStripMenuItem_AutoCtlHighParas = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_ScreenSplit = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_ScreenOtherShow = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_ScreenOtherShowCancel = new System.Windows.Forms.ToolStripMenuItem();
            this.帮助ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.StatusStrip_Main = new System.Windows.Forms.StatusStrip();
            this.ToolStrip_Main = new System.Windows.Forms.ToolStrip();
            this.TabControl_Debug = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.TabControl_Main = new System.Windows.Forms.TabControl();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.skinEngine1 = new Sunisoft.IrisSkin.SkinEngine();
            this.MenuStrip_Main.SuspendLayout();
            this.TabControl_Debug.SuspendLayout();
            this.TabControl_Main.SuspendLayout();
            this.SuspendLayout();
            // 
            // MenuStrip_Main
            // 
            this.MenuStrip_Main.Font = new System.Drawing.Font("Microsoft YaHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.MenuStrip_Main.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.MenuStrip_Main.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItem_User,
            this.ToolStripMenuItem_Modes,
            this.ToolStripMenuItem_SoftConfig,
            this.ToolStripMenuItem_ScreenSplit,
            this.帮助ToolStripMenuItem});
            this.MenuStrip_Main.Location = new System.Drawing.Point(0, 0);
            this.MenuStrip_Main.Name = "MenuStrip_Main";
            this.MenuStrip_Main.Padding = new System.Windows.Forms.Padding(4, 2, 0, 2);
            this.MenuStrip_Main.Size = new System.Drawing.Size(885, 33);
            this.MenuStrip_Main.TabIndex = 1;
            this.MenuStrip_Main.Text = "menuStrip1";
            // 
            // ToolStripMenuItem_User
            // 
            this.ToolStripMenuItem_User.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItem_UserLogin,
            this.toolStripSeparator1,
            this.ToolStripMenuItem_Quit});
            this.ToolStripMenuItem_User.Name = "ToolStripMenuItem_User";
            this.ToolStripMenuItem_User.Size = new System.Drawing.Size(62, 29);
            this.ToolStripMenuItem_User.Text = "用户";
            // 
            // ToolStripMenuItem_UserLogin
            // 
            this.ToolStripMenuItem_UserLogin.Name = "ToolStripMenuItem_UserLogin";
            this.ToolStripMenuItem_UserLogin.Size = new System.Drawing.Size(122, 30);
            this.ToolStripMenuItem_UserLogin.Text = "登录";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(119, 6);
            // 
            // ToolStripMenuItem_Quit
            // 
            this.ToolStripMenuItem_Quit.Name = "ToolStripMenuItem_Quit";
            this.ToolStripMenuItem_Quit.Size = new System.Drawing.Size(122, 30);
            this.ToolStripMenuItem_Quit.Text = "退出";
            // 
            // ToolStripMenuItem_Modes
            // 
            this.ToolStripMenuItem_Modes.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItem_ModeI,
            this.ToolStripMenuItem_ModeII,
            this.ToolStripMenuItem_ModeIII});
            this.ToolStripMenuItem_Modes.Name = "ToolStripMenuItem_Modes";
            this.ToolStripMenuItem_Modes.Size = new System.Drawing.Size(100, 29);
            this.ToolStripMenuItem_Modes.Text = "系统模式";
            // 
            // ToolStripMenuItem_ModeI
            // 
            this.ToolStripMenuItem_ModeI.CheckOnClick = true;
            this.ToolStripMenuItem_ModeI.Name = "ToolStripMenuItem_ModeI";
            this.ToolStripMenuItem_ModeI.Size = new System.Drawing.Size(140, 30);
            this.ToolStripMenuItem_ModeI.Text = "模式I";
            this.ToolStripMenuItem_ModeI.Click += new System.EventHandler(this.ToolStripMenuItem_ModeI_Click);
            // 
            // ToolStripMenuItem_ModeII
            // 
            this.ToolStripMenuItem_ModeII.CheckOnClick = true;
            this.ToolStripMenuItem_ModeII.Name = "ToolStripMenuItem_ModeII";
            this.ToolStripMenuItem_ModeII.Size = new System.Drawing.Size(140, 30);
            this.ToolStripMenuItem_ModeII.Text = "模式II";
            this.ToolStripMenuItem_ModeII.Click += new System.EventHandler(this.ToolStripMenuItem_ModeII_Click);
            // 
            // ToolStripMenuItem_ModeIII
            // 
            this.ToolStripMenuItem_ModeIII.CheckOnClick = true;
            this.ToolStripMenuItem_ModeIII.Name = "ToolStripMenuItem_ModeIII";
            this.ToolStripMenuItem_ModeIII.Size = new System.Drawing.Size(140, 30);
            this.ToolStripMenuItem_ModeIII.Text = "模式III";
            this.ToolStripMenuItem_ModeIII.Click += new System.EventHandler(this.ToolStripMenuItem_ModeIII_Click);
            // 
            // ToolStripMenuItem_SoftConfig
            // 
            this.ToolStripMenuItem_SoftConfig.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItem_TuiJinQinParasSave,
            this.toolStripSeparator2,
            this.ToolStripMenuItem_AutoCtlDirParas,
            this.toolStripSeparator3,
            this.ToolStripMenuItem_AutoCtlHighParas});
            this.ToolStripMenuItem_SoftConfig.Name = "ToolStripMenuItem_SoftConfig";
            this.ToolStripMenuItem_SoftConfig.Size = new System.Drawing.Size(100, 29);
            this.ToolStripMenuItem_SoftConfig.Text = "软件配置";
            // 
            // ToolStripMenuItem_TuiJinQinParasSave
            // 
            this.ToolStripMenuItem_TuiJinQinParasSave.Name = "ToolStripMenuItem_TuiJinQinParasSave";
            this.ToolStripMenuItem_TuiJinQinParasSave.Size = new System.Drawing.Size(236, 30);
            this.ToolStripMenuItem_TuiJinQinParasSave.Text = "推进参数保存";
            this.ToolStripMenuItem_TuiJinQinParasSave.Click += new System.EventHandler(this.ToolStripMenuItem_TuiJinQinParasSave_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(233, 6);
            // 
            // ToolStripMenuItem_AutoCtlDirParas
            // 
            this.ToolStripMenuItem_AutoCtlDirParas.Name = "ToolStripMenuItem_AutoCtlDirParas";
            this.ToolStripMenuItem_AutoCtlDirParas.Size = new System.Drawing.Size(236, 30);
            this.ToolStripMenuItem_AutoCtlDirParas.Text = "自动定向参数配置";
            this.ToolStripMenuItem_AutoCtlDirParas.Click += new System.EventHandler(this.ToolStripMenuItem_AutoCtlDirParas_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(233, 6);
            // 
            // ToolStripMenuItem_AutoCtlHighParas
            // 
            this.ToolStripMenuItem_AutoCtlHighParas.Name = "ToolStripMenuItem_AutoCtlHighParas";
            this.ToolStripMenuItem_AutoCtlHighParas.Size = new System.Drawing.Size(236, 30);
            this.ToolStripMenuItem_AutoCtlHighParas.Text = "自动定高参数配置";
            this.ToolStripMenuItem_AutoCtlHighParas.Click += new System.EventHandler(this.ToolStripMenuItem_AutoCtlHighParas_Click);
            // 
            // ToolStripMenuItem_ScreenSplit
            // 
            this.ToolStripMenuItem_ScreenSplit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItem_ScreenOtherShow,
            this.ToolStripMenuItem_ScreenOtherShowCancel});
            this.ToolStripMenuItem_ScreenSplit.Name = "ToolStripMenuItem_ScreenSplit";
            this.ToolStripMenuItem_ScreenSplit.Size = new System.Drawing.Size(62, 29);
            this.ToolStripMenuItem_ScreenSplit.Text = "显示";
            // 
            // ToolStripMenuItem_ScreenOtherShow
            // 
            this.ToolStripMenuItem_ScreenOtherShow.Name = "ToolStripMenuItem_ScreenOtherShow";
            this.ToolStripMenuItem_ScreenOtherShow.Size = new System.Drawing.Size(160, 30);
            this.ToolStripMenuItem_ScreenOtherShow.Text = "分屏";
            this.ToolStripMenuItem_ScreenOtherShow.Click += new System.EventHandler(this.ToolStripMenuItem_ScreenOtherShow_Click);
            // 
            // ToolStripMenuItem_ScreenOtherShowCancel
            // 
            this.ToolStripMenuItem_ScreenOtherShowCancel.Name = "ToolStripMenuItem_ScreenOtherShowCancel";
            this.ToolStripMenuItem_ScreenOtherShowCancel.Size = new System.Drawing.Size(160, 30);
            this.ToolStripMenuItem_ScreenOtherShowCancel.Text = "取消分屏";
            this.ToolStripMenuItem_ScreenOtherShowCancel.Click += new System.EventHandler(this.ToolStripMenuItem_ScreenOtherShowCancel_Click);
            // 
            // 帮助ToolStripMenuItem
            // 
            this.帮助ToolStripMenuItem.Name = "帮助ToolStripMenuItem";
            this.帮助ToolStripMenuItem.Size = new System.Drawing.Size(62, 29);
            this.帮助ToolStripMenuItem.Text = "帮助";
            // 
            // StatusStrip_Main
            // 
            this.StatusStrip_Main.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.StatusStrip_Main.Location = new System.Drawing.Point(0, 569);
            this.StatusStrip_Main.Name = "StatusStrip_Main";
            this.StatusStrip_Main.Padding = new System.Windows.Forms.Padding(1, 0, 10, 0);
            this.StatusStrip_Main.Size = new System.Drawing.Size(885, 22);
            this.StatusStrip_Main.TabIndex = 2;
            this.StatusStrip_Main.Text = "statusStrip1";
            // 
            // ToolStrip_Main
            // 
            this.ToolStrip_Main.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.ToolStrip_Main.Location = new System.Drawing.Point(0, 22);
            this.ToolStrip_Main.Name = "ToolStrip_Main";
            this.ToolStrip_Main.Size = new System.Drawing.Size(587, 18);
            this.ToolStrip_Main.TabIndex = 3;
            this.ToolStrip_Main.Text = "toolStrip1";
            this.ToolStrip_Main.Visible = false;
            // 
            // TabControl_Debug
            // 
            this.TabControl_Debug.Controls.Add(this.tabPage1);
            this.TabControl_Debug.Controls.Add(this.tabPage2);
            this.TabControl_Debug.Location = new System.Drawing.Point(11, 100);
            this.TabControl_Debug.Margin = new System.Windows.Forms.Padding(2);
            this.TabControl_Debug.Name = "TabControl_Debug";
            this.TabControl_Debug.SelectedIndex = 0;
            this.TabControl_Debug.Size = new System.Drawing.Size(885, 542);
            this.TabControl_Debug.TabIndex = 5;
            // 
            // tabPage1
            // 
            this.tabPage1.Location = new System.Drawing.Point(4, 29);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(2);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(2);
            this.tabPage1.Size = new System.Drawing.Size(877, 509);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "控制与执行";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 29);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(2);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(2);
            this.tabPage2.Size = new System.Drawing.Size(877, 509);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "状态与监视";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // TabControl_Main
            // 
            this.TabControl_Main.Controls.Add(this.tabPage3);
            this.TabControl_Main.Controls.Add(this.tabPage4);
            this.TabControl_Main.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TabControl_Main.Location = new System.Drawing.Point(0, 33);
            this.TabControl_Main.Margin = new System.Windows.Forms.Padding(2);
            this.TabControl_Main.Name = "TabControl_Main";
            this.TabControl_Main.SelectedIndex = 0;
            this.TabControl_Main.Size = new System.Drawing.Size(885, 536);
            this.TabControl_Main.TabIndex = 7;
            // 
            // tabPage3
            // 
            this.tabPage3.Location = new System.Drawing.Point(4, 29);
            this.tabPage3.Margin = new System.Windows.Forms.Padding(2);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(2);
            this.tabPage3.Size = new System.Drawing.Size(877, 503);
            this.tabPage3.TabIndex = 0;
            this.tabPage3.Text = "控制与执行";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // tabPage4
            // 
            this.tabPage4.Location = new System.Drawing.Point(4, 29);
            this.tabPage4.Margin = new System.Windows.Forms.Padding(2);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(2);
            this.tabPage4.Size = new System.Drawing.Size(877, 503);
            this.tabPage4.TabIndex = 1;
            this.tabPage4.Text = "状态与监视";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // skinEngine1
            // 
            this.skinEngine1.@__DrawButtonFocusRectangle = true;
            this.skinEngine1.DisabledButtonTextColor = System.Drawing.Color.Gray;
            this.skinEngine1.DisabledMenuFontColor = System.Drawing.SystemColors.GrayText;
            this.skinEngine1.InactiveCaptionColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.skinEngine1.SerialNumber = "";
            this.skinEngine1.SkinFile = null;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(885, 591);
            this.Controls.Add(this.TabControl_Main);
            this.Controls.Add(this.TabControl_Debug);
            this.Controls.Add(this.ToolStrip_Main);
            this.Controls.Add(this.StatusStrip_Main);
            this.Controls.Add(this.MenuStrip_Main);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.MenuStrip_Main;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "FormMain";
            this.Text = "监控主界面";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormMain_FormClosed);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.MenuStrip_Main.ResumeLayout(false);
            this.MenuStrip_Main.PerformLayout();
            this.TabControl_Debug.ResumeLayout(false);
            this.TabControl_Main.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip MenuStrip_Main;
        private System.Windows.Forms.StatusStrip StatusStrip_Main;
        private System.Windows.Forms.ToolStrip ToolStrip_Main;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_User;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_UserLogin;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Quit;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Modes;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_ModeI;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_ModeII;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_ModeIII;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_SoftConfig;
        private System.Windows.Forms.ToolStripMenuItem 帮助ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_ScreenSplit;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_ScreenOtherShow;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_ScreenOtherShowCancel;
        private System.Windows.Forms.TabControl TabControl_Main;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.TabPage tabPage3;
        public System.Windows.Forms.TabPage tabPage1;
        public System.Windows.Forms.TabPage tabPage2;
        public System.Windows.Forms.TabControl TabControl_Debug;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_TuiJinQinParasSave;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_AutoCtlDirParas;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_AutoCtlHighParas;
        private Sunisoft.IrisSkin.SkinEngine skinEngine1;
    }
}