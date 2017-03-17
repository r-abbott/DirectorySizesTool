using DriveDirectorySize.Domain.Contracts;
using DriveDirectorySize.Domain.Models;
using DriveDirectorySize.Domain.ValueObjects;
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
            _directoryData = directories.OrderBy(x => x.Path.Depth).ThenBy(x => x.Name).ToList();
            ChangeCurrentDirectory(Root);
        }

        public DirectorySizeData ChangeDirectory(string name)
        {
            if (name == "..")
            {
                ChangeCurrentDirectory(GetCurrentParentDirectory());
                return _currentDirectory;
            }

            var directory = _directoryData.FirstOrDefault(d =>
                d.ParentName == _currentDirectory.Name
                && d.Name == name);

            if (directory != null)
            {
                ChangeCurrentDirectory(directory);
                return _currentDirectory;
            }
            return null;
        }

        public IEnumerable<DirectorySizeData> FindDirectoriesLargerThan(Size size)
        {
            return _directoryData.Where(d => d.TotalSize >= size.ByteValue)
                .OrderBy(d => d.Path.ParentPath)
                .ThenBy(d => d.Path.Identity)
                .ThenBy(d => d.Path.Depth);
                
        }

        public IEnumerable<DirectorySizeData> FindLargestDirectories(double percentageThreshold)
        {
            return FindLargestSubDirectories(Root, percentageThreshold);
        }

        private IEnumerable<DirectorySizeData> FindLargestSubDirectories(
            DirectorySizeData parentDirectory,
            double percentageThreshold
            )
        {
            double thresholdMultiplier = percentageThreshold;
            double thresholdMultiplierIncrement = percentageThreshold + (percentageThreshold / 2d);
            double includeParentThresholdMultiplier = 1 + (percentageThreshold * 4);

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

            foreach (var directory in directoriesWithinThreshold)
            {
                qualifyingDirectories.AddRange(FindLargestSubDirectories(directory, percentageThreshold));
            }

            return qualifyingDirectories;
        }

        private void ChangeCurrentDirectory(DirectorySizeData directory)
        {
            if (directory == null) return;

            _currentDirectory = directory;
            _currentSubDirectories = GetSubDirectories(directory);
        }

        private IEnumerable<DirectorySizeData> GetSubDirectories(DirectorySizeData directory)
        {
            var directories = _directoryData.Where(d =>
                d.Path.ParentPath == directory.Path.FullPath)
                .OrderBy(d => d.Name);

            return directories;
        }

        private DirectorySizeData GetCurrentParentDirectory()
        {
            int depth = _currentDirectory.Path.Depth - 1;
            return _directoryData.FirstOrDefault(x =>
                    x.Path.Depth == depth
                    && x.Name == _currentDirectory.ParentName);
        }
    }
}
