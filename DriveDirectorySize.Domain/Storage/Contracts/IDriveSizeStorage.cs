using DriveDirectorySize.Domain.Models;
using System.Collections.Generic;

namespace DriveDirectorySize.Domain.Storage.Contracts
{
    public interface IDriveSizeStorage
    {
        bool Exists(string filePath);
        IEnumerable<DirectorySizeData> Retrieve(string filePath);
        void Save(string filePath, IEnumerable<DirectorySizeData> data);
    }
}
