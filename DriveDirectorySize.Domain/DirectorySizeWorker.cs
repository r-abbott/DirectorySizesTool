using DriveDirectorySize.Domain.Models;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DriveDirectorySize.Domain
{
    internal class DirectorySizeWorker
    {
        private readonly BlockingCollection<string> _directoryPaths;
        private readonly BlockingCollection<DirectorySizeData> _localdirectorySizes;
        private readonly BlockingCollection<DirectorySizeData> _finalDirectorySizes;

        public DirectorySizeWorker(BlockingCollection<string> directoryPaths,
            BlockingCollection<DirectorySizeData> localDirectorySizes,
            BlockingCollection<DirectorySizeData> finalDirectorySizes
            )
        {
            _directoryPaths = directoryPaths;
            _localdirectorySizes = localDirectorySizes;
            _finalDirectorySizes = finalDirectorySizes;
        }

        public void ProcessLocalSizes()
        {
            foreach (var path in _directoryPaths.GetConsumingEnumerable())
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
                    var sizeData = new DirectorySizeData(di.FullName, size);
                    _localdirectorySizes.Add(sizeData);
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

        public void ProcessTotalSizes()
        {
            foreach (var directory in _localdirectorySizes.GetConsumingEnumerable())
            {
                UpdateTotalSize(directory);
            }
        }

        private void UpdateTotalSize(DirectorySizeData data)
        {
            int depth = data.Path.Depth;
            var totalSize = _localdirectorySizes
                .Where(d => d.Path.Depth > depth
                            && d.Path.IsDescendantOf(data.Path))
                .Sum(d => d.Size);
            data.IncreaseTotalSize(totalSize);
            _finalDirectorySizes.Add(data);
        }

    }
}
