namespace cb_downloader_v2
{
    interface IDownloaderProcessManager
    {
        int Count { get; }

        void Start();

        void Stop();

        void AddModel(string modelname, bool immediate = false);

        bool RemoveModel(string modelName);

        IDownloaderProcess this[string key] { get; set; }
    }
}
