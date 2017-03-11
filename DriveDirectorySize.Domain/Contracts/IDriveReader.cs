using DriveDirectorySize.Domain.Models;
using System;
using System.Collections.Generic;

namespace DriveDirectorySize.Domain.Contracts
{
    public interface IDriveReader
    {
        DirectorySizeData CurrentDirectory { get; }
        IEnumerable<DirectorySizeData> CurrentSubDirectories { get; }

        DirectorySizeData ChangeDirectory(string name);
        DirectorySizeData FindLargestSubDirectory();
        IEnumerable<DirectorySizeData> Find(Func<DirectorySizeData, bool> query);
        IEnumerable<DirectorySizeData> FindLargestDirectories(int limit);
    }
}
