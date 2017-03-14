using DriveDirectorySize.Domain.Contracts;
using System.Collections.Concurrent;
using System.IO;

namespace DriveDirectorySize.Domain
{
    internal class DirectoryMap
    {
        private string _drive;
        private BlockingCollection<string> _outputQueue;
        private ILog _log;

        public DirectoryMap(string drive, BlockingCollection<string> outputQueue, ILog log)
        {
            _log = log;
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
            try
            {
                var di = new DirectoryInfo(path);
                WriteConcurrent(di);
                _outputQueue.Add(path);
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

        private void WriteConcurrent(DirectoryInfo di)
        {
            if (_log != null)
            {
                if (di.FullName.Length > 100)
                {
                    _log.WriteConcurrent($"Processing: {di.FullName.Substring(0, 100)}");
                }
                else
                {
                    _log.WriteConcurrent($"Processing: {di.FullName}");
                }
            }
        }
    }
}
