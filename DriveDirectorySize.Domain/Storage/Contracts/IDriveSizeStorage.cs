using DriveDirectorySize.Domain.Models;
using System.Collections.Generic;

namespace DriveDirectorySize.Domain.Storage.Contracts
{
    public interface IDriveSizeStorage
    {
        bool Exists();
        IEnumerable<DirectorySizeData> Retrieve();
        void Save(IEnumerable<DirectorySizeData> data);
    }
}
