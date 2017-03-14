using DriveDirectorySize.Domain.Models;
using System.Collections.Generic;

namespace DriveDirectorySize.Domain.Contracts
{
    public interface IDriveReader
    {
        DirectorySizeData CurrentDirectory { get; }
        IEnumerable<DirectorySizeData> CurrentSubDirectories { get; }
        DirectorySizeData ChangeDirectory(string name);
        IEnumerable<DirectorySizeData> FindLargestDirectories(double percentageThreshold);
    }
}
