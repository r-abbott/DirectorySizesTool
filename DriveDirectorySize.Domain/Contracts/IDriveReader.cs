using System.Collections.Generic;

namespace DriveDirectorySize.Domain.Contracts
{
    public interface IDriveReader
    {
        IDriveDirectory CurrentDirectory { get; }
        IEnumerable<IDriveDirectory> CurrentSubDirectories { get; }

        IDriveDirectory ChangeDirectory(string name);
        IDriveDirectory FindLargestSubDirectory();
    }
}
