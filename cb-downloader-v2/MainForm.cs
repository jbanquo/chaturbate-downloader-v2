using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace cb_downloader_v2
{
    public partial class MainForm : Form
    {
        public static DateTime TimeNow;
        public const int ListenerSleepDelay = 60 * 1000;
        private const string ModelsFileName = "models.txt";
        public const string OutputFolderName = "Recordings";
        private readonly ConcurrentDictionary<string, LivestreamerProcess> _listeners = new ConcurrentDictionary<string, LivestreamerProcess>();
        private Thread _listenerThread;
        /// <summary>
        ///     Whether or not the modelsBox allows checking of items.
        /// </summary>
        private bool _checkingLocked = true;

        public MainForm()
        {
            InitializeComponent();
            PrepareOutputFolder();
            InitializeListener();
            LoadModelsFile();
            // TODO check if livestreamer is installed/accessible
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
            foreach (string modelName in Regex.Split(models, "\r\n").Where(modelName => modelName.Length != 0 && !modelName.StartsWith("#")))
            {
                AddUser(modelName);
            }
        }

        private void AddUser(string modelName, bool quickStart = false)
        {
            // Normalising name
            modelName = modelName.ToLower();

            // Checking input validity
            if (string.IsNullOrWhiteSpace(modelName))
            {
//                MessageBox.Show(this, "Invalid model name, cannot be empty or whitespace.", "Error",  MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

#if DEBUG
            Debug.WriteLine("AddUser: " + modelName);
#endif

            // Checking if the model is already being listened to
            if (modelsBox.Items.Cast<object>().Contains(modelName))
                return;

            // Create process and add listener to lists
            LivestreamerProcess proc = new LivestreamerProcess(this, modelName);
            modelsBox.Items.Add(modelName);
            _listeners.AddOrUpdate(modelName, proc, (s, listener) => listener);

            // Quick start functionality (i.e. start listener immediately)
            // XXX this may cause issues
            if (quickStart)
                proc.Start();
        }

        private void InitializeListener()
        {
            _listenerThread = new Thread(ListenerProcess);
            _listenerThread.Start();
        }

        private void ListenerProcess()
        {
            while (true)
            {
                Thread.Sleep(ListenerSleepDelay);

                // Skip iteration if no listeners are attached
                if (_listeners.Count == 0)
                    continue;

                // Update cached time
                TimeNow = DateTime.Now;

                // Handling each listener
                foreach (KeyValuePair<string, LivestreamerProcess> valuePair in _listeners)
                {
                    string modelName = valuePair.Key;
                    LivestreamerProcess proc = valuePair.Value;

                    // Checking if a (re)start is required
                    if (proc.RestartRequired && !proc.IsRunning && Environment.TickCount > proc.RestartTime)
                    {
                        proc.Start();
                    }

                    // Checking if URL was invalid
                    if (proc.InvalidUrlDetected)
                    {
                        // Telling user URL was invalid
                        Invoke((MethodInvoker)(() => MessageBox.Show(this, "Unregistered model detected: " + modelName, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)));

                        // Removing from listeners
                        LivestreamerProcess output;
                        _listeners.TryRemove(valuePair.Key, out output);

                        // Removing from UI
                        modelsBox.Invoke((MethodInvoker)(() => modelsBox.Items.Remove(modelName)));
                    }
                }
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

        private void addModelButton_Click(object sender, EventArgs e)
        {
            // Adding user to listener
            AddUser(modelNameTextBox.Text);
            modelNameTextBox.Text = "";
        }

        private void quickAddModelButton_Click(object sender, EventArgs e)
        {
            // Adding user to listener and start immediately
            AddUser(modelNameTextBox.Text, true);
            modelNameTextBox.Text = "";
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Asking user to confirm action
            if (_listeners.Count > 0 && MessageBox.Show(this, "Are you sure you want to quit, all active streams being listened to will be terminated.",
                "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.Cancel)
            {
                e.Cancel = true;
                return;
            }

            // Terminating manager thread
            if (_listenerThread.IsAlive)
            {
                try
                {
                    _listenerThread.Abort();
                }
                catch (ThreadAbortException)
                {
                }
            }

            // Terminating operational threads/process
            foreach (KeyValuePair<string, LivestreamerProcess> valuePair in _listeners)
            {
                // Fetching listener
                string modelName = valuePair.Key;
                LivestreamerProcess listener = valuePair.Value;

                // Initiating termination
                listener.Terminate();

                // Removing listener from list
                LivestreamerProcess output;
                _listeners.TryRemove(modelName, out output);
            }
        }

        private void modelNameTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            // Handling textbox enter key press
            if (e.KeyCode != Keys.Enter)
                return;
            AddUser(modelNameTextBox.Text);
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
            LivestreamerProcess listener = _listeners[modelName];

            // Initiating termination
            listener.Terminate();

            // Removing listener from lists
            modelsBox.Items.RemoveAt(idx);
            LivestreamerProcess output;
            _listeners.TryRemove(modelName, out output);
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
    }
}
