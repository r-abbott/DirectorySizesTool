using Newtonsoft.Json;
using System;
using System.Linq;

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

    public class Path
    {
        public string FullPath { get; }
        [JsonIgnore]
        public string ParentPath { get; }
        [JsonIgnore]
        public string Root { get; }
        [JsonIgnore]
        public string Identity { get; }
        [JsonIgnore]
        public string Parent { get; }
        [JsonIgnore]
        public string[] Parts { get; }
        [JsonIgnore]
        public int Length { get { return Parts.Length; } }
        [JsonIgnore]
        public int Depth { get { return Parts.Length - 1; } }

        private const char PATH_SEPARATOR = '\\';

        [JsonConstructor]
        public Path(string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath)) throw new ArgumentNullException("fullPath");

            if (fullPath.EndsWith("\\"))
            {
                fullPath = fullPath.Substring(0, fullPath.Length - 1);
            }

            FullPath = fullPath;
            Parts = CreateParts();
            Identity = ParseIdentity();
            Parent = ParseParent();
            ParentPath = ParseParentPath();
            Root = Parts[0];
        }

        public string PathToDepth(int depth)
        {
            if (depth < 1) return "";
            if (depth > Depth) return FullPath;

            return string.Join($"{PATH_SEPARATOR}", Parts.Take(depth));
        }

        public string IdentityAtDepth(int depth)
        {
            if (depth < 1) return "";
            if (depth > Depth) return "";

            return Parts[depth - 1];
        }

        public bool IsDescendentOf(Path path)
        {
            if (path == null) throw new ArgumentNullException("path");
            if (Depth <= path.Depth) return false;

            for(int i = 0; i < path.Depth; i++)
            {
                if(path.Parts[i] != Parts[i])
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsChildOf(Path path)
        {
            if(path == null) throw new ArgumentNullException("path");
            if (Depth <= path.Depth) return false;

            return ParentPath == path.FullPath;
        }

        public string GetAncestorPath(int depth)
        {
            if (depth >= Depth) return null;

            return PathToDepth(depth);
        }

        private string[] CreateParts()
        {
            return FullPath.Split(new[] { PATH_SEPARATOR }, StringSplitOptions.RemoveEmptyEntries);
        }

        private string ParseIdentity()
        {
            return Parts[Depth];
        }

        private string ParseParent()
        {
            if(Depth == 0)
            {
                return "";
            }
            return Parts[Depth - 1];
        }

        private string ParseParentPath()
        {
            if (Depth == 0)
            {
                return "";
            }

            return string.Join($"{PATH_SEPARATOR}", Parts.Take(Depth));
        }

        public override string ToString()
        {
            return FullPath;
        }

        public static implicit operator string (Path path)
        {
            return path.FullPath;
        }
    }
}
