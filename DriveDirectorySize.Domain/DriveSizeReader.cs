using DriveDirectorySize.Domain.Contracts;
using DriveDirectorySize.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DriveDirectorySize.Domain
{
    public class DriveSizeReader : IDriveReader
    {
        private readonly List<DirectorySizeData> _directoryData;

        private DirectorySizeData _currentDirectory;
        private IEnumerable<DirectorySizeData> _currentSubDirectories;

        public DirectorySizeData CurrentDirectory { get { return _currentDirectory; } }
        public IEnumerable<DirectorySizeData> CurrentSubDirectories { get { return _currentSubDirectories; } }

        private DirectorySizeData Root { get { return _directoryData.First(); } }

        internal DriveSizeReader(IEnumerable<DirectorySizeData> directories)
        {
            _directoryData = directories.OrderBy(x => x.Path.Length).ThenBy(x => x.Name).ToList();
            ChangeCurrentDirectory(Root);
        }

        public DirectorySizeData ChangeDirectory(string name)
        {
            int depth = _currentDirectory.Path.Length;
            var directory = _directoryData.FirstOrDefault(d =>
                d.ParentName == _currentDirectory.Name
                && d.Name == name);

            if (directory != null)
            {
                ChangeCurrentDirectory(directory);
                return _currentDirectory;
            }

            if (name == "..")
            {
                ChangeCurrentDirectory(GetCurrentParentDirectory());
                return _currentDirectory;
            }
            return null;
        }

        public DirectorySizeData FindLargestSubDirectory()
        {
            if (_currentSubDirectories.Count() == 0)
            {
                return null;
            }
            long maxTotalSize = _currentSubDirectories.Max(x => x.TotalSize);
            return _currentSubDirectories.First(s => s.TotalSize == maxTotalSize);
        }

        public IEnumerable<DirectorySizeData> FindLargestDirectories(int limit)
        {
            // Needs to be significant to the First Child Directory of Root, not it's own parent
            var topDirectories = FindLargestSubDirectories(Root, 0);

            var largestDirectories = new List<DirectorySizeData>();
            var directoriesToCheck = new List<DirectorySizeData>(topDirectories);
            while (directoriesToCheck.Count > 0)
            {
                var currentToCheck = new List<DirectorySizeData>(directoriesToCheck);
                foreach(var parentDir in currentToCheck)
                {
                    directoriesToCheck.Remove(parentDir);
                    var subDirs = FindLargestSubDirectories(parentDir, 1);
                    if (subDirs.Count() > 0)
                    {
                        directoriesToCheck.AddRange(subDirs);
                    }
                    else
                    {
                        largestDirectories.Add(parentDir);
                    }
                }
            }

            return largestDirectories.OrderByDescending(x=>x.TotalSize);
            
        }

        private IEnumerable<DirectorySizeData> FindLargestSubDirectories(DirectorySizeData parentDirectory, int depth)
        {
            
            return _directoryData
                .Where(x => x.Path.IsChildOf(parentDirectory.Path)
                            // TODO make this make more sense...
                            && IsSignificantSizeDifferent(_directoryData.First(a=>a.Path.FullPath == x.Path.GetAncestorPath(depth)).TotalSize , x.TotalSize))
                .OrderByDescending(x => x.TotalSize)
                .ToList();
        }

        private bool IsSignificantSizeDifferent(long parentTotalSize, long childTotalSize)
        {
            if (parentTotalSize == 0 || childTotalSize == 0) return false;

            return ((double)childTotalSize / (double)parentTotalSize) >= .25d;
        }

        public IEnumerable<DirectorySizeData> Find(Func<DirectorySizeData,bool> query)
        {
            return _directoryData.Where(query);
        }

        private void ChangeCurrentDirectory(DirectorySizeData directory)
        {
            _currentDirectory = directory;
            _currentSubDirectories = GetSubDirectories(directory);
        }

        private IEnumerable<DirectorySizeData> GetSubDirectories(DirectorySizeData directory)
        {
            int depth = directory.Path.Length;
            var directories = _directoryData.Where(d =>
                d.ParentName == directory.Name
                && d.Path.Length == depth + 1)
                .OrderBy(d => d.Name);

            return directories;
        }

        private DirectorySizeData GetCurrentParentDirectory()
        {
            int depth = _currentDirectory.Path.Length - 1;
            return _directoryData.FirstOrDefault(x =>
                    x.Path.Length == depth
                    && x.Name == _currentDirectory.ParentName);
        }

        
    }
}
