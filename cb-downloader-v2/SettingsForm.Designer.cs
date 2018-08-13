namespace cb_downloader_v2
{
    partial class SettingsForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.doneButton = new System.Windows.Forms.Button();
            this.streamlinkExecutableTextBox = new System.Windows.Forms.TextBox();
            this.streamlinkExecutableBrowseButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(112, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Streamlink Executable";
            // 
            // doneButton
            // 
            this.doneButton.Location = new System.Drawing.Point(228, 32);
            this.doneButton.Name = "doneButton";
            this.doneButton.Size = new System.Drawing.Size(75, 23);
            this.doneButton.TabIndex = 2;
            this.doneButton.Text = "Done";
            this.doneButton.UseVisualStyleBackColor = true;
            this.doneButton.Click += new System.EventHandler(this.doneButton_Click);
            // 
            // streamlinkExecutableTextBox
            // 
            this.streamlinkExecutableTextBox.Location = new System.Drawing.Point(130, 6);
            this.streamlinkExecutableTextBox.Name = "streamlinkExecutableTextBox";
            this.streamlinkExecutableTextBox.Size = new System.Drawing.Size(222, 20);
            this.streamlinkExecutableTextBox.TabIndex = 0;
            // 
            // streamlinkExecutableBrowseButton
            // 
            this.streamlinkExecutableBrowseButton.Location = new System.Drawing.Point(358, 6);
            this.streamlinkExecutableBrowseButton.Name = "streamlinkExecutableBrowseButton";
            this.streamlinkExecutableBrowseButton.Size = new System.Drawing.Size(26, 20);
            this.streamlinkExecutableBrowseButton.TabIndex = 1;
            this.streamlinkExecutableBrowseButton.Text = "...";
            this.streamlinkExecutableBrowseButton.UseVisualStyleBackColor = true;
            this.streamlinkExecutableBrowseButton.Click += new System.EventHandler(this.browseStreamlinkExecutableButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(309, 32);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 3;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(395, 63);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.streamlinkExecutableBrowseButton);
            this.Controls.Add(this.streamlinkExecutableTextBox);
            this.Controls.Add(this.doneButton);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Name = "SettingsForm";
            this.Text = "Settings";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button doneButton;
        private System.Windows.Forms.TextBox streamlinkExecutableTextBox;
        private System.Windows.Forms.Button streamlinkExecutableBrowseButton;
        private System.Windows.Forms.Button cancelButton;
    }
}