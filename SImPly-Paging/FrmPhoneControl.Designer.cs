namespace SImPly_Paging
{
    partial class frmMainScreen
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
            this.components = new System.ComponentModel.Container();
            this.btnListTapi = new System.Windows.Forms.Button();
            this.cmbProviders = new System.Windows.Forms.ComboBox();
            this.btnCall = new System.Windows.Forms.Button();
            this.txtCallNumber = new System.Windows.Forms.TextBox();
            this.tmrFlash = new System.Windows.Forms.Timer(this.components);
            this.btnAnswer = new System.Windows.Forms.Button();
            this.chkAutoAnswer = new System.Windows.Forms.CheckBox();
            this.pctCurScreen = new System.Windows.Forms.PictureBox();
            this.btnGetScreen = new System.Windows.Forms.Button();
            this.btnTransfer = new System.Windows.Forms.Button();
            this.btnHold = new System.Windows.Forms.Button();
            this.lblIncomingCall = new System.Windows.Forms.Label();
            this.lblOutgoingCall = new System.Windows.Forms.Label();
            this.dataPhoneInfo = new System.Windows.Forms.DataGridView();
            this.Parameter = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Value = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cmbInformation = new System.Windows.Forms.ComboBox();
            this.btnStream = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.chkContinousRefresh = new System.Windows.Forms.CheckBox();
            this.btnSendMessage = new System.Windows.Forms.Button();
            this.txtChat = new System.Windows.Forms.RichTextBox();
            this.lblChat = new System.Windows.Forms.Label();
            this.mnuMain = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tmrScreenshot = new System.Windows.Forms.Timer(this.components);
            this.trkBar = new System.Windows.Forms.TrackBar();
            this.lblValue = new System.Windows.Forms.Label();
            this.btnNavUp = new System.Windows.Forms.Button();
            this.btnNavDwn = new System.Windows.Forms.Button();
            this.btnKeyLeft = new System.Windows.Forms.Button();
            this.btnKeyRight = new System.Windows.Forms.Button();
            this.btnSettings = new System.Windows.Forms.Button();
            this.btnNavSelect = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.btnDirectory = new System.Windows.Forms.Button();
            this.btnMessages = new System.Windows.Forms.Button();
            this.btnServices = new System.Windows.Forms.Button();
            this.btnSoft1 = new System.Windows.Forms.Button();
            this.btnSoft2 = new System.Windows.Forms.Button();
            this.btnSoft3 = new System.Windows.Forms.Button();
            this.btnSoft4 = new System.Windows.Forms.Button();
            this.btnSoft5 = new System.Windows.Forms.Button();
            this.wrkScreenshot = new System.ComponentModel.BackgroundWorker();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnResend = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pctCurScreen)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataPhoneInfo)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.mnuMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trkBar)).BeginInit();
            this.SuspendLayout();
            // 
            // btnListTapi
            // 
            this.btnListTapi.Location = new System.Drawing.Point(384, 12);
            this.btnListTapi.Name = "btnListTapi";
            this.btnListTapi.Size = new System.Drawing.Size(114, 37);
            this.btnListTapi.TabIndex = 0;
            this.btnListTapi.Text = "List Tapi Providers";
            this.btnListTapi.UseVisualStyleBackColor = true;
            this.btnListTapi.Click += new System.EventHandler(this.btnListView_Click);
            // 
            // cmbProviders
            // 
            this.cmbProviders.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbProviders.FormattingEnabled = true;
            this.cmbProviders.Location = new System.Drawing.Point(17, 27);
            this.cmbProviders.Name = "cmbProviders";
            this.cmbProviders.Size = new System.Drawing.Size(349, 21);
            this.cmbProviders.TabIndex = 1;
            this.cmbProviders.SelectedIndexChanged += new System.EventHandler(this.cmbProviders_SelectedIndexChanged);
            // 
            // btnCall
            // 
            this.btnCall.Location = new System.Drawing.Point(465, 104);
            this.btnCall.Name = "btnCall";
            this.btnCall.Size = new System.Drawing.Size(76, 39);
            this.btnCall.TabIndex = 3;
            this.btnCall.Text = "Call";
            this.btnCall.UseVisualStyleBackColor = true;
            this.btnCall.Click += new System.EventHandler(this.btnCall_Click);
            // 
            // txtCallNumber
            // 
            this.txtCallNumber.Location = new System.Drawing.Point(384, 78);
            this.txtCallNumber.Name = "txtCallNumber";
            this.txtCallNumber.Size = new System.Drawing.Size(157, 20);
            this.txtCallNumber.TabIndex = 4;
            // 
            // tmrFlash
            // 
            this.tmrFlash.Interval = 700;
            this.tmrFlash.Tick += new System.EventHandler(this.tmrFlash_Tick);
            // 
            // btnAnswer
            // 
            this.btnAnswer.Enabled = false;
            this.btnAnswer.Location = new System.Drawing.Point(383, 104);
            this.btnAnswer.Name = "btnAnswer";
            this.btnAnswer.Size = new System.Drawing.Size(76, 39);
            this.btnAnswer.TabIndex = 5;
            this.btnAnswer.Text = "Answer";
            this.btnAnswer.UseVisualStyleBackColor = true;
            this.btnAnswer.Click += new System.EventHandler(this.btnAnswer_Click);
            // 
            // chkAutoAnswer
            // 
            this.chkAutoAnswer.AutoSize = true;
            this.chkAutoAnswer.Checked = true;
            this.chkAutoAnswer.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAutoAnswer.Location = new System.Drawing.Point(384, 55);
            this.chkAutoAnswer.Name = "chkAutoAnswer";
            this.chkAutoAnswer.Size = new System.Drawing.Size(165, 17);
            this.chkAutoAnswer.TabIndex = 6;
            this.chkAutoAnswer.Text = "Auto Answer (Speakerphone)";
            this.chkAutoAnswer.UseVisualStyleBackColor = true;
            this.chkAutoAnswer.CheckedChanged += new System.EventHandler(this.chkAutoAnswer_CheckedChanged);
            // 
            // pctCurScreen
            // 
            this.pctCurScreen.Location = new System.Drawing.Point(17, 54);
            this.pctCurScreen.Name = "pctCurScreen";
            this.pctCurScreen.Size = new System.Drawing.Size(355, 242);
            this.pctCurScreen.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pctCurScreen.TabIndex = 7;
            this.pctCurScreen.TabStop = false;
            // 
            // btnGetScreen
            // 
            this.btnGetScreen.Location = new System.Drawing.Point(239, 543);
            this.btnGetScreen.Name = "btnGetScreen";
            this.btnGetScreen.Size = new System.Drawing.Size(91, 39);
            this.btnGetScreen.TabIndex = 8;
            this.btnGetScreen.Text = "Get Current Phone Screen";
            this.btnGetScreen.UseVisualStyleBackColor = true;
            this.btnGetScreen.Click += new System.EventHandler(this.btnGetScreen_Click);
            // 
            // btnTransfer
            // 
            this.btnTransfer.Location = new System.Drawing.Point(20, 393);
            this.btnTransfer.Name = "btnTransfer";
            this.btnTransfer.Size = new System.Drawing.Size(147, 39);
            this.btnTransfer.TabIndex = 9;
            this.btnTransfer.Text = "Transfer";
            this.btnTransfer.UseVisualStyleBackColor = true;
            this.btnTransfer.Click += new System.EventHandler(this.btnTransfer_Click);
            // 
            // btnHold
            // 
            this.btnHold.Location = new System.Drawing.Point(183, 393);
            this.btnHold.Name = "btnHold";
            this.btnHold.Size = new System.Drawing.Size(147, 39);
            this.btnHold.TabIndex = 10;
            this.btnHold.Text = "Hold";
            this.btnHold.UseVisualStyleBackColor = true;
            this.btnHold.Click += new System.EventHandler(this.btnHold_Click);
            // 
            // lblIncomingCall
            // 
            this.lblIncomingCall.AutoSize = true;
            this.lblIncomingCall.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblIncomingCall.Location = new System.Drawing.Point(637, 529);
            this.lblIncomingCall.Name = "lblIncomingCall";
            this.lblIncomingCall.Size = new System.Drawing.Size(87, 13);
            this.lblIncomingCall.TabIndex = 11;
            this.lblIncomingCall.Text = "Incoming Call:";
            // 
            // lblOutgoingCall
            // 
            this.lblOutgoingCall.AutoSize = true;
            this.lblOutgoingCall.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOutgoingCall.Location = new System.Drawing.Point(637, 557);
            this.lblOutgoingCall.Name = "lblOutgoingCall";
            this.lblOutgoingCall.Size = new System.Drawing.Size(87, 13);
            this.lblOutgoingCall.TabIndex = 12;
            this.lblOutgoingCall.Text = "Outgoing Call:";
            // 
            // dataPhoneInfo
            // 
            this.dataPhoneInfo.AllowUserToOrderColumns = true;
            this.dataPhoneInfo.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataPhoneInfo.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Parameter,
            this.Value});
            this.dataPhoneInfo.Location = new System.Drawing.Point(518, 172);
            this.dataPhoneInfo.Name = "dataPhoneInfo";
            this.dataPhoneInfo.Size = new System.Drawing.Size(429, 252);
            this.dataPhoneInfo.TabIndex = 13;
            this.dataPhoneInfo.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataPhoneInfo_CellContentClick);
            this.dataPhoneInfo.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataPhoneInfo_CellContentDoubleClick);
            // 
            // Parameter
            // 
            this.Parameter.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Parameter.HeaderText = "Parameter";
            this.Parameter.Name = "Parameter";
            // 
            // Value
            // 
            this.Value.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Value.HeaderText = "Value";
            this.Value.Name = "Value";
            // 
            // cmbInformation
            // 
            this.cmbInformation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbInformation.FormattingEnabled = true;
            this.cmbInformation.Items.AddRange(new object[] {
            "Device Information",
            "Network Configuration",
            "Ethernet Information",
            "Port Information",
            "Device Log",
            "Streaming Statistics"});
            this.cmbInformation.Location = new System.Drawing.Point(519, 431);
            this.cmbInformation.Name = "cmbInformation";
            this.cmbInformation.Size = new System.Drawing.Size(428, 21);
            this.cmbInformation.TabIndex = 14;
            this.cmbInformation.SelectedIndexChanged += new System.EventHandler(this.cmbInformation_SelectedIndexChanged);
            // 
            // btnStream
            // 
            this.btnStream.Location = new System.Drawing.Point(20, 597);
            this.btnStream.Name = "btnStream";
            this.btnStream.Size = new System.Drawing.Size(75, 39);
            this.btnStream.TabIndex = 15;
            this.btnStream.Text = "Stream";
            this.btnStream.UseVisualStyleBackColor = true;
            this.btnStream.Click += new System.EventHandler(this.btnStream_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(760, 486);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(200, 100);
            this.tabControl1.TabIndex = 16;
            this.tabControl1.Visible = false;
            // 
            // tabPage1
            // 
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(192, 74);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(192, 74);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // chkContinousRefresh
            // 
            this.chkContinousRefresh.AutoSize = true;
            this.chkContinousRefresh.Location = new System.Drawing.Point(558, 509);
            this.chkContinousRefresh.Name = "chkContinousRefresh";
            this.chkContinousRefresh.Size = new System.Drawing.Size(177, 17);
            this.chkContinousRefresh.TabIndex = 17;
            this.chkContinousRefresh.Text = "Continously Refresh Screenshot";
            this.chkContinousRefresh.UseVisualStyleBackColor = true;
            this.chkContinousRefresh.CheckedChanged += new System.EventHandler(this.chkContinousRefresh_CheckedChanged);
            // 
            // btnSendMessage
            // 
            this.btnSendMessage.Location = new System.Drawing.Point(586, 121);
            this.btnSendMessage.Name = "btnSendMessage";
            this.btnSendMessage.Size = new System.Drawing.Size(244, 45);
            this.btnSendMessage.TabIndex = 18;
            this.btnSendMessage.Text = "Send Message to Phone";
            this.btnSendMessage.UseVisualStyleBackColor = true;
            this.btnSendMessage.Click += new System.EventHandler(this.btnSendMessage_Click);
            // 
            // txtChat
            // 
            this.txtChat.Location = new System.Drawing.Point(577, 28);
            this.txtChat.Name = "txtChat";
            this.txtChat.Size = new System.Drawing.Size(263, 84);
            this.txtChat.TabIndex = 19;
            this.txtChat.Text = "";
            // 
            // lblChat
            // 
            this.lblChat.AutoSize = true;
            this.lblChat.Location = new System.Drawing.Point(637, 12);
            this.lblChat.Name = "lblChat";
            this.lblChat.Size = new System.Drawing.Size(53, 13);
            this.lblChat.TabIndex = 20;
            this.lblChat.Text = "Chat Box:";
            // 
            // mnuMain
            // 
            this.mnuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.mnuMain.Location = new System.Drawing.Point(0, 0);
            this.mnuMain.Name = "mnuMain";
            this.mnuMain.Size = new System.Drawing.Size(960, 24);
            this.mnuMain.TabIndex = 21;
            this.mnuMain.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.settingsToolStripMenuItem.Text = "Settings";
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // tmrScreenshot
            // 
            this.tmrScreenshot.Interval = 3000;
            this.tmrScreenshot.Tick += new System.EventHandler(this.tmrScreenshot_Tick);
            // 
            // trkBar
            // 
            this.trkBar.Location = new System.Drawing.Point(558, 458);
            this.trkBar.Maximum = 5;
            this.trkBar.Minimum = 1;
            this.trkBar.Name = "trkBar";
            this.trkBar.Size = new System.Drawing.Size(166, 45);
            this.trkBar.TabIndex = 22;
            this.trkBar.Value = 1;
            this.trkBar.Scroll += new System.EventHandler(this.trkBar_Scroll);
            // 
            // lblValue
            // 
            this.lblValue.AutoSize = true;
            this.lblValue.Location = new System.Drawing.Point(790, 573);
            this.lblValue.Name = "lblValue";
            this.lblValue.Size = new System.Drawing.Size(0, 13);
            this.lblValue.TabIndex = 23;
            // 
            // btnNavUp
            // 
            this.btnNavUp.Font = new System.Drawing.Font("Microsoft Sans Serif", 27.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNavUp.Location = new System.Drawing.Point(412, 347);
            this.btnNavUp.Name = "btnNavUp";
            this.btnNavUp.Size = new System.Drawing.Size(42, 50);
            this.btnNavUp.TabIndex = 24;
            this.btnNavUp.Text = "^";
            this.btnNavUp.UseVisualStyleBackColor = true;
            this.btnNavUp.Click += new System.EventHandler(this.btnKeyUp_Click);
            // 
            // btnNavDwn
            // 
            this.btnNavDwn.Font = new System.Drawing.Font("Microsoft Sans Serif", 27.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNavDwn.Location = new System.Drawing.Point(411, 458);
            this.btnNavDwn.Name = "btnNavDwn";
            this.btnNavDwn.Size = new System.Drawing.Size(42, 56);
            this.btnNavDwn.TabIndex = 25;
            this.btnNavDwn.Text = "Down";
            this.btnNavDwn.UseVisualStyleBackColor = true;
            this.btnNavDwn.Click += new System.EventHandler(this.btnKeyUp_Click);
            // 
            // btnKeyLeft
            // 
            this.btnKeyLeft.Font = new System.Drawing.Font("Microsoft Sans Serif", 27.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnKeyLeft.Location = new System.Drawing.Point(361, 403);
            this.btnKeyLeft.Name = "btnKeyLeft";
            this.btnKeyLeft.Size = new System.Drawing.Size(42, 49);
            this.btnKeyLeft.TabIndex = 26;
            this.btnKeyLeft.Text = "<";
            this.btnKeyLeft.UseVisualStyleBackColor = true;
            this.btnKeyLeft.Click += new System.EventHandler(this.btnKeyUp_Click);
            // 
            // btnKeyRight
            // 
            this.btnKeyRight.Font = new System.Drawing.Font("Microsoft Sans Serif", 27.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnKeyRight.Location = new System.Drawing.Point(465, 403);
            this.btnKeyRight.Name = "btnKeyRight";
            this.btnKeyRight.Size = new System.Drawing.Size(42, 49);
            this.btnKeyRight.TabIndex = 27;
            this.btnKeyRight.Text = ">";
            this.btnKeyRight.UseVisualStyleBackColor = true;
            this.btnKeyRight.Click += new System.EventHandler(this.btnKeyUp_Click);
            // 
            // btnSettings
            // 
            this.btnSettings.Location = new System.Drawing.Point(20, 448);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(147, 37);
            this.btnSettings.TabIndex = 28;
            this.btnSettings.Text = "Settings";
            this.btnSettings.UseVisualStyleBackColor = true;
            this.btnSettings.Click += new System.EventHandler(this.btnKeyUp_Click);
            // 
            // btnNavSelect
            // 
            this.btnNavSelect.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNavSelect.Location = new System.Drawing.Point(406, 403);
            this.btnNavSelect.Name = "btnNavSelect";
            this.btnNavSelect.Size = new System.Drawing.Size(54, 49);
            this.btnNavSelect.TabIndex = 29;
            this.btnNavSelect.Text = "Select";
            this.btnNavSelect.UseVisualStyleBackColor = true;
            this.btnNavSelect.Click += new System.EventHandler(this.btnKeyUp_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(859, 83);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 80);
            this.button1.TabIndex = 30;
            this.button1.Text = "Manually set IP of phone";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnDirectory
            // 
            this.btnDirectory.Location = new System.Drawing.Point(183, 448);
            this.btnDirectory.Name = "btnDirectory";
            this.btnDirectory.Size = new System.Drawing.Size(147, 37);
            this.btnDirectory.TabIndex = 31;
            this.btnDirectory.Text = "Directory";
            this.btnDirectory.UseVisualStyleBackColor = true;
            this.btnDirectory.Click += new System.EventHandler(this.btnKeyUp_Click);
            // 
            // btnMessages
            // 
            this.btnMessages.Location = new System.Drawing.Point(20, 494);
            this.btnMessages.Name = "btnMessages";
            this.btnMessages.Size = new System.Drawing.Size(147, 37);
            this.btnMessages.TabIndex = 32;
            this.btnMessages.Text = "Messages";
            this.btnMessages.UseVisualStyleBackColor = true;
            this.btnMessages.Click += new System.EventHandler(this.btnKeyUp_Click);
            // 
            // btnServices
            // 
            this.btnServices.Location = new System.Drawing.Point(183, 494);
            this.btnServices.Name = "btnServices";
            this.btnServices.Size = new System.Drawing.Size(147, 37);
            this.btnServices.TabIndex = 33;
            this.btnServices.Text = "Services";
            this.btnServices.UseVisualStyleBackColor = true;
            this.btnServices.Click += new System.EventHandler(this.btnKeyUp_Click);
            // 
            // btnSoft1
            // 
            this.btnSoft1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btnSoft1.Location = new System.Drawing.Point(12, 302);
            this.btnSoft1.Name = "btnSoft1";
            this.btnSoft1.Size = new System.Drawing.Size(75, 23);
            this.btnSoft1.TabIndex = 34;
            this.btnSoft1.UseVisualStyleBackColor = false;
            this.btnSoft1.Click += new System.EventHandler(this.btnSoft1_Click);
            // 
            // btnSoft2
            // 
            this.btnSoft2.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btnSoft2.Location = new System.Drawing.Point(92, 302);
            this.btnSoft2.Name = "btnSoft2";
            this.btnSoft2.Size = new System.Drawing.Size(75, 23);
            this.btnSoft2.TabIndex = 35;
            this.btnSoft2.UseVisualStyleBackColor = false;
            this.btnSoft2.Click += new System.EventHandler(this.btnSoft1_Click);
            // 
            // btnSoft3
            // 
            this.btnSoft3.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btnSoft3.Location = new System.Drawing.Point(173, 302);
            this.btnSoft3.Name = "btnSoft3";
            this.btnSoft3.Size = new System.Drawing.Size(75, 23);
            this.btnSoft3.TabIndex = 36;
            this.btnSoft3.UseVisualStyleBackColor = false;
            this.btnSoft3.Click += new System.EventHandler(this.btnSoft1_Click);
            // 
            // btnSoft4
            // 
            this.btnSoft4.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btnSoft4.Location = new System.Drawing.Point(254, 302);
            this.btnSoft4.Name = "btnSoft4";
            this.btnSoft4.Size = new System.Drawing.Size(75, 23);
            this.btnSoft4.TabIndex = 37;
            this.btnSoft4.UseVisualStyleBackColor = false;
            this.btnSoft4.Click += new System.EventHandler(this.btnSoft1_Click);
            // 
            // btnSoft5
            // 
            this.btnSoft5.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btnSoft5.Location = new System.Drawing.Point(335, 302);
            this.btnSoft5.Name = "btnSoft5";
            this.btnSoft5.Size = new System.Drawing.Size(75, 23);
            this.btnSoft5.TabIndex = 38;
            this.btnSoft5.UseVisualStyleBackColor = false;
            this.btnSoft5.Click += new System.EventHandler(this.btnSoft1_Click);
            // 
            // wrkScreenshot
            // 
            this.wrkScreenshot.DoWork += new System.ComponentModel.DoWorkEventHandler(this.wrkScreenshot_DoWork);
            this.wrkScreenshot.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.wrkScreenshot_RunWorkerCompleted);
            // 
            // txtPort
            // 
            this.txtPort.Location = new System.Drawing.Point(20, 571);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(100, 20);
            this.txtPort.TabIndex = 39;
            this.txtPort.Text = "20490";
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(101, 597);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 38);
            this.btnStop.TabIndex = 40;
            this.btnStop.Text = "Stop stream";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnResend
            // 
            this.btnResend.Location = new System.Drawing.Point(183, 597);
            this.btnResend.Name = "btnResend";
            this.btnResend.Size = new System.Drawing.Size(75, 38);
            this.btnResend.TabIndex = 41;
            this.btnResend.Text = "Resend";
            this.btnResend.UseVisualStyleBackColor = true;
            this.btnResend.Click += new System.EventHandler(this.btnResend_Click);
            // 
            // frmMainScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(960, 648);
            this.Controls.Add(this.btnResend);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.txtPort);
            this.Controls.Add(this.btnSoft5);
            this.Controls.Add(this.btnSoft4);
            this.Controls.Add(this.btnSoft3);
            this.Controls.Add(this.btnSoft2);
            this.Controls.Add(this.btnSoft1);
            this.Controls.Add(this.btnServices);
            this.Controls.Add(this.btnMessages);
            this.Controls.Add(this.btnDirectory);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnNavSelect);
            this.Controls.Add(this.btnSettings);
            this.Controls.Add(this.btnKeyRight);
            this.Controls.Add(this.btnKeyLeft);
            this.Controls.Add(this.btnNavDwn);
            this.Controls.Add(this.btnNavUp);
            this.Controls.Add(this.lblValue);
            this.Controls.Add(this.trkBar);
            this.Controls.Add(this.lblChat);
            this.Controls.Add(this.txtChat);
            this.Controls.Add(this.btnSendMessage);
            this.Controls.Add(this.chkContinousRefresh);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.btnStream);
            this.Controls.Add(this.cmbInformation);
            this.Controls.Add(this.dataPhoneInfo);
            this.Controls.Add(this.lblOutgoingCall);
            this.Controls.Add(this.lblIncomingCall);
            this.Controls.Add(this.btnHold);
            this.Controls.Add(this.btnTransfer);
            this.Controls.Add(this.btnGetScreen);
            this.Controls.Add(this.pctCurScreen);
            this.Controls.Add(this.chkAutoAnswer);
            this.Controls.Add(this.btnAnswer);
            this.Controls.Add(this.txtCallNumber);
            this.Controls.Add(this.btnCall);
            this.Controls.Add(this.cmbProviders);
            this.Controls.Add(this.btnListTapi);
            this.Controls.Add(this.mnuMain);
            this.MainMenuStrip = this.mnuMain;
            this.Name = "frmMainScreen";
            this.Text = "Phone Control V0.1";
            this.Load += new System.EventHandler(this.frmMainScreen_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pctCurScreen)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataPhoneInfo)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.mnuMain.ResumeLayout(false);
            this.mnuMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trkBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void btnStream_Click(object sender, System.EventArgs e)
        {
            // Create and start RTP client
            RTPReceive = new ccierants.baseclasses.RTPClient(int.Parse(txtPort.Text));

            RTPReceive.StartClient();
            RTPReceive.packetReceived += RTPReceive_packetReceived;

        }

     

      
        #endregion

        private System.Windows.Forms.Button btnListTapi;
        private System.Windows.Forms.ComboBox cmbProviders;
        private System.Windows.Forms.Button btnCall;
        private System.Windows.Forms.TextBox txtCallNumber;
        private System.Windows.Forms.Timer tmrFlash;
        private System.Windows.Forms.Button btnAnswer;
        private System.Windows.Forms.CheckBox chkAutoAnswer;
        private System.Windows.Forms.PictureBox pctCurScreen;
        private System.Windows.Forms.Button btnGetScreen;
        private System.Windows.Forms.Button btnTransfer;
        private System.Windows.Forms.Button btnHold;
        private System.Windows.Forms.Label lblIncomingCall;
        private System.Windows.Forms.Label lblOutgoingCall;
        private System.Windows.Forms.DataGridView dataPhoneInfo;
        private System.Windows.Forms.Button btnStream;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.CheckBox chkContinousRefresh;
        private System.Windows.Forms.Button btnSendMessage;
        private System.Windows.Forms.RichTextBox txtChat;
        private System.Windows.Forms.Label lblChat;
        private System.Windows.Forms.ComboBox cmbInformation;
        private System.Windows.Forms.MenuStrip mnuMain;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.DataGridViewTextBoxColumn Parameter;
        private System.Windows.Forms.DataGridViewTextBoxColumn Value;
        private System.Windows.Forms.Timer tmrScreenshot;
        private System.Windows.Forms.TrackBar trkBar;
        private System.Windows.Forms.Label lblValue;
        private System.Windows.Forms.Button btnNavUp;
        private System.Windows.Forms.Button btnNavDwn;
        private System.Windows.Forms.Button btnKeyLeft;
        private System.Windows.Forms.Button btnKeyRight;
        private System.Windows.Forms.Button btnSettings;
        private System.Windows.Forms.Button btnNavSelect;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnDirectory;
        private System.Windows.Forms.Button btnMessages;
        private System.Windows.Forms.Button btnServices;
        private System.Windows.Forms.Button btnSoft1;
        private System.Windows.Forms.Button btnSoft2;
        private System.Windows.Forms.Button btnSoft3;
        private System.Windows.Forms.Button btnSoft4;
        private System.Windows.Forms.Button btnSoft5;
        private System.ComponentModel.BackgroundWorker wrkScreenshot;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnResend;
    }
}

