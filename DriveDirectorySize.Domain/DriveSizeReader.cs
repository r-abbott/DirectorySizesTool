using DriveDirectorySize.Domain.Contracts;
using System.Collections.Generic;
using System.Linq;

namespace DriveDirectorySize.Domain
{
    public class DriveSizeReader : IDriveReader
    {
        private readonly List<DirectorySizeData> _directoryData;

        private DirectorySizeData _currentDirectory;
        private IEnumerable<DirectorySizeData> _currentSubDirectories;

        public IDriveDirectory CurrentDirectory { get { return _currentDirectory; } }
        public IEnumerable<IDriveDirectory> CurrentSubDirectories { get { return _currentSubDirectories; } }

        internal DriveSizeReader(IEnumerable<IDriveDirectory> directories)
            : this(directories.Select(d => new DirectorySizeData(d.Name, d.ParentName, d.Size)).ToList())
        {
        }

        internal DriveSizeReader(IEnumerable<DirectorySizeData> directories)
        {
            _directoryData = directories.OrderBy(x => x.Path.Length).ThenBy(x => x.Name).ToList();
            ChangeCurrentDirectory(_directoryData.First());
        }

        public IDriveDirectory ChangeDirectory(string name)
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
            }
            return null;
        }

        public IDriveDirectory FindLargestSubDirectory()
        {
            if (_currentSubDirectories.Count() == 0)
            {
                return null;
            }
            long maxTotalSize = _currentSubDirectories.Max(x => x.TotalSize);
            return _currentSubDirectories.First(s => s.TotalSize == maxTotalSize);
        }

        private void ChangeCurrentDirectory(DirectorySizeData directory)
        {
            _currentDirectory = GetDirectoryWithTotalSize(directory);
            _currentSubDirectories = GetSubDirectories(directory);
        }

        private IEnumerable<DirectorySizeData> GetSubDirectories(DirectorySizeData directory)
        {
            int depth = directory.Path.Length;
            var directories = _directoryData.Where(d =>
                d.ParentName == directory.Name
                && d.Path.Length == depth + 1);

            return directories.Select(GetDirectoryWithTotalSize).OrderBy(d => d.Name);
        }

        private DirectorySizeData GetDirectoryWithTotalSize(DirectorySizeData data)
        {
            int depth = data.Path.Length - 1;
            var totalSize = _directoryData
                .Where(d => d.Path.Length >= depth + 1 && d.Path[depth] == data.Path[depth])
                .Sum(d => d.Size);
            return new DirectorySizeData(data.Name, data.ParentName, data.Size, totalSize, data.Path);

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
