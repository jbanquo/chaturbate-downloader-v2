using System;
using System.Diagnostics;
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
        private const string CommandArguments = "chaturbate.com/{0} {1} -o " + MainForm.OutputFolderName + "/{0}-{2}-{3}.flv";
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

        public void Start()
        {
            // Checking if a process is not already running
            if (_process != null)
                return;

            // Initialising process
            DateTime timeNow = MainForm.TimeNow;
            _cancelToken = new CancellationTokenSource();
            // XXX still need to improve file clash mechanism?

            _process = new Process
            {
                StartInfo =
                {
                    FileName = "livestreamer.exe",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    Arguments = string.Format(CommandArguments, _modelName, Quality, timeNow.ToString("ddMMyy"), timeNow.ToString("hhmmss"))
                }
            };
            
            // Delayed start, a tad bit randomised, to prevent massive cpu spikes
            Task.Delay(Random.Next(100, MainForm.ListenerSleepDelay / 2), _cancelToken.Token).ContinueWith(task =>
            {
                // Cancel if process is null
                if (_process == null)
                    return;

#if DEBUG
                Debug.WriteLine("Started #" + _modelName);
#endif

                // Updating flags and starting process
                RestartRequired = false;
                InvalidUrlDetected = false;
                _process.OutputDataReceived += _process_OutputDataReceived;
                _process.Start();
                _process.BeginOutputReadLine();
                Started = true;
                _mf.SetCheckState(_modelName, CheckState.Checked);
            }, _cancelToken.Token);
        }

        private void _process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            string line = e.Data;

            // Checking line validity
            if (string.IsNullOrEmpty(line))
                return;

            // Checking if the stream was terminated
            if (line.Equals(StreamTerminatedMessage))
            {
#if DEBUG
                Debug.WriteLine("[" + _modelName + "] Terminated");
#endif

                // Terminating the thread and marking for a restart
                RestartRequired = true;
                RestartTime = 0;
                Terminate();
            }

            // Checking if the username is invalid
            if (line.Contains(StreamInvalidLinkMessagePart))
            {
#if DEBUG
                Debug.WriteLine("[" + _modelName + "] Not Found (404)");
#endif

                // Terminating the thread and marking as invalid url
                InvalidUrlDetected = true;
                RestartRequired = false;
                RestartTime = 0;
                Terminate();
            }

            // Checking if stream is offline
            if (line.StartsWith(StreamOfflineMessagePart))
            {
#if DEBUG
                Debug.WriteLine("[" + _modelName + "] is Offline");
#endif

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
            _mf.SetCheckState(_modelName, CheckState.Unchecked);
        }
    }
}
