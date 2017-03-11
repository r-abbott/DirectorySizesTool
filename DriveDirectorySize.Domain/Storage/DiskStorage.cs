using DriveDirectorySize.Domain.Models;
using DriveDirectorySize.Domain.Storage.Contracts;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace DriveDirectorySize.Domain.Storage
{
    internal class DiskStorage : IDriveSizeStorage
    {
        private const string FilePath = "driveSizeStorage.json";

        public bool Exists()
        {
            return File.Exists(FilePath);
        }

        public IEnumerable<DirectorySizeData> Retrieve()
        {
            if (Exists())
            {
                var data = File.ReadAllText(FilePath);
                return JsonConvert.DeserializeObject<IEnumerable<DirectorySizeData>>(data);
            }
            return null;
        }

        public void Save(IEnumerable<DirectorySizeData> data)
        {
            var json = JsonConvert.SerializeObject(data);
            File.WriteAllText(FilePath, json);
        }
    }
}
