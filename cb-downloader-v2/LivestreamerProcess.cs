using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using cb_downloader_v2.Utils;

namespace cb_downloader_v2
{
    public class LivestreamerProcess : IDownloaderProcess
    {
        // NOTE: issues occur when, i.e. you close the app and THEN a stream begins being listened to (i.e. this is not terminated), etc.
        private static readonly Random Random = new Random();
        private static string StreamTerminatedMessage = "[cli][info] Stream ended";
        private static string StreamServiceUnavailablePart = "503 Server Error: Service Temporarily Unavailable";
        private static string StreamOfflineMessagePart = "error: No streams found on this URL: ";
        private static string CommandArguments = "chaturbate.com/{0} {1} -o {2}";
        private static string FileNameTemplate = MainForm.OutputFolderName + "/{0}-{1}-{2}{3}.flv";
        private static string DefaultQuality = "best";
        private static TimeSpan RestartDelay = TimeSpan.FromSeconds(5);
        private readonly MainForm _mf;
        private CancellationTokenSource _cancelToken;
        private Process _process;
        public string ModelName { get; }
        private bool Running { get; set; }
        public bool RestartRequired { get; private set; } = true;
        public DateTimeOffset RestartTime { get; private set; }
        private DateTimeOffset StandardRestartDelay => DateTimeOffset.UtcNow + RestartDelay;

        private readonly string _streamInvalidUsernameMessage;
        private readonly string _streamReadTimeoutMessage;

        public LivestreamerProcess(MainForm parent, string modelName)
        {
            ModelName = modelName;
            _mf = parent;
            _streamInvalidUsernameMessage =
                "error: Unable to open URL: http://chaturbate.com/" + ModelName + " (404 Client Error: Not Found)";
            _streamReadTimeoutMessage =
                "error: Unable to open URL: http://chaturbate.com/" + ModelName + " (HTTPSConnectionPool(host='chaturbate.com', port=443): Read timed out.)";
        }

        public void Start(bool quickStart = false)
        {
            // Delayed start, a tad bit randomised, to prevent massive cpu spikes
            var delay = quickStart ? 100 : Random.Next(1000, 10000);
            Start(delay);
        }

        public void Start(int delay)
        {
            // Checking if a process is already running
            if (_process != null)
                return;

            _mf.SetStatus(ModelName, Status.Connecting);
            Logger.Log(ModelName, $"Connecting in {delay}ms...");

            // Fetching file name
            _cancelToken = new CancellationTokenSource();
            var fileName = SeedFileName();

            Task.Delay(delay, _cancelToken.Token).ContinueWith(task =>
            {
                // Cancel if process cancellation is requested
                if (_cancelToken.IsCancellationRequested)
                    return;

                // Create process
                var quality = QualityOptions();
                // TODO if stream does not exist, remove model
                var selectedQuality = SelectQuality(quality, Properties.Settings.Default.TargetQuality);
                
                _process = new Process
                {
                    StartInfo =
                    {
                        FileName = Properties.Settings.Default.StreamlinkExecutable,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardOutput = true,
                        Arguments = GenerateArguments(fileName, selectedQuality)
                    }
                };

                // Updating flags and starting process
                RestartRequired = false;
                _process.OutputDataReceived += LivestreamerProcess_OutputDataReceived;
                _process.Start();
                _process.BeginOutputReadLine();
                Running = true;
                _mf.SetStatus(ModelName, Status.Connected);
                Logger.Log(ModelName, "Started");

                // Check if cancellation was called
                if (_cancelToken.IsCancellationRequested)
                {
                    Terminate();
                }
            }, _cancelToken.Token);
        }

        private string SelectQuality(List<int> qualities, int targetQuality)
        {
            if (qualities == null)
                return DefaultQuality;

            foreach (var q in qualities)
            {
                if (q >= targetQuality)
                {
                    return q + "p";
                }
            }
            return qualities.Last() + "p";
        }

        private string GenerateArguments(string fileName, string quality)
        {
            string args = string.Format(CommandArguments, ModelName, quality, fileName);

            // HTTP proxy settings
            if (Properties.Settings.Default.UseHttpProxy)
            {
                args += $" --http-proxy \"{Properties.Settings.Default.HttpProxyUrl}\"";
            }

            // HTTPS proxy settings
            if (Properties.Settings.Default.UseHttpsProxy)
            {
                args += $" --https-proxy \"{Properties.Settings.Default.HttpsProxyUrl}\"";
            }
            return args;
        }

        private string SeedFileName()
        {
            DateTime timeNow = DateTime.Now;
            string date = timeNow.ToString("ddMMyy");
            string time = timeNow.ToString("hhmmss");
            string fileName = string.Format(FileNameTemplate, ModelName, date, time, "");

            // Get a file name which is not in use
            int no = 0;

            while (File.Exists(fileName))
            {
                fileName = string.Format(FileNameTemplate, ModelName, date, time, "-" + no++);
            }
            return fileName;
        }

        private void LivestreamerProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            string line = e.Data;

            // Checking if data is valid
            if (line == null) {
				Logger.Log(ModelName, "End of stream");
                Terminate(true); // custom errors are usually fired before this, so this becomes ignored
                return;
			}
			
			if (line.Length == 0)
				return;
            
            // Parsing line
            Logger.Log(ModelName, "[raw]" + line);

            // Checking if the stream was terminated server-side
            if (line.Equals(StreamTerminatedMessage))
            {
                Logger.Log(ModelName, "Terminated");
                Terminate(true);
            }

            // Checking if service is offline (i.e. being ddosed)
            if (line.Contains(StreamServiceUnavailablePart))
            {
                Logger.Log(ModelName, "Service Unavailable");
                Terminate(true, StandardRestartDelay);
            }

            // Checking if the username is invalid
            if (line.Contains(_streamInvalidUsernameMessage))
            {
                Logger.Log(ModelName, "Invalid username (404)");

                // Terminating the thread and marking it as an invalid url
                Terminate();

                // Removing model from GUI
                _mf.RemoveInvalidUrlModel(ModelName);
            }

            // Checking if stream is offline
            if (line.StartsWith(StreamOfflineMessagePart) || line.Contains(_streamReadTimeoutMessage))
            {
                Logger.Log(ModelName, "Offline");
                Terminate(true, StandardRestartDelay);
            }
        }

        public void Terminate()
        {
            Terminate(false);
        }
        
        public void Terminate(bool restartRequired, DateTimeOffset? restartTime = null)
        {
            // Checking if process is existent/running
            if (_process == null || !Running)
                return;

            // Update flags
            RestartRequired = restartRequired;
            RestartTime = restartTime ?? DateTimeOffset.UtcNow;

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
            _mf.SetStatus(ModelName, Status.Disconnected);
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

        private List<int> QualityOptions()
        {
            // Run process
            var process = new Process
            {
                StartInfo =
                {
                    FileName = Properties.Settings.Default.StreamlinkExecutable,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    Arguments = "http://chaturbate.com/" + ModelName
                }
            };
            process.Start();
            var output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            // Parse output
            // e.g. 'Available streams: 240p (worst), 480p, 720p, 1080p (best)'
            var streams = Regex.Split(output, Environment.NewLine)
                .SingleOrDefault(l => l.StartsWith("Available streams: "));

            if (string.IsNullOrEmpty(streams))
            {
                return null;
            }

            streams = streams.Replace("Available streams: ", "")
                .Replace(" (worst)", "")
                .Replace(" (best)", "")
                .Replace("p", "");
            return Regex.Split(streams, ", ")
                .Select(x =>
                {
                    if (int.TryParse(x, out var result))
                    {
                        return result;
                    }
                    return -1;
                })
                .Where(x => x > 0)
                .Distinct()
                .ToList();
        }

        public bool CanRestart()
        {
            return RestartRequired && !Running && DateTimeOffset.UtcNow >= RestartTime;
        }

        public bool IsRunning()
        {
            return Running;
        }
    }
}
