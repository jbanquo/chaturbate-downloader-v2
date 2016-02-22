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
        // TODO play with checkbox stuff until it makes sense - right now its always checked due to how it flows, after initial start of process
        // TODO dont let user check/uncheck the checkboxes
        private const int ListenerSleepDelay = 5 * 1000;
        private const string ModelsFileName = "models.txt";
        public const string OutputFolderName = "Recordings";
        private readonly ConcurrentDictionary<string, LivestreamerProcess> _listeners = new ConcurrentDictionary<string, LivestreamerProcess>();
        private Thread _listenerThread;

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

            // Parsing lines, we filter out comments and empty lines
            foreach (string modelName in Regex.Split(models, "\r\n").Where(modelName => modelName.Length != 0 && !modelName.StartsWith("#")))
            {
                AddUser(modelName);
            }
        }

        private void AddUser(string modelName)
        {
            // Normalising name
            modelName = modelName.ToLower();

            // Checking input validity
            if (string.IsNullOrWhiteSpace(modelName))
            {
                MessageBox.Show(this, "Invalid model name, cannot be empty or whitespace.", "Error",  MessageBoxButtons.OK, MessageBoxIcon.Information);
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

                // Handling each listener
                foreach (KeyValuePair<string, LivestreamerProcess> valuePair in _listeners)
                {
                    string modelName = valuePair.Key;
                    LivestreamerProcess proc = valuePair.Value;

                    // Checking if a (re)start is required
                    if (Environment.TickCount > proc.RestartTime && !proc.IsRunning && proc.RestartRequired)
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

        public void SetCheckState(string modelName, CheckState state)
        {
            for (int i = 0; i < modelsBox.Items.Count; i++)
            {
                var item = modelsBox.Items[i];

                // Locating item with given name
                if (!item.Equals(modelName))
                    continue;
                modelsBox.Invoke((MethodInvoker)(() => modelsBox.SetItemCheckState(i, state)));
                return;
            }
        }
    }
}
