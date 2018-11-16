namespace cb_downloader_v2
{
    interface IDownloaderProcess
    {
        string ModelName { get; }

        Status Status { get; }

        bool CanRestart();

        void Start(bool quickStart = false);

        void Start(int delay);

        void Terminate();
    }
}
