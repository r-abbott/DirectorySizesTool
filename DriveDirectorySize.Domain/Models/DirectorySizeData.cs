using Newtonsoft.Json;
using System;

namespace DriveDirectorySize.Domain.Models
{
    public class DirectorySizeData
    {
        [JsonIgnore]
        public string Name { get { return Path.Identity; } }
        [JsonIgnore]
        public string ParentName { get { return Path.Parent; } }
        public Path Path { get; }
        public long Size { get; }
        public long TotalSize { get; private set; }

        internal DirectorySizeData(string path, long size)
        {
            Path = new Path(path);
            Size = size;
            TotalSize = size;
        }

        [JsonConstructor]
        internal DirectorySizeData(Path path, long size, long totalSize)
        {
            if (path == null) throw new ArgumentNullException("path");

            Path = new Path(path);
            Size = size;
            TotalSize = totalSize;
        }

        internal void IncreaseTotalSize(long size)
        {
            TotalSize += size;
        }
    }
}
