using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace cb_downloader_v2
{
    public partial class MainForm : Form
    {
        public static DateTime TimeNow = DateTime.Now;
        public const int ListenerSleepDelay = 60 * 1000;
        private const string ModelsFileName = "models.txt";
        public const string OutputFolderName = "Recordings";
        private DownloaderProcessManager _manager;
        // TODO fix issue where if you remove a model, it can still attempt to start it (something to do with task.delay/start pipeline i imagine)
        /// <summary>
        ///     Whether or not the modelsBox allows checking of items.
        /// </summary>
        private bool _checkingLocked = true;

        public MainForm()
        {
            InitializeComponent();
            PrepareOutputFolder();
            InitializeManager();
            LoadModelsFile();
            LoadModelNamesResourceFile();
            // TODO check if streamlink is installed/accessible
        }

        private async void LoadModelsFile()
        {
            // Checking if file exists
            if (!File.Exists(ModelsFileName))
                return;

            // Reading model file content
            StreamReader r = new StreamReader(ModelsFileName);
            string models = await r.ReadToEndAsync();
            r.Close();

            // Parsing lines, we filter out comments and empty lines
            foreach (string modelName in Regex.Split(models, "\r\n")
                .Where(modelName => modelName.Length != 0 && !modelName.StartsWith("#")))
            {
                _manager.AddModel(modelName);
            }
        }

        private void LoadModelNamesResourceFile()
        {
            string modelNames = EmbeddedResourceHelper.ReadText("cb_downloader_v2.models_list.txt");
            AutoCompleteStringCollection col = new AutoCompleteStringCollection();
            col.AddRange(Regex.Split(modelNames, "\r\n"));
            modelNameTextBox.AutoCompleteCustomSource = col;
        }

        private void InitializeManager()
        {
            _manager = new DownloaderProcessManager(this, modelsBox);
            _manager.Start();
        }

        public void RemoveInvalidUrlModel(string modelName, LivestreamerProcess proc)
        {
            // Telling user URL was invalid
            Invoke((MethodInvoker)(() => MessageBox.Show(this, "Unregistered model detected: " + modelName, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)));

            // Removing from listeners
            _manager.RemoveModel(modelName);

            // Removing from UI
            modelsBox.Invoke((MethodInvoker)(() => modelsBox.Items.Remove(modelName)));
        }

        private void PrepareOutputFolder()
        {
            try
            {
                // Creating recordings folder if non-existent
                if (!Directory.Exists(OutputFolderName))
                {
                    Directory.CreateDirectory(OutputFolderName);
                }
            }
            catch (Exception e)
            {
                // We exit since an issue occured when creating the folder
                if (MessageBox.Show(this, $"Error creating output folder: {e.Message}\r\nApplication will now exit.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK)
                {
                    Application.Exit();
                }
            }
        }

        private void quickAddModelButton_Click(object sender, EventArgs e)
        {
            // Adding user to listener and start immediately
            _manager.AddModel(modelNameTextBox.Text, true);
            modelNameTextBox.Text = "";
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Asking user to confirm action
            if (_manager.Count > 0 && MessageBox.Show(this, "Are you sure you want to quit, all active streams being listened to will be terminated.",
                "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.Cancel)
            {
                e.Cancel = true;
                return;
            }

            // Terminating manager + download listeners
            _manager.Stop();
        }

        private void modelNameTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            // Handling textbox enter key press
            if (e.KeyCode != Keys.Enter)
                return;
            _manager.AddModel(modelNameTextBox.Text, true);
            modelNameTextBox.Text = "";
            e.Handled = e.SuppressKeyPress = true;
        }

        private void modelNameTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            // Suppressing beeping sound for text box enter press
            if (e.KeyCode == Keys.Enter && modelNameTextBox.Text.Length > 0)
            {
                e.Handled = e.SuppressKeyPress = true;
            }
        }


        /// <summary>
        ///     This uses the _checkingLocked variable to not allow the property to affect it.
        /// </summary>
        /// <param name="modelName"></param>
        /// <param name="state"></param>
        public void SetCheckState(string modelName, CheckState state)
        {
            _checkingLocked = false;

            // XXX could use binary search to speed up - since models list is sorted alphabetically
            for (int i = 0; i < modelsBox.Items.Count; i++)
            {
                var item = modelsBox.Items[i];

                // Locating item with given name
                if (!item.Equals(modelName))
                    continue;
                modelsBox.Invoke((MethodInvoker)(() => modelsBox.SetItemCheckState(i, state)));
                _checkingLocked = true;
                return;
            }
        }

        private void removeMenuItem_Click(object sender, EventArgs e)
        {
            int idx = modelsBox.SelectedIndex;

            // Validating item
            if (idx == -1)
                return;

            string modelName = modelsBox.Items[idx].ToString();

            // Fetching process
            IDownloaderProcess listener = _manager[modelName];

            // Initiating termination
            listener.Terminate();

            // Removing listener from lists
            modelsBox.Items.RemoveAt(idx);
            _manager.RemoveModel(modelName);
            Logger.Log(modelName, "Remove");
        }

        private void saveModelsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Prepare dialog
            SaveFileDialog dialog = new SaveFileDialog
            {
                AddExtension = true,
                DefaultExt = ".txt",
                Filter = "Text File|*.txt|All Files|*.*",
                FileName = "models"
            };

            // Show dialog and save if chosen to
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                // Construct file content
                string fileContent = modelsBox.Items.Cast<object>().Aggregate("", (current, item) => current + (item + "\r\n"));

                // Attempt to save content
                string fileName = dialog.FileName;

                try
                {
                    // Writing new file
                    File.WriteAllText(fileName, fileContent);
                }
                catch (Exception)
                {
                    MessageBox.Show(this, "Error saving file to: " + fileName, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void modelsBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            // Disabling user checking
            if (_checkingLocked)
            {
                e.NewValue = e.CurrentValue;
            }
        }

        private void logToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Toggle log to file
            logToolStripMenuItem.Checked = !logToolStripMenuItem.Checked;
            Logger.LogToFile = logToolStripMenuItem.Checked;
        }

        private void restartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string modelName = GetSelectedModelName();

            if (modelName == null)
                return;

            // Fetching process
            IDownloaderProcess listener = _manager[modelName];

            // Cancel restart if the listener is already running
            if (listener.IsRunning())
                return;

            // Otherwise, continue with the manual start
            listener.Start(true);
            Logger.Log(modelName, "Manual restart");
        }

        private void removeAllUncheckedToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            // Gather IDs
            ArrayList modelIds = new ArrayList();

            for (int i = 0; i < modelsBox.Items.Count; i++)
            {
                if (modelsBox.GetItemCheckState(i) == CheckState.Unchecked)
                {
                    modelIds.Add(i);
                }
            }

            // Reverse so we start at the end, or as we remove elements the indexes will be incorrect
            modelIds.Reverse();

            // Batch remove
            foreach (int id in modelIds)
            {
                string modelName = modelsBox.Items[id].ToString();

                // Fetching process
                IDownloaderProcess listener = _manager[modelName];

                // Initiating termination
                listener.Terminate();

                // Removing listener from lists
                modelsBox.Items.RemoveAt(id);
                _manager.RemoveModel(modelName);
                Logger.Log(modelName, "Remove all unchecked");
            }
        }

        private string GetSelectedModelName()
        {
            int idx = modelsBox.SelectedIndex;

            // Validating index
            if (idx == -1)
                return null;

            return modelsBox.Items[idx].ToString();
        }

        private void modelsBoxCtxMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            int idx = modelsBox.SelectedIndex;

            //  Disable element if selectedindex=-1
            removeMenuItem.Enabled = idx != -1;

            //  Disable element if selectedindex=-1 not checked
            restartToolStripMenuItem.Enabled = idx != -1
                && modelsBox.GetItemCheckState(idx) == CheckState.Unchecked;

            // if there is more than 0 unchecked models
            removeAllUncheckedToolStripMenuItem.Enabled = ModelsStateCount(CheckState.Unchecked) > 0;
        }

        /// <summary>
        ///     Counts the amount of models in the modelsBox with the given
        ///     CheckState.
        /// </summary>
        /// <param name="target">The checkstate to count for.</param>
        /// <returns></returns>
        private int ModelsStateCount(CheckState target)
        {
            int count = 0;

            for (int i = 0; i < modelsBox.Items.Count; i++)
            {
                if (modelsBox.GetItemCheckState(i) == target)
                    count++;
            }
            return count;
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SettingsForm form = new SettingsForm();
            form.ShowDialog(this);
            form.Dispose();
        }
    }
}
