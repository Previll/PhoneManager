namespace CCIERantsPhoneControl
{
    partial class frmMsgView
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
            this.pageSetupDialog1 = new System.Windows.Forms.PageSetupDialog();
            this.rchTxtMsg = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // rchTxtMsg
            // 
            this.rchTxtMsg.Location = new System.Drawing.Point(0, 0);
            this.rchTxtMsg.Name = "rchTxtMsg";
            this.rchTxtMsg.Size = new System.Drawing.Size(530, 339);
            this.rchTxtMsg.TabIndex = 1;
            this.rchTxtMsg.Text = "";
            // 
            // frmMsgView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(533, 342);
            this.Controls.Add(this.rchTxtMsg);
            this.Name = "frmMsgView";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Log Message View";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.frmMsgView_Load);
            this.ResizeEnd += new System.EventHandler(this.frmMsgView_ResizeEnd);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.PageSetupDialog pageSetupDialog1;
        private System.Windows.Forms.RichTextBox rchTxtMsg;
    }
}