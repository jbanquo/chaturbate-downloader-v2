namespace cb_downloader_v2
{
    interface IDownloaderProcess
    {
        bool CanRestart();

        bool IsRunning();

        void Start(bool quickStart = false);

        void Terminate();
    }
}
