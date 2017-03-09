using DriveDirectorySize.Domain.Contracts;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace DriveDirectorySize.UI
{
    public class DiskStorage : IDriveSizeStorage
    {
        private const string FilePath = "driveSizeStorage.json";

        public bool Exists()
        {
            return File.Exists(FilePath);
        }

        public IEnumerable<IDriveDirectory> Retrieve()
        {
            if (Exists())
            {
                var data = File.ReadAllText(FilePath);
                return JsonConvert.DeserializeObject<IEnumerable<IDriveDirectory>>(data);
            }
            return null;
        }

        public void Save(IEnumerable<IDriveDirectory> data)
        {
            var json = JsonConvert.SerializeObject(data);
            File.WriteAllText(FilePath, json);
        }
    }
}
