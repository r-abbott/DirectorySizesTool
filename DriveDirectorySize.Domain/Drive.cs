using DriveDirectorySize.Domain.Contracts;
using DriveDirectorySize.Domain.Storage;
using DriveDirectorySize.Domain.Storage.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DriveDirectorySize.Domain
{
    public class Drive : IDrive
    {
        private readonly char _driveLetter;
        private readonly IDriveSizeStorage _storage;
        private string _driveDirectory;
        private string _saveFilePath;
        private ILog _log;

        public Drive(char driveLetter, ILog log)
        {
            _driveDirectory = $"{driveLetter}:\\";
            var driveInfo = new DriveInfo(_driveDirectory);
            
            if (driveInfo == null)
            {
                throw new InvalidOperationException($"Drive {_driveLetter} does not exist.");
            }
            _driveDirectory = driveInfo.RootDirectory.FullName;
            _driveLetter = driveLetter;
            _storage = new DiskStorage(log);
            _log = log;
            CreateFilePath(driveLetter);
        }

        public static IEnumerable<string> AvailableDrives()
        {
            var validDriveTypes = new List<DriveType>
            {
                DriveType.Fixed,
                DriveType.Removable
            };

            return DriveInfo.GetDrives().Where(d => validDriveTypes.Contains(d.DriveType)).Select(d => d.Name);
        }

        public IDriveReader ReadFromStorage()
        {
            var data = _storage.Retrieve(_saveFilePath);
            if (data == null)
            {
                return null;
            }
            return new DriveSizeReader(data);
        }

        public IDriveReader Read()
        {
            var executor = new DriveExecutor(_driveDirectory, _log);
            WriteOutput("Reading drive:\n");
            var data = executor.Run();
            _storage.Save(_saveFilePath, data);
            return new DriveSizeReader(data);
        }

        private void CreateFilePath(char drive)
        {
            _saveFilePath = $"Drive_{drive}.json";
        }

        private void WriteOutput(string text)
        {
            if(_log != null)
            {
                _log.Write(text);
            }
        }

    }
}
