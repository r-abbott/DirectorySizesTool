using DriveDirectorySize.Domain.Models;
using DriveDirectorySize.Domain.ValueObjects;
using System.Collections.Generic;

namespace DriveDirectorySize.Domain.Contracts
{
    public interface IDriveReader
    {
        DirectorySizeData CurrentDirectory { get; }
        IEnumerable<DirectorySizeData> CurrentSubDirectories { get; }
        DirectorySizeData ChangeDirectory(string name);
        IEnumerable<DirectorySizeData> FindLargestDirectories(double percentageThreshold);
        IEnumerable<DirectorySizeData> FindDirectoriesLargerThan(Size size);
    }
}
