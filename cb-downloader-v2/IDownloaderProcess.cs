namespace cb_downloader_v2
{
    interface IDownloaderProcess
    {
        string ModelName { get; }

        bool CanRestart();

        bool IsRunning();

        void Start(bool quickStart = false);

        void Start(int delay);

        void Terminate();
    }
}
