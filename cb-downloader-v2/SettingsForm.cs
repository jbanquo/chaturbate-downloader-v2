using System;
using System.Windows.Forms;

namespace cb_downloader_v2
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();

            // Set streamlink executable
            streamlinkExecutableTextBox.Text = Properties.Settings.Default.StreamlinkExecutable;
        }

        private void browseStreamlinkExecutableButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                CheckFileExists = true
            };

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                streamlinkExecutableTextBox.Text = dialog.FileName;
            }
            dialog.Dispose();
        }

        private void doneButton_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.StreamlinkExecutable = streamlinkExecutableTextBox.Text;
            Properties.Settings.Default.Save();
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
