using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;

namespace DriveDirectorySize.Domain
{
    internal class DirectorySizeWorker
    {
        private readonly BlockingCollection<string> _inputQueue;
        private readonly Action<DirectorySizeData> _addData;

        public DirectorySizeWorker(BlockingCollection<string> inputQueue, Action<DirectorySizeData> addData)
        {
            _inputQueue = inputQueue;
            _addData = addData;
        }

        public void Run()
        {
            foreach (var path in _inputQueue.GetConsumingEnumerable())
            {
                DirectoryInfo di = null;
                try
                {
                    di = new DirectoryInfo(path);
                }
                catch { }

                if (di != null)
                {
                    var size = GetLocalSize(di);
                    var sizeData = new DirectorySizeData(di.FullName, di.Parent == null ? "" : di.Parent.FullName, size);
                    _addData(sizeData);
                }
            }
        }

        private long GetLocalSize(DirectoryInfo di)
        {
            long localSize = 0;
            IEnumerable<FileInfo> files = null;
            try
            {
                files = di.EnumerateFiles();
                foreach (var fi in files)
                {
                    try
                    {
                        localSize += fi.Length;
                    }
                    catch { }
                }
            }
            catch { }
            return localSize;
        }
    }
}
