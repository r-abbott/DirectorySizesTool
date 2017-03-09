using DriveDirectorySize.Domain.Contracts;
using Newtonsoft.Json;
using System;

namespace DriveDirectorySize.Domain
{
    internal class DirectorySizeData : IDriveDirectory
    {
        public string Name { get; }
        public string ParentName { get; }
        public string[] Path { get; }
        public long Size { get; }
        public long TotalSize { get; private set; }

        public DirectorySizeData(string name, string parentName, long size)
        {
            Path = GetSplitPath(name);
            Name = Path[Path.Length - 1];
            Size = size;
            TotalSize = size;

            if (Path.Length > 1)
            {
                ParentName = Path[Path.Length - 2];
            }
        }

        [JsonConstructor]
        public DirectorySizeData(string name, string parentName, long size, long totalSize, string[] path)
        {
            Path = path;
            Name = Path[Path.Length - 1];
            Size = size;
            TotalSize = totalSize;

            if (Path.Length > 1)
            {
                ParentName = Path[Path.Length - 2];
            }
        }

        public void IncreaseTotalSize(long size)
        {
            TotalSize += size;
        }

        private string[] GetSplitPath(string path)
        {
            return path.Split(new[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
