namespace CivilFilingClient
{
    partial class frmSearchNotices
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
            this.btnSearchNotices = new System.Windows.Forms.Button();
            this.dtSearchNotices = new System.Windows.Forms.DateTimePicker();
            this.rtbSearchNoticies = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // btnSearchNotices
            // 
            this.btnSearchNotices.Location = new System.Drawing.Point(644, 249);
            this.btnSearchNotices.Name = "btnSearchNotices";
            this.btnSearchNotices.Size = new System.Drawing.Size(99, 80);
            this.btnSearchNotices.TabIndex = 0;
            this.btnSearchNotices.Text = "Search";
            this.btnSearchNotices.UseVisualStyleBackColor = true;
            this.btnSearchNotices.Click += new System.EventHandler(this.btnSearchNotices_Click);
            // 
            // dtSearchNotices
            // 
            this.dtSearchNotices.Location = new System.Drawing.Point(28, 27);
            this.dtSearchNotices.Name = "dtSearchNotices";
            this.dtSearchNotices.Size = new System.Drawing.Size(439, 31);
            this.dtSearchNotices.TabIndex = 1;
            // 
            // rtbSearchNoticies
            // 
            this.rtbSearchNoticies.Location = new System.Drawing.Point(28, 120);
            this.rtbSearchNoticies.Name = "rtbSearchNoticies";
            this.rtbSearchNoticies.Size = new System.Drawing.Size(439, 264);
            this.rtbSearchNoticies.TabIndex = 2;
            this.rtbSearchNoticies.Text = "";
            // 
            // frmSearchNotices
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.rtbSearchNoticies);
            this.Controls.Add(this.dtSearchNotices);
            this.Controls.Add(this.btnSearchNotices);
            this.Name = "frmSearchNotices";
            this.Text = "frmSearchNotices";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnSearchNotices;
        private System.Windows.Forms.DateTimePicker dtSearchNotices;
        private System.Windows.Forms.RichTextBox rtbSearchNoticies;
    }
}