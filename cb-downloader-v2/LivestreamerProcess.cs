using System;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace cb_downloader_v2
{
    public class LivestreamerProcess : IDownloaderProcess
    {
        // NOTE: issues occur when, i.e. you close the app and THEN a stream begins being listened to (i.e. this is not terminated), etc.
        private static readonly Random Random = new Random();
        private const string StreamTerminatedMessage = "[cli][info] Stream ended";
        private const string StreamServiceUnavailablePart = "503 Server Error: Service Temporarily Unavailable";
        private const string StreamOfflineMessagePart = "error: No streams found on this URL: ";
        private const string CommandArguments = "chaturbate.com/{0} {1} -o {2}";
        private const string FileNameTemplate = MainForm.OutputFolderName + "/{0}-{1}-{2}{3}.flv";
        private const string Quality = "best";
        private const int RestartDelay = MainForm.ListenerSleepDelay - 15000; // XXX validate value
        private readonly MainForm _mf;
        private readonly string _modelName;
        private CancellationTokenSource _cancelToken;
        private Process _process;
        private bool Running { get; set; }
        public bool RestartRequired { get; private set; } = true;
        public int RestartTime { get; private set; }
        private int StandardRestartDelay => Environment.TickCount + RestartDelay;

        private readonly string _streamInvalidUsernameMessage;
        private readonly string _streamReadTimeoutMessage;

        public LivestreamerProcess(MainForm parent, string modelName)
        {
            _mf = parent;
            _modelName = modelName;
            _streamInvalidUsernameMessage =
                "error: Unable to open URL: http://chaturbate.com/" + _modelName + " (404 Client Error: Not Found)";
            _streamReadTimeoutMessage =
                "error: Unable to open URL: http://chaturbate.com/" + _modelName + " (HTTPSConnectionPool(host='chaturbate.com', port=443): Read timed out.)";
        }

        public void Start(bool quickStart = false)
        {
            // Checking if a process is already running
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
                    FileName = Properties.Settings.Default.StreamlinkExecutable,
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
                // Cancel if process is null or cancellation is requested
                if (_process == null | _cancelToken.IsCancellationRequested)
                    return;

                // Updating flags and starting process
                RestartRequired = false;
                _process.OutputDataReceived += LivestreamerProcess_OutputDataReceived;
                _process.Start();
                _process.BeginOutputReadLine();
                Running = true;
                _mf.SetCheckState(_modelName, CheckState.Checked);
                Logger.Log(_modelName, "Started");

                // Check if cancellation was called
                if (_cancelToken.IsCancellationRequested)
                {
                    Terminate();
                }
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

            // Checking if data is valid
            if (line == null) {
				Logger.Log(_modelName, "End of stream");
                Terminate(true); // custom errors are usually fired before this, so this becomes ignored
                return;
			}
			
			if (line.Length == 0)
				return;
            
            // Parsing line
            Logger.Log(_modelName, "[raw]" + line);

            // Checking if the stream was terminated server-side
            if (line.Equals(StreamTerminatedMessage))
            {
                Logger.Log(_modelName, "Terminated");
                Terminate(true);
            }

            // Checking if service is offline (i.e. being ddosed)
            if (line.Contains(StreamServiceUnavailablePart))
            {
                Logger.Log(_modelName, "Service Unavailable");
                Terminate(true, StandardRestartDelay);
            }

            // Checking if the username is invalid
            if (line.Contains(_streamInvalidUsernameMessage))
            {
                Logger.Log(_modelName, "Invalid username (404)");

                // Terminating the thread and marking it as an invalid url
                Terminate();

                // Removing model from GUI
                _mf.RemoveInvalidUrlModel(_modelName, this);
            }

            // Checking if stream is offline
            if (line.StartsWith(StreamOfflineMessagePart) || line.Contains(_streamReadTimeoutMessage))
            {
                Logger.Log(_modelName, "Offline");
                Terminate(true, StandardRestartDelay);
            }
        }

        public void Terminate()
        {
            Terminate(false);
        }
        
        public void Terminate(bool restartRequired, int restartTime = 0)
        {
            // Checking if process is existent/running
            if (_process == null || !Running)
                return;

            // Update flags
            RestartRequired = restartRequired;
            RestartTime = restartTime;

            // Activate cancellation token
            if (_cancelToken != null && !_cancelToken.IsCancellationRequested)
            {
                _cancelToken.Cancel();
            }

            // Attempting to close process
            try
            {
                if (_process != null)
                {
                    try
                    {
                        KillProcessTree(_process.Id);
                    }
                    catch
                    {
                        // ignored
                    }
                    finally
                    {
                        _process.Close();
                        _process = null;
                    }
                }
            }
            catch
            {
                // ignored
            }

            // Uncheck on models list in form
            _mf.SetCheckState(_modelName, CheckState.Unchecked);
            Running = false;
        }

        /// <summary>
        ///     Kill a process, and all of its children, grandchildren, etc.
        ///     Source: https://stackoverflow.com/questions/5901679/kill-process-tree-programatically-in-c-sharp?answertab=active#tab-top
        /// </summary>
        /// <param name="pid">Process ID.</param>
        private static void KillProcessTree(int pid)
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher
                ("Select * From Win32_Process Where ParentProcessID=" + pid);
            ManagementObjectCollection moc = searcher.Get();

            foreach (ManagementObject mo in moc)
            {
                KillProcessTree(Convert.ToInt32(mo["ProcessID"]));
            }

            try
            {
                Process proc = Process.GetProcessById(pid);
                proc.Kill();
            }
            catch (ArgumentException)
            {
                // Process already exited.
            }
        }

        public bool CanRestart()
        {
            return RestartRequired && !Running && Environment.TickCount > RestartTime;
        }

        public bool IsRunning()
        {
            return Running;
        }
    }
}
