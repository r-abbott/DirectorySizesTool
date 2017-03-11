using DriveDirectorySize.Domain.Contracts;
using DriveDirectorySize.Domain.Storage;
using DriveDirectorySize.Domain.Storage.Contracts;

namespace DriveDirectorySize.Domain
{
    public class Drive : IDrive
    {
        private readonly IDriveSizeStorage _storage;

        public Drive(IDriveSizeStorage storage)
        {
            _storage = storage;
        }

        public Drive()
        {
            _storage = new DiskStorage();
        }

        public IDriveReader ReadFromStorage()
        {
            var data = _storage.Retrieve();
            if (data == null)
            {
                return null;
            }
            return new DriveSizeReader(data);
        }

        public IDriveReader Read(string drive)
        {
            var executor = new DriveExecutor(drive);
            var data = executor.Run();
            _storage.Save(data);
            return new DriveSizeReader(data);
        }

    }
}
