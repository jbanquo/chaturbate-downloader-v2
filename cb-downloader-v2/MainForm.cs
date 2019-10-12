using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using cb_downloader_v2.Utils;
using log4net;

namespace cb_downloader_v2
{
    public partial class MainForm : Form
    {
        public static readonly string OutputFolderName = "Recordings";
        private static readonly ILog Log = LogManager.GetLogger(typeof(MainForm));
        private static readonly string ModelsFileName = "models.txt";
        private IDownloaderProcessManager _manager;
        private ModelsGridWrapper models;
        // TODO fix issue where if you remove a model, it can still attempt to start it (something to do with task.delay/start pipeline i imagine)

        public MainForm()
        {
            InitializeComponent();
            models = new ModelsGridWrapper(modelsGrid);
            models.SortByModelNameAscending();

            PrepareOutputFolder();
            InitializeManager();
            LoadModelsFile();
            CheckStreamlinkInstall();
            LoadModelNamesResourceFile();
            Log.Info("Test");
        }

        private void CheckStreamlinkInstall()
        {
            if (!FileHelper.IsFileAccessible(Properties.Settings.Default.StreamlinkExecutable))
            {
                var binary = Properties.Settings.Default.StreamlinkExecutable;
                Log.Error($"Streamlink binary not found: {binary}");
                MessageBox.Show(this, $"'{binary}' inaccessible, the application may fail to work properly.\r\n" +
                                      "Please ensure it is either in the current working directory, or in your system path variable.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void LoadModelsFile()
        {
            // Checking if file exists
            if (!File.Exists(ModelsFileName))
                return;

            // Reading model file content
            var r = new StreamReader(ModelsFileName);
            var modelNames = await r.ReadToEndAsync();
            r.Close();

            // Parsing lines, we filter out comments and empty lines
            foreach (var modelName in Regex.Split(modelNames, Environment.NewLine)
                .Select(line => line.Trim())
                .Where(modelName => modelName.Length != 0 && !modelName.StartsWith("#")))
            {
                _manager.AddModel(modelName);
            }
        }

        private void LoadModelNamesResourceFile()
        {
            var modelNames = EmbeddedResourceHelper.ReadText("cb_downloader_v2.models_list.txt");
            var col = new AutoCompleteStringCollection();
            col.AddRange(Regex.Split(modelNames, Environment.NewLine));
            modelNameTextBox.AutoCompleteCustomSource = col;
        }

        private void InitializeManager()
        {
            _manager = new DownloaderProcessManager(this, models);
            _manager.Start();
        }

        public void RemoveInvalidUrlModel(string modelName)
        {
            // Telling user URL was invalid
            Log.Warn($"Invalid model: {modelName}");
            Invoke((MethodInvoker)(() => MessageBox.Show(this, $"Unregistered model detected: {modelName}",
                "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)));

            // Removing from listeners
            if (_manager.RemoveModel(modelName))
            {
                // Removing from UI
                models.RemoveModel(modelName);
            }
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

        internal void SetStatus(string modelName, Status status)
        {
            models.SetStatus(modelName, status);
        }

        private void removeMenuItem_Click(object sender, EventArgs e)
        {
            foreach (var modelName in models.SelectedModelNames)
            {
                // Terminating process
                var listener = _manager[modelName];
                listener.Terminate();

                // Removing listener from collections
                if (_manager.RemoveModel(modelName))
                {
                    models.RemoveModel(modelName);
                    Log.Debug($"{modelName}: Remove");
                }
                else
                {
                    Log.Debug($"{modelName}: Remove failed");
                }
            }
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
                var fileContent = models.CreateModelsFileContent();

                // Attempt to save content
                var fileName = dialog.FileName;

                try
                {
                    // Writing new file
                    File.WriteAllText(fileName, fileContent);
                }
                catch (Exception)
                {
                    MessageBox.Show(this, $"Error saving file to: {fileName}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void restartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (var modelName in models.SelectedModelNames)
            {
                // Fetching process
                var listener = _manager[modelName];

                // Cancel restart if the listener is already running
                if (listener.Status != Status.Disconnected)
                    return;

                // Otherwise, continue with the manual start
                listener.Start(true);
                Log.Debug($"{modelName}: Manual restart");
            }
        }

        private void removeAllUncheckedToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Log.Debug($"Remove all disconnected");

            // Gather IDs
            var modelNames = models.ModelsWithStatus(Status.Disconnected);

            // Batch remove
            foreach (var modelName in modelNames)
            {
                // Fetching process
                var listener = _manager[modelName];

                // Initiating termination
                listener.Terminate();

                // Removing listener from lists
                if (_manager.RemoveModel(modelName))
                {
                    models.RemoveModel(modelName);
                    Log.Debug($"{modelName}: Remove all disconnected");
                }
            }
        }

        private void modelsBoxCtxMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // TODO inspect
            var idx = models.SelectedIndex;

            //  Disable element if selectedindex=-1
            removeCtxMenuItem.Enabled = idx != null;

            //  Disable element if selectedindex=-1 not checked
            restartCtxMenuItem.Enabled = idx != null
                && models.GetStatus(idx.Value) == Status.Disconnected;

            // if there is more than 0 unchecked models
            removeAllDisconnectedCtxMenuItem.Enabled = models.ModelsWithStatus(Status.Disconnected).Count > 0;
        }
        
        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SettingsForm form = new SettingsForm();
            form.ShowDialog(this);
            form.Dispose();
        }
    }
}
