namespace EVE_Bot
{
    partial class frmMain
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
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.btnAnnounce = new System.Windows.Forms.Button();
            this.rtbInput = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // btnAnnounce
            // 
            this.btnAnnounce.Location = new System.Drawing.Point(12, 12);
            this.btnAnnounce.Name = "btnAnnounce";
            this.btnAnnounce.Size = new System.Drawing.Size(75, 23);
            this.btnAnnounce.TabIndex = 0;
            this.btnAnnounce.Text = "开机提醒";
            this.btnAnnounce.UseVisualStyleBackColor = true;
            this.btnAnnounce.Click += new System.EventHandler(this.btnAnnounce_Click);
            // 
            // rtbInput
            // 
            this.rtbInput.Location = new System.Drawing.Point(248, 12);
            this.rtbInput.Name = "rtbInput";
            this.rtbInput.Size = new System.Drawing.Size(295, 205);
            this.rtbInput.TabIndex = 1;
            this.rtbInput.Text = "";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(556, 233);
            this.Controls.Add(this.rtbInput);
            this.Controls.Add(this.btnAnnounce);
            this.Name = "frmMain";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnAnnounce;
        private System.Windows.Forms.RichTextBox rtbInput;
    }
}

