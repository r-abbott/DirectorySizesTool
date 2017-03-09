using System.Collections.Generic;

namespace DriveDirectorySize.Domain.Contracts
{
    public interface IDriveSizeStorage
    {
        bool Exists();
        IEnumerable<IDriveDirectory> Retrieve();
        void Save(IEnumerable<IDriveDirectory> data);
    }
}
