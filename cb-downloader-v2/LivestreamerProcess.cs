using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace cb_downloader_v2
{
    internal class LivestreamerProcess
    {
        private static readonly Random Random = new Random();
        private const string StreamTerminatedMessage = "[cli][info] Stream ended";
        private const string StreamInvalidLinkMessagePart = "(404 Client Error: NOT FOUND)";
        private const string StreamOfflineMessagePart = "error: No streams found on this URL: ";
        private const string CommandArguments = "chaturbate.com/{0} {1} -o {2}";
        private const string FileNameTemplate = MainForm.OutputFolderName + "/{0}-{1}-{2}{3}.flv";
        private const string Quality = "best";
        private const int RestartDelay = MainForm.ListenerSleepDelay - 15000; // XXX validate value
        private readonly MainForm _mf;
        private readonly string _modelName;
        private CancellationTokenSource _cancelToken;
        private Process _process;

        public bool IsRunning
        {
            get
            {
                // TODO clean up
                try
                {
                    return _process != null && Started && !_process.HasExited;
                } catch (Exception) {
                    return false;
                }
            }
        }

        private bool Started { get; set; }
        public bool RestartRequired { get; private set; } = true;
        public bool InvalidUrlDetected { get; private set; }
        public int RestartTime { get; private set; }

        public LivestreamerProcess(MainForm parent, string modelName)
        {
            _mf = parent;
            _modelName = modelName;
        }

        public void Start(bool quickStart = false)
        {
            // Checking if a process is not already running
            if (_process != null)
                return;

            // Fetching file name
            _cancelToken = new CancellationTokenSource();
            string fileName = SeedFileName();

            // Create process
            _process = new Process
            {
                StartInfo =
                {
                    FileName = "livestreamer.exe",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    Arguments = string.Format(CommandArguments, _modelName, Quality, fileName)
                }
            };
            
            // Delayed start, a tad bit randomised, to prevent massive cpu spikes
            int delay = Random.Next(100, MainForm.ListenerSleepDelay/2);
            Task.Delay(quickStart ? 100 : delay, _cancelToken.Token).ContinueWith(task =>
            {
                // Cancel if process is null
                if (_process == null)
                    return;

                // Updating flags and starting process
                RestartRequired = false;
                InvalidUrlDetected = false;
                _process.OutputDataReceived += LivestreamerProcess_OutputDataReceived;
                _process.Start();
                _process.BeginOutputReadLine();
                Started = true;
                _mf.SetCheckState(_modelName, CheckState.Checked);
                Logger.Log(_modelName, "Started");
            }, _cancelToken.Token);
        }

        private string SeedFileName()
        {
            DateTime timeNow = MainForm.TimeNow;
            string date = timeNow.ToString("ddMMyy");
            string time = timeNow.ToString("hhmmss");
            string fileName = string.Format(FileNameTemplate, _modelName, date, time, "");

            // Get a file name which is not in use
            int no = 0;

            while (File.Exists(fileName))
            {
                fileName = string.Format(FileNameTemplate, _modelName, date, time, "-" + no++);
            }
            return fileName;
        }

        private void LivestreamerProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            string line = e.Data;

            // Checking line validity
            if (string.IsNullOrEmpty(line))
                return;
            
            Logger.Log(_modelName + "#RAW", line);

            // Checking if the stream was terminated
            if (line.Equals(StreamTerminatedMessage))
            {
                Logger.Log(_modelName, "Terminated");

                // Terminating the thread and marking for a restart
                RestartRequired = true;
                RestartTime = 0;
                Terminate();
            }

            // Checking if the username is invalid
            if (line.Contains(StreamInvalidLinkMessagePart))
            {
                Logger.Log(_modelName, "Not Found (404)");

                // Terminating the thread and marking as invalid url
                InvalidUrlDetected = true;
                RestartRequired = false;
                RestartTime = 0;
                Terminate();
            }

            // Checking if stream is offline
            if (line.StartsWith(StreamOfflineMessagePart))
            {
                Logger.Log(_modelName, "Offline");

                // Terminating the thread and marking for a delayed restart
                RestartRequired = true;
                RestartTime = Environment.TickCount + RestartDelay;
                Terminate();
            }
        }

        public void Terminate()
        {
            // Activate cancellation token
            if (_cancelToken != null && !_cancelToken.IsCancellationRequested)
            {
                _cancelToken.Cancel();
            }

            // Checking if process is existent
            if (_process == null)
                return;

            // Attempting to close process
            if (IsRunning)
            {
                _process.Kill();
                _process.Close();
            }
            _process = null;

            // Uncheck on models list in form
            _mf.SetCheckState(_modelName, CheckState.Unchecked);
        }
    }
}
