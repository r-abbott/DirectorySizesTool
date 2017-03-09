using System.Collections.Concurrent;
using System.IO;

namespace DriveDirectorySize.Domain
{
    internal class DirectoryMap
    {
        private string _drive;
        private BlockingCollection<string> _outputQueue;

        public DirectoryMap(string drive, BlockingCollection<string> outputQueue)
        {
            _drive = drive;
            _outputQueue = outputQueue;
        }

        public void ReadAll()
        {
            GetAllDirectories(_drive);
            _outputQueue.CompleteAdding();
        }

        private void GetAllDirectories(string path)
        {
            _outputQueue.Add(path);

            try
            {
                var di = new DirectoryInfo(path);
                foreach (var sd in di.EnumerateDirectories())
                {
                    try
                    {
                        GetAllDirectories(sd.FullName);
                    }
                    catch { }
                }
            }
            catch { }
        }
    }
}
