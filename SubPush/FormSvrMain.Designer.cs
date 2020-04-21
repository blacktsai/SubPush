namespace SubPush
{
    partial class FormSvrMain
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器
        /// 修改這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSvrMain));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsbClose = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbStartSvr = new System.Windows.Forms.ToolStripButton();
            this.tsbQuerySub = new System.Windows.Forms.ToolStripButton();
            this.tsbSubPush = new System.Windows.Forms.ToolStripButton();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslSvrStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBoxListenPort = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.dgvClientInfo = new System.Windows.Forms.DataGridView();
            this.ColTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColIP = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label1 = new System.Windows.Forms.Label();
            this.dgvSubStockList = new System.Windows.Forms.DataGridView();
            this.ColSymbol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColSymbolName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColTempRefer = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColRefer = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColLastDeal = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColDiff = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColMarket = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColStartDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColStopDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColDrawDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColCouponDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColRefundDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label2 = new System.Windows.Forms.Label();
            this.timerMain = new System.Windows.Forms.Timer(this.components);
            this.toolStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvClientInfo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSubStockList)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbClose,
            this.toolStripSeparator1,
            this.tsbStartSvr,
            this.tsbQuerySub,
            this.tsbSubPush});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(900, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tsbClose
            // 
            this.tsbClose.Image = ((System.Drawing.Image)(resources.GetObject("tsbClose.Image")));
            this.tsbClose.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbClose.Name = "tsbClose";
            this.tsbClose.Size = new System.Drawing.Size(52, 22);
            this.tsbClose.Text = "關閉";
            this.tsbClose.Click += new System.EventHandler(this.tsbClose_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // tsbStartSvr
            // 
            this.tsbStartSvr.Image = ((System.Drawing.Image)(resources.GetObject("tsbStartSvr.Image")));
            this.tsbStartSvr.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbStartSvr.Name = "tsbStartSvr";
            this.tsbStartSvr.Size = new System.Drawing.Size(52, 22);
            this.tsbStartSvr.Text = "啟動";
            this.tsbStartSvr.Click += new System.EventHandler(this.tsbStartSvr_Click);
            // 
            // tsbQuerySub
            // 
            this.tsbQuerySub.Image = ((System.Drawing.Image)(resources.GetObject("tsbQuerySub.Image")));
            this.tsbQuerySub.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbQuerySub.Name = "tsbQuerySub";
            this.tsbQuerySub.Size = new System.Drawing.Size(76, 22);
            this.tsbQuerySub.Text = "查詢股票";
            this.tsbQuerySub.Click += new System.EventHandler(this.tsbQuerySub_Click);
            // 
            // tsbSubPush
            // 
            this.tsbSubPush.Image = ((System.Drawing.Image)(resources.GetObject("tsbSubPush.Image")));
            this.tsbSubPush.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbSubPush.Name = "tsbSubPush";
            this.tsbSubPush.Size = new System.Drawing.Size(76, 22);
            this.tsbSubPush.Text = "訂閱推播";
            this.tsbSubPush.Click += new System.EventHandler(this.tsbSubPush_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.tsslSvrStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 494);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(900, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "狀態";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(32, 17);
            this.toolStripStatusLabel1.Text = "狀態";
            // 
            // tsslSvrStatus
            // 
            this.tsslSvrStatus.ForeColor = System.Drawing.Color.Blue;
            this.tsslSvrStatus.Name = "tsslSvrStatus";
            this.tsslSvrStatus.Size = new System.Drawing.Size(44, 17);
            this.tsslSvrStatus.Text = "未執行";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.textBox3);
            this.panel1.Controls.Add(this.textBox2);
            this.panel1.Controls.Add(this.textBox1);
            this.panel1.Controls.Add(this.textBoxListenPort);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 25);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(900, 67);
            this.panel1.TabIndex = 2;
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(654, 17);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(75, 22);
            this.textBox3.TabIndex = 4;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(550, 17);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(75, 22);
            this.textBox2.TabIndex = 3;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(438, 17);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(75, 22);
            this.textBox1.TabIndex = 2;
            // 
            // textBoxListenPort
            // 
            this.textBoxListenPort.Location = new System.Drawing.Point(69, 11);
            this.textBoxListenPort.Name = "textBoxListenPort";
            this.textBoxListenPort.Size = new System.Drawing.Size(54, 22);
            this.textBoxListenPort.TabIndex = 1;
            this.textBoxListenPort.Text = "0";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 14);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(51, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "listen Port";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 92);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.dgvClientInfo);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.dgvSubStockList);
            this.splitContainer1.Panel2.Controls.Add(this.label2);
            this.splitContainer1.Size = new System.Drawing.Size(900, 402);
            this.splitContainer1.SplitterDistance = 164;
            this.splitContainer1.TabIndex = 3;
            // 
            // dgvClientInfo
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("新細明體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvClientInfo.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvClientInfo.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvClientInfo.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColTime,
            this.ColIP,
            this.ColStatus});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("新細明體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvClientInfo.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvClientInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvClientInfo.Location = new System.Drawing.Point(0, 20);
            this.dgvClientInfo.Name = "dgvClientInfo";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("新細明體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvClientInfo.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvClientInfo.RowTemplate.Height = 24;
            this.dgvClientInfo.Size = new System.Drawing.Size(900, 144);
            this.dgvClientInfo.TabIndex = 1;
            // 
            // ColTime
            // 
            this.ColTime.DataPropertyName = "ConnectionTime";
            this.ColTime.HeaderText = "時間";
            this.ColTime.Name = "ColTime";
            // 
            // ColIP
            // 
            this.ColIP.DataPropertyName = "ClientIP";
            this.ColIP.HeaderText = "IP";
            this.ColIP.Name = "ColIP";
            // 
            // ColStatus
            // 
            this.ColStatus.DataPropertyName = "Status";
            this.ColStatus.HeaderText = "狀態";
            this.ColStatus.Name = "ColStatus";
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.Gray;
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(900, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "連線";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // dgvSubStockList
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("新細明體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvSubStockList.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dgvSubStockList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSubStockList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColSymbol,
            this.ColSymbolName,
            this.ColTempRefer,
            this.ColRefer,
            this.ColLastDeal,
            this.ColDiff,
            this.ColMarket,
            this.ColStartDate,
            this.ColStopDate,
            this.ColDrawDate,
            this.ColCouponDate,
            this.ColRefundDate});
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("新細明體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvSubStockList.DefaultCellStyle = dataGridViewCellStyle5;
            this.dgvSubStockList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvSubStockList.Location = new System.Drawing.Point(0, 20);
            this.dgvSubStockList.Name = "dgvSubStockList";
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("新細明體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvSubStockList.RowHeadersDefaultCellStyle = dataGridViewCellStyle6;
            this.dgvSubStockList.RowTemplate.Height = 24;
            this.dgvSubStockList.Size = new System.Drawing.Size(900, 214);
            this.dgvSubStockList.TabIndex = 4;
            // 
            // ColSymbol
            // 
            this.ColSymbol.DataPropertyName = "Symbol";
            this.ColSymbol.HeaderText = "代碼";
            this.ColSymbol.Name = "ColSymbol";
            // 
            // ColSymbolName
            // 
            this.ColSymbolName.DataPropertyName = "SYmbolName";
            this.ColSymbolName.HeaderText = "名稱";
            this.ColSymbolName.Name = "ColSymbolName";
            // 
            // ColTempRefer
            // 
            this.ColTempRefer.DataPropertyName = "TempRefer";
            this.ColTempRefer.HeaderText = "暫定承銷價";
            this.ColTempRefer.Name = "ColTempRefer";
            // 
            // ColRefer
            // 
            this.ColRefer.DataPropertyName = "Refer";
            this.ColRefer.HeaderText = "實際承銷價";
            this.ColRefer.Name = "ColRefer";
            // 
            // ColLastDeal
            // 
            this.ColLastDeal.DataPropertyName = "LastDeal";
            this.ColLastDeal.HeaderText = "昨收價";
            this.ColLastDeal.Name = "ColLastDeal";
            // 
            // ColDiff
            // 
            this.ColDiff.DataPropertyName = "PriceDiffRatio";
            this.ColDiff.HeaderText = "價差幅度";
            this.ColDiff.Name = "ColDiff";
            // 
            // ColMarket
            // 
            this.ColMarket.DataPropertyName = "Market";
            this.ColMarket.HeaderText = "市場";
            this.ColMarket.Name = "ColMarket";
            // 
            // ColStartDate
            // 
            this.ColStartDate.DataPropertyName = "StartDate";
            this.ColStartDate.HeaderText = "申購日";
            this.ColStartDate.Name = "ColStartDate";
            // 
            // ColStopDate
            // 
            this.ColStopDate.DataPropertyName = "StopDate";
            this.ColStopDate.HeaderText = "申購截止日";
            this.ColStopDate.Name = "ColStopDate";
            // 
            // ColDrawDate
            // 
            this.ColDrawDate.DataPropertyName = "DrawDate";
            this.ColDrawDate.HeaderText = "抽簽日";
            this.ColDrawDate.Name = "ColDrawDate";
            // 
            // ColCouponDate
            // 
            this.ColCouponDate.DataPropertyName = "CouponDate";
            this.ColCouponDate.HeaderText = "撥券日";
            this.ColCouponDate.Name = "ColCouponDate";
            // 
            // ColRefundDate
            // 
            this.ColRefundDate.DataPropertyName = "RefundDate";
            this.ColRefundDate.HeaderText = "退款日";
            this.ColRefundDate.Name = "ColRefundDate";
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.Gray;
            this.label2.Dock = System.Windows.Forms.DockStyle.Top;
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(900, 20);
            this.label2.TabIndex = 3;
            this.label2.Text = "申購股票";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FormSvrMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 516);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "FormSvrMain";
            this.Text = "股票申購訂閱";
            this.Load += new System.EventHandler(this.FormSvrMain_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvClientInfo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSubStockList)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tsbClose;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel tsslSvrStatus;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox textBoxListenPort;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridView dgvClientInfo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton tsbStartSvr;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColIP;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColStatus;
        private System.Windows.Forms.ToolStripButton tsbQuerySub;
        private System.Windows.Forms.DataGridView dgvSubStockList;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColSymbol;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColSymbolName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColTempRefer;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColRefer;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColLastDeal;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColDiff;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColMarket;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColStartDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColStopDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColDrawDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColCouponDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColRefundDate;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.ToolStripButton tsbSubPush;
        private System.Windows.Forms.Timer timerMain;
    }
}

