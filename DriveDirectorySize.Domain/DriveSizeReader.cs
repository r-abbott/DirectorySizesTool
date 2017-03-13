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

        public IEnumerable<DirectorySizeData> FindLargestDirectories()
        {
            return FindLargestSubDirectories(Root);
        }

        private IEnumerable<DirectorySizeData> FindLargestSubDirectories(DirectorySizeData parentDirectory)
        {
            const double thresholdMultiplier = .05d; //.075d;
            const double thresholdMultiplierIncrement = .075d; //.085d;
            const double includeParentThresholdMultiplier = 1.25d;

            var thresholdSize = parentDirectory.TotalSize * (thresholdMultiplier + (thresholdMultiplierIncrement * parentDirectory.Path.Depth));

            var qualifyingDirectories = new List<DirectorySizeData>();

            var directoriesWithinThreshold = _directoryData
                .Where(d => d.Path.ParentPath == parentDirectory.Path.FullPath
                && d.TotalSize >= thresholdSize).ToList();
                

            var subSize = directoriesWithinThreshold.Sum(d => d.TotalSize);

            if (Root != parentDirectory
                && (parentDirectory.TotalSize - subSize) >= (thresholdSize * includeParentThresholdMultiplier))
            {
                qualifyingDirectories.Add(parentDirectory);
            }

            foreach(var directory in directoriesWithinThreshold)
            {
                qualifyingDirectories.AddRange(FindLargestSubDirectories(directory));
            }

            return qualifyingDirectories;
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
