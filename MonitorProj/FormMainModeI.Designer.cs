namespace MonitorProj
{
    partial class FormMainModeI
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("8功能阀箱", 0, 0);
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("16功能阀箱", 1, 1);
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("舱内控制");
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("水面控制盒");
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("串口原始", 3, 3);
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("控制面板", 5, 5, new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode3,
            treeNode4,
            treeNode5});
            System.Windows.Forms.TreeNode treeNode7 = new System.Windows.Forms.TreeNode("运行记录", 1, 1);
            System.Windows.Forms.TreeNode treeNode8 = new System.Windows.Forms.TreeNode("运行记录", 4, 4, new System.Windows.Forms.TreeNode[] {
            treeNode7});
            System.Windows.Forms.TreeNode treeNode9 = new System.Windows.Forms.TreeNode("退出", 1, 1);
            System.Windows.Forms.TreeNode treeNode10 = new System.Windows.Forms.TreeNode("系统控制", 2, 2, new System.Windows.Forms.TreeNode[] {
            treeNode9});
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.treeView_MonCtlAll = new System.Windows.Forms.TreeView();
            this.panel_FormContainer = new System.Windows.Forms.Panel();
            this.Panel_AlarmAndRunningInfo = new System.Windows.Forms.Panel();
            this.groupBox_SoftRuningInfo = new System.Windows.Forms.GroupBox();
            this.textBox_Communication = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.richTextBox_InfoShow = new System.Windows.Forms.RichTextBox();
            this.DataGridView_SysAlarmInfo = new System.Windows.Forms.DataGridView();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem_DeleteSelected = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem_ClearAll = new System.Windows.Forms.ToolStripMenuItem();
            this.timer_Communication = new System.Windows.Forms.Timer(this.components);
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.Panel_AlarmAndRunningInfo.SuspendLayout();
            this.groupBox_SoftRuningInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DataGridView_SysAlarmInfo)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(2);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.treeView_MonCtlAll);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.AutoScroll = true;
            this.splitContainer1.Panel2.BackColor = System.Drawing.Color.DarkSeaGreen;
            this.splitContainer1.Panel2.Controls.Add(this.panel_FormContainer);
            this.splitContainer1.Panel2.Controls.Add(this.Panel_AlarmAndRunningInfo);
            this.splitContainer1.Size = new System.Drawing.Size(1182, 978);
            this.splitContainer1.SplitterDistance = 107;
            this.splitContainer1.TabIndex = 0;
            // 
            // treeView_MonCtlAll
            // 
            this.treeView_MonCtlAll.BackColor = System.Drawing.Color.DarkSeaGreen;
            this.treeView_MonCtlAll.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView_MonCtlAll.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.treeView_MonCtlAll.Indent = 19;
            this.treeView_MonCtlAll.ItemHeight = 28;
            this.treeView_MonCtlAll.Location = new System.Drawing.Point(0, 0);
            this.treeView_MonCtlAll.Margin = new System.Windows.Forms.Padding(4);
            this.treeView_MonCtlAll.Name = "treeView_MonCtlAll";
            treeNode1.ImageIndex = 0;
            treeNode1.Name = "BoardA";
            treeNode1.SelectedImageIndex = 0;
            treeNode1.Text = "8功能阀箱";
            treeNode2.ImageIndex = 1;
            treeNode2.Name = "BoardB";
            treeNode2.SelectedImageIndex = 1;
            treeNode2.Text = "16功能阀箱";
            treeNode3.Name = "BoardC";
            treeNode3.Text = "舱内控制";
            treeNode4.Name = "BoardD";
            treeNode4.Text = "水面控制盒";
            treeNode5.ImageIndex = 3;
            treeNode5.Name = "ComDataShow";
            treeNode5.SelectedImageIndex = 3;
            treeNode5.Text = "串口原始";
            treeNode6.ImageIndex = 5;
            treeNode6.Name = "MonCtlRootNode";
            treeNode6.SelectedImageIndex = 5;
            treeNode6.Text = "控制面板";
            treeNode7.ImageIndex = 1;
            treeNode7.Name = "LogNode";
            treeNode7.SelectedImageIndex = 1;
            treeNode7.Text = "运行记录";
            treeNode8.ImageIndex = 4;
            treeNode8.Name = "LogRootNode";
            treeNode8.SelectedImageIndex = 4;
            treeNode8.Text = "运行记录";
            treeNode9.ImageIndex = 1;
            treeNode9.Name = "QuitNode";
            treeNode9.SelectedImageIndex = 1;
            treeNode9.Text = "退出";
            treeNode10.ImageIndex = 2;
            treeNode10.Name = "SysCtlRootNode";
            treeNode10.SelectedImageIndex = 2;
            treeNode10.Text = "系统控制";
            this.treeView_MonCtlAll.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode6,
            treeNode8,
            treeNode10});
            this.treeView_MonCtlAll.ShowNodeToolTips = true;
            this.treeView_MonCtlAll.Size = new System.Drawing.Size(103, 974);
            this.treeView_MonCtlAll.TabIndex = 2;
            this.treeView_MonCtlAll.MouseClick += new System.Windows.Forms.MouseEventHandler(this.treeView_MonCtlAll_MouseClick);
            // 
            // panel_FormContainer
            // 
            this.panel_FormContainer.AutoScroll = true;
            this.panel_FormContainer.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel_FormContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_FormContainer.Location = new System.Drawing.Point(0, 0);
            this.panel_FormContainer.Margin = new System.Windows.Forms.Padding(2);
            this.panel_FormContainer.Name = "panel_FormContainer";
            this.panel_FormContainer.Size = new System.Drawing.Size(713, 974);
            this.panel_FormContainer.TabIndex = 30;
            // 
            // Panel_AlarmAndRunningInfo
            // 
            this.Panel_AlarmAndRunningInfo.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Panel_AlarmAndRunningInfo.Controls.Add(this.groupBox_SoftRuningInfo);
            this.Panel_AlarmAndRunningInfo.Controls.Add(this.DataGridView_SysAlarmInfo);
            this.Panel_AlarmAndRunningInfo.Dock = System.Windows.Forms.DockStyle.Right;
            this.Panel_AlarmAndRunningInfo.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Panel_AlarmAndRunningInfo.Location = new System.Drawing.Point(713, 0);
            this.Panel_AlarmAndRunningInfo.Margin = new System.Windows.Forms.Padding(2);
            this.Panel_AlarmAndRunningInfo.Name = "Panel_AlarmAndRunningInfo";
            this.Panel_AlarmAndRunningInfo.Size = new System.Drawing.Size(354, 974);
            this.Panel_AlarmAndRunningInfo.TabIndex = 31;
            // 
            // groupBox_SoftRuningInfo
            // 
            this.groupBox_SoftRuningInfo.Controls.Add(this.textBox_Communication);
            this.groupBox_SoftRuningInfo.Controls.Add(this.label1);
            this.groupBox_SoftRuningInfo.Controls.Add(this.richTextBox_InfoShow);
            this.groupBox_SoftRuningInfo.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBox_SoftRuningInfo.Location = new System.Drawing.Point(0, 502);
            this.groupBox_SoftRuningInfo.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox_SoftRuningInfo.Name = "groupBox_SoftRuningInfo";
            this.groupBox_SoftRuningInfo.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox_SoftRuningInfo.Size = new System.Drawing.Size(350, 468);
            this.groupBox_SoftRuningInfo.TabIndex = 32;
            this.groupBox_SoftRuningInfo.TabStop = false;
            // 
            // textBox_Communication
            // 
            this.textBox_Communication.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox_Communication.Location = new System.Drawing.Point(232, -1);
            this.textBox_Communication.Name = "textBox_Communication";
            this.textBox_Communication.ReadOnly = true;
            this.textBox_Communication.Size = new System.Drawing.Size(27, 27);
            this.textBox_Communication.TabIndex = 128;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(121, 0);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(106, 24);
            this.label1.TabIndex = 0;
            this.label1.Text = "运行状态";
            // 
            // richTextBox_InfoShow
            // 
            this.richTextBox_InfoShow.BackColor = System.Drawing.Color.DarkSeaGreen;
            this.richTextBox_InfoShow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox_InfoShow.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.richTextBox_InfoShow.Location = new System.Drawing.Point(2, 30);
            this.richTextBox_InfoShow.Margin = new System.Windows.Forms.Padding(2);
            this.richTextBox_InfoShow.Name = "richTextBox_InfoShow";
            this.richTextBox_InfoShow.Size = new System.Drawing.Size(346, 436);
            this.richTextBox_InfoShow.TabIndex = 0;
            this.richTextBox_InfoShow.Text = "";
            // 
            // DataGridView_SysAlarmInfo
            // 
            this.DataGridView_SysAlarmInfo.AllowUserToAddRows = false;
            this.DataGridView_SysAlarmInfo.BackgroundColor = System.Drawing.Color.DarkSeaGreen;
            this.DataGridView_SysAlarmInfo.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.DataGridView_SysAlarmInfo.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DataGridView_SysAlarmInfo.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1});
            this.DataGridView_SysAlarmInfo.ContextMenuStrip = this.contextMenuStrip1;
            this.DataGridView_SysAlarmInfo.Dock = System.Windows.Forms.DockStyle.Top;
            this.DataGridView_SysAlarmInfo.Location = new System.Drawing.Point(0, 0);
            this.DataGridView_SysAlarmInfo.Margin = new System.Windows.Forms.Padding(2);
            this.DataGridView_SysAlarmInfo.Name = "DataGridView_SysAlarmInfo";
            this.DataGridView_SysAlarmInfo.ReadOnly = true;
            this.DataGridView_SysAlarmInfo.RightToLeft = System.Windows.Forms.RightToLeft.No;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.DataGridView_SysAlarmInfo.RowHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.DataGridView_SysAlarmInfo.RowHeadersVisible = false;
            this.DataGridView_SysAlarmInfo.RowTemplate.Height = 27;
            this.DataGridView_SysAlarmInfo.Size = new System.Drawing.Size(350, 495);
            this.DataGridView_SysAlarmInfo.TabIndex = 31;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_DeleteSelected,
            this.toolStripSeparator1,
            this.toolStripMenuItem_ClearAll});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(199, 58);
            // 
            // toolStripMenuItem_DeleteSelected
            // 
            this.toolStripMenuItem_DeleteSelected.Name = "toolStripMenuItem_DeleteSelected";
            this.toolStripMenuItem_DeleteSelected.Size = new System.Drawing.Size(198, 24);
            this.toolStripMenuItem_DeleteSelected.Text = "清除当前报警信息";
            this.toolStripMenuItem_DeleteSelected.Click += new System.EventHandler(this.toolStripMenuItem_DeleteSelected_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(195, 6);
            // 
            // toolStripMenuItem_ClearAll
            // 
            this.toolStripMenuItem_ClearAll.Name = "toolStripMenuItem_ClearAll";
            this.toolStripMenuItem_ClearAll.Size = new System.Drawing.Size(198, 24);
            this.toolStripMenuItem_ClearAll.Text = "清除所有报警信息";
            this.toolStripMenuItem_ClearAll.Click += new System.EventHandler(this.toolStripMenuItem_ClearAll_Click);
            // 
            // timer_Communication
            // 
            this.timer_Communication.Enabled = true;
            this.timer_Communication.Interval = 1000;
            this.timer_Communication.Tick += new System.EventHandler(this.timer_Communication_Tick);
            // 
            // Column1
            // 
            this.Column1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.DarkSeaGreen;
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
            this.Column1.DefaultCellStyle = dataGridViewCellStyle1;
            this.Column1.HeaderText = "报警信息";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            // 
            // FormMainModeI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1182, 978);
            this.Controls.Add(this.splitContainer1);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "FormMainModeI";
            this.Text = "监控主界面";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMainModeI_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormMain_FormClosed);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.Panel_AlarmAndRunningInfo.ResumeLayout(false);
            this.groupBox_SoftRuningInfo.ResumeLayout(false);
            this.groupBox_SoftRuningInfo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DataGridView_SysAlarmInfo)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView treeView_MonCtlAll;
        private System.Windows.Forms.Panel Panel_AlarmAndRunningInfo;
        private System.Windows.Forms.DataGridView DataGridView_SysAlarmInfo;
        private System.Windows.Forms.GroupBox groupBox_SoftRuningInfo;
        private System.Windows.Forms.Panel panel_FormContainer;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_DeleteSelected;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_ClearAll;
        private System.Windows.Forms.TextBox textBox_Communication;
        private System.Windows.Forms.Timer timer_Communication;
        public System.Windows.Forms.RichTextBox richTextBox_InfoShow;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;

    }
}

