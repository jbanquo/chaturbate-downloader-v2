using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace cb_downloader_v2
{
    public partial class MainForm : Form
    {
        private const string ModelsFileName = "models.txt";
        private const string OutputFolderName = "Recordings";

        public MainForm()
        {
            InitializeComponent();
            InitializeListener();
            LoadModelsFile();
            PrepareOutputFolder();
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
            // Debug output
#if DEBUG
            Debug.WriteLine("AddUser: " + modelName);
#endif
        }

        private void InitializeListener()
        {
            // TODO impl
        }

        private void PrepareOutputFolder()
        {
            // Creating recordings folder if non-existant
            try
            {
                if (!Directory.Exists(OutputFolderName))
                {
                    Directory.CreateDirectory(OutputFolderName);
                }
            }
            catch (UnauthorizedAccessException e)
            {
                if (MessageBox.Show(this, "Error creating output folder: " + e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK)
                {
                    Application.Exit();
                }
            }
        }

        private void addModelButton_Click(object sender, EventArgs e)
        {
            // Checking if input is valid
            if (string.IsNullOrEmpty(modelNameTextBox.Text))
                return;

            // Adding user to listener
            AddUser(modelNameTextBox.Text);
            modelNameTextBox.Text = "";
        }
    }
}
