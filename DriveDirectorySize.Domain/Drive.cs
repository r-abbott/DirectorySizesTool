using DriveDirectorySize.Domain.Contracts;
using DriveDirectorySize.Domain.Storage;
using DriveDirectorySize.Domain.Storage.Contracts;
using System.IO;

namespace DriveDirectorySize.Domain
{
    public class Drive : IDrive
    {
        private readonly char _driveLetter;
        private readonly IDriveSizeStorage _storage;
        private string _filePath;
        private ILog _log;

        public Drive(char driveLetter, ILog log)
        {
            _driveLetter = driveLetter;
            _storage = new DiskStorage(log);
            _log = log;
            CreateFilePath(driveLetter);
        }

        public IDriveReader ReadFromStorage()
        {
            var data = _storage.Retrieve(_filePath);
            if (data == null)
            {
                return null;
            }
            return new DriveSizeReader(data);
        }

        public IDriveReader Read()
        {
            var executor = new DriveExecutor($"{_driveLetter}:\\", _log);
            WriteOutput("Reading drive:\n");
            var data = executor.Run();
            _storage.Save(_filePath, data);
            return new DriveSizeReader(data);
        }

        private void CreateFilePath(char drive)
        {
            _filePath = $"Drive_{drive}.json";
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
