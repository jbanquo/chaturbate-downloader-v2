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
            this.useHttpProxyCheckBox = new System.Windows.Forms.CheckBox();
            this.useHttpsProxyCheckBox = new System.Windows.Forms.CheckBox();
            this.httpsProxyTextBox = new System.Windows.Forms.TextBox();
            this.httpProxyTextBox = new System.Windows.Forms.TextBox();
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
            this.doneButton.Location = new System.Drawing.Point(228, 84);
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
            this.cancelButton.Location = new System.Drawing.Point(309, 84);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 3;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // useHttpProxyCheckBox
            // 
            this.useHttpProxyCheckBox.AutoSize = true;
            this.useHttpProxyCheckBox.Location = new System.Drawing.Point(15, 34);
            this.useHttpProxyCheckBox.Name = "useHttpProxyCheckBox";
            this.useHttpProxyCheckBox.Size = new System.Drawing.Size(106, 17);
            this.useHttpProxyCheckBox.TabIndex = 6;
            this.useHttpProxyCheckBox.Text = "Use HTTP Proxy";
            this.useHttpProxyCheckBox.UseVisualStyleBackColor = true;
            // 
            // useHttpsProxyCheckBox
            // 
            this.useHttpsProxyCheckBox.AutoSize = true;
            this.useHttpsProxyCheckBox.Location = new System.Drawing.Point(15, 61);
            this.useHttpsProxyCheckBox.Name = "useHttpsProxyCheckBox";
            this.useHttpsProxyCheckBox.Size = new System.Drawing.Size(113, 17);
            this.useHttpsProxyCheckBox.TabIndex = 7;
            this.useHttpsProxyCheckBox.Text = "Use HTTPS Proxy";
            this.useHttpsProxyCheckBox.UseVisualStyleBackColor = true;
            // 
            // httpsProxyTextBox
            // 
            this.httpsProxyTextBox.Location = new System.Drawing.Point(130, 58);
            this.httpsProxyTextBox.Name = "httpsProxyTextBox";
            this.httpsProxyTextBox.Size = new System.Drawing.Size(253, 20);
            this.httpsProxyTextBox.TabIndex = 8;
            // 
            // httpProxyTextBox
            // 
            this.httpProxyTextBox.Location = new System.Drawing.Point(130, 32);
            this.httpProxyTextBox.Name = "httpProxyTextBox";
            this.httpProxyTextBox.Size = new System.Drawing.Size(253, 20);
            this.httpProxyTextBox.TabIndex = 9;
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(395, 118);
            this.Controls.Add(this.httpProxyTextBox);
            this.Controls.Add(this.httpsProxyTextBox);
            this.Controls.Add(this.useHttpsProxyCheckBox);
            this.Controls.Add(this.useHttpProxyCheckBox);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.streamlinkExecutableBrowseButton);
            this.Controls.Add(this.streamlinkExecutableTextBox);
            this.Controls.Add(this.doneButton);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Name = "SettingsForm";
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.SettingsForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button doneButton;
        private System.Windows.Forms.TextBox streamlinkExecutableTextBox;
        private System.Windows.Forms.Button streamlinkExecutableBrowseButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.CheckBox useHttpProxyCheckBox;
        private System.Windows.Forms.CheckBox useHttpsProxyCheckBox;
        private System.Windows.Forms.TextBox httpsProxyTextBox;
        private System.Windows.Forms.TextBox httpProxyTextBox;
    }
}