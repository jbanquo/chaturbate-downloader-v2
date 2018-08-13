using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using cb_downloader_v2.Utils;

namespace cb_downloader_v2
{
    class DownloaderProcessManager
    {
        public static DateTime TimeNow = DateTime.Now;
        private readonly ConcurrentDictionary<string, IDownloaderProcess> _listeners = new ConcurrentDictionary<string, IDownloaderProcess>();
        private Thread _thread;
        private MainForm _parent;
        private ListBox _modelsBox;

        public bool Running => _thread != null && _thread.IsAlive;
        public int Count => _listeners.Count;
        public int TickSleepDelay = 60 * 100;

        public DownloaderProcessManager(MainForm parent, ListBox modelsBox)
        {
            _parent = parent;
            _modelsBox = modelsBox;
        }
        
        public void Start()
        {
            _thread = new Thread(Tick);
            _thread.Start();
        }

        public void Stop()
        {
            // Stop thread
            if (Running)
                _thread.Abort();

            // Stop all downloader processes
            foreach (KeyValuePair<string, IDownloaderProcess> valuePair in _listeners)
            {
                string modelName = valuePair.Key;
                IDownloaderProcess listener = valuePair.Value;

                // Initiate termination
                listener.Terminate();

                // Remove listener from list
                IDownloaderProcess output;
                _listeners.TryRemove(modelName, out output);
            }
        }

        public void Tick()
        {
            while (Thread.CurrentThread.IsAlive)
            {
                Thread.Sleep(TickSleepDelay);

                // Update cached time
                TimeNow = DateTime.Now;

                // Restart processes where necessary
                foreach (IDownloaderProcess process in _listeners.Values
                    .Where(p => p.CanRestart()))
                {
                    process.Start();
                }
            }
        }

        public void AddModel(string modelName, bool immediate = false)
        {
            // Normalising name
            modelName = NormaliseModelName(modelName).ToLower();

            // Check input validity
            if (string.IsNullOrWhiteSpace(modelName))
                return;
            
            // Check if the model is already being listened to
            if (_modelsBox.Items.Cast<object>().Contains(modelName))
                return;

            // Create process and add listener to lists
            IDownloaderProcess proc = new LivestreamerProcess(_parent, modelName);
            _modelsBox.Items.Add(modelName);
            _listeners.AddOrUpdate(modelName, proc, (s, listener) => listener);

            // Quick start functionality (i.e. start listener immediately)
            proc.Start(immediate);
            Logger.Log(modelName, "Added");
        }

        public bool RemoveModel(string modelName)
        {
            IDownloaderProcess output;
            return _listeners.TryRemove(modelName, out output);
        }

        public string NormaliseModelName(string modelName)
        {
            // Check if cb link or not
            if (UrlHelper.IsChaturbateUrl(modelName))
            {
                return UrlHelper.GetModelName(modelName);
            }

            // If not a cb link, ie assume its a model name
            return modelName.Trim(' ', '\t', '/', '\\');
        }
        
        public IDownloaderProcess this[string key]
        {
            get { return _listeners[key]; }
            set { _listeners[key] = value; }
        }
    }
}
