using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DriveDirectorySize.Domain.Models
{
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
        public int Depth { get { return Parts.Length - 1; } }

        private const char PATH_SEPARATOR = '\\';

        [JsonConstructor]
        public Path(string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath)) throw new ArgumentNullException("fullPath");

            if (fullPath.EndsWith(PATH_SEPARATOR.ToString()))
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

        public string IdentityAtDepth(int depth)
        {
            if (depth < 0 || depth > Depth) return "";

            return Parts[depth];
        }

        public bool IsDescendantOf(Path path)
        {
            if (path == null) throw new ArgumentNullException("path");
            if (Depth <= path.Depth) return false;

            for (int i = 0; i <= path.Depth; i++)
            {
                if (path.Parts[i] != Parts[i])
                {
                    return false;
                }
            }
            return true;
        }

        public override string ToString()
        {
            return FullPath;
        }

        public static implicit operator string(Path path)
        {
            return path.FullPath;
        }

        public IEnumerable<string> GetAncestorPaths()
        {
            var ancestorPaths = new List<string>();

            for(int i = 0; i < Depth; i++)
            {
                ancestorPaths.Add(GetPathToDepth(i));
            }
            return ancestorPaths;
        }

        private string GetPathToDepth(int depth)
        {
            var path = new List<string>();

            for(int i = 0; i < depth+1; i++)
            {
                path.Add(Parts[i]);
            }
            return string.Join(PATH_SEPARATOR.ToString(), path.ToArray());
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
            if (Depth == 0)
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
        
    }
}
