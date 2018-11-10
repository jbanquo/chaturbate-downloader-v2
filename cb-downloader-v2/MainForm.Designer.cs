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
            this.modelsCtxMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.removeCtxMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.restartCtxMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.removeAllDisconnectedCtxMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.modelsGroupBox = new System.Windows.Forms.GroupBox();
            this.modelsGrid = new System.Windows.Forms.DataGridView();
            this.modelNameTextBox = new System.Windows.Forms.TextBox();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveModelsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.quickAddModelButton = new System.Windows.Forms.Button();
            this.ModelName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ModelStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.modelsCtxMenu.SuspendLayout();
            this.modelsGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.modelsGrid)).BeginInit();
            this.menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // modelsCtxMenu
            // 
            this.modelsCtxMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.removeCtxMenuItem,
            this.restartCtxMenuItem,
            this.toolStripMenuItem2,
            this.removeAllDisconnectedCtxMenuItem});
            this.modelsCtxMenu.Name = "modelsBoxCtxMenu";
            this.modelsCtxMenu.ShowImageMargin = false;
            this.modelsCtxMenu.Size = new System.Drawing.Size(185, 76);
            this.modelsCtxMenu.Opening += new System.ComponentModel.CancelEventHandler(this.modelsBoxCtxMenu_Opening);
            // 
            // removeCtxMenuItem
            // 
            this.removeCtxMenuItem.Name = "removeCtxMenuItem";
            this.removeCtxMenuItem.Size = new System.Drawing.Size(184, 22);
            this.removeCtxMenuItem.Text = "Remove";
            this.removeCtxMenuItem.Click += new System.EventHandler(this.removeMenuItem_Click);
            // 
            // restartCtxMenuItem
            // 
            this.restartCtxMenuItem.Name = "restartCtxMenuItem";
            this.restartCtxMenuItem.Size = new System.Drawing.Size(184, 22);
            this.restartCtxMenuItem.Text = "Restart";
            this.restartCtxMenuItem.Click += new System.EventHandler(this.restartToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(181, 6);
            // 
            // removeAllDisconnectedCtxMenuItem
            // 
            this.removeAllDisconnectedCtxMenuItem.Name = "removeAllDisconnectedCtxMenuItem";
            this.removeAllDisconnectedCtxMenuItem.Size = new System.Drawing.Size(184, 22);
            this.removeAllDisconnectedCtxMenuItem.Text = "Remove All Disconnected";
            this.removeAllDisconnectedCtxMenuItem.Click += new System.EventHandler(this.removeAllUncheckedToolStripMenuItem_Click_1);
            // 
            // modelsGroupBox
            // 
            this.modelsGroupBox.Controls.Add(this.modelsGrid);
            this.modelsGroupBox.Location = new System.Drawing.Point(12, 27);
            this.modelsGroupBox.Name = "modelsGroupBox";
            this.modelsGroupBox.Size = new System.Drawing.Size(399, 292);
            this.modelsGroupBox.TabIndex = 1;
            this.modelsGroupBox.TabStop = false;
            this.modelsGroupBox.Text = "Models";
            // 
            // modelsGrid
            // 
            this.modelsGrid.AllowUserToAddRows = false;
            this.modelsGrid.AllowUserToDeleteRows = false;
            this.modelsGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.modelsGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ModelName,
            this.ModelStatus});
            this.modelsGrid.ContextMenuStrip = this.modelsCtxMenu;
            this.modelsGrid.Location = new System.Drawing.Point(6, 19);
            this.modelsGrid.Name = "modelsGrid";
            this.modelsGrid.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.modelsGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.modelsGrid.Size = new System.Drawing.Size(387, 267);
            this.modelsGrid.TabIndex = 0;
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
            this.logToolStripMenuItem,
            this.toolStripMenuItem1,
            this.settingsToolStripMenuItem});
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
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(186, 6);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.settingsToolStripMenuItem.Text = "Settings...";
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
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
            // ModelName
            // 
            this.ModelName.HeaderText = "Model";
            this.ModelName.Name = "ModelName";
            this.ModelName.ReadOnly = true;
            this.ModelName.Width = 200;
            // 
            // ModelStatus
            // 
            this.ModelStatus.HeaderText = "Status";
            this.ModelStatus.Name = "ModelStatus";
            this.ModelStatus.ReadOnly = true;
            this.ModelStatus.Width = 125;
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
            this.modelsCtxMenu.ResumeLayout(false);
            this.modelsGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.modelsGrid)).EndInit();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.GroupBox modelsGroupBox;
        private System.Windows.Forms.TextBox modelNameTextBox;
        private System.Windows.Forms.ContextMenuStrip modelsCtxMenu;
        private System.Windows.Forms.ToolStripMenuItem removeCtxMenuItem;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveModelsToolStripMenuItem;
        private System.Windows.Forms.Button quickAddModelButton;
        private System.Windows.Forms.ToolStripMenuItem logToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem restartCtxMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeAllDisconnectedCtxMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.DataGridView modelsGrid;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.DataGridViewTextBoxColumn ModelName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ModelStatus;
    }
}

