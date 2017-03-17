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
        public long LocalSize { get; private set; }
        public long TotalSize { get; private set; }

        internal DirectorySizeData(string path, long size)
        {
            Path = new Path(path);
            LocalSize = size;
            TotalSize = size;
        }

        [JsonConstructor]
        internal DirectorySizeData(Path path, long size, long totalSize)
        {
            if (path == null) throw new ArgumentNullException("path");

            Path = new Path(path);
            LocalSize = size;
            TotalSize = totalSize;
        }

        public void SetSelfSize(long size)
        {
            if (size < 0) return;

            LocalSize = size;
            SetSizeFromDescendant(size);
        }

        public void SetSizeFromDescendant(long size)
        {
            if (size < 0) return;
            TotalSize += size;
        }
    }
}
