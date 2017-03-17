using DriveDirectorySize.Domain.Models;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;

namespace DriveDirectorySize.Domain
{
    internal class DirectorySizeWorker
    {
        private readonly BlockingCollection<string> _directoryPaths;
        private readonly ConcurrentDictionary<string, DirectorySizeData> _directorySizes;

        public DirectorySizeWorker(BlockingCollection<string> directoryPaths,
            ConcurrentDictionary<string, DirectorySizeData> directorySizes
            )
        {
            _directoryPaths = directoryPaths;
            _directorySizes = directorySizes;
        }

        public void ProcessSizes()
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

                    AddOrUpdateLocalSize(sizeData);

                    foreach(var ancestor in sizeData.Path.GetAncestorPaths())
                    {
                        AddOrUpdateAncestorSize(ancestor, size);
                    }
                }
            }
        }

        private void AddOrUpdateLocalSize(DirectorySizeData directorySize)
        {
            if (!_directorySizes.TryAdd(directorySize.Path, directorySize))
            {
                DirectorySizeData loadedDirectory;
                if (_directorySizes.TryGetValue(directorySize.Path, out loadedDirectory))
                {
                    loadedDirectory.SetSelfSize(directorySize.LocalSize);
                }
            }
        }

        private void AddOrUpdateAncestorSize(string path, long size)
        {
            var directory = new DirectorySizeData(path, 0);
            directory.SetSizeFromDescendant(size);
            if (!_directorySizes.TryAdd(path, directory))
            {
                if (_directorySizes.TryGetValue(path, out directory))
                {
                    directory.SetSizeFromDescendant(size);
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
