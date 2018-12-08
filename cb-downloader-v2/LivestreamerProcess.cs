using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using log4net;

namespace cb_downloader_v2
{
    public class LivestreamerProcess : IDownloaderProcess
    {
        // NOTE: issues occur when, i.e. you close the app and THEN a stream begins being listened to (i.e. this is not terminated), etc.
        private static readonly ILog Log = LogManager.GetLogger(typeof(LivestreamerProcess));
        private static readonly Random Random = new Random();
        private static string StreamTerminatedMessage = "[cli][info] Stream ended";
        private static string StreamServiceUnavailablePart = "503 Server Error: Service Temporarily Unavailable";
        private static string StreamOfflineMessagePart = "error: No playable streams found on this URL: ";
        private static string CommandArguments = "chaturbate.com/{0} {1} -o {2}";
        private static string FileNameTemplate = MainForm.OutputFolderName + "/{0}-{1}-{2}{3}.flv";
        private static string DefaultQuality = "best";
        private TimeSpan RestartDelay => TimeSpan.FromSeconds(Math.Max(_failures + 1, 6) * 10);
        private DateTimeOffset StandardRestartDelay => DateTimeOffset.UtcNow + RestartDelay;
        private readonly MainForm _mf;
        private Status _status = Status.Disconnected;
        private CancellationTokenSource _cancelToken;
        private Process _process;
        private int _failures;
        public string ModelName { get; }
        public Status Status
        {
            private set
            {
                _status = value;
                _mf.SetStatus(ModelName, _status);
            }
            get => _status;
        }
        public bool RestartRequired { get; private set; } = true;
        public DateTimeOffset RestartTime { get; private set; }

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
            Status = Status.Connecting;
            Log.Debug($"{ModelName}: Connecting in {delay}ms");

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

                if (quality == null)
                {
                    Log.Debug($"{ModelName}: Disconnected (failed to retrieve quality options)");
                    _failures++;
                    Status = Status.Disconnected;
                    RestartTime = StandardRestartDelay;
                    return;
                }

                _failures = 0;
                Log.Debug($"{ModelName}: Available qualities: {string.Join(",", quality)}");

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

                // Updating flags and start process
                RestartRequired = false;
                _process.OutputDataReceived += LivestreamerProcess_OutputDataReceived;
                _process.Start();
                _process.BeginOutputReadLine();
                Status = Status.Connected;
                Log.Debug($"{ModelName}: Started (quality={selectedQuality}p)");

                // Check if cancellation was called
                if (_cancelToken.IsCancellationRequested)
                {
                    Terminate();
                }
            }, _cancelToken.Token);
        }

        private static string SelectQuality(List<int> qualities, int targetQuality)
        {
            if (qualities == null || qualities.Count == 0)
                return DefaultQuality;

            foreach (var q in qualities)
            {
                if (q >= targetQuality)
                {
                    return q + "p";
                }
            }
            return DefaultQuality;
        }

        private string GenerateArguments(string fileName, string quality)
        {
            var args = string.Format(CommandArguments, ModelName, quality, fileName);

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
            var timeNow = DateTime.Now;
            var date = timeNow.ToString("ddMMyy");
            var time = timeNow.ToString("HHmmss");
            var fileName = string.Format(FileNameTemplate, ModelName, date, time, "");

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
            var line = e.Data;

            // Checking if data is valid
            if (line == null)
            {
                if (Status != Status.Disconnected) // if a custom errors is fired before this, this becomes ignored
                {
                    Log.Debug($"{ModelName}: Disconnecting (End of stream)");
                    Terminate(true);
                }
                else
                {
                    Log.Debug($"{ModelName}: End of stream");
                }
                return;
			}
			
			if (line.Length == 0)
				return;

            // Parsing line
            Log.Debug($"{ModelName}: [RAW]: {line}");

            // Checking if the stream was terminated server-side
            if (line.Equals(StreamTerminatedMessage))
            {
                Log.Debug($"{ModelName}: Disconnecting (Terminated)");
                Terminate(true);
            }

            // Checking if service is offline (i.e. being ddosed)
            if (line.Contains(StreamServiceUnavailablePart))
            {
                Log.Debug($"{ModelName}: Disconnecting (Service unavailable)");
                Terminate(true, StandardRestartDelay);
            }

            // Checking if the username is invalid
            if (line.Contains(_streamInvalidUsernameMessage))
            {
                Log.Debug($"{ModelName}: Disconnecting (HTTP404: Invalid username)");
                
                Terminate();
                _mf.RemoveInvalidUrlModel(ModelName);
            }

            // Checking if stream is offline
            if (line.StartsWith(StreamOfflineMessagePart) || line.Contains(_streamReadTimeoutMessage))
            {
                Log.Debug($"{ModelName}: Disconnecting (Stream offline)");
                Terminate(true, StandardRestartDelay);
            }
        }

        public void Terminate()
        {
            Terminate(false);
        }
        
        public void Terminate(bool restartRequired, DateTimeOffset? restartTime = null)
        {
            // Update flags
            RestartRequired = restartRequired;
            RestartTime = restartTime ?? DateTimeOffset.UtcNow;

            // Activate cancellation token
            if (_cancelToken != null && !_cancelToken.IsCancellationRequested)
            {
                _cancelToken.Cancel();
            }

            // Attempting to close process
            if (_process != null)
            {
                try
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
                catch
                {
                    // ignored
                }
            }

            // Uncheck on models list in form
            Status = Status.Disconnected;
        }

        /// <summary>
        ///     Kill a process, and all of its children, grandchildren, etc.
        ///     Source: https://stackoverflow.com/questions/5901679/kill-process-tree-programatically-in-c-sharp?answertab=active#tab-top
        /// </summary>
        /// <param name="pid">Process ID.</param>
        private static void KillProcessTree(int pid)
        {
            var searcher = new ManagementObjectSearcher("Select * From Win32_Process Where ParentProcessID=" + pid);
            var moc = searcher.Get();

            foreach (var mo in moc)
            {
                KillProcessTree(Convert.ToInt32(mo["ProcessID"]));
            }

            try
            {
                var proc = Process.GetProcessById(pid);
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
            var restartable = RestartRequired && Status == Status.Disconnected && DateTimeOffset.UtcNow >= RestartTime;

            if (restartable)
            {
                Log.Debug($"{ModelName}: Restartable (required={RestartRequired}, status={Status}, delay_ok={DateTimeOffset.UtcNow >= RestartTime})");
            }
            return restartable;
        }
    }
}
