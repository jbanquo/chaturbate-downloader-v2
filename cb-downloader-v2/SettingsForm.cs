using System;
using System.Windows.Forms;

namespace cb_downloader_v2
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            // Set streamlink executable
            streamlinkExecutableTextBox.Text = Properties.Settings.Default.StreamlinkExecutable;

            // Set HTTP proxy details
            useHttpProxyCheckBox.Checked = Properties.Settings.Default.UseHttpProxy;
            httpProxyTextBox.Text = Properties.Settings.Default.HttpProxyUrl;

            // Set HTTPS proxy details
            useHttpsProxyCheckBox.Checked = Properties.Settings.Default.UseHttpsProxy;
            httpsProxyTextBox.Text = Properties.Settings.Default.HttpsProxyUrl;
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
            // Set streamlink executable
            Properties.Settings.Default.StreamlinkExecutable = streamlinkExecutableTextBox.Text;

            // Set HTTP proxy details
            Properties.Settings.Default.UseHttpProxy = useHttpProxyCheckBox.Checked;
            Properties.Settings.Default.HttpProxyUrl = httpProxyTextBox.Text;

            // Set HTTPS proxy details
            Properties.Settings.Default.UseHttpsProxy = useHttpsProxyCheckBox.Checked;
            Properties.Settings.Default.HttpsProxyUrl = httpsProxyTextBox.Text;
            Properties.Settings.Default.Save();
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
