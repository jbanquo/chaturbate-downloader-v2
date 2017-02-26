namespace cb_downloader_v2
{
    partial class MainForm
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
            this.modelsBox = new System.Windows.Forms.CheckedListBox();
            this.modelsBoxCtxMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.removeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.restartToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeAllUncheckedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.modelsGroupBox = new System.Windows.Forms.GroupBox();
            this.modelNameTextBox = new System.Windows.Forms.TextBox();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveModelsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.quickAddModelButton = new System.Windows.Forms.Button();
            this.modelsBoxCtxMenu.SuspendLayout();
            this.modelsGroupBox.SuspendLayout();
            this.menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // modelsBox
            // 
            this.modelsBox.ContextMenuStrip = this.modelsBoxCtxMenu;
            this.modelsBox.FormattingEnabled = true;
            this.modelsBox.Location = new System.Drawing.Point(6, 19);
            this.modelsBox.Name = "modelsBox";
            this.modelsBox.Size = new System.Drawing.Size(387, 259);
            this.modelsBox.Sorted = true;
            this.modelsBox.TabIndex = 0;
            this.modelsBox.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.modelsBox_ItemCheck);
            // 
            // modelsBoxCtxMenu
            // 
            this.modelsBoxCtxMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.removeMenuItem,
            this.restartToolStripMenuItem,
            this.removeAllUncheckedToolStripMenuItem});
            this.modelsBoxCtxMenu.Name = "modelsBoxCtxMenu";
            this.modelsBoxCtxMenu.ShowImageMargin = false;
            this.modelsBoxCtxMenu.Size = new System.Drawing.Size(172, 70);
            this.modelsBoxCtxMenu.Opening += new System.ComponentModel.CancelEventHandler(this.modelsBoxCtxMenu_Opening);
            // 
            // removeMenuItem
            // 
            this.removeMenuItem.Name = "removeMenuItem";
            this.removeMenuItem.Size = new System.Drawing.Size(171, 22);
            this.removeMenuItem.Text = "Remove";
            this.removeMenuItem.Click += new System.EventHandler(this.removeMenuItem_Click);
            // 
            // restartToolStripMenuItem
            // 
            this.restartToolStripMenuItem.Name = "restartToolStripMenuItem";
            this.restartToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.restartToolStripMenuItem.Text = "Restart";
            this.restartToolStripMenuItem.Click += new System.EventHandler(this.restartToolStripMenuItem_Click);
            // 
            // removeAllUncheckedToolStripMenuItem
            // 
            this.removeAllUncheckedToolStripMenuItem.Name = "removeAllUncheckedToolStripMenuItem";
            this.removeAllUncheckedToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.removeAllUncheckedToolStripMenuItem.Text = "Remove All Unchecked";
            this.removeAllUncheckedToolStripMenuItem.Click += new System.EventHandler(this.removeAllUncheckedToolStripMenuItem_Click_1);
            // 
            // modelsGroupBox
            // 
            this.modelsGroupBox.Controls.Add(this.modelsBox);
            this.modelsGroupBox.Location = new System.Drawing.Point(12, 27);
            this.modelsGroupBox.Name = "modelsGroupBox";
            this.modelsGroupBox.Size = new System.Drawing.Size(399, 292);
            this.modelsGroupBox.TabIndex = 1;
            this.modelsGroupBox.TabStop = false;
            this.modelsGroupBox.Text = "Models";
            // 
            // modelNameTextBox
            // 
            this.modelNameTextBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.modelNameTextBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.modelNameTextBox.Location = new System.Drawing.Point(12, 327);
            this.modelNameTextBox.Name = "modelNameTextBox";
            this.modelNameTextBox.Size = new System.Drawing.Size(312, 20);
            this.modelNameTextBox.TabIndex = 2;
            this.modelNameTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.modelNameTextBox_KeyDown);
            this.modelNameTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.modelNameTextBox_KeyUp);
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(419, 24);
            this.menuStrip.TabIndex = 3;
            this.menuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveModelsToolStripMenuItem,
            this.logToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // saveModelsToolStripMenuItem
            // 
            this.saveModelsToolStripMenuItem.Name = "saveModelsToolStripMenuItem";
            this.saveModelsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveModelsToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.saveModelsToolStripMenuItem.Text = "Save Models...";
            this.saveModelsToolStripMenuItem.Click += new System.EventHandler(this.saveModelsToolStripMenuItem_Click);
            // 
            // logToolStripMenuItem
            // 
            this.logToolStripMenuItem.Name = "logToolStripMenuItem";
            this.logToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.logToolStripMenuItem.Text = "Log Debug Output";
            this.logToolStripMenuItem.Click += new System.EventHandler(this.logToolStripMenuItem_Click);
            // 
            // quickAddModelButton
            // 
            this.quickAddModelButton.Location = new System.Drawing.Point(330, 325);
            this.quickAddModelButton.Name = "quickAddModelButton";
            this.quickAddModelButton.Size = new System.Drawing.Size(75, 23);
            this.quickAddModelButton.TabIndex = 4;
            this.quickAddModelButton.Text = "Add";
            this.quickAddModelButton.UseVisualStyleBackColor = true;
            this.quickAddModelButton.Click += new System.EventHandler(this.quickAddModelButton_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(419, 356);
            this.Controls.Add(this.quickAddModelButton);
            this.Controls.Add(this.menuStrip);
            this.Controls.Add(this.modelNameTextBox);
            this.Controls.Add(this.modelsGroupBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MainMenuStrip = this.menuStrip;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "cb-downloader-2";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.modelsBoxCtxMenu.ResumeLayout(false);
            this.modelsGroupBox.ResumeLayout(false);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckedListBox modelsBox;
        private System.Windows.Forms.GroupBox modelsGroupBox;
        private System.Windows.Forms.TextBox modelNameTextBox;
        private System.Windows.Forms.ContextMenuStrip modelsBoxCtxMenu;
        private System.Windows.Forms.ToolStripMenuItem removeMenuItem;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveModelsToolStripMenuItem;
        private System.Windows.Forms.Button quickAddModelButton;
        private System.Windows.Forms.ToolStripMenuItem logToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem restartToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeAllUncheckedToolStripMenuItem;
    }
}

