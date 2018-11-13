using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using cb_downloader_v2.Utils;

namespace cb_downloader_v2
{
    class DownloaderProcessManager : IDownloaderProcessManager
    {
        private readonly ConcurrentDictionary<string, IDownloaderProcess> _listeners = new ConcurrentDictionary<string, IDownloaderProcess>();
        private readonly MainForm _parent;
        private readonly ModelsGridWrapper _models;
        private Thread _thread;

        public bool Running => _thread != null && _thread.IsAlive;
        public int Count => _listeners.Count;
        public TimeSpan TickSleepDelay = TimeSpan.FromSeconds(2);

        public DownloaderProcessManager(MainForm parent, ModelsGridWrapper models)
        {
            _parent = parent;
            _models = models;
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
                _listeners.TryRemove(modelName, out var output);
            }
        }

        public void Tick()
        {
            while (Thread.CurrentThread.IsAlive)
            {
                Thread.Sleep(TickSleepDelay);

                // Restart processes
                var restartable = _listeners.Values.Where(p => p.CanRestart()).ToList();

                for (var i = 0; i < restartable.Count; i++)
                {
                    var process = restartable[i];
                    var delay = (i + 1) * 500;
                    Logger.Log(process.ModelName, $"Automated restart in {delay}ms");
                    process.Start(delay);
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
            if (_models.Contains(modelName))
                return;

            // Create process and add listener to lists
            IDownloaderProcess process = new LivestreamerProcess(_parent, modelName);
            _models.AddModel(modelName);
            _listeners.AddOrUpdate(modelName, process, (s, listener) => listener);

            // Quick start functionality (i.e. start listener immediately)
            process.Start(immediate);
            Logger.Log(modelName, "Added");
        }

        public bool RemoveModel(string modelName)
        {
            return _listeners.TryRemove(modelName, out var output);
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
