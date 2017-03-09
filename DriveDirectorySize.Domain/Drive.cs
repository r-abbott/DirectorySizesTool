using DriveDirectorySize.Domain.Contracts;

namespace DriveDirectorySize.Domain
{
    public class Drive : IDrive
    {
        private readonly IDriveSizeStorage _storage;

        public Drive(IDriveSizeStorage storage)
        {
            _storage = storage;
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
